# Godot Android build

The active Android build is a **debug APK** exported from the Godot 4 project under `godot/`. It uses only free tooling and requires no repository secrets.

The old Unity workflow is not used for this build. The Unity project remains preserved and untouched.

## Workflows

`.github/workflows/godot-android.yml` runs on pushes that change `godot/` or the workflow itself, and it can also be started manually. It produces and uploads a GitHub Actions artifact.

`.github/workflows/godot-android-release.yml` is the artifact-independent delivery path. It is started manually with a release tag and title, builds the same debug APK, writes a SHA-256 checksum, and publishes both files as assets on a GitHub Release using the automatic `GITHUB_TOKEN`. It does not use repository secrets, external credentials, or GitHub Actions artifact storage.

`.github/workflows/godot-headless-smoke.yml` runs manually to import the project and execute a 120-second headless smoke test with `--debug --ignore-error-breaks`; it uploads the smoke log as an artifact. The release workflow now runs the same smoke gate before export and fails if the log contains `SCRIPT ERROR`, `Parse Error`, or any `ERROR:` line.

`.github/workflows/godot-gameplay-capture.yml` runs manually under `xvfb` with Godot's Dummy audio driver, passes `-- --capture-gameplay --capture-dir=<artifact dir>` to the game, fails on `SCRIPT ERROR`, `Parse Error`, or any `ERROR:` line, requires at least five `gameplay_*.png` frames, and uploads the frame set plus `godot-capture.log`. Run `45518284923` produced five 720x1280 RGBA frames that were nonblank and changing; this is a capture/smoke result, not aesthetic verification.

The Android export workflows:

1. Use `ubuntu-latest`.
2. Download Godot `4.7.2-stable` for Linux from the public Godot build release.
3. Download and install matching export templates.
4. Install Android SDK platform, build tools, platform tools, and licenses on the GitHub-hosted runner.
5. Generate a fresh Android debug keystore with `keytool`.
6. Import the Godot project headlessly.
7. Export `builds/IndianSubwayRunner.apk` with `--export-debug Android`.

The generated debug keystore uses the public Android debug alias and password. It is disposable CI state, not a credential or signing secret. No Unity license, Unity account, repository secret, release key, or signing password is required.

The release workflow needs `contents: write` so that the automatic token can create a Release and upload assets. It keeps the existing build workflow unchanged with `contents: read`.

## CI blocker history and current path

The first private-repository run, `45500464511`, failed before any step with GitHub's account-billing/spending-limit message. After the audit approved public visibility, the repository was switched to public so the standard hosted-runner path can be used without adding payment methods or increasing spending limits. The first public release run, `45504922295`, reached export but failed because only the debug keystore path environment variable was recognized; `8c81824` corrected the Godot 4.7.2 variable names. Release run `45505392990` then succeeded and published the first APK and checksum at [godot-debug-apk](https://github.com/inzyhere/indian-subway-runner/releases/tag/godot-debug-apk). After the blank-screen/portrait hotfix, release run `45508181618` replaced those assets with the corrected `0.1.1` / code `2` build. Release run `45510290459` replaced them again with the expanded clean-room gameplay build, `0.1.2` / code `3`. After headless smoke exposed the real blank-screen cause, smoke run `45512471537` passed cleanly and release run `45512629961` replaced the assets with `0.1.3` / code `4`. The owner rejected the flat `0.1.3` visuals, so the authored GLB visual pass was built and published as `1.0.0` / code `5` by release run `45519025297`.

The assets are displayed as `IndianSubwayRunner-debug.apk` and `IndianSubwayRunner-debug.apk.sha256`, with file names `IndianSubwayRunner.apk` and `IndianSubwayRunner.apk.sha256`. The currently published APK SHA-256 is `9db172007c8a9a25f1bfdb4548804e1bfd01de3c66a0d15cd751f0aedccd6bc7` and matches the published checksum; it is `1.0.0` / code `5` and includes the expanded clean-room gameplay, runtime fixes, and 13 authored GLB visual assets. The published checksum line references `builds/IndianSubwayRunner.apk`; the release asset is displayed as `IndianSubwayRunner-debug.apk`. The owner reported that the `0.1.2` APK showed the Godot splash and then a blank screen and later rejected the flat `0.1.3` visuals. Headless smoke traced the blank screen to GDScript parse errors, invalid `MeshInstance3D.material` property assignments, and `look_at()` before the camera entered the scene tree; all are fixed. The current build has not yet been retested on the device or visually signed off. Headless smoke, gameplay capture, export, checksum, and package metadata are tested for `1.0.0`; installation, gameplay, human visual acceptance, and immersive fullscreen behaviour are not.

## Export preset

`godot/export_presets.cfg` defines the `Android` preset:

- Package ID: `com.inzyhere.indiansubwayrunner`
- Version: `version/name="1.0.0"` / `version/code=5`
- Orientation: portrait (`screen/orientation=1` and `window/handheld/orientation=1`)
- Display: immersive fullscreen with a `720x1280` portrait viewport and `canvas_items`/`expand` stretch settings
- Architecture: ARM64
- Gradle custom build: disabled
- Export path: `builds/IndianSubwayRunner.apk`

## Local commands

With Godot 4.7.2 and Android export templates installed:

```sh
godot --headless --path godot --editor --quit
godot --headless --debug --ignore-error-breaks --path godot --quit-after 120
godot --headless --path godot --export-debug Android "$PWD/builds/IndianSubwayRunner.apk"
```

Set these variables for local debug signing if the machine does not already have a debug keystore:

```sh
export GODOT_ANDROID_KEYSTORE_DEBUG_PATH="$HOME/.android/debug.keystore"
export GODOT_ANDROID_KEYSTORE_DEBUG_USER="androiddebugkey"
export GODOT_ANDROID_KEYSTORE_DEBUG_PASSWORD="android"
```

Do not commit generated imports, build output, keystores, or SDK files. `godot/.gitignore` excludes `.godot/`, `builds/`, and `android/`.

## Evidence boundary

A successful export proves that an APK artifact was produced. It does not prove installation, rendering, input, performance, or gameplay behaviour on a device. Those claims require a real-device run and logcat/performance evidence.
