using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controleur de l'ecran shop configure dans l'editeur.
/// Le layout visuel doit etre prepare dans le prefab (pas de generation d'ecran en code).
/// </summary>
public class RuntimeShopScreen : MonoBehaviour
{
    private const int PriceSummaryMaxChars = 160;
    private const string CurrencyNotConfiguredMessage = "Monnaie non configuree.";

    [Header("Bindings UI (prefab)")]
    [SerializeField] private RectTransform slotsContainer;
    [SerializeField] private GridLayoutGroup slotsGridLayout;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text fallbackListText;
    [SerializeField] private Text stateLabel;
    [SerializeField] private Image rootBackdropImage;
    [SerializeField] private Image contentBackdropImage;

    [Header("Empty catalogue (Bezy)")]
    [Tooltip("Panel placeholder quand le catalogue n'a aucune offre. Optionnel.")]
    [SerializeField] private GameObject emptyCataloguePanel;

    [Header("Layout slots")]
    [SerializeField] private Vector2 slotCellSize = new(112f, 112f);
    [SerializeField] private Vector2 slotSpacing = new(14f, 14f);

    [Header("Popup achat item (optionnel)")]
    [Tooltip("Identifiant du popup a demander au ScreenPopupHost de l'ecran.")]
    [SerializeField] private string shopItemPopupId = PopupId.ShopItemPurchase;

    [Tooltip("Identifiant du popup feedback ressources (monnaie, inventaire plein, etc.).")]
    [SerializeField] private string shopResourceFeedbackPopupId = PopupId.ShopResourceFeedback;

    [Header("Source de donnees shop")]
    [Tooltip("Si assigne, ce catalogue ScriptableObject est utilise en priorite. Sinon fallback JSON prototype.")]
    [SerializeField] private ShopCatalogDefinition shopCatalogDefinition;

    private ItemDatabase itemDatabase;
    private InventorySlotUI slotPrefab;
    private int columnCount = 5;
    private readonly List<InventorySlotUI> slotViews = new();

    private bool initialized;
    private Button hookedCloseButton;
    private ShopItemPopupController shopItemPopupInstance;
    private ResourceFeedbackPopupUI resourceFeedbackPopupInstance;
    private ScreenPopupHost screenPopupHost;
    private bool shopPurchaseHandlerWired;
    private bool inventoryEventsSubscribed;

    /// <summary>Catalogue affiche (dernier Refresh). Sert au clic sur les slots.</summary>
    private List<MarketCatalogPrototype.ListingRow> lastListings;

    /// <summary>Definition SO associee a chaque ligne affichee (meme index que lastListings).</summary>
    private List<ShopItemDefinition> lastShopDefinitions;

    /// <summary>Injection depuis <see cref="UIManager"/>.</summary>
    public void Initialize(
        ItemDatabase database,
        InventorySlotUI slotPrefabOverride,
        int columns)
    {
        itemDatabase = database;
        slotPrefab = slotPrefabOverride;
        columnCount = Mathf.Max(1, columns);
        initialized = true;

        ResolveBindingsIfNeeded();
        HookCloseButton();
        SubscribeInventoryEvents();
        Refresh();
    }

    private void Awake()
    {
        ResolveBindingsIfNeeded();
        HookCloseButton();
    }

    private void OnEnable()
    {
        ResolveBindingsIfNeeded();
        HookCloseButton();
        SubscribeInventoryEvents();
        if (initialized)
            Refresh();
    }

    private void OnDisable() => UnsubscribeInventoryEvents();

    private void OnDestroy()
    {
        UnsubscribeInventoryEvents();
        UnhookCloseButton();

        if (shopItemPopupInstance != null && shopPurchaseHandlerWired)
        {
            shopItemPopupInstance.PurchaseRequested -= HandleShopPurchaseRequested;
            shopPurchaseHandlerWired = false;
        }
    }

