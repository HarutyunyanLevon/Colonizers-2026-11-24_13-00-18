# ColonizeEarth — Roadmap

M1 is the v1 prototype (v1 prototype: forest planet; v1 build plan). Later milestones are vertical slices; their tasks depend on TBD items and must not proceed past a blocking TBD without logging a question (see CLAUDE.md).

No schedule or development time budget exists. TBD: schedule and time budget per milestone.

---

## M1 — v1 prototype: playable core loop (forest map)
Goal: test click → settlers → progress → research on one flat forest map.

- [ ] M1-T1: Create the Unity project skeleton.
  - Acceptance: Unity 6000.6.8f1 project with URP; Entities, Entities Graphics, Burst, Collections, Mathematics present; assembly definitions per module in 02-tech-design.md#suggested-module-layout; empty boot scene opens without errors; .gitignore for Unity.
  - Relies on: 02-tech-design.md#platform-and-constraints, 02-tech-design.md#suggested-module-layout
  - Size: one Claude Code session
- [x] M1-T2: Game-owned big-number type wrapping Number Forge.
  - Acceptance: one type in `Game.Numbers` with add, multiply, compare, formatting for display, and serialization; no other module references Number Forge directly; edit-mode tests pass. If Number Forge is not yet imported by the owner, log a question and stop at the interface plus tests.
  - Relies on: 02-tech-design.md#platform-and-constraints, 01-gdd.md#economy-numbers
  - Size: one Claude Code session
- [x] M1-T3: Economy core in plain C#.
  - Acceptance: state holds settler count (starts at 1), colonization % and research points; tick adds settler count × progress per settler per second; colonization % caps at 100; research points accrue with progress; tests confirm determinism and caps.
  - Relies on: 01-gdd.md#v1-prototype-scope, 02-tech-design.md#economy-core-plain-c
  - Size: one Claude Code session
- [x] M1-T4: Click and research in the economy core.
  - Acceptance: click adds settlers-per-click (starts at 1); repeatable research "+1 settler per click" costs research points on a configurable cost curve; spending never lowers colonization %; purchase fails cleanly when unaffordable; tests cover all of this.
  - Relies on: 01-gdd.md#v1-prototype-scope, 01-gdd.md#research
  - Size: one Claude Code session
- [ ] M1-T5: Tuning data and IMGUI debug panel.
  - Acceptance: all economy values in one data asset (starting progress per settler per second ≈ 0.0001%); IMGUI panel shows state, edits values live, offers cheats (add settlers, add points, set %); panel and cheats compiled only in development builds.
  - Relies on: 02-tech-design.md#ui, 01-gdd.md#v1-prototype-scope
  - Size: one Claude Code session
- [ ] M1-T6: Numbers-only playtest build.
  - Acceptance: a development build where the full loop is playable through the debug panel (click button, research button); panel shows elapsed time and projected time to 100%; owner can tune toward 20-40 minutes before any art.
  - Relies on: 01-gdd.md#v1-prototype-scope
  - Size: one Claude Code session
- [ ] M1-T7: Flat forest map and camera.
  - Acceptance: one flat map with placeholder ground and tree meshes; camera pans and has limited zoom; one zoom level only. Map size is TBD: use a size that fits the view and log the question.
  - Relies on: 01-gdd.md#v1-prototype-scope, 02-tech-design.md#v1-map
  - Size: one Claude Code session
- [ ] M1-T8: Click spawns ECS settlers that wander.
  - Acceptance: clicking the map raises the economy settler count and spawns placeholder settler entities at the cursor; settlers wander using `ISystem`/`IJobEntity`, no Unity Physics; visible settler count is capped for performance while the economy total stays exact; 60 fps minimum holds in the profiler at the cap.
  - Relies on: 02-tech-design.md#settler-agent-system-ecs, 01-gdd.md#clicking
  - Size: one Claude Code session
- [ ] M1-T9: Progress-driven visual growth.
  - Acceptance: colonized tint spreads outward from the start point proportional to colonization %; trees inside the colonized area are cut and land shows as plowed; visuals are cosmetic and never feed back into the economy.
  - Relies on: 01-gdd.md#v1-prototype-scope
  - Size: one Claude Code session
