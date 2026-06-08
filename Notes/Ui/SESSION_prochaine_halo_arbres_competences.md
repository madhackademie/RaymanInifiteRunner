# Session prochaine — halo inventaire → arbres de compétences

**Statut :** plan d’exécution pour la **prochaine session** (après playtest **[P0-INV-HALO-004]**).  
**Date :** 2026-06-05  
**Branche :** `feature/inventory-halo-ui`  
**Contexte :** coque UI prête (Phase 3 wiring OK) — il reste à **lier chaque `PlayerHaloSlotUI` à un arbre de compétences réel**.

Docs liés :
- `Notes/Ui/SPEC_rework_inventaire_halo_progression.md`
- `Notes/Ui/ARBRE_inventory_halo_ui.md`
- `Notes/GDD/INBOX_notes_tablette_recherches.md` (notes perso)
- Scripts : `Assets/Scripts/UI/Inventory/Progression/`

---

## Décision architecture (rappel — déjà actée)

| Option | Choix |
|--------|--------|
| Scène Unity dédiée | **Non** — inventaire reste monté |
| `ScreenId` dédié (écran HUD séparé) | **Non** — overlay sur `InventoryScreen` |
| Onglets = une compétence chacun | **Non** — les **slots halo** sélectionnent la piste ; l’overlay affiche **un** arbre à la fois |

Flux existant (ne pas recréer) :

```
PlayerHaloSlotUI (trackId)
  → PlayerHaloPanelController.OnTrackSelected
    → InventoryScreenController
      → TalentTreeOverlayController.Open(trackId)
```

**Distinction importante :** la progression **joueur** (halo inventaire) ≠ progression **système aquaponique par niveau** (`Notes/GDD/SPEC_progression_systeme_aquaponique_par_niveau.md` — panneau in-scène `FirstLvl`, onglets Biofiltre / Poisson / Techno). Deux systèmes, deux UIs, synergies possibles plus tard.

---

## Notes manuscrites / perso — synthèse liée à cette tâche

> Contenu reconstitué depuis : notes tablette (partiel), `Inbox_gdd.md`, specs halo, échanges session 2026-06-04.  
> **À compléter** lors de [P0-IDEA-001] si la tablette apporte plus de détail (coûts, noms de nœuds, effets chiffrés).

### Vision générale (notes perso + spec halo)

- La zone haute inventaire = **portrait + niveau joueur** au centre, **recherches / boosts** en orbite (réf. `InventoryStats.png`, `InventorySplitStatsCompetances.png`).
- Chaque icône halo = une **piste de compétence** ; clic → **arbre** dédié (overlay, bouton Retour).
- **Déblocage par points** : on dépense des points gagnés pour acheter des nœuds (source XP exacte : à préciser — récoltes, quêtes, niveau joueur…).
- **Choix libre** : le joueur **choisit quel arbre pousser en premier** — pas de chemin linéaire imposé.
- **Équilibrage long terme** : chaque path doit rester viable (ex. *plus de salade plus vite* vs *vendre 3× plus cher* — à valider par chiffres, pas bloquant pour le prototype UI).

### Pistes de compétences — 8 slots (import tablette 2026-06-08)

| # slot halo (ordre 12 h) | ID | Piste | Contenu notes perso / spec |
|--------------------------|----|-------|----------------------------|
| 1 | `track.marketing` | **Marketing** | Vente, market, marges — **premier arbre prototype recommandé**. |
| 2 | `track.insect.feed` | **Nourriture & élevage insectes** | Alimentation et élevage insectes — détail nœuds **TBD**. |
| 3 | `track.bioconversion` | **Bioconversion** | Chaîne biofiltre / transformation — détail nœuds **TBD**. |
| 4 | `track.fish.reproduction` | **Reproduction poisson** | Cycle reproductif, bonus indirect plantes via eau / nutriments. |
| 5 | `track.water` | **Eau** | Qualité et équilibre hydrique — détail nœuds **TBD**. |
| 6 | `track.gardening` | **Jardinage plantes & graines** | 6 stades plantes (`Inbox_gdd.md`) : vitesse, rendement, germination. |
| 7 | `track.diy` | **DIY** | Acronyme notes tablette — **à préciser**. |
| 8 | `track.shop` | **Magasin** | Achats shop, remises, offres — lien direct shop existant. |

> Ancien plan « Commerce » scindé : **Marketing** (vendeur) + **Magasin** (acheteur).

### Notes perso hors halo (contexte — ne pas mélanger dans cette session)

- **Workflow auteur** (`Inbox_gdd.md`) : mini-Trello tâches 15 min/jour ; veille Farm City ; IA casual farm — **hors scope** implémentation arbres.
- **Progression ferme par niveau** (note session 2026-06-04) : bouton in-scène `FirstLvl`, onglets Biofiltre / Poisson / Techno, points **système** — document séparé `SPEC_progression_systeme_aquaponique_par_niveau.md`, **pas** cette session.

