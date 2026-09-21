using UnityEngine;

/// <summary>
/// Rect 2D auteur : vue caméra par défaut du biofiltre + bornes de zoom/pan.
/// Indépendant de <see cref="GridManager.GetWorldRect"/> (AABB grille) et du bake deck UV.
/// </summary>
[DisallowMultipleComponent]
public class BiofiltreViewBounds : MonoBehaviour
{
    private const float MinSize = 0.5f;

    [Tooltip("Centre du rect, local au biofiltre.")]
    [SerializeField] private Vector2 localCenter = Vector2.zero;

    [Tooltip("Taille locale (avant scale du transform).")]
    [SerializeField] private Vector2 localSize = new Vector2(8f, 6f);

    private readonly Vector2[] worldCornerBuffer = new Vector2[4];

    public Vector2 LocalCenter => localCenter;
    public Vector2 LocalSize => localSize;

    /// <summary>AABB monde du rect (4 coins transformés).</summary>
    public Rect GetWorldAabb()
    {
        GetWorldCorners(worldCornerBuffer);
        return EncapsulateCorners(worldCornerBuffer);
    }

    /// <summary>Coins monde : bas-gauche, bas-droit, haut-droit, haut-gauche.</summary>
    public void GetWorldCorners(Vector2[] corners)
    {
        Vector2 half = localSize * 0.5f;
        corners[0] = TransformLocal(localCenter + new Vector2(-half.x, -half.y));
        corners[1] = TransformLocal(localCenter + new Vector2(half.x, -half.y));
        corners[2] = TransformLocal(localCenter + new Vector2(half.x, half.y));
        corners[3] = TransformLocal(localCenter + new Vector2(-half.x, half.y));
    }

    public void SetLocalRect(Vector2 center, Vector2 size)
    {
        localCenter = center;
        localSize = new Vector2(Mathf.Max(MinSize, size.x), Mathf.Max(MinSize, size.y));
    }

    /// <summary>Copie l'AABB grille (point de départ, pas un lien runtime).</summary>
    public void FitFromGrid()
    {
        GridManager grid = GetComponent<GridManager>();
        if (grid == null)
            return;

        grid.RebuildMapperFromInspector();
        SetFromWorldAabb(grid.GetWorldRect());
    }

    /// <summary>Copie les bounds monde du sprite IBC.</summary>
    public void FitFromIbcSprite()
    {
        Transform ibcChild = transform.Find("IbcSprite");
        SpriteRenderer spriteRenderer = ibcChild != null
            ? ibcChild.GetComponent<SpriteRenderer>()
            : null;
        if (spriteRenderer == null)
            return;

        Bounds bounds = spriteRenderer.bounds;
        SetFromWorldAabb(new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y));
    }

    public void SetFromWorldAabb(Rect worldAabb)
    {
        Vector3 localMin = transform.InverseTransformPoint(worldAabb.min);
        Vector3 localMax = transform.InverseTransformPoint(worldAabb.max);
        Vector2 min = new Vector2(Mathf.Min(localMin.x, localMax.x), Mathf.Min(localMin.y, localMax.y));
        Vector2 max = new Vector2(Mathf.Max(localMin.x, localMax.x), Mathf.Max(localMin.y, localMax.y));
        SetLocalRect((min + max) * 0.5f, max - min);
    }

    private Vector2 TransformLocal(Vector2 localPoint) =>
        transform.TransformPoint(localPoint);

    private static Rect EncapsulateCorners(Vector2[] corners)
    {
        float minX = corners[0].x;
        float minY = corners[0].y;
        float maxX = corners[0].x;
        float maxY = corners[0].y;
        for (int i = 1; i < 4; i++)
        {
            minX = Mathf.Min(minX, corners[i].x);
            minY = Mathf.Min(minY, corners[i].y);
            maxX = Mathf.Max(maxX, corners[i].x);
            maxY = Mathf.Max(maxY, corners[i].y);
        }

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        FitFromGrid();
    }

    private void OnValidate()
    {
        localSize.x = Mathf.Max(MinSize, localSize.x);
        localSize.y = Mathf.Max(MinSize, localSize.y);
    }

    private void OnDrawGizmos()
    {
        GetWorldCorners(worldCornerBuffer);
        Gizmos.color = new Color(1f, 0.45f, 0.12f, 0.95f);
        for (int i = 0; i < 4; i++)
            Gizmos.DrawLine(worldCornerBuffer[i], worldCornerBuffer[(i + 1) % 4]);
    }
#endif
}
