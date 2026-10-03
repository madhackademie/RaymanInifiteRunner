# [BZ-FARM-PLANTING-CURSOR-ANIM-001] Slice + clips gant plantation

**Art ChatGPT (auteur, pas Bezy) :** `Notes/Art/PROMPT_curseur_gant_recolte_batch.md` §3.2 — joindre `@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Idle_Cursor.png` → promo `Plant_Planting_Sowing_Cursor.png`.

Sheets cible :

- `Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Idle_Cursor.png` — idle (**slice manuel OK** : `GlovePlantIdle_01` … `_06`, PPU 256, pivot bas)
- `Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Sowing_Cursor.png` — action clic (après ChatGPT §3.2) → slices `GlovePlantSowing_01` … `_06`

**Phase 1** : skip si idle déjà slicé avec les noms ci-dessus (auteur 2026-10-03).

**Calendrier auteur :**
- **Aujourd’hui** : Phase **3a** — clip idle `GlovePlantIdle.anim` seul (récolte idle = déjà `GloveIdle.anim`, ne pas refaire).
- **Demain (art sowing prêt)** : Phase **2** slice → **3b** clip sowing → **4** controller.

---

Clips : `Assets/Art/Animations/Farm/Cursor/`  
Modèle clip : `Assets/Art/Animations/Farm/Insects/Bee_Fly.anim` (12 fps, loop, courbe `m_Sprite` sur `SpriteRenderer`, path vide).

**Fond noir / gouttières** : comme récolte — le slice ne retire pas le noir.

Un prompt = une phase. `Save. List what changed. STOP.` Pas de Simulate.

**Ownership (crédits Bezy — Cursor s’abstient) :**

| Livrable | Agent |
|----------|--------|
| Slice, `.anim`, `.controller`, enfant prefab, SerializeField Inspector | **Bezy** (Ph. 1→5) |
| `FarmPlantingCursor.cs`, `IsPreviewModeActive` / `IsPaintStrokeActive` | **Cursor** (spec + script métier) |

**Cursor INTERDIT :** toucher `Biofiltre.prefab`, créer/modifier clips ou controllers, `Instantiate` curseur runtime, « fix rapide » pour playtest.

**Phase 5 :** livrée Bezy 2026-10-03 (`PlantPlacementCursor` sur `Biofiltre.prefab`).

**Reste (art sowing) :** Ph. **2** slice → **3b** `GlovePlantSowing.anim` → **4** motion sowing sur le controller (remplacer le placeholder idle).

---

## Phase 1 — slice idle plantation

```
[BZ-FARM-PLANTING-CURSOR-ANIM-001] Phase 1 slice planting idle only. STOP after this.

READ ONLY:
@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Idle_Cursor.png
@Assets/Art/Sprites/Farm/Insects/Bee_Fly.png
@Assets/Art/Sprites/Farm/Cursor/Plant_Harverst_Idle_Cursor.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT create clips. DO NOT touch harvest cursor PNGs.

Use harvest idle sheet importer as reference (same settings as GloveIdle slices).

Texture Type Sprite. Sprite Mode Multiple.
PPU 256. Mesh Type Full Rect. Pivot bottom-center (0.5, 0) per slice unless a slice needs nudge like harvest.
Filter Mode Bilinear. Mip Maps off. Alpha Is Transparency on.
Max size at least 4096 so the sheet is not downscaled.

Slice 6 sprites, left to right, one glove pose each, full sheet height.
Cut on black gaps — do not force equal width if gloves are centered in variable cells (same rule as harvest idle 2171px).
Names: GlovePlantIdle_01, GlovePlantIdle_02, GlovePlantIdle_03, GlovePlantIdle_04, GlovePlantIdle_05, GlovePlantIdle_06.

Save. List the 6 sprite names and their rects. STOP.
```

## Phase 2 — slice sowing (après PNG action livré)

```
[BZ-FARM-PLANTING-CURSOR-ANIM-001] Phase 2 slice planting sowing only. STOP after this.

READ ONLY:
@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Sowing_Cursor.png
@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Idle_Cursor.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT create clips. DO NOT change planting idle slice rects.

Same importer as planting idle sheet:
Sprite Multiple, PPU 256, Full Rect, pivot bottom-center (0.5, 0),
Bilinear, no mipmaps, Alpha Is Transparency, max size at least 4096.

6 sprites, left to right, one pose each, full sheet height.
Match idle sheet frame count and cut on black gaps. Nudge rects if a cut hits the glove.
Names: GlovePlantSowing_01 through GlovePlantSowing_06.

Save. List the 6 sprite names and their rects. STOP.
```

## Phase 3a — clip idle plantation seul (maintenant)

