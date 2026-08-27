using UnityEngine;

/// <summary>
/// Visual skin for a biofiltre bed. The grid / culture area comes from <see cref="GridManager"/>;
/// this asset only describes how the sprite's clay region maps onto that fixed rectangle.
/// Swap skins without changing columns, rows, or cell size.
/// </summary>
[CreateAssetMenu(menuName = "Game/Data/Ferme/Bac biofiltre (skin)", fileName = "BiofiltreBed_")]
public class BiofiltreBedSkin : ScriptableObject
{
    private const float MinInnerSpan = 0.2f;

    [SerializeField] private Sprite sprite;

    [Tooltip("Clay / plantable region inside the sprite (0-1, origin bottom-left). The grid is fitted to this rect, not the full texture.")]
    [SerializeField] private Rect innerRect = new Rect(0.12f, 0.36f, 0.76f, 0.50f);

    public Sprite Sprite => sprite;

    /// <summary>
    /// Computes sprite size, clay size, and pivot→clay-center offset in unscaled sprite world units.
    /// </summary>
    public bool TryGetLayout(out Vector2 spriteWorldSize, out Vector2 innerWorldSize, out Vector2 pivotToInnerCenter)
    {
        spriteWorldSize = default;
        innerWorldSize = default;
        pivotToInnerCenter = default;

        if (sprite == null || sprite.pixelsPerUnit <= 0.01f)
            return false;

        spriteWorldSize = new Vector2(
            sprite.rect.width / sprite.pixelsPerUnit,
            sprite.rect.height / sprite.pixelsPerUnit
        );

        Rect clay = ClampInnerRect(innerRect);
        innerWorldSize = new Vector2(spriteWorldSize.x * clay.width, spriteWorldSize.y * clay.height);
        Vector2 innerCenter = new Vector2(clay.xMin + clay.width * 0.5f, clay.yMin + clay.height * 0.5f);
        pivotToInnerCenter = new Vector2(
            (innerCenter.x - 0.5f) * spriteWorldSize.x,
            (innerCenter.y - 0.5f) * spriteWorldSize.y
        );

        return innerWorldSize.x > 0.01f && innerWorldSize.y > 0.01f;
    }

    private static Rect ClampInnerRect(Rect rect)
    {
        float x = Mathf.Clamp01(rect.x);
        float y = Mathf.Clamp01(rect.y);
        float width = Mathf.Clamp(rect.width, MinInnerSpan, 1f - x);
        float height = Mathf.Clamp(rect.height, MinInnerSpan, 1f - y);
        return new Rect(x, y, width, height);
    }
}
