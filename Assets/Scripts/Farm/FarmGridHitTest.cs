using UnityEngine;

/// <summary>
/// Tests géométriques grille ferme (losange iso, etc.).
/// </summary>
public static class FarmGridHitTest
{
    /// <summary>Point dans un quadrilatère convexe (4 coins, winding cohérent).</summary>
    public static bool IsPointInConvexQuad(Vector2 point, Vector2[] corners)
    {
        if (corners == null || corners.Length < 4)
            return false;

        bool hasPositive = false;
        bool hasNegative = false;

        for (int i = 0; i < 4; i++)
        {
            Vector2 a = corners[i];
            Vector2 b = corners[(i + 1) % 4];
            float cross = (b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x);

            if (cross > 0f)
                hasPositive = true;
            if (cross < 0f)
                hasNegative = true;

            if (hasPositive && hasNegative)
                return false;
        }

        return true;
    }
}
