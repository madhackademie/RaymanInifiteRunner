using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle : curseur caché. Armed : gant en boucle, on attend le clic.
/// Stroking : animation de récolte tant que le clic est tenu.
/// </summary>
public enum FarmBatchHarvestMode
{
    Idle,
    Armed,
    Stroking
}

/// <summary>
/// Récolte en balayage souris ou doigt. Le mode s'éteint au relâchement.
/// Une plante n'est prise qu'une fois, même si le curseur traverse son footprint.
/// </summary>
public class FarmBatchHarvestInput : MonoBehaviour
{
    private GridManager gridManager;
    private BiofiltreManager biofiltreManager;
    private Camera worldCamera;
    private FarmBatchHarvestMode mode = FarmBatchHarvestMode.Idle;
    private bool hasVisited;
    private Vector2Int visited;
    private readonly HashSet<int> seenPlantIds = new();

    public FarmBatchHarvestMode Mode => mode;

    /// <summary>La vue curseur s'abonne ici. Pas de lecture dans Update.</summary>
    public event Action<FarmBatchHarvestMode> ModeChanged;

    public bool IsStrokeActive => mode == FarmBatchHarvestMode.Stroking;

    public void Initialise(GridManager grid, BiofiltreManager manager, Camera camera)
    {
        gridManager = grid;
        biofiltreManager = manager;
        worldCamera = camera != null ? camera : Camera.main;
    }

    /// <summary>Bouton popup : attend le prochain clic grille, sans couper au relâchement du bouton.</summary>
    public void Arm()
    {
        hasVisited = false;
        seenPlantIds.Clear();
        SetMode(FarmBatchHarvestMode.Armed);
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
        seenPlantIds.Clear();
        hasVisited = false;
        SetMode(FarmBatchHarvestMode.Stroking);
    }

    /// <returns>Vrai si ce frame ne doit pas ouvrir un popup grille.</returns>
    public bool Tick()
    {
        if (mode == FarmBatchHarvestMode.Idle)
            return false;

        if (biofiltreManager != null && biofiltreManager.IsPlantPlacementPreviewActive)
        {
            SetMode(FarmBatchHarvestMode.Idle);
            return false;
        }

        if (FarmCameraInput.IsPinchGestureActive())
            return true;

        if (!FarmPointerInput.TryReadPrimaryStroke(
                out bool pressed,
                out bool held,
                out bool released,
                out bool overUi))
            return false;

        if (mode == FarmBatchHarvestMode.Armed)
        {
            if (!pressed || overUi)
                return false;

            EnterStroke();
        }

        if (mode != FarmBatchHarvestMode.Stroking)
            return false;

        if (!overUi && (held || released) && TryCellUnderPointer(out Vector2Int cell))
            HarvestPathTo(cell);

        if (mode == FarmBatchHarvestMode.Stroking && released)
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
            if (mode != FarmBatchHarvestMode.Stroking)
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
        hasVisited = false;
        seenPlantIds.Clear();
        SetMode(FarmBatchHarvestMode.Idle);
        biofiltreManager?.SuppressFarmPointerUiUntilPointerRelease();
    }

    private void SetMode(FarmBatchHarvestMode next)
    {
        if (mode == next)
            return;

        mode = next;
        FarmCameraInput.SetHarvestBlocksPan(next != FarmBatchHarvestMode.Idle);
        ModeChanged?.Invoke(mode);
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
