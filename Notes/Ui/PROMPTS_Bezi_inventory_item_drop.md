# Prompts Bezy — Inventaire détail item / drop `[P0-INV-DROP-001]`

**Ownership :** Bezy = prefab / Animator / bindings. Cursor = scripts (hooks déjà livrés).

**Ne pas rescanner tout le projet.** Réutiliser scripts + prefab existants.

| Élément | Chemin |
|--------|--------|
| Prefab popup | `Assets/Prefabs/Ui/ShopItemPopup.prefab` |
| Anims open/close | `Assets/Animations/UI/ShopItemPopup_Open.anim` / `_Close.anim` + controller |
| Scène bindings | `Assets/Scenes/NavigationHUD.unity` → `UIManager.runtimePopupBindings` |
| Scripts (ne pas modifier) | `ShopItemPopupView`, `ShopItemPopupController`, `InventoryUI`, `PopupId` |
| Layers | UI = `m_Layer: 5` |

**Ordre Bezy (séquentiel) :**
1. Binding — **OK**
2. Description lisible — **OK**
3. Anim ouverture (`IsOpen`) — **OK**
4. Anim drop compost / biomule (trigger script `DropToTrash`) — **OK**
4b. Retune motion : compost plus haut + item qui tombe vers compost — **OK** (Bezy, 2026-07-28)
4c. Retune : compost **+100 px** + FlyingIcon qui **atterrit sur** le compost — **prochaine Bezy**
5. *(optionnel)* Prefab dédié inventaire — skip OK (réutilise shop)

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

### Historique

- **Phase 1** : binding — **OK** (Bezy, 2026-07-28)
- **Phase 2** : description — **OK** (noop)
- **Phase 3** : open anim — **OK** (noop shop)
- **Phase 4** : drop compost biomule — **OK** (Bezy, 2026-07-28)
  - `DropTrashRoot` (inactive) + `TrashBin` + `FlyingIcon`, layer 5
  - Animator `ShopItemPopupDropTrash` + trigger `DropToTrash` + clip `ShopItemPopup_DropToTrash`
  - View wired : `dropTrashRoot` / `dropFlyingIcon` / `dropTrashAnimator` / duration `0.65`
  - Art : `Assets/Art/Sprites/UI/Inventory/DropCompost/CompostDrop.png`
- **Phase 4b** : retune anim — **OK** (Bezy 2026-07-28)
  - TrashBin `anchoredPosition.y` ≈ **110** (était ~-20)
  - FlyingIcon chute y **160 → ~100** (encore trop court — voir 4c)
- **Phase 4c** : compost **+100** (~210) + item atterrit **sur** le compost — **prochaine**
- Phase 5 : prefab dédié — optionnel (non requis)

### Direction visuelle Phase 4 (art auteur)

Remplacer l’idée « poubelle » par **compost biomule** :
- Item volant → tombe / est avalé dans un **tas / bac compost** (biomule)
- Art sheet : `Assets/Art/Sprites/UI/Inventory/DropCompost/CompostDrop.png` (13 frames : `_00`…`_11` + `_Idle`)
- Source dump : `Assets/Art/Assets Store Dump/compostAnim.png`
- Bind menu : `Rayman/UI/Bind Compost Drop Sprites` (`CompostDropSpriteBinder`)
- Hooks scripts : `dropTrashRoot` / `dropFlyingIcon` / trigger **`DropToTrash`**
- TrashBin idle sprite + flipbook dans `ShopItemPopup_DropToTrash.anim` ; durée `0.85s`

**Brief ChatGPT (art) — à coller :**
> Game UI sprite sheet for a cute aquaponics farm mobile game. Style: soft flat 2D, readable at small size. Subject: a small compost biomule / compost pile with earthworms (same vibe as our farm worm particles). Frames for a short UI animation (~6–8 frames): idle compost mound, item dropping in, worms wiggling, mound settling. Transparent PNG, centered, no text, consistent outline. Also deliver a simple static compost bin/mound icon and a generic “flying item” placeholder circle if needed. Colors: earth browns, soft greens, warm compost tones — not a grey metal trash can.

---

## Phase 1 — Binding popup — OK

Vérifié : `NavigationHUD` → `screenId=Inventory`, `popupId=inventory.item.detail`, prefab `ShopItemPopup`.

---

