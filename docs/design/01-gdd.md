# ColonizeEarth — Game Design Document

Sources: project memory (authoritative), Lucid specs, Reverie v1 cards. Mist cards appear only under [Ideas under consideration](#ideas-under-consideration). Card titles are cited in parentheses.

---

## Core experience
- The player should feel **satisfaction from steady research and expansion** (project memory; Spec: Title and description of game).
- Secondary feeling: **wondrous discovery of an alien-feeling Earth** (Spec: Title and description of game (scale and tech)).
- The world is visible and alive instead of being a menu of numbers.
- Note: the Reverie card "Title and description of game" records an earlier decision that the main feel should be "frantic, with the thrill coming from discovery". Project memory overrides it; the frantic framing is not a design goal.

## Target player
Idle/incremental game players (project memory; Spec: Title and description of game).

---

## Core loop
Cookie Clicker-style (project memory; Spec: Title and description of game):

1. **Click** anywhere to spawn settlers.
2. **Settlers colonize** unclaimed land and water on their own; the player does not control them.
3. **Colonization progress** accrues.
4. **Research** is bought with earned progress and makes clicks spawn multiple settlers or buildings.
5. **Buildings** (e.g. houses) spawn more settlers automatically.
6. **Towns** later appear from a single click.
7. Expansion reaches new biomes, where mini-bosses and a main boss are discovered and fought.

TBD: research currency. Memory says research "costs colonization progress already earned"; the v1 prototype uses separate research points that accrue with progress, and spending them never lowers colonization % (v1 prototype: forest planet; v1 build plan). See 04-open-questions.md.

---

## Mechanics

### Settlers
- Genderless humanoid units spawned by clicks and by buildings (glossary; Spec: Title and description of game).
- Not player-controlled; they wander off to colonize unclaimed land and water (Spec: Title and description of game (scale and tech)).
- They interact with the world (lava, ocean, monsters), **mostly visually** (project memory).
- Off-screen and distant settlers are per-region aggregate statistics, not individual simulations (project memory).
- TBD: whether settlers can die outside boss fights (hazards, monsters, starvation). The Lucid spec lists hazards killing settlers; the Mist "The Settler" card proposes no death; memory only confirms deaths inside boss fights.
- TBD: needs simulation (eating, starving, reproducing, building). Listed in the Lucid "Spec: Title and description of game" but not confirmed in memory.

### Clicking
- A click spawns settlers at the cursor (v1 build plan).
- Research raises the number of settlers (and later buildings) per click (project memory).
- During a boss fight, clicks spawn no settlers (project memory).

### Buildings
- Buildings such as houses spawn more settlers (project memory).
- Towns appear later in the game from one click (glossary).
- Player-placed **specialized buildings** are where dead boss-fight settlers respawn, if any exist (project memory).
- Buildings provide buffs that feed boss-fight DPS (project memory).
- Buildings produce a resource used to craft consumables (project memory).
- TBD: building types beyond houses and towns; spawn rates; placement rules; building resource name and rate; buff kinds and stacking (Buildings and buffs).

### Research
- Research costs earned colonization progress (see TBD above) and lets clicks spawn multiple settlers or buildings (project memory).
- Research also unlocks boss-fight consumables and power-ups, and adds to settler DPS buffs (project memory).
- The Lucid spec states research has multiple branches with no single strict centre, and that settlers adapt to harsh environments through it (Spec: Title and description of game).
- TBD: research tree structure, node list, costs, and environment-adaptation techs.

### World interactions
- Lava, ocean and monsters affect gameplay but are mostly visual (project memory).
- Implementation is grid lookups and cell flags, no physics (Plan and architecture: Unity DOTS).
- TBD: the gameplay effect of each hazard and monster type (death rate, slowdown, blocking).

### Scale and zoom
Pillars from project memory:

| Zoom | What is shown | Settlers |
|---|---|---|
| Close | World objects, trees, buildings | Individual, up to a fixed sample per area; crowded areas show the real total |
| Mid | Trees and buildings hidden; building icons only | Merged into numbered groups |
| Far | Sphere: world and colonized land only | Not shown |

- World shape: sphere at far zoom, flat local map when zoomed in (project memory).
- Transition: dithered transparency fade, no sorting; trees and buildings fade as building icons appear (project memory).
- Changing scale must feel seamless.

---

## Progression

### Biomes
- Launch content: **3-4 biomes, one main boss each** (project memory).
- Biomes are added one at a time: **forest and meadows first**, then real Earth types with alien twists (e.g. glowing jungle, crystal desert) (project memory).
- Biome difficulty is fixed; neighbouring biomes can differ. Only border visuals (darker fog, larger monsters) hint at difficulty (project memory).
- TBD: which 2-3 biomes follow forest and meadows at launch, and their order.
- TBD: how a new biome is unlocked or entered (the Mist card proposes "defeat the main boss to open the border").
- TBD: biome layout on the sphere (contiguous bands or scattered patches).

### Mini-bosses
- Optional, at fixed hand-placed lairs (project memory).
- Each drops a counter-item or research for its biome's main boss (project memory).
- Each mini-boss is fought once. A cleared lair becomes a special site with a permanent buff for its whole biome (project memory).
- TBD: kind of permanent lair buff; what drops do exactly; mini-boss fight rules (assumed to follow the boss fight rules below, unconfirmed).

### Win condition
- TBD. The Lucid spec says the game is won when the whole planet is colonized (Spec: Title and description of game). Memory confirms only that v1 has no end. The long-term win condition is not settled.

### Offline progress
- No idle-away (offline) progression in the first version. Running in the background may come later (project memory).

### Economy numbers
- Settler counts and currency use a big-number type: mantissa + exponent, unlimited range (project memory).
- TBD: all economy values beyond v1 (spawn rates, research cost curves, building outputs).

---

## Boss fights
All rules from project memory.

### Discovery and preparation
- Settlers find the boss lair.
- Lore pieces reveal the boss's weak and strong spots so the player can prepare.
- TBD: how lore pieces are found and presented.

### Starting a fight
- The fight starts on a player click after discovery.
- Only the initial settlers fight: they are teleported to an arena; survivors return afterwards.
- A lair holds a fixed settler count. When more settlers are available, the most buffed enter first.
- Outside the arena, all other settlers continue their usual activities, including new spawns from buildings. Clicks spawn no settlers during a fight.

### Fight math
- Boss has HP.
- Settler DPS = settler count × research and building buffs. The math is fixed; the visuals vary.
- The player clicks the boss or its weak spots.
- The player can also retreat, use the environment, and use consumables and power-ups.
- Consumables and power-ups are unlocked through research and crafted from a building resource plus mini-boss drops.

### Outcome
- Win: boss at 0 HP before the timer ends.
- Loss: boss alive at timeout; survivors retreat.
- After a loss, the boss fully heals before the next fight. The player can start another fight any time.
- Dead settlers respawn at player-placed specialized buildings, if any.
- TBD: timer length, boss HP values, click damage, weak-spot multipliers, lair settler counts, what "use the environment" means in practice, rewards for winning a main boss fight.

---

## Setting and tone
- A hand-authored fictional planet, the same every run, with no procedural generation (project memory; Spec: Title and description of game (scale and tech)).
- Tone: wondrous discovery of an alien-feeling Earth (project memory).
- Biomes after the first are real Earth types with alien twists (project memory).
- TBD: reconcile with the Lucid "Spec: Title and description of game", which states a "light, cartoonish and humorous" tone, and the Mist "The Settler" card (funny, clumsy). These may coexist (wondrous world, comic settlers) but this is not confirmed.
- TBD: story, lore content, and the reason for colonization.

## Art direction
- Low-poly 3D with soft toon shading and outlines (project memory).
- Implementation rules (Spec: Tech advice: engine, zoom limits, world size; Plan and architecture: Unity DOTS): one material with a palette texture and vertex colours; screen-space outline pass on depth and normals; blob shadow for every settler; one shadow cascade for terrain and buildings near the camera; dithered fades only.
- TBD: settler look. Only the Mist "The Settler" card describes it (see ideas below).
- TBD: settler animation approach (see contradiction in 02-tech-design.md#animation).

## Audio direction
- TBD: music style, SFX style, ambience, and whether settlers vocalize. Only Mist ideas exist (a squeaky spawn "boop", possible gibberish; The Settler).

## UI
- v1 HUD: progress bar, settler count, research button with its cost (v1 build plan).
- v1 debug: IMGUI debug UI, tuning panel and cheats in development builds only (v1 prototype: forest planet).
- Later: building icons and numbered group badges at mid zoom; boss HP bar and timer; lore/weak-spot information (project memory; Spec: Tech advice).
- TBD: production UI layout, menus, research screen, boss-fight HUD, font and style.

## Content
| Content | Status |
|---|---|
| Biome 1: forest and meadows | Confirmed as first biome |
| Biomes 2-4 | TBD (real Earth types with alien twists, e.g. glowing jungle, crystal desert) |
| Main bosses | One per biome; designs TBD |
| Mini-bosses | Optional, fixed lairs; count and designs TBD |
| Monsters | Exist (memory); types TBD |
| Building types | Houses, towns, specialized respawn buildings; others TBD |
| Research nodes | +1 settler/click (v1); others TBD |
| Consumables / power-ups | Exist (memory); list TBD |

---

## v1 prototype scope
From project memory and v1 prototype: forest planet. Purpose: test the click → settlers → progress → research loop.

- **World:** one flat forest map, one zoom level (camera pan and limited zoom). "100%" means this map is fully colonized. The player starts with one settler.
- **Spawning:** only clicks spawn settlers, at the cursor; settlers wander for show.
- **Progress:** formula, not simulation: progress per second = settler count × progress per settler per second.
- **Visuals:** cosmetic. The colonized tint spreads outward from the start point by progress %. Build plan also lists trees cut and land plowed, driven by progress % (v1 build plan).
- **Research:** one repeatable research, +1 settler per click, paid with research points that accrue with colonization progress. Spending never lowers colonization %.
- **Pacing:** target 20-40 minutes to 100%. 0.001% per settler per second is too fast (4-10 minutes). Start near 0.0001% per settler per second with a steeper research cost curve. All economy numbers are tunable in a tuning panel and tuned in playtests.
- **Boss:** at 100%, a ~15-second placeholder scene: a big "Heartwood" tree creature rises, settlers swarm it, its HP bar drains, it falls, and a "Forest colonized!" banner shows. No failure state.
- **End:** none. After 100%, the player can keep spawning settlers.
- **Save/load:** economy state (v1 build plan).
- **Left out:** buildings, towns, wolves, swamps, any other research, other biomes, sphere view, other zoom levels, offline progress.
- TBD: the research point accrual rate and cost curve formula (tuned in playtest).
- TBD: whether settlers "claim tiles" (v1 build plan item 6) or the tint is purely progress-driven (v1 prototype card). See 04-open-questions.md.

---

## Ideas under consideration
Unconfirmed Mist content. None of this is a decision.

### Discovery Frontier (Mist)
- Fog-of-war planet; waves of settlers reveal biomes, ruins, creatures and resources in bursts.
- Research branches unlocked by what settlers find rather than bought from a fixed tree.
- A "... Frontier" game name instead of "Discovery Rush" (the project is currently named ColonizeEarth).
- Gremlins not emphasized.
- Note: its "frantic" feel is overridden by project memory.

### The Settler (Mist)
- Small, clumsy, genderless humanoid; low poly, plain white body, two dot eyes, maybe a small mouth, no clothing; rounded, chunky, slightly oversized head and limbs in the Fall Guys spirit. Colour comes from the world.
- Funny, never cruel; failures are cartoonish (a pop, a dizzy spin, a respawn puff); no gore, no death.
- Animations: spawn pop with a squeaky "boop" and a hop; idle look-around, head scratch, wobble; colonize by stamping a flag and a small jump.
- Crowds read as a stream of tiny white figures.
- Open: one body shape or variations (height, research-unlocked hats); gibberish sounds.

### World: biomes and regions (Mist)
- Each biome is one region with a theme, places of interest, monsters, mini-bosses and a main boss; defeating the main boss unlocks the next region; start with the most survival-friendly biome.
- Monster density scales with distance from colonized land.
- Forest and Meadows proposals: places of interest Ancient Oak (research boost), Standing Stones (click multiplier), Abandoned Camp (free settlers), Glowing Clearing; monsters thorn boars, moss wolves, sporelings; mini-bosses Bramble Stag, Hollow Bear; main boss the Heartwood (a walking ancient tree whose roots bound the region; the name is used for the v1 placeholder).
- Proposed later biome order: Rivers and Wetlands, Hills and Steppe, Desert, Jungle, Tundra and Ice, Mountains, Volcanic Lands, Ocean and Islands, Alien Core (final boss). This exceeds the confirmed 3-4 launch biomes.
- Open: can colonized land be lost to monsters?

### Buildings and buffs (Mist)
- Defeating a main boss gives a global buff and transforms the biome.
- Some mini-bosses give drops instead of, or as well as, local buffs.
- Open: lair buff kinds (spawn rate, building output, research speed, settler survival), building types, how building, lair and boss buffs differ, stacking and caps, what drops do.

### Title and description of game (Reverie, superseded parts)
- Settlers eat, starve, reproduce and build; spamming settlers cannot win; technologies needed per environment, flora and fauna. Kept here because memory does not confirm them.
