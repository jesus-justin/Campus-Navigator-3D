using UnityEngine;

namespace CampusNavigator
{
    public class QuestInteractable : MonoBehaviour, IInteractable
    {
        public int locationId = 0;
        public string prompt = "Interact";

        public string GetPrompt()
        {
            return prompt;
        }

        public void Interact(PlayerInteraction player)
        {
            QuestManager.Instance?.TryInteractAtLocation(locationId);
        }
    }
}
