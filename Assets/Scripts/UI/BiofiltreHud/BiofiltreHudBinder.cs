using UnityEngine;

/// <summary>
/// Instancie et positionne le HUD world biofiltre depuis l’AABB grille de <see cref="GridManager"/>.
/// Offsets normalisés (0–1, origine bas-gauche) + extra monde — overridables par instance.
/// </summary>
[RequireComponent(typeof(GridManager))]
public class BiofiltreHudBinder : MonoBehaviour
{
    [SerializeField] private BiofiltreHudView hudPrefab;
    [SerializeField] private GridManager gridManager;

    [Header("Primary row (mockup IBC — override per instance)")]
    [SerializeField] private Vector2 primaryNormalizedAnchor = new Vector2(0.08f, 0.92f);
    [SerializeField] private Vector2 primaryWorldOffset;

    [Header("Star row (pivot droite recommandé sur le prefab)")]
    [SerializeField] private Vector2 starNormalizedAnchor = new Vector2(0.92f, 0.92f);
    [SerializeField] private Vector2 starWorldOffset;

    [Header("Secondary row")]
    [SerializeField] private Vector2 secondaryNormalizedAnchor = new Vector2(0.18f, 0.22f);
    [SerializeField] private Vector2 secondaryWorldOffset;

    [SerializeField] private float hudWorldZ = 0f;

    private BiofiltreHudView hudInstance;

    private void Awake()
    {
        if (gridManager == null)
            gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        InstantiateHudIfNeeded();
        RecalculateHudPositions();
    }

    /// <summary>Recale les widgets si la layout grille change (IBC vs futur bac).</summary>
    public void RecalculateHudPositions()
    {
        if (hudInstance == null || gridManager == null)
            return;

        Rect worldRect = gridManager.GetWorldRect();

        PositionRow(hudInstance.PrimaryRow, primaryNormalizedAnchor, primaryWorldOffset, worldRect);
        PositionRow(hudInstance.StarRow, starNormalizedAnchor, starWorldOffset, worldRect);
        PositionRow(hudInstance.SecondaryRow, secondaryNormalizedAnchor, secondaryWorldOffset, worldRect);
    }

    private void InstantiateHudIfNeeded()
    {
        if (hudInstance != null)
            return;

        if (hudPrefab == null)
        {
            Debug.LogWarning(
                "[BiofiltreHudBinder] hudPrefab is null — HUD biofiltre non instancié (fail closed).",
                this);
            return;
        }

        hudInstance = Instantiate(hudPrefab, transform);
        hudInstance.name = hudPrefab.name;

        Canvas canvas = hudInstance.GetComponentInChildren<Canvas>(true);
        if (canvas != null)
            canvas.renderMode = RenderMode.WorldSpace;
    }

    private void PositionRow(MonoBehaviour row, Vector2 normalizedAnchor, Vector2 worldOffset, Rect worldRect)
    {
        if (row == null)
            return;

        Vector3 worldPos = NormalizedAnchorToWorld(normalizedAnchor, worldOffset, worldRect);
        row.transform.position = new Vector3(worldPos.x, worldPos.y, hudWorldZ);
    }

    private static Vector3 NormalizedAnchorToWorld(Vector2 normalizedAnchor, Vector2 worldOffset, Rect worldRect)
    {
        float x = worldRect.x + normalizedAnchor.x * worldRect.width + worldOffset.x;
        float y = worldRect.y + normalizedAnchor.y * worldRect.height + worldOffset.y;
        return new Vector3(x, y, 0f);
    }
}
