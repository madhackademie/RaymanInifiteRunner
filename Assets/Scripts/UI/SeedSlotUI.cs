using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A single slot in the seed selection panel.
/// Displays the seed icon and name, and fires <see cref="OnSlotClicked"/> when selected.
/// </summary>
public class SeedSlotUI : MonoBehaviour
{
    [SerializeField] private Image seedIcon;
    [SerializeField] private TextMeshProUGUI seedNameLabel;
    [SerializeField] private Button button;

    /// <summary>Fired when the player clicks this slot. Passes the bound seed entry.</summary>
    public event Action<SeedEntry> OnSlotClicked;

    private SeedEntry boundEntry;

    private void Awake()
    {
        button.onClick.AddListener(HandleClick);
    }

    /// <summary>Binds a <see cref="SeedEntry"/> to this slot and refreshes the display.</summary>
    public void Bind(SeedEntry entry)
    {
        boundEntry = entry;
        seedNameLabel.text = entry.plantDefinition != null ? entry.plantDefinition.displayName : "—";

        if (entry.plantDefinition != null && entry.plantDefinition.spriteGraine != null)
            seedIcon.sprite = entry.plantDefinition.spriteGraine;
        else
            seedIcon.sprite = null;

        seedIcon.enabled = seedIcon.sprite != null;
    }

    /// <summary>
    /// Enables or disables the slot. A disabled slot is visually greyed out and cannot be clicked.
    /// Call this after <see cref="Bind"/> to reflect footprint availability.
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;

        // Dim icon and label when the footprint does not fit
        float alpha = interactable ? 1f : 0.35f;
        Color iconColor  = seedIcon.color;
        Color labelColor = seedNameLabel.color;
        iconColor.a  = alpha;
        labelColor.a = alpha;
        seedIcon.color         = iconColor;
        seedNameLabel.color    = labelColor;
    }

    private void HandleClick()
    {
        OnSlotClicked?.Invoke(boundEntry);
    }
}
