# Todo playtest — batch (pas prioritaire session)

Statut officiel des tâches : **`Notes/Todo_project.md`**.  
Cette note regroupe les **checklists playtest à faire en batch**, hors priorité Bezy / code.

Règle session (2026-07-23) : **Bezy en priorité** ; playtests reportés ici jusqu’à une session « batch QA ».

---

## Batch A — Points d'action V0

**ID :** `[P0-AP-PLAY-001]`  
**Branche :** `feature/points-actions`  
**Checklist détaillée :** `Notes/PLAYTEST_points_actions_v0.md`

- [ ] §1 HUD (visible, budget courant, refresh + pulse Spend)
- [ ] §2 Plantation nominale + PA insuffisants
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
**Checklist courte :**

- [ ] Récolter laitue → vendre Voisinage
- [ ] Bandeau grisé + overlay cooldown + timer qui descend
- [ ] Popup refusée si encore en cooldown
- [ ] Option test rapide : `neighborSaleCooldownSeconds = 60` ou `ignoreSaleCooldown` sur `SaleChannelService`

**Réfs :** `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md` §7

---

## Batch C — Ferme graines (post EmptyState Bezy)

**IDs liés :** `[P0-FARM-PLAY-001]`, `[P0-FARM-BUG-001]` (à revalider après `[CT-FARM-UI-001]`)

- [ ] Sac vide → EmptyStatePanel + CTA Acheter (shop)
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

## Comment clôturer un batch

1. Cocher ici au fil du playtest.
2. Reporter le statut `[x]` dans `Notes/Todo_project.md` pour l’ID concerné.
3. Tracer dans `PROJECT_LOG.md` si fin de session.
