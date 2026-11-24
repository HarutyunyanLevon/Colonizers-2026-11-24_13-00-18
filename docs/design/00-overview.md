# ColonizeEarth — Overview

## Pitch
ColonizeEarth is an idle/incremental game in the Cookie Clicker style: the player clicks to spawn settlers, then buildings such as houses spawn more, and later whole towns appear from one click. Unlike a menu of numbers, the colonization happens on a visible, living, alien-feeling Earth with thousands to millions of settlers and seamless zoom from individual settlers to the whole planet.

## Core experience
Satisfaction from steady research and expansion, combined with wondrous discovery of an alien-feeling Earth (project memory; Spec: Title and description of game (scale and tech)).

## Target player
Players of idle and incremental games, on a medium PC (current Steam Hardware Survey median).

## Pillars
1. **Scale is the core.** Whole-world scale with thousands to millions of units.
2. **Performance at every scale.** Close zoom runs uncapped with a 60 fps minimum on the medium PC.
3. **Three readable zoom levels.**
   - Close: units act individually, up to a fixed sample per area; crowded areas show the real total.
   - Mid: trees and buildings hidden, only building icons; units merge into numbered groups.
   - Far: only the world and colonized land.
4. **Seamless scale changes.** Moving between zoom levels must feel continuous.

## What makes it different
A visible living world at million-settler scale instead of a menu of numbers. This is affordable because only settlers near the camera are simulated individually; everything else is per-region aggregate statistics.

## Key facts
| Item | Value |
|---|---|
| Genre | Idle / incremental (clicker) |
| Platform | PC only, Steam |
| Engine | Unity 6000.6.8f1, URP, DOTS |
| Art | Low-poly 3D, soft toon shading, outlines |
| World | Hand-authored planet, same every run; sphere at far zoom, flat local map when zoomed in |
| Planet size | 6 × 4096² cells at 2 m per cell (~32 km around) |
| First playable (v1) | Flat forest map, click-spawned settlers, one research, placeholder boss at 100% |
| Monetization | TBD |

## Documents
| File | Contents |
|---|---|
| [01-gdd.md](01-gdd.md) | Game design: loop, mechanics, progression, bosses, setting, art, audio, UI, content, ideas under consideration |
| [02-tech-design.md](02-tech-design.md) | Architecture, modules, data, zoom pipeline, persistence, performance, testing |
| [03-roadmap.md](03-roadmap.md) | Milestones and tasks; M1 is the v1 prototype |
| [04-open-questions.md](04-open-questions.md) | Every TBD and contradiction, with affected cards |
| [05-assets.md](05-assets.md) | Images, references, and assets the game needs |
| [CLAUDE.md](CLAUDE.md) | Working rules for Claude Code |
| [CHANGELOG.md](CHANGELOG.md) | Version history of this pack |
