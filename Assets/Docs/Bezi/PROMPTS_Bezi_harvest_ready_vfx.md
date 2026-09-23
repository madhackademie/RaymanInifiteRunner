# Prompts Bezy — VFX récoltable (sparkle idle) `[BZ-POLISH-019]` / `[CT-FARM-POLISH-004]`

**Prefab cible :** `Assets/Prefabs/World/VFX/HarvestReadyFx.prefab`  
**Art (prêt, ne pas régénérer) :**
- Sprite recentré : `Assets/Art/Sprites/VFX/HarvestReady/StarsParticle.png`
- Material : `Assets/Art/Sprites/VFX/HarvestReady/M_HarvestReadySparkle.mat`
- Source brute (Dump) : `Assets/Art/Assets Store Dump/StarsParticle.png` — **ne pas utiliser** sur le ParticleSystem  
**Plante pilote :** `Assets/Prefabs/World/Plantes/LaitueObj.prefab`

**Objectif joueur :** quand un stade est récoltable (Mature feuilles/fruits **ou** Seedling graines), un soft sparkle idle au-dessus de la plante. Même prefab pour tous les loots.

**Règles :** ne pas rescanner tout le projet ; pas de nouveaux scripts C# ; pas de UI Canvas ; world VFX (layer default, **pas** layer 5) ; un prompt = une phase ; attendre OK avant la suivante.  
**Ne pas** demander Simulate / Play Mode / playtest rendu. Fin de phase : `Save. List what changed. STOP.`

**Cursor après Bezy (hors scope) :** hook `PlantGrow` → activer/désactiver l’ancre si `GetHarvestConfig(stage) != null` (miroir `SyncInsectPathForStage`).  
**Protocole création plante** : `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md` (§ HarvestReadyAnchor).

---

## Historique

- **Phase 1** : shell prefab + Sparkle PS — **OK** (Bezy 2026-07-29) ; **fix Cursor** `playOnAwake: 0`
- **Phase 2** : Color/Size/Velocity over Lifetime — **OK** (Bezy 2026-07-29) ; `playOnAwake` reste OFF
- **Phase 3** : material `M_HarvestReadySparkle` + StarsParticle BaseMap — **OK** (Bezy 2026-07-29)
- **Phase 4** : `HarvestReadyAnchor` + instance `HarvestReadyFx` sur `LaitueObj` — **OK** (Bezy 2026-07-29) ; ancre **INACTIVE** (normal)
- **Cursor hook** : `HarvestReadyFxAnchor` + `PlantGrow.SyncHarvestReadyFxForStage` — **OK** (2026-07-29)
  - Actif si `GetHarvestConfig(stage) != null` (Laitue : Mature + Seedling)
  - Debug menus : Force Mature / Force Seedling

---

## Phase 1 — Shell prefab (copier-coller Bezy)

```
[BZ-POLISH-019] Phase 1 ONLY — HarvestReadyFx shell. Wait success before Phase 2.

Do NOT rescan whole project. Do NOT create scripts. Do NOT regenerate art. Do NOT create UI Canvas.

CREATE folder if missing: Assets/Prefabs/World/VFX/
CREATE folder if missing: Assets/Art/Sprites/VFX/HarvestReady/
CREATE prefab: Assets/Prefabs/World/VFX/HarvestReadyFx.prefab

Hierarchy (world VFX — default layer, NOT layer 5):
- HarvestReadyFx (root, empty Transform)
  - Sparkle (ParticleSystem)

Sparkle ParticleSystem:
- Duration: 2.0
- Looping: ON
- Play On Awake: OFF
- Start Lifetime: 0.4–0.8 (random between)
- Start Speed: 0.05–0.25 (random, almost float in place)
- Start Size: 0.08–0.18 (random)
- Start Color: soft cream (R 1, G 0.95, B 0.75, A 0.85)
- Gravity Modifier: -0.05 (gentle rise)
- Simulation Space: Local
- Max Particles: 12
- Emission: Rate over Time = 2.5 ; NO burst
- Shape: Sphere, radius 0.18
- Renderer: Billboard ; leave default material (Phase 3)

Save prefab. Confirm hierarchy + Play On Awake OFF + Looping ON. List what changed. STOP.
```

---

## Phase 2 — Tuning (après Phase 1 OK)

```
[BZ-POLISH-019] Phase 2 ONLY — tune HarvestReadyFx. Wait success before Phase 3.

Open ONLY: Assets/Prefabs/World/VFX/HarvestReadyFx.prefab
Do NOT rescan whole project. Do NOT add scripts. Do NOT assign custom sprites.

On Sparkle:
- Color over Lifetime: cream opaque → fade alpha to 0
- Size over Lifetime: 0 → 1 at ~30% → 0 at end (twinkle pop)
- Velocity over Lifetime: soft upward Y ~+0.15 (or Radial ~0.05)
- Keep Looping ON, Play On Awake OFF, Rate ~2.5, Max Particles 12
- Feel: quiet idle sparkle above harvestable plant — NOT a burst, NOT fireworks

Save. Confirm Play On Awake still OFF. List what changed. STOP.
```

---

## Phase 3 — Material + StarsParticle (après Phase 2 OK)

