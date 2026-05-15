using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Attached to a plant GameObject. On click, ouvre le popup récolte via <see cref="ScreenPopupHost"/>.
/// </summary>
[RequireComponent(typeof(PlantGrow))]
[RequireComponent(typeof(Collider2D))]
public class PlantHarvestInteractor : MonoBehaviour, IPointerClickHandler
{
    private const string InventoryFullFeedbackMessage = "Inventaire plein !";

    [Header("Dependencies")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Harvest")]
    [Tooltip("Surcharge le harvestItemId de la PlantDefinition si renseigné.")]
    [SerializeField] private string harvestItemIdOverride;

    private PlantGrow plantGrow;
    private PlantDefinition cachedDefinition;
    private ScreenPopupHost injectedFarmPopupHost;

    // Contexte grille — fourni par BiofiltreManager après instantiation.
    private GridManager gridManager;
    private BiofiltreGridVisualizer visualizer;
    private Vector2Int[] occupiedCells;
    private Action onPlantRemoved;

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
    /// Injecte l'ItemDatabase depuis BiofiltreManager.
    /// </summary>
    public void InjectInventory(ItemDatabase database)
    {
        itemDatabase ??= database;
    }

    /// <summary>
    /// Hôte des popups ferme (même que <see cref="BiofiltreManager"/>).
    /// </summary>
    public void InjectFarmPopupHost(ScreenPopupHost host)
    {
        injectedFarmPopupHost = host;
    }

    /// <summary>
    /// Callback notifiee apres suppression de la plante (recolte/arrache).
    /// </summary>
    public void SetOnPlantRemoved(Action callback)
    {
        onPlantRemoved = callback;
    }

    // ── IPointerClickHandler ──────────────────────────────────────────────────

    /// <summary>
    /// Ouvre le popup d'info plante (nécessite Physics2DRaycaster sur la caméra).
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        TryHarvest();
    }

    /// <summary>
    /// Ouvre le popup d'info pour cette plante, peu importe son stade.
    /// </summary>
    public void TryHarvest()
    {
        if (!TryOpenHarvestPopup())
        {
            Debug.LogWarning(
                "[PlantHarvestInteractor] Popup récolte introuvable. " +
                $"Vérifiez ScreenPopupHost + binding ({ScreenId.FirstLvlFarm}, {PopupId.FarmPlantHarvest}).",
                this);
        }
    }

    /// <summary>
    /// Appelé par HarvestPanelUI quand le joueur confirme la récolte.
    /// </summary>
    public void ConfirmHarvest()
    {
        HarvestStageConfig? config = GetCurrentHarvestConfig();

        if (!config.HasValue)
        {
            Debug.Log($"[PlantHarvestInteractor] '{gameObject.name}' n'est pas récoltable à ce stade ({plantGrow.CurrentStage}).");
            return;
        }

        ItemDefinition item = ResolveItem(config.Value);
        if (item != null)
            ApplyHarvest(item, config.Value);
    }

    // ── Application ───────────────────────────────────────────────────────────

    private void ApplyHarvest(ItemDefinition item, HarvestStageConfig config)
    {
        PlayerInventory inventory = PlayerInventory.Instance;

        if (inventory == null)
        {
            Debug.LogWarning("[PlantHarvestInteractor] PlayerInventory.Instance introuvable — récolte annulée.", this);
            return;
        }

        int amount = UnityEngine.Random.Range(config.harvestAmountMin, config.harvestAmountMax + 1);
        InventoryResult result = inventory.TryAdd(item, amount);

        switch (result)
        {
            case InventoryResult.Success:
            case InventoryResult.Partial:
                Debug.Log($"[PlantHarvestInteractor] Récolté '{item.DisplayName}' x{amount}. Résultat : {result}.");
                OnHarvestSuccess();
                break;

            case InventoryResult.Full:
                Debug.Log($"[PlantHarvestInteractor] Inventaire plein — '{item.DisplayName}' non ajouté.");
                ShowInventoryFullFeedback();
                break;

            case InventoryResult.InvalidItem:
                Debug.LogWarning($"[PlantHarvestInteractor] Item invalide résolu pour '{gameObject.name}'.", this);
                break;
        }
    }

