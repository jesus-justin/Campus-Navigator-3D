using UnityEngine;

namespace CampusNavigator
{
    public class NpcQuestGiver : MonoBehaviour
    {
        public int questId = 1;
        public string prompt = "Press E to accept quest";

        private bool playerInRange;

        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                QuestManager.Instance?.StartQuestById(questId);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            playerInRange = true;
            QuestUI.Instance?.ShowPrompt(prompt);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            playerInRange = false;
            QuestUI.Instance?.ClearPrompt();
        }
    }
}
