using UnityEngine;

/// <summary>
/// Vue HUD world biofiltre : rangée ★ + slots primaires + secondaires.
/// Preview Inspector uniquement — pas de logique prestige.
/// </summary>
public class BiofiltreHudView : MonoBehaviour
{
    [SerializeField] private UiStarRowView starRow;
    [SerializeField] private UiBiofiltreSlotRowView primaryRow;
    [SerializeField] private UiBiofiltreSlotRowView secondaryRow;

    [Tooltip("Parent iso Primary+Star (Bezy TopIsoLine). Null = pose séparée.")]
    [SerializeField] private RectTransform topIsoLine;

    [Header("Preview (Inspector)")]
    [SerializeField] [Min(0)] private int previewStarFilled = 1;
    [SerializeField] [Min(1)] private int previewStarVisible = UiStarRowView.PrestigeStarCapacity;

    private void Awake()
    {
        ApplyPreview();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyPreview();
    }
#endif

    public UiStarRowView StarRow => starRow;
    public UiBiofiltreSlotRowView PrimaryRow => primaryRow;
    public UiBiofiltreSlotRowView SecondaryRow => secondaryRow;
    public RectTransform TopIsoLine => topIsoLine;

    private void ApplyPreview()
    {
        if (starRow != null)
        {
            starRow.SetVisibleSlotCount(previewStarVisible);
            starRow.SetFilledCount(previewStarFilled);
        }

        if (primaryRow != null)
        {
            primaryRow.SetVisibleSlotCount(UiBiofiltreSlotRowView.PrimaryCapacity);
            primaryRow.SetAllLocked();
        }

        if (secondaryRow != null)
        {
            secondaryRow.SetVisibleSlotCount(UiBiofiltreSlotRowView.SecondaryCapacity);
            secondaryRow.SetAllLocked();
        }
    }
}
