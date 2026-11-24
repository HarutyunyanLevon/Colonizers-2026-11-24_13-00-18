# ColonizeEarth — Open Questions

Each item: question, why it matters, cards that touch it.

## Contradictions between sources

| # | Question | Why it matters | Cards |
|---|---|---|---|
| C1 | Research currency: does research cost earned colonization progress (memory) or separate research points that accrue with progress (v1)? | Defines the core economy beyond v1. | memory; v1 prototype: forest planet; v1 build plan |
| C2 | Tone: "wondrous discovery of an alien-feeling Earth" vs "light, cartoonish and humorous" with Fall Guys-style settlers. Do both hold (wondrous world, comic settlers)? | Drives art, audio, writing. | Spec: Title and description of game; Spec: … (scale and tech); The Settler; Discovery Frontier |
| C3 | Settler death: can settlers die from hazards, monsters or starvation, or only in boss fights? The Settler (Mist) says no death at all. | Affects economy, regional death rates and tone. | Title and description of game; Spec: Title and description of game; The Settler; memory |
| C4 | Animation: vertex-only (architecture card) or hybrid skeletal near camera up to ~500 settlers (tech spec)? | Determines the settler rendering pipeline and art production. | Plan and architecture: Unity DOTS; Spec: Tech advice |
| C5 | Load sequence applies "offline deaths/colonizations" (architecture card) vs no offline progression in the first version (memory). | Save/load behaviour. Current rule: no offline application. | Plan and architecture: Unity DOTS; memory |
| C6 | Region counts as integers (architecture card) vs big-number type for settler counts (memory). | Overflow and data layout. | Plan and architecture: Unity DOTS; memory |
| C7 | v1: do settlers "claim tiles" (build plan item 6) or is the tint purely driven by progress % (v1 card)? | v1 task M1-T8/M1-T9 behaviour. Current rule: progress-driven tint. | v1 build plan; v1 prototype: forest planet |
| C8 | Win condition: whole planet colonized (Lucid spec) vs no defined end. | Long-term goal and content scope. | Spec: Title and description of game; v1 prototype |
| C9 | Game name: ColonizeEarth vs a "... Frontier" name. | Branding, Steam page. | Discovery Frontier |
| C10 | Feel: memory says satisfaction; Reverie card records "frantic". Memory is treated as authoritative; confirm the Reverie line can be retired. | Pacing and tone. | Title and description of game; Discovery Frontier |

## Gaps (TBD)

### Progression and economy
| # | Question | Why it matters | Cards |
|---|---|---|---|
| P1 | Which building types exist besides houses, towns and specialized respawn buildings? | Mid-game content and loop. | Buildings and buffs; memory |
| P2 | Buff kinds, how building, lair and boss buffs differ, stacking and caps. | Boss DPS math and balance. | Buildings and buffs; memory |
| P3 | Research structure: branches, nodes, costs; environment-adaptation techs; discovery-unlocked vs bought. | Progression spine. | Spec: Title and description of game; Discovery Frontier |
| P4 | Long-term win condition. | See C8. | Spec: Title and description of game |
| P5 | All economy values beyond v1; v1 research point rate and cost curve (tuned in playtest). | Pacing. | v1 prototype; v1 build plan |
| P6 | How a new biome is unlocked (boss defeat or otherwise). | Progression gating. | World: biomes and regions |
| P7 | Which 2-3 biomes follow forest and meadows at launch, and in what order. | Launch content scope. | memory; World: biomes and regions |
| P8 | Biome layout on the sphere (bands or patches). | World authoring. | World: biomes and regions |
| P9 | Can colonized land be lost to monsters? | Economy and tension. | World: biomes and regions |
| P10 | Do settlers simulate needs (eat, starve, reproduce, build)? | Simulation cost and design. | Title and description of game; Spec: Title and description of game |

### Bosses
| # | Question | Why it matters | Cards |
|---|---|---|---|
| B1 | Timer length, HP values, click damage, weak-spot multipliers, lair settler counts. | Fight balance. | memory |
| B2 | What "use the environment" means in a fight. | Fight mechanics. | memory |
| B3 | Main boss rewards (Mist: global buff and biome transformation). | Progression. | Buildings and buffs |
| B4 | Lair buff kinds and what mini-boss drops do; do mini-boss fights follow main-boss rules? | Mini-boss design. | Buildings and buffs; memory |
| B5 | How lore pieces are found and presented. | Discovery loop. | memory |
| B6 | Monster types and gameplay effects of hazards and monsters. | World interactions. | memory; World: biomes and regions |

### Setting, art, audio, UI
| # | Question | Why it matters | Cards |
|---|---|---|---|
| A1 | Settler look (only a Mist proposal exists); one body shape or variations. | Art production. | The Settler |
| A2 | Audio: music style, SFX style, ambience, settler vocalizations. | No audio direction exists. | The Settler |
| A3 | Story and lore content; reason for colonization. | Writing, lore pieces. | — |
| A4 | Production UI framework and layout (menus, research screen, boss HUD). | UI implementation. | v1 build plan |

### Tech
| # | Question | Why it matters | Cards |
|---|---|---|---|
| T1 | Colonized-land storage model (per-cell, per-region or both). | Data layout, save size. | Spec: … (scale and tech); Tech advice; Spec: Tech advice |
| T2 | Zoom thresholds (close < 5 m, mid 5-100 m, far > 100 m) appear small for a ~32 km planet; confirm units and values. | Zoom pipeline. | Plan and architecture: Unity DOTS |
| T3 | Planet authoring pipeline and tools. | Hand-authored world production. | Tech advice |
| T4 | v1 map size. | M1-T7. | v1 prototype |
| T5 | Save format, versioning, Steam Cloud; Steamworks integration and achievements. | Release readiness. | — |
| T6 | Consistency tuning between regional death rates and individual simulation. | Scale correctness. | Spec: Tech advice |
| T7 | CI and automated performance capture. | Testing. | — |
| T8 | The Mist Tech advice card's "Questions you didn't ask" section is cut off in the source. | Possible unrecorded considerations. | Tech advice: engine, zoom limits, world size |

### Business and scope
| # | Question | Why it matters | Cards |
|---|---|---|---|
| S1 | Monetization and price model. | No card or memory line covers it. | — |
| S2 | Schedule and development time budget. | Milestone planning. | v1 prototype; v1 build plan |
