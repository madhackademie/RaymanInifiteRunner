using UnityEngine;

/// <summary>
/// ScriptableObject describing a single item type in the game.
/// </summary>
[CreateAssetMenu(menuName = "Game/Data/Inventaire/Item (définition)", fileName = "Item_")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;

    [Header("Visuals")]
    [SerializeField] private Sprite icon;

    [Header("Stacking")]
    [SerializeField] private int maxStack = 99;

    /// <summary>Unique identifier used to look up this item in the database.</summary>
    public string ItemId => itemId;

    /// <summary>Human-readable name shown in the UI.</summary>
    public string DisplayName => displayName;

    /// <summary>Icon displayed in inventory slots.</summary>
    public Sprite Icon => icon;

    /// <summary>Maximum number of items that can share a single slot.</summary>
    public int MaxStack => maxStack;
}
