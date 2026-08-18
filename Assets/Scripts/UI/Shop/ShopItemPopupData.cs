using UnityEngine;

/// <summary>
/// Données minimales d'affichage/achat pour la popup item du shop.
/// DTO runtime (pas de ScriptableObject) : construit depuis le catalogue ou l'inventaire.
/// </summary>
public sealed class ShopItemPopupData
{
    public string ItemId { get; }
    public string DisplayName { get; }
    public string RarityLabel { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    public int UnitPrice { get; }
    public int MinQuantity { get; }
    public int MaxQuantity { get; }

    public ShopItemPopupData(
        string itemId,
        string displayName,
        string rarityLabel,
        string description,
        Sprite icon,
        int unitPrice,
        int minQuantity = 1,
        int maxQuantity = 99)
    {
        ItemId = itemId ?? string.Empty;
        DisplayName = displayName ?? string.Empty;
        RarityLabel = rarityLabel ?? string.Empty;
        Description = description ?? string.Empty;
        Icon = icon;
        UnitPrice = Mathf.Max(0, unitPrice);
        MinQuantity = Mathf.Max(1, minQuantity);
        MaxQuantity = Mathf.Max(MinQuantity, maxQuantity);
    }
}

/// <summary>
/// Sens de la transaction affichée dans la popup item (achat, vente canal, drop inventaire).
/// </summary>
public enum ShopItemPopupFlowMode
{
    Purchase,
    Sell,
    Drop,
    /// <summary>Confirmation lancement recherche déblocage canal de vente.</summary>
    Research
}
