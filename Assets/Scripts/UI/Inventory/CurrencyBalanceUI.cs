using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the balance of a specific <see cref="ItemDefinition"/> with <see cref="ItemInventoryBehavior.Currency"/>.
/// Refreshes automatically on every <see cref="PlayerInventory.OnInventoryChanged"/> event.
/// </summary>
public class CurrencyBalanceUI : MonoBehaviour
{
    [Header("Item")]
    [Tooltip("Currency item to display. Must have InventoryBehavior = Currency.")]
    [SerializeField] private ItemDefinition currencyItem;

    [Header("Affichage")]
    [Tooltip("Label TMP affichant le montant.")]
    [SerializeField] private TextMeshProUGUI amountLabel;

    [Tooltip("Optionnel : icône de la devise. Si assignée, prend le sprite du currencyItem.")]
    [SerializeField] private Image iconImage;

    [Tooltip("Format C# du montant. {0} = solde courant.")]
    [SerializeField] private string amountFormat = "{0}";

    private bool subscribed;

    private void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    private void OnDisable() => Unsubscribe();

    private void Start()
    {
        // Auto-resolve child references if not set via Inspector or SetCurrencyItem.
        if (amountLabel == null)
            amountLabel = GetComponentInChildren<TextMeshProUGUI>(true);

        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>(true);

        Subscribe();
        Refresh();
    }

    private void Subscribe()
    {
        if (subscribed || PlayerInventory.Instance == null)
            return;

        PlayerInventory.Instance.OnInventoryChanged += Refresh;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
            return;

        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= Refresh;

        subscribed = false;
    }

    /// <summary>
    /// Lightweight injection overload: sets only the currency item.
    /// Icon and label are resolved automatically from child components in Start().
    /// </summary>
    public void SetCurrencyItem(ItemDefinition item) => currencyItem = item;

    /// <summary>Currency item currently tracked by this widget.</summary>
    public ItemDefinition CurrencyItem => currencyItem;

    /// <summary>Updates the icon and balance label from the current inventory state.</summary>
    public void Refresh()
    {
        if (currencyItem == null)
        {
            Debug.LogWarning("[CurrencyBalanceUI] currencyItem n'est pas assigné.", this);
            return;
        }

        if (iconImage != null && currencyItem.Icon != null)
            iconImage.sprite = currencyItem.Icon;

        if (amountLabel == null)
            return;

        PlayerInventory inventory = PlayerInventory.Instance;
        int balance = inventory != null
            ? InventoryCurrencyAccount.GetBalance(inventory, currencyItem)
            : 0;

        amountLabel.text = string.Format(amountFormat, balance);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (currencyItem != null && currencyItem.InventoryBehavior != ItemInventoryBehavior.Currency)
            Debug.LogWarning(
                $"[CurrencyBalanceUI] « {currencyItem.name} » n'a pas InventoryBehavior = Currency.", this);
    }
#endif
}
