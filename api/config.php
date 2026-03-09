<?php
// Basic configuration. Update for your environment.

define('DB_HOST', '127.0.0.1');
define('DB_NAME', 'campus_navigator');
define('DB_USER', 'root');
define('DB_PASS', '');

define('TOKEN_TTL_DAYS', 7);

define('JSON_CONTENT_TYPE', 'application/json; charset=utf-8');

// Security headers
function set_security_headers(): void {
    header('X-Content-Type-Options: nosniff');
    header('X-Frame-Options: DENY');
    header('X-XSS-Protection: 1; mode=block');
    header('Referrer-Policy: strict-origin-when-cross-origin');
    header('Content-Security-Policy: default-src \'self\'');
}

function db(): PDO {
    static $pdo = null;
    if ($pdo === null) {
        $dsn = 'mysql:host=' . DB_HOST . ';dbname=' . DB_NAME . ';charset=utf8mb4';
        $pdo = new PDO($dsn, DB_USER, DB_PASS, [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC
        ]);
    }
    return $pdo;
}

function json_response($data, int $status = 200): void {
    http_response_code($status);
    set_security_headers();
    header('Content-Type: ' . JSON_CONTENT_TYPE);
    echo json_encode($data);
}

function read_json(): array {
    $raw = file_get_contents('php://input');
    if ($raw === false || $raw === '') {
        return [];
    }
    $data = json_decode($raw, true);
    return is_array($data) ? $data : [];
}

function get_bearer_token(): ?string {
    $header = $_SERVER['HTTP_AUTHORIZATION'] ?? '';
    if (stripos($header, 'Bearer ') === 0) {
        return trim(substr($header, 7));
    }
    return null;
}

function require_auth(): array {
    $token = get_bearer_token();
    if (!$token) {
        json_response(['error' => 'Unauthorized'], 401);
        exit;
    }

    $token_hash = hash('sha256', $token);
    $stmt = db()->prepare('SELECT s.id AS session_id, s.user_id, u.role
        FROM sessions s
        JOIN users u ON u.id = s.user_id
        WHERE s.token_hash = ? AND s.expires_at > NOW()');
    $stmt->execute([$token_hash]);
    $row = $stmt->fetch();
    if (!$row) {
        json_response(['error' => 'Unauthorized'], 401);
        exit;
    }
    return $row;
}

function require_admin(): array {
    $auth = require_auth();
    if ($auth['role'] !== 'admin') {
        json_response(['error' => 'Forbidden'], 403);
        exit;
    }
    return $auth;
}

function sanitize_datetime_or_default(string $value, string $default): string {
    $value = trim($value);
    if ($value === '') {
        return $default;
    }

    $dt = date_create($value);
    if ($dt === false) {
        return $default;
    }

    return $dt->format('Y-m-d H:i:s');
}

function sanitize_int_range($value, int $default, int $min, int $max): int {
    if (!is_numeric($value)) {
        return $default;
    }

    $int = (int)$value;
    if ($int < $min) {
        return $min;
    }
    if ($int > $max) {
        return $max;
    }

    return $int;
}
