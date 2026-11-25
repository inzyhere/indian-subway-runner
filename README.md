# Indian Subway Runner

> **Last updated:** 2026-11-25  
> **Status vocabulary:** Planned / In progress / Implemented / Tested / Verified.  
> **Overall position:** the expanded Godot 4 gameplay with story chapters, trains, power-ups, progressive difficulty, and 13 authored animated/textured GLB assets passes clean headless smoke, gameplay capture, and Android export/release; device retest and human visual sign-off are still pending.

## Vision

**INDIAN SUBWAY RUNNER** is an original, offline-capable, genuine 3D Android endless-chase game. An ACP Pradyuman-inspired pursuer chases a criminal through Indian railway stations and trains. The criminal is always ahead and is **never permanently caught in v1**; chapters, clues, records, and near-captures carry the story forward.

The player runs through three lanes with swipe left/right, jump, and slide. Runs include trains, obstacles, coins, power-ups, score and distance, increasing difficulty, procedural chunks, game over/restart, a story intro with chapters, contextual Hindi dialogue, and railway announcements. Identity comes from original Indian environments, humour, dialogue, trains, and station life.

## Active technology decision

[ADR-0004](docs/decisions/architecture-decisions.md) is the active, binding decision. The owner changed the implementation stack to Godot 4 + GDScript.

| Concern | Active choice |
| --- | --- |
| Engine / language | Godot 4, GDScript |
| Pinned CI version | Godot `4.7.2-stable` |
| Active project root | `godot/` |
| Platform | Android debug APK for side-load verification |
| Rendering | Godot 3D with compatibility-first settings |
| 3D assets | Original procedural GLB kits/models generated in `Tools/Blender/Scripts/generate_visual_glbs.py`, with OBJ fallback |
| Audio | Original WAV music and SFX copied from the preserved Unity project where useful |
| CI | GitHub Actions, secret-free; GitHub Release delivery |
| Signing | CI-generated Android debug keystore |

The existing Unity project in `Assets/`, `Packages/`, and `ProjectSettings/` is preserved untouched as history. ADR-0003 is superseded for new work.

No Unity credential, Unity license, Unity serial, Unity password, GitHub secret, release key, or unnecessary secret is required or permitted for the active Godot path.

**Prohibited for active gameplay:** modifying or deleting the preserved Unity project, requesting or storing Unity credentials, and using copyrighted Subway Surfers assets, decompiled output, scraped audio, celebrity voice clones, or protected game content.

## Current status

| Area | Status | Truthful note |
| --- | --- | --- |
| Public gameplay research | In progress | Engine-neutral, citation-based record in [docs/research/observable-gameplay-analysis.md](docs/research/observable-gameplay-analysis.md). |
| Architecture decision | Implemented | ADR-0004 is active; ADR-0001 through ADR-0003 are historical. |
| Environment assessment | Implemented | [docs/environment-assessment.md](docs/environment-assessment.md) records the current shell. |
| Preserved Unity project | Preserved | Unity files remain untouched and are not the active build target. |
| Godot project foundation | Tested — headless smoke passed | `godot/project.godot`, `main.tscn`, GDScript gameplay, HUD, camera, audio glue, and export preset load without script/engine errors in runs `45518280174` and `45519025297`. |
| Godot gameplay prototype | Tested — smoke and capture passed | Lane movement, jump, slide, safe-path barrier waves, trains, coins, power-ups, chapters, story intro, score, chase gap, procedural track, near-capture restart, and touch/keyboard input execute without script/engine errors; five 720x1280 gameplay frames were captured in run `45518284923`. Device input is not verified. |
| Original 3D art | Implemented — import/capture passed | Thirteen original procedural GLBs cover characters, train, barriers, coin, power-ups, track/platform/station/props kits; structural validation passed and GLB import/capture succeeded. Human aesthetic sign-off is pending. |
| Original music and SFX | Implemented — load smoke passed | Original WAV files were copied into `godot/assets/audio/` and loaded without runtime errors; audible playback is not verified. |
| Android build / device verification | Tested — export only | Run `45519025297` exported and published `1.0.0` / code `5` with a clean headless smoke gate; package metadata and checksum were verified locally. Device retest is pending. |
| GitHub remote | Implemented — public | `inzyhere/indian-subway-runner` is public after owner approval; `main` tracks `origin/main`. |

The headless smoke, gameplay capture, and Android export/release paths are **Tested**, but the `1.0.0` APK has not been retested on the device and the owner has not yet given visual sign-off; no gameplay or device behaviour is **Verified**. Full detail lives in [PROJECT_STATUS.md](PROJECT_STATUS.md).

## Environment blocker summary

This shell is `aarch64` Termux. It has Git and `gh` installed, Android tools such as `aapt`/`aapt2`/`zipalign`, Java, and `adb`; it has **no local Godot binary, no `sdkmanager`, and no connected Android device**. `ANDROID_HOME` and `ANDROID_SDK_ROOT` are unset. GDScript, project files, YAML, Markdown, and directory structure can be authored and statically checked here; Godot import, export, and device runs cannot.

The current debug APK is at [godot-debug-apk](https://github.com/inzyhere/indian-subway-runner/releases/tag/godot-debug-apk) with SHA-256 `9db172007c8a9a25f1bfdb4548804e1bfd01de3c66a0d15cd751f0aedccd6bc7`; it is version `1.0.0` / code `5`, portrait, immersive fullscreen, ARM64, and includes the clean-room story, chapters, trains, power-ups, progressive difficulty, and authored GLB visuals. The older staged path `/sdcard/Download/IndianSubwayRunner-debug.apk` still holds rejected `0.1.3` and must be replaced before device retest. The `0.1.2` blank-screen failure was traced to GDScript parse errors, invalid `MeshInstance3D.material` assignments, and `look_at()` before the camera was in the scene tree; all are fixed. The `1.0.0` APK has not been retested on the device. No payment method or spending limit was changed. The debug APK from the abandoned Unity stack is **not** the game.

## Quick document map

| Area | Document |
| --- | --- |
| Authoritative decision | [docs/decisions/architecture-decisions.md](docs/decisions/architecture-decisions.md) |
| Environment truth | [docs/environment-assessment.md](docs/environment-assessment.md) |
| Overall status | [PROJECT_STATUS.md](PROJECT_STATUS.md) |
| Active project | [godot/README.md](godot/README.md) |
| Godot Android build | [docs/godot-android-build.md](docs/godot-android-build.md) |
| Visual asset inventory | [docs/visual-asset-inventory.md](docs/visual-asset-inventory.md) |
| System architecture | [ARCHITECTURE.md](ARCHITECTURE.md) |
| Phased plan | [ROADMAP.md](ROADMAP.md) |
| Development workflow | [DEVELOPMENT.md](DEVELOPMENT.md) |
| Testing policy | [TESTING.md](TESTING.md) |
| Contributing | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Security | [SECURITY.md](SECURITY.md) |
| History | [CHANGELOG.md](CHANGELOG.md) |

Deeper `docs/` files written for ADR-0003 are historical until rewritten for Godot. ADR-0004 and the documents listed above take precedence.

## License

[MIT](LICENSE) for repository material. It does not grant rights to third-party characters, voices, audio, or assets; see [SECURITY.md](SECURITY.md).