### Encore à importer depuis tablette ([P0-IDEA-001])

- [ ] Liste exacte des nœuds Commerce (noms, coûts, effets %).
- [ ] Détail branches Culture plante / Poisson.
- [ ] Source et courbe des **points de compétence** joueur.
- [x] Nombre final de slots visibles au lancement → **8** (2026-06-08).
- [ ] Signification **DIY** (slot 7).

---

## Les 3 étapes — prochaine session

Ordre recommandé **après** validation playtest **[P0-INV-HALO-004]**.

---

### Étape 1 — Renommer `ProgressionTrackId` + aligner les 8 slots ✅ (2026-06-08)

**Objectif :** chaque `PlayerHaloSlotUI` expose un `trackId` stable et lisible ; prefab et `HaloSlotOrder` alignés.

**Fichiers mis à jour :**
- `ProgressionTrackId.cs` — constantes + `GetShortLabel` / `GetDisplayName`
- `PlayerHaloPanelController.cs` — libellés runtime
- `TalentTreeOverlayController.cs` — titre overlay
- `PlayerHaloPanel.prefab` — `trackId` par slot

**IDs intégrés :** `track.marketing`, `track.insect.feed`, `track.bioconversion`, `track.fish.reproduction`, `track.water`, `track.gardening`, `track.diy`, `track.shop`.

**Checklist :**
- [x] Constantes + `HaloSlotOrder` mis à jour (Cursor).
- [x] Labels courts slot via `Configure` runtime.
- [ ] Playtest : clic chaque slot → overlay titre = nom lisible.
- [ ] Signification **DIY** confirmée par auteur.

**Livrable :** playtest overlay avant merge.

---

### Étape 2 — Esquisser le modèle data + premier arbre mock — **GELÉ** (2026-06-08)

> **Blocage :** architecture gameplay des arbres non définie (liens transverses bioconversion ↔ insectes ↔ DIY / biogaz ; arbre global vs joueur + système aquaponique). Voir `INBOX_notes_tablette_recherches.md` § gel design. **Ne pas implémenter** tant que l’auteur n’a pas tranché.

**Objectif (quand débloqué) :** remplacer le placeholder texte de `TalentTreeOverlayController` par un chargement data-driven ; vertical slice sur une piste pilote (ex. Marketing).

**Nouveaux fichiers suggérés (Cursor) :**

```
Assets/Scripts/Progression/
  TalentTrackDefinition.cs      // ScriptableObject — 1 piste
  TalentNodeDefinition.cs       // ScriptableObject — 1 nœud
  TalentBranchId.cs             // constantes : buyer, seller, root
  PlayerTalentProgressState.cs  // runtime + future save
  TalentProgressionService.cs   // GetTrack, TryPurchaseNode, GetModifiers (stub)
```

**Assets designers (à créer en session) :**

```
Assets/Data/Progression/Tracks/
  Track_Marketing.asset
Assets/Data/Progression/Nodes/Marketing/
  Node_Marketing_Root.asset
  Node_Seller_PriceUp01.asset
  Node_Seller_VolumeBonus.asset
  …
```

**Arbre mock Marketing** (effets **placeholder** — Magasin = piste séparée slot 8) :

```
              [ Racine Marketing ]
                      │
                      ▼
              Branche VENDEUR
              - Prix vente +5 %
              - Bonus volume market
              (coûts : 1, 2, 3 pts…)
```

**Modifs scripts existants :**
- `TalentTreeOverlayController` : injecter `TalentProgressionService` ou `TalentTrackDefinition[]` ; `Open(trackId)` instancie les nœuds UI.
- Nouveau `TalentNodeUI.cs` : affiche icône, titre, état (locked / available / purchased), clic → `TryPurchaseNode`.

**Checklist :**
- [ ] SO + nœuds Marketing créés (même valeurs mock).
- [ ] Overlay affiche l’arbre Commerce quand slot 1 cliqué.
- [ ] Autres pistes : message « À venir » ou arbre vide propre (pas de crash).
- [ ] Service retourne modificateurs mock pour branche Acheteur/Vendeur (log debug OK).

**Hors scope étape 2 :** save JSON, application réelle prix shop/market (étape ultérieure **[CT-INV-HALO-005]**).

---

### Étape 3 — Prompt Bezy : zone arbre dans l’overlay

**Objectif :** remplacer `bodyPlaceholderLabel` par une hiérarchie scrollable pour les nœuds ; prefab nœud réutilisable.

**Fichier cible :** `Assets/Prefabs/Ui/InventoryScreen.prefab` (enfant `TalentTreeOverlay`).

**Hiérarchie cible :**

