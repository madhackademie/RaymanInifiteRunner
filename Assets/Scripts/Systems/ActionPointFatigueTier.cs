/// <summary>
/// Paliers de fatigue liés aux PA consommés dans la journée.
/// </summary>
public enum ActionPointFatigueTier
{
    /// <summary>0–80 PA consommés — aucun malus.</summary>
    Comfort = 0,

    /// <summary>80–120 PA consommés — +25 % sur le coût des actions.</summary>
    Caution = 1,

    /// <summary>120–160 PA consommés — +50 % sur le coût des actions.</summary>
    Fatigue = 2,
}
