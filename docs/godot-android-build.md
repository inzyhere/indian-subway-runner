# Godot Android build

The active Android build is a **debug APK** exported from the Godot 4 project under `godot/`. It uses only free tooling and requires no repository secrets.

The old Unity workflow is not used for this build. The Unity project remains preserved and untouched.

## Workflow

`.github/workflows/godot-android.yml` runs on pushes that change `godot/` or the workflow itself, and it can also be started manually.

The workflow:

1. Downloads Godot `4.7.2-stable` for Linux from the public Godot build release.
2. Downloads and installs matching export templates.
3. Installs Android SDK platform, build tools, platform tools, and licenses on the GitHub-hosted runner.
4. Generates a fresh Android debug keystore with `keytool`.
5. Imports the Godot project headlessly.
6. Exports `builds/IndianSubwayRunner.apk` with `--export-debug Android`.
7. Uploads the APK as a GitHub Actions artifact.

The generated debug keystore uses the public Android debug alias and password. It is disposable CI state, not a credential or signing secret. No Unity license, Unity account, GitHub secret, or release key is required.

## Export preset

`godot/export_presets.cfg` defines the `Android` preset:

- Package ID: `com.inzyhere.indiansubwayrunner`
- Version: `0.1.0` / code `1`
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
export GODOT_ANDROID_KEYSTORE_USER="androiddebugkey"
export GODOT_ANDROID_KEYSTORE_PASSWORD="android"
```

Do not commit generated imports, build output, keystores, or SDK files. `godot/.gitignore` excludes `.godot/`, `builds/`, and `android/`.

## Evidence boundary

A successful export proves that an APK artifact was produced. It does not prove installation, rendering, input, performance, or gameplay behaviour on a device. Those claims require a real-device run and logcat/performance evidence.
