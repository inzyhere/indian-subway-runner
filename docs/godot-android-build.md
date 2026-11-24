# Godot Android build

The active Android build is a **debug APK** exported from the Godot 4 project under `godot/`. It uses only free tooling and requires no repository secrets.

The old Unity workflow is not used for this build. The Unity project remains preserved and untouched.

## Workflows

`.github/workflows/godot-android.yml` runs on pushes that change `godot/` or the workflow itself, and it can also be started manually. It produces and uploads a GitHub Actions artifact.

`.github/workflows/godot-android-release.yml` is the artifact-independent delivery path. It is started manually with a release tag and title, builds the same debug APK, writes a SHA-256 checksum, and publishes both files as assets on a GitHub Release using the automatic `GITHUB_TOKEN`. It does not use repository secrets, external credentials, or GitHub Actions artifact storage.

Both workflows:

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

The first private-repository run, `45500464511`, failed before any step with GitHub's account-billing/spending-limit message. After the audit approved public visibility, the repository was switched to public so the standard hosted-runner path can be used without adding payment methods or increasing spending limits. The first public release run, `45504922295`, reached export but failed because only the debug keystore path environment variable was recognized; `8c81824` corrected the Godot 4.7.2 variable names. Release run `45505392990` then succeeded and published the APK and checksum at [godot-debug-apk](https://github.com/inzyhere/indian-subway-runner/releases/tag/godot-debug-apk).

The assets are displayed as `IndianSubwayRunner-debug.apk` and `IndianSubwayRunner-debug.apk.sha256`, with file names `IndianSubwayRunner.apk` and `IndianSubwayRunner.apk.sha256`. The downloaded APK SHA-256 matches the published checksum. This proves artifact export and Release delivery only; it does not prove installation or gameplay. The owner reported that this first APK showed the Godot splash and then a blank landscape screen, so it is not playable. A runtime parse-error fix and portrait/fullscreen settings are pending rebuild.

## Export preset

`godot/export_presets.cfg` defines the `Android` preset:

- Package ID: `com.inzyhere.indiansubwayrunner`
- Version: `version/name="0.1.1"` / `version/code=2`
- Orientation: portrait (`screen/orientation=1` and `window/handheld/orientation=1`)
- Display: immersive fullscreen with a `720x1280` portrait viewport and `canvas_items`/`expand` stretch settings
- Architecture: ARM64
- Gradle custom build: disabled
- Export path: `builds/IndianSubwayRunner.apk`

## Local commands

With Godot 4.7.2 and Android export templates installed:

```sh
godot --headless --path godot --editor --quit
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
