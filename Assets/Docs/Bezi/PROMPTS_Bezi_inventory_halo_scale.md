# [BZ-INV-HALO-SCALE-001] Halo inventaire plus grand (panneau + slots)

**Pourquoi le jeu ignore tes drags :** le runtime charge `Assets/Prefabs/Ui/InventoryScreen.prefab`. Une instance scène / un override `referenceHeight=320` + `localScale 0.5` écrase le prefab source.

**Cible visuel :** bandeau beige ~2× plus haut qu’aujourd’hui (Preferred 280 → 560). `InventoryPanel` (flexible) rétrécit tout seul. Slots / portrait suivent via `HaloContentScaler` (échelle = hauteur panneau / 320, X=Y).

**Prefab runtime :** `Assets/Prefabs/Ui/InventoryScreen.prefab` (ouvrir en Prefab Mode).  
**Nested :** instance `PlayerHaloPanel` (guid source `1432e647380543f4b8bdc5490d415e3a`).  
**Hors scope :** C#, Simulate, Play Mode, onglets, wallet, `PlayerHaloSlotUI.prefab`.

Succès = Save + liste. STOP.

---

## Phase 1 — Layout panneau (pas de nouveau GO)

```
[BZ-INV-HALO-SCALE-001] Phase 1 ONLY — grow PlayerHaloPanel layout height. STOP.

Do not rescan whole project. Do not modify C#. Do not add/delete GameObjects.
File ONLY: Assets/Prefabs/Ui/InventoryScreen.prefab
Open that prefab in Prefab Mode. Do not edit a scene instance.

On nested PlayerHaloPanel (child of InventorySplitLayout), LayoutElement:
- Preferred Height = 560
- Min Height = 400
- Flexible Height = 0 (or -1)
- Flexible Width = 1 (unchanged)

Do NOT change InventoryPanel LayoutElement (keep Flexible Height 1 so the list shrinks).
Do NOT change HaloSlots / PortraitFrame / LevelLabel sizeDelta or positions.
Do NOT change HaloContentScaler yet.

Save. List LayoutElement values. STOP.
```

---

## Phase 2 — Overrides scale + scaler

```
[BZ-INV-HALO-SCALE-001] Phase 2 ONLY — revert scale overrides, set scaler ref 320. STOP.

Do not rescan whole project. Do not modify C#.
File ONLY: Assets/Prefabs/Ui/InventoryScreen.prefab
Prefab Mode required.

On nested PlayerHaloPanel:
1) HaloContentScaler: referenceHeight = 320. Keep minScale 0.5, maxScale 3. Keep existing targets (HaloSlots, PortraitFrame, LevelLabel).
2) REVERT prefab overrides on HaloSlots, PortraitFrame, LevelLabel: m_LocalScale.x and m_LocalScale.y (currently 0.5). After revert, scaler may write uniform scale (X=Y, Z=1). Do not type different X vs Y.
3) HaloSlots RectTransform: Constrain Proportions Scale ON if the field exists. sizeDelta stay 400x400. Do not move slot children.

Do not change LayoutElement from Phase 1.

Save. List referenceHeight + the 3 localScales. STOP.
```

---

## Phase 3 — Source nested + plus d’override 320

```
[BZ-INV-HALO-SCALE-001] Phase 3 ONLY — inherit scaler from source, drop 320 override. STOP.

Do not rescan whole project. Do not modify C#.
Open Assets/Prefabs/Ui/InventoryScreen.prefab in Prefab Mode.

1) On the nested PlayerHaloPanel instance: REVERT the HaloContentScaler.referenceHeight override if it is still 320 (bold). Instance must inherit 320.
2) Open nested source Assets/Prefabs/Ui/Progression/PlayerHaloPanel.prefab (arrow on instance). Set HaloContentScaler.referenceHeight = 320 (not 200). Do not change that prefab root sizeDelta 663. Save source prefab.
3) Back on InventoryScreen: confirm nested LayoutElement Preferred Height still 560. Apply All on the nested instance if Unity asks.

GUID PlayerHaloPanel must stay 1432e647380543f4b8bdc5490d415e3a. GUID slot prefab a1931597dd60ec948aeb14c6a9ccfa34 unchanged.

Save both prefabs. List overrides remaining on PlayerHaloPanel instance. STOP.
```
