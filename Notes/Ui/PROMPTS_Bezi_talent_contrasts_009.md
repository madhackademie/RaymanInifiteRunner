# Prompts Bezy — Arbre talents Commerce contrastes `[BZ-POLISH-009]`

**Backlog lié :** `[BL-INV-TALENT-003]`  
**Spec :** `Notes/Ui/PROMPTS_Bezi_talent_tree.md` Phase 4 + `WORKFLOW_creation_arbre_talents.md`  
**Layers :** UI = `m_Layer: 5` — `Notes/Ui/CONVENTION_layers_unity.md`  
**Ne pas rescanner tout le projet.** Pas de nouveaux sprites / pas de C#.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

**Hors scope :** filigrane (`[BZ-POLISH-010]`), zoom/scroll (`[BL-INV-TALENT-002]`), nouveaux arbres P2–P6.

### Prefabs / scripts

| Asset | Rôle |
|-------|------|
| `Assets/Prefabs/Ui/InventoryScreen.prefab` | `TalentTreeOverlay` / `OverlayPanel` → ajouter `TreeMountHost` |
| `Assets/Prefabs/Ui/Progression/TalentNodeView.prefab` | Contrastes Locked / Available / Purchased |
| `Assets/Prefabs/Ui/Progression/TalentTreeEdgeView.prefab` | Contraste ligne |
| `Assets/Prefabs/Ui/Progression/Trees/Track_Commerce.prefab` | Instances (héritent du node prefab) — ne pas recomposer |
| `TalentTreeOverlayController.cs` | **ne pas modifier** — Bezy wire `treeMountHost` seulement |

### État actuel (repo)

- `treeMountHost` = **null** → runtime crée `TreeMountHost` (contournement Cursor).
- OverlayPanel `#261F33` a0.95 ; nœud fond `#5A4A78` ; Locked a0.65 ; Available vert a0.4 ; Purchased vert a0.3 ; Edge gris `#888`.
- Layers TalentTreeOverlay encore souvent `m_Layer: 0`.

**Ordre :** Ph.1 → attendre OK → Ph.2 → Ph.3.

---

## Phase 1 — `TreeMountHost` + layers overlay (COPIER TEL QUEL)

```
[BZ-POLISH-009] Phase 1 ONLY — TreeMountHost under OverlayPanel + layer UI=5 on TalentTreeOverlay subtree. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No new sprites.
Do not rename Header / BodyPlaceholder / TreeScrollView / TreeContent.
Do not touch Track_Commerce / TalentNodeView / TalentTreeEdgeView this phase.
Do not recreate InventoryScreen from scratch.

File ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab

REQUIRED:
1) Under TalentTreeOverlay/OverlayPanel create empty UI RectTransform child named TreeMountHost if missing:
   - Stretch full parent
   - offsetMin (16, 56) ; offsetMax (-16, -56)
   - pivot 0.5/0.5
   - NO Mask, NO ScrollRect, NO Image required
   - Sibling order: after Header, before BodyPlaceholder (or last before TreeScrollView) so tree is not covered by Header
2) Keep TreeContent empty. Keep TreeScrollView as-is (may stay inactive or behind TreeMountHost).
3) BodyPlaceholder: set Image raycastTarget=false OR keep inactive default if already unused for tree path — do not delete.
4) Set m_Layer: 5 on TalentTreeOverlay root AND every descendant under it (OverlayPanel, Header, TreeMountHost, BodyPlaceholder, TreeScrollView, TreeContent, Dimmer if child, all nested).
5) Do NOT wire SerializeFields yet (Phase 3).
6) Save.

Interdits: no C#; no Simulate; no contrast colors yet; no filigrane.

Done = Save. List TreeMountHost Rect offsets + each GO under TalentTreeOverlay with layer (all 5). STOP.
```

**Chars ~1450** — OK &lt; 3500.

### Checklist review Phase 1 (Cursor)

- [x] `TreeMountHost` existe sous `OverlayPanel` (stretch, sizeDelta -32/-112 = offsets 16/56)
- [x] pas de Mask / Image / ScrollRect sur host
- [x] `TalentTreeOverlay` + OverlayPanel subtree → `m_Layer: 5`
- [x] `treeMountHost` encore null (wiring = Ph.3)
- [~] Note : Dimmer/Header (hors OverlayPanel) restent layer 0 — hors chemin arbre ; OK pour Ph.1
- [~] Note : BodyPlaceholder encore après TreeMountHost (draw order) — à corriger en Ph.3

---

## Phase 2 — Contrastes nœuds + edges (COPIER TEL QUEL)

