using UnityEngine;

/// <summary>
/// Persistance des options joueur (PlayerPrefs proto). Extensible : volume musique / SFX, sensibilité caméra, etc.
/// </summary>
public static class GameOptionsStore
{
    private const string LanguagePrefKey = "options.language";

    // Future : private const string MusicVolumePrefKey = "options.music_volume";

    public static bool HasLanguageOverride => PlayerPrefs.HasKey(LanguagePrefKey);

    public static GameLanguage LoadLanguageOrDefault()
    {
        if (PlayerPrefs.HasKey(LanguagePrefKey))
            return (GameLanguage)PlayerPrefs.GetInt(LanguagePrefKey, (int)GameLanguage.English);

        return MapSystemLanguage(Application.systemLanguage);
    }

    public static void SaveLanguage(GameLanguage language)
    {
        PlayerPrefs.SetInt(LanguagePrefKey, (int)language);
        PlayerPrefs.Save();
    }

    public static string ToResourceSuffix(GameLanguage language)
    {
        return language switch
        {
            GameLanguage.French => "fr",
            GameLanguage.German => "de",
            GameLanguage.English => "en",
            GameLanguage.Spanish => "es",
            GameLanguage.Italian => "it",
            _ => "en"
        };
    }

    public static GameLanguage FromResourceSuffix(string suffix)
    {
        return suffix switch
        {
            "fr" => GameLanguage.French,
            "de" => GameLanguage.German,
            "en" => GameLanguage.English,
            "es" => GameLanguage.Spanish,
            "it" => GameLanguage.Italian,
            _ => GameLanguage.English
        };
    }

    private static GameLanguage MapSystemLanguage(SystemLanguage systemLanguage)
    {
        return systemLanguage switch
        {
            SystemLanguage.French => GameLanguage.French,
            SystemLanguage.German => GameLanguage.German,
            SystemLanguage.English => GameLanguage.English,
            SystemLanguage.Spanish => GameLanguage.Spanish,
            SystemLanguage.Italian => GameLanguage.Italian,
            _ => GameLanguage.English
        };
    }
}
