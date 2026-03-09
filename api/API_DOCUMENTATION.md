# API Documentation

Base URL: `http://localhost/Campus-Navigator-3D/api`

## Authentication

Most endpoints require authentication using Bearer tokens. Include the token in the Authorization header:

```
Authorization: Bearer YOUR_TOKEN_HERE
```

## Endpoints

### Health Check
**GET** `/health`

Check if the API is running.

**Response:**
```json
{
  "status": "ok",
  "timestamp": 1234567890
}
```

---

### Authentication

#### Login
**POST** `/auth/login`

Authenticate a user and receive a session token.

**Request Body:**
```json
{
  "external_id": "student-001",
  "display_name": "John Doe"
}
```

**Response:**
```json
{
  "token": "abc123...",
  "user_id": 1,
  "session_id": 42,
  "display_name": "John Doe"
}
```

---

### Locations

#### Get All Locations
**GET** `/locations`

Retrieve all campus locations.

**Headers:** Requires authentication

**Response:**
```json
{
  "locations": [
    {
      "id": 1,
      "name": "Main Library",
      "building_code": "LIB",
      "pos_x": 100.5,
      "pos_y": 0,
      "pos_z": 50.2,
      "type": "building"
    }
  ]
}
```

---

### Quests

#### Get Available Quests
**GET** `/quests`

Retrieve all available quests.

**Headers:** Requires authentication

**Response:**
```json
{
  "quests": [
    {
      "id": 1,
      "title": "Campus Orientation",
      "description": "Find key locations",
      "points": 100,
      "time_limit_seconds": 600
    }
  ]
}
```

#### Start Quest
**POST** `/quests/start`

Start a new quest run.

**Headers:** Requires authentication

**Request Body:**
```json
{
  "quest_id": 1
}
```

**Response:**
```json
{
  "quest_run_id": 42,
  "started_at": "2026-03-09T10:00:00Z"
}
```

#### Update Quest Step
**POST** `/quests/step`

Update progress on a quest step.

**Headers:** Requires authentication

**Request Body:**
```json
{
  "quest_run_id": 42,
  "step_number": 1,
  "completed": true
}
```

#### End Quest
**POST** `/quests/end`

Complete or abandon a quest.

**Headers:** Requires authentication

**Request Body:**
```json
{
  "quest_run_id": 42,
  "success": true,
  "final_score": 95
}
```

---

### Telemetry

#### Submit Batch Events
**POST** `/telemetry/batch`

Submit multiple analytics events at once.

**Headers:** Requires authentication

**Request Body:**
```json
{
  "events": [
    {
      "event_type": "player_move",
      "location_id": 5,
      "pos_x": 100.5,
      "pos_y": 0,
      "pos_z": 50.2,
      "timestamp": "2026-03-09T10:05:00Z",
      "metadata": {}
    }
  ]
}
```

**Response:**
```json
{
  "inserted": 1
}
```

---

### Admin Endpoints

All admin endpoints require authentication with an admin role.

#### Heatmap Data
**GET** `/admin/heatmap?from=YYYY-MM-DD&to=YYYY-MM-DD&type=visits`

Get location visit heatmap data.

**Query Parameters:**
- `from` (optional): Start date
- `to` (optional): End date
- `type` (optional): Heatmap type (visits, time_spent)

#### Path Analysis
**GET** `/admin/paths?limit=10`

Get common navigation paths.

**Query Parameters:**
- `limit` (optional): Number of paths to return (default: 10)

#### Quest Statistics
**GET** `/admin/quest-stats`

Get aggregated quest performance statistics.

#### Overview Metrics
**GET** `/admin/overview?from=YYYY-MM-DD&to=YYYY-MM-DD`

Get overview statistics.

**Query Parameters:**
- `from` (optional): Start date
- `to` (optional): End date

#### Quest Leaderboard
**GET** `/admin/quest-leaderboard?quest_id=1&limit=10`

Get top performers for quests.

**Query Parameters:**
- `quest_id` (optional): Filter by specific quest
- `limit` (optional): Number of results (default: 10)

---

## Error Responses

All endpoints may return error responses in this format:

```json
{
  "error": "Error message description"
}
```

Common HTTP status codes:
- `200`: Success
- `400`: Bad Request (invalid input)
- `401`: Unauthorized (missing or invalid token)
- `403`: Forbidden (insufficient permissions)
- `404`: Not Found
- `500`: Server Error
