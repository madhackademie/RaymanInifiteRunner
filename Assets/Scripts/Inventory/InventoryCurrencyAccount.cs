using UnityEngine;

/// <summary>
/// Opérations de solde pour les items « monnaie » (<see cref="ItemInventoryBehavior.Currency"/>).
/// Réutilisable pour d’autres devises ou ressources échangeables suivant le même modèle stackable.
/// </summary>
public static class InventoryCurrencyAccount
{
    /// <summary>Solde total pour cette définition (somme des piles).</summary>
    public static int GetBalance(PlayerInventory inventory, ItemDefinition currency)
    {
        if (inventory == null || currency == null)
            return 0;

        return inventory.Count(currency);
    }

    public static bool HasSufficientFunds(PlayerInventory inventory, ItemDefinition currency, int amount)
    {
        if (amount <= 0)
            return true;

        return GetBalance(inventory, currency) >= amount;
    }

    /// <summary>Ajoute des unités de monnaie (récompense, vente, remboursement).</summary>
    public static bool TryCredit(PlayerInventory inventory, ItemDefinition currency, int amount)
    {
        if (inventory == null || currency == null || amount < 0)
            return false;

        if (amount == 0)
            return true;

        InventoryResult r = inventory.TryAdd(currency, amount);
        return r == InventoryResult.Success || r == InventoryResult.Partial;
    }

    /// <summary>Retire des unités de monnaie (coût, achat côté vendeur).</summary>
    public static bool TryDebit(PlayerInventory inventory, ItemDefinition currency, int amount)
    {
        if (inventory == null || currency == null || amount < 0)
            return false;

        if (amount == 0)
            return true;

        return inventory.TryRemove(currency, amount) == InventoryResult.Success;
    }

    /// <summary>
    /// Achat atomique : vérifie la place pour la marchandise, débite la monnaie, puis <see cref="PlayerInventory.TryAdd"/>.
    /// Rembourse si l’ajout échoue après débit.
    /// </summary>
    public static bool TryPurchase(
        PlayerInventory inventory,
        ItemDefinition currency,
        int totalPrice,
        ItemDefinition purchasedItem,
        int quantity,
        out InventoryResult addResult)
    {
        addResult = InventoryResult.InvalidItem;

        if (inventory == null || purchasedItem == null || quantity <= 0)
            return false;

        if (totalPrice < 0)
            totalPrice = 0;

        if (!inventory.CanFitQuantity(purchasedItem, quantity))
        {
            addResult = InventoryResult.Full;
            return false;
        }

        if (totalPrice > 0)
        {
            if (currency == null)
                return false;

            if (!HasSufficientFunds(inventory, currency, totalPrice))
                return false;

            if (!TryDebit(inventory, currency, totalPrice))
                return false;
        }

        addResult = inventory.TryAdd(purchasedItem, quantity);

        if (addResult == InventoryResult.Success)
            return true;

        if (totalPrice > 0 && currency != null)
            TryCredit(inventory, currency, totalPrice);

        return false;
    }

    /// <summary>
    /// Vente atomique : retire l'item vendu puis crédite la monnaie.
    /// Rembourse l'item si le crédit échoue après retrait.
    /// </summary>
    public static bool TrySell(
        PlayerInventory inventory,
        ItemDefinition soldItem,
        int quantity,
        ItemDefinition currency,
        int totalGain,
        out InventoryResult removeResult)
    {
        removeResult = InventoryResult.InvalidItem;

        if (inventory == null || soldItem == null || quantity <= 0)
            return false;

        if (totalGain < 0)
            totalGain = 0;

        if (inventory.Count(soldItem) < quantity)
            return false;

        removeResult = inventory.TryRemove(soldItem, quantity);
        if (removeResult != InventoryResult.Success)
            return false;

        if (totalGain == 0)
            return true;

        if (currency == null)
        {
            inventory.TryAdd(soldItem, quantity);
            removeResult = InventoryResult.InvalidItem;
            return false;
        }

        InventoryResult creditResult = inventory.TryAdd(currency, totalGain);
        if (creditResult == InventoryResult.Success || creditResult == InventoryResult.Partial)
            return true;

        inventory.TryAdd(soldItem, quantity);
        removeResult = InventoryResult.Full;
        return false;
    }
}
