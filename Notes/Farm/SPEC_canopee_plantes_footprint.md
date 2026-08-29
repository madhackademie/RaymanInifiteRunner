# Spec — Canopée de plante et footprint grille (sans collider ni physique)

**Ticket** : `[CT-FARM-CANOPEE-001]`
**Statut** : art placeholder livré + code livré — **playtest Unity à faire**
**Date** : 2026-08-29
**Liens** : `Notes/Farm/SPEC_biofiltre_skin_3quart.md` · `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` · `PlantDefinition` · `FarmGridPointerInput`

---

## Problème

L'art actuel des plantes est un rendu **3/4 avec ombre portée cuite** (`04_MatureLaituce_image.png`). Posé sur une grille vue de dessus, il donne deux points de vue contradictoires dans la même image : le substrat est vu du dessus, la salade est vue de face. Le résultat est plat et incohérent.

Voir toute la plante n'a pas de sens non plus : dans un bac vu de dessus, on ne voit que le **haut** du feuillage.

---

## Décision — canopée vue de dessus

On ne dessine que la **canopée** : la plante telle qu'elle apparaît à la verticale. C'est la convention standard du farming mobile et des packs d'assets top-down (une tuile = un sprite, échange de sprite au changement de stade).

Conséquence directe et importante : **il n'y a plus rien à masquer**. Le masquage est cuit dans le dessin, au moment de la création de l'art, et coûte zéro à l'exécution. Un `SpriteMask` runtime coûterait des draw calls et un passage par le stencil buffer, ce qu'on évite sur mobile.

---

## Convention de dimensionnement

> **Un sprite couvrant N × M cellules fait (N × 256) × (M × 256) pixels, et son Pixels Per Unit vaut 256.**

| Plante | Footprint | Sprite | PPU | Couverture du cadre |
|--------|-----------|--------|-----|---------------------|
| Laitue Baby | 1 × 1 | 256 × 256 | 256 | 34 % |
| Laitue Growing | 1 × 1 | 256 × 256 | 256 | 66 % |
| Laitue Mature | 1 × 1 | 256 × 256 | 256 | 92 % |
| Tomate Mature | 2 × 2 | 512 × 512 | 256 | 92 % |

La « couverture » est la fraction du rectangle de footprint réellement occupée par le feuillage ; le reste est de l'alpha. Elle sert à lire la croissance : une jeune pousse remplit un tiers de sa cellule, une plante mature la remplit presque entièrement.

**Le PPU est obligatoire dans le `.meta`.** Sans lui, Unity importe à 100 et un sprite de 256 px occupe 2,56 cellules.

### Pourquoi le sprite ne doit jamais dépasser son footprint

Si un sprite reste dans son rectangle, deux plantes **ne peuvent pas se chevaucher**, puisque leurs footprints sont disjoints par construction. Il n'y a donc besoin d'**aucun tri par Y**, d'aucun ordre de rendu, d'aucun masque. Le problème disparaît par la géométrie plutôt que par du code.

Le tri par rangée ne redeviendra nécessaire que le jour où un feuillage sera autorisé à déborder sur les cases voisines. Ce n'est pas le cas aujourd'hui.

---

## Centrage automatique sur le footprint

La plante est instanciée au centre de la **cellule d'ancrage**, alors que la canopée doit être centrée sur le **rectangle complet**. `PlantDefinition.GetFootprintCenterOffset(cellSize)` calcule ce décalage :

```
offset = ( moyenne(colonnes) × cellWidth , -moyenne(lignes) × cellHeight )
```

Une tomate 2 × 2 ancrée en haut à gauche donne `(+0.5, -0.5)`. Un footprint 1 × 1 donne zéro, donc **les plantes existantes ne bougent pas**.

`spriteWorldOffset` reste disponible comme **réglage fin ajouté** à ce centrage, pour un art dont le pivot ne serait pas exactement au centre. Il doit rester à zéro pour une canopée normale.

Le fantôme de pose et la pose définitive passent tous les deux par `GetSpriteWorldPosition`, donc l'aperçu correspond exactement au résultat.

---

## Clic grille sans collider ni physique

**Décision auteur : aucun collider, aucune physique.**

L'ancien chemin était `BoxCollider2D` sur chaque cellule + `Physics2DRaycaster` sur la caméra + `IPointerClickHandler`. Une grille 10 × 10 créait **100 colliders** pour ce qui est un simple test d'appartenance à un rectangle.

