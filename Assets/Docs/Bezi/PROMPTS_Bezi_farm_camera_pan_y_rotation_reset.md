# Farm caméra — 3 passes : pan Y · rotation · reset

**Scène :** `FirstLvl` uniquement (`FarmCameraController` sur Main Camera).  
**Référence :** `Notes/Farm/NOTE_camera_view_zoom.md`  
**Une phase Bezy par appel.** Fin : `Save. List what changed. STOP.` — pas Simulate.

| Passe | Task ID | Agent |
|-------|---------|--------|
| 1 — Pan Y (pinch + zoom) | `[BZ-FARM-CAM-PAN-Y-001]` | Bezy C# |
| 2 — Rotation Z | `[BZ-FARM-CAM-ROTATE-001]` | Bezy C# |
| 3 — Bouton reset | `[BZ-FARM-CAM-RESET-001]` | Bezy prefab 3 phases + C# view |

**Ordre strict :** 1 → 2 → 3 (rotation et reset s’appuient sur le contrôleur corrigé).

---

## Passe 1 — `[BZ-FARM-CAM-PAN-Y-001]` Pan vertical + pinch

**Diagnostic attendu :** le pan math clamp déjà X **et** Y (`FarmCameraViewMath`). Si seul X bouge au pinch/zoom, corriger surtout la projection écran→monde (Z ortho) et le **translation du centroïde** pendant le pinch.

```
[BZ-FARM-CAM-PAN-Y-001] ONLY — Farm camera pan Y + ScreenToWorld fix. STOP.

READ ONLY (no full project scan):
@Assets/Scripts/Farm/FarmCameraController.cs
@Assets/Scripts/Farm/FarmCameraInput.cs
@Assets/Scripts/Farm/FarmCameraViewMath.cs
@Notes/Bezi/RULES_bezy_code.md

Do NOT edit scenes/prefabs. Do NOT touch GridManager, BiofiltreManager, NavigationHUD, popups.

GOAL: During pinch-zoom AND pan, camera moves on BOTH X and Y (world), then ApplyClamp.

1) FarmCameraController — ScreenToWorld:
   - Replace naive ScreenToWorldPoint(Vector2) with correct depth for ortho 2D:
     use gameplay plane z=0: e.g. float depth = -worldCamera.transform.position.z;
     Vector3 sp = new Vector3(screenPosition.x, screenPosition.y, depth);
     return worldCamera.ScreenToWorldPoint(sp) as Vector2 (x,y only).

2) HandlePinchZoom — centroid pan:
   - Cache lastPinchMidpoint (Vector2) alongside lastPinchDistance.
   - Each frame with valid pinch: AFTER zoom step, if pinchActive and last midpoint valid,
     world delta = ScreenToWorld(lastMid) - ScreenToWorld(currentMid);
     add delta to camera position (same as HandlePan).
   - Reset lastPinchMidpoint when pinch ends.

3) Do NOT remove HandlePan or ApplyClamp. Keep farm-scene-only behavior.

Save. List methods changed. STOP.
```

**Lancement :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_pan_y_rotation_reset.md
@Notes/Bezi/RULES_bezy_code.md
[BZ-FARM-CAM-PAN-Y-001] ONLY — Farm camera pan Y + ScreenToWorld fix. STOP.
```

**Playtest auteur (hors Bezy) :** pinch zoom + déplacer les 2 doigts → image bouge en X **et** Y ; zoom out max = centre peut rester locké (normal si frustum ≥ rect).

---

## Passe 2 — `[BZ-FARM-CAM-ROTATE-001]` Rotation Z

```
[BZ-FARM-CAM-ROTATE-001] ONLY — Two-finger twist rotation (farm cam). STOP.

READ ONLY:
@Assets/Scripts/Farm/FarmCameraController.cs
@Assets/Scripts/Farm/FarmCameraInput.cs
@Notes/Bezi/RULES_bezy_code.md

Do NOT edit scenes/prefabs yet. No new files unless one small helper in FarmCameraInput is required.

GOAL: Optional Z rotation on Main Camera (ortho farm), gesture twist 2 doigts when enableTouchCamera.

1) FarmCameraInput — add TryGetPinchTwistDelta(out float deltaDegrees):
   - Reuse same 2 in-progress touches as TryGetPinch (respect IsOverUi on touchIds).
   - deltaDegrees = angle between (touch1-touch2) this frame vs previous frame (store static/previous angle only inside input helper, or caller stores — prefer caller in controller).
   - Return false if not exactly 2 touches.

