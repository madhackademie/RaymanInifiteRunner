using UnityEngine;

/// <summary>
/// Cadrage ortho 2D : fit d'un AABB et clamp zoom/pan.
/// La grille n'est pas recalculée — la caméra est seulement une fenêtre.
/// </summary>
public static class FarmCameraViewMath
{
    private const float MinAspect = 0.01f;
    private const float MinOrthoSize = 0.1f;

    /// <summary>
    /// Ortho size pour que tout le rect soit visible (vue par défaut / zoom out max).
    /// </summary>
    public static float ComputeFitOrthographicSize(Rect worldAabb, float aspect, float paddingFactor)
    {
        float safeAspect = Mathf.Max(MinAspect, aspect);
        float fromHeight = worldAabb.height * 0.5f;
        float fromWidth = worldAabb.width * 0.5f / safeAspect;
        float fit = Mathf.Max(fromHeight, fromWidth);
        float pad = Mathf.Max(1f, paddingFactor);
        return Mathf.Max(MinOrthoSize, fit * pad);
    }

    public static float ClampOrthographicSize(float size, float minSize, float maxSize)
    {
        float maxSafe = Mathf.Max(MinOrthoSize, maxSize);
        float minSafe = Mathf.Clamp(minSize, MinOrthoSize, maxSafe);
        return Mathf.Clamp(size, minSafe, maxSafe);
    }

    /// <summary>
    /// Centre caméra pour rester dans le rect : zoomé out = lock centre ;
    /// zoomé in = frustum à l'intérieur de l'AABB.
    /// </summary>
    public static Vector2 ClampCameraCenter(Vector2 center, Rect bounds, float orthoSize, float aspect)
    {
        float halfHeight = orthoSize;
        float halfWidth = orthoSize * Mathf.Max(MinAspect, aspect);
        float x = ClampAxis(center.x, bounds.xMin, bounds.xMax, halfWidth);
        float y = ClampAxis(center.y, bounds.yMin, bounds.yMax, halfHeight);
        return new Vector2(x, y);
    }

    private static float ClampAxis(float center, float min, float max, float halfExtent)
    {
        float span = max - min;
        if (halfExtent * 2f >= span)
            return (min + max) * 0.5f;

        return Mathf.Clamp(center, min + halfExtent, max - halfExtent);
    }
}
