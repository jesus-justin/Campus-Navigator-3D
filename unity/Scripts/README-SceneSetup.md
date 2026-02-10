# Unity Sample Scene Setup

This guide creates a playable prototype scene using the scripts in this folder.

## 1) Scene Objects

Create these GameObjects:

- ApiConfig (add ApiConfig)
- ApiClient (add ApiClient)
- TelemetryBuffer (add TelemetryBuffer)
- QuestManager (add QuestManager)
- QuestUI (add QuestUI)
- LocationSpawner (add LocationSpawner)
- MiniMapCamera (add MiniMapCamera)
- WaypointArrow (add WaypointArrow)

## 2) Player

- Create an empty GameObject "Player" and tag it as Player.
- Add CharacterController.
- Add PlayerController and TelemetryTracker.
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

Option B (manual):
- Create an empty GameObject with a BoxCollider (Is Trigger = true).
- Add LocationMarker and set locationId to match the database.
- Optionally add a visible mesh (cube) to see the zone.

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
