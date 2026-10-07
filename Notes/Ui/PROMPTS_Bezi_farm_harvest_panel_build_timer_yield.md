# Bezy — Timer + rendement `FarmHarvestPanel_Build`

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-006]`  
**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Parent :** `Content` — after `ProgressTrack`  
**Référence :** `TimerPlaque`, `TimerLabel`, `YieldLabel` in `FarmHarvestPanel_WoodMockup.prefab`.

**Sprites (file name OK):** `Assets/Art/Sprites/UI/UiKit_Bar_rope_20260919.png` on TimerPlaque (Sliced).

**Hors scope :** C#, `HarvestPanelUI`, boutons, Simulate.

**Succès =** Save prefab + liste. **STOP.**

---

## Phase 1 — coller dans Bezy

```
[BZ-FARM-HARVEST-PANEL-BUILD-006] TimerPlaque + YieldLabel under Content. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy / Inspector ONLY. No C#. No HarvestPanelUI.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Do not rescan project. Layer 5.

Keep Content VLG + existing children unchanged. ADD after ProgressTrack in VLG order:

4) TimerPlaque
5) YieldLabel

Do NOT change CloseButton, ProgressTrack, or header.

--- TimerPlaque ---
- UI Image under Content, name: TimerPlaque
- LayoutElement: Preferred Height 88, Flexible Width 1
- Image sprite: Assets/Art/Sprites/UI/UiKit_Bar_rope_20260919.png — Type Sliced, white, Raycast OFF

--- TimerLabel (child of TimerPlaque) ---
- TextMeshProUGUI, name: TimerLabel (exact for HarvestPanelUI.timerLabel)
- RectTransform: stretch full plaque (anchors 0-1, offsets 8 L/R, 4 T/B)
- TMP: placeholder "02:15:30", center alignment, size ~24, parchment/light color, Raycast OFF

--- YieldLabel ---
- TMP under Content (sibling after TimerPlaque), name: YieldLabel
- LayoutElement: Preferred Height 36, Flexible Width 1
- TMP: placeholder "Rendement : ×3", center, size ~20, brown-tan (~0.36, 0.22, 0.12), Raycast OFF

No buttons this phase.

Save prefab. STOP. No Play Mode.
```

---

## Phase REDO — TimerPlaque invisible (2026-10-07)

**Cause :** `UiKit_Bar_rope_20260919` borders T+B = **90 px** avec `Preferred Height 88` → centre 9-slice **≤ 0** → Image invisible (le TMP « 02:15:30 » peut rester seul).

**Fix MVP (pas d’art neuf) :** bandeau **UiKit_Row_list_20260919** + hauteur **≥ 120** + PPU mult **1.15** si bordures encore épaisses.

```
[BZ-FARM-HARVEST-PANEL-BUILD-006-REDO] TimerPlaque visible. STOP.

@Notes/Bezi/RULES_bezy_code.md — TimerPlaque + TimerLabel ONLY. No C#.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Do not rescan project. Layer 5.

TARGET: GameObject TimerPlaque (keep TimerLabel child + names).

TimerPlaque Image:
- Sprite: Assets/Art/Sprites/UI/UiKit_Row_list_20260919.png (NOT Bar_rope)
- Type Sliced (1), Fill Center ON, color white, Raycast OFF
- Pixels Per Unit Multiplier: 1.15 (tune 1.0–1.3 if needed)

LayoutElement on TimerPlaque:
- Preferred Height: 120 (min 112)
- Flexible Width: 1, Flexible Height: 0

TimerLabel: keep stretch inside plaque, text "02:15:30", visible parchment color.

Do NOT change YieldLabel, ProgressTrack, buttons, CloseButton.

Save prefab. Report sprite name + preferred height + PPU mult. STOP. No Play Mode.
```

**Backlog art (post-MVP) :** planche dédiée « timer plaque » parchemin (H-farm-6) si Row_list trop fade sur bois.

**Livré REDO 2026-10-07 :** `UiKit_Row_list_20260919`, Preferred Height **120**, Min Height **112**, Flexible Height **0** — PPU mult resté **1** (OK si bande visible ; sinon **1.15** manuel).
