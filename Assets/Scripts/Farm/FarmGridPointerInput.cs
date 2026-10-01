using UnityEngine;

/// <summary>
/// Clic grille : écran → monde → (col, row) via <see cref="GridManager"/>.
/// PC : clic gauche au press. Tactile : tap au relâchement, sauf pose et récolte batch (glisser).
/// </summary>
[RequireComponent(typeof(GridManager))]
[RequireComponent(typeof(BiofiltreGridVisualizer))]
public class FarmGridPointerInput : MonoBehaviour
{
    [Tooltip("Caméra de la ferme. Vide = Camera.main.")]
    [SerializeField] private Camera worldCamera;

    private GridManager gridManager;
    private BiofiltreGridVisualizer visualizer;
    private BiofiltreManager biofiltreManager;
    private FarmBatchHarvestInput batchHarvest;

    private void Awake()
    {
        gridManager      = GetComponent<GridManager>();
        visualizer       = GetComponent<BiofiltreGridVisualizer>();
        biofiltreManager = GetComponent<BiofiltreManager>();
        batchHarvest     = GetComponent<FarmBatchHarvestInput>();
        if (batchHarvest == null)
            batchHarvest = gameObject.AddComponent<FarmBatchHarvestInput>();

        if (worldCamera == null)
            worldCamera = Camera.main;

        batchHarvest.Initialise(gridManager, biofiltreManager, worldCamera);
    }

    private void Update()
    {
        bool strokeActive = batchHarvest != null && batchHarvest.IsStrokeActive;
        if (biofiltreManager != null && biofiltreManager.ShouldSuppressFarmPointerUi && !strokeActive)
            return;

        if (batchHarvest != null && batchHarvest.Tick())
            return;

        if (FarmPointerInput.IsMouseDriven())
        {
            if (!FarmPointerInput.TryGetPrimaryPress(out Vector2 screenPosition, out int pointerId))
                return;

            if (FarmPointerInput.IsOverUi(pointerId))
                return;

            TryNotifyCellAtScreen(screenPosition);
            return;
        }

        // Tap au relâchement avant Tick : sinon TryGetLongPressPan reset l'état (doigt déjà levé).
        TryHandleTouchTapRelease();
        FarmCameraInput.TickLongPressPanTracking();
    }

    private void TryHandleTouchTapRelease()
    {
        if (!FarmCameraInput.TryGetPrimaryTapOnRelease(out Vector2 screenPosition, out int pointerId))
            return;

        if (FarmPointerInput.IsOverUi(pointerId))
            return;

        TryNotifyCellAtScreen(screenPosition);
    }

    private void TryNotifyCellAtScreen(Vector2 screenPosition)
    {
        if (TryResolveCell(screenPosition, out Vector2Int coords))
            visualizer.NotifyCellClicked(coords);
    }

    /// <summary>Screen → monde → cellule (sprite / footprint iso, puis sol).</summary>
    private bool TryResolveCell(Vector2 screenPosition, out Vector2Int coords)
    {
        coords = default;

        Camera camera = ResolveCamera();
        if (camera == null)
            return false;

        Vector2 world = ScreenToGameplayPlane(camera, screenPosition);
        return gridManager.TryResolveClickTarget(world, out coords);
    }

    private Camera ResolveCamera()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (worldCamera == null)
            Debug.LogWarning("[FarmGridPointerInput] Aucune caméra — clics grille ignorés.", this);

        return worldCamera;
    }

    private static Vector2 ScreenToGameplayPlane(Camera camera, Vector2 screenPosition)
    {
        float depth = Mathf.Abs(camera.transform.position.z);
        Vector3 world = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
        return new Vector2(world.x, world.y);
    }
}
