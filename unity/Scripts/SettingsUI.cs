using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class SettingsUI : MonoBehaviour
    {
        public GameObject panel;
        public Slider sensitivitySlider;
        public Slider volumeSlider;
        public Button closeButton;

        private void Start()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }
            Refresh();
        }

        public void Open()
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }
            Refresh();
        }

        public void Close()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        public void Refresh()
        {
            if (SettingsManager.Instance == null)
            {
                return;
            }
            if (sensitivitySlider != null)
            {
                sensitivitySlider.value = SettingsManager.Instance.mouseSensitivity;
                sensitivitySlider.onValueChanged.RemoveAllListeners();
                sensitivitySlider.onValueChanged.AddListener(SettingsManager.Instance.ApplyMouseSensitivity);
            }
            if (volumeSlider != null)
            {
                volumeSlider.value = SettingsManager.Instance.masterVolume;
                volumeSlider.onValueChanged.RemoveAllListeners();
                volumeSlider.onValueChanged.AddListener(SettingsManager.Instance.ApplyVolume);
            }
        }
    }
}
