# Campus-Navigator-3D
- A 3D map of a university where users play by navigating tasks.

## Quick Start

### Prerequisites
- XAMPP (PHP 7.4+ with MySQL)
- Unity 2021.3 LTS or later
- Modern web browser (Chrome, Firefox, Edge)
- Git (for version control)

### Installation

1. **Clone the repository** to your XAMPP htdocs folder:
	```bash
	cd C:\xampp\htdocs
	git clone <repository-url> Campus-Navigator-3D
	```

2. **Set up the database**:
	- Start XAMPP and ensure MySQL is running
	- Open phpMyAdmin (http://localhost/phpmyadmin)
	- Create a new database named `campus_navigator`
	- Import the schema:
	  ```bash
	  mysql -u root < db/schema.sql
	  ```
	- Import sample data:
	  ```bash
	  mysql -u root < db/seed.sql
	  ```

3. **Configure the API**:
	- Copy `api/.env.example` to `api/config.php` (or update existing config.php)
	- Update database credentials if needed:
	  ```php
	  define('DB_HOST', '127.0.0.1');
	  define('DB_NAME', 'campus_navigator');
	  define('DB_USER', 'root');
	  define('DB_PASS', '');
	  ```

4. **Test the API**:
	- Open http://localhost/Campus-Navigator-3D/api/index.php?path=/health
	- You should see: `{"status":"ok","db":"ok","time":"..."}`

5. **Open the Dashboard**:
	- Navigate to http://localhost/Campus-Navigator-3D/dashboard/
	- Use default credentials:
	  - External ID: `admin-001`
	  - Display Name: `Admin User`
	- Click "Request Token" to authenticate

6. **Open Unity Project**:
	- Launch Unity Hub
	- Add the project from `Campus-Navigator-3D/unity/`
	- Open with Unity 2021.3 LTS or later
	- Update API settings in `ApiConfig.cs` if needed

### Troubleshooting

**Database connection fails:**
- Verify MySQL is running in XAMPP
- Check database credentials in `api/config.php`
- Ensure `campus_navigator` database exists

**API returns 404:**
- Check that XAMPP Apache is running
- Verify the project is in the htdocs folder
- Try accessing via PATH_INFO or query parameter

**Dashboard won't authenticate:**
- Verify the API health endpoint works first
- Check browser console for errors
- Ensure the admin user exists in the database

## Concept Summary
- 3D campus navigation with NPC students, quests, and time-based challenges.
- Gamified tasks: find rooms, submit forms, escort NPCs, event check-ins.
- Analytics layer: heatmaps of visits, navigation efficiency, and behavior patterns.

## Core Gameplay Loop
1) Spawn at a hub (gate or main building).
2) Receive quest(s) from NPCs or kiosk UI.
3) Navigate to targets using 3D map, signage, and mini-map.
4) Complete task (scan, interact, submit, or reach within time).
5) Earn points, badges, or unlocks; log analytics events.

## Personas and Use Cases
- New students: orientation, locate offices and classrooms.
- Visitors: campus tour and key landmarks.
- Admin staff: crowd flow visibility and signage planning.

