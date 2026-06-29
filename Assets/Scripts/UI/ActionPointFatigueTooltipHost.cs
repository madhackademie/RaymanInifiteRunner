using TMPro;
using UnityEngine;

/// <summary>
/// Panneau tooltip partagé pour les zones fatigue de la barre PA.
/// </summary>
public class ActionPointFatigueTooltipHost : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI bodyLabel;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 12f);

    private void Awake()
    {
        Hide();
    }

    public void Show(ActionPointFatigueTier tier, RectTransform anchor)
    {
        if (panelRoot == null)
            return;

        if (titleLabel != null)
            titleLabel.text = ActionPointFatigueUiCopy.GetZoneTooltipTitle(tier);

        if (bodyLabel != null)
            bodyLabel.text = ActionPointFatigueUiCopy.GetZoneTooltipBody(tier);

        panelRoot.SetActive(true);
        PositionNearAnchor(anchor);
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void PositionNearAnchor(RectTransform anchor)
    {
        if (panelRect == null || anchor == null)
            return;

        Canvas canvas = anchor.GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector3[] corners = new Vector3[4];
        anchor.GetWorldCorners(corners);
        Vector2 center = (corners[0] + corners[2]) * 0.5f;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelRect.parent as RectTransform,
                RectTransformUtility.WorldToScreenPoint(eventCamera, center),
                eventCamera,
                out Vector2 localPoint))
        {
            panelRect.anchoredPosition = localPoint + screenOffset;
        }
    }
}