    public void Refresh()
    {
        SubscribeInventoryEvents();
        ResolveBindingsIfNeeded();

        if (!initialized)
            return;

        if (slotsContainer == null)
        {
            SetFooterLines(
                "Erreur: slotsContainer non configure",
                "Configure RuntimeShopScreen dans le prefab ShopScreen.");
            SetEmptyCatalogueVisible(true);
            return;
        }

        ApplyLayoutAndBackdrop();

        if (itemDatabase == null)
        {
            TryResolveDatabaseFromPlayer();
            if (itemDatabase == null)
            {
                SetFooterLines(
                    "Erreur : ItemDatabase indisponible",
                    "Assignez ItemDatabase sur PlayerInventory (NavigationHUD).");
                ShowFallbackText("ItemDatabase manquant - impossible de resoudre le JSON marche.");
                ClearSlotViews();
                SetEmptyCatalogueVisible(true);
                return;
            }
        }

        if (!TryResolveListings(out List<MarketCatalogPrototype.ListingRow> listings, out string loadError))
        {
            lastListings = null;
            lastShopDefinitions = null;
            SetFooterLines("Erreur catalogue JSON", loadError ?? string.Empty);
            ShowFallbackText(loadError ?? string.Empty);
            ClearSlotViews();
            SetEmptyCatalogueVisible(true);
            return;
        }

        if (fallbackListText != null)
            fallbackListText.gameObject.SetActive(false);

        if (slotPrefab == null)
        {
            lastListings = null;
            lastShopDefinitions = null;
            ShowFallbackText(BuildCatalogFallbackText(listings));
            SetFooterLines(
                $"Prototype JSON - {listings.Count} offre(s) (vue texte)",
                BuildPriceSummaryLine(listings));
            ClearSlotViews();
            SetEmptyCatalogueVisible(listings == null || listings.Count == 0);
            return;
        }

        lastListings = listings;
        EnsureSlotViews(listings.Count);

        for (int i = 0; i < slotViews.Count; i++)
        {
            MarketCatalogPrototype.ListingRow row = listings[i];
            slotViews[i].Refresh(row.Slot);
            int capturedIndex = i;
            slotViews[i].SetClickHandler(() => OnShopSlotClicked(capturedIndex));
        }

        SetEmptyCatalogueVisible(listings.Count == 0);
        SetFooterLines(
            $"Market (prototype JSON) - {listings.Count} offre(s)",
            BuildPriceSummaryLine(listings));
    }

    public void Close() => HideScreen();

    private bool TryResolveListings(out List<MarketCatalogPrototype.ListingRow> listings, out string errorMessage)
    {
        bool ok = ShopCatalogResolver.TryResolve(
            shopCatalogDefinition,
            itemDatabase,
            out listings,
            out List<ShopItemDefinition> definitions,
            out errorMessage);

        lastShopDefinitions = definitions;
        return ok;
    }

    private void TryResolveDatabaseFromPlayer()
    {
        if (PlayerInventory.Instance != null)
            itemDatabase = PlayerInventory.Instance.ItemDatabase;
    }

    private void ClearSlotViews()
    {
        lastListings = null;
        lastShopDefinitions = null;
        EnsureSlotViews(0);
    }

    private void OnShopSlotClicked(int index)
    {
        if (lastListings == null || index < 0 || index >= lastListings.Count)
            return;

        MarketCatalogPrototype.ListingRow row = lastListings[index];
        if (row.Slot == null || row.Slot.IsEmpty || row.Slot.Item == null)
            return;

        ShopItemPopupController popup = ResolveShopItemPopup();
        if (popup == null)
        {
            Debug.LogWarning(
                "[RuntimeShopScreen] ShopItemPopup introuvable. " +
                "Configurez un ScreenPopupBinding avec ce screenId et PopupId.ShopItemPurchase.");
            return;
        }

        EnsureShopPurchaseWired(popup);

        ItemDefinition item = row.Slot.Item;
        ShopItemDefinition shopDefinition = GetShopDefinitionByIndex(index);
        int maxPurchasableInListing = Mathf.Max(1, row.Slot.Quantity);
        int minQuantity = shopDefinition != null ? shopDefinition.MinPurchaseQuantity : 1;
        int maxQuantity = shopDefinition != null
            ? Mathf.Min(shopDefinition.MaxPurchaseQuantity, maxPurchasableInListing)
            : Mathf.Min(Mathf.Max(1, item.MaxStack), maxPurchasableInListing);
        maxQuantity = Mathf.Max(minQuantity, maxQuantity);

        var data = new ShopItemPopupData(
            item.ItemId,
            item.DisplayName,
            shopDefinition != null ? shopDefinition.RarityLabel : string.Empty,
            shopDefinition != null ? shopDefinition.Description : string.Empty,
            item.Icon,
            shopDefinition != null ? shopDefinition.UnitPrice : row.UnitPrice,
            minQuantity,
            maxQuantity);

        if (!popup.gameObject.activeSelf)
            popup.gameObject.SetActive(true);

        popup.transform.SetAsLastSibling();
        popup.Open(data);
    }

