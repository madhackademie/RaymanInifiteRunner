using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Contrôleur UI global persistant.
/// Construit un canvas unique puis y migre les canvases de scène.
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
    private const string OverlayCanvasName = "UIRootCanvas";
    private const string MenuLayerName = "MenuLayer";
    private const string GameplayLayerName = "GameplayLayer";
    private const string LoadingLayerName = "LoadingLayer";
    private const int GlobalCanvasSortingOrder = 1000;
    private static readonly Color LoadingOverlayColor = new(0f, 0f, 0f, 0.65f);

    [SerializeField] private string menuSceneName = DefaultMenuSceneName;
    [SerializeField] private string gameplaySceneName = DefaultGameplaySceneName;

    private static UIFlowController instance;

    private readonly List<GameObject> importedSceneUiRoots = new();
    private Coroutine loadingRoutine;
    private Canvas globalCanvas;
    private RectTransform menuLayer;
    private RectTransform gameplayLayer;
    private GameObject loadingPanel;
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

        BuildGlobalCanvas();
        EnsureEventSystem();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        ImportSceneUI(SceneManager.GetActiveScene());
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
        yield return null;

        ClearImportedSceneUi();

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
        EnsureEventSystem();
        ImportSceneUI(scene);
        SetState(ResolveState(scene.name));
    }

    private UIFlowState ResolveState(string sceneName)
    {
        return sceneName == menuSceneName ? UIFlowState.Menu : UIFlowState.Gameplay;
    }

    private void SetState(UIFlowState newState)
    {
        currentState = newState;

        bool isLoading = currentState == UIFlowState.Loading;
        bool isMenu = currentState == UIFlowState.Menu;
        bool isGameplay = currentState == UIFlowState.Gameplay;

        menuLayer.gameObject.SetActive(isMenu);
        gameplayLayer.gameObject.SetActive(isGameplay);
        loadingPanel.SetActive(isLoading);
    }

    private void ImportSceneUI(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return;

        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
            ImportCanvasesFromRoot(roots[i]);
    }

    private void ImportCanvasesFromRoot(GameObject root)
    {
        Canvas[] canvases = root.GetComponentsInChildren<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null || canvas == globalCanvas || canvas.renderMode == RenderMode.WorldSpace || IsNestedCanvas(canvas))
                continue;

            GameObject canvasRoot = canvas.gameObject;
            if (importedSceneUiRoots.Contains(canvasRoot))
                continue;

            bool isMenuUi = canvas.GetComponentInChildren<MainMenuUI>(true) != null;
            RectTransform targetLayer = isMenuUi ? menuLayer : gameplayLayer;

            canvasRoot.transform.SetParent(targetLayer, false);
            StripCanvasComponentsInHierarchy(canvasRoot.transform);
            StretchToParent(canvasRoot);

            importedSceneUiRoots.Add(canvasRoot);
        }
    }

    private static bool IsNestedCanvas(Canvas canvas)
    {
        Transform parent = canvas.transform.parent;
        return parent != null && parent.GetComponentInParent<Canvas>() != null;
    }

    private static void StripCanvasComponentsInHierarchy(Transform root)
    {
        Canvas[] canvases = root.GetComponentsInChildren<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
                Destroy(canvas);
        }

        CanvasScaler[] scalers = root.GetComponentsInChildren<CanvasScaler>(true);
        for (int i = 0; i < scalers.Length; i++)
        {
            CanvasScaler scaler = scalers[i];
            Canvas parentCanvas = scaler != null ? scaler.GetComponent<Canvas>() : null;
            if (scaler != null && (parentCanvas == null || parentCanvas.renderMode != RenderMode.WorldSpace))
                Destroy(scaler);
        }

        GraphicRaycaster[] raycasters = root.GetComponentsInChildren<GraphicRaycaster>(true);
        for (int i = 0; i < raycasters.Length; i++)
        {
            GraphicRaycaster raycaster = raycasters[i];
            Canvas parentCanvas = raycaster != null ? raycaster.GetComponent<Canvas>() : null;
            if (raycaster != null && (parentCanvas == null || parentCanvas.renderMode != RenderMode.WorldSpace))
                Destroy(raycaster);
        }
    }

    private static void StretchToParent(GameObject root)
    {
        RectTransform rect = root.GetComponent<RectTransform>();
        if (rect == null)
            return;

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private void ClearImportedSceneUi()
    {
        for (int i = 0; i < importedSceneUiRoots.Count; i++)
        {
            GameObject uiRoot = importedSceneUiRoots[i];
            if (uiRoot != null)
                Destroy(uiRoot);
        }

        importedSceneUiRoots.Clear();
    }

    private void BuildGlobalCanvas()
    {
        GameObject canvasRoot = new(OverlayCanvasName);
        canvasRoot.transform.SetParent(transform, false);

        globalCanvas = canvasRoot.AddComponent<Canvas>();
        globalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        globalCanvas.sortingOrder = GlobalCanvasSortingOrder;

        canvasRoot.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        menuLayer = CreateLayer(MenuLayerName, canvasRoot.transform);
        gameplayLayer = CreateLayer(GameplayLayerName, canvasRoot.transform);
        RectTransform loadingLayer = CreateLayer(LoadingLayerName, canvasRoot.transform);

        loadingPanel = CreateLoadingPanel(loadingLayer);
        loadingPanel.SetActive(false);
    }

    private static RectTransform CreateLayer(string layerName, Transform parent)
    {
        GameObject layer = new(layerName);
        layer.transform.SetParent(parent, false);

        RectTransform rect = layer.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static GameObject CreateLoadingPanel(Transform parent)
    {
        GameObject panel = new("LoadingOverlay");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = LoadingOverlayColor;
        return panel;
    }

    private static void EnsureEventSystem()
    {
        EventSystem[] systems = FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (systems.Length == 0)
        {
            GameObject root = new("UIEventSystem");
            root.AddComponent<EventSystem>();
            root.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(root);
            return;
        }

        EventSystem primary = systems[0];
        for (int i = 1; i < systems.Length; i++)
        {
            if (systems[i] != null && systems[i].gameObject.scene.name == "DontDestroyOnLoad")
            {
                primary = systems[i];
                break;
            }
        }

        for (int i = 0; i < systems.Length; i++)
        {
            EventSystem candidate = systems[i];
            if (candidate != null && candidate != primary)
                Destroy(candidate.gameObject);
        }

        if (primary.GetComponent<BaseInputModule>() == null)
            primary.gameObject.AddComponent<StandaloneInputModule>();

        if (primary.gameObject.scene.name != "DontDestroyOnLoad")
            DontDestroyOnLoad(primary.gameObject);
    }
}
