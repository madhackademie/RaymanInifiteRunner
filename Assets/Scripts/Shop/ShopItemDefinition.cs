using UnityEngine;

/// <summary>
/// Definition d'une offre item pour le shop runtime.
/// Lie un ItemDefinition et les donnees d'affichage/achat.
/// </summary>
[CreateAssetMenu(menuName = "Game/Data/Shop/Shop Item (definition)", fileName = "ShopItem_")]
public sealed class ShopItemDefinition : ScriptableObject
{
    [Header("Item")]
    [SerializeField] private ItemDefinition itemDefinition;

    [Header("Shop Display")]
    [SerializeField] private string rarityLabel;
    [TextArea(2, 6)]
    [SerializeField] private string description;

    [Header("Pricing")]
    [SerializeField] private int unitPrice = 10;

    [Header("Quantities")]
    [Tooltip("Quantite affichee dans la grille du shop.")]
    [SerializeField] private int listingQuantity = 1;
    [Tooltip("Quantite minimum achetable en une confirmation.")]
    [SerializeField] private int minPurchaseQuantity = 1;
    [Tooltip("Quantite maximum achetable en une confirmation.")]
    [SerializeField] private int maxPurchaseQuantity = 99;

    public ItemDefinition ItemDefinition => itemDefinition;
    public string RarityLabel => rarityLabel ?? string.Empty;
    public string Description => description ?? string.Empty;
    public int UnitPrice => Mathf.Max(0, unitPrice);
    public int ListingQuantity => Mathf.Max(1, listingQuantity);
    public int MinPurchaseQuantity => Mathf.Max(1, minPurchaseQuantity);
    public int MaxPurchaseQuantity => Mathf.Max(MinPurchaseQuantity, maxPurchaseQuantity);
}
