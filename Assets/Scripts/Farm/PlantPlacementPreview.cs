using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fantôme de pose aimanté à la grille. Vert = valide, rouge = invalide.
/// Souris et doigt : glisser pour peindre (pas = footprint), relâchement = fin.
/// Clic droit ou Échap = annulation. Le pinch reste le zoom.
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
    private bool             strokeActive;
    private bool             hasPaintedAnchor;
    private Vector2Int       paintedAnchor;
    private bool             hasVisitedAnchor;
    private Vector2Int       visitedAnchor;

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
        FarmCameraInput.SetPlacementBlocksPan(true);
        TickPaintStroke();
    }

    private void OnDisable()
    {
        FarmCameraInput.SetPlacementBlocksPan(false);
    }

    private void TickPaintStroke()
    {
        if (biofiltreManager == null)
            return;

        if (FarmCameraInput.IsPinchGestureActive())
            return;

        if (!FarmPointerInput.TryReadPrimaryStroke(
                out bool pressed,
                out bool held,
                out bool released,
                out bool overUi))
            return;

        if (pressed && !overUi)
            strokeActive = true;

        if (FarmPointerInput.WasCancelPressed())
        {
            EndPaintStroke();
            return;
        }

        if (strokeActive && !overUi && (held || released))
            PaintPathToCurrent();

        if (strokeActive && released)
            EndPaintStroke();
    }

    /// <summary>Comble les cases sautées si la souris va plus vite qu'une case par frame.</summary>
    private void PaintPathToCurrent()
    {
        if (!hasVisitedAnchor || visitedAnchor == currentCell)
        {
            VisitAndMaybePlant(currentCell);
            return;
        }

        Vector2Int from = visitedAnchor;
        Vector2Int to = currentCell;
        int steps = Mathf.Max(Mathf.Abs(to.x - from.x), Mathf.Abs(to.y - from.y));
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(from.x, to.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(from.y, to.y, t));
            VisitAndMaybePlant(SnapAnchorToFootprintStride(new Vector2Int(x, y)));
            if (ghostInstance == null || biofiltreManager == null)
                return;
        }
    }

    private void VisitAndMaybePlant(Vector2Int cell)
    {
        bool alreadyPlantedHere = hasPaintedAnchor && paintedAnchor == cell;
        if (hasVisitedAnchor && visitedAnchor == cell && alreadyPlantedHere)
            return;

        currentCell = cell;
        currentlyValid = biofiltreManager.CanPlace(currentCell, plantDefinition);
        RefreshFootprintPreview();
        hasVisitedAnchor = true;
        visitedAnchor = cell;
        TryPaintSnappedCell();
    }

    private void TryPaintSnappedCell()
    {
        if (!currentlyValid)
            return;

        if (hasPaintedAnchor && paintedAnchor == currentCell)
            return;

        Vector2Int cell = currentCell;
        ConfirmPlacement();
        if (ghostInstance == null)
            return;

        hasPaintedAnchor = true;
        paintedAnchor = cell;
    }

    private void EndPaintStroke()
    {
        if (biofiltreManager != null)
            biofiltreManager.SuppressFarmPointerUiUntilPointerRelease();

        if (enabled)
            Cancel();
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
        HideGhostSelectionGlow(ghostInstance);

        // Mature = meilleure lecture du footprint 2×2 iso (seedling trop haut / déborde latéralement).
        ghostRenderer = ghostInstance.GetComponent<SpriteRenderer>();
        if (ghostRenderer != null)
        {
            Sprite previewSprite = plantDefinition.spriteMature ?? plantDefinition.spriteGrowing;
            ghostRenderer.sprite = previewSprite ?? plantDefinition.spriteSeedling;
        }

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

    private static void HideGhostSelectionGlow(GameObject ghostRoot)
    {
        if (ghostRoot.TryGetComponent(out PlantSelectionHighlight highlight))
            highlight.enabled = false;

        Transform glow = ghostRoot.transform.Find("SelectionGlow");
        if (glow != null)
            glow.gameObject.SetActive(false);
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
        Vector2Int hoveredCell = SnapAnchorToFootprintStride(
            gridManager.WorldToGrid(mouseWorld - footprintCenter));

        if (hoveredCell != currentCell)
        {
            currentCell    = hoveredCell;
            currentlyValid = biofiltreManager.CanPlace(currentCell, plantDefinition);
            RefreshFootprintPreview();
        }

        // Hub iso 2×2 (sommet central) ou centre footprint + triche vue.
        Vector2 snapPos = gridManager.GetPlantSpriteWorldPosition(currentCell, plantDefinition);
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
            if (bioCell == null)
                continue;

            bioCell.ClearTransientHighlight();
            if (gridManager != null)
                bioCell.SetVisualState(!gridManager.IsCellFree(cell));
        }

        previewedCells.Clear();
    }

    /// <summary>
    /// Cale l'ancre sur un pas égal au footprint (roquette 1, laitue 2).
    /// Les blocs voisins se touchent sans décalage d'une case.
    /// </summary>
    private Vector2Int SnapAnchorToFootprintStride(Vector2Int cell)
    {
        Vector2Int[] footprint = plantDefinition != null ? plantDefinition.footprint : null;
        if (footprint == null || footprint.Length == 0)
            return cell;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        for (int i = 0; i < footprint.Length; i++)
        {
            Vector2Int offset = footprint[i];
            if (offset.x < minX) minX = offset.x;
            if (offset.y < minY) minY = offset.y;
            if (offset.x > maxX) maxX = offset.x;
            if (offset.y > maxY) maxY = offset.y;
        }

        int strideX = Mathf.Max(1, maxX - minX + 1);
        int strideY = Mathf.Max(1, maxY - minY + 1);
        int anchorX = minX + FloorToStride(cell.x - minX, strideX) * strideX;
        int anchorY = minY + FloorToStride(cell.y - minY, strideY) * strideY;
        return new Vector2Int(anchorX, anchorY);
    }

    private static int FloorToStride(int value, int stride)
    {
        int quotient = value / stride;
        if (value < 0 && value % stride != 0)
            quotient--;

        return quotient;
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
        strokeActive      = false;
        hasPaintedAnchor  = false;
        hasVisitedAnchor  = false;
        enabled           = false;
    }
}
