using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Survol d'une bande colorée de la barre PA — affiche le tooltip fatigue.
/// </summary>
[RequireComponent(typeof(UnityEngine.UI.Image))]
public class ActionPointFatigueZoneHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ActionPointFatigueTier tier = ActionPointFatigueTier.Comfort;
    [SerializeField] private ActionPointFatigueTooltipHost tooltipHost;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;

        UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            image.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipHost == null || rectTransform == null)
            return;

        tooltipHost.Show(tier, rectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipHost?.Hide();
    }
}
