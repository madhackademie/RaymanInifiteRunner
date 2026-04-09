using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject acting as a registry for all ItemDefinitions in the game.
/// Assign all items in the Inspector; query them at runtime via GetById.
/// </summary>
[CreateAssetMenu(menuName = "Game/Inventory/Item Database", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDefinition> items = new();

    private Dictionary<string, ItemDefinition> lookupCache;

    /// <summary>Read-only list of all registered items.</summary>
    public IReadOnlyList<ItemDefinition> Items => items;

    /// <summary>Returns the ItemDefinition matching the given id, or null if not found.</summary>
    public ItemDefinition GetById(string itemId)
    {
        BuildCacheIfNeeded();

        if (string.IsNullOrEmpty(itemId))
            return null;

        lookupCache.TryGetValue(itemId, out ItemDefinition result);
        return result;
    }

    // ── Cache management ──────────────────────────────────────────────────────

    private void BuildCacheIfNeeded()
    {
        if (lookupCache != null)
            return;

        lookupCache = new Dictionary<string, ItemDefinition>(items.Count);

        foreach (ItemDefinition item in items)
        {
            if (item == null || string.IsNullOrEmpty(item.ItemId))
                continue;

            if (!lookupCache.TryAdd(item.ItemId, item))
                Debug.LogWarning($"[ItemDatabase] Duplicate itemId '{item.ItemId}' — entry ignored.", this);
        }
    }

    private void OnValidate()
    {
        // Invalidate cache when the list is changed in the Inspector.
        lookupCache = null;
    }
}
