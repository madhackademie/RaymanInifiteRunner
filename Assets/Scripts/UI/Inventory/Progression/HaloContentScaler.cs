using UnityEngine;

/// <summary>
/// Met à l'échelle le contenu du halo (cercle de slots, portrait, labels) selon la
/// hauteur courante de PlayerHaloPanel. À placer sur PlayerHaloPanel.
/// Aucun reparentage requis : on scale le localScale des RectTransform ciblés.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class HaloContentScaler : MonoBehaviour
{
    private const float MinValidScale = 0.0001f;

    [Header("Cibles à mettre à l'échelle")]
    [Tooltip("RectTransform enfants du halo (HaloSlots, PortraitFrame, LevelLabel).")]
    [SerializeField] private RectTransform[] targets = new RectTransform[0];

    [Header("Référence")]
    [Tooltip("Hauteur de design du panel pour laquelle l'échelle vaut 1.")]
    [SerializeField] private float referenceHeight = 340f;

    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 3f;

    private RectTransform selfRect;

    private void Awake()
    {
        CacheSelf();
        ApplyScale();
    }

    private void OnEnable()
    {
        ApplyScale();
    }

    /// <summary>Callback Unity : appelé quand la taille du RectTransform change (layout parent).</summary>
    private void OnRectTransformDimensionsChange()
    {
        CacheSelf();
        ApplyScale();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CacheSelf();
        ApplyScale();
    }
#endif

    /// <summary>Recalcule et applique l'échelle (utilisable depuis l'éditeur ou un autre script).</summary>
    public void Refresh() => ApplyScale();

    private void CacheSelf()
    {
        if (selfRect == null)
            selfRect = (RectTransform)transform;
    }

    private void ApplyScale()
    {
        if (selfRect == null || referenceHeight <= 0f || targets == null)
            return;

        float ratio = selfRect.rect.height / referenceHeight;
        float scale = Mathf.Clamp(ratio, minScale, maxScale);
        if (scale < MinValidScale)
            return;

        Vector3 uniformScale = new Vector3(scale, scale, 1f);
        foreach (RectTransform target in targets)
        {
            if (target != null)
                target.localScale = uniformScale;
        }
    }
}
