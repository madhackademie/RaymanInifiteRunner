using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Persistance du solde journalier de points d'action.
/// </summary>
public static class ActionPointSaveService
{
    private const string SaveFileName = "action_points.json";

    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool TryLoad(out int remainingPoints, out long lastResetUtcTicks)
    {
        remainingPoints = 0;
        lastResetUtcTicks = 0;

        if (!File.Exists(SaveFilePath))
            return false;

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data == null)
                return false;

            remainingPoints = Mathf.Max(0, data.remainingPoints);
            lastResetUtcTicks = data.lastResetUtcTicks;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[ActionPointSaveService] Erreur chargement : {e.Message}");
            return false;
        }
    }

    public static void Save(int remainingPoints, long lastResetUtcTicks)
    {
        var data = new SaveData
        {
            remainingPoints = Mathf.Max(0, remainingPoints),
            lastResetUtcTicks = lastResetUtcTicks,
        };

        string json = JsonUtility.ToJson(data, prettyPrint: true);

        try
        {
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ActionPointSaveService] Erreur écriture : {e.Message}");
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
        public int remainingPoints;
        public long lastResetUtcTicks;
    }
}
