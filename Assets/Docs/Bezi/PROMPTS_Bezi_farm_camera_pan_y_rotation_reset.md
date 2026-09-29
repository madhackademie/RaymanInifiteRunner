# Farm caméra — reset vue (pan + zoom seulement)

**Scène :** `FirstLvl` uniquement (`FarmCameraController` sur Main Camera).  
**Référence :** `Notes/Farm/NOTE_camera_view_zoom.md`  
**Gestes :** long press pan · pinch zoom · **pas de rotation caméra** (feature retirée 2026-09-29).

| Passe | Task ID | Agent |
|-------|---------|--------|
| Reset bouton | `[BZ-FARM-CAM-RESET-001]` | Bezy C# Phase A + prefab + scène |

---

## Phase A — `[BZ-FARM-CAM-RESET-001]` Snapshot + ResetToDefault

```
[BZ-FARM-CAM-RESET-001] Phase A ONLY — Capture default view + ResetToDefault(). STOP.

READ ONLY:
@Assets/Scripts/Farm/FarmCameraController.cs
@Notes/Bezi/RULES_bezy_code.md

Do NOT edit scenes/prefabs in this phase.

GOAL: Reset = vue au chargement scène (position XY, ortho size, euler Z tel qu’au Start — souvent 0).

1) FarmCameraController — snapshot privé:
   - defaultPosition, defaultOrthoSize, defaultRotationZ, defaultCaptured.
   - CaptureDefaultView() en fin de Start (après FrameToBounds si frameOnStart).
   - public void ResetToDefault(): restaurer position (garder z caméra), orthoSize, eulerAngles.z ; ApplyClamp().
   - [ContextMenu] RecaptureDefaultView() pour auteur.

Save. List public methods. STOP.
```

**Lancement :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_pan_y_rotation_reset.md
@Notes/Bezi/RULES_bezy_code.md
[BZ-FARM-CAM-RESET-001] Phase A ONLY — Capture default view + ResetToDefault(). STOP.
```

---

## Prefab + scène (phases 1→3 + C)

Voir sections Phase 1–3 + Phase C dans l’historique git ou `Notes/Bezi/BEZY_QUEUE.md` — bouton `FarmCameraResetButton` sous `FarmUICanvas`.

---

## Annulé — ne pas implémenter

- **`[BZ-FARM-CAM-ROTATE-001]`** twist au pinch — retiré du code 2026-09-29.
