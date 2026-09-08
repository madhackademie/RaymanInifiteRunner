# Prompt monde — iso cartoon (base assets)

**Création :** 2026-09-08  
**Source direction :** `Notes/Art/NOTE_graphique.md`  
**Captures auteur :** `Notes/References/images/` (veille only, **pas** runtime)

Coller le **§ Prompt générique** pour tout asset monde (plante, bac, décor).  
La laitue a son brief dédié : `PROMPT_laitue_sprite_sheet_croissance.md`.

---

## Mix (prendre / laisser)

| Couche | Jeu / capture | **Prendre** | **Laisser** |
|--------|----------------|-------------|-------------|
| **Modèles cibles** | **Township** + **The Tribez** | Village iso cartoon, bâtiments chunky, lecture pouce | Cultures **à plat** (terre plane / champ flush sol) |
| **Volume (nous)** | Biofiltre / hydro | Plante **surélevée** sur deck IBC (billes d’argile), cuve 3/4 lisible | Dalle de dirt au niveau du terrain |
| **Pose têtes** | **SpongeBob farm** `spongebob_farm_cabbage_iso.png` | Têtes légume iso 3/4, feuilles simplifiées | Champ 3×6, sol cyan, motte de **terre de champ** |
| **Polish optionnel** | **Zombie Castaways** | Couleurs vives, ombrage doux | Thème zombie, gore |
| **Grille jeu** | Notre biofiltre | Losange **2:1**, arête **26,565°** | Iso 30°, canopée top-down |
| **Matière** | Décision | **Cartoon** d’abord | Papercraft Yoshi comme look principal, low-poly 3D, pixel |

---

## Prompt générique (ChatGPT / image — coller)

Joindre les 2 captures auteur si tu les as sous la main (choux SpongeBob + village Tribez). Ne pas coller de zombie.

```
Create a single 2D game sprite for a casual mobile aquaponics farm.

STYLE LOCK (strict):
- Cartoon isometric casual mobile, NOT photoreal, NOT pixel art, NOT 3D low-poly, NOT horror.
- Graphic polish like Zombie Castaways (Vizor): vibrant saturated colors, chunky toy-like silhouettes, soft painterly shading, high readability at small size.
- Stay CARTOON and cute. NO zombies, NO gore, NO skulls, NO boneberries, NO eyeballs as crops, NO grim palette.
- Crop/object pose like SpongeBob-style iso farm veggies: 3/4 isometric heads, simplified leaves, clear silhouette.
- The plant sits on a hydroponic IBC deck (clay pebbles / grow media), NOT a flat dirt field at ground level. Township/The Tribez are the village style, but those games plant on a planar soil lot — do NOT copy that flat farm plot.
- Prop volume like Township / The Tribez buildings: readable top + walls, chunky, toy-like. The tank itself creates the 3D volume.
- Camera: 2:1 game isometric (diamond width = 2x height, edge angle 26.565 degrees from horizontal). NOT 30-degree architectural iso. NOT top-down canopy / bird's eye.
- Optional light paper-crafted edges, but the look must stay cartoon painted, not cardboard diorama.
- Transparent background ONLY (alpha PNG). No floor tile, no grid, no UI bubbles, no text, no characters unless asked.
- ALPHA PERF (strict): hard clean silhouette edges. NO outer glow, NO fog, NO haze, NO bloom, NO soft aura, NO large soft transparent fringe around the subject. Minimal semi-transparent pixels — only 1-2px anti-alias at most. No soft drop-shadow on transparent background.

SUBJECT:
[VOTRE OBJET ICI — one object only]

Output: one PNG, ~1024x1024, subject centered, same ground baseline as a farm sprite.
```

---

## Variante plante sur grille (1 plant / footprint)

Ajouter à la fin du prompt :

```
One plant only, not a full field. Sits on a tiny mound of hydroponic clay pebbles (grow media), NOT a rectangular dirt plot at ground level. Must stay readable on a 2x2 isometric cell on top of an IBC tank. No stone farm-field border.
```
