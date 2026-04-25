using System.Collections.Generic;
using UnityEngine;

namespace CampusNavigator
{
    public class CampusMapCoverageMonitor : MonoBehaviour
    {
        public CampusMapRuntime mapRuntime;
        public LocationSpawner locationSpawner;
        public bool failOnMissingAnchors;

        private void Start()
        {
            if (mapRuntime == null)
            {
                mapRuntime = CampusMapRuntime.Instance;
            }

            if (locationSpawner == null)
            {
                locationSpawner = FindObjectOfType<LocationSpawner>();
            }

            if (locationSpawner != null)
            {
                locationSpawner.LocationsLoaded += OnLocationsLoaded;
            }
        }

        private void OnDestroy()
        {
            if (locationSpawner != null)
            {
                locationSpawner.LocationsLoaded -= OnLocationsLoaded;
            }
        }

        private void OnLocationsLoaded(LocationDto[] locations)
        {
            if (locations == null || locations.Length == 0 || mapRuntime == null)
            {
                return;
            }

            var missing = new List<string>();
            int anchored = 0;

            for (int i = 0; i < locations.Length; i++)
            {
                var loc = locations[i];
                if (mapRuntime.TryGetAnchor(loc.id, out _))
                {
                    anchored++;
                }
                else
                {
                    missing.Add(loc.id + ":" + loc.name);
                }
            }

            if (missing.Count == 0)
            {
                Debug.Log("CampusMapCoverageMonitor: all " + locations.Length + " API locations are mapped to campus anchors.");
                return;
            }

            float coveragePercent = (anchored * 100f) / locations.Length;
            string msg = "CampusMapCoverageMonitor: mapped " + anchored + "/" + locations.Length + " locations (" + coveragePercent.ToString("F1") + "%). Missing: " + string.Join(", ", missing.ToArray());

            if (failOnMissingAnchors)
            {
                Debug.LogError(msg);
            }
            else
            {
                Debug.LogWarning(msg);
            }
        }
    }
}
