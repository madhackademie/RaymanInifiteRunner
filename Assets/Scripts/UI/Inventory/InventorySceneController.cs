using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur du panneau inventaire. Vit sur le prefab InventoryPanel.
/// L'ouverture/fermeture est déléguée à UIManager — aucun chargement de scène.
/// </summary>
public class InventorySceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Button closeButton;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        // Start() est appelé après tous les Awake() — PlayerInventory.Instance est garanti prêt ici.
        // C'est le seul endroit qui déclenche le bind initial et BuildSlots.
        Debug.Log($"[InventorySceneController] Start — PlayerInventory.Instance = {(PlayerInventory.Instance != null ? "OK" : "NULL")}, inventoryUI = {(inventoryUI != null ? "OK" : "NULL")}");
        TryBindInventory();
    }

    private void OnEnable()
    {
        Debug.Log($"[InventorySceneController] OnEnable — IsBound={inventoryUI?.IsBound}");
        // Si déjà bindé (ré-activation sans rechargement), un simple Refresh suffit.
        // Le bind initial est géré dans Start().
        if (inventoryUI != null && inventoryUI.IsBound)
            inventoryUI.Refresh();
        NavigationHUD.ShowNavBar();
    }

    private void TryBindInventory()
    {
        if (inventoryUI == null || PlayerInventory.Instance == null)
        {
            Debug.LogWarning($"[InventorySceneController] TryBindInventory échoué — inventoryUI={inventoryUI != null}, Instance={PlayerInventory.Instance != null}");
            return;
        }

        Debug.Log($"[InventorySceneController] Bind OK — slots count: {PlayerInventory.Instance.Slots.Count}");
        inventoryUI.Bind(PlayerInventory.Instance);
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Ouvre le panneau inventaire via UIManager.
    /// Peut être appelé depuis n'importe quelle scène.
    /// </summary>
    public static void Open()
    {
        UIManager.Instance?.ShowScreen(ScreenId.Inventory);
    }

    /// <summary>Ferme le panneau inventaire via UIManager.</summary>
    public void Close()
    {
        UIManager.Instance?.HideScreen(ScreenId.Inventory);
    }
}
