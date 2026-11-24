# Character Documentation

> Status: Planned; no character model, rig, controller, or animation is implemented.

## Design rule

All characters must be original creations inspired only by publicly observable endless-runner archetypes and the project’s Indian railway premise. Do not copy Subway Surfers character designs, commercial mascot silhouettes, exact celebrity likenesses, voices, logos, or protected costume details.

## Character roster

### ACP Pradyuman-inspired pursuer

- Role: player character and chasing authority figure.
- Identity: energetic, determined, witty, instantly readable silhouette.
- Visual direction: practical detective coat or vest, cap, moustache-inspired facial hair handled as original stylized design, compact proportions for mobile readability.
- Palette: khaki, navy, warm skin tones, high-contrast accessories.
- Required animations: run, lane shift, jump, slide, stumble/recovery, power-up reaction, victory pose, defeat recovery.
- Asset source: original Blender `.blend`, exported FBX/GLB, Unity Animator controller.

### The criminal

- Role: uncatchable target visible ahead of the player.
- Identity: mischievous, agile, teasing but non-violent.
- Visual direction: distinctive colour-blocking and silhouette so players can track them at speed; avoid copying existing game characters.
- Behaviour: stays ahead, taunts through animation cues and occasional original one-liners, escapes at episode end.
- Required animations: run, look-back taunt, vault, lane switch, escape flourish.
- Asset source: original Blender `.blend`, exported FBX/GLB, Unity Animator controller.

### Railway staff and crowd

- Role: environmental life and Indian station identity.
- Variants: platform vendor, ticket checker, porter, commuter, child, elderly passenger, station guard.
- Behaviour: mostly background motion and reactive dodge animations; no complex NPC AI in v1.
- Visual direction: varied clothing, colour, posture, and props; avoid repeated clones.
- Asset source: modular Blender source and Unity prefabs.

## Rig and animation conventions

- Humanoid rigs where Unity Animator retargeting helps.
- Consistent scale: one character unit equals one metre unless the Unity project standard changes.
- Root at feet, forward along +Z unless Unity import convention requires otherwise.
- Separate mesh, rig, and animation FBX files where that improves import reliability.
- LOD0 for gameplay camera distance; optional LOD1 for crowd.
- Animation event hooks for footfalls, jumps, slide impact, and dialogue cues.

## Cosmetic progression

Planned cosmetic categories:

- ACP jackets/caps.
- Criminal disguises.
- Luggage and prop variants.
- Footwear.
- Clue-themed accessories.

Cosmetics must not change collision size, hitboxes, lane rules, or competitive fairness. Monetization remains planned and disabled until core stability is verified.

## Status tracking

| Item | Status |
| --- | --- |
| Concept direction | Planned |
| Blender source model | Not started |
| FBX/GLB import | Not started |
| Prefab | Not started |
| Animator controller | Not started |
| PlayMode test | Not started |
| Verified Android display | Not started |
