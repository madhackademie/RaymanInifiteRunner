using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Persistent bottom navigation bar loaded additively at game startup.
/// Survives all scene loads via DontDestroyOnLoad.
/// Controls its own visibility mode: full nav bar, exit-only, or hidden.
/// </summary>
public class NavigationHUD : MonoBehaviour
{
    // ── Scene names ───────────────────────────────────────────────────────────

    private const string SceneFirstLvl   = "FirstLvl";
    private const string SceneInventaire = "Inventaire";

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

    [Header("Tab Icons (tinted on selection)")]
    [SerializeField] private Image tabAventuresIcon;
    [SerializeField] private Image tabInventaireIcon;

    [Header("Exit Button")]
    [SerializeField] private Button exitButton;

    [Header("Tab Colors")]
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
        Instance.RefreshTabVisuals(GetCurrentTab());
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

    /// <summary>Closes the Inventaire overlay and returns to FirstLvl.</summary>
    public void OnTabAventuresClicked()
    {
        UnloadIfLoaded(SceneInventaire);
        LoadAdditive(SceneFirstLvl);
        RefreshTabVisuals(Tab.Aventures);
    }

    /// <summary>Opens the Inventaire scene additively over FirstLvl.</summary>
    public void OnTabInventaireClicked()
    {
        LoadAdditive(SceneFirstLvl);
        LoadAdditive(SceneInventaire);
        RefreshTabVisuals(Tab.Inventaire);
    }

    /// <summary>
    /// Exit button handler used in full-screen scenes.
    /// Unloads all additive feature scenes and returns to FirstLvl.
    /// </summary>
    public void OnExitClicked()
    {
        UnloadIfLoaded(SceneInventaire);
        LoadAdditive(SceneFirstLvl);
        ShowExitOnly();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void LoadAdditive(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    private static void UnloadIfLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.isLoaded)
            SceneManager.UnloadSceneAsync(scene);
    }

    private static Tab GetCurrentTab()
    {
        return SceneManager.GetSceneByName(SceneInventaire).isLoaded
            ? Tab.Inventaire
            : Tab.Aventures;
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
