# Bezy — Refaire Récolter mockup popup farm

**Task :** `[BZ-FARM-HARVEST-MOCK-KIT-001-REDO]`  
**Prefab :** `Assets/Prefabs/Ui/FarmHarvestPanel_WoodMockup.prefab`  
**Référence visuelle OK isolée :** `Assets/Prefabs/Ui/UiKit_Piece_BtnPrimary_Teal.prefab` (360×120 labo — **ne pas** nested stretch).  
**Référence layout OK dans le même mockup :** `UprootButton` (Image + Button + Label sur le même pattern).  
**Scène test auteur :** Canvas **1080×1920**, Game **1920×1080 Portrait** — ne pas modifier Render Mode Canvas scène.

**Problème session 2026-10-06 :** nested kit + stretch / mauvaise taille → **9-slice teal cassé**, bouton moche.  
**Décision :** **abandonner** nested prefab sur `HarvestButton` ; recopier le **pattern UprootButton** avec le **sprite teal du kit**.

**Succès = Save + liste changements. STOP. Pas Simulate / Play Mode.**

---

## Phase REDO — Récolter (coller tel quel dans Bezy)

```
[BZ-FARM-HARVEST-MOCK-KIT-001-REDO] Refaire HarvestButton — pattern Uproot, pas nested kit. STOP.

OPEN Prefab Mode: Assets/Prefabs/Ui/FarmHarvestPanel_WoodMockup.prefab
@Notes/Bezi/RULES_bezy_code.md — layout/Inspector only. Do NOT edit HarvestPanelUI.cs.

READ (do not rescan project):
- UiKit_Piece_BtnPrimary_Teal.prefab — copy Image sprite + Sliced + PPU Multiplier 2.64 ONLY.
- UprootButton in same mockup — copy hierarchy pattern (Image+Button+LayoutElement+Label child).

TARGET: GameObject HarvestButton (must keep name + Button for HarvestPanelUI.harvestButton).

1) REMOVE nested prefab instance UiKit_Piece_BtnPrimary_Teal under HarvestButton (broken 9-slice).

2) REBUILD HarvestButton like UprootButton:
   - RectTransform: layout-driven (anchors 0,0 like Uproot, pivot 0.5,0).
   - CanvasRenderer + Image on HarvestButton root:
     - Same sprite as kit teal (from UiKit_Piece_BtnPrimary_Teal root Image).
     - Image Type: Sliced (NOT Simple stretch).
     - Pixels Per Unit Multiplier: 2.64 (match kit).
   - Button on same GO; Target Graphic = this Image; Color Tint transition.
   - Layout Element: Preferred Width 320, Preferred Height 96, Flexible Width 0 (match UprootButton row).

3) One child Label (TMP): text "Récolter", styling like kit Label or Uproot Label (white, centered, raycast off).

4) Hierarchy order under mockup root unchanged: BatchHarvestButton immediately above HarvestButton.

5) Do NOT touch: BatchHarvestButton, AnnulerButton, CloseButton, FarmHarvestPanel.prefab.

6) Re-check HarvestPanelUI inspector: harvestButton still assigned to HarvestButton's Button.

Save prefab. Report final RectTransform size + Image type + PPU mult. STOP. No Play Mode.
```

---

## Si encore trop grand / 9-slice sale (Phase 2 optionnelle, auteur playtest d'abord)

```
[BZ-FARM-HARVEST-MOCK-KIT-001-REDO] P2 tune size only. REDO OK. STOP.

Same prefab. HarvestButton only.
Try LayoutElement Preferred 300×88 OR raise Image PPU Multiplier to 3.0 (smaller wood border on screen).
Do not change sprite asset or switch to full-width stretch.

Save. STOP.
```
