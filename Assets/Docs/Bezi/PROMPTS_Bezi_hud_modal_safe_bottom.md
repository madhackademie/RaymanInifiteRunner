# [BZ-HUD-MODAL-SAFE-BOTTOM-001] Modales HUD — même shell que FeaturesHub (Bottom 260)

**Ref Play Mode auteur :** `FeaturesHubScreen(Clone)` → RectTransform **Bottom = 260**.  
**But :** Shop / Hub / Vente / Inventaire = **même cadre** au-dessus de la nav zoomée.  
**Interdit :** `FirstLvl.unity` (scène gameplay, hors canvas HUD). `NavigationHUD.unity` NavBarContainer **reste 128**. Pas de `.cs` (Cursor fera `UIManager.NavBarHeight = 260` **après** Bezy).

**Canvas :** scaler 1080×1920, match 0.5. Unités = Inspector Left/Right/Top/Bottom.

**Compte (pourquoi 260, pas 128) :**

| Piece | px canvas |
|--------|-----------|
| `NavBarContainer` height | 128 |
| cadre actif `sizeDeltaExpand.y` | 105 |
| cadre actif `posY` | 38 |
| Haut géométrique du cadre ≈ 128 + 105/2 + 38 | **~218** |
| Marge air (auteur Play Mode) | **+42** |
| **Cible unique** | **Bottom = 260** |

Si tu as une autre valeur Inspector, dis-la **avant** Phase 1 — on substitue 260 partout.

**Succès :** Save. List. STOP. Pas de Simulate / Play Mode.

---

## Phase 1 — ShopScreen RectTransforms ONLY

Prefab Mode **obligatoire** : `Assets/Prefabs/Ui/ShopScreen.prefab`

```
/prefab-ui-3phases
Task ID: [BZ-HUD-MODAL-SAFE-BOTTOM-001]
Prefab: Assets/Prefabs/Ui/ShopScreen.prefab
Phase: 1
```

```
[BZ-HUD-MODAL-SAFE-BOTTOM-001] PHASE 1 ONLY — ShopScreen root+grid RectTransforms. STOP.

OPEN Assets/Prefabs/Ui/ShopScreen.prefab in Prefab Mode. Do NOT rescan. No .cs. No new GO. No FirstLvl.unity. No NavigationHUD.unity. m_Layer 5.

ROOT ShopScreen RectTransform:
- anchors min (0,0) max (1,1), pivot (0.5,0.5)
- Left 0, Right 0, Top 0, Bottom 260
- (offsetMin.y = 260, offsetMax.y = 0, offsetMin.x = 0, offsetMax.x = 0)
- Keep Image + RuntimeShopScreen. Do not change sprite.

CHILD SlotsGrid RectTransform:
- anchors min (0,0) max (1,1), pivot (0.5,0.5)
- Left 24, Right 24, Top 16, Bottom 0
- (offsetMin = 24,0 ; offsetMax = -24,-16)
- Keep GridLayoutGroup values (cell 112, spacing 14, pad 16, constraint 5).
- Do NOT remove ContentSizeFitter yet (Phase 2).

CHILD EmptyCataloguePanel RectTransform:
- same insets as SlotsGrid: Left 24, Right 24, Top 16, Bottom 0.
- Keep inactive.

Do NOT move CloseButton. Do NOT rename nodes.

Save. Reply: root Bottom, SlotsGrid Left/Right/Top/Bottom. STOP.
```

---

## Phase 2 — ShopScreen ContentSizeFitter ONLY

```
/prefab-ui-3phases
Task ID: [BZ-HUD-MODAL-SAFE-BOTTOM-001]
Prefab: Assets/Prefabs/Ui/ShopScreen.prefab
Phase: 2
```

