/// <summary>
/// Result of an inventory add or remove operation.
/// </summary>
public enum InventoryResult
{
    /// <summary>The full requested amount was processed successfully.</summary>
    Success,

    /// <summary>Only part of the requested amount could be processed.</summary>
    Partial,

    /// <summary>No space left in the inventory — nothing was added.</summary>
    Full,

    /// <summary>The item reference was null or not found in the database.</summary>
    InvalidItem,
}
