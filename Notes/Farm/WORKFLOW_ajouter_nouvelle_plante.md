# Workflow — ajouter une nouvelle plante (prototype)

Objectif : depuis zéro ou en dupliquant une plante existante, obtenir une culture **plantable** sur le biofiltre (grille), avec **preview** vert/rouge et **occupation multi-cases** cohérente.

**Références code** : `PlantDefinition`, `PlantGrow`, `BiofiltreManager`, `PlantPlacementPreview`, `SeedSelectionUI` / `SeedEntry`, `GridManager`.  
**Référence footprint** : `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`.  
**Contexte pipeline** : `Notes/Farm/TODO_plantation_pipeline.md`.

---

## Vue d’ensemble du flux joueur

1. Clic sur une case **vide** du biofiltre → `BiofiltreManager` ouvre `SeedSelectionUI`.
2. Pour chaque `SeedEntry`, le slot est **cliquable** seulement si `BiofiltreManager.CanPlace(ancre, plantDefinition)` est vrai (tout le footprint doit tenir sur des cases libres).
3. Après choix d’une graine → `PlantPlacementPreview` affiche un **fantôme** sous la souris ; **clic gauche** confirme si vert, **clic droit** ou **Échap** annule.
4. Confirmation → `BiofiltreManager.PlantSeedAt` instancie le prefab, met `PlantGrow` en stade **Graine**, occupe toutes les cellules du footprint.

*(Sans `PlantPlacementPreview` assigné dans l’UI, repli : placement **direct** sur la case du premier clic — utile pour débug rapide seulement.)*

---

## Étape 1 — Créer ou dupliquer un `PlantDefinition`

**Création** : menu Unity **Create → Game → Plants → Plant Definition** (attribut `[CreateAssetMenu]` sur `PlantDefinition`).

**Duplication** : clic droit sur un asset existant (ex. laitue) → **Duplicate**, puis renommer le fichier et ajuster les champs pour éviter les collisions d’identité.

### Champs à renseigner (minimum jouable)

| Zone | Rôle |
|------|------|
| **Identity** | `plantId` (clé logique stable), `displayName` (affichage). |
| **Harvest** | `harvestItemId`, quantités min/max, `maxHarvestCount` — utilisés par la future récolte ; pas bloquant pour la plantation seule. |
| **Stage Sprites** | Un sprite par stade visible dans `PlantGrow` (`Graine` → `Seedling`). Laisser vide seulement pour des tests très brouillon. |
| **Grid Placement** | `footprint` + `spriteWorldOffset` — voir étape 2. |
| **Stage Durations** | Entrées `StageDuration` (stade + secondes). Durée `0` = pas d’auto-avancement pour ce stade. |

**Validation éditeur** : le `footprint` **doit** contenir `(0, 0)` (warning dans la Console sinon).

---

## Étape 1b — Configuration spécifique des plantes à fruits (refactor)

Depuis le refactor, `PlantDefinition` expose un profil de croissance pour gérer les divergences entre
plantes à feuilles et plantes à fruits sans créer un second type de ScriptableObject.

### Réglages à appliquer pour une plante à fruits

- **Growth Pattern** : `Fruiting`
- **Harvest Stage** : `Mature` (valeur recommandée par défaut)

### Ordre des stades selon le profil

| Profil | Séquence en fin de cycle |
|--------|---------------------------|
| `Leafy` | `... -> Growing -> Mature (recolte) -> Flowering -> Seedling` |
| `Fruiting` | `... -> Growing -> Flowering -> Mature (recolte) -> Seedling` |

### Impact sur les sprites (mapping inchangé)

Les 7 slots de sprites restent identiques. Seule leur interprétation change selon le profil :

- `spriteFlowering` : phase de floraison avant maturité (important pour les fruits)
- `spriteMature` : stade récoltable
- `spriteSeedling` : stade final orienté production de graines

### Durées à vérifier

Pour les plantes à fruits, configure soigneusement `Stage Durations` sur `Flowering`, car ce stade
précède désormais la récolte et influence directement le temps avant maturité.

---

## Étape 2 — Footprint et offset visuel

- Le **point d’ancrage** de pose est la cellule cliquée : elle correspond à l’offset `(0, 0)` du tableau `footprint`.
- Chaque autre case occupée est `origin + offset` via `PlantDefinition.GetOccupiedCells(origin)` — détail, exemples 2×2, convention grille (+X droite, +Y bas) : **`GUIDE_footprint_GetOccupiedCells.md`**.
- **Éviter les doublons** dans `footprint` : la même case pourrait être traitée deux fois (comportements futurs / compteurs).
- **`spriteWorldOffset`** (Vector2, espace monde) : décale le **SpriteRenderer** pour aligner le pivot du sprite sur le visuel. Exemple documenté dans l’Inspector : plante 2×2 avec pivot bas-centre → souvent `(+0.5, 0)` avec cellules de taille 1.  
  **Important** : la preview souris utilise un centre géométrique du footprint **sans** ce offset pour le calcul de la case sous la souris (`PlantPlacementPreview`) — ne pas « corriger » le snap en gonflant abusivement `spriteWorldOffset`.

