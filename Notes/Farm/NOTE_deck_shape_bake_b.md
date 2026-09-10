# Deck shape UV — bake colonnes/lignes (stratégie B)

**ID :** `[P0-FARM-DECK-SHAPE-BAKE-001]` · **2026-09-10** · **Playtest OK** (warning bake min col/row : ignorable si pose/clics OK)

## Données

- `BiofiltreLayoutDefinition.deckShapeUv` — 4 coins en UV 0–1 (origine bas-gauche du sprite).
- `deckNormalized` reste pour **Fit once** legacy uniquement.

## Workflow auteur

1. **Hiérarchie** : sélectionner la racine **Biofiltre** (pas seulement l’enfant `Grid`).
2. **Inspector** : composant **Biofiltre Layout Binder** → en bas, section **Bake grille (stratégie B)** → bouton bake.
3. **Scene** (objet Biofiltre sélectionné) : poignées **SW / SE / NE / NW** (cyan) sur le sprite IBC pour le quad ; contour cyan = `deckShapeUv`.
4. **Gizmos** activés dans la barre Scene (icône Gizmos) pour la grille verte `GridManager`.
5. Layout SO : `Assets/Data/Ferme/BiofiltreLayout_Standard10x10.asset` — champs UV ou bouton *Copier deckNormalized → deckShapeUv*.
6. Caler `IbcSprite` + enfant `Grid` — **ne pas** changer `cellSize` ni rotation iso → **Bake B** → playtest.

**Un SO par variante de bac** : dupliquer l’asset ; ne pas partager le même fichier entre deux arts différents.

## Code

- `BiofiltreGridLayoutMath.TryBakeColumnRowCountB`
- `BiofiltreDeckShapeUvUtility` (UV → monde)
- Éditeurs : `BiofiltreLayoutBinderEditor`, `BiofiltreLayoutDefinitionEditor`

## Limites (B)

- Grille **rectangulaire** pleine ; coins hors shape non masqués → backlog **cercle / masque** : `[BL-FARM-DECK-CIRCLE-MASK-001]` (`BACKLOG_deck_cercle_masque_plantable.md`).
- Si warning « dépasse avant (0,0) » : déplacer l’enfant **Grid** (handle).
