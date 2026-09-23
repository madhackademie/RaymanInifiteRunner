# [BZ-FARM-CAMERA-VIEW-001] Câbler vue caméra + zoom (rect orange)

**Branche :** `main` (scripts `BiofiltreViewBounds` / `FarmCameraController` déjà dans le repo).  
**C# déjà livré Cursor** — Bezy **n’écrit pas** de `.cs` ni ne modifie la logique zoom. Job = Inspector / prefab / scène.  
**Slice 1 = PC** (molette + clic milieu). Pinch / Township = slice 2, **pas** ces phases.

| Script | Rôle |
|--------|------|
| `Assets/Scripts/Farm/BiofiltreViewBounds.cs` | Rect 2D auteur (vue défaut + zoom out max) |
| `Assets/Scripts/Farm/FarmCameraController.cs` | Frame + molette/pinch + pan milieu |
| `Assets/Editor/BiofiltreViewBoundsEditor.cs` | Poignées Scene + boutons Init |

**Hors scope :** `GridManager`, bake deck UV, `cellSize`, `BiofiltreManager`, popups, `NavigationHUD`, C#.  
**Pas** `/prefab-ui-3phases` (prefab World, pas `Prefabs/Ui`).  
**Pas** le job `[BZ-FARM-MOBILE-SCALE-001]` (cadrage ortho size one-shot). Celui-ci **remplace** ce one-shot au runtime.

**Succès :** `Save. List what changed. STOP.` — pas Simulate / Play Mode.

**Une phase par appel.**

---

## Phase 1 — Prefab `Biofiltre` (rect vue)

```
[BZ-FARM-CAMERA-VIEW-001] Phase 1 ONLY — Add BiofiltreViewBounds on Biofiltre.prefab. STOP.

OPEN Assets/Prefabs/World/Biofiltre.prefab FIRST. Do NOT rescan whole project.
Do NOT edit any .cs. Do NOT edit FirstLvl.unity. Do NOT edit NavigationHUD.
Do NOT change GridManager, deckShapeUv, cellSize, IbcSprite transform, BiofiltreHud.

GOAL: Orange view rect on the biofiltre root. This is the default camera view and zoom-out limit.

1) Root GameObject Biofiltre: Add Component BiofiltreViewBounds if missing.
2) Inspector BiofiltreViewBounds: click "Init from IBC sprite".
   If IbcSprite missing, click "Init from grid AABB" instead.
3) Keep the orange gizmo. Optional: enlarge localSize slightly so HUD slots stay inside the rect (small pad only).
4) Do NOT add FarmCameraController on this prefab.

Save prefab. Reply: localCenter, localSize. STOP. No Play Mode.
```

**Lancement Unity :** Prefab Mode `Biofiltre` ouvert.

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_view.md
[BZ-FARM-CAMERA-VIEW-001] Phase 1 ONLY — Add BiofiltreViewBounds on Biofiltre.prefab. STOP.
```

---

## Phase 2 — Scène `FirstLvl` (controller caméra)

```
[BZ-FARM-CAMERA-VIEW-001] Phase 2 ONLY — Add FarmCameraController on FirstLvl Main Camera. STOP.

OPEN Assets/Scenes/FirstLvl.unity FIRST. Do NOT rescan. Do NOT edit .cs.
Do NOT edit Assets/Prefabs/World/Biofiltre.prefab (instance wiring OK).
Do NOT edit NavigationHUD.unity. Do NOT change GridManager / deck UV.

GOAL: Runtime camera frames BiofiltreViewBounds on play. No handwritten ortho size.

1) Main Camera: Add Component FarmCameraController if missing.
2) FarmCameraController:
   - worldCamera = this Camera (or leave empty)
   - viewBounds = Biofiltre instance in the scene (BiofiltreViewBounds)
   - frameOnStart = true
   - paddingFactor = 1.08
   - minOrthoSize = 1.5
   - allowPan = true
   - enableTouchCamera = false
3) Keep Camera orthographic ON. Do NOT retune orthographic size by hand (runtime FrameToBounds).
4) Do NOT scale the Biofiltre instance.

Save scene. Reply: viewBounds assigned yes/no, paddingFactor, minOrthoSize. STOP. No Play Mode.
```

**Lancement Unity :** scène `FirstLvl` ouverte.

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_view.md
[BZ-FARM-CAMERA-VIEW-001] Phase 2 ONLY — Add FarmCameraController on FirstLvl Main Camera. STOP.
```

---

## Après Bezy (auteur, hors prompt)

Playtest FirstLvl **PC** : biofiltre entier à l’écran ; molette zoom ; clic milieu pan ; pas de zoom infini ; clic gauche plante encore. Pinch = plus tard (`NOTE_camera_view_zoom.md`).
