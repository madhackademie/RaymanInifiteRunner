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

    [Tooltip("Court texte de définition affiché dans la popup détail inventaire.")]
    [SerializeField] [TextArea(2, 4)] private string description;

    [Header("Comportement")]
    [SerializeField] private ItemInventoryBehavior inventoryBehavior = ItemInventoryBehavior.Standard;

    [Header("Filtres inventaire")]
    [SerializeField] private ItemCategory itemCategory = ItemCategory.Material;
    [SerializeField] private ItemGameScope gameScope = ItemGameScope.Farm;

    [Header("Visuals")]
    [SerializeField] private Sprite icon;

    [Header("Stacking")]
    [SerializeField] private int maxStack = 99;

    /// <summary>Unique identifier used to look up this item in the database.</summary>
    public string ItemId => itemId;

    /// <summary>Human-readable name shown in the UI.</summary>
    public string DisplayName => displayName;

    /// <summary>Définition courte pour les popups inventaire / détail item.</summary>
    public string Description => description ?? string.Empty;

    /// <summary>Rôle inventaire (standard ou monnaie).</summary>
    public ItemInventoryBehavior InventoryBehavior => inventoryBehavior;

    /// <summary>Catégorie pour onglets inventaire (Graines, Récoltes…).</summary>
    public ItemCategory Category => itemCategory;

    /// <summary>Univers gameplay où l'item est défini.</summary>
    public ItemGameScope GameScope => gameScope;

    /// <summary>Icon displayed in inventory slots.</summary>
    public Sprite Icon => icon;

    /// <summary>Maximum number of items that can share a single slot.</summary>
    public int MaxStack => maxStack;
}
