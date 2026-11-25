# Visual Asset Inventory

Status: audit complete; replacement implementation in progress. This inventory records the shipped Godot visual state before the visual-quality rework.

## Existing 3D assets

All existing model files are paired `.obj`/`.mtl` exports with flat colours and no texture images. The same 11 pairs exist in both `godot/assets/models/` and `Assets/Resources/Art/Models/`; duplicates are byte-identical. Total size per location: 58,683 bytes (55,131 bytes OBJ, 3,552 bytes MTL).

| Model | Current role | Current quality | Replacement |
| --- | --- | --- | --- |
| `SM_Characters_Runner_001.obj/.mtl` | Player character | Primitive blockout humanoid; no rig, animation, face, uniform detail, or authored silhouette. | `SM_Character_PoliceRunner_Animated.glb`: stylised Indian police runner with cap, badge, uniform, satchel, and run/idle/slide/jump animations. |
| `SM_Characters_Pursuer_001.obj/.mtl` | ACP pursuer | Primitive blockout humanoid; no rig, animation, or character identity. | `SM_Character_Suspect_Animated.glb`: distinct suspect/pursuer silhouette, jacket, trousers, face detail, and running animation. |
| `SM_Trains_Express_001.obj/.mtl` | Train obstacle | Simple box train; no windows, doors, wheels, roof, livery, lighting, or interior suggestion. | `SM_Train_Express_Detailed.glb`: compartment coach with windows, doors, roof ribs, wheels, buffers, warning stripes, and Indian rail livery. |
| `SM_Obstacles_LowBarrier_001.obj/.mtl` | Jumpable low obstacle | Flat barrier box; no readable railway context. | `SM_Obstacle_PlatformBarrier_Low.glb`: platform barrier with painted hazard bands, feet, and readable low silhouette. |
| `SM_Obstacles_HighBarrier_001.obj/.mtl` | Slide-under obstacle | Flat barrier box; no readable railway context. | `SM_Obstacle_LuggageStack_High.glb`: stacked luggage/parcel obstacle with supporting posts, readable overhead clearance, and cloth/metal variation. |
| `SM_Collectibles_Coin_001.obj/.mtl` | Coin collectible | Flat cylinder; no rim, emboss, sparkle, or ₹ identity. | `SM_Collectible_Coin_Polished.glb`: double-sided coin with rim, embossed rupee mark, metallic material, and highlight ring. |
| `SM_PowerUps_Magnet_001.obj/.mtl` | Magnet power-up | Primitive shape; no iconography. | `SM_Powerup_Magnet.glb`: horseshoe magnet with metallic poles and glow accent. |
| `SM_PowerUps_Shield_001.obj/.mtl` | Shield power-up | Primitive shape; no iconography. | `SM_Powerup_Shield.glb`: police shield badge with rim and blue enamel finish. |
| `SM_PowerUps_Multiplier_001.obj/.mtl` | Boost power-up | Primitive shape; no iconography. | `SM_Powerup_Boost.glb`: stylised sprint/boost marker with arrow motif and warm metallic accents. |
| `SM_Environment_Platform_001.obj/.mtl` | Copied but unused | Primitive platform box; never loaded by Godot. | `SM_Platform_Kit.glb`: modular platform tiles, edge stripe, columns, canopy beams, benches, fans/lamps, signs, and rails. |
| `SM_Environment_Station_001.obj/.mtl` | Copied but unused | Primitive station box; never loaded by Godot. | `SM_Station_Kit.glb`: station facade with arches, windows, signage, roof, pillars, and clock/signal details. |

## Placeholder models and code-generated visuals

| Location | Placeholder | Replacement |
| --- | --- | --- |
| `godot/scripts/main.gd:150-232` `_build_world()` | Runtime `BoxMesh` floor, rails, station blocks, and lamp blocks repeated per segment. | `SM_Track_Kit.glb`, `SM_Platform_Kit.glb`, `SM_Station_Kit.glb`, and `SM_Props_RailwayKit.glb` instances with authored materials and collider-matched bounds. |
| `godot/scripts/main.gd:169-172` | Flat-coloured `StandardMaterial3D` materials for track, rails, station, lamps. | PBR-style material set with roughness/metallic variation, tiled textures, decals, and accent colours. |
| `godot/scripts/main.gd:197-207` | Character root nodes using scaled OBJ meshes. | GLB character instances with imported skeletons/animations and animation state control. |
| `godot/scripts/main.gd:209-213` | Static chase camera with `look_at()`. | Reframed chase camera with smoother tracking, slight pitch, readable lane composition, and chapter-dependent lighting. |
| `godot/scripts/main.gd:319-325` `_make_box()` | Generic box factory used for most environment geometry. | Retained only as fallback; primary visuals use designed GLB kits. |
| `godot/scripts/main.gd:327-345` `_build_character()` | OBJ mesh with flat `material_override`. | GLB `PackedScene` instance helper with animation playback and fallback only if import fails. |
| `godot/scripts/main.gd:545-582` `_spawn_obstacle()` | OBJ or box obstacle with flat colour override. | Designed obstacle GLB instances; code collision dimensions updated to match new visuals. |
| `godot/scripts/main.gd:584-608` `_spawn_coin()` | OBJ cylinder or generated cylinder with flat gold. | Polished coin GLB with metallic material and highlight. |
| `godot/scripts/main.gd:614-638` `_spawn_powerup()` | OBJ shape or generated sphere with flat colour. | Distinct designed power-up GLBs. |
| `godot/scripts/main.gd:234-310` `_build_hud()`/`_make_label()` | Unstyled `Label`/`Button` stack and plain `PanelContainer`. | Themed HUD with top status bar, chapter card, score/coin/distance chips, chase-gap meter, power-up badges, stylised case-file panel, accessible contrast, and touch-sized buttons. |

