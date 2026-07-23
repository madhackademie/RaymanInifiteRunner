# Prompts Bezy — Polish UX ShopItemPopup `[CT-SHOP-002]`

Prefab : `Assets/Prefabs/Ui/ShopItemPopup.prefab`  
Script : `Assets/Scripts/UI/Shop/ShopItemPopupView.cs` (ne pas modifier)  
Anims : `Assets/Animations/UI/ShopItemPopup*.anim` + controller  
**Layers :** UI = `m_Layer: 5` — `@Notes/Ui/CONVENTION_layers_unity.md`

**Contrôle QA :** `Notes/PLAYTEST_shop_polish_ct002.md` (Batch D)

---

## Historique

- **Phase 1** : layers UI + contraste boutons — **OK** (Bezy, 2026-07-23)
- **Phase 2** : lisibilité textes / confirm overlay — **OK** (Bezy, 2026-07-23)
- **Phase 3** : polish transitions Open/Close — **OK** (Bezy, 2026-07-23)

---

## Phase 1 — OK

- 47/47 objets `m_Layer: 5`
- CTA verts ; Cancel/Close secondaires ; +/−/Max distincts

---

## Phase 2 — OK

- HeaderTitle/ItemName 28 ; ConfirmTotal/Message 22
- ConfirmOverlay inactif ; backdrop alpha 0.6

---

## Phase 3 — OK

Validé repo (Bezy 2026-07-23) :
- Open 0.25 s : slide Y + scale 0.94 → 1.02 → 1.0 (`Root/Card`)
- Close 0.2 s : slide Y + scale 1 → 0.96
- Pas d’anim alpha CanvasGroup carte ; bool `IsOpen` intact
- `ShopItemPopupView.animator` + `canvasGroup` câblés
- Mineur restant : `QuantityText` fontSize **18** (cible ≥ 22) — optionnel Batch D
- Note : `backdropImage` SerializeField encore None (fallback script `FindBackdropImage` OK)

---

## Anti-patterns

- Pas de fade CanvasGroup sur toute la carte
- Pas de nouveaux scripts / rebuild prefab from scratch
