# C# — bases utiles (grille) + Cursor / Unity

Fiche de **rappel** issue des échanges projet (2026-04). À compléter avec vos propres exemples.

## `byte` et tableaux 2D

- **`byte`** : entier non signé 8 bits, valeurs **0–255**.
- **`new byte[columns, rows]`** : crée un **tableau 2D** avec **`columns * rows`** cellules ; le runtime initialise tout à **0** (valeur par défaut de `byte`) **sans** boucle `for` dans votre code.
- Dans ce projet, **`GridData`** stocke l’état des cases ainsi (ex. libre / occupé).

## Syntaxe `=>` sur une méthode

Une méthode peut être écrite en **forme expression** :

```csharp
public bool IsFree(Vector2Int cell) =>
    IsInBounds(cell) && cells[cell.x, cell.y] == Free;
```

Équivaut à un corps `{ return ...; }` sur une seule expression. Utile pour des **petites** méthodes ; sinon préférer des accolades pour la lisibilité.

## Chaîne `AreAllFree`

- **`GridData.AreAllFree(IEnumerable<Vector2Int>)`** parcourt la liste et vérifie **`IsFree`** pour chaque cellule.
- Appel indirect actuel : **`BiofiltreManager`** → **`GridManager.AreAllCellsFree`** → **`GridData.AreAllFree`**.

## Cursor — navigation C# (définition, références)

- Le marketplace **Cursor** n’est pas identique à celui de **VS Code** ; les extensions **Microsoft** (C#, Dev Kit) peuvent être **absentes ou incompatibles** (licence / éditeur).
- Les extensions **C# « free / libre »** (souvent stack open source type **OmniSharp**) sont souvent la voie réaliste dans Cursor.
- Pour que l’indexation fonctionne avec **Unity** : **Edit → Preferences → External Tools → Regenerate project files** (fichiers `.csproj` / `.sln` à jour).
- Raccourcis usuels une fois le serveur C# actif : **F12** (définition), **Shift+F12** (références).

## Voir aussi (projet)

- Singleton inventaire et ordre d’`Awake` : `Notes/Farm/PlayerInventory_Instance_et_ordre_Awake.md`
- Carte des flux ferme : `Notes/Farm/SYSTEMES_carte_mentale.md`
