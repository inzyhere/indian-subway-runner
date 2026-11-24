# Blender Workflow

> Status: Planned. Blender is unavailable in the current shell; no model file is claimed.

## Purpose

Blender is the source of truth for all original 3D model assets. Unity consumes exported FBX/GLB and texture files; Unity should not be the place where mesh authoring begins.

## Tool requirement

- Blender LTS version to be pinned after installation.
- Python scripting enabled for repeatable export checks.
- Unity FBX/GLB importer validation required after every export.

## Folder layout

```text
Tools/Blender/
├─ Source/
│  ├─ Characters/
│  ├─ Trains/
│  ├─ Stations/
│  ├─ Props/
│  └─ Environments/
├─ Scripts/
│  ├─ export_fbx.py
│  ├─ export_glb.py
│  ├─ validate_scene.py
│  └─ naming_conventions.md
└─ Exports/
   ├─ Characters/
   ├─ Trains/
   ├─ Stations/
   ├─ Props/
   └─ Environments/
```

## Naming conventions

- Source: `ISR_<category>_<name>_<variant>.blend`
- Mesh: `SM_<category>_<name>_<variant>`
- Rig: `RIG_<category>_<name>`
- Animation: `AN_<character>_<action>`
- Material: `M_<surface>_<variant>`
- Texture: `T_<asset>_<map>` where map is `BC`, `N`, `RMA`, or `E`.

## Scale and axes

- 1 Blender unit = 1 metre.
- Character roots at feet.
- Props pivot at logical placement point.
- Export with Unity-compatible forward/up conversion verified in a test scene.
- Apply transforms before export unless rig constraints require otherwise.
- Recalculate normals and remove unused materials before export.

## Export checklist

1. Source `.blend` saved under `Tools/Blender/Source/**`.
2. Mesh, rig, and animation naming validated.
3. UVs present and non-overlapping where needed.
4. Texture maps exported under `Assets/Art/Textures/**`.
5. FBX or GLB exported to the matching `Assets/Art/**` folder.
6. Unity import settings checked: scale, normals, materials, animation clips, rig type.
7. Prefab created under `Assets/Prefabs/**`.
8. Collider complexity reduced to gameplay needs.
9. LODs configured where required.
10. `docs/asset-production-plan.md` updated with source-to-import mapping.

## Python helper scripts

`Tools/Blender/Scripts/` will contain scripts only after Blender is available and the scripts can be tested. Do not commit untested scripts as Verified automation.

Planned script responsibilities:

- Validate unit scale and object naming.
- Check missing textures and duplicate materials.
- Export selected collections with consistent settings.
- Generate a JSON manifest for asset provenance.

## Current limitation

This environment reports `blender NOT_FOUND`. No `.blend`, FBX, GLB, rig, animation, or texture can be produced here. If a temporary primitive is used later for logic testing, it must be labelled temporary and replaced before release.
