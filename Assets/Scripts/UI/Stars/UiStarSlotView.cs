using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Slot étoile UI réutilisable : fond (slot) + fill (étoile active).
/// L’activation est pilotée de l’extérieur (<see cref="SetFilled"/>) — pas de logique métier ici.
/// Redimensionnement : changer le RectTransform racine ; Slot/Fill stretch parent.
/// </summary>
public class UiStarSlotView : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private bool filledOnStart;

    public bool IsFilled { get; private set; }

    private void Awake()
    {
        ApplyFilled(filledOnStart);
    }

    public void SetFilled(bool filled)
    {
        ApplyFilled(filled);
    }

    private void ApplyFilled(bool filled)
    {
        IsFilled = filled;

        if (fillImage != null)
            fillImage.enabled = filled;
    }
}
