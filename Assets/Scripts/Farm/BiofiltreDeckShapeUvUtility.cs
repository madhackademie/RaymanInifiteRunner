using UnityEngine;

/// <summary>
/// UV shape → monde via le <see cref="SpriteRenderer"/> IBC.
/// </summary>
public static class BiofiltreDeckShapeUvUtility
{
    private static readonly Vector2[] CornerBuffer = new Vector2[4];

    public static bool TryGetShapeWorldCorners(
        BiofiltreDeckShapeUv shape,
        SpriteRenderer spriteRenderer,
        out Vector2[] worldCorners)
    {
        worldCorners = null;

        if (!shape.IsValid() || spriteRenderer == null || spriteRenderer.sprite == null)
            return false;

        shape.GetCorners(CornerBuffer);
        worldCorners = new Vector2[4];
        Transform t = spriteRenderer.transform;
        Sprite sprite = spriteRenderer.sprite;

        for (int i = 0; i < 4; i++)
            worldCorners[i] = NormalizedRectUvToWorld(sprite, t, CornerBuffer[i]);

        return true;
    }

    /// <summary>UV 0–1 relatif au rect du sprite (origine bas-gauche).</summary>
    public static Vector2 NormalizedRectUvToWorld(Sprite sprite, Transform spriteTransform, Vector2 uv01)
    {
        Rect rect = sprite.rect;
        Vector2 pivot = sprite.pivot;
        float ppu = sprite.pixelsPerUnit;

        float px = rect.x + uv01.x * rect.width;
        float py = rect.y + uv01.y * rect.height;
        Vector2 local = new Vector2(
            (px - rect.x - pivot.x) / ppu,
            (py - rect.y - pivot.y) / ppu);

        return spriteTransform.TransformPoint(local);
    }

    /// <summary>Monde → UV 0–1 sur le rect du sprite (plan XY du transform).</summary>
    public static bool TryWorldToNormalizedRectUv(
        Sprite sprite,
        Transform spriteTransform,
        Vector3 worldPosition,
        out Vector2 uv01)
    {
        uv01 = default;
        if (sprite == null || spriteTransform == null)
            return false;

        Vector3 local3 = spriteTransform.InverseTransformPoint(worldPosition);
        Vector2 local = new Vector2(local3.x, local3.y);

        Rect rect = sprite.rect;
        Vector2 pivot = sprite.pivot;
        float ppu = sprite.pixelsPerUnit;

        float px = local.x * ppu + rect.x + pivot.x;
        float py = local.y * ppu + rect.y + pivot.y;

        uv01 = new Vector2(
            (px - rect.x) / rect.width,
            (py - rect.y) / rect.height);

        return uv01.x >= -0.05f && uv01.x <= 1.05f && uv01.y >= -0.05f && uv01.y <= 1.05f;
    }
}
