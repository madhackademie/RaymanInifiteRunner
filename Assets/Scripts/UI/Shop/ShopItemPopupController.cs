using System;
using UnityEngine;

/// <summary>
/// Contrôleur de la popup item shop :
/// quantité (+ / − / saisie / Max), confirmation optionnelle, puis demande d'achat.
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

    private void HandleMaxClicked()
    {
        if (!HasItem())
            return;

        currentQuantity = ComputeMaxAffordableQuantity();
        Refresh();
    }

    private void HandleQuantityInputSubmitted(string rawText)
    {
        if (!HasItem())
            return;

        if (!int.TryParse(rawText, out int parsed))
            parsed = currentData.MinQuantity;

        currentQuantity = parsed;
        Refresh();
    }

    private void HandleConfirmClicked()
    {
        if (!HasItem())
            return;

        int totalPrice = ComputeTotalPrice();
        if (totalPrice <= 0 && currentData.UnitPrice > 0)
            return;

        if (view != null && view.HasConfirmOverlay)
        {
            view.ShowConfirmOverlay(totalPrice);
            return;
        }

        EmitPurchaseRequested(totalPrice);
    }

    private void HandleConfirmPurchaseClicked()
    {
        if (!HasItem())
            return;

        int totalPrice = ComputeTotalPrice();
        view?.HideConfirmOverlay();
        EmitPurchaseRequested(totalPrice);
    }

    private void HandleConfirmCancelClicked()
    {
        view?.HideConfirmOverlay();
    }

    private void HandleCloseClicked()
    {
        Close();
    }

    private void EmitPurchaseRequested(int totalPrice)
    {
        if (!HasItem())
            return;

        PurchaseRequested?.Invoke(currentData, currentQuantity, totalPrice);
    }

    private void Refresh()
    {
        if (!HasItem() || view == null)
            return;

        currentQuantity = Mathf.Clamp(currentQuantity, currentData.MinQuantity, currentData.MaxQuantity);

        int totalPrice = ComputeTotalPrice();
        view.SetQuantity(currentQuantity);
        view.SetTotalPrice(totalPrice);
        view.SetConfirmInteractable(CanConfirmPurchase(totalPrice));
        view.RefreshWallet();
    }

    private bool CanConfirmPurchase(int totalPrice)
    {
        if (!HasItem())
            return false;

        if (currentData.UnitPrice > 0 && totalPrice <= 0)
            return false;

        return currentQuantity >= currentData.MinQuantity;
    }

    private int ComputeMaxAffordableQuantity()
    {
        if (!HasItem())
            return 1;

        int cap = currentData.MaxQuantity;
        ItemDefinition item = ResolvePurchasedItem();
        PlayerInventory inventory = PlayerInventory.Instance;

        if (inventory != null && item != null)
            cap = Mathf.Min(cap, ComputeMaxInventoryFit(inventory, item, currentData.MaxQuantity));

        cap = Mathf.Min(cap, ComputeMaxByFunds(inventory));

        return Mathf.Max(currentData.MinQuantity, cap);
    }

    private int ComputeMaxByFunds(PlayerInventory inventory)
    {
        if (currentData.UnitPrice <= 0)
            return currentData.MaxQuantity;

        if (inventory == null || inventory.ItemDatabase == null)
            return currentData.MaxQuantity;

        ItemDefinition currency = inventory.ItemDatabase.PrimaryCurrency;
        if (currency == null)
            return currentData.MaxQuantity;

        int balance = InventoryCurrencyAccount.GetBalance(inventory, currency);
        if (balance <= 0)
            return currentData.MinQuantity;

        return Mathf.Max(currentData.MinQuantity, balance / currentData.UnitPrice);
    }

    private int ComputeMaxInventoryFit(PlayerInventory inventory, ItemDefinition item, int listingCap)
    {
        int max = currentData.MinQuantity;
        int upper = Mathf.Max(currentData.MinQuantity, listingCap);

        for (int quantity = currentData.MinQuantity; quantity <= upper; quantity++)
        {
            if (!inventory.CanFitQuantity(item, quantity))
                break;

            max = quantity;
        }

        return max;
    }

    private ItemDefinition ResolvePurchasedItem()
    {
        if (!HasItem())
            return null;

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null || inventory.ItemDatabase == null)
            return null;

        return inventory.ItemDatabase.GetById(currentData.ItemId);
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
        view.OnMaxClicked += HandleMaxClicked;
        view.OnQuantityInputSubmitted += HandleQuantityInputSubmitted;
        view.OnConfirmClicked += HandleConfirmClicked;
        view.OnConfirmPurchaseClicked += HandleConfirmPurchaseClicked;
        view.OnConfirmCancelClicked += HandleConfirmCancelClicked;
        view.OnCloseClicked += HandleCloseClicked;
    }

    private void UnbindViewEvents()
    {
        if (view == null)
            return;

        view.OnPlusClicked -= HandlePlusClicked;
        view.OnMinusClicked -= HandleMinusClicked;
        view.OnMaxClicked -= HandleMaxClicked;
        view.OnQuantityInputSubmitted -= HandleQuantityInputSubmitted;
        view.OnConfirmClicked -= HandleConfirmClicked;
        view.OnConfirmPurchaseClicked -= HandleConfirmPurchaseClicked;
        view.OnConfirmCancelClicked -= HandleConfirmCancelClicked;
        view.OnCloseClicked -= HandleCloseClicked;
    }
}
