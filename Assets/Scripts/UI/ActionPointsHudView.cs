using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Affiche la consommation journalière de points d'action (HUD NavigationHUD).
/// Lecture seule — la consommation passe par <see cref="ActionPointService"/>.
/// </summary>
public class ActionPointsHudView : MonoBehaviour
{
    private const string SpendTriggerName = "Spend";
    private const string RefuseTriggerName = "Refuse";
    private const float RefuseLabelFlashDuration = 0.4f;
    private static readonly Color RefuseLabelFlashColor = new Color(1f, 0.45f, 0.45f, 1f);

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI pointsLabel;
    [SerializeField] private TextMeshProUGUI subtitleLabel;

    [Header("Indicateur fatigue")]
    [SerializeField] private Image fatigueIconImage;

    [Header("Barre")]
    [SerializeField] private Image barFillImage;
    [SerializeField] private Image barBackgroundImage;

    [Header("Overlay PA consommés")]
    [Tooltip("Assombrit la portion déjà consommée ; les bandes colorées (Bezy) restent visibles en dessous.")]
    [SerializeField] private Color consumedOverlayColor = new Color(0f, 0f, 0f, 0.42f);

    [Header("Polish anim (Bezy)")]
    [Tooltip("Animator racine : Spend → SpendPulse, Refuse → RefuseShake.")]
    [SerializeField] private Animator animator;

    private static readonly int SpendTriggerHash = Animator.StringToHash(SpendTriggerName);
    private static readonly int RefuseTriggerHash = Animator.StringToHash(RefuseTriggerName);

    private bool subscribed;
    private bool buffSubscribed;
    private int lastConsumedPoints = -1;
    private Coroutine refuseLabelFlashRoutine;
    private Color pointsLabelNormalColor = Color.white;
    private bool hasCachedPointsLabelColor;

    private void OnEnable()
    {
        CachePointsLabelNormalColor();
        Subscribe();
        SubscribeBuffs();
        Refresh();
    }

    private void OnDisable()
    {
        StopRefuseLabelFlash();
        Unsubscribe();
        UnsubscribeBuffs();
    }

    private void Start()
    {
        ResolveFatigueIconIfNeeded();
        Subscribe();
        SubscribeBuffs();
        Refresh();
    }

    private void ResolveFatigueIconIfNeeded()
    {
        if (fatigueIconImage != null)
            return;

        Transform iconTransform = transform.Find("Row/Icon");
        if (iconTransform != null)
            fatigueIconImage = iconTransform.GetComponent<Image>();
    }

    private void Subscribe()
    {
        if (subscribed || ActionPointService.Instance == null)
            return;

        ActionPointService.Instance.OnActionPointsChanged += Refresh;
        ActionPointService.Instance.OnSpendRefused += PlayRefusePulse;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
            return;

        if (ActionPointService.Instance != null)
        {
            ActionPointService.Instance.OnActionPointsChanged -= Refresh;
            ActionPointService.Instance.OnSpendRefused -= PlayRefusePulse;
        }

        subscribed = false;
    }

    private void SubscribeBuffs()
    {
        if (buffSubscribed || BuffManager.Instance == null)
            return;

        BuffManager.Instance.OnModifiersChanged += Refresh;
        buffSubscribed = true;
    }

    private void UnsubscribeBuffs()
    {
        if (!buffSubscribed)
            return;

        if (BuffManager.Instance != null)
            BuffManager.Instance.OnModifiersChanged -= Refresh;

        buffSubscribed = false;
    }

