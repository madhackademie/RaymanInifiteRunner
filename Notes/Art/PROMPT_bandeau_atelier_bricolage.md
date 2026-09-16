# Prompt art — bandeau atelier / Bricolage

**Date :** 2026-09-16  
**Usage :** fond du bandeau hub **Bricolage** (remplace le papier peint rouages + feuilles, trop plat).  
**Dump :** `Assets/Art/Assets Store Dump/Ui/Tab_Plus/`  
**Promo (après OK auteur) :** `Assets/Art/Sprites/UI/FeaturesHub/` (ou `Ui/Tab_Plus/` si le bandeau reste local au hub Plus)  
**Charte :** cartoon cozy rustique (`PROMPT_generation_icones.md` §1) — **pas** iso monde, **pas** horreur.  
**Refs auteur (hors repo / 2 slots ChatGPT) :** atelier bois aquarelle · savant fou cartoon · garage bras KUKA · mockup titre *Bricolage*.

## Combo d’images (2 max)

| Slot | Joindre | Pourquoi |
|------|---------|----------|
| **A (recommandé)** | Atelier bois (image 1) + **truite du jeu** `Dump/Poisson/cartoon-rainbow-trout-…webp` | Garde le décor + le corps exact de la chimère. Bras / robot / poireau = texte. |
| B (chat déjà lancé) | Atelier bois + savant fou (image 2) | OK si déjà envoyé. Le prompt ci-dessous **interdit** le bonhomme enragé, le blob orange et le garage sombre. |
| À **ne pas** coller en 3ᵉ | Bras mécanique (image 3) | Pas de slot : tout le bras est décrit dans le prompt. |
| Hors génération | Mockup *Bricolage* (caisse à outils + titre or) | Réf **format** bandeau + zone titre à droite — pas le contenu de scène. |

Si le savant / l’éclairage sombre fuite : **nouveau chat**, combo **A**.

## Prompt à coller (anglais)

