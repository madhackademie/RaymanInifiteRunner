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
    [Tooltip("Animator sur la racine ; trigger Spend → clip SpendPulse. Optionnel jusqu'à Phase 5 Bezy.")]
    [SerializeField] private Animator animator;

    private static readonly int SpendTriggerHash = Animator.StringToHash(SpendTriggerName);

    private bool subscribed;
    private bool buffSubscribed;
    private int lastConsumedPoints = -1;

    private void OnEnable()
    {
        Subscribe();
        SubscribeBuffs();
        Refresh();
    }

    private void OnDisable()
    {
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
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed)
            return;

        if (ActionPointService.Instance != null)
            ActionPointService.Instance.OnActionPointsChanged -= Refresh;

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
        int max = Mathf.Max(1, service.MaxDailyPoints);

        if (pointsLabel != null)
            pointsLabel.text = $"{consumed} / {max}";

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
            return "≈ 0 min de travail effectué";

        if (totalMinutes < 60)
            return $"≈ {totalMinutes} min de travail effectué";

        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        if (minutes == 0)
            return $"≈ {hours} h de travail effectué";

        return $"≈ {hours} h {minutes} min de travail effectué";
    }

    private static ActionPointFatigueTier ResolveFatigueTier(int consumedPoints)
    {
        BuffManager buffs = BuffManager.Instance;
        return buffs != null
            ? buffs.GetFatigueTier(consumedPoints)
            : ActionPointFatigueTier.Comfort;
    }
}
