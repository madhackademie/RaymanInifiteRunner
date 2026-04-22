using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Contrôleur UI central persistant entre scènes.
/// Gère les états Menu / Loading / Gameplay et masque les canvases selon le contexte.
/// </summary>
public sealed class UIFlowController : MonoBehaviour
{
    private enum UIFlowState
    {
        Menu,
        Loading,
        Gameplay
    }

    private const string DefaultMenuSceneName = "SampleScene";
    private const string DefaultGameplaySceneName = "FirstLvl";
    private const int LoadingSortingOrder = 1000;
    private static readonly Color LoadingOverlayColor = new(0f, 0f, 0f, 0.65f);

    [SerializeField] private string menuSceneName = DefaultMenuSceneName;
    [SerializeField] private string gameplaySceneName = DefaultGameplaySceneName;

    private static UIFlowController instance;

    private Canvas loadingCanvas;
    private Coroutine loadingRoutine;
    private UIFlowState currentState;

    public static bool TryGet(out UIFlowController controller)
    {
        controller = EnsureInstance();
        return controller != null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    private static UIFlowController EnsureInstance()
    {
        if (instance != null)
            return instance;

        GameObject root = new("UIFlowController");
        instance = root.AddComponent<UIFlowController>();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildLoadingOverlay();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        SetState(ResolveState(SceneManager.GetActiveScene().name));
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    /// <summary>API simple appelée depuis le menu principal.</summary>
    public void LoadGameplay()
    {
        LoadSceneWithTransition(gameplaySceneName);
    }

    /// <summary>Charge une scène avec affichage du mode Loading.</summary>
    public void LoadSceneWithTransition(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[UIFlowController] Scene name is empty.");
            return;
        }

        if (loadingRoutine != null)
            StopCoroutine(loadingRoutine);

        loadingRoutine = StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        SetState(UIFlowState.Loading);

        // Laisse un frame au canvas de loading pour s'afficher avant le chargement.
        yield return null;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (asyncLoad == null)
        {
            Debug.LogError($"[UIFlowController] Unable to load scene '{sceneName}'.");
            SetState(ResolveState(SceneManager.GetActiveScene().name));
            loadingRoutine = null;
            yield break;
        }

        while (!asyncLoad.isDone)
            yield return null;

        loadingRoutine = null;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode _)
    {
        SetState(ResolveState(scene.name));
    }

    private UIFlowState ResolveState(string sceneName)
    {
        if (sceneName == menuSceneName)
            return UIFlowState.Menu;

        return UIFlowState.Gameplay;
    }

    private void SetState(UIFlowState newState)
    {
        currentState = newState;
        SetLoadingVisible(currentState == UIFlowState.Loading);
        ApplySceneCanvasVisibility();
    }

    private void SetLoadingVisible(bool isVisible)
    {
        if (loadingCanvas != null)
            loadingCanvas.gameObject.SetActive(isVisible);
    }

    private void ApplySceneCanvasVisibility()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        bool hasMenuCanvasInScene = false;

        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null || IsPersistentLoadingCanvas(canvas))
                continue;

            if (canvas.GetComponentInChildren<MainMenuUI>(true) != null)
                hasMenuCanvasInScene = true;
        }

        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null || IsPersistentLoadingCanvas(canvas))
                continue;

            bool isMenuCanvas = canvas.GetComponentInChildren<MainMenuUI>(true) != null;
            bool shouldBeActive = ShouldCanvasBeVisible(hasMenuCanvasInScene, isMenuCanvas);

            if (canvas.gameObject.activeSelf != shouldBeActive)
                canvas.gameObject.SetActive(shouldBeActive);
        }
    }

    private bool ShouldCanvasBeVisible(bool hasMenuCanvasInScene, bool isMenuCanvas)
    {
        if (currentState == UIFlowState.Loading)
            return false;

        if (!hasMenuCanvasInScene)
            return true;

        if (currentState == UIFlowState.Menu)
            return isMenuCanvas;

        return !isMenuCanvas;
    }

    private bool IsPersistentLoadingCanvas(Canvas canvas)
    {
        return loadingCanvas != null && canvas == loadingCanvas;
    }

    private void BuildLoadingOverlay()
    {
        GameObject overlayRoot = new("UILoadingOverlay");
        overlayRoot.transform.SetParent(transform, false);

        loadingCanvas = overlayRoot.AddComponent<Canvas>();
        loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        loadingCanvas.sortingOrder = LoadingSortingOrder;

        overlayRoot.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = overlayRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject panel = new("Background");
        panel.transform.SetParent(overlayRoot.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = LoadingOverlayColor;

        SetLoadingVisible(false);
    }
}
