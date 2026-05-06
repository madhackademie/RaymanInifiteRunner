using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Binds a single InventorySlot's data to its UI elements (icon + quantity label).
/// Clic optionnel (shop, etc.) : par défaut aucun handler, l'inventaire reste passif.
/// </summary>
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityLabel;
    [SerializeField] private GameObject emptyOverlay;

    private Action clickAction;

    /// <summary>Dernier slot affiché (référence partagée avec le modèle appelant).</summary>
    public InventorySlot BoundSlot { get; private set; }

    /// <summary>
    /// Enregistre un callback au clic (bouton gauche). Passer null pour désactiver.
    /// Le slot vide ignore le clic même si un handler est défini.
    /// </summary>
    public void SetClickHandler(Action handler)
    {
        clickAction = handler;
    }

    /// <summary>Refreshes the slot visuals from the given data model.</summary>
    public void Refresh(InventorySlot slot)
    {
        BoundSlot = slot;
        bool isEmpty = slot == null || slot.IsEmpty;
        
        if (!isEmpty)
            Debug.Log($"[InventorySlotUI] '{slot.Item.DisplayName}' x{slot.Quantity} — icon={(slot.Item.Icon != null ? slot.Item.Icon.name : "NULL")}");

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (clickAction == null || BoundSlot == null || BoundSlot.IsEmpty)
            return;

        clickAction.Invoke();
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

        bool hasQuantity = quantity > 0;
        quantityLabel.enabled = hasQuantity;
        quantityLabel.color = Color.white;
        quantityLabel.text = hasQuantity ? quantity.ToString() : string.Empty;
    }
}
