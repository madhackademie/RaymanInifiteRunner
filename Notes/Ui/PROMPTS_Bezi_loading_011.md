# Prompts Bezy — LoadingScreen layout `[BZ-POLISH-011]`

**Scène :** `Assets/Scenes/Bootstrap.unity`  
**Script (ne pas modifier) :** `Assets/Scripts/UI/LoadingScreen.cs`  
**Fill runtime :** `ProgressBarFill.rectTransform.anchorMax.x` (0→1) — **pas** `Image.fillAmount`  
**Layers :** UI = `m_Layer: 5` — `Notes/Ui/CONVENTION_layers_unity.md`  
**Hors scope :** illustration finale splash (`[BL-ART-006]` / `LOADINGSCREEN_image_workflow.md`)

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

**Hiérarchie actuelle :**
```
LoadingCanvas (layer 5 OK)
  LoadingScreen (CanvasGroup + LoadingScreen.cs)
    Background
    SplashImage
    ProgressBarContainer
      ProgressBarBg
        ProgressBarFill (+ HorizontalGradient)
      PercentageText (TMP)
```

**SerializeFields à préserver :** `fillImage` → ProgressBarFill · `percentageText` → PercentageText · `canvasGroup` → LoadingScreen · `GameBootstrap.loadingScreen`

**Ordre :** Ph.1 → Ph.2 → Ph.3 (attendre succès avant la suivante).

---

## Phase 1 — Layers UI = 5 — **OK** (Bezy 2026-08-05)

```
[BZ-POLISH-011] Phase 1 ONLY — LoadingScreen layers UI=5. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not add/rename art sprites.
Do not rename LoadingScreen / LoadingCanvas / ProgressBarFill / PercentageText.
Keep LoadingScreen SerializeFields wired: fillImage, percentageText, canvasGroup.
Keep ProgressBarFill RectTransform fill method: anchors left stretch (anchorMin 0,0 ; anchorMax.x driven by code). Do NOT switch Image Type to Filled. Do NOT remove HorizontalGradient.

Scene ONLY:
- Assets/Scenes/Bootstrap.unity

Hierarchy (existing):
LoadingCanvas > LoadingScreen > Background, SplashImage, ProgressBarContainer > ProgressBarBg > ProgressBarFill ; PercentageText

REQUIRED:
1) Set m_Layer: 5 on LoadingCanvas AND every child under it (Background, SplashImage, ProgressBarContainer, ProgressBarBg, ProgressBarFill, PercentageText, LoadingScreen).
2) Layer UI = m_Layer: 5 (TagManager Water=4, UI=5). Never use layer 4 for UI.
3) Do not change RectTransforms, colors, sprites, TMP, or sibling order this phase.
4) Do not touch GameBootstrap object except leave loadingScreen reference intact.
5) Save scene.

Interdits: no C#; no Simulate/Play Mode; no new GameObjects; no illustration import; do not break fillImage/percentageText/canvasGroup refs.

Done = Save. List each GO name + m_Layer. Confirm fillImage still ProgressBarFill + percentageText still PercentageText. STOP.
```

**Chars ~1424** — OK &lt; 3500.

### Checklist review Phase 1 (Cursor)

- [x] Tous les GO sous `LoadingCanvas` en `m_Layer: 5` (Background, SplashImage, ProgressBar*, PercentageText, LoadingScreen)
- [x] Refs `fillImage` / `percentageText` / `canvasGroup` intactes ; `GameBootstrap.loadingScreen` OK
- [x] `GameBootstrap` reste layer 0 (hors UI — normal)

---

## Phase 2 — Layout barre + % — **OK** (Bezy 2026-08-05)

