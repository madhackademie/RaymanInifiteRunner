# [BZ-INV-TABS-003] Inventaire — onglet « Récoltes » entre Tout et Graines

**Prefab :** `Assets/Prefabs/Ui/InventoryScreen.prefab`  
**C# :** ne pas modifier `.cs` (wiring `InventoryFilterTabBar` inchangé)  
**Aujourd’hui (gauche → droite) :** Tout · Graines · Consommables · Récoltes  
**Cible :** Tout · **Récoltes** · Graines · Consommables

Pas de `/prefab-ui-3phases` (reorder sibling only).

```
[BZ-INV-TABS-003] Reorder harvest tab ONLY. OPEN InventoryScreen.prefab. STOP.

Do NOT rescan project. Do NOT edit .cs. Do NOT add/remove GO. m_Layer 5.

GOAL: under InventoryFilterBar, sibling order left→right (HorizontalLayoutGroup):
  1 TabButton_All          ("Tout")
  2 TabButton_Harvests     ("Récoltes")
  3 TabButton_Seeds        ("Graines")
  4 TabButton_Consumables  ("Consommables")

Keep labels, sizes, SelectedHighlight, InventoryFilterTabBar Button + highlight refs.

Save. List new child order. STOP. No Play Mode.
```

## Lancement Bezy

Prefab Mode **`Assets/Prefabs/Ui/InventoryScreen.prefab`**, puis :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_inventory_tab_recoltes_second.md
[BZ-INV-TABS-003] Reorder harvest tab ONLY. STOP.
```
