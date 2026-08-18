using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Déblocage progressif des canaux (conditions, recherche timer, persistance).
/// </summary>
public class SaleChannelUnlockService : MonoBehaviour
{
    public static SaleChannelUnlockService Instance { get; private set; }

    public readonly struct ProgressCounters
    {
        public ProgressCounters(int saleCount, int itemsSold, bool prerequisiteUnlocked)
        {
            SaleCount = saleCount;
            ItemsSold = itemsSold;
            PrerequisiteUnlocked = prerequisiteUnlocked;
        }

        public int SaleCount { get; }
        public int ItemsSold { get; }
        public bool PrerequisiteUnlocked { get; }
    }

    [Header("Definitions")]
    [SerializeField] private SaleChannelUnlockDefinition[] unlockDefinitions = Array.Empty<SaleChannelUnlockDefinition>();

    [Header("Debug")]
    [SerializeField] private bool ignoreUnlockRequirements;
    [Tooltip("Remplace la durée SO (playtest rapide). 0 = durée SO.")]
    [SerializeField] private float debugResearchDurationSeconds;

    private readonly Dictionary<string, SaleChannelUnlockDefinition> definitionByChannelId = new();
    private SaleChannelPersistedData persistedData;

    internal SaleChannelPersistedData SharedData => persistedData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SaleChannelUnlockService] Instance dupliquée — une seule attendue.", this);
            Destroy(this);
            return;
        }

        Instance = this;
        CacheDefinitions();
        LoadProgress();
        EnsureDefaultUnlocks();
        FinalizeCompletedResearch(forcePersist: false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnApplicationQuit()
    {
        PersistProgress();
    }

    [ContextMenu("Debug/Clear Sale Channel Progress")]
    public void DebugClearProgress()
    {
        persistedData = new SaleChannelPersistedData();
        EnsureDefaultUnlocks();
        PersistProgress();
        Debug.Log("[SaleChannelUnlockService] Progression canaux réinitialisée.");
    }

    /// <summary>Remet le bandeau vélo en état verrouillé (déblocage + recherche + cooldown effacés).</summary>
    [ContextMenu("Debug/Reset Velo Bandeau (verrouillé)")]
    public void DebugResetVeloBandeau()
    {
        persistedData.UnlockedChannelIds.Remove(SaleChannelId.Bike);
        persistedData.ResearchEndUtcTicksByChannel.Remove(SaleChannelId.Bike);
        persistedData.LastSaleUtcTicksByChannel.Remove(SaleChannelId.Bike);
        PersistProgress();
        Debug.Log("[SaleChannelUnlockService] Bandeau vélo réinitialisé — canal verrouillé.");
    }

    public bool IsChannelUnlocked(string channelId)
    {
        if (string.IsNullOrWhiteSpace(channelId))
            return false;

        if (channelId == SaleChannelId.Neighbor)
            return true;

        return persistedData.UnlockedChannelIds.Contains(channelId);
    }

    public bool TryGetProgressSnapshot(string channelId, out SaleChannelUnlockProgressSnapshot snapshot)
    {
        snapshot = default;

        if (string.IsNullOrWhiteSpace(channelId) || channelId == SaleChannelId.Neighbor)
            return false;

        if (!definitionByChannelId.TryGetValue(channelId, out SaleChannelUnlockDefinition definition))
            return false;

        SaleChannelProgressionPhase phase = ResolvePhase(channelId, definition, out float researchRemainingSeconds);
        ProgressCounters counters = BuildCounters(definition);
        int walletGold = ResolveWalletGold();

        snapshot = new SaleChannelUnlockProgressSnapshot(
            channelId,
            phase,
            SaleChannelUnlockUiCopy.BuildTooltipTitle(definition),
            SaleChannelUnlockUiCopy.BuildTooltipBody(
                definition,
                counters,
                walletGold,
                phase,
                researchRemainingSeconds),
            SaleChannelUnlockUiCopy.BuildStatusLabel(phase, researchRemainingSeconds),
            phase == SaleChannelProgressionPhase.Unlockable);

        return true;
    }

    public bool TryGetResearchLaunchPresentation(
        string channelId,
        out string displayName,
        out int costGold,
        out float durationSeconds)
    {
        displayName = null;
        costGold = 0;
        durationSeconds = 0f;

        if (!definitionByChannelId.TryGetValue(channelId, out SaleChannelUnlockDefinition definition))
            return false;

        displayName = definition.DisplayName;
        costGold = definition.ResearchCostGold;
        durationSeconds = ResolveResearchDurationSeconds(definition);
        return true;
    }

    public bool TryStartResearch(string channelId, out string failureMessage)
    {
        failureMessage = null;

        if (!TryGetProgressSnapshot(channelId, out SaleChannelUnlockProgressSnapshot snapshot))
        {
            failureMessage = "Canal inconnu.";
            return false;
        }

        if (snapshot.Phase != SaleChannelProgressionPhase.Unlockable)
        {
            failureMessage = snapshot.Phase == SaleChannelProgressionPhase.ResearchInProgress
                ? "Recherche déjà en cours."
                : "Conditions de déblocage non remplies.";
            return false;
        }

        if (!definitionByChannelId.TryGetValue(channelId, out SaleChannelUnlockDefinition definition))
        {
            failureMessage = "Définition introuvable.";
            return false;
        }

        PlayerInventory inventory = PlayerInventory.Instance;
        ItemDefinition currency = inventory?.ItemDatabase?.PrimaryCurrency;
        if (inventory == null || currency == null)
        {
            failureMessage = "Inventaire indisponible.";
            return false;
        }

        if (!InventoryCurrencyAccount.HasSufficientFunds(inventory, currency, definition.ResearchCostGold))
        {
            failureMessage = $"Or insuffisant ({definition.ResearchCostGold} requis).";
            return false;
        }

        if (!InventoryCurrencyAccount.TryDebit(inventory, currency, definition.ResearchCostGold))
        {
            failureMessage = "Impossible de payer la recherche.";
            return false;
        }

        float durationSeconds = ResolveResearchDurationSeconds(definition);
        DateTime endUtc = DateTime.UtcNow.AddSeconds(durationSeconds);
        persistedData.ResearchEndUtcTicksByChannel[channelId] = endUtc.Ticks;
        PersistProgress();
        return true;
    }

    public bool TryGetLastSaleUtcTicks(string channelId, out long utcTicks)
    {
        utcTicks = 0;
        if (string.IsNullOrWhiteSpace(channelId))
            return false;

        return persistedData.LastSaleUtcTicksByChannel.TryGetValue(channelId, out utcTicks) && utcTicks > 0;
    }

    public void SetLastSaleUtcTicks(string channelId, long utcTicks)
    {
        if (string.IsNullOrWhiteSpace(channelId) || utcTicks <= 0)
            return;

        persistedData.LastSaleUtcTicksByChannel[channelId] = utcTicks;
        PersistProgress();
    }

    public void RecordSale(string channelId, int quantitySold)
    {
        if (string.IsNullOrWhiteSpace(channelId) || quantitySold <= 0)
            return;

        if (!persistedData.StatsByChannel.TryGetValue(channelId, out SaleChannelStatBlock stats))
        {
            stats = new SaleChannelStatBlock();
            persistedData.StatsByChannel[channelId] = stats;
        }

        stats.SaleCount += 1;
        stats.ItemsSold += quantitySold;
        PersistProgress();
    }

    public void RefreshProgress()
    {
        FinalizeCompletedResearch(forcePersist: true);
    }

    public bool HasActiveResearch()
    {
        FinalizeCompletedResearch(forcePersist: false);

        foreach (KeyValuePair<string, long> entry in persistedData.ResearchEndUtcTicksByChannel)
        {
            if (TryGetResearchRemainingSeconds(entry.Key, out float remaining) && remaining > 0f)
                return true;
        }

        return false;
    }

    public bool TryGetResearchRemainingSeconds(string channelId, out float remainingSeconds)
    {
        remainingSeconds = 0f;

        if (!persistedData.ResearchEndUtcTicksByChannel.TryGetValue(channelId, out long endTicks) || endTicks <= 0)
            return false;

        DateTime endUtc = new DateTime(endTicks, DateTimeKind.Utc);
        double remaining = (endUtc - DateTime.UtcNow).TotalSeconds;
        if (remaining <= 0d)
            return false;

        remainingSeconds = (float)remaining;
        return true;
    }

    private void CacheDefinitions()
    {
        definitionByChannelId.Clear();

        if (unlockDefinitions == null)
            return;

        foreach (SaleChannelUnlockDefinition definition in unlockDefinitions)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.ChannelId))
                continue;

            definitionByChannelId[definition.ChannelId] = definition;
        }
    }

    private void LoadProgress()
    {
        if (SaleChannelSaveService.TryLoad(out SaleChannelPersistedData loaded))
            persistedData = loaded;
        else
            persistedData = new SaleChannelPersistedData();
    }

    private void PersistProgress()
    {
        SaleChannelSaveService.Save(persistedData);
    }

    private void EnsureDefaultUnlocks()
    {
        persistedData.UnlockedChannelIds.Add(SaleChannelId.Neighbor);
    }

    private void FinalizeCompletedResearch(bool forcePersist)
    {
        if (persistedData.ResearchEndUtcTicksByChannel.Count == 0)
            return;

        var completed = new List<string>();

        foreach (KeyValuePair<string, long> entry in persistedData.ResearchEndUtcTicksByChannel)
        {
            DateTime endUtc = new DateTime(entry.Value, DateTimeKind.Utc);
            if (DateTime.UtcNow >= endUtc)
                completed.Add(entry.Key);
        }

        if (completed.Count == 0)
            return;

        foreach (string channelId in completed)
        {
            persistedData.ResearchEndUtcTicksByChannel.Remove(channelId);
            persistedData.UnlockedChannelIds.Add(channelId);
        }

        if (forcePersist)
            PersistProgress();
    }

    private SaleChannelProgressionPhase ResolvePhase(
        string channelId,
        SaleChannelUnlockDefinition definition,
        out float researchRemainingSeconds)
    {
        researchRemainingSeconds = 0f;

        if (IsChannelUnlocked(channelId))
            return SaleChannelProgressionPhase.Unlocked;

        if (TryGetResearchRemainingSeconds(channelId, out researchRemainingSeconds))
            return SaleChannelProgressionPhase.ResearchInProgress;

        if (ignoreUnlockRequirements || AreUnlockConditionsMet(definition))
            return SaleChannelProgressionPhase.Unlockable;

        return SaleChannelProgressionPhase.Locked;
    }

    private bool AreUnlockConditionsMet(SaleChannelUnlockDefinition definition)
    {
        ProgressCounters counters = BuildCounters(definition);
        int walletGold = ResolveWalletGold();

        if (!counters.PrerequisiteUnlocked)
            return false;

        if (definition.RequiredSaleCount > 0 && counters.SaleCount < definition.RequiredSaleCount)
            return false;

        if (definition.RequiredItemsSold > 0 && counters.ItemsSold < definition.RequiredItemsSold)
            return false;

        if (definition.RequiredWalletGold > 0 && walletGold < definition.RequiredWalletGold)
            return false;

        if (definition.ResearchCostGold > 0 && walletGold < definition.ResearchCostGold)
            return false;

        return true;
    }

    private ProgressCounters BuildCounters(SaleChannelUnlockDefinition definition)
    {
        string statsChannelId = string.IsNullOrWhiteSpace(definition.RequiredSaleCountChannelId)
            ? definition.ChannelId
            : definition.RequiredSaleCountChannelId;

        int saleCount = 0;
        int itemsSold = 0;

        if (persistedData.StatsByChannel.TryGetValue(statsChannelId, out SaleChannelStatBlock stats))
        {
            saleCount = stats.SaleCount;
            itemsSold = stats.ItemsSold;
        }

        bool prerequisiteUnlocked = string.IsNullOrWhiteSpace(definition.RequiredUnlockedChannelId)
                                    || IsChannelUnlocked(definition.RequiredUnlockedChannelId);

        return new ProgressCounters(saleCount, itemsSold, prerequisiteUnlocked);
    }

    private static int ResolveWalletGold()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        ItemDefinition currency = inventory?.ItemDatabase?.PrimaryCurrency;
        if (inventory == null || currency == null)
            return 0;

        return InventoryCurrencyAccount.GetBalance(inventory, currency);
    }

    private float ResolveResearchDurationSeconds(SaleChannelUnlockDefinition definition)
    {
        if (debugResearchDurationSeconds > 0f)
            return debugResearchDurationSeconds;

        return definition.ResearchDurationSeconds;
    }
}
