using UnityEngine;

/// <summary>
/// Rect monde de la scène ferme (niveau entier, multi-biofiltres).
/// Branché sur <see cref="FarmCameraController"/> pour clamp pan/zoom — optionnel tant que non posé en scène.
/// </summary>
[DisallowMultipleComponent]
public class FarmLevelViewBounds : MonoBehaviour
{
    private const float MinSize = 1f;

    [SerializeField] private Vector2 localCenter = Vector2.zero;
    [SerializeField] private Vector2 localSize = new Vector2(64f, 64f);

    private readonly Vector2[] worldCornerBuffer = new Vector2[4];

    public Rect GetWorldAabb()
    {
        GetWorldCorners(worldCornerBuffer);
        return EncapsulateCorners(worldCornerBuffer);
    }

    public void GetWorldCorners(Vector2[] corners)
    {
        Vector2 half = localSize * 0.5f;
        corners[0] = TransformLocal(localCenter + new Vector2(-half.x, -half.y));
        corners[1] = TransformLocal(localCenter + new Vector2(half.x, -half.y));
        corners[2] = TransformLocal(localCenter + new Vector2(half.x, half.y));
        corners[3] = TransformLocal(localCenter + new Vector2(-half.x, half.y));
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
    private void OnDrawGizmosSelected()
    {
        GetWorldCorners(worldCornerBuffer);
        Gizmos.color = new Color(0.2f, 0.55f, 1f, 0.85f);
        for (int i = 0; i < 4; i++)
            Gizmos.DrawLine(worldCornerBuffer[i], worldCornerBuffer[(i + 1) % 4]);
    }
#endif
}
