# Workflow — création d'un arbre de talents (éditeur Unity)

**Création :** 2026-06-12  
**Statut :** procédure auteur active  
**Branche type :** **`main`** (merge talent tree 2026-06-15)  
**Public :** auteur du projet (composition manuelle après livraison Bezy Phases 1–3)

### État `Track_Commerce` (2026-06-15)

| Étape | Statut |
|-------|--------|
| 0 SO | OK — `Assets/Data/Progression/Commerce/` |
| 1–4 | OK — prefab avec `Nodes` / `Edges` + 3 nœuds + 2 edges |
| 5 Collect | **À revérifier** — `nodeViews` / `edgeViews` peuvent être vides → boutons Collect |
| 6 Sauvegarde | OK — `Trees/Track_Commerce.prefab` |
| **7 Binding** | **Prochaine session** — `trackPrefabBindings` vide dans `InventoryScreen` |
| 8 Playtest | Après étape 7 |

Docs liés :
- `Notes/Ui/SPEC_talent_tree_layout_editeur.md` — architecture et décisions
- `Notes/Ui/PROMPTS_Bezi_talent_tree.md` — phases Bezy (briques UI)
- `Notes/Ui/ARBRE_inventory_halo_ui.md` — hiérarchie inventaire / overlay
- Scripts : `Assets/Scripts/UI/Inventory/Progression/`, `Assets/Scripts/Progression/`

Exemple de référence : **`Track_Commerce`** (premier arbre prototype).

---

## Où travailler + contrôles entre étapes

### Pourquoi tu ne vois « rien » dans `InventoryScreen`

C’est **normal** aux étapes 1–2 :

| Raison | Détail |
|--------|--------|
| Overlay **inactif** | `TalentTreeOverlay` est désactivé par défaut dans le prefab (comportement runtime). |
| `CanvasGroup` alpha 0 | L’overlay est invisible tant qu’il n’est pas ouvert en jeu. |
| `TreeContent` vide | Pas encore d’instances `TalentNodeView` — étapes 1–2 = structure vide. |
| Pas de binding | `trackPrefabBindings` vide jusqu’à l’**étape 7** — en Play, l’arbre ne se charge pas tout seul. |
| Dossiers `Nodes` / `Edges` | GameObjects **sans Image** → rien à l’écran, seulement dans la hiérarchie. |

**Recommandation :** composer **`Track_Commerce` en Prefab Mode** (double-clic sur le prefab dans Project), **pas** en permanence sous `InventoryScreen`. Réserver `InventoryScreen` à l’**étape 7** (binding overlay).

### Contrôle visuel rapide dans `InventoryScreen` (optionnel)

Si tu veux quand même voir la zone scroll dans l’overlay :

1. Ouvrir `InventoryScreen.prefab`
2. Activer temporairement **`TalentTreeOverlay`** (case à cocher à gauche du nom)
3. Sélectionner **`OverlayPanel` → `TreeScrollView`** — tu dois voir la zone scroll (viewport vide)
4. **Désactiver** `TalentTreeOverlay` avant de sauver (remettre l’état inactif comme à l’origine)

Ne pas laisser l’overlay actif en permanence dans le prefab sauf si tu sais ce que tu fais — le runtime s’attend à alpha 0 + inactif au départ.

### Tableau — quoi contrôler après chaque étape

| Étape | Où vérifier | Tu dois voir / avoir |
|-------|-------------|----------------------|
| **0** SO | Project + Inspector | 3 assets `.asset` dans `Assets/Data/Progression/Commerce/`, `nodeId` exacts |
| **1** Racine | Prefab Mode `Track_Commerce` | Composant `TalentTreeLayoutRoot`, `trackId` = `track.commerce` |
| **2** Hiérarchie | Hiérarchie prefab (pas Scene obligatoire) | `Nodes` + `Edges` vides ; **Edges avant Nodes** |
| **3** Nœuds | **Scene / Prefab Mode** | **3 carrés 120×120** (cadres sombres) + labels TMP |
| **4** Edges | Scene / Prefab Mode | **Lignes grises** entre racine et branches (se mettent à jour si tu bouges un nœud) |
| **5** Collect | Inspector `TalentTreeLayoutRoot` | Arrays : 3 nodes, 2 edges |
| **6** Sauvegarde | Project | Fichier `Trees/Track_Commerce.prefab` |
| **7** Binding | `InventoryScreen` → overlay | Entrée `track.commerce` dans `trackPrefabBindings` |
| **8** Playtest | Play mode | Arbre complet au clic P1 Commerce |

