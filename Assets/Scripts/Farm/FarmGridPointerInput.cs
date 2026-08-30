using UnityEngine;

/// <summary>
/// Clic grille oldschool : écran → monde → (col, row) via <see cref="GridManager"/>.
/// Aucun collider, aucun raycast physique — modèle CodeMonkey.
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

    private void Awake()
    {
        gridManager      = GetComponent<GridManager>();
        visualizer       = GetComponent<BiofiltreGridVisualizer>();
        biofiltreManager = GetComponent<BiofiltreManager>();

        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    private void Update()
    {
        if (!FarmPointerInput.TryGetPrimaryPress(out Vector2 screenPosition, out int pointerId))
            return;

        if (FarmPointerInput.IsOverUi(pointerId))
            return;

        if (biofiltreManager != null && biofiltreManager.ShouldSuppressFarmPointerUi)
            return;

        if (TryResolveCell(screenPosition, out Vector2Int coords))
            visualizer.NotifyCellClicked(coords);
    }

    /// <summary>Screen → world → cellule (délègue au mapper orthogonal ou iso du GridManager).</summary>
    private bool TryResolveCell(Vector2 screenPosition, out Vector2Int coords)
    {
        coords = default;

        Camera camera = ResolveCamera();
        if (camera == null)
            return false;

        Vector2 world = camera.ScreenToWorldPoint(screenPosition);
        return gridManager.TryWorldToCell(world, out coords);
    }

    private Camera ResolveCamera()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (worldCamera == null)
            Debug.LogWarning("[FarmGridPointerInput] Aucune caméra — clics grille ignorés.", this);

        return worldCamera;
    }
}
