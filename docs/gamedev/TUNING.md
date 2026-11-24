# Tuning

All economy values live in one asset: `Assets/Game/Data/EconomyTuning.asset` (type `Game.App.EconomyTuningAsset`, created by ProjectSetup). The code defaults are in `Assets/Game/Economy/EconomyTuning.cs`. They can be edited live in the dev debug panel (F1 → Tuning). In the Editor, live edits made in Play mode are saved to the asset.

| Value | Field | Current | Why |
|---|---|---|---|
| Starting settlers | `startingSettlers` | 1 | Design: player starts with one settler |
| Settlers per click (base) | `baseSettlersPerClick` | 1 | Design: click adds settlers-per-click, starts at 1 |
| Settlers per click per research level | `settlersPerClickPerResearchLevel` | 1 | Design: research "+1 settler per click" |
| Progress per settler per second | `progressPercentPerSettlerPerSecond` | 0.0001 (%) | Design: start near 0.0001% |
| Colonization cap | `maxColonizationPercent` | 100 | Design: 100% = map colonized |
| Research points per 1% gained | `researchPointsPerPercent` | 10 | Pack TBD. Picked from the bot sweep so research matters without dominating (DESIGN-Q) |
| Research base cost | `researchBaseCost` | 100 | Pack TBD (DESIGN-Q) |
| Research cost growth per level | `researchCostGrowth` | 2 | Pack asks for a "steeper research cost curve". Exponential: cost = base × growth^level (DESIGN-Q) |

Bot pacing at these values (headless sim, greedy research): 19.1 min at 1 click/s, 11.0 min at 3 clicks/s, 7.8 min at 6 clicks/s. Target is 20-40 min. See `reports/2026-11-24-pacing-sim.md`.

Performance budgets: per the PC/Steam platform row in `CLAUDE.md` (60 fps minimum on the median Steam PC at close zoom). No overrides yet.
