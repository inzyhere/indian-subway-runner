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
- `assets/`: engine-neutral OBJ/MTL and WAV assets copied from the preserved Unity project.

See [docs/godot-android-build.md](../docs/godot-android-build.md) for the secret-free Android APK workflow.
