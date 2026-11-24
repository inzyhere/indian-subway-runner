# Project Overview

> **Status:** In progress — final-stack documentation. The hand-authorable Unity skeleton is present but **not Editor-verified**.
> **Last updated:** 2026-11-25
> **Authoritative decision:** [ADR-0003](./decisions/architecture-decisions.md) — Unity 6000.0.86f1+ LTS, C#, URP, Android.
> **Applies to:** Indian Subway Runner, all phases.

## Vision

Build a **polished, original 3D Android endless-chase game** in which the player controls a criminal fleeing an ACP Pradyuman-inspired pursuer through Indian railway environments. The pursuer is always close but **never catches the criminal in v1**. The experience combines genuine 3D presentation, responsive 3-lane controls, readable mobile UI, Hindi dialogue, railway announcements, and an original soundtrack.

The game is offline-capable, respectful of intellectual property, and built for sustained **60 FPS on mainstream Android hardware** with graceful scaling on lower-tier devices.

## Final stack

| Concern | Final choice |
| --- | --- |
| Engine | Unity **6000.0.86f1 or newer 6000.0 LTS patch** |
| Language | C# with `.asmdef` boundaries |
| Rendering | Universal Render Pipeline (URP) |
| Platform | Android, ARM64, min API 24, target API 36 |
| Scripting backend | IL2CPP; 16 KB page-size compatible native plugins |
| 3D asset source | Original Blender `.blend`, exported FBX / GLB |
| Animation | Unity Animator / Animator Controllers |
| Physics | Unity Physics — colliders and triggers in `FixedUpdate` |
| Audio | Unity Audio System / AudioMixer |
| Data | ScriptableObjects; Addressables for large or optional content |
| Tests | Unity Test Framework — EditMode and PlayMode |
| Tooling | Git + GitHub; Markdown + Mermaid documentation |
| Distribution | AAB primary; APK for owner/local sideload |

**Prohibited for gameplay:** Kotlin, libGDX, Three.js, WebView gameplay, React Native, Flutter, and any unapproved engine switch. Kotlin is allowed **only** in specialized Android-native Unity plugins under `Assets/Plugins/Android/`, never for gameplay. Unity-generated Gradle output is a build artefact, never source of truth.

## Repository map — target Unity layout

The source of truth is `Assets/`, `Packages/manifest.json`, and `ProjectSettings/`. The skeleton below is the **target** described in [environment-assessment.md](./environment-assessment.md); the hand-authorable parts now exist, but they must be opened and verified in the pinned Editor. Unity-generated scene, prefab, renderer-data, and settings assets remain absent until Unity Editor is available.

```text
indian-subway-runner/
├─ Assets/
│  ├─ Art/{Characters,Trains,Stations,Props,Environments,UI,Materials,Textures}/
│  ├─ Audio/{Music,Sfx,Voice,Announcements}/
│  ├─ Data/{Config,Dialogue,Missions,Progression}/
│  ├─ Editor/
│  ├─ Plugins/Android/
│  ├─ Prefabs/{Characters,Trains,Environment,Obstacles,Collectibles,UI}/
│  ├─ Scenes/{Boot,MainMenu,StoryIntro,Game,Results}.unity
│  ├─ Scripts/<23 IndianSubwayRunner.* assemblies>/
│  ├─ Settings/URP/
│  └─ Tests/{EditMode,PlayMode}/
├─ Packages/manifest.json
├─ ProjectSettings/
├─ Tools/Blender/
├─ docs/
└─ .github/
```

The former legacy `:core`, `:game`, `:android`, and `:desktop` Gradle modules have been removed from the active tree and are **superseded and non-authoritative**. They are not the final stack and must not inform gameplay decisions.

## Truthful status

