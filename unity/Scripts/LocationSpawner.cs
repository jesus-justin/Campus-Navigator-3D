using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CampusNavigator
{
    public class LocationSpawner : MonoBehaviour
    {
        public event Action<LocationDto[]> LocationsLoaded;

        public GameObject markerPrefab;
        public bool autoLoginIfMissing = true;
        public string autoLoginExternalId = "student-001";
        public string autoLoginName = "Student";
        public CampusMapRuntime mapRuntime;
        public bool snapToCampusAnchors = true;
        public bool hideMarkerRendererWhenAnchored = true;
        public bool logCoverageOnLoad = true;

        private readonly HashSet<int> mappedIds = new HashSet<int>();
        private int totalLoaded;

        private void Start()
        {
            if (mapRuntime == null)
            {
                mapRuntime = CampusMapRuntime.Instance;
            }
            StartCoroutine(LoadLocations());
        }

        private IEnumerator LoadLocations()
        {
            if (ApiClient.Instance == null)
            {
                yield break;
            }

            if (!ApiClient.Instance.HasToken && autoLoginIfMissing)
            {
                bool loginDone = false;
                yield return ApiClient.Instance.Login(autoLoginExternalId, autoLoginName, _ => { loginDone = true; }, _ => { loginDone = true; });
                while (!loginDone)
                {
                    yield return null;
                }
            }

            yield return ApiClient.Instance.GetLocations(OnLocationsLoaded, _ => { });
        }

        private void OnLocationsLoaded(LocationsResponse res)
        {
            if (res == null || res.locations == null)
            {
                return;
            }

            LocationsLoaded?.Invoke(res.locations);

            mappedIds.Clear();
            totalLoaded = res.locations.Length;

            foreach (var loc in res.locations)
            {
                SpawnMarker(loc);
            }

            if (logCoverageOnLoad)
            {
                int covered = mappedIds.Count;
                Debug.Log("LocationSpawner: loaded " + totalLoaded + " API locations. Anchored to 3D model: " + covered + ".");
            }
        }

        private void SpawnMarker(LocationDto loc)
        {
            GameObject marker;
            if (markerPrefab != null)
            {
                marker = Instantiate(markerPrefab, transform);
            }
            else
            {
                marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                marker.transform.SetParent(transform);
                marker.transform.localScale = new Vector3(1f, 0.2f, 1f);
                var col = marker.GetComponent<Collider>();
                if (col != null)
                {
                    col.isTrigger = true;
                }
            }

            marker.name = "Location_" + loc.id + "_" + loc.name;

            bool hasAnchor = false;
            if (snapToCampusAnchors && mapRuntime != null && mapRuntime.TryGetAnchor(loc.id, out var anchor) && anchor != null)
            {
                marker.transform.position = anchor.position;
                hasAnchor = true;
                mappedIds.Add(loc.id);
            }
            else
            {
                marker.transform.position = new Vector3(loc.pos_x, loc.pos_y, loc.pos_z);
            }

            var markerComp = marker.GetComponent<LocationMarker>();
            if (markerComp == null)
            {
                markerComp = marker.AddComponent<LocationMarker>();
            }
            markerComp.locationId = loc.id;
            markerComp.locationName = loc.name;

            LocationRegistry.Register(loc.id, marker.transform, loc.name);

            float radius = Mathf.Max(1f, loc.radius_m);
            if (mapRuntime != null)
            {
                float overrideRadius = mapRuntime.GetRadiusOverride(loc.id);
                if (overrideRadius > 0f)
                {
                    radius = overrideRadius;
                }
            }
            marker.transform.localScale = new Vector3(radius * 2f, marker.transform.localScale.y, radius * 2f);

            if (hasAnchor && hideMarkerRendererWhenAnchored)
            {
                var renderers = marker.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].enabled = false;
                }
            }
        }
    }
}
