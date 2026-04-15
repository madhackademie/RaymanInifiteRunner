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
        // If playerInventory was set via Inspector (e.g. in the game scene), initialise immediately.
        if (playerInventory != null)
            Initialise();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= Refresh;
    }

    /// <summary>
    /// Injects a <see cref="PlayerInventory"/> at runtime (e.g. from <see cref="InventorySceneController"/>
    /// when the inventory is opened as an additive scene and the singleton lives in DontDestroyOnLoad).
    /// </summary>
    public void Bind(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

        // Unsubscribe from any previous inventory.
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= Refresh;

        playerInventory = inventory;
        Initialise();
    }

    private void Initialise()
    {
        BuildSlots();
        playerInventory.OnInventoryChanged += Refresh;
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