- [ ] M1-T10: HUD.
  - Acceptance: progress bar with %, settler count (big-number formatted), research button showing cost and disabled when unaffordable; HUD updates every frame from economy state. UI framework TBD: use the conservative default and log the question.
  - Relies on: 01-gdd.md#ui, 02-tech-design.md#ui
  - Size: one Claude Code session
- [ ] M1-T11: Placeholder boss sequence at 100%.
  - Acceptance: on first reaching 100%, a ~15-second scene plays: Heartwood placeholder rises, settlers swarm it, HP bar drains, it falls, "Forest colonized!" banner shows; no failure state; afterwards the player can keep spawning settlers; the sequence does not replay on reload.
  - Relies on: 01-gdd.md#v1-prototype-scope
  - Size: one Claude Code session
- [ ] M1-T12: Save and load economy state.
  - Acceptance: economy state (including boss-seen flag) saves on quit and at an interval, loads on start; no offline progress applied; round-trip test passes.
  - Relies on: 02-tech-design.md#save-data, 01-gdd.md#offline-progress
  - Size: one Claude Code session
- [ ] M1-T13: Playtest instrumentation.
  - Acceptance: development builds log time to 100%, clicks per minute and session end time to a local file; the owner can run a playtest and read the results.
  - Relies on: 02-tech-design.md#testing-approach
  - Size: one Claude Code session

---

## M2 — Scale slice: regions and zoom levels
Goal: prove the scale pillars on a region of the real planet. Deferred from v1: sphere view, zoom levels, region aggregates.

- [ ] M2-T1: Region statistics manager.
  - Acceptance: per-region aggregate state using the big-number type; economy reads totals across regions; tests cover aggregation.
  - Relies on: 02-tech-design.md#region-statistics-manager
  - Size: one Claude Code session
- [ ] M2-T2: Generate and fold individual settlers per region.
  - Acceptance: entering a region at close zoom spawns settlers from its statistics up to the crowd cap with the real total displayed; leaving folds results back; spawning spread over several frames.
  - Relies on: 02-tech-design.md#region-statistics-manager, 02-tech-design.md#zoom-pipeline
  - Size: one Claude Code session
- [ ] M2-T3: Flow fields and density-grid avoidance.
  - Acceptance: settlers move by a shared flow field per region; density-grid avoidance; no pairwise collision; tick adapts 20 → 10 Hz under load with interpolation.
  - Relies on: 02-tech-design.md#settler-agent-system-ecs
  - Size: one Claude Code session
- [ ] M2-T4: Colonization grid.
  - Acceptance: grid stored at the resolution in 02-tech-design.md#world; settlers claim cells; one map drives close-zoom ground tint. Storage model TBD: log the question if it blocks.
  - Relies on: 02-tech-design.md#world
  - Size: one Claude Code session
- [ ] M2-T5: Logarithmic zoom camera with close and mid levels.
  - Acceptance: single detail value from camera height; hysteresis thresholds; mid zoom hides trees and buildings via dithered fade and shows numbered settler groups.
  - Relies on: 02-tech-design.md#zoom-pipeline, 01-gdd.md#scale-and-zoom
  - Size: one Claude Code session
- [ ] M2-T6: Cube-sphere terrain and far zoom.
  - Acceptance: CDLOD terrain on a cube-sphere face set, flat near the camera and curved as it rises; far zoom shows the sphere with colonization texture; only changed tiles uploaded.
  - Relies on: 02-tech-design.md#zoom-pipeline, 02-tech-design.md#world
  - Size: one Claude Code session
- [ ] M2-T7: Heightmap streaming.
  - Acceptance: tiles stream in as the camera approaches and unload beyond the horizon without hitches. Authoring pipeline TBD.
  - Relies on: 02-tech-design.md#world
  - Size: one Claude Code session
- [ ] M2-T8: Performance pass at scale.
  - Acceptance: profiling capture on target-class hardware shows close zoom ≥ 60 fps at the crowd cap; results recorded against the budgets.
  - Relies on: 02-tech-design.md#performance-budgets
  - Size: one Claude Code session

