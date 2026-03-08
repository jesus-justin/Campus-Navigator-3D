<?php
require_once __DIR__ . '/config.php';

$method = $_SERVER['REQUEST_METHOD'] ?? 'GET';
$path = $_SERVER['PATH_INFO'] ?? '';
if ($path === '' && isset($_GET['path'])) {
    $path = '/' . ltrim($_GET['path'], '/');
}

$path = rtrim($path, '/');
if ($path === '') {
    $path = '/';
}

try {
    route_request($method, $path);
} catch (Throwable $e) {
    json_response(['error' => 'Server error'], 500);
}

function route_request(string $method, string $path): void {
    if ($method === 'GET' && $path === '/health') {
        handle_health();
        return;
    }

    if ($method === 'POST' && $path === '/auth/login') {
        handle_login();
        return;
    }

    if ($method === 'GET' && $path === '/locations') {
        require_auth();
        handle_locations();
        return;
    }

    if ($method === 'GET' && $path === '/quests') {
        require_auth();
        handle_quests();
        return;
    }

    if ($method === 'POST' && $path === '/quests/start') {
        $auth = require_auth();
        handle_quest_start((int)$auth['user_id'], (int)$auth['session_id']);
        return;
    }

    if ($method === 'POST' && $path === '/quests/step') {
        $auth = require_auth();
        handle_quest_step((int)$auth['user_id'], (int)$auth['session_id']);
        return;
    }

    if ($method === 'POST' && $path === '/quests/end') {
        $auth = require_auth();
        handle_quest_end((int)$auth['user_id'], (int)$auth['session_id']);
        return;
    }

    if ($method === 'POST' && $path === '/telemetry/batch') {
        $auth = require_auth();
        handle_telemetry_batch((int)$auth['user_id'], (int)$auth['session_id']);
        return;
    }

    if ($method === 'GET' && $path === '/admin/heatmap') {
        require_admin();
        handle_admin_heatmap();
        return;
    }

    if ($method === 'GET' && $path === '/admin/paths') {
        require_admin();
        handle_admin_paths();
        return;
    }

    if ($method === 'GET' && $path === '/admin/quest-stats') {
        require_admin();
        handle_admin_quest_stats();
        return;
    }

    if ($method === 'GET' && $path === '/admin/overview') {
        require_admin();
        handle_admin_overview();
        return;
    }

    json_response(['error' => 'Not found'], 404);
}

function handle_health(): void {
    $db_ok = false;

    try {
        db()->query('SELECT 1');
        $db_ok = true;
    } catch (Throwable $e) {
        $db_ok = false;
    }

    json_response([
        'status' => $db_ok ? 'ok' : 'degraded',
        'db' => $db_ok ? 'ok' : 'error',
        'time' => date('c')
    ], $db_ok ? 200 : 503);
}

function handle_login(): void {
    $data = read_json();
    $external_id = trim((string)($data['externalId'] ?? ''));
    $display_name = trim((string)($data['displayName'] ?? ''));

    if ($external_id === '' && $display_name === '') {
        json_response(['error' => 'externalId or displayName is required'], 400);
        return;
    }

    $pdo = db();
    $pdo->beginTransaction();

    if ($external_id !== '') {
        $stmt = $pdo->prepare('SELECT id, display_name, role FROM users WHERE external_id = ?');
        $stmt->execute([$external_id]);
        $user = $stmt->fetch();
    } else {
        $user = null;
    }

    if ($user) {
        if ($display_name !== '' && $display_name !== $user['display_name']) {
            $upd = $pdo->prepare('UPDATE users SET display_name = ? WHERE id = ?');
            $upd->execute([$display_name, $user['id']]);
        }
        $user_id = (int)$user['id'];
        $role = (string)$user['role'];
    } else {
        if ($display_name === '') {
            $display_name = 'Guest';
        }
        $ins = $pdo->prepare('INSERT INTO users (external_id, display_name, role) VALUES (?, ?, ?)' );
        $ins->execute([$external_id !== '' ? $external_id : null, $display_name, 'player']);
        $user_id = (int)$pdo->lastInsertId();
        $role = 'player';
    }

    $token = bin2hex(random_bytes(32));
    $token_hash = hash('sha256', $token);
    $device_label = substr((string)($_SERVER['HTTP_USER_AGENT'] ?? 'Unity'), 0, 100);
    $expires_at = (new DateTime('now'))->modify('+' . TOKEN_TTL_DAYS . ' days')->format('Y-m-d H:i:s');

    $stmt = $pdo->prepare('INSERT INTO sessions (user_id, token_hash, device_label, expires_at) VALUES (?, ?, ?, ?)');
    $stmt->execute([$user_id, $token_hash, $device_label, $expires_at]);

    $pdo->commit();

    json_response([
        'token' => $token,
        'user' => [
            'id' => $user_id,
            'displayName' => $display_name,
            'role' => $role
        ]
    ]);
}

function handle_locations(): void {
    $stmt = db()->query('SELECT id, name, type, parent_id, floor, pos_x, pos_y, pos_z, radius_m FROM locations');
    json_response(['locations' => $stmt->fetchAll()]);
}

