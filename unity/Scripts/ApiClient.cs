using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CampusNavigator
{
    public class ApiClient : MonoBehaviour
    {
        public static ApiClient Instance { get; private set; }

        private string token;

        public bool HasToken => !string.IsNullOrEmpty(token);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            token = PlayerPrefs.GetString("cn_token", "");
        }

        public void SetToken(string value)
        {
            token = value ?? "";
            PlayerPrefs.SetString("cn_token", token);
        }

        public void ClearToken()
        {
            token = "";
            PlayerPrefs.DeleteKey("cn_token");
        }

        public IEnumerator Login(string externalId, string displayName, Action<LoginResponse> onSuccess, Action<string> onError)
        {
            var req = new LoginRequest { externalId = externalId, displayName = displayName };
            yield return PostJson("/auth/login", req, (LoginResponse res) =>
            {
                if (!string.IsNullOrEmpty(res.token))
                {
                    SetToken(res.token);
                }
                onSuccess?.Invoke(res);
            }, onError, includeAuth: false);
        }

        public IEnumerator GetLocations(Action<LocationsResponse> onSuccess, Action<string> onError)
        {
            yield return GetJson("/locations", onSuccess, onError);
        }

        public IEnumerator GetQuests(Action<QuestListResponse> onSuccess, Action<string> onError)
        {
            yield return GetJson("/quests", onSuccess, onError);
        }

        public IEnumerator StartQuest(int questId, Action<QuestStartResponse> onSuccess, Action<string> onError)
        {
            var req = new QuestStartRequest { questId = questId };
            yield return PostJson("/quests/start", req, onSuccess, onError);
        }

        public IEnumerator QuestStep(int questRunId, int stepId, string actionPayload, Action onSuccess, Action<string> onError)
        {
            var req = new QuestStepRequest { questRunId = questRunId, stepId = stepId, actionPayload = actionPayload };
            yield return PostJson("/quests/step", req, _ => onSuccess?.Invoke(), onError);
        }

        public IEnumerator EndQuest(int questRunId, string status, int timeUsedSec, Action onSuccess, Action<string> onError)
        {
            var req = new QuestEndRequest { questRunId = questRunId, status = status, timeUsedSec = timeUsedSec };
            yield return PostJson("/quests/end", req, _ => onSuccess?.Invoke(), onError);
        }

        public IEnumerator SendTelemetry(TelemetryBatchRequest request, Action<TelemetryBatchResponse> onSuccess, Action<string> onError)
        {
            yield return PostJson("/telemetry/batch", request, onSuccess, onError);
        }

        private IEnumerator GetJson<T>(string path, Action<T> onSuccess, Action<string> onError)
        {
            var url = BuildUrl(path);
            using (var uwr = UnityWebRequest.Get(url))
            {
                ApplyHeaders(uwr, includeAuth: true);
                yield return uwr.SendWebRequest();
                HandleResponse(uwr, onSuccess, onError);
            }
        }

        private IEnumerator PostJson<TReq, TRes>(string path, TReq payload, Action<TRes> onSuccess, Action<string> onError, bool includeAuth = true)
        {
            var url = BuildUrl(path);
            var json = JsonUtility.ToJson(payload);
            var body = Encoding.UTF8.GetBytes(json);

            using (var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                uwr.uploadHandler = new UploadHandlerRaw(body);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");
                if (includeAuth)
                {
                    ApplyHeaders(uwr, includeAuth);
                }
                yield return uwr.SendWebRequest();
                HandleResponse(uwr, onSuccess, onError);
            }
        }

        private void ApplyHeaders(UnityWebRequest uwr, bool includeAuth)
        {
            if (includeAuth && !string.IsNullOrEmpty(token))
            {
                uwr.SetRequestHeader("Authorization", "Bearer " + token);
            }
        }

        private void HandleResponse<T>(UnityWebRequest uwr, Action<T> onSuccess, Action<string> onError)
        {
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(uwr.error);
                return;
            }

            var text = uwr.downloadHandler.text;
            if (string.IsNullOrEmpty(text))
            {
                onError?.Invoke("Empty response");
                return;
            }

            try
            {
                var data = JsonUtility.FromJson<T>(text);
                onSuccess?.Invoke(data);
            }
            catch (Exception ex)
            {
                onError?.Invoke("JSON parse error: " + ex.Message);
            }
        }

        private string BuildUrl(string path)
        {
            var baseUrl = ApiConfig.Instance != null ? ApiConfig.Instance.baseUrl : "";
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = "http://localhost/Campus-Navigator-3D/api";
            }
            return baseUrl.TrimEnd('/') + path;
        }
    }
}
