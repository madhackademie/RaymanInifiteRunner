# Bezy — Barre batch `FarmHarvestPanel_Build`

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-007]`  
**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Parent :** `Content` — after `YieldLabel`  
**Référence :** `BatchHarvestButton` in `FarmHarvestPanel_WoodMockup.prefab` (PPU mult ~12 on wide bar — tune if borders vanish).

**Sprite :** `Assets/Art/Assets Store Dump/Ui/Farm/UiKit_BtnBatchHarvest_bar_20261005.png`

**Hors scope :** C#, Harvest/Uproot/Annuler, Simulate.

---

```
[BZ-FARM-HARVEST-PANEL-BUILD-007] BatchHarvestButton under Content. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy ONLY. No C#.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Layer 5. Do not rescan project.

ADD under Content after YieldLabel (VLG order):
- BatchHarvestButton

Pattern like WoodMockup (NOT nested kit prefab):
- Root: Image + Button + LayoutElement
- Sprite: Assets/Art/Assets Store Dump/Ui/Farm/UiKit_BtnBatchHarvest_bar_20261005.png
- Image Type Sliced, Fill Center ON, Raycast ON, color white
- LayoutElement: Preferred Height 120, Flexible Width 1, Flexible Height 0
- Start Pixels Per Unit Multiplier 2.5; if center invisible raise toward 8–12 (mockup uses 12)

Child Label (TMP): text "Récolte groupée" or "Balayer récolte", center, raycast off, dark brown readable on bar.

Button: no onClick. Target Graphic = root Image.

Do NOT add HarvestButton, UprootButton, AnnulerButton yet.

Save prefab. STOP. No Play Mode.
```

**Livré 2026-10-07 :** sprite H-farm-1 OK · Fill Center **on** en YAML · PPU **1** — auteur règle **8–12** à la main si centre invisible (Bezy n’expose pas PPU).
