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
| Godot project foundation | Tested — headless smoke passed | `godot/project.godot`, `godot/main.tscn`, GDScript gameplay, HUD, camera, audio glue, and Android export preset load without script/engine errors in runs `45518280174` and `45519025297`. |
| Godot gameplay prototype | Tested — smoke and capture passed | Three-lane movement, jump, slide, safe-path low/high barriers, trains, coin trails, shield/magnet/boost power-ups, chapter banners, story intro, score, chase gap, increasing speed, procedural track, near-capture restart, and touch/keyboard input execute without script/engine errors; five gameplay frames were captured in run `45518284923`. Device input is not verified. |
| Godot assets | Implemented — import/capture passed | Thirteen original procedural GLBs cover animated characters, train, barriers, coin, power-ups, track/platform/station/props kits; structural validation passed and Godot import/capture succeeded. Human aesthetic sign-off is pending. Original WAV audio remains load-smoke tested only. |
| Hindi dialogue and announcements | Implemented — text only | Original case-file dialogue, chapter hints, and railway-announcement-style text exist in code; generated music/SFX play through the audio glue. No voice pack or recorded announcement clip exists. |
| Android build | Tested — export only | Run `45519025297` published `1.0.0` / code `5` with authored GLB visuals, portrait immersive settings, and a clean headless smoke gate; checksum and package metadata were verified locally. Device retest is pending. |
| GitHub remote | Implemented — public | `inzyhere/indian-subway-runner` is public after owner approval; `main` tracks `origin/main`. |

## What is verified

- Repository files, Markdown, Mermaid, YAML, GDScript text, JSON, XML, shell commands, and folder structure can be statically checked.
- No Unity license, Unity credential, repository secret, release keystore, or signing password is required by the active Godot workflows.
- The GitHub Release delivery workflow uses only the automatic `GITHUB_TOKEN` with `contents: write`.
- No unauthorized third-party asset is committed.
- `Tools/Blender/Scripts/generate_assets.py` previously generated 11 original OBJ/MTL model pairs, and copies remain under `godot/assets/models/` as fallback assets.
- `python3 Tools/Blender/Scripts/generate_visual_glbs.py` generated 13 original textured/animated GLB assets under `godot/assets/models/`; structural validation passed for JSON/BIN chunks, accessors, buffers, materials, textures, animation channels, and bounds.
- `python3 Tools/Audio/generate_audio.py` previously generated one original music loop and seven original SFX WAV files, and copies now exist under `godot/assets/audio/`.
- Headless smoke runs `45518280174` and `45519025297` succeeded with Godot `4.7.2-stable`; their logs contain no script errors, parse errors, or engine errors.
- Gameplay capture run `45518284923` succeeded and produced five 720x1280 RGBA frames; pixel analysis found nonblank frames (about 19k unique colours each) and frame-to-frame motion (mean absolute diff 11.76-16.63, 26.2-37.4% pixels changed by more than 8).
- The `0.1.2` blank-screen failure was traced to GDScript parse errors from untyped `min()`/`max()`/array inference, invalid `MeshInstance3D.material` assignments, and `look_at()` before the camera was in the scene tree; all are fixed.
- The `0.1.3` APK was exported successfully but visually rejected by the owner; the authored GLB visual pass replaced the flat placeholder look.
- Release run `45519025297` succeeded from commit `51f76eb`; release tag `godot-debug-apk` contains the `1.0.0` APK and checksum assets, and its release gate executed the clean headless smoke test.
- The current APK SHA-256 matches the published checksum: `9db172007c8a9a25f1bfdb4548804e1bfd01de3c66a0d15cd751f0aedccd6bc7`.
- `aapt dump badging` on the current APK reports package `com.inzyhere.indiansubwayrunner`, version `1.0.0` / code `5`, and `arm64-v8a` native code.
- The published APK label is `Indian Subway Runner`; the release asset is displayed as `IndianSubwayRunner-debug.apk` with file name `IndianSubwayRunner.apk`.
- Runs `45508181618` (`0.1.1`), `45510290459` (`0.1.2`), and `45512629961` (`0.1.3`) remain as historical evidence; the current release is `1.0.0` / code `5`.
- The owner reported that the `0.1.2` APK showed the Godot splash screen followed by a blank screen; it was not playable.
- Static inspection of the expanded `godot/scripts/main.gd` found balanced brackets/braces and 35 existing `res://` asset references.

