using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Charge les tables JSON par locale, expose GetText avec fallback EN, notifie les vues TMP.
/// </summary>
[DefaultExecutionOrder(-900)]
public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    public event Action LanguageChanged;

    public GameLanguage CurrentLanguage { get; private set; } = GameLanguage.English;

    private readonly Dictionary<string, string> _entries = new();
    private Dictionary<string, string> _fallbackEnglish = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureExists()
    {
        if (Instance != null)
            return;

        var host = new GameObject(nameof(LanguageManager));
        host.AddComponent<LanguageManager>();
        DontDestroyOnLoad(host);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _fallbackEnglish = LoadTable(GameLanguage.English);
        var initial = GameOptionsStore.LoadLanguageOrDefault();
        ApplyLanguage(initial, notify: false);
    }

    public void SetLanguage(GameLanguage language)
    {
        if (language == CurrentLanguage)
            return;

        ApplyLanguage(language, notify: true);
        GameOptionsStore.SaveLanguage(language);
    }

    public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        if (_entries.TryGetValue(key, out string value) && !string.IsNullOrEmpty(value))
            return value;

        if (_fallbackEnglish.TryGetValue(key, out string fallback) && !string.IsNullOrEmpty(fallback))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning($"[LanguageManager] Key manquante '{key}' pour {CurrentLanguage}, fallback EN.");
#endif
            return fallback;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning($"[LanguageManager] Key inconnue '{key}'.");
#endif
        return key;
    }

    private void ApplyLanguage(GameLanguage language, bool notify)
    {
        CurrentLanguage = language;
        _entries.Clear();
        foreach (var pair in LoadTable(language))
            _entries[pair.Key] = pair.Value;

        if (notify)
            LanguageChanged?.Invoke();
    }

    private static Dictionary<string, string> LoadTable(GameLanguage language)
    {
        var result = new Dictionary<string, string>();
        string suffix = GameOptionsStore.ToResourceSuffix(language);
        var asset = Resources.Load<TextAsset>($"Localization/strings_{suffix}");
        if (asset == null)
        {
            Debug.LogError($"[LanguageManager] Fichier introuvable : Resources/Localization/strings_{suffix}.json");
            return result;
        }

        ParseJson(asset.text, result);
        return result;
    }

    private static void ParseJson(string json, Dictionary<string, string> target)
    {
        if (string.IsNullOrWhiteSpace(json))
            return;

        LocalizationTableDto dto = JsonUtility.FromJson<LocalizationTableDto>(json);
        if (dto?.entries == null)
            return;

        foreach (LocalizationEntryDto entry in dto.entries)
        {
            if (entry == null || string.IsNullOrEmpty(entry.key))
                continue;

            target[entry.key] = entry.value ?? string.Empty;
        }
    }
}
