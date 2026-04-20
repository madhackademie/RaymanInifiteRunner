using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory. Lives in the NavigationHUD scene which is never unloaded.
/// Exposes add/remove operations and fires OnInventoryChanged whenever the state changes.
/// Automatically saves to disk on every mutation and loads on first Awake.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    /// <summary>Singleton instance. Resolved from the NavigationHUD scene, always available.</summary>
    public static PlayerInventory Instance { get; private set; }

    [SerializeField] private int slotCount = 20;

    [Tooltip("Database used to resolve item IDs during save/load.")]
    [SerializeField] private ItemDatabase itemDatabase;

    private readonly List<InventorySlot> slots = new();

    /// <summary>Fired after any successful mutation of the inventory.</summary>
    public event Action OnInventoryChanged;

    /// <summary>Read-only view of all inventory slots.</summary>
    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[PlayerInventory] Instance dupliquée détectée — une seule doit exister dans NavigationHUD.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitialiseSlots();
        LoadFromDisk();
    }

    private void OnApplicationQuit() => SaveToDisk();

    // ── Initialisation ────────────────────────────────────────────────────────

    private void InitialiseSlots()
    {
        slots.Clear();
        for (int i = 0; i < slotCount; i++)
            slots.Add(new InventorySlot());
    }

    // ── Persistence ───────────────────────────────────────────────────────────

    /// <summary>Persists the current inventory state to disk.</summary>
    public void SaveToDisk()
    {
        InventorySaveService.Save(slots);
    }

    /// <summary>Restores the inventory state from disk. Replaces all current slot data.</summary>
    public void LoadFromDisk()
    {
        if (itemDatabase == null)
        {
            Debug.LogWarning("[PlayerInventory] itemDatabase non assigné — sauvegarde désactivée.");
            return;
        }

        if (InventorySaveService.TryLoad(itemDatabase, slots, out int count))
            Debug.Log($"[PlayerInventory] {count} slot(s) restauré(s) depuis la sauvegarde.");
    }

    /// <summary>Clears all slots and deletes the save file from disk.</summary>
    public void ResetAndDeleteSave()
    {
        InitialiseSlots();
        InventorySaveService.Delete();
        OnInventoryChanged?.Invoke();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Tries to add the given quantity of an item to the inventory.
    /// Items are stacked into existing matching slots first, then into empty slots.
    /// </summary>
    /// <returns>
    /// <see cref="InventoryResult.Success"/>     — all added.<br/>
    /// <see cref="InventoryResult.Partial"/>     — partially added (inventory nearly full).<br/>
    /// <see cref="InventoryResult.Full"/>        — nothing could be added.<br/>
    /// <see cref="InventoryResult.InvalidItem"/> — item reference is null.
    /// </returns>
    public InventoryResult TryAdd(ItemDefinition item, int quantity)
    {
        if (item == null)
            return InventoryResult.InvalidItem;

        if (quantity <= 0)
            return InventoryResult.Success;

        int remaining = quantity;

        // Pass 1 — fill existing partial stacks
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty || slot.Item != item || slot.IsFull)
                continue;

            remaining -= slot.Add(remaining);

            if (remaining <= 0)
                break;
        }

        // Pass 2 — use empty slots
        if (remaining > 0)
        {
            foreach (InventorySlot slot in slots)
            {
                if (!slot.IsEmpty)
                    continue;

                slot.Set(item, 0);
                remaining -= slot.Add(remaining);

                if (remaining <= 0)
                    break;
            }
        }

        bool anythingAdded = remaining < quantity;

        if (anythingAdded)
        {
            OnInventoryChanged?.Invoke();
            SaveToDisk();
        }

        if (remaining >= quantity)
            return InventoryResult.Full;

        return remaining > 0 ? InventoryResult.Partial : InventoryResult.Success;
    }

    /// <summary>
    /// Tries to remove the given quantity of an item from the inventory.
    /// </summary>
    /// <returns>
    /// <see cref="InventoryResult.Success"/>     — all removed.<br/>
    /// <see cref="InventoryResult.Partial"/>     — partially removed (not enough stock).<br/>
    /// <see cref="InventoryResult.Full"/>        — none found to remove.<br/>
    /// <see cref="InventoryResult.InvalidItem"/> — item reference is null.
    /// </returns>
    public InventoryResult TryRemove(ItemDefinition item, int quantity)
    {
        if (item == null)
            return InventoryResult.InvalidItem;

        if (quantity <= 0)
            return InventoryResult.Success;

        int remaining = quantity;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty || slot.Item != item)
                continue;

            remaining -= slot.Remove(remaining);

            if (remaining <= 0)
                break;
        }

        bool anythingRemoved = remaining < quantity;

        if (anythingRemoved)
        {
            OnInventoryChanged?.Invoke();
            SaveToDisk();
        }

        if (remaining >= quantity)
            return InventoryResult.Full;

        return remaining > 0 ? InventoryResult.Partial : InventoryResult.Success;
    }

    /// <summary>Returns the total quantity of the given item across all slots.</summary>
    public int Count(ItemDefinition item)
    {
        if (item == null)
            return 0;

        int total = 0;
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty && slot.Item == item)
                total += slot.Quantity;
        }
        return total;
    }

    /// <summary>Returns true if the inventory can accept at least one unit of the given item.</summary>
    public bool HasSpaceFor(ItemDefinition item)
    {
        if (item == null)
            return false;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
                return true;

            if (slot.Item == item && !slot.IsFull)
                return true;
        }
        return false;
    }
}