## Phase 2 — Description lisible — OK (rien à changer)

Review Cursor : `Description` actif, `fontSize: 18`, `descriptionText` câblé, `ConfirmOverlay` inactif.

---

## Phase 3 — Anim ouverture panel — OK (rien à changer)

Review Cursor : param `IsOpen`, clips Open/Close, `ShopItemPopupView.animator` câblé. Déjà livré polish shop.

---

## Phase 4 — Anim drop compost biomule — OK

Review Cursor (2026-07-28) :
- `DropTrashRoot` inactive, children `TrashBin` + `FlyingIcon`
- Controller `ShopItemPopupDropTrash` : Idle + trigger `DropToTrash`
- Clip `ShopItemPopup_DropToTrash.anim`
- `ShopItemPopupView` fields wired, duration 0.65

---

## Phase 4c — Compost +100 px + item atterrit dessus — PROCHAINE

```
[P0-INV-DROP-001] Phase 4c ONLY — raise compost +100px and make FlyingIcon land ON it. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts.
Do not rename DropTrashRoot / TrashBin / FlyingIcon / trigger DropToTrash.
Prefab: Assets/Prefabs/Ui/ShopItemPopup.prefab
Clip: Assets/Animations/UI/ShopItemPopup_DropToTrash.anim
Keep CompostDrop flipbook keys on TrashBin. Keep dropTrashDuration ~0.85.

Current problem:
- Compost still too low
- FlyingIcon does not fall far enough / does not visibly land on compost

REQUIRED changes:

1) TrashBin (prefab):
- Raise anchoredPosition.y by +100 from current (~110 → ~210).
- Keep anchor bottom-center, size ~180x140, Preserve Aspect, layer 5.

2) FlyingIcon clip keys (DropToTrash):
- Start HIGH above compost (e.g. anchoredPosition.y ~320..380, x=0).
- End ON the compost mound top — not below TrashBin.
  Target end Y ≈ TrashBin.y + TrashBin.height*0.55..0.75
  With TrashBin.y≈210 and height≈140 → end Y ≈ 280..315.
- Mid key optional (~0.45s) for a clear downward fall.
- Scale: 1 → ~0.35 at impact.
- Alpha: opaque until last ~0.12s, then fade to 0 as it sinks into compost.
- x stays ~0 (centered on compost).

3) Timing:
- Item should reach compost around 0.55–0.65s.
- Optional TrashBin squash when item arrives.
- Clip length stays ~0.85s.

4) Verify visually in Editor Animation window (no Play Mode required):
- Path goes clearly DOWN into the mound.
- Final FlyingIcon position overlaps TrashBin top, never ends under TrashBin pivot.

Save prefab + anim. List final TrashBin Y and FlyingIcon start/end Y. STOP.
```

---

## Phase 4b — Retune drop : compost plus haut + item tombe vers compost — OK

Review Cursor (2026-07-28) :
- TrashBin Y **110**, size 180×140
- FlyingIcon start Y **100** (prefab) / curve start **160** → tombe vers compost
- Clip ~0.85s + squash TrashBin + flipbook conservé

---

## Phase 5 — (optionnel) Prefab dédié inventaire

Skip si le look shop convient. Sinon :

```
[P0-INV-DROP-001] Phase 5 OPTIONAL — InventoryItemPopup duplicate. Wait success. STOP after save.

1) Duplicate ShopItemPopup → Assets/Prefabs/Ui/InventoryItemPopup.prefab
2) Keep ShopItemPopupController + View + DropTrash wiring
3) Update binding popupPrefab → InventoryItemPopup (Inventory / inventory.item.detail)

Save. List files. STOP.
```

---

## Checklist auteur (après Bezy)

- [x] Binding Inventory + `inventory.item.detail`
- [x] Description lisible dans le panel (déjà OK prefab)
- [x] Ouverture : anim `IsOpen` (déjà OK shop)
- [x] Confirm drop → FlyingIcon → compost (`DropToTrash`) câblé Bezy
- [ ] Playtest auteur : Cancel confirm + drop réel + descriptions items
- [ ] Remplir `ItemDefinition.description` sur les assets (si vides)

## Anti-patterns

- Pas d’instance popup hors `ScreenPopupHost`
- Ne pas renommer `IsOpen` / `DropToTrash`
- Pas de Simulate comme succès Bezy
