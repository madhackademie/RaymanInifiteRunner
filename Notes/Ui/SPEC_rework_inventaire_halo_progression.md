# Spéc — rework inventaire : halo joueur + grille items

Vision **brute → polish** pour scinder l’écran inventaire en **deux zones**, sur le modèle de la référence mobile (portrait central + slots périphériques + grille basse).

Statut : **coque UI Phase 1 en cours** (scripts + builder éditeur placeholders). Gameplay talents / save : après notes tablette.  
Références visuelles :
- `Assets/Art/Models/ImageRef/UI/InventoryStats.png` (halo seul)
- `Assets/Art/Models/ImageRef/UI/InventorySplitStatsCompetances.png` (split complet ; **zone footer A hors scope**)
- Arbre technique : `Notes/Ui/ARBRE_inventory_halo_ui.md`

---

## 1. Intention produit

| Aujourd’hui | Cible |
|-------------|-------|
| Écran inventaire ≈ **grille d’items** (+ wallet) | Écran **double fonction** : identité / progression joueur **en haut**, stock **en bas** |
| Progression joueur peu visible dans l’inventaire | **Niveau**, portrait et **pistes de recherche / boost** « en gravité » autour du joueur |
| Shop / market séparés | Les talents **commerce** modulent achat et vente ; l’inventaire devient aussi un **hub de progression** |

L’inventaire reste la **source de vérité du stock** (`PlayerInventory`). La partie haute ne remplace pas l’équipement objet-à-objet du screenshot : elle **réutilise la même grammaire visuelle** (slots autour d’un centre) pour des **recherches et boosts**.

---

## 2. Layout cible (2 zones)

```
┌─────────────────────────────────────────────────────────┐
│  ZONE HAUTE — « Halo joueur » (fond clair / parchemin)   │
│                                                          │
│     [Recherche A]              [Recherche B]             │
│          [Boost 1]    ┌──────────┐    [Boost 2]          │
│                       │ Portrait │                       │
│     [Recherche C]     │ Niv. 58  │     [Recherche D]     │
│          [Boost 3]    └──────────┘    [Boost 4]          │
│                                                          │
│     (slots circulaires / carrés — clic → arbre talents)  │
├─────────────────────────────────────────────────────────┤
│  BARRE FILTRES (onglets All / Équip / Conso / Matériaux) │
│  + action « Sélection multiple » (optionnel, phase 2)    │
├─────────────────────────────────────────────────────────┤
│  ZONE BASSE — Grille inventaire (existant)               │
│  [slot][slot][slot][slot][slot]                          │
│  [slot][slot][slot][slot][slot]                          │
│  … scroll …                                              │
└─────────────────────────────────────────────────────────┘
```

### Zone haute — Halo joueur

- **Centre** : portrait (ou avatar placeholder), **niveau joueur**, éventuellement barre XP (phase polish).
- **Orbite** : 6–10 emplacements pour **icônes de recherche / technologie / boost** (remplace les slots d’équipement de la ref.).
- Chaque slot affiche au minimum :
  - icône de la piste ;
  - niveau ou badge de progression (règles à préciser via notes perso — cf. §4) ;
  - état verrouillé (cadenas) si non débloqué.
- **Gravité** : disposition symétrique autour du portrait (comme la ref.), pas une liste verticale.

### Zone basse — Inventaire items

- Conserver le comportement actuel : `InventoryUI` + `InventorySlotUI`, filtres, scroll, wallet si présent.
- Séparation nette visuelle (bandeau sombre + onglets comme sur la ref.).

---

## 3. Interaction — clic sur une recherche

**Action** : clic sur une icône halo → ouvrir l’**arbre de talents** correspondant.

### Navigation proposée

1. L’écran inventaire **reste monté** (pas de changement de scène gameplay).
2. Transition UI : overlay ou panneau plein écran / demi-écran avec **retour** explicite vers l’inventaire.
3. Respect du pipeline popup / écrans :
   - préférer un **`ScreenId`** dédié ou un **overlay** orchestré par `UIManager` ;
   - si popup : nouveau `PopupId` + binding `ScreenPopupBinding` (cf. règle popup générique).

### Modèle d’arbre (exemple Commerce / Marketing)

```
                    [ Racine : Commerce ]
                           │
           ┌───────────────┴───────────────┐
           ▼                               ▼
    Branche GAUCHE                  Branche DROITE
    « Acheteur »                    « Vendeur »
    - payer moins cher              - vendre plus cher
    - remises shop                  - bonus volume vente
    - déblocage offres              - meilleurs marges market
```

- **Gauche** = talents **acheteur** (modificateurs prix d’achat, shop).
- **Droite** = talents **vendeur** (modificateurs prix / volume de vente, market).
- **Racine / centre** = perks communs ou prérequis (optionnel).

Autres pistes : à définir quand les **notes manuscrites / recherches perso** seront importées (cf. §4).

---

## 4. Référence « tablette » (notes perso)

**Tablette** = uniquement la **référence** où l’auteur garde ses notes manuscrites et ses recherches. Ce n’est **pas** un nom de feature in-game ni un document GDD figé.

Quand tu apporteras ce contenu au projet :

→ déposer / structurer dans [`Notes/GDD/INBOX_notes_tablette_recherches.md`](../GDD/INBOX_notes_tablette_recherches.md)

**Tâche projet :** session **[P0-IDEA-001]** (`Notes/Todo_project.md`) — synthèse boucle + priorités ; vrac déjà numérisé dans [`Inbox_gdd.md`](../GDD/Inbox_gdd.md).

Jusque-là, le halo UI reste une **intention visuelle + navigation vers arbres** ; les règles métier (coûts, niveaux, effets shop/market, save) viendront **après** import des notes, pas inventées ici.

---