**Premier retour visuel net :** fin de l’**étape 3** (nœuds instanciés).

### Mini playtests intermédiaires (optionnel)

| Moment | Faisable ? | Ce que tu obtiens |
|--------|------------|-------------------|
| Après étape 2 | Non visuel | Hiérarchie seulement |
| Après étape 3–6 | Oui, en glissant `Track_Commerce` **à la main** sous `TreeContent` + binding temporaire | Aperçu layout sans achat SO |
| Après étape 7–8 | Oui, test complet | Swap runtime + achat nœuds |

Pour un aperçu avant l’étape 7 : ouvre `InventoryScreen`, active overlay, glisse une **instance** de `Track_Commerce` sous `TreeContent`, Play → Inventaire → P1. Sans binding, le code ne l’instanciera pas tout seul — c’est un **preview manuel** éditeur seulement.

---

## Prérequis

- Branche **`main`** (lot talent tree mergé)
- Unity ouvert, prefabs Bezy compilés sans erreur
- Dossier à créer si absent : `Assets/Prefabs/Ui/Progression/Trees/`
- Dossier SO : `Assets/Data/Progression/Commerce/` (à créer)

---

## Étape 0 — Créer les 3 ScriptableObjects (obligatoire pour les clics)

Le service mock utilise ces **IDs exacts**. Chaque nœud UI doit pointer vers un SO avec les **mêmes** `nodeId`.

**Menu :** clic droit → *Create → Game → Progression → Talent Node*

| Asset (nom fichier) | nodeId | displayName | trackId | cost | prerequis |
|---------------------|--------|-------------|---------|------|-----------|
| `TalentNode_Commerce_Root` | `talent.commerce.root` | Racine Commerce | `track.commerce` | 1 | *(vide)* |
| `TalentNode_Commerce_Buyer` | `talent.commerce.buyer.discount1` | Acheteur -5% | `track.commerce` | 1 | `talent.commerce.root` |
| `TalentNode_Commerce_Seller` | `talent.commerce.seller.price1` | Vendeur +5% | `track.commerce` | 1 | `talent.commerce.root` |

Pour les prérequis : dans l’Inspector du SO, section *Prerequisite NodeIds*, **taille = 1**, élément 0 = `talent.commerce.root`.

---

## Étape 1 — Créer le prefab racine

1. Dans `Trees/`, clic droit → **Create Empty** → renommer **`Track_Commerce`**
2. **Add Component** → `RectTransform` (si besoin) — en fait crée plutôt via **UI → Empty** sous un canvas temporaire, ou :
   - Ouvrir `InventoryScreen` en mode prefab
   - Sous `TreeContent`, clic droit → **Create Empty**
   - Renommer `Track_Commerce`
3. Sur `Track_Commerce` : **Add Component** → **`TalentTreeLayoutRoot`**
4. Inspector `TalentTreeLayoutRoot` :
   - **Track Id** : `track.commerce`

---

## Étape 2 — Hiérarchie cible (organiser le prefab avant de placer nœuds et lignes)

### Objectif

Structurer **`Track_Commerce`** en deux zones logiques :

- **`Nodes/`** — tout ce qui est cliquable (instances `TalentNodeView`)
- **`Edges/`** — tout ce qui relie visuellement les nœuds (instances `TalentTreeEdgeView`)

Ce n’est pas du code : c’est du **rangment éditeur** pour composer l’arbre comme un petit level design UI. Les dossiers vides n’ont pas de rendu à l’écran.

### Hiérarchie attendue

```
Track_Commerce          [TalentTreeLayoutRoot]  trackId = track.commerce
├── Nodes               (GameObject vide — conteneur)
│   ├── Node_Root       ← étape 3 (instance TalentNodeView)
│   ├── Node_Buyer
│   └── Node_Seller
└── Edges               (GameObject vide — conteneur)
    ├── Edge_Root_Buyer ← étape 4 (instance TalentTreeEdgeView)
    └── Edge_Root_Seller
```