```
Edit using the two attached images as references ONLY as specified below.

TASK: create ONE wide mobile-game UI BANNER illustration (landscape ~5:1, e.g. 1920x384). Isolated vignette, fully transparent background outside the scene (or clean white). NO watermark, NO stock-site marks, NO text, NO letters, NO numbers, NO title.

STYLE LOCK (game charter — strict):
- 2D casual mobile farming game, cozy cartoon, rustic light-brown WOOD, thick dark outlines, smooth cel shading, warm saturated colors, punchy readable silhouettes.
- Same family as a wooden toolbox UI banner (chunky, toy-like, high punch).
- NOT photoreal, NOT 3D render, NOT pixel art, NOT watercolor stock, NOT grimdark, NOT horror.
- NO blood, NO gore, NO stitches dripping, NO zombies, NO angry human face.
- Lighting: warm daylight from a window, cozy workshop — NOT fluorescent garage, NOT night.

KEEP FROM IMAGE 1 (workshop setting only):
- The rustic wooden L-shaped lab/workshop: timber beams, window with blue sky, shelves of bottles, brass microscope, glassware, upper drawers, papers on the wall, hanging green lamp.
- Keep the CHARMING clutter of a handmade aquaponics tinkerer lab.
- Do NOT copy watercolor/paper texture or any watermark from image 1.
- Recrop as a WIDE banner: we see the workshop as a BACKGROUND, not a full square room.

FROM IMAGE 2 (mood only — do NOT copy characters):
- Keep only the “mad inventor tinkering at a workbench” energy.
- FORBIDDEN to copy: the angry human, the orange blob creature, the dark pipes, the grim industrial lighting.

ADD — INDUSTRIAL ROBOT ARM (no reference image; describe exactly):
- A large 6-axis workshop manipulator arm, KUKA / factory-robot type, occupying the LEFT third of the banner.
- Base pedestal planted on the far LEFT, bolted at the operating-table edge.
- 4–5 chunky painted-metal segments in warm ORANGE / amber (farm-game accent, not rust-brown).
- Thick BLACK joint rings / accordion bellows between segments.
- Visible cartoon hydraulic hoses and cable bundles along the arm.
- Wrist ends in a two-finger metal GRIPPER (clamp), holding the chimera above the table.
- Chunky toy silhouette, thick outline, cel-shaded. NOT a human arm, NOT a slim surgical robot, NOT a crane.

ADD — OPERATING TABLE (covers the bottom of the workshop):
- A wide wooden + metal operating / work table runs along the BOTTOM of the banner, in the foreground.
- It MASKS the stool AND the lower ~two drawer rows of the workshop (we still see upper drawers, microscope, shelves, window).
- Straps, clamps, small rustic tools (wrench, screwdriver, saw) on the table — bricolage, not a hospital.
- The table is the visual “punch” strip at the bottom.

ADD — FISH-VEGETABLE CHIMERA on the table (Frankenstein lab, but CUTE):
- Body: a plump cartoon RAINBOW TROUT identical to our game fish (if a trout image is attached: copy that body exactly — olive-gold back, pink-orange lateral stripe, black + magenta spots, cream belly, big round amber eye, orange fins, friendly slightly-open mouth). If no trout image: use that description.
- Head REPLACED by a cartoon LEEK head: white shaft as neck, long green flag-like leaves as “hair”, cute vegetable face (small eyes, tiny smile). Funny mad-science hybrid, NOT scary, NOT realistic anatomy.
- Optional tiny cute bolts at the neck (Frankenstein gag) — no gore.
- The orange robot gripper holds / supports this chimera over the table, left-center.

ADD — PLANT IN A GIANT SPECIMEN FLASK (in-vitro / formol vibe, cozy):
- One HERO oversized glass carboy / specimen jar on the table or just behind it (mid-right of the action, still left of the empty title zone).
- Inside: a leafy plant (lettuce-like clone) fully submerged / rooted in slightly green-tinted clear liquid (nutrient / “formol” look), tiny bubbles, visible roots. Cork or brass lid.
- Scientific cloning jar, not a horror organ jar.

REPLACE THE HUMAN with a ROBOT:
- A small friendly workshop ROBOT (round head, antenna, wood + metal body, maybe tiny overalls). Cute farm-lab assistant, NOT a person, NOT the angry scientist.
- Stands at the table, slightly right of center, looking at the chimera. Smaller visual weight than the arm + chimera.

COMPOSITION (banner, left → right):
1) LEFT 28%: orange industrial arm coming in from the left edge, gripper over the table.
2) BOTTOM 35%: operating table spanning most of the width (masks stool + 2 lower drawer rows).
3) CENTER: leek-trout chimera on the table + cute robot.
4) MID: giant plant flask; rustic workshop continues behind (window, microscope, upper shelves).
5) RIGHT 35–40%: keep this zone QUIET and less busy (soft workshop wall / parchment wood) so a gold UI title can overlay later. No important details, no character, no flask here.

OUTPUT: one PNG, wide banner, transparent outside the vignette, hard clean silhouette edges (1–2px AA max), no outer glow, no fog.
```

## Base validée — `FirstTryBandeauAtelier.png`

Rendu Dump 2026-09-16 : bras, table, fiole, robot, zone droite OK. **Bug :** truite et poireau = 2 persos côte à côte (le bras tient le poireau).  
Itérations = **une** passe à la fois, crédits ChatGPT.

## Passe 1 — fusion chimère (à coller maintenant)

**Joindre :** `FirstTryBandeauAtelier.png` + collage tête-poireau / corps-truite (découpe auteur).

