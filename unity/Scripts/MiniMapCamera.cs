using UnityEngine;

namespace CampusNavigator
{
    public class MiniMapCamera : MonoBehaviour
    {
        public Transform target;
        public float height = 40f;
        public float size = 35f;

        private Camera cam;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = size;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
