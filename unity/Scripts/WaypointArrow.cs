using UnityEngine;
using UnityEngine.UI;

namespace CampusNavigator
{
    public class WaypointArrow : MonoBehaviour
    {
        public RectTransform arrow;
        public Text distanceText;
        public Transform player;
        public Camera mainCamera;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (arrow == null || player == null || mainCamera == null)
            {
                return;
            }

            if (!QuestManager.Instance || !QuestManager.Instance.HasActiveTarget)
            {
                arrow.gameObject.SetActive(false);
                if (distanceText != null)
                {
                    distanceText.text = "";
                }
                return;
            }

            if (!LocationRegistry.TryGetPosition(QuestManager.Instance.ActiveTargetLocationId, out var targetPos))
            {
                arrow.gameObject.SetActive(false);
                if (distanceText != null)
                {
                    distanceText.text = "";
                }
                return;
            }

            arrow.gameObject.SetActive(true);

            Vector3 viewport = mainCamera.WorldToViewportPoint(targetPos);
            Vector2 fromCenter = new Vector2(viewport.x - 0.5f, viewport.y - 0.5f);

            float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
            arrow.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            if (distanceText != null)
            {
                float dist = Vector3.Distance(player.position, targetPos);
                distanceText.text = Mathf.RoundToInt(dist) + " m";
            }
        }
    }
}
