# [BZ-UIKIT-POPUP-MOCK-001] Mockup chrome bois — popup item (copie)

**Idée :** `Card` = panneau 9-slice. `IconFrame` = cadre troué **autour de l’icône seulement**. Qty = bandeau liste. CTA = pill orange. Croix inchangée (coin Card).

**Prefab cible :** `Assets/Prefabs/Ui/ShopItemPopup_WoodMockup.prefab`  
(dupliquer `ShopItemPopup.prefab` — **ne pas** modifier l’original ni les bindings `UIManager`).

**Sprites :** `Assets/Art/Sprites/UI/UiKit_*_20260919.png`  
**Hors scope :** C#, Simulate, Play Mode, `ConfirmOverlay`, `DropTrashRoot`, scripts view.

Succès = Save + liste. STOP.

---

## Phase 1 — Dupliquer + taille Card (ONLY)

```
[BZ-UIKIT-POPUP-MOCK-001] PHASE 1 ONLY. Duplicate + Card size/padding. STOP.

Do not rescan whole project. No C#. Layer 5. Do NOT edit ShopItemPopup.prefab.

1) If Assets/Prefabs/Ui/ShopItemPopup_WoodMockup.prefab missing:
   Duplicate ShopItemPopup.prefab in the same folder. Rename copy to ShopItemPopup_WoodMockup.
2) Open ONLY the copy in Prefab Mode.
3) Card RectTransform: keep center pivot; SizeDelta = 480 x 760; AnchoredPosition y = 20.
4) Card VerticalLayoutGroup padding L=48 R=48 T=56 B=48. Do not remove Header/Body/Footer/CloseButton.
5) Do NOT change sprites, Image Type, or serialized ShopItemPopupView refs.

Save copy. List Card size + padding. STOP. No Play Mode.
```

---

## Phase 2 — Sprites + Image Type (ONLY)

**Retry 2026-09-19 :** P2 précédente = **noop** (tous `m_Sprite: {fileID: 0}`). Relancer ce bloc, prefab **WoodMockup** ouvert.

```
[BZ-UIKIT-POPUP-MOCK-001] PHASE 2 RETRY. Sprites on WoodMockup ONLY. STOP.

OPEN Prefab Mode: Assets/Prefabs/Ui/ShopItemPopup_WoodMockup.prefab
Do not rescan project. No C#. Do not touch ShopItemPopup.prefab. Layer 5.
Image color ALL = (1,1,1,1). Image Type Sliced = 1, Simple = 0.

Assign these PNG (Project search exact name):

1) Card Image
   sprite Assets/Art/Sprites/UI/UiKit_Panel_9s_20260919.png
   guid c4a91e7b2d6f48b09e3c1a5d8f0b2467
   Type Sliced (1). Fill Center ON.

2) IconFrame Image
   sprite Assets/Art/Sprites/UI/UiKit_Frame_icon_20260919.png
   guid a8e3521f610382f4d2705e91c34f680b
   Type Simple (0). Preserve Aspect ON. Keep children (item icon).

3) QuantityRow — ADD CanvasRenderer + Image if missing
   sprite Assets/Art/Sprites/UI/UiKit_Row_list_20260919.png
   guid caf574318325a4b6f49270b3e561802d
   Type Sliced (1). Keep HorizontalLayoutGroup.

4) ConfirmButton Image
   sprite Assets/Art/Sprites/UI/UiKit_BtnPrimary_orange_20260919.png
   guid d5b02f8c3e7059c1af4d2b6e901c3578
   Type Sliced (1).

5) Header Image
   sprite Assets/Art/Sprites/UI/UiKit_Bar_rope_20260919.png
   guid f7d2410e509271e3c16f4d80b23e579a
   Type Sliced (1).

FORBIDDEN: Frame sprite on Card. Oval sprite. Original ShopItemPopup.

Save prefab to disk. List each GO: sprite name + Type. STOP. No Play Mode.
```

---

## Phase 3 — Layout tailles (ONLY)

```
[BZ-UIKIT-POPUP-MOCK-001] PHASE 3 ONLY. P2 OK. Sizes on COPY. STOP.

File ONLY: Assets/Prefabs/Ui/ShopItemPopup_WoodMockup.prefab
No C#. Keep same GameObjects so View refs stay valid.

1) IconFrame LayoutElement: Preferred/Min Width+Height = 160. Flexible 0.
2) ConfirmButton LayoutElement: Preferred Height = 112, Min Height = 112. Flexible Height 0.
   Width stretch OK. Do not scale Y wildly (pill).
3) QuantityRow LayoutElement Preferred Height = 72.
4) Header LayoutElement Preferred Height = 64 if present.
5) CloseButton stays last sibling under Card, Ignore Layout, anchors top-right (-8,-8), 56x56.
6) TMP on parchment: HeaderTitle / item name readable dark brown, not white-on-white.

Save. List sizes. STOP. No Play Mode. No UIManager binding.
```
