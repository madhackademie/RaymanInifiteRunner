using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject acting as a registry for all ItemDefinitions in the game.
/// Assign all items in the Inspector; query them at runtime via GetById.
/// </summary>
[CreateAssetMenu(menuName = "Game/Data/Inventaire/Base d'items (ItemDatabase)", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDefinition> items = new();

    [Header("Économie")]
    [Tooltip("Monnaie utilisée pour les prix du shop (débit avant TryAdd sur l’objet acheté).")]
    [SerializeField] private ItemDefinition primaryCurrency;

    private Dictionary<string, ItemDefinition> lookupCache;

    /// <summary>Monnaie principale (ex. euros), ou null si non configurée.</summary>
    public ItemDefinition PrimaryCurrency => primaryCurrency;

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

        if (primaryCurrency != null &&
            primaryCurrency.InventoryBehavior != ItemInventoryBehavior.Currency)
        {
            Debug.LogWarning(
                $"[ItemDatabase] « {primaryCurrency.name} » est défini comme monnaie mais " +
                $"InventoryBehavior != Currency — vérifiez l’asset.",
                this);
        }
    }
}
