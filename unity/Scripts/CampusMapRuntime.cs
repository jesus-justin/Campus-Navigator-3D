using System.Collections.Generic;
using UnityEngine;

namespace CampusNavigator
{
    public class CampusMapRuntime : MonoBehaviour
    {
        public static CampusMapRuntime Instance { get; private set; }

        [Header("Profile")]
        public CampusMapProfile profile;
        public Transform modelParent;
        public bool instantiateModelOnStart = true;
        public bool strictLicenseGate = true;

        private readonly Dictionary<int, Transform> anchorByLocationId = new Dictionary<int, Transform>();
        private Transform modelRoot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (profile == null)
            {
                Debug.LogWarning("CampusMapRuntime: no profile assigned.");
                return;
            }

            if (strictLicenseGate && !profile.HasLicenseMetadata())
            {
                Debug.LogError("CampusMapRuntime: missing model license metadata in CampusMapProfile.");
                return;
            }

            if (instantiateModelOnStart)
            {
                InstantiateModel();
            }

            RebuildAnchorIndex();
        }

        public void InstantiateModel()
        {
            if (profile == null || profile.modelPrefab == null)
            {
                return;
            }

            if (modelRoot != null)
            {
                Destroy(modelRoot.gameObject);
            }

            var parent = modelParent != null ? modelParent : transform;
            var modelObj = Instantiate(profile.modelPrefab, parent);
            modelObj.name = "CampusModel_" + profile.campusCode;
            modelObj.transform.localPosition = profile.modelPosition;
            modelObj.transform.localRotation = Quaternion.Euler(profile.modelRotationEuler);
            modelObj.transform.localScale = profile.modelScale;
            modelRoot = modelObj.transform;
        }

        public void RebuildAnchorIndex()
        {
            anchorByLocationId.Clear();

            if (modelRoot == null)
            {
                modelRoot = transform;
            }

            var discoveredAnchors = modelRoot.GetComponentsInChildren<CampusLocationAnchor>(true);
            for (int i = 0; i < discoveredAnchors.Length; i++)
            {
                var anchor = discoveredAnchors[i];
                if (anchor != null && anchor.locationId > 0)
                {
                    anchorByLocationId[anchor.locationId] = anchor.transform;
                }
            }

            if (profile == null || profile.anchorBindings == null)
            {
                return;
            }

            for (int i = 0; i < profile.anchorBindings.Count; i++)
            {
                var binding = profile.anchorBindings[i];
                if (binding == null || binding.locationId <= 0 || string.IsNullOrWhiteSpace(binding.anchorPath))
                {
                    continue;
                }

                var pathAnchor = modelRoot.Find(binding.anchorPath);
                if (pathAnchor != null)
                {
                    anchorByLocationId[binding.locationId] = pathAnchor;
                }
            }
        }

        public bool TryGetAnchor(int locationId, out Transform anchor)
        {
            if (locationId > 0 && anchorByLocationId.TryGetValue(locationId, out anchor) && anchor != null)
            {
                return true;
            }

            anchor = null;
            return false;
        }

        public float GetRadiusOverride(int locationId)
        {
            if (profile != null && profile.TryGetBinding(locationId, out var binding) && binding.markerRadiusOverride > 0f)
            {
                return binding.markerRadiusOverride;
            }

            return -1f;
        }

        public int GetAnchorCoverageCount()
        {
            return anchorByLocationId.Count;
        }
    }
}
