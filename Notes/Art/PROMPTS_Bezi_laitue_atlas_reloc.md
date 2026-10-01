# Bezy — laitue atlas déplacé + review offsets `[BZ-FARM-LAITUE-ATLAS-RELOC-002]`

**Contexte 2026-10-01 :** atlas déplacé par l’auteur  
**Playtest auteur :** décalage grille = **pivot slices reset** (reimport) — corrigé pivots bas ; PPU **33** obligatoire pour la taille.  
**Ancien :** `Assets/Art/Sprites/Plantes/Laitue/AtlasLaitue.png` (guid mort — **ne plus utiliser**)  
**Nouveau (source de vérité) :** `Assets/Art/Sprites/Plantes/AtlasLaitue.png`

**Cibles :** `Laitue.asset` · `LaitueObj.prefab` · items graine/récolte · offsets grille iso  
**Référence offsets :** `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` (lecture seule)  
**Conformité atlas :** `Notes/Art/NOTE_atlas_plantes_conformite.md`

**Succès = Save + liste fichiers modifiés. STOP. Pas Simulate / Play Mode.**

---

## Noms des 7 sprites (gauche → droite)

1. `Laitue_01_Graine`
2. `Laitue_02_Starting`
3. `Laitue_03_Baby`
4. `Laitue_04_Growing`
5. `Laitue_05_Mature`
6. `Laitue_06_Flowering`
7. `Laitue_07_Seedling`

---

## Phase 1 — Atlas (subdivision + rename + pivots)

```
[BZ-FARM-LAITUE-ATLAS-RELOC-002] Phase 1 ONLY — AtlasLaitue at new path. STOP.

Do NOT rescan whole project. Do NOT edit C# scripts. No Simulate.

TARGET ONLY:
- Assets/Art/Sprites/Plantes/AtlasLaitue.png

Do NOT use Assets/Art/Sprites/Plantes/Laitue/AtlasLaitue.png (removed).

1) Inspector — Sprite Multiple, **Pixels Per Unit: 33** (WAS on old atlas before move — required or plants shrink ~3×). Tight, Alpha ON, no mipmaps.

2) Sprite Editor: exactly 7 slices left→right. Delete extra slices. Grid 7×1 then trim tight to opaque pixels.

3) Rename:
Laitue_01_Graine, Laitue_02_Starting, Laitue_03_Baby, Laitue_04_Growing,
Laitue_05_Mature, Laitue_06_Flowering, Laitue_07_Seedling

4) All 7: Pivot Bottom (0.5, 0). Re-trim if needed. Apply. Save.

Done = Save. List slice count + names. STOP.
```

---

## Phase 2 — Re-câblage refs (Missing sprites)

```
[BZ-FARM-LAITUE-ATLAS-RELOC-002] Phase 2 ONLY — Rewire laitue refs to new atlas. STOP.

Do NOT rescan whole project. Do NOT edit C# scripts. No Simulate.

READ: Assets/Art/Sprites/Plantes/AtlasLaitue.png (7 sub-sprites)

EDIT ONLY:
- Assets/Data/Ferme/Laitue.asset
- Assets/Prefabs/World/Plantes/LaitueObj.prefab
- Assets/Data/Inventaire/LaitueSeedling.asset (icon → Laitue_01_Graine)
- Assets/Data/Inventaire/LaitueMature.asset (icon → Laitue_05_Mature)

Laitue.asset — assign 7 stage sprites from NEW atlas path. Do NOT change plantId, harvestStages, footprint 2×2, stageDurations, insectKind.

LaitueObj.prefab — PlantGrow / SpriteRenderer / any sprite field: no Missing; default graine sprite from atlas.

Fix any other Missing sprite on LaitueObj only if same prefab.

Done = Save. List each file + confirm zero Missing on Laitue.asset. STOP.
```

---

## Phase 3 — Offsets pied de plante (`PlantDefinition`)

```
[BZ-FARM-LAITUE-ATLAS-RELOC-002] Phase 3 ONLY — Laitue.asset world offsets. STOP.

Do NOT rescan whole project. Do NOT edit C# scripts. No Simulate / Play Mode.

TARGET: Assets/Data/Ferme/Laitue.asset

CONTEXT: After atlas move/re-slice, plant feet may float vs iso grid. Code uses:
- spriteWorldOffset
- isoSpriteViewOffset (Y− = toward player / bottom screen)
- footprint 2×2 unchanged

TASK:
1) Open scene FirstLvl (read-only except if you must place one LaitueObj for Scene view — prefer existing instance).
2) In Scene view (NOT Play), compare laitue sprite base vs grid/biofiltre ground. Adjust ONLY on Laitue.asset:
   - spriteWorldOffset (small steps, e.g. 0.02)
   - isoSpriteViewOffset (try Y from 0 to −0.15 if feet float)
3) Do NOT change sprite refs from Phase 2.

Done = Save. Report final spriteWorldOffset + isoSpriteViewOffset values. STOP.
```

**Auteur :** playtest pose 2×2 + croissance après Phase 3 ; Cursor review diff si besoin.

---

## Cursor après Bezy

- Vérifier guid `6e57c6ff3b80ae74d818c4f3724240c9` partout (plus `325a2cf…`).
- `RoquetteObj.prefab` ne doit pas référencer l’ancien atlas laitue (sprite default).
- Mettre à jour chemins docs si encore `Laitue/AtlasLaitue`.
