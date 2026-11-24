# Blender Workflow

> Status: In progress. Blender 4.5.14 LTS generated OBJ/MTL models; Unity import, FBX/GLB export, rigs, and prefabs are not verified.

## Purpose

Blender is the source of truth for all original 3D model assets. Unity consumes validated FBX/GLB or OBJ/MTL and texture files; Unity should not be the place where mesh authoring begins.

## Tool requirement

- Current shell: Blender 4.5.14 LTS at `blender-4.5`; desktop Blender LTS may be used for FBX/GLB.
- Python scripting enabled for repeatable generation and export checks.
- Unity importer validation required after every FBX/GLB or OBJ/MTL export.

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

`Tools/Blender/Scripts/generate_assets.py` is tested in this shell and generates OBJ/MTL models. Do not label scripts, imports, or prefabs Verified until Unity and desktop export evidence exists.

Planned script responsibilities:

- Validate unit scale and object naming.
- Check missing textures and duplicate materials.
- Export selected collections with consistent settings.
- Generate a JSON manifest for asset provenance.

## Current limitation

This Android Blender build has a broken FBX export operator, so the tested generator writes OBJ/MTL instead. No `.blend`, FBX, GLB, rig, animation, or texture is claimed here. If a temporary primitive is used later for logic testing, it must be labelled temporary and replaced before release.
