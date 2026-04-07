# Pipeline plantation — ordre recommandé (prototype)

Références : `GUIDE_footprint_GetOccupiedCells.md`, `SPEC_plant_footprint_prompt.md`, scripts `GridManager` / `GridData` / `PlantDefinition`.

---

## Objectif

Avoir une **zone cultivable** (prefab) avec une **grille logique** cohérente, puis permettre au joueur de **choisir une graine**, voir un **feedback de placement** (footprint), et enfin **poser** la plante. **Implémenté** (2026-04) : pas de classe `BuildManager` — équivalent **`BiofiltreManager`** + **`PlantPlacementPreview`** + UI **`SeedSelectionUI`** (voir `PROJECT_LOG.md` du 2026-04-07).

---

## Étape 1 — Prefab base plantation (grille) — **fait**

- Créer un GameObject racine (ex. `PlantationBed` / futur biofiltre).
- Ajouter **`GridManager`**.
- Pour un premier test sans asset `GridConfig` : **Use Scriptable Config** = désactivé ; renseigner **Instance Columns / Rows**, **Instance Cell Size** (ou largeur/hauteur si cases non carrées).
- Si le coin haut-gauche de la grille doit suivre l’objet : **Origin From Transform** = activé + ajuster **Origin Offset** si besoin.
- Vérifier en **Scene** les gizmos (cases alignées avec le décor).
- Optionnel : **BoxCollider2D** (ou équivalent) couvrant la zone pour les raycasts souris → `WorldToGrid`.

**DoD minimal** : une grille visible / compréhensible, `GridData` créée au `Awake`, dimensions stables.

---

## Étape 2 — UI plantation — **fait**

**Pourquoi en premier** : le flux « graine sélectionnée » fixe une **`PlantDefinition`** active ; tout le reste (fantôme, couleur vert/rouge, texte d’aide) peut lire **`footprint`** / `GetOccupiedCells` sur cette référence. L’UI ne remplace pas la logique de placement mais **porte la vérité** « quelle plante je pose ».

Contenu minimal possible :

- Liste ou boutons de **sélection de graine** (références vers assets `PlantDefinition`).
- Indication du **footprint** (ex. grille 2×2 schématique, ou simple texte « occupe N cases » au tout début).

**DoD minimal** : une `PlantDefinition` courante accessible (champ statique, service, ou script UI qui expose l’asset sélectionné) pour que le code de preview / placement la consomme.

---

## Étape 3 — Placement (équivalent « BuildManager ») — **fait**

Implémentation réelle : **`BiofiltreManager`** (`CanPlace`, `PlantSeedAt`, `OccupyCells`) + **`PlantPlacementPreview`** (fantôme, snap grille, clic confirme / annule). Entrée utilisateur : clic sur **`BiofiltreCell`** → UI graine → preview.

- Entrée : `GridManager`, `PlantDefinition` courante, position monde (raycast) ou cellule sous curseur.
- **`WorldToGrid`** → origine de pose (convention : case sous le curseur = ancrage `(0,0)` du footprint, à figer une fois).
- **`CanPlace`** : `plant.GetOccupiedCells(origin)` + `grid.AreAllCellsFree(...)` + `IsInBounds` pour chaque cellule.
- **Clic** : `OccupyCells` + instanciation du prefab plante (ou spawn) positionné avec `GridToWorld` / centre footprint.
- **Preview** : même footprint, sprite semi-transparent, teinte selon `CanPlace`.

**DoD minimal** : placer une plante à footprint 1×1 puis 2×2 (laitue) sans chevauchement illégal.

---

## Hors scope prototype actuel

- **Rotation** des cultures : volontairement non prioritaire (un footprint fixe par plante).

---

## Fichiers code utiles

| Fichier | Rôle |
|---------|------|
| `Assets/Scripts/Farm/GridManager.cs` | Grille, conversions, occupation |
| `Assets/Scripts/Data/GridData.cs` | Stockage libre / occupé |
| `Assets/Scripts/Data/GridConfig.cs` | Preset optionnel SO |
| `Assets/Scripts/Data/PlantDefinition.cs` | `footprint`, `GetOccupiedCells` |
| `Assets/Scripts/Farm/BiofiltreManager.cs` | Pont grille / UI, placement, occupation |
| `Assets/Scripts/Farm/BiofiltreGridVisualizer.cs` | Cellules cliquables, conteneur plantes |
| `Assets/Scripts/Farm/BiofiltreCell.cs` | Une case du biofiltre |
| `Assets/Scripts/Farm/PlantPlacementPreview.cs` | Fantôme + validation footprint |
| `Assets/Scripts/UI/SeedSelectionUI.cs` | Panneau choix de graine |
| `Assets/Scripts/UI/SeedSlotUI.cs` | Slot une graine dans la liste |
| `Assets/Scripts/Farm/GridLinesRenderer.cs` | Lignes de grille (optionnel) |
