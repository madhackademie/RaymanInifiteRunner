using UnityEngine;

/// <summary>
/// Scale le sprite cuve IBC pour que le dessus plantable accepte
/// <see cref="GridManager.GetWorldRect"/>. La grille ne change pas.
/// Runtime only : pas d'ExecuteAlways, pas de transform.Find.
/// </summary>
[RequireComponent(typeof(GridManager))]
public class BiofiltreIbcSpriteFitter : MonoBehaviour
{
    private const int DefaultSortingOrder = -1;
    private const string RuntimeChildName = "IbcSprite";

    /// <summary>
    /// Deck = argile du carré ortho (mesure auteur, dépliage UV
    /// <c>Cuve_IBC_deck_carre_plus_face.png</c>).
    /// </summary>
    private static readonly Rect DefaultDeckNormalized =
        new Rect(0.0266f, 0.4483f, 0.9446f, 0.5232f);

    [Tooltip("Sprite promu (Sprites/Farm/Biofiltre), pas le Dump.")]
    [SerializeField] private Sprite ibcSprite;

    [SerializeField] private int sortingOrder = DefaultSortingOrder;

    [Tooltip("Dessus plantable en UV sprite 0–1, origine bas-gauche.")]
    [SerializeField] private Rect deckNormalized = DefaultDeckNormalized;

    private GridManager gridManager;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        FitToGrid();
    }

    /// <summary>Recale la cuve sur l'AABB grille courante.</summary>
    public void FitToGrid()
    {
        if (ibcSprite == null)
        {
            Debug.LogWarning(
                "[BiofiltreIbcSpriteFitter] ibcSprite is null — cuve non affichée (fail closed).",
                this);
            return;
        }

        if (gridManager == null)
            gridManager = GetComponent<GridManager>();

        EnsureRenderer();
        ApplyFit(gridManager.GetWorldRect());
    }

    private void EnsureRenderer()
    {
        if (spriteRenderer != null)
            return;

        var child = new GameObject(RuntimeChildName);
        child.transform.SetParent(transform, worldPositionStays: false);
        spriteRenderer = child.AddComponent<SpriteRenderer>();
    }

    private void ApplyFit(Rect worldRect)
    {
        Rect deckPixels = GetDeckPixelRect(ibcSprite, deckNormalized);
        if (deckPixels.width < 1f || deckPixels.height < 1f)
        {
            Debug.LogWarning("[BiofiltreIbcSpriteFitter] deckNormalized invalide.", this);
            return;
        }

        float ppu = ibcSprite.pixelsPerUnit;
        float scaleX = worldRect.width * ppu / deckPixels.width;
        float scaleY = worldRect.height * ppu / deckPixels.height;
        spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        Vector2 deckCenterLocal = GetDeckCenterLocalUnscaled(ibcSprite, deckPixels);
        Vector2 scaledOffset = Vector2.Scale(deckCenterLocal, new Vector2(scaleX, scaleY));
        Vector2 worldCenter = worldRect.center;
        spriteRenderer.transform.position = new Vector3(
            worldCenter.x - scaledOffset.x,
            worldCenter.y - scaledOffset.y,
            0f);

        spriteRenderer.sprite = ibcSprite;
        spriteRenderer.sortingOrder = sortingOrder;
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