2) FarmCameraController:
   - [SerializeField] bool allowTouchRotation = true;
   - [SerializeField] [Range(-45f, 0f)] float minRotationZ = -15f;
   - [SerializeField] [Range(0f, 45f)] float maxRotationZ = 15f;
   - [SerializeField] float rotationSensitivity = 1f;
   - In LateUpdate when enableTouchCamera && allowTouchRotation: apply twist delta to transform.eulerAngles.z (unwrap/clamp between min/max).
   - Rotation around camera position (transform.Rotate(0,0,delta, Space.Self) or euler Z clamp).

3) ApplyClamp: keep position clamp unchanged for V1 (no rotated AABB math).

Save. List new APIs. STOP.
```

**Lancement :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_pan_y_rotation_reset.md
@Notes/Bezi/RULES_bezy_code.md
[BZ-FARM-CAM-ROTATE-001] ONLY — Two-finger twist rotation (farm cam). STOP.
```

---

## Passe 3 — `[BZ-FARM-CAM-RESET-001]` Bouton reset vue

**Cible UI :** canvas `FarmUICanvas` dans `Assets/Scenes/FirstLvl.unity` (coin haut-droit, sous safe area, layer UI = 5).

### Phase 3a — C# snapshot + reset (Bezy, avant ou avec prefab)

```
[BZ-FARM-CAM-RESET-001] Phase A ONLY — Capture default view + ResetToDefault(). STOP.

READ ONLY:
@Assets/Scripts/Farm/FarmCameraController.cs
@Notes/Bezi/RULES_bezy_code.md

Do NOT edit scenes/prefabs in this phase.

GOAL: Reset = état après FrameToBounds au Start (position XY, ortho size, rotation Z).

1) FarmCameraController:
   - After first FrameToBounds in Start (or end of Start when frameOnStart), call CaptureDefaultView().
   - Store: defaultPosition (Vector3), defaultOrthoSize (float), defaultRotationZ (float).
   - public void ResetToDefault(): restore those three; then ApplyClamp().
   - Re-capture optional: public void RecaptureDefaultView() for auteur tuning (Inspector context menu OK).

Save. List public methods. STOP.
```

### Phase 3b — Prefab bouton (skill 3 phases)

**Prefab :** `Assets/Prefabs/Ui/Farm/FarmCameraResetButton.prefab` (créer dossier si besoin).

**Phase 1 — shell**

```
/prefab-ui-3phases
Task ID: [BZ-FARM-CAM-RESET-001]
Prefab: Assets/Prefabs/Ui/Farm/FarmCameraResetButton.prefab
Phase: 1
```

```
[BZ-FARM-CAM-RESET-001] Phase 1 ONLY — Prefab shell FarmCameraResetButton. STOP.

CREATE Assets/Prefabs/Ui/Farm/FarmCameraResetButton.prefab.
Hierarchy only: Root (RectTransform 64x64), child Icon optional empty.
Layer UI = 5. Do NOT add Button/Image yet if skill says components phase 2.
Save. STOP.
```

**Phase 2 — composants**

```
[BZ-FARM-CAM-RESET-001] Phase 2 ONLY — Button + Image on FarmCameraResetButton. STOP.

OPEN prefab. Add Image (sprite icône reset si dispo under Assets/Art/Sprites/Ui/, sinon placeholder blanc).
Add Button. TMP_Text child label "Reset" optional small.
Layer 5. Save. STOP.
```

**Phase 3 — wiring**

```
[BZ-FARM-CAM-RESET-001] Phase 3 ONLY — FarmCameraResetButtonView + wire. STOP.

CREATE Assets/Scripts/UI/Farm/FarmCameraResetButtonView.cs:
- [SerializeField] Button resetButton;
- [SerializeField] FarmCameraController cameraController;
- Awake: cache; onClick -> cameraController.ResetToDefault() if ref set.
Add component on prefab root; wire button + leave cameraController empty (scene instance).

Save prefab. STOP.
```

### Phase 3c — Scène FirstLvl

```
[BZ-FARM-CAM-RESET-001] Phase C ONLY — Instance on FarmUICanvas FirstLvl. STOP.

OPEN Assets/Scenes/FirstLvl.unity ONLY.
Instantiate FarmCameraResetButton under FarmUICanvas.
Anchor top-right, margin ~16px, sorting above gameplay, not blocking nav exit.
Wire FarmCameraResetButtonView.cameraController = Main Camera FarmCameraController.

Save scene. STOP.
```

---

## Après les 3 passes (Cursor revue)

- Diff `FarmCameraController` / `FarmCameraInput` / view reset.
- Playtest : pinch pan XY, twist rotation bornée, reset → même cadrage qu’au load.
- Si Y encore bloqué zoom out : agrandir rect orange `BiofiltreViewBounds.localSize.y` (auteur Scene, pas code).
