using System;
using UnityEngine;

/// <summary>
/// Quad en UV 0–1 (origine bas-gauche du <see cref="Sprite.rect"/>).
/// Définit la zone logique du deck pour le bake colonnes/lignes — sans sémantique « billes ».
/// </summary>
[Serializable]
public struct BiofiltreDeckShapeUv
{
    public Vector2 southWest;
    public Vector2 southEast;
    public Vector2 northEast;
    public Vector2 northWest;

    public void SetFromNormalizedRect(Rect normalizedRect)
    {
        float x = normalizedRect.x;
        float y = normalizedRect.y;
        float w = normalizedRect.width;
        float h = normalizedRect.height;

        southWest = new Vector2(x, y);
        southEast = new Vector2(x + w, y);
        northEast = new Vector2(x + w, y + h);
        northWest = new Vector2(x, y + h);
    }

    public Rect GetBoundingRect()
    {
        float minX = Mathf.Min(southWest.x, southEast.x, northEast.x, northWest.x);
        float maxX = Mathf.Max(southWest.x, southEast.x, northEast.x, northWest.x);
        float minY = Mathf.Min(southWest.y, southEast.y, northEast.y, northWest.y);
        float maxY = Mathf.Max(southWest.y, southEast.y, northEast.y, northWest.y);
        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    public void GetCorners(Vector2[] buffer)
    {
        if (buffer == null || buffer.Length < 4)
            return;

        buffer[0] = southWest;
        buffer[1] = southEast;
        buffer[2] = northEast;
        buffer[3] = northWest;
    }

    public bool IsValid()
    {
        Rect bounds = GetBoundingRect();
        return bounds.width >= 0.001f && bounds.height >= 0.001f;
    }
}
