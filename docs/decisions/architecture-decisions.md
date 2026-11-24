# Architecture Decision Records

> **Status:** ADR-0003 is the authoritative engine decision.  
> **Last updated:** 2026-11-25

This file records architecture decisions for **Indian Subway Runner**. ADR-0003 is the final, binding engine and platform decision. ADR-0001 and ADR-0002 are retained only as historical records and must not be used to make implementation choices.

All other design documents are subordinate to ADR-0003. See `../../ARCHITECTURE.md` for the top-level system overview and `../../README.md` for repository context.

## ADR index

| ADR | Title | Status | Date | Notes |
| --- | --- | --- | --- | --- |
| ADR-0001 | Use Unity + C# | Superseded by ADR-0002, then restored by ADR-0003 | 2026-11-24 | Historical; read ADR-0003 instead. |
| ADR-0002 | Kotlin-first native Android with libGDX 1.14.2 | Superseded by ADR-0003 | 2026-11-24 | Prohibited by the final decision; retained for history. |
| ADR-0003 | Final platform: Unity 6 LTS, C#, URP, Android | Accepted | 2026-11-25 | Authoritative final decision. |
| ADR-0004 | Save format and migration policy | Planned | — | Not written yet. |
| ADR-0005 | Contextual voice-pack delivery | Planned | — | Not written yet. |
| ADR-0006 | Monetization SDK integration | Planned | — | Not written yet. |
| ADR-0007 | Analytics and telemetry | Planned | — | Not written yet. |

Only ADR-0003 is accepted. Rows marked **Planned** reserve future decisions and do not describe implemented behaviour.

## Decision-making rules

### When an ADR is required

Create an ADR when a choice is cross-cutting, hard to reverse, externally visible, or likely to be questioned later. Examples include:

- Engine, runtime, language, or primary framework selection.
- Rendering pipeline, asset pipeline, or frame-budget trade-offs.
- On-disk save format, schema migration, or compatibility guarantees.
- Introduction of an external SDK, analytics provider, monetization provider, or network call.
- Voice-pack licensing, packaging, or delivery model.
- Privacy boundaries for player behaviour processing.
- Any decision that changes an accepted ADR or an approved design document.

Local style choices, naming of private helpers, and reversible implementation details do not require an ADR.

### Status lifecycle

- **Proposed:** a question is open and evidence is being gathered.
- **Planned:** a future decision is reserved but no ADR has been written.
- **Accepted:** an owner-approved decision is recorded and binding.
- **Superseded:** a later ADR replaces this decision; the old ADR is retained with a link to the replacement.
- **Deprecated:** the decision is no longer recommended but has not been fully replaced.
- **Rejected:** a proposed option was considered and not adopted.

Changing a status requires updating this index, the ADR body, and `../../CHANGELOG.md`. Once Accepted, an ADR should not be silently rewritten; write a new ADR that supersedes it.

### Relationship to other documents

- `../../ARCHITECTURE.md` describes the Unity/C# target structure.
- `../technical-architecture.md` expands Unity assembly, scene, prefab, and ScriptableObject responsibilities.
- `../rendering-engine.md` is subordinate to ADR-0003 and specifies URP.
- `../save-data.md` is subordinate to ADR-0004 when that ADR is written.
- `../contextual-voice-engine.md` is subordinate to ADR-0005 when that ADR is written.
- `../monetization-architecture.md` is subordinate to ADR-0006 when that ADR is written.
- `../player-behaviour-system.md` is subordinate to ADR-0007 when that ADR is written.
- `../research/observable-gameplay-analysis.md` records public Subway Surfers observations and must remain engine-neutral.

## ADR-0001: Use Unity + C#

### Status

Superseded by ADR-0002 on 2026-11-24; restored as the final direction by ADR-0003 on 2026-11-25.

### Context

An earlier decision selected Unity + C#. A later mid-session override replaced it with Kotlin/libGDX. The owner has now issued a final master brief that explicitly declares Unity + C# the final technology decision and prohibits changing engines without explicit approval.

### Decision

Do not follow ADR-0002. Unity + C# is the gameplay engine and implementation language. ADR-0003 adds the binding URP, Android, tooling, and architecture details.

### Consequence

Unity terminology — scenes, prefabs, assemblies, ScriptableObjects, URP, Animator, AudioMixer, Unity Test Framework, Addressables, and Unity Profiler — is authoritative again.

## ADR-0002: Kotlin-first native Android with libGDX 1.14.2

### Status

Superseded by ADR-0003 — 2026-11-25.

### Context

ADR-0002 was accepted during an earlier iteration of the brief that required Kotlin-first native Android and libGDX. The owner has now replaced that requirement with a final Unity + C# decision.

### Decision

This decision is no longer valid. Kotlin may only be used for specialized Android-native integrations through Unity plugins when genuinely required. Building the gameplay engine in Kotlin and using LibGDX are strictly prohibited.

### Consequence

All Gradle/Kotlin/libGDX gameplay modules, source trees, and build instructions inherited from the earlier iteration are non-authoritative and must be removed from the active Unity project. Historical mention is allowed only when clearly marked superseded.

## ADR-0003: Final platform — Unity 6 LTS, C#, URP, Android

### Status

Accepted — 2026-11-25. This is the final engine decision and does not require re-evaluation.

