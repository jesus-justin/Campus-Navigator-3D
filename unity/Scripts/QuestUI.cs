using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class QuestUI : MonoBehaviour
    {
        public static QuestUI Instance { get; private set; }

        public Text questTitle;
        public Text questStep;
        public Text statusText;
        public Text promptText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetQuestList(QuestDto[] quests)
        {
            if (quests != null && quests.Length > 0)
            {
                SetStatus("Quests loaded: " + quests.Length);
            }
        }

        public void SetActiveQuest(QuestDto quest, QuestStepDto step)
        {
            if (questTitle != null)
            {
                questTitle.text = quest != null ? quest.title : "No quest";
            }
            if (questStep != null)
            {
                questStep.text = step != null ? "Step: " + step.action_type + " @ " + step.target_location_id : "";
            }
            SetStatus("Quest active");
        }

        public void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        public void ShowPrompt(string message)
        {
            if (promptText != null)
            {
                promptText.text = message;
            }
        }

        public void ClearPrompt()
        {
            if (promptText != null)
            {
                promptText.text = "";
            }
        }
    }
}
