# File Bezy — polish semaine (overload crédits)

**Objectif :** liste de jobs **Bezy-only** (prefabs / Animator / layers / lisibilité / micro-anims).  
**Hors scope Bezy :** génération d’images/sprites, logique C# métier (Cursor).  
**Playtests :** batch `Notes/Todo_playtest.md` (pas prioritaire tant que file Bezy).

Statuts officiels : `Notes/Todo_project.md`.  
Prompts : créer/étendre `Notes/Ui/PROMPTS_Bezi_*.md` **avant** d’envoyer (phases 1→2→3, &lt; 3500 car.).

---

## Ordre suggéré (semaine)

| # | ID | Job Bezy | Prefab / assets | Effort | Prérequis |
|---|-----|----------|-----------------|--------|-----------|
| 1 | **[BZ-POLISH-001]** | Micro-fix shop : `QuantityText` ≥ 22 + câbler `backdropImage` | `ShopItemPopup.prefab` | XS | Post `[CT-SHOP-002]` |
| 2 | **[BZ-POLISH-002]** | Tooltip PA : layer UI 5 + fade léger panel | `ActionPointsHudWidget` + `FatigueTooltipPanel` | S | HUD PA OK |
| 3 | **[BZ-POLISH-003]** | EmptyState graines : polish anim apparition (CanvasGroup/scale) | `SeedSelectionUI` | S | `[CT-FARM-UI-001]` OK |
| 4 | **[BZ-POLISH-004]** | Bandeaux vente : pulse locked / fade cooldown | `SaleChannelBandeauView` | M | Vente V0 OK |
| 5 | **[BZ-POLISH-005]** | `RuntimeShopScreen` : layers 5 + contraste grille slots + empty catalogue | prefab shop screen | M | — |
| 6 | **[BZ-POLISH-006]** | NavigationHUD : hit areas onglets ≥ 44, contrastes, layer audit | `NavigationHUD` | M | — |
| 7 | **[BZ-POLISH-007]** | Toast / feedback récolte : polish entrée-sortie (scale+fade) | `FarmHarvestReward` / feedback popup | S | déjà en main |
| 8 | **[BZ-POLISH-008]** | Popup inventaire plein (récolte) : lisibilité + Open/Close soft | popup pipeline concernée | S | — |
| 9 | **[BZ-POLISH-009]** | Arbre talents Commerce : contrastes nœuds + edges + `TreeMountHost` | `Track_Commerce` / overlay | M | `[BL-INV-TALENT-003]` |
| 10 | **[BZ-POLISH-010]** | Filigrane placeholder piste Commerce (couleur/motif UI, **pas** art final) | sous `TreeMountHost` | M | placeholders OK |
| 11 | **[BZ-POLISH-011]** | LoadingScreen : layout polish barre + % (pas illustration finale) | `Bootstrap` LoadingCanvas | S | — |
| 12 | **[BZ-POLISH-012]** | HomeScene / hub : boutons + titres lisibilité mobile | scènes hub | M | — |
| 13 | **[BZ-POLISH-013]** | Audit layers UI global (scenes + prefabs UI) | multi | L | checklist `TODO_Bezi_audit_scene_ui_refactor` |
| 14 | **[BZ-POLISH-014]** | Prefab plante : shell overlay insecte **placeholder** (Image + Animator idle) | prefab plante | M | art sheet **plus tard** `[CT-FARM-POLISH-002]` |
| 15 | **[BZ-POLISH-015]** | Wallet / CurrencyBalanceUI : polish chiffres + punch +1/−1 | widgets wallet | S | — |
| 16 | **[BZ-POLISH-016]** / **[CT-FARM-POLISH-003]** | VFX particules plantation + récolte (burst circulaire) | prefab PS + sprites planting | M | art sheet prêt |

---

## Détail rapide (quoi demander à Bezy)

### 1 — Shop micro-fix `[BZ-POLISH-001]`
- `QuantityText` fontSize 22–26  
- `backdropImage` → Image Backdrop  
- Pas de rebuild

### 2 — Tooltip PA `[BZ-POLISH-002]`
- `FatigueTooltipPanel` + enfants → layer 5  
- Option : CanvasGroup fade 0→1 en 0.12 s à l’apparition (si script le permet ; sinon scale only)

### 3 — EmptyState pulse `[BZ-POLISH-003]`
- Animator Idle / Show sur `EmptyStatePanel` (scale 0.96→1)  
- Trigger ou bool ; wiring SerializeField si Cursor ajoute champ (sinon anim Always Animate à l’activation)

### 4 — Bandeaux vente `[BZ-POLISH-004]`
- Locked : léger pulse cadenas ou teinte  
- Cooldown : fade-in overlay (pas toucher logique timer)

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

### 14 — Insecte shell
- Enfant `InsectOverlay` inactif + Animator Idle placeholder (scale breathe)  
- Sprite final = session art séparée

### 15 — Wallet punch
- Même pattern que PA `Spend` (trigger + clip court)

### 16 — VFX plantation / récolte `[BZ-POLISH-016]` / `[CT-FARM-POLISH-003]`
- **Art (déjà livré)** : `Assets/Art/Sprites/VFX/Planting/PlantationDirtParticules.png` — Sprite Multiple 3×3 (`dirt_01..03`, `pebble_01..03`, `leaf_01..03`)
- **Bezy** : prefab Particle System (burst radial / cercle), Texture Sheet Animation ou liste sprites Random, material Alpha Blended ; durée courte (~0.4–0.8 s) ; un prefab réutilisable plant + harvest (ou 2 variants size)
- **Cursor (après Bezy)** : `Play()` / `Emit` au plant seed + à la récolte
- Prompts : créer `Notes/Ui/PROMPTS_Bezi_planting_dirt_vfx.md` (phases 1→2→3) avant envoi
- Hors scope Bezy : pas de régénération sprites

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
