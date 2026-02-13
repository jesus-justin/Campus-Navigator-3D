using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class QuestTrackerUI : MonoBehaviour
    {
        public Text titleText;
        public Text stepText;

        private void OnEnable()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.QuestUpdated += OnQuestUpdated;
            }
        }

        private void OnDisable()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.QuestUpdated -= OnQuestUpdated;
            }
        }

        private void OnQuestUpdated(QuestDto quest, QuestStepDto step)
        {
            if (titleText != null)
            {
                titleText.text = quest != null ? quest.title : "No active quest";
            }
            if (stepText != null)
            {
                if (step != null)
                {
                    string name = LocationRegistry.GetName(step.target_location_id);
                    stepText.text = step.action_type + " @ " + name;
                }
                else
                {
                    stepText.text = "";
                }
            }
        }
    }
}
