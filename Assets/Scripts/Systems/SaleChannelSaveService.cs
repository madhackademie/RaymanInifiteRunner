using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Persistance vente : cooldowns, stats, déblocages et recherches en cours.
/// </summary>
public static class SaleChannelSaveService
{
    private const string SaveFileName = "sale_channels.json";

    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool TryLoad(out SaleChannelPersistedData data)
    {
        data = new SaleChannelPersistedData();

        if (!File.Exists(SaveFilePath))
            return false;

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData raw = JsonUtility.FromJson<SaveData>(json);
            if (raw == null)
                return false;

            data = raw.ToRuntimeData();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaleChannelSaveService] Erreur chargement : {e.Message}");
            return false;
        }
    }

    public static void Save(SaleChannelPersistedData data)
    {
        SaveData raw = SaveData.FromRuntimeData(data ?? new SaleChannelPersistedData());
        string json = JsonUtility.ToJson(raw, prettyPrint: true);

        try
        {
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaleChannelSaveService] Erreur écriture : {e.Message}");
        }
    }

    public static void Delete()
    {
        if (File.Exists(SaveFilePath))
            File.Delete(SaveFilePath);
    }

    [Serializable]
    private class SaveData
    {
        public List<ChannelRecord> channels = new();
        public List<string> unlockedChannelIds = new();
        public List<StatRecord> channelStats = new();
        public List<ResearchRecord> activeResearch = new();

        public static SaveData FromRuntimeData(SaleChannelPersistedData data)
        {
            var save = new SaveData();

            foreach (KeyValuePair<string, long> entry in data.LastSaleUtcTicksByChannel)
            {
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Value <= 0)
                    continue;

                save.channels.Add(new ChannelRecord
                {
                    channelId = entry.Key,
                    lastSaleUtcTicks = entry.Value,
                });
            }

            foreach (string channelId in data.UnlockedChannelIds)
            {
                if (!string.IsNullOrWhiteSpace(channelId))
                    save.unlockedChannelIds.Add(channelId);
            }

            foreach (KeyValuePair<string, SaleChannelStatBlock> entry in data.StatsByChannel)
            {
                if (entry.Value == null || string.IsNullOrWhiteSpace(entry.Key))
                    continue;

                save.channelStats.Add(new StatRecord
                {
                    channelId = entry.Key,
                    saleCount = entry.Value.SaleCount,
                    itemsSold = entry.Value.ItemsSold,
                    goldEarned = entry.Value.GoldEarned,
                });
            }

            foreach (KeyValuePair<string, long> entry in data.ResearchEndUtcTicksByChannel)
            {
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Value <= 0)
                    continue;

                save.activeResearch.Add(new ResearchRecord
                {
                    channelId = entry.Key,
                    researchEndUtcTicks = entry.Value,
                });
            }

            return save;
        }

        public SaleChannelPersistedData ToRuntimeData()
        {
            var data = new SaleChannelPersistedData();

            if (channels != null)
            {
                foreach (ChannelRecord record in channels)
                {
                    if (record == null || string.IsNullOrWhiteSpace(record.channelId) || record.lastSaleUtcTicks <= 0)
                        continue;

                    data.LastSaleUtcTicksByChannel[record.channelId.Trim()] = record.lastSaleUtcTicks;
                }
            }

            if (unlockedChannelIds != null)
            {
                foreach (string channelId in unlockedChannelIds)
                {
                    if (!string.IsNullOrWhiteSpace(channelId))
                        data.UnlockedChannelIds.Add(channelId.Trim());
                }
            }

            if (channelStats != null)
            {
                foreach (StatRecord record in channelStats)
                {
                    if (record == null || string.IsNullOrWhiteSpace(record.channelId))
                        continue;

                    data.StatsByChannel[record.channelId.Trim()] = new SaleChannelStatBlock
                    {
                        SaleCount = Mathf.Max(0, record.saleCount),
                        ItemsSold = Mathf.Max(0, record.itemsSold),
                        GoldEarned = Mathf.Max(0, record.goldEarned),
                    };
                }
            }

            if (activeResearch != null)
            {
                foreach (ResearchRecord record in activeResearch)
                {
                    if (record == null || string.IsNullOrWhiteSpace(record.channelId) || record.researchEndUtcTicks <= 0)
                        continue;

                    data.ResearchEndUtcTicksByChannel[record.channelId.Trim()] = record.researchEndUtcTicks;
                }
            }

            return data;
        }
    }

    [Serializable]
    private class ChannelRecord
    {
        public string channelId;
        public long lastSaleUtcTicks;
    }

    [Serializable]
    private class StatRecord
    {
        public string channelId;
        public int saleCount;
        public int itemsSold;
        public int goldEarned;
    }

    [Serializable]
    private class ResearchRecord
    {
        public string channelId;
        public long researchEndUtcTicks;
    }
}

/// <summary>
/// Bloc stats vente par canal (persisté).
/// </summary>
[Serializable]
public class SaleChannelStatBlock
{
    public int SaleCount;
    public int ItemsSold;
    public int GoldEarned;
}

/// <summary>
/// Données runtime persistées pour les canaux de vente.
/// </summary>
public class SaleChannelPersistedData
{
    public Dictionary<string, long> LastSaleUtcTicksByChannel { get; } = new();
    public HashSet<string> UnlockedChannelIds { get; } = new();
    public Dictionary<string, SaleChannelStatBlock> StatsByChannel { get; } = new();
    public Dictionary<string, long> ResearchEndUtcTicksByChannel { get; } = new();
}
