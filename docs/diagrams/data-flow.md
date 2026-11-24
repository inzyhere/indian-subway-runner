# Data Flow Diagram

> **Status:** Planned — target Unity runtime data flow. No runtime is running and no data has been verified in Unity.
> **Last updated:** 2026-11-25
> **Authority:** [ADR-0003](../decisions/architecture-decisions.md) — Unity 6000.0.86f1+, C#, URP, Android.

This diagram describes how data moves in the intended Unity implementation. It is a **design model**, not a trace of running code. Solid lines are planned v1 flows; dashed lines are reserved future boundaries that are not implemented or enabled.

## 1. Runtime data flow overview

```mermaid
flowchart TD
    subgraph AUTH["Authoring / source of truth"]
        SO["ScriptableObject assets<br/>Assets/Data/*"]
        AD["Addressables groups<br/>Chapters / Audio / Core"]
        PR["Unity Prefabs<br/>Characters, Trains, Obstacles, UI"]
        PL["Player Settings<br/>ProjectSettings/"]
    end

    subgraph BOOT["Boot + GameFlow"]
        GF["GameFlow bootstrap<br/>persistent services"]
        CFG["Validated config snapshots"]
        LS["Load / release coordinator"]
    end

    subgraph RUN["Game scene runtime"]
        IN["Touch / pause input<br/>Update sampling"]
        GM["Gameplay rules<br/>lanes, score, difficulty"]
        PG["ProceduralGeneration<br/>seeded chunk descriptors"]
        PO["Pool manager<br/>bounded instances"]
        PH["Unity Physics<br/>FixedUpdate triggers"]
        PS["Player state<br/>lane / jump / slide"]
        SC["Score + combo<br/>Core rules"]
        PU["PowerUp timers<br/>Core rules"]
    end

    subgraph PRES["Presentation"]
        AN["Animator parameters"]
        AU["AudioMixer groups"]
        DL["Dialogue + announcements"]
        UI["HUD / menu / results"]
        RD["URP renderer"]
    end

    subgraph PLAT["Android boundary"]
        AI["AndroidIntegration"]
        OS["OS lifecycle / focus / safe area / back"]
    end

    SO -->|"load at Boot"| GF
    GF --> CFG
    PR --> AD
    AD --> LS
    LS -->|"load at scene / chapter boundary"| PO
    PL --> GF

    IN --> GM
    CFG --> GM
    CFG --> PG
    PG -->|"chunk descriptor"| PO
    PO -->|"activate prefabs"| PH
    IN --> PS
    PS --> PH
    PH -->|"collision / trigger events"| GM
    GM --> SC
    GM --> PU
    SC --> UI
    PU --> UI
    PS --> AN
    GM --> AU
    DL --> AU
    DL --> UI
    AN --> RD
    PO --> RD
    UI --> RD

    OS <--> AI
    AI -->|"read-only lifecycle signals"| GF
    GF --> UI

    SC -.->|"future ADR-0004"| SAVE["SaveSystem / local persistence"]
    GM -.->|"future ADR-0006"| AN["Analytics counters<br/>disabled in v1"]
    UI -.->|"future ADR-0007"| MON["Monetization boundary<br/>disabled in v1"]
```

## 2. Run-start sequence

```mermaid
sequenceDiagram
    participant B as Boot scene
    participant GF as GameFlow
    participant D as ScriptableObject data
    participant AD as Addressables
    participant M as MainMenu scene
    participant G as Game scene

    B->>GF: create persistent bootstrap
    GF->>D: load and validate config assets
    D-->>GF: immutable config snapshots
    GF->>AD: initialize required groups
    GF->>M: load MainMenu
    M->>GF: StartRun / StartChapter
    GF->>AD: load chapter prefabs and audio
    AD-->>GF: loaded handles
    GF->>G: load Game with run config
    G->>G: pool warmup, then start FixedUpdate loop
```

## 3. Per-frame and per-fixed-step flow

```mermaid
flowchart LR
    subgraph U["Update — presentation / sampling"]
        I["Sample input"] --> BUF["Buffer action"]
        V["Visual smoothing<br/>camera, spin"] --> R["URP render"]
    end

    subgraph F["FixedUpdate — authoritative"]
        BUF --> LANE["Resolve lane target"]
        LANE --> MOVE["Move player rigidbody"]
        MOVE --> TRIG["Physics triggers"]
        TRIG --> OUT["Apply collision outcome"]
        OUT --> SCORE["Score / combo / pickup"]
        OUT --> DEATH["Game over if fatal"]
    end

    F -->|"state snapshot"| U
    OUT --> P["Pool return / reacquire"]
    P --> R
```

`Update` never changes lane identity or collision results; it only samples input and smooths presentation. `FixedUpdate` owns authoritative movement and triggers.

## 4. Data ownership rules

| Data | Owner | Mutability during run |
| --- | --- | --- |
| Tuning configs | `Core` / `Gameplay` | Immutable snapshots |
| Chapter definitions | `Progression` | Immutable |
| Dialogue / announcement metadata | `Dialogue`, `Announcements` | Immutable; selection state only |
| Player run state | `Player` | Mutable within run |
| Chunk descriptors | `ProceduralGeneration` | Mutable within run, seeded |
| Pooled instances | Pool manager | Recycled, never duplicated |
| Score / combo | `Core` rules | Mutable within run |
| Save data | `SaveSystem` | Future ADR-0004; no format claimed |
| Analytics / monetization | Passive future boundaries | Disabled in v1 |

## 5. Cross-document index

- [technical-architecture.md](../technical-architecture.md) — assemblies, scenes, data assets, Android boundary
- [system-architecture.md](./system-architecture.md) — full Unity system graph
- [gameplay-flow.md](./gameplay-flow.md) — scene and run flow
- [game-design-document.md](../game-design-document.md) — gameplay rules
- [story-and-narrative-design.md](../story-and-narrative-design.md), [audio-system.md](../audio-system.md) — content metadata
