using UnityEngine;

/// <summary>
/// Contrôleur de la scène FirstLvl.
/// Active le bouton de sortie du NavigationHUD et gère le retour vers HomeScene via SceneNavigator.
/// </summary>
public class FirstLvlController : MonoBehaviour
{
    private async void Start()
    {
        // Garantit que le shell est chargé même si la scène est ouverte directement depuis l'éditeur.
        await UIManager.EnsureShellLoaded();

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

    /// <summary>Retourne en HomeScene via SceneNavigator (décharge FirstLvl, charge HomeScene).</summary>
    private async void ReturnToHome()
    {
        NavigationHUD.Hide();
        await SceneNavigator.Instance.GoTo(SceneId.HomeScene);
        NavigationHUD.ShowNavBar();
    }
}
