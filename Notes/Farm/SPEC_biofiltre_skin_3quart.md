# Spec — Skin biofiltre vue 3/4 (art collé, grille carrée inchangée)

**Ticket** : `[CT-FARM-BED-ART-001]`
**Statut** : spec — **aucun code écrit**, aucun script à modifier
**Date** : 2026-08-29
**Liens** : `Assets/Scripts/Farm/GridManager.cs` · `Assets/Scripts/Farm/BiofiltreGridVisualizer.cs` · `Assets/Prefabs/World/Biofiltre.prefab` · `PROJECT_LOG.md`
**Historique** : remplace le chantier « bed skin » annulé le 2026-08-29 (commit `ab090b3`, retour à l'état `main`). Lire § *Règles anti-régression* avant toute implémentation.

---

## Objectif joueur

Le biofiltre doit se lire comme un **bac posé au sol**, avec du volume, et non comme une grille qui flotte dans le vide. On veut voir **une partie des flancs** de la cuve tout en gardant la surface de billes d'argile plane et jouable.

---

## Contrainte auteur (2026-08-29) — cadre non négociable

| Sujet | Décision |
|-------|----------|
| Cellules | **Carrées**, `cellSize = 1`, `instanceUniformCellSize = true` |
| Compression verticale (facteur k) | **Refusée** — perf et logique simplifiée |
| Maths de grille | **Aucune modification** de `GridManager` |
| Code runtime | **Aucun** — pas de génération de bac, pas de script Editor |
| Méthode | **Coller de l'art** : un enfant statique dans le prefab |

L'illusion de volume vient donc **entièrement du dessin**, pas de la géométrie.

---

## Principe : projection oblique (« triche Zelda »)

Dans *A Link to the Past*, le sol d'une pièce est dessiné vu de dessus, sans déformation, et les murs sont dessinés **de face**, en élévation, collés au bord de la pièce. Aucun point de fuite, aucun point de vue cohérent : c'est géométriquement impossible et l'œil l'accepte.

Appliqué au bac : la surface d'argile reste le carré de la grille, et les flancs sont des pixels **hors du rectangle de grille**. Ils n'ont aucune existence logique — pas de cellule, pas de collider, pas de coordonnée.

```
        ┌──────────────────────────┐   ← lèvre arrière : on voit sa face INTÉRIEURE
        │                          │
        │   surface billes argile  │   ← rectangle de grille : 10 × 10 cellules de 1
        │   (carré, vu de dessus)  │
        │                          │
        ├──────────────────────────┤   ← bord avant de la grille (y = -10)
        │  flanc avant (élévation) │   ← HORS grille : on voit sa face EXTÉRIEURE
        └──────────────────────────┘
```

**La clé artistique** : on voit la face **intérieure** de la paroi du fond et la face **extérieure** de la paroi avant. C'est ce contraste qui crée le volume sans aucune déformation. Une lèvre arrière fine, un flanc avant épais.

---

## Géométrie exacte

État actuel du prefab `Biofiltre` (`GridManager`, layout instance) : `instanceColumns = 10`, `instanceRows = 10`, `instanceCellSize = 1`, `originFromTransform = true`, `originOffset = (0, 0)`.

La surface jouable occupe donc, en coordonnées **locales** du prefab, le rectangle `x ∈ [0, 10]`, `y ∈ [-10, 0]`, de **centre `(5, -5)`**.

### Découpe du PNG

Dessiner la surface intérieure à **exactement 1000 × 1000 px**, puis ajouter les marges décoratives autour :

| Marge | Valeur conseillée | Rendu en unités monde |
|-------|-------------------|-----------------------|
| Gauche / droite (`C`) | 60 px | 0,6 |
| Haut, lèvre arrière (`B`) | 40 px | 0,4 |
| Bas, flanc avant (`F`) | 180 px | **1,8** (≈ 2 cellules visibles) |

Taille totale du PNG : `W = 60 + 1000 + 60 = 1120 px`, `H = 40 + 1000 + 180 = 1220 px`.

### Réglages d'import

- **Pixels Per Unit** = `1000 / (10 × 1) = 100`
- **Pivot** = *Custom*, placé au centre de la surface intérieure :
  - `x = (C + 500) / W = 560 / 1120 = 0.5`
  - `y = (F + 500) / H = 680 / 1220 = 0.557`
- `Alpha Is Transparency` = coché, `Mesh Type` = Full Rect

### Placement dans le prefab

Enfant `Bed` à `localPosition = (5, -5, 0)`, `localScale = 1`, rotation nulle. **C'est tout** : le pivot custom fait l'alignement, aucun calcul au runtime.

> **Formule générale** si la grille change un jour :
> `PPU = largeurIntérieurePx / (columns × cellSize)` et `localPosition = (columns × cellSize / 2, −rows × cellSize / 2)`.

### La grille ne déborde pas des billes d'argile

Par construction, le rectangle de grille **est** la surface d'argile : les 1000 px intérieurs du PNG correspondent exactement aux 10 unités monde des 10 cellules. Les marges décoratives (`C`, `B`, `F`) sont en dehors, donc aucune cellule ne peut tomber sur une paroi.

Si au playtest la grille déborde malgré tout, il n'y a que trois causes possibles :

| Symptôme | Cause | Correctif |
|----------|-------|-----------|
| Grille plus grande que l'argile | PPU ≠ 1000 / 10 | Remettre PPU = 100 |
| Grille décalée en diagonale | Pivot non custom (resté Center) | Repositionner le pivot au centre de la surface intérieure |
| Grille décalée d'un demi-bac | `localPosition` ≠ (5, −5) | Corriger la position de l'enfant |

Aucun de ces cas ne demande de code.

---

## Hiérarchie cible du prefab

```
Biofiltre                    (GridManager + BiofiltreGridVisualizer)
├── Bed      SpriteRenderer, order -20   ← le bac : lèvre arrière, intérieur, flancs
├── Grid     (gridContainer)  order   0  ← cellules générées au runtime
└── Plants   (plantsContainer)           ← plantes instanciées à la pose
```

| Élément | Sorting order | Justification |
|---------|---------------|---------------|
| `Bed` | −20 | Derrière tout, y compris les cellules de feedback |
| Cellules | 0 | Valeur actuelle `cellSortingOrder`, inchangée |
| Plantes | prefab | Inchangé — **ne pas toucher aux plantes** |

### Un seul sprite suffit — décision auteur 2026-08-29

Un seul enfant `Bed`, pas de découpe avant/arrière. La raison est physique : **une plante ne pousse pas sur un mur**. Elle est plantée dans une cellule, donc dans le substrat, donc toujours en retrait du flanc avant. Le cas « une plante masque la paroi avant » ne peut pas se produire.

Le cas inverse, lui, est **normal et souhaitable** : une plante de la rangée du fond recouvre partiellement la lèvre arrière, puisqu'elle se tient devant elle du point de vue de la caméra. C'est exactement ce que fait un vrai bac de culture.

Seul cas limite, cosmétique : les sprites de laitue ont un pivot au centre, donc une plante plus haute qu'une cellule déborde autant en haut qu'en bas. En rangée du fond ce débord passe sur la lèvre arrière — correct. En rangée avant, il effleure le haut du flanc. Ça se lit comme un feuillage qui retombe sur le rebord du bac, ce qui est naturel : **on ne corrige pas**.

---

## Règles anti-régression (issues du rollback du 2026-08-29)

Le chantier précédent a été annulé pour cause de cuve en double et d'erreurs Inspector. Causes identifiées, à ne pas reproduire :

1. **Aucun GameObject créé au runtime** pour le bac. Un enfant statique posé dans le prefab ne peut pas se dupliquer, par construction.
2. **Pas de `transform.Find("BedSprite")`** : cette méthode ignore les enfants inactifs et recrée un doublon à chaque appel. C'était la cause directe du bug.
3. **Pas de `ScriptableObject` de skin**, pas de script Editor, pas de gizmo de preview, pas de menu « nettoyer la sélection ». Changer de bac = changer le champ `Sprite` dans l'Inspector.
4. **Pas de modification** de `GridManager` ni de `BiofiltreGridVisualizer`.
5. **Pas de `Collider2D`** sur le bac : les clics doivent atteindre les cellules.
6. **Pas de masquage de la grille hors mode pose** — c'était aussi dans le lot annulé, c'est un autre sujet.

---

## Hors périmètre

- Compression verticale de la grille (refusée par l'auteur).
- Pivots des sprites de laitue et `spriteWorldOffset` : **ne pas toucher aux plantes**.
- Tri par rangée des plantes. Il n'existe aujourd'hui **aucun** tri par Y dans le projet. À ouvrir en ticket séparé uniquement si le playtest montre des chevauchements gênants.
- Variante bois (`Planteur_carre_3quart.png`) tant que la variante IBC n'est pas validée.

---

## Art disponible

Conservés par le rollback, réutilisables comme base ou référence :

- `Assets/Art/Sprites/Farm/Biofiltre/Cuve_IBC_coupee_3quart.png`
- `Assets/Art/Sprites/Farm/Biofiltre/Planteur_carre_3quart.png`
- `Assets/Art/Sprites/Farm/Biofiltre/Planteur_carre_vue_grille.png`

Ils ne sont plus référencés par aucun prefab. À redécouper aux dimensions du § *Géométrie exacte*.

---

## Checklist de validation (playtest auteur)

1. `FirstLvl` en Play : **un seul** enfant `Bed` sous `Biofiltre`, zéro warning Console.
2. La grille ne déborde pas de l'argile : les cellules des 4 coins tombent pile dans la surface intérieure (sinon voir le tableau § *La grille ne déborde pas*).
3. Le flanc avant est visible sous la grille et ne masque aucune cellule cliquable.
4. Clic sur les 4 cellules de coin : la sélection répond partout.
5. Pose d'une laitue en rangée du fond : elle passe **devant** la lèvre arrière (comportement attendu).
6. Pose d'une laitue en rangée avant : elle passe devant l'argile et ne disparaît pas derrière le bac.
7. Sortie du Play : aucun objet `Bed` en double resté dans la scène.

---

## Prompt Bezy — phase 1 (hiérarchie seule)

```
[BZ-FARM-BED-002] Phase 1 ONLY — add static bed child to Biofiltre prefab.

Do NOT rescan whole project. Do NOT create or edit C# scripts.
Do NOT create ScriptableObjects. Do NOT add Editor scripts or gizmos.
Reuse existing scripts unless asked otherwise.

OPEN prefab: Assets/Prefabs/World/Biofiltre.prefab

Create ONE child GameObject directly under the prefab ROOT:
- name: Bed
- layer: Default (0) — this is a WORLD object, NOT UI. Do not set layer 5.
- localPosition: (5, -5, 0)
- localRotation: (0, 0, 0)
- localScale: (1, 1, 1)
- make it the FIRST sibling under the root

Do NOT add any component yet (Transform only).
Do NOT modify GridManager or BiofiltreGridVisualizer.
Do NOT touch the Grid or Plants children.

Save. List what changed. STOP.
```

## Prompt Bezy — phase 2 (composant seul)

```
[BZ-FARM-BED-002] Phase 2 ONLY — add SpriteRenderer to Bed.

Do NOT rescan whole project. Do NOT create or edit C# scripts.
Do NOT add any Collider. Do NOT create new GameObjects.

OPEN prefab: Assets/Prefabs/World/Biofiltre.prefab
SELECT existing child: Bed

Add ONE component: SpriteRenderer
- Sprite: Assets/Art/Sprites/Farm/Biofiltre/Cuve_IBC_coupee_3quart.png
- Draw Mode: Simple
- Sorting Layer: Default
- Order in Layer: -20
- Color: white, alpha 1

Do NOT change the Transform set in phase 1.
Do NOT modify GridManager or BiofiltreGridVisualizer.

Save. List what changed. STOP.
```

> Le playtest (Simulate / Play Mode) est fait par l'auteur **après** les phases Bezy, jamais demandé à Bezy.