    public void Refresh()
    {
        ResolveFatigueIconIfNeeded();

        ActionPointService service = ActionPointService.Instance;
        if (service == null)
        {
            SetFallbackDisplay();
            return;
        }

        int consumed = service.ConsumedPoints;
        int remaining = service.RemainingPoints;
        int max = Mathf.Max(1, service.MaxDailyPoints);

        // Compteur = PA restants (0 = plus d'actions possibles). La barre = part consommée.
        if (pointsLabel != null)
            pointsLabel.text = $"{remaining} / {max}";

        if (subtitleLabel != null)
            subtitleLabel.text = FormatConsumedWorkTimeSubtitle(consumed, service.MinutesPerPoint);

        if (fatigueIconImage != null)
        {
            ActionPointFatigueTier tier = ResolveFatigueTier(consumed);
            fatigueIconImage.color = ActionPointFatigueUiCopy.GetFatigueIndicatorColor(tier);
        }

        if (barFillImage != null)
        {
            barFillImage.fillAmount = (float)consumed / max;
            barFillImage.color = consumedOverlayColor;
        }

        PlaySpendPulseIfConsumedIncreased(consumed);
    }

    private void PlaySpendPulseIfConsumedIncreased(int consumed)
    {
        bool shouldPulse = lastConsumedPoints >= 0 && consumed > lastConsumedPoints;
        lastConsumedPoints = consumed;

        if (!shouldPulse || animator == null)
            return;

        animator.ResetTrigger(SpendTriggerHash);
        animator.SetTrigger(SpendTriggerHash);
    }

    private void PlayRefusePulse()
    {
        CachePointsLabelNormalColor();
        StopRefuseLabelFlash();

        if (animator != null)
        {
            animator.ResetTrigger(RefuseTriggerHash);
            animator.SetTrigger(RefuseTriggerHash);
        }

        if (pointsLabel != null && isActiveAndEnabled)
            refuseLabelFlashRoutine = StartCoroutine(RefuseLabelFlashRoutine());
    }

    // Mémorise une seule fois la couleur normale du label (avant tout flash).
    private void CachePointsLabelNormalColor()
    {
        if (hasCachedPointsLabelColor)
            return;

        pointsLabelNormalColor = pointsLabel != null ? pointsLabel.color : Color.white;
        hasCachedPointsLabelColor = pointsLabel != null;
    }

    private void RestorePointsLabelNormalColor()
    {
        CachePointsLabelNormalColor();
        if (pointsLabel != null)
            pointsLabel.color = pointsLabelNormalColor;
    }

    private void StopRefuseLabelFlash()
    {
        if (refuseLabelFlashRoutine != null)
        {
            StopCoroutine(refuseLabelFlashRoutine);
            refuseLabelFlashRoutine = null;
        }

        RestorePointsLabelNormalColor();
    }

    private IEnumerator RefuseLabelFlashRoutine()
    {
        Color baseColor = pointsLabelNormalColor;
        float elapsed = 0f;

        while (elapsed < RefuseLabelFlashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float wave = 0.5f + 0.5f * Mathf.Sin(elapsed * 28f);
            pointsLabel.color = Color.Lerp(baseColor, RefuseLabelFlashColor, wave * (1f - elapsed / RefuseLabelFlashDuration));
            yield return null;
        }

        refuseLabelFlashRoutine = null;
        RestorePointsLabelNormalColor();
    }

    private void SetFallbackDisplay()
    {
        if (pointsLabel != null)
            pointsLabel.text = "-- / --";

        if (subtitleLabel != null)
            subtitleLabel.text = string.Empty;

        if (barFillImage != null)
            barFillImage.fillAmount = 0f;
    }

    private static string FormatConsumedWorkTimeSubtitle(int consumedPoints, int minutesPerPoint)
    {
        int totalMinutes = consumedPoints * minutesPerPoint;
        if (totalMinutes <= 0)
            return "~ 0 min de travail effectué";

        if (totalMinutes < 60)
            return $"~ {totalMinutes} min de travail effectué";

        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        if (minutes == 0)
            return $"~ {hours} h de travail effectué";

        return $"~ {hours} h {minutes} min de travail effectué";
    }

    private static ActionPointFatigueTier ResolveFatigueTier(int consumedPoints)
    {
        BuffManager buffs = BuffManager.Instance;
        return buffs != null
            ? buffs.GetFatigueTier(consumedPoints)
            : ActionPointFatigueTier.Comfort;
    }
}
