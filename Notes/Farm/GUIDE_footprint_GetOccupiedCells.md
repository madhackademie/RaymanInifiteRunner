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

Si `origin = (5, 3)`, les cellules occupées seront : `(5,3), (6,3), (5,4), (6,4)` avec la convention grille ci-dessous.

### Convention grille (lecture « européenne »)

- **Origine** : coin **haut-gauche** = `(0,0)`.
- **+X** : une colonne vers la **droite**.
- **+Y** : une ligne vers le **bas** (comme une double boucle `for (y…)` puis `for (x…)` sur une image).

Tous les offsets du `footprint` et les rotations ci-dessous supposent cette convention.

---

## 4) Rotation **sens des aiguilles d’une montre** (↻) — les 4 orientations

Le `footprint` dans l’asset reste défini **sans rotation** (orientation de référence). À la pose ou en prévisualisation, tu appliques une rotation **uniquement horaire** en nombre de **quarts de tour**.

### Paramètre à utiliser

- **`clockwiseQuarters`** (ou `rotationIndex`) : entier **`0`, `1`, `2` ou `3`**.
  - **`0`** : pas de rotation (footprint tel quel).
  - **`1`** : un quart de tour **horaire** (90° ↻).
  - **`2`** : deux quarts (180°).
  - **`3`** : trois quarts (270° ↻ = un quart **anti**-horaire visuel, mais tu n’as pas besoin d’une API séparée : c’est « 3× horaire »).

Tu **ne changes pas** une valeur dans le `PlantDefinition` pour tourner : tu gardes ce paramètre sur le **contexte de placement** (preview, `BuildManager`, ou données d’instance posée). Si tu préfères des **degrés** dans l’Inspector, convertis : `clockwiseQuarters = (degrees / 90) % 4` (avec normalisation pour les négatifs si besoin).

### Une étape horaire sur un offset `(dx, dy)`

\[
(dx, dy) \xrightarrow{\text{↻}} (-dy,\; dx)
\]

### Table des **4** rotations (toutes en quarts **horaires**)

En notant `o = (dx, dy)` :

| `clockwiseQuarters` | Offset résultant |
|--------------------:|------------------|
| 0 | `(dx, dy)` |
| 1 | `(-dy, dx)` |
| 2 | `(-dx, -dy)` |
| 3 | `(dy, -dx)` |

### Code C# — appliquer aux offsets puis aux cellules absolues

```csharp
/// <summary>
/// Rotation d’un offset autour de l’ancrage (0,0), par quarts de tour **horaires**.
/// Grille : origine haut-gauche, +X droite, +Y bas.
/// </summary>
public static Vector2Int RotateFootprintOffsetClockwise(Vector2Int o, int clockwiseQuarters)
{
    int k = ((clockwiseQuarters % 4) + 4) % 4;
    switch (k)
    {
        case 0: return o;
        case 1: return new Vector2Int(-o.y, o.x);
        case 2: return new Vector2Int(-o.x, -o.y);
        case 3: return new Vector2Int(o.y, -o.x);
        default: return o;
    }
}

// Exemple : cellules occupées avec rotation (sans toucher au ScriptableObject)
public static IEnumerable<Vector2Int> GetOccupiedCellsRotated(
    PlantDefinition plant,
    Vector2Int origin,
    int clockwiseQuarters)
{
    if (plant?.footprint == null) yield break;
    foreach (Vector2Int offset in plant.footprint)
    {
        Vector2Int r = RotateFootprintOffsetClockwise(offset, clockwiseQuarters);
        yield return origin + r;
    }
}
```

*(Tu pourras plus tard déplacer cette logique dans `PlantDefinition` sous la forme `GetOccupiedCells(origin, clockwiseQuarters)` si tu veux un seul point d’entrée.)*

**Rappel** : `(0,0)` reste `(0,0)` pour tout `k` — l’ancrage ne bouge pas.

---

## 5) À traiter en prochaine session : **dédoublonnage**

*(Tu as mentionné « déboulonnage » : on part du principe que c’est le **dédoublonnage** des entrées du `footprint`.)*

**Problème** : si `footprint` contient deux fois `(0,0)`, la boucle `foreach (GetOccupiedCells)` traitera la même case deux fois → risque de double `Occupy`, compteurs faux, etc.

**Pistes** (à détailler la prochaine fois) :
- méthode du type `GetOccupiedCellsDistinct(origin)` retournant un `HashSet<Vector2Int>` ou une liste unique ;
- ou normalisation du tableau dans `OnValidate` (trier + fusionner doublons) ;
- ou règle de conception stricte : « pas de doublons dans l’asset » + warning éditeur.

---

## 6) Liens

- Spec d’ensemble : `Notes/Farm/SPEC_plant_footprint_prompt.md`
- Architecture plante : `Notes/Learning/Plant_Architecture_Unity_SO_MB.md`
