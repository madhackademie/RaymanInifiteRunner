# Prompts Bezy — étoiles bandeaux vente `[P0-SALE-STAR-UI-001]`

**Tâche :** polish 3 bandeaux (même prefab) — ★ pleines/vides + tooltip hover rangée ★ (palier courant / suivant).  
**Branche :** `feature/sale-bandeaux`  
**GDD :** `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §2.9  
**Spec :** `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`  
**Règle :** un seul prefab `SaleChannelBandeauView` — ne pas forker le vélo. `[BL-SALE-BANDEAU-TPL-001]`

**Succès Bezy = Save. List what changed. STOP. Pas de Simulate / Play Mode.**

Scripts Cursor (ne pas modifier) :
- `SaleChannelStarHover.cs`
- `SaleChannelStarTooltipHost.cs`
- `SaleChannelBandeauView.cs` (`starImages[5]` déjà câblé)
- `RuntimeSaleChannelsScreen.cs`

**Ne pas toucher :** `LockedOverlay`, `SaleChannelUnlockTooltip`, `UnlockableFxAnchor`, `CooldownOverlay`.

**Prérequis Editor :** ouvrir le prefab ciblé en mode Prefab avant d’envoyer.  
**Ordre :** Ph.1 → attendre succès → Ph.2 → Ph.3.

---

## Phase 1 — Shell tooltip étoiles (hiérarchie seule)

```
[P0-SALE-STAR-UI-001] Phase 1 ONLY — star tooltip shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate prefabs.
Do NOT edit SaleChannelUnlockTooltip, LockedOverlay, CooldownOverlay, UnlockableFxAnchor.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Context:
- Root SaleChannelsScreen children today: Header, Body, SaleChannelUnlockTooltip.
- Add a NEW dedicated tooltip (unlock tooltip stays for padlock hover).

REQUIRED: last sibling under SaleChannelsScreen root (after SaleChannelUnlockTooltip):

SaleChannelStarTooltip (INACTIVE m_IsActive: 0)
├── CurrentBlock
│   ├── CurrentTitle
│   └── CurrentBody
└── NextBlock
    ├── NextTitle
    └── NextBody

Rules:
- Empty RectTransforms only this phase. No Image, TMP, Layout, CanvasGroup, Animator yet.
- m_Layer: 5 on SaleChannelStarTooltip and ALL children.
- Do not clone/rename SaleChannelUnlockTooltip.
- Keep Header, Body, unlock tooltip wiring intact.

Interdits: no C#; no Simulate; no new scripts; no bandeau prefab edits this phase.

Done = Save prefab. List GO names created + parent path. STOP.
```

**Checklist review Ph.1 (Cursor)** — **OK Bezy 2026-08-25**
- [x] `SaleChannelStarTooltip` inactif, dernier sibling racine (après unlock tooltip)
- [x] CurrentBlock / NextBlock + 4 labels vides, layer 5, RectTransform only
- [x] `SaleChannelUnlockTooltip` + host inchangés ; bandeau non touché

---

## Phase 2 — Visuels étoiles + tooltip (après Ph.1 OK)

```
[P0-SALE-STAR-UI-001] Phase 2 ONLY — star visuals + tooltip panel. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not add scripts this phase.

Files ONLY:
- Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

A) Bandeau — HeaderRow/Stars (existing Star1..Star5):
- Star1 color RGB 0.95,0.35,0.55 (filled pink). Star2-5 RGB 0.45,0.45,0.48 (empty).
- Keep size 24x24. Placeholder Image OK (no new art).
- Star1-5: raycastTarget OFF.
- On Stars GO: add Image, color white alpha 0.01, raycastTarget ON (hover hit). Keep HorizontalLayoutGroup.
- Do NOT add extra layout children under Stars.

B) SaleChannelStarTooltip (Ph.1 GOs):
- Root: Image dark panel alpha ~0.94, raycast OFF, width ~320, pivot bottom-center.
- CanvasGroup alpha 1, blocksRaycasts OFF, interactable OFF.
- VerticalLayoutGroup + ContentSizeFitter preferred on root.
- CurrentTitle / NextTitle: TMP bold ~18, white.
- CurrentBody / NextBody: TMP regular ~15, wrap.
- Placeholders: CurrentTitle "★1 — Palier actuel" / CurrentBody "1 voisin · 1-3 salades".
  NextTitle "★2 — Palier suivant" / NextBody "Conditions + recompense TBD".
- NextBlock: thin top separator OK. All labels raycast OFF.
- m_Layer: 5. Stay INACTIVE.

Do NOT edit unlock tooltip or LockedOverlay. No Animator. No Simulate.

