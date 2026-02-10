using UnityEngine;

namespace CampusNavigator
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 1.6f, 0f);

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }
            transform.position = target.position + offset;
        }
    }
}
