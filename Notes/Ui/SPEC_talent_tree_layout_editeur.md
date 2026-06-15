# Spec — arbres de talents : layout éditeur (WYSIWYG)

**Statut :** décision architecture **actée** (2026-06-12) — foundation Cursor implémentée.  
**Contexte :** suite session halo inventaire ; remplace l’approche « modules layout calculés en code » pour la **partie visuelle**.

Docs liés :
- **`Notes/Ui/WORKFLOW_creation_arbre_talents.md`** ← procédure auteur pas-à-pas (composition prefab)
- `Notes/Ui/SESSION_prochaine_halo_arbres_competences.md`
- `Notes/Ui/SPEC_rework_inventaire_halo_progression.md`
- `Notes/Ui/ARBRE_inventory_halo_ui.md`
- `Notes/Ui/PROMPTS_Bezi_talent_tree.md`
- Scripts : `Assets/Scripts/Progression/`, `Assets/Scripts/UI/Inventory/Progression/`

---

## Décision (2026-06-07)

| Besoin auteur | Choix |
|---------------|-------|
| Manipuler le rendu visuel | **Prefab UI** — RectTransform, Images, sprites, couleurs |
| Placer / déplacer les nœuds | **Éditeur Unity** (Scene view + mode Prefab), pas positions calculées en C# |
| Règles d’achat, prérequis, coûts | **ScriptableObjects** + `TalentProgressionService` (inchangé) |
| Modules (hub, grille, linéaire) | **GameObjects groupes** dans la hiérarchie prefab, pas de `GridLayoutStrategy` runtime |

Référence visuelle cible : arbres non linéaires (hubs, grilles, chaînes, connexions) — composition manuelle dans l’IDE.

---

## Séparation des responsabilités

| Couche | Contenu | Manipulation |
|--------|---------|--------------|
| **Layout visuel** | Prefab arbre (`RectTransform`, lignes, icônes) | Auteur / Bezy dans Unity |
| **Données métier** | `TalentNodeDefinition`, `TalentTrackDefinition` | Inspector assets SO |
| **État runtime** | `TalentProgressionService`, `PlayerTalentProgressState` | Code uniquement |

Le code **ne recalcule pas** la position des nœuds. Il **lie** les vues éditeur aux SO et **rafraîchit** les états (locked / available / purchased).

---

## Structure prefab cible

```
Assets/Prefabs/Ui/Progression/Trees/
├── Track_Commerce.prefab          ← arbre composé à la main (auteur)
├── Track_Plant.prefab             (plus tard)
└── (briques atomiques Bezy)
    ├── TalentNodeView.prefab
    └── TalentTreeEdgeView.prefab

Track_Commerce (exemple)
└── TreeContent [TalentTreeLayoutRoot]
    ├── Module_Hub                    (GameObject vide = groupe visuel)
    │   ├── Node_Root                 [TalentNodeView]
    │   ├── Node_Buyer01
    │   └── Node_Seller01
    └── Edges
        ├── Edge_Root_Buyer           [TalentTreeEdgeView]
        └── Edge_Root_Seller
```

Parent scroll (Bezy) :

```
TalentTreeOverlay
└── OverlayPanel
    ├── Header (titre + Retour)
    └── TreeScrollView [ScrollRect]
        └── TreeContent               ← instanciation / swap prefab piste
```

---

## Workflow auteur (Unity Editor)

1. Ouvrir le prefab `Track_Commerce` en **mode Prefab**.
2. Dupliquer des instances de `TalentNodeView`.
3. **Déplacer / redimensionner** avec l’outil RectTransform (anchors libres).
4. Sur chaque nœud : assigner le SO `TalentNodeDefinition` dans l’Inspector.
5. Créer les lignes (`TalentTreeEdgeView`) : lier `fromNode` / `toNode` — la ligne se met à jour quand un nœud bouge (`[ExecuteAlways]`).
6. Organiser visuellement avec des GameObjects vides (`Module_Hub`, `Module_Grid`, …).
7. Play mode : clic slot halo Commerce → overlay → états nœuds + achat.

**À éviter sur `TreeContent` :** `GridLayoutGroup`, `VerticalLayoutGroup` — ils écrasent les positions manuelles.

---

## Scripts foundation (Cursor — implémentés 2026-06-12)

### `TalentNodeView`

- Sur chaque nœud UI du prefab arbre.
- Champs : `TalentNodeDefinition`, refs visuelles (icône, overlays locked/purchased), bouton.
- API : `NodeId`, `Bind(service)`, `Refresh(status)`.

### `TalentTreeEdgeView`

- Ligne entre deux nœuds.
- Champs : `fromNode`, `toNode`, `RectTransform lineImage` (Image étirée).
- `[ExecuteAlways]` : repositionne la ligne en **edit mode** et en play mode quand les ancres bougent.

