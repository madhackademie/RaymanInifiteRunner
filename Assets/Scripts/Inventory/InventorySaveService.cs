using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles serialisation and deserialisation of the player inventory to/from a JSON file
/// located at <see cref="Application.persistentDataPath"/>.
/// </summary>
public static class InventorySaveService
{
    private const string SaveFileName = "inventory.json";

    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Serialises the given slots to JSON and writes them to disk.
    /// Only non-empty slots are persisted.
    /// </summary>
    public static void Save(IReadOnlyList<InventorySlot> slots)
    {
        var records = new List<SlotRecord>(slots.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if (slot.IsEmpty)
                continue;

            records.Add(new SlotRecord
            {
                slotIndex = i,
                itemId    = slot.Item.ItemId,
                quantity  = slot.Quantity,
            });
        }

        string json = JsonUtility.ToJson(new SaveData { slots = records }, prettyPrint: true);

        try
        {
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[InventorySaveService] Impossible d'écrire la sauvegarde : {e.Message}");
        }
    }

    /// <summary>
    /// Loads the save file from disk and restores slot data via <paramref name="database"/>.
    /// Returns false if no save file exists or an error occurs.
    /// </summary>
    public static bool TryLoad(ItemDatabase database, IReadOnlyList<InventorySlot> slots, out int restoredCount)
    {
        restoredCount = 0;

        if (!File.Exists(SaveFilePath))
            return false;

        try
        {
            string json    = File.ReadAllText(SaveFilePath);
            SaveData data  = JsonUtility.FromJson<SaveData>(json);

            if (data?.slots == null)
                return false;

            foreach (SlotRecord record in data.slots)
            {
                if (record.slotIndex < 0 || record.slotIndex >= slots.Count)
                {
                    Debug.LogWarning($"[InventorySaveService] Index de slot invalide ({record.slotIndex}) — ignoré.");
                    continue;
                }

                ItemDefinition item = database.GetById(record.itemId);

                if (item == null)
                {
                    Debug.LogWarning($"[InventorySaveService] Item inconnu '{record.itemId}' — ignoré.");
                    continue;
                }

                slots[record.slotIndex].Set(item, record.quantity);
                restoredCount++;
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[InventorySaveService] Erreur lors du chargement : {e.Message}");
            return false;
        }
    }

    /// <summary>Deletes the save file from disk. Useful for resets or new game.</summary>
    public static void Delete()
    {
        if (File.Exists(SaveFilePath))
            File.Delete(SaveFilePath);
    }

    // ── Data structures ───────────────────────────────────────────────────────

    [Serializable]
    private class SaveData
    {
        public List<SlotRecord> slots;
    }

    [Serializable]
    private class SlotRecord
    {
        public int    slotIndex;
        public string itemId;
        public int    quantity;
    }
}
