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
/// Cible grille : <see cref="GridManager.TryResolveClickTarget"/> (losanges footprint iso), pas raycast Unity UI.
/// </summary>
public class FarmBatchHarvestInput : MonoBehaviour
{
    private GridManager gridManager;
    private BiofiltreGridVisualizer gridVisualizer;
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

    public void Initialise(
        GridManager grid,
        BiofiltreManager manager,
        BiofiltreGridVisualizer visualizer,
        Camera camera)
    {
        gridManager = grid;
        biofiltreManager = manager;
        gridVisualizer = visualizer;
        worldCamera = camera != null ? camera : Camera.main;
    }

    private void Update()
    {
        if (mode == FarmBatchHarvestMode.Idle)
        {
            ClearHarvestHover();
            return;
        }

        if (FarmPointerInput.IsOverUi(GetPointerIdForHover()))
        {
            ClearHarvestHover();
            return;
        }

        RefreshHarvestHover();
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

        UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
        bool mouseDriven = mouse != null && FarmPointerInput.IsMouseDriven();

        if (mode == FarmBatchHarvestMode.Armed && mouseDriven && mouse.rightButton.wasPressedThisFrame)
        {
            EndStroke();
            return true;
        }

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

            if (!TryResolveHarvestCellForAction(out _))
                return true;

            EnterStroke();
        }

        if (mode != FarmBatchHarvestMode.Stroking)
            return false;

        if (!overUi && (held || released) && TryResolveHarvestCellForAction(out Vector2Int cell))
            HarvestPathTo(cell);

        if (mode == FarmBatchHarvestMode.Stroking && released)
            EndStroke();

        return true;
    }

    /// <summary>
    /// Position du gant : cellule résolue (centre / plante) ou pointeur brut — pas d'aimant auto.
    /// </summary>
    public bool TryGetCursorWorld(out Vector3 worldPosition)
    {
        worldPosition = default;
        if (!TryGetPointerWorld(out Vector2 pointerWorld))
            return false;

        if (TryResolveHoverCell(out Vector2Int cell) && TryGetSnapWorldForCell(cell, out worldPosition))
            return true;

        worldPosition = new Vector3(pointerWorld.x, pointerWorld.y, 0f);
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

    /// <summary>Quitte le mode récolte groupée (Armed/Stroking) vers Idle, ex. avant de quitter la scène.</summary>
    public void CancelBatchHarvest()
    {
        EndStroke();
    }

    private void EndStroke()
    {
        hasVisited = false;
        seenPlantIds.Clear();
        ClearHarvestHover();
        SetMode(FarmBatchHarvestMode.Idle);
        biofiltreManager?.SuppressFarmPointerUiUntilPointerRelease();
    }

    private void SetMode(FarmBatchHarvestMode next)
    {
        if (mode == next)
            return;

        if (next == FarmBatchHarvestMode.Idle)
            ClearHarvestHover();

        mode = next;
        FarmCameraInput.SetHarvestBlocksPan(next != FarmBatchHarvestMode.Idle);
        ModeChanged?.Invoke(mode);
    }

    private void RefreshHarvestHover()
    {
        if (gridVisualizer == null || gridManager == null)
            return;

        if (!TryResolveHoverCell(out Vector2Int cell))
        {
            ClearHarvestHover();
            return;
        }

        gridVisualizer.SetHarvestHoverCell(cell, IsCellHarvestable(cell));
    }

    private void ClearHarvestHover()
    {
        gridVisualizer?.ClearHarvestHoverCell();
    }

    private bool IsCellHarvestable(Vector2Int cell)
    {
        GameObject plant = gridManager.GetPlantAt(cell);
        return plant != null &&
               plant.TryGetComponent(out PlantHarvestInteractor interactor) &&
               interactor.CanHarvestNow();
    }

    /// <summary>Hover / curseur : hit strict (losange footprint ou sol sous le pointeur).</summary>
    private bool TryResolveHoverCell(out Vector2Int cell)
    {
        cell = default;
        if (gridManager == null || !TryGetPointerWorld(out Vector2 world))
            return false;

        return gridManager.TryResolveClickTarget(world, out cell);
    }

    /// <summary>Clic / balayage : hit strict puis clamp bordure pour ne pas quitter le mode.</summary>
    private bool TryResolveHarvestCellForAction(out Vector2Int cell)
    {
        cell = default;
        if (gridManager == null || !TryGetPointerWorld(out Vector2 world))
            return false;

        if (gridManager.TryResolveClickTarget(world, out cell))
            return true;

        cell = gridManager.ClampWorldToBoundsCell(world);
        return gridManager.IsInBounds(cell);
    }

    private bool TryGetSnapWorldForCell(Vector2Int cell, out Vector3 worldPosition)
    {
        worldPosition = default;
        if (gridManager == null)
            return false;

        GameObject plant = gridManager.GetPlantAt(cell);
        if (plant != null)
        {
            Vector3 p = plant.transform.position;
            worldPosition = new Vector3(p.x, p.y, 0f);
            return true;
        }

        Vector2 center = gridManager.GridToWorldCenter(cell);
        worldPosition = new Vector3(center.x, center.y, 0f);
        return true;
    }

    private bool TryGetPointerWorld(out Vector2 world)
    {
        world = default;
        if (!FarmPointerInput.TryGetScreenPosition(out Vector2 screen))
            return false;

        Camera camera = worldCamera != null ? worldCamera : Camera.main;
        if (camera == null)
            return false;

        float depth = Mathf.Abs(camera.transform.position.z);
        Vector3 world3 = camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, depth));
        world = new Vector2(world3.x, world3.y);
        return true;
    }

    private static int GetPointerIdForHover()
    {
        UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null && FarmPointerInput.IsMouseDriven())
            return -1;

        return UnityEngine.InputSystem.Touchscreen.current?.primaryTouch.touchId.ReadValue() ?? -1;
    }
}
