# Colony Floor — verified project state

- Game: `ant_house_fps`
- Engine: Unity 6 URP; scene: `Assets/Scenes/ColonyKitchen.unity`
- Stage: nest extension verified and published. WebGL revision `1790557665` completed successfully; all four matching build artifacts are ready and the platform game manifest was created.
- Publication: `tmp/unity-backup/logs/unity-backup-result-eec2025e-43b3-4282-ab56-ca990ce494f5_e6266541372d4a4b5102745922330c90.json`; final publisher status `SUCCESS`.
- Controls: WASD, mouse look, Shift sprint, Space jump, left click acid, E gather, Q soldier, C collector, Hold Alt nest HUD, Escape pause/resume.
- Actual implementation: CharacterController player; swept visible projectiles; regenerating health; renewable food; four allied soldier slots; three collector slots; player nest (360 HP) that raiders attack; player respawn at nest; wall climbing on tall kitchen surfaces; furniture-aware ant navigation; continuous enemy spawning; destructible anthill; loading, help, pause, win/loss and replay screens.

## Change record
- Added a destructible NW player nest. Enemy raiders attack it; nest HP 0 loses the raid.
- Player death now respawns at the nest instead of immediately ending the game.
- HUD spawn dock: Collector (2 food) gathers crumbs; Soldier (3 food) attacks nearest enemies and the enemy hill. Hold Alt to click the buttons.
- Ants climb tall kitchen walls. Floor grout is ignored so spawn walking stays grounded.

## Verification
- Compilation: `compile_success`, no compile errors.
- `ColonyRegression`: 23/23 Play Mode checks passed. Covers UI Play, movement/look, jump, pickups, soldier summon, collector HUD spawn, friendly-fire, animation, pause/resume, raider spawn, enemy projectiles, nest targeting, wall climb, hill victory, spawn stop, menu, replay reset, nest respawn, nest defeat, restart.
- Runtime console: no error entries after the final regression.
- Visual checks: 1280×720 HUD, 1280×800 HUD, 1024×768 HUD. Nest bar, spawn buttons, and food/soldier/collector counts are readable. No magenta materials.

## Asset provenance
- Animated ant: mujtaba-io, CC0. https://opengameart.org/content/ant-3d-model-rigging-animated-low-poly-ish
- Player nest reuses the local anthill mesh with a clay nest material.
- Collector ant reuses the soldier prefab with gold chitin.

## Finish notes
- Gameplay test driver is Editor-only and is not attached to the saved scene.
