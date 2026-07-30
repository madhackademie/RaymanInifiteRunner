# File Bezy — polish semaine (overload crédits)

**Crédits Bezy :** reset mensuel en dur le **30** de chaque mois (pas le 1er / pas « fin de mois » si ≠ 30). Planifier jobs lourds juste après.

**Objectif :** liste de jobs **Bezy-only** (prefabs / Animator / layers / lisibilité / micro-anims).  
**Hors scope Bezy :** génération d’images/sprites, logique C# métier (Cursor).  
**Playtests :** batch `Notes/Todo_playtest.md` (pas prioritaire tant que file Bezy).

Statuts officiels : `Notes/Todo_project.md`.  
Prompts : créer/étendre `Notes/Ui/PROMPTS_Bezi_*.md` **avant** d’envoyer (phases 1→2→3, &lt; 3500 car.).

---

## Ordre suggéré (semaine)

> **Priorité polish Bezy :** file **#11** ([BZ-POLISH-011] LoadingScreen) — [BZ-POLISH-010] filigrane **Bezy CLOS** 2026-07-30.
> **Park UX 2026-07-27 :** `[BZ-POLISH-015]` — docs only (code jeté) ; attendre GDD §5.8 (`NOTE_affichage_monnaie_hud.md`).  
> **Clos 2026-07-23 :** `[BZ-POLISH-001]` shop micro-form · `[BZ-POLISH-003]` EmptyState · `[BZ-POLISH-004]` bandeaux vente cooldown (fade + pulse + locked lisible).

| # | ID | Job Bezy | Prefab / assets | Effort | Prérequis |
|---|-----|----------|-----------------|--------|-----------|
| ~~1~~ | ~~**[BZ-POLISH-001]**~~ | ~~Micro-fix shop~~ | — | — | **CLOS** |
| ~~P0~~ | ~~**[BZ-POLISH-002]**~~ | ~~HUD PA suite~~ | — | — | **Bezy CLOS** 2026-07-29 (hooks Cursor) |
| ~~3~~ | ~~**[BZ-POLISH-003]**~~ | ~~EmptyState graines anim~~ | — | — | **CLOS** |
| ~~4~~ | ~~**[BZ-POLISH-004]**~~ | ~~Bandeaux vente cooldown / locked~~ | — | — | **CLOS** (playtest OK) |
| ~~5~~ | ~~**[BZ-POLISH-005]**~~ | ~~ShopScreen polish~~ | — | — | **Bezy CLOS** 2026-07-29 + Cursor empty |
| ~~6~~ | ~~**[BZ-POLISH-006]**~~ | ~~NavigationHUD press tabs + layer 5~~ | — | — | **CLOS** playtest OK 2026-07-29 |
| ~~7~~ | ~~**[BZ-POLISH-007]**~~ | ~~Toast récolte~~ | — | — | **Bezy CLOS** 2026-07-29 + Cursor Show |
| ~~8~~ | ~~**[BZ-POLISH-008]**~~ | ~~Popup inventaire plein~~ | — | — | **Bezy CLOS** 2026-07-29 + Cursor Open/Close |
| ~~9~~ | ~~**[BZ-POLISH-009]**~~ | ~~Arbre talents Commerce contrastes~~ | — | — | **Bezy CLOS** 2026-07-30 — playtest auteur |
| ~~10~~ | ~~**[BZ-POLISH-010]**~~ | ~~Filigrane Commerce~~ | — | — | **Bezy CLOS** 2026-07-30 — playtest auteur |
| 11 | **[BZ-POLISH-011]** | LoadingScreen : layout polish barre + % (pas illustration finale) | `Bootstrap` LoadingCanvas | S | — |
| 12 | **[BZ-POLISH-012]** | HomeScene / hub : boutons + titres lisibilité mobile | scènes hub | M | — |
| 13 | **[BZ-POLISH-013]** | Audit layers UI global (scenes + prefabs UI) | multi | L | checklist `TODO_Bezi_audit_scene_ui_refactor` |
| 14 | **[BZ-POLISH-014]** | Insecte Flowering : prefab `Bee` + `InsectPath` nodes sur `LaitueObj` | Bee + LaitueObj | M | art `Bee_Fly` prêt — `PROMPTS_Bezi_insecte_flowering.md` |
| ~~15~~ | ~~**[BZ-POLISH-015]**~~ | ~~Wallet punch +1/−1~~ | — | — | **PARK UX** — Bezy OK, surface invisible au delta |
| 16 | **[BZ-POLISH-016]** / **[CT-FARM-POLISH-003]** | VFX particules plantation + récolte (burst circulaire) | prefab PS + sprites planting | M | art sheet prêt |
| ~~17~~ | ~~**[BZ-POLISH-017]**~~ | ~~HUD Vente fond opaque~~ | — | — | **CLOS Bezy** (playtest auteur) |
| 18 | **[BZ-POLISH-018]** | VFX pièces / billets au feedback vente (canal) | `SaleMoneyBurst` PS | M | art coin/billet (sinon Ph.1–2 placeholder) |
| 19 | **[BZ-POLISH-019]** / **[CT-FARM-POLISH-004]** | Sparkle idle récoltable (Mature + graines) | `HarvestReadyFx` + ancre `LaitueObj` | M | Default-Particle — `PROMPTS_Bezi_harvest_ready_vfx.md` |