> Variante plus tard : sous `Nodes/`, tu peux ajouter des groupes vides (`Module_Hub`, `Module_Grid`…) pour séparer visuellement des branches. **Pas obligatoire** pour le prototype Commerce.

### Procédure Unity (pas à pas)

**Contexte :** tu as terminé l’étape 1 — racine `Track_Commerce` avec `TalentTreeLayoutRoot` et `trackId` = `track.commerce`.

1. Sélectionner **`Track_Commerce`** dans la hiérarchie (mode Prefab ou instance temporaire).
2. Clic droit sur `Track_Commerce` → **Create Empty** → renommer **`Nodes`**.
3. Recommencer → **Create Empty** → renommer **`Edges`**.
4. Vérifier l’**ordre des enfants** sous `Track_Commerce` (du haut vers le bas dans la hiérarchie) :
   - **`Edges` en premier** (au-dessus dans la liste)
   - **`Nodes` en second** (en dessous)

   Pourquoi : en UI Unity, un sibling **plus bas** dans la hiérarchie est dessiné **par-dessus**. Les lignes restent derrière les nœuds.

5. Sur `Nodes` et `Edges` (RectTransform) :
   - **Anchors** : stretch plein parent (min 0,0 — max 1,1) *ou* center selon ton habitude ;
   - **Pivot** : 0.5 / 0.5 ;
   - **Pos** : 0, 0 ;
   - **Taille** : laisser le parent gérer (offset 0) — **pas de LayoutGroup** sur ces dossiers.

6. Ne pas encore glisser `TalentNodeView` ni `TalentTreeEdgeView` — c’est l’**étape 3** et **4**.

### Ce que cette étape ne fait pas

| Hors scope étape 2 | Étape concernée |
|--------------------|-----------------|
| Instancier les nœuds | Étape 3 |
| Assigner les SO | Étape 3 |
| Positionner RectTransform des nœuds | Étape 3 |
| Créer / câbler les edges | Étape 4 |
| Bouton *Collect child nodes* | Étape 5 |
| Sauver le `.prefab` sur disque | Étape 6 |

### Validation rapide (avant étape 3)

- [ ] `Track_Commerce` a le composant **`TalentTreeLayoutRoot`** (`trackId` = `track.commerce`).
- [ ] Deux enfants directs : **`Nodes`** et **`Edges`** (GameObjects vides, sans Image obligatoire).
- [ ] **`Edges`** listé **avant** **`Nodes`** dans la hiérarchie (lignes sous les icônes).
- [ ] Aucun `VerticalLayoutGroup` / `GridLayoutGroup` sur `Track_Commerce`, `Nodes` ou `Edges`.
- [ ] Aucune instance `TalentNodeView` / `TalentTreeEdgeView` encore — normal à ce stade.

### Erreurs fréquentes

- **Mettre les nœuds directement sous `Track_Commerce`** sans dossier `Nodes/` → fonctionne au runtime, mais la hiérarchie devient illisible dès 5+ nœuds ; le bouton *Collect child nodes* ramasse quand même les vues, mais le rangement est pénible.
- **Edges après Nodes dans la hiérarchie** → lignes qui passent **par-dessus** les icônes (illisible).
- **Ajouter un LayoutGroup** sur `Nodes` → écrase les positions manuelles de l’étape 3 (anti-pattern spec layout éditeur).

### Durée estimée

~2–5 minutes. Si tu bloques sur le mode Prefab : ouvre `Track_Commerce` en **Prefab Mode** (double-clic sur le prefab dans Project) pour isoler l’édition.

---

## Étape 3 — Placer les 3 nœuds

1. Glisser **`TalentNodeView.prefab`** ×3 sous `Nodes/`
2. Renommer : `Node_Root`, `Node_Buyer`, `Node_Seller`
3. **RectTransform** (anchors **center**, pivot 0.5/0.5) — positions de départ :

| Nœud | Pos X | Pos Y | SO à assigner |
|------|-------|-------|---------------|
| Node_Root | 0 | 120 | `TalentNode_Commerce_Root` |
| Node_Buyer | -160 | -40 | `TalentNode_Commerce_Buyer` |
| Node_Seller | 160 | -40 | `TalentNode_Commerce_Seller` |

4. Sur chaque instance, composant **`TalentNodeView`** → champ **Node Definition** = le SO correspondant

