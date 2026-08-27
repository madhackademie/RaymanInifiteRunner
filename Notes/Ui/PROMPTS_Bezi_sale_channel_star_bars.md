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

---

## Playtest 2026-08-26 — layout cassé (fix Cursor, pas Bezy)

**Symptôme :** texte hors panneau, pas de mini-barres visibles.  
**Cause :** Ph.1 laissait `NextBlock` + barres en **100×100** centré (pas stretch largeur tooltip).  
**Fix Cursor :** stretch horizontal barres + `ChildForceExpandWidth` + rebuild layout host.  
**Re-playtest** après pull : hover ★ → 3 barres pleine largeur + texte centré dedans.

**Bezy Ph.5** (optionnel polish visuel seulement si auteur le demande) — voir fin de fichier.

---

## Phase 5 — Polish layout barres (OPTIONNEL — seulement si playtest encore KO)

```
[P0-SALE-STAR-BARS-001] Phase 5 OPTIONAL — bar layout polish only if playtest still broken. STOP after save.

File ONLY: Assets/Prefabs/Ui/SaleChannelsScreen.prefab
SaleChannelStarTooltip / NextBlock / SalesBar ItemsBar GoldBar ONLY.

Verify each bar: stretch full NextBlock width, height 22, Track dark + Fill colored + Label centered ON TOP.
Do NOT recreate scripts. No Simulate.

Done = Save. List anchor changes. STOP.
```

---

## Phase 6 — Icônes entête barres `[P0-SALE-STAR-BARS-002]`

**But :** icône **balance** (Ventes) + icône **salade mature** (Salades) à gauche de chaque jauge.  
**Or gagné :** pas d’icône demandée — spacer `Icon` vide 18 px pour aligner les 3 barres.  
**Sprites (déjà importés, ne pas recréer) :**
- Balance : `Assets/Art/Sprites/UI/Progression/CommerceFiligrane.png`
- Salade : `Assets/Art/Sprites/Plantes/Laitue/04_MatureLaituce_image.png` (même que `LaitueMature.asset`)

**Prérequis :** Ph.1–3 jauges OK + playtest Cursor OK. Ouvrir prefab `SaleChannelsScreen` avant envoi.  
**Ordre :** Ph.6.1 → succès → Ph.6.2. **Pas de C#.** **Pas de Simulate.**

---

### Phase 6.1 — Shell Icon + BarBody (hiérarchie seule)

```
[P0-SALE-STAR-BARS-002] Phase 6.1 ONLY — icon row shells on star tooltip bars. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate prefabs. No Simulate.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Scope ONLY: SaleChannelStarTooltip / NextBlock / SalesBar, ItemsBar, GoldBar.

For EACH of SalesBar, ItemsBar, GoldBar (same recipe):

1) Create child RectTransform named BarBody. Move EXISTING children Track, Fill, Label under BarBody (reparent only — do NOT delete/recreate Track/Fill/Label).

2) Create sibling Icon (RectTransform only, no Image yet) as FIRST child of the bar (order: Icon, BarBody).

3) On bar root add HorizontalLayoutGroup:
   - spacing 6
   - childAlignment MiddleLeft
   - childControlWidth = true, childControlHeight = true
   - childForceExpandWidth = false, childForceExpandHeight = false

4) Icon: LayoutElement preferredWidth 18, preferredHeight 18, min 18/18.

5) BarBody: LayoutElement flexibleWidth 1, preferredHeight 22, minHeight 22.

6) Inside BarBody keep Track/Fill/Label stretch BarBody (anchors 0,0–1,1). Child order Track, Fill, Label.

7) Keep existing LayoutElement on bar root (height 22) + SaleChannelStarProgressBarView refs (fillImage→Fill, label→Label). Do NOT rewire host.

8) m_Layer: 5 on Icon + BarBody + all moved children.

Do NOT edit SaleChannelUnlockTooltip, LockedOverlay, bandeau prefab, NextBody text.

Done = Save prefab. List reparent paths (Icon/BarBody/Track). STOP.
```

**Checklist review Ph.6.1 (Cursor)** — **OK Bezy 2026-08-26**
- [x] Icon + BarBody sur SalesBar / ItemsBar / GoldBar
- [x] Track / Fill / Label reparentés sous BarBody (refs ProgressBarView intactes)
- [x] HorizontalLayoutGroup bar root, Icon 18 px, BarBody flex width
- [x] Layer 5, tooltip inactif, unlock/bandeau intacts

---

### Phase 6.2 — Sprites icônes Ventes + Salades

