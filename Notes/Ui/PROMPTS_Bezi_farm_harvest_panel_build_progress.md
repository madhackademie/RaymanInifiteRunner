# Bezy — Piste progression `FarmHarvestPanel_Build`

**Statut :** **abandonné 2026-10-07** — auteur : barre progression popup = polish inutile. Ne pas relancer. Timer TMP seul sur mockup.

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-005]`  
**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Parent :** `Content` — **after** `IconFrame` in VLG order  
**Référence :** `ProgressTrack` + `ProgressFill` in `FarmHarvestPanel_WoodMockup.prefab`.

**Sprites (search by file name OK):**
- Track : `Assets/Art/Assets Store Dump/Ui/Farm/UiKit_Bar_progressTrack_9s_20261005.png` (H-farm-4). If missing / not Sprite, fallback `Assets/Art/Sprites/UI/UiKit_Row_list_20260919.png` Sliced.
- Fill : `Assets/Art/Assets Store Dump/Ui/UiKit_Bar_fill_teal_20261005.png` — child **ProgressFill** (HarvestPanelUI `stageProgressFill`).

**Hors scope :** C#, `HarvestPanelUI`, timer/yield/boutons, Simulate.

**Succès =** Save prefab + liste. **STOP.**

---

## Phase 1 — coller dans Bezy

```
[BZ-FARM-HARVEST-PANEL-BUILD-005] ProgressTrack + ProgressFill under Content. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy / Inspector ONLY. No C#. No HarvestPanelUI.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Do not rescan project. Layer 5.

TARGET: Content — keep existing children order; ADD ProgressTrack as NEXT sibling AFTER IconFrame (VLG order: PlantNameLabel, StageLabel, IconFrame, ProgressTrack).

Do NOT change root, CloseButton, Content VLG, or existing header objects.

--- ProgressTrack ---
- UI Image under Content, name: ProgressTrack
- LayoutElement: Preferred Height 76, Flexible Width 1, Flexible Height 0
- Image: prefer Assets/Art/Assets Store Dump/Ui/Farm/UiKit_Bar_progressTrack_9s_20261005.png
  If not importable: Assets/Art/Sprites/UI/UiKit_Row_list_20260919.png
  Type Sliced (1), Fill Center ON, color white, Raycast OFF

--- ProgressFill (child of ProgressTrack) ---
- UI Image, name: ProgressFill (exact — future stageProgressFill)
- RectTransform: stretch inside track with offsets ~18 each side (Left/Right/Top/Bottom 18) or match WoodMockup inset (-36 sizeDelta if stretch)
- Sprite: Assets/Art/Assets Store Dump/Ui/UiKit_Bar_fill_teal_20261005.png
- Image Type: Filled (3)
- Fill Method: Horizontal (0)
- Fill Origin: Left (0)
- Fill Amount: 0.65 (lab preview only)
- Raycast OFF, color white

Do NOT add TimerPlaque, YieldLabel, or buttons.

Save prefab. Report track sprite path + fill settings. STOP. No Play Mode.
```

**Livré 2026-10-07 :** track H-farm-4 Dump (`8dcce42e…`), fill teal horizontal **0.65**, inset **-36** — OK.
