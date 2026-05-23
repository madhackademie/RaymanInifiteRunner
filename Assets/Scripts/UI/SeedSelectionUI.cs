using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Seed selection panel. Opens on empty biofiltre cell click; lists seeds owned in <see cref="PlayerInventory"/>.
/// </summary>
public class SeedSelectionUI : MonoBehaviour
{
    private const string DefaultEmptyMessage =
        "Aucune graine dans l'inventaire. Ouvrez le Shop (barre du bas) pour en acheter.";
    private enum SeedInventoryVisualState
    {
        Unknown = 0,
        Empty = 1,
        HasSeeds = 2
    }

    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private Button closeButton;

    [Header("Seed slots")]
    [Tooltip("Seeds plantable when the player owns stock (seedItem required per entry).")]
    [SerializeField] private List<SeedEntry> availableSeeds = new();
    [SerializeField] private SeedSlotUI slotPrefab;
    [SerializeField] private Transform slotsContainer;

    [Header("Empty inventory")]
    [SerializeField] private GameObject emptyStatePanel;
    [SerializeField] private TextMeshProUGUI emptyStateLabel;
    [SerializeField] private Button openShopButton;

    [Header("Placement preview")]
    [SerializeField] private PlantPlacementPreview placementPreview;

    private BiofiltreCell targetCell;
    private BiofiltreManager targetManager;
    private GridManager gridManager;
    private PlayerInventory playerInventory;

    public bool IsPreviewActive => placementPreview != null && placementPreview.enabled;

