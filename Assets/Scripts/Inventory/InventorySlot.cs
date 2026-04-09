using System;
using UnityEngine;

/// <summary>
/// Represents a single inventory slot holding an item and a quantity.
/// </summary>
[Serializable]
public class InventorySlot
{
    [SerializeField] private ItemDefinition item;
    [SerializeField] private int quantity;

    /// <summary>The item stored in this slot, or null if the slot is empty.</summary>
    public ItemDefinition Item => item;

    /// <summary>Current quantity of items in this slot.</summary>
    public int Quantity => quantity;

    /// <summary>True when the slot holds no item.</summary>
    public bool IsEmpty => item == null || quantity <= 0;

    /// <summary>True when the slot is at its maximum stack capacity.</summary>
    public bool IsFull => item != null && quantity >= item.MaxStack;

    /// <summary>Remaining space available in this slot.</summary>
    public int RemainingSpace => item == null ? 0 : item.MaxStack - quantity;

    /// <summary>Sets this slot to a new item and quantity, replacing previous contents.</summary>
    public void Set(ItemDefinition newItem, int newQuantity)
    {
        item     = newItem;
        quantity = Mathf.Clamp(newQuantity, 0, newItem != null ? newItem.MaxStack : 0);
    }

    /// <summary>Adds a quantity to this slot. Returns the amount actually added.</summary>
    public int Add(int amount)
    {
        if (item == null || amount <= 0)
            return 0;

        int canAdd   = Mathf.Min(amount, RemainingSpace);
        quantity    += canAdd;
        return canAdd;
    }

    /// <summary>Removes a quantity from this slot. Returns the amount actually removed.</summary>
    public int Remove(int amount)
    {
        if (IsEmpty || amount <= 0)
            return 0;

        int canRemove = Mathf.Min(amount, quantity);
        quantity     -= canRemove;

        if (quantity <= 0)
            Clear();

        return canRemove;
    }

    /// <summary>Clears the slot, making it empty.</summary>
    public void Clear()
    {
        item     = null;
        quantity = 0;
    }
}
