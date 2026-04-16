using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Point d'entrée unique du jeu. Vit dans la scène Bootstrap (index 0 en Build Settings).
/// Charge le shell UI puis FirstLvl en pilotant l'écran de chargement.
/// UIManager.Instance est garanti disponible avant les Awake/Start de FirstLvl.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class GameBootstrap : MonoBehaviour
{
    private const string SceneNavigationHUD = "NavigationHUD";
    private const string SceneFirstLvl      = "FirstLvl";

    [SerializeField] private LoadingScreen loadingScreen;

    private async void Awake()
    {
        loadingScreen.SetProgress(0f);

        // 1. Shell UI : NavigationHUD + UIManager (0 % → 50 %)
        await LoadWithProgress(SceneNavigationHUD, 0f, 0.5f);
        NavigationHUD.ShowExitOnly();

        // 2. Scène de jeu principale (50 % → 100 %)
        await LoadWithProgress(SceneFirstLvl, 0.5f, 1f);

        // 3. Masque l'écran de chargement avec un fade-out.
        await loadingScreen.Hide();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Charge une scène additivement en mettant à jour la barre de progression
    /// entre startProgress et endProgress.
    /// </summary>
    private async Awaitable LoadWithProgress(string sceneName, float startProgress, float endProgress)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;

        // AsyncOperation.progress plafonne à 0.9 tant que allowSceneActivation est false.
        while (op.progress < 0.9f)
        {
            float t = op.progress / 0.9f;
            loadingScreen.SetProgress(Mathf.Lerp(startProgress, endProgress, t));
            await Awaitable.NextFrameAsync();
        }

        loadingScreen.SetProgress(endProgress);
        op.allowSceneActivation = true;

        while (!op.isDone)
            await Awaitable.NextFrameAsync();
    }
}
