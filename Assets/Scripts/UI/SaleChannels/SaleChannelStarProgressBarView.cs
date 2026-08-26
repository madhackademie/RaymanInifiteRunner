using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barre Track + Fill + label overlay. Progression via ancres du Fill (fiable sans sprite UI).
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

        float ratio = row.Required <= 0
            ? 1f
            : Mathf.Clamp01((float)row.Current / row.Required);

        fillImage.type = Image.Type.Simple;

        RectTransform fillRect = fillImage.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(ratio, 1f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.sizeDelta = Vector2.zero;
    }
}
