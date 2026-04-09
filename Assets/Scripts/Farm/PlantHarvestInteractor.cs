using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attached to a plant GameObject. On click, checks whether the plant is mature,
/// resolves the harvest item via ItemDatabase, and adds it to the PlayerInventory.
/// Triggers InventoryFeedbackUI if the inventory is full.
/// </summary>
[RequireComponent(typeof(PlantGrow))]
[RequireComponent(typeof(Collider2D))]
public class PlantHarvestInteractor : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventoryFeedbackUI feedbackUI;

    [Header("Harvest")]
    [Tooltip("Overrides PlantDefinition.harvestItemId when non-empty.")]
    [SerializeField] private string harvestItemIdOverride;

    private PlantGrow plantGrow;
    private PlantDefinition plantDefinition;

    private void Awake()
    {
        plantGrow = GetComponent<PlantGrow>();
    }

    // PlantGrow does not expose the PlantDefinition directly — we retrieve it
    // via a sibling component that caches the reference after placement.
    // If a PlantDefinitionHolder is not present, harvestItemIdOverride must be set.

    private void OnMouseDown()
    {
        TryHarvest();
    }

    // ── Harvest logic ─────────────────────────────────────────────────────────

    /// <summary>
    /// Attempts to harvest the plant. Resolves the item, tries to add it to the
    /// inventory, and triggers feedback on failure.
    /// </summary>
    public void TryHarvest()
    {
        if (!IsMature())
        {
            Debug.Log($"[PlantHarvestInteractor] '{gameObject.name}' is not mature yet.");
            return;
        }

        string itemId = ResolveHarvestItemId();

        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning($"[PlantHarvestInteractor] No harvestItemId configured on '{gameObject.name}'.", this);
            return;
        }

        if (itemDatabase == null)
        {
            Debug.LogWarning("[PlantHarvestInteractor] No ItemDatabase assigned.", this);
            return;
        }

        ItemDefinition item = itemDatabase.GetById(itemId);

        if (item == null)
        {
            Debug.LogWarning($"[PlantHarvestInteractor] ItemId '{itemId}' not found in ItemDatabase.", this);
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogWarning("[PlantHarvestInteractor] No PlayerInventory assigned.", this);
            return;
        }

        InventoryResult result = playerInventory.TryAdd(item, 1);
        HandleResult(result, item);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private bool IsMature()
    {
        // Defer to the PlantDefinition's configured harvest stage if available.
        if (TryGetComponent(out PlantDefinitionHolder holder) && holder.Definition != null)
            return plantGrow.CurrentStage == holder.Definition.HarvestStage;

        // Fallback: accept Mature as the universal harvest stage.
        return plantGrow.CurrentStage == PlantGrow.GrowthStage.Mature;
    }

    private string ResolveHarvestItemId()
    {
        if (!string.IsNullOrEmpty(harvestItemIdOverride))
            return harvestItemIdOverride;

        if (TryGetComponent(out PlantDefinitionHolder holder) && holder.Definition != null)
            return holder.Definition.harvestItemId;

        return null;
    }

    private void HandleResult(InventoryResult result, ItemDefinition item)
    {
        switch (result)
        {
            case InventoryResult.Success:
            case InventoryResult.Partial:
                Debug.Log($"[PlantHarvestInteractor] Harvested '{item.DisplayName}' from '{gameObject.name}'. Result: {result}.");
                OnHarvestSuccess();
                break;

            case InventoryResult.Full:
                Debug.Log($"[PlantHarvestInteractor] Inventory full — could not harvest '{item.DisplayName}'.");
                feedbackUI?.ShowInventoryFull();
                break;

            case InventoryResult.InvalidItem:
                Debug.LogWarning($"[PlantHarvestInteractor] Invalid item resolved for '{gameObject.name}'.", this);
                break;
        }
    }

    /// <summary>
    /// Called when a harvest succeeds. Override or extend this placeholder to trigger
    /// animations, advance the plant stage, or reduce harvest count.
    /// </summary>
    protected virtual void OnHarvestSuccess()
    {
        // Placeholder — advance to next stage after harvest, mirroring PlantGrow's pipeline.
        // Extend this in a subclass or wire an event when the design is finalised.
    }
}
