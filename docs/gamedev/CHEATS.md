# Cheats

All cheats are in the dev debug panel (`Assets/Game/Debug/DebugPanel.cs`, toggle **F1**). The Game.Debug assembly compiles only in the Editor and in development builds.

| Cheat | What it does | Game API |
|---|---|---|
| +10 settlers / +1K / x10 | Adds settlers | `EconomyModel.AddSettlers` |
| +100 RP / +10K RP / RP = 0 | Changes research points | `EconomyModel.AddResearchPoints` |
| Set 0% / 50% / 99.9% / 100% | Sets colonization. 100% records the time it was reached | `EconomyModel.SetColonizationPercent` |
| Time 1x / 10x / 100x | Speeds up play time (economy ticks with scaled time) | `Time.timeScale` |
| Reset run | New run with the current tuning | `EconomyModel.ResetRun` |
| Tuning fields | Live edit of every `EconomyTuning` value (decimal point is `.`) | `EconomyTuning` fields + `Sanitize` |

The panel also holds the v1 loop buttons (Click, Research), which use the normal game API (`Click`, `TryBuyResearch`).
