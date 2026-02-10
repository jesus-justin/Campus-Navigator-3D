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

            if (step.target_location_id != locationId)
            {
                QuestUI.Instance?.SetStatus("Wrong location: " + locationName);
                return;
            }

            string payload = "{\"locationId\":" + locationId + "}";
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
