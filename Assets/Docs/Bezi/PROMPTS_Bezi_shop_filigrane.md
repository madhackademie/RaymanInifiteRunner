# Prompts Bezy — Filigrane étal shop `[BZ-SHOP-FILIGRANE-001]`

**Prefab :** `Assets/Prefabs/Ui/ShopScreen.prefab` (root `ShopScreen`)  
**Sprite (déjà promu, même que l’onglet HUD) :** `Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Shop_glyph.png`  
**Layers :** UI = `m_Layer: 5`  
**Ne pas rescanner tout le projet.** Pas de C#. Ne pas régénérer le sprite.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

**Contexte :** P1 YAML **déjà là** (`ShopFiligrane` sibling 0, α 0.30).  
Le voile playtest = **Image sur `SlotsGrid`** (pas un GO séparé). Runtime `HudModalBackdrop.ApplyContentPanel` remet α **0.995**. Bezy bloqué s’il cherche un enfant « Backdrop ».

**Hors scope :** `RuntimeShopScreen.cs`, `NavigationHUD`, `ShopItemPopup`, C#, `InventoryScreen`.

**Prérequis Editor :** ouvrir `ShopScreen.prefab` en Prefab Mode avant l’appel.

---

## Phase 1 — Image filigrane 30% — OK YAML (archive)

`ShopFiligrane` créé, sprite onglet, stretch, α 0.30. `SlotsGrid` Image α 0 **dans le prefab** — écrasé au Play.

---

## Phase 2 — Couper le voile SlotsGrid (COPIER TEL QUEL)

```
[BZ-SHOP-FILIGRANE-001] Phase 2 ONLY — disable SlotsGrid Image veil. HINT below. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate sprites.
Do not rename ShopScreen, ShopFiligrane, SlotsGrid, EmptyCataloguePanel.
Do not change RectTransforms. Do not delete SlotsGrid GameObject.
Do not move ShopFiligrane above SlotsGrid (would cover shop slots).

HINT — Bezy was stuck looking for a Backdrop child. THERE IS NONE.
The veil is the Image COMPONENT on the SAME GameObject named SlotsGrid
(also has GridLayoutGroup + ContentSizeFitter). Runtime C# paints that
Image Color alpha 0.995 on Play. Alpha 0 in prefab is overwritten.
Fix = disable that Image component so it cannot draw.

File ONLY:
- Assets/Prefabs/Ui/ShopScreen.prefab

REQUIRED:
1) Keep ShopFiligrane as-is (sibling 0, sprite, alpha 0.30, raycast off).
2) On SlotsGrid: set the Image component m_Enabled = 0 (checkbox off).
   Keep GridLayoutGroup enabled. Keep SlotsGrid GameObject active.
3) Do NOT disable CanvasRenderer if Unity needs it for grid children.
4) Keep RuntimeShopScreen refs (slotsContainer, contentBackdropImage can stay wired).
5) Save.

Interdits: no C#; no Simulate; no new GO; do not parent ShopFiligrane under SlotsGrid.

Done = Save. List SlotsGrid Image enabled=false + ShopFiligrane sibling index. STOP.
```

**Chars ~1550** — OK &lt; 3500.

### Checklist review Phase 2 (Cursor)

- [ ] `SlotsGrid` Image `m_Enabled: 0` ; GridLayout toujours on
- [ ] `ShopFiligrane` inchangé (sibling 0, α 0.30)
- [ ] Pas de GO Backdrop inventé ; `SlotsGrid` non détruit

### Après Bezy (auteur)

1. Playtest : **Stop puis Play**. Filigrane visible derrière les slots ; Home ne traverse pas (root Image reste).
2. Tweak live : `ShopScreen(Clone)/ShopFiligrane` Color A.

---

## Lancement Unity

Prefab Mode : `Assets/Prefabs/Ui/ShopScreen.prefab`

```
/prefab-ui-3phases
Task ID: [BZ-SHOP-FILIGRANE-001]
Prefab: Assets/Prefabs/Ui/ShopScreen.prefab
Phase: 2
```

Puis coller le bloc Phase 2, **ou** :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_shop_filigrane.md
[BZ-SHOP-FILIGRANE-001] Execute PHASE 2 only. Prefab Mode ShopScreen.prefab. Disable SlotsGrid Image. No .cs. Save. List. STOP.
```
