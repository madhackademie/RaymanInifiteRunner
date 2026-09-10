# IBC / biofiltre — gabarit deck plantable (iso 2:1)

**Création :** 2026-09-09  
**Charte :** `Notes/Art/NOTE_graphique.md` · `Notes/Art/PROMPT_assets_monde_iso.md`  
**Runtime :** `BiofiltreLayoutDefinition` · `Assets/Data/Ferme/BiofiltreLayout_Standard10x10.asset`  
**Câblage :** `Notes/Farm/CABLAGE_biofiltre_ibc_grille_bezi.md`

---

## Principe (factory → énorme carte)

| Couche | Qui décide | Stable sur toute la carte |
|--------|------------|---------------------------|
| **Gameplay** | `BiofiltreLayoutDefinition` : `columns`, `rows`, `cellSize` | Même `cellSize` partout (ex. `1` × scale enfant `Grid` `0.55`) |
| **Art IBC** | 1 sprite + `deckShapeUv` (quad UV) **par variante** | Grand bac = **nouveau PNG** ; `deckNormalized` = legacy Fit once |
| **Taille grille** | Bake B sur prefab (`BiofiltreLayoutBinder`) | `columns` / `rows` depuis le quad — `cellSize` inchangé |
| **Calage visuel** | Transforms enfants `Grid` + `IbcSprite` sur le prefab | Une fois par variante (Prefab Mode) |
| **Plantes** | `PlantDefinition.footprint` variable + ancrage code | Même règle tous bacs |

**Ne pas** compter sur `FitToGrid` runtime en iso (AABB rect ≠ losange deck). Bouton éditeur « Fit once » = point de départ seulement.

---

## Gabarit deck (à joindre aux prompts IA)

Créer / regénérer :  
`Assets/Art/Assets Store Dump/ElementProd/Biofiltre/DECK_TEMPLATE_iso_2x1.png`

Spécifications du PNG transparent (1024×1024 recommandé) :

```
┌─────────────────────────────────────┐
│                                     │
│            ▲ sommet nord            │
│           ╱ ╲                         │
│          ╱   ╲   LOSANGE VERT         │
│         ╱     ╲  (zone plantable)     │
│        ╱       ╲ largeur = 70% canvas │
│       ╱         ╲ ratio 2:1 (h = w/2) │
│      ╱___________╲ arêtes 26,565°     │
│     ◇─────────────◇                   │
│          ▼ sommet sud                 │
│                                     │
│   (rien en dessous — cuve IA)       │
└─────────────────────────────────────┘
```

| Paramètre | Valeur |
|-----------|--------|
| Losange plantable | **70 %** largeur canvas, centré horizontalement |
| Ratio | largeur = **2 ×** hauteur |
| Angle arête | **26,565°** (`atan(0.5)`) |
| Couleur guide | vert semi-transparent `#00FF0040` (export sans pour runtime) |
| Interdit dans l’asset final | grille dessinée, plantes, UI, texte |

Script de regen optionnel : à faire manuellement dans Photopea / Figma une fois, puis réutiliser pour tous les IBC.

---

## Prompt ChatGPT — coller tel quel

**Étape A :** joindre `DECK_TEMPLATE_iso_2x1.png` + ref style `IbcIso.png` (ligne art projet).  
**Étape B :**

```
Create a single 2D game sprite for a casual mobile aquaponics IBC grow tank.

REFERENCE (strict):
- Join DECK_TEMPLATE_iso_2x1.png: the clay-pebble grow deck MUST align exactly with the green diamond overlay (2:1 isometric, 26.565 degree edges).
- Join IbcIso.png only for ART STYLE (cartoon iso 3/4, chunky, vibrant) — do NOT copy its exact proportions if they conflict with the template.

SUBJECT:
- One IBC hydroponic tank, 3/4 isometric view (readable top deck + one side wall).
- The plantable surface is ONLY the flat clay-pebble deck inside the template diamond.
- Tank body, cage, pipes BELOW / AROUND the deck — must NOT cover the diamond.
- Cartoon mobile farm, NOT photoreal, NOT pixel, NOT top-down, NOT 30-degree architectural iso.

DECK RULES (strict):
- Deck diamond width = 2x height (game iso 2:1).
- Deck centered horizontally; occupies ~70% of canvas width.
- No grid lines drawn on the deck. No crops. No lettuce.
- Transparent background ONLY. Hard alpha silhouette.

OUTPUT:
- One PNG ~1024x1024 or 2048x2048, transparent background.
```

---

## Checklist validation (avant promo Dump → Sprites)

```
[ ] Superposer DECK_TEMPLATE sur le PNG exporté — losange billes = losange guide
[ ] Pas de grille peinte dans l’image
[ ] Pas de plante / UI / texte
[ ] Fond transparent, alpha propre (pas de halo)
[ ] Mesurer deckNormalized (§ ci-dessous) — NE PAS copier une autre variante
[ ] Créer ou mettre à jour BiofiltreLayoutDefinition (columns/rows/cellSize + sprite + deck UV)
[ ] Prefab : BiofiltreLayoutBinder → layout SO ; caler Grid + IbcSprite à la main ; playtest
```

---

## Mesurer `deckNormalized` (UV 0–1)

Rectangle **axis-aligned** autour des billes visibles, en coordonnées UV du sprite importé (origine **bas-gauche** du rect sprite).

1. Unity → Sprite Editor ou Photopea sur le PNG importé.  
2. Noter `x, y, width, height` en fraction du sprite (0–1).  
3. Renseigner sur le `BiofiltreLayoutDefinition` de **cette** variante.

Ex. standard actuel (`IbcIso.png`) : `(0.059, 0.5239, 0.8844, 0.441)` — **spécifique à cet asset**.

---

## Nouvelle variante (grand bac factory)

1. Dupliquer `BiofiltreLayout_Standard10x10.asset` → ex. `BiofiltreLayout_Factory24x18.asset`.  
2. Ajuster `columns` / `rows` (plus de cellules, **même `cellSize`**).  
3. Générer IBC avec le prompt + gabarit → Dump → promo `Sprites/Farm/Biofiltre/`.  
4. Assigner `ibcSprite` + `deckNormalized` mesuré sur le layout SO.  
5. Dupliquer prefab `Biofiltre` → caler `Grid` + `IbcSprite` (transforms) une fois.  
6. Playtest : losanges sur billes, pose plante, clic footprint.

---

## Plantes sur le deck

- Footprint variable par `PlantDefinition` (1×1, 2×2, L…).  
- Ancrage sprite : **sommet sud** de la cellule footprint la plus « avant » (`col+row` max) — code `GridManager.TryGetFootprintFrontSouthVertex`.  
- `isoSpriteViewOffset` : micro-triche **par espèce** seulement (pivot art), pas pour compenser un IBC mal calé.

Prompt plantes : `PROMPT_laitue_sprite_sheet_croissance.md` (motte billes, baseline commune).
