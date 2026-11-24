# Environment Design

> Status: Planned; no Unity scene, station, train, or environment prefab is implemented.

## Visual goal

A genuine 3D Indian railway world that reads clearly at speed: layered platforms, local and express trains, colourful signage, tea stalls, overhead wires, footbridges, crowded platforms, and city/rural transitions. The world must feel original and energetic without copying any commercial game’s maps or assets.

## Core environment grammar

- Three playable lanes aligned with railway tracks.
- Clear lane boundaries through rails, sleepers, and lighting.
- Obstacles silhouettes readable at speed.
- Safe paths must remain possible; procedural generation must validate every chunk.
- Environment art supports gameplay clarity, not decoration at the cost of reaction time.

## Planned biomes

### Morning Suburban Platform

- Warm low sunlight, calm palette, commuters, tea stalls, cycle stands.
- Teaches basic lane changes and jumping.

### Busy Junction

- Dense crowd, platform vendors, service carts, dynamic announcements.
- Introduces moving hazards and tighter timing.

### Express Yard

- Parallel tracks, parked coaches, maintenance props, signal towers.
- Introduces train-based obstacles and longer sightlines.

### Elevated City Rail

- City skyline, flyovers, market roofs, hoardings with original text.
- Introduces verticality in background and denser prop variety.

### Night Terminal

- Cool lighting, illuminated signage, platform lamps, final chapter energy.
- Combines all mechanics with high-speed escalation.

## Train design

- Distinct local, express, and freight silhouettes.
- Original liveries; no real railway branding unless licensed.
- Clear front/rear lighting cues for approaching hazards.
- Modular coaches to control memory and draw calls.
- Physics colliders simplified and deterministic.

## Props and set dressing

Planned original Blender props:

- Tea stall, snack cart, luggage, benches, signboards, trash bins, water tank, signal post, footbridge, railing, overhead wire supports, scooter, bicycle, street dog, potted plants, sari/shirt colour variants for crowd.

Rules:

- Props outside the playable corridor use cheap meshes and LODs.
- Near-corridor props must not visually confuse obstacles.
- All text must be original and legible enough to support Indian identity.
- No Subway Surfers graffiti style, characters, trains, or map layouts.

## Lighting and URP

- URP asset per quality tier: Low, Medium, High.
- Baked directional lighting where possible.
- Minimal real-time lights.
- Fog for depth and chunk transitions.
- Soft shadows only where affordable.
- Post-processing: gentle bloom, colour grading, vignette; no effects that hide obstacles.

## Streaming and pooling

- World is built from reusable chunks.
- Each chunk has a validated obstacle graph with at least one safe path.
- Chunks recycle behind the player.
- Trains, obstacles, collectibles, and crowd use pools.
- No per-frame allocation in spawning or recycling.

## Verification criteria

A biome is Implemented only when its prefabs exist in Unity. It is Tested when PlayMode tests validate chunk spawning and collision. It is Verified when real-device captures and Profiler data demonstrate acceptable performance and readability.
