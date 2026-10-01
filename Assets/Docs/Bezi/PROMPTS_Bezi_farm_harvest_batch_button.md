# [BZ-FARM-HARVEST-BATCH-BTN-001] Bouton blanc récolte batch

**Demande auteur 2026-10-01.** Un seul prefab. Le C# est déjà dans `HarvestPanelUI.batchHarvestButton` (`Awake` écoute le clic). Pas de nouveau script, pas de `onClick` persistant.

**Prefab :** `Assets/Prefabs/Ui/FarmHarvestPanel.prefab` — ouvert en Prefab Mode avant chaque phase.

**Succès :** `Save. List what changed. STOP.` Pas de Simulate / Play Mode.

Parent des boutons : `FarmHarvestPanel`. Rangée Récolter / Fermer à `y = 62`. Le bouton Batch est au-dessus, pleine largeur, `y = 114`, hauteur 44.

---

## Phase 1 — hiérarchie

```
[BZ-FARM-HARVEST-BATCH-BTN-001] Phase 1 hierarchy only. STOP after this.

READ ONLY:
@Assets/Prefabs/Ui/FarmHarvestPanel.prefab

Prefab Mode: FarmHarvestPanel.prefab must already be open.
DO NOT rescan the project. DO NOT edit any .cs.
DO NOT add Image, Button, or TMP. DO NOT wire any SerializeField.
DO NOT move or resize HarvestButton, UprootButton, or CloseButton.

Parent: FarmHarvestPanel.
Create BatchHarvestButton.
Create child Label under BatchHarvestButton.

BatchHarvestButton RectTransform:
anchor min (0, 0), anchor max (1, 0), pivot (0.5, 0)
anchoredPosition (0, 114), sizeDelta (-20, 44)

Label RectTransform: stretch fill, anchors (0,0) to (1,1), offsets 0.

Layer UI (5) on BatchHarvestButton and Label.

Save. List what changed. STOP.
```

## Phase 2 — composants

```
[BZ-FARM-HARVEST-BATCH-BTN-001] Phase 2 components only. STOP after this.

READ ONLY:
@Assets/Prefabs/Ui/FarmHarvestPanel.prefab

Prefab Mode must stay open.
DO NOT rescan the project. DO NOT edit any .cs.
DO NOT wire SerializeField. DO NOT set Button.onClick.
DO NOT change HarvestButton, UprootButton, or CloseButton.

On BatchHarvestButton, copy HarvestButton components:
CanvasRenderer, Image, Button.
Image color (1, 1, 1, 1). No new sprite. Raycast Target on.

On Label, copy HarvestButton/Label TextMeshProUGUI.
Text = Batch. Center alignment. Same font size as HarvestButton/Label.

Layer UI (5) on BatchHarvestButton and Label.

Save. List components added. STOP.
```

## Phase 3 — câblage

```
[BZ-FARM-HARVEST-BATCH-BTN-001] Phase 3 wiring only. STOP after this.

READ ONLY:
@Assets/Prefabs/Ui/FarmHarvestPanel.prefab
@Assets/Scripts/UI/Inventory/HarvestPanelUI.cs

Prefab Mode: FarmHarvestPanel.prefab must already be open.
DO NOT rescan the project. DO NOT edit any .cs.
DO NOT change hierarchy. DO NOT add components.
DO NOT set Button.onClick. HarvestPanelUI.Awake already adds the listener.

On FarmHarvestPanel, HarvestPanelUI field batchHarvestButton
= the Button on BatchHarvestButton.

Leave harvestButton, uprootButton, and closeButton unchanged.

Save. List the field wired. STOP.
```