```
[BZ-FARM-PLANTING-CURSOR-ANIM-001] Phase 3a planting idle clip only. STOP after this.

READ ONLY:
@Assets/Art/Animations/Farm/Insects/Bee_Fly.anim
@Assets/Art/Animations/Farm/Cursor/GloveIdle.anim
@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Idle_Cursor.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT change slice rects on Plant_Planting_Idle_Cursor. DO NOT create GlovePlantSowing.anim. DO NOT create a controller.

Create folder Assets/Art/Animations/Farm/Cursor/ if missing.

Create Assets/Art/Animations/Farm/Cursor/GlovePlantIdle.anim
Copy curve setup from GloveIdle.anim (SpriteRenderer m_Sprite, empty path, 12 fps, Loop Time on, length 0.5 s).
Use sprites GlovePlantIdle_01, GlovePlantIdle_02, GlovePlantIdle_03, GlovePlantIdle_04, GlovePlantIdle_05, GlovePlantIdle_06 from Plant_Planting_Idle_Cursor.png in that order left to right.

Save. List clip path and the 6 sprite references used. STOP.
```

## Phase 3b — clip sowing (après Phase 2)

```
[BZ-FARM-PLANTING-CURSOR-ANIM-001] Phase 3b planting sowing clip only. STOP after this.

READ ONLY:
@Assets/Art/Animations/Farm/Cursor/GlovePlantIdle.anim
@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Sowing_Cursor.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT change slice rects. DO NOT create a controller. DO NOT modify GlovePlantIdle.anim.

Create GlovePlantSowing.anim in Assets/Art/Animations/Farm/Cursor/
Same setup as GlovePlantIdle.anim: SpriteRenderer m_Sprite, empty path, 12 fps, Loop Time on, length 0.5 s.
Frames GlovePlantSowing_01 through GlovePlantSowing_06 in order.

Save. List clip path. STOP.
```

## Phase 3 — clips plantation (les deux — option une session)

```
[BZ-FARM-PLANTING-CURSOR-ANIM-001] Phase 3 planting animation clips only. STOP after this.

READ ONLY:
@Assets/Art/Animations/Farm/Insects/Bee_Fly.anim
@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Idle_Cursor.png
@Assets/Art/Sprites/Farm/Cursor/Plant_Planting_Sowing_Cursor.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT change slice rects. DO NOT create a controller.

Create folder Assets/Art/Animations/Farm/Cursor/ if missing.

Create GlovePlantIdle.anim
Sprite curve on SpriteRenderer (empty path, same as Bee_Fly).
Frames GlovePlantIdle_01 to _06 at 12 fps. Loop Time on. Length 0.5 s.

Create GlovePlantSowing.anim
Same setup with GlovePlantSowing_01 to _06. Loop Time on. 12 fps. Length 0.5 s.

Save. List the two clip paths. STOP.
```

## Phase 4 — controller plantation (sans prefab)

```
[BZ-FARM-PLANTING-CURSOR-ANIM-001] Phase 4 planting controller only. STOP after this.

READ ONLY:
@Assets/Art/Animations/Farm/Cursor/GlovePlantIdle.anim
@Assets/Art/Animations/Farm/Cursor/GlovePlantSowing.anim
@Assets/Art/Animations/Farm/Cursor/GloveCursor.controller

DO NOT rescan the project. DO NOT edit any .cs. DO NOT change slice rects or clip curves. DO NOT modify Biofiltre.prefab.

Create Assets/Art/Animations/Farm/Cursor/GlovePlantCursor.controller
Bool parameter Planting (same pattern as Harvesting on GloveCursor).
Default state GlovePlantIdle, motion = GlovePlantIdle.anim, loop.
State GlovePlantSowing, motion = GlovePlantSowing.anim, loop.
Transition idle -> sowing when Planting is true. Has Exit Time off. Duration 0.
Transition sowing -> idle when Planting is false. Has Exit Time off. Duration 0.

Do NOT wire prefab in this phase.

Save. List controller path and parameter name. STOP.
```

## Phase 5 — câblage prefab (Bezy — obligatoire pour playtest)

```
[BZ-FARM-PLANTING-CURSOR-ANIM-001] Phase 5 prefab wire only. STOP after this.

READ ONLY:
@Assets/Prefabs/World/Biofiltre.prefab
@Assets/Prefabs/World/Biofiltre.prefab (child BatchHarvestCursor — copy pattern)
@Assets/Scripts/Farm/FarmPlantingCursor.cs
@Assets/Art/Animations/Farm/Cursor/GlovePlantCursor.controller
@Assets/Scripts/Farm/PlantPlacementPreview.cs (on Biofiltre root)

DO NOT rescan the project. DO NOT edit any .cs. DO NOT change slice rects or clip curves.

On Biofiltre.prefab root, add child PlantPlacementCursor (sibling of BatchHarvestCursor):
- Transform localScale 0.75, 0.75, 0.75 (same as harvest cursor).
- SpriteRenderer: disabled by default, sorting order 60, default URP 2D material like BatchHarvestCursor.
- Animator: controller = GlovePlantCursor.
- FarmPlantingCursor: marker = SpriteRenderer, animator = Animator, placementPreview = PlantPlacementPreview on Biofiltre root.

Do not duplicate BatchHarvestCursor. One planting cursor child only.

Save. List prefab path and wired SerializeFields. STOP.
```
