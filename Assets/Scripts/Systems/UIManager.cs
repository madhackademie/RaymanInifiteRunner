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
///
/// Inventaire HUD : prefab + <see cref="InventoryUI"/> (pas de logique d’écran dans ce fichier).
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>Hauteur réservée en bas de l'écran pour la nav bar du HUD (en unités canvas).</summary>
    private const float NavBarHeight = 120f;

    // ── Singleton ─────────────────────────────────────────────────────────────

    /// <summary>Instance singleton disponible depuis toute scène après le boot.</summary>
    public static UIManager Instance { get; private set; }

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Shell — parent des écrans instanciés")]
    [Tooltip("Transform enfant du Canvas shell sous lequel tous les écrans sont instanciés.")]
    [SerializeField] private Transform screenRoot;

    [Header("Modales HUD (Shop, Inventaire, …)")]
    [Tooltip("Optionnel — sinon Resources/Shop/shop_backdrop. N’affecte pas FirstLvl (scène gameplay séparée).")]
    [SerializeField] private Sprite hudModalBackdropSprite;

    [SerializeField] private Color hudModalBackdropTint = Color.white;

    [Tooltip("Tri au-dessus du canvas HUD (souvent 50) et de la Home (0).")]
    [SerializeField] private int hudModalCanvasSortingOrder = 400;

    /// <summary>Sprite de fond des panneaux plein écran UIManager (prioritaire sur Resources).</summary>
    public Sprite HudModalBackdropSprite => hudModalBackdropSprite;

    /// <summary>Teinte du sprite de fond quand une texture est utilisée.</summary>
    public Color HudModalBackdropTint => hudModalBackdropTint;

    public int HudModalCanvasSortingOrder => hudModalCanvasSortingOrder;

    [Header("Écrans prioritaires — préchargés au démarrage")]
    [SerializeField] private List<ScreenEntry> priorityScreens = new();

    [Header("Écrans secondaires — lazy load à la première demande")]
    [SerializeField] private List<ScreenEntry> secondaryScreens = new();
    
    [Header("Runtime bindings")]
    [Tooltip("Prefab visuel d'un slot inventaire pour les ecrans Inventory configures dans l'editeur.")]
    [SerializeField] private InventorySlotUI runtimeInventorySlotPrefab;
    [SerializeField] private int runtimeInventoryColumns = 5;
    [Tooltip("Prefab visuel d'un slot shop. Si non defini, utilise le prefab inventaire.")]
    [SerializeField] private InventorySlotUI runtimeShopSlotPrefab;
    [SerializeField] private int runtimeShopColumns = 5;
    [Tooltip("Compatibilite legacy shop. Utilise si aucun binding PopupId.ShopItemPurchase n'est configure.")]
    [SerializeField] private ShopItemPopupController shopItemPopupPrefab;

    [Header("Runtime popup bindings")]
    [Tooltip("Catalogue ecran -> popup id -> prefab instancie a la demande dans ScreenPopupHost.")]
    [SerializeField] private List<ScreenPopupBinding> runtimePopupBindings = new();

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
        // Dernier enfant de screenRoot = dessiné au-dessus des autres écrans (Shop / Inventaire).
        entry.instance.transform.SetAsLastSibling();
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

        // Réserve la hauteur de la nav bar en bas pour que les boutons HUD restent accessibles.
        if (entry.instance.TryGetComponent<RectTransform>(out RectTransform rect))
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(rect.offsetMin.x, NavBarHeight);
            rect.offsetMax = new Vector2(rect.offsetMax.x, 0f);
        }

        WireScreenAfterInstantiate(entry);
    }

    /// <summary>
    /// Branche les écrans dont la logique attend une injection après instanciation du prefab.
    /// </summary>
    private void WireScreenAfterInstantiate(ScreenEntry entry)
    {
        if (entry.instance == null)
            return;

        RegisterRuntimePopups(entry);

        if (entry.screenId == ScreenId.Shop)
        {
            RuntimeShopScreen shop = entry.instance.GetComponentInChildren<RuntimeShopScreen>(true);
            if (shop != null)
            {
                InventorySlotUI slotPrefab = runtimeShopSlotPrefab != null ? runtimeShopSlotPrefab : runtimeInventorySlotPrefab;
                ItemDatabase shopDb = PlayerInventory.Instance != null ? PlayerInventory.Instance.ItemDatabase : null;
                shop.Initialize(shopDb, slotPrefab, runtimeShopColumns);
            }

            return;
        }

        if (entry.screenId == ScreenId.Inventory)
        {
            InventoryUI inventoryUi = entry.instance.GetComponentInChildren<InventoryUI>(true);
            if (inventoryUi == null)
                return;

            inventoryUi.ApplyShellSlotSettings(runtimeInventorySlotPrefab, runtimeInventoryColumns);

            if (!inventoryUi.IsBound && PlayerInventory.Instance != null)
                inventoryUi.Bind(PlayerInventory.Instance);
        }
    }

    private void RegisterRuntimePopups(ScreenEntry entry)
    {
        if (entry.instance == null)
            return;

        bool hasBinding = HasPopupBindingForScreen(entry.screenId);
        ScreenPopupHost host = entry.instance.GetComponentInChildren<ScreenPopupHost>(true);

        if (host == null && hasBinding)
            host = entry.instance.AddComponent<ScreenPopupHost>();

        if (host == null)
            return;

        foreach (ScreenPopupBinding binding in runtimePopupBindings)
        {
            if (binding == null || binding.screenId != entry.screenId || binding.popupPrefab == null)
                continue;

            host.RegisterRuntimePopup(binding.popupId, binding.popupPrefab);
        }

        RegisterLegacyShopPopup(entry.screenId, host);
    }

    private bool HasPopupBindingForScreen(string screenId)
    {
        foreach (ScreenPopupBinding binding in runtimePopupBindings)
        {
            if (binding != null && binding.screenId == screenId && binding.popupPrefab != null)
                return true;
        }

        return screenId == ScreenId.Shop && shopItemPopupPrefab != null;
    }

    private void RegisterLegacyShopPopup(string screenId, ScreenPopupHost host)
    {
        if (screenId != ScreenId.Shop || host == null || shopItemPopupPrefab == null)
            return;

        foreach (ScreenPopupBinding binding in runtimePopupBindings)
        {
            if (binding != null &&
                binding.screenId == ScreenId.Shop &&
                binding.popupId == PopupId.ShopItemPurchase &&
                binding.popupPrefab != null)
            {
                return;
            }
        }

        host.RegisterRuntimePopup(PopupId.ShopItemPurchase, shopItemPopupPrefab.gameObject);
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

}
