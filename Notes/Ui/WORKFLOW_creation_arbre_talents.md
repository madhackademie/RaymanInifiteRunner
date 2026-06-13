# Workflow — création d'un arbre de talents (éditeur Unity)

**Création :** 2026-06-12  
**Statut :** procédure auteur active  
**Branche type :** `feature/talent-tree-ui`  
**Public :** auteur du projet (composition manuelle après livraison Bezy Phases 1–3)

Docs liés :
- `Notes/Ui/SPEC_talent_tree_layout_editeur.md` — architecture et décisions
- `Notes/Ui/PROMPTS_Bezi_talent_tree.md` — phases Bezy (briques UI)
- `Notes/Ui/ARBRE_inventory_halo_ui.md` — hiérarchie inventaire / overlay
- Scripts : `Assets/Scripts/UI/Inventory/Progression/`, `Assets/Scripts/Progression/`

Exemple de référence : **`Track_Commerce`** (premier arbre prototype).

---

## Prérequis

- Branche **`feature/talent-tree-ui`**
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

## Étape 2 — Hiérarchie cible

```
Track_Commerce          [TalentTreeLayoutRoot]
├── Nodes
│   ├── Node_Root       (instance TalentNodeView)
│   ├── Node_Buyer
│   └── Node_Seller
└── Edges
    ├── Edge_Root_Buyer (instance TalentTreeEdgeView)
    └── Edge_Root_Seller
```

Créer les dossiers vides `Nodes` et `Edges` (Create Empty).

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
- [ ] `Track_Commerce.prefab` + `TalentTreeLayoutRoot` (`track.commerce`)
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