## Missing models and visual systems

- No `.blend`, `.blend1`, FBX, GLB, glTF, DAE, rigged character, animation clip, texture image, shader, saved material asset, LOD, or authored collision mesh exists.
- No detailed Indian railway environment exists: sleepers, ballast, track bed variation, signalling, overhead equipment wires/poles, platform edge stripe, station canopy, benches, luggage, chai stalls, signboards, or contextual crowd props are absent.
- No designed character identity or animation exists for the player or pursuer.
- No detailed train exists; current trains are boxes.
- No polished collectibles/power-ups exist.
- No authored texture set or material library exists; all materials are flat runtime colours.
- No authored UI theme, fonts, icon set, panel style, HUD hierarchy, or mobile-readable typography exists.
- No collision geometries matched to designed models exist; only code-based lane/z bounds and jump/slide checks currently define gameplay collisions.
- No automated Android gameplay frame capture or visual acceptance gate exists.

## Proposed replacement set

| Asset | Proposed file | Priority | Notes |
| --- | --- | --- | --- |
| Animated police runner | `godot/assets/models/SM_Character_PoliceRunner_Animated.glb` | High | Named animation clips: `idle`, `run`, `jump`, `slide`; target <= 4k triangles. |
| Animated suspect/pursuer | `godot/assets/models/SM_Character_Suspect_Animated.glb` | High | Named animation clips: `idle`, `run`; target <= 4k triangles. |
| Detailed express coach | `godot/assets/models/SM_Train_Express_Detailed.glb` | High | Windows, doors, roof ribs, wheels, buffers, livery; target <= 8k triangles. |
| Low platform barrier | `godot/assets/models/SM_Obstacle_PlatformBarrier_Low.glb` | High | Clear jumpable silhouette; target <= 1.5k triangles. |
| High luggage obstacle | `godot/assets/models/SM_Obstacle_LuggageStack_High.glb` | High | Clear slide-under gap; target <= 2k triangles. |
| Polished coin | `godot/assets/models/SM_Collectible_Coin_Polished.glb` | Medium | Metallic rim and rupee motif; target <= 700 triangles. |
| Shield/magnet/boost icons | `SM_Powerup_Shield.glb`, `SM_Powerup_Magnet.glb`, `SM_Powerup_Boost.glb` | Medium | Distinct silhouettes and colours; target <= 800 triangles each. |
| Track kit | `godot/assets/models/SM_Track_Kit.glb` | High | Ballast, sleepers, rails, rail chairs, signals, OHE poles/wires; target <= 12k triangles per segment. |
| Platform kit | `godot/assets/models/SM_Platform_Kit.glb` | High | Tiles, edge stripe, columns, canopy, benches, lamps, signage; target <= 10k triangles per segment. |
| Station kit | `godot/assets/models/SM_Station_Kit.glb` | Medium | Facade, arches, windows, roof, clock, signboard; target <= 10k triangles per segment. |
| Railway props kit | `godot/assets/models/SM_Props_RailwayKit.glb` | Medium | Chai stall, luggage, crates, posters, trash bin, water tank, signals; target <= 8k triangles per kit. |
| UI theme and icons | Code-generated Godot theme plus embedded textures | High | Mobile-first HUD, case-file dialogue, readable score/chase/coin/power states. |

## Implementation plan

1. Write a deterministic GLB generator because Blender is unavailable in this environment; keep the generator in the repo and make it reproducible.
2. Generate the replacement GLBs into `godot/assets/models/`, with embedded procedural textures/materials and named animation clips where applicable.
3. Update `godot/scripts/main.gd` to instance GLB scenes instead of OBJ meshes, preserving gameplay signals, lane logic, collision rules, chapter progression, collectibles, and power-ups.
4. Replace flat environment boxes with modular GLB kits and update collision bounds to match the designed models.
5. Add a themed HUD/case-file overlay and improved camera/lighting/atmosphere.
6. Add an opt-in gameplay frame capture harness and CI workflow, then inspect actual rendered frames before any visual status is called `Verified`.
7. Rebuild the Android debug APK after visual changes and report truthful status.
