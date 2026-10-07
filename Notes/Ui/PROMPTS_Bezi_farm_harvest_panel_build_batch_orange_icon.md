# Bezy — Batch orange + icône `ImageBtnFarmBatch`

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-007-LITE]`  
**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Cible :** `BatchHarvestButton` (remplace barre bois H-farm-1)  
**Référence PPU :** `UiKit_Piece_BtnPrimary_Teal.prefab` → **2.64** sur 9-slice primary.

**Sprites :**
- Cadre : `Assets/Art/Sprites/UI/UiKit_BtnPrimary_orange_20260919.png` (Sliced, border ~200/90)
- Icône : `Assets/Art/Assets Store Dump/Ui/Bouton/ImageBtnFarmBatch.png` (Simple, Preserve Aspect)

**Hors scope :** C#, Simulate, Harvest/Uproot/Annuler, nested kit prefab stretch.

---

```
[BZ-FARM-HARVEST-PANEL-BUILD-007-LITE] BatchHarvestButton orange + icon. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy ONLY. No C#.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Layer 5 UI. Do not rescan whole project.

EDIT BatchHarvestButton (under Content):
- Root Image: sprite UiKit_BtnPrimary_orange_20260919.png
- Type Sliced, Fill Center ON, color white, Raycast ON
- Pixels Per Unit Multiplier 2.64 (tune 2.2–3.5 if borders too thick)
- Keep Button + LayoutElement (Preferred Height ~120, Flexible Width 1)

Children order:
1) IconBatch — NEW Image child
   - Sprite ImageBtnFarmBatch.png, Type Simple, Preserve Aspect ON, Raycast OFF
   - RectTransform: anchor stretch vertical, left inset ~16, width ~96–112, height fills with ~8px vertical padding
2) Label — existing TMP
   - Text "Récolte groupée", raycast OFF
   - Anchor left-center or stretch; left offset ~120 so text sits right of icon; dark brown readable on orange

Do NOT use UiKit_BtnBatchHarvest_bar_20261005.png on root anymore.
Do NOT nest UiKit_Piece prefabs as stretched child.

Save prefab. List what changed. STOP. No Play Mode.
```

**Playtest auteur :** FirstLvl sous `FarmPopupRoot` — ajuster PPU / taille icône si parchemin trop large.
