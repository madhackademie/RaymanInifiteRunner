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
    [SerializeField] private TextMeshProUGUI tabAventuresLabel;

    [Header("Exit Button")]
    [SerializeField] private Button exitButton;

    [Header("Tab icon fade (sprites full color)")]
    [SerializeField] private float iconAlphaActive = 1f;
    [SerializeField] private float iconAlphaInactive = 0.55f;

    [Header("Tab Aventures — zoom actif")]
    [SerializeField] private float activeTabIconScale = 1.3f;
    [SerializeField] private float activeTabIconLiftY = 18f;
    [SerializeField] private float inactiveTabIconGray = 0.5f;
    [SerializeField] private Color activeTabLabelColor = new Color(1f, 0.92f, 0.2f, 1f);

    private Vector2 tabAventuresIconLiftRestPosition;
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
        ApplyTabVisual(tabInventaireIcon, tabInventaireSelectedFrame, active == Tab.Inventaire);
        ApplyTabVisual(tabShopIcon, tabShopSelectedFrame, active == Tab.Shop);
        ApplyTabVisual(tabSaleChannelsIcon, tabSaleChannelsSelectedFrame, active == Tab.SaleChannels);
    }

    /// <summary>Mockup nav : inactif gris ; actif = zoom, lueur, label, couleurs pleines.</summary>
    private void ApplyAventuresTabVisual(bool isActive)
    {
        // Ancien cadre 4 bordures : désactivé pour ce pilote (lueur + zoom à la place).
        if (tabAventuresSelectedFrame != null)
            tabAventuresSelectedFrame.SetActive(false);

        bool hasSprite = tabAventuresIcon != null && tabAventuresIcon.sprite != null;

        bool showActiveFx = isActive && hasSprite;

        if (tabAventuresGlow != null)
        {
            tabAventuresGlow.SetActive(showActiveFx);
            if (showActiveFx)
            {
                Image glowImage = tabAventuresGlow.GetComponent<Image>();
                if (glowImage != null)
                    glowImage.color = new Color(activeTabLabelColor.r, activeTabLabelColor.g, activeTabLabelColor.b, 0.45f);
            }
        }

        if (tabAventuresLabel != null)
        {
            tabAventuresLabel.gameObject.SetActive(showActiveFx);
            if (showActiveFx)
                tabAventuresLabel.color = activeTabLabelColor;
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

        if (!hasSprite)
        {
            tabAventuresIcon.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        if (isActive)
        {
            tabAventuresIcon.color = new Color(1f, 1f, 1f, iconAlphaActive);
            return;
        }

        float gray = inactiveTabIconGray;
        tabAventuresIcon.color = new Color(gray, gray, gray, iconAlphaInactive);
    }

    /// <summary>Actif = cadre visible + icône pleine opacité. Inactif = pas de cadre + léger fade (pas de tint couleur).</summary>
    private void ApplyTabVisual(Image icon, GameObject selectedFrame, bool isActive)
    {
        if (selectedFrame != null)
            selectedFrame.SetActive(isActive);

        if (icon == null)
            return;

        float alpha = isActive ? iconAlphaActive : iconAlphaInactive;
        icon.color = new Color(1f, 1f, 1f, alpha);
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
