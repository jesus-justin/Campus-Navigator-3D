using System.Linq;
using UnityEngine;

namespace CampusNavigator
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactRadius = 2f;
        public LayerMask interactMask = ~0;
        public InputConfig inputConfig;
        public bool requireCursorLock = true;

        private IInteractable current;

        private void Update()
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen)
            {
                return;
            }
            if (requireCursorLock && Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }
            FindInteractable();

            if (current != null)
            {
                QuestUI.Instance?.ShowPrompt(current.GetPrompt());
                if (Input.GetKeyDown(GetInteractKey()))
                {
                    current.Interact(this);
                }
            }
            else
            {
                QuestUI.Instance?.ClearPrompt();
            }
        }

        private void FindInteractable()
        {
            var hits = Physics.OverlapSphere(transform.position, interactRadius, interactMask);
            current = null;

            if (hits == null || hits.Length == 0)
            {
                return;
            }

            current = hits
                .Select(h => h.GetComponent<IInteractable>())
                .FirstOrDefault(i => i != null);
        }

        private KeyCode GetInteractKey()
        {
            return inputConfig != null ? inputConfig.interactKey : KeyCode.E;
        }
    }
}
