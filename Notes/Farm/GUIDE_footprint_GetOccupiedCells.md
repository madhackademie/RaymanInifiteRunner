# Guide — `GetOccupiedCells`, BuildManager, footprint 2×2

Référence code : `Assets/Scripts/Data/PlantDefinition.cs` (`footprint`, `GetOccupiedCells`).

---

## 1) Que fait `GetOccupiedCells(Vector2Int origin)` ?

C’est une méthode qui **ne modifie rien** : elle **énumère** toutes les cases de grille **absolues** occupées par la plante si l’**ancrage** (origine de pose) est `origin`.

- Pour chaque **offset** dans `footprint`, elle retourne : **`origin + offset`**.
- Type de retour : **`IEnumerable<Vector2Int>`** (itérateur avec `yield return`) → tu peux l’utiliser dans un `foreach`, ou la copier dans une liste si besoin.

```csharp
// Exemple : uniquement pour lire les cellules
foreach (Vector2Int cell in plantDefinition.GetOccupiedCells(placementOrigin))
{
    // cell = une case à tester / occuper
}
```

**Convention** : `footprint` **doit** contenir `(0,0)` (validé en éditeur via `OnValidate` sur `PlantDefinition`).

---

## 2) Comment l’appeler depuis un futur `BuildManager` ?

Le `BuildManager` (ou `PlacementService`) connaît :
- la **`PlantDefinition`** choisie (ou son asset),
- la **`Vector2Int origin`** (case cliquée / curseur grille).

### Étape A — Vérifier que le placement est possible

```csharp
public bool CanPlace(PlantDefinition plant, Vector2Int origin)
{
    if (plant == null) return false;

    foreach (Vector2Int cell in plant.GetOccupiedCells(origin))
    {
        if (!_grid.IsFree(cell))
            return false;
    }
    return true;
}
```

### Étape B — Après validation, occuper les cases

```csharp
public void Occupy(PlantDefinition plant, Vector2Int origin, PlantInstanceHandle handle)
{
    foreach (Vector2Int cell in plant.GetOccupiedCells(origin))
        _grid.Occupy(cell, handle);
}
```

*(Les noms `PlantInstanceHandle` / `_grid` sont indicatifs : à adapter à ton API réelle.)*

**Important** : aujourd’hui `GetOccupiedCells` peut retourner **la même cellule deux fois** si le tableau `footprint` contient des doublons → voir section **dédoublonnage** (prochaine explication en session).

---

## 3) Écrire un footprint **2×2** (ex. salade)

Dans l’Inspector sur l’asset `PlantDefinition`, mets **Size = 4** et les offsets :

| Index | x | y |
|------:|--:|--:|
| 0 | 0 | 0 |
| 1 | 1 | 0 |
| 2 | 0 | 1 |
| 3 | 1 | 1 |

En C# (tests ou `[InitializeOnLoad]` / données par défaut) :

```csharp
footprint = new Vector2Int[]
{
    new Vector2Int(0, 0),
    new Vector2Int(1, 0),
    new Vector2Int(0, 1),
    new Vector2Int(1, 1),
};
```

Si `origin = (5, 3)`, les cellules occupées seront : `(5,3), (6,3), (5,4), (6,4)` — **selon ta convention d’axes** (X droite, Y haut ou bas : à figer une fois pour le projet).

---

## 4) À traiter en prochaine session : **dédoublonnage**

*(Tu as mentionné « déboulonnage » : on part du principe que c’est le **dédoublonnage** des entrées du `footprint`.)*

**Problème** : si `footprint` contient deux fois `(0,0)`, la boucle `foreach (GetOccupiedCells)` traitera la même case deux fois → risque de double `Occupy`, compteurs faux, etc.

**Pistes** (à détailler la prochaine fois) :
- méthode du type `GetOccupiedCellsDistinct(origin)` retournant un `HashSet<Vector2Int>` ou une liste unique ;
- ou normalisation du tableau dans `OnValidate` (trier + fusionner doublons) ;
- ou règle de conception stricte : « pas de doublons dans l’asset » + warning éditeur.

---

## 5) Liens

- Spec d’ensemble : `Notes/Farm/SPEC_plant_footprint_prompt.md`
- Architecture plante : `Notes/Learning/Plant_Architecture_Unity_SO_MB.md`
