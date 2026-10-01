# Prompts Bezy — atlas roquette → `Roquette.asset` `[BZ-FARM-ROQUETTE-ATLAS-001]`

**Atlas :** `Assets/Art/Sprites/Plantes/AtlasRoquette.png`  
**Référence import :** `Assets/Art/Sprites/Plantes/AtlasLaitue.png` (read-only)  
**Cible data (Phase 3) :** `Assets/Data/Ferme/Roquette.asset`  
**Conformité :** `Notes/Art/NOTE_atlas_plantes_conformite.md`

**Succès Bezy = Save + liste changements. STOP. Pas Simulate / Play Mode.**

**2026-10-01 auteur :** Phase 1–2 **faites à la main** (7 slices + pivot **base plante**, X pas forcément 0,5). Bezy = **Phase 3 wiring** seulement.

**Pivot :** `(0.5, 0)` = convention si le pied est centré dans le rect trimé. Sinon pivot **sur la base visuelle** (Y=0, X libre) = **OK** et souvent mieux en iso.

---

## Noms des 7 sprites (gauche → droite)

1. `Roquette_01_Graine`
2. `Roquette_02_Starting`
3. `Roquette_03_Baby`
4. `Roquette_04_Growing`
5. `Roquette_05_Mature`
6. `Roquette_06_Flowering`
7. `Roquette_07_Seedling`

---

## Mapping → `PlantDefinition` (Phase 3)

| Slice | Champ |
|-------|--------|
| `Roquette_01_Graine` | `spriteGraine` |
| `Roquette_02_Starting` | `spriteStarting` |
| `Roquette_03_Baby` | `spriteBaby` |
| `Roquette_04_Growing` | `spriteGrowing` |
| `Roquette_05_Mature` | `spriteMature` |
| `Roquette_06_Flowering` | `spriteFlowering` |
| `Roquette_07_Seedling` | `spriteSeedling` |

---

## Lancement (une phase par appel)

```
Task ID: [BZ-FARM-ROQUETTE-ATLAS-001]
Phase: 1
```

Puis `@Notes/Art/PROMPTS_Bezi_roquette_atlas.md` (section Phase N).

---

## Phase 1 — Subdivision + renommage (auteur demande 2026-10-01)

```
[BZ-FARM-ROQUETTE-ATLAS-001] Phase 1 ONLY — AtlasRoquette slice + rename. STOP.

Do NOT rescan whole project. Do NOT edit C# scripts. Do NOT edit Roquette.asset yet.
Do NOT Play Mode or Simulate.

TARGET ONLY:
- Assets/Art/Sprites/Plantes/AtlasRoquette.png

REFERENCE (read-only, same import as laitue):
- Assets/Art/Sprites/Plantes/AtlasLaitue.png

1) Select AtlasRoquette.png. Inspector — match AtlasLaitue:
   - Texture Type: Sprite (2D and UI)
   - Sprite Mode: Multiple
   - Pixels Per Unit: **33** (same as AtlasLaitue — not 100)
   - Mesh Type: Tight
   - Alpha Is Transparency: ON
   - Generate Mip Maps: OFF
   - Filter Mode: Bilinear

2) Sprite Editor: DELETE extra/auto slices. Exactly 7 sprites, growth order left→right.
   - Start: Slice → Grid By Cell Count → Columns 7, Rows 1 (then manual trim).
   - Trim each rect tight to opaque pixels (no black/glow halo in rect).

3) Rename left to right EXACTLY:
   Roquette_01_Graine
   Roquette_02_Starting
   Roquette_03_Baby
   Roquette_04_Growing
   Roquette_05_Mature
   Roquette_06_Flowering
   Roquette_07_Seedling

4) Apply. Save assets.

Done = Save. List final slice names + rect count (must be 7). STOP.
```

---

## Phase 2 — Pivots bas

**Clos auteur** (manuel) — ne pas relancer Bezy Phase 2.

---

## Phase 3 — Wiring `Roquette.asset`

```
[BZ-FARM-ROQUETTE-ATLAS-001] Phase 3 ONLY — Wire PlantDefinition. STOP.

Do NOT rescan whole project. Do NOT edit C# scripts. No Simulate.

TARGET: Assets/Data/Ferme/Roquette.asset

Do NOT change plantId, harvestStages, footprint, stageDurations, insectKind.

Assign sub-sprites from AtlasRoquette.png:
- spriteGraine    → Roquette_01_Graine
- spriteStarting  → Roquette_02_Starting
- spriteBaby      → Roquette_03_Baby
- spriteGrowing   → Roquette_04_Growing
- spriteMature    → Roquette_05_Mature
- spriteFlowering → Roquette_06_Flowering
- spriteSeedling  → Roquette_07_Seedling

Optional same atlas icons (if fields exist on items, skip if not in scope):
- RoquetteSeedling.asset icon → Roquette_01_Graine
- RoquetteMature.asset icon → Roquette_05_Mature

Leave spriteWorldOffset (0,0) unless obviously wrong. Save.

Done = Save. List assigned fields. STOP.
```

**Cursor après Bezy :** review diff `.meta` + `Roquette.asset`, corriger refs / conformité, playtest auteur.
