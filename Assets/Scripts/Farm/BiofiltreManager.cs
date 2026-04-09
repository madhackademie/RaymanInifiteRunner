using UnityEngine;

/// <summary>
/// Bridges the biofiltre grid and the planting UI.
/// Listens to cell clicks from <see cref="BiofiltreGridVisualizer"/>,
/// opens the seed selection panel for empty cells, and handles plant placement.
/// </summary>
[RequireComponent(typeof(BiofiltreGridVisualizer))]
[RequireComponent(typeof(GridManager))]
public class BiofiltreManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The seed selection panel prefab or scene reference.")]
    [SerializeField] private SeedSelectionUI seedSelectionUI;

    private BiofiltreGridVisualizer visualizer;
    private GridManager gridManager;

    private void Awake()
    {
        visualizer   = GetComponent<BiofiltreGridVisualizer>();
        gridManager  = GetComponent<GridManager>();
    }

    private void OnEnable()
    {
        visualizer.OnCellClicked += HandleCellClicked;
    }

    private void OnDisable()
    {
        visualizer.OnCellClicked -= HandleCellClicked;
    }

    // ── Cell click ────────────────────────────────────────────────────────────

    private void HandleCellClicked(BiofiltreCell cell)
    {
        // A placement preview is already running — let it handle all clicks.
        if (seedSelectionUI != null && seedSelectionUI.IsPreviewActive)
            return;

        if (!gridManager.IsCellFree(cell.GridCoordinates))
        {
            Debug.Log($"[BiofiltreManager] Cell {cell.GridCoordinates} is occupied — ignoring click.");
            return;
        }

        if (seedSelectionUI == null)
        {
            Debug.LogWarning("[BiofiltreManager] No SeedSelectionUI assigned.", this);
            return;
        }

        seedSelectionUI.Open(cell, this);
    }

    // ── Footprint query (called by SeedSelectionUI) ───────────────────────────

    /// <summary>
    /// Returns true if every cell of the plant's footprint is free at the given anchor.
    /// Used by the UI to enable or disable seed slots before the player selects one.
    /// </summary>
    public bool CanPlace(Vector2Int anchor, PlantDefinition plantDefinition)
    {
        if (plantDefinition == null) return false;
        return gridManager.AreAllCellsFree(plantDefinition.GetOccupiedCells(anchor));
    }

    // ── Plant placement ───────────────────────────────────────────────────────

    /// <summary>
    /// Plants the given definition on the target cell.
    /// Called by <see cref="SeedSelectionUI"/> after the player selects a seed (legacy direct path).
    /// </summary>
    public void PlantSeed(BiofiltreCell cell, PlantDefinition plantDefinition, GameObject plantPrefab)
    {
        PlantSeedAt(cell.GridCoordinates, plantDefinition, plantPrefab);
    }

    /// <summary>
    /// Plants the given definition at the specified grid anchor.
    /// Called by <see cref="PlantPlacementPreview"/> after the player confirms placement.
    /// </summary>
    public void PlantSeedAt(Vector2Int anchor, PlantDefinition plantDefinition, GameObject plantPrefab)
    {
        if (plantDefinition == null || plantPrefab == null)
        {
            Debug.LogWarning("[BiofiltreManager] PlantSeedAt called with null definition or prefab.", this);
            return;
        }

        // Verify the footprint is still free (multi-cell plants)
        foreach (Vector2Int occupied in plantDefinition.GetOccupiedCells(anchor))
        {
            if (!gridManager.IsCellFree(occupied))
            {
                Debug.Log($"[BiofiltreManager] Cannot plant — cell {occupied} is occupied.");
                return;
            }
        }

        // Instantiate under Plants container
        Vector2 worldCenter   = gridManager.GridToWorldCenter(anchor);
        Vector2 spawnPosition = worldCenter + plantDefinition.spriteWorldOffset;
        GameObject instance   = Instantiate(
            plantPrefab,
            spawnPosition,
            Quaternion.identity,
            visualizer.PlantsContainer
        );
        instance.name = $"{plantDefinition.displayName}_{anchor}";

        // Initialize PlantGrow to Graine stage
        if (instance.TryGetComponent(out PlantGrow plantGrow))
            plantGrow.SetStage(PlantGrow.GrowthStage.Graine);

        // Provide PlantDefinition to optional harvest interactor
        if (instance.TryGetComponent(out PlantDefinitionHolder holder))
            holder.Initialise(plantDefinition);

        // Mark cells occupied in GridData
        gridManager.OccupyCells(plantDefinition.GetOccupiedCells(anchor));

        // Update visual states of affected cells
        foreach (Vector2Int coords in plantDefinition.GetOccupiedCells(anchor))
        {
            BiofiltreCell affectedCell = visualizer.GetCell(coords);
            affectedCell?.SetVisualState(true);
        }

        Debug.Log($"[BiofiltreManager] Planted '{plantDefinition.displayName}' at {anchor}.");
    }
}