| Item | Status | Evidence / limit |
| --- | --- | --- |
| ADR-0003 final engine decision | Implemented — documented | [architecture-decisions.md](./decisions/architecture-decisions.md) |
| Environment / tooling assessment | Implemented — documented | [environment-assessment.md](./environment-assessment.md) |
| Design and architecture documents | In progress — Markdown only | This document and its siblings |
| Unity `Assets/` / `Packages/` / `ProjectSettings/` | Implemented — unverified | Hand-authored skeleton and asmdefs exist; must be opened and verified in Unity Editor |
| Unity scenes: Boot, MainMenu, StoryIntro, Game, Results | Planned | No `.unity` files exist |
| Gameplay systems and C# source | In progress — unverified | Pure C# core types exist; no Unity-compiled gameplay assembly exists |
| Prefabs, materials, URP settings | Planned | No Unity assets exist |
| 3D models, textures, animations | Planned | Blender unavailable in this shell |
| Original audio, Hindi dialogue, railway announcements | Planned | No authored assets exist |
| EditMode / PlayMode tests | In progress — unverified | EditMode test source exists; no Unity Test Framework result exists |
| Android AAB / APK build | Planned | No Unity Editor, no `adb`/`sdkmanager` |
| Legacy Kotlin/libGDX skeleton | Removed / superseded | Non-authoritative; not part of the active project |

> **No subsystem is claimed Implemented, Tested, or Verified without evidence.** Markdown describes targets; it does not create Unity content.

## Current environment limits

- No Unity Editor — cannot compile, import, open scenes, or run tests.
- No Blender — cannot author or verify `.blend`, FBX, or GLB assets.
- No `dotnet`/`csc` — cannot compile C# directly.
- No `adb`/`sdkmanager` — cannot install, launch, or verify on device.
- `gh` present but **unauthenticated**; no CI, release, or remote verification is claimed.
- Android SDK path may exist but `ANDROID_HOME` is unset; Java 21 and Gradle are non-authoritative here.

## Source-of-truth rules

1. [ADR-0003](./decisions/architecture-decisions.md) is the **only accepted engine decision**. ADR-0002 (Kotlin/libGDX) is historical, superseded, and prohibited.
2. All other design docs are subordinate to ADR-0003 and must be revised if they conflict with it.
3. Unity `Assets/`, `Packages/manifest.json`, and `ProjectSettings/` are the runtime source of truth; generated Gradle output is not.
4. [environment-assessment.md](./environment-assessment.md) records what this shell can honestly claim; it outranks optimistic status wording.
5. [PROJECT_STATUS.md](../PROJECT_STATUS.md) defines status vocabulary and evidence requirements; [README.md](../README.md) and [ARCHITECTURE.md](../ARCHITECTURE.md) define vision and top-level system shape.
6. No engine switch without a new accepted ADR that explicitly supersedes ADR-0003.

## Documentation map

| Document | Purpose |
| --- | --- |
| [README.md](../README.md) | Vision, stack, constraints, overall status |
| [ARCHITECTURE.md](../ARCHITECTURE.md) | Top-level Unity/URP system overview |
| [PROJECT_STATUS.md](../PROJECT_STATUS.md) | Status vocabulary and evidence gates |
| [ADR-0003](./decisions/architecture-decisions.md) | Final binding engine decision |
| [environment-assessment.md](./environment-assessment.md) | Shell facts and target folder structure |
| [unity-project-setup.md](./unity-project-setup.md) | Editor, packages, assembly definitions, Player Settings |
| [android-build-instructions.md](./android-build-instructions.md) | Android build prerequisites and status rules |
| [technical-architecture.md](./technical-architecture.md) | Assemblies, scenes, data, Addressables, pooling, Android boundary |
| [rendering-engine.md](./rendering-engine.md) | URP settings, quality tiers, camera, lighting, materials, performance |
| [game-design-document.md](./game-design-document.md) | Complete gameplay, content, UI, audio, and test outline |
| [performance-optimization.md](./performance-optimization.md) | Profiling and mobile performance plan |
| [testing-strategy.md](./testing-strategy.md), [TESTING.md](../TESTING.md) | Test layers and verification gates |
| [diagrams/system-architecture.md](./diagrams/system-architecture.md) | Unity system graph |
| [diagrams/data-flow.md](./diagrams/data-flow.md) | Runtime data flow |
| [diagrams/gameplay-flow.md](./diagrams/gameplay-flow.md) | Scene and run flow |

## Constraints

- Original assets and original writing only; no copying, voice cloning, or real-celebrity impersonation.
- The criminal is never permanently caught in v1; tension comes from near misses and narrow escapes.
- Offline core loop; no network dependency for gameplay.
- 60 FPS target on mainstream Android, graceful scaling on lower-tier hardware.
- Analytics, monetization, and cloud save are deferred boundaries, not active features.
