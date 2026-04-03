# Spec + prompt — footprint plantes (grille)

Objectif : documenter l’architecture pour **placer une plante sur plusieurs cellules** et en faire un **prompt copiable** pour **Bezi** (Unity) et/ou **Cursor** (C#).

Contexte projet actuel :
- Données plante : `PlantDefinition` (`ScriptableObject`) + `PlantGrow` (`MonoBehaviour`) sur prefab / scène.
- Pas encore de `BuildManager` / grille explicite dans cette note — à créer ou étendre.

---

## 1) Modèle de données (cible)

### Structure logique runtime (exemple C#)

```csharp
public readonly struct Plant
{
    public Vector2Int Origin { get; }
    public IReadOnlyList<Vector2Int> Footprint { get; }
    // ou List<CellOffset> si tu préfères un type dédié CellOffset : struct { int x, y; }
}
```

- **`origin`** : cellule “ancrage” choisie par le joueur (ex. coin bas-gauche du footprint en grille tile).
- **`footprint`** : liste d’**offsets** relatifs à `origin`. Toute cellule occupée = `origin + offset`.

### Exemples (offsets en grille “avant” = +Y ou selon convention projet)

| Plante   | Footprint (offsets) |
|----------|----------------------|
| **Poireau** | `[(0,0)]` — 1×1 |
| **Salade**  | `[(0,0), (1,0), (0,1), (1,1)]` — bloc 2×2 (toujours 2×2 mais défini comme liste pour rester flexible) |
| **Tomate**  | `[(0,0), (1,0), (0,1), (-1,0), (0,-1)]` — croix (+ centro) |

Les **entiers négatifs** sont valides tant que la convention `origin + offset` reste dans les bornes de la grille.

---

## 2) Extension `PlantData` / `PlantDefinition` (Unity)

Ajouter sur le **ScriptableObject** existant (ou un nouveau type si tu séparres “stats” et “placement”) :

```csharp
[CreateAssetMenu(menuName = "Game Data/Plant Definition", fileName = "PlantDefinition")]
public class PlantDefinition : ScriptableObject
{
    public string plantName;
    public Vector2Int[] footprint;   // offsets relatifs à l’origine de pose
    public float growthTime;
    // ... champs déjà présents (sprites, durées, ids, etc.)
}
```

Exemple **Salade** (dans l’inspector ou en `Reset` / tests) :

```csharp
footprint = new Vector2Int[]
{
    new Vector2Int(0, 0),
    new Vector2Int(1, 0),
    new Vector2Int(0, 1),
    new Vector2Int(1, 1),
};
```

**Règle** : toujours inclure `(0,0)` pour que l’ancrage corresponde à une cellule réellement occupée par la plante.

---

## 3) Validation placement (`BuildManager` / service grille)

Comportement attendu quand le joueur place une plante en `origin` :

```csharp
bool CanPlace(PlantDefinition plant, Vector2Int origin)
{
    foreach (var offset in plant.footprint)
    {
        Vector2Int cell = origin + offset;
        if (!Grid.IsFree(cell))
            return false;
    }
    return true;
}
```

- Si **une seule** cellule n’est pas libre → **placement refusé** (pas de pose partielle).
- Après validation : réserver / marquer toutes les cellules du footprint comme occupées (par la même instance / id plante).

---

## 4) Conventions à figer avant d’implémenter

1. **Sens des axes** : `Vector2Int` → quel axe est “droite”, quel axe est “haut” par rapport à l’affichage isométrique / top-down.
2. **Rotation** : est-ce que le footprint **tourne** avec l’orientation du joueur ? Si oui, prévoir `RotateFootprint(footprint, int quarterTurns)`.
3. **Une instance = une racine** : un seul `GameObject` / `PlantGrow` pour toute la zone, ou un parent + enfants par cellule (recommandation : **une racine** + données footprint pour simplifier récolte / destruction).
4. **Interaction** : clic sur n’importe quelle cellule du footprint sélectionne la même plante.

---

## 5) Prompt court (copier-coller pour Bezi ou Cursor)

> Dans ce projet Unity 2D farm, étendre `PlantDefinition` avec un tableau `Vector2Int[] footprint` (offsets relatifs à la cellule d’ancrage).  
> Implémenter une grille simple (`IsFree(Vector2Int)`, `Occupy`, `Release`) et un `BuildManager` (ou service) qui, pour une origine cliquée, vérifie **toutes** les cellules `origin + offset`.  
> Refuser le placement si une cellule est occupée.  
> Exemples de données : poireau 1×1 `[(0,0)]`, salade 2×2, tomate en croix `[(0,0),(1,0),(0,1),(-1,0),(0,-1)]`.  
> Respecter les règles du repo : `SerializeField`, pas de `Update` inutile, une responsabilité par script.  
> Brancher au besoin sur le prefab plante existant (`PlantGrow`) sans casser les stades visuels.

---

## 6) Références internes

- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Farm/PlantGrow.cs`
- `Notes/Learning/Plant_Architecture_Unity_SO_MB.md`
- `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` — utilisation de `GetOccupiedCells`, BuildManager, exemple 2×2, piste dédoublonnage