---

## Détail rapide (quoi demander à Bezy)

### 1 — Shop micro-fix `[BZ-POLISH-001]` — CLOS
- Livré avec `[CT-SHOP-002]` (2026-07-23).

### 2 — HUD PA suite `[BZ-POLISH-002]` — **Bezy Ph.1–3 CLOS** (2026-07-29)
- Prefab : `Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab`
- **Refuse** : trigger `Refuse` + clip `RefuseShake` (Row pulse) — OK
- **Fill conso** : `SpendPulse` path `ProgressBar/BarFill` scale punch — OK
- **Tooltip** : layer 5 + CanvasGroup + FadeIn/Out 0.12 s — OK
- **Suite Cursor :** hooks `Refuse` + fade tooltip — **OK** (2026-07-29) ; playtest Batch E

### 3 — EmptyState pulse `[BZ-POLISH-003]` — CLOS
- Livré Ph.1–3 (2026-07-23).

### 4 — Bandeaux vente `[BZ-POLISH-004]` — CLOS (playtest OK 2026-07-23)
- Fade overlay cooldown + pulse timer + locked Bandoulière/Vélo lisible.

### 5 — Écran shop `[BZ-POLISH-005]` — **Bezy Ph.1–3 CLOS** (2026-07-29)
- Prefab : `Assets/Prefabs/Ui/ShopScreen.prefab`
- Layer 5 ; grille / close ; `EmptyCataloguePanel` + Cursor wire Show/Hide
- Prompts : `Notes/Ui/PROMPTS_Bezi_shop_screen_005.md`

### 6 — HUD navigation `[BZ-POLISH-006]` — **CLOS** (playtest OK 2026-07-29)
- Press soft : `NavTab.controller` + Transition Animation sur 4 tabs ; layer 5 ; hit layout OK
- Prompts : `Notes/Ui/PROMPTS_Bezi_nav_tabs_press.md`
- Prérequis Editor documenté (scène ouverte) — `Notes/Bezi/README_bezi.md`

### 7 — Toast feedback récolte `[BZ-POLISH-007]` — **Bezy Ph.1–3 CLOS** (2026-07-29)
- Prefab : `Assets/Prefabs/Ui/HarvestRewardFeedbackPopup.prefab`
- Layers 5 + lisibilité + scale punch `Show` (pas alpha) + Cursor hook OK
- Prompts : `Notes/Ui/PROMPTS_Bezi_harvest_reward_toast.md`

### 8 — Popup inventaire plein `[BZ-POLISH-008]` — **Bezy Ph.1–3 CLOS** (2026-07-29)
- Prefab partagé : `Assets/Prefabs/Ui/ResourceFeedbackPopup.prefab`
- Layers 5 + lisibilité + Open/Close soft + Cursor hooks OK
- Prompts : `Notes/Ui/PROMPTS_Bezi_resource_feedback_008.md`

### 9 — Talents contrastes `[BZ-POLISH-009]` — **Bezy Ph.1–3 CLOS** (2026-07-30)
- `TreeMountHost` fixe + wire `treeMountHost` ; contrastes nœuds/edges ; layers 5
- Prompts : `Notes/Ui/PROMPTS_Bezi_talent_contrasts_009.md`
- Suite : playtest auteur Inventaire → P1 Commerce

### 10 — Filigrane `[BZ-POLISH-010]` — **Bezy Ph.1–2 CLOS** (2026-07-30)
- Sprite : `CommerceFiligrane.png` + `FondPanel` opaque plein cadre ; Filigrane centré 960² PreserveAspect
- Ordre : Backdrop → Filigrane (α 0.12) → Track_* runtime
- Prompts : `Notes/Ui/PROMPTS_Bezi_talent_filigrane_010.md`
- Suite : playtest auteur Inventaire → P1 Commerce

### 11–12 — Bootstrap / Home
- Lisibilité uniquement ; art LoadingScreen = autre workflow