```
[BZ-POLISH-009] Phase 2 ONLY — contrast Locked/Available/Purchased on TalentNodeView + edge Line. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No new sprites.
Do not rename LockedOverlay / AvailableOverlay / PurchasedOverlay / TitleLabel / Icon / Line.
Do not edit InventoryScreen or Track_Commerce hierarchy this phase.
Keep GUID of TalentNodeView / TalentTreeEdgeView unchanged.

Files ONLY:
- Assets/Prefabs/Ui/Progression/TalentNodeView.prefab
- Assets/Prefabs/Ui/Progression/TalentTreeEdgeView.prefab

REQUIRED (TalentNodeView):
1) Root Image (node bg): lighter than OverlayPanel — target RGB ~ (0.45, 0.38, 0.58) alpha 1 (clearer than current ~0.35/0.29/0.47).
2) LockedOverlay Image: dark dim RGB (0,0,0) alpha 0.55–0.70 (keep readable, not full black).
3) AvailableOverlay Image: accent green RGB ~ (0.25, 0.90, 0.40) alpha <= 0.40.
4) PurchasedOverlay Image: gold/green owned RGB ~ (0.95, 0.80, 0.25) OR soft green (0.35, 0.95, 0.50) alpha 0.25–0.35 — visually DISTINCT from Available.
5) TitleLabel: last sibling, white, fontSize >= 14 Bold, above node (anchor top-center). Keep raycastTarget false.
6) Layers = 5 on root + all children.

REQUIRED (TalentTreeEdgeView):
7) Line Image: height >= 4 ; color RGB ~ (0.75, 0.72, 0.85) alpha 1 (lighter than #888 so visible on dark panel).
8) Layer = 5 on root + Line.

Interdits: no Animator changes; no C#; no Simulate; no new GO names.

Done = Save. List final colors (RGBA) for root/Locked/Available/Purchased/Line. STOP.
```

**Chars ~1750** — OK &lt; 3500.

### Checklist review Phase 2 (Cursor)

- [x] Available (0.25, 0.9, 0.4, 0.4) α ≤ 0.4
- [x] Purchased gold (0.95, 0.8, 0.25, 0.3) distinct de Available
- [x] Fond nœud (0.45, 0.38, 0.58) plus clair
- [x] Locked (0,0,0,0.65) OK
- [x] TitleLabel last sibling, blanc, fontSize 15, raycast off
- [x] Edge (0.75, 0.72, 0.85) height 4 ; layers 5
- [x] GUID inchangés (diff couleurs/layers seulement)

---

## Phase 3 — Wire `treeMountHost` (COPIER TEL QUEL)

```
[BZ-POLISH-009] Phase 3 ONLY — wire TalentTreeOverlayController.treeMountHost. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No new sprites.
Do not recreate TreeMountHost. Keep layers = 5.
Do not rename nodes. Do not break trackPrefabBindings.

File ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab

REQUIRED:
1) On TalentTreeOverlay → TalentTreeOverlayController:
   - Assign treeMountHost → OverlayPanel/TreeMountHost (RectTransform)
   - Keep bypassScrollRectForTreeMount = true
   - Keep treeContentHost → TreeContent if already wired
   - Keep trackPrefabBindings (track.commerce → Track_Commerce) intact
2) Confirm TreeMountHost has no Mask / no ScrollRect.
3) BodyPlaceholder must not sit above TreeMountHost in draw order when both active — TreeMountHost sibling after BodyPlaceholder OR BodyPlaceholder inactive OK.
4) Save.

Interdits: no C# edits; no Simulate; no filigrane; do not clear bindings.

Done = Save. Confirm treeMountHost fileID non-null + list SerializeField summary. STOP.
```

**Chars ~1100** — OK &lt; 3500.

### Checklist review Phase 3 (Cursor)

- [x] `treeMountHost` → `TreeMountHost` (`fileID: 3447304019327482190`)
- [x] `bypassScrollRectForTreeMount` = true
- [x] Binding `track.commerce` → `Track_Commerce` intact
- [x] Sibling order : TreeMountHost **après** BodyPlaceholder (draw OK)
- [x] Host sans Mask / ScrollRect

### Après Bezy (auteur / Cursor)

1. [ ] Playtest : Inventaire → P1 Commerce → nœuds Locked/Available/Owned lisibles + edges visibles.
2. Si OK : cocher `[BZ-POLISH-009]` dans `Notes/Todo_project.md` ; prochain Bezy = `[BZ-POLISH-010]` filigrane.
3. Optionnel Cursor : runtime `EnsureRuntimeTreeMountHost` ne crée plus (Find trouve le host) — cleanup plus tard OK.
