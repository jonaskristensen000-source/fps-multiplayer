# Colony Floor — Tiny Ant House FPS

## Game overview
- **Title:** Colony Floor
- **Genre:** 3D first-person shooter with squad summoning
- **Target players:** Casual FPS players who want a short, readable raid
- **Platform:** Unity 6 URP, WebGL/fullpack via Seele
- **Session length:** 4–8 minutes

You play as a worker ant on a kitchen floor that feels like a city. Food crumbs buy soldier ants. A rival anthill keeps spawning shooters until you destroy the mound.

## Dimensionality and camera
- **3D**, first-person camera on a CharacterController
- Eye height ~0.72 m, capsule height 0.9 m (ant-scale world)
- Mouse look, pitch clamped −80..80
- No third-person body in view; mandible acid sprayer is a first-person viewmodel

## Core loop
1. Load assets (`N/M`, 30 s timeout) → title screen
2. Start → spawn at the northwest nest, facing the southeast anthill
3. Collect food crumbs along the floor, under the table, and near the fridge
4. Hold Alt and use HUD buttons (or Q/C) to spawn collectors and soldiers from the nest
5. Fight raiders pouring from the anthill; raiders try to destroy the nest
6. Climb kitchen walls by walking into them
7. Damage the mound until it collapses → victory
8. Player HP 0 → respawn at the nest; nest HP 0 → defeat
9. Restart from the results screen

## Controls
| Input | Action |
|---|---|
| WASD | Move |
| Mouse | Look |
| Left Shift | Sprint |
| Space | Jump |
| Left mouse | Fire formic-acid glob |
| E | Pick up nearby food (also auto-pickup on overlap) |
| Q | Spawn soldier ant (3 food) |
| C | Spawn collector ant (2 food) |
| Hold Alt | Show cursor and click nest spawn HUD |
| Esc | Pause / resume |

## Player
- HP 100, regenerates 4/s after 5 s without damage
- Walk 5.2, sprint 8.4
- Acid glob: simulated projectile, 18 dmg, 22 m/s, 0.18 s fire interval, visible trail
- Inventory: Food (0–12). Pickup +1. Soldier costs 3 food, collector costs 2 food
- Max 4 living soldiers, max 3 collectors
- Player death respawns at the nest while the nest still stands
- Position writer: `PlayerController` CharacterController.Move; wall climb via `ColonyClimber`

## Allies
- **Soldier ants** spawn from the nest HUD, follow the player, engage nearest enemies and the enemy anthill within 14 m
- Soldier HP 55, acid glob 10 dmg
- **Collector ants** seek the nearest crumb, collect it, and keep foraging
- Collector HP 40, no gun
- Both climb tall kitchen walls when a target sits above them
- Die independently; do not fail the mission

## Enemies
- **Raider ants** spawn from the anthill every 4.5 s (ramps to 2.2 s), cap 8
- HP 28, hitscan-style acid spit with visible tracer, 8 dmg
- Chase nearby player/soldiers; otherwise attack the player nest
- **Anthill base:** 420 HP, landmark in the SE corner, cannot move
- **Player nest:** 360 HP, NW landmark, raider target, player respawn
- Destroying the anthill stops spawns and wins; destroying the nest loses

## Win / lose / restart
- Win: anthill HP ≤ 0
- Lose: player nest HP ≤ 0
- Player death respawns at the nest instead of ending the raid
- Score: food collected + enemies killed × 10 + remaining HP + nest HP + remaining soldiers × 25 + collectors × 15
- Results screen: Win/Lose, score, Restart, Menu

## Level — giant kitchen (ant scale)
World unit = 1 meter. The kitchen is a 48 × 36 m floor with 8 m walls (a real 2.4 × 1.8 m kitchen at 1:20).

### Spatial skeleton
- **Spawn / safe:** NW, under a window shaft of warm light. First look frames the anthill across open tile.
- **Landmark:** SE anthill mound + fridge wall silhouette
- **Main route:** open tile → crumb field → under-table canyon → fridge alley → anthill plaza
- **Side route:** along west baseboard behind chair legs (cover, extra food)
- **Combat plaza:** 12 m open around the mound
- **Rest / reward:** under-table shadow with two food piles
- **Bounds:** four kitchen walls; no fall-off. Safety floor 2 m below tiles, QA-only layer

