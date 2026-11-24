# Project Status

> Last updated: 2026-11-25  
> Status vocabulary: Planned, In progress, Implemented, Tested, Verified.

Indian Subway Runner is an original, offline-capable, 3D Android endless-chase game. The player controls an ACP Pradyuman-inspired pursuer chasing a criminal through Indian railway environments. The criminal is always ahead and is never permanently caught.

The active implementation stack is Godot 4 + GDScript under ADR-0004. The existing Unity project is preserved untouched as history and is not the active build target.

## Current status

| Area | Status | Truthful note |
| --- | --- | --- |
| Public gameplay research | In progress | `docs/research/observable-gameplay-analysis.md` records publicly observable principles. It remains engine-neutral and citation-based. |
| Architecture decision | Implemented | ADR-0004 in `docs/decisions/architecture-decisions.md` is the active binding decision: Godot 4 + GDScript + Android. ADR-0003 is superseded. |
| Environment assessment | Implemented | See `docs/environment-assessment.md`. No local Godot binary or Android SDK environment is available. |
| Preserved Unity project | Preserved | `Assets/`, `Packages/`, and `ProjectSettings/` are retained without modification or deletion. No Unity credentials are used. |
| Godot project foundation | Implemented — unverified | `godot/project.godot`, `godot/main.tscn`, GDScript gameplay, HUD, camera, audio glue, and Android export preset exist. Godot import has not run locally. |
| Godot gameplay prototype | Implemented — unverified | Three-lane movement, jump, slide, low/high barriers, coins, score, chase gap, increasing speed, procedural track, restart, and touch/keyboard input exist in `godot/scripts/main.gd`. No runtime evidence exists yet. |
| Godot assets | Implemented — unverified | Original OBJ/MTL models and WAV audio were copied into `godot/assets/` from the preserved Unity project. Godot import and playback are not verified. |
| Hindi dialogue and announcements | In progress | Generated music/SFX exist; no voice pack or announcement clip exists. |
| Android build | Tested — release artifact | `.github/workflows/godot-android-release.yml` published the debug APK and SHA-256 checksum in run `45505392990`; the downloaded checksum and `aapt` package metadata were verified. Device install/runtime remains unverified. |
| GitHub remote | Implemented — public | `inzyhere/indian-subway-runner` is public after owner approval; `main` tracks `origin/main`. |

## What is verified

- Repository files, Markdown, Mermaid, YAML, GDScript text, JSON, XML, shell commands, and folder structure can be statically checked.
- No Unity license, Unity credential, repository secret, release keystore, or signing password is required by the active Godot workflows.
- The GitHub Release delivery workflow uses only the automatic `GITHUB_TOKEN` with `contents: write`.
- No unauthorized third-party asset is committed.
- `Tools/Blender/Scripts/generate_assets.py` previously generated 11 original OBJ/MTL model pairs, and copies now exist under `godot/assets/models/`.
- `python3 Tools/Audio/generate_audio.py` previously generated one original music loop and seven original SFX WAV files, and copies now exist under `godot/assets/audio/`.
- Release run `45505392990` succeeded after the keystore variable fix in `8c81824`; release tag `godot-debug-apk` contains the APK and checksum assets.
- The downloaded APK SHA-256 matches the published checksum: `884a86a3dca7240cdab94e80af0ab67d254a4de6926079eaf63c0c6736b3da3e`.
- `aapt dump badging` reports package `com.inzyhere.indiansubwayrunner`, version `0.1.0` / code `1`, debuggable application, landscape/faketouch requirements, and `arm64-v8a` native code.

## What is not verified

- Godot project import, resource import, GDScript execution, scene loading, rendering, input handling, or audio playback.
- APK installation, runtime behaviour, GDScript execution, rendering, audio playback, frame rate, battery/thermal behaviour, or touch input on a real Android device.
- Save format, contextual voice packs, monetization, telemetry, and cloud save; all are future work.

## Next milestone

1. Static-check the new GDScript, project files, export preset, and workflow syntax. **Done.**
2. Commit and push the Godot project, documentation, and workflow. **Done in `f981952`.**
3. Add an artifact-independent GitHub Release workflow and switch the repository to public after owner approval. **Done in this change.**
4. Run `.github/workflows/godot-android-release.yml` and verify the Release APK and SHA-256 checksum. **Done in `8c81824` via run `45505392990`.**
5. Install the APK on a real Android device, capture logcat, and verify input/rendering/audio/gameplay.
6. Expand the prototype into chapter flow, story intro, richer railway environments, trains, power-ups, and progressive difficulty.
7. Add save format, voice-pack, monetization, and telemetry only through future ADRs.

## Blockers requiring owner or environment action

- A connected Android device is required for runtime, performance, input, and audio verification.
