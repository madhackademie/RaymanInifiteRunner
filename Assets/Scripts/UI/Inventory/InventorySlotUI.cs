using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Binds a single InventorySlot's data to its UI elements (icon + quantity label).
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityLabel;
    [SerializeField] private GameObject emptyOverlay;

    /// <summary>Refreshes the slot visuals from the given data model.</summary>
    public void Refresh(InventorySlot slot)
    {
        bool isEmpty = slot == null || slot.IsEmpty;

        if (emptyOverlay != null)
            emptyOverlay.SetActive(isEmpty);

        if (isEmpty)
        {
            SetIconVisible(false, null);
            SetQuantityLabel(0);
            return;
        }

        SetIconVisible(true, slot.Item.Icon);
        SetQuantityLabel(slot.Quantity);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetIconVisible(bool visible, Sprite sprite)
    {
        if (iconImage == null)
            return;

        iconImage.enabled = visible;
        iconImage.sprite  = sprite;
    }

    private void SetQuantityLabel(int quantity)
    {
        if (quantityLabel == null)
            return;

        quantityLabel.enabled = quantity > 1;
        quantityLabel.text    = quantity > 1 ? quantity.ToString() : string.Empty;
    }
}