    private ShopItemDefinition GetShopDefinitionByIndex(int index)
    {
        if (lastShopDefinitions == null || index < 0 || index >= lastShopDefinitions.Count)
            return null;

        return lastShopDefinitions[index];
    }

    private ShopItemPopupController ResolveShopItemPopup()
    {
        if (shopItemPopupInstance != null)
            return shopItemPopupInstance;

        ScreenPopupHost popupHost = ResolvePopupHost();
        if (popupHost != null &&
            popupHost.HasPopup(shopItemPopupId) &&
            popupHost.TryGetPopup(shopItemPopupId, out ShopItemPopupController popupFromHost))
        {
            shopItemPopupInstance = popupFromHost;
            return shopItemPopupInstance;
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

    private void EnsureShopPurchaseWired(ShopItemPopupController popup)
    {
        if (popup == null || shopPurchaseHandlerWired)
            return;

        popup.PurchaseRequested += HandleShopPurchaseRequested;
        shopPurchaseHandlerWired = true;
    }

    private void HandleShopPurchaseRequested(ShopItemPopupData data, int quantity, int totalPrice)
    {
        if (data == null)
            return;

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null)
        {
            Debug.LogWarning("[RuntimeShopScreen] PlayerInventory.Instance introuvable - achat annule.");
            return;
        }

        if (itemDatabase == null)
            TryResolveDatabaseFromPlayer();

        if (itemDatabase == null)
        {
            Debug.LogWarning("[RuntimeShopScreen] ItemDatabase indisponible - achat annule.");
            return;
        }

        ItemDefinition item = itemDatabase.GetById(data.ItemId);
        if (item == null)
        {
            Debug.LogWarning($"[RuntimeShopScreen] itemId shop introuvable dans ItemDatabase: '{data.ItemId}'.");
            return;
        }

        int qty = Mathf.Max(1, quantity);
        ItemDefinition currency = itemDatabase.PrimaryCurrency;

        if (totalPrice > 0 && currency == null)
        {
            Debug.LogWarning("[RuntimeShopScreen] ItemDatabase.PrimaryCurrency non assigne - impossible de payer.");
            ShowFeedbackMessage(CurrencyNotConfiguredMessage);
            return;
        }

        if (totalPrice > 0 && !InventoryCurrencyAccount.HasSufficientFunds(inventory, currency, totalPrice))
        {
            ShowInsufficientResourcesFeedback();
            return;
        }

        if (!inventory.CanFitQuantity(item, qty))
        {
            ShowFeedbackMessage(UiMessages.InventoryFull);
            return;
        }

        if (!InventoryCurrencyAccount.TryPurchase(inventory, currency, totalPrice, item, qty, out InventoryResult addResult)
            || addResult != InventoryResult.Success)
        {
            Debug.LogWarning($"[RuntimeShopScreen] Echec achat inattendu ({addResult}).");
            return;
        }

        Debug.Log($"[RuntimeShopScreen] Achat : {item.DisplayName} x{qty} pour {totalPrice} (monnaie debitee).");

        if (shopItemPopupInstance != null)
            shopItemPopupInstance.Close();

        Refresh();
    }

    private void SubscribeInventoryEvents()
    {
        if (inventoryEventsSubscribed || PlayerInventory.Instance == null)
            return;

        PlayerInventory.Instance.OnInventoryChanged += HandlePlayerInventoryChanged;
        inventoryEventsSubscribed = true;
    }

    private void UnsubscribeInventoryEvents()
    {
        if (!inventoryEventsSubscribed)
            return;

        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= HandlePlayerInventoryChanged;

        inventoryEventsSubscribed = false;
    }

    private void HandlePlayerInventoryChanged()
    {
        if (initialized)
            Refresh();
    }

    private string BuildCurrencyBalanceLine()
    {
        if (itemDatabase == null)
            TryResolveDatabaseFromPlayer();

        ItemDefinition currency = itemDatabase != null ? itemDatabase.PrimaryCurrency : null;
        if (currency == null)
            return null;

        PlayerInventory inv = PlayerInventory.Instance;
        if (inv == null)
            return $"{currency.DisplayName} : -";

        int balance = InventoryCurrencyAccount.GetBalance(inv, currency);
        return $"{currency.DisplayName} : {balance}";
    }

    private ResourceFeedbackPopupUI ResolveResourceFeedbackPopup()
    {
        if (resourceFeedbackPopupInstance != null)
            return resourceFeedbackPopupInstance;

        ScreenPopupHost host = ResolvePopupHost();
        if (host != null &&
            host.HasPopup(shopResourceFeedbackPopupId) &&
            host.TryGetPopup(shopResourceFeedbackPopupId, out ResourceFeedbackPopupUI fromHost))
        {
            resourceFeedbackPopupInstance = fromHost;
            return resourceFeedbackPopupInstance;
        }

        return null;
    }

    private void ShowInsufficientResourcesFeedback()
    {
        ResourceFeedbackPopupUI popup = ResolveResourceFeedbackPopup();
        if (popup != null)
        {
            popup.ShowInsufficientResources();
            return;
        }

        Debug.LogWarning(
            "[RuntimeShopScreen] ResourceFeedbackPopup introuvable. " +
            "Ajoutez un ScreenPopupBinding (Shop + PopupId.ShopResourceFeedback) sur UIManager.runtimePopupBindings (NavigationHUD).");
    }

    private void ShowFeedbackMessage(string message)
    {
        ResourceFeedbackPopupUI popup = ResolveResourceFeedbackPopup();
        if (popup != null)
        {
            popup.ShowMessage(message);
            return;
        }

        Debug.LogWarning(
            "[RuntimeShopScreen] ResourceFeedbackPopup introuvable - message non affiche: " + message +
            ". Ajoutez un ScreenPopupBinding (Shop + PopupId.ShopResourceFeedback) sur UIManager.runtimePopupBindings (NavigationHUD).");
    }

    private void ResolveBindingsIfNeeded()
    {
        if (slotsContainer == null && slotsGridLayout != null)
            slotsContainer = slotsGridLayout.GetComponent<RectTransform>();

        if (slotsContainer == null)
        {
            GridLayoutGroup grid = GetComponentInChildren<GridLayoutGroup>(true);
            if (grid != null)
            {
                slotsGridLayout = grid;
                slotsContainer = grid.GetComponent<RectTransform>();
            }
        }
        else if (slotsGridLayout == null)
        {
            slotsGridLayout = slotsContainer.GetComponent<GridLayoutGroup>();
        }

        if (closeButton == null)
            closeButton = FindCloseButton();

        if (rootBackdropImage == null)
            rootBackdropImage = GetComponent<Image>();

        if (contentBackdropImage == null && slotsContainer != null && slotsContainer.parent != null)
            contentBackdropImage = slotsContainer.parent.GetComponent<Image>();

        if (emptyCataloguePanel == null)
        {
            Transform empty = transform.Find("EmptyCataloguePanel");
            if (empty != null)
                emptyCataloguePanel = empty.gameObject;
        }

        if (screenPopupHost == null)
            screenPopupHost = ResolvePopupHost();
    }

    private void SetEmptyCatalogueVisible(bool visible)
    {
        if (emptyCataloguePanel == null)
            return;

        if (emptyCataloguePanel.activeSelf != visible)
            emptyCataloguePanel.SetActive(visible);
    }

    private Button FindCloseButton()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        if (buttons == null || buttons.Length == 0)
            return null;

        foreach (Button button in buttons)
        {
            if (button != null && button.gameObject.name.Contains("Close"))
                return button;
        }

        return buttons[0];
    }

