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
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Item")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Quantity / Price")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text confirmButtonText;

    [Header("Buttons")]
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    public Action OnPlusClicked;
    public Action OnMinusClicked;
    public Action OnConfirmClicked;
    public Action OnCloseClicked;

    private void Awake()
    {
        BindButtons();
    }

    private void OnDestroy()
    {
        UnbindButtons();
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
        if (quantityText != null)
            quantityText.text = $"x{Mathf.Max(0, quantity)}";
    }

    public void SetTotalPrice(int totalPrice)
    {
        if (confirmButtonText != null)
            confirmButtonText.text = $"Acheter {Mathf.Max(0, totalPrice)} €";
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
    }

    public void Show()
    {
        if (root != null)
        {
            root.SetActive(true);
            return;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
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

        if (confirmButton != null)
            confirmButton.onClick.AddListener(HandleConfirmClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(HandleCloseClicked);
    }

    private void UnbindButtons()
    {
        if (plusButton != null)
            plusButton.onClick.RemoveListener(HandlePlusClicked);

        if (minusButton != null)
            minusButton.onClick.RemoveListener(HandleMinusClicked);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(HandleConfirmClicked);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(HandleCloseClicked);
    }

    private void HandlePlusClicked() => OnPlusClicked?.Invoke();
    private void HandleMinusClicked() => OnMinusClicked?.Invoke();
    private void HandleConfirmClicked() => OnConfirmClicked?.Invoke();
    private void HandleCloseClicked() => OnCloseClicked?.Invoke();
}
