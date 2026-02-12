using UnityEngine;

namespace CampusNavigator
{
    public class WebGLApiOverride : MonoBehaviour
    {
        public bool useRelativePath = true;
        public string relativePath = "/Campus-Navigator-3D/api";

        private void Awake()
        {
#if UNITY_WEBGL
            if (useRelativePath && ApiConfig.Instance != null)
            {
                ApiConfig.Instance.baseUrl = relativePath;
            }
#endif
        }
    }
}
