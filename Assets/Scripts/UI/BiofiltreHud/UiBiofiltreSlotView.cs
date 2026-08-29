using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Atome slot biofiltre : cadre (slot) + fill (équipé) + overlay cadenas.
/// Calque <see cref="UiStarSlotView"/> — piloté de l’extérieur, sans métier.
/// </summary>
public class UiBiofiltreSlotView : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image lockImage;
    [SerializeField] private BiofiltreSlotVisualState stateOnStart = BiofiltreSlotVisualState.Locked;

    public BiofiltreSlotVisualState State { get; private set; }

    private void Awake()
    {
        ApplyState(stateOnStart);
    }

    public void SetState(BiofiltreSlotVisualState state)
    {
        ApplyState(state);
    }

    public void SetEquippedSprite(Sprite sprite)
    {
        if (fillImage == null)
            return;

        fillImage.sprite = sprite;
    }

    private void ApplyState(BiofiltreSlotVisualState state)
    {
        State = state;

        bool showLock = state == BiofiltreSlotVisualState.Locked;
        bool showFill = state == BiofiltreSlotVisualState.Equipped;

        if (lockImage != null)
            lockImage.enabled = showLock;

        if (fillImage != null)
            fillImage.enabled = showFill;
    }
}
