# BatStateU Lipa 3D Campus Map Integration

This guide integrates the Sketchfab campus model into the existing API-driven location and quest system.

## 1) Source Model and License

Requested source model:
- Model: Batangas State University- The NEU Lipa Map
- URL: https://sketchfab.com/3d-models/batangas-state-university-the-neu-lipa-map-abff63aeea7c42a1a7916b1a2a25c24a
- Author: BSU-TNEU_mtw2024
- License: CC BY 4.0

Required for compliance:
1. Keep attribution in project documentation and release notes.
2. Preserve model source URL in your Unity CampusMapProfile asset.
3. Confirm downstream distribution includes CC BY attribution.

## 2) Import to Unity

1. Download from Sketchfab in a format Unity can import (GLB/FBX).
2. Add the file under Assets/Models/BatStateU-Lipa/.
3. Create a prefab from the imported model, example: Assets/Prefabs/BatStateU-Lipa-Campus.prefab.
4. Normalize transform:
- Forward axis: Z+
- Up axis: Y+
- Scale target: 1 unit = 1 meter

## 3) Create CampusMapProfile

1. Create asset: Create > CampusNavigator > Campus Map Profile.
2. Fill fields:
- campusCode: batstateu-lipa
- campusName: Batangas State University - The NEU Lipa Campus
- sourceUrl: model URL above
- authorName: BSU-TNEU_mtw2024
- licenseName: CC BY 4.0
- licenseUrl: https://creativecommons.org/licenses/by/4.0/
- attributionText: model attribution statement
- modelPrefab: BatStateU prefab
3. Configure modelPosition, modelRotationEuler, and modelScale.

## 4) Bind DB Locations to 3D Anchors

Option A: Component binding (best for large scenes)
1. Add CampusLocationAnchor to target transforms inside the model prefab.
2. Set locationId to match locations.id in MySQL.
3. Set locationName (optional).

Option B: Profile path binding (best for central management)
1. In CampusMapProfile.anchorBindings, add one item per location.
2. Set locationId.
3. Set anchorPath relative to model root.
4. Optional: set markerRadiusOverride.

## 5) Scene Wiring

1. Add CampusMapRuntime to a scene object.
2. Assign CampusMapProfile.
3. Enable strictLicenseGate for release builds.
4. Set LocationSpawner.mapRuntime.
5. Add CampusMapCoverageMonitor and assign:
- mapRuntime
- locationSpawner

## 6) Validation and Enterprise Readiness

Use this acceptance checklist before deployment:
1. API location load succeeds without fallback marker drift.
2. CampusMapCoverageMonitor reports 100% mapped locations for production IDs.
3. Trigger colliders fire reach and submit steps at expected anchors.
4. Telemetry enter events include correct locationId for all major buildings.
5. Quest pathing and waypoint arrow target names align with visible building labels.
6. Attribution is visible in documentation and app credits.

## 7) Operational Recommendations

1. Keep separate map profiles for dev/staging/prod.
2. Version map profile and DB seed together per release tag.
3. Run a location coverage smoke test after each model update.
4. Keep model optimization budget:
- Target < 250k triangles for WebGL export
- Use LOD groups for distant geometry
- Use baked lighting for static campus geometry
