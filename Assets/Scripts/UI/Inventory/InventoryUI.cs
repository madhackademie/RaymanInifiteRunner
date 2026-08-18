using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the inventory panel: spawns InventorySlotUI instances and keeps them
/// in sync with the PlayerInventory via the OnInventoryChanged event.
/// Clic slot → popup détail / drop via ScreenPopupHost (mode strict).
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Popups (ScreenPopupHost)")]
    [Tooltip("Identifiant popup détail item / drop (binding Inventory).")]
    [SerializeField] private string itemDetailPopupId = PopupId.InventoryItemDetail;

    private InventoryFilterTabBar.TabId activeFilterTab = InventoryFilterTabBar.TabId.Seeds;
    private InventoryFilterTabBar boundFilterTabBar;
    private readonly List<InventorySlotUI> spawnedSlots = new();
    private bool hasWarnedAboutNestedSlotPrefab;
    private bool hasWarnedAboutLegacyViewportMask;
    private ShopItemPopupController itemPopupInstance;
    private bool dropHandlerWired;
    private ScreenPopupHost screenPopupHost;
    private int pendingDropSlotIndex = -1;

    /// <summary>True une fois que Bind() a été appelé avec un inventaire valide.</summary>
    public bool IsBound => playerInventory != null;

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= Refresh;

        UnhookFilterTabBar();
        UnhookDropHandler();
    }

    /// <summary>Lie la barre d'onglets inventaire (Bezy + Cursor).</summary>
    public void BindFilterTabBar(InventoryFilterTabBar tabBar)
    {
        UnhookFilterTabBar();

        boundFilterTabBar = tabBar;
        if (boundFilterTabBar == null)
            return;

        boundFilterTabBar.TabChanged += HandleFilterTabChanged;
        ApplyFilterTab(boundFilterTabBar.ActiveTab);
    }

    /// <summary>Applique un onglet filtre et rafraîchit la grille.</summary>
    public void ApplyFilterTab(InventoryFilterTabBar.TabId tabId)
    {
        activeFilterTab = tabId;
        Refresh();
    }

    /// <summary>Réinitialise l'onglet par défaut à l'ouverture de l'écran.</summary>
    public void ResetFilterTabToDefault()
    {
        activeFilterTab = InventoryFilterTabBar.TabId.Seeds;
        boundFilterTabBar?.SelectTab(activeFilterTab, notify: false);
        Refresh();
    }

    /// <summary>
    /// Injecte un <see cref="PlayerInventory"/> et construit les slots.
    /// </summary>
    public void Bind(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

        if (playerInventory != null && playerInventory != inventory)
            playerInventory.OnInventoryChanged -= Refresh;

        playerInventory = inventory;
        Initialise();
    }

    private void Initialise()
    {
        playerInventory.OnInventoryChanged -= Refresh;
        playerInventory.OnInventoryChanged += Refresh;
        EnsureViewportMaskCompatibility();
        BuildSlots();
    }

    /// <summary>
    /// Applique slot prefab / colonnes depuis le shell (<see cref="UIManager"/>) puis reconstruit si déjà lié.
    /// </summary>
    public void ApplyShellSlotSettings(InventorySlotUI slotPrefabOverride, int columns)
    {
        if (slotPrefabOverride != null)
            slotPrefab = slotPrefabOverride;

        if (slotsContainer != null &&
            slotsContainer.TryGetComponent(out GridLayoutGroup grid) &&
            columns > 0)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
        }

        if (playerInventory == null)
            return;

        EnsureViewportMaskCompatibility();
        BuildSlots();
    }

    // ── Slot management ───────────────────────────────────────────────────────

    private void EnsureViewportMaskCompatibility()
    {
        Transform viewportTransform = scrollRect != null
            ? scrollRect.viewport
            : slotsContainer != null ? slotsContainer.parent : null;
        if (viewportTransform == null)
            return;

        if (viewportTransform.GetComponent<RectMask2D>() == null)
            viewportTransform.gameObject.AddComponent<RectMask2D>();

        Mask legacyMask = viewportTransform.GetComponent<Mask>();
        if (legacyMask != null && legacyMask.enabled)
        {
            legacyMask.enabled = false;

            if (!hasWarnedAboutLegacyViewportMask)
            {
                Debug.LogWarning(
                    "[InventoryUI] Viewport Mask legacy detecte. Remplace par RectMask2D " +
                    "pour eviter la disparition des elements Maskable dans le ScrollRect.");
                hasWarnedAboutLegacyViewportMask = true;
            }
        }
    }

    private void BuildSlots()
    {
        foreach (InventorySlotUI slot in spawnedSlots)
            Destroy(slot.gameObject);

        spawnedSlots.Clear();

        foreach (InventorySlot _ in playerInventory.Slots)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, slotsContainer);

            // Ancienne scène Inventaire : si le slot prefab est encore encapsulé, on le recolle
            // sous le container pour rester compatible sans casser la migration.
            if (slotUI.transform.parent != slotsContainer)
            {
                Transform wrapper = slotUI.transform.parent;
                slotUI.transform.SetParent(slotsContainer, false);
                wrapper.SetParent(null);
                Destroy(wrapper.gameObject);

                if (!hasWarnedAboutNestedSlotPrefab)
                {
                    Debug.LogWarning(
                        "[InventoryUI] InventorySlotUI prefab encapsulé détecté. " +
                        "Simplifie le prefab pour éviter les coûts de ré-parentage.");
                    hasWarnedAboutNestedSlotPrefab = true;
                }
            }

            int slotIndex = spawnedSlots.Count;
            spawnedSlots.Add(slotUI);
            slotUI.SetClickHandler(() => OnSlotClicked(slotIndex));
        }

        Refresh();
    }

    /// <summary>Repopulates all slot UIs from the current inventory state.</summary>
    public void Refresh()
    {
        if (playerInventory == null)
            return;

        IReadOnlyList<InventorySlot> slots = playerInventory.Slots;

        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            InventorySlot data = i < slots.Count ? slots[i] : null;
            InventorySlot visible = IsVisibleInFilter(data, activeFilterTab) ? data : null;
            spawnedSlots[i].Refresh(visible);
        }
    }

    private static bool IsVisibleInFilter(InventorySlot slot, InventoryFilterTabBar.TabId tab)
    {
        if (slot == null || slot.IsEmpty || slot.Item == null)
            return true;

        if (slot.Item.InventoryBehavior == ItemInventoryBehavior.Currency)
            return false;

        if (tab == InventoryFilterTabBar.TabId.All)
            return true;

        ItemCategory expected = TabToCategory(tab);
        return slot.Item.Category == expected;
    }

    private static ItemCategory TabToCategory(InventoryFilterTabBar.TabId tab)
    {
        return tab switch
        {
            InventoryFilterTabBar.TabId.Seeds => ItemCategory.Seed,
            InventoryFilterTabBar.TabId.Consumables => ItemCategory.Consumable,
            InventoryFilterTabBar.TabId.Harvests => ItemCategory.Harvest,
            _ => ItemCategory.Material
        };
    }

    private void HandleFilterTabChanged(InventoryFilterTabBar.TabId tabId)
    {
        ApplyFilterTab(tabId);
    }

    private void UnhookFilterTabBar()
    {
        if (boundFilterTabBar == null)
            return;

        boundFilterTabBar.TabChanged -= HandleFilterTabChanged;
        boundFilterTabBar = null;
    }

    // ── Popup détail / drop ───────────────────────────────────────────────────

    private void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= spawnedSlots.Count)
            return;

        InventorySlot bound = spawnedSlots[slotIndex].BoundSlot;
        if (bound == null || bound.IsEmpty || bound.Item == null)
            return;

        if (bound.Item.InventoryBehavior == ItemInventoryBehavior.Currency)
            return;

        OpenDropPopup(slotIndex, bound);
    }

    private void OpenDropPopup(int slotIndex, InventorySlot stack)
    {
        if (stack == null || stack.IsEmpty || stack.Item == null)
            return;

        // Max / drop limités au stack du slot cliqué (pas au total inventaire).
        int stackQuantity = stack.Quantity;
        if (stackQuantity <= 0)
            return;

        ShopItemPopupController popup = ResolveItemPopup();
        if (popup == null)
        {
            Debug.LogWarning(
                "[InventoryUI] Popup détail inventaire introuvable. " +
                $"Ajoutez un ScreenPopupBinding ({ScreenId.Inventory} + {PopupId.InventoryItemDetail} " +
                "+ prefab ShopItemPopup) dans UIManager.runtimePopupBindings (NavigationHUD).",
                this);
            return;
        }

        EnsureDropWired(popup);
        pendingDropSlotIndex = slotIndex;

        ItemDefinition item = stack.Item;
        var data = new ShopItemPopupData(
            item.ItemId,
            item.DisplayName,
            string.Empty,
            item.Description,
            item.Icon,
            unitPrice: 0,
            minQuantity: 1,
            maxQuantity: stackQuantity);

        if (!popup.gameObject.activeSelf)
            popup.gameObject.SetActive(true);

        popup.transform.SetAsLastSibling();
        popup.Open(data, ShopItemPopupFlowMode.Drop);
    }

    private void HandleDropRequested(ShopItemPopupData data, int quantity, int _)
    {
        if (data == null || playerInventory == null || quantity <= 0)
            return;

        int slotIndex = pendingDropSlotIndex;
        pendingDropSlotIndex = -1;

        if (slotIndex < 0 || slotIndex >= playerInventory.Slots.Count)
        {
            Debug.LogWarning("[InventoryUI] Drop sans slot valide.", this);
            return;
        }

        InventorySlot slot = playerInventory.Slots[slotIndex];
        if (slot == null || slot.IsEmpty || slot.Item == null)
            return;

        if (slot.Item.InventoryBehavior == ItemInventoryBehavior.Currency)
            return;

        if (slot.Item.ItemId != data.ItemId)
        {
            Debug.LogWarning(
                $"[InventoryUI] Slot {slotIndex} ne correspond plus à '{data.ItemId}'.",
                this);
            return;
        }

        InventoryResult result = playerInventory.TryRemoveFromSlot(slotIndex, quantity);
        if (result != InventoryResult.Success && result != InventoryResult.Partial)
        {
            Debug.LogWarning(
                $"[InventoryUI] Drop impossible slot {slotIndex} x{quantity} (result={result}).",
                this);
            return;
        }

        ResolveItemPopup()?.Close();
    }

    private ShopItemPopupController ResolveItemPopup()
    {
        if (itemPopupInstance != null)
            return itemPopupInstance;

        ScreenPopupHost host = ResolvePopupHost();
        if (host != null &&
            host.HasPopup(itemDetailPopupId) &&
            host.TryGetPopup(itemDetailPopupId, out ShopItemPopupController popupFromHost))
        {
            itemPopupInstance = popupFromHost;
            return itemPopupInstance;
        }

        return null;
    }

    private ScreenPopupHost ResolvePopupHost()
    {
        if (screenPopupHost != null)
            return screenPopupHost;

        screenPopupHost = GetComponentInParent<ScreenPopupHost>(true);
        if (screenPopupHost != null)
            return screenPopupHost;

        screenPopupHost = GetComponentInChildren<ScreenPopupHost>(true);
        return screenPopupHost;
    }

    private void EnsureDropWired(ShopItemPopupController popup)
    {
        if (popup == null || dropHandlerWired)
            return;

        popup.PurchaseRequested += HandleDropRequested;
        dropHandlerWired = true;
    }

    private void UnhookDropHandler()
    {
        if (!dropHandlerWired || itemPopupInstance == null)
            return;

        itemPopupInstance.PurchaseRequested -= HandleDropRequested;
        dropHandlerWired = false;
    }
}
