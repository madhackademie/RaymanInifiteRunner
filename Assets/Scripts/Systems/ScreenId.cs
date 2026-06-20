/// <summary>
/// Identifiants uniques des écrans gérés par UIManager.
/// Ajouter une constante ici pour enregistrer un nouvel écran.
/// </summary>
public static class ScreenId
{
    public const string Inventory = "Inventory";
    public const string Shop = "Shop";

    /// <summary>Canaux de vente production (écoulement local — voisinage, bandoulière, vélo).</summary>
    public const string SaleChannels = "SaleChannels";

    /// <summary>
    /// Popups gameplay ferme (scène FirstLvl, hors prefab écran UIManager).
    /// Utilisé uniquement comme clé dans <see cref="UIManager"/> runtimePopupBindings.
    /// </summary>
    public const string FirstLvlFarm = "FirstLvlFarm";

    // Futurs écrans :
    // public const string Market   = "Market";
    // public const string Settings = "Settings";
    // public const string Talents  = "Talents";
}
