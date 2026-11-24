# Indian Subway Runner

> **Last updated:** 2026-11-25  
> **Status vocabulary:** Planned / In progress / Implemented / Tested / Verified.  
> **Overall position:** foundation decisions and source implementation are in progress; Unity compilation, tests, builds, and device work remain unverified or blocked.

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
| 3D assets | Blender source; current shell generates OBJ/MTL because its FBX operator is broken; FBX/GLB remains the desktop target |
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
| Unity project foundation | Implemented — unverified | Scaffold, package manifest, assembly definitions, core C# source, and test source exist; Unity import/compiler verification has not happened. |
| Legacy Kotlin/libGDX skeleton and old APK | Removed / superseded | Legacy source tree removed; any old APK remains outside the repository and is not the final build. |
| Gameplay systems | Implemented — unverified | C# gameplay source, `RunnerSimulation`, input, HUD, and audio glue exist; no Unity compile or playtest evidence exists. |
| Scenes, URP visuals, Animator, Physics | In progress | Scene generator source exists; Unity scenes, prefabs, URP assets, Animator controllers, and Physics behaviour are not verified. |
| Original 3D art | Implemented — unverified | 11 original OBJ/MTL model pairs were generated; Unity import, prefab creation, rigs, and textures are not verified. |
| Original music and SFX | Implemented — unverified | One original WAV loop and seven original WAV SFX were generated; Unity import and playback are not verified. |
| Hindi dialogue / announcements | In progress | Runtime cue strings exist; no voice pack or generated announcement clip exists. |
| Unity Test Framework results | Planned | EditMode source exists, but no EditMode or PlayMode result exists. |
| Android build / device verification | Planned — blocked here | Requires Unity Editor, Android modules, SDK configuration, and a connected device. |
| GitHub remote | Implemented — pushed | Private `inzyhere/indian-subway-runner`; `main` tracks `origin/main`. |

No subsystem is marked **Tested** or **Verified**. Full detail lives in [PROJECT_STATUS.md](PROJECT_STATUS.md).

## Environment blocker summary

This shell is `aarch64` Termux. It has Git and `gh` installed, an Android SDK path with `aapt`/`aapt2`/`zipalign`, Java, `adb`, and Blender 4.5.14 LTS; it has **no Unity Editor, no C# compiler, no `sdkmanager`, and no connected Android device**. `ANDROID_HOME` and `ANDROID_SDK_ROOT` are unset. `gh` was unauthenticated at initial assessment and is now authenticated as `inzyhere`; the directory was not a Git repository when assessed but now tracks the private `inzyhere/indian-subway-runner` remote. Markdown, JSON, XML, directory structure, Python-generated WAV audio, and Blender-generated OBJ/MTL models can be authored and statically checked here; Unity compilation, scenes, Unity imports, tests, builds, and device runs cannot.

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