    private readonly List<SeedSlotUI> spawnedSlots = new();
    private string defaultPanelTitle = string.Empty;
    private SeedInventoryVisualState visualState = SeedInventoryVisualState.Unknown;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);

        if (openShopButton != null)
            openShopButton.onClick.AddListener(HandleOpenShopClicked);

        if (emptyStatePanel != null)
            emptyStatePanel.SetActive(false);

        CacheDefaultPanelTitle();
        visualState = SeedInventoryVisualState.Unknown;
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        UnsubscribeInventory();
    }

    public void InjectPlacementPreview(PlantPlacementPreview preview)
    {
        if (preview != null)
            placementPreview = preview;
    }

    public void InjectPlayerInventory(PlayerInventory inventory)
    {
        UnsubscribeInventory();
        playerInventory = inventory;
        SubscribeInventory();
    }

    public void Open(BiofiltreCell cell, BiofiltreManager manager)
    {
        targetCell = cell;
        targetManager = manager;
        gridManager = manager.GetComponent<GridManager>();

        if (playerInventory == null)
            playerInventory = PlayerInventory.Instance;

        BuildSlots();
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        targetCell = null;
        targetManager = null;
        gridManager = null;
    }

    private void SubscribeInventory()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged += HandleInventoryChanged;
    }

    private void UnsubscribeInventory()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;
    }

    private void HandleInventoryChanged()
    {
        if (!panel.activeSelf)
            return;

        BuildSlots();
    }

    private void BuildSlots()
    {
        ClearSpawnedSlots();

        if (playerInventory == null)
        {
            SetVisualState(SeedInventoryVisualState.Empty, DefaultEmptyMessage);
            return;
        }

        int visibleCount = 0;

        foreach (SeedEntry entry in availableSeeds)
        {
            if (!IsEntryPlantable(entry, out int stock))
                continue;

            SeedSlotUI slot = Instantiate(slotPrefab, slotsContainer);
            slot.Bind(entry, stock);

            bool fits = targetManager != null &&
                        targetCell != null &&
                        targetManager.CanPlace(targetCell.GridCoordinates, entry.plantDefinition);
            slot.SetInteractable(fits && stock > 0);

            slot.OnSlotClicked += HandleSeedSelected;
            spawnedSlots.Add(slot);
            visibleCount++;
        }

        if (visibleCount == 0)
            SetVisualState(SeedInventoryVisualState.Empty, DefaultEmptyMessage);
        else
            SetVisualState(SeedInventoryVisualState.HasSeeds);
    }

    private bool IsEntryPlantable(SeedEntry entry, out int stock)
    {
        stock = 0;

        if (entry == null || entry.plantDefinition == null || entry.plantPrefab == null || entry.seedItem == null)
            return false;

        stock = playerInventory.Count(entry.seedItem);
        return stock > 0;
    }

    private void SetVisualState(SeedInventoryVisualState nextState, string emptyMessage = null)
    {
        if (visualState == nextState && nextState != SeedInventoryVisualState.Empty)
            return;

        visualState = nextState;
        bool hasSeeds = nextState == SeedInventoryVisualState.HasSeeds;

        if (slotsContainer != null)
            slotsContainer.gameObject.SetActive(hasSeeds);

        if (emptyStatePanel != null)
        {
            emptyStatePanel.SetActive(!hasSeeds);
            if (!hasSeeds && emptyStateLabel != null)
                emptyStateLabel.text = string.IsNullOrEmpty(emptyMessage) ? DefaultEmptyMessage : emptyMessage;

            if (openShopButton != null)
                openShopButton.gameObject.SetActive(!hasSeeds);

            if (hasSeeds)
                RestoreDefaultPanelTitle();

            return;
        }

        if (hasSeeds)
        {
            RestoreDefaultPanelTitle();
            return;
        }

        string message = string.IsNullOrEmpty(emptyMessage) ? DefaultEmptyMessage : emptyMessage;
        TextMeshProUGUI fallbackLabel = ResolveFallbackTitleLabel();
        if (fallbackLabel != null)
        {
            if (fallbackLabel.text != message)
                defaultPanelTitle = fallbackLabel.text;

            fallbackLabel.text = message;
        }
    }

    private void CacheDefaultPanelTitle()
    {
        TextMeshProUGUI fallbackLabel = ResolveFallbackTitleLabel();
        if (fallbackLabel != null)
            defaultPanelTitle = fallbackLabel.text;
    }

    private TextMeshProUGUI ResolveFallbackTitleLabel()
    {
        if (titleLabel != null)
            return titleLabel;

        if (panel == null)
            return null;

        TextMeshProUGUI[] labels = panel.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI label in labels)
        {
            if (label != null && label.transform.parent == panel.transform)
            {
                titleLabel = label;
                return titleLabel;
            }
        }

        if (labels.Length > 0)
            titleLabel = labels[0];

        return titleLabel;
    }

    private void RestoreDefaultPanelTitle()
    {
        if (string.IsNullOrEmpty(defaultPanelTitle))
            return;

        TextMeshProUGUI fallbackLabel = ResolveFallbackTitleLabel();
        if (fallbackLabel != null)
            fallbackLabel.text = defaultPanelTitle;
    }

    private void ClearSpawnedSlots()
    {
        foreach (SeedSlotUI slot in spawnedSlots)
        {
            slot.OnSlotClicked -= HandleSeedSelected;
            Destroy(slot.gameObject);
        }

        spawnedSlots.Clear();
    }

    private void HandleSeedSelected(SeedEntry entry)
    {
        if (targetCell == null || targetManager == null || entry == null)
            return;

        if (playerInventory == null || playerInventory.Count(entry.seedItem) <= 0)
        {
            BuildSlots();
            return;
        }

        BiofiltreCell cell = targetCell;
        BiofiltreManager manager = targetManager;
        GridManager grid = gridManager;

        Close();

        if (placementPreview == null)
        {
            if (!manager.TryPlantSeedAt(cell.GridCoordinates, entry.plantDefinition, entry.plantPrefab, entry.seedItem))
                Debug.LogWarning("[SeedSelectionUI] Plantation impossible (stock ou emplacement).", this);
            return;
        }

        placementPreview.Begin(
            entry.plantDefinition,
            entry.plantPrefab,
            entry.seedItem,
            cell,
            grid,
            manager);
    }

    private void HandleOpenShopClicked()
    {
        Close();
        targetManager?.HideFarmSeedSelectionPopup();

        if (UIManager.Instance != null && UIManager.Instance.TryShowScreen(ScreenId.Shop))
            return;

        Debug.LogWarning("[SeedSelectionUI] Impossible d'ouvrir le shop (UIManager).", this);
    }

    public bool TryGetPlantPrefab(PlantDefinition definition, out GameObject prefab)
    {
        prefab = null;
        if (definition == null)
            return false;

        foreach (SeedEntry entry in availableSeeds)
        {
            if (entry.plantDefinition == definition && entry.plantPrefab != null)
            {
                prefab = entry.plantPrefab;
                return true;
            }
        }

        return false;
    }

    public bool TryGetPlantDefinitionById(string plantId, out PlantDefinition definition)
    {
        definition = null;
        if (string.IsNullOrEmpty(plantId))
            return false;

        foreach (SeedEntry entry in availableSeeds)
        {
            if (entry.plantDefinition != null && entry.plantDefinition.plantId == plantId)
            {
                definition = entry.plantDefinition;
                return true;
            }
        }

        return false;
    }
}

/// <summary>Plant definition, world prefab, and inventory item consumed when planting.</summary>
[Serializable]
public class SeedEntry
{
    public PlantDefinition plantDefinition;
    public GameObject plantPrefab;
    public ItemDefinition seedItem;
}
