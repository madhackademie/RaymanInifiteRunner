# Caméra farm — vue rect + zoom hybride

**Ticket :** `[P0-FARM-CAMERA-VIEW-001]` · tactile `[P0-FARM-CAMERA-TOUCH-001]` — **playtest pan + zoom OK auteur 2026-09-29** (APK).  
**Branche :** `feature/plant-harvest-zoom` (nouvelle, depuis `main`, 2026-09-21)  
**Rect orange (`BiofiltreViewBounds`) :** cadrage de **départ** ou `Focus()` sur un biofiltre — **ne bloque plus** pan/zoom en jeu (2026-09-29).  
**Bornes niveau :** `FarmLevelViewBounds` sur la scène (TODO auteur) — seul clamp pan quand assigné.  
**Zoom :** `minOrthoSize` (in) · `maxOrthoSize` sur caméra (0 = pas de plafond arrière).

**Scènes :** molette / pan / pinch = **uniquement** les scènes de contenu ferme (ex. `FirstLvl`). Pas hub (`HomeScene`), pas runner, pas caméra shell UI. `FarmCameraController` sur la Main Camera de la scène ferme seulement.

## Slice 1 — PC (fine tuning)

Entrées :
- **Molette** : zoom vers le curseur.
- **Clic milieu** : pan.
- **Clic gauche** : plantation / grille — **jamais** pan.

Bornes : frustum clampé dans le rect orange. `enableTouchCamera = false` (pinch off).

Câblage Bezy : `PROMPTS_Bezi_farm_camera_view.md` Ph.1–2.

## Slice 2 — tactile mobile (après PC OK)

**Ticket pan décor :** `[P0-FARM-CAMERA-PAN-LONGPRESS-001]`  
**Constat 2026-09-23 :** pan perçu **horizontal seulement** — piste #1 : `ScreenToWorldPoint` sans profondeur Z ortho ; piste #2 : clamp Y quand frustum ≥ hauteur rect.

**Plan Bezy restant :** `@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_pan_y_rotation_reset.md`

| Passe | ID | Contenu |
|-------|-----|---------|
| ~~Pan au pinch~~ | — | **Annulé** — pinch = zoom ; pan = long press |
| ~~Rotation twist~~ | `[BZ-FARM-CAM-ROTATE-001]` | **Annulé 2026-09-29** — pan + zoom suffisent |
| 1 | `[BZ-FARM-CAM-RESET-001]` | Snapshot vue Start + bouton UI `FarmUICanvas` (optionnel) |

**Limites zoom (runtime) :** `minOrthoSize` (zoom in max) · zoom out max = tout le rect orange (`BiofiltreViewBounds` + `paddingFactor` ~1,08) via `FarmCameraViewMath`.

### Option retenue — Township : long press + drag (grille incluse)

**Prompt Bezy :** `Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_township_longpress_pan.md` · `[BZ-FARM-CAM-TOWN-PAN-001]`

| Étape | Comportement |
|-------|----------------|
| **Tap court** (relâche &lt; ~450 ms, slop &lt; ~12 px) | Clic grille / plante — **au relâchement** (tactile) |
| **Long press** (~450 ms) puis drag | Pan caméra **X + Y** (carte suit le doigt) |
| Relâcher après pan | Fin pan ; pas de clic grille |
| Pinch 2 doigts | **Zoom** seulement (pas pan au centroïde) |
| PC | Molette · clic **milieu** pan · LMB clic immédiat |

**Sens Township :** doigt → droite = décor → droite ⇒ caméra monde **←** (`delta = worldBefore - worldAfter`, déjà dans `HandlePan`).

Implémentation :
- **Bezy :** `FarmCameraInput` + `FarmCameraController` (long press pan).
- **Cursor :** `FarmGridPointerInput` tap au relâchement + respect `IsPrimaryPointerConsumedByCamera`.
- Pas d’UI sous le doigt (`IsOverUi`).

### Option Township (doc historique — non prioritaire)

Township / Hay Day : drag 1 doigt = pan partout hors mode plante. Gardé en référence si le long press décor est trop restrictif.

| Geste | Hors plantation | Mode plantation |
|-------|-----------------|-----------------|
| Tap court (déplacement &lt; slop ~12 px) | Clic cellule / IBC | — |
| 1 doigt drag | Pan caméra | Ghost grille ; pose au relâche |
| Pinch 2 doigts | Zoom | Zoom |

Règles Township :
- Slop : en dessous = tap ; au-dessus = pan.
- Pas de pan au touch down immédiat.
- Overlay UI : ignorer zoom/pan.

PC inchangé : molette + **clic milieu** pan ; LMB = clic grille, pas drag-pan.

## Hors scope

- Letterbox / scale du biofiltre pour coller à l’écran (`[BZ-FARM-MOBILE-SCALE-001]` one-shot ortho).
- Recalc bake / `cellSize`.

---

## Livré validé APK (2026-09-29) — ne pas refaire Bezy sauf bug

| Sujet | Fichiers | Règle |
|-------|----------|--------|
| Pan long press + pinch zoom | `FarmCameraInput.cs`, `FarmCameraController.cs` | Pinch = zoom ; pan = long press ~450 ms |
| Clic grille tactile | `FarmGridPointerInput.cs` | Tap au relâchement ; ordre avant tracking long press |
| UI / doigt | `FarmPointerInput.cs` | `IsOverUi` avec `fingerId` |
| Consommation pointeur | `FarmCameraInput.IsPrimaryPointerConsumedByCamera` | Grille ignore si caméra a pris le geste |
| Rect orange | `BiofiltreViewBounds.cs` | Vue initiale / `Focus()` — plus de prison runtime |
| Bornes niveau | `FarmLevelViewBounds.cs` | Clamp pan si assigné sur la caméra — **TODO scène** `[P0-FARM-LEVEL-CAM-BOUNDS-001]` |
| Rotation pinch | — | **Annulé** — ne pas réintroduire |

---

## Nouveau thread Bezy (farm clic + caméra, FirstLvl)

Thread plein pan/zoom/clic = **clos**. Ne pas rescanner tout le projet.

**Message 1 (read-only)** — `@` les 7 scripts farm cam + `NOTE_camera_view_zoom.md` + `RULES_bezy_code.md` ; confirmer lecture + APIs publiques ; `ResetToDefault` absent = normal. STOP.

**Message 2 — reset (optionnel) :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_pan_y_rotation_reset.md
@Notes/Bezi/RULES_bezy_code.md
[BZ-FARM-CAM-RESET-001] Phase A ONLY — Capture default view + ResetToDefault(). STOP.
```

**Message 2 — bug ciblé :** une phrase de symptôme + `@FarmCameraInput.cs` + `@FarmGridPointerInput.cs` + `RULES_bezy_code.md` ; change ONLY fix ; pas rotation ; pas `SceneNavigator`. STOP.

**Cursor (pas Bezy) :** bornes niveau — `Notes/Todo_project.md` `[P0-FARM-LEVEL-CAM-BOUNDS-001]`.
