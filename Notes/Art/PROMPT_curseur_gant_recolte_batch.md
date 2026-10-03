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

---

## 3 — Plantation (2 sheets) — décision auteur 2026-10-03

**Polish :** idle + action séparés (comme §1–§2 récolte). Ref gant : `Assets/Art/Sprites/Farm/Cursor/Plant_Harverst_Idle_Cursor.png` (design only).

**Code (plus tard) :** bool Animator type `Planting` — idle = sheet §3.1, clic tenu = sheet §3.2.

**Sortie :** Dump `Ui/CurseurBatch/` → promo `Sprites/Farm/Cursor/` :
- Idle : `Plant_Planting_Idle_Cursor.png`
- Action clic : `Plant_Planting_Sowing_Cursor.png`

Générer **idle §3.1 d’abord**, joindre pour **§3.2**.

Dimensions export : cible doc **1536×256** ; si ChatGPT livre ~**2171×402** avec gouttières noires (comme récolte), OK — slice manuel Bezy.

### 3.1 — Idle plantation (`GlovePlantIdle`) — avant clic

```
Create one horizontal sprite sheet PNG for a casual mobile farm game cursor.

REFERENCE (mandatory):
- The attached harvest idle glove sheet shows the EXACT glove design, colors, line weight, shading, and camera angle. REDRAW that SAME gardening glove only — tan leather, green ribbed cuff, thick black outline, soft cel shading. Do NOT copy the grass tuft from the reference.

STYLE LOCK (strict):
- Cartoon isometric 3/4, casual mobile, chunky toy silhouette, vibrant saturated colors, soft painterly shading inside the shape.
- Like Township / The Tribez readability and Zombie Castaways color polish. NO zombies, NO gore, NO grim palette.
- NOT photoreal, NOT pixel art, NOT 3D low-poly, NOT top-down, NOT a UI icon on a white square.
- Camera: 2:1 game isometric (edge angle 26.565 degrees). NOT 30-degree architectural iso.
- Transparent background ONLY. NO white backdrop, NO floor tile, NO grid, NO text, NO frame numbers.

EDGE LOCK / ALPHA PERF (strict):
- Pure black (#000000) outer outline. Hard pixel-clean alpha edge. NO anti-aliased gray fringe.
- NO outer glow, NO fog, NO haze, NO bloom, NO soft aura, NO soft transparent drop shadow.
- Shade with opaque paint inside the sprite only.

SPRITE SHEET (strict):
- Exactly 6 equal square frames, ONE horizontal row, left to right.
- Total size 1536 x 256 (or match reference harvest sheet proportions ~2171 x 402 if needed).
- Subject stays inside margins so the black outline is never clipped.

ANIMATION LOCK (strict):
- One gardening glove (SAME as reference) + a small low mound of dark brown soil with a visible round planting HOLE / dent beside or below the hand (cartoon, not photoreal).
- Hand pose: open palm facing UP toward the sky (cupped palm), fingers slightly curved — NOT a pinch, NOT thumb-and-index grip, NOT fist.
- 2–3 tiny brown seeds rest in the cupped palm the entire loop. They bounce IN PLACE on the palm (small vertical hop, a few millimeters), never leaving the palm silhouette.
- The glove WRIST stays on the same pixel every frame: horizontal center of the cell, wrist bottom ~32 px above the cell bottom.
- The glove does NOT scale, rotate as a whole, or change colors. Only fingers curl slightly, palm cup depth, and seed positions on the palm move.
- Idle loop = gentle bounce of seeds on the open upward palm above the empty hole — playful but subtle, mobile-readable.
- FORBIDDEN in this idle sheet: pinch grip, seeds falling downward, seeds thrown out of the hand, seeds released into the hole, seeds on the soil mound, empty palm, planting tap, palm turned down on dirt.
- The hole stays empty and unchanged — no seed ever enters it during idle.
- Frame 6 must lead back into frame 1 with no pop.

FRAMES left to right:
1) Open palm facing up toward the sky, cupped gently; two or three small brown seeds resting on the palm; a cartoon soil mound with an empty round planting hole visible below or beside the hand; relaxed wrist anchor.
2) Same palm-up pose; fingers curl slightly deeper into a cup; seeds press a little into the palm — start of the bounce (wind-up).
3) Seeds at the peak of a tiny hop still entirely inside the cupped palm — not airborne outside the hand, not falling toward the hole.
4) Seeds settle back onto the open upward palm; no pinch, no thumb-and-index grip; hole still empty.
5) Second, smaller on-palm bounce — seeds lift slightly again within the palm only.
6) Match frame 1 — palm up, seeds on palm, empty hole unchanged; ready to loop seamlessly into frame 1.

GUARDRAIL: Cupped palm UP only — NO pinch. Seeds bounce in place ON the palm. Hole stays empty. NO drop, NO plant, NO pat.

Export one PNG, transparent background.
```

### 3.2 — Plantation (`GlovePlantSowing`) — clic tenu

Joindre **`Plant_Planting_Idle_Cursor.png`** (sheet idle validée). Coller :

```
Create a second horizontal sprite sheet, SAME glove and SAME soil mound as the attached planting idle sheet.

REFERENCE (mandatory):
- Attached idle sheet = exact glove (tan leather, green cuff, black outline), same mound, same hole position, same wrist anchor, same sheet height and frame spacing.

STYLE LOCK, EDGE LOCK, and sheet layout match the idle planting sheet:
- Cartoon isometric 3/4, chunky, vibrant, black #000000 outline, hard alpha, no glow, no shadow, no text, no grid.
- Exactly 6 frames, one horizontal row, same total pixel size as the idle sheet (match width/height/gutters).
- SAME glove scale and wrist pixel as idle. Only the gesture changes. Frame 6 loops to frame 1 of THIS action sheet.

ANIMATION LOCK (strict):
- This sheet is CLICK-HOLD planting only — seeds leave the hand here (not in idle).
- Start from the idle end pose: cupped palm had seeds above the empty hole; now the planting action begins.
- End state: patched soil mound, no visible seeds, hole covered.

FRAMES left to right:
1) Palm tilts from cup-up toward the hole; seeds slide off the palm and fall into the empty hole (first frame seeds leave the hand).
2) Seeds inside the hole; fingers opening away; palm not flat on dirt yet.
3) Open palm hovers flat above the mound, fingers spread — about to pat.
4) First tap: palm presses soil down; hole covered; slight squash on the mound.
5) Lighter second pat — neat patched mound, no visible seeds.
6) Hand lifts slightly beside the tidy mound; ready to loop to frame 1 of this action sheet.

GUARDRAIL: Same art line as idle sheet. NO idle bounce on this sheet. NO empty hole after frame 2.

Export one PNG. Transparent background or same black gutters as idle/harvest cursor sheets.

Save as planting sowing cursor sheet for Unity slice (6 frames).
```
