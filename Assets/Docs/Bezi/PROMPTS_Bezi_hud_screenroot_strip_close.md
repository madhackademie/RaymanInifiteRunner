# [BZ-HUD-CLOSE-STRIP-001] ScreenRoot — retirer les CloseButton (hors FirstLvl)

**Pourquoi :** les écrans HUD sous `UIManager.ScreenRoot` se ferment via la nav. La croix utile reste **FirstLvl** (`NavigationHUD/ExitButton`).  
**Ne pas toucher :** `FirstLvl.unity`, `NavigationHUD.unity` (`ExitButton`), popups (`ShopItemPopup`, `ResourceFeedbackPopup`, `FarmHarvestPanel`, `SeedSelectionUI`).  
**FeaturesHubScreen :** déjà sans Close — skip.

Un prefab par phase. Pas de `/prefab-ui-3phases`. Pas de `.cs`.

---

## Phase 1 — Inventaire

```
[BZ-HUD-CLOSE-STRIP-001] Phase 1 ONLY — delete Inventory CloseButton. STOP.

OPEN Assets/Prefabs/Ui/InventoryScreen.prefab. Do NOT rescan. No .cs. m_Layer 5.

DELETE GameObject Header/CloseButton (and its children, e.g. "x" label).
KEEP Header + title. KEEP InventoryFilterBar, grid, wallet, halo.

On InventorySceneController: set closeButton = None (no Missing).

Save. List deleted GO. STOP. No Play Mode.
```

---

## Phase 2 — Shop

```
[BZ-HUD-CLOSE-STRIP-001] Phase 2 ONLY — delete Shop CloseButton. STOP.

OPEN Assets/Prefabs/Ui/ShopScreen.prefab. Do NOT rescan. No .cs. m_Layer 5.

DELETE GameObject CloseButton (child CloseLabel goes with it).
KEEP SlotsGrid, EmptyCataloguePanel, root layout.

On RuntimeShopScreen: set closeButton = None (no Missing).

Save. List deleted GO. STOP. No Play Mode.
```

---

## Phase 3 — Vente

```
[BZ-HUD-CLOSE-STRIP-001] Phase 3 ONLY — delete Sale CloseButton. STOP.

OPEN Assets/Prefabs/Ui/SaleChannelsScreen.prefab. Do NOT rescan. No .cs. m_Layer 5.

DELETE GameObject Header/CloseButton (and CloseLabel child).
KEEP Header + TitleLabel. KEEP bandeaux / star rows.

On RuntimeSaleChannelsScreen: set closeButton = None (no Missing).

FORBIDDEN: FirstLvl.unity, NavigationHUD ExitButton, any popup prefab.

Save. List deleted GO. STOP. No Play Mode.
```

---

## Lancement Bezy (un thread par phase)

Ph.1 — Prefab Mode `InventoryScreen.prefab` :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md
[BZ-HUD-CLOSE-STRIP-001] Phase 1 ONLY. STOP.
```

Ph.2 — Prefab Mode `ShopScreen.prefab` :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md
[BZ-HUD-CLOSE-STRIP-001] Phase 2 ONLY. STOP.
```

Ph.3 — Prefab Mode `SaleChannelsScreen.prefab` :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md
[BZ-HUD-CLOSE-STRIP-001] Phase 3 ONLY. STOP.
```

## Après Bezy (Cursor, hors prompt)

`RuntimeShopScreen.FindCloseButton` ne doit **pas** retomber sur `buttons[0]` une fois Close absent.
