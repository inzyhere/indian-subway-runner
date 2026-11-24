# Security Policy

> **Last updated:** 2026-11-25  
> **Status:** Planned — policy only; no release, remote, audit, or verified build exists.

This policy covers the planned Unity/URP Android project under [ADR-0003](docs/decisions/architecture-decisions.md). It does not claim protection for third-party services, store infrastructure, or user devices. No version has been released and no security audit, penetration test, dependency scan, or threat-model review has been performed.

## Supported versions

| Version | Status | Security support |
| --- | --- | --- |
| None | Not released | No support window. |
| Future v1 | Planned | Support policy must be defined before public release. |

The legacy Kotlin/libGDX debug APK is not a release, not supported, and not the final game.

## Reporting a vulnerability

Do not open a public issue. Until a GitHub remote and private reporting channel exist after `gh auth login`, contact the owner through an existing private channel. Include affected component, impact, reproduction steps, device/OS/app version if any, redacted evidence, and whether the issue is already public. Never send signing keys, tokens, credentials, or private user data with a report. Coordinate disclosure: acknowledge, triage, fix under [TESTING.md](TESTING.md) gates, then publish details only with a practical fix path and reporter consent.

## Secrets, signing, and GitHub

- Never commit tokens, API keys, passwords, `.keystore`/`debug.keystore` files, or signed artifacts. A debug keystore exists locally in the home directory and must stay out of source control.
- Release signing must come from secure local storage or CI secrets; never from Gradle/project files committed to the repository.
- Keep the GitHub repository private. `gh` is authenticated as `inzyhere`; `origin` is the private `inzyhere/indian-subway-runner` repository. Verify no secret is present before each push.
- Do not paste credentials or unreleased licensed asset files into issues, docs, or logs.

## Asset licensing

- All art, models, textures, fonts, music, sound effects, dialogue, and announcements must be original, commissioned, or licensed for Android game distribution.
- Blender `.blend` sources must be original; imports must be FBX/GLB with PNG/JPEG or optimized textures and URP materials. Record creator, source, license, date, modifications, path, and reviewer acceptance for every asset group.
- Prohibited: ripped or decompiled game content, scraped audio/memes, unlicensed third-party assets, celebrity voice clones, and protected character likenesses. ACP-inspired writing must remain original and avoid copying protected lines or mannerisms.
- Publicly observable gameplay mechanics may inspire independent implementations; no protected game content may ship.

## Privacy baseline

| Area | v1 baseline | Status |
| --- | --- | --- |
| Saves | Local-only, versioned format (future ADR-0004); no cloud in v1. | Planned |
| Accounts | No login or user identity system. | Planned |
| Analytics | No network calls; any future telemetry needs its own ADR. | Planned |
| Device identifiers | No collection. | Planned |
| Voice | Original/licensed local clips only; no uploads and no voice cloning. | Planned |
| Permissions | Only those required by a verified feature. | Planned |

Adding network activity, analytics, cloud save, leaderboards, ads, or purchases requires a documented privacy review before implementation is complete.

## Future purchase and plugin rules

Monetization is future work. If introduced: entitlements require server-side receipt verification; client claims are never authoritative; debug entitlements must not ship; premium state fails closed when verification is unavailable; store SDKs stay behind an approved boundary. Native Android plugins are permitted only when genuinely needed, must live in `Assets/Plugins/Android/`, and must be rebuilt for 16 KB page-size compatibility. No plugin may silently collect data or bypass user settings.

## Audit status

No audit or store submission has occurred. Any future finding must be recorded with owner, severity, evidence, and resolution before the affected feature can be marked **Verified**. See [CONTRIBUTING.md](CONTRIBUTING.md) for contribution rules and [PROJECT_STATUS.md](PROJECT_STATUS.md) for truthful current status.
