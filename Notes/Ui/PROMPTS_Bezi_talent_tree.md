# Prompts Bezy — arbres talents (layout éditeur)

Spec : `Notes/Ui/SPEC_talent_tree_layout_editeur.md`  
Workflow auteur (après Phase 3) : `Notes/Ui/WORKFLOW_creation_arbre_talents.md`  
**Ne pas rescanner tout le projet.** Réutiliser scripts existants.

---

## Phase 1 — Shell hiérarchie (attendre validation avant Phase 2)

**Objectif :** briques prefab + zone scroll dans l'overlay. **Pas de scripts custom, pas de wiring.**

Créer / modifier :

1. `Assets/Prefabs/Ui/Progression/TalentNodeView.prefab`
   - Racine RectTransform 120×120, enfant `Icon`, `TitleLabel` (vide), `LockedOverlay`, `AvailableOverlay`, `PurchasedOverlay` (GameObjects vides).

2. `Assets/Prefabs/Ui/Progression/TalentTreeEdgeView.prefab`
   - Racine RectTransform, enfant `Line` (Image fine horizontale).

3. Patch `Assets/Prefabs/Ui/InventoryScreen.prefab` → sous `TalentTreeOverlay/OverlayPanel` :
   - Ajouter `TreeScrollView` (ScrollRect vertical)
   - Enfant `TreeContent` (RectTransform libre, **sans** LayoutGroup)
   - Garder `Header` (titre + Retour) et masquer ou déplacer `BodyPlaceholder` sous scroll si besoin.

**Contraintes :** pas de GridLayout/VerticalLayout sur `TreeContent`. Pas de TalentNodeView/TalentTreeEdgeView scripts en Phase 1.

Confirmer fichiers créés + capture hiérarchie avant Phase 2.

---

## Phase 2 — Composants UI (après Phase 1 OK)

Sur `TalentNodeView.prefab` : Image racine + Button, TMP `TitleLabel`, Images overlays (semi-transparent), pas de wiring Inspector custom.

Sur `TalentTreeEdgeView.prefab` : Image `Line` (couleur gris #888, height 4).

Sur `TreeScrollView` : ScrollRect + Mask + Image viewport, `TreeContent` taille min 800×600.

---

## Phase 3 — Wiring (après Phase 2 OK)

Ajouter scripts (déjà dans le projet) :

| Prefab / GO | Script | Champs clés |
|-------------|--------|-------------|
| `TalentNodeView` | `TalentNodeView` | purchaseButton, iconImage, titleLabel, overlays |
| `TalentTreeEdgeView` | `TalentTreeEdgeView` | fromNode, toNode, lineRect, lineImage |
| `TalentTreeOverlay` | `TalentTreeOverlayController` | treeContentHost → `TreeContent`, trackPrefabBindings (vide pour l'instant) |

Menu fallback Cursor : `Rayman → UI → Wire Track Commerce Binding (overlay)`.

---

## Phase 4 — Fix affichage arbre en jeu (2026-06-16, après playtest auteur)

**Symptôme :** titre overlay « Commerce » visible ; arbre `Track_Commerce` masqué ou illisible en Game view.

**Objectif :** rendre l’arbre visible sans contournement runtime Cursor (`TreeMountHost` dynamique).

Modifier **uniquement** :

1. `Assets/Prefabs/Ui/InventoryScreen.prefab` → `TalentTreeOverlay/OverlayPanel` :
   - Ajouter **`TreeMountHost`** (RectTransform stretch, offsets ~16/56, **sans Mask**).
   - Assigner `TalentTreeOverlayController.treeMountHost` → `TreeMountHost` (si champ exposé).
   - `TreeContent` reste **vide** ; `TreeScrollView` peut rester pour scroll futur ou être désactivé par défaut.
   - `BodyPlaceholder` : ne doit pas recouvrir la zone arbre quand arbre actif.

2. `Assets/Prefabs/Ui/Progression/TalentNodeView.prefab` :
   - `TitleLabel` **dernier enfant**, au-dessus du nœud (anchor top-center, blanc 14px Bold).
   - Fond nœud plus clair que `OverlayPanel` ; `AvailableOverlay` alpha ≤ 0.4.

3. Vérifier binding : `track.commerce` → `Track_Commerce.prefab` dans `trackPrefabBindings`.

**Ne pas rescanner le projet.** Confirmer fichiers modifiés + playtest Bootstrap → Inventaire → P1.

---

## Phase auteur (hors Bezy)

Composer `Assets/Prefabs/Ui/Progression/Trees/Track_Commerce.prefab` à la main (dupliquer NodeView + EdgeView, placer RectTransform, assigner SO plus tard).
