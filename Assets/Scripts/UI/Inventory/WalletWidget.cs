using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fixed bottom-right wallet widget inside the inventory screen.
/// Collapsed state: primary currency icon + balance + toggle button (≡).
/// Expanded state: panel above showing all other Currency items from the inventory.
/// </summary>
public class WalletWidget : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("Primary currency shown in the collapsed row.")]
    [SerializeField] private ItemDefinition primaryCurrency;

    [Header("Collapsed row refs")]
    [SerializeField] private Image coinIcon;
    [SerializeField] private TextMeshProUGUI amountLabel;
    [SerializeField] private Button toggleButton;

    [Header("Expanded panel")]
    [SerializeField] private GameObject expandedPanel;
    [Tooltip("Prefab for each secondary currency row inside the expanded panel.")]
    [SerializeField] private WalletRowUI rowPrefab;

    private bool isExpanded;
    private bool subscribed;

    private void Awake()
    {
        if (primaryCurrency == null && PlayerInventory.Instance?.ItemDatabase?.PrimaryCurrency != null)
            primaryCurrency = PlayerInventory.Instance.ItemDatabase.PrimaryCurrency;
    }

    private readonly List<WalletRowUI> spawnedRows = new();

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        Subscribe();
        RefreshAll();
    }

    private void OnDisable() => Unsubscribe();

    private void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleExpanded);

        Subscribe();
        SetExpanded(false, animate: false);
        RefreshAll();
    }

    private void OnDestroy()
    {
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(ToggleExpanded);

        Unsubscribe();
    }

    // ── Subscription ──────────────────────────────────────────────────────────

    private void Subscribe()
    {
        if (subscribed || PlayerInventory.Instance == null)
            return;

        PlayerInventory.Instance.OnInventoryChanged += RefreshAll;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
            return;

        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= RefreshAll;

        subscribed = false;
    }

    // ── Toggle ────────────────────────────────────────────────────────────────

    private void ToggleExpanded() => SetExpanded(!isExpanded, animate: true);

    private void SetExpanded(bool expanded, bool animate)
    {
        isExpanded = expanded;

        if (expandedPanel != null)
            expandedPanel.SetActive(isExpanded);

        if (isExpanded)
            RebuildExpandedRows();
    }

    // ── Refresh ───────────────────────────────────────────────────────────────

    /// <summary>Refreshes the primary currency display and the expanded rows if visible.</summary>
    public void RefreshAll()
    {
        RefreshPrimary();

        if (isExpanded)
            RebuildExpandedRows();
    }

    private void RefreshPrimary()
    {
        if (primaryCurrency == null)
            return;

        if (coinIcon != null && primaryCurrency.Icon != null)
            coinIcon.sprite = primaryCurrency.Icon;

        if (amountLabel == null)
            return;

        PlayerInventory inventory = PlayerInventory.Instance;
        int balance = inventory != null
            ? InventoryCurrencyAccount.GetBalance(inventory, primaryCurrency)
            : 0;

        amountLabel.text = balance.ToString("N0");
    }

    private void RebuildExpandedRows()
    {
        if (expandedPanel == null || rowPrefab == null)
            return;

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null)
            return;

        // Collect all currency items that are NOT the primary currency and have a balance.
        List<(ItemDefinition item, int balance)> entries = new();

        foreach (InventorySlot slot in inventory.Slots)
        {
            if (slot.IsEmpty || slot.Item == null)
                continue;

            if (slot.Item.InventoryBehavior != ItemInventoryBehavior.Currency)
                continue;

            if (slot.Item == primaryCurrency)
                continue;

            // Merge duplicate items (multiple slots of the same currency).
            bool merged = false;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].item == slot.Item)
                {
                    entries[i] = (slot.Item, entries[i].balance + slot.Quantity);
                    merged = true;
                    break;
                }
            }

            if (!merged)
                entries.Add((slot.Item, slot.Quantity));
        }

        // Resize row pool.
        while (spawnedRows.Count < entries.Count)
        {
            WalletRowUI row = Instantiate(rowPrefab, expandedPanel.transform);
            spawnedRows.Add(row);
        }

        while (spawnedRows.Count > entries.Count)
        {
            int last = spawnedRows.Count - 1;
            Destroy(spawnedRows[last].gameObject);
            spawnedRows.RemoveAt(last);
        }

        for (int i = 0; i < entries.Count; i++)
            spawnedRows[i].Bind(entries[i].item, entries[i].balance);
    }
}
