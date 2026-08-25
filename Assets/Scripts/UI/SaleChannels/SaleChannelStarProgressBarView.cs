using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barre Image Filled + label overlay (texte dans la jauge). Wiring Bezy.
/// </summary>
public class SaleChannelStarProgressBarView : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI label;

    public void Apply(SaleChannelStarProgressRow row)
    {
        if (label != null)
            label.text = row.OverlayText;

        if (fillImage == null)
            return;

        int required = Mathf.Max(0, row.Required);
        int current = Mathf.Max(0, row.Current);
        fillImage.fillAmount = required <= 0 ? 1f : Mathf.Clamp01((float)current / required);
    }
}
