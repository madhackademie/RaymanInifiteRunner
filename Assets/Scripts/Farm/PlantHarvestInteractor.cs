using UnityEngine;

/// <summary>
/// Attached to a plant GameObject. On click, vérifie la maturité, résout l'item
/// via ItemDatabase et ajoute la récolte au PlayerInventory.
/// Délègue l'affichage du panneau à <see cref="HarvestPanelUI"/>.
/// </summary>
[RequireComponent(typeof(PlantGrow))]
[RequireComponent(typeof(Collider2D))]
public class PlantHarvestInteractor : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventoryFeedbackUI feedbackUI;
    [SerializeField] private HarvestPanelUI harvestPanelUI;

    [Header("Harvest")]
    [Tooltip("Surcharge le harvestItemId de la PlantDefinition si renseigné.")]
    [SerializeField] private string harvestItemIdOverride;

    private PlantGrow plantGrow;
    private PlantDefinition cachedDefinition;

    // Contexte grille — fourni par BiofiltreManager après instantiation.
    private GridManager gridManager;
    private BiofiltreGridVisualizer visualizer;
    private Vector2Int[] occupiedCells;

    private void Awake()
    {
        plantGrow = GetComponent<PlantGrow>();
    }

    // ── Initialisation ────────────────────────────────────────────────────────

    /// <summary>
    /// Appelé par BiofiltreManager après l'instantiation pour fournir le contexte de grille.
    /// </summary>
    public void Initialise(GridManager grid, BiofiltreGridVisualizer gridVisualizer, Vector2Int[] cells)
    {
        gridManager   = grid;
        visualizer    = gridVisualizer;
        occupiedCells = cells;
    }

    /// <summary>
    /// Injecte le HarvestPanelUI depuis BiofiltreManager si non assigné dans le prefab.
    /// </summary>
    public void InjectHarvestPanel(HarvestPanelUI panel)
    {
        harvestPanelUI ??= panel;
    }

    // ── Logique de récolte ────────────────────────────────────────────────────

    /// <summary>
    /// Vérifie la maturité et ouvre le HarvestPanel si disponible,
    /// ou applique directement la récolte sinon.
    /// </summary>
    public void TryHarvest()
    {
        if (!IsMature())
        {
            Debug.Log($"[PlantHarvestInteractor] '{gameObject.name}' n'est pas encore mature.");
            return;
        }

        PlantDefinition definition = ResolveDefinition();
        ItemDefinition  item       = ResolveItem(definition);

        if (item == null)
            return;

        if (harvestPanelUI != null && definition != null)
        {
            int amount = ResolveHarvestAmount(definition);
            harvestPanelUI.Open(this, definition, amount);
            return;
        }

        // Fallback direct sans panel
        ApplyHarvest(item, definition);
    }

    /// <summary>
    /// Appelé par HarvestPanelUI quand le joueur confirme la récolte.
    /// </summary>
    public void ConfirmHarvest()
    {
        PlantDefinition definition = ResolveDefinition();
        ItemDefinition  item       = ResolveItem(definition);

        if (item != null)
            ApplyHarvest(item, definition);
    }

    // ── Application ───────────────────────────────────────────────────────────

    private void ApplyHarvest(ItemDefinition item, PlantDefinition definition)
    {
        int amount = ResolveHarvestAmount(definition);
        InventoryResult result = playerInventory.TryAdd(item, amount);

        switch (result)
        {
            case InventoryResult.Success:
            case InventoryResult.Partial:
                Debug.Log($"[PlantHarvestInteractor] Récolté '{item.DisplayName}' x{amount}. Résultat : {result}.");
                OnHarvestSuccess();
                break;

            case InventoryResult.Full:
                Debug.Log($"[PlantHarvestInteractor] Inventaire plein — '{item.DisplayName}' non ajouté.");
                feedbackUI?.ShowInventoryFull();
                break;

            case InventoryResult.InvalidItem:
                Debug.LogWarning($"[PlantHarvestInteractor] Item invalide résolu pour '{gameObject.name}'.", this);
                break;
        }
    }

    private void OnHarvestSuccess()
    {
        // Libérer les cellules dans la grille
        if (gridManager != null && occupiedCells != null)
            gridManager.FreeCells(occupiedCells);

        // Remettre les cellules visuelles à l'état vide
        if (visualizer != null && occupiedCells != null)
        {
            foreach (Vector2Int coords in occupiedCells)
                visualizer.GetCell(coords)?.SetVisualState(false);
        }

        // Supprimer la plante de la scène
        Destroy(gameObject);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private bool IsMature()
    {
        PlantDefinition def = ResolveDefinition();
        if (def != null)
            return plantGrow.CurrentStage == def.HarvestStage;

        return plantGrow.CurrentStage == PlantGrow.GrowthStage.Mature;
    }

    private PlantDefinition ResolveDefinition()
    {
        if (cachedDefinition != null)
            return cachedDefinition;

        if (TryGetComponent(out PlantDefinitionHolder holder) && holder.Definition != null)
            cachedDefinition = holder.Definition;

        return cachedDefinition;
    }

    private ItemDefinition ResolveItem(PlantDefinition definition)
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("[PlantHarvestInteractor] Aucun PlayerInventory assigné.", this);
            return null;
        }

        if (itemDatabase == null)
        {
            Debug.LogWarning("[PlantHarvestInteractor] Aucun ItemDatabase assigné.", this);
            return null;
        }

        string itemId = !string.IsNullOrEmpty(harvestItemIdOverride)
            ? harvestItemIdOverride
            : definition?.harvestItemId;

        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning($"[PlantHarvestInteractor] Aucun harvestItemId configuré sur '{gameObject.name}'.", this);
            return null;
        }

        ItemDefinition item = itemDatabase.GetById(itemId);

        if (item == null)
            Debug.LogWarning($"[PlantHarvestInteractor] ItemId '{itemId}' introuvable dans l'ItemDatabase.", this);

        return item;
    }

    private static int ResolveHarvestAmount(PlantDefinition definition)
    {
        if (definition == null)
            return 1;

        return Random.Range(definition.harvestAmountMin, definition.harvestAmountMax + 1);
    }
}
