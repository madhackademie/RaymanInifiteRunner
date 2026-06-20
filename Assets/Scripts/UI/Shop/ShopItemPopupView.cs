using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vue UI de la popup shop item.
/// Affichage uniquement : aucun calcul métier ni achat réel ici.
/// </summary>
public sealed class ShopItemPopupView : MonoBehaviour
{
    private const string DefaultConfirmMessage = "Confirmer l'achat ?";
    private const string DefaultSellConfirmMessage = "Confirmer la vente ?";

    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Item")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Quantity / Price")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_InputField quantityInputField;
    [SerializeField] private TMP_Text confirmButtonText;

    [Header("Buttons")]
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button maxButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [Header("Wallet (solde monnaie)")]
    [Tooltip("Affiche le solde PrimaryCurrency pendant l'achat. Utilise CurrencyBalanceUI.")]
    [SerializeField] private CurrencyBalanceUI walletBalance;

    [SerializeField] private GameObject walletRoot;

    [Header("Confirmation overlay (optionnel)")]
    [SerializeField] private GameObject confirmOverlayRoot;
    [SerializeField] private TMP_Text confirmMessageText;
    [SerializeField] private TMP_Text confirmTotalText;
    [SerializeField] private Button confirmPurchaseButton;
    [SerializeField] private Button confirmCancelButton;

    private bool suppressQuantityInputEvent;

    public Action OnPlusClicked;
    public Action OnMinusClicked;
    public Action OnMaxClicked;
    public Action OnConfirmClicked;
    public Action OnCloseClicked;
    public Action<string> OnQuantityInputSubmitted;
    public Action OnConfirmPurchaseClicked;
    public Action OnConfirmCancelClicked;

    public bool HasConfirmOverlay => confirmOverlayRoot != null;

    private void Awake()
    {
        ResolveWalletReferences();
        BindButtons();
        BindQuantityInput();
    }

    private void OnDestroy()
    {
        UnbindButtons();
        UnbindQuantityInput();
    }

    public void SetItemVisuals(ShopItemPopupData data)
    {
        if (data == null)
            return;

        if (itemIcon != null)
            itemIcon.sprite = data.Icon;

        if (itemNameText != null)
            itemNameText.text = data.DisplayName;

        if (rarityText != null)
            rarityText.text = data.RarityLabel;

        if (descriptionText != null)
            descriptionText.text = data.Description;
    }

    public void SetQuantity(int quantity)
    {
        int value = Mathf.Max(0, quantity);

        if (quantityText != null)
            quantityText.text = $"x{value}";

        if (quantityInputField != null)
        {
            suppressQuantityInputEvent = true;
            quantityInputField.SetTextWithoutNotify(value.ToString());
            suppressQuantityInputEvent = false;
        }
    }

    public void SetTotalPrice(int totalPrice, ShopItemPopupFlowMode flowMode = ShopItemPopupFlowMode.Purchase)
    {
        if (confirmButtonText == null)
            return;

        int value = Mathf.Max(0, totalPrice);
        confirmButtonText.text = flowMode == ShopItemPopupFlowMode.Sell
            ? $"Vendre {value}"
            : $"Acheter {value}";
    }

    public void SetConfirmMessageForFlow(ShopItemPopupFlowMode flowMode)
    {
        if (confirmMessageText == null)
            return;

        confirmMessageText.text = flowMode == ShopItemPopupFlowMode.Sell
            ? DefaultSellConfirmMessage
            : DefaultConfirmMessage;
    }

    public void SetConfirmButtonLabel(string label)
    {
        if (confirmButtonText != null)
            confirmButtonText.text = label;
    }

    public void SetConfirmInteractable(bool interactable)
    {
        if (confirmButton != null)
            confirmButton.interactable = interactable;

        if (maxButton != null)
            maxButton.interactable = interactable;
    }

    public void ShowConfirmOverlay(int totalPrice, ShopItemPopupFlowMode flowMode = ShopItemPopupFlowMode.Purchase)
    {
        if (confirmOverlayRoot == null)
            return;

        SetConfirmMessageForFlow(flowMode);

        if (confirmTotalText != null)
            confirmTotalText.text = $"Total : {Mathf.Max(0, totalPrice)}";

        RefreshWallet();
        confirmOverlayRoot.SetActive(true);
        confirmOverlayRoot.transform.SetAsLastSibling();
    }

    public void HideConfirmOverlay()
    {
        if (confirmOverlayRoot != null)
            confirmOverlayRoot.SetActive(false);
    }

    public void RefreshWallet()
    {
        ResolveWalletReferences();

        if (walletRoot != null)
            walletRoot.SetActive(true);

        walletBalance?.Refresh();
    }

    public void Show()
    {
        HideConfirmOverlay();
        RefreshWallet();

        if (root != null)
        {
            root.SetActive(true);
            return;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        HideConfirmOverlay();

        if (root != null)
        {
            root.SetActive(false);
            return;
        }

        gameObject.SetActive(false);
    }

    private void BindButtons()
    {
        if (plusButton != null)
            plusButton.onClick.AddListener(HandlePlusClicked);

        if (minusButton != null)
            minusButton.onClick.AddListener(HandleMinusClicked);

        if (maxButton != null)
            maxButton.onClick.AddListener(HandleMaxClicked);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(HandleConfirmClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(HandleCloseClicked);

        if (confirmPurchaseButton != null)
            confirmPurchaseButton.onClick.AddListener(HandleConfirmPurchaseClicked);

        if (confirmCancelButton != null)
            confirmCancelButton.onClick.AddListener(HandleConfirmCancelClicked);
    }

    private void UnbindButtons()
    {
        if (plusButton != null)
            plusButton.onClick.RemoveListener(HandlePlusClicked);

        if (minusButton != null)
            minusButton.onClick.RemoveListener(HandleMinusClicked);

        if (maxButton != null)
            maxButton.onClick.RemoveListener(HandleMaxClicked);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(HandleConfirmClicked);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(HandleCloseClicked);

        if (confirmPurchaseButton != null)
            confirmPurchaseButton.onClick.RemoveListener(HandleConfirmPurchaseClicked);

        if (confirmCancelButton != null)
            confirmCancelButton.onClick.RemoveListener(HandleConfirmCancelClicked);
    }

    private void BindQuantityInput()
    {
        if (quantityInputField == null)
            return;

        quantityInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        quantityInputField.onEndEdit.AddListener(HandleQuantityInputEndEdit);
    }

    private void UnbindQuantityInput()
    {
        if (quantityInputField == null)
            return;

        quantityInputField.onEndEdit.RemoveListener(HandleQuantityInputEndEdit);
    }

    private void HandleQuantityInputEndEdit(string text)
    {
        if (suppressQuantityInputEvent)
            return;

        OnQuantityInputSubmitted?.Invoke(text);
    }

    private void HandlePlusClicked() => OnPlusClicked?.Invoke();
    private void HandleMinusClicked() => OnMinusClicked?.Invoke();
    private void HandleMaxClicked() => OnMaxClicked?.Invoke();
    private void HandleConfirmClicked() => OnConfirmClicked?.Invoke();
    private void HandleCloseClicked() => OnCloseClicked?.Invoke();
    private void HandleConfirmPurchaseClicked() => OnConfirmPurchaseClicked?.Invoke();
    private void HandleConfirmCancelClicked() => OnConfirmCancelClicked?.Invoke();

    private void ResolveWalletReferences()
    {
        if (walletBalance == null)
            walletBalance = GetComponentInChildren<CurrencyBalanceUI>(true);

        if (walletRoot == null && walletBalance != null)
            walletRoot = walletBalance.gameObject;

        if (walletBalance == null)
            return;

        if (walletBalance.CurrencyItem != null)
            return;

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory?.ItemDatabase?.PrimaryCurrency != null)
            walletBalance.SetCurrencyItem(inventory.ItemDatabase.PrimaryCurrency);
    }
}