## M3 — Economy slice: buildings and research
Blocked in part by TBDs on building types, buffs and research structure.

- [ ] M3-T1: Houses that spawn settlers.
  - Acceptance: player can place a house; it spawns settlers over time into the economy; rates in tuning data.
  - Relies on: 01-gdd.md#buildings
  - Size: one Claude Code session
- [ ] M3-T2: Research tree data model.
  - Acceptance: data-driven research nodes with costs and effects; existing +1 settler/click migrated into it. Structure and currency TBD.
  - Relies on: 01-gdd.md#research
  - Size: one Claude Code session
- [ ] M3-T3: Research that makes clicks spawn buildings.
  - Acceptance: a research node lets one click place multiple settlers or buildings.
  - Relies on: 01-gdd.md#research, 01-gdd.md#core-loop
  - Size: one Claude Code session
- [ ] M3-T4: Towns from one click.
  - Acceptance: a late research lets a click create a town (group of buildings). Town composition TBD.
  - Relies on: 01-gdd.md#buildings
  - Size: one Claude Code session
- [ ] M3-T5: Building icons at mid zoom.
  - Acceptance: instanced icons fade in as buildings fade out.
  - Relies on: 01-gdd.md#scale-and-zoom, 02-tech-design.md#zoom-pipeline
  - Size: one Claude Code session

## M4 — Biome slice: forest and meadows with bosses

- [ ] M4-T1: Hazard and monster cell flags with visual reactions.
  - Acceptance: lava/water/monster presence as grid flags; settlers react visually. Gameplay effects TBD.
  - Relies on: 01-gdd.md#world-interactions
  - Size: one Claude Code session
- [ ] M4-T2: Boss lair discovery and lore pieces.
  - Acceptance: settlers discover a lair; lore pieces reveal weak and strong spots in UI.
  - Relies on: 01-gdd.md#discovery-and-preparation
  - Size: one Claude Code session
- [ ] M4-T3: Boss fight core.
  - Acceptance: click starts the fight; lair's fixed settler count, most buffed first, teleported to arena; DPS = count × buffs; timer; win at 0 HP, loss at timeout with survivors returning; boss fully heals after loss; clicks spawn no settlers during the fight while buildings keep spawning.
  - Relies on: 01-gdd.md#boss-fights
  - Size: one Claude Code session
- [ ] M4-T4: Player actions in boss fights.
  - Acceptance: clicking the boss and weak spots deals damage; retreat ends the fight as a loss; respawn of dead settlers at specialized buildings.
  - Relies on: 01-gdd.md#fight-math, 01-gdd.md#outcome
  - Size: one Claude Code session
- [ ] M4-T5: Consumables and power-ups.
  - Acceptance: researched recipes craft items from building resource plus mini-boss drops; usable in fights. Item list TBD.
  - Relies on: 01-gdd.md#fight-math
  - Size: one Claude Code session
- [ ] M4-T6: Mini-boss lairs.
  - Acceptance: hand-placed lairs; each fought once; cleared lair becomes a special site with a permanent biome-wide buff and a drop. Buff and drop kinds TBD.
  - Relies on: 01-gdd.md#mini-bosses
  - Size: one Claude Code session

## M5 — Presentation slice
- [ ] M5-T1: Toon shading and screen-space outlines.
  - Acceptance: single palette material, soft toon shading, depth/normal outline pass; cost independent of unit count.
  - Relies on: 01-gdd.md#art-direction, 02-tech-design.md#rendering-layer
  - Size: one Claude Code session
- [ ] M5-T2: Settler model and animation.
  - Acceptance: settler mesh within budget with LODs; animation per the decided approach. Look and animation approach TBD.
  - Relies on: 01-gdd.md#art-direction, 02-tech-design.md#animation
  - Size: one Claude Code session
- [ ] M5-T3: Audio pass.
  - Acceptance: blocked until audio direction is decided.
  - Relies on: 01-gdd.md#audio-direction
  - Size: one Claude Code session

Later milestones (biomes 2-4, Steam integration, release) are TBD.