    /// <summary>
    /// Arrache la plante sans récolter.
    /// </summary>
    public void Uproot()
    {
        if (gridManager != null && occupiedCells != null)
        {
            gridManager.FreeCells(occupiedCells);
            gridManager.UnregisterPlant(occupiedCells);
        }

        if (visualizer != null && occupiedCells != null)
        {
            foreach (Vector2Int coords in occupiedCells)
                visualizer.GetCell(coords)?.SetVisualState(false);
        }

        onPlantRemoved?.Invoke();
        Destroy(gameObject);
    }

    private void OnHarvestSuccess()
    {
        if (gridManager != null && occupiedCells != null)
        {
            gridManager.FreeCells(occupiedCells);
            gridManager.UnregisterPlant(occupiedCells);
        }

        if (visualizer != null && occupiedCells != null)
        {
            foreach (Vector2Int coords in occupiedCells)
                visualizer.GetCell(coords)?.SetVisualState(false);
        }

        onPlantRemoved?.Invoke();
        Destroy(gameObject);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private bool TryOpenHarvestPopup()
    {
        ScreenPopupHost host = ResolvePopupHost();
        if (host == null)
            return false;

        if (!host.TryShowPopup(PopupId.FarmPlantHarvest, out HarvestPanelUI panel))
            return false;

        panel.Open(this, plantGrow, ResolveDefinition());
        return true;
    }

    private ScreenPopupHost ResolvePopupHost()
    {
        if (injectedFarmPopupHost != null)
            return injectedFarmPopupHost;

        Debug.LogWarning(
            "[PlantHarvestInteractor] ScreenPopupHost non injecté. " +
            "Vérifiez BiofiltreManager / LevelController (FirstLvl).",
            this);
        return null;
    }

    public HarvestStageConfig? GetCurrentHarvestConfig()
    {
        PlantDefinition definition = ResolveDefinition();
        return definition?.GetHarvestConfig(plantGrow.CurrentStage);
    }

    private PlantDefinition ResolveDefinition()
    {
        if (cachedDefinition != null)
            return cachedDefinition;

        if (TryGetComponent(out PlantDefinitionHolder holder) && holder.Definition != null)
            cachedDefinition = holder.Definition;

        return cachedDefinition;
    }

    private ItemDefinition ResolveItem(HarvestStageConfig config)
    {
        if (itemDatabase == null)
        {
            Debug.LogWarning("[PlantHarvestInteractor] Aucun ItemDatabase assigné.", this);
            return null;
        }

        string itemId = !string.IsNullOrEmpty(harvestItemIdOverride)
            ? harvestItemIdOverride
            : config.harvestItemId;

        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning($"[PlantHarvestInteractor] Aucun harvestItemId configuré pour le stade '{config.stage}' sur '{gameObject.name}'.", this);
            return null;
        }

        ItemDefinition item = itemDatabase.GetById(itemId);

        if (item == null)
            Debug.LogWarning($"[PlantHarvestInteractor] ItemId '{itemId}' introuvable dans l'ItemDatabase.", this);

        return item;
    }

    private void ShowInventoryFullFeedback()
    {
        ScreenPopupHost host = ResolvePopupHost();
        if (host == null)
            return;

        if (host.HasPopup(PopupId.FarmInventoryFeedback) &&
            host.TryGetPopup(PopupId.FarmInventoryFeedback, out ResourceFeedbackPopupUI popup))
        {
            popup.ShowMessage(InventoryFullFeedbackMessage);
            return;
        }

        Debug.LogWarning(
            "[PlantHarvestInteractor] Popup inventaire plein introuvable. " +
            "Ajoutez un ScreenPopupBinding (FirstLvlFarm + PopupId.FarmInventoryFeedback + prefab ResourceFeedbackPopup) " +
            "dans UIManager.runtimePopupBindings (NavigationHUD).",
            this);
    }
}
