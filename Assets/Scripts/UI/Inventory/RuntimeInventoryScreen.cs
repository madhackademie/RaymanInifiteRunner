using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ecran inventaire runtime autonome utilisé par UIManager quand aucun prefab
/// d'inventaire global n'est encore configuré.
/// </summary>
public class RuntimeInventoryScreen : MonoBehaviour
{
    private const float HeaderHeight = 64f;
    private const float FooterHeight = 56f;
    private const int FontSize = 24;
    private const int SmallFontSize = 18;
    private const float SlotSize = 80f;
    private const float SlotSpacing = 10f;

    private PlayerInventory playerInventory;
    private InventorySlotUI slotPrefab;
    private int columnCount = 5;
    private RectTransform slotsContainer;
    private readonly List<InventorySlotUI> slotViews = new();
    private Text fallbackListText;
    private Text stateLabel;

    /// <summary>
    /// Initialisation explicite depuis UIManager (injection des dependances runtime).
    /// </summary>
    public void Initialize(PlayerInventory inventory, InventorySlotUI slotPrefabOverride, int columns)
    {
        playerInventory = inventory;
        slotPrefab = slotPrefabOverride;
        columnCount = Mathf.Max(1, columns);
        BuildIfNeeded();
        TryBindInventory();
        Refresh();
    }

    private void Awake()
    {
        BuildIfNeeded();
        TryBindInventory();
    }

    private void OnEnable()
    {
        TryBindInventory();
        Refresh();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= Refresh;
    }

    private void TryBindInventory()
    {
        PlayerInventory candidate = playerInventory != null ? playerInventory : PlayerInventory.Instance;
        if (candidate == null)
            return;

        if (playerInventory != null && playerInventory != candidate)
            playerInventory.OnInventoryChanged -= Refresh;

        playerInventory = candidate;
        playerInventory.OnInventoryChanged -= Refresh;
        playerInventory.OnInventoryChanged += Refresh;
    }

    private void BuildIfNeeded()
    {
        if (slotsContainer != null)
            return;

        RectTransform root = GetComponent<RectTransform>();
        if (root == null)
            return;

        // En runtime, on garantit les composants UI minimaux du root.
        if (GetComponent<CanvasRenderer>() == null)
            gameObject.AddComponent<CanvasRenderer>();

        Image background = GetComponent<Image>();
        if (background == null)
            background = gameObject.AddComponent<Image>();

        if (background == null)
        {
            Debug.LogError("[RuntimeInventoryScreen] Impossible d'ajouter Image sur l'ecran runtime inventaire.");
            return;
        }

        background.color = new Color(0f, 0f, 0f, 0.78f);

        CreateHeader(root);
        CreateContent(root);
        CreateFooter(root);
    }

    private void CreateHeader(RectTransform root)
    {
        RectTransform header = CreatePanel("Header", root, new Color(0.1f, 0.1f, 0.1f, 0.95f));
        SetAnchors(header, 0f, 1f, 1f, 1f, 0f, -HeaderHeight, 0f, 0f);

        Text title = CreateText("Title", header, "Inventaire");
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
        RectTransform content = CreatePanel("Content", root, new Color(0f, 0f, 0f, 0.15f));
        SetAnchors(content, 0f, 1f, 0f, 1f, 16f, FooterHeight + 12f, -16f, -HeaderHeight - 12f);

        ScrollRect scrollRect = content.gameObject.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        RectTransform viewport = CreatePanel("Viewport", content, new Color(0f, 0f, 0f, 0f));
        SetAnchors(viewport, 0f, 1f, 0f, 1f, 0f, 0f, 0f, 0f);
        viewport.gameObject.AddComponent<RectMask2D>();

        // Grille de slots similaire a l'ancien inventaire.
        GameObject grid = new("SlotsGrid", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        slotsContainer = grid.GetComponent<RectTransform>();
        slotsContainer.SetParent(viewport, false);
        slotsContainer.anchorMin = new Vector2(0f, 1f);
        slotsContainer.anchorMax = new Vector2(0f, 1f);
        slotsContainer.pivot = new Vector2(0f, 1f);
        slotsContainer.anchoredPosition = Vector2.zero;
        slotsContainer.sizeDelta = Vector2.zero;

        GridLayoutGroup gridLayout = grid.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(SlotSize, SlotSize);
        gridLayout.spacing = new Vector2(SlotSpacing, SlotSpacing);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columnCount;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.UpperLeft;

        ContentSizeFitter fitter = grid.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport;
        scrollRect.content = slotsContainer;

        // Fallback texte: utile si le prefab InventorySlotUI n'est pas assigne.
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

        stateLabel = CreateText("State", footer, "Ready");
        stateLabel.alignment = TextAnchor.MiddleLeft;
        stateLabel.fontSize = SmallFontSize;
        stateLabel.rectTransform.offsetMin = new Vector2(24f, 0f);
        stateLabel.rectTransform.offsetMax = new Vector2(-24f, 0f);
    }

    public void Refresh()
    {
        if (slotsContainer == null)
            return;

        if (playerInventory == null)
        {
            SetState("Error: PlayerInventory indisponible");
            ShowFallbackText("Inventaire indisponible (service non initialise).");
            return;
        }

        IReadOnlyList<InventorySlot> slots = playerInventory.Slots;
        if (slotPrefab == null)
        {
            ShowFallbackText(BuildFallbackContent(slots));
            SetState("Ready (fallback texte)");
            return;
        }

        if (fallbackListText != null)
            fallbackListText.gameObject.SetActive(false);

        EnsureSlotViews(slots.Count);

        for (int i = 0; i < slotViews.Count; i++)
        {
            InventorySlot slotData = i < slots.Count ? slots[i] : null;
            slotViews[i].Refresh(slotData);
        }

        SetState("Ready");
    }

    /// <summary>
    /// Ajuste le nombre d'instances UI a la taille actuelle de l'inventaire.
    /// </summary>
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

    private static string BuildFallbackContent(IReadOnlyList<InventorySlot> slots)
    {
        bool hasItems = false;
        System.Text.StringBuilder sb = new();

        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if (slot == null || slot.IsEmpty || slot.Item == null)
                continue;

            hasItems = true;
            sb.Append("- ");
            sb.Append(slot.Item.DisplayName);
            sb.Append(" x");
            sb.Append(slot.Quantity);
            sb.AppendLine();
        }

        return hasItems ? sb.ToString() : "Inventaire vide";
    }

    private void SetState(string status)
    {
        if (stateLabel != null)
            stateLabel.text = status;
    }

    private void HideScreen()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.HideScreen(ScreenId.Inventory);
    }

    private static RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(Image));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
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
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static Button CreateButton(string name, Transform parent, string label)
    {
        GameObject buttonGo = new(name, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rect = buttonGo.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        buttonGo.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

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
