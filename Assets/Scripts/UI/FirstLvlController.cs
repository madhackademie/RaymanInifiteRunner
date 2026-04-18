using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contrôleur de la scène FirstLvl.
/// Active le bouton de sortie du NavigationHUD et gère le retour vers HomeScene.
/// </summary>
public class FirstLvlController : MonoBehaviour
{
    private async void Start()
    {
        // Garantit que le shell est chargé même si la scène est ouverte directement depuis l'éditeur.
        await UIManager.EnsureShellLoaded();

        NavigationHUD.ShowExitOnly();

        // Abonne la croix au retour Home.
        if (NavigationHUD.Instance != null)
            NavigationHUD.Instance.OnExitToHomeRequested += ReturnToHome;
    }

    private void OnDestroy()
    {
        if (NavigationHUD.Instance != null)
            NavigationHUD.Instance.OnExitToHomeRequested -= ReturnToHome;
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    /// <summary>Décharge FirstLvl et charge HomeScene en additif.</summary>
    private async void ReturnToHome()
    {
        NavigationHUD.Hide();

        AsyncOperation load = SceneManager.LoadSceneAsync(SceneId.HomeScene, LoadSceneMode.Additive);
        if (load == null)
        {
            Debug.LogError("[FirstLvlController] Impossible de charger HomeScene. Vérifie les Build Settings.");
            return;
        }

        while (!load.isDone)
            await Awaitable.NextFrameAsync();

        await SceneManager.UnloadSceneAsync(SceneId.FirstLvl);
    }
}
