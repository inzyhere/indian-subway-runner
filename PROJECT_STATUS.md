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
| Android build | Blocked | `.github/workflows/godot-android.yml` provides a secret-free debug APK workflow. Run `45500464511` failed before any step because GitHub reports failed account payments or a spending-limit issue; no APK artifact, install, or device verification exists yet. |
| GitHub remote | Implemented | Private `inzyhere/indian-subway-runner`; `main` tracks `origin/main`. |

## What is verified

- Repository files, Markdown, Mermaid, YAML, GDScript text, JSON, XML, shell commands, and folder structure can be statically checked.
- No Unity license, Unity credential, GitHub secret, release keystore, or signing password is required by the active Godot workflow.
- No unauthorized third-party asset is committed.
- `Tools/Blender/Scripts/generate_assets.py` previously generated 11 original OBJ/MTL model pairs, and copies now exist under `godot/assets/models/`.
- `python3 Tools/Audio/generate_audio.py` previously generated one original music loop and seven original SFX WAV files, and copies now exist under `godot/assets/audio/`.

## What is not verified

- Godot project import, resource import, GDScript execution, scene loading, rendering, input handling, or audio playback.
- GitHub Actions export success or APK artifact contents.
- APK installation, runtime behaviour, rendering, frame rate, battery/thermal behaviour, or touch input on a real Android device.
- Save format, contextual voice packs, monetization, telemetry, and cloud save; all are future work.

## Next milestone

1. Static-check the new GDScript, project files, export preset, and workflow syntax. **Done.**
2. Commit and push the Godot project, documentation, and workflow. **Done in `f981952`.**
3. Resolve the GitHub Actions billing/spending-limit blocker, then rerun `.github/workflows/godot-android.yml` and verify the uploaded debug APK artifact.
4. Install the APK on a real Android device, capture logcat, and verify input/rendering/audio/gameplay.
5. Expand the prototype into chapter flow, story intro, richer railway environments, trains, power-ups, and progressive difficulty.
6. Add save format, voice-pack, monetization, and telemetry only through future ADRs.

## Blockers requiring owner or environment action

- A connected Android device is required for runtime, performance, input, and audio verification.
- Godot is not installed in this shell, so local headless import/export cannot be performed here.
- `sdkmanager` is unavailable locally, so local Android SDK preparation cannot be performed here.
