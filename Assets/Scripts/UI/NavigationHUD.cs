using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barre de navigation persistante du shell UI.
/// Gère deux modes d'affichage : barre de navigation complète ou bouton de sortie seul.
/// La navigation entre écrans est entièrement déléguée à UIManager.
/// </summary>
public class NavigationHUD : MonoBehaviour
{

    // ── Singleton ─────────────────────────────────────────────────────────────

    /// <summary>Singleton instance. Available from any scene after first load.</summary>
    public static NavigationHUD Instance { get; private set; }

    // ── Inspector references ──────────────────────────────────────────────────

    [Header("Mode Containers")]
    [SerializeField] private GameObject navBarContainer;
    [SerializeField] private GameObject exitButtonContainer;

    [Header("Nav Bar Buttons")]
    [SerializeField] private Button tabAventuresButton;
    [SerializeField] private Button tabInventaireButton;

    [Header("Tab Icons (teintées selon sélection)")]
    [SerializeField] private Image tabAventuresIcon;
    [SerializeField] private Image tabInventaireIcon;

    [Header("Exit Button")]
    [SerializeField] private Button exitButton;

    [Header("Couleurs de tab")]
    [SerializeField] private Color colorActive   = new Color(1f,   0.78f, 0.2f,  1f);
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
        DontDestroyOnLoad(gameObject);

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

    // ── Public display API ────────────────────────────────────────────────────

    /// <summary>
    /// Shows the full navigation bar.
    /// Call this from feature scenes: Inventaire, Marché, Talents, etc.
    /// </summary>
    public static void ShowNavBar()
    {
        if (Instance == null) return;
        Instance.navBarContainer.SetActive(true);
        Instance.exitButtonContainer.SetActive(false);
        Instance.RefreshTabVisuals();
    }

    /// <summary>
    /// Shows only the small exit button (red cross).
    /// Call this from full-screen scenes: FirstLvl gameplay, etc.
    /// </summary>
    public static void ShowExitOnly()
    {
        if (Instance == null) return;
        Instance.navBarContainer.SetActive(false);
        Instance.exitButtonContainer.SetActive(true);
    }

    /// <summary>
    /// Hides the HUD entirely.
    /// Call this from cinematics, splash screens, loading screens, etc.
    /// </summary>
    public static void Hide()
    {
        if (Instance == null) return;
        Instance.navBarContainer.SetActive(false);
        Instance.exitButtonContainer.SetActive(false);
    }

    // ── Tab callbacks ─────────────────────────────────────────────────────────

    /// <summary>Retour au gameplay : masque tous les écrans, passe en mode exit-only.</summary>
    public void OnTabAventuresClicked()
    {
        UIManager.Instance?.HideAllGlobalUI();
        ShowExitOnly();
    }

    /// <summary>Ouvre l'inventaire via UIManager et affiche la nav bar complète.</summary>
    public void OnTabInventaireClicked()
    {
        UIManager.Instance?.ShowScreen(ScreenId.Inventory);
        ShowNavBar();
        RefreshTabVisuals(Tab.Inventaire);
    }

    /// <summary>Ferme tous les écrans et repasse en mode exit-only (gameplay).</summary>
    public void OnExitClicked()
    {
        UIManager.Instance?.HideAllGlobalUI();
        ShowExitOnly();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Déduit le tab actif depuis UIManager et met à jour les icônes.</summary>
    private void RefreshTabVisuals()
    {
        bool inventoryVisible = UIManager.Instance != null &&
                                UIManager.Instance.IsScreenVisible(ScreenId.Inventory);
        RefreshTabVisuals(inventoryVisible ? Tab.Inventaire : Tab.Aventures);
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

    // ── Tab enum ──────────────────────────────────────────────────────────────

    private enum Tab { Aventures, Inventaire }
}
