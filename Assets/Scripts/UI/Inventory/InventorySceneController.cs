using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur du panneau inventaire.
/// La scène Inventaire reste chargée en permanence — son Canvas est affiché/masqué
/// par SceneNavigator via SetActive sur les racines.
/// OnEnable gère le bind et le refresh à chaque ré-activation.
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

        NavigationHUD.ShowNavBar();
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

    /// <summary>
    /// Affiche la scène inventaire via SceneNavigator.
    /// Peut être appelé depuis n'importe quelle scène.
    /// </summary>
    public static void Open()
    {
        SceneNavigator.Instance?.ShowScene(SceneId.Inventaire);
    }

    /// <summary>Retourne à HomeScene via SceneNavigator.</summary>
    public void Close()
    {
        SceneNavigator.Instance?.ShowScene(SceneId.HomeScene);
    }
}