## 5. Liens avec l’existant

| Système | Lien |
|---------|------|
| `InventoryScreen` / `InventoryUI` | Zone basse ; refactor prefab pour ajouter le panneau halo |
| `UIManager` / `ScreenId.Inventory` | Point d’entrée unique depuis le HUD |
| `PlayerInventory` | Stock items — inchangé |
| XP joueur | `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md` — niveau au centre du halo |
| Progression ferme (par niveau) | `Notes/GDD/SPEC_progression_systeme_aquaponique_par_niveau.md` — panneau in-scène, distinct du halo |
| Shop | Talents acheteur → prix final dans popup achat |
| Market | Talents vendeur → prix / capacité vente |
| Popups | Nouveaux écrans talents via pipeline générique si overlay modal |

Docs inventaire actuelles :

- `Notes/Ui/NOTE_inventory_wallet_upgrade.md`
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`

---

## 6. Phases d’implémentation suggérées

### Phase 0 — Design (actuel)

- [x] Noter la vision et la ref. visuelle (ce document).
- [x] Introduire le document tablette GDD.
- [x] Valider la liste des pistes v1 — **8 compétences** (Marketing, Insectes, Bioconversion, Poisson, Eau, Jardinage, DIY, Magasin) — voir `ProgressionTrackId.cs`.
- [ ] Maquette statique (Figma ou prefab Bezi sans logique).

### Phase 1 — Coque UI

- [ ] Refactor prefab `InventoryScreen` : split vertical halo + grille — **Bezy.ai** (cf. `Notes/Ui/ARBRE_inventory_halo_ui.md`).
- [x] Scripts coque + IDs placeholder — `Assets/Scripts/UI/Inventory/Progression/`.
- [ ] Placeholders visuels halo — prefabs **Bezy** (`PlayerHaloSlotUI`, `PlayerHaloPanel`, patch `InventoryScreen`).
- [ ] Filtres / barre intermédiaire alignés ref (`FilterBarPlaceholder` inactif).

### Phase 2 — Navigation talents

- [x] Overlay arbre — script `TalentTreeOverlayController` (prefab racine overlay : **Bezy**).
- [x] Clic halo → overlay placeholder — wiring Phase 3 OK (2026-06-05).
- [x] Bouton retour → inventaire — wiring Phase 3 OK (`CanvasGroup` sur `InventoryPanel`).

### Phase 3 — Données & gameplay

- [ ] Modèle data piste + arbre + nœuds — **plan 3 étapes** : `Notes/Ui/SESSION_prochaine_halo_arbres_competences.md` ([P0-INV-HALO-006]…[008]).
- [ ] Service progression + save.
- [ ] Application modificateurs shop / market.

### Phase 4 — Polish

- [ ] Animations ouverture arbre, feedback achat talent.
- [ ] Localisation, tooltips, états verrouillé.
- [ ] QA perf (pas de rebuild grille à chaque ouverture halo).

---

## 7. Contraintes techniques (projet)

- Navigation : pas de `SceneManager.LoadScene` direct depuis l’UI inventaire — `SceneNavigator` / `UIManager`.
- Popups talents : `PopupId` + `ScreenPopupBinding` si modal ; pas de double instanciation.
- Pas de logique métier lourde dans les vues UI — controllers / services (`IPlayerModifiersService`, etc.).
- Bezi : decouper prefab en phases (shell → composants → wiring), cf. règle exécution Bezy.

---

## 8. Questions ouvertes & pistes de design

### Déjà acté (direction produit)

- **Déblocage par points** : la progression dans les arbres se fait une fois des **points acquis** (source exacte à préciser via notes perso / GDD).
- **Choix libre de l’ordre** : le joueur **choisit quel arbre développer en premier** — pas de chemin imposé au départ.
- **Pistes envisagées** (liste non exhaustive) : **commerce**, **culture plante**, **culture poisson**, etc. Chaque path a ses **avantages et inconvénients**.
- **Équilibrage = chantier majeur** : les paths doivent rester **viables entre eux**, sans qu’un seul domine toujours. Exemple ouvert : produire **plus de salade, plus vite** pourrait rapporter autant (voire plus) qu’un joueur qui **vend la même salade 3× plus cher** — **à valider par playtest et chiffres**, pas tranché aujourd’hui. C’est l’un des travaux les **plus longs et fastidieux** du projet ; à planifier comme phase dédiée, pas bloquante pour la coque UI.

### Encore à trancher (UI / technique)

- [ ] Halo : nombre exact de slots visibles au lancement (6 vs 8 vs slots déblocables au fil des points) ?
- [ ] Arbre talents : plein écran ou panneau slide par-dessus la grille ?
- [ ] Filtres inventaire : reprendre les catégories de la ref. (All / Equip / Conso / Matériaux) ou adapter au contenu ferme actuel ?
- [ ] Wallet : reste-t-il dans la zone basse ou remonte-t-il dans le halo ?
- [ ] Premier arbre **jouable en prototype** : commerce, culture plante ou autre (vertical slice UI) ?

### Équilibrage (GDD — long terme)

- [ ] Courbe de gain par path (volume / vitesse vs marge / prix unitaire).
- [ ] Coût en points par nœud et par branche ; risque de « rush » d’un seul arbre.
- [ ] Synergies ou pénalités entre paths (ex. focus poisson vs focus plante).
- [ ] Métriques de validation : temps pour atteindre un seuil de revenu équivalent entre builds.

---

## 9. Référence visuelle

Capture de référence (session 2026-05-22) — structure à imiter, contenu à adapter :

- Partie haute : centre personnage + slots périphériques → **joueur + recherches**.
- Partie basse : onglets + grille 5 colonnes → **inventaire items**.

*(Fichier image dans le workspace Cursor assets, session utilisateur.)*
