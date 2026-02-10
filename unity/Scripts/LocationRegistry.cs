using System.Collections.Generic;
using UnityEngine;

namespace CampusNavigator
{
    public static class LocationRegistry
    {
        private static readonly Dictionary<int, Transform> registry = new Dictionary<int, Transform>();

        public static void Register(int locationId, Transform marker)
        {
            if (locationId <= 0 || marker == null)
            {
                return;
            }
            registry[locationId] = marker;
        }

        public static void Unregister(int locationId, Transform marker)
        {
            if (locationId <= 0 || marker == null)
            {
                return;
            }
            if (registry.TryGetValue(locationId, out var current) && current == marker)
            {
                registry.Remove(locationId);
            }
        }

        public static bool TryGetPosition(int locationId, out Vector3 position)
        {
            if (registry.TryGetValue(locationId, out var marker) && marker != null)
            {
                position = marker.position;
                return true;
            }
            position = Vector3.zero;
            return false;
        }
    }
}
