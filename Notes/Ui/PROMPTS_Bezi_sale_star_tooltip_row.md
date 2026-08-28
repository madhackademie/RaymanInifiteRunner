# Prompts Bezy — UiStarRow sur tooltip ★ `[P0-SALE-STAR-TOOLTIP-ROW-001]`

**But :** remplacer les carrés Unicode `★` dans les titres TMP par des **UiStarRow** visuels (slot + fill).  
**Base ★0 :** départ sans étoile remplie (Cursor code déjà aligné).  
**Prefab :** `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` → `SaleChannelStarTooltip` uniquement.

**Prérequis Cursor (déjà fait) :** `SaleChannelStarTooltipHost` expose `currentStarRow` + `nextStarRow`. Compiler Unity avant Ph.2.

**Prefab réutilisé :** `Assets/Prefabs/Ui/Common/UiStarRow.prefab` (nested, ne pas unpack).

**Garder intact :** jauges SalesBar/ItemsBar/GoldBar, host refs existantes, unlock tooltip, bandeau.

**Succès Bezy = Save. List what changed. STOP. Pas de Simulate.**  
**Ordre :** Ph.1 → Ph.2.

---

## Phase 1 — Title rows + nested UiStarRow

```
[P0-SALE-STAR-TOOLTIP-ROW-001] Phase 1 ONLY — tooltip title star rows. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT edit SaleChannelBandeauView or unlock tooltip.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Scope: SaleChannelStarTooltip / CurrentBlock / NextBlock ONLY.

A) CurrentBlock — wrap CurrentTitle:
1) Create empty GO CurrentTitleRow (RectTransform + HorizontalLayoutGroup).
   - spacing 8, childAlignment MiddleLeft
   - childControlWidth/Height false, childForceExpand false
2) Nest ONE UiStarRow prefab instance under CurrentTitleRow. Name: CurrentStarRow
   - localScale 0.65, 0.65, 0.65
   - anchors middle-left, pivot 0,0.5
3) Move EXISTING CurrentTitle TMP under CurrentTitleRow (after CurrentStarRow).
4) CurrentTitle TMP placeholder text: "Palier actuel" (NO ★ unicode character).

B) NextBlock — wrap NextTitle (same recipe):
1) Create NextTitleRow + HorizontalLayoutGroup (same settings).
2) Nest UiStarRow named NextStarRow (scale 0.65).
3) Move EXISTING NextTitle TMP under NextTitleRow.
4) NextTitle placeholder: "Palier suivant" (NO ★ character).

C) Keep CurrentBody, NextBody, SalesBar, ItemsBar, GoldBar unchanged.
D) m_Layer: 5. Tooltip stays INACTIVE.

Done = Save. List CurrentTitleRow + NextTitleRow hierarchy. STOP.
```

**Checklist review Ph.1 (Cursor)** — **OK Bezy 2026-08-28**
- [x] CurrentStarRow + NextStarRow nested UiStarRow (scale 0.65)
- [x] TMP sans caractère ★
- [x] Jauges intactes

---

## Phase 2 — Wire host star rows

```
[P0-SALE-STAR-TOOLTIP-ROW-001] Phase 2 ONLY — wire tooltip star rows. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. No Simulate. No unpack.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

On SaleChannelStarTooltipHost (already on SaleChannelStarTooltip):
- currentStarRow → CurrentBlock/CurrentTitleRow/CurrentStarRow UiStarRowView
- nextStarRow → NextBlock/NextTitleRow/NextStarRow UiStarRowView
Keep ALL existing host refs (panelRoot, titles, bodies, salesBar, itemsBar, goldBar, panelRect, panelCanvasGroup).

Do not change bar icons or Track/Fill/Label.

Done = Save. Confirm currentStarRow + nextStarRow assigned. STOP.
```

**Checklist review Ph.2 (Cursor)** — **OK Bezy 2026-08-28**
- [x] Host câblé (`currentStarRow` + `nextStarRow`)
- [ ] Playtest hover ★ : 0 étoile or sur bandeau + tooltip, titres sans carrés

---

## Playtest auteur

1. Bandeau débloqué : **0 étoile or**, 5 slots bruns  
2. Tooltip : CurrentStarRow 0 fill, NextStarRow 1 fill (preview ★1)  
3. Plus de carrés Unicode dans les titres
