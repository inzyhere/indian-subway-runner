# Contributing

> **Last updated:** 2026-11-25  
> **Status:** active Godot 4 implementation; the Unity instructions below are preserved history. Godot smoke, gameplay capture, Android export, and debug APK release have CI evidence; device and human visual verification remain pending.

The active repository follows ADR-0004: Godot 4, GDScript, Android. [ADR-0003](docs/decisions/architecture-decisions.md) and the Unity rules below are historical only. Do not reintroduce Kotlin/libGDX gameplay, Three.js, WebView gameplay, React Native, Flutter, or an engine switch without explicit owner approval.

## Current collaboration state

The public `inzyhere/indian-subway-runner` remote is connected, with CI evidence recorded in [PROJECT_STATUS.md](PROJECT_STATUS.md) and a debug APK published at [godot-debug-apk](https://github.com/inzyhere/indian-subway-runner/releases/tag/godot-debug-apk). Work with focused branches from `main`, open a pull request linked to an issue, and merge only after the gates in [TESTING.md](TESTING.md) pass.

## Historical Unity workflow

1. Identify the issue and milestone from [ROADMAP.md](ROADMAP.md) before non-trivial work.
2. Make the smallest coherent change in the Unity project (`Assets/`, `Packages/`, `ProjectSettings/`).
3. Add Unity Test Framework coverage for behaviour changes: EditMode for pure logic, PlayMode for scene/runtime behaviour.
4. Update the relevant docs and [CHANGELOG.md](CHANGELOG.md) in the same milestone.
5. Verify in the pinned Editor; record evidence before moving any status beyond **Implemented**.
6. Review for dependency rules, asset provenance, secrets, and truthfulness before merge.

## Historical Unity assembly definition rules

- Place gameplay code in `Assets/Scripts/**` under an `IndianSubwayRunner.<Area>` `.asmdef`; areas are listed in [ARCHITECTURE.md](ARCHITECTURE.md).
- `Core` must not depend on scene objects, UI, audio, or platform SDKs. `GameFlow` coordinates lifecycle; `AndroidIntegration` depends on `GameFlow`, never the reverse.
- Tests reference the assembly under test plus `Core`; do not add broad cross-assembly references to make tests pass.
- `Analytics` and `Monetization` remain passive and disabled until an owner-approved ADR enables them.
- Do not hand-maintain Unity-generated Gradle output as gameplay source.

## Documentation truthfulness

Use only **Planned**, **In progress**, **Implemented**, **Tested**, and **Verified**. A change is Implemented when real files exist, Tested only when Unity tests pass, and Verified only when build/device evidence is recorded. Never describe planned scenes, art, audio, gameplay, builds, or test results as finished. Docs older than ADR-0003 are historical until reconciled; cite the ADR, the environment assessment, and [PROJECT_STATUS.md](PROJECT_STATUS.md) as authority.

## Asset provenance

All shipped content must be original, commissioned, or licensed for distribution in an Android game:

- Keep Blender `.blend` sources in `Tools/Blender/Source/**`; import gameplay meshes as FBX/GLB and textures as PNG/JPEG into `Assets/Art/**`.
- Use URP-compatible materials only; validate scale, pivots, normals, UVs, and LODs in Unity.
- Record creator, source, license, date, modifications, destination path, and reviewer for each asset group.
- Prohibited: ripped/decompiled game content, scraped audio or memes, unauthorized textures/models/fonts, celebrity voice clones, and protected third-party character likenesses. Publicly observable mechanics may inspire independent implementations only.

## Secrets and credentials

Never commit tokens, API keys, signing passwords, `.keystore`/`debug.keystore` files, credentials, or private licensed asset bundles. Use secure local storage or CI secrets for signing; the GitHub remote is public after owner approval, so scan before each push. The Godot Release workflow uses only the automatic `GITHUB_TOKEN`; do not add repository secrets for it. Report suspected exposure privately as described in [SECURITY.md](SECURITY.md).

## Commit conventions

```text
type(scope): short imperative summary
```

Types: `docs`, `feat`, `fix`, `test`, `perf`, `refactor`, `chore`, `build`. Unity-oriented scopes include `docs`, `core`, `gameplay`, `ui`, `audio`, `art`, `rendering`, `android`, `data`, and `tests`. Examples: `docs(adr): finalize unity stack decision`, `test(playmode): cover lane transitions`. Do not force-push shared branches without agreement.

A Developer Certificate of Origin is not required; add `Signed-off-by` only voluntarily.
