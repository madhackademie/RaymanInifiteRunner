using UnityEngine;

/// <summary>
/// Copie du sprite en silhouette unie (blanc) + scale/offset = halo de sélection.
/// Nécessite le shader <c>Farm/SpriteSelectionSilhouette</c> (alpha du sprite, pas la couleur texture).
/// </summary>
[DisallowMultipleComponent]
public class PlantSelectionHighlight : MonoBehaviour
{
    private const string SilhouetteChildName = "GlowInner";
    private const string FallbackSilhouetteName = "SelectionSilhouetteWhite";
    private const string SilhouetteShaderName = "Farm/SpriteSelectionSilhouette";

    [SerializeField] private SpriteRenderer plantRenderer;
    [SerializeField] private GameObject glowRoot;
    [SerializeField] private SpriteRenderer[] glowRenderers;

    [Header("Silhouette blanche")]
    [SerializeField] private Color silhouetteColor = Color.white;
    [Tooltip("Plus proche de 1 = contour plus fin (ex. 1.02–1.03).")]
    [SerializeField] private float silhouetteLocalScale = 1.012f;
    [Tooltip("Décalage local ; réduire pour un halo moins épais (souvent ~0,01 ou 0,0).")]
    [SerializeField] private Vector2 silhouetteLocalOffset = new(0.004f, 0.004f);
    [SerializeField] private int sortingOrderBehindPlant = 2;

    private static Material sharedSilhouetteMaterial;

    private SpriteRenderer silhouetteRenderer;
    private bool isHighlightActive;
    private bool warnedMissingGlow;

    private void Awake()
    {
        ResolveReferences();
    }

    public void SetHighlightActive(bool active)
    {
        if (active)
            ResolveReferences();

        isHighlightActive = active;

        if (!EnsureSilhouetteReady())
        {
            if (active && !warnedMissingGlow)
            {
                warnedMissingGlow = true;
                Debug.LogWarning("[PlantSelectionHighlight] Silhouette blanche introuvable.", this);
            }
            return;
        }

        if (active)
            SyncSilhouetteFromPlant();

        if (glowRoot != null)
            glowRoot.SetActive(active);
    }

    public void RefreshIfActive()
    {
        if (!isHighlightActive)
            return;

        SyncSilhouetteFromPlant();
    }

    private void ResolveReferences()
    {
        if (plantRenderer == null)
            plantRenderer = GetComponent<SpriteRenderer>();

        if (glowRoot == null)
        {
            Transform found = transform.Find("SelectionGlow");
            if (found != null)
                glowRoot = found.gameObject;
        }
    }

    private bool EnsureSilhouetteReady()
    {
        if (plantRenderer == null)
            return false;

        if (glowRoot == null)
        {
            glowRoot = new GameObject("SelectionGlow");
            glowRoot.transform.SetParent(transform, false);
            glowRoot.transform.localPosition = Vector3.zero;
            glowRoot.transform.localRotation = Quaternion.identity;
            glowRoot.transform.localScale = Vector3.one;
        }

        silhouetteRenderer = ResolveSilhouetteRenderer();
        if (silhouetteRenderer == null)
            return false;

        HideNonSilhouetteGlowChildren();
        ConfigureSilhouetteTransform(silhouetteRenderer.transform);
        ApplySilhouetteMaterial(silhouetteRenderer);
        glowRenderers = new[] { silhouetteRenderer };
        return true;
    }

    private SpriteRenderer ResolveSilhouetteRenderer()
    {
        Transform inner = glowRoot.transform.Find(SilhouetteChildName);
        if (inner != null && inner.TryGetComponent(out SpriteRenderer innerSr))
            return innerSr;

        Transform fallback = glowRoot.transform.Find(FallbackSilhouetteName);
        if (fallback != null && fallback.TryGetComponent(out SpriteRenderer fallbackSr))
            return fallbackSr;

        var go = new GameObject(FallbackSilhouetteName);
        go.transform.SetParent(glowRoot.transform, false);
        return go.AddComponent<SpriteRenderer>();
    }

    private void HideNonSilhouetteGlowChildren()
    {
        for (int i = 0; i < glowRoot.transform.childCount; i++)
        {
            Transform child = glowRoot.transform.GetChild(i);
            if (child.GetComponent<SpriteRenderer>() == silhouetteRenderer)
            {
                child.gameObject.SetActive(true);
                continue;
            }

            if (child.GetComponent<SpriteRenderer>() != null)
                child.gameObject.SetActive(false);
        }
    }

    private void ConfigureSilhouetteTransform(Transform silhouetteTransform)
    {
        silhouetteTransform.localPosition = new Vector3(
            silhouetteLocalOffset.x,
            silhouetteLocalOffset.y,
            0f);
        silhouetteTransform.localRotation = Quaternion.identity;
        silhouetteTransform.localScale = new Vector3(silhouetteLocalScale, silhouetteLocalScale, 1f);
    }

    private static void ApplySilhouetteMaterial(SpriteRenderer renderer)
    {
        Material mat = GetOrCreateSilhouetteMaterial();
        if (mat != null)
            renderer.sharedMaterial = mat;
    }

    private static Material GetOrCreateSilhouetteMaterial()
    {
        if (sharedSilhouetteMaterial != null)
            return sharedSilhouetteMaterial;

        Shader shader = Shader.Find(SilhouetteShaderName);
        if (shader == null)
        {
            Debug.LogError(
                $"[PlantSelectionHighlight] Shader '{SilhouetteShaderName}' introuvable. " +
                "Vérifiez Assets/Shaders/Farm/SpriteSelectionSilhouette.shader.");
            return null;
        }

        sharedSilhouetteMaterial = new Material(shader);
        return sharedSilhouetteMaterial;
    }

    private void SyncSilhouetteFromPlant()
    {
        if (plantRenderer == null || silhouetteRenderer == null)
            return;

        silhouetteRenderer.sprite = plantRenderer.sprite;
        silhouetteRenderer.flipX = plantRenderer.flipX;
        silhouetteRenderer.flipY = plantRenderer.flipY;
        silhouetteRenderer.sortingLayerID = plantRenderer.sortingLayerID;
        silhouetteRenderer.sortingOrder = plantRenderer.sortingOrder - sortingOrderBehindPlant;
        silhouetteRenderer.color = silhouetteColor;
        ApplySilhouetteMaterial(silhouetteRenderer);
        ConfigureSilhouetteTransform(silhouetteRenderer.transform);
    }
}
