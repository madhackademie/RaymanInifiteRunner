# Workflow — création d'un arbre de talents (8 étapes)

**Type :** procédure Unity éditeur (référence long terme)  
**Création :** 2026-06-12 · **Consolidé :** 2026-06-15  
**Statut doc :** stable — ne pas y mettre le statut de session (voir `Notes/Todo_project.md` → `[P0-INV-HALO-012]`)  
**Exemple :** premier arbre **`Track_Commerce`** (piste P1 Inventaire)

> **Tu reviens dans 1–2 mois ?** Ouvre **uniquement ce fichier**.  
> Pas besoin de parcourir `PROJECT_LOG.md` : l’avancement session est dans `Notes/Todo_project.md`.

---

## Sommaire

1. [Fichiers clés](#fichiers-clés)
2. [Vue d’ensemble des 8 étapes](#vue-densemble-des-8-étapes)
3. [Prérequis](#prérequis)
4. [Où travailler dans Unity](#où-travailler-dans-unity)
5. [Étape 0 — ScriptableObjects](#étape-0--scriptableobjects)
6. [Étape 1 — Prefab racine](#étape-1--prefab-racine)
7. [Étape 2 — Hiérarchie Nodes / Edges](#étape-2--hiérarchie-nodes--edges)
8. [Étape 3 — Placer les nœuds](#étape-3--placer-les-nœuds)
9. [Étape 4 — Placer les edges](#étape-4--placer-les-edges)
10. [Étape 5 — Collect](#étape-5--collect)
11. [Étape 6 — Sauver le prefab](#étape-6--sauver-le-prefab)
12. [Étape 7 — Binding overlay](#étape-7--binding-overlay)
13. [Étape 8 — Playtest](#étape-8--playtest)
14. [Dépannage](#dépannage)
15. [Checklist finale](#checklist-finale)
16. [Dupliquer pour une autre piste](#dupliquer-pour-une-autre-piste)
17. [Docs complémentaires](#docs-complémentaires)

---

## Fichiers clés

| Rôle | Chemin |
|------|--------|
| Prefab arbre Commerce | `Assets/Prefabs/Ui/Progression/Trees/Track_Commerce.prefab` |
| Briques nœud / edge | `Assets/Prefabs/Ui/Progression/TalentNodeView.prefab`, `TalentTreeEdgeView.prefab` |
| Overlay inventaire | `Assets/Prefabs/Ui/InventoryScreen.prefab` |
| SO Commerce | `Assets/Data/Progression/Commerce/` |
| Script layout | `Assets/Scripts/UI/Inventory/Progression/TalentTreeLayoutRoot.cs` |
| Script overlay | `Assets/Scripts/UI/Inventory/Progression/TalentTreeOverlayController.cs` |
| Custom Editor | `Assets/Editor/TalentTreeLayoutRootEditor.cs` |

**Branche de travail type :** `main` (lot talent tree mergé).

---

## Vue d’ensemble des 8 étapes

| Étape | Action | Validation rapide |
|-------|--------|-------------------|
| **0** | Créer 3 SO `TalentNodeDefinition` | 3 `.asset` avec `nodeId` exacts |
| **1** | Racine `Track_Commerce` + `TalentTreeLayoutRoot` | `trackId` = `track.commerce` |
| **2** | Dossiers `Edges` puis `Nodes` | Hiérarchie vide, ordre OK |
| **3** | 3 × `TalentNodeView` + SO assignés | 3 carrés visibles en Prefab Mode |
| **4** | 2 × `TalentTreeEdgeView` câblés | Lignes entre racine et branches |
| **5** | Collect nodes + edges | Arrays : 3 nodes, 2 edges |
| **6** | Sauver `.prefab` sur disque | Fichier dans `Trees/` |
| **7** | Binding dans `InventoryScreen` | `track.commerce` → prefab arbre |
| **8** | Playtest P1 Commerce | Arbre + achat mock OK |

**Premier retour visuel net :** fin de l’**étape 3**.

---

## Prérequis

- Unity ouvert, projet compile sans erreur.
- Prefabs Bezy livrés : `TreeScrollView`, `TreeContent`, `TalentNodeView`, `TalentTreeEdgeView` dans `InventoryScreen`.
- Dossiers (créer si absents) :
  - `Assets/Prefabs/Ui/Progression/Trees/`
  - `Assets/Data/Progression/Commerce/`

---

## Où travailler dans Unity

### Règle d’or

Composer **`Track_Commerce` en Prefab Mode** (double-clic sur le prefab dans Project).  
Réserver **`InventoryScreen`** surtout à l’**étape 7** (binding).

### Pourquoi tu ne vois « rien » aux étapes 1–2

| Raison | Détail |
|--------|--------|
| Overlay inactif | `TalentTreeOverlay` désactivé par défaut |
| Alpha 0 | `CanvasGroup` invisible jusqu’à ouverture runtime |
| `TreeContent` vide | Normal avant étapes 3+ |
| Pas de binding | `trackPrefabBindings` vide jusqu’à **étape 7** |
| Dossiers vides | `Nodes` / `Edges` sans Image → hiérarchie seulement |

### Contrôle visuel optionnel dans `InventoryScreen`

1. Ouvrir `InventoryScreen.prefab`
2. Activer temporairement **`TalentTreeOverlay`**
3. Vérifier **`TreeScrollView`** / viewport
4. **Désactiver** l’overlay avant sauvegarde (état d’origine)

### Mini playtests intermédiaires

| Moment | Faisable ? | Résultat |
|--------|------------|----------|
| Après étape 2 | Non visuel | Hiérarchie seulement |
| Après étapes 3–6 | Oui (preview manuel sous `TreeContent`) | Layout sans swap runtime |
| Après étapes 7–8 | Oui | Test complet swap + achat |

---

## Étape 0 — ScriptableObjects

Le mock runtime exige des **`nodeId` exacts**. Chaque nœud UI pointe vers le SO correspondant.

**Menu :** clic droit → *Create → Game → Progression → Talent Node*

| Fichier asset (exemple repo) | nodeId | displayName | trackId | cost | prerequis |
|------------------------------|--------|-------------|---------|------|-----------|
| `TalentNode_Commerce_Root` | `talent.commerce.root` | Racine Commerce | `track.commerce` | 1 | *(vide)* |
| `TalentNode_Buyer` | `talent.commerce.buyer.discount1` | Acheteur -5% | `track.commerce` | 1 | `talent.commerce.root` |
| `TalentNode_Seller` | `talent.commerce.seller.price1` | Vendeur +5% | `track.commerce` | 1 | `talent.commerce.root` |

**Prérequis SO :** section *Prerequisite NodeIds*, taille = 1, élément 0 = `talent.commerce.root`.

---

## Étape 1 — Prefab racine

> **Important :** la racine doit avoir un **`RectTransform`** (objet UI), pas un `Transform` 3D (`Create Empty` seul dans Project). Sinon l’arbre ne s’affiche pas en Play (`TalentTreeOverlayController` warning).

1. Ouvrir **`InventoryScreen.prefab`** (ou un Canvas temporaire) → clic droit → **UI → Empty** → renommer **`Track_Commerce`**.
2. **Add Component** → **`TalentTreeLayoutRoot`**
3. Inspector :
   - **Track Id** : `track.commerce`
4. Vérifier le composant affiché est bien **`Rect Transform`** (pas `Transform` seul).
5. Composer les étapes 2–6 en **Prefab Mode**, puis sauver dans `Assets/Prefabs/Ui/Progression/Trees/Track_Commerce.prefab`.

---

## Étape 2 — Hiérarchie Nodes / Edges

### Hiérarchie cible

```
Track_Commerce          [TalentTreeLayoutRoot]  trackId = track.commerce
├── Edges               (GameObject vide — EN PREMIER dans la liste)
│   ├── Edge_Root_Buyer     ← étape 4
│   └── Edge_Root_Seller
└── Nodes               (GameObject vide)
    ├── Node_Root           ← étape 3
    ├── Node_Buyer
    └── Node_Seller
```

### Procédure

1. Sous `Track_Commerce` → **Create Empty** → **`Edges`**
2. **Create Empty** → **`Nodes`**
3. Ordre hiérarchie : **`Edges` au-dessus**, **`Nodes` en dessous** (lignes derrière les icônes)
4. RectTransform `Nodes` / `Edges` : anchors stretch ou center, pivot 0.5, pos 0 — **pas de LayoutGroup**

### Validation

- [ ] `TalentTreeLayoutRoot` + `track.commerce`
- [ ] `Edges` avant `Nodes`
- [ ] Pas encore d’instances nœud/edge

### Erreurs fréquentes

- Nœuds directement sous racine → hiérarchie illisible dès 5+ nœuds
- `Edges` après `Nodes` → lignes par-dessus les icônes
- `LayoutGroup` sur `Nodes` → écrase le placement manuel

---

## Étape 3 — Placer les nœuds

1. Glisser **`TalentNodeView.prefab`** ×3 sous `Nodes/`
2. Renommer : `Node_Root`, `Node_Buyer`, `Node_Seller`
3. **RectTransform** (anchors center, pivot 0.5) :

| Nœud | Pos X | Pos Y | SO (Node Definition) |
|------|-------|-------|----------------------|
| Node_Root | 0 | 120 | `TalentNode_Commerce_Root` |
| Node_Buyer | -160 | -40 | `TalentNode_Buyer` |
| Node_Seller | 160 | -40 | `TalentNode_Seller` |

4. Chaque **`TalentNodeView`** → **Node Definition** = SO correspondant

```
           [Racine]
          /        \
    [Acheteur]    [Vendeur]
```

Ajuster à la main — pas de LayoutGroup.

---

## Étape 4 — Placer les edges

1. Glisser **`TalentTreeEdgeView.prefab`** ×2 sous `Edges/`
2. Renommer : `Edge_Root_Buyer`, `Edge_Root_Seller`
3. Câbler **TalentTreeEdgeView** :

| Edge | From Node | To Node |
|------|-----------|---------|
| Edge_Root_Buyer | Node_Root | Node_Buyer |
| Edge_Root_Seller | Node_Root | Node_Seller |

4. Inspector `Track_Commerce` → **Validate edges vs prerequisites** (sans warning bloquant)

Les lignes se mettent à jour en éditeur (`ExecuteAlways`) quand tu bouges un nœud.

---

## Étape 5 — Collect

Sur **`TalentTreeLayoutRoot`** (racine `Track_Commerce`) :

1. **Collect child nodes**
2. **Collect child edges**

**Attendu :** 3 entrées dans `nodeViews`, 2 dans `edgeViews`.

> Si les arrays sont vides après sauvegarde : rouvrir le prefab et refaire Collect avant l’étape 7.

---

## Étape 6 — Sauver le prefab

1. Si composé dans une scène temporaire : glisser la racine vers  
   `Assets/Prefabs/Ui/Progression/Trees/Track_Commerce.prefab`
2. Supprimer toute instance temporaire sous `InventoryScreen` ou scène de test
3. Vérifier le fichier dans Project

---

## Étape 7 — Binding overlay

1. Ouvrir **`Assets/Prefabs/Ui/InventoryScreen.prefab`**
2. Vérifier que **`TreeContent`** est **vide** (pas d’instance `Track_Commerce` enfant — le runtime instancie seul).
3. Sélectionner **`TalentTreeOverlay`** → composant **`TalentTreeOverlayController`**
4. **Track Prefab Bindings** → **+** :
   - **Track Id** : `track.commerce`
   - **Tree Prefab** : glisser **`Track_Commerce.prefab`** depuis Project (asset `.prefab`, pas une instance scene)
5. Vérifier **Tree Content Host** = `TreeContent` (wiring Bezy)
6. Sauver le prefab

Sans cette étape, le Play mode affiche le fallback texte MVP au lieu de l’arbre visuel.

---

## Étape 8 — Playtest

1. **Play** depuis **`Bootstrap`**
2. Onglet **Inventaire** → clic **P1 Commerce**
3. **Attendu :**
   - Overlay : **3 nœuds** + **2 lignes**
   - Racine disponible (vert), branches verrouillées (sombre)
   - Clic Racine → achat 1 pt → état acheté
   - Puis Acheteur ou Vendeur débloqué
   - **Retour** → overlay fermé, grille OK
4. Pas de placeholder texte si arbre visible
5. Console : aucune erreur rouge

---

## Dépannage

| Problème | Cause probable | Action |
|----------|----------------|--------|
| Texte seulement, pas d’arbre | `trackPrefabBindings` vide ou mauvais `trackId` | Étape 7 |
| Warning « sans RectTransform sur la racine » | Racine créée via **Create Empty** (3D) au lieu de **UI → Empty** | Refaire étape 1 |
| Arbre monté mais invisible (pas d’erreur) | Instance sous `TreeContent` en preview + mauvaise position world | Vider `TreeContent` ; laisser le runtime instancier |
| **Arbre entier masqué en jeu** (titre overlay OK) | `TreeScrollView` / **Mask viewport** clippe le contenu ; `BodyPlaceholder` ou `Dimmer` recouvre | Bezy : ajouter **`TreeMountHost`** fixe sous `OverlayPanel` (sans Mask) ; revoir ScrollRect. Cursor : bypass runtime `TreeMountHost` (2026-06-16) |
| Titre nœud invisible | `TitleLabel` sous overlays plein cadre ou contraste faible | Bezy : `TitleLabel` **dernier enfant**, au-dessus du nœud, blanc 14px |
| Clic sans effet | SO absent ou `nodeId` ≠ mock | Étape 0 + 3 |
| « Noeud (SO manquant) » | **Node Definition** non assigné | Étape 3 |
| Lignes mal placées | `fromNode` / `toNode` inversés | Étape 4 |
| Arrays vides au runtime | Collect non fait | Étape 5 |
| Rien visible étapes 1–2 | Normal | Prefab Mode + étape 3 |

---

## Checklist finale

- [ ] 3 SO Commerce, IDs exacts
- [ ] Hiérarchie `Edges` + `Nodes`, ordre OK
- [ ] 3 nœuds positionnés + SO assignés
- [ ] 2 edges câblés + validation OK
- [ ] Collect : 3 nodes, 2 edges
- [ ] Prefab sauvé dans `Trees/`
- [ ] Binding `track.commerce` dans overlay
- [ ] Playtest P1 OK

---

## Dupliquer pour une autre piste

1. Dupliquer le prefab : `Track_Plant`, `Track_Fish`, …
2. Créer les SO avec `trackId` = `track.plant`, `track.fish`, …
3. Composer un layout visuel différent (hub, chaîne, grille…)
4. **Étape 7** : nouvelle entrée dans **Track Prefab Bindings**
5. **Étape 8** : playtest via le slot halo (P2, P3, …)

Piste sans binding : fallback texte « À venir » (MVP overlay).

---

## Docs complémentaires

| Sujet | Fichier |
|-------|---------|
| Architecture layout | `Notes/Ui/SPEC_talent_tree_layout_editeur.md` |
| Phases Bezy (prefabs) | `Notes/Ui/PROMPTS_Bezi_talent_tree.md` |
| Hiérarchie inventaire | `Notes/Ui/ARBRE_inventory_halo_ui.md` |
| Statut tâche / prochaine session | `Notes/Todo_project.md` → `[P0-INV-HALO-012]` |
| Journal session | `PROJECT_LOG.md` (historique uniquement) |
