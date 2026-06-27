using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Affiche le solde journalier de points d'action (HUD NavigationHUD).
/// Lecture seule — la consommation passe par <see cref="ActionPointService"/>.
/// </summary>
public class ActionPointsHudView : MonoBehaviour
{
    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI pointsLabel;
    [SerializeField] private TextMeshProUGUI subtitleLabel;

    [Header("Barre")]
    [SerializeField] private Image barFillImage;
    [SerializeField] private Image barBackgroundImage;

    private bool subscribed;

    private void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    private void OnDisable() => Unsubscribe();

    private void Start()
    {
        Subscribe();
        Refresh();
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

    public void Refresh()
    {
        ActionPointService service = ActionPointService.Instance;
        if (service == null)
        {
            SetFallbackDisplay();
            return;
        }

        int remaining = service.RemainingPoints;
        int max = Mathf.Max(1, service.MaxDailyPoints);

        if (pointsLabel != null)
            pointsLabel.text = $"{remaining} / {max}";

        if (subtitleLabel != null)
            subtitleLabel.text = FormatWorkTimeSubtitle(remaining, service.MinutesPerPoint);

        if (barFillImage != null)
            barFillImage.fillAmount = (float)remaining / max;
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

    private static string FormatWorkTimeSubtitle(int remainingPoints, int minutesPerPoint)
    {
        int totalMinutes = remainingPoints * minutesPerPoint;
        if (totalMinutes <= 0)
            return "≈ 0 min de travail";

        if (totalMinutes < 60)
            return $"≈ {totalMinutes} min de travail";

        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        if (minutes == 0)
            return $"≈ {hours} h de travail";

        return $"≈ {hours} h {minutes} min de travail";
    }
}
