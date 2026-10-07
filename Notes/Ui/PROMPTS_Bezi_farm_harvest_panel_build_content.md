# Bezy — `Content` + VLG sur `FarmHarvestPanel_Build`

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-003]`  
**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Référence (lecture seule) :** VLG sur `FarmHarvestPanel_WoodMockup.prefab` root — padding L88 R88 T120 B96, spacing 14, child alignment middle center.

**Hors scope :** C#, `HarvestPanelUI`, labels/boutons, resize root, Simulate.

**Succès =** Save prefab + liste. **STOP.**

---

## Phase 1 — Content shell (coller dans Bezy)

```
[BZ-FARM-HARVEST-PANEL-BUILD-003] Content + VLG shell. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy / Inspector ONLY. No C#. No HarvestPanelUI.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Do not rescan whole project. Layer 5 on all UI.

ROOT FarmHarvestPanel_Build: keep existing panel Image + RectTransform size (~598x994). Do NOT remove root Image.

1) CLEANUP: Remove any nested prefab instance under root (e.g. UiKit_Piece_BtnPrimary_Teal). Keep CloseButton.

2) HIERARCHY order under root (top to bottom draw = first to last):
   - Content (NEW)
   - CloseButton (existing — must stay LAST sibling for draw order)

3) CREATE Content:
   - UI Empty under root, name exactly: Content
   - RectTransform: anchors stretch-stretch (0,0)-(1,1)
   - Offsets: Left 48, Right 48, Top 100, Bottom 48 (room for CloseButton top-right)
   - No Image on Content (layout container only)

4) ADD VerticalLayoutGroup on Content:
   - Padding L48 R48 T16 B16 (inner, in addition to rect offsets OK)
   - Spacing 14
   - Child Alignment: Middle Center
   - Control Child Size: Width ON, Height ON
   - Use Child Scale: OFF
   - Child Force Expand: Width OFF, Height OFF

5) Do NOT add children under Content yet (no Header, no buttons).
6) Do NOT move CloseButton inside Content.
7) Do NOT edit FarmHarvestPanel.prefab / WoodMockup / TestUi scene.

Save prefab. Report Content offsets + VLG padding/spacing + sibling order. STOP. No Play Mode.
```

---

## Phase 2 — Header

Voir **`Notes/Ui/PROMPTS_Bezi_farm_harvest_panel_build_header.md`** — `[BZ-FARM-HARVEST-PANEL-BUILD-004]`.
