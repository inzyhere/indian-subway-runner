# Asset Production Plan

> Status: Planned; no final art asset is implemented.

## Principles

- All shipped assets are original or explicitly licensed for this project.
- No decompiled, ripped, scraped, or copied assets from commercial games.
- Blender is the 3D source of truth.
- Unity receives FBX/GLB models and PNG/JPEG or optimized textures.
- Temporary primitives are acceptable only for logic tests and must be clearly labelled and replaced.
- Asset names, scale, materials, and import settings follow `docs/blender-workflow.md`.

## Asset categories

| Category | Examples | Priority | Status |
| --- | --- | --- | --- |
| Characters | ACP pursuer, criminal, crowd variants | Critical | Planned |
| Trains | local, express, freight, coach modules | Critical | Planned |
| Environment | platforms, tracks, signals, footbridges, stalls | Critical | Planned |
| Obstacles | crates, carts, barriers, gaps, debris | Critical | Planned |
| Collectibles | coins, clue tokens, power-up icons | Critical | Planned |
| UI | icons, buttons, chapter cards, HUD frames | High | Planned |
| VFX | dust, speed lines, coin burst, shield effect | Medium | Planned |
| Audio | original score, SFX, announcements | High | Planned |

## Source-to-import mapping template

Each completed asset must be recorded in a table like this:

| Asset ID | Blender source | Export file | Unity prefab | Licence | Verified by |
| --- | --- | --- | --- | --- | --- |
| `CHAR_ACP_001` | planned | planned | planned | original project work | not yet |

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
