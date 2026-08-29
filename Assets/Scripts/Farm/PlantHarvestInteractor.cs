using UnityEngine;
using System;

/// <summary>
/// Attaché à une plante posée : résolution de la récolte et ouverture du popup associé.
/// </summary>
/// <remarks>
/// Ne reçoit plus le clic directement : il arrive par la cellule de grille
/// (<see cref="FarmGridPointerInput"/> puis <see cref="BiofiltreManager"/>), donc
/// sans collider ni physique sur la plante.
/// </remarks>
[RequireComponent(typeof(PlantGrow))]
public class PlantHarvestInteractor : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Harvest")]
    [Tooltip("Surcharge le harvestItemId de la PlantDefinition si renseigné.")]
    [SerializeField] private string harvestItemIdOverride;

    private PlantGrow plantGrow;
    private PlantDefinition cachedDefinition;
    private ScreenPopupHost injectedFarmPopupHost;
    private BiofiltreManager biofiltreManager;

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
    /// Manager ferme pour ignorer les clics consommés par la preview de placement.
    /// </summary>
    public void InjectBiofiltreManager(BiofiltreManager manager)
    {
        biofiltreManager = manager;
    }

    /// <summary>
    /// Callback notifiee apres suppression de la plante (recolte/arrache).
    /// </summary>
    public void SetOnPlantRemoved(Action callback)
    {
        onPlantRemoved = callback;
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

        ActionPointService actionPoints = ActionPointService.Instance;
        int harvestCost = actionPoints != null
            ? actionPoints.GetCostForAction(ActionPointActionId.Harvest)
            : 0;

        if (actionPoints != null &&
            !actionPoints.TryConsume(ActionPointActionId.Harvest, harvestCost, out string apMessage))
        {
            Debug.Log($"[PlantHarvestInteractor] ConfirmHarvest: {apMessage}", this);
            return;
        }

        int amount = UnityEngine.Random.Range(config.harvestAmountMin, config.harvestAmountMax + 1);
        InventoryResult result = inventory.TryAdd(item, amount);

        // Capturer la position monde AVANT RemovePlantFromGrid / Destroy.
        Vector3 harvestWorldPos = transform.position;

        switch (result)
        {
            case InventoryResult.Success:
            case InventoryResult.Partial:
                ShowHarvestRewardFeedback(item, amount, harvestWorldPos);
                OnHarvestSuccess();
                break;

            case InventoryResult.Full:
                Debug.Log($"[PlantHarvestInteractor] Inventaire plein — '{item.DisplayName}' non ajouté.");
                actionPoints?.Refund(harvestCost);
                ShowInventoryFullFeedback();
                break;

            case InventoryResult.InvalidItem:
                Debug.LogWarning($"[PlantHarvestInteractor] Item invalide résolu pour '{gameObject.name}'.", this);
                actionPoints?.Refund(harvestCost);
                break;
        }
    }

    /// <summary>
    /// Arrache la plante sans récolter.
    /// </summary>
    public void Uproot() => RemovePlantFromGrid();

    private void OnHarvestSuccess() => RemovePlantFromGrid();

    /// <summary>
    /// Libère les cellules occupées, réinitialise leurs visuels, notifie et détruit la plante.
    /// Chemin commun à la récolte réussie et à l'arrachage.
    /// </summary>
    private void RemovePlantFromGrid()
    {
        Vector3 burstWorldPos = transform.position;

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

        // Destroy() est différé en fin de frame : retirer du conteneur avant la sauvegarde
        // pour ne pas réécrire la plante dans farm_state.json (CT-FARM-004).
        transform.SetParent(null, worldPositionStays: true);

        biofiltreManager?.PlayPlantingDirtBurst(burstWorldPos);

        onPlantRemoved?.Invoke();
        Destroy(gameObject);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

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

    private HarvestStageConfig? GetCurrentHarvestConfig()
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
            popup.ShowMessage(UiMessages.InventoryFull);
            return;
        }

        Debug.LogWarning(
            "[PlantHarvestInteractor] Popup inventaire plein introuvable. " +
            "Ajoutez un ScreenPopupBinding (FirstLvlFarm + PopupId.FarmInventoryFeedback + prefab ResourceFeedbackPopup) " +
            "dans UIManager.runtimePopupBindings (NavigationHUD).",
            this);
    }

    private void ShowHarvestRewardFeedback(ItemDefinition item, int amount, Vector3 worldPos)
    {
        if (item == null)
            return;

        ScreenPopupHost host = ResolvePopupHost();
        if (host == null)
            return;

        if (host.TryGetPopup(PopupId.FarmHarvestReward, out HarvestRewardFeedbackPopupUI popup))
        {
            popup.ShowHarvestReward(item.Icon, item.DisplayName, amount, worldPos);
            return;
        }

        Debug.LogWarning(
            "[PlantHarvestInteractor] Popup récompense récolte introuvable. " +
            "Ajoutez un ScreenPopupBinding (FirstLvlFarm + PopupId.FarmHarvestReward + prefab HarvestRewardFeedbackPopup) " +
            "dans UIManager.runtimePopupBindings (NavigationHUD).",
            this);
    }
}
