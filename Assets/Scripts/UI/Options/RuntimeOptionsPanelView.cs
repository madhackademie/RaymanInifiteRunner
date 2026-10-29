using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panneau Options extensible (langue V1 ; sections audio / caméra plus tard).
/// </summary>
public class RuntimeOptionsPanelView : MonoBehaviour
{
    private static readonly (GameLanguage lang, string labelKey)[] LanguageChoices =
    {
        (GameLanguage.French, "LANG_NAME_FR"),
        (GameLanguage.German, "LANG_NAME_DE"),
        (GameLanguage.English, "LANG_NAME_EN"),
        (GameLanguage.Spanish, "LANG_NAME_ES"),
        (GameLanguage.Italian, "LANG_NAME_IT")
    };

    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI languageSectionLabel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI closeButtonLabel;
    [SerializeField] private Transform languageToggleRoot;
    [SerializeField] private Transform futureSectionsRoot;

    public event Action CloseRequested;

    private readonly List<(Toggle toggle, GameLanguage language)> _languageToggles = new();
    private bool _suppressToggleCallback;

    public void WireBuiltReferences(
        TextMeshProUGUI title,
        TextMeshProUGUI languageSection,
        Button close,
        TextMeshProUGUI closeLabel,
        Transform togglesRoot,
        Transform futureRoot)
    {
        titleLabel = title;
        languageSectionLabel = languageSection;
        closeButton = close;
        closeButtonLabel = closeLabel;
        languageToggleRoot = togglesRoot;
        futureSectionsRoot = futureRoot;
        Bind();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.LanguageChanged += OnLanguageChanged;

        RefreshStaticLabels();
        SyncLanguageToggles();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.LanguageChanged -= OnLanguageChanged;
    }

    private void Bind()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(HandleCloseClicked);
            closeButton.onClick.AddListener(HandleCloseClicked);
        }
    }

    private void HandleCloseClicked() => CloseRequested?.Invoke();

    private void OnLanguageChanged()
    {
        RefreshStaticLabels();
        SyncLanguageToggles();
    }

    private void RefreshStaticLabels()
    {
        if (LanguageManager.Instance == null)
            return;

        if (titleLabel != null)
            titleLabel.text = LanguageManager.Instance.GetText("OPTIONS_TITLE");

        if (languageSectionLabel != null)
            languageSectionLabel.text = LanguageManager.Instance.GetText("OPTIONS_SECTION_LANGUAGE");

        if (closeButtonLabel != null)
            closeButtonLabel.text = LanguageManager.Instance.GetText("OPTIONS_CLOSE");

        foreach (var (toggle, language) in _languageToggles)
        {
            if (toggle == null)
                continue;

            var label = toggle.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = LanguageManager.Instance.GetText(LanguageLabelKey(language));
        }
    }

    private void SyncLanguageToggles()
    {
        if (LanguageManager.Instance == null || _languageToggles.Count == 0)
            return;

        _suppressToggleCallback = true;
        GameLanguage current = LanguageManager.Instance.CurrentLanguage;
        foreach (var (toggle, language) in _languageToggles)
        {
            if (toggle != null)
                toggle.isOn = language == current;
        }

        _suppressToggleCallback = false;
    }

    public void RegisterLanguageToggle(Toggle toggle, GameLanguage language)
    {
        if (toggle == null)
            return;

        _languageToggles.Add((toggle, language));
        toggle.onValueChanged.AddListener(isOn => HandleLanguageToggle(language, isOn));
    }

    private void HandleLanguageToggle(GameLanguage language, bool isOn)
    {
        if (_suppressToggleCallback || !isOn || LanguageManager.Instance == null)
            return;

        LanguageManager.Instance.SetLanguage(language);
    }

    private static string LanguageLabelKey(GameLanguage language)
    {
        return language switch
        {
            GameLanguage.French => "LANG_NAME_FR",
            GameLanguage.German => "LANG_NAME_DE",
            GameLanguage.English => "LANG_NAME_EN",
            GameLanguage.Spanish => "LANG_NAME_ES",
            GameLanguage.Italian => "LANG_NAME_IT",
            _ => "LANG_NAME_EN"
        };
    }

    /// <summary>Host pour sliders musique / SFX (vide en V1).</summary>
    public Transform FutureSectionsRoot => futureSectionsRoot;
}
