using System.Collections.Generic;
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
                stageElapsedSeconds = grow.CurrentStageElapsedSeconds,
                stageUpdatedUtcTicks = FarmTimeService.UtcNowTicks
            });
        }

        return records;
    }

    /// <summary>Délègue à <see cref="FarmTimeService"/> (ticks + plafond + garde-fous horloge).</summary>
    public static float ComputeOfflineSeconds(long savedUtcTicks, string lastSavedUtcIso) =>
        FarmTimeService.ComputeOfflineSeconds(savedUtcTicks, lastSavedUtcIso);
}
