using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the Inventaire scene lifecycle: open from any scene via LoadAdditive
/// and close by unloading the scene.
/// </summary>
public class InventorySceneController : MonoBehaviour
{
    private const string InventorySceneName = "Inventaire";

    [Header("References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        // Inject the PlayerInventory singleton into InventoryUI at runtime,
        // since PlayerInventory lives in DontDestroyOnLoad and cannot be
        // referenced in the Inspector across scenes.
        if (inventoryUI != null && PlayerInventory.Instance != null)
            inventoryUI.Bind(PlayerInventory.Instance);
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Loads the Inventaire scene additively on top of the current scene.
    /// Safe to call from any other scene.
    /// </summary>
    public static void Open()
    {
        if (SceneManager.GetSceneByName(InventorySceneName).isLoaded)
            return;

        SceneManager.LoadScene(InventorySceneName, LoadSceneMode.Additive);
    }

    /// <summary>Unloads the Inventaire scene.</summary>
    public void Close()
    {
        SceneManager.UnloadSceneAsync(InventorySceneName);
    }
}
