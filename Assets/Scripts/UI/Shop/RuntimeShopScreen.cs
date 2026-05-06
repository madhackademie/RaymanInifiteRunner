using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Écran shop prototype : grille identique à l’inventaire (<see cref="InventorySlotUI"/> + grille).
/// Le catalogue est simulé par JSON (<see cref="MarketCatalogPrototype"/>), sans achat réel pour l’instant.
/// Fond / tri : <see cref="HudModalBackdrop"/> + <see cref="UIManager"/> (comme l’inventaire HUD).
/// </summary>
public class RuntimeShopScreen : MonoBehaviour
{
    private const float HeaderHeight = 72f;
    private const float FooterHeight = 80f;
    private const int FontSize = 28;
    private const int SmallFontSize = 18;
    private const float SlotSize = 112f;
    private const float SlotSpacing = 14f;
    private const int PriceSummaryMaxChars = 160;

    private ItemDatabase itemDatabase;
    private InventorySlotUI slotPrefab;
    private int columnCount = 5;
    private RectTransform slotsContainer;
    private readonly List<InventorySlotUI> slotViews = new();
    private Text fallbackListText;
    private Text stateLabel;
    private Image rootBackdropImage;
    private Image contentBackdropImage;

    private bool initialized;

    [Header("Popup achat item (optionnel)")]
    [Tooltip("Si non assigné ici, utilise la référence injectée par UIManager ou un enfant ShopItemPopupController.")]
    [SerializeField] private ShopItemPopupController shopItemPopupPrefabOverride;

    private ShopItemPopupController shopItemPopupPrefabInjected;
    private ShopItemPopupController shopItemPopupInstance;
    private bool shopPurchaseHandlerWired;

    /// <summary>Catalogue affiché (dernier Refresh). Sert au clic sur les slots.</summary>
    private List<MarketCatalogPrototype.ListingRow> lastListings;

    /// <summary>Injection depuis <see cref="UIManager"/> : préférez appeler avant activation du GameObject.</summary>
    public void Initialize(
        ItemDatabase database,
        InventorySlotUI slotPrefabOverride,
        int columns,
        ShopItemPopupController itemPopupPrefab = null)
    {
        itemDatabase = database;
        slotPrefab = slotPrefabOverride;
        columnCount = Mathf.Max(1, columns);
        shopItemPopupPrefabInjected = itemPopupPrefab;
        BuildIfNeeded();
        initialized = true;
        Refresh();
    }

    private void OnDestroy()
    {
        if (shopItemPopupInstance != null && shopPurchaseHandlerWired)
        {
            shopItemPopupInstance.PurchaseRequested -= HandleShopPurchaseRequested;
            shopPurchaseHandlerWired = false;
        }
    }

    private void Awake()
    {
        BuildIfNeeded();
    }

    private void OnEnable()
    {
        if (initialized)
            Refresh();
    }

    public void Refresh()
    {
        if (slotsContainer == null)
            return;

        if (!initialized)
            return;

        ApplyLayoutAndBackdrop();

        if (itemDatabase == null)
        {
            TryResolveDatabaseFromPlayer();
            if (itemDatabase == null)
            {
                SetFooterLines(
                    "Erreur : ItemDatabase indisponible",
                    "Assignez ItemDatabase sur PlayerInventory (NavigationHUD).");
                ShowFallbackText("ItemDatabase manquant — impossible de résoudre le JSON marché.");
                ClearSlotViews();
                return;
            }
        }

        if (!MarketCatalogPrototype.TryLoad(itemDatabase, out List<MarketCatalogPrototype.ListingRow> listings, out string loadError))
        {
            lastListings = null;
            SetFooterLines("Erreur catalogue JSON", loadError ?? string.Empty);
            ShowFallbackText(loadError ?? string.Empty);
            ClearSlotViews();
            return;
        }

        if (fallbackListText != null)
            fallbackListText.gameObject.SetActive(false);

        if (slotPrefab == null)
        {
            lastListings = null;
            ShowFallbackText(BuildCatalogFallbackText(listings));
            SetFooterLines(
                $"Prototype JSON — {listings.Count} offre(s) (vue texte)",
                BuildPriceSummaryLine(listings));
            ClearSlotViews();
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

        SetFooterLines(
            $"Market (prototype JSON) — {listings.Count} offre(s)",
            BuildPriceSummaryLine(listings));
    }

    private void TryResolveDatabaseFromPlayer()
    {
        if (PlayerInventory.Instance != null)
            itemDatabase = PlayerInventory.Instance.ItemDatabase;
    }

    private void ClearSlotViews()
    {
        lastListings = null;
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
                "[RuntimeShopScreen] ShopItemPopup introuvable. Assignez ShopItemPopupController sur UIManager " +
                "(champ dédié) ou placez la popup en enfant de l'écran shop, ou sur ce composant (override).");
            return;
        }

