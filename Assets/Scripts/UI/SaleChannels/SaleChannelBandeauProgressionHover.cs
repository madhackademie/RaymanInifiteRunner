using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Survol / clic d'un bandeau verrouillé — tooltip conditions + lancement recherche si prêt.
/// Composant à placer sur <c>LockedOverlay</c> (wiring Bezy).
/// </summary>
public class SaleChannelBandeauProgressionHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private SaleChannelBandeauView bandeauView;
    [SerializeField] private SaleChannelUnlockTooltipHost tooltipHost;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        ResolveBandeauIfNeeded();

        UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            image.raycastTarget = true;
    }

    public void ConfigureFromHierarchy(SaleChannelUnlockTooltipHost host)
    {
        tooltipHost = host;
        ResolveBandeauIfNeeded();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bandeauView == null || tooltipHost == null || !bandeauView.ShowsProgressionOverlay)
            return;

        if (!bandeauView.TryGetProgressSnapshot(out SaleChannelUnlockProgressSnapshot snapshot))
            return;

        tooltipHost.Show(snapshot, rectTransform != null ? rectTransform : bandeauView.transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipHost?.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (bandeauView == null || !bandeauView.CanStartUnlockResearch)
            return;

        bandeauView.RequestUnlockResearch();
    }

    private void ResolveBandeauIfNeeded()
    {
        if (bandeauView == null)
            bandeauView = GetComponentInParent<SaleChannelBandeauView>();
    }
}
