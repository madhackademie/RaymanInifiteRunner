# Bezy — Boutons actions (layout manuel auteur)

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-008]`  
**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Décision :** **VLG désactivé** sur `Content` — l’auteur place les RectTransform à la main après Bezy.

**Références (lecture seule) :** `UprootButton`, `HarvestButton`, `AnnulerButton` dans `FarmHarvestPanel_WoodMockup.prefab` · kit `UiKit_Piece_BtnPrimary_Teal.prefab` (sprite + PPU 2.64, **pas** nested prefab).

---

```
[BZ-FARM-HARVEST-PANEL-BUILD-008] Action buttons — manual layout. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy ONLY. No C#. No HarvestPanelUI.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Layer 5. Do not rescan project.

0) On Content: DISABLE VerticalLayoutGroup component (uncheck — do not delete). Author will move widgets manually.

1) ADD three buttons as children of Content (siblings — default stack center, author repositions later):
   Order in hierarchy bottom to top: BatchHarvestButton (keep if exists), HarvestButton, UprootButton, AnnulerButton.

Each button = root Image + Button + child TMP Label (stretch 0-1, raycast off). NO nested kit prefabs.

--- HarvestButton ---
- Name: HarvestButton
- RectTransform: anchor middle-center, pivot 0.5,0.5, SizeDelta 320 x 96, Pos (0, -80) temp
- Image: copy sprite from UiKit_Piece_BtnPrimary_Teal root — Type Sliced, PPU Mult 2.64, Raycast ON
- Button: target = Image, no onClick
- Label TMP: "Récolter", white, centered, size ~28

--- UprootButton ---
- Name: UprootButton
- SizeDelta 320 x 96, Pos (0, -190) temp
- Image sprite: Assets/Art/Assets Store Dump/Ui/UiKit_BtnDanger_coral_20261005.png — Simple or Sliced per sprite import, Preserve Aspect if Simple, Raycast ON
- Label: "Arracher", white centered

--- AnnulerButton ---
- Name: AnnulerButton (HarvestPanelUI.cancelButton)
- SizeDelta 280 x 72, Pos (0, -290) temp
- Image: read sprite from UiKit_Piece_BtnSecondary_Oval.prefab root Image — Sliced, Raycast ON
- Label: "Annuler", dark brown readable

2) Remove LayoutElement from BatchHarvestButton if it blocks manual resize (optional delete LayoutElement on batch only).

3) Do NOT wire HarvestPanelUI. Do NOT edit WoodMockup.

Save prefab. List created objects + confirm Content VLG disabled. STOP. No Play Mode.
```

**Auteur après Bezy :** Prefab Mode → déplacer/redimensionner chaque enfant sous `Content` ; batch PPU **8–12** si barre invisible ; Apply prefab.
