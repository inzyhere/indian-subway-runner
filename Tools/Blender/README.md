# Blender Tooling

Original asset generation scripts and provenance. `Scripts/generate_assets.py` generated 11 original OBJ/MTL model pairs; those files remain in the repository as fallback assets.

`Scripts/generate_visual_glbs.py` generated 13 original textured/animated GLB assets under `godot/assets/models/`. It is deterministic pure Python with embedded procedural PNG textures and does not require Blender. Structural validation passed for GLB chunks, accessors, buffers, materials, textures, animations, and bounds; Godot import, smoke, gameplay capture, and Android export passed in CI. Human visual sign-off and real-device verification are still pending. See `docs/visual-asset-inventory.md`.
