# Note graphique — direction art monde

**Création :** 2026-09-08  
**Branche :** `main` (iso 2:1 mergé)  
**IDs :** `[BL-ART-003]` (amorcé) · grille `[P0-FARM-ISO-GRID-001]`  
**Backlog assets :** `Notes/Art/PROMPT_generation_icones.md`  
**Jeux de ref :** `Notes/References/REFERENCES_jeux_inspiration.md` § **F**

---

## Décision auteur (2026-09-08)

Le monde ferme passe sur :

1. **Vue isométrique 2:1** (losange jeu, pas caméra ortho carrée, pas Stardew top-down).
2. **Style cartoon iso** (casual mobile, couleurs vives).
3. **Modèles les plus proches : Township + The Tribez** (village iso, bâtiments chunky, lecture pouce).
4. Matière carton / papercraft : **secondaire**. Cartoon d’abord. Zombie Castaways = polish optionnel, **sans** thème zombie.

Cible visuelle monde (biofiltre, bacs, plantes, décors). Icônes UI : brief bois rustique du backlog, jusqu’à décision contraire.

**Prompt unique assets :** `Notes/Art/PROMPT_assets_monde_iso.md`

---

## Écart vs Township / Tribez — le volume

Township et The Tribez posent les cultures **à plat** : parcelle de terre au niveau du sol, champ « plane ».

Chez nous le volume vient des **biofiltres** et des **zones hydroponiques** :

- La plante ne pousse **pas** dans un champ de terre au sol.
- Elle est **surélevée** : cuve IBC (corps + cage) + deck plantable (billes d’argile) + plant iso 3/4 par-dessus.
- Un biofiltre = un **objet 3/4 lisible** (toit/dessus + faces), comme un bâtiment Township/Tribez, **pas** une dalle de dirt flush au terrain.
- Plusieurs biofiltres / bacs = étagement dans la scène (avant/arrière iso, hauteurs différentes possibles).

Conséquence art : copier le **langage** Township/Tribez (cartoon iso, silhouettes, densité village). **Ne pas** copier le champ plat. Les sprites plante s’ancrent sur le **deck hydro** (media / motte minuscule de billes), jamais sur une parcelle terre plein sol.

---

## Vue — isométrique 2:1

| Règle | Détail |
|--------|--------|
| Grille jeu | Losange **largeur = 2 × hauteur**. Angle d’arête **26,565°** (`atan(0.5)`), **pas** l’iso illustration 30°. |
| Caméra | 2D `SpriteRenderer` posé sur la grille iso (déjà `coordinateMode = Isometric` sur `Biofiltre`). |
| Structures | 3/4 iso : on lit le **dessus** (deck plantable) **et** une face. Ex. cuve IBC `IbcIso.png`. |
| Interdit | Grille carrée ortho comme cible monde. Iso 30° « architectural ». Caméra 3D orbit par défaut. |

Réf technique IBC : `Notes/Farm/CABLAGE_biofiltre_ibc_grille_bezi.md` · brief recale `PROMPT_generation_icones.md` § W1.

---

## Style — cartoon iso (Zombie Castaways = polish, pas le thème)

Objectif : ferme **cartoon** lisible sur téléphone, volume jouet iso — pas un mesh 3D, pas du pixel, pas de l’horreur.

**À viser**
- Couleurs saturées, silhouettes chunky, ombrage peintre doux (famille Vizor / Game Insight iso).
- Toits + murs / têtes de plantes **lus en 3/4**.
- Une plante = un sprite (footprint grille), pas un champ entier.

**À éviter**
- Thème **zombie** (même si on aime le polish Zombie Castaways).
- Canopée top-down, low-poly Gen3D, photoreal, pixel art.
- Copier Bikini Bottom (sol cyan, bordure violette) ou l’UI bulles Tribez.

**Mix refs auteur**

