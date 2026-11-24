# Stage: 1 — Prototype
Set: 2026-11-24 (inferred)
Design pack: Dreamer v1 (docs/design/CHANGELOG.md)

## Why
A Dreamer design pack (v1) is present in `docs/design/` and meets the Concept exit criteria (see `CONCEPT.md`). The core verb (click → settlers → progress → research) isn't playable in a scene yet. M1 in `docs/design/03-roadmap.md` is this stage's build plan.

## Exit criteria
- [ ] Core verb playable with placeholder art in one scene. Current state: the economy loop (click, research, progress) works in code, with 44/44 tests passing (headless dotnet run, 2026-11-24). It can be played through the dev debug panel once the Boot scene is generated (M1-T1/T5/T6 need an in-editor check). Map, settlers, HUD and boss are not started (M1-T7..T11).
- [ ] At least 3 people played it unguided and at least 2 wanted to keep playing. Current state: no playtests yet.
- [ ] Riskiest assumption tested. Current state: not yet. Pacing is the first sub-risk, and a bot simulation already shows the design's 0.0001%/settler/s lands under 20 min for steady clickers (`reports/2026-11-24-pacing-sim.md`).

## Roadmap progress (M1)
- [ ] M1-T1 skeleton: code done, Unity resolved the DOTS packages and compiled Numbers/Economy. Still needed: the Editor compiles the remaining assemblies and generates the Boot scene, and that scene opens without errors.
- [x] M1-T2 big-number type
- [x] M1-T3 economy core
- [x] M1-T4 click and research
- [ ] M1-T5 tuning data and IMGUI panel: code done and type-checked. Still needed: check it in Play mode.
- [ ] M1-T6 numbers-only playtest build: build script done. Still needed: make the build and play it.
- [ ] M1-T7..T13 not started

## Next step
Focus the Unity Editor so it compiles and generates `Assets/Scenes/Boot.unity`. Then run Play mode on Boot, play the loop from the debug panel, and make the Windows dev build (menu ColonizeEarth/Build).
