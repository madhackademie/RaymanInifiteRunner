/// <summary>
/// Identifiants des pistes de progression (halo inventaire).
/// Les IDs restent stables pour éviter les casses de sauvegarde.
/// </summary>
public static class ProgressionTrackId
{
    public const string Commerce = "track.commerce";
    public const string PlantCulture = "track.plant";
    public const string FishCulture = "track.fish";
    public const string Agronomy = "track.agronomy";
    public const string Logistics = "track.logistics";
    public const string Technology = "track.technology";
    public const string Reserved07 = "track.reserved.07";
    public const string Reserved08 = "track.reserved.08";

    /// <summary>Ordre halo : sens horaire depuis le haut (12 h).</summary>
    public static readonly string[] HaloSlotOrder =
    {
        Commerce,
        PlantCulture,
        FishCulture,
        Agronomy,
        Logistics,
        Technology,
        Reserved07,
        Reserved08,
    };

    public static bool IsReserved(string trackId) =>
        trackId == Reserved07 || trackId == Reserved08;

    public static string GetShortLabel(string trackId)
    {
        return trackId switch
        {
            Commerce => "Commerce",
            PlantCulture => "Plante",
            FishCulture => "Poisson",
            Agronomy => "Agro",
            Logistics => "Logis",
            Technology => "Tech",
            Reserved07 => "Bientot",
            Reserved08 => "Bientot",
            _ => "Inconnu",
        };
    }

    public static string GetDisplayName(string trackId)
    {
        return trackId switch
        {
            Commerce => "Commerce",
            PlantCulture => "Culture des plantes",
            FishCulture => "Culture des poissons",
            Agronomy => "Agronomie",
            Logistics => "Logistique",
            Technology => "Technologie",
            Reserved07 => "Piste reservee 7",
            Reserved08 => "Piste reservee 8",
            _ => trackId,
        };
    }
}
