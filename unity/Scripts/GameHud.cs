using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class GameHud : MonoBehaviour
    {
        public Text hintText;
        public InputConfig inputConfig;

        [TextArea]
        public string unlockedHint = "Click to play. WASD move, mouse look.";

        [TextArea]
        public string lockedHint = "WASD move, mouse look, E interact, I inventory, Q quests, ESC unlock.";

        private void Update()
        {
            if (hintText == null)
            {
                return;
            }

            bool locked = Cursor.lockState == CursorLockMode.Locked;
            hintText.text = locked ? BuildHint(lockedHint) : BuildHint(unlockedHint);
        }

        private string BuildHint(string baseText)
        {
            if (inputConfig == null)
            {
                return baseText;
            }

            string hint = baseText;
            hint = hint.Replace("E", inputConfig.interactKey.ToString());
            hint = hint.Replace("I", inputConfig.inventoryKey.ToString());
            hint = hint.Replace("Q", inputConfig.questListKey.ToString());
            hint = hint.Replace("ESC", KeyCode.Escape.ToString());
            return hint;
        }
    }
}
