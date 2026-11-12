using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représentation visuelle et interactive d'une entrée du hub de navigation.
/// Se configure automatiquement depuis un <see cref="MapNodeData"/> et l'état de progression.
/// </summary>
[RequireComponent(typeof(Button))]
public class MapNodeButton : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image lockIcon;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI subtitleLabel;

    [Header("Couleurs")]
    [SerializeField] private Color unlockedColor = new Color(0.27f, 0.7f, 0.25f, 1f);
    [SerializeField] private Color lockedColor   = new Color(0.4f,  0.4f, 0.4f,  0.8f);

    // ── Runtime ───────────────────────────────────────────────────────────────

    private MapNodeData nodeData;
    private Button button;

    /// <summary>Événement déclenché quand le joueur clique sur une entrée déverrouillée.</summary>
    public event System.Action<MapNodeData> OnNodeSelected;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.LanguageChanged += ApplyLabel;
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.LanguageChanged -= ApplyLabel;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(HandleClick);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Initialise le bouton avec les données de l'entrée et son état de déverrouillage.
    /// </summary>
    public void Setup(MapNodeData data, bool isUnlocked)
    {
        nodeData = data;

        ApplyLabel();

        if (subtitleLabel != null)
        {
            subtitleLabel.text = data.subtitle;
            subtitleLabel.gameObject.SetActive(!string.IsNullOrEmpty(data.subtitle));
        }

        if (backgroundImage != null)
        {
            Sprite sprite = isUnlocked ? data.unlockedSprite : data.lockedSprite;
            if (sprite != null)
                backgroundImage.sprite = sprite;
            backgroundImage.color = isUnlocked ? unlockedColor : lockedColor;
        }

        if (lockIcon != null)
            lockIcon.gameObject.SetActive(!isUnlocked);

        button.interactable = isUnlocked;
    }

    // ── Label ─────────────────────────────────────────────────────────────────

    private const string FirstLevelNodeId = "FirstLvl";
    private const string StartAdventureKey = "MAP_START_ADVENTURE";
    private const string StartAdventureFallback = "Commencer l'aventure";

    private void ApplyLabel()
    {
        if (label == null || nodeData == null)
            return;

        if (nodeData.nodeId == FirstLevelNodeId)
            label.text = GetLocalizedOrFallback(StartAdventureKey, StartAdventureFallback);
        else
            label.text = nodeData.displayName;
    }

    private string GetLocalizedOrFallback(string key, string fallback)
    {
        if (LanguageManager.Instance == null)
            return fallback;

        string text = LanguageManager.Instance.GetText(key);
        if (string.IsNullOrEmpty(text) || text == key)
            return fallback;

        return text;
    }

    // ── Callbacks ─────────────────────────────────────────────────────────────

    private void HandleClick()
    {
        if (nodeData != null)
            OnNodeSelected?.Invoke(nodeData);
    }
}
