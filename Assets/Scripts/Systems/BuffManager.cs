using System;
using UnityEngine;

/// <summary>
/// Agrège les modificateurs actifs du joueur (fatigue PA, buffs futurs).
/// Singleton persistant dans NavigationHUD.
/// </summary>
public class BuffManager : MonoBehaviour
{
    public const int MaxDailyActionPoints = 160;
    public const int ComfortZoneEnd = 80;
    public const int CautionZoneEnd = 120;

    public const float CautionCostMultiplier = 0.25f;
    public const float FatigueCostMultiplier = 0.50f;

    public static BuffManager Instance { get; private set; }

    public event Action OnModifiersChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[BuffManager] Instance dupliquée — une seule attendue dans NavigationHUD.", this);
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (ActionPointService.Instance != null)
            ActionPointService.Instance.OnActionPointsChanged += HandleActionPointsChanged;
    }

    private void OnDisable()
    {
        if (ActionPointService.Instance != null)
            ActionPointService.Instance.OnActionPointsChanged -= HandleActionPointsChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        if (ActionPointService.Instance != null)
            ActionPointService.Instance.OnActionPointsChanged += HandleActionPointsChanged;

        NotifyChanged();
    }

    private void HandleActionPointsChanged() => NotifyChanged();

    public ActionPointFatigueTier GetFatigueTier(int consumedActionPoints)
    {
        int consumed = Mathf.Clamp(consumedActionPoints, 0, MaxDailyActionPoints);

        if (consumed < ComfortZoneEnd)
            return ActionPointFatigueTier.Comfort;

        if (consumed < CautionZoneEnd)
            return ActionPointFatigueTier.Caution;

        return ActionPointFatigueTier.Fatigue;
    }

    public ActionPointFatigueTier GetCurrentFatigueTier()
    {
        ActionPointService service = ActionPointService.Instance;
        int consumed = service != null ? service.ConsumedPoints : 0;
        return GetFatigueTier(consumed);
    }

    public float GetActionPointCostMultiplier(int consumedActionPoints)
    {
        return GetFatigueTier(consumedActionPoints) switch
        {
            ActionPointFatigueTier.Caution => CautionCostMultiplier,
            ActionPointFatigueTier.Fatigue => FatigueCostMultiplier,
            _ => 0f,
        };
    }

    public float GetCurrentActionPointCostMultiplier()
    {
        ActionPointService service = ActionPointService.Instance;
        int consumed = service != null ? service.ConsumedPoints : 0;
        return GetActionPointCostMultiplier(consumed);
    }

    public int ApplyActionPointCostMultiplier(int baseCost, int consumedActionPoints)
    {
        if (baseCost <= 0)
            return 0;

        float multiplier = 1f + GetActionPointCostMultiplier(consumedActionPoints);
        return Mathf.Max(1, Mathf.CeilToInt(baseCost * multiplier));
    }

    public int ApplyCurrentActionPointCostMultiplier(int baseCost)
    {
        ActionPointService service = ActionPointService.Instance;
        int consumed = service != null ? service.ConsumedPoints : 0;
        return ApplyActionPointCostMultiplier(baseCost, consumed);
    }

    private void NotifyChanged()
    {
        OnModifiersChanged?.Invoke();
    }
}