Le nouveau chemin est purement arithmétique :

```
position écran → Camera.ScreenToWorldPoint → GridManager.WorldToGrid → IsInBounds
```

| Composant | Rôle |
|-----------|------|
| `FarmPointerInput` | Lecture pointeur unifiée souris + tactile, et test « au-dessus de l'UI » |
| `FarmGridPointerInput` | Sur la racine du biofiltre : résout la cellule et la transmet au visualizer |
| `BiofiltreGridVisualizer.NotifyCellClicked` | Point d'entrée unique du clic grille |

Les clics sur une plante passent désormais par **sa cellule** : `BiofiltreManager` interroge le registre `GetPlantAt(coords)` et ouvre le popup. `PlantHarvestInteractor` n'a plus de `IPointerClickHandler`, ce qui supprime au passage un second chemin popup pour le même cas d'usage (règle `ui_popup_generic_runtime.mdc`).

### Effet de bord réglé : le tactile

`Mouse.current` est **null sur mobile**, ce qui faisait planter la preview de pose (`[P0-FARM-PLANT-TOUCH-001]`). Tout passe maintenant par `FarmPointerInput`, qui lit d'abord `Touchscreen.current` puis retombe sur la souris. En tactile, la position n'existe que pendant l'appui : le fantôme suit le doigt et se pose au tap, au lieu de dépendre d'un survol qui n'existe pas.

---

## Nettoyage manuel restant (Unity)

Le code ne peut pas retirer des composants déjà sérialisés dans les prefabs et la scène :

1. Supprimer le `Collider2D` des prefabs de plante (il n'est plus requis, mais reste présent dans l'asset).
2. Supprimer le `Physics2DRaycaster` de la caméra de `FirstLvl` s'il n'est utilisé par rien d'autre.
3. Ajouter le composant `FarmGridPointerInput` sur la racine du prefab `Biofiltre` — **sans lui, plus aucun clic grille ne fonctionne**.
4. Brancher les nouveaux sprites de canopée sur `Laitue.asset` (slots Baby / Growing / Mature).

---

## Art livré (placeholder)

- `Assets/Art/Sprites/Plantes/Laitue/Canopee/Laitue_Canopee_02_Baby.png`
- `Assets/Art/Sprites/Plantes/Laitue/Canopee/Laitue_Canopee_03_Growing.png`
- `Assets/Art/Sprites/Plantes/Laitue/Canopee/Laitue_Canopee_04_Mature.png`
- `Assets/Art/Sprites/Plantes/Tomate/Canopee/Tomate_Canopee_04_Mature.png`

Générés pour valider le parti pris visuel, pas pour la production : la charte finale reste à trancher. Les stades Graine, Starting, Flowering et Seedling manquent encore.

Chaque sprite porte une **ombre de contact** douce et quasi centrée, cuite dans l'image. Sans elle, les canopées ressemblent à des autocollants posés sur le substrat. Soleil au zénith oblige, le décalage est minime.

---

## Question de design ouverte — 1 × 1 contre 2 × 2

Dans Hay Day ou Township, un champ n'est pas une grosse plante sur plusieurs tuiles : c'est **beaucoup de petits amas identiques de 1 tuile**, et la densité visuelle vient de la répétition.

La salade est aujourd'hui documentée en 2 × 2 (`GUIDE_footprint_GetOccupiedCells.md` § 3). En canopée, une salade qui remplit 2 × 2 revient à quatre tuiles de feuillage : autant poser quatre salades 1 × 1 et gagner en lisibilité de clic. Le footprint multi-cellules garde tout son sens pour un **pied de tomate**, qui est réellement un buisson large.

À trancher avant de produire l'art définitif, puisque ça change le nombre de sprites à dessiner.

---

## Checklist de playtest

1. Clic sur une cellule vide : le popup graines s'ouvre. Clic sur une cellule occupée : le popup plante s'ouvre.
2. Clic sur les 4 coins de la grille, puis juste en dehors : aucune réaction en dehors.
3. Clic sur un popup ouvert : la grille en dessous **ne réagit pas**.
4. Pose d'une laitue : le fantôme et la plante finale sont exactement au même endroit.
5. Pose d'une plante 2 × 2 : la canopée couvre pile ses 4 cellules, et ces 4 cellules passent en occupé.
6. Récolte : le popup s'ouvre, la plante disparaît, les cellules se libèrent.
7. Sur mobile : tap pour poser, aucune `NullReferenceException` dans le logcat.
