# Design questions

Questions Claude Code ran into while building. One per line:
`- [ ] Q (task id): question — context`. Answer them in Dreamer, then generate a new design version.

- [ ] Q (M1-T4): What are the v1 research point accrual rate and research cost curve? — pack marks both TBD (tuned in playtest); options: 10 RP per 1% gained and cost 100 × 2^level / another formula; default used: 10 RP per %, cost 100 × 2^level
- [ ] Q (M1-T3): Should research points keep accruing after colonization reaches 100%? — pack says points accrue with progress; options: stop at 100% / keep accruing per settler; default used: stop at 100%
- [ ] Q (M1-T6): Pacing: at 0.0001%/settler/s a bot hits 100% in 19 min at 1 click/s, 11 min at 3/s, 8 min at 6/s (14 min at 3/s even with no research). Is 20-40 min for slow clickers, or lower the base rate? options: keep 0.0001 / lower it; default used: keep 0.0001
