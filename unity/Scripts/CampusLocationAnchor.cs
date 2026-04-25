using UnityEngine;

namespace CampusNavigator
{
    public class CampusLocationAnchor : MonoBehaviour
    {
        [Tooltip("Must match locations.id in MySQL")]
        public int locationId;
        public string locationName;

        private void OnEnable()
        {
            if (locationId > 0)
            {
                LocationRegistry.Register(locationId, transform, locationName);
            }
        }

        private void OnDisable()
        {
            if (locationId > 0)
            {
                LocationRegistry.Unregister(locationId, transform);
            }
        }
    }
}
