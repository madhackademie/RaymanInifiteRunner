# [BZ-FARM-CAMERA-TOUCH-ZOOM-001] Pinch 2 doigts — zoom caméra biofiltre

**Branche :** `main`  
**Référence design :** `Notes/Farm/NOTE_camera_view_zoom.md` (slice 2 — **pinch zoom seulement** ; pan 1 doigt = ticket séparé).  
**Input :** `UnityEngine.InputSystem` direct (`FarmCameraInput`) — **pas** de migration `.inputactions` (cf. `Notes/Systems/SPEC_input_hybride_pc_mobile.md`).

## Périmètre scènes (farm uniquement)

Zoom / pan / pinch = **scènes de jeu ferme** avec biofiltre (`SceneId.FirstLvl` → `FirstLvl.unity` aujourd’hui).  
**Pas** sur hub / menu (`HomeScene`), **pas** sur futur runner, **pas** sur shell UI (`NavigationHUD` additive).  
`FarmCameraController` reste **uniquement** sur la Main Camera de ces scènes de contenu ferme — ne jamais l’ajouter à une caméra globale ou à une scène sans `BiofiltreViewBounds`.  
Pas de singleton ni d’écoute globale : si le composant n’est pas dans la scène active, aucun zoom tactile.

| Fichier | Rôle |
|---------|------|
| `Assets/Scripts/Farm/FarmCameraController.cs` | Zoom ortho + pivot écran + clamp `BiofiltreViewBounds` |
| `Assets/Scripts/Farm/FarmCameraInput.cs` | Molette, pinch 2 touches, pan milieu / centroïde pinch |
| `Assets/Scripts/Farm/FarmPointerInput.cs` | `IsOverUi(pointerId)` — réutiliser pour le pinch |
| `Assets/Scenes/FirstLvl.unity` | `FarmCameraController` sur Main Camera |

**Hors scope :** `GridManager`, plantation, `BiofiltreManager`, `NavigationHUD`, popups, pan drag 1 doigt, inertie, `.inputactions`.  
**PC inchangé :** molette + clic milieu ; ne pas casser `enableTouchCamera = false` en éditeur sauf test explicite.

**Succès Bezy :** `Save. List what changed. STOP.` — pas Simulate / Play Mode.

**Une phase par appel Bezy.**

---

## Phase 1 — C# pinch + garde UI + mobile auto

```
[BZ-FARM-CAMERA-TOUCH-ZOOM-001] Phase 1 ONLY — Touch pinch zoom (Input System). STOP.

READ ONLY these files first (do NOT rescan whole project):
@Assets/Scripts/Farm/FarmCameraInput.cs
@Assets/Scripts/Farm/FarmCameraController.cs
@Assets/Scripts/Farm/FarmPointerInput.cs
@Notes/Bezi/RULES_bezy_code.md

Do NOT edit scenes, prefabs, NavigationHUD, GridManager, BiofiltreManager, .inputactions.
Do NOT add FarmCameraController to HomeScene, runner scenes, or any non-farm scene.

GOAL: Farm-scene-only pinch (component on farm content camera only). Two-finger pinch zooms ortho toward pinch midpoint (HandlePinchZoom). Harden input + enable pinch on mobile when THIS component is active — not game-wide.

1) FarmCameraInput.cs — TryGetPinch / TryReadTwoTouches:
   - When collecting 2 in-progress touches, read each touchId (TouchControl.touchId).
   - If FarmPointerInput.IsOverUi(touchId) for EITHER touch → return false (no pinch over HUD/nav/popups).
   - Keep midpoint + distance logic; distance > 1f unchanged.

2) FarmCameraController.cs:
   - Add [SerializeField] bool autoEnableTouchOnMobile = true.
   - In Start(), after TryResolve(), if autoEnableTouchOnMobile && Application.isMobilePlatform → enableTouchCamera = true.
   - Do NOT set enableTouchCamera = true unconditionally in Editor.
   - Optional: [SerializeField] [Range(0.25f, 2f)] float pinchZoomStrength = 1f.
     In HandlePinchZoom, when applying zoom factor (lastPinchDistance/distance), use Mathf.Pow(ratio, pinchZoomStrength) only if you add the field — default 1f = behavior unchanged.

3) Do NOT add Update() elsewhere. Keep LateUpdate pipeline. No new files.

Save. List files + public/serialized fields touched. STOP.
```

**Lancement Bezy :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_touch_zoom.md
@Notes/Bezi/RULES_bezy_code.md
[BZ-FARM-CAMERA-TOUCH-ZOOM-001] Phase 1 ONLY — Touch pinch zoom (Input System). STOP.
```

---

## Phase 2 — Scène FirstLvl (valeurs Inspector)

```
[BZ-FARM-CAMERA-TOUCH-ZOOM-001] Phase 2 ONLY — FarmCameraController Inspector FirstLvl. STOP.

OPEN Assets/Scenes/FirstLvl.unity FIRST (farm content scene ONLY). Do NOT rescan. Do NOT edit .cs.
Do NOT edit Biofiltre.prefab except instance refs already in scene.
Do NOT edit NavigationHUD, HomeScene, or other scenes.
Do NOT add FarmCameraController outside FirstLvl / future farm SceneId scenes.

GOAL: Farm scene Main Camera ready for Android pinch test after Phase 1 C#.

On Main Camera → FarmCameraController:
- viewBounds = Biofiltre instance (unchanged if already wired)
- frameOnStart = true
- allowPan = true
- enableTouchCamera = false (mobile auto via autoEnableTouchOnMobile from Phase 1)
- autoEnableTouchOnMobile = true (if field exists)
- pinchZoomStrength = 1 (if field exists)
- paddingFactor / minOrthoSize = keep current values

Camera stays orthographic. Do NOT hand-tune orthographic size.

Save scene. Reply: autoEnableTouchOnMobile, enableTouchCamera serialized value. STOP.
```

**Lancement Bezy :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_touch_zoom.md
[BZ-FARM-CAMERA-TOUCH-ZOOM-001] Phase 2 ONLY — FarmCameraController Inspector FirstLvl. STOP.
```

---

## Après Bezy (auteur + Cursor revue)

**Statut 2026-09-23 :** Ph.1 C# livré · Ph.2 `FirstLvl` no-op (Inspector déjà : `autoEnableTouchOnMobile=true`, `enableTouchCamera=false`).

**Playtest Android / Device Simulator :** 2 doigts écartés = zoom in sur le biofiltre ; rapprochés = zoom out ; bornes = rect orange ; pinch sur bandeau nav = ignoré.  
**Playtest PC :** molette + milieu OK ; pas de pinch souris requis.

**Suite produit :** pan 1 doigt Township — `Notes/Farm/NOTE_camera_view_zoom.md` (`[P0-FARM-CAMERA-TOUCH-001]`).
