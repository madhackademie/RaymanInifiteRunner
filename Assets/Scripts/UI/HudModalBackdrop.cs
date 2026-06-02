using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Style commun des panneaux plein écran sous <see cref="UIManager"/> (Shop, Inventaire, futures modales HUD).
/// Ne s’applique pas aux scènes gameplay comme <c>FirstLvl</c> (hors ce canvas).
/// </summary>
public static class HudModalBackdrop
{
    /// <summary>Chemin Resources sans extension (PNG/Sprite). Même fichier pour toutes les modales HUD.</summary>
    public const string ResourcesRelativePath = "Shop/shop_backdrop";

    /// <summary>Couleur de secours si aucune texture / sprite n’est disponible.</summary>
    public static readonly Color RootSolidWhenNoTexture = new Color(0.04f, 0.04f, 0.06f, 0.97f);

    /// <summary>Zone centrale (liste) : presque opaque pour ne pas voir la Home à travers.</summary>
    public static readonly Color ContentPanelColor = new Color(0.07f, 0.07f, 0.09f, 0.995f);

    private static Sprite s_cachedWhiteSprite;
    private static Sprite s_resourcesBackdropSprite;
    private static bool s_triedLoadResourcesBackdrop;

    /// <summary>Fond racine plein écran : override UIManager → Resources → blanc 1×1.</summary>
    public static void ApplyRootBackground(Image image)
    {
        if (image == null)
            return;

        Sprite inspectorOverride = UIManager.Instance != null ? UIManager.Instance.HudModalBackdropSprite : null;
        image.sprite = ResolveBackdropSprite(inspectorOverride);
        image.type = Image.Type.Simple;
        image.preserveAspect = false;

        bool usesArt = inspectorOverride != null || TryLoadResourcesBackdropSprite() != null;
        image.color = usesArt
            ? (UIManager.Instance != null ? UIManager.Instance.HudModalBackdropTint : Color.white)
            : RootSolidWhenNoTexture;

        image.raycastTarget = true;
    }

    /// <summary>Pan sous la grille (opaque + sprite blanc pour rendu fiable).</summary>
    public static void ApplyContentPanel(Image image)
    {
        if (image == null)
            return;

        SetupSolidFillImage(image);
        image.color = ContentPanelColor;
        image.raycastTarget = true;
    }

    /// <summary>Sans sprite, certains builds n’affichent pas la couleur : on force un blanc 1×1.</summary>
    public static void SetupSolidFillImage(Image image)
    {
        if (image == null)
            return;

        image.sprite = GetOrCreateWhiteSprite();
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
    }

    private static Sprite ResolveBackdropSprite(Sprite inspectorOverride)
    {
        if (inspectorOverride != null)
            return inspectorOverride;

        Sprite fromResources = TryLoadResourcesBackdropSprite();
        if (fromResources != null)
            return fromResources;

        return GetOrCreateWhiteSprite();
    }

    private static Sprite TryLoadResourcesBackdropSprite()
    {
        if (s_triedLoadResourcesBackdrop)
            return s_resourcesBackdropSprite;

        s_triedLoadResourcesBackdrop = true;

        s_resourcesBackdropSprite = Resources.Load<Sprite>(ResourcesRelativePath);
        if (s_resourcesBackdropSprite != null)
            return s_resourcesBackdropSprite;

        Texture2D texture = Resources.Load<Texture2D>(ResourcesRelativePath);
        if (texture == null)
            return null;

        s_resourcesBackdropSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect);

        return s_resourcesBackdropSprite;
    }

    private static Sprite GetOrCreateWhiteSprite()
    {
        if (s_cachedWhiteSprite != null)
            return s_cachedWhiteSprite;

        Texture2D texture = new(1, 1, TextureFormat.RGBA32, false)
        {
            name = "HudModal_WhiteTex",
            hideFlags = HideFlags.HideAndDontSave
        };
        texture.SetPixel(0, 0, Color.white);
        texture.Apply(false, true);

        s_cachedWhiteSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect);
        s_cachedWhiteSprite.name = "HudModal_WhiteSprite";
        return s_cachedWhiteSprite;
    }
}
