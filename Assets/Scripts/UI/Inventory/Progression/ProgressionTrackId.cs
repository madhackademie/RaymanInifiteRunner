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
        if (LanguageManager.Instance == null)
            return frenchFallback;

        string text = LanguageManager.Instance.GetText(key);
        return string.IsNullOrEmpty(text) || text == key ? frenchFallback : text;
    }

    /// <summary>Nom complet localise de la piste (fallback francais si cle absente).</summary>
    public static string GetDisplayName(string trackId)
    {
        return trackId switch
        {
            Commerce => Localize("TRACK_NAME_COMMERCE", "Commerce"),
            PlantCulture => Localize("TRACK_NAME_PLANT", "Culture des plantes"),
            FishCulture => Localize("TRACK_NAME_FISH", "Culture des poissons"),
            Agronomy => Localize("TRACK_NAME_AGRO", "Agronomie"),
            Logistics => Localize("TRACK_NAME_LOGISTICS", "Logistique"),
            Technology => Localize("TRACK_NAME_TECH", "Technologie"),
            Reserved07 => Localize("TRACK_NAME_RESERVED_07", "Piste reservee 7"),
            Reserved08 => Localize("TRACK_NAME_RESERVED_08", "Piste reservee 8"),
            _ => trackId,
        };
    }
}
