using System;

/// <summary>
/// Jeux / univers où l'item est pertinent (metadata multiverse).
/// </summary>
[Flags]
public enum ItemGameScope
{
    None = 0,
    Farm = 1 << 0,
    Runner = 1 << 1,
    Shooter = 1 << 2,

    /// <summary>Tous les jeux (cross-univers).</summary>
    Global = Farm | Runner | Shooter
}
