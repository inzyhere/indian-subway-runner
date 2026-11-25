# Godot project

This is the active implementation project for **Indian Subway Runner**. It is a standalone Godot 4 project rooted at `godot/`.

The existing Unity project in `Assets/`, `Packages/`, and `ProjectSettings/` is preserved untouched as history. Do not edit or delete it while working on this project.

## Play

- Desktop: `A`/`D` or arrow keys change lane, `Space` jumps, `Shift` slides, `R` restarts.
- Touch/mouse: swipe or drag left/right to change lane, up to jump, down to slide; tap to start or restart.

## Active files

- `project.godot`: Godot project configuration.
- `main.tscn`: entry scene.
- `scripts/main.gd`: prototype gameplay, procedural track, HUD, camera, and audio.
- `export_presets.cfg`: Android debug APK export preset.
- `assets/models/`: 13 authored textured/animated GLB assets plus original OBJ/MTL fallback assets.
- `assets/audio/`: original WAV music and SFX copied from the preserved Unity project.
- `assets/indian_pack_v01/`: optional original starter pack import (GLB props, environment blocks, and temporary voice guides); import it with `python3 Tools/Assets/import_starter_asset_pack.py /path/to/IndianSubwayRunner_AssetPack_v0.1.zip`.
- `scripts/indian_asset_pack_runtime.gd` and `scripts/voice_pack_runtime.gd`: optional prop/environment and voice hooks; missing pack files are safely skipped.

Optional gameplay capture:

```sh
godot --path . --resolution 720x1280 -- --capture-gameplay --capture-dir=/path/to/captures
```

See [docs/visual-asset-inventory.md](../docs/visual-asset-inventory.md) for the shipped visual set and [docs/godot-android-build.md](../docs/godot-android-build.md) for the secret-free Android APK workflow.
