# Testing

> **Last updated:** 2026-11-25  
> **Status:** In progress — Unity Test Framework and device policy; EditMode source exists, but no Unity test, build, or device evidence exists.

This document defines evidence gates for **Indian Subway Runner** under [ADR-0003](docs/decisions/architecture-decisions.md). Testing exists to produce evidence, not confidence. Statuses are only **Planned**, **In progress**, **Implemented**, **Tested**, or **Verified**.

## Test layers

| Layer | Method | Scope | Status |
| --- | --- | --- | --- |
| EditMode | Unity Test Framework | Lane math, scoring, difficulty curves, pooling, data validation, future save-format logic. | In progress — source exists; not run |
| PlayMode | Unity Test Framework | Boot → menu → story → game → results flow, input, collision, chunk streaming, HUD, restart, pause/resume. | Planned |
| Scene / prefab checks | Editor validation | Scene Build Settings order, prefab integrity, URP asset assignment, Animator controllers, missing references. | In progress — generator source exists; not run |
| Manual playtest | Scripted device sessions | Touch feel, hazard fairness, Hindi dialogue timing, announcements, interruption/background-resume, low-memory behaviour. | Planned |
| Device matrix | Physical Android devices + emulator sanity checks | OS versions, screen sizes/aspect ratios, refresh rates, low-memory devices, install paths. | Planned |
| Performance | Unity Profiler + logcat | Frame time, GC allocations, GPU cost, memory growth, thermal throttling, startup, pooled entity counts. | Planned |
| Build verification | Unity Android build | API 36 target, ARM64/IL2CPP, 16 KB plugin compatibility, AAB and APK output, install and launch. | Planned |

No JUnit 5, Espresso, Robolectric, Gradle managed device, libGDX, or legacy Kotlin test path is in scope.

## Device matrix

| Class | Characteristics | Purpose |
| --- | --- | --- |
| Low-memory | Limited RAM, lower GPU, 60 Hz | Validate bounded pools, quality scaling, and stability. |
| Mid-range | Representative mainstream device, 60/90 Hz | Validate default quality and normal sessions. |
| High-refresh | 90/120 Hz capable | Validate optional refresh targets and frame pacing. |

Emulator runs are sanity checks only; physical device evidence is required for touch, thermal, and GPU claims. Exact devices are not yet selected.

## Build verification gates

Before any AAB/APK is called acceptable:

1. Project opens in Unity 6000.0.86f1+ LTS with no import/compiler errors; URP active.
2. All EditMode and PlayMode tests pass and results are recorded.
3. Player Settings: package `com.indiansubwayrunner.game`, min API 24, target API 36, ARM64, IL2CPP, linear colour, 16 KB-compatible native plugins.
4. App launches to menu without crash or black screen; safe area, orientation, back gesture, interruptions, and background/resume behave correctly.
5. Core run loop reaches results and restart; touch input latency is acceptable.
6. Sustained 60 FPS target is supported by Unity Profiler captures on a reference device.
7. No placeholder art, debug-only path, unauthorized asset, secret, or signing material is embedded.
8. Build hash, Unity version, package versions, date, and tested device are recorded in [PROJECT_STATUS.md](PROJECT_STATUS.md).

Owner-requested APK handoff to `/sdcard/Download/Indian-Subway-Runner-Unity.apk` is planned only for a real Unity release APK. The legacy debug APK from the abandoned stack is not a result and must not be copied or presented as final. Full build steps: [docs/android-build-instructions.md](docs/android-build-instructions.md).

## Evidence rules

- **Implemented** requires real code/assets; **Tested** requires passing EditMode/PlayMode results; **Verified** requires build hash, device/Profiler evidence, licensing and privacy review, and reviewer acceptance.
- Evidence must be tied to the exact build and stored with the milestone that produced it. A screenshot, a compile, or an opinion is not verification.
- Performance claims require repeated Profiler sessions long enough to expose growth and throttling; the 60 FPS / 16.67 ms target remains **Planned** until then.
- Any S0 (crash/data loss/security exposure) or S1 (core play unplayable/unfair) defect blocks **Verified** for the affected area.
- Status transitions are recorded in [CHANGELOG.md](CHANGELOG.md) and [PROJECT_STATUS.md](PROJECT_STATUS.md) in the same milestone.

## Explicit current state

No Unity Editor, C# compiler, `sdkmanager`, or connected device is available in this shell. Blender 4.5.14 LTS and `adb` 1.0.41 are installed, but no Unity project has been imported; no EditMode or PlayMode test has been run; no generated scene has been executed; no prefab, URP render, Animator, Physics, AudioMixer, or Addressables behaviour has been observed; no AAB/APK has been built; no Profiler capture, logcat, device matrix, or licensing review exists. Every testing layer above remains **Planned** or **In progress**, and none is **Tested** or **Verified**.
