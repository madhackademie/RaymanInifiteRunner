using UnityEngine;

/// <summary>
/// Résout les clics grille par calcul : écran → monde → colonne/ligne.
/// Pas de collider ni de raycast physique (style grille CodeMonkey).
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

    private bool TryResolveCell(Vector2 screenPosition, out Vector2Int coords)
    {
        coords = default;

        Camera camera = ResolveCamera();
        if (camera == null)
            return false;

        Vector3 world = camera.ScreenToWorldPoint(screenPosition);
        coords = gridManager.WorldToGrid(world);
        return gridManager.IsInBounds(coords);
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
