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

    [Header("Tab Icons")]
    [SerializeField] private Image tabAventuresIcon;
    [SerializeField] private Image tabInventaireIcon;

    [Header("Exit Button")]
    [SerializeField] private Button exitButton;

    [Header("Couleurs de tab")]
    [SerializeField] private Color colorActive   = new Color(1f,    0.78f, 0.2f,  1f);
    [SerializeField] private Color colorInactive = new Color(0.55f, 0.55f, 0.55f, 1f);

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        tabAventuresButton.onClick.AddListener(OnTabAventuresClicked);
        tabInventaireButton.onClick.AddListener(OnTabInventaireClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDestroy()
    {
        tabAventuresButton.onClick.RemoveListener(OnTabAventuresClicked);
        tabInventaireButton.onClick.RemoveListener(OnTabInventaireClicked);
        exitButton.onClick.RemoveListener(OnExitClicked);
    }

    // ── Display API ───────────────────────────────────────────────────────────

    /// <summary>Affiche la barre de navigation complète.</summary>
    public static void ShowNavBar()
    {
        if (Instance == null) return;
        Instance.navBarContainer.SetActive(true);
        Instance.exitButtonContainer.SetActive(false);
        Instance.RefreshTabVisuals();
    }

    /// <summary>Affiche uniquement le bouton de sortie (croix). Pour les scènes gameplay.</summary>
    public static void ShowExitOnly()
    {
        if (Instance == null) return;
        Instance.navBarContainer.SetActive(false);
        Instance.exitButtonContainer.SetActive(true);
    }

    /// <summary>Masque le HUD entièrement. Pour les cinématiques et l'écran de chargement.</summary>
    public static void Hide()
    {
        if (Instance == null) return;
        Instance.navBarContainer.SetActive(false);
        Instance.exitButtonContainer.SetActive(false);
    }

    // ── Tab callbacks ─────────────────────────────────────────────────────────

    /// <summary>Navigue vers HomeScene via SceneNavigator.</summary>
    public async void OnTabAventuresClicked()
    {
        if (SceneNavigator.Instance == null || SceneNavigator.Instance.IsTransitioning) return;
        SetTabsInteractable(false);
        ShowNavBar();
        await SceneNavigator.Instance.GoTo(SceneId.HomeScene);
        RefreshTabVisuals(Tab.Aventures);
        SetTabsInteractable(true);
    }

    /// <summary>Navigue vers la scène Inventaire via SceneNavigator.</summary>
    public async void OnTabInventaireClicked()
    {
        if (SceneNavigator.Instance == null || SceneNavigator.Instance.IsTransitioning) return;
        SetTabsInteractable(false);
        ShowNavBar();
        await SceneNavigator.Instance.GoTo(SceneId.Inventaire);
        RefreshTabVisuals(Tab.Inventaire);
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
        tabAventuresButton.interactable  = interactable;
        tabInventaireButton.interactable = interactable;
    }

    private void RefreshTabVisuals()
    {
        bool onInventaire = SceneNavigator.Instance != null &&
                            SceneNavigator.Instance.CurrentScene == SceneId.Inventaire;
        RefreshTabVisuals(onInventaire ? Tab.Inventaire : Tab.Aventures);
    }

    private void RefreshTabVisuals(Tab active)
    {
        SetIconColor(tabAventuresIcon,  active == Tab.Aventures);
        SetIconColor(tabInventaireIcon, active == Tab.Inventaire);
    }

    private void SetIconColor(Image icon, bool isActive)
    {
        if (icon != null)
            icon.color = isActive ? colorActive : colorInactive;
    }

    // ── Enums ─────────────────────────────────────────────────────────────────

    private enum Tab { Aventures, Inventaire }
}
