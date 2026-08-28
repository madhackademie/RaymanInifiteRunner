using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tooltip palier étoiles (courant + suivant). Panneau dédié — ne pas fusionner avec le tooltip cadenas.
/// </summary>
public class SaleChannelStarTooltipHost : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private GameObject nextBlockRoot;
    [SerializeField] private TextMeshProUGUI currentTitleLabel;
    [SerializeField] private TextMeshProUGUI currentBodyLabel;
    [SerializeField] private TextMeshProUGUI nextTitleLabel;
    [SerializeField] private TextMeshProUGUI nextBodyLabel;
    [SerializeField] private UiStarRowView currentStarRow;
    [SerializeField] private UiStarRowView nextStarRow;
    [SerializeField] private SaleChannelStarProgressBarView salesBar;
    [SerializeField] private SaleChannelStarProgressBarView itemsBar;
    [SerializeField] private SaleChannelStarProgressBarView goldBar;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, -16f);
    [SerializeField] private float canvasEdgePadding = 12f;
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

    public void Show(SaleChannelStarTierSnapshot snapshot, RectTransform anchor)
    {
        if (panelRoot == null)
            return;

        ResolveRefsIfNeeded();
        panelRoot.SetActive(true);

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 1f;

        ApplyCopy(snapshot, applyBars: false);

        Canvas.ForceUpdateCanvases();
        RebuildBarLayouts(salesBar, itemsBar, goldBar, nextBlockRoot != null ? nextBlockRoot.transform as RectTransform : null, panelRect);

        ApplyBarProgress(snapshot);

        PositionNearAnchor(anchor);
    }

    public void Hide()
    {
        HideImmediate();
    }

    private void ApplyCopy(SaleChannelStarTierSnapshot snapshot, bool applyBars = true)
    {
        if (currentTitleLabel != null)
            currentTitleLabel.text = snapshot.CurrentTitle;
        if (currentBodyLabel != null)
            currentBodyLabel.text = snapshot.CurrentBody;
        if (nextTitleLabel != null)
            nextTitleLabel.text = snapshot.NextTitle;

        bool hasBars = salesBar != null && itemsBar != null && goldBar != null;
        if (applyBars && hasBars)
            ApplyBarProgress(snapshot);

        if (nextBodyLabel != null)
            nextBodyLabel.text = hasBars ? snapshot.RewardText : snapshot.NextBody;

        ApplyStarRows(snapshot);

        bool showNext = hasBars || !string.IsNullOrWhiteSpace(snapshot.NextBody);
        if (nextBlockRoot != null)
            nextBlockRoot.SetActive(showNext);
    }

    private void ApplyStarRows(SaleChannelStarTierSnapshot snapshot)
    {
        if (currentStarRow != null)
        {
            currentStarRow.SetVisibleSlotCount(UiStarRowView.PrestigeStarCapacity);
            currentStarRow.SetFilledCount(snapshot.FilledStarCount);
        }

        if (nextStarRow != null)
        {
            int nextFilled = Mathf.Clamp(snapshot.FilledStarCount + 1, 0, UiStarRowView.PrestigeStarCapacity);
            nextStarRow.SetVisibleSlotCount(UiStarRowView.PrestigeStarCapacity);
            nextStarRow.SetFilledCount(nextFilled);
        }
    }

    private void ApplyBarProgress(SaleChannelStarTierSnapshot snapshot)
    {
        if (salesBar != null)
            salesBar.Apply(snapshot.Sales);
        if (itemsBar != null)
            itemsBar.Apply(snapshot.Items);
        if (goldBar != null)
            goldBar.Apply(snapshot.Gold);
    }

    private static void RebuildBarLayouts(
        SaleChannelStarProgressBarView sales,
        SaleChannelStarProgressBarView items,
        SaleChannelStarProgressBarView gold,
        RectTransform nextBlock,
        RectTransform panel)
    {
        if (nextBlock != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(nextBlock);

        RebuildBarLayout(sales);
        RebuildBarLayout(items);
        RebuildBarLayout(gold);

        if (panel != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
    }

    private static void RebuildBarLayout(SaleChannelStarProgressBarView bar)
    {
        if (bar == null)
            return;

        RectTransform barRect = bar.transform as RectTransform;
        if (barRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(barRect);
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

        if (!TryGetAnchorLocalPoint(anchor, out Vector2 localPoint))
            return;

        panelRect.anchoredPosition = localPoint + screenOffset;
        ClampPanelToParent(panelRect, canvasEdgePadding);
    }

    private bool TryGetAnchorLocalPoint(RectTransform anchor, out Vector2 localPoint)
    {
        localPoint = Vector2.zero;
        RectTransform parent = panelRect.parent as RectTransform;
        Canvas canvas = anchor.GetComponentInParent<Canvas>();
        if (parent == null || canvas == null)
            return false;

        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector3[] corners = new Vector3[4];
        anchor.GetWorldCorners(corners);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(eventCamera, (corners[0] + corners[2]) * 0.5f);
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, eventCamera, out localPoint);
    }

    private static void ClampPanelToParent(RectTransform panel, float padding)
    {
        RectTransform parent = panel.parent as RectTransform;
        if (parent == null)
            return;

        Vector2 size = panel.rect.size;
        Vector2 pivot = panel.pivot;
        Rect parentRect = parent.rect;
        float pad = Mathf.Max(0f, padding);

        float minX = parentRect.xMin + pad + size.x * pivot.x;
        float maxX = parentRect.xMax - pad - size.x * (1f - pivot.x);
        float minY = parentRect.yMin + pad + size.y * pivot.y;
        float maxY = parentRect.yMax - pad - size.y * (1f - pivot.y);

        Vector2 pos = panel.anchoredPosition;
        pos.x = minX <= maxX ? Mathf.Clamp(pos.x, minX, maxX) : parentRect.center.x;
        pos.y = minY <= maxY ? Mathf.Clamp(pos.y, minY, maxY) : parentRect.center.y;
        panel.anchoredPosition = pos;
    }
}
