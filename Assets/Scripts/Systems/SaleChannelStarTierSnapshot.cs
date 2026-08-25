/// <summary>
/// Copy tooltip palier étoiles (lvl courant + suivant) pour un canal de vente.
/// </summary>
public readonly struct SaleChannelStarTierSnapshot
{
    public SaleChannelStarTierSnapshot(
        string channelTitle,
        int filledStarCount,
        string currentTitle,
        string currentBody,
        string nextTitle,
        string nextBody,
        string rewardText,
        SaleChannelStarProgressRow sales,
        SaleChannelStarProgressRow items,
        SaleChannelStarProgressRow gold)
    {
        ChannelTitle = channelTitle;
        FilledStarCount = filledStarCount;
        CurrentTitle = currentTitle;
        CurrentBody = currentBody;
        NextTitle = nextTitle;
        NextBody = nextBody;
        RewardText = rewardText;
        Sales = sales;
        Items = items;
        Gold = gold;
    }

    public string ChannelTitle { get; }
    public int FilledStarCount { get; }
    public string CurrentTitle { get; }
    public string CurrentBody { get; }
    public string NextTitle { get; }
    public string NextBody { get; }
    public string RewardText { get; }
    public SaleChannelStarProgressRow Sales { get; }
    public SaleChannelStarProgressRow Items { get; }
    public SaleChannelStarProgressRow Gold { get; }
}