Disposition visuelle (hub) :

```
           [Racine]
          /        \
    [Acheteur]    [Vendeur]
```

Ajuste à la main avec l’outil **Rect Transform** — pas de LayoutGroup.

---

## Étape 4 — Placer les 2 edges

1. Glisser **`TalentTreeEdgeView.prefab`** ×2 sous `Edges/`
2. Renommer : `Edge_Root_Buyer`, `Edge_Root_Seller`
3. Câbler **TalentTreeEdgeView** :

| Edge | From Node | To Node |
|------|-----------|---------|
| Edge_Root_Buyer | `Node_Root` | `Node_Buyer` |
| Edge_Root_Seller | `Node_Root` | `Node_Seller` |

Les lignes se mettent à jour seules (`ExecuteAlways`) quand tu déplaces les nœuds.

4. Bouton Inspector sur `Track_Commerce` : **Validate edges vs prerequisites** → doit passer sans warning (ou seulement des warnings explicites si typo d’ID).

---

## Étape 5 — Collecter les listes

Sur **`TalentTreeLayoutRoot`** (racine `Track_Commerce`) :

1. **Collect child nodes**
2. **Collect child edges**

Vérifie que les tableaux contiennent 3 nodes et 2 edges.

---

## Étape 6 — Sauver le prefab

1. Glisser `Track_Commerce` de la hiérarchie vers `Assets/Prefabs/Ui/Progression/Trees/Track_Commerce.prefab`
2. Supprimer l’instance temporaire dans la scène/prefab parent si tu as composé dans `InventoryScreen`

---

## Étape 7 — Binder dans l’overlay

1. Ouvrir **`InventoryScreen.prefab`**
2. Sélectionner **`TalentTreeOverlay`** → `TalentTreeOverlayController`
3. **Track Prefab Bindings** → **+** :
   - **Track Id** : `track.commerce`
   - **Tree Prefab** : glisser **`Track_Commerce.prefab`** (Unity prend le `TalentTreeLayoutRoot` sur la racine)
4. Vérifier **Tree Content Host** = `TreeContent` (déjà câblé par Bezy)
5. **Save** le prefab

---

## Étape 8 — Playtest

1. Play depuis **Bootstrap**
2. Onglet **Inventaire** → clic **P1 Commerce**
3. Attendu :
   - Overlay avec **3 nœuds** + lignes
   - Racine **disponible** (overlay vert), autres **verrouillés** (sombre)
   - Clic Racine → achat (1 pt) → état **acheté**
   - Puis Acheteur ou Vendeur débloqué
   - **Retour** → overlay fermé, grille OK
4. Pas de texte placeholder (masqué si arbre visible)
5. Console : pas d’erreur rouge

---

## Dépannage rapide

| Problème | Cause probable |
|----------|----------------|
| Arbre pas visible, texte seulement | `trackPrefabBindings` vide ou mauvais `trackId` |
| Clic nœud sans effet | SO absent ou `nodeId` ≠ mock |
| « Noeud (SO manquant) » | **Node Definition** non assigné sur l’instance |
| Lignes mal placées | `fromNode` / `toNode` inversés ou non assignés |
| Tous les overlays visibles en même temps | Normal avant Play — au runtime un seul état actif |

---

## Checklist finale

- [ ] 3 SO Commerce avec IDs exacts
- [ ] Étape 2 : dossiers `Nodes` + `Edges` créés, ordre hiérarchie OK (Edges avant Nodes)
- [ ] 3 nœuds positionnés + SO assignés
- [ ] 2 edges câblés + validation OK
- [ ] Collect nodes/edges fait
- [ ] Binding overlay `track.commerce` → prefab
- [ ] Playtest P1 achat visuel OK

---

## Répéter pour une autre piste

1. Dupliquer la structure : `Track_Plant`, `Track_Fish`, etc.
2. Créer les SO `TalentNodeDefinition` avec `trackId` = `track.plant`, `track.fish`, …
3. Composer le layout visuel (forme différente par piste — grille, chaîne, hub…)
4. Ajouter une entrée dans **Track Prefab Bindings** (`ProgressionTrackId` stable)
5. Playtest via le slot halo correspondant (P2 Plante, P3 Poisson, …)

Piste sans prefab bindé : message texte « À venir » (fallback overlay MVP).
