using UnityEngine;

namespace CampusNavigator
{
    public class SubmitStation : MonoBehaviour, IInteractable
    {
        public int locationId = 0;
        public string prompt = "Submit item";

        public string GetPrompt()
        {
            return prompt;
        }

        public void Interact(PlayerInteraction player)
        {
            QuestManager.Instance?.TrySubmitAtLocation(locationId);
        }
    }
}
