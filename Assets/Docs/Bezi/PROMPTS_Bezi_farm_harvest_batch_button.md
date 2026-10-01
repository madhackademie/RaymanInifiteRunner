# [BZ-FARM-HARVEST-BATCH-BTN-001] Bouton blanc récolte batch

**Demande auteur 2026-10-01.** Un seul prefab. Le C# est déjà dans `HarvestPanelUI.batchHarvestButton`. Pas de nouveau script.

**Succès :** `Save. List what changed. STOP.` Pas de Simulate / Play Mode.

---

## Phase unique — prefab

```
[BZ-FARM-HARVEST-BATCH-BTN-001] Prefab only. STOP after this.

READ ONLY:
@Assets/Prefabs/Ui/FarmHarvestPanel.prefab
@Assets/Scripts/UI/Inventory/HarvestPanelUI.cs

DO NOT rescan the project. DO NOT edit any .cs. DO NOT change HarvestButton, UprootButton or CloseButton.

Duplicate HarvestButton as a sibling named BatchHarvestButton.
Image color = white (1,1,1,1), placeholder, no sprite swap.
Child label text = Batch.
Place it just above HarvestButton and UprootButton. Width = both buttons together. Height = 44.
Copy the layer of HarvestButton.
Wire the existing HarvestPanelUI field batchHarvestButton to BatchHarvestButton. Do not add a component.

Save. List what changed. STOP.
```
