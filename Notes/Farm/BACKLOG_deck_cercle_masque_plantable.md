# Backlog — biofiltre deck circulaire (masque plantable)

**ID :** `[BL-FARM-DECK-CIRCLE-MASK-001]` · lié `[P0-FARM-GRID-PLANTABLE-MASK-001]` (option C)  
**Statut :** idée validée, **pas prioritaire** (après bake rect plus grand + feedback plante).

---

## Intention produit

- Reproduire un bac **rond** proche du **modèle physique** (terrasse / promo « t’aireau »).
- Grille gameplay : **carré** `columns × rows` (même `cellSize` canonique) généré par **bake B** comme aujourd’hui.
- **Cellules hors cercle** : inactives (non plantables, visuel grisé) — indices `(col, row)` conservés.

## Prérequis techniques (déjà posés)

- Bake B : `BiofiltreLayoutBinder` + `deckShapeUv` → `columns` / `rows`.
- À coder plus tard : `IsPlantable(cell)` + bake masque depuis polygone/cercle en UV ou monde.

## Hors scope immédiat

- Pas de warp de grille (4 coins qui déforment les losanges).
- Pas de recalcul runtime par frame — données **statiques** sur le layout SO (comme le bake B).

## Références

- `Notes/Farm/NOTE_deck_shape_bake_b.md`
- Discussion session 2026-09-10 (cercle = carré + masque).
