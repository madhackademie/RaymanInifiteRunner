# [BZ-INV-DROP-CLOSE-POS-001] Croix popup drop — coin haut-droit de la Card

**Pourquoi :** `CloseButton` est enfant de `Root` (stretch plein écran), anchors **bas-centre**, `anchoredPosition y = -44`. La croix tombe dans la bande nav. Cible : coin **haut-droit de `Card`** (bandeau « Gold »).

**Prefab :** `Assets/Prefabs/Ui/ShopItemPopup.prefab` (Prefab Mode). Même popup shop / vente / drop — un seul move.

**Hors scope :** C#, Simulate, Play Mode, `ConfirmOverlay`, `DropTrashRoot`, anims, labels, couleurs.

Succès = Save + liste. STOP.

---

## Phase 1 — Reparent + anchors (ONLY)

```
[BZ-INV-DROP-CLOSE-POS-001] Phase 1 ONLY — move CloseButton to Card top-right. STOP.

Do not rescan whole project. Do not modify C#. Do not rename GameObjects.
Do not delete CloseButton. Do not recreate it (keep same GO so ShopItemPopupView.closeButton stays wired).
File ONLY: Assets/Prefabs/Ui/ShopItemPopup.prefab
Open that prefab in Prefab Mode. Layer 5.

CURRENT BUG:
- CloseButton is child of Root
- AnchorMin/Max = (0.5, 0) bottom-center
- Pivot = (0.5, 0)
- AnchoredPosition = (0, -44)
- SizeDelta = 72 x 72
→ sits in the HUD nav strip, not on the popup.

REQUIRED:
1) Reparent CloseButton under Card (keep child CloseLabel).
   Card has VerticalLayoutGroup — CloseButton MUST ignore layout:
   add LayoutElement if missing, set Ignore Layout = true.
2) Set CloseButton as LAST sibling under Card (draw on top of Header).
3) RectTransform CloseButton:
   - AnchorMin = (1, 1)
   - AnchorMax = (1, 1)
   - Pivot = (1, 1)
   - AnchoredPosition = (-8, -8)
   - SizeDelta = 56 x 56
   - LocalScale = (1,1,1)
   - Rotation identity
4) Keep Image, Button, CloseLabel (stretch fill parent). Keep CloseLabel text "x".
5) Do NOT touch Card size (420x700), Header, Footer, ConfirmOverlay, DropTrashRoot, Backdrop.
6) Do NOT change ShopItemPopupView serialized refs except they must still point to the same CloseButton.

Save prefab. List parent, anchors, pivot, pos, size, Ignore Layout. STOP. No Play Mode.
```
