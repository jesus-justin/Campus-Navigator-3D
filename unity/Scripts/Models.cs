using System;

namespace CampusNavigator
{
    [Serializable]
    public class UserDto
    {
        public int id;
        public string displayName;
        public string role;
    }

    [Serializable]
    public class LoginRequest
    {
        public string externalId;
        public string displayName;
    }

    [Serializable]
    public class LoginResponse
    {
        public string token;
        public UserDto user;
    }

    [Serializable]
    public class LocationDto
    {
        public int id;
        public string name;
        public string type;
        public int parent_id;
        public int floor;
        public float pos_x;
        public float pos_y;
        public float pos_z;
        public float radius_m;
    }

    [Serializable]
    public class LocationsResponse
    {
        public LocationDto[] locations;
    }

    [Serializable]
    public class QuestDto
    {
        public int id;
        public string title;
        public string description;
        public string type;
        public int reward_points;
        public int time_limit_sec;
    }

    [Serializable]
    public class QuestStepDto
    {
        public int id;
        public int quest_id;
        public int step_order;
        public int target_location_id;
        public string action_type;
        public string action_payload;
    }

    [Serializable]
    public class QuestListResponse
    {
        public QuestDto[] quests;
        public QuestStepDto[] steps;
    }

    [Serializable]
    public class QuestStartRequest
    {
        public int questId;
    }

    [Serializable]
    public class QuestStartResponse
    {
        public int questRunId;
    }

    [Serializable]
    public class QuestStepRequest
    {
        public int questRunId;
        public int stepId;
        public string actionPayload;
    }

    [Serializable]
    public class QuestEndRequest
    {
        public int questRunId;
        public string status;
        public int timeUsedSec;
    }

    [Serializable]
    public class TelemetryEvent
    {
        public string eventType;
        public int locationId;
        public float posX;
        public float posY;
        public float posZ;
        public object payload;
        public string createdAt;
    }

    [Serializable]
    public class TelemetryBatchRequest
    {
        public TelemetryEvent[] events;
    }

    [Serializable]
    public class TelemetryBatchResponse
    {
        public int inserted;
    }
}
