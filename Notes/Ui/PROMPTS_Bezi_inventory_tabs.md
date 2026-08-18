# Prompts Bezy — Barre onglets inventaire `[BZ-INV-TABS-001]`

**Spec GDD :** `Notes/GDD/SPEC_inventaire_multiverse_hub.md`  
**Tâche :** `[P0-INV-TABS-001]` — onglets grille farm V0  
**Prefab :** `Assets/Prefabs/Ui/InventoryScreen.prefab`  
**Cible hiérarchie :** remplacer / évoluer `FilterBarPlaceholder` sous `InventorySplitLayout`  
**Script (Cursor, ne pas modifier en Bezy) :** `InventoryFilterTabBar.cs` (à créer avant Ph.3)  
**Layers :** UI = `m_Layer: 5` — `Notes/Ui/CONVENTION_layers_unity.md`  
**Réf. press tabs :** `NavTab` pattern — `Notes/Ui/PROMPTS_Bezi_nav_tabs_press.md`

**Succès Bezy = Save + liste changements. STOP. Pas de Simulate / Play Mode.**

**Onglets V0 farm (labels TMP) :**
1. **Graines**
2. **Consommables**
3. **Récoltes**
4. **Tout** (vue global — dernier onglet, visuellement distinct : outline ou couleur accent)

**Hors scope Bezy :** logique filtre C#, `ItemCategory`, playtest, Simulate.

**Prérequis Editor Ph.1–3 :** ouvrir `InventoryScreen.prefab` en mode Prefab avant d’envoyer le prompt (workaround path Bezy — `Notes/Bezi/README_bezi.md`).

**Ordre :** Ph.1 → Ph.2 → Ph.3 (attendre succès avant la suivante).

---

## Phase 1 — Shell hiérarchie barre onglets

```
[BZ-INV-TABS-001] Phase 1 ONLY — Inventory filter tab bar shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate InventoryScreen from scratch.
Keep existing: PlayerHaloPanel, InventoryPanel, TalentTreeOverlay, Dimmer, InventorySplitLayout, InventoryScreenController.

File ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab

Context:
- Under InventorySplitLayout, rename FilterBarPlaceholder → InventoryFilterBar (or replace in place).
- InventoryFilterBar: active (m_IsActive: 1), stretch horizontal, fixed height ~56–64 px, between halo and InventoryPanel.

REQUIRED hierarchy under InventoryFilterBar:
- TabButton_Seeds      (empty GO)
- TabButton_Consumables
- TabButton_Harvests
- TabButton_All

Rules:
- m_Layer: 5 on InventoryFilterBar and ALL children.
- Do not add Image/Button/TMP yet — empty RectTransforms only this phase.
- Horizontal row: equal width tabs, small gap 8 px, padding horizontal 16.
- Preserve InventoryPanel + halo wiring; do not break SerializeFields on InventoryScreenController.

Interdits: no C#; no Simulate; no new scripts; no TalentTreeOverlay edits.

Done = Save prefab. List GO names created/renamed + parent path. STOP.
```

**Checklist review Ph.1 (Cursor)**
- [x] `InventoryFilterBar` actif entre halo et grille
- [x] 4 enfants `TabButton_*` layer 5
- [x] `InventoryPanel` / halo / overlay intacts

---

## Phase 2 — Composants visuels onglets — **OK** (Bezy 2026-08-18)

```
[BZ-INV-TABS-001] Phase 2 ONLY — Tab buttons visuals (Image, Button, TMP). Wait success. STOP.

Do not rescan whole project. Do not modify C#. Same prefab ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab

Parent: InventoryFilterBar → TabButton_Seeds, TabButton_Consumables, TabButton_Harvests, TabButton_All.

Per tab (same structure):
- Root: Image (dark panel bg ~0.12,0.12,0.16 alpha 0.9) + Button (Transition: ColorTint OK for now)
- Child Label: TextMeshProUGUI, font size 22–24, Bold, white, center, raycast off
- Min tap height 48 px, width flexible (HorizontalLayoutGroup on InventoryFilterBar: child force expand width)

Labels EXACT text:
- TabButton_Seeds → "Graines"
- TabButton_Consumables → "Consommables"
- TabButton_Harvests → "Récoltes"
- TabButton_All → "Tout"

TabButton_All accent: slightly lighter bg OR thin outline Image child — visually distinct as "global view" tab.

Add HorizontalLayoutGroup on InventoryFilterBar: spacing 8, padding 16,16,8,8, control child W+H.

Layers: m_Layer 5 on every GO.

Do NOT wire InventoryScreenController or new scripts. Do NOT hook OnClick.

Interdits: no C#; no Simulate; no Animator this phase.

Done = Save. List components added per tab. STOP.
```

**Checklist review Ph.2 (Cursor)**
- [x] 4 boutons lisibles mobile
- [x] **Tout** visuellement distinct
- [x] Layout horizontal stable

---

## Phase 3 — Wiring `InventoryFilterTabBar` — **OK** (Bezy 2026-08-18)

**Prérequis Cursor (avant Ph.3 Bezy) :** créer `Assets/Scripts/UI/Inventory/InventoryFilterTabBar.cs` avec SerializeFields :
- `Button tabSeeds`, `tabConsumables`, `tabHarvests`, `tabAll`
- (optionnel) `Image` highlight / état actif — Cursor complète après Bezy si besoin

```
[BZ-INV-TABS-001] Phase 3 ONLY — Wire InventoryFilterTabBar on InventoryFilterBar. Wait success. STOP.

Do not rescan whole project. Do not modify C# scripts. Prefab ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab

REQUIRED:
1) Add component InventoryFilterTabBar on GameObject InventoryFilterBar.
2) Wire SerializeFields:
   - tabSeeds → TabButton_Seeds Button
   - tabConsumables → TabButton_Consumables Button
   - tabHarvests → TabButton_Harvests Button
   - tabAll → TabButton_All Button
3) InventoryScreenController.filterBarPlaceholder → assign InventoryFilterBar root (same field name if still "filterBarPlaceholder").
4) m_Layer 5 unchanged on all UI under bar.

Do NOT add OnClick UnityEvents (Cursor hooks in code).
Do NOT run Simulate / Play Mode.

Done = Save. Confirm all 4 Button refs assigned + filterBarPlaceholder wired. STOP.
```

**Checklist review Ph.3 (Cursor)**
- [x] `InventoryFilterTabBar` refs OK (4 Button)
- [x] `InventoryScreenController.filterBarPlaceholder` → barre
- [x] Cursor : masquage retiré + filtre `InventoryUI` + `ItemCategory`

---

## Suite Cursor (hors Bezy)

1. Enum `ItemCategory` + champ sur `ItemDefinition`
2. Filtre `InventoryUI` selon onglet + mode vue jeu / **Tout**
3. Retirer masquage `filterBarPlaceholder` dans `InventoryScreenController.OnEnable`
4. Playtest `[P0-INV-TABS-PLAY-001]`

**ID Bezy file polish :** ajouter #20 `[BZ-INV-TABS-001]` dans `Notes/Ui/TODO_Bezy_polish_semaine.md` si suivi file centralisé.
