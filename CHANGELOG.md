# Changelog

> **Last updated:** 2026-11-25  
> All notable changes are recorded here using Keep a Changelog conventions. No released version exists.  
> Status vocabulary: Planned / In progress / Implemented / Tested / Verified.

**No Unity gameplay, scene, art, audio, test, Android build, or device verification is claimed.** Unity and legacy Kotlin/libGDX entries below are preserved only as history; ADR-0003 is **superseded** by ADR-0004.

## [Unreleased]

### 2026-11-25 — Active Godot prototype foundation (unverified)

#### Added

- [ADR-0004](docs/decisions/architecture-decisions.md) as the active binding decision: Godot 4, GDScript, Android debug APK, and the preserved Unity project retained only as history. ADR-0003 and earlier are historical.
- New Godot 4 project under `godot/`: `project.godot`, `main.tscn`, `export_presets.cfg`, `icon.svg`, `.gitignore`, and `README.md`.
- `godot/scripts/main.gd` with an unverified playable prototype: three lanes, keyboard/touch input, jump, slide, low/high barriers, coins, increasing speed, score, chase gap, procedural track, HUD, restart, camera, and original audio glue.
- Original OBJ/MTL models copied to `godot/assets/models/` and original WAV music/SFX copied to `godot/assets/audio/`; Godot import and playback are not verified.
- `.github/workflows/godot-android.yml` for a secret-free debug APK: public Godot `4.7.2-stable` download, matching export templates, Android SDK setup, CI-generated debug keystore, headless import, debug export, and artifact upload.
- [docs/godot-android-build.md](docs/godot-android-build.md) with the active build path and evidence boundary.

#### Changed

- Root documentation now treats Godot/GDScript as active and Unity as preserved history.
- Android signing expectations changed to a disposable CI-generated debug keystore; no Unity credential, GitHub secret, release key, or signing password is required.
- Status records now explicitly separate implemented source/assets from tested export and verified device evidence.

#### Not implemented

- No Godot import, GDScript runtime, resource import, rendering, input, audio playback, APK artifact, installation, logcat, performance capture, or device verification exists yet.
- GitHub Actions run `45500464511` failed before any step because GitHub reported failed account payments or a spending-limit issue; the workflow remains unexecuted and no artifact was produced.

### 2026-11-25 — Source implementation and generated original assets (Unity-unverified)

#### Added

- Pure C# gameplay source for action arbitration, locomotion, scoring, difficulty, power-ups, chase gap, collision, RNG, and procedural chunk validation/generation, plus EditMode test source.
- Unity-facing gameplay glue: `RunnerSimulation`, input, runner/chunk views, HUD, audio playback, `GameBootstrap`, and an editor scene generator for Boot/MainMenu/StoryIntro/Game/Results.
- `Tools/Blender/Scripts/generate_assets.py`, tested with Blender 4.5.14 LTS; it generated 11 original OBJ/MTL model pairs under `Assets/Resources/Art/Models`.
- `Tools/Audio/generate_audio.py`, tested in this shell; it generated one original WAV music loop and seven original WAV SFX under `Assets/Resources/Audio`.

#### Changed

- Status documents now distinguish generated source/assets from Unity-compiled, Tested, or Verified content.
- Environment notes now record installed Blender and `adb`, the broken Android Blender FBX operator, and the OBJ/MTL workaround.

### 2026-11-25 — Private remote connected

#### Added

- Private GitHub remote `inzyhere/indian-subway-runner` configured as `origin`; local `main` tracks `origin/main`.

#### Changed

- Remote `Initial commit` was fetched and merged into local history without force; the expanded README and owner-attributed MIT LICENSE were retained.
- Stale docs that described GitHub auth or the remote as blocked were updated.

### 2026-11-25 — Unity foundation decision (final)

#### Added

