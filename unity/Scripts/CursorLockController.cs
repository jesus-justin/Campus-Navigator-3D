using UnityEngine;

namespace CampusNavigator
{
    public class CursorLockController : MonoBehaviour
    {
        public static CursorLockController Instance { get; private set; }

        public bool lockOnStart = false;
        public bool requireClickToLock = true;
        public KeyCode unlockKey = KeyCode.Escape;

        public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (lockOnStart)
            {
                LockCursor();
            }
            else
            {
                UnlockCursor();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(unlockKey))
            {
                UnlockCursor();
                return;
            }

            if (!IsLocked && ShouldLock())
            {
                LockCursor();
            }
        }

        private bool ShouldLock()
        {
            if (requireClickToLock)
            {
                return Input.GetMouseButtonDown(0);
            }
            return Input.anyKeyDown;
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
