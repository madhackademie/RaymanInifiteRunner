# File Bezy — polish semaine (overload crédits)

**Objectif :** liste de jobs **Bezy-only** (prefabs / Animator / layers / lisibilité / micro-anims).  
**Hors scope Bezy :** génération d’images/sprites, logique C# métier (Cursor).  
**Playtests :** batch `Notes/Todo_playtest.md` (pas prioritaire tant que file Bezy).

Statuts officiels : `Notes/Todo_project.md`.  
Prompts : créer/étendre `Notes/Ui/PROMPTS_Bezi_*.md` **avant** d’envoyer (phases 1→2→3, &lt; 3500 car.).

---

## Ordre suggéré (semaine)

> **Priorité prochaine session :** **[BZ-POLISH-002]** HUD PA suite (Refuse shake + fill conso + tooltip fade).  
> **Clos 2026-07-23 :** `[BZ-POLISH-001]` shop micro-fix · `[BZ-POLISH-003]` EmptyState · `[BZ-POLISH-004]` bandeaux vente cooldown (fade + pulse + locked lisible).

| # | ID | Job Bezy | Prefab / assets | Effort | Prérequis |
|---|-----|----------|-----------------|--------|-----------|
| ~~1~~ | ~~**[BZ-POLISH-001]**~~ | ~~Micro-fix shop~~ | — | — | **CLOS** |
| **P0** | **[BZ-POLISH-002]** | **HUD PA suite** : Refuse shake, fill conso, tooltip fade | `ActionPointsHudWidget` + `FatigueTooltipPanel` | M | HUD PA base OK |
| ~~3~~ | ~~**[BZ-POLISH-003]**~~ | ~~EmptyState graines anim~~ | — | — | **CLOS** |
| ~~4~~ | ~~**[BZ-POLISH-004]**~~ | ~~Bandeaux vente cooldown / locked~~ | — | — | **CLOS** (playtest OK) |
| 5 | **[BZ-POLISH-005]** | `RuntimeShopScreen` : layers 5 + contraste grille slots + empty catalogue | prefab shop screen | M | — |
| 6 | **[BZ-POLISH-006]** | NavigationHUD : hit areas onglets ≥ 44, contrastes, layer audit | `NavigationHUD` | M | — |
| 7 | **[BZ-POLISH-007]** | Toast / feedback récolte : polish entrée-sortie (scale+fade) | `FarmHarvestReward` / feedback popup | S | déjà en main |
| 8 | **[BZ-POLISH-008]** | Popup inventaire plein (récolte) : lisibilité + Open/Close soft | popup pipeline concernée | S | — |
| 9 | **[BZ-POLISH-009]** | Arbre talents Commerce : contrastes nœuds + edges + `TreeMountHost` | `Track_Commerce` / overlay | M | `[BL-INV-TALENT-003]` |
| 10 | **[BZ-POLISH-010]** | Filigrane placeholder piste Commerce (couleur/motif UI, **pas** art final) | sous `TreeMountHost` | M | placeholders OK |
| 11 | **[BZ-POLISH-011]** | LoadingScreen : layout polish barre + % (pas illustration finale) | `Bootstrap` LoadingCanvas | S | — |
| 12 | **[BZ-POLISH-012]** | HomeScene / hub : boutons + titres lisibilité mobile | scènes hub | M | — |
| 13 | **[BZ-POLISH-013]** | Audit layers UI global (scenes + prefabs UI) | multi | L | checklist `TODO_Bezi_audit_scene_ui_refactor` |
| 14 | **[BZ-POLISH-014]** | Insecte Flowering : prefab `Bee` + `InsectPath` nodes sur `LaitueObj` | Bee + LaitueObj | M | art `Bee_Fly` prêt — `PROMPTS_Bezi_insecte_flowering.md` |
| 15 | **[BZ-POLISH-015]** | Wallet / CurrencyBalanceUI : polish chiffres + punch +1/−1 | widgets wallet | S | — |
| 16 | **[BZ-POLISH-016]** / **[CT-FARM-POLISH-003]** | VFX particules plantation + récolte (burst circulaire) | prefab PS + sprites planting | M | art sheet prêt |
| ~~17~~ | ~~**[BZ-POLISH-017]**~~ | ~~HUD Vente fond opaque~~ | — | — | **CLOS Bezy** (playtest auteur) |
| 18 | **[BZ-POLISH-018]** | VFX pièces / billets au feedback vente (canal) | `SaleMoneyBurst` PS | M | art coin/billet (sinon Ph.1–2 placeholder) |

---

## Détail rapide (quoi demander à Bezy)

### 1 — Shop micro-fix `[BZ-POLISH-001]` — CLOS
- Livré avec `[CT-SHOP-002]` (2026-07-23).

### 2 — HUD PA suite `[BZ-POLISH-002]` — **PRIORITÉ prochaine session**
- Prefab : `Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab`
- **Refuse** : pulse / shake léger du `Row` (ou icône) quand action refusée faute de PA
- **Fill conso** : micro-anim sur la barre / overlay consommé à chaque dépense (complément du SpendPulse Row déjà livré Ph.5)
- **Tooltip** : `FatigueTooltipPanel` layer 5 + CanvasGroup **fade in/out** (~0.12 s) — structure Ph.4 déjà en place
- Cursor : hooks trigger `Refuse` / fade tooltip si besoin après Bezy

### 3 — EmptyState pulse `[BZ-POLISH-003]` — CLOS
- Livré Ph.1–3 (2026-07-23).

### 4 — Bandeaux vente `[BZ-POLISH-004]` — CLOS (playtest OK 2026-07-23)
- Fade overlay cooldown + pulse timer + locked Bandoulière/Vélo lisible.

### 5 — Écran shop `[BZ-POLISH-005]`
- Layer 5 partout  
- Slots plus lisibles ; état vide catalogue (panel placeholder)

### 6 — HUD navigation `[BZ-POLISH-006]`
- Onglets Ferme / Shop / Inventaire / Vente : taille tactile, couleurs distinctes  
- Audit layer 5

### 7–8 — Feedbacks gameplay
- Entrée/sortie soft sans casser `CanvasGroup` métier  
- Textes ≥ 22 mobile

### 9–10 — Talents
- Contrastes Locked/Available/Owned  
- Filigrane = Image plein fond alpha faible (couleur), **pas** sprite final

### 11–12 — Bootstrap / Home
- Lisibilité uniquement ; art LoadingScreen = autre workflow

### 13 — Audit layers
- Passe Bezy ciblée par dossier prefab (pas rescans monolithe)

### 14 — Insecte Flowering `[BZ-POLISH-014]` — **Bezy P1–P3 CLOS** (2026-07-25)
- Art : `Bee_Fly.png` · Prefabs : `Bee` + `LaitueObj/InsectPath`
- Cursor scripts + hook Flowering OK
- Suite : playtest auteur `[P0-FARM-INSECT-PLAY-001]`

### 15 — Wallet punch
- Même pattern que PA `Spend` (trigger + clip court)

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
- **Bezy** : prefab `Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab` (`CoinBurst` + `BillBurst`), Play On Awake OFF
- **Art (auteur)** : `Assets/Art/Sprites/VFX/Sale/CoinParticle.png` + `BillParticle.png` — **absent du repo** ; Ph.1–2 OK en couleur placeholder ; Ph.3 après import
- **Cursor (après Bezy)** : `Play()` après vente réussie (recommandé) près du bandeau — pas au scope Bezy
- Prompts prêts : `Notes/Ui/PROMPTS_Bezi_sale_money_vfx.md` (phases 1→2→3)

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
