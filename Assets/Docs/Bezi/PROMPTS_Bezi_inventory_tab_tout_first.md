# [BZ-INV-TABS-002] Inventaire — onglet « Tout » en premier + actif par défaut

**Prefab :** `Assets/Prefabs/Ui/InventoryScreen.prefab`  
**C# Cursor (déjà livré) :** défaut filtre = `TabId.All` — **ne pas modifier .cs**

```
[BZ-INV-TABS-002] Reorder filter tabs ONLY. OPEN InventoryScreen.prefab. STOP.

Do NOT rescan project. Do NOT edit .cs. m_Layer 5.

GOAL: TabButton_All = FIRST (left) in InventoryFilterBar row.

1) Under InventoryFilterBar, reorder sibling index:
   1 TabButton_All
   2 TabButton_Seeds
   3 TabButton_Consumables
   4 TabButton_Harvests
   (HorizontalLayoutGroup — order = display left→right)

2) TabButton_All root Image idle color = SAME as others RGB(0.12,0.12,0.16) a=0.9
   Selection = SelectedHighlight only (not permanent bright bg on All).

3) Keep InventoryFilterTabBar wiring (4 Button + 4 SelectedHighlight refs).

Save. List new child order. STOP. No Play Mode.
```

Launch:
```
@Assets/Docs/Bezi/PROMPTS_Bezi_inventory_tab_tout_first.md
[BZ-INV-TABS-002] Reorder filter tabs ONLY. STOP.
```
