# Asset Production Plan

> Status: In progress; generated original OBJ/MTL models and WAV audio exist, but Unity import, prefabs, and in-game evidence do not.

## Principles

- All shipped assets are original or explicitly licensed for this project.
- No decompiled, ripped, scraped, or copied assets from commercial games.
- Blender is the 3D source of truth.
- Desktop Blender should export FBX/GLB models; the current Android Blender 4.5.14 build has a broken FBX operator, so generated assets currently use OBJ/MTL.
- Unity receives validated FBX/GLB or OBJ/MTL models and PNG/JPEG or optimized textures.
- Temporary primitives are acceptable only for logic tests and must be clearly labelled and replaced.
- Asset names, scale, materials, and import settings follow `docs/blender-workflow.md`.

## Asset categories

| Category | Examples | Priority | Status |
| --- | --- | --- | --- |
| Characters | ACP pursuer, criminal, crowd variants | Critical | In progress — generated OBJ/MTL; rig/Unity import pending |
| Trains | local, express, freight, coach modules | Critical | In progress — express OBJ/MTL generated; Unity import pending |
| Environment | platforms, tracks, signals, footbridges, stalls | Critical | In progress — platform/station OBJ/MTL generated |
| Obstacles | crates, carts, barriers, gaps, debris | Critical | In progress — low/high barrier OBJ/MTL generated |
| Collectibles | coins, clue tokens, power-up icons | Critical | In progress — coin/shield/magnet/multiplier OBJ/MTL generated |
| UI | icons, buttons, chapter cards, HUD frames | High | Planned |
| VFX | dust, speed lines, coin burst, shield effect | Medium | Planned |
| Audio | original score, SFX, announcements | High | In progress — original music/SFX WAVs generated; announcements and Unity import pending |

## Source-to-import mapping template

Each completed asset must be recorded in a table like this:

| Asset ID | Blender source | Export file | Unity prefab | Licence | Verified by |
| --- | --- | --- | --- | --- | --- |
| `SM_Characters_Runner_001` | `Tools/Blender/Scripts/generate_assets.py` | `Assets/Resources/Art/Models/SM_Characters_Runner_001.obj` + `.mtl` | not yet | original project work | not yet |
| `SM_Characters_Pursuer_001` | `Tools/Blender/Scripts/generate_assets.py` | `Assets/Resources/Art/Models/SM_Characters_Pursuer_001.obj` + `.mtl` | not yet | original project work | not yet |
| `SM_Trains_Express_001` | `Tools/Blender/Scripts/generate_assets.py` | `Assets/Resources/Art/Models/SM_Trains_Express_001.obj` + `.mtl` | not yet | original project work | not yet |
| `SM_Obstacles_LowBarrier_001`, `SM_Obstacles_HighBarrier_001` | `Tools/Blender/Scripts/generate_assets.py` | matching OBJ/MTL under `Assets/Resources/Art/Models/` | not yet | original project work | not yet |
| `SM_Collectibles_Coin_001`, `SM_PowerUps_Shield_001`, `SM_PowerUps_Magnet_001`, `SM_PowerUps_Multiplier_001` | `Tools/Blender/Scripts/generate_assets.py` | matching OBJ/MTL under `Assets/Resources/Art/Models/` | not yet | original project work | not yet |
| `SM_Environment_Platform_001`, `SM_Environment_Station_001` | `Tools/Blender/Scripts/generate_assets.py` | matching OBJ/MTL under `Assets/Resources/Art/Models/` | not yet | original project work | not yet |
| `ISR_Loop_Main_001` and `ISR_SFX_*_001` | `Tools/Audio/generate_audio.py` | WAV files under `Assets/Resources/Audio/` | not applicable | original project work | not yet |

No asset may be marked Verified until the source, export, Unity prefab, and in-game capture are recorded.

## Model budget targets

Targets to validate with Unity Profiler, not claims of current compliance:

- Character gameplay mesh: low-poly stylized; exact triangle budget set after first Profiler pass.
- Crowd variants: shared materials, LODs, minimal rig complexity.
- Train coaches: modular, pooled, reduced collider count.
- Station chunks: reuse modular props and materials.
- Textures: compressed mobile formats; atlas UI and small props.
- Materials: URP/Lit or URP/Unlit where appropriate; avoid expensive custom shaders unless measured.

## Texture policy

- Store source textures outside Unity or under clearly marked source folders.
- Runtime textures use compressed formats appropriate to target devices.
- Avoid overly large texture dimensions; validate visually on device.
- All signage and UI text must be original.
- Do not use screenshots or ripped images from protected games.

## Audio policy

- Music and SFX must be original or licensed with written permission recorded in the repository.
- No scraped meme audio.
- No celebrity voice cloning.
- No copied railway announcements from protected media.
- Dialogue and announcements use `docs/story-and-narrative-design.md` as the source of tone.

## Placeholder policy

Temporary primitive objects may be used only to validate movement, collision, and spawning. Each temporary object must:

- live in a clearly named temporary prefab folder;
- be excluded from release builds;
- be listed in `PROJECT_STATUS.md`;
- be replaced by original Blender-authored art before release.

## Verification evidence required

For every asset group:

1. Blender source file path.
2. Export file path and digest.
3. Unity import screenshot or inspector note.
4. Prefab path.
5. In-game scene capture.
6. Profiler impact note.
7. Licence/provenance statement.

Until these exist, status remains Planned or In progress.