```
[P0-SALE-STAR-BARS-002] Phase 6.2 ONLY — assign bar header icons. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not change Track/Fill/Label layout. No Simulate.
Do NOT set Fill fillAmount or fillMethod (runtime Cursor).

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Scope ONLY: SalesBar/Icon, ItemsBar/Icon, GoldBar/Icon under SaleChannelStarTooltip.

1) SalesBar/Icon: add Image
   - Sprite = Assets/Art/Sprites/UI/Progression/CommerceFiligrane.png
   - Preserve Aspect = true
   - Color white RGB (1,1,1) alpha 1 (readable icon — NOT filigrane 0.12)
   - raycastTarget = false
   - RectTransform stretch Icon parent (0,0–1,1 offsets 0)

2) ItemsBar/Icon: add Image
   - Sprite = Assets/Art/Sprites/Plantes/Laitue/04_MatureLaituce_image.png
   - Same Image settings as SalesBar/Icon

3) GoldBar/Icon: leave empty spacer only (no Image OR Image without sprite). Keep 18x18 LayoutElement for alignment. Do NOT invent a gold sprite.

4) BarBody Track/Fill/Label unchanged. Tooltip stays INACTIVE. m_Layer: 5.

Done = Save. List sprite paths on SalesBar/Icon + ItemsBar/Icon + GoldBar spacer. STOP.
```

**Checklist review Ph.6.2 (Cursor)** — **OK Bezy 2026-08-26**
- [x] Balance CommerceFiligrane sur SalesBar/Icon (α=1, preserve aspect)
- [x] Salade mature sur ItemsBar/Icon
- [x] GoldBar Icon spacer 18 px sans sprite
- [ ] Jauges + labels runtime inchangés ; playtest hover ★ `[P0-SALE-STAR-PLAY-002]`

---

### Phase 6.3 — Icône billet Or gagné (GoldBar)

**Sprite :** `Assets/Art/Assets Store Dump/billet-poulpe-lowpoly-violet.png`  
**Prérequis :** Ph.6.2 OK. Prefab `SaleChannelsScreen` ouvert.

```
[P0-SALE-STAR-BARS-002] Phase 6.3 ONLY — GoldBar bill icon. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT change SalesBar/Icon or ItemsBar/Icon. Do NOT touch Track/Fill/Label.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Scope ONLY: GoldBar/Icon under SaleChannelStarTooltip/NextBlock.

GoldBar/Icon already exists (LayoutElement 18x18, no Image yet). ADD Image on GoldBar/Icon:
- Sprite = Assets/Art/Assets Store Dump/billet-poulpe-lowpoly-violet.png
- Preserve Aspect = true
- Color white RGB (1,1,1) alpha 1
- raycastTarget = false
- RectTransform stretch Icon parent (anchors 0,0–1,1 ; offsets 0)

Match SalesBar/Icon Image settings exactly. Keep LayoutElement 18/18. m_Layer: 5.
Tooltip stays INACTIVE. Do not edit unlock tooltip or bandeau.

Done = Save. Confirm sprite path on GoldBar/Icon only. STOP.
```

---

### Phase 6.4 — Rebrancher GoldBar sur chemin canonique (hors Dump)

**Sprite canonique :** `Assets/Art/Sprites/UI/Currency/GoldBill.png`  
**Ne plus utiliser :** `Assets/Art/Assets Store Dump/billet-poulpe-lowpoly-violet.png`

```
[P0-SALE-STAR-BARS-002] Phase 6.4 ONLY — swap GoldBar icon sprite to canonical path. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT change SalesBar/Icon or ItemsBar/Icon. Do NOT touch Track/Fill/Label.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Scope ONLY: GoldBar/Icon Image under SaleChannelStarTooltip/NextBlock.

On GoldBar/Icon Image ONLY:
- Set Source Image sprite = Assets/Art/Sprites/UI/Currency/GoldBill.png
- Keep Preserve Aspect = true, Color white alpha 1, raycastTarget = false
- Keep LayoutElement 18x18 and RectTransform stretch

Do NOT reference Assets/Art/Assets Store Dump/ anymore.

Done = Save. Confirm new sprite path GoldBill.png on GoldBar/Icon. STOP.
```

**Checklist review Ph.6.4 (Cursor)** — **OK Bezy 2026-08-27**
- [x] GoldBar/Icon → `GoldBill.png` (guid `7595e70b…`)
- [x] Plus aucune ref Dump `b512d8…` sur GoldBar
- [ ] Playtest hover ★ : billet sans fond blanc si PNG OK `[P0-SALE-STAR-PLAY-002]`
