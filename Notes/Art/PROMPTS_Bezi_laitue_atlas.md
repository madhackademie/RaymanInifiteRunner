# Prompts Bezy — atlas laitue → `Laitue.asset` `[BZ-FARM-LAITUE-ATLAS-001]`

**Statut :** prêt à lancer  
**Branche :** `main` (ex-`feature/biofiltre-isometric`)  
**Hors skill UI** : import texture + wiring `ScriptableObject` (pas de prefab).

**Atlas (promu auteur) :** `Assets/Art/Sprites/Plantes/Laitue/AtlasLaitue.png`  
**Cible data :** `Assets/Data/Ferme/Laitue.asset`  
**Brief art :** `Notes/Art/PROMPT_laitue_sprite_sheet_croissance.md`

**Succès Bezy = Save + liste changements. STOP. Pas de Simulate / Play Mode.**

---

## Mapping slices → `PlantDefinition`

| Slice (gauche → droite) | Champ `Laitue.asset` |
|-------------------------|----------------------|
| `Laitue_01_Graine` | `spriteGraine` |
| `Laitue_02_Starting` | `spriteStarting` |
| `Laitue_03_Baby` | `spriteBaby` |
| `Laitue_04_Growing` | `spriteGrowing` |
| `Laitue_05_Mature` | `spriteMature` |
| `Laitue_06_Flowering` | `spriteFlowering` |
| `Laitue_07_Seedling` | `spriteSeedling` |

**Un seul atlas** — ne pas extraire 7 PNG séparés.  
**Pivot cible :** Bottom (0.5, 0) sur les 7.

---

## Lancement (une phase par appel Bezy)

```
Task ID: BZ-FARM-LAITUE-ATLAS-001
Phase: 1
```

Puis `@Notes/Art/PROMPTS_Bezi_laitue_atlas.md` (section Phase N).

---

## Phase 1 — Import + renommage slices

```
Task: BZ-FARM-LAITUE-ATLAS-001 Phase 1/3 — AtlasLaitue import + rename slices.

Do NOT rescan whole project. Do NOT edit C# scripts. Do NOT Play Mode or Simulate.

TARGET ONLY:
- Assets/Art/Sprites/Plantes/Laitue/AtlasLaitue.png

REFERENCE (read-only, no edits):
- Assets/Data/Ferme/Laitue.asset

1) Select AtlasLaitue.png. Inspector:
   - Texture Type: Sprite (2D and UI)
   - Sprite Mode: Multiple
   - Pixels Per Unit: 100
   - Mesh Type: Tight
   - Alpha Is Transparency: ON
   - Generate Mip Maps: OFF
   - Filter Mode: Bilinear

2) Sprite Editor: exactly 7 slices left-to-right (growth order). If wrong, Slice → Grid By Cell Count → Columns 7, Rows 1. Trim each slice to remove empty transparent margins (no glow halo).

3) Rename left to right:
   Laitue_01_Graine, Laitue_02_Starting, Laitue_03_Baby, Laitue_04_Growing,
   Laitue_05_Mature, Laitue_06_Flowering, Laitue_07_Seedling

4) Apply. Save assets.

Done = Save. List slice names + import settings. STOP.
```

---

## Phase 2 — Pivot Bottom (grille iso)

```
Task: BZ-FARM-LAITUE-ATLAS-001 Phase 2/3 — AtlasLaitue pivots.

Do NOT rescan whole project. Do NOT edit C# scripts. Do NOT Play Mode / Simulate.

FILE: Assets/Art/Sprites/Plantes/Laitue/AtlasLaitue.png

For ALL 7 sprites (Laitue_01_Graine … Laitue_07_Seedling):
- Pivot: Bottom (0.5, 0)
- Re-trim if needed after pivot change
- Keep growth order left-to-right in the atlas

Apply. Save.

Done = Save. List pivot per slice. STOP.
```

---

## Phase 3 — Wiring `Laitue.asset`

```
Task: BZ-FARM-LAITUE-ATLAS-001 Phase 3/3 — Wire PlantDefinition.

Do NOT rescan whole project. Do NOT edit C# scripts.
Do NOT change plantId, harvestStages, footprint, stageDurations, insectKind.
Do NOT Play Mode / Simulate.

TARGET: Assets/Data/Ferme/Laitue.asset

Assign sub-sprites from AtlasLaitue.png:
- spriteGraine    → Laitue_01_Graine
- spriteStarting  → Laitue_02_Starting
- spriteBaby      → Laitue_03_Baby
- spriteGrowing   → Laitue_04_Growing
- spriteMature    → Laitue_05_Mature
- spriteFlowering → Laitue_06_Flowering
- spriteSeedling  → Laitue_07_Seedling

Leave spriteWorldOffset (0,0) unless clearly wrong. Save.

Done = Save. Confirm 7 refs assigned, none missing. STOP.
```

---

## Checklist auteur (hors Bezy)

- [ ] Playtest biofiltre : pose 2×2, stades croissance, récolte Mature + Seedling
- [ ] Alpha OK (pas halo noir)
- [ ] W2 → `promu` dans `PROMPT_generation_icones.md`
