using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Point d'entrée unique du jeu. Vit dans la scène Bootstrap (index 0 en Build Settings).
/// Charge le shell UI puis HomeScene en pilotant l'écran de chargement.
/// UIManager.Instance est garanti disponible avant les Awake/Start de HomeScene.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private LoadingScreen loadingScreen;

    [Tooltip("Durée minimale d'affichage de l'écran de chargement (secondes). Utile en éditeur où le chargement est quasi-instantané.")]
    [SerializeField] private float minimumDisplaySeconds = 2f;

    private async void Awake()
    {
        try
        {
            if (loadingScreen == null)
            {
                Debug.LogError("[GameBootstrap] loadingScreen est null — vérifie la référence dans l'Inspector.");
                return;
            }

            loadingScreen.SetProgress(0f);

            float startTime = Time.realtimeSinceStartup;

            // 1. Shell UI : NavigationHUD + UIManager + SceneNavigator (0 % → 50 %)
            await LoadWithProgress(SceneId.NavigationHUD, 0f, 0.5f);
            NavigationHUD.Hide();

            // 2. Scène de contenu initiale : HomeScene (50 % → 100 %)
            await LoadWithProgress(SceneId.HomeScene, 0.5f, 1f);
            SceneNavigator.Instance?.SetInitialScene(SceneId.HomeScene);

            // 3. Temps d'affichage minimum garanti.
            float elapsed = Time.realtimeSinceStartup - startTime;
            float remaining = minimumDisplaySeconds - elapsed;
            if (remaining > 0f)
                await Awaitable.WaitForSecondsAsync(remaining);

            // 4. Fade-out.
            await loadingScreen.Hide();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameBootstrap] Erreur pendant le chargement : {e.Message}\n{e.StackTrace}");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Charge une scène additivement en mettant à jour la barre de progression
    /// entre startProgress et endProgress.
    /// </summary>
    private async Awaitable LoadWithProgress(string sceneName, float startProgress, float endProgress)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (op == null)
        {
            Debug.LogError($"[GameBootstrap] Impossible de charger '{sceneName}'. Vérifie qu'elle est dans les Build Settings.");
            return;
        }

        op.allowSceneActivation = false;

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