### Layers
1. Landmark: fridge slab, table plateau, anthill
2. Functional: food crumbs, spawn, cover bottle/spoon, anthill
3. Environment: chair legs, baseboards, cabinet kickplates, cereal box, bottle
4. Detail: grout, crumbs scatter, dust motes, under-table cobweb cards

### Support surfaces
Kitchen tile floor, table top (optional climb later — not required for v1), fridge toe-kick. All walkable meshes have non-trigger colliders on layer `Ground`.

## UI / HUD
UI Toolkit only. One `GameUI.uxml` / `GameUI.uss`.
- Loading: `N/M` plus bar, skip after 30 s
- Start: title, Play, How to Play
- How to Play: controls + goal
- Pause: Resume, How to Play, Menu
- HUD: HP bar, food/soldier/collector counts, anthill HP, nest HP, spawn buttons, crosshair, hint
- Results: outcome, score, Restart, Menu
Art direction: warm amber + chitin red, paper-label kitchen type, crumb-grain panels.

## UI design fingerprint
- Experience: quick mouse-and-keyboard raids, readable from floor-level combat, desktop landscape layouts.
- Decisions: health and cover first, food and summon readiness second, anthill damage as persistent mission progress.
- Vocabulary: cream kitchen labels, dark chitin, golden crumbs, six-legged colony symbols.
- Palette: ink #21180F, cream #FFF0D6, crumb gold #F2C44A, rust danger #D65C2A, leaf ally #A6D66A, muted tan #C49E76.
- Typography: native Unity runtime font, bold 44 px headings, 18 px body and prompts, 22 px values/buttons. English coverage. Avoid narrow text boxes and overlay stacking.
- Layout: world-visible left title panel, compact edge-weighted HUD, 24 px safe area, flexible panel widths up to 620 px; PanelSettings scales for 16:9, 16:10 and 4:3.
- Shapes: square paper-label panels with a single gold accent edge, restrained 4 px corners; no glass or neon dashboard decoration.
- Feedback: immediate crumb/summon message, colored projectile trails, health bars, brief attack motion; result screen clearly separates replay and menu.
- Memorable point: the food-to-soldier relationship is called out directly: three golden crumbs buy one green allied ant.

## Art / audio
- Stylized realistic kitchen, warm tungsten, long floor shadows
- Ants: reddish worker, darker soldier, black-red raiders
- Projectiles: glowing amber glob + short trail (procedural)
- BGM: tense quiet kitchen raid
- SFX: spit, hit, pickup, summon, mound crack, player hurt

## Assets
Retrieve first, then generate, then Blender DCC.
- Ant (ally soldier / enemy raider) — `thing`, rig if needed
- Anthill mound — `thing`
- Food (crumb, fruit scrap) — `thing`
- Giant kitchen props: bottle, cereal box, fork/spoon, chair, table fragment — `thing`
- Floor/wall materials, indoor HDRI
- Loop motions: idle, walk; one-shot: attack, death
- BGM / SFX

House shell (floor, walls, cabinets) is engine-authored with final URP materials.

## Technical constraints
- Engine: Unity, URP, WebGL-safe
- Events via `EventDispatcher` / `GameEventTypes`
- Cursor hidden only after Play
- Test mode (`?qa=1` or QA bootstrap): snapshots, spawn food, damage hill, force win/lose
- All 3D file models: local download + Blender −Y front admission
- Projectiles: simulated acid globs with sweep collision

## Key assumptions
- First-person, no visible player body mesh (viewmodel mandible only)
- One kitchen arena, no extra maps
- Soldiers are AI followers, not player-possessed
- Anthill is the only win object

## Acceptance
- Load bar N/M, 30 s fallback
- WASD + mouse + shoot visible globs
- Food pickup increases food; Q summons a soldier that fights
- Enemy anthill spawns shooters until destroyed
- Win and lose + restart work
- Player stays grounded; no magenta materials