Done = Save both prefabs. List components added. STOP.
```

**Checklist review Ph.2 (Cursor)** — **OK Bezy 2026-08-25** (YAML, pas le capture plein écran)
- [x] ★1 rose 0.95/0.35/0.55, ★2–5 grises, 24², raycast OFF ; `Stars` Image α 0.01 raycast ON + HLG ; 5 enfants only
- [x] Tooltip inactif, layer 5, Image dark α 0.94, width 320, pivot bas, CanvasGroup blocksRaycasts OFF, VLG + CSF preferred
- [x] 4 TMP placeholders + bodies wrap ; cadenas / unlock host intacts
- [ ] **Nit Ph.3 :** 4 TMP `fontAsset` vide + `isOrthographic=0` (texte UI illisible tant que non copié depuis unlock TitleLabel)

---

## Phase 3 — Wiring Inspector (après Ph.2 OK)

Scripts déjà dans le projet (ne pas recréer) :
- `SaleChannelStarHover` → GO `Stars`
- `SaleChannelStarTooltipHost` → GO `SaleChannelStarTooltip`

```
[P0-SALE-STAR-UI-001] Phase 3 ONLY — wire star hover + tooltip host. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. Do not add OnClick UnityEvents.

Files ONLY:
- Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

1) On Stars (bandeau prefab): add SaleChannelStarHover. Leave bandeauView and tooltipHost empty (runtime resolves).

2) On SaleChannelStarTooltip: add SaleChannelStarTooltipHost. Wire:
- panelRoot → SaleChannelStarTooltip
- nextBlockRoot → NextBlock
- currentTitleLabel → CurrentTitle TMP
- currentBodyLabel → CurrentBody TMP
- nextTitleLabel → NextTitle TMP
- nextBodyLabel → NextBody TMP
- panelRect → SaleChannelStarTooltip RectTransform
- panelCanvasGroup → CanvasGroup on root

3) On SaleChannelsScreen root RuntimeSaleChannelsScreen:
- starTooltipHost → SaleChannelStarTooltipHost

4) Confirm SaleChannelBandeauView.starImages still Star1..Star5 in order.
5) Keep SaleChannelUnlockTooltipHost + LockedOverlay hover unchanged.
6) m_Layer: 5 unchanged.

7) TMP fix on CurrentTitle, CurrentBody, NextTitle, NextBody:
- Font Asset + material = same as SaleChannelUnlockTooltip TitleLabel (LiberationSans SDF already on this prefab).
- Extra Settings: Orthographic ON (UI). Keep bodies wrapping.

Done = Save both. Confirm all refs assigned + 4 TMP fonts set. STOP.
```

**Checklist review Ph.3 (Cursor)** — **OK Bezy 2026-08-25**
- [x] `SaleChannelStarHover` sur `Stars` (refs vides, resolve runtime)
- [x] Host tooltip 8 champs câblés + `starTooltipHost` écran
- [x] `starImages[5]` intact, `SaleChannelBandeauProgressionHover` cadenas intact
- [x] 4 TMP LiberationSans + `isOrthographic=1`

---

## Phase 4 — Tooltip sous les étoiles (après Ph.3 OK)

Playtest : panneau trop haut, coupé par le bord écran. Cursor a déjà un **clamp canvas**. Bezy = placement préféré **sous** la rangée ★.

```
[P0-SALE-STAR-UI-001] Phase 4 ONLY — star tooltip below stars, stay on screen. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate prefabs. No Simulate.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

On SaleChannelStarTooltip ONLY (not SaleChannelUnlockTooltip):

1) RectTransform pivot = (0.5, 1) — top-center so the panel hangs DOWN from the star row.
2) On SaleChannelStarTooltipHost:
   - screenOffset = (0, -16)
   - canvasEdgePadding stays 12 if the field exists (do not delete it)
3) Keep width 320, Image, CanvasGroup, layout, TMP, host refs. Stay INACTIVE.
4) m_Layer: 5. Do not edit LockedOverlay, bandeau prefab, or unlock tooltip.

Done = Save. List pivot + screenOffset. STOP.
```

**Checklist review Ph.4 (Cursor)** — **OK Bezy 2026-08-25**
- [x] Pivot `SaleChannelStarTooltip` = (0.5, 1)
- [x] `screenOffset` (0, -16), `canvasEdgePadding` 12
- [x] Unlock tooltip inchangé (pivot bas, offset +24)

**Pas de Ph.5 Bezy** — tooltip palier ★ sur canal **débloqué** (vente + cooldown). Overlay cadenas = tooltip déblocage. Cursor : `AllowsStarTooltip`.

**Suite jauges (2026-08-25) :** `[P0-SALE-STAR-BARS-001]` — `Notes/Ui/PROMPTS_Bezi_sale_channel_star_bars.md` (Ph.1 maintenant).
