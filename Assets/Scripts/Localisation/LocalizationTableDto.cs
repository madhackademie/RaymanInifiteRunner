using System;

/// <summary>Format JSON Resources/Localization/strings_xx.json</summary>
[Serializable]
public class LocalizationTableDto
{
    public int version;
    public string locale;
    public LocalizationEntryDto[] entries;
}

[Serializable]
public class LocalizationEntryDto
{
    public string key;
    public string value;
}
