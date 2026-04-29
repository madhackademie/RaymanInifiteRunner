using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration sérialisable d'un écran UI géré par UIManager.
/// </summary>
[Serializable]
public class ScreenEntry
{
    [Tooltip("Identifiant unique de l'écran. Utiliser les constantes de ScreenId.")]
    public string screenId;

    [Tooltip("Prefab racine de l'écran UI instancié sous le ScreenRoot.")]
    public GameObject prefab;

    /// <summary>Instance runtime créée par UIManager. Non sérialisée.</summary>
    [NonSerialized] public GameObject instance;

    /// <summary>True si le prefab a été instancié au moins une fois.</summary>
    public bool IsLoaded => instance != null;

    /// <summary>True si l'écran est actuellement visible.</summary>
    public bool IsVisible => IsLoaded && instance.activeSelf;
}

/// <summary>
/// Gestionnaire UI global. Singleton persistant chargé dans la scène shell NavigationHUD.
/// Gère deux listes d'écrans :
///   - Prioritaires : préchargés au démarrage, affichés/masqués via SetActive.
///   - Secondaires  : chargés à la première demande puis conservés en mémoire.
///
/// Note d'usage:
/// Cette architecture "scene shell + écrans activés/désactivés" permet aussi
/// d'ajouter des overlays flottants non liés à une scène de gameplay précise:
///   - Popups de confirmation
///   - Notifications / toasts
///   - Fenêtres modales
/// </summary>
public class UIManager : MonoBehaviour
{

    // ── Singleton ─────────────────────────────────────────────────────────────

    /// <summary>Instance singleton disponible depuis toute scène après le boot.</summary>
    public static UIManager Instance { get; private set; }

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Shell — parent des écrans instanciés")]
    [Tooltip("Transform enfant du Canvas shell sous lequel tous les écrans sont instanciés.")]
    [SerializeField] private Transform screenRoot;

    [Header("Écrans prioritaires — préchargés au démarrage")]
    [SerializeField] private List<ScreenEntry> priorityScreens = new();

    [Header("Écrans secondaires — lazy load à la première demande")]
    [SerializeField] private List<ScreenEntry> secondaryScreens = new();
    
    [Header("Fallback runtime")]
    [Tooltip("Crée un écran inventaire runtime minimal si aucun écran Inventory n'est configuré dans les listes.")]
    [SerializeField] private bool autoCreateInventoryScreen = true;
    [Tooltip("Prefab visuel d'un slot inventaire pour le fallback runtime.")]
    [SerializeField] private InventorySlotUI runtimeInventorySlotPrefab;
    [SerializeField] private int runtimeInventoryColumns = 5;
    [Tooltip("Crée un écran shop runtime minimal si aucun écran Shop n'est configuré dans les listes.")]
    [SerializeField] private bool autoCreateShopScreen = true;
    [Tooltip("Prefab visuel d'un slot shop pour le fallback runtime. Si non défini, utilise le prefab inventaire.")]
    [SerializeField] private InventorySlotUI runtimeShopSlotPrefab;
    [SerializeField] private int runtimeShopColumns = 5;

    // ── Runtime ───────────────────────────────────────────────────────────────

