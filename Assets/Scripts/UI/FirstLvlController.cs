using UnityEngine;

/// <summary>
/// Contrôleur de la scène FirstLvl.
/// Active le bouton de sortie du NavigationHUD et gère le retour vers HomeScene via SceneNavigator.
/// </summary>
public class FirstLvlController : MonoBehaviour
{
    private void Start()
    {
        NavigationHUD.ShowExitOnly();
        if (NavigationHUD.Instance != null)
            NavigationHUD.Instance.OnExitToHomeRequested += ReturnToHome;
    }

    private void OnDestroy()
    {
        if (NavigationHUD.Instance != null)
            NavigationHUD.Instance.OnExitToHomeRequested -= ReturnToHome;
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    /// <summary>Retourne en HomeScene via SceneNavigator (masque FirstLvl, affiche HomeScene).</summary>
    private async void ReturnToHome()
    {
        NavigationHUD.Hide();
        await SceneNavigator.Instance.ShowScene(SceneId.HomeScene);
        NavigationHUD.ShowNavBar();
    }
}
