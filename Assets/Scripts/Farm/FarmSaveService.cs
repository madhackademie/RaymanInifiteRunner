using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Persistance JSON prototype de l'etat des plantes en ferme.
/// </summary>
public static class FarmSaveService
{
    private const string SaveFileName = "farm_state.json";
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    /// <summary>
    /// Sauvegarde la liste runtime des plantes posees.
    /// On stocke volontairement un format simple (id, ancre, stade, temps courant)
    /// pour faciliter la migration vers cloud plus tard.
    /// </summary>
    public static void Save(List<FarmPlantRecord> plants)
    {
        FarmSaveData data = new()
        {
            saveVersion = 1,
            lastSavedUtc = DateTime.UtcNow.ToString("O"),
            plants = plants ?? new List<FarmPlantRecord>()
        };

        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[FarmSaveService] Echec ecriture save: {e.Message}");
        }
    }

    public static bool TryLoad(out FarmSaveData data)
    {
        data = null;

        if (!File.Exists(SaveFilePath))
            return false;

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            data = JsonUtility.FromJson<FarmSaveData>(json);
            return data != null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FarmSaveService] Echec lecture save: {e.Message}");
            return false;
        }
    }

    [Serializable]
    public class FarmSaveData
    {
        public int saveVersion = 1;
        public string lastSavedUtc;
        public List<FarmPlantRecord> plants = new();
    }
}

[Serializable]
public class FarmPlantRecord
{
    // Identifiant fonctionnel de la plante (stable, independant du nom du prefab).
    public string plantId;
    // Ancre grille de la plante (point de reference pour GetOccupiedCells()).
    public int anchorX;
    public int anchorY;
    // Stade courant + progression du stade au moment de la sauvegarde.
    public PlantGrow.GrowthStage currentStage;
    public float stageElapsedSeconds;
}

