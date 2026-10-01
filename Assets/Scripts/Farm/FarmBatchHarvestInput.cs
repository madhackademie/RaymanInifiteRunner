using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Récolte en balayage souris. Le mode s'éteint au relâchement.
/// Une plante n'est prise qu'une fois, même si le curseur traverse son footprint.
/// </summary>
public class FarmBatchHarvestInput : MonoBehaviour
{
    private enum StrokeMode
    {
        Idle,
        Armed,
        Stroking
    }

    private GridManager gridManager;
    private BiofiltreManager biofiltreManager;
    private Camera worldCamera;
    private StrokeMode mode = StrokeMode.Idle;
    private bool hasVisited;
    private Vector2Int visited;
    private readonly HashSet<int> seenPlantIds = new();

    public bool IsStrokeActive => mode == StrokeMode.Stroking;

    public void Initialise(GridManager grid, BiofiltreManager manager, Camera camera)
    {
        gridManager = grid;
        biofiltreManager = manager;
        worldCamera = camera != null ? camera : Camera.main;
    }

    /// <summary>Bouton popup : attend le prochain clic grille, sans couper au relâchement du bouton.</summary>
    public void Arm()
    {
        mode = StrokeMode.Armed;
        hasVisited = false;
        seenPlantIds.Clear();
    }

    /// <summary>Double-clic : la plante courante est déjà récoltée, le glisser continue.</summary>
    public void BeginStroke(Vector2Int fromCell)
    {
        EnterStroke();
        hasVisited = true;
        visited = fromCell;
    }

    private void EnterStroke()
    {
        mode = StrokeMode.Stroking;
        seenPlantIds.Clear();
        hasVisited = false;
    }

    /// <returns>Vrai si ce frame ne doit pas ouvrir un popup grille.</returns>
    public bool TickMouse()
    {
        if (mode == StrokeMode.Idle || !FarmPointerInput.IsMouseDriven())
            return false;

        if (biofiltreManager != null && biofiltreManager.IsPlantPlacementPreviewActive)
        {
            mode = StrokeMode.Idle;
            return false;
        }

        Mouse mouse = Mouse.current;
        if (mouse == null)
            return false;

        bool overUi = FarmPointerInput.IsOverUi(FarmPointerInput.MousePointerId);
        if (mode == StrokeMode.Armed)
        {
            if (!mouse.leftButton.wasPressedThisFrame || overUi)
                return false;

            EnterStroke();
        }

        if (mode != StrokeMode.Stroking)
            return false;

        bool released = mouse.leftButton.wasReleasedThisFrame;
        if (!overUi && (mouse.leftButton.isPressed || released) && TryCellUnderPointer(out Vector2Int cell))
            HarvestPathTo(cell);

        if (mode == StrokeMode.Stroking && released)
            EndStroke();

        return true;
    }

    private void HarvestPathTo(Vector2Int target)
    {
        if (!hasVisited || visited == target)
        {
            HarvestCell(target);
            return;
        }

        Vector2Int from = visited;
        int steps = Mathf.Max(Mathf.Abs(target.x - from.x), Mathf.Abs(target.y - from.y));
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(from.x, target.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(from.y, target.y, t));
            HarvestCell(new Vector2Int(x, y));
            if (mode != StrokeMode.Stroking)
                return;
        }
    }

    private void HarvestCell(Vector2Int cell)
    {
        hasVisited = true;
        visited = cell;

        GameObject plant = gridManager != null ? gridManager.GetPlantAt(cell) : null;
        if (plant == null || !seenPlantIds.Add(plant.GetInstanceID()))
            return;

        if (!plant.TryGetComponent(out PlantHarvestInteractor interactor) || !interactor.CanHarvestNow())
            return;

        if (!interactor.ConfirmHarvest())
            EndStroke();
    }

    private void EndStroke()
    {
        mode = StrokeMode.Idle;
        hasVisited = false;
        seenPlantIds.Clear();
        biofiltreManager?.SuppressFarmPointerUiUntilPointerRelease();
    }

    private bool TryCellUnderPointer(out Vector2Int cell)
    {
        cell = default;
        if (gridManager == null || !FarmPointerInput.TryGetScreenPosition(out Vector2 screen))
            return false;

        Camera camera = worldCamera != null ? worldCamera : Camera.main;
        if (camera == null)
            return false;

        float depth = Mathf.Abs(camera.transform.position.z);
        Vector3 world = camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, depth));
        return gridManager.TryResolveClickTarget(world, out cell);
    }
}
