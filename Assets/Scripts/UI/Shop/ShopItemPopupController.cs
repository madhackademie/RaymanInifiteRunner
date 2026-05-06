using System;
using UnityEngine;

/// <summary>
/// Contrôleur de la popup item shop :
/// gère quantité, prix total et émission de la demande d'achat.
/// </summary>
public sealed class ShopItemPopupController : MonoBehaviour
{
    [SerializeField] private ShopItemPopupView view;

    private ShopItemPopupData currentData;
    private int currentQuantity;

    public event Action<ShopItemPopupData, int, int> PurchaseRequested;

    private void OnEnable()
    {
        BindViewEvents();
    }

    private void OnDisable()
    {
        UnbindViewEvents();
    }

    public void Open(ShopItemPopupData data)
    {
        if (data == null || view == null)
            return;

        currentData = data;
        currentQuantity = data.MinQuantity;

        view.SetItemVisuals(data);
        view.Show();
        Refresh();
    }

    public void Close()
    {
        currentData = null;
        currentQuantity = 0;

        if (view != null)
            view.Hide();
    }

    private void HandlePlusClicked()
    {
        if (!HasItem())
            return;

        currentQuantity = Mathf.Min(currentQuantity + 1, currentData.MaxQuantity);
        Refresh();
    }

    private void HandleMinusClicked()
    {
        if (!HasItem())
            return;

        currentQuantity = Mathf.Max(currentData.MinQuantity, currentQuantity - 1);
        Refresh();
    }

    private void HandleConfirmClicked()
    {
        if (!HasItem())
            return;

        int totalPrice = ComputeTotalPrice();
        if (totalPrice <= 0)
            return;

        PurchaseRequested?.Invoke(currentData, currentQuantity, totalPrice);
    }

    private void HandleCloseClicked()
    {
        Close();
    }

    private void Refresh()
    {
        if (!HasItem() || view == null)
            return;

        currentQuantity = Mathf.Clamp(currentQuantity, currentData.MinQuantity, currentData.MaxQuantity);

        int totalPrice = ComputeTotalPrice();
        view.SetQuantity(currentQuantity);
        view.SetTotalPrice(totalPrice);
        view.SetConfirmInteractable(totalPrice > 0);
    }

    private int ComputeTotalPrice()
    {
        if (!HasItem())
            return 0;

        return currentData.UnitPrice * currentQuantity;
    }

    private bool HasItem()
    {
        return currentData != null;
    }

    private void BindViewEvents()
    {
        if (view == null)
            return;

        view.OnPlusClicked += HandlePlusClicked;
        view.OnMinusClicked += HandleMinusClicked;
        view.OnConfirmClicked += HandleConfirmClicked;
        view.OnCloseClicked += HandleCloseClicked;
    }

    private void UnbindViewEvents()
    {
        if (view == null)
            return;

        view.OnPlusClicked -= HandlePlusClicked;
        view.OnMinusClicked -= HandleMinusClicked;
        view.OnConfirmClicked -= HandleConfirmClicked;
        view.OnCloseClicked -= HandleCloseClicked;
    }
}
