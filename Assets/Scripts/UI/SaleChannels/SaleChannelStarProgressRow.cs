/// <summary>
/// Une jauge ★ (ventes / salades / or) pour le tooltip palier.
/// </summary>
public readonly struct SaleChannelStarProgressRow
{
    public SaleChannelStarProgressRow(string label, int current, int required)
    {
        Label = label ?? string.Empty;
        Current = current;
        Required = required;
    }

    public string Label { get; }
    public int Current { get; }
    public int Required { get; }

    public string OverlayText => $"{Label}  {Current}/{Required}";
}
