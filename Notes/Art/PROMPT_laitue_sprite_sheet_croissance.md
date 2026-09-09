# Laitue — sprite sheet croissance (7 stades) + prompt ChatGPT

**Création :** 2026-09-08 · **MAJ :** 2026-09-08 (ref runtime + charte iso)  
**Charte art :** `Notes/Art/NOTE_graphique.md`  
**Base prompt monde :** `Notes/Art/PROMPT_assets_monde_iso.md`  
**Cible code :** `PlantDefinition` laitue (`Assets/Data/Ferme/Laitue.asset`) — pattern **Leafy**  
**Footprint jeu :** 2×2 cellules · grille **iso 2:1** (`main`)  
**Backlog :** `Notes/Art/PROMPT_generation_icones.md` → **W2**

---

## 0) Règle charte (à lire avant ChatGPT)

| ✅ Cible | ❌ Interdit |
|---------|------------|
| **Iso 3/4 cartoon** (losange jeu 2:1, arête **26,565°**) | Canopée top-down / bird's eye (`Canopee/` = **ne pas joindre**) |
| Polish **Zombie Castaways** (couleurs, chunky) **sans** zombie | Thème mort-vivant, gore, pixel, photoreal |
| **Reprendre l’espèce + progression** des sprites **runtime actuels** (tableau §3) | Copier le low-poly facetté ou le fond noir des anciens PNG |
| Pose iso 3/4 sur **media hydro** (billes argile), deck IBC | Champ terre plat Township, sol cyan SpongeBob |
| **Une** laitue par frame, footprint 2×2 | Bordure pierre, grille visible, UI |
| Fond **transparent** (alpha) | Fond noir · **glow / brouillard / halo** autour du sujet |

---

## 1) Les 7 stades (ordre = frames gauche → droite)

| Frame | Code Unity | Libellé UI | Récolte | Sprite **runtime actuel** (`Laitue.asset`) |
|-------|------------|------------|---------|---------------------------------------------|
| **1** | `Graine` | Graine | — | `0_GraineGermé.png` |
| **2** | `Starting` | Germination | — | `01_StartingPlant.png` |
| **3** | `Baby` | Plantule | — | `02_BabyLaituce_image.png` |
| **4** | `Growing` | Croissance | — | `03_GrowingLaituce_image.png` |
| **5** | `Mature` | Mature | **Oui** → `laitue_mature` | `04_MatureLaituce_image.png` ★ **ancre espèce** |
| **6** | `Flowering` | Floraison | — | `05_FlowerLaituce_image.png` |
| **7** | `Seedling` | Graines | **Oui** → `laitue_seed` | `06_SeedlingLaituce_image.png` |

Dossier : `Assets/Art/Sprites/Plantes/Laitue/` (vue **3/4** actuelle — **pas** `Canopee/`).

**Cycle Leafy :** … → **Mature** (feuilles) → Flowering → **Seedling** (graines).

---

## 2) Sprite sheet de référence runtime (déjà généré)

Fichier prêt à joindre à ChatGPT — **7 stades en une passe**, assemblés depuis les PNG séparés de `Laitue.asset` :

**`Assets/Art/Assets Store Dump/Plantes/Laitue/Laitue_Runtime_Ref_7stades.png`**  
(3584×512 · 7×512 · ordre Graine → Seedling)

Regénérer après changement des sprites runtime :

```powershell
py -3 scripts/build_plant_runtime_ref_sheet.py
```

Script : `scripts/build_plant_runtime_ref_sheet.py` (réutilisable pour d’autres plantes plus tard).

### Fichiers source (1 sprite par stade, pas d’atlas en jeu)

| Frame | Fichier `Sprites/Plantes/Laitue/` |
|-------|-----------------------------------|
| 1–7 | `0_GraineGermé.png` … `06_SeedlingLaituce_image.png` (voir §1) |

---

## 3) Autres références (après la base runtime)

| # | Fichier | Rôle |
|---|---------|------|
| **R0** | `Assets/Art/Assets Store Dump/Plantes/Laitue/Laitue_Runtime_Ref_7stades.png` | **Obligatoire** — espèce + 7 stades runtime en **un seul PNG** (généré par `scripts/build_plant_runtime_ref_sheet.py`) |
| **R1** | `Assets/Art/Sprites/Farm/Biofiltre/IbcIso.png` | Ligne art **projet** iso 2:1 cartoon |
| **R2** | `Notes/References/images/spongebob_farm_cabbage_iso.png` | Pose iso 3/4 tête — **pas** le sol cyan ni le champ 3×6 |
| **R3** | `Notes/References/images/tribez_village_iso.png` | Volume chunky (optionnel) |

**Ne pas joindre :** `Canopee/*.png` (top-down, non branché au runtime).

---

## 4) Sprite sheet — sortie attendue

