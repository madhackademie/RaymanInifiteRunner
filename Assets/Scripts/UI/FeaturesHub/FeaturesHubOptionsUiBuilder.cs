using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Construit le panneau Options sur le hub Plus (contenu seulement — bouton = prefab SettingsButton).
/// </summary>
public static class FeaturesHubOptionsUiBuilder
{
    private static readonly Color AccentColor = new(1f, 0.78f, 0.2f, 1f);

    /// <summary>Crée le panneau Options sous OptionsPanelHost (stretch).</summary>
    public static GameObject BuildOptionsPanel(
        RectTransform host,
        TMP_FontAsset font,
        out RuntimeOptionsPanelView panelView)
    {
        panelView = null;
        if (host == null)
            return null;

        var root = new GameObject("OptionsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        root.layer = 5;
        var rootRt = root.GetComponent<RectTransform>();
        rootRt.SetParent(host, false);
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

        var rootImage = root.GetComponent<Image>();
        HudModalBackdrop.ApplyContentPanel(rootImage);

        var content = new GameObject("Content", typeof(RectTransform));
        content.layer = 5;
        var contentRt = content.GetComponent<RectTransform>();
        contentRt.SetParent(rootRt, false);
        contentRt.anchorMin = new Vector2(0.5f, 0.5f);
        contentRt.anchorMax = new Vector2(0.5f, 0.5f);
        contentRt.sizeDelta = new Vector2(620f, 720f);
        contentRt.anchoredPosition = Vector2.zero;

        var title = CreateTmp(contentRt, font, "TitleLabel", 44f, TextAlignmentOptions.Center, new Vector2(0f, 300f), new Vector2(560f, 64f));
        var section = CreateTmp(contentRt, font, "LanguageSectionLabel", 32f, TextAlignmentOptions.Left, new Vector2(-260f, 200f), new Vector2(520f, 48f));

        var toggleRoot = new GameObject("LanguageToggles", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ToggleGroup));
        toggleRoot.layer = 5;
        var toggleRt = toggleRoot.GetComponent<RectTransform>();
        toggleRt.SetParent(contentRt, false);
        toggleRt.anchorMin = new Vector2(0.5f, 1f);
        toggleRt.anchorMax = new Vector2(0.5f, 1f);
        toggleRt.pivot = new Vector2(0.5f, 1f);
        toggleRt.anchoredPosition = new Vector2(0f, 160f);
        toggleRt.sizeDelta = new Vector2(520f, 320f);

        var toggleGroup = toggleRoot.GetComponent<ToggleGroup>();
        toggleGroup.allowSwitchOff = false;

        var vlg = toggleRoot.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 8f;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        var futureRoot = new GameObject("FutureSectionsHost", typeof(RectTransform));
        futureRoot.layer = 5;
        var futureRt = futureRoot.GetComponent<RectTransform>();
        futureRt.SetParent(contentRt, false);
        futureRt.anchorMin = new Vector2(0.5f, 0f);
        futureRt.anchorMax = new Vector2(0.5f, 0f);
        futureRt.pivot = new Vector2(0.5f, 0f);
        futureRt.anchoredPosition = new Vector2(0f, 120f);
        futureRt.sizeDelta = new Vector2(520f, 80f);

        var closeButton = CreateCloseButton(contentRt, font, out TextMeshProUGUI closeLabel);

        panelView = root.AddComponent<RuntimeOptionsPanelView>();
        panelView.WireBuiltReferences(title, section, closeButton, closeLabel, toggleRt, futureRt);

        CreateLanguageToggles(toggleRt, font, panelView);

        return root;
    }

    private static void CreateLanguageToggles(Transform parent, TMP_FontAsset font, RuntimeOptionsPanelView panelView)
    {
        var languages = new[]
        {
            GameLanguage.French,
            GameLanguage.German,
            GameLanguage.English,
            GameLanguage.Spanish,
            GameLanguage.Italian
        };

        var group = parent.GetComponent<ToggleGroup>();
        foreach (GameLanguage language in languages)
        {
            Toggle toggle = CreateLanguageToggleRow(parent, font, language, group);
            panelView.RegisterLanguageToggle(toggle, language);
        }
    }

    private static Toggle CreateLanguageToggleRow(
        Transform parent,
        TMP_FontAsset font,
        GameLanguage language,
        ToggleGroup group)
    {
        var row = new GameObject($"Lang_{language}", typeof(RectTransform), typeof(Toggle), typeof(Image), typeof(LayoutElement));
        row.layer = 5;
        var rowRt = row.GetComponent<RectTransform>();
        rowRt.SetParent(parent, false);
        row.GetComponent<LayoutElement>().preferredHeight = 52f;

        var bg = row.GetComponent<Image>();
        HudModalBackdrop.SetupSolidFillImage(bg);
        bg.color = new Color(0.14f, 0.14f, 0.17f, 1f);

        var toggle = row.GetComponent<Toggle>();
        toggle.targetGraphic = bg;
        toggle.group = group;

        var check = new GameObject("Check", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        check.layer = 5;
        var checkRt = check.GetComponent<RectTransform>();
        checkRt.SetParent(rowRt, false);
        checkRt.anchorMin = new Vector2(0f, 0.5f);
        checkRt.anchorMax = new Vector2(0f, 0.5f);
        checkRt.pivot = new Vector2(0f, 0.5f);
        checkRt.anchoredPosition = new Vector2(16f, 0f);
        checkRt.sizeDelta = new Vector2(28f, 28f);
        var checkImg = check.GetComponent<Image>();
        HudModalBackdrop.SetupSolidFillImage(checkImg);
        checkImg.color = AccentColor;
        toggle.graphic = checkImg;

        var label = CreateTmp(rowRt, font, "Label", 28f, TextAlignmentOptions.Left, new Vector2(56f, 0f), new Vector2(420f, 48f));
        label.rectTransform.anchorMin = new Vector2(0f, 0.5f);
        label.rectTransform.anchorMax = new Vector2(1f, 0.5f);
        label.rectTransform.offsetMin = new Vector2(56f, -24f);
        label.rectTransform.offsetMax = new Vector2(-8f, 24f);
        if (LanguageManager.Instance != null)
            label.text = LanguageManager.Instance.GetText(LanguageKey(language));

        return toggle;
    }

    private static string LanguageKey(GameLanguage language)
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

    private static Button CreateCloseButton(RectTransform parent, TMP_FontAsset font, out TextMeshProUGUI label)
    {
        var go = new GameObject("CloseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.layer = 5;
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 16f);
        rt.sizeDelta = new Vector2(320f, 72f);

        var image = go.GetComponent<Image>();
        HudModalBackdrop.SetupSolidFillImage(image);
        image.color = new Color(0.2f, 0.45f, 0.35f, 1f);

        label = CreateTmp(rt, font, "Label", 30f, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero);
        label.rectTransform.anchorMin = Vector2.zero;
        label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = Vector2.zero;
        label.rectTransform.offsetMax = Vector2.zero;

        return go.GetComponent<Button>();
    }

    private static TextMeshProUGUI CreateTmp(
        RectTransform parent,
        TMP_FontAsset font,
        string name,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2 anchoredPos,
        Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.layer = 5;
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (font != null)
            tmp.font = font;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        return tmp;
    }
}
