# Android Build Instructions

> Status: Planned. No Unity Android build has been produced yet. The GitHub Actions debug APK workflow is pushed but has not been run.

## Prerequisites

- Unity 6000.0.86f1 or newer 6000.0 LTS patch.
- Android Build Support, SDK, NDK, and OpenJDK 17 modules installed through Unity Hub.
- Android SDK Platform 36, Build Tools 36.0.0, Command-line Tools 21.0, Platform Tools 37.0.1.
- NDK r27c as required by Unity 6000.0.
- All native plugins rebuilt for 16 KB page-size compatibility.
- Real Android device and `adb` for verification.

## GitHub Actions CI build

Manual workflow `.github/workflows/build-android.yml` builds on `ubuntu-latest` with GameCI Unity Builder.

Required repository secrets:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

Professional/serial licenses additionally require extending the workflow with `UNITY_SERIAL`.

The workflow is pushed; run `Actions > Build Android APK > Run workflow`. The output artifact is `IndianSubwayRunner-debug-apk` containing `Builds/IndianSubwayRunner.apk`. The workflow calls `IndianSubwayRunner.Editor.AndroidBuild.BuildCiApk`, which generates scenes before building. This is a debug APK for side-load testing, not a signed release artifact.

## Player Settings target

| Setting | Value |
| --- | --- |
| Package name | `com.inzyhere.indiansubwayrunner` for the first debug APK |
| Minimum API level | 24 |
| Target API level | 36 |
| Scripting backend | IL2CPP |
| CPU architectures | ARM64 required; ARMv7 only if justified |
| Colour space | Linear |
| Rendering | URP |
| Input | New Input System preferred; old Input only if compatibility requires it |
| Internet permission | Only if a verified feature requires it |
| Android target architecture | 64-bit |

## Development APK build

1. In Unity, open `File > Build Profiles > Android`.
2. Confirm scenes in this order:
   - `Assets/Scenes/Boot.unity`
   - `Assets/Scenes/MainMenu.unity`
   - `Assets/Scenes/StoryIntro.unity`
   - `Assets/Scenes/Game.unity`
   - `Assets/Scenes/Results.unity`
3. Set development build on for diagnostics.
4. Set scripting backend to IL2CPP.
5. Connect a device and select it.
6. Choose `Build And Run`.
7. Capture Unity Profiler, logcat, and device screenshots.
8. Record evidence in `docs/testing-strategy.md` and `docs/performance-optimization.md`.

## Release AAB build

1. Verify all EditMode and PlayMode tests pass.
2. Verify no debug-only code, fake buttons, or placeholder art remains.
3. Set development build off.
4. Use the project’s signing configuration from secure CI or local secure store.
5. Never commit keystores, signing passwords, or credentials.
6. Build Android App Bundle.
7. Archive the AAB and mapping/debug symbols if applicable.
8. Record build hash, Unity version, package versions, and date in `docs/release-process.md`.

## Owner-requested APK handoff

The owner requested a built APK copied to `/sdcard/Download` as the final deliverable.

When Unity or CI is available:

1. Build the debug APK with `com.inzyhere.indiansubwayrunner` application id.
2. Verify install on a real device.
3. Copy the APK to `/sdcard/Download/IndianSubwayRunner.apk`.
4. Record SHA-256, size, Unity version, build date, and tested device in `PROJECT_STATUS.md`.

This shell cannot perform those steps because Unity Editor, Android build modules, and `sdkmanager` are unavailable, and no device is attached.

## Verification gates before calling a build acceptable

- App launches to the menu without crash or black screen.
- Orientation, safe area, back button, interruption, background/resume, and permissions behave correctly.
- Input works on device with acceptable latency.
- Core run loop completes and results screen appears.
- 60 FPS target verified with Profiler on the reference device.
- No unauthorized asset, secret, or debug credential is embedded.
- Target API 36 and 16 KB compatibility requirements are satisfied.
