using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Survol de la rangée d'étoiles — tooltip palier. Clic transmis au bandeau (vente / recherche).
/// À placer sur <c>Stars</c> (wiring Bezy).
/// </summary>
public class SaleChannelStarHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private SaleChannelBandeauView bandeauView;
    [SerializeField] private SaleChannelStarTooltipHost tooltipHost;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        ResolveBandeauIfNeeded();
    }

    public void ConfigureFromHierarchy(SaleChannelStarTooltipHost host)
    {
        tooltipHost = host;
        ResolveBandeauIfNeeded();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bandeauView == null || tooltipHost == null || !bandeauView.AllowsStarTooltip)
            return;

        tooltipHost.Show(bandeauView.GetStarTooltipSnapshot(), rectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipHost?.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tooltipHost?.Hide();
        bandeauView?.ForwardBandeauClick();
    }

    private void ResolveBandeauIfNeeded()
    {
        if (bandeauView == null)
            bandeauView = GetComponentInParent<SaleChannelBandeauView>();
    }
}
