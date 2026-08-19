using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Tooltip partagé pour les bandeaux verrouillés (conditions de déblocage).
/// Panneau UI créé et câblé par Bezy sur <see cref="RuntimeSaleChannelsScreen"/>.
/// </summary>
public class SaleChannelUnlockTooltipHost : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI bodyLabel;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 24f);
    [SerializeField] private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        ResolveRefsIfNeeded();
        HideImmediate();
    }

    private void OnDisable()
    {
        HideImmediate();
    }

    public void Show(SaleChannelUnlockProgressSnapshot snapshot, RectTransform anchor)
    {
        if (panelRoot == null)
            return;

        ResolveRefsIfNeeded();

        if (titleLabel != null)
            titleLabel.text = snapshot.TooltipTitle;

        if (bodyLabel != null)
            bodyLabel.text = snapshot.TooltipBody;

        panelRoot.SetActive(true);

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 1f;

        PositionNearAnchor(anchor);
    }

    public void Hide()
    {
        HideImmediate();
    }

    private void HideImmediate()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void ResolveRefsIfNeeded()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        if (panelRect == null && panelRoot != null)
            panelRect = panelRoot.transform as RectTransform;

        if (panelCanvasGroup == null && panelRoot != null)
            panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
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
