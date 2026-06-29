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
            ActionPointFatigueTier.Caution => "Zone modérée",
            ActionPointFatigueTier.Fatigue => "Zone fatigue",
            _ => "Zone confort",
        };
    }

    public static string GetZoneTooltipBody(ActionPointFatigueTier tier)
    {
        return tier switch
        {
            ActionPointFatigueTier.Caution =>
                $"De {BuffManager.ComfortZoneEnd} à {BuffManager.CautionZoneEnd} PA consommés.\n" +
                $"+{(int)(BuffManager.CautionCostMultiplier * 100f)} % sur le coût de toutes vos actions.",

            ActionPointFatigueTier.Fatigue =>
                $"De {BuffManager.CautionZoneEnd} à {BuffManager.MaxDailyActionPoints} PA consommés.\n" +
                $"+{(int)(BuffManager.FatigueCostMultiplier * 100f)} % sur le coût de toutes vos actions.",

            _ =>
                $"De 0 à {BuffManager.ComfortZoneEnd} PA consommés.\n" +
                "Aucun malus sur vos actions.",
        };
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
