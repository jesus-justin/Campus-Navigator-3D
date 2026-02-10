using UnityEngine;

namespace CampusNavigator
{
    public class ApiConfig : MonoBehaviour
    {
        [Header("API")]
        public string baseUrl = "http://localhost/Campus-Navigator-3D/api";

        public static ApiConfig Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
