using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject panel;
        public Button resumeButton;
        public Button quitButton;
        public InputConfig inputConfig;
        public StartScreen startScreen;

        private bool paused;

        private void Start()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(Resume);
            }
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(Quit);
            }
        }

        private void Update()
        {
            if (startScreen != null && startScreen.IsActive)
            {
                return;
            }

            if (Input.GetKeyDown(GetPauseKey()))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            paused = true;
            if (panel != null)
            {
                panel.SetActive(true);
            }
            Time.timeScale = 0f;
            CursorLockController.Instance?.UnlockCursor();
        }

        public void Resume()
        {
            paused = false;
            if (panel != null)
            {
                panel.SetActive(false);
            }
            Time.timeScale = 1f;
            CursorLockController.Instance?.LockCursor();
        }

        public void Quit()
        {
            Application.Quit();
        }

        private KeyCode GetPauseKey()
        {
            return inputConfig != null ? inputConfig.pauseKey : KeyCode.P;
        }
    }
}