## What is not verified

- Real-device rendering, input handling, audible audio playback, immersive fullscreen behaviour, frame rate, battery/thermal behaviour, or gameplay after the smoke/capture/export passes. Those results do not prove device behaviour.
- Human visual sign-off of the authored GLB look; the five captured frames were programmatically checked for nonblank content and motion, but no person or vision-capable reviewer has judged aesthetics.
- Successful device replacement of the old staged `/sdcard/Download/IndianSubwayRunner-debug.apk`, which still holds rejected `0.1.3`.
- Save format, contextual voice packs, monetization, telemetry, and cloud save; all are future work.

## Next milestone

1. Static-check the new GDScript, project files, export preset, and workflow syntax. **Done.**
2. Commit and push the Godot project, documentation, and workflow. **Done in `f981952`.**
3. Add an artifact-independent GitHub Release workflow and switch the repository to public after owner approval. **Done in this change.**
4. Run `.github/workflows/godot-android-release.yml` and verify the Release APK and SHA-256 checksum. **Done initially in `45505392990`; final corrected build in `45508181618`.**
5. Fix the undeclared `_hud` parse error, switch to portrait immersive fullscreen, and bump the version to `0.1.1` / code `2`. **Done in `8d9c3c7` and `fb277a0`.**
6. Rebuild and publish the corrected APK and verify its checksum/package metadata. **Done in `45508181618`.**
7. Reinstall it, capture logcat, and verify input/rendering/audio/gameplay.
8. Expand the prototype into chapter flow, story intro, richer railway environments, trains, power-ups, and progressive difficulty. **Done in `a620f52` as `0.1.2`.**
9. Build and publish `0.1.2` / code `3`, verify checksum/package metadata, and overwrite the staged APK. **Done in `45510290459`.**
10. Reinstall the staged `0.1.2` APK, capture logcat, and verify input/rendering/audio/gameplay on a real device. **Superseded by the blank-screen diagnosis below.**
11. Add save format, voice-pack, monetization, and telemetry only through future ADRs.
12. Add a headless Godot smoke workflow and a release gate that fails on script/engine errors. **Done in `291d035`, `9dccffb`, `e8b018b`, `846a065`; clean run `45512471537`.**
13. Fix GDScript inference errors, Godot 4 material overrides, and camera `look_at()` tree order; bump to `0.1.3` / code `4`. **Done in `e8b018b` and `ec72c23` and `846a065`.**
14. Rebuild and publish `0.1.3` / code `4`, verify checksum/package metadata, and overwrite the staged APK. **Done in `45512629961`.**
15. Reinstall the staged `0.1.3` APK, capture logcat, and verify input/rendering/audio/gameplay on a real device. **Superseded by the 1.0.0 visual pass.**
16. Audit primitive visuals, write `Tools/Blender/Scripts/generate_visual_glbs.py`, generate 13 original GLB assets, and validate GLB structure. **Done in `be5f6d7` and `840c52c`.**
17. Integrate GLB models, animation state, environment kits, HUD restyle, and opt-in gameplay frame capture into `godot/scripts/main.gd`; add `.github/workflows/godot-gameplay-capture.yml`. **Done in `840c52c` and `f252e6a`; capture run `45518284923`.**
18. Bump to `1.0.0` / code `5`, rebuild, and publish the visual-pass APK with a clean release smoke gate. **Done in `51f76eb` and `45519025297`.**
19. Replace the old staged `0.1.3` APK, inspect the new visuals and gameplay on a real device, capture logcat, and give visual sign-off.

## Blockers requiring owner or environment action

- A connected Android device is required for runtime, performance, input, and audio verification.
- Human visual review of the captured frames or the installed `1.0.0` APK is required before the visual quality can be marked `Verified`.
