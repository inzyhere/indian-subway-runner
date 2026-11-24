# Assets

Unity source-of-truth folder. Do not place generated Gradle files, Kotlin gameplay code, LibGDX dependencies, secrets, or unauthorized third-party assets here.

## Current contents

- `Scripts/` — C# runtime assemblies with `.asmdef` boundaries.
- `Tests/EditMode/` — pure-logic tests using NUnit and Unity Test Framework.
- `Tests/PlayMode/` — planned scene/physics/input tests; no tests are claimed until written and passing.
- `Art/`, `Audio/`, `Data/`, `Prefabs/`, `Scenes/`, `Settings/URP/` — planned content folders; see `docs/environment-assessment.md`.

The repository currently contains no Unity scenes, prefabs, art, audio, or imported assets. These will be added only after Unity Editor is available and the content is verified.
