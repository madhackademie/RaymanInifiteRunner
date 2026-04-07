using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Minimal seed selection panel.
/// Opens when the player clicks an empty biofiltre cell, displays available seeds,
/// and starts the <see cref="PlantPlacementPreview"/> on selection.
/// </summary>
public class SeedSelectionUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeButton;

    [Header("Seed slots")]
    [Tooltip("List of seeds available for planting, with their matching world prefab.")]
    [SerializeField] private List<SeedEntry> availableSeeds = new();
    [SerializeField] private SeedSlotUI slotPrefab;
    [SerializeField] private Transform slotsContainer;

    [Header("Placement preview")]
    [Tooltip("The PlantPlacementPreview component that handles ghost snapping and click-to-confirm.")]
    [SerializeField] private PlantPlacementPreview placementPreview;

    private BiofiltreCell    targetCell;
    private BiofiltreManager targetManager;
    private GridManager      gridManager;

    /// <summary>True while a placement ghost is active and following the mouse.</summary>
    public bool IsPreviewActive => placementPreview != null && placementPreview.enabled;

    private readonly List<SeedSlotUI> spawnedSlots = new();

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        panel.SetActive(false);
    }

    /// <summary>Opens the panel targeting a specific cell and manager.</summary>
    public void Open(BiofiltreCell cell, BiofiltreManager manager)
    {
        targetCell    = cell;
        targetManager = manager;
        gridManager   = manager.GetComponent<GridManager>();

        BuildSlots();
        panel.SetActive(true);
    }

    /// <summary>Closes and resets the panel.</summary>
    public void Close()
    {
        panel.SetActive(false);
        targetCell    = null;
        targetManager = null;
        gridManager   = null;
    }

    // ── Slot building ─────────────────────────────────────────────────────────

    private void BuildSlots()
    {
        // Clear previous slots
        foreach (SeedSlotUI slot in spawnedSlots)
        {
            slot.OnSlotClicked -= HandleSeedSelected;
            Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();

        foreach (SeedEntry entry in availableSeeds)
        {
            if (entry.plantDefinition == null || entry.plantPrefab == null)
                continue;

            SeedSlotUI slot = Instantiate(slotPrefab, slotsContainer);
            slot.Bind(entry);

            bool fits = targetManager.CanPlace(targetCell.GridCoordinates, entry.plantDefinition);
            slot.SetInteractable(fits);

            slot.OnSlotClicked += HandleSeedSelected;
            spawnedSlots.Add(slot);
        }
    }

    private void HandleSeedSelected(SeedEntry entry)
    {
        if (targetCell == null || targetManager == null)
            return;

        // Capture references before Close() nulls them
        BiofiltreCell    cell    = targetCell;
        BiofiltreManager manager = targetManager;
        GridManager      grid    = gridManager;

        Close();

        if (placementPreview == null)
        {
            Debug.LogWarning("[SeedSelectionUI] No PlantPlacementPreview assigned — falling back to direct placement.", this);
            manager.PlantSeed(cell, entry.plantDefinition, entry.plantPrefab);
            return;
        }

        placementPreview.Begin(
            entry.plantDefinition,
            entry.plantPrefab,
            cell,
            grid,
            manager
        );
    }
}

/// <summary>Associates a <see cref="PlantDefinition"/> with the world prefab to instantiate.</summary>
[Serializable]
public class SeedEntry
{
    public PlantDefinition plantDefinition;
    public GameObject      plantPrefab;
}
