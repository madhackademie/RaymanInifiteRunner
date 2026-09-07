using UnityEngine;

/// <summary>
/// Free-transform UI : 4 coins locaux indépendants (losange / trapèze).
/// Le RectTransform Unity reste un rectangle — le mesh des Images est déformé.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UiQuadWarp : MonoBehaviour
{
    [SerializeField] private Vector2 bottomLeft;
    [SerializeField] private Vector2 bottomRight;
    [SerializeField] private Vector2 topRight;
    [SerializeField] private Vector2 topLeft;
    [SerializeField] private bool hasCustomWarp;

    public bool HasCustomWarp => hasCustomWarp;

    public RectTransform RectTransform => (RectTransform)transform;

    public Rect SourceRect => RectTransform.rect;

    private void OnEnable()
    {
        UiQuadWarpGraphicUtil.EnsureEffects(this);
        DirtyMeshes();
    }

    private void OnDisable()
    {
        DirtyMeshes();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        DirtyMeshes();
    }
#endif

    public Vector2 GetCorner(int index)
    {
        if (!hasCustomWarp)
            return GetDefaultCorner(index);

        return index switch
        {
            0 => bottomLeft,
            1 => bottomRight,
            2 => topRight,
            _ => topLeft,
        };
    }

    public void SetCorner(int index, Vector2 localPoint)
    {
        if (!hasCustomWarp)
            CopyDefaultsToCustom();

        switch (index)
        {
            case 0: bottomLeft = localPoint; break;
            case 1: bottomRight = localPoint; break;
            case 2: topRight = localPoint; break;
            default: topLeft = localPoint; break;
        }

        DirtyMeshes();
    }

    public void ResetCornersToRect()
    {
        CopyDefaultsToCustom();
        hasCustomWarp = false;
        DirtyMeshes();
    }

    public Vector3 WarpVertex(Transform graphicTransform, Vector3 graphicLocalPos)
    {
        if (!hasCustomWarp)
            return graphicLocalPos;

        Vector3 world = graphicTransform.TransformPoint(graphicLocalPos);
        Vector3 local = transform.InverseTransformPoint(world);
        Vector2 mapped = MapPoint(local);
        Vector3 worldMapped = transform.TransformPoint(new Vector3(mapped.x, mapped.y, local.z));
        return graphicTransform.InverseTransformPoint(worldMapped);
    }

    public void DirtyMeshes()
    {
        UiQuadWarpGraphicUtil.DirtyChildren(this);
    }

    private Vector2 MapPoint(Vector2 localPoint)
    {
        Rect rect = SourceRect;
        float u = rect.width > 0.0001f
            ? Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x)
            : 0.5f;
        float v = rect.height > 0.0001f
            ? Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y)
            : 0.5f;

        Vector2 bottom = Vector2.Lerp(GetCorner(0), GetCorner(1), u);
        Vector2 top = Vector2.Lerp(GetCorner(3), GetCorner(2), u);
        return Vector2.Lerp(bottom, top, v);
    }

    private Vector2 GetDefaultCorner(int index)
    {
        Rect rect = SourceRect;
        return index switch
        {
            0 => new Vector2(rect.xMin, rect.yMin),
            1 => new Vector2(rect.xMax, rect.yMin),
            2 => new Vector2(rect.xMax, rect.yMax),
            _ => new Vector2(rect.xMin, rect.yMax),
        };
    }

    private void CopyDefaultsToCustom()
    {
        hasCustomWarp = true;
        bottomLeft = GetDefaultCorner(0);
        bottomRight = GetDefaultCorner(1);
        topRight = GetDefaultCorner(2);
        topLeft = GetDefaultCorner(3);
    }
}
