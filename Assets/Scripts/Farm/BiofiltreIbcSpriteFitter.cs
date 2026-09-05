using UnityEngine;

/// <summary>
/// Affiche la cuve IBC. Le transform de <c>IbcSprite</c> est manuel
/// (pas de calage auto sur la surface grille / deck).
/// </summary>
[RequireComponent(typeof(GridManager))]
public class BiofiltreIbcSpriteFitter : MonoBehaviour
{
    private const float MinDeckUvSize = 0.01f;
    private const int DefaultSortingOrder = -1;
    private const float DefaultExtraScale = 1.05f;

    private static readonly Rect DefaultDeckNormalized =
        new Rect(0.059f, 0.5239f, 0.8844f, 0.441f);

    [Tooltip("Enfant IbcSprite. Transform libre (Move / Rotate / Scale).")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Sprite promu (Sprites/Farm/Biofiltre/IbcIso), pas le Dump.")]
    [SerializeField] private Sprite ibcSprite;

    [SerializeField] private int sortingOrder = DefaultSortingOrder;

    [Tooltip("Réservé au bouton Inspector « Fit once ». Ignoré au runtime.")]
    [SerializeField] private Rect deckNormalized = DefaultDeckNormalized;

    [Tooltip("Réservé au bouton Inspector « Fit once ». Ignoré au runtime.")]
    [SerializeField] private float extraScale = DefaultExtraScale;

    [Tooltip("Réservé au bouton Inspector « Fit once ». Ignoré au runtime.")]
    [SerializeField] private float extraRotationZ;

    private GridManager gridManager;

    private void OnEnable()
    {
        ApplySpriteAsset();
    }

    private void ApplySpriteAsset()
    {
        if (spriteRenderer == null || ibcSprite == null)
            return;

        spriteRenderer.sprite = ibcSprite;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Calage optionnel deck ↔ grille. Ne pas appeler au Play / OnEnable.
    /// </summary>
    public void FitToGrid()
    {
        if (ibcSprite == null || spriteRenderer == null)
            return;

        if (gridManager == null)
            gridManager = GetComponent<GridManager>();

        gridManager.RebuildMapperFromInspector();
        ApplyFit(gridManager.GetSpriteFitWorldRect());
    }

    private void ApplyFit(Rect worldRect)
    {
        Rect deckUv = ResolveDeckUv();
        Rect deckPixels = GetDeckPixelRect(ibcSprite, deckUv);
        if (deckPixels.width < 1f || deckPixels.height < 1f)
            return;

        float ppu = ibcSprite.pixelsPerUnit;
        float scaleMul = extraScale > 0.01f ? extraScale : DefaultExtraScale;
        float scaleX = worldRect.width * ppu / deckPixels.width * scaleMul;
        float scaleY = worldRect.height * ppu / deckPixels.height * scaleMul;
        spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        Vector2 deckCenterLocal = GetDeckCenterLocalUnscaled(ibcSprite, deckPixels);
        Vector2 scaledOffset = Vector2.Scale(deckCenterLocal, new Vector2(scaleX, scaleY));
        Vector2 worldCenter = worldRect.center;
        spriteRenderer.transform.position = new Vector3(
            worldCenter.x - scaledOffset.x,
            worldCenter.y - scaledOffset.y,
            0f);

        spriteRenderer.transform.rotation = Quaternion.identity;
        if (Mathf.Abs(extraRotationZ) > 0.001f)
            spriteRenderer.transform.RotateAround(worldCenter, Vector3.forward, extraRotationZ);

        ApplySpriteAsset();
    }

    private Rect ResolveDeckUv()
    {
        if (deckNormalized.width >= MinDeckUvSize && deckNormalized.height >= MinDeckUvSize)
            return deckNormalized;

        return DefaultDeckNormalized;
    }

    private static Rect GetDeckPixelRect(Sprite sprite, Rect deckUv)
    {
        Rect spriteRect = sprite.rect;
        return new Rect(
            spriteRect.x + deckUv.x * spriteRect.width,
            spriteRect.y + deckUv.y * spriteRect.height,
            deckUv.width * spriteRect.width,
            deckUv.height * spriteRect.height);
    }

    private static Vector2 GetDeckCenterLocalUnscaled(Sprite sprite, Rect deckPixels)
    {
        float ppu = sprite.pixelsPerUnit;
        Rect spriteRect = sprite.rect;
        Vector2 pivot = sprite.pivot;
        float centerX = deckPixels.x + deckPixels.width * 0.5f;
        float centerY = deckPixels.y + deckPixels.height * 0.5f;
        return new Vector2(
            (centerX - (spriteRect.x + pivot.x)) / ppu,
            (centerY - (spriteRect.y + pivot.y)) / ppu);
    }
}