    private void HookCloseButton()
    {
        if (closeButton == null || hookedCloseButton == closeButton)
            return;

        UnhookCloseButton();
        closeButton.onClick.AddListener(Close);
        hookedCloseButton = closeButton;
    }

    private void UnhookCloseButton()
    {
        if (hookedCloseButton == null)
            return;

        hookedCloseButton.onClick.RemoveListener(Close);
        hookedCloseButton = null;
    }

    private void ApplyLayoutAndBackdrop()
    {
        if (rootBackdropImage != null)
            HudModalBackdrop.ApplyRootBackground(rootBackdropImage);

        if (contentBackdropImage != null)
            HudModalBackdrop.ApplyContentPanel(contentBackdropImage);

        if (slotsGridLayout != null)
            ApplyGridLayout(slotsGridLayout);
    }

    private void ApplyGridLayout(GridLayoutGroup gridLayout)
    {
        if (gridLayout == null)
            return;

        gridLayout.cellSize = slotCellSize;
        gridLayout.spacing = slotSpacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columnCount;
    }

    private void EnsureSlotViews(int count)
    {
        while (slotViews.Count < count)
        {
            InventorySlotUI view = Instantiate(slotPrefab, slotsContainer);
            slotViews.Add(view);
        }

        while (slotViews.Count > count)
        {
            int index = slotViews.Count - 1;
            InventorySlotUI view = slotViews[index];
            slotViews.RemoveAt(index);
            if (view != null)
                Destroy(view.gameObject);
        }
    }

