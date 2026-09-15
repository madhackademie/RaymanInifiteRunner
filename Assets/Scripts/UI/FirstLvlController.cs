using UnityEngine;

/// <summary>
/// Contrôleur de la scène FirstLvl (aventure).
/// Force le HUD en mode croix (pas de barre d'onglets) et gère le retour Home.
/// </summary>
public class FirstLvlController : MonoBehaviour
{
    private bool hudBound;

    private void OnEnable()
    {
        SceneNavigator.OnNavigatorAvailable += HandleNavigatorAvailable;
        TryEnterAdventureHud();
    }

    private void Start()
    {
        TryEnterAdventureHud();
    }

    private void OnDisable()
    {
        SceneNavigator.OnNavigatorAvailable -= HandleNavigatorAvailable;
        UnbindHud();
    }

    private void HandleNavigatorAvailable(SceneNavigator _)
    {
        TryEnterAdventureHud();
    }

    /// <summary>Masque la nav, active la croix, écoute le clic sortie.</summary>
    private void TryEnterAdventureHud()
    {
        NavigationHUD hud = NavigationHUD.Instance;
        if (hud == null)
            return;

        if (!hudBound)
        {
            hud.OnExitToHomeRequested += ReturnToHome;
            hudBound = true;
        }

        hud.EnterAdventureMode();
    }

    private void UnbindHud()
    {
        if (!hudBound)
            return;

        if (NavigationHUD.Instance != null)
            NavigationHUD.Instance.OnExitToHomeRequested -= ReturnToHome;

        hudBound = false;
    }

    /// <summary>Retourne en HomeScene via SceneNavigator (masque FirstLvl, affiche HomeScene).</summary>
    private async void ReturnToHome()
    {
        if (SceneNavigator.Instance == null)
            return;

        await SceneNavigator.Instance.ShowScene(SceneId.HomeScene);
    }
}
