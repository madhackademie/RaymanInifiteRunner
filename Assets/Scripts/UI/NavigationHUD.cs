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

    [Header("HUD chrome — modales plein écran")]
    [Tooltip("Masqué sur Inventaire / Shop / Hub Plus. Visible sur Vente (coût PA).")]
    [SerializeField] private GameObject actionPointsHudRoot;

    [Header("Nav Bar Buttons")]
    [SerializeField] private Button tabAventuresButton;
    [SerializeField] private Button tabInventaireButton;
    [SerializeField] private Button tabShopButton;
    [SerializeField] private Button tabSaleChannelsButton;
    [SerializeField] private Button tabMoreOptionButton;

    [Header("Tab Icons")]
    [SerializeField] private Image tabAventuresIcon;
    [SerializeField] private Image tabInventaireIcon;
    [SerializeField] private Image tabShopIcon;
    [SerializeField] private Image tabSaleChannelsIcon;
    [SerializeField] private Image tabMoreOptionIcon;

    [Header("Tab Selected Frame (Bezy — cadre actif, pas de tint icône)")]
    [SerializeField] private GameObject tabAventuresSelectedFrame;
    [SerializeField] private GameObject tabInventaireSelectedFrame;
    [SerializeField] private GameObject tabShopSelectedFrame;
    [SerializeField] private GameObject tabSaleChannelsSelectedFrame;
    [SerializeField] private GameObject tabMoreOptionSelectedFrame;

    [Header("Tab Aventures — mockup zoom (pilote Bezy 2026-09-10)")]
    [SerializeField] private RectTransform tabAventuresIconLift;
    [SerializeField] private GameObject tabAventuresGlow;
    [SerializeField] private GameObject tabAventuresLavaBackdrop;
    [SerializeField] private TextMeshProUGUI tabAventuresLabel;

    [Header("Tab Inventaire — mockup zoom (même pipeline que Aventures)")]
    [SerializeField] private RectTransform tabInventaireIconLift;
    [SerializeField] private GameObject tabInventaireGlow;
    [SerializeField] private GameObject tabInventaireLavaBackdrop;
    [SerializeField] private TextMeshProUGUI tabInventaireLabel;

    [Header("Tab Shop — mockup zoom (même pipeline que Aventures)")]
    [SerializeField] private RectTransform tabShopIconLift;
    [SerializeField] private GameObject tabShopGlow;
    [SerializeField] private GameObject tabShopLavaBackdrop;
    [SerializeField] private TextMeshProUGUI tabShopLabel;

    [Header("Tab Vente — mockup zoom (même pipeline que Aventures)")]
    [SerializeField] private RectTransform tabSaleChannelsIconLift;
    [SerializeField] private GameObject tabSaleChannelsGlow;
    [SerializeField] private GameObject tabSaleChannelsLavaBackdrop;
    [SerializeField] private TextMeshProUGUI tabSaleChannelsLabel;

    [Header("Tab MoreOption — hub Plus (même pipeline, sprite optionnel)")]
    [SerializeField] private RectTransform tabMoreOptionIconLift;
    [SerializeField] private GameObject tabMoreOptionGlow;
    [SerializeField] private GameObject tabMoreOptionLavaBackdrop;
    [SerializeField] private TextMeshProUGUI tabMoreOptionLabel;

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
    [Tooltip("Ajout au sizeDelta repos (stretch). Valeurs positives = cadre PLUS grand.")]
    [SerializeField] private Vector2 activeTabFrameSizeDeltaExpand = new Vector2(20f, 105f);
    [SerializeField] private float activeTabFrameActivePosY = 38f;

    [Header("Nav bar — layout manuel 5 slots")]
    [Tooltip("Padding horizontal (doit matcher NavBarContainer HLG si présent).")]
    [SerializeField] private float navBarHorizontalPadding = 10f;
    [Tooltip("Réduit zoom/lift sur le 5e onglet (bord écran).")]
    [SerializeField] private float lastNavTabActiveIconScaleMultiplier = 0.82f;
    [SerializeField] private float lastNavTabActiveLiftMultiplier = 0.88f;

    private const int NavTabSlotCount = 5;

    /// <summary>
    /// Hauteur repos des 5 onglets, independante du fond NavBarContainer.
    /// Un bandeau plus haut ne doit pas agrandir icone/cadre actifs (zoom = multiplicateur).
    /// </summary>
    private const float NavTabSlotHeight = 128f;
    private NavTabMockupRest tabAventuresMockupRest;
    private NavTabMockupRest tabInventaireMockupRest;
    private NavTabMockupRest tabShopMockupRest;
    private NavTabMockupRest tabSaleChannelsMockupRest;
    private NavTabMockupRest tabMoreOptionMockupRest;
    private static Sprite uiWhiteSprite;

    private struct NavTabMockupRest
    {
        public Vector2 IconLiftAnchoredPosition;
        public Vector2 IconAnchoredPosition;
        public Vector2 SelectedFrameAnchoredPosition;
        public Vector2 SelectedFrameSizeDelta;
    }

    private struct NavTabMockupRefs
    {
        public Image Icon;
        public GameObject SelectedFrame;
        public RectTransform IconLift;
        public GameObject Glow;
        public GameObject LavaBackdrop;
        public TextMeshProUGUI Label;
    }
    private HudMode currentMode = HudMode.Hidden;
    private HorizontalLayoutGroup navBarHorizontalLayout;
    private bool navBarHorizontalLayoutDisabled;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        tabAventuresMockupRest = CaptureNavTabMockupRest(
            tabAventuresIcon,
            tabAventuresIconLift,
            tabAventuresSelectedFrame);
        tabInventaireMockupRest = CaptureNavTabMockupRest(
            tabInventaireIcon,
            tabInventaireIconLift,
            tabInventaireSelectedFrame);
        tabShopMockupRest = CaptureNavTabMockupRest(
            tabShopIcon,
            tabShopIconLift,
            tabShopSelectedFrame);
        tabSaleChannelsMockupRest = CaptureNavTabMockupRest(
            tabSaleChannelsIcon,
            tabSaleChannelsIconLift,
            tabSaleChannelsSelectedFrame);
        tabMoreOptionMockupRest = CaptureNavTabMockupRest(
            tabMoreOptionIcon,
            tabMoreOptionIconLift,
            tabMoreOptionSelectedFrame);

        ConfigureNavTabGlowHierarchy(tabAventuresIconLift, tabAventuresGlow);
        ConfigureNavTabGlowHierarchy(tabInventaireIconLift, tabInventaireGlow);
        ConfigureNavTabGlowHierarchy(tabShopIconLift, tabShopGlow);
        ConfigureNavTabGlowHierarchy(tabSaleChannelsIconLift, tabSaleChannelsGlow);
        ConfigureNavTabGlowHierarchy(tabMoreOptionIconLift, tabMoreOptionGlow);

        if (!useActiveTabLavaBackdrop && tabAventuresLavaBackdrop != null)
            tabAventuresLavaBackdrop.SetActive(false);
        if (!useActiveTabLavaBackdrop && tabInventaireLavaBackdrop != null)
            tabInventaireLavaBackdrop.SetActive(false);
        if (!useActiveTabLavaBackdrop && tabShopLavaBackdrop != null)
            tabShopLavaBackdrop.SetActive(false);
        if (!useActiveTabLavaBackdrop && tabSaleChannelsLavaBackdrop != null)
            tabSaleChannelsLavaBackdrop.SetActive(false);
        if (!useActiveTabLavaBackdrop && tabMoreOptionLavaBackdrop != null)
            tabMoreOptionLavaBackdrop.SetActive(false);

        ApplyNavTabLabelStyle(tabAventuresLabel);
        ApplyNavTabLabelStyle(tabInventaireLabel);
        ApplyNavTabLabelStyle(tabShopLabel);
        ApplyNavTabLabelStyle(tabSaleChannelsLabel);
        ApplyNavTabLabelStyle(tabMoreOptionLabel);
        PreventNavTabLabelsFromDrivingBarLayout(
            tabAventuresLabel,
            tabInventaireLabel,
            tabShopLabel,
            tabSaleChannelsLabel,
            tabMoreOptionLabel);

        if (tabAventuresButton != null)
            tabAventuresButton.onClick.AddListener(OnTabAventuresClicked);
        if (tabInventaireButton != null)
            tabInventaireButton.onClick.AddListener(OnTabInventaireClicked);
        if (tabShopButton != null)
            tabShopButton.onClick.AddListener(OnTabShopClicked);
        if (tabSaleChannelsButton != null)
            tabSaleChannelsButton.onClick.AddListener(OnTabSaleChannelsClicked);
        if (tabMoreOptionButton != null)
            tabMoreOptionButton.onClick.AddListener(OnTabMoreOptionClicked);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        RefreshModalHudPresentation();
        RebuildNavBarLayoutImmediate();
    }

    /// <summary>
    /// Masque la barre PA sur Shop / Inventaire / Hub Plus (pas d’usage PA).
    /// Vente la garde : les actions de vente consomment des PA.
    /// Le widget est un sibling après ScreenRoot — visible par-dessus si on ne le coupe pas.
    /// La nav reste dernier sibling sous HUDRoot (pas de reorder ScreenRoot).
    /// </summary>
    public void RefreshModalHudPresentation()
    {
        ResolveActionPointsHudRoot();

        if (UIManager.Instance == null)
            return;

        SetActionPointsHudVisible(!UIManager.Instance.ShouldHideActionPointsHud());
        RebuildNavBarLayoutImmediate();
    }

    /// <summary>
    /// Le champ Inspector peut pointer le prefab disque au lieu de l’instance scène
    /// (réf. avec guid, sans GameObject stripped). SetActive sur l’asset ne masque rien.
    /// </summary>
    private void ResolveActionPointsHudRoot()
    {
        if (IsSceneInstance(actionPointsHudRoot))
            return;

        ActionPointsHudView view = GetComponentInChildren<ActionPointsHudView>(true);
        if (view == null)
        {
            Debug.LogWarning("[NavigationHUD] ActionPointsHudWidget introuvable sous HUDRoot.", this);
            return;
        }

        actionPointsHudRoot = view.gameObject;
    }

    private void SetActionPointsHudVisible(bool visible)
    {
        if (actionPointsHudRoot != null)
            actionPointsHudRoot.SetActive(visible);
    }

    private static bool IsSceneInstance(GameObject go)
    {
        return go != null && go.scene.IsValid();
    }

    private void OnEnable()
    {
        SceneNavigator.OnNavigatorAvailable += BindNavigator;
        SceneNavigator.OnNavigatorUnavailable += UnbindNavigator;

        // SceneNavigator est un enfant de HUDRoot : son Awake (Instance) tourne
        // après celui du parent. Sans ce bind ici, OnNavigatorAvailable est déjà
        // passé et le HUD ne passe jamais en ExitOnly en FirstLvl.
        if (SceneNavigator.Instance != null)
            BindNavigator(SceneNavigator.Instance);
    }

    private void OnDisable()
    {
        SceneNavigator.OnNavigatorAvailable -= BindNavigator;
        SceneNavigator.OnNavigatorUnavailable -= UnbindNavigator;
        UnbindNavigator();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (tabAventuresButton != null)
            tabAventuresButton.onClick.RemoveListener(OnTabAventuresClicked);
        if (tabInventaireButton != null)
            tabInventaireButton.onClick.RemoveListener(OnTabInventaireClicked);
        if (tabShopButton != null)
            tabShopButton.onClick.RemoveListener(OnTabShopClicked);
        if (tabSaleChannelsButton != null)
            tabSaleChannelsButton.onClick.RemoveListener(OnTabSaleChannelsClicked);
        if (tabMoreOptionButton != null)
            tabMoreOptionButton.onClick.RemoveListener(OnTabMoreOptionClicked);
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
    /// Onglet Plus / MoreOption. Ouvre FeaturesHub s'il est bindé ; sinon sélectionne l'onglet (V0 sans écran).
    /// </summary>
    public void OnTabMoreOptionClicked()
    {
        if (IsSceneTransitionBlocking())
            return;

        SetTabsInteractable(false);
        HideOtherModalScreens(ScreenId.FeaturesHub);

        if (UIManager.Instance != null &&
            UIManager.Instance.HasScreen(ScreenId.FeaturesHub) &&
            UIManager.Instance.TryShowScreen(ScreenId.FeaturesHub))
        {
            RefreshTabVisuals(Tab.MoreOption);
            SetTabsInteractable(true);
            return;
        }

        Debug.LogWarning(
            "[NavigationHUD] Ecran FeaturesHub introuvable dans UIManager. " +
            "TabMoreOption actif sans hub (V0). Bind le prefab plus tard.");
        RefreshTabVisuals(Tab.MoreOption);
        SetTabsInteractable(true);
    }

    /// <summary>
    /// En mode exit-only : notifie la scène gameplay pour qu'elle gère le retour.
    /// En mode nav bar : sans effet (les tabs gèrent la navigation).
    /// </summary>
    public void OnExitClicked()
    {
        if (exitButtonContainer != null && exitButtonContainer.activeSelf)
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
        if (tabMoreOptionButton != null)
            tabMoreOptionButton.interactable = interactable;
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
        {
            ApplyMode(HudMode.Hidden);
            return;
        }

        if (boundNavigator != null)
            HandleSceneShown(boundNavigator.CurrentScene);
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

    /// <summary>
    /// HUD aventure : barre d'onglets masquée, croix de sortie active.
    /// À appeler depuis le contrôleur de la scène gameplay (ex. FirstLvl).
    /// </summary>
    public void EnterAdventureMode()
    {
        ApplyMode(HudMode.ExitOnly);
    }

    private void ApplyMode(HudMode mode)
    {
        currentMode = mode;

        if (navBarContainer != null)
            navBarContainer.SetActive(mode == HudMode.Navigation);

        if (exitButtonContainer != null)
            exitButtonContainer.SetActive(mode == HudMode.ExitOnly);

        if (mode == HudMode.Navigation)
        {
            RefreshTabVisuals();
            RebuildNavBarLayoutImmediate();
        }
    }

    private void RefreshTabVisuals()
    {
        bool onInventaire = UIManager.Instance != null &&
                            UIManager.Instance.IsScreenVisible(ScreenId.Inventory);
        bool onShop = UIManager.Instance != null &&
                      UIManager.Instance.IsScreenVisible(ScreenId.Shop);
        bool onSaleChannels = UIManager.Instance != null &&
                              UIManager.Instance.IsScreenVisible(ScreenId.SaleChannels);
        bool onMoreOption = UIManager.Instance != null &&
                            UIManager.Instance.HasScreen(ScreenId.FeaturesHub) &&
                            UIManager.Instance.IsScreenVisible(ScreenId.FeaturesHub);

        if (onMoreOption)
        {
            RefreshTabVisuals(Tab.MoreOption);
            return;
        }

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
        ApplyNavTabMockupVisual(BuildAventuresMockupRefs(), tabAventuresMockupRest, active == Tab.Aventures);
        ApplyNavTabMockupVisual(BuildInventaireMockupRefs(), tabInventaireMockupRest, active == Tab.Inventaire);
        ApplyNavTabMockupVisual(BuildShopMockupRefs(), tabShopMockupRest, active == Tab.Shop);
        ApplyNavTabMockupVisual(BuildSaleChannelsMockupRefs(), tabSaleChannelsMockupRest, active == Tab.SaleChannels);
        ApplyNavTabMockupVisual(
            BuildMoreOptionMockupRefs(),
            tabMoreOptionMockupRest,
            active == Tab.MoreOption,
            allowActiveWithoutSprite: true,
            activeIconScaleMultiplier: lastNavTabActiveIconScaleMultiplier,
            activeIconLiftMultiplier: lastNavTabActiveLiftMultiplier);

        RebuildNavBarLayoutImmediate();
    }

    private NavTabMockupRefs BuildAventuresMockupRefs()
    {
        return new NavTabMockupRefs
        {
            Icon = tabAventuresIcon,
            SelectedFrame = tabAventuresSelectedFrame,
            IconLift = tabAventuresIconLift,
            Glow = tabAventuresGlow,
            LavaBackdrop = tabAventuresLavaBackdrop,
            Label = tabAventuresLabel
        };
    }

    private NavTabMockupRefs BuildInventaireMockupRefs()
    {
        return new NavTabMockupRefs
        {
            Icon = tabInventaireIcon,
            SelectedFrame = tabInventaireSelectedFrame,
            IconLift = tabInventaireIconLift,
            Glow = tabInventaireGlow,
            LavaBackdrop = tabInventaireLavaBackdrop,
            Label = tabInventaireLabel
        };
    }

    private NavTabMockupRefs BuildShopMockupRefs()
    {
        return new NavTabMockupRefs
        {
            Icon = tabShopIcon,
            SelectedFrame = tabShopSelectedFrame,
            IconLift = tabShopIconLift,
            Glow = tabShopGlow,
            LavaBackdrop = tabShopLavaBackdrop,
            Label = tabShopLabel
        };
    }

    private NavTabMockupRefs BuildSaleChannelsMockupRefs()
    {
        return new NavTabMockupRefs
        {
            Icon = tabSaleChannelsIcon,
            SelectedFrame = tabSaleChannelsSelectedFrame,
            IconLift = tabSaleChannelsIconLift,
            Glow = tabSaleChannelsGlow,
            LavaBackdrop = tabSaleChannelsLavaBackdrop,
            Label = tabSaleChannelsLabel
        };
    }

    private NavTabMockupRefs BuildMoreOptionMockupRefs()
    {
        return new NavTabMockupRefs
        {
            Icon = tabMoreOptionIcon,
            SelectedFrame = tabMoreOptionSelectedFrame,
            IconLift = tabMoreOptionIconLift,
            Glow = tabMoreOptionGlow,
            LavaBackdrop = tabMoreOptionLavaBackdrop,
            Label = tabMoreOptionLabel
        };
    }

    private static NavTabMockupRest CaptureNavTabMockupRest(
        Image icon,
        RectTransform iconLift,
        GameObject selectedFrame)
    {
        NavTabMockupRest rest = default;

        if (iconLift != null)
            rest.IconLiftAnchoredPosition = iconLift.anchoredPosition;

        if (icon != null)
            rest.IconAnchoredPosition = icon.rectTransform.anchoredPosition;

        if (selectedFrame != null)
        {
            RectTransform frameRect = selectedFrame.GetComponent<RectTransform>();
            if (frameRect != null)
            {
                rest.SelectedFrameAnchoredPosition = frameRect.anchoredPosition;
                rest.SelectedFrameSizeDelta = frameRect.sizeDelta;
            }
        }

        return rest;
    }

    /// <summary>Mockup nav : inactif gris, icône en bas ; actif = zoom, lueur, label, cadre étendu.</summary>
    private void ApplyNavTabMockupVisual(
        NavTabMockupRefs refs,
        NavTabMockupRest rest,
        bool isActive,
        bool allowActiveWithoutSprite = false,
        float activeIconScaleMultiplier = 1f,
        float activeIconLiftMultiplier = 1f)
    {
        bool hasSprite = refs.Icon != null && refs.Icon.sprite != null;
        bool showActiveFx = isActive && (hasSprite || allowActiveWithoutSprite);

        ApplyNavTabSelectedFrameLayout(refs.SelectedFrame, rest, showActiveFx);

        if (refs.LavaBackdrop != null)
            refs.LavaBackdrop.SetActive(useActiveTabLavaBackdrop && showActiveFx);

        if (refs.Glow != null)
        {
            bool showGlow = showActiveFx && hasSprite;
            refs.Glow.SetActive(showGlow);
            if (showGlow)
            {
                Image glowImage = refs.Glow.GetComponent<Image>();
                if (glowImage != null)
                {
                    if (activeTabGlowMaterial != null)
                        glowImage.material = activeTabGlowMaterial;
                    glowImage.color = new Color(
                        activeTabLabelColor.r,
                        activeTabLabelColor.g,
                        activeTabLabelColor.b,
                        activeTabGlowAlpha);
                }
            }
        }

        if (refs.Label != null)
        {
            refs.Label.gameObject.SetActive(showActiveFx);
            if (showActiveFx)
                ApplyNavTabLabelStyle(refs.Label);
        }

        if (refs.IconLift != null)
        {
            refs.IconLift.localScale = Vector3.one;
            float liftY = showActiveFx ? activeTabIconLiftY * activeIconLiftMultiplier : 0f;
            refs.IconLift.anchoredPosition = rest.IconLiftAnchoredPosition + new Vector2(0f, liftY);
        }

        if (refs.Icon == null)
            return;

        RectTransform iconRect = refs.Icon.rectTransform;
        float iconScale = showActiveFx ? activeTabIconScale * activeIconScaleMultiplier : 1f;
        iconRect.localScale = new Vector3(iconScale, iconScale, 1f);

        float iconOffsetY = showActiveFx ? activeTabIconLocalOffsetY * activeIconLiftMultiplier : 0f;
        iconRect.anchoredPosition = rest.IconAnchoredPosition + new Vector2(0f, iconOffsetY);

        if (!hasSprite)
        {
            refs.Icon.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        ApplyNavTabIconAppearance(refs.Icon, isActive, hasSprite);
    }

    private void ApplyNavTabSelectedFrameLayout(GameObject selectedFrame, NavTabMockupRest rest, bool showActiveFx)
    {
        if (selectedFrame == null)
            return;

        selectedFrame.SetActive(showActiveFx);

        RectTransform frameRect = selectedFrame.GetComponent<RectTransform>();
        if (frameRect == null)
            return;

        if (!showActiveFx)
        {
            frameRect.anchoredPosition = rest.SelectedFrameAnchoredPosition;
            frameRect.sizeDelta = rest.SelectedFrameSizeDelta;
            return;
        }

        frameRect.anchoredPosition = rest.SelectedFrameAnchoredPosition + new Vector2(0f, activeTabFrameActivePosY);
        frameRect.sizeDelta = rest.SelectedFrameSizeDelta + activeTabFrameSizeDeltaExpand;
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

    private void ConfigureNavTabGlowHierarchy(RectTransform iconLift, GameObject glow)
    {
        if (glow == null || iconLift == null)
            return;

        RectTransform glowRect = glow.transform as RectTransform;
        if (glowRect != null && glowRect.parent != iconLift)
        {
            glowRect.SetParent(iconLift, false);
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

        Image glowImage = glow.GetComponent<Image>();
        if (glowImage != null)
        {
            glowImage.raycastTarget = false;
            glowImage.sprite = GetUiWhiteSprite();
            if (activeTabGlowMaterial != null)
                glowImage.material = activeTabGlowMaterial;
        }
    }

    /// <summary>
    /// Les labels actifs ne doivent pas influencer la largeur des slots HLG (sinon le 5e onglet est écrasé).
    /// </summary>
    private static void PreventNavTabLabelsFromDrivingBarLayout(params TextMeshProUGUI[] labels)
    {
        foreach (TextMeshProUGUI label in labels)
        {
            if (label == null)
                continue;

            LayoutElement layout = label.GetComponent<LayoutElement>();
            if (layout == null)
                layout = label.gameObject.AddComponent<LayoutElement>();

            layout.ignoreLayout = true;
        }
    }

    private void RebuildNavBarLayoutImmediate()
    {
        if (navBarContainer == null)
            return;

        NormalizeNavTabRootTransforms();
        EnsureManualNavTabStripLayout();
    }

    private void EnsureManualNavTabStripLayout()
    {
        if (navBarContainer == null)
            return;

        if (!navBarHorizontalLayoutDisabled)
        {
            navBarHorizontalLayout = navBarContainer.GetComponent<HorizontalLayoutGroup>();
            if (navBarHorizontalLayout != null)
                navBarHorizontalLayout.enabled = false;
            navBarHorizontalLayoutDisabled = true;
        }

        ApplyEqualNavTabSlots();
    }

    /// <summary>5 colonnes égales en pixels — évite dérive HLG sur le dernier slot (Plus).</summary>
    private void ApplyEqualNavTabSlots()
    {
        RectTransform barRect = navBarContainer.transform as RectTransform;
        if (barRect == null)
            return;

        Canvas.ForceUpdateCanvases();
        float barHeight = barRect.rect.height;
        float innerWidth = barRect.rect.width - navBarHorizontalPadding * 2f;
        if (innerWidth <= 1f || barHeight <= 1f)
            return;

        float slotWidth = innerWidth / NavTabSlotCount;
        Button[] tabs =
        {
            tabAventuresButton,
            tabInventaireButton,
            tabShopButton,
            tabSaleChannelsButton,
            tabMoreOptionButton
        };

        for (int i = 0; i < tabs.Length; i++)
        {
            Button tabButton = tabs[i];
            if (tabButton == null)
                continue;

            RectTransform slot = tabButton.transform as RectTransform;
            LayoutNavTabSlot(slot, slotWidth, i);
        }
    }

    /// <summary>Slot ancré bas, hauteur fixe — le surplus de fond reste au-dessus (lift/zoom).</summary>
    private void LayoutNavTabSlot(RectTransform slot, float slotWidth, int slotIndex)
    {
        slot.anchorMin = new Vector2(0f, 0f);
        slot.anchorMax = new Vector2(0f, 0f);
        slot.pivot = new Vector2(0.5f, 0f);
        slot.sizeDelta = new Vector2(slotWidth, NavTabSlotHeight);
        float centerX = navBarHorizontalPadding + slotWidth * (slotIndex + 0.5f);
        slot.anchoredPosition = new Vector2(centerX, 0f);
    }

    private void NormalizeNavTabRootTransforms()
    {
        ResetNavTabRootTransform(tabAventuresButton);
        ResetNavTabRootTransform(tabInventaireButton);
        ResetNavTabRootTransform(tabShopButton);
        ResetNavTabRootTransform(tabSaleChannelsButton);
        ResetNavTabRootTransform(tabMoreOptionButton);
    }

    private static void ResetNavTabRootTransform(Button tabButton)
    {
        if (tabButton == null)
            return;

        RectTransform rect = tabButton.transform as RectTransform;
        if (rect == null)
            return;

        rect.localScale = Vector3.one;
    }

    private void ApplyNavTabLabelStyle(TextMeshProUGUI label)
    {
        if (label == null)
            return;

        label.color = activeTabLabelColor;
        label.fontSize = activeTabLabelFontSize;
        label.fontStyle = FontStyles.Bold;

        // outlineWidth instancie le material via CanvasRenderer.
        // Sur un GO inactif, TMP n'a pas encore Awake → NRE (label Plus sans fontMaterial).
        if (!label.isActiveAndEnabled || label.font == null)
            return;

        label.outlineWidth = activeTabLabelOutlineWidth;
        label.outlineColor = Color.black;
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
        if (UIManager.Instance.HasScreen(ScreenId.FeaturesHub))
            UIManager.Instance.HideScreen(ScreenId.FeaturesHub);
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
        if (keepVisibleScreenId != ScreenId.FeaturesHub &&
            UIManager.Instance.HasScreen(ScreenId.FeaturesHub))
            UIManager.Instance.HideScreen(ScreenId.FeaturesHub);
    }

    private enum Tab { Aventures, Inventaire, Shop, SaleChannels, MoreOption }
}