### 13 — Audit layers
- Passe Bezy ciblée par dossier prefab (pas rescans monolithe)

### 14 — Insecte Flowering `[BZ-POLISH-014]` — **Bezy P1–P3 CLOS** (2026-07-25)
- Art : `Bee_Fly.png` · Prefabs : `Bee` + `LaitueObj/InsectPath`
- Cursor scripts + hook Flowering OK
- Suite : playtest auteur `[P0-FARM-INSECT-PLAY-001]` — **OK** (2026-07-29)

### 15 — Wallet punch `[BZ-POLISH-015]` — **PARK UX** (2026-07-27)
- Bezy Ph.1–3 OK, hooks Cursor OK — **mais widgets hors moment de jeu** (inventaire sans gain ; shop wallet non perçu / popup ferme après débit).
- **Ne plus investir** Bezy/Cursor. Assets `WalletBalance*` / `WalletWidget*` + triggers `Gain`/`Spend` **conservés**.
- Reprise après GDD §5.8 / `Notes/GDD/NOTE_affichage_monnaie_hud.md` (HUD chip vs feedback local vs solde shop lisible).
- Vente : garder `SaleMoneyBurst` (déjà feedback local utile).

### 16 — VFX plantation / récolte `[BZ-POLISH-016]` / `[CT-FARM-POLISH-003]`
- **Art (déjà livré)** : `PlantationDirtParticules.png` (`_0`…`_10`) + `wurmParticleFarmPlantation.png` (`_0`)
- **Bezy** : prefab `PlantingDirtBurst` (DirtBurst + WormBurst), burst radial, Alpha Blended, ~0.6 s ; 1 prefab plant/arrachage/récolte
- **Cursor (après Bezy)** : `Play()` au plant + arrachage + récolte
- Prompts prêts : `Notes/Ui/PROMPTS_Bezi_planting_dirt_vfx.md` (phases 1→2→3)
- Hors scope Bezy : pas de régénération sprites

### 17 — Fond opaque HUD Vente `[BZ-POLISH-017]` — **Bezy CLOS** (2026-07-26)
- Root Image `(0.04, 0.04, 0.06) a=0.98` + `Body/ContentBackdrop` `(0.07, 0.07, 0.09) a=0.99`, layer 5, sibling 0
- **Note review :** sprites Image encore `null` — OK Editor souvent ; si Home encore visible en build → Cursor `HudModalBackdrop` (white sprite runtime)
- Suite : playtest auteur HUD → Vente

### 18 — VFX monnaie vente `[BZ-POLISH-018]`
- **Feel** : burst pièces + billets au feedback vente (canal Voisinage)
- **Bezy** : prefab `SaleMoneyBurst` Ph.1–2 OK ; **Ph.4** `MoneyBurstAnchor` sur `ShopItemPopup` **OK** (2026-07-29)
- **Cursor** : burst ancré bouton **Confirmer/Valider** (`ConfirmPurchaseButton`) — **playtest OK** (2026-07-29)
- **Art (auteur)** : `CoinParticle` + `BillParticle` — absent ; Ph.3 après import
- Prompts : `Notes/Ui/PROMPTS_Bezi_sale_money_vfx.md`

### 19 — Sparkle récoltable `[BZ-POLISH-019]` / `[CT-FARM-POLISH-004]`
- **Feel** : idle sparkle crème quand stade récoltable (Mature **ou** graines Seedling)
- **Bezy** : prefab `HarvestReadyFx` Ph.1–4 + ancre `LaitueObj` — **CLOS** (2026-07-29)
- **Texture** : `StarsParticle.png` recentré + `M_HarvestReadySparkle` (Cursor 2026-07-29)
- **Cursor après** : hook `PlantGrow` — **OK** (2026-07-29) `HarvestReadyFxAnchor` + sync récoltable
- Prompts : `Notes/Ui/PROMPTS_Bezi_harvest_ready_vfx.md`

---

## Anti-patterns (semaine)

- Ne pas demander à Bezy de **générer des images**  
- Un thread = un job ; phases 1→2→3  
- Ne pas fusionner audit layers + rebuild shop dans un seul prompt  
- Playtests / hooks vente / design PA = **Cursor ou batch**, pas Bezy
- Si **Add Component / wiring sur prefab disque échoue** (path bug) → workaround Bootstrap documenté dans `Notes/Bezi/README_bezi.md` (halo 2026-07-23) ; vérifier GUID inchangé après Apply

---

## Suivi

Cocher au fil de l’eau dans `Notes/Todo_project.md` (section *File Bezy polish semaine*) + journaliser en fin de session.
