# Todo playtest — batch (session QA dédiée)

Statut officiel des tâches : **`Notes/Todo_project.md`**.  
Cette note = **checklists playtest en batch** + **comment forcer PA / inventaire / vente à la mano**.

Règle : Bezy / code en priorité session ; playtests batch ici jusqu’à une session « QA ».

---

## Comment forcer l’état à la mano (cheatsheet)

> Play Mode depuis **Bootstrap**. Objets cibles vivent dans **`NavigationHUD`** (ne se décharge pas).

### Chemin saves (disque)

Company / Product Unity = `DefaultCompany` / `My project` :

`%USERPROFILE%\AppData\LocalLow\DefaultCompany\My project\`

| Fichier | Contenu utile |
|---------|----------------|
| `action_points.json` | `remainingPoints`, `lastResetUtcTicks` |
| `inventory.json` | slots items + flags pack départ |

Éditer les JSON **hors Play Mode** (ou quitter Play puis relancer) pour éviter un overwrite au quit.

### Points d’action (`ActionPointService`)

**Où :** Hierarchy Play Mode → scène `NavigationHUD` → objet avec `ActionPointService` (souvent près de `PlayerInventory`).

**Context Menu** (clic droit composant Inspector) :

| Menu | Effet |
|------|--------|
| `PA Debug/Set remaining = 0` | Budget restant **0** → tester refus plantation / récolte / vente |
| `PA Debug/Set remaining = 1` | Exactement **1** PA → 1 action puis refus |
| `PA Debug/Refill full budget` | Remplit au `dailyBudget` Inspector |
| `PA Debug/Delete action_points.json + refill` | Reset save PA + refill |

**Inspector utile (Play Mode OK) :**

- `dailyBudget` — plafond affiché (ex. 160 / 240)
- `plantSeedCost` / `harvestCost` / `sellCost` — mettre **0** pour actions gratuites (smoke sans vider le budget)
- Coûts > 0 + `remaining = 0` → refus attendu

**JSON manuel :** éditer `remainingPoints` dans `action_points.json`, puis relancer Play.

**HUD :** compteur = **consommés / max** (`ConsumedPoints / MaxDailyPoints`), pas le « restant ».  
Ex. budget 160, remaining 0 → HUD **160 / 160**.

### Inventaire (`PlayerInventory`)

**Où :** même `NavigationHUD` → `PlayerInventory`.

| Astuce | Comment |
|--------|---------|
| Reset profil inventaire | Context Menu **`Inventaire — Reset + supprimer inventory.json`** → recharge pack monnaie + graines départ |
| Vider un stack | Inventaire → détail item → Jeter (drop) |
| Remplir pour « inventaire plein » | Récolter / acheter jusqu’à `slotCount` (souvent 20) slots occupés, ou éditer `inventory.json` |
| Or / monnaie | Item `PrimaryCurrency` dans l’inventaire (shop / vente) ; reset inventaire recrédit le pack départ Inspector |
| Graines | Pack départ Inspector `startingSeedAmount` ; ou shop ; ou plantation puis reset |

### Vente / cooldown (`SaleChannelService`)

**Où :** `NavigationHUD` → `SaleChannelService` (souvent sur même GO que inventaire).

| Champ Debug | Effet |
|-------------|--------|
| `ignoreSaleCooldown` = ON | Ignore le cooldown 24 h |
| `neighborSaleCooldownSeconds` = `60` | Cooldown court pour tester overlay / timer |

### Ferme / insecte / VFX

| Besoin | Astuce |
|--------|--------|
| Forcer Flowering (insecte) | Sur la plante en Play : `PlantGrow` → Context Menu **`Debug/Force Flowering (insecte)`** |
| DirtBurst | Planter / arracher / récolter (hooks déjà branchés) |
| Croissance rapide | Réduire durées sur `PlantDefinition` (asset Laitue) **temporairement**, ou Force Flowering |

### Talents (si besoin batch)

`TalentProgressionService` → Context Menu `Talents/Add 1 skill point` / `Talents/Reset progression`.

---

## Batch A — Points d'action V0

**ID :** `[P0-AP-PLAY-001]`  
**Checklist détaillée :** `Notes/PLAYTEST_points_actions_v0.md`  
**Forcer PA :** Context Menu `PA Debug/*` ci-dessus.

- [ ] §1 HUD (visible, budget courant, refresh + pulse Spend)
- [ ] §2 Plantation nominale + PA insuffisants (`remaining = 0`)
- [ ] §3 Récolte nominale −1 PA
- [ ] §3 Inventaire plein → popup + PA remboursés, plante intacte
- [ ] §3 PA à 0 → récolte refusée, plante intacte
- [ ] §3 Arrachage sans coût PA
- [ ] §4 Persistance `action_points.json` + relance
- [ ] §5 Vente −1 PA (après hook vente `[P0-AP-CODE-002]`)

**Prérequis code avant §5 :** hook vente dans `SaleChannelService`.

---

## Batch B — Canaux de vente (cooldown)

**ID :** `[P0-SALE-PLAY-004]`

- [ ] Récolter laitue → vendre Voisinage
- [ ] Bandeau grisé + overlay cooldown + timer qui descend
- [ ] Popup refusée si encore en cooldown
- [ ] Option test rapide : `neighborSaleCooldownSeconds = 60` ou `ignoreSaleCooldown` sur `SaleChannelService`

**Réfs :** `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md` §7

---

## Batch C — Ferme graines (post EmptyState Bezy)

**IDs liés :** `[P0-FARM-PLAY-001]`, `[P0-FARM-BUG-001]` (à revalider après `[CT-FARM-UI-001]`)

- [ ] Sac vide → EmptyStatePanel + CTA Acheter (shop)  
  *(vider graines : jeter stacks, ou reset inventaire puis consommer le pack)*
- [ ] Après achat shop → plus de message empty + slot simultanés
- [ ] Plantation consomme stock ; 0 stock → empty state

**Réfs :** `Notes/Farm/REFACTOR_graines_plantation_inventaire.md` §4.4 / §9

---

## Batch D — Polish shop `[CT-SHOP-002]` (contrôle post-Bezy)

**Checklist détaillée :** `Notes/PLAYTEST_shop_polish_ct002.md`

- [ ] §0 Éditeur (layers 5, animator/canvasGroup câblés, Root/ConfirmOverlay inactifs)
- [ ] §1 **Bootstrap sans UI parasite** (LoadingScreen seul pendant load)
- [ ] §2 Open / Close transitions (slide+scale, carte opaque)
- [ ] §3 Quantité + ConfirmOverlay (annuler / confirmer)
- [ ] §4 Smoke navigation HUD (pas de popup orpheline)

**Priorité auteur :** §1 Bootstrap si UI encore visible au boot.

---

## Batch E — HUD PA polish `[BZ-POLISH-002]` (après Bezy Ph.1–3)

> À faire **après** livraison Bezy Refuse / Fill / Tooltip fade + hooks Cursor éventuels.

- [ ] Trigger **Refuse** : action refusée faute de PA → shake Row (pas de conso)
- [ ] **Fill** : à chaque dépense PA, punch scale `BarFill` en plus du SpendPulse Row
- [ ] **Tooltip** zones fatigue : fade in/out ~0.12 s, layer 5, textes Comfort / Caution / Fatigue OK
- [ ] Régression : SpendPulse + HUD compteur toujours OK

---

## Batch F — Clos hors batch (session 2026-07-29) — ne pas retester sauf régression

- [x] `[P0-INV-DROP-PLAY-001]` Inventaire drop / compost
- [x] `[P0-FARM-INSECT-PLAY-001]` Insecte Flowering
- [x] `[P0-FARM-VFX-PLAY-002]` DirtBurst plant / arrachage / récolte

---

## Ordre suggéré session batch QA

1. **Cheatsheet** : localiser `ActionPointService` + `PlayerInventory` + `SaleChannelService` en Play  
2. **Batch A** (§1–4) — PA  
3. **Batch C** — graines empty  
4. **Batch B** — cooldown vente (cooldown court)  
5. **Batch D** — shop / Bootstrap  
6. **Batch E** — dès Bezy HUD PA OK  
7. Batch A **§5** — quand `[P0-AP-CODE-002]` livré  

---

## Comment clôturer un batch

1. Cocher ici au fil du playtest.  
2. Reporter le statut `[x]` dans `Notes/Todo_project.md` pour l’ID concerné.  
3. Tracer dans `PROJECT_LOG.md` si fin de session.
