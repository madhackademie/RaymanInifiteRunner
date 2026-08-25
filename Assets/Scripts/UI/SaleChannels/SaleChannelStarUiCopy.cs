using System.Text;
using UnityEngine;

/// <summary>
/// Textes tooltip étoiles : palier courant + progression vers le suivant.
/// </summary>
public static class SaleChannelStarUiCopy
{
    private const int DefaultFilledStars = 1;
    private const string RewardText = "Récompense : +1 voisin / volume / autres légumes (TBD)";

    public static SaleChannelStarTierSnapshot Build(string channelTitle, string channelId, int filledStarCount)
    {
        int filled = filledStarCount > 0 ? filledStarCount : DefaultFilledStars;
        TryGetCounters(channelId, out int saleCount, out int itemsSold, out int goldEarned);

        var sales = new SaleChannelStarProgressRow(
            "Ventes", saleCount, SaleChannelStarProgression.Star2RequiredSales);
        var items = new SaleChannelStarProgressRow(
            "Salades", itemsSold, SaleChannelStarProgression.Star2RequiredItems);
        var gold = new SaleChannelStarProgressRow(
            "Or gagné", goldEarned, SaleChannelStarProgression.Star2RequiredGold);

        return new SaleChannelStarTierSnapshot(
            channelTitle,
            filled,
            "★1 — Palier actuel",
            "1 voisin · 1–3 salades après cooldown",
            "★2 — Palier suivant",
            BuildFallbackBody(sales, items, gold),
            RewardText,
            sales,
            items,
            gold);
    }

    private static string BuildFallbackBody(
        SaleChannelStarProgressRow sales,
        SaleChannelStarProgressRow items,
        SaleChannelStarProgressRow gold)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Progression :");
        AppendLine(builder, sales);
        AppendLine(builder, items);
        AppendLine(builder, gold);
        builder.Append(RewardText);
        return builder.ToString();
    }

    private static void AppendLine(StringBuilder builder, SaleChannelStarProgressRow row)
    {
        int clamped = Mathf.Max(0, row.Current);
        string mark = clamped >= row.Required ? "✓" : "○";
        builder.AppendLine($"{mark} {row.Label} : {clamped}/{row.Required}");
    }

    private static void TryGetCounters(string channelId, out int saleCount, out int itemsSold, out int goldEarned)
    {
        saleCount = 0;
        itemsSold = 0;
        goldEarned = 0;

        SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
        if (unlockService == null || string.IsNullOrWhiteSpace(channelId))
            return;

        unlockService.TryGetChannelStats(channelId, out saleCount, out itemsSold, out goldEarned);
    }
}