        EnsureShopPurchaseWired(popup);

        ItemDefinition item = row.Slot.Item;
        var data = new ShopItemPopupData(
            item.ItemId,
            item.DisplayName,
            string.Empty,
            string.Empty,
            item.Icon,
            row.UnitPrice,
            1,
            Mathf.Max(1, item.MaxStack));

        popup.transform.SetAsLastSibling();
        popup.Open(data);
    }

    private ShopItemPopupController ResolveShopItemPopup()
    {
        if (shopItemPopupInstance != null)
            return shopItemPopupInstance;

        shopItemPopupInstance = GetComponentInChildren<ShopItemPopupController>(true);
        if (shopItemPopupInstance != null)
            return shopItemPopupInstance;

        ShopItemPopupController prefab = shopItemPopupPrefabInjected != null
            ? shopItemPopupPrefabInjected
            : shopItemPopupPrefabOverride;

        if (prefab == null)
            return null;

        shopItemPopupInstance = Instantiate(prefab, transform);
        shopItemPopupInstance.gameObject.name = "ShopItemPopup";

        if (shopItemPopupInstance.TryGetComponent<RectTransform>(out RectTransform popupRect))
        {
            popupRect.anchorMin = Vector2.zero;
            popupRect.anchorMax = Vector2.one;
            popupRect.offsetMin = Vector2.zero;
            popupRect.offsetMax = Vector2.zero;
        }

        shopItemPopupInstance.gameObject.SetActive(false);
        return shopItemPopupInstance;
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
        Debug.Log(
            $"[RuntimeShopScreen] Achat demandé (prototype) : {data.DisplayName} x{quantity} = {totalPrice} — " +
            "brancher wallet / inventaire ici.");

        if (shopItemPopupInstance != null)
            shopItemPopupInstance.Close();
    }

    private void BuildIfNeeded()
    {
        if (slotsContainer != null)
            return;

        RectTransform root = GetComponent<RectTransform>();
        if (root == null)
            return;

        if (GetComponent<CanvasRenderer>() == null)
            gameObject.AddComponent<CanvasRenderer>();

        Image background = GetComponent<Image>();
        if (background == null)
            background = gameObject.AddComponent<Image>();

        if (background == null)
        {
            Debug.LogError("[RuntimeShopScreen] Impossible d'ajouter Image sur l'ecran runtime shop.");
            return;
        }

        HudModalBackdrop.ApplyRootBackground(background);
        rootBackdropImage = background;

        HudModalBackdrop.EnsureModalCanvas(gameObject);

        CreateHeader(root);
        CreateContent(root);
        CreateFooter(root);
    }

    private void CreateHeader(RectTransform root)
    {
        RectTransform header = CreatePanel("Header", root, new Color(0.1f, 0.1f, 0.1f, 0.95f));
        SetAnchors(header, 0f, 1f, 1f, 1f, 0f, -HeaderHeight, 0f, 0f);

        Text title = CreateText("Title", header, "Market (prototype)");
        title.alignment = TextAnchor.MiddleLeft;
        title.fontSize = FontSize;
        title.rectTransform.offsetMin = new Vector2(24f, 0f);
        title.rectTransform.offsetMax = new Vector2(-120f, 0f);

        Button closeButton = CreateButton("CloseButton", header, "Fermer");
        SetAnchors(closeButton.GetComponent<RectTransform>(), 1f, 1f, 0.5f, 0.5f, -104f, -20f, -16f, 20f);
        closeButton.onClick.AddListener(HideScreen);
    }

    private void CreateContent(RectTransform root)
    {
        RectTransform content = CreatePanel("Content", root, HudModalBackdrop.ContentPanelColor);
        contentBackdropImage = content.GetComponent<Image>();
        HudModalBackdrop.ApplyContentPanel(contentBackdropImage);
        SetAnchors(content, 0f, 1f, 0f, 1f, 16f, FooterHeight + 12f, -16f, -HeaderHeight - 12f);

        ScrollRect scrollRect = content.gameObject.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        RectTransform viewport = CreatePanel("Viewport", content, new Color(0f, 0f, 0f, 0f));
        SetAnchors(viewport, 0f, 1f, 0f, 1f, 0f, 0f, 0f, 0f);
        viewport.gameObject.AddComponent<RectMask2D>();

        GameObject grid = new("SlotsGrid", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        slotsContainer = grid.GetComponent<RectTransform>();
        slotsContainer.SetParent(viewport, false);
        slotsContainer.anchorMin = new Vector2(0f, 1f);
        slotsContainer.anchorMax = new Vector2(0f, 1f);
        slotsContainer.pivot = new Vector2(0f, 1f);
        slotsContainer.anchoredPosition = Vector2.zero;
        slotsContainer.sizeDelta = Vector2.zero;

        GridLayoutGroup gridLayout = grid.GetComponent<GridLayoutGroup>();
        ApplyGridLayout(gridLayout);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.UpperLeft;

        ContentSizeFitter fitter = grid.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport;
        scrollRect.content = slotsContainer;

        fallbackListText = CreateText("FallbackList", content, string.Empty);
        fallbackListText.alignment = TextAnchor.UpperLeft;
        fallbackListText.fontSize = SmallFontSize;
        fallbackListText.rectTransform.offsetMin = new Vector2(16f, 16f);
        fallbackListText.rectTransform.offsetMax = new Vector2(-16f, -16f);
        fallbackListText.gameObject.SetActive(false);
    }

    private void CreateFooter(RectTransform root)
    {
        RectTransform footer = CreatePanel("Footer", root, new Color(0.1f, 0.1f, 0.1f, 0.85f));
        SetAnchors(footer, 0f, 1f, 0f, 0f, 0f, 0f, 0f, FooterHeight);

        stateLabel = CreateText("State", footer, string.Empty);
        stateLabel.verticalOverflow = VerticalWrapMode.Overflow;
        stateLabel.alignment = TextAnchor.MiddleLeft;
        stateLabel.fontSize = SmallFontSize;
        stateLabel.rectTransform.offsetMin = new Vector2(24f, 0f);
        stateLabel.rectTransform.offsetMax = new Vector2(-24f, 0f);
    }

    private void ApplyLayoutAndBackdrop()
    {
        if (rootBackdropImage != null)
            HudModalBackdrop.ApplyRootBackground(rootBackdropImage);

        if (contentBackdropImage != null)
            HudModalBackdrop.ApplyContentPanel(contentBackdropImage);

        if (slotsContainer == null)
            return;

        GridLayoutGroup grid = slotsContainer.GetComponent<GridLayoutGroup>();
        ApplyGridLayout(grid);
    }

    private void ApplyGridLayout(GridLayoutGroup gridLayout)
    {
        if (gridLayout == null)
            return;

        gridLayout.cellSize = new Vector2(SlotSize, SlotSize);
        gridLayout.spacing = new Vector2(SlotSpacing, SlotSpacing);
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
            return "Catalogue vide (vérifiez le JSON ou les itemId dans ItemDatabase).";

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
            sb.Append(" ×");
            sb.Append(row.Slot.Quantity);
            sb.Append(" @");
            sb.Append(row.UnitPrice);

            if (sb.Length >= PriceSummaryMaxChars)
            {
                sb.Append(" …");
                break;
            }
        }

        return sb.ToString();
    }

    private void SetFooterLines(string lineA, string lineB)
    {
        if (stateLabel == null)
            return;

        stateLabel.text = string.IsNullOrEmpty(lineB) ? lineA : $"{lineA}\n{lineB}";
    }

    private void HideScreen()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.HideScreen(ScreenId.Shop);
    }

    private static RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(Image));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        Image img = go.GetComponent<Image>();
        HudModalBackdrop.SetupSolidFillImage(img);
        img.color = color;
        img.raycastTarget = true;
        return rect;
    }

    private static Text CreateText(string name, Transform parent, string content)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(Text));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        SetAnchors(rect, 0f, 1f, 0f, 1f, 0f, 0f, 0f, 0f);

        Text text = go.GetComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static Button CreateButton(string name, Transform parent, string label)
    {
        GameObject buttonGo = new(name, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rect = buttonGo.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        Image buttonImage = buttonGo.GetComponent<Image>();
        HudModalBackdrop.SetupSolidFillImage(buttonImage);
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Text text = CreateText("Label", rect, label);
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = SmallFontSize;
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;

        return buttonGo.GetComponent<Button>();
    }

    private static void SetAnchors(
        RectTransform rect,
        float minX,
        float maxX,
        float minY,
        float maxY,
        float left,
        float bottom,
        float right,
        float top)
    {
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(right, top);
    }
}
