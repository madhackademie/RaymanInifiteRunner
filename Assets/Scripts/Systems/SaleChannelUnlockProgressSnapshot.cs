/// <summary>
/// État courant d'un canal pour tooltip UI et feedback bandeau.
/// </summary>
public readonly struct SaleChannelUnlockProgressSnapshot
{
    public SaleChannelUnlockProgressSnapshot(
        string channelId,
        SaleChannelProgressionPhase phase,
        string tooltipTitle,
        string tooltipBody,
        string statusLabel,
        bool canStartResearch)
    {
        ChannelId = channelId;
        Phase = phase;
        TooltipTitle = tooltipTitle;
        TooltipBody = tooltipBody;
        StatusLabel = statusLabel;
        CanStartResearch = canStartResearch;
    }

    public string ChannelId { get; }
    public SaleChannelProgressionPhase Phase { get; }
    public string TooltipTitle { get; }
    public string TooltipBody { get; }
    public string StatusLabel { get; }
    public bool CanStartResearch { get; }
}
