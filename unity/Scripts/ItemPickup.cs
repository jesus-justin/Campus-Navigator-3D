using UnityEngine;

namespace CampusNavigator
{
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        public string itemId = "Enrollment Form";
        public string prompt = "Pick up item";

        public string GetPrompt()
        {
            return prompt + ": " + itemId;
        }

        public void Interact(PlayerInteraction player)
        {
            InventoryManager.Instance?.AddItem(itemId);
            QuestUI.Instance?.SetStatus("Picked up: " + itemId);
            Destroy(gameObject);
        }
    }
}
