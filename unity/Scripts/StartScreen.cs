using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class StartScreen : MonoBehaviour
    {
        public GameObject panel;
        public Button playButton;
        public InputConfig inputConfig;

        public bool IsActive => panel != null && panel.activeSelf;

        private void Start()
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }
            CursorLockController.Instance?.UnlockCursor();
            if (playButton != null)
            {
                playButton.onClick.AddListener(StartGame);
            }
        }

        private void Update()
        {
            if (IsActive && Input.GetKeyDown(GetStartKey()))
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
            CursorLockController.Instance?.LockCursor();
        }

        private KeyCode GetStartKey()
        {
            return inputConfig != null ? inputConfig.startKey : KeyCode.Return;
        }
    }
}