```
[BZ-POLISH-011] Phase 2 ONLY — ProgressBarContainer layout polish (mobile readable). Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not import splash art.
Do not rename nodes. Keep SerializeFields: fillImage=ProgressBarFill, percentageText=PercentageText, canvasGroup on LoadingScreen.
CRITICAL: ProgressBarFill width = anchorMax.x (0..1). Keep Image Type=Simple. Keep HorizontalGradient. Do NOT use Fill Amount for progress.

Scene ONLY: Assets/Scenes/Bootstrap.unity

REQUIRED under LoadingScreen/ProgressBarContainer:
1) Container RectTransform: anchorMin (0,0) anchorMax (1,0) pivot (0.5,0) ; height sizeDelta.y = 120 ; left/right inset via offsetMin.x=+48 offsetMax.x=-48 ; anchoredPos Y = 48 (safe bottom).
2) ProgressBarBg: NOT full container. Anchor bottom stretch: anchorMin (0,0) anchorMax (1,0) pivot (0.5,0) ; height sizeDelta.y = 28 ; offsetMin.x=+0 offsetMax.x=0 ; anchoredPos Y = 8. Image color RGB (0.12,0.12,0.14) a=1 ; raycastTarget=false. Assign built-in UISprite if sprite null (solid OK).
3) ProgressBarFill child of ProgressBarBg: keep anchors Min(0,0) Max(0,1) pivot(0,0.5) sizeDelta(0,0). Leave HorizontalGradient colors (blue→green). Ensure Image alpha 1.
4) PercentageText sibling of ProgressBarBg: anchor top of container — anchorMin(0,1) Max(1,1) pivot(0.5,1) ; height 48 ; anchoredPos Y=-4. Center align. fontSize 36 ; fontStyle Bold ; color RGB(0.95,0.95,0.97) a=1 ; raycastTarget=false. Keep text "0 %".
5) Sibling order in container: 0=ProgressBarBg, 1=PercentageText.
6) m_Layer: 5 unchanged. Save.

Interdits: no C#; no Simulate; do not move SplashImage/Background; do not change LoadingScreen CanvasGroup; do not recreate hierarchy.

Done = Save. List container insets/height + ProgressBarBg height + PercentageText fontSize/color + confirm fillImage wiring. STOP.
```

**Chars ~1834** — OK &lt; 3500.

### Checklist review Phase 2 (Cursor)

- [x] Container inset ~48 (`sizeDelta.x=-96`) + Y=48 + height 120
- [x] Track `ProgressBarBg` h=28, Y=8, color (0.12,0.12,0.14)
- [x] `%` font 36 Bold color (0.95,0.95,0.97) ; siblings Bg puis Text
- [x] Fill toujours `anchorMax.x` + `HorizontalGradient` ; Image Type Simple
- [ ] Note mineure : sprites Image encore `null` (Bg/Fill) — Ph.3 UISprite si besoin build

---

## Phase 3 — Contraste fond / placeholder splash — **OK** (Bezy 2026-08-05)

```
[BZ-POLISH-011] Phase 3 ONLY — contrast polish Background track + % legibility. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not add final splash illustration (out of scope).
Keep names + SerializeFields fillImage / percentageText / canvasGroup. Keep ProgressBarFill anchorMax.x fill (NOT Image.FillAmount). Keep HorizontalGradient.

Scene ONLY: Assets/Scenes/Bootstrap.unity

REQUIRED:
1) Background Image: color RGB (0.06,0.06,0.08) a=1 ; stretch full LoadingScreen ; raycastTarget=true (blocks clicks during load). Sprite = built-in UISprite if null.
2) SplashImage: leave sprite as-is (may be null). color alpha <= 0.35 if no sprite (placeholder only). PreserveAspect=true ; raycastTarget=false. Do not stretch-crop future art.
3) ProgressBarBg: ensure visible track vs Background (darker/lighter contrast). Optional 2px outline via child Outline ONLY if already present — else skip Outline. No new scripts.
4) PercentageText: outline/underlay soft if TMP supports without new assets — fontSize >= 34 ; color near white ; alignment center. raycastTarget=false.
5) Confirm GameBootstrap.loadingScreen still points to LoadingScreen component.
6) All UI under LoadingCanvas still m_Layer: 5. Save.

Interdits: no C#; no Simulate/Play Mode; no new illustration PNG; do not delete SplashImage; do not break ProgressBarFill anchors.

Done = Save. List Background RGBA + SplashImage alpha + PercentageText fontSize + GameBootstrap.loadingScreen OK + layers. STOP.
```

**Chars ~1497** — OK &lt; 3500.

### Checklist review Phase 3 (Cursor)

- [x] Background `(0.06,0.06,0.08) a=1` + raycast ; SplashImage α `0.35` PreserveAspect
- [x] `GameBootstrap.loadingScreen` + SerializeFields OK ; layers 5
- [x] Fill `anchorMax.x` + HorizontalGradient intacts
- [ ] Note : sprites Image encore `null` (Background/Bg/Fill) — OK Editor ; si build noir → UISprite / `HudModalBackdrop` pattern
- [x] Playtest auteur (hors Bezy) : barre 0→100 % + fade — **OK 2026-08-05**

---

## Après Bezy (auteur / Cursor)

1. Playtest `Bootstrap` : progression + fade, pas de flash.
2. Marquer `[BZ-POLISH-011]` clos dans `Notes/Todo_project.md` + `PROJECT_LOG.md`.
3. Art splash = chantier séparé `[BL-ART-006]`.
