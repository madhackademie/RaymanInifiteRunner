using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Écran HUD des canaux de vente (écoulement production).
/// Shell placeholder — bandeaux scrollables et popups de vente : chantier Bezy + SaleChannelService.
/// </summary>
public class RuntimeSaleChannelsScreen : MonoBehaviour
{
    private const string PlaceholderBody =
        "Canaux de vente — voisinage, bandoulière, vélo…\n" +
        "Bandeaux interactifs à venir (Bezy).";

    [Header("Bindings UI (prefab)")]
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI bodyPlaceholderLabel;
    [SerializeField] private Image rootBackdropImage;

    private Button hookedCloseButton;

    private void Awake()
    {
        ResolveBindingsIfNeeded();
        HookCloseButton();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (bodyPlaceholderLabel != null)
            bodyPlaceholderLabel.text = PlaceholderBody;

        ApplyShellBackdrop();
    }

    private void ResolveBindingsIfNeeded()
    {
        if (closeButton == null)
            closeButton = transform.Find("Header/CloseButton")?.GetComponent<Button>();

        if (bodyPlaceholderLabel == null)
            bodyPlaceholderLabel = transform.Find("Body/PlaceholderLabel")?.GetComponent<TextMeshProUGUI>();

        if (rootBackdropImage == null)
            rootBackdropImage = GetComponent<Image>();
    }

    private void HookCloseButton()
    {
        if (closeButton == null || closeButton == hookedCloseButton)
            return;

        if (hookedCloseButton != null)
            hookedCloseButton.onClick.RemoveListener(HandleCloseClicked);

        hookedCloseButton = closeButton;
        hookedCloseButton.onClick.AddListener(HandleCloseClicked);
    }

    private void HandleCloseClicked()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.HideScreen(ScreenId.SaleChannels);
    }

    private void ApplyShellBackdrop()
    {
        if (UIManager.Instance == null || rootBackdropImage == null)
            return;

        Sprite backdrop = UIManager.Instance.HudModalBackdropSprite;
        if (backdrop == null)
            return;

        rootBackdropImage.sprite = backdrop;
        rootBackdropImage.color = UIManager.Instance.HudModalBackdropTint;
    }
}
