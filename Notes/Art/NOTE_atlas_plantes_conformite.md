# Atlas plantes — conformité (référence laitue)

**Statut :** brouillon règles · **2026-10-01**

Pas encore de charte figée en GDD ; la **laitue runtime** sert de référence empirique.

## Dimensions PNG (7 stades, 1 ligne)

| Atlas | Largeur | Hauteur | Footprint jeu | Commentaire |
|-------|---------|---------|---------------|-------------|
| `AtlasLaitue.png` | **2089** | **753** | 2×2 | Référence |
| `AtlasRoquette.png` | **2089** | **518** | 1×1 | OK — plus bas car moins large / tiges plus courtes en base |

**Règle provisoire :** garder **largeur 2089 px** et **7 frames gauche → droite** pour réutiliser le même workflow de slice (grille 7 colonnes + trim alpha). La **hauteur** peut varier selon footprint et stades floraison / graines.

## Import Unity (aligné laitue)

| Paramètre | Valeur |
|-----------|--------|
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Multiple |
| Pixels Per Unit | **Laitue = 33** · **Roquette = 100** + `stageDisplayScales` sur `Roquette.asset` (graines plus grosses) |
| Mesh Type | Tight |
| Alpha Is Transparency | ON |
| Mip Maps | OFF |

## Slices

- **Exactement 7** sprites, ordre croissance : Graine → Starting → Baby → Growing → Mature → Flowering → Seedling.
- Nommage : `{Plante}_01_Graine` … `{Plante}_07_Seedling` (ex. `Roquette_01_Graine`).
- **Pivot :** coin bas-gauche du trim (`pivot 0,0` + rect serré), comme laitue — le code ancre via `GridManager.GetPlantSpriteWorldPosition`.
- **Ne pas** laisser Unity en auto-slice (9+ morceaux).
- **Pivot :** bas de la plante dans l’art (Y=0) ; **X** = centre du pied visuel, pas obligatoirement 0,5 si le trim est asymétrique.

## Hauteurs stade 5 (Mature) — indicateur

| Plante | Hauteur slice Mature (px) |
|--------|---------------------------|
| Laitue | ~330 |
| Roquette | ~392 |

Écart acceptable (espèce + footprint). Ajuster à la main en jeu via `isoSpriteViewOffset` / `spriteWorldOffset` sur `PlantDefinition` si le pied flotte.

## Prompts / Bezy

- Laitue : `Notes/Art/PROMPTS_Bezi_laitue_atlas.md`
- Roquette : `Notes/Art/PROMPTS_Bezi_roquette_atlas.md`
