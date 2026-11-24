# Pacing simulation: 2026-11-24

**Method:** `Assets/Game/Economy/Tests/PacingSimulationTests.cs`. A bot clicks at a fixed rate, buys research whenever it can afford it, and ticks at 0.1 s. It was run headlessly with dotnet against the game's own economy code. This is a bot, not a player: real players click in bursts and stop.

## Default tuning (committed)
progress 0.0001 %/settler/s, 10 RP per %, research cost 100 × 2^level

| Clicks/s | Time to 100% |
|---|---|
| 1 | 19.1 min |
| 3 | 11.0 min |
| 6 | 7.8 min |

## Sweep of the undecided research values
At 3 clicks/s, time to 100% ranged from 5.1 min (100 RP per %, cost 10 × 1.5^level) to 13.6 min (1 RP per %, base 100). In other words, research can only make the game faster. With 0.0001 %/settler/s, a steady 3 clicks/s with almost no research gives about 13.6 min. Click-only colonization grows with the square of time: t ≈ √(200 / (0.0001 × clicks/s)).

## Finding
The design expects 0.0001 %/settler/s plus a steep research curve to give 20-40 min. That holds only for slow clickers (about 1 click/s or less). For steady clickers the base rate, not the research curve, decides pacing. This was logged as a design question in `docs/design/QUESTIONS.md` (M1-T6). The owner should check it against real playtests (M1-T13 instrumentation) before changing anything.
