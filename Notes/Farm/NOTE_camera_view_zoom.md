# Caméra farm — vue rect + zoom hybride

**Ticket :** `[P0-FARM-CAMERA-VIEW-001]` · tactile `[P0-FARM-CAMERA-TOUCH-001]`  
**Branche :** `feature/plant-harvest-zoom` (nouvelle, depuis `main`, 2026-09-21)  
**Rect auteur :** `BiofiltreViewBounds` (vue défaut + zoom out max). La grille n’est pas recalculée.

## Slice 1 — PC (fine tuning)

Entrées :
- **Molette** : zoom vers le curseur.
- **Clic milieu** : pan.
- **Clic gauche** : plantation / grille — **jamais** pan.

Bornes : frustum clampé dans le rect orange. `enableTouchCamera = false` (pinch off).

Câblage Bezy : `PROMPTS_Bezi_farm_camera_view.md` Ph.1–2.

## Slice 2 — tactile façon Township (après PC OK)

Township / Hay Day : la carte se **prend** au doigt ; planter est un **mode**, pas le geste par défaut.

| Geste | Hors plantation | Mode plantation |
|-------|-----------------|-----------------|
| Tap court (déplacement &lt; slop ~12 px) | Clic cellule / IBC | — |
| 1 doigt drag | **Pan** caméra | Ghost sur la grille ; **pose au relâche** (`[P0-FARM-MOBILE-PLANT-RELEASE-001]`) |
| Pinch 2 doigts | Zoom vers le milieu + pan du centroïde | Idem (zoom reste dispo) |
| 2 doigts drag sans pinch | Pan | Pan (le 1 doigt reste au ghost) |

Règles :
- Slop : en dessous = tap (grille) ; au-dessus = pan. Sinon chaque balayage plante.
- Pas de pan au **touch down** immédiat.
- Inertie pan : optionnelle, légère, clampée au rect — pas obligatoire V1 tactile.
- Overlay UI (croix, popups) : ignorer zoom/pan.

PC ne change pas en slice 2 : LMB reste clic, pas drag-pan.

## Hors scope

- Letterbox / scale du biofiltre pour coller à l’écran (`[BZ-FARM-MOBILE-SCALE-001]` one-shot ortho).
- Recalc bake / `cellSize`.
