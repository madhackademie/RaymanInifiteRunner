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

    [Tooltip("Panel de récolte ouvert quand le joueur clique sur une plante mature.")]
    [SerializeField] private HarvestPanelUI harvestPanelUI;

    [Header("Harvest")]
    [Tooltip("Base de données d'items pour résoudre les récoltes. Injectée dans chaque PlantHarvestInteractor.")]
    [SerializeField] private ItemDatabase itemDatabase;

    private PlayerInventory playerInventory;
    private BiofiltreGridVisualizer visualizer;
    private GridManager gridManager;

    private void Awake()
    {
        visualizer      = GetComponent<BiofiltreGridVisualizer>();
        gridManager     = GetComponent<GridManager>();
        playerInventory = PlayerInventory.Instance;

        if (playerInventory == null)
            Debug.LogWarning("[BiofiltreManager] PlayerInventory.Instance est null — le GameManager est-il présent dans la scène ?", this);
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

        if (gridManager.IsCellFree(cell.GridCoordinates))
        {
            // Cellule libre → sélection de graine
            if (seedSelectionUI == null)
            {
                Debug.LogWarning("[BiofiltreManager] No SeedSelectionUI assigned.", this);
                return;
            }
            seedSelectionUI.Open(cell, this);
        }
        else
        {
            // Cellule occupée → ouvrir le popup d'info plante
            TryOpenPlantPopup(cell.GridCoordinates);
        }
    }

    /// <summary>
    /// Ouvre le popup d'info pour la plante occupant la cellule cliquée.
    /// Lookup O(1) via le registre de GridManager — aucune recherche spatiale.
    /// </summary>
    private void TryOpenPlantPopup(Vector2Int coords)
    {
        GameObject plantObj = gridManager.GetPlantAt(coords);

        if (plantObj == null)
        {
            Debug.Log($"[BiofiltreManager] Aucune plante enregistrée à la cellule {coords}.");
            return;
        }

        if (harvestPanelUI == null)
        {
            Debug.LogWarning("[BiofiltreManager] HarvestPanelUI non assigné.", this);
            return;
        }

        PlantGrow plantGrow = plantObj.GetComponent<PlantGrow>();
        PlantDefinitionHolder holder = plantObj.GetComponent<PlantDefinitionHolder>();
        PlantHarvestInteractor interactor = plantObj.GetComponent<PlantHarvestInteractor>();

        if (plantGrow == null)
        {
            Debug.LogWarning($"[BiofiltreManager] PlantGrow manquant sur '{plantObj.name}'.", this);
            return;
        }

        harvestPanelUI.Open(interactor, plantGrow, holder != null ? holder.Definition : null);
    }

    /// <summary>
    /// Cherche le PlantHarvestInteractor sur la plante occupant la cellule cliquée
    /// et délègue l'ouverture du panneau de récolte.
    /// </summary>
    private void TryOpenHarvestPanel(Vector2Int coords)
    {
        // On cherche la plante dans le container des plantes par position de cellule
        Vector2 worldCenter = gridManager.GridToWorldCenter(coords);

        PlantHarvestInteractor interactor = FindInteractorAt(worldCenter);

        if (interactor == null)
        {
            Debug.Log($"[BiofiltreManager] Aucun PlantHarvestInteractor trouvé à la cellule {coords}.");
            return;
        }

        interactor.TryHarvest();
    }

    /// <summary>
    /// Cherche le <see cref="PlantHarvestInteractor"/> le plus proche du centre monde donné
    /// parmi les enfants du container de plantes.
    /// </summary>
    private PlantHarvestInteractor FindInteractorAt(Vector2 worldCenter)
    {
        const float SearchRadius = 0.1f;

        PlantHarvestInteractor closest  = null;
        float                  minDist  = float.MaxValue;

        foreach (Transform child in visualizer.PlantsContainer)
        {
            float dist = Vector2.Distance(child.position, worldCenter);

            if (dist < SearchRadius && dist < minDist)
            {
                if (child.TryGetComponent(out PlantHarvestInteractor interactor))
                {
                    closest = interactor;
                    minDist = dist;
                }
            }
        }

        // Fallback : on cherche par footprint (plantes multi-cellules)
        if (closest == null)
        {
            foreach (Transform child in visualizer.PlantsContainer)
            {
                if (!child.TryGetComponent(out PlantHarvestInteractor interactor))
                    continue;

                if (!child.TryGetComponent(out PlantDefinitionHolder holder) || holder.Definition == null)
                    continue;

                Vector2Int anchor = gridManager.WorldToGrid(child.position);
                foreach (Vector2Int cell in holder.Definition.GetOccupiedCells(anchor))
                {
                    if (cell == gridManager.WorldToGrid(worldCenter))
                    {
                        closest = interactor;
                        break;
                    }
                }

                if (closest != null)
                    break;
            }
        }

        return closest;
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

        // Fournir le contexte grille et le panel de récolte à l'interacteur
        if (instance.TryGetComponent(out PlantHarvestInteractor harvestInteractor))
        {
            Vector2Int[] cells = System.Linq.Enumerable.ToArray(plantDefinition.GetOccupiedCells(anchor));
            harvestInteractor.Initialise(gridManager, visualizer, cells);
            harvestInteractor.InjectHarvestPanel(harvestPanelUI);
            harvestInteractor.InjectInventory(itemDatabase, playerInventory);
        }

        // Mark cells occupied in GridData + plant registry
        Vector2Int[] footprintCells = System.Linq.Enumerable.ToArray(plantDefinition.GetOccupiedCells(anchor));
        gridManager.OccupyCells(footprintCells);
        gridManager.RegisterPlant(footprintCells, instance);

        // Update visual states of affected cells
        foreach (Vector2Int coords in plantDefinition.GetOccupiedCells(anchor))
        {
            BiofiltreCell affectedCell = visualizer.GetCell(coords);
            affectedCell?.SetVisualState(true);
        }

        Debug.Log($"[BiofiltreManager] Planted '{plantDefinition.displayName}' at {anchor}.");
    }
}