```
[BZ-HUD-MODAL-SAFE-BOTTOM-001] PHASE 2 ONLY — ShopScreen SlotsGrid fitter. STOP.

OPEN Assets/Prefabs/Ui/ShopScreen.prefab in Prefab Mode. No .cs. No FirstLvl. No new sprites.

SlotsGrid has ContentSizeFitter VerticalFit=Preferred — that shrinks the shop to a 1-row bar (~144px). BAD.

REQUIRED:
1) ContentSizeFitter on SlotsGrid: set Vertical Fit = Unconstrained (0). Horizontal Fit stay Unconstrained. Prefer disable the component if Unconstrained both axes.
2) Keep GridLayoutGroup + Image. Keep Phase 1 rects (root Bottom 260, grid insets 24/24/16/0).
3) EmptyCataloguePanel stays inactive.

Save. Reply: ContentSizeFitter enabled? verticalFit value. STOP. No Play Mode.
```

---

## Phase 3 — même Bottom 260 sur Hub / Vente / Inventaire

**Un prefab par appel.** Même chiffres. Ouvrir Prefab Mode à chaque fois.

### 3a — FeaturesHubScreen

```
/prefab-ui-3phases
Task ID: [BZ-HUD-MODAL-SAFE-BOTTOM-001]
Prefab: Assets/Prefabs/Ui/FeaturesHubScreen.prefab
Phase: 1
```

```
[BZ-HUD-MODAL-SAFE-BOTTOM-001] PHASE 3a ONLY — FeaturesHubScreen root Bottom 260. STOP.

OPEN Assets/Prefabs/Ui/FeaturesHubScreen.prefab. No .cs. No FirstLvl. No NavigationHUD.

ROOT FeaturesHubScreen:
- stretch anchors (0,0)-(1,1), pivot 0.5/0.5
- Left 0, Right 0, Top 0, Bottom 260

Keep PlaceholderRoot 640x720 centered. Keep WipIllustration 560x560, Title 600x60, Subtitle 600x50. Keep RuntimeFeaturesHubScreen wiring.

Save. Reply root Bottom. STOP.
```

### 3b — SaleChannelsScreen

```
/prefab-ui-3phases
Task ID: [BZ-HUD-MODAL-SAFE-BOTTOM-001]
Prefab: Assets/Prefabs/Ui/SaleChannelsScreen.prefab
Phase: 1
```

```
[BZ-HUD-MODAL-SAFE-BOTTOM-001] PHASE 3b ONLY — SaleChannelsScreen root Bottom 260. STOP.

OPEN Assets/Prefabs/Ui/SaleChannelsScreen.prefab. No .cs. No FirstLvl.

ROOT SaleChannelsScreen:
- stretch (0,0)-(1,1), pivot 0.5/0.5
- Left 0, Right 0, Top 0, Bottom 260

Do NOT change bandeaux / star rows / CloseButton / TitleLabel. Keep RuntimeSaleChannelsScreen refs.

Save. Reply root Bottom. STOP.
```

### 3c — InventoryScreen

```
/prefab-ui-3phases
Task ID: [BZ-HUD-MODAL-SAFE-BOTTOM-001]
Prefab: Assets/Prefabs/Ui/InventoryScreen.prefab
Phase: 1
```

```
[BZ-HUD-MODAL-SAFE-BOTTOM-001] PHASE 3c ONLY — InventoryScreen root Bottom 260. STOP.

OPEN Assets/Prefabs/Ui/InventoryScreen.prefab. No .cs. No FirstLvl.

ROOT InventoryScreen:
- stretch (0,0)-(1,1), pivot 0.5/0.5
- Left 0, Right 0, Top 0, Bottom 260

Do NOT change ScrollView sizeDelta (-24, -108). Do NOT retouch tabs / Talent overlay.

Save. Reply root Bottom. STOP.
```

---

## Après Bezy (Cursor) — fait 2026-09-16

`UIManager.NavBarHeight` = **260f**. Sans ça, Instantiate écrasait le Bottom prefab à 128.