```
Edit IMAGE 1 (the full workshop banner). Keep the entire scene identical: orange robot arm, wooden table, tools, plant jar, cute robot, shelves, window, empty right wall. Same cartoon rustic style, same colors, same camera, transparent outside the vignette. NO text. NO watermark.

ONE change only: the trout and the leek are currently TWO separate characters. Replace them with ONE single fused creature.

Use IMAGE 2 only as anatomy reference (ignore the jagged cutout edges, ignore transparency holes):
- ONE organism, left-to-right: rainbow TROUT BODY (olive-gold back, pink stripe, black + magenta spots, cream belly, orange tail and fins — keep the trout body from image 1).
- NO fish head, NO fish mouth, NO fish eyes.
- Where the neck would be, attach the LEEK HEAD from image 2: round cream-white bulb, cute kawaii face (two shiny black eyes, tiny blush, small smile), long green leek leaves fanning up and to the right like hair.
- Seamless join at the neck (white leek shaft grows out of the trout shoulders). Optional tiny cute Frankenstein bolts at the join — no gore, no stitches dripping.
- Same size as the current trout+leek pair combined. Still lying on the two small stands on the table.
- The orange gripper now holds this ONE chimera (grip the green leaves or the back — not a second vegetable).

Delete the separate trout. Delete the separate leek. Do not add a second fish. Do not restyle the rest of the banner.
```

Prochaine passe (après OK fusion) : éventuellement pose du bras / échelle chimère / détails table — **pas** dans cette passe.

## Passe 2 — inpaint zone chimère (2 têtes encore là)

Le modèle garde la gueule de truite + le poireau à côté. **Ne pas** régénérer le bandeau entier.

**Dans ChatGPT :** bouton **Modifier** → sélectionner **uniquement** truite + poireau + pince du bras (pas la fiole, pas le robot, pas le mur).  
**Joindre en 2ᵉ image** le collage fusion si le slot le permet.

```
Inpaint ONLY the selected area. Do not change anything outside the selection.

The current art is WRONG: it shows TWO heads (a fish face AND a leek face). That is forbidden.

Redraw as ONE chimera nicknamed Plantfischstein:
- LEFT half = trout BODY only (tail, dorsal fin, pink spotted flank, cream belly, orange pectoral fins). Stop the fish at the shoulders / gill line.
- The fish HEAD is completely gone. ZERO fish eyes. ZERO fish mouth. ZERO fish lips. ZERO fish snout. Erase them.
- RIGHT half = the leek IS the only head: cream-white round bulb sitting where the fish head was, cute kawaii face on that bulb (two black shiny eyes, small smile), green leek leaves as hair fanning up-right.
- One neck, one face, one silhouette. The white leek shaft grows out of the trout body like a swapped head (Frankenstein head-transplant gag, cute, no gore).
- Orange gripper holds the green leaves of THIS same creature. Not a second vegetable.

If you draw any fish face, the image is a failure. One creature only.
```

Critère OK : silhouette unique, **un seul visage** (poireau), queue de truite à gauche.

## État 2026-09-16 soir — crédits ChatGPT = 0

- Base : `FirstTryBandeauAtelier.png` à garder.
- Passe 2 **non lancée** (plus de crédits). Sélection auteur : un lasso truite+poireau+pince, bocal dehors — **suffisante** pour demain.
- **Demain `[P0-ART-BANDEAU-ATELIER-001]` :** relancer Passe 2. Si encore 2 têtes → génération **Cursor** (pas ChatGPT), PNG dans `Dump/Ui/Tab_Plus/`.

## Passe de correction (si ça dérape)

Coller **une** phrase à la fois, image résultat jointe :

```
Keep this exact composition. Replace any human with the small cute workshop robot. Keep the LEFT orange 6-axis industrial gripper arm. Keep the leek-head trout-body chimera cute, not scary. Recolor lighting to warm window daylight. Empty the right 40% for a title overlay. No text. No watermark.
```

Bras trop grêle / trop gris :

```
Make the left robot arm much chunkier: thick orange painted segments, black joint rings, two-finger gripper holding the chimera. Factory manipulator, not a human arm.
```

Truite pas la nôtre :

```
Redesign the fish body to match the attached trout: plump cartoon rainbow trout, olive-gold back, pink-orange stripe, black and magenta spots, cream belly, big amber eye, orange fins. Keep the leek head.
```

## Après génération

1. PNG → `Assets/Art/Assets Store Dump/Ui/Tab_Plus/BandeauAtelier_Bricolage_YYYYMMDD.png`  
2. **Pas** de tiling (ce n’est plus un papier peint) : Image Unity *Simple* / *Preserve Aspect*, titre **Bricolage** en TMP or par-dessus la zone droite.  
3. Promo Sprites seulement après OK auteur.
