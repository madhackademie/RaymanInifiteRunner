using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Écran de chargement affiché au démarrage de la scène Bootstrap.
/// Expose SetProgress() pour mettre à jour la barre et Hide() pour un fade-out.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    private const float FadeDuration = 0.4f;

    [Header("Progress Bar")]
    [SerializeField] private Image fillImage;

    [Header("Label")]
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Met à jour la barre de progression et le texte de pourcentage.
    /// </summary>
    /// <param name="value">Valeur normalisée entre 0 et 1.</param>
    public void SetProgress(float value)
    {
        value = Mathf.Clamp01(value);

        if (fillImage != null)
            fillImage.fillAmount = value;

        if (percentageText != null)
            percentageText.text = $"{Mathf.RoundToInt(value * 100)} %";
    }

    /// <summary>
    /// Fade-out puis désactive l'écran de chargement.
    /// </summary>
    public async Awaitable Hide()
    {
        float elapsed = 0f;

        while (elapsed < FadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / FadeDuration);
            await Awaitable.NextFrameAsync();
        }

        gameObject.SetActive(false);
    }
}
