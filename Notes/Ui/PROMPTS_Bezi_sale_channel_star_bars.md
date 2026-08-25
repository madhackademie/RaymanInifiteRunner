# Prompts Bezy — jauges tooltip ★ `[P0-SALE-STAR-BARS-001]`

**But :** 3 barres de progression **dans** le tooltip palier, texte **à l’intérieur** (`Ventes  2/5`).  
**Branche :** `feature/sale-bandeaux`  
**Prefab :** `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` uniquement.  
**Règle :** un seul bandeau prefab — **ne pas** éditer `SaleChannelBandeauView`. `[BL-SALE-BANDEAU-TPL-001]`

**Succès Bezy = Save. List what changed. STOP. Pas de Simulate / Play Mode.**

Scripts Cursor déjà dans le projet (ne pas modifier, ne pas recréer) :
- `SaleChannelStarProgressBarView.cs`
- `SaleChannelStarTooltipHost.cs`
- `SaleChannelStarHover.cs`

**Ne pas toucher :** `LockedOverlay`, `SaleChannelUnlockTooltip`, `UnlockableFxAnchor`, `CooldownOverlay`, bandeau.

**Prérequis Editor :** ouvrir `SaleChannelsScreen` en mode Prefab avant d’envoyer.  
**Ordre :** Ph.1 → succès → Ph.2 → Ph.3.  
**Crédits Bezy :** reset **30 août**.

---

## Phase 1 — Shell jauges (hiérarchie seule)

```
[P0-SALE-STAR-BARS-001] Phase 1 ONLY — star tooltip bar shells. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate prefabs. No Simulate.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Edit SaleChannelStarTooltip / NextBlock ONLY.
Current children: NextTitle, NextBody.

Add THREE siblings BETWEEN NextTitle and NextBody (NextBody stays last):

SalesBar
├── Track
├── Fill
└── Label
ItemsBar (same 3 children, same names)
GoldBar (same 3 children, same names)

Rules:
- Empty RectTransforms only. No Image, TMP, Layout, Slider, Animator.
- m_Layer: 5 on every new GO (UI=5; Water=4 — never 4).
- Exact names. Do not rename NextTitle / NextBody / CurrentBlock.
- Do NOT edit SaleChannelUnlockTooltip, LockedOverlay, bandeau prefab.

Interdits: no C#; no new scripts; no Simulate.

Done = Save prefab. List GO names + parent path. STOP.
```

**Checklist review Ph.1 (Cursor)** — **OK Bezy 2026-08-25**
- [x] 3 barres sous NextBlock, entre NextTitle et NextBody (ordre : SalesBar, ItemsBar, GoldBar)
- [x] Track / Fill / Label par barre, RectTransform only, layer 5
- [x] Unlock tooltip + bandeau intacts

---

## Phase 2 — Visuels jauges + texte overlay (après Ph.1 OK)

```
[P0-SALE-STAR-BARS-001] Phase 2 ONLY — filled bars + overlay text. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Slider. No Simulate.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

On SalesBar, ItemsBar, GoldBar (same recipe):
- LayoutElement preferredHeight 22, minHeight 22.
- Child order: Track, Fill, Label (Label last = on top).
- Track + Fill + Label: stretch parent (anchors 0,0–1,1), sizeDelta 0, raycast OFF.

Track: Image color RGB 0.18,0.18,0.22.
Fill: Image Type=Filled, Method=Horizontal, Origin=Left.
- SalesBar Fill color RGB 0.95,0.35,0.55, fillAmount 0.4
- ItemsBar Fill color RGB 0.45,0.75,0.40, fillAmount 0.5
- GoldBar Fill color RGB 0.95,0.78,0.22, fillAmount 0.25
Label: TMP center, size 13, white, wrap OFF, overflow Overflow.
Placeholder: "Ventes  2/5" / "Salades  4/50" / "Or gagné  50/2000"
Font Asset + orthographic = same as NextBody (LiberationSans SDF).

Keep NextBody. Tooltip stays INACTIVE. m_Layer: 5.
Do NOT edit unlock tooltip or bandeau.

Done = Save. List Image type + TMP names. STOP.
```

**Checklist review Ph.2 (Cursor)** — **OK Bezy 2026-08-25** (+ fix Cursor fill YAML)
- [x] Image Fill = Filled Horizontal Left (Bezy API ne expose pas fillMethod/fillAmount → patch Cursor)
- [x] Label overlay stretch, font NextBody, layer 5 — placeholders OK
- [x] LayoutElement h=22 ; tooltip inactif
- [x] Couleurs : rose / vert / or sur SalesBar / ItemsBar / GoldBar

**Checklist review Ph.3 (Cursor)** — **OK Cursor 2026-08-25** (Bezy non requis — wiring YAML)
- [x] 3 `SaleChannelStarProgressBarView` câblés Fill + Label
- [x] Host `salesBar` / `itemsBar` / `goldBar`
- [x] Cadenas / unlock host inchangés

Script existant : `SaleChannelStarProgressBarView` (ne pas recréer).

```
[P0-SALE-STAR-BARS-001] Phase 3 ONLY — wire star progress bars. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. No OnClick. No Simulate.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

1) Add SaleChannelStarProgressBarView on SalesBar, ItemsBar, GoldBar.
   Each: fillImage → child Fill Image ; label → child Label TMP.

2) On SaleChannelStarTooltipHost (already on SaleChannelStarTooltip) wire:
- salesBar → SalesBar view
- itemsBar → ItemsBar view
- goldBar → GoldBar view
Keep existing host refs (panelRoot, titles, nextBodyLabel, etc.).

3) m_Layer: 5. Tooltip stays INACTIVE.
4) Do not edit SaleChannelUnlockTooltip, LockedOverlay, bandeau prefab.

Done = Save. Confirm 3 views + 3 host refs assigned. STOP.
```

**Checklist review Ph.3 (Cursor)**
- [ ] 3 `SaleChannelStarProgressBarView` câblés Fill + Label
- [ ] Host `salesBar` / `itemsBar` / `goldBar`
- [ ] Cadenas / unlock host inchangés

**Pas de Ph.4 Bezy** — fillAmount runtime = Cursor (`SaleChannelStarProgressBarView.Apply`). Playtest auteur `[P0-SALE-STAR-PLAY-001]`.
