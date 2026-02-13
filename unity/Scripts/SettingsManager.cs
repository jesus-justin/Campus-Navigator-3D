using UnityEngine;

namespace CampusNavigator
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [Range(0.2f, 10f)]
        public float mouseSensitivity = 2f;
        [Range(0f, 1f)]
        public float masterVolume = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            ApplyVolume();
        }

        public void ApplyMouseSensitivity(float value)
        {
            mouseSensitivity = value;
        }

        public void ApplyVolume(float value)
        {
            masterVolume = value;
            AudioListener.volume = masterVolume;
        }
    }
}
