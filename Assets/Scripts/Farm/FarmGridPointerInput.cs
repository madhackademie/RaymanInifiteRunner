using UnityEngine;

/// <summary>
/// Résout les clics sur la grille du biofiltre par calcul, sans collider ni raycast physique.
/// La position écran est convertie en monde puis en cellule via <see cref="GridManager"/>.
/// </summary>
/// <remarks>
/// Remplace l'ancien chemin BoxCollider2D + Physics2DRaycaster + IPointerClickHandler :
/// une grille 10×10 créait 100 colliders pour un simple test d'appartenance à un rectangle.
/// </remarks>
[RequireComponent(typeof(GridManager))]
[RequireComponent(typeof(BiofiltreGridVisualizer))]
public class FarmGridPointerInput : MonoBehaviour
{
    [Tooltip("Caméra de rendu de la ferme. Vide = Camera.main résolue au démarrage.")]
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

        // Clic déjà consommé par la pose ou par un fantôme actif.
        if (biofiltreManager != null && biofiltreManager.ShouldSuppressFarmPointerUi)
            return;

        if (TryResolveCell(screenPosition, out Vector2Int coords))
            visualizer.NotifyCellClicked(coords);
    }

    /// <summary>Convertit une position écran en coordonnées de cellule dans les limites de la grille.</summary>
    private bool TryResolveCell(Vector2 screenPosition, out Vector2Int coords)
    {
        coords = default;

        Camera camera = ResolveCamera();
        if (camera == null)
            return false;

        Vector2 worldPosition = camera.ScreenToWorldPoint(screenPosition);
        coords = gridManager.WorldToGrid(worldPosition);
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
