using TMPro;
using UnityEngine;

/// <summary>
/// Met à jour un TMP à partir d'une key LanguageManager.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTmpText : MonoBehaviour
{
    [SerializeField] private string localizationKey;

    private TextMeshProUGUI _label;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.LanguageChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.LanguageChanged -= Refresh;
    }

    public void SetKey(string key)
    {
        localizationKey = key;
        Refresh();
    }

    private void Refresh()
    {
        if (_label == null)
            _label = GetComponent<TextMeshProUGUI>();

        if (_label == null || string.IsNullOrEmpty(localizationKey))
            return;

        if (LanguageManager.Instance == null)
            return;

        _label.text = LanguageManager.Instance.GetText(localizationKey);
    }
}
