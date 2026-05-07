using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Catalogue ScriptableObject du shop.
/// Permet d'alimenter RuntimeShopScreen sans JSON prototype.
/// </summary>
[CreateAssetMenu(menuName = "Game/Data/Shop/Shop Catalog", fileName = "ShopCatalog_")]
public sealed class ShopCatalogDefinition : ScriptableObject
{
    [SerializeField] private List<ShopItemDefinition> items = new();

    public IReadOnlyList<ShopItemDefinition> Items => items;
}
