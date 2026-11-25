# Original Indian Starter Asset Pack Integration

Status: importer and optional runtime hooks implemented on a feature branch; engine import, visual placement, audible playback, and device performance are **not yet tested**.

## Source and rights boundary

The separate IndianSubwayRunner_AssetPack_v0.1.zip is an independently generated starter pack. It contains original low-poly GLB models, four modular environment blocks, and temporary synthetic WAV guide voices. It is not extracted from Subway Surfers.

The uploaded Subway Surfers reverse-engineering archive is used only to understand high-level, publicly observable gameplay patterns (three-lane movement, jump/slide timing, obstacle readability, safe-path generation, and progression). Do not copy or redistribute its source code, textures, models, UI, audio, localization, branding, or other proprietary content. The game's current Godot implementation remains an independent implementation.

## Import

Download the starter pack ZIP to the development machine/Termux environment, then run from the repository root:

~~~sh
python3 Tools/Assets/import_starter_asset_pack.py /path/to/IndianSubwayRunner_AssetPack_v0.1.zip
~~~

The importer copies only the pack's GLB, WAV, text, documentation, and manifest files to:

~~~text
godot/assets/indian_pack_v01/
~~~

It rejects unsafe archive paths, does not overwrite existing files by default, and writes an import manifest containing file sizes and SHA-256 hashes. To intentionally replace an existing import, add --overwrite.

## Runtime integration

- indian_asset_pack_runtime.gd attaches the chai stall, benches, suitcases, and four environment blocks as optional set dressing when their imported files exist.
- The original pack's GLBs are Z-up, so the runtime hook rotates them into Godot's Y-up world. Their placement is provisional and requires visual review.
- voice_pack_runtime.gd plays the optional police call at chase start, a station announcement on chapter changes, and occasional criminal taunts. It enforces one-at-a-time playback and cooldowns.
- The existing authored 13-GLB art kit, music, SFX, gameplay, and HUD remain the primary game systems. Missing optional pack files do not prevent the game from loading.

## Asset limitations

The v0.1 pack is prototype/blockout quality. The generic police/criminal characters are unrigged placeholders and are deliberately not used to replace the game's animated characters. The train, track, signal, and OHE assets are retained in the imported pack for future scene-specific use; the current game already has a detailed train and track kit.

The five voice clips are robotic synthetic guides, not final Indian voice acting. They are temporary integration placeholders and must be replaced with properly licensed human recordings before public release.

## Required validation before merging

1. Import the pack in Godot 4.7.2 and confirm all GLBs and WAVs import without errors.
2. Run the headless smoke workflow and the gameplay capture workflow.
3. Inspect every environment block and prop in the 720x1280 captures; adjust rotation, scale, offsets, and segment placement if needed.
4. Confirm voice clips do not overlap, repeat too frequently, or obscure important SFX.
5. Build the Android APK and measure actual frame time, memory, and thermal behaviour on a real device.
6. Keep status at **Implemented — unverified** until those checks pass. Human visual sign-off and device verification are separate gates.
