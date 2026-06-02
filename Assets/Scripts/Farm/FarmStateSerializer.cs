using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Conversion entre l'état runtime des plantes (scène) et les enregistrements de sauvegarde.
/// Logique de (dé)sérialisation extraite de BiofiltreManager.
/// </summary>
public static class FarmStateSerializer
{
    /// <summary>
    /// Construit la liste des enregistrements à partir des plantes présentes sous le conteneur.
    /// Une plante n'est incluse que si elle porte un marqueur de persistance et un PlantGrow valides.
    /// </summary>
    public static List<FarmPlantRecord> BuildRecords(Transform plantsContainer)
    {
        var records = new List<FarmPlantRecord>();
        if (plantsContainer == null)
            return records;

        foreach (Transform child in plantsContainer)
        {
            if (!child.TryGetComponent(out PlantPersistenceMarker marker))
                continue;

            if (!child.TryGetComponent(out PlantGrow grow))
                continue;

            if (string.IsNullOrEmpty(marker.PlantId))
                continue;

            records.Add(new FarmPlantRecord
            {
                plantId = marker.PlantId,
                anchorX = marker.Anchor.x,
                anchorY = marker.Anchor.y,
                currentStage = grow.CurrentStage,
                stageElapsedSeconds = grow.CurrentStageElapsedSeconds
            });
        }

        return records;
    }

    /// <summary>
    /// Secondes écoulées (hors ligne) entre l'horodatage UTC sauvegardé et maintenant.
    /// Retourne 0 si l'horodatage est absent ou invalide.
    /// </summary>
    public static float ComputeOfflineSeconds(string lastSavedUtcIso)
    {
        if (string.IsNullOrEmpty(lastSavedUtcIso) ||
            !DateTime.TryParse(lastSavedUtcIso, null, DateTimeStyles.RoundtripKind, out DateTime savedUtc))
        {
            return 0f;
        }

        return Mathf.Max(0f, (float)(DateTime.UtcNow - savedUtc).TotalSeconds);
    }
}
