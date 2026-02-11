using System.Collections;
using UnityEngine;

namespace CampusNavigator
{
    public class LocationSpawner : MonoBehaviour
    {
        public GameObject markerPrefab;
        public bool autoLoginIfMissing = true;
        public string autoLoginExternalId = "student-001";
        public string autoLoginName = "Student";

        private void Start()
        {
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

            foreach (var loc in res.locations)
            {
                SpawnMarker(loc);
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
            marker.transform.position = new Vector3(loc.pos_x, loc.pos_y, loc.pos_z);

            var markerComp = marker.GetComponent<LocationMarker>();
            if (markerComp == null)
            {
                markerComp = marker.AddComponent<LocationMarker>();
            }
            markerComp.locationId = loc.id;
            markerComp.locationName = loc.name;

            LocationRegistry.Register(loc.id, marker.transform, loc.name);

            float radius = Mathf.Max(1f, loc.radius_m);
            marker.transform.localScale = new Vector3(radius * 2f, marker.transform.localScale.y, radius * 2f);
        }
    }
}
