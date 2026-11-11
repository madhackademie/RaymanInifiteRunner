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
        switch (trackId)
        {
            case Commerce: return Localize("TRACK_SHORT_COMMERCE", "Commerce");
            case PlantCulture: return Localize("TRACK_SHORT_PLANT", "Plante");
            case FishCulture: return Localize("TRACK_SHORT_FISH", "Poisson");
            case Agronomy: return Localize("TRACK_SHORT_AGRO", "Agro");
            case Logistics: return Localize("TRACK_SHORT_LOGISTICS", "Logis");
            case Technology: return Localize("TRACK_SHORT_TECH", "Tech");
            case Reserved07:
            case Reserved08:
                return Localize("TRACK_SHORT_SOON", "Bientôt");
            default: return "Inconnu";
        }
    }

    private static string Localize(string key, string frenchFallback)
    {
        return LanguageManager.Instance != null ? LanguageManager.Instance.GetText(key) : frenchFallback;
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
