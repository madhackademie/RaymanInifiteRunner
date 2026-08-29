# Skill Bezi — `/prefab-ui-3phases` (copie repo)

**Slash :** `/prefab-ui-3phases`  
**Install Bezi (source runtime) :** `%AppData%\Roaming\com.bezi.app\skills\prefab-ui-3phases\SKILL.md`  
**Chemin Bezi UI :** `/.bezi/skills/prefab-ui-3phases/SKILL.md`

Cette note est le **miroir Git** du skill installé (Cursor / GitHub). Bezi exécute la copie AppData. Si les deux divergent, aligner depuis ce fichier vers Bezi (prompt : « update skill prefab-ui-3phases from Notes/Bezi/SKILL_prefab_ui_3_phases.md ») ou l’inverse.

**Mise à jour :** 2026-08-29 — draft validé + inserts Prefab Mode Ph.1–2 (recommandé) et never-unpack `UiStarRow` / `UiStarSlot`.

Smoke test : `/prefab-ui-3phases` doit demander Task ID + chemin `Assets/Prefabs/Ui/` + phase, puis **STOP** si ce n’est pas un job réel.

---

Le corps ci-dessous est le `SKILL.md` installé (frontmatter Agent Skills inclus).

---

```markdown
---
name: prefab-ui-3phases
description: Standardizes UI prefab work on RaymanInfiniteRunner into a strict 3-phase workflow (hierarchy, then components, then wiring), executing exactly one phase per invocation via live Bezi Actions mutations. Use when creating or modifying a UI prefab, HUD widget, popup shell, or inventory/shop screen layout under Assets/Prefabs/Ui/, when the user @mentions a target prefab there, or references a PROMPTS_Bezi_*.md task ID like [BZ-XXX-NNN]. Does not touch C# scripts, does not run Simulate/Play Mode, and never merges phases even when the user says "all".
license: internal
---

# Prefab UI 3-Phases

## What this skill does

Executes UI prefab work on a target prefab under `Assets/Prefabs/Ui/` as three strictly separated phases — hierarchy shell, visual components, then wiring — mutating the prefab live in the Editor via Bezi Actions. Each invocation runs exactly one phase, ends with an explicit save-and-report, then stops for author review of the Git diff between phases.

## When the agent should invoke this

- Slash command `/prefab-ui-3phases`.
- Natural language: "create the inventory tabs prefab", "add the shop popup panel", "wire up the HUD exit button", "do phase 2 on ShopItemPopup.prefab".
- The user provides or `@mentions` a target prefab path under `Assets/Prefabs/Ui/`.
- The user references a `PROMPTS_Bezi_*.md` task ID such as `[BZ-INV-TABS-001]`.

**Exclusions:** gameplay prefabs outside `Assets/Prefabs/Ui/`, requests to edit `UIManager`/`SceneNavigator`/`ScreenPopupHost`/controller C# logic (report as out of scope, don't touch), requests to Simulate/Play Mode/visually confirm (decline, remind author it's author's step), requests to run all three phases in one shot (execute Phase 1 only, then stop).

## Required context

Before acting, confirm you have:

1. **Task ID** (e.g. `[BZ-INV-TABS-001]`) — ask if missing.
2. **Target prefab path**, exact, under `Assets/Prefabs/Ui/` — ask if missing.
3. **Phase number**: `1`, `2`, `3`, or `"all"` — ask if missing. If `"all"`, treat as Phase 1 only for this invocation.
4. Optional: scene to keep open (`Bootstrap.unity` or prefab mode).
5. Optional: read-only script paths to wire in Phase 3 (never edit them unless the user explicitly asks).

Do not rescan the whole project to infer these — ask for exactly what's missing, then use only the exact paths given. Read prior state with `getAsset` (prefab-level) or `getGameObjectsByPath` (specific GameObjects) on the exact target path before mutating.

For Phase 1 and Phase 2, recommend (do not require) that the target prefab already be open in Prefab Mode — disk-path edits on existing GameObjects can silently no-op otherwise. Do not open the scene or prefab yourself; ask the author to open it if they haven't. The hard stop on this precondition applies to Phase 3 only.

## Orchestration steps

### Phase 1 — Hierarchy shell only

1. Read current prefab state: `getAsset({ assetPath: <target prefab path> })` or `getGameObjectsByPath` on known sub-paths, to avoid clobbering existing children or SerializeFields.
2. Create/update GameObjects and their `Transform`/`RectTransform` layout only, via `createGameObject` (with `transform`), `reparentGameObject`, `updateGameObject`. Use `deleteGameObject` only when the task explicitly requires removing a shell node.
3. Do NOT add `Image`, `Button`, `TextMeshProUGUI`, any `LayoutGroup`, `Animator`, or wire any `SerializeField` — that is Phase 2/3.
4. Preserve existing children and controller SerializeFields unless the task says otherwise.
5. Set `layer: "UI"` (m_Layer 5) via `updateGameObject` on the Canvas root AND every new/moved UI child. Never use layer index 4 (Water).
6. Save (automatic after a successful batch — no separate save action needed). List: parent paths touched, new GameObject names, layer verification (confirm all touched objects report layer 5). STOP.

### Phase 2 — Components only

1. Read current prefab state for the target hierarchy via `getGameObjectsByPath` (`detailLevel: "WithProps"`) to avoid duplicating existing components.
2. Add or update `UnityEngine.UI.Image`, `UnityEngine.UI.Button`, `TMPro.TextMeshProUGUI`, `UnityEngine.UI.LayoutGroup` variants, `LayoutElement`, `CanvasGroup`, etc. via `addOrUpdateComponent`. Use `removeComponent` only to fix an explicitly flagged wrong component.
3. Do NOT wire `SerializeField` references or `Button.onClick` events yet — Phase 3 only.
4. Match the project visual baseline: dark panels `~(0.12, 0.12, 0.16, 0.9)`, TMP white centered text, minimum tap target 48px.
5. Re-verify layer 5 on any newly added child objects.
6. Save (automatic). List: components added per GameObject path. STOP.

### Phase 3 — Wiring only

⚠️ Approval required: if any part of this phase would require creating or editing a C# script (e.g. a missing public method to hook an event to), stop and ask the author before touching any `.cs` file. Default is to not touch C#.

1. Confirm precondition: the target prefab (prefab-mode) or the scene containing it is already open in the Editor. If not, tell the author to open it and stop — do not attempt to open it yourself as a workaround for this precondition.
2. Read existing controller/view components on the target hierarchy via `getGameObjectsByPath` (`detailLevel: "WithProps"`, `includeComponents` naming the controller types) to see current SerializeField state.
3. Assign SerializeField references and wire `Button.onClick`/similar UnityEvents on existing view/controller components via `addOrUpdateComponent` (re-applying the same component type with updated props targets its existing serialized fields).
4. Do NOT change hierarchy or add new visual components, unless fixing a broken reference explicitly flagged by the author.
5. Reuse existing scripts only — never duplicate `UIManager`, `SceneNavigator`, or `ScreenPopupHost` logic; route navigation through `SceneNavigator.ShowScene`, popups through `PopupId` + `ScreenPopupBinding` + `ScreenPopupHost`.
6. Save (automatic). List: each SerializeField filled (GameObject path → field → target), each event wired, any missing manual link flagged for the author. STOP.

### Disk-path workaround (any phase, only if direct prefab edit fails)

⚠️ Approval required: confirm with the author before using this workaround — it creates a temporary scene object.

1. `instantiatePrefab` the target prefab temporarily into the open scene (e.g. `Bootstrap.unity`).
2. Apply the phase's changes on that instance using the normal phase actions, scoped to the scene rather than the prefab asset.
3. `applyPrefabOverrides({ prefabInstanceRoot: { gameObjectPath: <instance path> } })` to push changes back to the source prefab.
4. Verify the prefab's asset GUID is unchanged via `getAsset` on the prefab path before and after.
5. `deleteGameObject` the temporary instance from the scene.
6. Report this workaround was used in the end-of-phase summary.

## Decision rules

- One phase per invocation, always. Never merge Phase 1+2+3 in one script or one message, even if the user says "all" — execute Phase 1 only, then stop.
- Inside a phase, execute all mutations immediately once inputs are confirmed — no mid-phase "does this look good?" pause.
- Never call Simulate, enter Play Mode, or request visual confirmation, in any phase.
- Never rescan the whole project — operate only on the exact target prefab path and exact GameObject paths given or discovered by direct lookup on that path.
- If the target prefab or scene is not open in the Editor before Phase 3, stop and ask the author to open it; do not substitute the disk-path workaround for this precondition.
- Never unpack nested prefabs (e.g. `UiStarRow`, `UiStarSlot`) — edit nested prefab instances in place, as instances, without unpacking them.
- If a task would require creating or editing a C# script, stop and flag it — do not implement it without explicit approval.
- If required input (task ID, prefab path, phase) is missing, ask for exactly the missing piece before mutating anything.

## Output / deliverable

Each invocation ends with exactly one phase's mutations applied live to the target prefab, plus a summary listing:

- Files changed (the prefab path, and any temporary workaround artifacts).
- GameObject paths touched, with the phase-specific checklist (layer verification for Phase 1, components-per-object for Phase 2, fields/events wired for Phase 3).
- A reminder that playtesting is the author's step, not this skill's.
- The suggested next command, e.g. `/prefab-ui-3phases Phase 2 <same task ID and prefab path>`.

## MCP requirements

None. (GitHub MCP, if connected, is used only to read `PROMPTS_Bezi_*.md` task specs — it is not required to run this skill.)

## Runtime constraints

- No Simulate, no Play Mode, no visual-confirmation requests, in any phase.
- No C# file creation or edits without explicit author approval.
- No merging of phases in a single invocation.
- Layer 5 (UI) required on Canvas root and every UI child; layer index 4 (Water) is reserved and must never be used for UI.
- Phase 1 and 2: recommend (not require) the target prefab already be open in Prefab Mode, since disk-path edits on existing GameObjects can silently no-op otherwise. Do not open it yourself.
- Phase 3 requires the target prefab/scene already open in the Editor; this is a hard precondition, not a step this skill performs.
- Operate only on exact paths provided or discovered from them — no project-wide scans.

## Verification

- Success: the end-of-phase action responses report no errors, and the phase-specific checklist (layer 5 on all touched objects for Phase 1; expected component list per GameObject for Phase 2; each SerializeField/event confirmed wired for Phase 3) is included in the summary.
- Failure modes to detect and report rather than silently skip: missing required input, target prefab/scene not open before Phase 3, an action response indicating a rolled-back batch, a SerializeField that has no matching existing script reference to wire.
- Untested by this skill: actual runtime behavior of wired events (author validates via their own playtest, per Runtime constraints).
```
