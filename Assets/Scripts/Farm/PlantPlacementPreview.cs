using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fantôme de pose aimanté à la grille. Vert = valide, rouge = invalide.
/// Appui principal = pose ; clic droit ou Échap = annulation.
/// </summary>
[DefaultExecutionOrder(-100)]
public class PlantPlacementPreview : MonoBehaviour
{
    private static readonly Color ColorValid   = new Color(0.3f, 1f, 0.4f, 0.75f);
    private static readonly Color ColorInvalid = new Color(1f, 0.2f, 0.2f, 0.6f);
    private const int GhostSortingOrder = 50;

    private GridManager      gridManager;
    private BiofiltreGridVisualizer visualizer;
    private BiofiltreManager biofiltreManager;
    private PlantDefinition  plantDefinition;
    private GameObject       plantPrefab;
    private ItemDefinition   seedItem;
    private BiofiltreCell    originCell;

    private GameObject       ghostInstance;
    private SpriteRenderer   ghostRenderer;

    private Vector2Int       currentCell;
    private bool             currentlyValid;
    private Camera           mainCamera;
    private readonly List<Vector2Int> previewedCells = new();

    // ── Initialisation ────────────────────────────────────────────────────────

    /// <summary>
    /// Initialises and activates the preview mode.
    /// Called by <see cref="SeedSelectionUI"/> after the player selects a seed.
    /// </summary>
    public void Begin(
        PlantDefinition  definition,
        GameObject       prefab,
        ItemDefinition   seed,
        BiofiltreCell    origin,
        GridManager      grid,
        BiofiltreManager manager)
    {
        plantDefinition  = definition;
        plantPrefab      = prefab;
        seedItem         = seed;
        originCell       = origin;
        gridManager      = grid;
        visualizer       = manager.GetComponent<BiofiltreGridVisualizer>();
        biofiltreManager = manager;
        mainCamera       = Camera.main;

        biofiltreManager.OnPlacementPreviewStarted();
        SpawnGhost();

        if (biofiltreManager.TryResolvePlacementAnchor(origin.GridCoordinates, plantDefinition, out Vector2Int resolvedAnchor))
            currentCell = resolvedAnchor;

        currentlyValid = biofiltreManager.CanPlace(currentCell, plantDefinition);
        RefreshFootprintPreview();
        enabled = true;
    }

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        enabled = false;  // inactive until Begin() is called
    }

    private void Update()
    {
        if (gridManager == null || ghostInstance == null)
            return;

        UpdateGhostPosition();

        bool confirmPressed = FarmPointerInput.TryGetPrimaryPress(out _, out int pointerId) &&
                              !FarmPointerInput.IsOverUi(pointerId);
        bool cancelPressed  = FarmPointerInput.WasCancelPressed();

        if (confirmPressed || cancelPressed)
            biofiltreManager.SuppressFarmPointerUiUntilPointerRelease();

        if (confirmPressed)
        {
            if (currentlyValid)
                ConfirmPlacement();
            else
                Cancel();
        }
        else if (cancelPressed)
        {
            Cancel();
        }
    }

    // ── Ghost management ──────────────────────────────────────────────────────

    private void SpawnGhost()
    {
        if (plantPrefab == null)
            return;

        ghostInstance = Instantiate(plantPrefab);
        ghostInstance.name = $"Ghost_{plantDefinition.displayName}";

        // PlantGrow.Awake a déjà tourné pendant Instantiate (peut activer InsectPath).
        // On coupe croissance + insectes pour le fantôme de pose uniquement.
        if (ghostInstance.TryGetComponent(out PlantGrow grow))
            grow.enabled = false;

        HideGhostInsects(ghostInstance);

        // Directly assign the final-stage sprite so the ghost always shows the mature plant.
        ghostRenderer = ghostInstance.GetComponent<SpriteRenderer>();
        if (ghostRenderer != null)
            ghostRenderer.sprite = plantDefinition.spriteSeedling;

        foreach (SpriteRenderer sr in ghostInstance.GetComponentsInChildren<SpriteRenderer>(true))
            sr.sortingOrder = GhostSortingOrder;

        // Disable all colliders so the ghost does not interfere with raycasts
        foreach (Collider2D col in ghostInstance.GetComponentsInChildren<Collider2D>(true))
            col.enabled = false;
    }

    private static void HideGhostInsects(GameObject ghostRoot)
    {
        foreach (InsectPathAnchor path in ghostRoot.GetComponentsInChildren<InsectPathAnchor>(true))
            path.SetPathActive(false);

        foreach (InsectPathFollower follower in ghostRoot.GetComponentsInChildren<InsectPathFollower>(true))
            follower.enabled = false;
    }

    private void UpdateGhostPosition()
    {
        if (!FarmPointerInput.TryGetScreenPosition(out Vector2 screenPosition))
            return;

        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(screenPosition);

        // Use the footprint's geometric center (in world units) for mouse-to-cell tracking.
        // spriteWorldOffset is a purely visual offset for the sprite pivot and must NOT be used
        // here — it can exceed one cell in magnitude and break the floor-based WorldToGrid calculation.
        Vector2 footprintCenter = ComputeFootprintCenterWorldOffset();
        Vector2Int hoveredCell = gridManager.WorldToGrid(mouseWorld - footprintCenter);

        if (hoveredCell != currentCell)
        {
            currentCell    = hoveredCell;
            currentlyValid = biofiltreManager.CanPlace(currentCell, plantDefinition);
            RefreshFootprintPreview();
        }

        // Sprite sits on the footprint geometric center; spriteWorldOffset is a pivot tweak only.
        Vector2 snapPos = gridManager.GetFootprintWorldCenter(currentCell, plantDefinition.footprint)
                          + plantDefinition.spriteWorldOffset;
        ghostInstance.transform.position = new Vector3(snapPos.x, snapPos.y, 0f);

        ApplyTint(currentlyValid ? ColorValid : ColorInvalid);
    }

    private void RefreshFootprintPreview()
    {
        ClearFootprintPreview();
        if (plantDefinition == null || visualizer == null)
            return;

        foreach (Vector2Int cell in plantDefinition.GetOccupiedCells(currentCell))
        {
            BiofiltreCell bioCell = visualizer.GetCell(cell);
            if (bioCell == null)
                continue;

            bioCell.SetPlacementPreview(currentlyValid);
            previewedCells.Add(cell);
        }
    }

    private void ClearFootprintPreview()
    {
        for (int i = 0; i < previewedCells.Count; i++)
        {
            Vector2Int cell = previewedCells[i];
            BiofiltreCell bioCell = visualizer != null ? visualizer.GetCell(cell) : null;
            if (bioCell != null && gridManager != null)
                bioCell.SetVisualState(!gridManager.IsCellFree(cell));
        }

        previewedCells.Clear();
    }

    /// <summary>
    /// Returns the world-space offset from the anchor cell center to the geometric center
    /// of the footprint. Used to keep the footprint centered under the mouse cursor.
    /// </summary>
    private Vector2 ComputeFootprintCenterWorldOffset()
    {
        return gridManager.GetFootprintWorldCenter(Vector2Int.zero, plantDefinition.footprint)
               - gridManager.GridToWorldCenter(Vector2Int.zero);
    }

    private void ApplyTint(Color color)
    {
        foreach (SpriteRenderer sr in ghostInstance.GetComponentsInChildren<SpriteRenderer>())
            sr.color = color;
    }

    // ── Placement / cancellation ──────────────────────────────────────────────

    private void ConfirmPlacement()
    {
        if (!biofiltreManager.TryPlantSeedAt(currentCell, plantDefinition, plantPrefab, seedItem))
        {
            Cancel();
            return;
        }

        PlayerInventory inventory = PlayerInventory.Instance;
        bool noSeedsLeft = seedItem == null ||
                           inventory == null ||
                           inventory.Count(seedItem) <= 0;

        // Tant qu'il reste des graines : garder la preview active pour enchaîner
        // les plantations (le ghost continue de suivre la souris).
        if (!noSeedsLeft)
        {
            currentlyValid = biofiltreManager.CanPlace(currentCell, plantDefinition);
            RefreshFootprintPreview();
            return;
        }

        // Dernière graine consommée : fermer la preview et ré-ouvrir la sélection
        // de graines en état vide ("plus de graines"), que le joueur fermera lui-même.
        // Cancel() remet biofiltreManager à null (Cleanup) : on capture la référence avant.
        BiofiltreCell cellForReopen = originCell;
        BiofiltreManager managerForReopen = biofiltreManager;
        Cancel();
        managerForReopen.ReopenSeedSelectionAfterLastSeedPlanted(cellForReopen);
    }

    /// <summary>Cancels the preview and destroys the ghost without placing anything.</summary>
    public void Cancel()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        ClearFootprintPreview();

        if (ghostInstance != null)
            Destroy(ghostInstance);

        ghostInstance    = null;
        ghostRenderer    = null;
        plantDefinition  = null;
        plantPrefab      = null;
        seedItem         = null;
        gridManager      = null;
        visualizer       = null;
        biofiltreManager = null;
        originCell       = null;
        enabled          = false;
    }
}
