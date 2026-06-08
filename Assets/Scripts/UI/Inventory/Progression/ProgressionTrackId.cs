using System.Collections.Generic;

/// <summary>
/// Identifiants des 8 pistes de compétence (halo inventaire).
/// Source : notes tablette importées 2026-06-08.
/// </summary>
public static class ProgressionTrackId
{
    public const string Marketing = "track.marketing";
    public const string InsectFeed = "track.insect.feed";
    public const string Bioconversion = "track.bioconversion";
    public const string FishReproduction = "track.fish.reproduction";
    public const string Water = "track.water";
    public const string Gardening = "track.gardening";
    public const string Dis = "track.dis";
    public const string Shop = "track.shop";

    /// <summary>Ordre halo : sens horaire depuis le haut (12 h).</summary>
    public static readonly string[] HaloSlotOrder =
    {
        Marketing,
        InsectFeed,
        Bioconversion,
        FishReproduction,
        Water,
        Gardening,
        Dis,
        Shop,
    };

    private static readonly Dictionary<string, TrackLabels> LabelsById = BuildLabels();

    /// <summary>Libellé court pour les slots halo (72 px).</summary>
    public static string GetShortLabel(string trackId)
    {
        if (TryGetLabels(trackId, out TrackLabels labels))
            return labels.Short;

        return trackId ?? string.Empty;
    }

    /// <summary>Libellé complet pour titres overlay / tooltips.</summary>
    public static string GetDisplayName(string trackId)
    {
        if (TryGetLabels(trackId, out TrackLabels labels))
            return labels.Full;

        return trackId ?? string.Empty;
    }

    public static bool IsKnownTrack(string trackId) =>
        !string.IsNullOrEmpty(trackId) && LabelsById.ContainsKey(trackId);

    private static bool TryGetLabels(string trackId, out TrackLabels labels)
    {
        if (!string.IsNullOrEmpty(trackId) && LabelsById.TryGetValue(trackId, out labels))
            return true;

        labels = default;
        return false;
    }

    private static Dictionary<string, TrackLabels> BuildLabels()
    {
        var map = new Dictionary<string, TrackLabels>(HaloSlotOrder.Length);
        for (int i = 0; i < HaloSlotOrder.Length; i++)
        {
            string id = HaloSlotOrder[i];
            map[id] = DefaultLabels[i];
        }

        return map;
    }

    private static readonly TrackLabels[] DefaultLabels =
    {
        new("Marketing", "Marketing"),
        new("Insectes", "Nourriture & élevage insectes"),
        new("Bioconv.", "Bioconversion"),
        new("Poisson", "Reproduction poisson"),
        new("Eau", "Eau"),
        new("Jardin", "Jardinage plantes & graines"),
        new("DIS", "DIS"),
        new("Magasin", "Magasin"),
    };

    private readonly struct TrackLabels
    {
        public TrackLabels(string shortLabel, string fullLabel)
        {
            Short = shortLabel;
            Full = fullLabel;
        }

        public string Short { get; }
        public string Full { get; }
    }
}
