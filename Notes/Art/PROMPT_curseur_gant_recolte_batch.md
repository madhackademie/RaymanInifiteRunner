# Curseur gant — sprite sheets (charte)

**Charte :** `Notes/Art/NOTE_graphique.md` · base `Notes/Art/PROMPT_assets_monde_iso.md`  
**Dump :** `Assets/Art/Assets Store Dump/Ui/CurseurBatch/` — pas `Assets/Art/Sprites/` tant que non validé.  
**Code :** deux clips plus tard, sans nouveau script. `Harvesting` false = `GloveIdle`, true = `GloveHarvest`.

Générer **la sheet idle d’abord**. Pour la sheet récolte, **joindre le PNG idle** comme référence (même gant, même pivot).

---

## Format (les deux sheets)

| Paramètre | Valeur |
|-----------|--------|
| Frames | **6** (boucle) |
| Case | **256 × 256** px, carrées, égales |
| Sheet | **1536 × 256** (6 cases, **une** rangée) |
| Ordre | gauche → droite, frame 1 à frame 6 |
| Fond | transparent |
| Import Unity (plus tard) | Sprite Mode Multiple, grille 256, **PPU 256** (1 case = 1 unité monde), mipmaps off, alpha is transparency |

6 frames : assez pour anticiper, tirer, revenir. 8 frames casse trop souvent le recalage. 4 frames donne un à-coup en boucle.

256 px : lisible au pouce, plus petit qu’une plante (512). Le gant ne doit pas couvrir une case de grille.

---

## Contraintes d’animation (dans le prompt)

1. **Recalage** — le poignet du gant est sur le **même pixel** à chaque frame (centre X de la case, bas du gant à 32 px du bord bas). Seuls les doigts et l’herbe bougent.
2. **Échelle** — le gant ne grossit pas, ne rétrécit pas, ne saute pas de case.
3. **Boucle** — la frame 6 est juste avant la frame 1. Aucun saut au retour.
4. **Même personnage** — les deux sheets partagent couleur, contour, taille et pivot. Seul le geste change.
5. **Contour** — outline noir `#000000`, bord alpha dur, pas de frange grise, pas de glow, pas d’ombre portée transparente.
6. **Vide** — pas de chiffre, pas de ligne de grille, pas de fond, pas de sol, pas de personnage.

---

## 1 — Idle (`GloveIdle`) — on attend le clic

```
Create one horizontal sprite sheet PNG for a casual mobile farm game cursor.

STYLE LOCK (strict):
- Cartoon isometric 3/4, casual mobile, chunky toy silhouette, vibrant saturated colors, soft painterly shading inside the shape.
- Like Township / The Tribez readability and Zombie Castaways color polish. NO zombies, NO gore, NO grim palette.
- NOT photoreal, NOT pixel art, NOT 3D low-poly, NOT top-down, NOT a UI icon on a white square.
- Camera: 2:1 game isometric (edge angle 26.565 degrees). NOT 30-degree architectural iso.
- Transparent background ONLY. NO black backdrop, NO white backdrop, NO floor, NO grid, NO text, NO frame numbers, NO drop shadow.

EDGE LOCK / ALPHA PERF (strict):
- Pure black (#000000) outer outline. Hard pixel-clean alpha edge. NO anti-aliased gray fringe.
- NO outer glow, NO fog, NO haze, NO bloom, NO soft aura.
- Shade with opaque paint inside the sprite only.

SPRITE SHEET (strict):
- Exactly 6 equal square frames, ONE horizontal row, left to right.
- Total size 1536 x 256. Each frame is 256 x 256. No gaps, no gutters, no labels.
- Subject stays inside an 16 px margin so the outline is never cut by the cell edge.

ANIMATION LOCK (strict):
- One gardening glove (tan leather, green cuff) and one small tuft of bright green grass.
- The glove WRIST stays on the same pixel every frame: horizontal center of the cell, wrist bottom 32 px above the cell bottom.
- The glove does NOT scale, rotate as a whole, or change colors between frames. Only fingers and grass bend.
- Gentle idle loop: the glove tugs the grass, the tuft stays attached.
- Frame 6 must lead back into frame 1 with no pop.

FRAMES left to right:
1) Glove hovers, fingers open, grass tuft upright.
2) Fingers close on the tuft.
3) Small upward tug, grass bends.
4) Hold the tug.
5) Ease back down.
6) Almost frame 1, ready to loop.

Export one PNG, transparent background.
```

## 2 — Récolte (`GloveHarvest`) — clic tenu

Joindre la sheet idle. Coller :

```
Create a second horizontal sprite sheet, same glove as the attached idle sheet.

STYLE LOCK, EDGE LOCK, and sheet size are identical to the idle sheet:
- Cartoon isometric 3/4, chunky, vibrant, black #000000 outline, hard alpha, no glow, no shadow, no text, no grid, no background.
- Exactly 6 equal 256 x 256 frames, one row, total 1536 x 256.
- SAME glove design, SAME colors, SAME scale, SAME wrist pixel (cell center X, wrist 32 px above the cell bottom).
- Only the gesture is stronger. Frame 6 leads back into frame 1.

FRAMES left to right:
1) Fingers grip the grass tuft.
2) Yank upward.
3) A few blades detach.
4) Tuft high, loose blades in the air.
5) Short shake.
6) Return toward the grip, ready to loop.

Export one PNG, transparent background.
```
