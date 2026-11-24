# Project Status

> Last updated: 2026-11-25  
> Status vocabulary: Planned, In progress, Implemented, Tested, Verified.

Indian Subway Runner is an original, offline-capable, 3D Android endless-chase game being built in **Unity 6 LTS with C# and URP**. The player controls an ACP Pradyuman-inspired pursuer chasing a criminal through Indian railway environments. The criminal is always ahead and is never permanently caught.

## Current status

| Area | Status | Truthful note |
| --- | --- | --- |
| Public gameplay research | In progress | `docs/research/observable-gameplay-analysis.md` records publicly observable principles. It must remain engine-neutral and citation-based. |
| Architecture decision | Implemented | ADR-0003 in `docs/decisions/architecture-decisions.md` is the final binding decision: Unity + C# + URP + Android. |
| Environment assessment | Implemented | See `docs/environment-assessment.md`. Blender and `adb` are installed; Unity Editor and `sdkmanager` are unavailable; no device is attached. |
| Unity project foundation | Implemented — unverified | `Assets/`, `Packages/`, `ProjectSettings/`, assembly definitions, pure C# core types, and EditMode test source are hand-authored. They must be opened and verified in Unity Editor. |
| Legacy Kotlin/libGDX skeleton | Removed / superseded | The old Gradle/Kotlin/libGDX source tree and generated APK were not the final stack and are not part of the active project. |
| Gameplay systems | Implemented — unverified | C# source exists for lanes, input buffering, locomotion, scoring, difficulty, power-ups, chase gap, collisions, procedural chunks, `RunnerSimulation`, HUD, and audio glue. It has not compiled or run in Unity. |
| 3D characters, trains, stations, props | Implemented — unverified | `Tools/Blender/Scripts/generate_assets.py` generated 11 original OBJ/MTL models under `Assets/Resources/Art/Models`; Unity import and prefab creation are not verified. Runtime primitive fallbacks remain in view scripts. |
| Animations | Planned | No Animator controllers or clips are claimed. |
| Hindi dialogue and announcements | In progress | Runtime cue strings exist in gameplay glue, and original generated music/SFX exist as WAV files. No voice pack, announcement clip, or licensed external audio exists. |
| URP scene, lighting, post-processing | In progress | `Assets/Editor/SceneBootstrapGenerator.cs` can create Boot/MainMenu/StoryIntro/Game/Results scenes and build settings in Unity, but it has not been run or verified. |
| Android build | In progress | `.github/workflows/build-android.yml` and `AndroidBuild.BuildCiApk()` exist for an x64 GitHub Actions debug APK build. No APK/AAB has been built, uploaded, installed, or verified yet; Unity license secrets and a pushed workflow are required. |
| GitHub remote | Implemented | Private `inzyhere/indian-subway-runner`; `main` tracks `origin/main`. |

## What is verified

- Repository files, Markdown, Mermaid, JSON, XML, shell scripts, and folder structure can be statically checked.
- ADR-0003 is consistent with the owner’s final technology decision.
- No Kotlin gameplay source, Gradle gameplay module, or LibGDX dependency is present in the active Unity project after migration.
- Pure C# core types and NUnit EditMode test source exist as text; they have not been compiled or run by Unity.
- `blender-4.5 --background --python Tools/Blender/Scripts/generate_assets.py` generated 11 original OBJ/MTL model pairs under `Assets/Resources/Art/Models`.
- `python3 Tools/Audio/generate_audio.py` generated one original music loop and seven original SFX WAV files under `Assets/Resources/Audio`.
- No secret, token, keystore, signing password, or unauthorized third-party asset is committed.

## What is not verified

- Unity compilation, scene loading, prefab integrity, URP rendering, Animator behaviour, Physics, AudioMixer, Addressables, UI, or SaveSystem.
- Unity Test Framework EditMode and PlayMode results.
- Unity import of generated OBJ/MTL models and WAV files.
- Android API 36 build compatibility, 16 KB page-size compatibility, device performance, 60 FPS target, battery/thermal behaviour.
- Monetization, telemetry, cloud save, or contextual voice-pack delivery; all are future work.

## Next milestone

1. Open the Unity project in Unity 6000.0.86f1 or newer 6000.0 LTS patch and resolve import/compiler errors.
2. Run `Indian Subway Runner > Generate Game Scenes` from the Unity Editor and verify scene Build Settings order.
3. Verify URP package resolution, input actions, assembly definitions, generated OBJ imports, and WAV imports.
4. Run EditMode tests and add PlayMode coverage for boot/menu/story/game/results, input, collision, chunk streaming, HUD, and restart.
5. Replace runtime primitive fallbacks with prefabs from imported original models and create Animator/AudioMixer assets.
6. Push the workflow, configure `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD`, run `Build Android APK`, and verify the uploaded debug APK; then perform real-device Profiler verification.
7. Add monetization only after core stability is verified.

## Blockers requiring owner or environment action

- Push the uncommitted working tree, including `.github/workflows/build-android.yml`, to the private remote.
- Configure GitHub repository secrets `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` for the CI build.
- Install Unity 6000.0.86f1 or newer 6000.0 LTS patch with Android Build Support, SDK, NDK, and JDK modules when building locally.
- Provide a connected Android device for runtime and performance verification.
- Provide or configure Android `sdkmanager`/SDK licenses in a supported Unity environment before Android builds.
