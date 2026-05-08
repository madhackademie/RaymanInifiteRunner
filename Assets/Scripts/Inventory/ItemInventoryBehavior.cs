/// <summary>
/// Rôle stocké dans l'inventaire : ressource classique ou unité de valeur échangeable (monnaie).
/// </summary>
public enum ItemInventoryBehavior
{
    /// <summary>Objets standards (graines, récoltes, etc.).</summary>
    Standard,

    /// <summary>
    /// Comptage comme solde (euros, etc.) : débit/crédit via
    /// <see cref="InventoryCurrencyAccount"/> pour le commerce.
    /// </summary>
    Currency
}
