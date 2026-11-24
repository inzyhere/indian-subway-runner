# Indian Subway Runner

> **Last updated:** 2026-11-25  
> **Status vocabulary:** Planned / In progress / Implemented / Tested / Verified.  
> **Overall position:** the Godot 4 prototype foundation is implemented but unverified; no APK artifact or device result exists yet.

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
| 3D assets | Original OBJ/MTL models copied from the preserved Unity project where useful |
| Audio | Original WAV music and SFX copied from the preserved Unity project where useful |
| CI | GitHub Actions, secret-free |
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
| Godot project foundation | Implemented — unverified | `godot/project.godot`, `main.tscn`, GDScript gameplay, HUD, camera, audio glue, and export preset exist. |
| Godot gameplay prototype | Implemented — unverified | Lane movement, jump, slide, barriers, coins, score, chase gap, procedural track, restart, and touch/keyboard input exist in GDScript. |
| Original 3D art | Implemented — unverified | Original OBJ/MTL assets were copied into `godot/assets/models/`; Godot import is not verified. |
| Original music and SFX | Implemented — unverified | Original WAV files were copied into `godot/assets/audio/`; Godot playback is not verified. |
| Android build / device verification | Blocked | Secret-free GitHub Actions workflow exists, but run `45500464511` failed before any step because GitHub reports failed account payments or a spending-limit issue; no APK artifact or device evidence exists yet. |
| GitHub remote | Implemented — pushed | Private `inzyhere/indian-subway-runner`; `main` tracks `origin/main`. |

No subsystem is marked **Tested** or **Verified**. Full detail lives in [PROJECT_STATUS.md](PROJECT_STATUS.md).

## Environment blocker summary

This shell is `aarch64` Termux. It has Git and `gh` installed, Android tools such as `aapt`/`aapt2`/`zipalign`, Java, and `adb`; it has **no local Godot binary, no `sdkmanager`, and no connected Android device**. `ANDROID_HOME` and `ANDROID_SDK_ROOT` are unset. GDScript, project files, YAML, Markdown, and directory structure can be authored and statically checked here; Godot import, export, and device runs cannot.

There is therefore no installable or playable build to download yet. The first Godot Actions run was additionally blocked by GitHub account billing/spending limits before any workflow step executed. The old debug APK from the abandoned stack is **not** the game and must not be presented as a deliverable.

## Quick document map

| Area | Document |
| --- | --- |
| Authoritative decision | [docs/decisions/architecture-decisions.md](docs/decisions/architecture-decisions.md) |
| Environment truth | [docs/environment-assessment.md](docs/environment-assessment.md) |
| Overall status | [PROJECT_STATUS.md](PROJECT_STATUS.md) |
| Active project | [godot/README.md](godot/README.md) |
| Godot Android build | [docs/godot-android-build.md](docs/godot-android-build.md) |
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