### `TalentTreeLayoutRoot`

- Racine du prefab arbre (`TreeContent`).
- Champs : `trackId`, liste `TalentNodeView[]` (collect auto ou manuelle).
- API : `Bind(service)`, `RefreshAll()` après achat.
- Custom Editor (optionnel) : boutons « Collect child nodes », « Validate edges vs prérequis SO ».

### `TalentTreeOverlayController` (évolution)

- Tableau `TalentTrackPrefabBinding` : `trackId` → prefab `TalentTreeLayoutRoot`.
- `Open(trackId)` : instancier le prefab arbre dans `TreeContent` (ou activer l’enfant pré-placé), puis `layoutRoot.Bind(service)`.
- Piste sans prefab : message « À venir » (pas de crash).

---

## Rendu contrôlable dans l’IDE

| Élément | Manipulation |
|---------|--------------|
| Position nœud | `RectTransform` |
| Icône / cadre | `Image` + sprites par état |
| Couleur branche | Sprite ligne ou `branchColor` sur edge |
| Connexions | Prefab `TalentTreeEdgeView` |
| Zone scrollable | `ScrollRect` + taille `TreeContent` |
| Groupes | GameObjects parents vides |
| Animation sélection | `Animator` sur nœud (optionnel) |

---

## Répartition Bezy / Cursor / auteur

| Livrable | Agent |
|----------|-------|
| `TreeScrollView`, prefabs `TalentNodeView`, `TalentTreeEdgeView` | **Bezy** (phases 1→3) |
| Scripts `TalentNodeView`, `TalentTreeEdgeView`, `TalentTreeLayoutRoot`, Custom Editor | **Cursor** |
| **Composition visuelle** de chaque arbre (placer les nœuds) | **Auteur** |
| SO Commerce + service | **Cursor** (partiellement fait — mock runtime) |

Bezy fournit les **briques** ; l’auteur **compose** les arbres comme un level design UI.

---

## Ce qui reste inchangé

- Flux : `PlayerHaloSlotUI` → `InventoryScreenController` → `TalentTreeOverlayController.Open(trackId)`.
- Overlay inline inventaire (pas popup générique).
- `TalentProgressionService` : prérequis, points, `TryPurchaseNode`, event `StateChanged`.
- Distinction progression joueur (halo) ≠ progression ferme aquaponique.

---

## Phases d’implémentation suggérées

| Phase | Contenu |
|-------|---------|
| **A** | Bezy — briques UI + `TreeScrollView` (conteneur **libre**, sans auto-layout) |
| **B** | Cursor — scripts foundation + wiring overlay |
| **C** | Auteur — prefab `Track_Commerce` (5 nœuds mock + edges) |
| **D** | Cursor — SO designer sur disque (remplacer mock runtime) |
| **E** | Polish — validation editor, tooltips, save progression |

---

## Décisions actées (2026-06-12)

| Point | Décision |
|-------|----------|
| Prefab par piste | **1 prefab arbre par piste** (`Track_Commerce`, …) |
| Conteneur overlay | **`TreeContent` partagé** dans `TreeScrollView` |
| Runtime | **Swap dynamique** : `Open(trackId)` → `Instantiate` → `Bind` → `RefreshAll` |
| Lignes MVP | **Droites** (`TalentTreeEdgeView`, `[ExecuteAlways]`) |
| Validation editor | **Warning** via bouton Custom Editor (pas bloquant) |
| Scroll | **`ScrollRect` seul** pour le MVP (pas de pinch) |
| Nommage scripts | **`TalentNodeView`**, `TalentTreeEdgeView`, `TalentTreeLayoutRoot` |

Fichiers :
- `Assets/Scripts/UI/Inventory/Progression/TalentNodeView.cs`
- `Assets/Scripts/UI/Inventory/Progression/TalentTreeEdgeView.cs`
- `Assets/Scripts/UI/Inventory/Progression/TalentTreeLayoutRoot.cs`
- `Assets/Scripts/UI/Inventory/Progression/TalentTrackPrefabBinding.cs`
- `Assets/Editor/TalentTreeLayoutRootEditor.cs`
- `TalentTreeOverlayController` : bindings `trackPrefabBindings` + `treeContentHost`

## Points ouverts (polish)

- [ ] Courbes / angles sur edges (post-MVP)
- [ ] Zoom pinch mobile si arbre très large

---

## Anti-patterns

- Générer la grille / hub en C# (`HubLayoutStrategy`, etc.) comme workflow principal.
- `LayoutGroup` sur le conteneur des nœuds placés à la main.
- Mélanger layout et logique dans `TalentTreeOverlayController` (reste orchestrateur léger).
