/// <summary>
/// Identifiants uniques des popups runtime.
/// Permet d'eviter les magic strings entre ecrans et gestionnaires.
/// </summary>
public static class PopupId
{
    public const string ShopItemPurchase = "shop.item.purchase";

    /// <summary>Feedback monnaie / inventaire plein / erreurs de coût (écran Shop).</summary>
    public const string ShopResourceFeedback = "shop.resource.feedback";

    /// <summary>Choix de graine sur cellule libre (scène FirstLvl / ferme).</summary>
    public const string FarmSeedSelection = "farm.seed.selection";

    /// <summary>Info plante + récolte / arrachage (scène FirstLvl / ferme).</summary>
    public const string FarmPlantHarvest = "farm.plant.harvest";

    /// <summary>Message court inventaire / contraintes sac (ex. inventaire plein à la récolte).</summary>
    public const string FarmInventoryFeedback = "farm.inventory.feedback";

    /// <summary>Toast succès récolte : icône item + quantité animée (scène FirstLvl / ferme).</summary>
    public const string FarmHarvestReward = "farm.harvest.reward";

    /// <summary>Popup quantité / confirmation vente canal (écran SaleChannels).</summary>
    public const string SaleChannelSell = "sale.channel.sell";
}
