# Unity Sample Scene Setup

This guide creates a playable prototype scene using the scripts in this folder.

## 1) Scene Objects

Create these GameObjects:

- ApiConfig (add ApiConfig)
- ApiClient (add ApiClient)
- CampusMapRuntime (add CampusMapRuntime)
- CampusMapCoverageMonitor (add CampusMapCoverageMonitor)
- TelemetryBuffer (add TelemetryBuffer)
- QuestManager (add QuestManager)
- QuestUI (add QuestUI)
- LocationSpawner (add LocationSpawner)
- MiniMapCamera (add MiniMapCamera)
- WaypointArrow (add WaypointArrow)
- DialogueManager (add DialogueManager)
- InventoryManager (add InventoryManager)
- InventoryUI (add InventoryUI)
- PlayerInteraction (add PlayerInteraction)
- CursorLockController (add CursorLockController)
- GameHud (add GameHud)
- WebGLApiOverride (add WebGLApiOverride, optional)
- WebGLFocusHelper (add WebGLFocusHelper, optional)
- StartScreen (add StartScreen)
- PauseMenu (add PauseMenu)
- SettingsManager (add SettingsManager)
- SettingsUI (add SettingsUI)
- QuestTrackerUI (add QuestTrackerUI)

Input config:
- In Project window, create InputConfig asset (Create > CampusNavigator > InputConfig).
- Assign it to PlayerInteraction, DialogueManager, QuestUI, and InventoryUI.
- Assign it to GameHud (optional for key hints).

Campus map config:
- In Project window, create CampusMapProfile asset (Create > CampusNavigator > Campus Map Profile).
- Fill sourceUrl, authorName, licenseName, and attributionText from your model source.
- Assign the imported BatStateU Lipa model prefab to CampusMapProfile.modelPrefab.
- Assign the profile to CampusMapRuntime.
- Set LocationSpawner.mapRuntime to the CampusMapRuntime object.
- Set CampusMapCoverageMonitor.mapRuntime and .locationSpawner.

## 2) Player

- Create an empty GameObject "Player" and tag it as Player.
- Add CharacterController.
- Add PlayerController, TelemetryTracker, and PlayerInteraction.
- Create a child Camera (MainCamera tag).
- Add CameraFollow to the Camera, set target to Player.
- Set PlayerController.cameraPivot to the Camera transform.

## 3) NPC Quest Giver

- Create a capsule "NPC" with a SphereCollider (Is Trigger = true).
- Add NpcQuestGiver.
- Set questId to 1.

## 4) Locations

Option A (recommended): Auto-spawn from API
- Add LocationSpawner to the LocationSpawner object.
- (Optional) Assign a marker prefab with a trigger collider.
- Set LocationSpawner.snapToCampusAnchors = true to place triggers on model anchors.
- Add CampusLocationAnchor to model child transforms, and set locationId to match DB locations.id.

Option B (manual):
- Create an empty GameObject with a BoxCollider (Is Trigger = true).
- Add LocationMarker and set locationId to match the database.
- Optionally add a visible mesh (cube) to see the zone.

Quest interactable (optional):
- Add QuestInteractable to any object and set locationId.

Submit station (submit steps):
- Add SubmitStation to the quest target location.
- Set locationId to match the step target.

Item pickup:
- Add ItemPickup to a prop and set itemId (e.g., Enrollment Form).

## 5) UI Canvas

- Create a Canvas > Panel.
- Add four Text elements: QuestTitle, QuestStep, StatusText, PromptText.
- Assign them to QuestUI fields.

Mini-map UI:
- Create a RenderTexture and assign it to a RawImage.
- Set the RawImage size to a square (e.g., 180x180).
- Create a Camera tagged as MiniMapCamera and assign the RenderTexture to its Target Texture.
- Add MiniMapCamera script and set target to Player.

Waypoint arrow UI:
- Create a UI Image (arrow icon) and a Text for distance.
- Add WaypointArrow script and assign arrow RectTransform, distance Text, player, and main camera.
- (Optional) Assign a Text for target name.

Dialogue UI:
- Create a Panel with a Title Text, Body Text, and 3 Buttons.
- Add DialogueUI and wire the panel, texts, and buttons.
- Add DialogueManager and assign DialogueUI + InputConfig.

Inventory UI:
- Create a Panel with a Text element for inventory list.
- Add InventoryUI and assign panel + list text + InputConfig.

HUD:
- Create a Text element anchored to the top-left for hints.
- Add GameHud and assign the Text (and InputConfig).

Start screen:
- Create a Panel with a Play button.
- Add StartScreen and assign panel, button, and InputConfig.

Pause menu:
- Create a Panel with Resume and Quit buttons.
- Add PauseMenu and assign panel, buttons, InputConfig, and StartScreen reference.

Settings UI:
- Create a Panel with two Sliders (mouse sensitivity and volume).
- Add SettingsUI and assign panel + sliders + close button.
- Add SettingsManager to any scene object.

Quest tracker:
- Create two Text elements for quest title and step.
- Add QuestTrackerUI and assign both Texts.

## 6) API

- Update ApiConfig baseUrl to your XAMPP API URL.
- Ensure the database is seeded with quest/location data.

## 7) Test

- Press Play.
- Walk into the NPC trigger and press E.
- Walk into location markers to complete steps.

Notes:
- This uses Unity's legacy input axes (Horizontal, Vertical, Mouse X/Y).
- Ensure your project has the Input Manager defaults enabled.