```
[BZ-POLISH-019] Phase 3 ONLY — StarsParticle material on HarvestReadyFx.

Open ONLY: Assets/Prefabs/World/VFX/HarvestReadyFx.prefab
Do NOT rescan whole project. Do NOT create scripts. Do NOT regenerate PNG.
Do NOT use Assets/Art/Assets Store Dump/StarsParticle.png

Art already ready (Cursor):
- Material: Assets/Art/Sprites/VFX/HarvestReady/M_HarvestReadySparkle.mat
- Sprite/Texture: Assets/Art/Sprites/VFX/HarvestReady/StarsParticle.png

Sparkle Renderer:
- Material = M_HarvestReadySparkle
- Billboard
- Texture Sheet Animation Mode=Sprites with StarsParticle (single) OR leave texture on material BaseMap
- Keep Play On Awake OFF, Looping ON

Save prefab. List material path + confirm StarsParticle (HarvestReady folder). STOP.
```

---

## Phase 4 — Ancre sur LaitueObj (après Phase 3 OK)

```
[BZ-POLISH-019] Phase 4 ONLY — HarvestReadyAnchor on LaitueObj. Wait success. STOP after save.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT retune ParticleSystem.

EDIT prefab: Assets/Prefabs/World/Plantes/LaitueObj.prefab

Under root LaitueObj, CREATE child:
HarvestReadyAnchor (INACTIVE by default)
  - Transform local position ~ (0, 0.45, 0) — above lettuce crown
  - Child: instance of Assets/Prefabs/World/VFX/HarvestReadyFx.prefab
    name: HarvestReadyFx
    local position (0,0,0)

Do NOT wire PlantGrow scripts (Cursor later).
Keep InsectPath untouched.

Save. Confirm HarvestReadyAnchor starts DISABLED. List hierarchy. STOP.
```

---

## Checklist auteur (après Bezy, hors prompt)

| Check | OK ? |
|-------|------|
| Prefab `HarvestReadyFx` existe | |
| Looping ON, Play On Awake OFF | |
| Material = StarsParticle (HarvestReady) | |
| `LaitueObj/HarvestReadyAnchor` inactive | |
| Cursor : hook `PlantGrow` sur stade récoltable | |
| Playtest : Mature + Seedling (si harvest config) → sparkle | |
| Flowering → insecte ON, sparkle OFF (sauf si aussi récoltable) | |

---

## Phase 5 — Polish lisibilité (particules plus grosses) `[BZ-FARM-HARVEST-READY-VFX-002]`

**Statut :** **CLOS playtest 2026-09-23** (Ph.5–5f Bezy).  
**Brief auteur 2026-09-23 :** sparkle visible mais **trop timide** (Seedling OK faible ; Mature souvent invisible). **Bezy = propriétaire** du prefab VFX (pas Cursor).

**Prefab Mode :** `Assets/Prefabs/World/VFX/HarvestReadyFx.prefab` uniquement.

```
[BZ-FARM-HARVEST-READY-VFX-002] Phase 5 ONLY — harvest sparkle readability. STOP.

Do NOT rescan whole project. Do NOT edit .cs. Edit ONLY HarvestReadyFx.prefab.
Do NOT change LaitueObj unless Sparkle has instance overrides (then match prefab).

Sparkle ParticleSystem — SET (or confirm) these targets:
- Start Size: random between 0.22 and 0.45 (minMaxState Two Constants)
- Start Color: maxColor cream RGB(1, 0.95, 0.75) alpha 1
- Emission Rate over Time: 4
- Max Particles: 16
- Shape Sphere radius: 0.32
- Renderer Max Particle Size: 2
- Material: M_HarvestReadySparkle (unchanged)
- Looping ON, Play On Awake OFF, soft idle (no burst)

Keep Size/Color over Lifetime twinkle from Phase 2 if present.

Save prefab. List final Start Size, Rate, Max Particles, radius, Max Particle Size. STOP. No Play Mode.
```

---

## Phase 5b — Tri 2D sparkles (C# visuel, Bezy)

**Fichiers :** `HarvestReadyFxAnchor.cs`, `PlantGrow.cs` (appel après `SetFxActive(true)`).  
**Référence :** même pattern que `InsectPathFollower.SyncSortingOrderWithPlant` (+2 order).

```
[BZ-FARM-HARVEST-READY-VFX-002] Phase 5b ONLY — sparkle sorting above plant sprite. STOP.

@Notes/Bezi/RULES_bezy_code.md

Do NOT rescan whole project. Edit ONLY:
- Assets/Scripts/Farm/HarvestReadyFxAnchor.cs
- Assets/Scripts/Farm/PlantGrow.cs (SyncHarvestReadyFxForStage only)

GOAL: ParticleSystemRenderer sortingLayerID = plant; sortingOrder = plant + 2 when FX turns on.

HarvestReadyFxAnchor:
- Add SyncSortingWithPlant(SpriteRenderer plantRenderer) — set all child ParticleSystemRenderer layers/orders.
- On SetFxActive(true): find ancestor SpriteRenderer on plant root, call SyncSortingWithPlant before Play.

PlantGrow.SyncHarvestReadyFxForStage: after SetFxActive(true), call harvestReadyFx.SyncSortingWithPlant(spriteRenderer).

No other logic changes. Save. List methods touched. STOP. No Play Mode.
```
