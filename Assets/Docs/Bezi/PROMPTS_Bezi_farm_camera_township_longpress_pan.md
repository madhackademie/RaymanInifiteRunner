# [BZ-FARM-CAM-TOWN-PAN-001] Pan Township — long press + drag (tactile)

**Scène :** `FirstLvl` · caméra ferme uniquement.  
**Référence :** `Notes/Farm/NOTE_camera_view_zoom.md`  
**PC inchangé :** molette zoom · **clic milieu** pan · **LMB** = clic grille immédiat.

**Pinch 2 doigts :** zoom seulement (pas pan au centroïde pinch sur tactile).

---

## Sens du doigt (Township)

**Carte « attrapée » :** le décor suit le doigt.

- Doigt vers la **droite** → le biofiltre semble aller à droite → la **caméra se déplace à gauche** (monde).
- Formule déjà dans `FarmCameraController.HandlePan` :
  `delta = worldBefore - worldAfter` puis `position += delta`.

**Ne pas inverser** ce signe si tu veux le feeling Township.

---

## Gestes tactiles cibles

| Geste | Effet |
|--------|--------|
| Tap court + relâche (sans slop, &lt; ~450 ms) | Clic grille / plante (au **relâchement**) |
| Appui long (~450 ms) puis drag | Pan caméra X+Y (clamp rect orange) |
| Drag avant la fin du long press (&gt; slop ~12 px) | Annule le futur tap · **ne plante pas** |
| Pinch 2 doigts | Zoom uniquement |
| UI sous le doigt | Ignorer pan / long press |

---

## Découpage livraison

| Étape | Qui | Fichiers |
|-------|-----|----------|
| **A** | **Cursor** | `FarmGridPointerInput.cs` — tap au relâchement si tactile farm |
| **B** | **Bezy** | `FarmCameraInput.cs` + `FarmCameraController.cs` — long press pan |
| **C** | Auteur | Playtest APK : tap plante · long press pan · pinch zoom |

Bezy **ne modifie pas** `FarmGridPointerInput` (étape A obligatoire avant playtest plante).

---

## Phase B — Bezy (C#)

```
[BZ-FARM-CAM-TOWN-PAN-001] ONLY — Long-press pan (touch farm camera). STOP.

READ ONLY:
@Assets/Scripts/Farm/FarmCameraInput.cs
@Assets/Scripts/Farm/FarmCameraController.cs
@Assets/Scripts/Farm/FarmPointerInput.cs
@Notes/Bezi/RULES_bezy_code.md

Do NOT edit FarmGridPointerInput, BiofiltreManager, scenes, prefabs.

GOAL: When enableTouchCamera, 1-finger long press then drag pans camera (Township). Pinch pan path OFF for touch.

1) FarmCameraInput — constants:
   LongPressSeconds = 0.45f, TapSlopPixels = 12f.

2) FarmCameraInput — state + API:
   - public static bool IsPrimaryPointerConsumedByCamera { get; private set; }
   - Reset on pointer up / touch cancel / UI.
   - TryGetLongPressPanScreenPosition(out Vector2 screenPos):
     * Single primary touch OR (optional) skip mouse — touch only when Touchscreen.current != null.
     * Not over UI (pointerId).
     * Not while 2-finger pinch active.
     * Track press start time + start screen pos.
     * If moved > slop before long press fired → mark consumed, no tap (grid handles via Cursor step A).
     * After hold >= LongPressSeconds → enter panning; IsPrimaryPointerConsumedByCamera = true until release.
     * While panning return current screen position each frame.

3) FarmCameraController — when enableTouchCamera:
   - HandlePan: prefer TryGetLongPressPanScreenPosition; do NOT use TryGetPanScreenPosition(includePinchPan:true) for touch (pinch = zoom only).
   - Keep middle-mouse pan on PC when !enableTouchCamera or Mouse middle button path unchanged.
   - Reuse existing pan delta math + ScreenToWorld (fix Z depth if PAN-Y pass merged: depth = -camera.transform.position.z).

4) LateUpdate order: pinch zoom → scroll → long-press pan → clamp.

Save. List APIs + fields. STOP.
```

**Lancement Bezy :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_township_longpress_pan.md
@Notes/Bezi/RULES_bezy_code.md
[BZ-FARM-CAM-TOWN-PAN-001] ONLY — Long-press pan (touch farm camera). STOP.
```

---

## Phase A — Cursor (avant playtest)

Spec pour `FarmGridPointerInput` :

- Si `Touchscreen.current != null` **et** pas clic milieu PC :
  - **Ne plus** planter sur `wasPressedThisFrame`.
  - Sur **relâchement** (`wasReleasedThisFrame`) : si `!FarmCameraInput.IsPrimaryPointerConsumedByCamera` **et** durée &lt; long press **et** slop OK → `TryResolveCell` + clic.
- Souris LMB editor/PC : comportement actuel (press frame) OU aligner sur relâchement — **garder press immédiat** si `!Application.isMobilePlatform` et pas touch actif.

---

## Playtest checklist

- [ ] Long press 0,5 s sur cellule vide → pan, **pas** de popup graines au relâche
- [ ] Tap rapide sur cellule → graines / plante OK
- [ ] Pinch zoom sans déplacer la caméra en X seul bizarre
- [ ] Clic milieu PC pan OK
- [ ] Pan : doigt droite = carte vers droite
