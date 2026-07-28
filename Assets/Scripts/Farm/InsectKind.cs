/// <summary>
/// Espèce d'insecte pendant le stade Flowering.
/// </summary>
public enum InsectKind
{
    None = 0,
    Bee = 1,
    Butterfly = 2,
    /// <summary>Au démarrage Flowering : 50 % abeille / 50 % papillon (gardé jusqu'à la fin du stade).</summary>
    RandomBeeOrButterfly = 3,
}
