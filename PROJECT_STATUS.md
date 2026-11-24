# Project Status

> Last updated: 2026-11-25  
> Status vocabulary: Planned, In progress, Implemented, Tested, Verified.

Indian Subway Runner is an original, offline-capable, 3D Android endless-chase game being built in **Unity 6 LTS with C# and URP**. The player controls an ACP Pradyuman-inspired pursuer chasing a criminal through Indian railway environments. The criminal is always ahead and is never permanently caught.

## Current status

| Area | Status | Truthful note |
| --- | --- | --- |
| Public gameplay research | In progress | `docs/research/observable-gameplay-analysis.md` records publicly observable principles. It must remain engine-neutral and citation-based. |
| Architecture decision | Implemented | ADR-0003 in `docs/decisions/architecture-decisions.md` is the final binding decision: Unity + C# + URP + Android. |
| Environment assessment | Implemented | See `docs/environment-assessment.md`. Unity Editor, Blender, GitHub authentication, `adb`, and `sdkmanager` are unavailable here. |
| Unity project foundation | Implemented — unverified | `Assets/`, `Packages/`, `ProjectSettings/`, assembly definitions, pure C# core types, and EditMode test source are hand-authored. They must be opened and verified in Unity Editor. |
| Legacy Kotlin/libGDX skeleton | Removed / superseded | The old Gradle/Kotlin/libGDX source tree and generated APK were not the final stack and are not part of the active project. |
| Gameplay systems | Planned | No gameplay code is claimed as implemented until C# scripts exist and compile in Unity. |
| 3D characters, trains, stations, props | Planned | No final Blender-authored models exist in this environment. |
| Animations | Planned | No Animator controllers or clips are claimed. |
| Hindi dialogue and announcements | Planned | No voice assets or licensed audio exist. |
| URP scene, lighting, post-processing | Planned | No Unity scene is claimed until created and verified in Unity Editor. |
| Android build | Blocked | Unity Editor is required. No Android APK/AAB can be built or verified in this environment. |
| GitHub remote | Implemented | Private `inzyhere/indian-subway-runner`; `main` tracks `origin/main`. |

## What is verified

- Repository files, Markdown, Mermaid, JSON, XML, shell scripts, and folder structure can be statically checked.
- ADR-0003 is consistent with the owner’s final technology decision.
- No Kotlin gameplay source, Gradle gameplay module, or LibGDX dependency is present in the active Unity project after migration.
- Pure C# core types and NUnit EditMode test source exist as text; they have not been compiled or run by Unity.
- No secret, token, keystore, signing password, or unauthorized third-party asset is committed.

## What is not verified

- Unity compilation, scene loading, prefab integrity, URP rendering, Animator behaviour, Physics, AudioMixer, Addressables, UI, or SaveSystem.
- Unity Test Framework EditMode and PlayMode results.
- Android API 36 build compatibility, 16 KB page-size compatibility, device performance, 60 FPS target, battery/thermal behaviour.
- Monetization, telemetry, cloud save, or contextual voice-pack delivery; all are future work.

## Next milestone

1. Open the Unity project in Unity 6000.0.86f1 or newer 6000.0 LTS patch and resolve import/compiler errors.
2. Verify URP package resolution, scene template, input actions, and assembly definitions.
3. Implement the boot/menu/game/results flow with Unity Test Framework coverage.
4. Implement core 3-lane movement, jump, slide, obstacle collision, train movement, collectibles, scoring, and procedural chunk streaming.
5. Integrate original Blender assets as they become available.
6. Create Android build and perform real-device Profiler verification.
7. Add monetization only after core stability is verified.

## Blockers requiring owner or environment action

- Install Unity 6000.0.86f1 or newer 6000.0 LTS patch with Android Build Support, SDK, NDK, and JDK modules.
- Provide Blender or an already Blender-capable asset workstation for original 3D models.
- Run `gh auth login` or provide an authenticated GitHub environment before creating/pushing the private remote.
- Provide `adb` and a connected Android device for runtime and performance verification.