    private void ShowFallbackText(string content)
    {
        if (fallbackListText == null)
            return;

        fallbackListText.gameObject.SetActive(true);
        fallbackListText.text = content;
    }

    private static string BuildCatalogFallbackText(IReadOnlyList<MarketCatalogPrototype.ListingRow> listings)
    {
        if (listings == null || listings.Count == 0)
            return "Catalogue vide (verifiez le JSON ou les itemId dans ItemDatabase).";

        var sb = new StringBuilder();
        for (int i = 0; i < listings.Count; i++)
        {
            MarketCatalogPrototype.ListingRow row = listings[i];
            if (row.Slot == null || row.Slot.IsEmpty || row.Slot.Item == null)
                continue;

            sb.Append("- ");
            sb.Append(row.Slot.Item.DisplayName);
            sb.Append(" x");
            sb.Append(row.Slot.Quantity);
            sb.Append(" @ ");
            sb.Append(row.UnitPrice);
            sb.AppendLine();
        }

        return sb.Length > 0 ? sb.ToString() : "Catalogue vide.";
    }

    private static string BuildPriceSummaryLine(IReadOnlyList<MarketCatalogPrototype.ListingRow> listings)
    {
        if (listings == null || listings.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        for (int i = 0; i < listings.Count; i++)
        {
            MarketCatalogPrototype.ListingRow row = listings[i];
            if (row.Slot == null || row.Slot.IsEmpty || row.Slot.Item == null)
                continue;

            if (sb.Length > 0)
                sb.Append(" | ");

            sb.Append(row.Slot.Item.DisplayName);
            sb.Append(" x");
            sb.Append(row.Slot.Quantity);
            sb.Append(" @");
            sb.Append(row.UnitPrice);

            if (sb.Length >= PriceSummaryMaxChars)
            {
                sb.Append(" ...");
                break;
            }
        }

        return sb.ToString();
    }

    private void SetFooterLines(string lineA, string lineB)
    {
        if (stateLabel == null)
            return;

        string balanceLine = BuildCurrencyBalanceLine();
        string core = string.IsNullOrEmpty(lineB) ? lineA : $"{lineA}\n{lineB}";
        stateLabel.text = string.IsNullOrEmpty(balanceLine) ? core : $"{balanceLine}\n{core}";
    }

    private void HideScreen()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.HideScreen(ScreenId.Shop);
    }
}
