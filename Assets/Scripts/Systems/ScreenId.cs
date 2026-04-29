/// <summary>
/// Identifiants uniques des écrans gérés par UIManager.
/// Ajouter une constante ici pour enregistrer un nouvel écran.
/// </summary>
public static class ScreenId
{
    public const string Inventory = "Inventory";
    public const string Shop = "Shop";

    // Futurs écrans :
    // public const string Market   = "Market";
    // public const string Settings = "Settings";
    // public const string Talents  = "Talents";
}

/// <summary>
/// Identifiants des scènes du jeu utilisés lors des transitions.
/// Chaque constante doit correspondre exactement au nom de fichier .unity (sans extension).
/// </summary>
public static class SceneId
{
    public const string Bootstrap     = "Bootstrap";
    public const string NavigationHUD = "NavigationHUD";

    /// <summary>Écran d'accueil principal : sélection du mode de jeu, zones de farm, etc.</summary>
    public const string HomeScene     = "HomeScene";

    /// <summary>Carte du monde (à venir) : navigation spatiale entre les zones.</summary>
    public const string Map           = "Map";

    public const string FirstLvl      = "FirstLvl";
}
