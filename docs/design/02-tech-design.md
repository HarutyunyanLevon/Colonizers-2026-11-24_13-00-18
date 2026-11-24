# ColonizeEarth — Technical Design

There is no card with the Tech role. This document is built from project memory, "Plan and architecture: Unity DOTS" (Lucid), "Spec: Tech advice: engine, zoom limits, world size" (Lucid), "v1 prototype: forest planet" and "v1 build plan". The Mist "Tech advice" card is used only where the Lucid spec adopted it.

## Platform and constraints
| Item | Value | Source |
|---|---|---|
| Platform | PC only, Steam | memory |
| Engine | Unity 6000.6.8f1, URP | memory; Plan and architecture |
| DOTS | Entities and Entities Graphics (core packages, no version pin), Burst, Jobs, Collections, Mathematics | Plan and architecture |
| Target hardware | Current Steam Hardware Survey median (roughly RTX 3060/4060-class, 8 GB VRAM, 6-core CPU, 16 GB RAM; verify against the survey) | memory; Spec: Tech advice |
| Frame rate | Close zoom uncapped with a 60 fps minimum on the target PC | memory |
| Planet | 6 × 4096² cells at 2 m (~100 million cells, ~32 km around), hand-authored, same every run | memory |
| Big numbers | Mantissa + exponent, unlimited range, via the Number Forge asset (imported by the owner) behind one game-owned type | memory; v1 prototype |
| Debug | IMGUI debug UI, tuning panel and cheats in development builds only | v1 prototype |
| Steam integration | TBD (Steamworks wrapper, achievements, cloud saves) | — |
| Input | Mouse (click, pan, zoom); key bindings TBD | v1 build plan |

### API rules (Unity 6.5+)
- Use `ISystem`, `IJobEntity` and `SystemAPI.Query`.
- Do not use `Entities.ForEach`, `Job.WithCode` or Aspects; they were removed in Unity 6.5.
- No Unity Physics for settlers. Movement, lava, water, monsters and other interactions are simulated without a physics engine.

## Architecture
Three layers (Plan and architecture: Unity DOTS):

### Economy core (plain C#)
- v1: economy in plain C#, not ECS (v1 prototype).
- Holds settler count, colonization progress, research points, research levels, clicks-per-spawn, all as the game-owned big-number type.
- Pure, deterministic tick function so it can be unit-tested without the engine.
- Tunable values in one data asset exposed to the tuning panel.

### Settler agent system (ECS)
- Component: `SettlementAgent { position, velocity, state, properties }`.
- Flow field per region; no individual A* pathfinding.
- Density-grid avoidance (the architecture card places it on GPU compute); no pairwise collision.
- Simulation tick adaptive: 20 Hz, dropping to 10 Hz under heavy load; motion interpolated between ticks (Spec: Tech advice).
- Hazards (lava, water, cold) are cell flags on the colonization grid; lookups, no physics.
- v1: settlers spawn at the cursor and wander for show; they do not drive progress.

### Region statistics manager
- Off-screen and distant settlers are per-region aggregates: counts, death rates, production.
- `SettlementRegion { count, rate_data, buffer_size }`.
- When the camera enters a region at close zoom, individual settlers are generated from its statistics; when it leaves, results (deaths, newly colonized cells) fold back.
- Crowd cap: in a crowded area at close zoom, show a fixed maximum sample per area plus the real total.
- TBD: exact field list and the big-number vs integer choice for region counts (the architecture card says "integer counts"; memory requires big numbers for settler counts).
- TBD: tuning method for consistency between regional death rates and the individual simulation (Spec: Tech advice suggests running the individual simulation on small test cases).

### Rendering layer
- GPU-driven instanced drawing through Entities Graphics; LOD index in a `RenderLayer` component.
- Single material with a palette texture; vertex colours for variation.
- Screen-space outline pass post-render (edge detection on depth and normals).
- Blob shadow for every settler; one shadow cascade for terrain and buildings near the camera.
- Transparency: dithered fades only. Gradient sky and simple fog.

### Animation
- TBD (contradiction). "Plan and architecture: Unity DOTS" says vertex animation only, no skeletons. "Spec: Tech advice: engine, zoom limits, world size" says hybrid: skeletal animation near the camera (limit about 500 settlers), vertex-shader animation beyond. v1 uses placeholder meshes; until decided, use vertex-shader animation (the conservative option common to both).

### UI
- v1 debug: IMGUI.
- TBD: production UI framework (UI Toolkit or uGUI; not named by any card).
- Mid-zoom number badges from one distance-field font atlas; building icons as instanced billboards (Spec: Tech advice, Mist; not confirmed).

