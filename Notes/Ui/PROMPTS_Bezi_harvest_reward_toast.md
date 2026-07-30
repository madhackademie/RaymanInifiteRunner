# Prompts Bezy — Toast feedback récolte `[BZ-POLISH-007]`

**Prefab :** `Assets/Prefabs/Ui/HarvestRewardFeedbackPopup.prefab`  
**Script (ne pas modifier) :** `Assets/Scripts/UI/HarvestRewardFeedbackPopupUI.cs`  
**PopupId :** `farm.harvest.reward`  
**Layers :** UI = `m_Layer: 5`  
**Ne pas rescanner tout le projet.** Pas de nouveaux sprites / pas de C#.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

### Contrainte critique (anti-conflit)

Le script pilote déjà :
- `animatedRoot.anchoredPosition` (montée)
- `animatedCanvasGroup.alpha` (fade)

**Interdit Bezy :** animer `CanvasGroup.alpha` ou `anchoredPosition` sur `LootFlyoutGroup` / root animé.  
**OK Bezy :** `localScale` uniquement + layers + lisibilité (couleurs / fonts).

Hiérarchie connue : `HarvestRewardFeedbackPopup` → `LootFlyoutGroup` → `IconSlot` + `AmountLabel`.

**Ordre :** Ph.1–3 — **OK** (Bezy 2026-07-29) + Cursor `Show` — **CLOS**

### Review Phase 1 (Cursor, 2026-07-29)
- [x] Root + LootFlyoutGroup + IconSlot + AmountLabel → m_Layer: 5
- [x] SerializeFields HarvestRewardFeedbackPopupUI OK

### Review Phase 2 (Cursor, 2026-07-29)
- [x] AmountLabel fontSize 32 ; IconSlot SizeDelta 100 ; LayoutElement preferred 64
- [x] Refs OK ; CanvasGroup intact
- [~] Mineur : pas de fond sombre optionnel derrière icône — OK V0

### Review Phase 3 (Cursor, 2026-07-29)
- [x] Idle + ShowPunch (0.86→1.08→1 @0.22s), path "" on LootFlyoutGroup
- [x] Trigger Show, no alpha curves
- [x] Cursor: SetTrigger Show in ShowHarvestReward

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

## Phase 3 — Scale punch entrée (COPIER TEL QUEL)

```
[BZ-POLISH-007] Phase 3 ONLY — entry scale punch Animator (scale ONLY). Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No new sprites.
Keep layers = 5. Do not rename nodes.

Files:
- Assets/Prefabs/Ui/HarvestRewardFeedbackPopup.prefab
- Create: Assets/Animations/UI/HarvestRewardFeedback_Idle.anim
- Create: Assets/Animations/UI/HarvestRewardFeedback_ShowPunch.anim
- Create: Assets/Animations/UI/HarvestRewardFeedback.controller

CRITICAL:
- Animate path "" OR "LootFlyoutGroup" localScale ONLY.
- Do NOT key CanvasGroup.alpha.
- Do NOT key anchoredPosition / position.

REQUIRED:
1) Clip Idle: scale 1, short, loop OFF.
2) Clip ShowPunch ~0.22s: scale 0.86 → 1.08 → 1.0 (punch).
3) Controller on LootFlyoutGroup (or root if that is animatedRoot):
   - Trigger: Show
   - Default Idle
   - Any → ShowPunch when Show (exit time OFF, Can Transition To Self OFF)
   - ShowPunch → Idle (exit time ON = 1)
4) Keep HarvestRewardFeedbackPopupUI fields wired (animatedRoot / CanvasGroup / icon / label).
5) Save.

Note: Cursor will SetTrigger("Show") later — Bezy does NOT edit C#.

Interdits: no alpha keys; no Simulate; no C#.

Done = Save. List clips + trigger Show + confirm NO alpha curves. STOP.
```

**Après Bezy (Cursor) :** `HarvestRewardFeedbackPopupUI` → `animator.SetTrigger("Show")` au début de `ShowHarvestReward`.
