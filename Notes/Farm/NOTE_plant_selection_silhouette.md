# Sélection plante — silhouette blanche

**ID :** `[P0-FARM-PLANT-SELECT-GLOW-001]` · **clos playtest** 2026-09-10

## Runtime

- Clic footprint → socle vert (`BiofiltreGridVisualizer`) + silhouette (`PlantSelectionHighlight`).
- Une seule plante active ; clear à la fermeture popup récolte ou changement de cible.
- Pose graine : pas de glow sur le ghost (`PlantPlacementPreview.HideGhostSelectionGlow`).

## Visuel

- Shader **`Farm/SpriteSelectionSilhouette`** : forme = alpha du sprite, couleur = tint (blanc).
- Enfant prefab **`SelectionGlow/GlowInner`** (GlowOuter désactivé au runtime).
- Réglages sur **`LaitueObj`** → `PlantSelectionHighlight` :
  - `silhouetteLocalScale` — défaut **1,012** (plus fin = plus proche de 1).
  - `silhouetteLocalOffset` — défaut **(0,004, 0,004)** ; `(0,0)` pour centré.

## Suite farm

- `[CT-FARM-BAKE-RECT-001]` grand bac rect.
- `[BZ-FARM-HARVEST-READY-VFX-002]` sparkle récolte.
