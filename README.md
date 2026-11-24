# Indian Subway Runner

> **Last updated:** 2026-11-25  
> **Status vocabulary:** Planned / In progress / Implemented / Tested / Verified.  
> **Overall position:** foundation decisions Implemented; all Unity gameplay, builds, tests, and device work remain Planned or blocked.

## Vision

**INDIAN SUBWAY RUNNER** is an original, offline-capable, genuine 3D Android endless-chase game. An ACP Pradyuman-inspired pursuer chases a criminal through Indian railway stations and trains. The criminal is always ahead and is **never permanently caught in v1**; chapters, clues, records, and near-captures carry the story forward.

The player runs through three lanes with swipe left/right, jump, and slide. Runs include trains, obstacles, coins, power-ups, score and distance, increasing difficulty, procedural chunks, game over/restart, a story intro with chapters, contextual Hindi dialogue, and railway announcements. Identity comes from original Indian environments, humour, dialogue, trains, and station life. Visuals target URP with a sustained 60 FPS on mainstream Android hardware and graceful scaling on budget devices.

## Final technology decision

[ADR-0003](docs/decisions/architecture-decisions.md) is the final, binding decision. No engine switch without explicit owner approval.

| Concern | Final choice |
| --- | --- |
| Engine / language | Unity **6000.0.86f1 or newer 6000.0 LTS patch**, C# |
| Rendering | Universal Render Pipeline (URP) |
| Platform | Android (ARM64 primary, API 36 target) |
| 3D assets | Blender source (`.blend`), exported FBX / GLB; URP materials |
| Textures | PNG / JPEG and optimized runtime formats |
| Animation / physics / audio | Unity Animator, Unity Physics, Unity Audio / AudioMixer |
| Tooling | Git + GitHub, Markdown + Mermaid docs |

**Prohibited for gameplay:** Kotlin gameplay, libGDX, Three.js, WebView gameplay, React Native, Flutter, and any unapproved engine change. Kotlin is permitted only as a specialized Android-native Unity plugin when genuinely required. Unity-generated Gradle output is a build artifact, never gameplay source.

## Current status

| Area | Status | Truthful note |
| --- | --- | --- |
| Public gameplay research | In progress | Engine-neutral, citation-based record in [docs/research/observable-gameplay-analysis.md](docs/research/observable-gameplay-analysis.md). |
| Architecture decision | Implemented | ADR-0003 is final; ADR-0001/ADR-0002 are historical. |
| Environment assessment | Implemented | [docs/environment-assessment.md](docs/environment-assessment.md) records the current shell. |
| Unity project foundation | In progress | Decision, setup steps, layout, and package direction are documented; Unity import/compiler verification has not happened. |
| Legacy Kotlin/libGDX skeleton and old APK | Removed / superseded | Legacy source tree removed; any old APK remains outside the repository and is not the final build. |
| Gameplay systems, scenes, URP visuals, Animator, Physics, Audio | Planned | No C# gameplay, Unity scene, prefab, animation, or audio asset is claimed. |
| Original 3D art | Planned | No Blender-authored model, FBX, GLB, rig, or texture is claimed. |
| Hindi dialogue / announcements | Planned | No voice pack or licensed audio exists. |
| Unity Test Framework results | Planned | No EditMode or PlayMode result exists. |
| Android build / device verification | Planned — blocked here | Requires Unity Editor, Android modules, `adb`, and a device. |
| GitHub remote | Implemented — pushed | Private `inzyhere/indian-subway-runner`; `main` tracks `origin/main`. |

No subsystem is marked **Tested** or **Verified**. Full detail lives in [PROJECT_STATUS.md](PROJECT_STATUS.md).

## Environment blocker summary

This shell is `aarch64` Termux. It has Git and `gh` installed, an Android SDK path with `aapt`/`aapt2`/`zipalign`, and Java; it has **no Unity Editor, no Blender, no `dotnet`/`csc`, no `adb`, and no `sdkmanager`**. `ANDROID_HOME` and `ANDROID_SDK_ROOT` are unset. `gh` was unauthenticated at initial assessment and is now authenticated as `inzyhere`; the directory was not a Git repository when assessed but now tracks the private `inzyhere/indian-subway-runner` remote. Markdown, JSON, XML, and directory structure can be authored and statically checked here; Unity compilation, scenes, art, audio, tests, builds, and device runs cannot.

There is therefore no installable or playable build to download. The old debug APK from the legacy stack is **not** the game and must not be presented as a deliverable. See [DEVELOPMENT.md](DEVELOPMENT.md) for the Unity-side prerequisites.

## Quick document map

| Area | Document |
| --- | --- |
| Authoritative decision | [docs/decisions/architecture-decisions.md](docs/decisions/architecture-decisions.md) |
| Environment truth | [docs/environment-assessment.md](docs/environment-assessment.md) |
| Overall status | [PROJECT_STATUS.md](PROJECT_STATUS.md) |
| Unity project setup | [docs/unity-project-setup.md](docs/unity-project-setup.md) |
| Android build target | [docs/android-build-instructions.md](docs/android-build-instructions.md) |
| System architecture | [ARCHITECTURE.md](ARCHITECTURE.md) |
| Phased plan | [ROADMAP.md](ROADMAP.md) |
| Development workflow | [DEVELOPMENT.md](DEVELOPMENT.md) |
| Testing policy | [TESTING.md](TESTING.md) |
| Contributing | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Security | [SECURITY.md](SECURITY.md) |
| History | [CHANGELOG.md](CHANGELOG.md) |

Deeper `docs/` files are written for ADR-0003; any remaining Kotlin/libGDX mention is explicitly marked as historical, prohibited, or superseded. ADR-0003 and the documents listed above take precedence.

## License

[MIT](LICENSE) for repository material. It does not grant rights to third-party characters, voices, audio, or assets; see [SECURITY.md](SECURITY.md).