| Couche | Source | Fichier |
|--------|--------|---------|
| **Modèles cibles** | **Township** + **The Tribez** | village iso, props — **sauf** cultures à plat |
| Village / props | **The Tribez** | `Notes/References/images/tribez_village_iso.png` |
| Pose têtes de cultures | **SpongeBob farm** (In A Jam, *un truc du genre*) | `Notes/References/images/spongebob_farm_cabbage_iso.png` — têtes 3/4, **posées sur le deck IBC**, pas un champ |
| Polish optionnel | **Zombie Castaways** (Vizor) | store — sans thème zombie |
| Features + cozy iso | **My Dear Farm** | playtest B5 / F0d |
| Farm + graphique | **Harvest Land** | playtest A12 / F0e |
| Ferme iso mer | **Family Farm Seaside** | playtest A13 / F0h |
| Multiplayer | **Big Farm: Mobile Harvest** | playtest A9 / F0g |

Détail jeux : `REFERENCES_jeux_inspiration.md` § F.

---

## Plantes vs structures

| Asset | Cible | Note |
|--------|--------|------|
| IBC / bacs / bâtiments | Iso 3/4 **cartoon** | Volume type bâtiment Township/Tribez (dessus + faces). C’est ça qui **crée le relief**. |
| Plantes monde | **Iso 3/4 cartoon** | Têtes type SpongeBob farm, **ancrées sur le deck hydro** (pas un champ terre plane). Un plant / footprint. |

Brief laitue (7 stades, iso cartoon) : `Notes/Art/PROMPT_laitue_sprite_sheet_croissance.md` — charte `NOTE_graphique.md`.

---

## Pipeline (rappel `[BL-ART-003]`)

- **Prod actuelle :** sprites 2D (Dump → OK auteur → `Sprites/`). Règle : `.cursor/rules/art_asset_dump.mdc`.
- **3D :** piste ultérieure, seulement si perf mobile OK (`[BL-ART-005]`). Pas le chemin par défaut.
- Cette note **remplace** « on verra 2D vs 3D » pour le **look monde** : 2D iso **cartoon**.

---

## Do / don’t (prompts & revue)

**Do**
- Grille 2:1 dans le brief monde (arêtes 26,565°).
- Un objet isolé, fond transparent, silhouette cartoon chunky.
- **Alpha serré** : bords nets, pas de halo / glow / brouillard autour du sprite (perf mobile + overdraw).
- Même ancrage / même losange que la grille pour tout ce qui se pose **sur le deck** du biofiltre.

**Don’t**
- Recoller un brut Dump sur un prefab sans promo auteur.
- Mélanger canopée top-down et iso 3/4 dans une même sheet sans l’écrire.
- Copier un champ de terre **à plat** (Township / Tribez / Hay Day).
- Copier le thème zombie, le sol cyan Bikini Bottom, ou le cozy wood des icônes UI sur les props monde.
- **Glow, fog, bloom, aura, soft haze** autour du sujet ; **grandes zones alpha** en dégradé sur les bords (anti-aliasing excessif).

### Alpha / perf (décision auteur 2026-09-08)

| Règle | Pourquoi |
|--------|----------|
| Silhouette **coupée nette** (1–2 px d’anti-alias max) | Moins d’overdraw que halo transparent large |
| **Pas** d’ombre portée en dégradé transparent autour de la plante | Préférer ombre **dans** le dessin (couleur) ou VFX séparé |
| Recadrer au **plus près** du sujet (Sprite Editor → Trim) | Moins de pixels alpha vides dans le mesh |
| Import Unity : **Alpha Is Transparency** + mesh tight ; éviter **Generate Mip Maps** sur UI/plantes 2D si inutile | Perf fill-rate mobile |

Post-prod auteur : Photopea / GIMP → **Matting** ou niveau alpha dur ; ou Unity **Sprite Editor** trim + éventuellement **Alpha cutoff** matériel si besoin.
