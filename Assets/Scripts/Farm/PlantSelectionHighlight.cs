using UnityEngine;

/// <summary>
/// Halo silhouette (contour) quand la plante est sélectionnée pour récolte / info.
/// Visuel : prefab Bezy <c>SelectionGlow</c> ; sinon création runtime minimale pour dev.
/// </summary>
[DisallowMultipleComponent]
public class PlantSelectionHighlight : MonoBehaviour
{
    private const float GlowScale = 1.08f;
    private static readonly Color GlowTint = new(1f, 1f, 1f, 0.65f);
    private const int SortingOrderBehindPlant = -1;

    [SerializeField] private SpriteRenderer plantRenderer;
    [SerializeField] private GameObject glowRoot;
    [SerializeField] private SpriteRenderer[] glowRenderers;

    private bool isHighlightActive;
    private bool warnedMissingGlow;

    private void Awake()
    {
        ResolveReferences();
    }

    /// <summary>Active ou désactive le contour (sync sprite stade courant si activé).</summary>
    public void SetHighlightActive(bool active)
    {
        if (active)
            ResolveReferences();

        isHighlightActive = active;

        if (!EnsureGlowReady())
        {
            if (active && !warnedMissingGlow)
            {
                warnedMissingGlow = true;
                Debug.LogWarning(
                    "[PlantSelectionHighlight] Glow non câblé — lancer Bezy " +
                    "[BZ-FARM-PLANT-SELECT-GLOW-001] sur LaitueObj. Socle vert footprint seul.",
                    this);
            }
            return;
        }

        if (active)
            SyncGlowFromPlant();

        if (glowRoot != null)
            glowRoot.SetActive(active);
    }

    /// <summary>Appelé par <see cref="PlantGrow"/> si le stade change pendant la sélection.</summary>
    public void RefreshIfActive()
    {
        if (!isHighlightActive)
            return;

        SyncGlowFromPlant();
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

        if (glowRenderers == null || glowRenderers.Length == 0)
        {
            if (glowRoot != null)
                glowRenderers = glowRoot.GetComponentsInChildren<SpriteRenderer>(true);
        }
    }

    private bool EnsureGlowReady()
    {
        if (glowRoot != null && glowRenderers != null && glowRenderers.Length > 0)
            return true;

        if (plantRenderer == null)
            return false;

        CreateRuntimeGlowFallback();
        return glowRoot != null && glowRenderers != null && glowRenderers.Length > 0;
    }

    private void CreateRuntimeGlowFallback()
    {
        glowRoot = new GameObject("SelectionGlow");
        glowRoot.transform.SetParent(transform, false);
        glowRoot.transform.localPosition = Vector3.zero;
        glowRoot.transform.localRotation = Quaternion.identity;
        glowRoot.transform.localScale = Vector3.one;

        var glowSpriteGo = new GameObject("GlowSprite");
        glowSpriteGo.transform.SetParent(glowRoot.transform, false);
        glowSpriteGo.transform.localScale = new Vector3(GlowScale, GlowScale, 1f);

        var glowSr = glowSpriteGo.AddComponent<SpriteRenderer>();
        glowSr.sortingLayerID = plantRenderer.sortingLayerID;
        glowSr.sortingOrder = plantRenderer.sortingOrder + SortingOrderBehindPlant;
        glowSr.color = GlowTint;

        glowRenderers = new[] { glowSr };
        glowRoot.SetActive(false);
    }

    private void SyncGlowFromPlant()
    {
        if (plantRenderer == null || glowRenderers == null)
            return;

        Sprite sprite = plantRenderer.sprite;
        for (int i = 0; i < glowRenderers.Length; i++)
        {
            if (glowRenderers[i] == null)
                continue;

            glowRenderers[i].sprite = sprite;
            glowRenderers[i].flipX = plantRenderer.flipX;
            glowRenderers[i].flipY = plantRenderer.flipY;
        }
    }
}
