using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Persistance des horodatages de vente par canal (cooldown 24 h).
/// </summary>
public static class SaleChannelSaveService
{
    private const string SaveFileName = "sale_channels.json";

    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool TryLoad(out Dictionary<string, long> lastSaleUtcTicksByChannel)
    {
        lastSaleUtcTicksByChannel = new Dictionary<string, long>();

        if (!File.Exists(SaveFilePath))
            return false;

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data?.channels == null)
                return false;

            foreach (ChannelRecord record in data.channels)
            {
                if (record == null || string.IsNullOrWhiteSpace(record.channelId) || record.lastSaleUtcTicks <= 0)
                    continue;

                lastSaleUtcTicksByChannel[record.channelId.Trim()] = record.lastSaleUtcTicks;
            }

            return lastSaleUtcTicksByChannel.Count > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaleChannelSaveService] Erreur chargement : {e.Message}");
            return false;
        }
    }

    public static void Save(IReadOnlyDictionary<string, long> lastSaleUtcTicksByChannel)
    {
        var records = new List<ChannelRecord>();

        if (lastSaleUtcTicksByChannel != null)
        {
            foreach (KeyValuePair<string, long> entry in lastSaleUtcTicksByChannel)
            {
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Value <= 0)
                    continue;

                records.Add(new ChannelRecord
                {
                    channelId = entry.Key,
                    lastSaleUtcTicks = entry.Value,
                });
            }
        }

        string json = JsonUtility.ToJson(new SaveData { channels = records }, prettyPrint: true);

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
    }

    [Serializable]
    private class ChannelRecord
    {
        public string channelId;
        public long lastSaleUtcTicks;
    }
}
