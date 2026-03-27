# Architecture plante générique (Unity + C#)

## Objectif

Construire une base réutilisable pour toutes les plantes (laitue, tomate, etc.) avec :
- croissance dans le temps,
- état récoltable (`Mature`),
- clic de récolte,
- tentative d’ajout inventaire via API unique.

---

## Recommandation d’architecture (copie fidèle du cadrage)

Le meilleur compromis est **hybride dès le départ** :

- **ScriptableObject** = données statiques de type de plante (template)
- **MonoBehaviour** = état runtime de l’instance dans la scène

### 1) `PlantDefinition` (ScriptableObject)
Un asset par type de plante (laitue, tomate, etc.) avec :

- `plantId` (string/enum)
- `displayName`
- `growthDurationSeconds` (durée totale)
- `harvestItemId`
- `harvestAmountMin`, `harvestAmountMax` (ou fixe)
- `maxHarvestCount` (si récolte multiple)
- références visuelles par stade (prefab/sprite/material)
- (optionnel) courbe de croissance `AnimationCurve`

### 2) `PlantInstance` (MonoBehaviour)
Sur l’objet en scène, avec :

- `PlantDefinition definition`
- `PlantState state` (`Seed`, `Growing`, `Mature`, `Harvested`)
- `float elapsedGrowth`
- `bool isMature` (ou dérivé de `state`)
- `int remainingHarvests`
- timestamps si offline (`lastUpdateUtcTicks`)
- refs runtime (renderer/collider)

### 3) Events/listeners à prévoir

- `OnStateChanged(PlantState old, PlantState next)`
- `OnMatured(PlantInstance plant)`
- `OnHarvestRequested(PlantInstance plant)`
- `OnHarvestSucceeded(PlantInstance plant, int amount)`
- `OnHarvestFailed(PlantInstance plant, HarvestFailReason reason)`

---

## Pourquoi pas “MonoBehaviour only” ?

Possible, mais moins scalable :
- duplication des données par prefab/objet,
- maintenance plus coûteuse dès 5+ types de plantes,
- refactor plus lourd ensuite.

Le combo SO + MB garde une implémentation simple maintenant et robuste ensuite.

---

## Squelette de code — prêt à coller

```csharp
// PlantState.cs
public enum PlantState
{
    Seed,
    Growing,
    Mature,
    Harvested
}
```

```csharp
// InventoryAddResult.cs
public enum InventoryAddResult
{
    Success,
    InventoryFull,
    InvalidItem
}
```

```csharp
// PlantDefinition.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Plant Definition", fileName = "PlantDefinition")]
public class PlantDefinition : ScriptableObject
{
    [Header("Identity")]
    public string plantId;
    public string displayName;

    [Header("Growth")]
    public float growthDurationSeconds = 30f;

    [Header("Harvest")]
    public string harvestItemId;
    public int harvestAmountMin = 1;
    public int harvestAmountMax = 1;
    public int maxHarvestCount = 1;

    [Header("Visuals (optional)")]
    public GameObject seedVisual;
    public GameObject growingVisual;
    public GameObject matureVisual;
}
```

```csharp
// IInventoryService.cs
public interface IInventoryService
{
    InventoryAddResult TryAdd(string itemId, int amount);
}
```

```csharp
// PlantInstance.cs
using System;
using UnityEngine;

public class PlantInstance : MonoBehaviour
{
    [SerializeField] private PlantDefinition definition;
    [SerializeField] private PlantState state = PlantState.Seed;
    [SerializeField] private float elapsedGrowth;
    [SerializeField] private int remainingHarvests = 1;
    [SerializeField] private long lastUpdateUtcTicks;

    public event Action<PlantState, PlantState> OnStateChanged;
    public event Action<PlantInstance> OnMatured;
    public event Action<PlantInstance> OnHarvestRequested;
    public event Action<PlantInstance, int> OnHarvestSucceeded;
    public event Action<PlantInstance, InventoryAddResult> OnHarvestFailed;

    public PlantState State => state;
    public bool IsMature => state == PlantState.Mature;

    private IInventoryService inventoryService;

    public void Initialize(IInventoryService inventory)
    {
        inventoryService = inventory;
        remainingHarvests = Mathf.Max(1, definition.maxHarvestCount);
        if (state == PlantState.Seed)
            SetState(PlantState.Growing);
    }

    private void Update()
    {
        if (state != PlantState.Growing || definition == null) return;

        elapsedGrowth += Time.deltaTime;
        if (elapsedGrowth >= definition.growthDurationSeconds)
        {
            SetState(PlantState.Mature);
            OnMatured?.Invoke(this);
        }
    }

    private void OnMouseDown()
    {
        if (!IsMature) return;
        TryHarvest();
    }

    public void TryHarvest()
    {
        if (!IsMature || inventoryService == null || definition == null) return;

        OnHarvestRequested?.Invoke(this);

        int amount = UnityEngine.Random.Range(definition.harvestAmountMin, definition.harvestAmountMax + 1);
        var result = inventoryService.TryAdd(definition.harvestItemId, amount);

        if (result == InventoryAddResult.Success)
        {
            remainingHarvests--;
            OnHarvestSucceeded?.Invoke(this, amount);
            SetState(remainingHarvests > 0 ? PlantState.Growing : PlantState.Harvested);
            if (state == PlantState.Growing) elapsedGrowth = 0f;
        }
        else
        {
            OnHarvestFailed?.Invoke(this, result);
            // L'objet reste Mature en cas d'échec d'inventaire.
        }
    }

    private void SetState(PlantState next)
    {
        if (state == next) return;
        var prev = state;
        state = next;
        OnStateChanged?.Invoke(prev, next);
    }
}
```

---

## Ordre d’implémentation recommandé

1. Créer `PlantState`, `PlantDefinition`, `PlantInstance`.
2. Brancher une implémentation minimale de `IInventoryService`.
3. Tester 3 scénarios:
   - plante pas mature -> clic ignoré,
   - plante mature + inventaire OK -> récolte,
   - plante mature + inventaire plein -> échec + plante reste mature.
4. Ajouter ensuite la persistance offline (timestamp UTC).
