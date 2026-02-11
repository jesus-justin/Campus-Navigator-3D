using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CampusNavigator
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        public string autoLoginExternalId = "student-001";
        public string autoLoginName = "Student";

        private QuestDto[] quests = new QuestDto[0];
        private QuestStepDto[] steps = new QuestStepDto[0];
        private QuestDto activeQuest;
        private List<QuestStepDto> activeSteps = new List<QuestStepDto>();
        private int activeStepIndex;
        private int questRunId;

        public int ActiveTargetLocationId { get; private set; }
        public bool HasActiveTarget => ActiveTargetLocationId > 0;
        public bool HasActiveQuest => activeQuest != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartCoroutine(ApiClient.Instance.Login(autoLoginExternalId, autoLoginName, _ =>
            {
                StartCoroutine(ApiClient.Instance.GetQuests(OnQuestsLoaded, OnError));
            }, OnError));
        }

        private void OnQuestsLoaded(QuestListResponse res)
        {
            quests = res.quests ?? new QuestDto[0];
            steps = res.steps ?? new QuestStepDto[0];
            QuestUI.Instance?.SetQuestList(quests);
        }

        public void StartQuestById(int questId)
        {
            var quest = quests.FirstOrDefault(q => q.id == questId);
            if (quest == null)
            {
                QuestUI.Instance?.SetStatus("Quest not found");
                return;
            }

            StartCoroutine(ApiClient.Instance.StartQuest(questId, res =>
            {
                activeQuest = quest;
                questRunId = res.questRunId;
                activeSteps = steps.Where(s => s.quest_id == questId).OrderBy(s => s.step_order).ToList();
                activeStepIndex = 0;
                ActiveTargetLocationId = GetActiveStep() != null ? GetActiveStep().target_location_id : 0;
                QuestUI.Instance?.SetActiveQuest(activeQuest, GetActiveStep());
            }, OnError));
        }

        public void OnReachLocation(int locationId, string locationName, Vector3 pos)
        {
            var step = GetActiveStep();
            if (step == null)
            {
                return;
            }

            if (step.action_type != "reach")
            {
                QuestUI.Instance?.SetStatus("Step requires action: " + step.action_type);
                return;
            }

            if (step.target_location_id != locationId)
            {
                QuestUI.Instance?.SetStatus("Wrong location: " + locationName);
                return;
            }

            CompleteStep(step, "{\"locationId\":" + locationId + "}");
        }

        public void TrySubmitAtLocation(int locationId)
        {
            var step = GetActiveStep();
            if (step == null || step.action_type != "submit")
            {
                QuestUI.Instance?.SetStatus("No submit step active");
                return;
            }

            if (step.target_location_id != locationId)
            {
                QuestUI.Instance?.SetStatus("Submit at correct location");
                return;
            }

            string required = GetRequiredItem(step);
            if (!string.IsNullOrEmpty(required) && !InventoryManager.Instance.HasItem(required))
            {
                QuestUI.Instance?.SetStatus("Need item: " + required);
                return;
            }

            if (!string.IsNullOrEmpty(required))
            {
                InventoryManager.Instance.RemoveItem(required);
            }

            CompleteStep(step, "{\"locationId\":" + locationId + ",\"item\":\"" + required + "\"}");
        }

        public void TryInteractAtLocation(int locationId)
        {
            var step = GetActiveStep();
            if (step == null || step.action_type != "interact")
            {
                QuestUI.Instance?.SetStatus("No interact step active");
                return;
            }

            if (step.target_location_id != locationId)
            {
                QuestUI.Instance?.SetStatus("Interact at correct location");
                return;
            }

            CompleteStep(step, "{\"locationId\":" + locationId + "}");
        }

        public void ExplainActiveStep()
        {
            var step = GetActiveStep();
            if (step == null)
            {
                QuestUI.Instance?.SetStatus("No active quest");
                return;
            }

            string targetName = LocationRegistry.GetName(step.target_location_id);
            QuestUI.Instance?.SetStatus("Head to: " + targetName);
        }

        private void FinishQuest(string status)
        {
            StartCoroutine(ApiClient.Instance.EndQuest(questRunId, status, 0, () =>
            {
                QuestUI.Instance?.SetStatus("Quest completed");
                activeQuest = null;
                activeSteps.Clear();
                ActiveTargetLocationId = 0;
            }, OnError));
        }

        private void CompleteStep(QuestStepDto step, string payload)
        {
            StartCoroutine(ApiClient.Instance.QuestStep(questRunId, step.id, payload, () =>
            {
                activeStepIndex++;
                var next = GetActiveStep();
                if (next == null)
                {
                    ActiveTargetLocationId = 0;
                    FinishQuest("success");
                }
                else
                {
                    ActiveTargetLocationId = next.target_location_id;
                    QuestUI.Instance?.SetActiveQuest(activeQuest, next);
                }
            }, OnError));
        }

        private string GetRequiredItem(QuestStepDto step)
        {
            if (step == null || string.IsNullOrEmpty(step.action_payload))
            {
                return "";
            }
            const string token = "\"item\"";
            int idx = step.action_payload.IndexOf(token, System.StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
            {
                return "";
            }
            int colon = step.action_payload.IndexOf(':', idx);
            if (colon < 0)
            {
                return "";
            }
            int quote1 = step.action_payload.IndexOf('"', colon + 1);
            int quote2 = step.action_payload.IndexOf('"', quote1 + 1);
            if (quote1 < 0 || quote2 < 0)
            {
                return "";
            }
            return step.action_payload.Substring(quote1 + 1, quote2 - quote1 - 1);
        }

        private QuestStepDto GetActiveStep()
        {
            if (activeStepIndex < 0 || activeStepIndex >= activeSteps.Count)
            {
                return null;
            }
            return activeSteps[activeStepIndex];
        }

        private void OnError(string message)
        {
            QuestUI.Instance?.SetStatus("Error: " + message);
        }
    }
}
