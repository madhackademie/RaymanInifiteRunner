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

    private const float DisabledAlpha = 0.35f;

    private SeedEntry boundEntry;
    private bool fitsGrid = true;
    private bool hasStock = true;

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

    /// <summary>
    /// Configure le slot. Le bouton reste cliquable dès qu'il y a du stock, pour que le feedback PA
    /// fonctionne aussi sur les graines grisées (ne rentre pas dans la grille, PA insuffisants).
    /// </summary>
    public void ConfigureSlot(bool fitsGrid, bool hasStock, bool canAffordPa)
    {
        this.fitsGrid = fitsGrid;
        this.hasStock = hasStock;
        button.interactable = hasStock;

        float alpha = (!hasStock || !fitsGrid || !canAffordPa) ? DisabledAlpha : 1f;
        Color iconColor = seedIcon.color;
        Color labelColor = seedNameLabel.color;
        iconColor.a = alpha;
        labelColor.a = alpha;
        seedIcon.color = iconColor;
        seedNameLabel.color = labelColor;
    }

    private void HandleClick()
    {
        if (!hasStock)
            return;

        if (!CanAffordAction(ActionPointActionId.PlantSeed))
        {
            PlayPaRefuseFeedback(ActionPointActionId.PlantSeed);
            return;
        }

        if (!fitsGrid)
            return;

        OnSlotClicked?.Invoke(boundEntry);
    }

    private static bool CanAffordAction(string actionId)
    {
        ActionPointService service = ActionPointService.Instance;
        return service == null || service.CanAfford(service.GetCostForAction(actionId));
    }

    // Déclenche le feedback PA (le throttle est géré dans le service) sans effet gameplay.
    private static void PlayPaRefuseFeedback(string actionId)
    {
        if (ActionPointService.Instance != null && !CanAffordAction(actionId))
            ActionPointService.Instance.TryConsume(actionId, out _);
    }
}
