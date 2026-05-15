using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Attached to a plant GameObject. On click, vérifie la maturité, résout l'item
/// via ItemDatabase et ajoute la récolte au PlayerInventory singleton.
/// Délègue l'affichage du panneau à <see cref="HarvestPanelUI"/>.
/// </summary>
[RequireComponent(typeof(PlantGrow))]
[RequireComponent(typeof(Collider2D))]
public class PlantHarvestInteractor : MonoBehaviour, IPointerClickHandler
{
    private const string InventoryFullFeedbackMessage = "Inventaire plein !";

    [Header("Dependencies")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private HarvestPanelUI harvestPanelUI;

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
    /// Injecte le HarvestPanelUI depuis BiofiltreManager si non assigné dans le prefab.
    /// </summary>
    public void InjectHarvestPanel(HarvestPanelUI panel)
    {
        harvestPanelUI ??= panel;
    }

    /// <summary>
    /// Injecte l'ItemDatabase depuis BiofiltreManager.
    /// Appelé après instantiation si la référence n'est pas déjà assignée dans le prefab.
    /// </summary>
    public void InjectInventory(ItemDatabase database)
    {
        itemDatabase ??= database;
    }

    /// <summary>
    /// Hôte des popups ferme (même que <see cref="BiofiltreManager"/>). Évite un Find répété si injecté.
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
    /// Triggered by the EventSystem when the player clicks the plant in the scene.
    /// Requires a Physics2DRaycaster on the camera.
    /// Récolte directement si le stade le permet, sans ouvrir de panel intermédiaire.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        ConfirmHarvest();
    }

    /// <summary>
    /// Ouvre le popup d'info pour cette plante, peu importe son stade.
    /// Le panel gère lui-même la visibilité du bouton de récolte.
    /// </summary>
    public void TryHarvest()
    {
        PlantDefinition definition = ResolveDefinition();

        ScreenPopupHost popupHost = FindFirstObjectByType<ScreenPopupHost>();
        if (popupHost != null &&
            popupHost.TryShowPopup(PopupId.FarmPlantHarvest, out HarvestPanelUI hostPanel))
        {
            hostPanel.Open(this, plantGrow, definition);
            return;
        }

        if (harvestPanelUI != null)
        {
            harvestPanelUI.Open(this, plantGrow, definition);
            return;
        }

        // Fallback direct sans panel
        HarvestStageConfig? config = GetCurrentHarvestConfig();
        if (!config.HasValue)
        {
            Debug.Log($"[PlantHarvestInteractor] '{gameObject.name}' n'est pas récoltable à ce stade.");
            return;
        }

        ItemDefinition item = ResolveItem(config.Value);
        if (item != null)
            ApplyHarvest(item, config.Value);
    }

    /// <summary>
    /// Appelé par HarvestPanelUI quand le joueur confirme la récolte.
    /// Résout la config du stade courant et applique la récolte si le stade est récoltable.
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
    /// Arrache la plante sans récolter : libère les cellules et la détruit.
    /// Appelé par HarvestPanelUI via le bouton "Arracher".
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
        // Libérer les cellules dans la grille + désenregistrer la plante
        if (gridManager != null && occupiedCells != null)
        {
            gridManager.FreeCells(occupiedCells);
            gridManager.UnregisterPlant(occupiedCells);
        }

        // Remettre les cellules visuelles à l'état vide
        if (visualizer != null && occupiedCells != null)
        {
            foreach (Vector2Int coords in occupiedCells)
                visualizer.GetCell(coords)?.SetVisualState(false);
        }

        // Supprimer la plante de la scène
        onPlantRemoved?.Invoke();
        Destroy(gameObject);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Retourne la config de récolte du stade courant, ou null si la plante n'est pas récoltable maintenant.
    /// </summary>
    public HarvestStageConfig? GetCurrentHarvestConfig()
    {
        PlantDefinition definition = ResolveDefinition();
        return definition?.GetHarvestConfig(plantGrow.CurrentStage);
    }

    private bool IsHarvestable() => GetCurrentHarvestConfig().HasValue;

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
        ScreenPopupHost host = injectedFarmPopupHost;
        if (host == null)
            host = FindFirstObjectByType<ScreenPopupHost>();

        if (host != null &&
            host.HasPopup(PopupId.FarmInventoryFeedback) &&
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
