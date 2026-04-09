using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the inventory panel: spawns InventorySlotUI instances and keeps them
/// in sync with the PlayerInventory via the OnInventoryChanged event.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsContainer;

    private readonly List<InventorySlotUI> spawnedSlots = new();

    private void Start()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("[InventoryUI] No PlayerInventory assigned.", this);
            return;
        }

        BuildSlots();
        playerInventory.OnInventoryChanged += Refresh;
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= Refresh;
    }

    // ── Slot management ───────────────────────────────────────────────────────

    private void BuildSlots()
    {
        foreach (InventorySlotUI slot in spawnedSlots)
            Destroy(slot.gameObject);

        spawnedSlots.Clear();

        foreach (InventorySlot _ in playerInventory.Slots)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, slotsContainer);
            spawnedSlots.Add(slotUI);
        }

        Refresh();
    }

    /// <summary>Repopulates all slot UIs from the current inventory state.</summary>
    public void Refresh()
    {
        IReadOnlyList<InventorySlot> slots = playerInventory.Slots;

        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            InventorySlot data = i < slots.Count ? slots[i] : null;
            spawnedSlots[i].Refresh(data);
        }
    }
}
