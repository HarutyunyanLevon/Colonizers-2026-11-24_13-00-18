# Decisions

Engineering and process decisions only. Design decisions live in Dreamer (`docs/design/QUESTIONS.md`).

## 2026-11-24
- **Stage inferred as 1 (Prototype).** A Dreamer v1 pack meets the Concept criteria, and nothing is playable yet. Build plan = M1 in `03-roadmap.md`.
- **Editor version: 6000.6.10f1.** The project file and installed editor are 6000.6.10f1, while the pack says 6000.6.8f1. It's the same minor version (a patch release), so no change was made.
- **DOTS packages pinned to the editor's built-in versions:** entities/entities.graphics/collections 6.6.0, burst 2.0.0, mathematics 1.4.0, test-framework 1.8.0. These were read from the editor's `BuiltInPackages`, and Unity resolved them.
- **Module layout as in 02-tech-design.md, with one change.** Game.App was added as the composition root (bootstrap and the tuning ScriptableObject), so Game.Economy and Game.Numbers stay engine-free (`noEngineReferences`). Namespaces avoid shadowing Unity types: `Game.DebugTools`, `Game.EditorTools` and `Game.CameraControl` (the assemblies keep their pack names).
- **BigNum compares numbers itself.** Number Forge's `BigNumber.CompareTo` reports `0 > 0.5` because zero is stored with exponent 0. `Game.Numbers.BigNum` does its own sign-aware compare, pinned by the `Compare_ZeroAgainstFractions` test. Saves use our own invariant-culture text form, because Number Forge's `ToString` follows the machine culture and the owner's locale writes commas.
- **Debug assembly compiled out of release builds** by the asmdef define constraint `UNITY_EDITOR || DEVELOPMENT_BUILD`. The panel creates itself at startup, so no scene references it and release builds have no missing script.
- **Boot scene and tuning asset are created by editor code** (`Game.EditorTools.ProjectSetup`) through Unity APIs, never by hand-editing YAML. It runs once when `Assets/Scenes/Boot.unity` is missing, or from the menu ColonizeEarth/Setup.
- **Tests run two ways.** Unity Test Framework edit-mode assemblies are in the project. Because the Editor is open (which blocks batch-mode `unity test`), the same NUnit tests were also run headlessly with dotnet against the same source files. Result: 44/44 pass.
- **Commits.** M1-T3 and M1-T4 share files (one economy model), so they go in one commit tagged with both task ids. The owner's own pending changes (ProjectSettings, Assets/Editor deletion, CLAUDE.md, the Number Forge import) were left unstaged.
- **Not done:** installing the `com.unity.pipeline` package so the CLI can drive the open Editor. The action was blocked in this session; it needs owner approval.
- **Dreamer workspace not written.** `CLAUDE.md` has no `Dreamer project:` / `Dreamer build plan:` lines, so build-plan cards aren't ticked. Progress is tracked in `03-roadmap.md`, as the Dreamer block in CLAUDE.md asks, and in STAGE.md.