**Rotation** : le prototype actuel pose le footprint **tel quel** (pas de rotation à l’exécution). Si tu ajoutes la rotation plus tard, réutilise la logique décrite dans le guide footprint.

---

## Étape 3 — Prefab monde avec `PlantGrow`

1. Crée un GameObject racine (ex. `Plante_MaLaitue`).
2. Ajoute **`SpriteRenderer`** (requis par `[RequireComponent]` sur `PlantGrow`).
3. Ajoute **`PlantGrow`** ; dans l’Inspector, assigne le **`PlantDefinition`** créé à l’étape 1.
4. Règle éventuellement **`initialStage`** pour prévisualiser un stade dans l’éditeur (`OnValidate` met à jour le sprite). **En jeu**, après plantation, `BiofiltreManager` appelle **`SetStage(Graine)`** : le stade initial du prefab ne s’applique pas aux cultures posées via le biofiltre.
5. Enregistre comme **Prefab** (dossier projet au choix, ex. sous `Assets/.../Prefabs/Farm/`).

**Fantôme de preview** : `PlantPlacementPreview` instancie ce prefab, **désactive** `PlantGrow`, force un sprite (actuellement **`spriteSeedling`** du `PlantDefinition`) et désactive les **Collider2D** pour ne pas bloquer les raycasts. Aucune config spéciale supplémentaire sur le prefab pour la preview.

---

## Étape 4 — Enregistrer la plante dans `SeedSelectionUI`

Sur le GameObject qui porte **`SeedSelectionUI`** (scène ou prefab UI) :

1. Liste **`Available Seeds`** : ajoute un élément **`SeedEntry`**.
2. **`plantDefinition`** → ton nouvel asset.
3. **`plantPrefab`** → le prefab de l’étape 3.

Vérifie aussi :

- **`Seed Slot UI`** : prefab de ligne/bouton (`SeedSlotUI`).
- **`Slots Container`** : parent des slots instanciés.
- **`Placement Preview`** : référence vers le composant **`PlantPlacementPreview`** actif dans la scène (souvent sur un objet dédié).
- Sur le biofiltre : **`BiofiltreManager`** a bien le champ **`Seed Selection UI`** rempli vers ce panneau.

Sans entrée dans `availableSeeds`, la plante **n’apparaît pas** dans le panneau et ne peut pas être choisie.

---

## Étape 5 — Vérifications sur la grille (test manuel)

1. **Lancer la scène** avec biofiltre + UI câblés (`BiofiltreManager`, `GridManager`, `BiofiltreGridVisualizer`).
2. Cliquer une case **vide** à l’intérieur de la grille : le panneau s’ouvre.
3. **Slot grisé / non cliquable** : normal si le footprint ne tient pas depuis **cette** case d’ancrage (ex. 2×2 trop près du bord ou chevauchement). Tester depuis une case plus centrale.
4. Après sélection : le fantôme doit être **vert** sur les positions valides, **rouge** sinon ; **clic gauche** pose la plante ; les cases couvertes passent en occupé (visuel `BiofiltreCell` + logique `GridManager`).
5. **Plusieurs poses** : après un placement réussi, la preview reste active pour enchaîner (pas de fermeture automatique) — **clic droit** ou **Échap** pour sortir du mode placement.

**Débug rapide** : logs `[BiofiltreManager]` en Console si placement refusé (case occupée, références nulles).

---

## Fichiers à connaître (rappel)

| Fichier | Rôle court |
|---------|------------|
| `Assets/Scripts/Data/PlantDefinition.cs` | Données plante, `footprint`, `GetOccupiedCells`, sprites, durées. |
| `Assets/Scripts/Farm/PlantGrow.cs` | Stades, sprites, timers entre stades. |
| `Assets/Scripts/Farm/BiofiltreManager.cs` | `CanPlace`, `PlantSeedAt`, instanciation, occupation grille. |
| `Assets/Scripts/Farm/PlantPlacementPreview.cs` | Fantôme, validité, confirmation. |
| `Assets/Scripts/UI/SeedSelectionUI.cs` | Liste `SeedEntry`, ouverture preview ou placement direct. |

---

## Pistes d’évolution (hors scope de ce guide)

- Récolte + inventaire (`TryAdd`, feedback « inventaire plein ») — voir `Notes/Todo_project.md`.
- State machine culture / timer global — scripts `PlantGrow`, `Timer.cs`, todos projet.
