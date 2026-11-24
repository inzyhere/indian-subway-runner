# Editor

Editor tooling source for scene generation and Android APK builds exists in `SceneBootstrapGenerator.cs` and `AndroidBuild.cs`. `AndroidBuild.BuildCiApk()` generates scenes before building for CI. Unity execution, compilation, and APK output are not claimed until verified in Unity.
