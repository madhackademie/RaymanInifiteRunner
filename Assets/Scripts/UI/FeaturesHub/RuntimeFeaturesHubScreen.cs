using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Écran hub Plus — placeholder + accès Options (langue V1).
/// Bouton Options = prefab uniquement (SettingsButton).
/// </summary>
public class RuntimeFeaturesHubScreen : MonoBehaviour
{
    private const string DefaultTitle = "En construction";
    private const string DefaultSubtitle = "Bientôt disponible";

    [Header("Shell")]
    [SerializeField] private Image rootBackdropImage;
    [SerializeField] private Image contentBackdropImage;

    [Header("Placeholder centre")]
    [SerializeField] private GameObject placeholderRoot;
    [SerializeField] private Image comingSoonIcon;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI subtitleLabel;

    [Header("Options (prefab)")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private RectTransform optionsPanelHost;

    [Header("Copy (legacy — remplacé par i18n si LanguageManager présent)")]
    [SerializeField] private string titleOverride;
    [SerializeField] private string subtitleOverride;

    private GameObject _optionsPanelRoot;
    private RuntimeOptionsPanelView _optionsPanel;
    private bool _optionsUiBuilt;
    private bool _listenersBound;

    /// <summary>Appelé par UIManager après instanciation du prefab hub.</summary>
    public void EnsureInitialized()
    {
        ResolveReferences();
        BindListeners();
        EnsureOptionsPanelBuilt();
    }

    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnEnable()
    {
        ApplyBackdrop();
        ApplyPlaceholderCopy();
        ShowHubPlaceholder();
        if (settingsButton != null)
            settingsButton.gameObject.SetActive(true);
    }

    private void ResolveReferences()
    {
        if (placeholderRoot == null)
        {
            Transform placeholder = transform.Find("PlaceholderRoot");
            if (placeholder != null)
                placeholderRoot = placeholder.gameObject;
        }

        if (settingsButton == null)
            settingsButton = transform.Find("SettingsButton")?.GetComponent<Button>();

        if (optionsPanelHost == null)
            optionsPanelHost = transform.Find("OptionsPanelHost") as RectTransform;
    }

    private void BindListeners()
    {
        if (_listenersBound || settingsButton == null)
            return;

        settingsButton.onClick.RemoveListener(ShowOptionsPanel);
        settingsButton.onClick.AddListener(ShowOptionsPanel);
        _listenersBound = true;
    }

    private void EnsureOptionsPanelBuilt()
    {
        if (_optionsUiBuilt)
            return;

        ResolveReferences();

        if (settingsButton == null)
        {
            Debug.LogError("[RuntimeFeaturesHubScreen] SettingsButton manquant sur FeaturesHubScreen.prefab.", this);
            return;
        }

        if (optionsPanelHost == null)
        {
            Debug.LogError("[RuntimeFeaturesHubScreen] OptionsPanelHost manquant sur FeaturesHubScreen.prefab.", this);
            return;
        }

        if (optionsPanelHost.childCount > 0)
        {
            _optionsPanel = optionsPanelHost.GetComponentInChildren<RuntimeOptionsPanelView>(true);
            _optionsPanelRoot = _optionsPanel != null ? _optionsPanel.gameObject : null;
            if (_optionsPanel != null)
            {
                _optionsPanel.CloseRequested += ShowHubPlaceholder;
                _optionsUiBuilt = true;
            }

            return;
        }

        TMP_FontAsset font = titleLabel != null ? titleLabel.font : null;
        _optionsPanelRoot = FeaturesHubOptionsUiBuilder.BuildOptionsPanel(optionsPanelHost, font, out _optionsPanel);
        if (_optionsPanel == null)
            return;

        _optionsPanel.CloseRequested += ShowHubPlaceholder;
        _optionsUiBuilt = true;
    }

    private void ShowOptionsPanel()
    {
        EnsureOptionsPanelBuilt();
        if (!_optionsUiBuilt || _optionsPanelRoot == null)
            return;

        if (placeholderRoot != null)
            placeholderRoot.SetActive(false);

        if (settingsButton != null)
            settingsButton.gameObject.SetActive(false);

        optionsPanelHost.gameObject.SetActive(true);
    }

    private void ShowHubPlaceholder()
    {
        if (optionsPanelHost != null)
            optionsPanelHost.gameObject.SetActive(false);

        if (placeholderRoot != null)
            placeholderRoot.SetActive(true);

        if (settingsButton != null)
            settingsButton.gameObject.SetActive(true);

        ApplyPlaceholderCopy();
    }

    private void ApplyBackdrop()
    {
        if (rootBackdropImage != null)
        {
            HudModalBackdrop.ApplyRootBackground(rootBackdropImage);
            Color c = rootBackdropImage.color;
            c.a = 1f;
            rootBackdropImage.color = c;
        }

        if (contentBackdropImage != null)
            HudModalBackdrop.ApplyContentPanel(contentBackdropImage);
    }

    private void ApplyPlaceholderCopy()
    {
        if (LanguageManager.Instance != null)
        {
            if (titleLabel != null)
                titleLabel.text = LanguageManager.Instance.GetText("HUB_TITLE_UNDER_CONSTRUCTION");

            if (subtitleLabel != null)
                subtitleLabel.text = LanguageManager.Instance.GetText("HUB_SUBTITLE_COMING_SOON");
        }
        else
        {
            string title = string.IsNullOrWhiteSpace(titleOverride) ? DefaultTitle : titleOverride;
            string subtitle = string.IsNullOrWhiteSpace(subtitleOverride) ? DefaultSubtitle : subtitleOverride;

            if (titleLabel != null)
                titleLabel.text = title;

            if (subtitleLabel != null)
                subtitleLabel.text = subtitle;
        }

        if (comingSoonIcon != null && comingSoonIcon.sprite == null)
            comingSoonIcon.enabled = false;
    }

    private void OnDestroy()
    {
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(ShowOptionsPanel);

        if (_optionsPanel != null)
            _optionsPanel.CloseRequested -= ShowHubPlaceholder;
    }
}
