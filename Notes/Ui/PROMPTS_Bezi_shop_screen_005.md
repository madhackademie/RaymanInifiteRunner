# Prompts Bezy — Écran shop `[BZ-POLISH-005]`

**Prefab :** `Assets/Prefabs/Ui/ShopScreen.prefab`  
**Script (ne pas modifier) :** `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs`  
**Layers :** UI = `m_Layer: 5` — `Notes/Ui/CONVENTION_layers_unity.md`  
**Ne pas rescanner tout le projet.** Pas de nouveaux sprites / pas de C#.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

Hiérarchie actuelle (minimale) : `ShopScreen` → `CloseButton` / `CloseLabel` / `SlotsGrid`.  
Slots catalogue = spawn runtime (`InventorySlotUI`) dans `SlotsGrid`.

**Ordre :** Ph.1–3 — **OK** (Bezy 2026-07-29) + Cursor wire empty catalogue — **CLOS**

### Review Phase 1 (Cursor, 2026-07-29)
- [x] ShopScreen / CloseButton / CloseLabel / SlotsGrid → m_Layer: 5
- [x] RuntimeShopScreen refs OK

### Review Phase 2 (Cursor, 2026-07-29)
- [x] SlotsGrid cell 112 / spacing 14 / padding 16 ; Close 88×48 ; fonts/backdrops OK

### Review Phase 3 (Cursor, 2026-07-29)
- [x] EmptyCataloguePanel inactive, layer 5, Image #1A1A1A a0.92
- [x] Title "Catalogue vide" 28 / Body 18 ; sibling after SlotsGrid
- [x] Cursor: `emptyCataloguePanel` wired + Show/Hide

---

## Phase 1 — Layers UI — OK (archive)

```
[BZ-POLISH-005] Phase 1 ONLY — ShopScreen layer UI=5 everywhere. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. No new sprites.
Do not rename ShopScreen / CloseButton / CloseLabel / SlotsGrid.
Do not recreate prefab from scratch.

File ONLY:
- Assets/Prefabs/Ui/ShopScreen.prefab

REQUIRED:
1) Set m_Layer: 5 on ShopScreen root AND every child (CloseButton, CloseLabel, SlotsGrid, any nested).
2) Fix any m_Layer: 0 (CloseLabel currently 0).
3) Keep RuntimeShopScreen SerializeFields wired (slotsContainer, closeButton, etc.).
4) Save.

Interdits: no hierarchy rebuild; no Simulate; no C#.

Done = Save. List each GO + layer (all must be 5). STOP.
```

---


### Review Phase 2 (Cursor, 2026-07-29)
- [x] SlotsGrid: cell 112x112, spacing 14, padding >=8
- [x] CloseButton 88x48 (>=44), CloseLabel fontSize 24, contrast OK
- [x] Root alpha 0.95 + SlotsGrid/content alpha 0.9 ; refs RuntimeShopScreen OK

## Phase 2 — Lisibilité grille / close — OK (archive)

```
[BZ-POLISH-005] Phase 2 ONLY — ShopScreen grid + close contrast. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No new sprites.
Keep all layers = 5. Do not rename nodes.

File ONLY:
- Assets/Prefabs/Ui/ShopScreen.prefab

REQUIRED:
1) SlotsGrid GridLayoutGroup: cell ~120x120 (or keep 112 if already), spacing >= 14, padding >= 8.
2) CloseButton: hit area >= 44x44; label contrast (light text on dark OR dark on light).
3) CloseLabel TMP/Text fontSize >= 22 if present.
4) Root / content backdrops (if Images exist): dark readable alpha >= 0.9 so Home does not bleed through.
5) Do not break RuntimeShopScreen refs.
6) Save.

Interdits: no empty panel yet (Phase 3); no Simulate; no C#.

Done = Save. List GridLayout values + CloseButton size. STOP.
```

---

## Phase 3 — Empty catalogue panel (COPIER TEL QUEL)

```
[BZ-POLISH-005] Phase 3 ONLY — EmptyCataloguePanel placeholder. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No new sprites.
Keep layers = 5. Do not rename ShopScreen / SlotsGrid / CloseButton.

File ONLY:
- Assets/Prefabs/Ui/ShopScreen.prefab

REQUIRED:
1) Under ShopScreen create EmptyCataloguePanel (INACTIVE by default):
   - Image dark #1A1A1A alpha ~0.92, stretch over SlotsGrid area (or centered card)
   - Child TMP Title: "Catalogue vide" fontSize >= 24
   - Child TMP Body: "Aucun article a afficher pour le moment." fontSize >= 18, wrap
2) Sibling order: panel above SlotsGrid visually when active.
3) Layer 5 on panel + children.
4) Do NOT wire new SerializeFields (Cursor later). Keep existing RuntimeShopScreen refs intact.
5) Save.

Interdits: no C#; no Simulate; do not delete SlotsGrid.

Done = Save. Confirm EmptyCataloguePanel inactive + hierarchy. STOP.
```

**Après Bezy (Cursor) :** optionnel — SerializeField `emptyCataloguePanel` + Show/Hide quand listings vides (remplace/complète `fallbackListText`).
