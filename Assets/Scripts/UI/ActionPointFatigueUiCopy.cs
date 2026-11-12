using UnityEngine;

/// <summary>
/// Textes HUD pour les paliers fatigue PA (tooltips zones).
/// </summary>
public static class ActionPointFatigueUiCopy
{
    public static string GetZoneTooltipTitle(ActionPointFatigueTier tier)
    {
        return tier switch
        {
            ActionPointFatigueTier.Caution => GetLocalizedOrFallback("AP_ZONE_CAUTION_TITLE", "Zone modérée"),
            ActionPointFatigueTier.Fatigue => GetLocalizedOrFallback("AP_ZONE_FATIGUE_TITLE", "Zone fatigue"),
            _ => GetLocalizedOrFallback("AP_ZONE_COMFORT_TITLE", "Zone confort"),
        };
    }

    public static string GetZoneTooltipBody(ActionPointFatigueTier tier)
    {
        const string RangeBodyKey = "AP_ZONE_RANGE_BODY";
        const string RangeBodyFallback = "De {0} à {1} PA consommés.\n+{2} % sur le coût de toutes vos actions.";

        switch (tier)
        {
            case ActionPointFatigueTier.Caution:
                return string.Format(
                    GetLocalizedOrFallback(RangeBodyKey, RangeBodyFallback),
                    BuffManager.ComfortZoneEnd,
                    BuffManager.CautionZoneEnd,
                    (int)(BuffManager.CautionCostMultiplier * 100f));

            case ActionPointFatigueTier.Fatigue:
                return string.Format(
                    GetLocalizedOrFallback(RangeBodyKey, RangeBodyFallback),
                    BuffManager.CautionZoneEnd,
                    BuffManager.MaxDailyActionPoints,
                    (int)(BuffManager.FatigueCostMultiplier * 100f));

            default:
                return string.Format(
                    GetLocalizedOrFallback("AP_ZONE_COMFORT_BODY", "De 0 à {0} PA consommés.\nAucun malus sur vos actions."),
                    BuffManager.ComfortZoneEnd);
        }
    }

    /// <summary>
    /// Retourne le texte localisé de la clé, ou le fallback si le LanguageManager est absent ou la clé introuvable.
    /// </summary>
    private static string GetLocalizedOrFallback(string key, string fallback)
    {
        if (LanguageManager.Instance == null)
            return fallback;

        string text = LanguageManager.Instance.GetText(key);
        if (string.IsNullOrEmpty(text) || text == key)
            return fallback;

        return text;
    }

    public static string GetZoneTooltip(ActionPointFatigueTier tier)
    {
        return $"{GetZoneTooltipTitle(tier)}\n{GetZoneTooltipBody(tier)}";
    }

    public static Color GetFatigueIndicatorColor(ActionPointFatigueTier tier)
    {
        return tier switch
        {
            ActionPointFatigueTier.Caution => new Color(0.91f, 0.784f, 0.125f, 1f),
            ActionPointFatigueTier.Fatigue => new Color(0.91f, 0.471f, 0.094f, 1f),
            _ => new Color(0.235f, 0.722f, 0.353f, 1f),
        };
    }
}
