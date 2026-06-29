using System;
using UnityEngine;

/// <summary>
/// Budget journalier de points d'action (PA). Singleton persistant dans NavigationHUD.
/// 1 PA = <see cref="MinutesPerPoint"/> minutes de travail (affichage HUD futur).
/// </summary>
public class ActionPointService : MonoBehaviour
{
    public const int DefaultDailyBudget = BuffManager.MaxDailyActionPoints;
    public const int DefaultMinutesPerPoint = 6;
    public const int DefaultPlantSeedCost = 1;
    public const int DefaultHarvestCost = 1;
    public const int DefaultSellCost = 1;

    public static ActionPointService Instance { get; private set; }

    [Header("Budget journalier")]
    [SerializeField] [Min(1)] private int dailyBudget = DefaultDailyBudget;
    [SerializeField] [Min(1)] private int minutesPerPoint = DefaultMinutesPerPoint;

    [Header("Coûts V0")]
    [SerializeField] [Min(0)] private int plantSeedCost = DefaultPlantSeedCost;
    [SerializeField] [Min(0)] private int harvestCost = DefaultHarvestCost;
    [SerializeField] [Min(0)] private int sellCost = DefaultSellCost;

    private int remainingPoints;
    private long lastResetUtcTicks;

    public int RemainingPoints => remainingPoints;
    public int MaxDailyPoints => dailyBudget;
    public int ConsumedPoints => Mathf.Max(0, dailyBudget - remainingPoints);
    public int MinutesPerPoint => minutesPerPoint;

    public event Action OnActionPointsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[ActionPointService] Instance dupliquée — une seule attendue dans NavigationHUD.", this);
            Destroy(this);
            return;
        }

        Instance = this;
        LoadState();
        RefreshDayIfNeeded();
        NotifyChanged();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnApplicationQuit()
    {
        PersistState();
    }

    public bool CanAfford(int cost)
    {
        RefreshDayIfNeeded();
        return cost <= 0 || remainingPoints >= cost;
    }

    public int GetBaseCostForAction(string actionId)
    {
        if (string.IsNullOrWhiteSpace(actionId))
            return 0;

        if (actionId == ActionPointActionId.PlantSeed)
            return plantSeedCost;

        if (actionId == ActionPointActionId.Harvest)
            return harvestCost;

        if (actionId == ActionPointActionId.Sell)
            return sellCost;

        return 0;
    }

    /// <summary>Coût effectif après malus fatigue (via <see cref="BuffManager"/>).</summary>
    public int GetCostForAction(string actionId)
    {
        int baseCost = GetBaseCostForAction(actionId);
        return ApplyFatigueCost(baseCost);
    }

    public int ApplyFatigueCost(int baseCost)
    {
        if (baseCost <= 0)
            return 0;

        BuffManager buffs = BuffManager.Instance;
        if (buffs == null)
            return baseCost;

        return buffs.ApplyCurrentActionPointCostMultiplier(baseCost);
    }

    public bool TryConsume(string actionId, out string failureMessage)
    {
        int cost = GetCostForAction(actionId);
        return TryConsume(actionId, cost, out failureMessage);
    }

    public bool TryConsume(string actionId, int cost, out string failureMessage)
    {
        failureMessage = null;
        RefreshDayIfNeeded();

        if (cost < 0)
        {
            failureMessage = "Coût en points d'action invalide.";
            return false;
        }

        if (cost == 0)
            return true;

        if (remainingPoints < cost)
        {
            failureMessage =
                $"Points d'action insuffisants ({remainingPoints} / {dailyBudget} restants).";
            return false;
        }

        remainingPoints -= cost;
        PersistState();
        NotifyChanged();

        Debug.Log(
            $"[ActionPointService] -{cost} PA ({actionId ?? "?"}) — reste {remainingPoints}/{dailyBudget}.",
            this);

        return true;
    }

    /// <summary>Rembourse des PA (rollback si une action échoue après débit).</summary>
    public void Refund(int amount)
    {
        if (amount <= 0)
            return;

        remainingPoints = Mathf.Min(dailyBudget, remainingPoints + amount);
        PersistState();
        NotifyChanged();
    }

    /// <summary>Recharge le budget si un nouveau jour UTC a commencé.</summary>
    public void RefreshDayIfNeeded()
    {
        DateTime todayUtc = DateTime.UtcNow.Date;
        DateTime lastResetUtc = lastResetUtcTicks > 0
            ? new DateTime(lastResetUtcTicks, DateTimeKind.Utc).Date
            : DateTime.MinValue;

        if (lastResetUtcTicks > 0 && todayUtc <= lastResetUtc)
            return;

        remainingPoints = dailyBudget;
        lastResetUtcTicks = todayUtc.Ticks;
        PersistState();
        NotifyChanged();

        Debug.Log(
            $"[ActionPointService] Reset journalier — {remainingPoints} PA disponibles (UTC {todayUtc:yyyy-MM-dd}).",
            this);
    }

    private void LoadState()
    {
        if (!ActionPointSaveService.TryLoad(out int loadedRemaining, out long loadedResetTicks))
        {
            remainingPoints = dailyBudget;
            lastResetUtcTicks = DateTime.UtcNow.Date.Ticks;
            return;
        }

        remainingPoints = Mathf.Clamp(loadedRemaining, 0, dailyBudget);
        lastResetUtcTicks = loadedResetTicks;
    }

    private void PersistState()
    {
        ActionPointSaveService.Save(remainingPoints, lastResetUtcTicks);
    }

    private void NotifyChanged()
    {
        OnActionPointsChanged?.Invoke();
    }
}
