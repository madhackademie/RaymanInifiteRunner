/// <summary>
/// Identifiants uniques des popups runtime.
/// Permet d'eviter les magic strings entre ecrans et gestionnaires.
/// </summary>
public static class PopupId
{
    public const string ShopItemPurchase = "shop.item.purchase";

    /// <summary>Choix de graine sur cellule libre (scène FirstLvl / ferme).</summary>
    public const string FarmSeedSelection = "farm.seed.selection";
}
