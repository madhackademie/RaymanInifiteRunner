# Prompts Bezy — VFX plantation / arrachage / récolte `[BZ-POLISH-016]` / `[CT-FARM-POLISH-003]`

**Prefab cible :** `Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab`  
**Art (déjà importé, ne pas régénérer) :**
- Terre / cailloux / feuilles : `Assets/Art/Sprites/VFX/Planting/PlantationDirtParticules.png`  
  Sprites : `PlantationDirtParticules_0` … `_10`
- Vers : `Assets/Art/Sprites/VFX/Planting/wurmParticleFarmPlantation.png`  
  Sprite : `wurmParticleFarmPlantation_0`

**Règles :** ne pas rescanner tout le projet ; pas de nouveaux scripts C# ; un prefab réutilisable plant + arrachage + récolte.  
**Cursor après Bezy :** hook `Play()` (hors scope Bezy).

---

## Historique

- **Phase 1** : shell prefab + 2 ParticleSystem — **OK** (Bezy 2026-07-23) ; correctif Cursor `playOnAwake` OFF
- **Phase 2** : tuning burst / fade — **OK** (Bezy 2026-07-23, validé Cursor repo)
- **Phase 3** : sprites + material — **fix Cursor YAML** (2026-07-25)
  - Bezy Phase 3 : material + Worm 2–5 OK ; sprites non branchés
  - Bezy Phase 3b : stop (« can't verify Simulate ») — normal, limite agent
  - Cursor : Texture Sheet Animation Mode=Sprites branché (Dirt `_0…_10`, Worm `_0`) ; Start Frame dirt random
  - Fix 2026-07-25 : Mesh Type sprites **Full Rect** (pas Tight) ; materials BaseMap ; menu `Rayman/VFX/Bind PlantingDirtBurst Sprites` si Inspector vide

### Validation Phase 2 (repo)

| Critère | Statut |
|---------|--------|
| Size over Lifetime 1 → 0.4 | OK |
| Color over Lifetime alpha → 0 | OK |
| Velocity radial ~1.2 | OK |
| Rotation Dirt ±180° / Worm ±90° | OK |
| Play On Awake OFF | OK |
| Rate=0 + Burst 14/1 | OK |

### Validation Phase 1 (repo)

| Critère | Statut |
|---------|--------|
| Prefab `Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab` | OK |
| Hiérarchie `DirtBurst` + `WormBurst` | OK |
| Duration 0.6, Looping OFF | OK |
| Rate=0, Burst Dirt=14 / Worm=1 | OK |
| Max Particles 24 / 2, Gravity 0.4 / 0.15 | OK |
| Shape Circle radius 0.15 edge | OK |
| Play On Awake OFF | OK (fix Cursor après livraison Bezy qui était ON) |

---

## Phase 1 — Shell prefab (copier-coller Bezy)

```
[BZ-POLISH-016] Phase 1 ONLY — PlantingDirtBurst shell. Wait success before Phase 2.

Do NOT rescan whole project. Do NOT create scripts. Do NOT regenerate art. Do NOT create UI Canvas.

CREATE folder if missing: Assets/Prefabs/World/VFX/
CREATE prefab: Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab

Hierarchy (world, NOT UI — default layer OK, not layer 5):
- PlantingDirtBurst (root, empty Transform)
  - DirtBurst (ParticleSystem)
  - WormBurst (ParticleSystem)

Both PS:
- Duration 0.6
- Looping OFF
- Play On Awake OFF
- Start Lifetime 0.35–0.55 (random between)
- Start Speed 1.5–3.5 (random)
- Start Size: DirtBurst 0.12–0.35 ; WormBurst 0.35–0.55
- Gravity Modifier: Dirt 0.4 ; Worm 0.15
- Simulation Space: World
- Max Particles: Dirt 24 ; Worm 2
- Emission: Rate over Time = 0 ; Burst time=0 count Dirt=14 Worm=1
- Shape: Circle, radius 0.15, emit from Edge, align to Direction
- Renderer: Billboard ; leave default material for now (Phase 3)

Save prefab. Confirm hierarchy + Play On Awake OFF. STOP.
```

---

## Phase 2 — Tuning modules (après Phase 1 OK)

```
[BZ-POLISH-016] Phase 2 ONLY — tune PlantingDirtBurst. Wait success before Phase 3.

Open ONLY: Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab
Do NOT rescan whole project. Do NOT add scripts. Do NOT assign sprites yet.

On DirtBurst + WormBurst:
- Color over Lifetime: opaque → alpha 0 (fade out)
- Size over Lifetime: 1.0 → ~0.4
- Velocity over Lifetime: Radial ~1.2 (outward burst feel)
- Rotation over Lifetime: Dirt ±180° random ; Worm ±90°
- Keep Duration ~0.6, Looping OFF, Play On Awake OFF, Rate=0 + Burst

Feel: short radial dirt pop (~0.5–0.7s), worm rarer/larger, no endless emit.
Save. Confirm Play On Awake still OFF. STOP.
```

---

## Phase 3 — Sprites + material (après Phase 2 OK)

```
[BZ-POLISH-016] Phase 3 ONLY — sprites + material on PlantingDirtBurst.

Open ONLY: Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab
Do NOT rescan whole project. Do NOT create scripts. Do NOT regenerate PNG.

Material: URP Particles/Unlit, Transparent / Alpha Blended (create under Assets/Art/Sprites/VFX/Planting/ if missing, e.g. M_PlantingDirtParticles).

DirtBurst Renderer:
- Material = that particle material
- Assign Texture Sheet Animation OR Renderer Sprites list with:
  PlantationDirtParticules_0 … _10 (random / frame over lifetime OK)
- Do NOT include worm here

WormBurst Renderer:
- Same material
- Sprite ONLY: wurmParticleFarmPlantation_0

Keep Play On Awake OFF.
Burst counts:
- DirtBurst: ≈14 (unchanged)
- WormBurst: Emission Burst count = Random between 2 and 5 (NOT fixed 1)
- WormBurst Max Particles: at least 5 (was 2 — raise if needed)

Save prefab. List final hierarchy + material path + Worm burst min/max. STOP.
```

---

## Phase 3b — Fix sprites ONLY (après review Cursor KO)

```
[BZ-POLISH-016] Phase 3b ONLY — assign sprites on PlantingDirtBurst. Wait success. STOP after.

Open ONLY: Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab
Keep material M_PlantingDirtParticles already assigned. Do NOT rescan project. Do NOT create scripts. Do NOT regenerate PNG. Do NOT change Dirt/Worm motion tuning except sprites.

On DirtBurst → Particle System → Texture Sheet Animation:
- Enable module
- Mode = Sprites (NOT Grid)
- Clear empty sprite slot
- Add sprites PlantationDirtParticules_0 through PlantationDirtParticules_10
  (from Assets/Art/Sprites/VFX/Planting/PlantationDirtParticules.png)
- Frame over Time or Start Frame: random among frames OK

On WormBurst → Texture Sheet Animation:
- Enable module
- Mode = Sprites
- Sprite ONLY: wurmParticleFarmPlantation_0
  (from Assets/Art/Sprites/VFX/Planting/wurmParticleFarmPlantation.png)
- Do NOT add dirt sprites here

Keep Play On Awake OFF. Worm burst still Random 2–5. Max Particles Worm >= 5.
Save. List hierarchy + which sprites assigned on DirtBurst / WormBurst. STOP.
(Author playtests Simulate separately — do NOT require Bezy to confirm visual result.)
```

---

## Checklist validation (auteur)

- [x] Prefab existe : `Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab`
- [x] 2 enfants PS : `DirtBurst` + `WormBurst`
- [x] Play On Awake = OFF (Cursor appellera `Play()`)
- [x] Burst unique court, pas de loop
- [x] WormBurst : 2–5 particules (random), Max Particles ≥ 5
- [x] Material `M_PlantingDirtParticles` assigné
- [x] Sprites terre + vers branchés (YAML Cursor) — **playtest Simulate auteur**
- [ ] Un seul prefab pour plant / arrachage / récolte

## Anti-patterns

- Pas de Canvas / layer UI 5 (c’est du VFX monde)
- Pas de `Play On Awake` (sinon burst au spawn de scène)
- Pas de Rate over Time > 0 (fuite de particules)
- Pas de régénération d’art ChatGPT
- Pas de hook C# dans ce job Bezy
