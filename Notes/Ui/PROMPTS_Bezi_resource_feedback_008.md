# Prompts Bezy — Popup inventaire plein / feedback ressources `[BZ-POLISH-008]`

**Prefab (partagé shop + ferme) :** `Assets/Prefabs/Ui/ResourceFeedbackPopup.prefab`  
**Script (ne pas modifier) :** `Assets/Scripts/UI/ResourceFeedbackPopupUI.cs`  
**PopupIds :** `farm.inventory.feedback` · `shop.resource.feedback`  
**Layers :** UI = `m_Layer: 5`  
**Ne pas rescanner tout le projet.** Pas de nouveaux sprites / pas de C#.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

Hiérarchie connue : `ResourceFeedbackPopup` → `DimBackdrop` + `Panel` → `MessageText` / `CloseButton` / `CloseText`.

**Note :** même prefab pour inventaire plein (ferme) et feedback shop — polish = les deux surfaces.

**Ordre :** Ph.1–3 — **OK** (Bezy 2026-07-29) + Cursor Open/Close — **CLOS**

### Review Phase 1 (Cursor, 2026-07-29)
- [x] Root + DimBackdrop + Panel + MessageText + CloseButton + CloseText → m_Layer: 5
- [x] ResourceFeedbackPopupUI refs OK

### Review Phase 2 (Cursor, 2026-07-29)
- [x] MessageText 24 + wrap ; Panel #1A1A1A a0.95 ; DimBackdrop a0.55
- [x] CloseButton 240×64 ; CloseText 22 ; refs OK

### Review Phase 3 (Cursor, 2026-07-29)
- [x] Panel CanvasGroup + Animator ; Open/Close/Idle clips (scale+alpha)
- [x] Triggers Open/Close ; Cursor hooks + panelAnimator wired

---

## Phase 1 — Layers UI — OK (archive)

```
(livré Bezy 2026-07-29)
```

---

## Phase 2 — Lisibilité — OK (archive)

```
(livré Bezy 2026-07-29)
```

---

## Phase 3 — Open/Close soft (COPIER TEL QUEL)

```
[BZ-POLISH-008] Phase 3 ONLY — Open/Close soft Animator on Panel. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No new sprites.
Keep layers = 5. Do not rename nodes.

Files:
- Assets/Prefabs/Ui/ResourceFeedbackPopup.prefab
- Create: Assets/Animations/UI/ResourceFeedback_Idle.anim
- Create: Assets/Animations/UI/ResourceFeedback_Open.anim
- Create: Assets/Animations/UI/ResourceFeedback_Close.anim
- Create: Assets/Animations/UI/ResourceFeedback.controller

REQUIRED:
1) On Panel: add CanvasGroup if missing (blocksRaycasts OK when shown).
2) Clip Open ~0.18s: Panel localScale 0.92→1.02→1.0 + CanvasGroup.alpha 0→1.
3) Clip Close ~0.14s: Panel localScale 1→0.94 + CanvasGroup.alpha 1→0.
4) Clip Idle: scale 1, alpha 1, short.
5) Animator on Panel → ResourceFeedback.controller:
   - Bool or Triggers: prefer Triggers Open + Close
   - Default Idle (or Hidden with alpha 0)
   - Any→Open when Open ; Any→Close when Close
   - Open→Idle (exit 1) ; Close→Hidden/Idle (exit 1)
6) Keep ResourceFeedbackPopupUI fields wired (root, messageLabel, closeButton).
7) Save.

Note: Cursor will fire Open/Close later — Bezy does NOT edit C#.
Script currently SetActive on root — Animator ready for Cursor hooks.

Interdits: no C#; no Simulate; do not break DimBackdrop.

Done = Save. List clips + triggers + confirm Panel CanvasGroup. STOP.
```

**Après Bezy (Cursor) :** [x] triggers `Open`/`Close` + hide après Close 0.14 s.
