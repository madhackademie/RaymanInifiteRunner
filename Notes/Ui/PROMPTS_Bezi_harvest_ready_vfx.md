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
