using UnityEngine;

namespace CampusNavigator
{
    public class WebGLFocusHelper : MonoBehaviour
    {
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                CursorLockController.Instance?.UnlockCursor();
            }
        }
    }
}
