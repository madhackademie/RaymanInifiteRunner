using System;
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

    [Header("Tab Icons")]
    [SerializeField] private Image tabAventuresIcon;
    [SerializeField] private Image tabInventaireIcon;
    [SerializeField] private Image tabShopIcon;

    [Header("Exit Button")]
    [SerializeField] private Button exitButton;

    [Header("Couleurs de tab")]
    [SerializeField] private Color colorActive   = new Color(1f,    0.78f, 0.2f,  1f);
    [SerializeField] private Color colorInactive = new Color(0.55f, 0.55f, 0.55f, 1f);

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

        if (tabAventuresButton != null)
            tabAventuresButton.onClick.AddListener(OnTabAventuresClicked);
        if (tabInventaireButton != null)
            tabInventaireButton.onClick.AddListener(OnTabInventaireClicked);
        if (tabShopButton != null)
            tabShopButton.onClick.AddListener(OnTabShopClicked);
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
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);
    }

    // ── Display API ───────────────────────────────────────────────────────────

    /// <summary>Affiche la barre de navigation complète.</summary>
    public static void ShowNavBar()
    {
        if (Instance == null) return;
        Instance.ApplyMode(HudMode.Navigation);
    }

    /// <summary>Affiche uniquement le bouton de sortie (croix). Pour les scènes gameplay.</summary>
    public static void ShowExitOnly()
    {
        if (Instance == null) return;
        Instance.ApplyMode(HudMode.ExitOnly);
    }

    /// <summary>Masque le HUD entièrement. Pour les cinématiques et l'écran de chargement.</summary>
    public static void Hide()
    {
        if (Instance == null) return;
        Instance.ApplyMode(HudMode.Hidden);
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
        if (SceneNavigator.Instance == null || SceneNavigator.Instance.IsTransitioning) return;
        SetTabsInteractable(false);

        if (UIManager.Instance != null && UIManager.Instance.TryShowScreen(ScreenId.Inventory))
        {
            UIManager.Instance.HideScreen(ScreenId.Shop);
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
        if (SceneNavigator.Instance == null || SceneNavigator.Instance.IsTransitioning) return;
        SetTabsInteractable(false);

        if (UIManager.Instance != null && UIManager.Instance.TryShowScreen(ScreenId.Shop))
        {
            UIManager.Instance.HideScreen(ScreenId.Inventory);
            RefreshTabVisuals(Tab.Shop);
            SetTabsInteractable(true);
            return;
        }

        Debug.LogWarning("[NavigationHUD] Ecran Shop introuvable dans UIManager.");
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

    private void SetTabsInteractable(bool interactable)
    {
        if (tabAventuresButton != null)
            tabAventuresButton.interactable = interactable;
        if (tabInventaireButton != null)
            tabInventaireButton.interactable = interactable;
        if (tabShopButton != null)
            tabShopButton.interactable = interactable;
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

        if (onShop)
        {
            RefreshTabVisuals(Tab.Shop);
            return;
        }

        RefreshTabVisuals(onInventaire ? Tab.Inventaire : Tab.Aventures);
    }

    private void RefreshTabVisuals(Tab active)
    {
        SetIconColor(tabAventuresIcon,  active == Tab.Aventures);
        SetIconColor(tabInventaireIcon, active == Tab.Inventaire);
        SetIconColor(tabShopIcon, active == Tab.Shop);
    }

    private void SetIconColor(Image icon, bool isActive)
    {
        if (icon != null)
            icon.color = isActive ? colorActive : colorInactive;
    }

    // ── Enums ─────────────────────────────────────────────────────────────────

    private void HideGlobalPanels()
    {
        if (UIManager.Instance == null)
            return;

        UIManager.Instance.HideScreen(ScreenId.Inventory);
        UIManager.Instance.HideScreen(ScreenId.Shop);
    }

    private enum Tab { Aventures, Inventaire, Shop }
}
