using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barre de navigation persistante du shell UI.
/// Gère deux modes : nav bar complète ou bouton de sortie seul.
/// Les transitions de scènes sont déléguées à SceneNavigator.
/// </summary>
public class NavigationHUD : MonoBehaviour
{
    private enum HudMode
    {
        Hidden,
        Navigation,
        ExitOnly
    }

    // ── Singleton ─────────────────────────────────────────────────────────────

    public static NavigationHUD Instance { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Déclenché quand le bouton de sortie est pressé en mode exit-only.
    /// Abonnez-vous depuis le contrôleur de la scène de gameplay active.
    /// </summary>
    public event Action OnExitToHomeRequested;

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Mode Containers")]
    [SerializeField] private GameObject navBarContainer;
    [SerializeField] private GameObject exitButtonContainer;

    [Header("Nav Bar Buttons")]
    [SerializeField] private Button tabAventuresButton;
    [SerializeField] private Button tabInventaireButton;
    [SerializeField] private Button tabShopButton;
    [SerializeField] private Button tabSaleChannelsButton;

    [Header("Tab Icons")]
    [SerializeField] private Image tabAventuresIcon;
    [SerializeField] private Image tabInventaireIcon;
    [SerializeField] private Image tabShopIcon;
    [SerializeField] private Image tabSaleChannelsIcon;

    [Header("Tab Selected Frame (Bezy — cadre actif, pas de tint icône)")]
    [SerializeField] private GameObject tabAventuresSelectedFrame;
    [SerializeField] private GameObject tabInventaireSelectedFrame;
    [SerializeField] private GameObject tabShopSelectedFrame;
    [SerializeField] private GameObject tabSaleChannelsSelectedFrame;

    [Header("Tab Aventures — mockup zoom (pilote Bezy 2026-09-10)")]
    [SerializeField] private RectTransform tabAventuresIconLift;
    [SerializeField] private GameObject tabAventuresGlow;
    [SerializeField] private GameObject tabAventuresLavaBackdrop;
    [SerializeField] private TextMeshProUGUI tabAventuresLabel;

    [Header("Tab Inventaire — label (mockup TMP sous l’icône)")]
    [SerializeField] private TextMeshProUGUI tabInventaireLabel;

    [Header("Exit Button")]
    [SerializeField] private Button exitButton;

    [Header("Tab icon — inactif N&B / actif couleur")]
    [SerializeField] private Material inactiveTabGrayscaleMaterial;

    [Header("Tab icon fade (sprites full color)")]
    [SerializeField] private float iconAlphaActive = 1f;
    [SerializeField] private float iconAlphaInactive = 0.85f;

    [Header("Tab mockup zoom actif")]
    [SerializeField] private Material activeTabGlowMaterial;
    [SerializeField] private Material activeTabLavaMaterial;
    [SerializeField] private bool useActiveTabLavaBackdrop;
    [SerializeField] private float activeTabIconScale = 1.87f;
    [SerializeField] private float activeTabIconLiftY = 48f;
    [SerializeField] private float activeTabIconLocalOffsetY = 33f;
    [SerializeField] private float activeTabGlowAlpha = 0.42f;
    [SerializeField] private float activeTabGlowSize = 108f;
    [SerializeField] private float inactiveTabIconGray = 0.5f;
    [SerializeField] private Color activeTabLabelColor = new Color(1f, 0.78f, 0.2f, 1f);
    [SerializeField] private float activeTabLabelFontSize = 31f;
    [SerializeField] private float activeTabLabelOutlineWidth = 0.28f;

    [Header("Tab actif — cadre SelectedFrame (englobe l’icône zoomée)")]
    [SerializeField] private Vector2 activeTabFrameSizeDeltaAdd = new Vector2(-22f, -92f);
    [SerializeField] private float activeTabFrameActivePosY = 34f;

    private Vector2 tabAventuresIconLiftRestPosition;
    private Vector2 tabAventuresIconRestAnchoredPosition;
    private Vector2 tabAventuresSelectedFrameRestPosition;
    private Vector2 tabAventuresSelectedFrameRestSizeDelta;
    private static Sprite uiWhiteSprite;
    private HudMode currentMode = HudMode.Hidden;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (tabAventuresIconLift != null)
            tabAventuresIconLiftRestPosition = tabAventuresIconLift.anchoredPosition;

        if (tabAventuresIcon != null)
            tabAventuresIconRestAnchoredPosition = tabAventuresIcon.rectTransform.anchoredPosition;

        if (tabAventuresSelectedFrame != null)
        {
            RectTransform frameRect = tabAventuresSelectedFrame.GetComponent<RectTransform>();
            if (frameRect != null)
            {
                tabAventuresSelectedFrameRestPosition = frameRect.anchoredPosition;
                tabAventuresSelectedFrameRestSizeDelta = frameRect.sizeDelta;
            }
        }

        ConfigureAventuresTabFxHierarchy();
        ApplyNavTabLabelStyle(tabAventuresLabel);
        ApplyNavTabLabelStyle(tabInventaireLabel);

        if (tabAventuresButton != null)
            tabAventuresButton.onClick.AddListener(OnTabAventuresClicked);
        if (tabInventaireButton != null)
            tabInventaireButton.onClick.AddListener(OnTabInventaireClicked);
        if (tabShopButton != null)
            tabShopButton.onClick.AddListener(OnTabShopClicked);
        if (tabSaleChannelsButton != null)
            tabSaleChannelsButton.onClick.AddListener(OnTabSaleChannelsClicked);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        if (SceneNavigator.Instance != null)
            BindNavigator(SceneNavigator.Instance);
    }

