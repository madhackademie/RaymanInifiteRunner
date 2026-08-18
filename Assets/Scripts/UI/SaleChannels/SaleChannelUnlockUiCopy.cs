using System.Text;
using UnityEngine;

/// <summary>
/// Textes tooltip / statut pour le déblocage des canaux de vente.
/// </summary>
public static class SaleChannelUnlockUiCopy
{
    public static string BuildTooltipTitle(SaleChannelUnlockDefinition definition)
    {
        if (definition == null)
            return "Canal verrouillé";

        return $"Débloquer — {definition.DisplayName}";
    }

    public static string BuildTooltipBody(
        SaleChannelUnlockDefinition definition,
        SaleChannelUnlockService.ProgressCounters counters,
        int walletGold,
        SaleChannelProgressionPhase phase,
        float researchRemainingSeconds)
    {
        if (definition == null)
            return "Conditions indisponibles.";

        if (phase == SaleChannelProgressionPhase.Unlocked)
            return "Canal actif.";

        if (phase == SaleChannelProgressionPhase.ResearchInProgress)
        {
            return $"Recherche en cours…\n" +
                   $"Temps restant : {SaleChannelCooldownFormatter.FormatRemainingSeconds(researchRemainingSeconds)}";
        }

        var builder = new StringBuilder();
        builder.AppendLine("Conditions :");

        AppendConditionLine(
            builder,
            BuildSaleCountLabel(definition),
            counters.SaleCount,
            definition.RequiredSaleCount);

        if (definition.RequiredItemsSold > 0)
        {
            AppendConditionLine(
                builder,
                "Salades écoulées (canal requis)",
                counters.ItemsSold,
                definition.RequiredItemsSold);
        }

        if (!string.IsNullOrWhiteSpace(definition.RequiredUnlockedChannelId))
        {
            bool prerequisiteMet = counters.PrerequisiteUnlocked;
            builder.AppendLine(FormatCheck(prerequisiteMet) + " " + BuildPrerequisiteLabel(definition));
        }

        if (definition.RequiredWalletGold > 0)
        {
            AppendConditionLine(
                builder,
                "Or en poche (minimum)",
                walletGold,
                definition.RequiredWalletGold);
        }

        builder.AppendLine();
        builder.AppendLine($"Coût recherche : {definition.ResearchCostGold} or");
        builder.AppendLine(
            $"Durée : {FormatResearchDuration(definition.ResearchDurationSeconds)}");

        if (phase == SaleChannelProgressionPhase.Unlockable)
            builder.AppendLine("\nToutes les conditions sont remplies — touchez pour lancer la recherche.");

        return builder.ToString().TrimEnd();
    }

    public static string BuildStatusLabel(
        SaleChannelProgressionPhase phase,
        float researchRemainingSeconds)
    {
        return phase switch
        {
            SaleChannelProgressionPhase.Unlockable => "Prêt !",
            SaleChannelProgressionPhase.ResearchInProgress =>
                SaleChannelCooldownFormatter.FormatRemainingSeconds(researchRemainingSeconds),
            _ => "Bientôt",
        };
    }

    private static void AppendConditionLine(StringBuilder builder, string label, int current, int required)
    {
        if (required <= 0)
            return;

        builder.AppendLine($"{FormatCheck(current >= required)} {label} : {current}/{required}");
    }

    private static string FormatCheck(bool met) => met ? "✓" : "○";

    private static string BuildSaleCountLabel(SaleChannelUnlockDefinition definition)
    {
        string channelLabel = ResolveChannelLabel(definition.RequiredSaleCountChannelId);
        return $"Ventes {channelLabel}";
    }

    private static string BuildPrerequisiteLabel(SaleChannelUnlockDefinition definition)
    {
        return $"Canal « {ResolveChannelLabel(definition.RequiredUnlockedChannelId)} » débloqué";
    }

    private static string ResolveChannelLabel(string channelId)
    {
        if (channelId == SaleChannelId.Neighbor)
            return "voisinage";

        if (channelId == SaleChannelId.Sash)
            return "bandoulière";

        if (channelId == SaleChannelId.Bike)
            return "vélo marchand";

        return channelId ?? "?";
    }

    private static string FormatResearchDuration(float seconds)
    {
        if (seconds >= 3600f)
        {
            int hours = Mathf.CeilToInt(seconds / 3600f);
            return hours <= 1 ? "1 h" : $"{hours} h";
        }

        int minutes = Mathf.CeilToInt(seconds / 60f);
        return minutes <= 1 ? "1 min" : $"{minutes} min";
    }
}
