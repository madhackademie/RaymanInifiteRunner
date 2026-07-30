# Prompts Bezy — VFX pièces / billets vente `[BZ-POLISH-018]`

**Prefab cible :** `Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab`  
**Écran lié :** `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` (spawn runtime Cursor, pas de wiring Bezy)  
**Spec UI :** `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`  
**Pattern de référence :** `Notes/Ui/PROMPTS_Bezi_planting_dirt_vfx.md` (`PlantingDirtBurst`)

**Feel cible :** au feedback vente réussie (hook Cursor), burst court de **pièces** + **billets** ancré sur le **popup quantité** (`ShopItemPopup`), pas sur le bandeau (~0.7–1.0 s).

**Art (prérequis auteur — Bezy ne régénère PAS de PNG) :**
- Dossier cible : `Assets/Art/Sprites/VFX/Sale/`
- `CoinParticle.png` → slices `CoinParticle_0` … (1+ frames OK)
- `BillParticle.png` → slices `BillParticle_0` … (1+ frames OK)
- Si art absent : Phases 1–2 avec **Start Color** placeholder (jaune pièce / vert billet) ; Phase 3 seulement quand les PNG sont importés.

**Règles :** ne pas rescanner tout le projet ; pas de scripts C# ; un prompt = une phase ; `Play On Awake` OFF ; pas de playtest Simulate comme critère Bezy.  
**Cursor après Bezy :** `SaleMoneyBurstVfx` + `RuntimeSaleChannelsScreen` — ancre = `MoneyBurstAnchor` sous `ShopItemPopup` (Phase 4).  
Runtime = burst **UI Images** (HUD Overlay masque les PS monde) ; prefab PS = ref Bezy / Phase 3 sprites.

---

## Historique

- **Phase 1** : shell prefab + 2 ParticleSystem — **OK** (Bezy 2026-07-26) ; correctif Cursor `playOnAwake` OFF (même piège que DirtBurst)
- **Phase 2** : tuning burst / gravity / fade — **OK** (Bezy 2026-07-26, validé Cursor repo)
- **Phase 3** : sprites + material — bloqué tant que art manquant
- **Phase 4** : ancre `MoneyBurstAnchor` sur `ShopItemPopup` — **OK** (Bezy 2026-07-29) ; hook Cursor ancré popup **OK**

### Validation Phase 2 (repo)

| Critère | Statut |
|---------|--------|
| Size over Lifetime 1 → 0.55 | OK |
| Color over Lifetime alpha → 0 (fade fin) | OK |
| Velocity radial ~0.8 + Y +0.5 | OK |
| Rotation Coin ±360° / Bill ±90° | OK |
| Play On Awake OFF | OK |

### Validation Phase 1 (repo)

| Critère | Statut |
|---------|--------|
| Prefab `Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab` | OK |
| Hiérarchie `CoinBurst` + `BillBurst` | OK |
| Duration 0.85, Looping OFF | OK |
| Rate=0, Burst Coin=10 / Bill=4 | OK |
| Max Particles 20 / 10, Gravity 1.1 / 0.55 | OK |
| Play On Awake OFF | OK (fix Cursor) |

---

## Phase 1 — Shell prefab (copier-coller Bezy)

```
[BZ-POLISH-018] Phase 1 ONLY — SaleMoneyBurst shell. Wait success before Phase 2.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT regenerate art. Do NOT edit SaleChannelsScreen.

CREATE folder if missing: Assets/Prefabs/Ui/VFX/
CREATE prefab: Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab

Hierarchy (NOT UI Canvas — default layer OK, not layer 5):
- SaleMoneyBurst (root, empty Transform)
  - CoinBurst (ParticleSystem)
  - BillBurst (ParticleSystem)

Both PS:
- Duration 0.85
- Looping OFF
- Play On Awake OFF
- Start Lifetime 0.45–0.75 (random between)
- Start Speed: Coin 2.0–4.0 ; Bill 1.2–2.8 (random)
- Start Size: Coin 0.18–0.32 ; Bill 0.28–0.45
- Start Color placeholder: Coin yellow (#FFD54A) ; Bill green (#7BC67E)
- Gravity Modifier: Coin 1.1 ; Bill 0.55
- Simulation Space: World
- Max Particles: Coin 20 ; Bill 10
- Emission: Rate over Time = 0
  Burst time=0 : Coin count=10 ; Bill count=4
- Shape: Circle, radius 0.12, emit from Edge, align to Direction
- Renderer: Billboard ; default material OK (Phase 3)

Save. Confirm hierarchy + Play On Awake OFF. List paths. STOP.
```

---

## Phase 2 — Tuning modules (après Phase 1 OK)

