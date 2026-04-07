using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Spawns a ghost preview of a plant that follows the mouse snapped to the grid.
/// Tints green when the footprint is valid, red when invalid.
/// Left-click confirms placement; right-click or Escape cancels.
/// </summary>
public class PlantPlacementPreview : MonoBehaviour
{
    private static readonly Color ColorValid   = new Color(0.3f, 1f, 0.4f, 0.6f);
    private static readonly Color ColorInvalid = new Color(1f, 0.2f, 0.2f, 0.6f);

    private GridManager      gridManager;
    private BiofiltreManager biofiltreManager;
    private PlantDefinition  plantDefinition;
    private GameObject       plantPrefab;
    private BiofiltreCell    originCell;

    private GameObject       ghostInstance;
    private SpriteRenderer   ghostRenderer;

    private Vector2Int       currentCell;
    private bool             currentlyValid;
    private Camera           mainCamera;

    // ── Initialisation ────────────────────────────────────────────────────────

    /// <summary>
    /// Initialises and activates the preview mode.
    /// Called by <see cref="SeedSelectionUI"/> after the player selects a seed.
    /// </summary>
    public void Begin(
        PlantDefinition  definition,
        GameObject       prefab,
        BiofiltreCell    origin,
        GridManager      grid,
        BiofiltreManager manager)
    {
        plantDefinition  = definition;
        plantPrefab      = prefab;
        originCell       = origin;
        gridManager      = grid;
        biofiltreManager = manager;
        mainCamera       = Camera.main;

        SpawnGhost();
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

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentlyValid)
                ConfirmPlacement();
            else
                Cancel(); // left-click on invalid red position = deselect
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
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

        // Disable PlantGrow entirely: its Start() would reset the stage to Graine on the next frame,
        // overriding any SetStage call made during instantiation.
        if (ghostInstance.TryGetComponent(out PlantGrow grow))
            grow.enabled = false;

        // Directly assign the final-stage sprite so the ghost always shows the mature plant.
        ghostRenderer = ghostInstance.GetComponent<SpriteRenderer>();
        if (ghostRenderer != null)
            ghostRenderer.sprite = plantDefinition.spriteSeedling;

        // Disable all colliders so the ghost does not interfere with raycasts
        foreach (Collider2D col in ghostInstance.GetComponentsInChildren<Collider2D>())
            col.enabled = false;
    }

    private void UpdateGhostPosition()
    {
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Use the footprint's geometric center (in world units) for mouse-to-cell tracking.
        // spriteWorldOffset is a purely visual offset for the sprite pivot and must NOT be used
        // here — it can exceed one cell in magnitude and break the floor-based WorldToGrid calculation.
        Vector2 footprintCenter = ComputeFootprintCenterWorldOffset();
        Vector2Int hoveredCell = gridManager.WorldToGrid(mouseWorld - footprintCenter);

        if (hoveredCell != currentCell)
        {
            currentCell    = hoveredCell;
            currentlyValid = biofiltreManager.CanPlace(currentCell, plantDefinition);
        }

        // Place the ghost transform at the sprite pivot position (anchor center + visual offset)
        Vector2 snapPos = gridManager.GridToWorldCenter(currentCell) + plantDefinition.spriteWorldOffset;
        ghostInstance.transform.position = new Vector3(snapPos.x, snapPos.y, 0f);

        ApplyTint(currentlyValid ? ColorValid : ColorInvalid);
    }

    /// <summary>
    /// Returns the world-space offset from the anchor cell center to the geometric center
    /// of the footprint. Used to keep the footprint centered under the mouse cursor.
    /// </summary>
    private Vector2 ComputeFootprintCenterWorldOffset()
    {
        Vector2Int[] footprint = plantDefinition.footprint;
        Vector2 cellSize = gridManager.CellSizeWorld;

        float sumCol = 0f;
        float sumRow = 0f;
        foreach (Vector2Int cell in footprint)
        {
            sumCol += cell.x;
            sumRow += cell.y;
        }

        float centerCol = sumCol / footprint.Length;
        float centerRow = sumRow / footprint.Length;

        // Convert grid-cell offset to world-space offset (rows increase downward → negate Y)
        return new Vector2(centerCol * cellSize.x, -centerRow * cellSize.y);
    }

    private void ApplyTint(Color color)
    {
        foreach (SpriteRenderer sr in ghostInstance.GetComponentsInChildren<SpriteRenderer>())
            sr.color = color;
    }

    // ── Placement / cancellation ──────────────────────────────────────────────

    private void ConfirmPlacement()
    {
        biofiltreManager.PlantSeedAt(currentCell, plantDefinition, plantPrefab);
        // Do NOT cleanup — stay active so the player can chain placements immediately.
        // The ghost is reused as-is; UpdateGhostPosition will recompute validity next frame.
    }

    /// <summary>Cancels the preview and destroys the ghost without placing anything.</summary>
    public void Cancel()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (ghostInstance != null)
            Destroy(ghostInstance);

        ghostInstance    = null;
        ghostRenderer    = null;
        plantDefinition  = null;
        plantPrefab      = null;
        gridManager      = null;
        biofiltreManager = null;
        originCell       = null;
        enabled          = false;
    }
}
