# Caméra farm — vue rect + zoom hybride

**Ticket :** `[P0-FARM-CAMERA-VIEW-001]` · tactile `[P0-FARM-CAMERA-TOUCH-001]`  
**Branche :** `feature/plant-harvest-zoom` (nouvelle, depuis `main`, 2026-09-21)  
**Rect auteur :** `BiofiltreViewBounds` (vue défaut + zoom out max). La grille n’est pas recalculée.

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
**Constat 2026-09-23 :** pan perçu **horizontal seulement** — vérifier clamp `FarmCameraViewMath` (zoom out = centre verrouillé sur un axe) avant tout code.

### Option retenue par l’auteur — long press sur décor (prioritaire)

Plus simple qu’un pan systématique au **drag 1 doigt** ou qu’un **pan à deux doigts** :

| Étape | Comportement |
|-------|----------------|
| Tap court sur grille / plante | Inchangé — plantation, récolte, IBC |
| **Long press** (~400–500 ms) sur **décor non activable** (haie, sol hors cellules, fond IBC sans hit farm) | Entre en mode **pan caméra** |
| Drag pendant le hold | Pan **X et Y**, clampé `BiofiltreViewBounds` |
| Relâcher | Fin pan ; tap court suivant = jeu normal |
| Pinch 2 doigts | **Zoom** seulement (option slice 2 — pas le geste principal pour se déplacer) |

Implémentation cible :
- Colliders ou layer dédié **sans** handler plantation (`FarmPointerInput` ignore ou filtre « décor pan »).
- Pas d’UI overlay sous le doigt (`IsOverUi`).
- C# : étendre `FarmCameraInput` + orchestration avec `FarmCameraController` (Cursor).

### Option Township (doc historique — non prioritaire)

Township / Hay Day : drag 1 doigt = pan partout hors mode plante. Gardé en référence si le long press décor est trop restrictif.

| Geste | Hors plantation | Mode plantation |
|-------|-----------------|-----------------|
| Tap court (déplacement &lt; slop ~12 px) | Clic cellule / IBC | — |
| 1 doigt drag | Pan caméra | Ghost grille ; pose au relâche |
| Pinch 2 doigts | Zoom (+ évent. pan centroïde) | Zoom |

Règles Township :
- Slop : en dessous = tap ; au-dessus = pan.
- Pas de pan au touch down immédiat.
- Overlay UI : ignorer zoom/pan.

PC inchangé : molette + **clic milieu** pan ; LMB = clic grille, pas drag-pan.

## Hors scope

- Letterbox / scale du biofiltre pour coller à l’écran (`[BZ-FARM-MOBILE-SCALE-001]` one-shot ortho).
- Recalc bake / `cellSize`.