function handle_quests(): void {
    $stmt = db()->query('SELECT id, title, description, type, reward_points, time_limit_sec FROM quests WHERE is_active = 1');
    $quests = $stmt->fetchAll();

    $steps_stmt = db()->query('SELECT id, quest_id, step_order, target_location_id, action_type, action_payload FROM quest_steps ORDER BY quest_id, step_order');
    $steps = $steps_stmt->fetchAll();

    json_response(['quests' => $quests, 'steps' => $steps]);
}

function handle_quest_start(int $user_id, int $session_id): void {
    $data = read_json();
    $quest_id = (int)($data['questId'] ?? 0);
    if ($quest_id <= 0) {
        json_response(['error' => 'questId is required'], 400);
        return;
    }

    $stmt = db()->prepare('SELECT id FROM quests WHERE id = ? AND is_active = 1');
    $stmt->execute([$quest_id]);
    if (!$stmt->fetch()) {
        json_response(['error' => 'Quest not found'], 404);
        return;
    }

    $ins = db()->prepare('INSERT INTO quest_runs (quest_id, user_id) VALUES (?, ?)');
    $ins->execute([$quest_id, $user_id]);
    $quest_run_id = (int)db()->lastInsertId();

    insert_event($user_id, $session_id, 'quest_start', null, null, null, null, ['questId' => $quest_id]);

    json_response(['questRunId' => $quest_run_id]);
}

function handle_quest_step(int $user_id, int $session_id): void {
    $data = read_json();
    $quest_run_id = (int)($data['questRunId'] ?? 0);
    $step_id = (int)($data['stepId'] ?? 0);
    $payload = $data['actionPayload'] ?? null;

    if ($quest_run_id <= 0 || $step_id <= 0) {
        json_response(['error' => 'questRunId and stepId are required'], 400);
        return;
    }

    $stmt = db()->prepare('SELECT id FROM quest_runs WHERE id = ? AND user_id = ?');
    $stmt->execute([$quest_run_id, $user_id]);
    if (!$stmt->fetch()) {
        json_response(['error' => 'Quest run not found'], 404);
        return;
    }

    insert_event($user_id, $session_id, 'quest_step', null, null, null, null, [
        'questRunId' => $quest_run_id,
        'stepId' => $step_id,
        'actionPayload' => $payload
    ]);

    json_response(['ok' => true]);
}

function handle_quest_end(int $user_id, int $session_id): void {
    $data = read_json();
    $quest_run_id = (int)($data['questRunId'] ?? 0);
    $status = (string)($data['status'] ?? 'success');
    $time_used = (int)($data['timeUsedSec'] ?? 0);

    if ($quest_run_id <= 0) {
        json_response(['error' => 'questRunId is required'], 400);
        return;
    }

    $allowed = ['success', 'failed'];
    if (!in_array($status, $allowed, true)) {
        json_response(['error' => 'Invalid status'], 400);
        return;
    }

    $stmt = db()->prepare('UPDATE quest_runs SET status = ?, ended_at = NOW(), time_used_sec = ? WHERE id = ? AND user_id = ?');
    $stmt->execute([$status, $time_used, $quest_run_id, $user_id]);

    insert_event($user_id, $session_id, 'quest_end', null, null, null, null, [
        'questRunId' => $quest_run_id,
        'status' => $status,
        'timeUsedSec' => $time_used
    ]);

    json_response(['ok' => true]);
}

