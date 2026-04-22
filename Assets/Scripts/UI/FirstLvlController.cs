using UnityEngine;

/// <summary>
/// Contrôleur de la scène FirstLvl.
/// Gère le retour vers HomeScene via SceneNavigator.
/// </summary>
public class FirstLvlController : MonoBehaviour
{
    private void Start()
    {
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
        if (SceneNavigator.Instance == null)
            return;

        await SceneNavigator.Instance.ShowScene(SceneId.HomeScene);
    }
}
