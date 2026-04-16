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
        // Injection du singleton PlayerInventory dans InventoryUI.
        // Ce Start() est appelé une seule fois à l'instanciation du prefab par UIManager.
        if (inventoryUI != null && PlayerInventory.Instance != null)
            inventoryUI.Bind(PlayerInventory.Instance);
    }

    private void OnEnable()
    {
        // Rafraîchit l'affichage à chaque ré-activation de l'écran.
        inventoryUI?.Refresh();
        NavigationHUD.ShowNavBar();
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
