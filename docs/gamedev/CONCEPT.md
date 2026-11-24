# Concept

Design source: `docs/design/` (Dreamer v1). The design lives in Dreamer; this file only copies what `stage-ladder` needs.

- **Core verb:** click to spawn settlers, who colonize the world on their own. Research makes each click worth more.
- **Core loop:** see `docs/design/01-gdd.md#core-loop`. v1 tests click → settlers → progress → research on one flat forest map (`01-gdd.md#v1-prototype-scope`).
- **Audience, pillars, platform:** `docs/design/00-overview.md`.
- **Monetization:** TBD in the pack (`04-open-questions.md` S1).
- **Riskiest assumption (team reading of the pack, to confirm in Dreamer):** that clicking settlers into a visible world, which spreads colonization and research, is satisfying enough to hold an idle-game player for 20-40 minutes on one map. The scale and performance pillars are the riskiest *technical* assumption, but the v1 roadmap defers them to M2.