```
┌────────┬────────┬────────┬────────┬────────┬────────┬────────┐
│   1    │   2    │   3    │   4    │   5    │   6    │   7    │
│ Graine │ Germ.  │ Plant. │ Croiss.│ Mature │ Fleur  │ Graines│
└────────┴────────┴────────┴────────┴────────┴────────┴────────┘
```

| Paramètre | Valeur |
|---------|--------|
| Format | PNG **transparent** |
| Grille | **1 × 7** cases carrées égales |
| Taille | **3584 × 512** (7×512) — ou test **1792 × 256** |
| Vue sortie | **Iso 3/4 cartoon** (charte) — **pas** recopier l’angle low-poly des refs |

**Dump sortie :** `Laitue_Croissance_Sheet_YYYYMMDD.png`  
**Promo :** `Laitue_01_Graine.png` … `Laitue_07_Seedling.png` → `Sprites/Plantes/Laitue/`

---

## 5) Prompt ChatGPT — coller tel quel

**Étape A :** joindre **R0** `Laitue_Runtime_Ref_7stades.png` + **R1** `IbcIso.png`.  
**Étape B :** coller :

```
Create a single horizontal sprite sheet PNG for a casual mobile aquaponics farm.

REFERENCE IMAGES (read carefully):
- The attached CURRENT GAME lettuce sprites (7-stage reference sheet OR individual runtime sprites) show the EXACT species, growth progression, and stage logic we already use in Unity. Keep the same lettuce identity (ruffled head shape, leaf count progression, flowering stalks, seed pods) and the same left-to-right growth order.
- REDRAW them in the NEW target style below. Do NOT copy their low-poly 3D facets, harsh CGI look, or black background. Do NOT switch to top-down canopy / bird's eye (we are NOT using our Canopee top-down crops).
- The attached IbcIso.png is the target ART LINE for our game world: cartoon iso 2:1, bold readable shapes, soft painterly shading.

STYLE LOCK (strict):
- Cartoon isometric 3/4 plant sprites for a 2:1 game grid (diamond width = 2x height, edge angle 26.565 degrees from horizontal).
- NOT top-down canopy. NOT 30-degree architectural iso. NOT low-poly facets. NOT pixel art. NOT photoreal. NOT horror.
- Polish like Zombie Castaways (Vizor): vibrant saturated colors, chunky toy silhouettes, soft shading, mobile-readable.
- NO zombies, NO gore, NO grim palette.
- Each plant sits on a tiny mound of hydroponic clay pebbles (grow media on an IBC deck), NOT flat Township dirt at ground level.
- Transparent background ONLY. NO black, NO white, NO floor tile, NO grid, NO text.
- ALPHA PERF (strict): hard clean silhouette on every frame. NO outer glow, NO fog, NO haze, NO bloom, NO soft aura, NO mist around the lettuce. NO soft transparent drop shadow on the background. Minimal semi-transparent pixels — at most 1-2px anti-alias on leaf edges. Shade with opaque paint inside the sprite, not with transparent halos outside.

SPRITE SHEET OUTPUT (strict):
- Exactly 7 equal square frames, ONE horizontal row, left to right, 3584 x 512 total (512 per frame).
- Same ground baseline every frame. Progressive scale: 1 smallest → 5 largest head → 6-7 taller stalks.
- One lettuce per frame.

THE 7 FRAMES must match the attached runtime reference progression:
1) GRAIN — like reference frame 1 (germinating seed / split seed).
2) GERMINATION — like reference frame 2 (cotyledons / young sprout).
3) BABY — like reference frame 3 (~40% mature size).
4) GROWING — like reference frame 4 (~65%).
5) MATURE HARVEST — like reference frame 5 (full head, same species as current game lettuce).
6) FLOWERING — like reference frame 6 (yellow flowers on stalks).
7) SEED STAGE — like reference frame 7 (seed pods, falling seeds).

Export one PNG sprite sheet, transparent background.
```

### Post-prod Unity (après génération)

1. **Sprite Editor** → **Trim** sur chaque slice (enlever marge alpha vide).
2. Import : **Alpha Is Transparency** ON · **Generate Mip Maps** OFF (plantes monde).
3. Si halo résiduel : Photopea → calque alpha durci, ou seuil alpha avant import.

---

## 6) Après génération

1. Dump → alpha OK (pas halo noir).
2. 7 slices → `Laitue_01` … `Laitue_07`.
3. Réassigner `Laitue.asset`.
4. Playtest biofiltre iso 2×2.
5. W2 → `promu`.

---

## 7) Checklist

```
[ ] Laitue_Runtime_Ref_7stades.png joint à ChatGPT (+ IbcIso)
[ ] Pas Canopee top-down
[ ] Sortie iso cartoon + fond transparent
[ ] Laitue.asset mis à jour
[ ] Playtest grille iso
```
