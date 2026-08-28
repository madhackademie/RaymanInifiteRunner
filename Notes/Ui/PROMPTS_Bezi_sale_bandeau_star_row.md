# Prompts Bezy — UiStarRow sur bandeaux vente `[P0-SALE-STAR-ROW-001]`

**But :** remplacer les 5 Images carrées (`Star1`…`Star5`) par **1 nested** `UiStarRow` (5 `UiStarSlot`).  
**Prefab :** `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab` uniquement (1 template → 3 instances).  
**Règle :** `[BL-SALE-BANDEAU-TPL-001]` — ne pas forker le vélo.

**Prérequis Cursor (déjà fait) :** `SaleChannelBandeauView` expose `starRow` (`UiStarRowView`) à la place de `starImages[]`.  
Compiler Unity avant Ph.2.

**Prefabs réutilisés (ne pas recréer) :**
- `Assets/Prefabs/Ui/Common/UiStarRow.prefab`
- `Assets/Prefabs/Ui/Common/UiStarSlot.prefab`

**Garder intact :**
- `Stars` GO + `SaleChannelStarHover` + Image hit (α≈0.01, raycast)
- `LockedOverlay`, `CooldownOverlay`, `UnlockableFxAnchor`, tooltips écran

**Succès Bezy = Save. List what changed. STOP. Pas de Simulate.**  
**Credits Bezy :** reset **30** de chaque mois.  
**Ordre :** Ph.1 → succès → Ph.2.

**Taille :** anciens carrés 24×24 ; slots row = 32. Scale `StarRow` à **0.75** (≈24) pour coller au header.

---

## Phase 1 — Remplacer Star1–5 par nested UiStarRow

```
[P0-SALE-STAR-ROW-001] Phase 1 ONLY — nest UiStarRow under Stars. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT recreate UiStarRow / UiStarSlot. Do NOT edit SaleChannelsScreen.prefab.
Do NOT touch LockedOverlay, CooldownOverlay, UnlockableFxAnchor, illustration, button.

File ONLY:
- Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab

Scope: HeaderRow / Stars ONLY.

1) Under Stars: DELETE children Star1, Star2, Star3, Star4, Star5 (old square Images).

2) Nest ONE Prefab Instance of Assets/Prefabs/Ui/Common/UiStarRow.prefab
   as the ONLY child of Stars. Name it: StarRow
   (Unity nested prefab — do NOT unpack / do NOT copy hierarchy).

3) StarRow RectTransform:
   - anchors middle-left, pivot 0,0.5
   - localScale 0.75, 0.75, 0.75 (32px slots → ~24px like old squares)
   - anchoredPosition 0,0

4) Keep on Stars (do NOT remove):
   - HorizontalLayoutGroup (spacing 4 OK)
   - Image hit (near-transparent, raycastTarget true)
   - SaleChannelStarHover
   - sizeDelta ~140 x 32 (or widen to ~148 if clipped)

5) m_Layer: 5. Do not wire scripts this phase.

Done = Save. List Stars children (must be StarRow only) + StarRow scale. STOP.
```

**Checklist review Ph.1 (Cursor)** — **OK Bezy 2026-08-28**
- [x] Star1–5 supprimés
- [x] Nested `UiStarRow` nommé `StarRow`, scale 0.75
- [x] `SaleChannelStarHover` + Image hit intactes

---

## Phase 2 — Wire starRow sur SaleChannelBandeauView

```
[P0-SALE-STAR-ROW-001] Phase 2 ONLY — wire starRow on bandeau. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. No Simulate. No unpack.

File ONLY:
- Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab

On root SaleChannelBandeauView component:
- starRow → child Stars/StarRow UiStarRowView
- Clear / leave empty any obsolete starImages array if still visible (field removed in script — ignore if gone)
- Keep all other bindings (bandeauButton, titleLabel, lockedOverlay, cooldown*, illustrationImage)

Keep SaleChannelStarHover on Stars. Keep StarRow nested. m_Layer: 5.

Done = Save. Confirm starRow assigned to Stars/StarRow. STOP.
```

**Checklist review Ph.2 (Cursor)** — **OK Bezy 2026-08-28**
- [x] `starRow` → `Stars/StarRow`
- [ ] Hover ★ + tooltip toujours OK
- [ ] Playtest : `ApplyStarFill(1)` → 1 or remplie + 4 slots vides

---

## Playtest auteur

1. Écran ventes → bandeau Voisinage : 1 ★ or + 4 slots bruns  
2. Hover rangée ★ → tooltip jauges intact  
3. Canal verrouillé : 0 ★ remplies (`ApplyStarFill(0)`)
