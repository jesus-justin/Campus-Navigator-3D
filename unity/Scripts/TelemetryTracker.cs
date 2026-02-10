using UnityEngine;

namespace CampusNavigator
{
    public class TelemetryTracker : MonoBehaviour
    {
        public float sampleInterval = 2f;
        public float minMoveDistance = 0.5f;

        private float nextSample;
        private Vector3 lastPos;

        private void Start()
        {
            lastPos = transform.position;
        }

        private void Update()
        {
            if (Time.time < nextSample)
            {
                return;
            }
            nextSample = Time.time + sampleInterval;

            float dist = Vector3.Distance(lastPos, transform.position);
            if (dist < minMoveDistance)
            {
                return;
            }

            lastPos = transform.position;
            var evt = new TelemetryEvent
            {
                eventType = "move",
                locationId = 0,
                posX = transform.position.x,
                posY = transform.position.y,
                posZ = transform.position.z,
                createdAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            FindObjectOfType<TelemetryBuffer>()?.Enqueue(evt);
        }
    }
}
