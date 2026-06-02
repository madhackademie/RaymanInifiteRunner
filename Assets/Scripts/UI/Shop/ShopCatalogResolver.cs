using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Résout les offres affichées par le shop depuis sa source de données :
/// un <see cref="ShopCatalogDefinition"/> (prioritaire) sinon le prototype JSON marché.
/// Logique de résolution extraite de RuntimeShopScreen.
/// </summary>
public static class ShopCatalogResolver
{
    /// <summary>
    /// Construit les offres et, le cas échéant, les définitions shop associées (même index).
    /// <paramref name="definitions"/> est null quand la source est le JSON marché (pas de SO par ligne).
    /// </summary>
    public static bool TryResolve(
        ShopCatalogDefinition catalog,
        ItemDatabase itemDatabase,
        out List<MarketCatalogPrototype.ListingRow> listings,
        out List<ShopItemDefinition> definitions,
        out string errorMessage)
    {
        if (catalog != null)
            return TryBuildFromScriptableObject(catalog, out listings, out definitions, out errorMessage);

        definitions = null;
        return MarketCatalogPrototype.TryLoad(itemDatabase, out listings, out errorMessage);
    }

    private static bool TryBuildFromScriptableObject(
        ShopCatalogDefinition catalog,
        out List<MarketCatalogPrototype.ListingRow> listings,
        out List<ShopItemDefinition> definitions,
        out string errorMessage)
    {
        listings = new List<MarketCatalogPrototype.ListingRow>();
        definitions = new List<ShopItemDefinition>();
        errorMessage = null;

        if (catalog.Items == null || catalog.Items.Count == 0)
            return true;

        for (int i = 0; i < catalog.Items.Count; i++)
        {
            ShopItemDefinition entry = catalog.Items[i];
            if (entry == null)
            {
                Debug.LogWarning("[ShopCatalogResolver] ShopCatalogDefinition: entree null ignoree.");
                continue;
            }

            ItemDefinition item = entry.ItemDefinition;
            if (item == null)
            {
                Debug.LogWarning("[ShopCatalogResolver] ShopCatalogDefinition: ItemDefinition manquant, entree ignoree.");
                continue;
            }

            var slot = new InventorySlot();
            slot.Set(item, entry.ListingQuantity);

            listings.Add(new MarketCatalogPrototype.ListingRow(slot, entry.UnitPrice));
            definitions.Add(entry);
        }

        return true;
    }
}