    private void OnEnable()
    {
        SceneNavigator.OnNavigatorAvailable += BindNavigator;
        SceneNavigator.OnNavigatorUnavailable += UnbindNavigator;
    }

    private void OnDestroy()
    {
        SceneNavigator.OnNavigatorAvailable -= BindNavigator;
        SceneNavigator.OnNavigatorUnavailable -= UnbindNavigator;
        UnbindNavigator();

        if (tabAventuresButton != null)
            tabAventuresButton.onClick.RemoveListener(OnTabAventuresClicked);
        if (tabInventaireButton != null)
            tabInventaireButton.onClick.RemoveListener(OnTabInventaireClicked);
        if (tabShopButton != null)
            tabShopButton.onClick.RemoveListener(OnTabShopClicked);
        if (tabSaleChannelsButton != null)
            tabSaleChannelsButton.onClick.RemoveListener(OnTabSaleChannelsClicked);
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);
    }

    // ── Tab callbacks ─────────────────────────────────────────────────────────

    /// <summary>Affiche HomeScene via SceneNavigator.</summary>
    public async void OnTabAventuresClicked()
    {
        if (SceneNavigator.Instance == null || SceneNavigator.Instance.IsTransitioning) return;
        SetTabsInteractable(false);
        HideGlobalPanels();

        await SceneNavigator.Instance.ShowScene(SceneId.HomeScene);
        RefreshTabVisuals(Tab.Aventures);
        SetTabsInteractable(true);
    }

    /// <summary>Affiche l'écran inventaire global via UIManager.</summary>
    public void OnTabInventaireClicked()
    {
        if (IsSceneTransitionBlocking())
            return;

        SetTabsInteractable(false);

        if (UIManager.Instance != null && UIManager.Instance.TryShowScreen(ScreenId.Inventory))
        {
            HideOtherModalScreens(ScreenId.Inventory);
            RefreshTabVisuals(Tab.Inventaire);
            SetTabsInteractable(true);
            return;
        }

        Debug.LogWarning("[NavigationHUD] Ecran Inventory introuvable dans UIManager.");
        SetTabsInteractable(true);
    }

    /// <summary>Affiche l'écran shop global via UIManager.</summary>
    public void OnTabShopClicked()
    {
        if (IsSceneTransitionBlocking())
            return;

        SetTabsInteractable(false);

        if (UIManager.Instance != null && UIManager.Instance.TryShowScreen(ScreenId.Shop))
        {
            HideOtherModalScreens(ScreenId.Shop);
            RefreshTabVisuals(Tab.Shop);
            SetTabsInteractable(true);
            return;
        }

        Debug.LogWarning("[NavigationHUD] Ecran Shop introuvable dans UIManager.");
        SetTabsInteractable(true);
    }

    /// <summary>Affiche l'écran canaux de vente via UIManager.</summary>
    public void OnTabSaleChannelsClicked()
    {
        if (IsSceneTransitionBlocking())
            return;

        SetTabsInteractable(false);

        if (UIManager.Instance != null && UIManager.Instance.TryShowScreen(ScreenId.SaleChannels))
        {
            HideOtherModalScreens(ScreenId.SaleChannels);
            RefreshTabVisuals(Tab.SaleChannels);
            SetTabsInteractable(true);
            return;
        }

        Debug.LogWarning("[NavigationHUD] Ecran SaleChannels introuvable dans UIManager.");
        SetTabsInteractable(true);
    }

    /// <summary>
    /// En mode exit-only : notifie la scène gameplay pour qu'elle gère le retour.
    /// En mode nav bar : sans effet (les tabs gèrent la navigation).
    /// </summary>
    public void OnExitClicked()
    {
        if (exitButtonContainer.activeSelf)
            OnExitToHomeRequested?.Invoke();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Bloque uniquement pendant une transition de scène si un SceneNavigator existe.
    /// Les modales UIManager (inventaire / shop) restent utilisables sans navigateur.
    /// </summary>
    private static bool IsSceneTransitionBlocking()
    {
        return SceneNavigator.Instance != null && SceneNavigator.Instance.IsTransitioning;
    }

    private void SetTabsInteractable(bool interactable)
    {
        if (tabAventuresButton != null)
            tabAventuresButton.interactable = interactable;
        if (tabInventaireButton != null)
            tabInventaireButton.interactable = interactable;
        if (tabShopButton != null)
            tabShopButton.interactable = interactable;
        if (tabSaleChannelsButton != null)
            tabSaleChannelsButton.interactable = interactable;
    }

    private SceneNavigator boundNavigator;

    private void BindNavigator(SceneNavigator navigator)
    {
        if (navigator == null)
            return;

        if (boundNavigator != null && boundNavigator != navigator)
            UnbindNavigator();

        boundNavigator = navigator;
        boundNavigator.OnAfterSceneShown -= HandleSceneShown;
        boundNavigator.OnAfterSceneShown += HandleSceneShown;

        boundNavigator.OnTransitionStateChanged -= HandleTransitionChanged;
        boundNavigator.OnTransitionStateChanged += HandleTransitionChanged;

        if (navigator.IsTransitioning)
            ApplyMode(HudMode.Hidden);
        else
            HandleSceneShown(navigator.CurrentScene);
    }

    private void UnbindNavigator()
    {
        if (boundNavigator == null)
            return;

        boundNavigator.OnAfterSceneShown -= HandleSceneShown;
        boundNavigator.OnTransitionStateChanged -= HandleTransitionChanged;
        boundNavigator = null;
    }

    private void HandleTransitionChanged(bool isTransitioning)
    {
        if (isTransitioning)
            ApplyMode(HudMode.Hidden);
    }

    private void HandleSceneShown(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            ApplyMode(HudMode.Hidden);
            return;
        }

        if (sceneName == SceneId.HomeScene)
        {
            ApplyMode(HudMode.Navigation);
            HideGlobalPanels();
            return;
        }

        ApplyMode(HudMode.ExitOnly);
    }

    private void ApplyMode(HudMode mode)
    {
        if (currentMode == mode)
            return;

        currentMode = mode;
        navBarContainer.SetActive(mode == HudMode.Navigation);
        exitButtonContainer.SetActive(mode == HudMode.ExitOnly);

        if (mode == HudMode.Navigation)
            RefreshTabVisuals();
    }

    private void RefreshTabVisuals()
    {
        bool onInventaire = UIManager.Instance != null &&
                            UIManager.Instance.IsScreenVisible(ScreenId.Inventory);
        bool onShop = UIManager.Instance != null &&
                      UIManager.Instance.IsScreenVisible(ScreenId.Shop);
        bool onSaleChannels = UIManager.Instance != null &&
                              UIManager.Instance.IsScreenVisible(ScreenId.SaleChannels);

        if (onSaleChannels)
        {
            RefreshTabVisuals(Tab.SaleChannels);
            return;
        }

        if (onShop)
        {
            RefreshTabVisuals(Tab.Shop);
            return;
        }

        RefreshTabVisuals(onInventaire ? Tab.Inventaire : Tab.Aventures);
    }

    private void RefreshTabVisuals(Tab active)
    {
        ApplyAventuresTabVisual(active == Tab.Aventures);
        ApplyStandardTabVisual(tabInventaireIcon, tabInventaireSelectedFrame, tabInventaireLabel, active == Tab.Inventaire);
        ApplyStandardTabVisual(tabShopIcon, tabShopSelectedFrame, null, active == Tab.Shop);
        ApplyStandardTabVisual(tabSaleChannelsIcon, tabSaleChannelsSelectedFrame, null, active == Tab.SaleChannels);
    }

    /// <summary>Mockup nav : inactif gris ; actif = zoom, lueur, label, couleurs pleines.</summary>
    private void ApplyAventuresTabVisual(bool isActive)
    {
        bool hasSprite = tabAventuresIcon != null && tabAventuresIcon.sprite != null;

        bool showActiveFx = isActive && hasSprite;

        ApplyAventuresSelectedFrameLayout(showActiveFx);

        if (tabAventuresLavaBackdrop != null)
            tabAventuresLavaBackdrop.SetActive(useActiveTabLavaBackdrop && showActiveFx);

        if (tabAventuresGlow != null)
        {
            tabAventuresGlow.SetActive(showActiveFx);
            if (showActiveFx)
            {
                Image glowImage = tabAventuresGlow.GetComponent<Image>();
                if (glowImage != null)
                {
                    if (activeTabGlowMaterial != null)
                        glowImage.material = activeTabGlowMaterial;
                    glowImage.color = new Color(activeTabLabelColor.r, activeTabLabelColor.g, activeTabLabelColor.b, activeTabGlowAlpha);
                }
            }
        }

        if (tabAventuresLabel != null)
        {
            tabAventuresLabel.gameObject.SetActive(showActiveFx);
            if (showActiveFx)
                ApplyNavTabLabelStyle(tabAventuresLabel);
        }

        if (tabAventuresIconLift != null)
        {
            tabAventuresIconLift.localScale = Vector3.one;

            float liftY = isActive && hasSprite ? activeTabIconLiftY : 0f;
            tabAventuresIconLift.anchoredPosition = tabAventuresIconLiftRestPosition + new Vector2(0f, liftY);
        }

        if (tabAventuresIcon == null)
            return;

        RectTransform iconRect = tabAventuresIcon.rectTransform;
        float iconScale = isActive && hasSprite ? activeTabIconScale : 1f;
        iconRect.localScale = new Vector3(iconScale, iconScale, 1f);

        float iconOffsetY = isActive && hasSprite ? activeTabIconLocalOffsetY : 0f;
        iconRect.anchoredPosition = tabAventuresIconRestAnchoredPosition + new Vector2(0f, iconOffsetY);

        if (!hasSprite)
        {
            tabAventuresIcon.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        ApplyNavTabIconAppearance(tabAventuresIcon, isActive, hasSprite);
    }

    private void ApplyAventuresSelectedFrameLayout(bool showActiveFx)
    {
        if (tabAventuresSelectedFrame == null)
            return;

        tabAventuresSelectedFrame.SetActive(showActiveFx);

        RectTransform frameRect = tabAventuresSelectedFrame.GetComponent<RectTransform>();
        if (frameRect == null)
            return;

        if (!showActiveFx)
        {
            frameRect.anchoredPosition = tabAventuresSelectedFrameRestPosition;
            frameRect.sizeDelta = tabAventuresSelectedFrameRestSizeDelta;
            return;
        }

        frameRect.anchoredPosition = tabAventuresSelectedFrameRestPosition + new Vector2(0f, activeTabFrameActivePosY);
        frameRect.sizeDelta = tabAventuresSelectedFrameRestSizeDelta + activeTabFrameSizeDeltaAdd;
    }

    /// <summary>Onglets sans IconLift/Glow : N&B inactif, couleur actif, label TMP seulement si actif.</summary>
    private void ApplyStandardTabVisual(Image icon, GameObject selectedFrame, TextMeshProUGUI label, bool isActive)
    {
        bool hasSprite = icon != null && icon.sprite != null;
        bool showActive = isActive && hasSprite;

        if (selectedFrame != null)
            selectedFrame.SetActive(showActive);
        if (label != null)
        {
            label.gameObject.SetActive(showActive);
            if (showActive)
                ApplyNavTabLabelStyle(label);
        }

        ApplyNavTabIconAppearance(icon, isActive, hasSprite);
    }

    private void ConfigureAventuresTabFxHierarchy()
    {
        if (tabAventuresGlow != null && tabAventuresIconLift != null)
        {
            RectTransform glowRect = tabAventuresGlow.transform as RectTransform;
            if (glowRect != null && glowRect.parent != tabAventuresIconLift)
            {
                glowRect.SetParent(tabAventuresIconLift, false);
                glowRect.SetAsFirstSibling();
            }

            if (glowRect != null)
            {
                glowRect.anchorMin = new Vector2(0.5f, 0.5f);
                glowRect.anchorMax = new Vector2(0.5f, 0.5f);
                glowRect.pivot = new Vector2(0.5f, 0.5f);
                glowRect.anchoredPosition = new Vector2(0f, 8f);
                glowRect.sizeDelta = new Vector2(activeTabGlowSize, activeTabGlowSize);
            }

            Image glowImage = tabAventuresGlow.GetComponent<Image>();
            if (glowImage != null)
            {
                glowImage.raycastTarget = false;
                glowImage.sprite = GetUiWhiteSprite();
                if (activeTabGlowMaterial != null)
                    glowImage.material = activeTabGlowMaterial;
            }
        }

        if (!useActiveTabLavaBackdrop && tabAventuresLavaBackdrop != null)
            tabAventuresLavaBackdrop.SetActive(false);

        if (useActiveTabLavaBackdrop && tabAventuresLavaBackdrop == null && tabAventuresButton != null && activeTabLavaMaterial != null)
        {
            GameObject lavaGo = new GameObject("ActiveLava", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            lavaGo.layer = 5;
            RectTransform lavaRect = lavaGo.GetComponent<RectTransform>();
            lavaRect.SetParent(tabAventuresButton.transform, false);
            lavaRect.SetSiblingIndex(1);

            lavaRect.anchorMin = new Vector2(0f, 0f);
            lavaRect.anchorMax = new Vector2(1f, 0f);
            lavaRect.pivot = new Vector2(0.5f, 0f);
            lavaRect.anchoredPosition = new Vector2(0f, 0f);
            lavaRect.sizeDelta = new Vector2(0f, 78f);

            Image lavaImage = lavaGo.GetComponent<Image>();
            lavaImage.raycastTarget = false;
            lavaImage.sprite = GetUiWhiteSprite();
            lavaImage.material = activeTabLavaMaterial;
            lavaImage.color = Color.white;

            tabAventuresLavaBackdrop = lavaGo;
            tabAventuresLavaBackdrop.SetActive(false);
        }
        else if (tabAventuresLavaBackdrop != null)
        {
            Image lavaImage = tabAventuresLavaBackdrop.GetComponent<Image>();
            if (lavaImage != null)
            {
                lavaImage.raycastTarget = false;
                if (lavaImage.sprite == null)
                    lavaImage.sprite = GetUiWhiteSprite();
                if (activeTabLavaMaterial != null)
                    lavaImage.material = activeTabLavaMaterial;
            }
        }
    }

    private void ApplyNavTabLabelStyle(TextMeshProUGUI label)
    {
        if (label == null)
            return;

        label.color = activeTabLabelColor;
        label.fontSize = activeTabLabelFontSize;
        label.outlineWidth = activeTabLabelOutlineWidth;
        label.outlineColor = Color.black;
        label.fontStyle = FontStyles.Bold;
    }

    private static Sprite GetUiWhiteSprite()
    {
        if (uiWhiteSprite != null)
            return uiWhiteSprite;

        Texture2D tex = Texture2D.whiteTexture;
        uiWhiteSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        return uiWhiteSprite;
    }

    /// <summary>Inactif = désaturation (material). Actif = couleurs sprite (material par défaut).</summary>
    private void ApplyNavTabIconAppearance(Image icon, bool isActive, bool hasSprite)
    {
        if (icon == null)
            return;

        if (!hasSprite)
        {
            icon.material = null;
            icon.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        if (isActive)
        {
            icon.material = null;
            icon.color = new Color(1f, 1f, 1f, iconAlphaActive);
            return;
        }

        if (inactiveTabGrayscaleMaterial != null)
        {
            icon.material = inactiveTabGrayscaleMaterial;
            icon.color = new Color(1f, 1f, 1f, iconAlphaInactive);
            return;
        }

        float gray = inactiveTabIconGray;
        icon.material = null;
        icon.color = new Color(gray, gray, gray, iconAlphaInactive);
    }

    // ── Enums ─────────────────────────────────────────────────────────────────

    private void HideGlobalPanels()
    {
        if (UIManager.Instance == null)
            return;

        UIManager.Instance.HideScreen(ScreenId.Inventory);
        UIManager.Instance.HideScreen(ScreenId.Shop);
        UIManager.Instance.HideScreen(ScreenId.SaleChannels);
    }

    private static void HideOtherModalScreens(string keepVisibleScreenId)
    {
        if (UIManager.Instance == null)
            return;

        if (keepVisibleScreenId != ScreenId.Inventory)
            UIManager.Instance.HideScreen(ScreenId.Inventory);
        if (keepVisibleScreenId != ScreenId.Shop)
            UIManager.Instance.HideScreen(ScreenId.Shop);
        if (keepVisibleScreenId != ScreenId.SaleChannels)
            UIManager.Instance.HideScreen(ScreenId.SaleChannels);
    }

    private enum Tab { Aventures, Inventaire, Shop, SaleChannels }
}
