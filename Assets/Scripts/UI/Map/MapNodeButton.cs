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

        if (label != null)
            label.text = data.displayName;

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

    // ── Callbacks ─────────────────────────────────────────────────────────────

    private void HandleClick()
    {
        if (nodeData != null)
            OnNodeSelected?.Invoke(nodeData);
    }
}
