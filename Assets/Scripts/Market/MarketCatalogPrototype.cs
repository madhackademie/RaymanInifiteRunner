using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prototype marché : charge un catalogue depuis un JSON dans Resources (pas de réseau).
/// Les lignes résolues réutilisent <see cref="InventorySlot"/> pour affichage via <see cref="InventorySlotUI"/>.
/// </summary>
public static class MarketCatalogPrototype
{
    /// <summary>Chemin sans extension pour Resources.Load&lt;TextAsset&gt;.</summary>
    public const string ResourcesRelativePath = "Market/market_catalog";

    /// <summary>Une offre affichable comme un slot inventaire + prix unitaire simulé.</summary>
    public sealed class ListingRow
    {
        public ListingRow(InventorySlot slot, int unitPrice)
        {
            Slot = slot;
            UnitPrice = unitPrice;
        }

        public InventorySlot Slot { get; }
        public int UnitPrice { get; }
    }

    [Serializable]
    private class CatalogFileDto
    {
        public MarketListingDto[] listings;
    }

    [Serializable]
    public class MarketListingDto
    {
        public string itemId;
        public int price;
        public int quantity = 1;
    }

    /// <summary>
    /// Charge et résout les entrées contre <paramref name="database"/>.
    /// Retourne false seulement si le fichier est absent ou illisible ; une liste vide est valide.
    /// </summary>
    public static bool TryLoad(ItemDatabase database, out List<ListingRow> rows, out string errorMessage)
    {
        rows = new List<ListingRow>();
        errorMessage = null;

        if (database == null)
        {
            errorMessage = "ItemDatabase null.";
            return false;
        }

        TextAsset asset = Resources.Load<TextAsset>(ResourcesRelativePath);
        if (asset == null)
        {
            errorMessage =
                $"JSON marché introuvable : Resources/{ResourcesRelativePath}.json — créez ce fichier pour le prototype.";
            return false;
        }

        CatalogFileDto file;
        try
        {
            file = JsonUtility.FromJson<CatalogFileDto>(asset.text);
        }
        catch (Exception ex)
        {
            errorMessage = $"JSON marché invalide : {ex.Message}";
            return false;
        }

        if (file?.listings == null || file.listings.Length == 0)
            return true;

        foreach (MarketListingDto dto in file.listings)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.itemId))
            {
                Debug.LogWarning("[MarketCatalogPrototype] Entrée ignorée : itemId vide.");
                continue;
            }

            ItemDefinition item = database.GetById(dto.itemId.Trim());
            if (item == null)
            {
                Debug.LogWarning($"[MarketCatalogPrototype] itemId inconnu dans ItemDatabase : '{dto.itemId}'.");
                continue;
            }

            int qty = Mathf.Max(1, dto.quantity);
            var slot = new InventorySlot();
            slot.Set(item, qty);
            rows.Add(new ListingRow(slot, Mathf.Max(0, dto.price)));
        }

        return true;
    }
}
