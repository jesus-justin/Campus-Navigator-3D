using UnityEngine;

namespace CampusNavigator
{
    public class NpcQuestGiver : MonoBehaviour, IInteractable
    {
        public int questId = 1;
        public string prompt = "Talk to NPC";
        public string greeting = "Hello! Need help finding your next task?";

        public string GetPrompt()
        {
            return prompt;
        }

        public void Interact(PlayerInteraction player)
        {
            DialogueManager.Instance?.ShowQuestDialogue(greeting, questId);
        }
    }
}
