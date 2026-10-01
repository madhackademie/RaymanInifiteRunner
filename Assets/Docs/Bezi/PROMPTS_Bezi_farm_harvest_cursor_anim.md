# [BZ-FARM-HARVEST-CURSOR-ANIM-001] Slice + clips gant

Sheets (déjà rangées) :

- `Assets/Art/Sprites/Farm/Cursor/Plant_Harverst_Idle_Cursor.png` — 2171×402, 6 gants
- `Assets/Art/Sprites/Farm/Cursor/Plant_Harvest_Harvesting_Cursor.png` — 2172×647, 6 gants

Clips cible : `Assets/Art/Animations/Farm/Cursor/`  
Modèle : `Bee_Fly.anim` (12 fps, loop, courbe `m_Sprite` sur le `SpriteRenderer` du même objet).

**Fond noir opaque** sur les deux PNG. Le slice ne le retire pas.

Un prompt = une phase. `Save. List what changed. STOP.` Pas de Simulate.

---

## Phase 1 — slice idle

```
[BZ-FARM-HARVEST-CURSOR-ANIM-001] Phase 1 slice idle only. STOP after this.

READ ONLY:
@Assets/Art/Sprites/Farm/Cursor/Plant_Harverst_Idle_Cursor.png
@Assets/Art/Sprites/Farm/Insects/Bee_Fly.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT create clips. DO NOT touch the harvest PNG.

Texture Type Sprite. Sprite Mode Multiple.
PPU 256. Mesh Type Full Rect. Pivot bottom-center (0.5, 0).
Filter Mode Bilinear. Mip Maps off. Alpha Is Transparency on.
Max size at least 4096 so the sheet is not downscaled.

Slice 6 sprites, left to right, one glove each, full sheet height.
2171 is not divisible by 6: do not force equal 256 cells. Cut on the black gaps.
Names: GloveIdle_01, GloveIdle_02, GloveIdle_03, GloveIdle_04, GloveIdle_05, GloveIdle_06.

Save. List the 6 sprite names and their rects. STOP.
```

## Phase 2 — slice récolte

```
[BZ-FARM-HARVEST-CURSOR-ANIM-001] Phase 2 slice harvest only. STOP after this.

READ ONLY:
@Assets/Art/Sprites/Farm/Cursor/Plant_Harvest_Harvesting_Cursor.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT create clips. DO NOT change the idle sheet.

Same importer as the idle sheet:
Sprite Multiple, PPU 256, Full Rect, pivot bottom-center (0.5, 0),
Bilinear, no mipmaps, Alpha Is Transparency, max size at least 4096.

6 sprites, left to right, one glove each, full height 647.
Width 2172 splits as 6 x 362 only if the gloves stay centered in each cell. If a cut hits a glove, nudge that slice.
Names: GloveHarvest_01 through GloveHarvest_06.

Save. List the 6 sprite names and their rects. STOP.
```

## Phase 3 — clips

```
[BZ-FARM-HARVEST-CURSOR-ANIM-001] Phase 3 animation clips only. STOP after this.

READ ONLY:
@Assets/Art/Animations/Farm/Insects/Bee_Fly.anim
@Assets/Art/Sprites/Farm/Cursor/Plant_Harverst_Idle_Cursor.png
@Assets/Art/Sprites/Farm/Cursor/Plant_Harvest_Harvesting_Cursor.png

DO NOT rescan the project. DO NOT edit .cs. DO NOT change slice rects. DO NOT create a controller.

Create folder Assets/Art/Animations/Farm/Cursor/ if missing.

Create GloveIdle.anim
Sprite curve on SpriteRenderer (empty path, same as Bee_Fly).
Frames GloveIdle_01 to _06 at 12 fps. Loop Time on. Length 0.5 s.

Create GloveHarvest.anim
Same setup with GloveHarvest_01 to _06. Loop Time on. 12 fps. Length 0.5 s.

Save. List the two clip paths. STOP.
```

## Phase 4 — controller + câblage

```
[BZ-FARM-HARVEST-CURSOR-ANIM-001] Phase 4 controller and prefab wire. STOP after this.

READ ONLY:
@Assets/Art/Animations/Farm/Cursor/GloveIdle.anim
@Assets/Art/Animations/Farm/Cursor/GloveHarvest.anim
@Assets/Scripts/Farm/FarmBatchHarvestCursor.cs
@Assets/Prefabs/World/Biofiltre.prefab

DO NOT rescan the project. DO NOT edit any .cs. DO NOT change slice rects or clip curves.

Create Assets/Art/Animations/Farm/Cursor/GloveCursor.controller
Bool parameter Harvesting.
Default state GloveIdle, motion = GloveIdle.anim, loop.
State GloveHarvest, motion = GloveHarvest.anim, loop.
Transition GloveIdle -> GloveHarvest when Harvesting is true. Has Exit Time off. Duration 0.
Transition GloveHarvest -> GloveIdle when Harvesting is false. Has Exit Time off. Duration 0.

On Biofiltre.prefab child BatchHarvestCursor:
Add Animator if missing. Controller = GloveCursor.
Wire FarmBatchHarvestCursor.animator to that Animator.
Leave marker on the existing SpriteRenderer. Do not add a second renderer.

Save. List controller path and the prefab field wired. STOP.
```