## System Architecture (High Level)
- Unity client (C#): 3D world, gameplay, UI, local telemetry buffering.
- PHP API (XAMPP): REST endpoints for auth, quests, telemetry.
- MySQL: storage for users, quests, locations, analytics events.
- JS dashboard: admin analytics, heatmaps, funnel and path analysis.

Client and server flow:
- Unity sends auth -> receives token.
- Unity loads quests + location metadata.
- Unity logs events (move, enter building, quest start/finish).
- Dashboard queries aggregated metrics and renders heatmaps.

## Data Model (MySQL)
Key tables and purpose:
- users: player identity and roles.
- locations: buildings, rooms, landmarks (with coordinates).
- quests: quest definitions, steps, rewards.
- quest_runs: per-player quest session state.
- telemetry_events: raw analytics events.
- sessions: login sessions and device metadata.

Suggested schema (minimal):

```
users
- id (PK)
- external_id (unique, optional)
- display_name
- role (player, admin)
- created_at

sessions
- id (PK)
- user_id (FK)
- token_hash
- device_label
- created_at
- expires_at

locations
- id (PK)
- name
- type (building, room, landmark)
- parent_id (FK to locations)
- floor
- pos_x, pos_y, pos_z
- radius_m

quests
- id (PK)
- title
- description
- type (find, deliver, submit, escort, timed)
- reward_points
- time_limit_sec
- is_active

quest_steps
- id (PK)
- quest_id (FK)
- step_order
- target_location_id (FK)
- action_type (reach, interact, submit)
- action_payload (JSON)

quest_runs
- id (PK)
- quest_id (FK)
- user_id (FK)
- status (active, success, failed)
- started_at
- ended_at
- time_used_sec

telemetry_events
- id (PK)
- user_id (FK)
- session_id (FK)
- event_type (move, enter, quest_start, quest_end, interact)
- location_id (FK, nullable)
- pos_x, pos_y, pos_z
- payload (JSON)
- created_at
```

Indexes to add:
- telemetry_events (event_type, created_at)
- telemetry_events (location_id, created_at)
- quest_runs (user_id, started_at)
- locations (type, parent_id)

## API Design (PHP REST)
Auth:
- POST /api/auth/login
	- body: { externalId, displayName }
	- response: { token, user }

Quest and location:
- GET /api/locations
- GET /api/quests
- POST /api/quests/start { questId }
- POST /api/quests/step { questRunId, stepId, actionPayload }
- POST /api/quests/end { questRunId, status, timeUsedSec }

Telemetry:
- POST /api/telemetry/batch
	- body: [{ eventType, locationId, posX, posY, posZ, payload, createdAt }]

Admin analytics:
- GET /api/admin/heatmap?from=&to=&type=building
- GET /api/admin/paths?from=&to=&top=20
- GET /api/admin/quest-stats?from=&to=

Security notes:
- Use bearer tokens with short TTL, refresh on login.
- Store token hashes only, never raw tokens.
- Rate limit telemetry endpoint.

## Unity Client Design
Scenes:
- Boot: auth and config fetch.
- Campus: main world, NPCs, quests, mini-map, HUD.
- Analytics test: developer-only visualizations.

Systems:
- Navigation: waypoint system, path hints, minimap.
- Quest system: state machine for steps and timers.
- NPC system: dialogue triggers and quest assignment.
- Telemetry: buffered event queue with batch upload.

Unity telemetry event batching (suggested):
- Buffer events in memory (max 200).
- Flush every 10 seconds or on critical events.
- Retry with backoff on failure.

## Analytics Dashboard (JS)
Pages:
- Overview: DAU, sessions, quest completion rate.
- Heatmap: building or room intensity over time.
- Paths: top navigation routes between locations.
- Quest performance: completion time distribution.

Heatmap data approach:
- Aggregate telemetry events by location_id and time bucket.
- Return list of { locationId, count } for a time range.

Navigation efficiency:
- For each quest, compare shortest path vs actual path length.
- Store efficiency score per quest run.

## Campus 3D Asset Pipeline
- If using a Sketchfab model, verify license and attribution terms.
- Import to Blender, clean topology, separate buildings by mesh.
- Apply consistent scale and axes (Unity: 1 unit = 1 meter).
- Export FBX per building or per block.
- Add simple colliders and navigation meshes in Unity.

BatStateU Lipa integration (implemented in this repo):
- Runtime map profile + model loader: unity/Scripts/CampusMapProfile.cs and unity/Scripts/CampusMapRuntime.cs
- Anchor-to-location mapping: unity/Scripts/CampusLocationAnchor.cs
- Coverage validation: unity/Scripts/CampusMapCoverageMonitor.cs
- Integration guide: unity/README-Batangas-Lipa-Map.md
- Attribution manifest: unity/THIRD_PARTY_ASSETS.md

Model reference used:
- https://sketchfab.com/3d-models/batangas-state-university-the-neu-lipa-map-abff63aeea7c42a1a7916b1a2a25c24a
- License reported on source page: CC BY 4.0

## Privacy and Data Guidance (Recommended)
- Pseudonymize users (external_id) and avoid real names.
- Avoid collecting exact real-world identifiers in telemetry.
- Allow opt-out for analytics on public kiosks.
- Retain raw telemetry for limited time (e.g., 90 days).

## Implementation Plan (Phased)
Phase 1: Prototype
- Base campus map, 3-5 quests, basic telemetry.

Phase 2: Analytics
- Heatmap and paths dashboard, quest stats.

Phase 3: Polish
- NPC dialogue, optimized navigation, rewards.

## Open Decisions
- Authentication model (anonymous vs campus accounts).
- Data retention and reporting requirements.
- Target platforms (PC, mobile, web build).
