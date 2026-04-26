using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur legacy du panneau inventaire.
/// Conserve le bind/refresh pour les anciennes instances de panel,
/// mais l'ouverture/fermeture runtime passe désormais par UIManager.
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

    private void OnEnable()
    {
        // OnEnable est appelé à chaque SetActive(true) sur la racine de la scène.
        // On bind si ce n'est pas encore fait, sinon on rafraîchit simplement.
        if (inventoryUI == null)
            return;

        if (!inventoryUI.IsBound)
            TryBindInventory();
        else
            inventoryUI.Refresh();

    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void TryBindInventory()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogWarning("[InventorySceneController] PlayerInventory.Instance introuvable.");
            return;
        }

        inventoryUI.Bind(PlayerInventory.Instance);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Affiche l'écran inventaire global via UIManager.</summary>
    public static void Open()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ShowScreen(ScreenId.Inventory);
    }

    /// <summary>Retourne à HomeScene via SceneNavigator.</summary>
    public void Close()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsScreenVisible(ScreenId.Inventory))
        {
            UIManager.Instance.HideScreen(ScreenId.Inventory);
            return;
        }

        _ = SceneNavigator.Instance?.ShowScene(SceneId.HomeScene);
    }
}
