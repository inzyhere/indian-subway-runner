# Pull Request

## Summary

<!-- Describe the change in one or two sentences. -->

## Related issue

Closes #

## Final-stack checklist

- [ ] Change uses Unity 6000.0.86f1 or newer 6000.0 LTS patch, C#, URP, and Android-compatible practices.
- [ ] No Kotlin gameplay, libGDX, Three.js, WebView gameplay, React Native, Flutter, or unapproved engine change introduced.
- [ ] Unity-generated Gradle output was not hand-edited as gameplay source.
- [ ] Architecture follows [ADR-0003](docs/decisions/architecture-decisions.md).

## Status truth

- [ ] No feature is marked `Tested` without passing EditMode or PlayMode evidence.
- [ ] No feature is marked `Verified` without build or real-device evidence.
- [ ] Placeholders, disabled flags, and planned ideas are described honestly.

## Validation

- [ ] Unity import/compiler result attached, if Unity was available.
- [ ] EditMode results attached, if tests were changed.
- [ ] PlayMode results attached, if runtime behaviour changed.
- [ ] Android build/device evidence attached, if platform behaviour changed.
- [ ] Static checks completed for Markdown, JSON, YAML, XML, and paths when no Unity environment was available.

## Assets, privacy, and security

- [ ] All new art, audio, dialogue, fonts, and packages are original or licensed with provenance.
- [ ] No scraped audio, protected game content, decompiled output, copied characters, or cloned voices.
- [ ] No secret, token, keystore, signing password, API key, or private asset committed.
- [ ] No telemetry, ad, purchase, or analytics SDK added without an accepted ADR.

## Documentation

- [ ] Relevant docs updated.
- [ ] Mermaid diagrams remain readable.
- [ ] Status tables and README/PROJECT_STATUS remain consistent.
- [ ] `CHANGELOG.md` updated when required by the ADR rules.

## Review focus

<!-- List what reviewers should inspect closely. -->