## Data

### World
- Cube-sphere planet, six faces. Full size 6 × 4096² ground cells at ~2 m.
- Faces partitioned into tiles (example: 256×256 cells per tile) (Plan and architecture).
- Heightmaps compressed (the card gives Draco or quantized 8-bit as examples) and streamed per chunk as the camera approaches; unloaded past the horizon.
- Colonization grid: 6 × 1024² cells at 8 m (~6 million cells, ~6 MB).
- Trees and objects are not stored individually; generated per area from density maps with a fixed hash, identical every run.
- TBD: colonized-land storage model. The "Spec: Title and description of game (scale and tech)" leaves it open; the Mist Tech advice recommends both (regions for economy, per-cell grid for visuals). The Lucid tech spec specifies one shared colonization map used at all zoom levels.
- TBD: planet authoring pipeline (tools and import format). Mist advice mentions Gaea or World Machine plus a paint tool; not decided.
- Biome regions map to groups of cube-sphere tiles (World: biomes and regions, Mist; unconfirmed).

### v1 map
- One flat forest map, one zoom level, camera pan and limited zoom.
- TBD: v1 map size in cells and metres.

### Save data
- v1: economy state only (v1 build plan).
- Later: region statistics only; world state folded to aggregates; on load, individual agents within close zoom are reconstructed from current counts (Plan and architecture).
- Contradiction: the architecture card says "offline deaths/colonizations applied first" on load. Memory states no offline progression in the first version. Do not implement offline application. Listed in 04-open-questions.md.
- TBD: save format, location, versioning, Steam Cloud.

## Zoom pipeline
- One continuous camera with logarithmic zoom; a single detail value driven by camera height.
- Thresholds from the architecture card: close < 5 m, mid 5-100 m, far > 100 m. TBD: verify these values; they appear small relative to a ~32 km planet and may refer to a different unit.
- Separate zoom-in and zoom-out thresholds (hysteresis) to avoid flicker.
- Settlers pre-generated ahead of the threshold, work spread over several frames.
- Settler aggregation/de-spawning smoothed over 2-3 seconds.
- Terrain: CDLOD quadtree on the cube-sphere; flat near the camera, blended to curved in the vertex shader as the camera rises.
- Trees and buildings fade out with dithered transparency while building icons fade in (memory).
- Settlers slide toward the group centre while the number badge grows; reversed on zoom-in.
- Precision: doubles on the CPU, camera-relative positions on the GPU (Mist advice; TBD confirm).

| Zoom | Render content |
|---|---|
| Close | Individual agents, full toon shading, outlines, shadows, terrain detail |
| Mid | Grouped badges (hundreds), instanced building icons, dithered fade-out of trees/objects |
| Far | Sphere mesh with colonization texture atlas, atmosphere shader; upload only changed tiles |

## Performance budgets
Estimates to confirm in prototypes (Spec: Tech advice):

| Item | Estimate |
|---|---|
| Simulated settlers at close zoom | 50,000-150,000 |
| Drawn settlers at close zoom | 20,000-100,000 (with LOD) |
| Tree/object instances visible | 100,000-300,000 |
| Settler mesh | ~300-500 triangles, 2-3 LODs (Mist advice) |
| Mid-zoom badges | hundreds to a few thousand |

Main costs are triangle count and shadows, not draw calls.

## Suggested module layout
Proposed folder/assembly split for Claude Code; adjust only if it blocks work.

| Module | Responsibility |
|---|---|
| `Game.Numbers` | Game-owned big-number type wrapping Number Forge |
| `Game.Economy` | Plain C# economy state, tick, click, research, tuning data |
| `Game.Settlers` | ECS components and systems for settler spawn and movement |
| `Game.World` | Map, colonization grid, tint, trees |
| `Game.Camera` | Pan and zoom |
| `Game.UI` | HUD |
| `Game.Boss` | v1 placeholder boss sequence; later fight system |
| `Game.Save` | Save/load |
| `Game.Debug` | IMGUI debug, tuning panel, cheats (development builds only) |

## Testing approach
- Unity Test Framework edit-mode tests for `Game.Numbers` and `Game.Economy` (deterministic tick, click, research cost, research points never reduce progress, save round-trip).
- Play-mode smoke tests for spawning and HUD binding where feasible.
- Performance: profile in development builds on target-class hardware; close zoom must stay at or above 60 fps.
- Pacing: playtest time to 100% (target 20-40 minutes), clicks per minute, and when players stop (v1 build plan).
- TBD: CI, automated performance capture.