function handle_telemetry_batch(int $user_id, int $session_id): void {
    $events = read_json();
    if (isset($events['events']) && is_array($events['events'])) {
        $events = $events['events'];
    }
    if (!is_array($events)) {
        json_response(['error' => 'Invalid payload'], 400);
        return;
    }

    $stmt = db()->prepare('INSERT INTO telemetry_events
        (user_id, session_id, event_type, location_id, pos_x, pos_y, pos_z, payload, created_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)');

    $count = 0;
    foreach ($events as $event) {
        $event_type = (string)($event['eventType'] ?? 'move');
        $location_id = isset($event['locationId']) ? (int)$event['locationId'] : null;
        $pos_x = isset($event['posX']) ? (float)$event['posX'] : null;
        $pos_y = isset($event['posY']) ? (float)$event['posY'] : null;
        $pos_z = isset($event['posZ']) ? (float)$event['posZ'] : null;
        $payload = $event['payload'] ?? null;
        $created_at = (string)($event['createdAt'] ?? '');
        $created_at = $created_at !== '' ? $created_at : date('Y-m-d H:i:s');

        $stmt->execute([
            $user_id,
            $session_id,
            $event_type,
            $location_id,
            $pos_x,
            $pos_y,
            $pos_z,
            $payload ? json_encode($payload) : null,
            $created_at
        ]);
        $count++;
    }

    json_response(['inserted' => $count]);
}

function handle_admin_heatmap(): void {
    $from = trim((string)($_GET['from'] ?? ''));
    $to = trim((string)($_GET['to'] ?? ''));
    $type = trim((string)($_GET['type'] ?? 'building'));

    $from = $from !== '' ? $from : date('Y-m-d H:i:s', strtotime('-7 days'));
    $to = $to !== '' ? $to : date('Y-m-d H:i:s');

    $stmt = db()->prepare('SELECT l.id AS location_id, l.name, COUNT(t.id) AS count
        FROM telemetry_events t
        JOIN locations l ON l.id = t.location_id
        WHERE t.created_at BETWEEN ? AND ?
          AND l.type = ?
        GROUP BY l.id, l.name
        ORDER BY count DESC');
    $stmt->execute([$from, $to, $type]);

    json_response(['from' => $from, 'to' => $to, 'type' => $type, 'data' => $stmt->fetchAll()]);
}

function handle_admin_paths(): void {
    $from = trim((string)($_GET['from'] ?? ''));
    $to = trim((string)($_GET['to'] ?? ''));
    $top = (int)($_GET['top'] ?? 20);
    $top = $top > 0 ? $top : 20;

    $from = $from !== '' ? $from : date('Y-m-d H:i:s', strtotime('-7 days'));
    $to = $to !== '' ? $to : date('Y-m-d H:i:s');

    $sql = "
        SELECT prev_location_id AS from_location_id, location_id AS to_location_id, COUNT(*) AS count
        FROM (
            SELECT
                session_id,
                location_id,
                LAG(location_id) OVER (PARTITION BY session_id ORDER BY created_at) AS prev_location_id
            FROM telemetry_events
            WHERE event_type = 'enter'
              AND created_at BETWEEN ? AND ?
              AND location_id IS NOT NULL
        ) t
        WHERE prev_location_id IS NOT NULL
        GROUP BY prev_location_id, location_id
        ORDER BY count DESC
        LIMIT ?
    ";

    $stmt = db()->prepare($sql);
    $stmt->execute([$from, $to, $top]);
    $rows = $stmt->fetchAll();

    json_response(['from' => $from, 'to' => $to, 'data' => $rows]);
}

function handle_admin_quest_stats(): void {
    $from = trim((string)($_GET['from'] ?? ''));
    $to = trim((string)($_GET['to'] ?? ''));

    $from = $from !== '' ? $from : date('Y-m-d H:i:s', strtotime('-30 days'));
    $to = $to !== '' ? $to : date('Y-m-d H:i:s');

    $stmt = db()->prepare('SELECT q.id AS quest_id, q.title,
        COUNT(r.id) AS total_runs,
        SUM(r.status = \'success\') AS success_runs,
        AVG(r.time_used_sec) AS avg_time_sec
        FROM quests q
        LEFT JOIN quest_runs r ON r.quest_id = q.id AND r.started_at BETWEEN ? AND ?
        GROUP BY q.id, q.title
        ORDER BY total_runs DESC');
    $stmt->execute([$from, $to]);

    json_response(['from' => $from, 'to' => $to, 'data' => $stmt->fetchAll()]);
}

function handle_admin_overview(): void {
    $from = sanitize_datetime_or_default((string)($_GET['from'] ?? ''), date('Y-m-d H:i:s', strtotime('-7 days')));
    $to = sanitize_datetime_or_default((string)($_GET['to'] ?? ''), date('Y-m-d H:i:s'));

    $events_stmt = db()->prepare('SELECT COUNT(*) AS total FROM telemetry_events WHERE created_at BETWEEN ? AND ?');
    $events_stmt->execute([$from, $to]);
    $events_total = (int)($events_stmt->fetch()['total'] ?? 0);

    $sessions_stmt = db()->prepare('SELECT COUNT(*) AS total FROM sessions WHERE created_at BETWEEN ? AND ?');
    $sessions_stmt->execute([$from, $to]);
    $sessions_total = (int)($sessions_stmt->fetch()['total'] ?? 0);

    $users_stmt = db()->prepare('SELECT COUNT(*) AS total FROM users WHERE created_at BETWEEN ? AND ?');
    $users_stmt->execute([$from, $to]);
    $users_total = (int)($users_stmt->fetch()['total'] ?? 0);

    $runs_stmt = db()->prepare('SELECT COUNT(*) AS total FROM quest_runs WHERE started_at BETWEEN ? AND ?');
    $runs_stmt->execute([$from, $to]);
    $runs_total = (int)($runs_stmt->fetch()['total'] ?? 0);

    json_response([
        'from' => $from,
        'to' => $to,
        'data' => [
            'eventsTotal' => $events_total,
            'sessionsTotal' => $sessions_total,
            'newUsersTotal' => $users_total,
            'questRunsTotal' => $runs_total
        ]
    ]);
}

function insert_event(int $user_id, int $session_id, string $event_type, ?int $location_id, ?float $x, ?float $y, ?float $z, ?array $payload): void {
    $stmt = db()->prepare('INSERT INTO telemetry_events
        (user_id, session_id, event_type, location_id, pos_x, pos_y, pos_z, payload, created_at)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, NOW())');

    $stmt->execute([
        $user_id,
        $session_id,
        $event_type,
        $location_id,
        $x,
        $y,
        $z,
        $payload ? json_encode($payload) : null
    ]);
}
