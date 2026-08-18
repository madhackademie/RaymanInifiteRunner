/// <summary>
/// Phase de déblocage d'un canal de vente (hors cooldown post-vente).
/// </summary>
public enum SaleChannelProgressionPhase
{
    Unlocked,
    Locked,
    Unlockable,
    ResearchInProgress,
}