```
[BZ-POLISH-018] Phase 2 ONLY — tune SaleMoneyBurst. Wait success before Phase 3.

Open ONLY: Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab
Do NOT rescan whole project. Do NOT add scripts. Do NOT assign sprites yet.

On CoinBurst + BillBurst:
- Color over Lifetime: opaque → alpha 0 (fade out last ~30%)
- Size over Lifetime: 1.0 → ~0.55
- Velocity over Lifetime: Radial ~0.8 (outward pop) + slight upward bias if easy (Y +0.5 start)
- Rotation over Lifetime: Coin ±360° ; Bill ±90° (bills flatter tumble)
- Keep Duration ~0.85, Looping OFF, Play On Awake OFF, Rate=0 + Burst

Feel: short money pop (~0.7–1.0s), coins more + faster, bills fewer + larger, no endless emit.
Save. Confirm Play On Awake still OFF. STOP.
```

---

## Phase 3 — Sprites + material (après Phase 2 OK **et** art importé)

```
[BZ-POLISH-018] Phase 3 ONLY — sprites + material on SaleMoneyBurst.

Open ONLY: Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab
Do NOT rescan whole project. Do NOT create scripts. Do NOT regenerate PNG.

Prereq art (author already imported):
- Assets/Art/Sprites/VFX/Sale/CoinParticle.png → CoinParticle_0…
- Assets/Art/Sprites/VFX/Sale/BillParticle.png → BillParticle_0…
Mesh Type sprites = Full Rect (not Tight).

Material: URP Particles/Unlit, Transparent / Alpha Blended
  create under Assets/Art/Sprites/VFX/Sale/ if missing, e.g. M_SaleMoneyParticles.
  GPU Instancing OFF. Enable _BASEMAP / Base Map keyword if needed.

CoinBurst Renderer:
- Material = M_SaleMoneyParticles
- Texture Sheet Animation Mode=Sprites : CoinParticle_0… (all coin frames)
- Start Frame random OK

BillBurst Renderer:
- Same material
- Texture Sheet Animation Mode=Sprites : BillParticle_0… only (no coins)

Keep Play On Awake OFF. Burst counts unchanged (Coin≈10, Bill≈4).
Save. List material path + sprites assigned. STOP.
(Author playtests Simulate separately — do NOT require Bezy to confirm visual.)
```

---

## Phase 4 — Ancre burst sur popup vente — **CLOS Bezy + Cursor** (2026-07-29)

**Livré Bezy :** `ShopItemPopup/Root/MoneyBurstAnchor` (layer 5, centre +Y20) + enfant `SaleMoneyBurst` inactif.  
**Livré Cursor :** `RuntimeSaleChannelsScreen` ancre `SaleMoneyBurstVfx` sur `MoneyBurstAnchor` avant `Close()`.

**Objectif (historique) :** point d’ancrage UI sur `ShopItemPopup` (quantité / validation voisinage).

```
[BZ-POLISH-018] Phase 4 ONLY — MoneyBurstAnchor on ShopItemPopup. Wait success. STOP after save.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT edit SaleChannelsScreen. Do NOT retune SaleMoneyBurst particles.

EDIT ONLY: Assets/Prefabs/Ui/ShopItemPopup.prefab

Under Root (child of ShopItemPopup), CREATE empty UI child:
  MoneyBurstAnchor
  - RectTransform
  - Layer = 5 (UI)
  - Anchors center of Card (or center of Root if Card hard to parent):
    anchorMin/Max (0.5, 0.5), pivot (0.5, 0.5)
    sizeDelta (0, 0)
    anchoredPosition (0, 0) — center of popup card
  - NO Image, NO Button, NO Canvas
  - Leave active (empty marker only)

Optional (inactive): nest instance of Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab
  as child of MoneyBurstAnchor, name SaleMoneyBurst, SetActive false
  (Simulate ref only — runtime Cursor uses UI burst)

Do NOT wire scripts. Do NOT move ConfirmButton / Header / Backdrop.

Save. List hierarchy path of MoneyBurstAnchor. STOP. No Simulate / Play Mode.
```

**Cursor :** livré — burst sur `MoneyBurstAnchor` avant `Close()` (plus sur le bandeau).

---

## Checklist validation (auteur)

| Critère | OK? |
|---------|-----|
| Prefab `Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab` | |
| Enfants `CoinBurst` + `BillBurst` | |
| Play On Awake OFF | |
| Rate=0 + Burst unique (pas de loop) | |
| Art coin/billet importé Full Rect | |
| Material + Texture Sheet Sprites branchés | |
| `ShopItemPopup/Root/MoneyBurstAnchor` (+ SaleMoneyBurst inactif) | x |
| Cursor hook ancré bouton Confirmer/Valider (`ConfirmPurchaseButton`) | x (playtest 2026-07-29) |

## Anti-patterns

- Pas de Canvas / layer UI 5 sur le prefab VFX (spawn monde / overlay Cursor)
- Pas de `Play On Awake` (sinon burst au spawn)
- Pas de Rate over Time > 0
- Pas de régénération d’art par Bezy
- Pas de hook C# / wiring `SaleChannelsScreen` dans ce job Bezy
- Pas de « confirm Simulate looks good » comme critère Bezy
