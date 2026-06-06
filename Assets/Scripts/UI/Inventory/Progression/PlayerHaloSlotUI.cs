using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Slot périphérique du halo : zone cliquable stable + couche visuelle animable (placeholder).
/// </summary>
public class PlayerHaloSlotUI : MonoBehaviour
{
    [SerializeField] private string trackId = ProgressionTrackId.Commerce;
    [SerializeField] private Button clickButton;
    [SerializeField] private Image animatedVisual;
    [SerializeField] private TextMeshProUGUI placeholderLabel;
    [SerializeField] private TextMeshProUGUI levelBadge;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject lockedOverlay;

    public string TrackId => trackId;

    public event Action<PlayerHaloSlotUI> OnClicked;

    private void Awake()
    {
        if (clickButton != null)
            clickButton.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (clickButton != null)
            clickButton.onClick.RemoveListener(HandleClick);
    }

    /// <summary>Bind runtime (builder éditeur ou prefab).</summary>
    public void Configure(string id, string shortLabel, bool locked, int displayLevel)
    {
        trackId = id;

        if (placeholderLabel != null)
            placeholderLabel.text = shortLabel;

        if (levelBadge != null)
            levelBadge.text = displayLevel > 0 ? displayLevel.ToString() : string.Empty;

        if (lockedOverlay != null)
            lockedOverlay.SetActive(locked);

        if (clickButton != null)
            clickButton.interactable = !locked;
    }

    public void SetAnimator(Animator value) => animator = value;

    public void PlayTrigger(string triggerName)
    {
        if (animator == null || string.IsNullOrEmpty(triggerName))
            return;

        animator.SetTrigger(triggerName);
    }

    private void HandleClick() => OnClicked?.Invoke(this);
}
