using System;
using System.Collections.Generic;
using UnityEngine;

namespace CampusNavigator
{
    [CreateAssetMenu(fileName = "CampusMapProfile", menuName = "CampusNavigator/Campus Map Profile")]
    public class CampusMapProfile : ScriptableObject
    {
        [Header("Identity")]
        public string campusCode = "batstateu-lipa";
        public string campusName = "Batangas State University - The NEU Lipa Campus";

        [Header("Source Model")]
        public string sourceUrl = "https://sketchfab.com/3d-models/batangas-state-university-the-neu-lipa-map-abff63aeea7c42a1a7916b1a2a25c24a";
        public string authorName = "BSU-TNEU_mtw2024";
        public string licenseName = "CC BY 4.0";
        public string licenseUrl = "https://creativecommons.org/licenses/by/4.0/";
        [TextArea(2, 5)] public string attributionText = "Batangas State University- The NEU Lipa Map by BSU-TNEU_mtw2024, licensed under CC BY 4.0.";

        [Header("Model Prefab")]
        public GameObject modelPrefab;
        public Vector3 modelPosition;
        public Vector3 modelRotationEuler;
        public Vector3 modelScale = Vector3.one;

        [Header("Location Binding")]
        public List<LocationAnchorBinding> anchorBindings = new List<LocationAnchorBinding>();

        public bool HasLicenseMetadata()
        {
            return !string.IsNullOrWhiteSpace(sourceUrl)
                && !string.IsNullOrWhiteSpace(authorName)
                && !string.IsNullOrWhiteSpace(licenseName)
                && !string.IsNullOrWhiteSpace(attributionText);
        }

        public bool TryGetBinding(int locationId, out LocationAnchorBinding binding)
        {
            binding = null;
            if (locationId <= 0)
            {
                return false;
            }

            for (int i = 0; i < anchorBindings.Count; i++)
            {
                var candidate = anchorBindings[i];
                if (candidate != null && candidate.locationId == locationId)
                {
                    binding = candidate;
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public class LocationAnchorBinding
    {
        public int locationId;
        public string expectedLocationName;
        [Tooltip("Path under the model root, e.g. Buildings/Admin/Registrar")]
        public string anchorPath;
        [Tooltip("Override API radius if greater than 0.")]
        public float markerRadiusOverride;
    }
}
