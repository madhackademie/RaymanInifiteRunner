# [BZ-INV-WALLET-NAV-BAND-001] Wallet inventaire → bandeau nav

**Prefab :** `Assets/Prefabs/Ui/InventoryScreen.prefab` (Prefab Mode obligatoire)  
**P1 :** CLOS 2026-09-16 (Y -190, trop bas + derrière les onglets — HUD `NavBarContainer` est dernier sibling canvas 50).  
**P2 :** remonter + Canvas override (devant la nav).  
**Interdit :** `NavigationHUD.unity` (ne pas réordonner HUDRoot), `.cs`, root Bottom **260**, reparent HUD.

---

## Phase 1 — archive (livrée, ne pas relancer)

Y=-190 trop bas ; chip derrière TabVente/Plus.

---

## Phase 2 — remonter + devant la nav

```
[BZ-INV-WALLET-NAV-BAND-002] Raise wallet + Canvas in front of nav. PREFAB ONLY. STOP.

OPEN Assets/Prefabs/Ui/InventoryScreen.prefab in Prefab Mode. UI m_Layer=5. Do NOT rescan. No .cs. No NavigationHUD.unity. No new sprites. Do NOT reparent to HUD.

CAUSE: WalletWidget Y=-190 sits on idle tab icons. HUDRoot draws NavBarContainer AFTER screenRoot (same Canvas sorting 50) so tabs paint OVER the chip.

KEEP: child of WalletBar. Root Bottom 260. WalletBar height 0 / Image alpha 0. ScrollView sizeDelta.y -16. ExpandedPanel grows UP. WalletWidget.cs refs.

1) WalletWidget RectTransform:
   - Anchors min (1,0) max (1,0). Pivot (1, 0.5).
   - sizeDelta (400, 80).
   - anchoredPosition (-16, -40).  (auteur 2026-09-16 — was -100 / -190)

2) WalletWidget ADD components (same GameObject, after existing WalletWidget script):
   - Canvas: Render Mode Screen Space Overlay. overrideSorting ON. sortingOrder = 60 (HUD canvas is 50).
   - GraphicRaycaster: default (Ignore Reversed Graphics ON). Needed for ≡ button.

3) Do NOT add RectMask2D. Do NOT change Header / Halo / TalentTree / NavBar.

Save prefab. Reply: WalletWidget anchoredPosition.y, Canvas overrideSorting + sortingOrder. STOP. No Play Mode / Simulate.
```

**Lancement Unity :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_inventory_wallet_nav_band.md
[BZ-INV-WALLET-NAV-BAND-002] Raise wallet + Canvas in front of nav. PREFAB ONLY. STOP.
```

**Playtest :** chip plus haut que les glyphes idle Vente/Plus ; timbres + 275 **devant** les onglets ; ≡ cliquable ; Shop/Hub sans wallet.
