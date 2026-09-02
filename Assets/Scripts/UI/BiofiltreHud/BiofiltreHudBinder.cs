using UnityEngine;

/// <summary>
/// Instancie et positionne le HUD world biofiltre depuis l’AABB grille de <see cref="GridManager"/>.
/// Offsets normalisés (0–1, origine bas-gauche) + extra monde — overridables par instance.
/// </summary>
[RequireComponent(typeof(GridManager))]
public class BiofiltreHudBinder : MonoBehaviour
{
    private const int HudSortingOrder = 20;
    private const float FallbackRowWidthPixels = 280f;
    private const float HudWidthVsGrid = 0.72f;

    [SerializeField] private BiofiltreHudView hudPrefab;
    [SerializeField] private GridManager gridManager;

    [Header("Primary row (mockup IBC — override per instance)")]
    [SerializeField] private Vector2 primaryNormalizedAnchor = new Vector2(0.02f, 1.14f);
    [SerializeField] private Vector2 primaryWorldOffset;

    [Header("Star row (pivot droite — haut-droit du deck)")]
    [SerializeField] private Vector2 starNormalizedAnchor = new Vector2(0.98f, 1.14f);
    [SerializeField] private Vector2 starWorldOffset;

    [Header("Secondary row (face cuve, sous le gravier)")]
    [SerializeField] private Vector2 secondaryNormalizedAnchor = new Vector2(0.04f, -0.18f);
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
        FitCanvasToGrid(worldRect);

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
        ConfigureWorldCanvas();
        ApplyRowPivots();
    }

    private void ConfigureWorldCanvas()
    {
        Canvas canvas = hudInstance.GetComponent<Canvas>();
        if (canvas == null)
            canvas = hudInstance.GetComponentInChildren<Canvas>(true);
        if (canvas == null)
            return;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = HudSortingOrder;

        Camera cam = Camera.main;
        if (cam != null)
            hudInstance.transform.rotation =
                Quaternion.LookRotation(-cam.transform.forward, cam.transform.up);
    }

    /// <summary>
    /// LookRotation face caméra inverse le X local : les rangées (pivot gauche)
    /// s’étendaient hors écran. Scale X négatif rétablit gauche → droite.
    /// </summary>
    private void FitCanvasToGrid(Rect worldRect)
    {
        float rowWidthPx = FallbackRowWidthPixels;
        if (hudInstance.SecondaryRow != null &&
            hudInstance.SecondaryRow.transform is RectTransform rowRt &&
            rowRt.rect.width > 1f)
            rowWidthPx = rowRt.rect.width;

        float scale = worldRect.width * HudWidthVsGrid / rowWidthPx;
        hudInstance.transform.localScale = new Vector3(-scale, scale, scale);
    }

    private void ApplyRowPivots()
    {
        SetLeftPivot(hudInstance.PrimaryRow);
        SetLeftPivot(hudInstance.SecondaryRow);
        SetRightPivot(hudInstance.StarRow);
    }

    private static void SetLeftPivot(MonoBehaviour row)
    {
        if (row != null && row.transform is RectTransform rt)
            rt.pivot = new Vector2(0f, 0.5f);
    }

    private static void SetRightPivot(MonoBehaviour row)
    {
        if (row != null && row.transform is RectTransform rt)
            rt.pivot = new Vector2(1f, 0.5f);
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
