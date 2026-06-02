using System;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Horodatage UTC ferme (prototype). Source unique pour delta offline.
/// Évolution prévue : remplacer <see cref="UtcNowTicks"/> par un temps serveur (UGS / cloud).
/// </summary>
public static class FarmTimeService
{
    public const int SaveVersion = 2;

    /// <summary>Plafond offline (72 h) — anti-farm infini si horloge avancée.</summary>
    public const float MaxOfflineSeconds = 72f * 3600f;

    public static long UtcNowTicks => DateTime.UtcNow.Ticks;

    public static string UtcNowIso => DateTime.UtcNow.ToString("O");

    /// <summary>
    /// Secondes écoulées entre une sauvegarde et maintenant.
    /// Priorité aux ticks UTC ; repli ISO (saves v1) ; plafond ; rejet si horloge reculée.
    /// </summary>
    public static float ComputeOfflineSeconds(long savedUtcTicks, string savedUtcIso)
    {
        if (!TryParseSavedUtc(savedUtcTicks, savedUtcIso, out DateTime savedUtc))
            return 0f;

        DateTime nowUtc = DateTime.UtcNow;
        if (nowUtc < savedUtc)
        {
            Debug.LogWarning(
                "[FarmTimeService] Horloge système antérieure à la sauvegarde — croissance offline ignorée.");
            return 0f;
        }

        float delta = (float)(nowUtc - savedUtc).TotalSeconds;
        if (delta > MaxOfflineSeconds)
        {
            Debug.Log(
                $"[FarmTimeService] Delta offline plafonné à {MaxOfflineSeconds:F0}s (était {delta:F0}s).");
            delta = MaxOfflineSeconds;
        }

        return Mathf.Max(0f, delta);
    }

    private static bool TryParseSavedUtc(long savedUtcTicks, string savedUtcIso, out DateTime savedUtc)
    {
        if (savedUtcTicks > 0)
        {
            savedUtc = new DateTime(savedUtcTicks, DateTimeKind.Utc);
            return true;
        }

        if (!string.IsNullOrEmpty(savedUtcIso) &&
            DateTime.TryParse(savedUtcIso, null, DateTimeStyles.RoundtripKind, out savedUtc))
        {
            return true;
        }

        savedUtc = default;
        return false;
    }
}