    private readonly Dictionary<string, ScreenEntry> registry = new();
    private SceneNavigator boundNavigator;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildRegistry();
    }

    private void Start()
    {
        PreloadPriorityScreens();
        EnsureInventoryScreenAvailable();
        EnsureShopScreenAvailable();
    }

    private void OnEnable()
    {
        SceneNavigator.OnNavigatorAvailable += BindNavigator;
        SceneNavigator.OnNavigatorUnavailable += UnbindNavigator;

        if (SceneNavigator.Instance != null)
            BindNavigator(SceneNavigator.Instance);
    }

    private void OnDisable()
    {
        SceneNavigator.OnNavigatorAvailable -= BindNavigator;
        SceneNavigator.OnNavigatorUnavailable -= UnbindNavigator;
        UnbindNavigator();
    }


    // ── Preload API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Instancie (caché) tous les écrans de la liste prioritaire.
    /// Appelé automatiquement au Start de UIManager.
    /// </summary>
    public void PreloadPriorityScreens()
    {
        foreach (ScreenEntry entry in priorityScreens)
            EnsureInstantiated(entry);
    }

    /// <summary>
    /// Instancie (caché) un écran secondaire à la demande puis le conserve en mémoire.
    /// Appeler avant ShowScreen pour éviter la latence au premier affichage.
    /// </summary>
    public void PreloadScreenLazy(string screenId)
    {
        if (TryGetEntry(screenId, out ScreenEntry entry))
            EnsureInstantiated(entry);
    }

    // ── Display API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Affiche un écran. L'instancie si c'est la première demande (lazy load).
    /// </summary>
    public void ShowScreen(string screenId)
    {
        TryShowScreen(screenId);
    }

    /// <summary>
    /// Masque un écran sans le détruire. Le prefab reste en mémoire pour un ré-affichage rapide.
    /// </summary>
    public void HideScreen(string screenId)
    {
        TryHideScreen(screenId);
    }

    /// <summary>Masque tous les écrans gérés par UIManager sans les détruire.</summary>
    public void HideAllGlobalUI()
    {
        foreach (ScreenEntry entry in registry.Values)
        {
            if (entry.IsLoaded)
                entry.instance.SetActive(false);
        }
    }

    // ── Query API ─────────────────────────────────────────────────────────────

    /// <summary>Retourne true si l'écran est actuellement visible.</summary>
    public bool IsScreenVisible(string screenId)
        => TryGetEntry(screenId, out ScreenEntry entry) && entry.IsVisible;

    /// <summary>Retourne true si l'écran a déjà été instancié.</summary>
    public bool IsScreenLoaded(string screenId)
        => TryGetEntry(screenId, out ScreenEntry entry) && entry.IsLoaded;

    /// <summary>Retourne true si l'identifiant d'écran existe dans le registre.</summary>
    public bool HasScreen(string screenId) => registry.ContainsKey(screenId);

    /// <summary>Affiche un écran si disponible, retourne true en cas de succès.</summary>
    public bool TryShowScreen(string screenId)
    {
        if (!TryGetEntry(screenId, out ScreenEntry entry))
            return false;

        EnsureInstantiated(entry);
        if (entry.instance == null)
            return false;

        entry.instance.SetActive(true);
        return true;
    }

    /// <summary>Masque un écran si disponible, retourne true en cas de succès.</summary>
    public bool TryHideScreen(string screenId)
    {
        if (!TryGetEntry(screenId, out ScreenEntry entry) || !entry.IsLoaded)
            return false;

        entry.instance.SetActive(false);
        return true;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void BuildRegistry()
    {
        registry.Clear();

        foreach (ScreenEntry entry in priorityScreens)
            RegisterEntry(entry);

        foreach (ScreenEntry entry in secondaryScreens)
            RegisterEntry(entry);
    }

    private void RegisterEntry(ScreenEntry entry)
    {
        if (string.IsNullOrEmpty(entry.screenId))
        {
            Debug.LogWarning("[UIManager] ScreenEntry sans screenId ignorée.", this);
            return;
        }

        if (registry.ContainsKey(entry.screenId))
        {
            Debug.LogWarning($"[UIManager] screenId dupliqué : '{entry.screenId}'. Ignoré.", this);
            return;
        }

        registry[entry.screenId] = entry;
    }

    private void EnsureInstantiated(ScreenEntry entry)
    {
        if (entry.IsLoaded)
            return;

        if (entry.prefab == null)
        {
            Debug.LogWarning($"[UIManager] Prefab manquant pour '{entry.screenId}'.", this);
            return;
        }

        entry.instance = Instantiate(entry.prefab, screenRoot);
        entry.instance.SetActive(false);
    }

    private bool TryGetEntry(string screenId, out ScreenEntry entry)
    {
        if (registry.TryGetValue(screenId, out entry))
            return true;

        Debug.LogWarning($"[UIManager] Écran inconnu : '{screenId}'. Vérifier ScreenId et l'Inspector.", this);
        return false;
    }

    private void BindNavigator(SceneNavigator navigator)
    {
        if (navigator == null)
            return;

        if (boundNavigator != null && boundNavigator != navigator)
            UnbindNavigator();

        boundNavigator = navigator;
        boundNavigator.OnTransitionStateChanged -= HandleTransitionStateChanged;
        boundNavigator.OnTransitionStateChanged += HandleTransitionStateChanged;
        boundNavigator.OnAfterSceneShown -= HandleSceneShown;
        boundNavigator.OnAfterSceneShown += HandleSceneShown;
    }

    private void UnbindNavigator()
    {
        if (boundNavigator == null)
            return;

        boundNavigator.OnTransitionStateChanged -= HandleTransitionStateChanged;
        boundNavigator.OnAfterSceneShown -= HandleSceneShown;
        boundNavigator = null;
    }

    private void HandleTransitionStateChanged(bool isTransitioning)
    {
        if (isTransitioning)
            HideAllGlobalUI();
    }

    private void HandleSceneShown(string _)
    {
        // No-op: conservé pour garder le hook navigator et permettre des extensions futures.
    }

    private void EnsureInventoryScreenAvailable()
    {
        if (HasScreen(ScreenId.Inventory) || !autoCreateInventoryScreen || screenRoot == null)
            return;

        GameObject root = new($"{ScreenId.Inventory}Screen", typeof(RectTransform));
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.SetParent(screenRoot, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        RuntimeInventoryScreen runtimeScreen = root.AddComponent<RuntimeInventoryScreen>();
        runtimeScreen.Initialize(PlayerInventory.Instance, runtimeInventorySlotPrefab, runtimeInventoryColumns);
        root.SetActive(false);

        registry[ScreenId.Inventory] = new ScreenEntry
        {
            screenId = ScreenId.Inventory,
            prefab = null,
            instance = root
        };
    }

    private void EnsureShopScreenAvailable()
    {
        if (HasScreen(ScreenId.Shop) || !autoCreateShopScreen || screenRoot == null)
            return;

        GameObject root = new($"{ScreenId.Shop}Screen", typeof(RectTransform));
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.SetParent(screenRoot, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        RuntimeShopScreen runtimeScreen = root.AddComponent<RuntimeShopScreen>();
        InventorySlotUI slotPrefab = runtimeShopSlotPrefab != null ? runtimeShopSlotPrefab : runtimeInventorySlotPrefab;
        runtimeScreen.Initialize(PlayerInventory.Instance, slotPrefab, runtimeShopColumns);
        root.SetActive(false);

        registry[ScreenId.Shop] = new ScreenEntry
        {
            screenId = ScreenId.Shop,
            prefab = null,
            instance = root
        };
    }
}