### Context

The project is an original Indian-themed 3D Android endless-chase game inspired only by publicly observable gameplay principles. ACP Pradyuman chases a criminal through Indian railway environments. The criminal is never permanently caught. The game must be genuinely 3D, lightweight, high-performance, offline-capable, and unmistakably Indian in identity, humour, dialogue, environments, trains, and station life.

The owner declared the following stack final:

| Concern | Final choice |
| --- | --- |
| Engine | Unity |
| Language | C# |
| Rendering | Universal Render Pipeline (URP) |
| Initial platform | Android |
| 3D modelling | Blender |
| Asset formats | FBX, GLB where appropriate |
| Textures | PNG, JPEG, optimized formats |
| Materials | URP-compatible materials |
| Animation | Unity Animator |
| Physics | Unity Physics |
| Audio | Unity Audio System |
| Version control | Git |
| Remote | GitHub |
| Documentation | Markdown + Mermaid |

The project must not use LibGDX, Three.js, WebView gameplay, React Native, Flutter, Kotlin gameplay, or an engine switch without explicit approval.

### Version recommendation

Use **Unity 6000.0.86f1 or newer 6000.0 LTS patch** as the pinned Editor version.

Reasons:

- Unity’s Android requirements state that Unity 6000.0.46f1 and later support Android API level 36, which is required for new Google Play app submissions from 2026-08-31.
- Unity 6000.0.86f1 is an LTS patch released in November 2026 and is documented with Android build-support fixes.
- Unity 6000.0.38f1 and later support 16 KB memory page size devices; this project must rebuild all native plugins for 16 KB compatibility before Play submission.
- Unity 6000.0 uses JDK 17 and NDK r27c. Unity’s supported dependency table lists SDK Build Tools 36.0.0, Command-line Tools 21.0, and Platform Tools 37.0.1 for 6000.0.86f1+.

Sources:

- <https://unity.com/releases/unity-6/support>
- <https://unity.com/releases/editor/whats-new/6000.0.86f1>
- <https://docs.unity.com/en-us/engine/6000.0/manual/platform-specific/android/introducing/requirements-and-compatibility>
- <https://docs.unity.com/en-us/engine/6000.0/manual/platform-specific/android/getting-started/sdksetup/supported-dependency-versions>
- <https://developer.android.com/google/play/requirements/target-sdk>

### Options considered

#### Option A — Unity 6 LTS with C# and URP

Accepted. It matches the final owner decision, provides a mature 3D Android toolchain, URP scalability, Animator, Physics, AudioMixer, Addressables, Unity Test Framework, and a single professional asset pipeline with Blender.

#### Option B — Kotlin/libGDX

Rejected. Explicitly prohibited by the final brief. It is also a 2D-first framework and does not satisfy the genuine 3D URP requirement.

#### Option C — WebView, React Native, Flutter, Three.js

Rejected. Explicitly prohibited for gameplay.

#### Option D — Unreal or another engine switch

Rejected. The owner declared Unity final and prohibited engine changes without explicit approval.

### Decision

Indian Subway Runner will be implemented as a genuine 3D Unity Android game in C# using URP. Kotlin may only appear in specialized Android-native Unity plugins when genuinely required. The repository will contain a Unity project with `Assets/`, `Packages/`, and `ProjectSettings/` as the source of truth. Unity-generated Gradle output remains a build implementation detail and must not be hand-maintained as the gameplay project.

Active gameplay assemblies will be separated with `.asmdef` files into Core, GameFlow, Gameplay, Player, Characters, Environment, ProceduralGeneration, Obstacles, Trains, Collectibles, PowerUps, Camera, Animation, Audio, Dialogue, Announcements, UI, Progression, Missions, SaveSystem, Analytics, Monetization, and AndroidIntegration. Data-driven content will use ScriptableObjects. Large or optional content will use Addressables. Tests will use Unity Test Framework EditMode and PlayMode tests.

### Consequences

- All active documentation, configuration, diagrams, issue templates, and build instructions must say Unity + C# + URP.
- Kotlin, libGDX, Gradle gameplay modules, JVM targets, and the old `:core`/`:game`/`:android`/`:desktop` module tree are no longer authoritative.
- Gameplay code must be C# in `Assets/Scripts/**` with Unity assembly definitions.
- 3D assets must originate from original Blender source files or clearly labelled temporary placeholders. Primitive cubes and capsules are not finished art.
- No copyrighted Subway Surfers assets, source code, decompiled output, scraped audio, celebrity voice clones, or protected game content may be used.
- Publicly observable mechanics may inspire independent implementations only.
- Android builds target API 36 and must be tested on real devices. Performance claims require Unity Profiler and device evidence.
- No gameplay subsystem may be documented as Implemented, Tested, or Verified without matching source, tests, and evidence.
- Monetization is future work until the core game is stable.

### Validation triggers

Revisit this ADR only if:

- The owner explicitly changes the final technology decision.
- A Unity LTS patch is required for Android API, 16 KB page-size, or device compatibility reasons; this is a patch within the final stack, not an engine switch.
- A Unity package or native plugin becomes incompatible with the pinned LTS version and no compatible patch exists.

Any other dissatisfaction with the stack is a implementation problem to solve inside Unity, not a reason to switch engines.
