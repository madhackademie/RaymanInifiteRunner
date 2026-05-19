using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A single slot in the seed selection panel.
/// </summary>
public class SeedSlotUI : MonoBehaviour
{
    [SerializeField] private Image seedIcon;
    [SerializeField] private TextMeshProUGUI seedNameLabel;
    [SerializeField] private Button button;

    public event Action<SeedEntry> OnSlotClicked;

    private SeedEntry boundEntry;

    private void Awake()
    {
        button.onClick.AddListener(HandleClick);
    }

    public void Bind(SeedEntry entry, int quantityInInventory)
    {
        boundEntry = entry;

        string displayName = entry.plantDefinition != null ? entry.plantDefinition.displayName : "—";
        seedNameLabel.text = quantityInInventory > 0
            ? $"{displayName} ×{quantityInInventory}"
            : displayName;

        if (entry.plantDefinition != null && entry.plantDefinition.spriteGraine != null)
            seedIcon.sprite = entry.plantDefinition.spriteGraine;
        else
            seedIcon.sprite = null;

        seedIcon.enabled = seedIcon.sprite != null;
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;

        float alpha = interactable ? 1f : 0.35f;
        Color iconColor = seedIcon.color;
        Color labelColor = seedNameLabel.color;
        iconColor.a = alpha;
        labelColor.a = alpha;
        seedIcon.color = iconColor;
        seedNameLabel.color = labelColor;
    }

    private void HandleClick()
    {
        OnSlotClicked?.Invoke(boundEntry);
    }
}
