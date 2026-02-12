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
        public GameObject questListPanel;
        public Text questListText;
        public InputConfig inputConfig;

        public bool IsQuestListOpen => questListPanel != null && questListPanel.activeSelf;

        private QuestDto[] cachedQuests = new QuestDto[0];

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
            cachedQuests = quests ?? new QuestDto[0];
            RefreshQuestList();
            if (cachedQuests.Length > 0)
            {
                SetStatus("Quests loaded: " + cachedQuests.Length);
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
                if (step != null)
                {
                    string name = LocationRegistry.GetName(step.target_location_id);
                    questStep.text = "Step: " + step.action_type + " @ " + name;
                }
                else
                {
                    questStep.text = "";
                }
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

        private void Update()
        {
            if (Input.GetKeyDown(GetQuestListKey()))
            {
                ToggleQuestList();
            }
        }

        private void ToggleQuestList()
        {
            if (questListPanel == null)
            {
                return;
            }
            questListPanel.SetActive(!questListPanel.activeSelf);
            RefreshQuestList();
        }

        private void RefreshQuestList()
        {
            if (questListText == null)
            {
                return;
            }
            if (cachedQuests == null || cachedQuests.Length == 0)
            {
                questListText.text = "No quests";
                return;
            }
            string list = "";
            for (int i = 0; i < cachedQuests.Length; i++)
            {
                list += (i + 1) + ". " + cachedQuests[i].title + "\n";
            }
            questListText.text = list.TrimEnd();
        }

        private KeyCode GetQuestListKey()
        {
            return inputConfig != null ? inputConfig.questListKey : KeyCode.Q;
        }
    }
}
