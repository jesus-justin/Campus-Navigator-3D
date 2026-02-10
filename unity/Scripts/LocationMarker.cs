using System;
using UnityEngine;

namespace CampusNavigator
{
    public class LocationMarker : MonoBehaviour
    {
        public int locationId = 0;
        public string locationName = "";

        private void OnEnable()
        {
            LocationRegistry.Register(locationId, transform);
        }

        private void OnDisable()
        {
            LocationRegistry.Unregister(locationId, transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            QuestManager.Instance?.OnReachLocation(locationId, locationName, transform.position);

            var buffer = FindObjectOfType<TelemetryBuffer>();
            if (buffer != null)
            {
                buffer.Enqueue(new TelemetryEvent
                {
                    eventType = "enter",
                    locationId = locationId,
                    posX = transform.position.x,
                    posY = transform.position.y,
                    posZ = transform.position.z,
                    createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }
    }
}