```
TalentTreeOverlay [TalentTreeOverlayController]
├── OverlayDimmer
└── OverlayPanel
    ├── Header (trackTitleLabel + backButton)
    └── TreeScrollView
        └── TreeContent          ← conteneur layout nœuds
            └── (instances TalentNodeUI)
```

**Prefab à créer :** `Assets/Prefabs/Ui/Progression/TalentNodeUI.prefab`  
Composants : `Button`, `Image` icône, TMP titre, états visuels locked/purchased, script `TalentNodeUI`.

**Prompt Bezy — Phase 4 (shell arbre, ≤ 3500 car.) :**

```
Tâche Phase 4 uniquement — NE PAS rescanner tout le projet.
Cible : Assets/Prefabs/Ui/InventoryScreen.prefab, enfant TalentTreeOverlay.

1) Sous OverlayPanel : ajouter TreeScrollView (ScrollRect vertical) + TreeContent (VerticalLayoutGroup ou conteneur libre pour nœuds futurs).
2) Garder trackTitleLabel, backButton, CanvasGroup, Animator existants sur TalentTreeOverlay.
3) Créer Assets/Prefabs/Ui/Progression/TalentNodeUI.prefab : Button + Icon + Title (TMP) + cadres états Locked/Purchased (GameObjects toggle). Script TalentNodeUI.cs déjà fourni par Cursor — ajouter le composant, laisser refs vides si besoin.
4) NE PAS câbler TalentTreeOverlayController vers TreeContent cette phase — wiring Phase 5.
5) Désactiver ou retirer bodyPlaceholderLabel du layout visible (peut rester en backup inactif).

Scripts existants : TalentTreeOverlayController.cs, TalentNodeUI.cs (Cursor).
Réutiliser styles couleurs InventoryScreen / halo parchemin.
Confirmer fichiers modifiés. Attendre validation avant Phase 5 wiring.
```

**Phase 5 Bezy (session suivante) :** binder `TreeContent`, `trackTitleLabel`, `backButton`, références sur `TalentTreeOverlayController` + test instanciation 1 `TalentNodeUI` en scène éditeur.

**Checklist :**
- [ ] Scroll + TreeContent présents dans prefab.
- [ ] `TalentNodeUI.prefab` créé.
- [ ] Retour overlay + fade inchangés (régression playtest).

---

## Ordre session recommandé (résumé)

| # | ID tâche | Qui | Dépend de |
|---|----------|-----|-----------|
| 0 | **[P0-INV-HALO-004]** playtest coque actuelle | Auteur | — |
| 1 | **[P0-INV-HALO-006]** Étape 1 — `ProgressionTrackId` | Cursor + Bezy labels | playtest OK |
| 2 | **[P0-INV-HALO-007]** Étape 2 — SO + arbre Marketing mock | Cursor | étape 1 |
| 3 | **[P0-INV-HALO-008]** Étape 3 — Bezy overlay arbre | Bezy puis Cursor review | étape 2 (SO + `TalentNodeUI.cs`) |
| — | **[P0-IDEA-001]** compléter notes tablette | Auteur | en parallèle si possible |

---

## Critères de fin de session

- [x] 8 pistes nommées ; overlay affiche le nom lisible.
- [ ] Arbre **Marketing** visible (nœuds mock).
- [ ] Prefab overlay prêt pour instanciation dynamique des nœuds (Phase 5).
- [ ] Notes tablette : section Commerce complétée dans `INBOX_notes_tablette_recherches.md` si nouveaux détails apportés.
- [ ] Trace dans `PROJECT_LOG.md` + statuts `Notes/Todo_project.md`.

---

## Questions à trancher en session (5 min)

1. ~~**6 ou 8** slots actifs au lancement ?~~ → **8** (2026-06-08).
2. ~~**DIY** (slot 7) ?~~ → **DIY** confirmé (faute « Dis ») — `track.diy`.
3. **Architecture** : arbre global vs **joueur** (halo) + **système aquaponique** (ferme) vs hybride (nœuds pont) ?
4. **Carte liens** : tabletop / Miro avant nœuds chiffrés ?
5. Overlay **demi-écran** (actuel) ou **quasi plein écran** pour l’arbre ?
6. Points compétence : **niveau joueur** uniquement ou aussi **actions ferme** ?

---

## Références code actuel

| Fichier | Rôle |
|---------|------|
| `PlayerHaloSlotUI.cs` | `trackId`, clic → `OnClicked` |
| `PlayerHaloPanelController.cs` | `OnTrackSelected`, mock 8 slots |
| `InventoryScreenController.cs` | Dim grille + ouvre overlay |
| `TalentTreeOverlayController.cs` | `Open(trackId)` — à enrichir étape 2 |
| `ProgressionTrackId.cs` | 8 IDs compétences intégrés (2026-06-08) |
