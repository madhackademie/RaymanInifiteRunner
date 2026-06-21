using System;

/// <summary>
/// Affichage lisible du temps restant avant déblocage d'un canal de vente.
/// </summary>
public static class SaleChannelCooldownFormatter
{
    public static string FormatRemainingSeconds(float remainingSeconds)
    {
        if (remainingSeconds <= 0f)
            return string.Empty;

        TimeSpan remaining = TimeSpan.FromSeconds(Math.Ceiling(remainingSeconds));

        if (remaining.TotalHours >= 1d)
            return $"{(int)remaining.TotalHours}h {remaining.Minutes:D2}m";

        if (remaining.TotalMinutes >= 1d)
            return $"{remaining.Minutes}m {remaining.Seconds:D2}s";

        return $"{remaining.Seconds}s";
    }
}