- [ADR-0003](docs/decisions/architecture-decisions.md) as the final, binding platform decision: Unity 6000.0.86f1+ LTS, C#, URP, Android; original Blender 3D assets, FBX/GLB, PNG/JPEG, URP materials, Animator, Unity Physics, Unity Audio; Git/GitHub and Markdown + Mermaid docs.
- [docs/environment-assessment.md](docs/environment-assessment.md) with exact shell facts: `aarch64` Termux, no Unity Editor, no Blender, no `dotnet`/`csc`, `gh` installed but unauthenticated, Git installed with no repository at assessment time, Android SDK path present with `aapt`/`aapt2`/`zipalign`, `adb`/`sdkmanager` absent, `ANDROID_HOME`/`ANDROID_SDK_ROOT` unset.
- Unity direction documents: [docs/unity-project-setup.md](docs/unity-project-setup.md) and [docs/android-build-instructions.md](docs/android-build-instructions.md).
- Root documentation rewritten to Unity/URP vocabulary; [README.md](README.md), [ARCHITECTURE.md](ARCHITECTURE.md), [ROADMAP.md](ROADMAP.md), [CONTRIBUTING.md](CONTRIBUTING.md), [DEVELOPMENT.md](DEVELOPMENT.md), [TESTING.md](TESTING.md), and [SECURITY.md](SECURITY.md) now follow ADR-0003 and the five-word status vocabulary.
- Explicit note that the criminal always escapes in v1, with story chapters and contextual Hindi dialogue as planned narrative support.

#### Changed

- Authoritative engine direction is now ADR-0003. ADR-0001 is historical, and ADR-0002 is superseded.
- Root status claims were reduced to evidence: research is **In progress**; ADR and environment assessment are **Implemented**; Unity foundation is **In progress** and Unity-unverified; gameplay, art, audio, tests, builds, and devices remain **Planned**.
- Documentation now states that Kotlin is limited to specialized Android-native Unity plugins; Kotlin gameplay, libGDX, Three.js, WebView gameplay, React Native, Flutter, and engine switches without owner approval are prohibited.

#### Deprecated / removed from active direction

- Kotlin/libGDX gameplay modules (`:core`, `:game`, `:android`, `:desktop`), Gradle gameplay build commands, `MainActivity`, `SpriteBatch`/`TextureAtlas`, Scene2D, and the old APK handoff flow. Any remaining legacy files are non-authoritative historical artifacts, not the Unity project.
- All root-documentation claims that the JVM modules compiled, a `:core` config test passed, or a legacy debug APK was a deliverable.

#### Security

- Policy restated for Unity: no secrets, tokens, or keystores in source control; no unauthorized or ripped assets; no voice cloning; GitHub remote was blocked at that stage until `gh auth login`. No security audit has been performed.

#### Not implemented

- No Unity project import, compile, scene, prefab, URP render, Animator, Physics, AudioMixer, Addressables group, EditMode/PlayMode test, AAB/APK, or device result exists in this environment.

### 2026-11-24 — LEGACY / SUPERSEDED Kotlin + libGDX foundation

> **Historical only.** These items were recorded under the now-superseded ADR-0002. They are not part of the active Unity project and must not be used as implementation guidance.

- Added documentation set, public-behaviour research record, and an explicit status vocabulary.
- Added a Gradle/Kotlin/libGDX skeleton with `:core`, `:game`, `:android`, and an optional `:desktop` harness; empty boot/menu/game screens; and a JVM config test.
- Recorded a local legacy debug APK (`/sdcard/Download/Indian-Subway-Runner-debug.apk`, SHA-256 `83d70d47a0f0c809e8cc3792c44a066aa6aa4e4ab6eea572544fbf247899eebc`). **This artifact was built from the abandoned stack, is not the final game, and is not a release or verification result.**
- Fixed Gradle repository-mode imports, JVM target alignment, and Android manifest component names in the legacy skeleton.
- Superseded ADR-0001 with ADR-0002 at that time; ADR-0002 itself was superseded by ADR-0003 on 2026-11-25.

## Change categories

| Category | Meaning |
| --- | --- |
| Added | New documented capability, policy, or specification. |
| Changed | Deliberate direction, scope, or documentation change. |
| Deprecated / removed | No longer part of the active direction. |
| Security | Privacy, secrets, or asset-provenance policy. |
| Not implemented | Explicit boundary against unsupported claims. |

Versioned sections such as `[1.0.0]` will be added only when an active build exists, a release version is assigned, and evidence is recorded. Until then all changes remain under `[Unreleased]`, and status claims must not exceed [TESTING.md](TESTING.md) gates.
