using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Écran hub Plus (FeaturesHub) — V0 placeholder plein écran.
/// Remplacé plus tard par sous-onglets + panneaux (spec hub Plus).
/// </summary>
public class RuntimeFeaturesHubScreen : MonoBehaviour
{
    private const string DefaultTitle = "En construction";
    private const string DefaultSubtitle = "Bientôt disponible";

    [Header("Shell")]
    [SerializeField] private Image rootBackdropImage;
    [SerializeField] private Image contentBackdropImage;

    [Header("Placeholder centre")]
    [SerializeField] private Image comingSoonIcon;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI subtitleLabel;

    [Header("Copy (optionnel — laisse vide pour défauts V0)")]
    [SerializeField] private string titleOverride;
    [SerializeField] private string subtitleOverride;

    private void OnEnable()
    {
        ApplyBackdrop();
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
        string title = string.IsNullOrWhiteSpace(titleOverride) ? DefaultTitle : titleOverride;
        string subtitle = string.IsNullOrWhiteSpace(subtitleOverride) ? DefaultSubtitle : subtitleOverride;

        if (titleLabel != null)
            titleLabel.text = title;

        if (subtitleLabel != null)
            subtitleLabel.text = subtitle;

        if (comingSoonIcon != null && comingSoonIcon.sprite == null)
            comingSoonIcon.enabled = false;
    }
}
