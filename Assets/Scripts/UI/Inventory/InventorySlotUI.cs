using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Binds a single InventorySlot's data to its UI elements (icon + quantity label).
/// Clic optionnel : shop, inventaire (détail / drop), etc. Sans handler, le slot ignore le clic.
/// </summary>
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityLabel;
    [SerializeField] private GameObject emptyOverlay;

    private Action clickAction;

    private void Awake()
    {
        EnsureRaycastTargetOnRoot();
    }

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
        // Currency items are displayed exclusively in the WalletWidget, not in inventory slots.
        bool isEmpty = slot == null || slot.IsEmpty
            || slot.Item.InventoryBehavior == ItemInventoryBehavior.Currency;

        BoundSlot = isEmpty ? null : slot;

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

    /// <summary>
    /// Garantit un Graphic raycastable sur la racine pour recevoir IPointerClickHandler,
    /// meme si le prefab n'a pas d'Image de fond configuree.
    /// </summary>
    private void EnsureRaycastTargetOnRoot()
    {
        Graphic rootGraphic = GetComponent<Graphic>();
        if (rootGraphic == null)
            rootGraphic = gameObject.AddComponent<Image>();

        rootGraphic.raycastTarget = true;

        if (rootGraphic is Image image)
            image.color = new Color(1f, 1f, 1f, 0f);
    }
}
