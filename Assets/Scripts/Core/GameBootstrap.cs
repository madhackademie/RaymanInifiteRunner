using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Point d'entrée unique du jeu. Vit dans la scène Bootstrap (index 0 en Build Settings).
/// Charge additivement le shell <c>NavigationHUD</c>, la scène de contenu <c>HomeScene</c>,
/// puis la scène <c>Inventaire</c> (chargée tôt pour éviter un hitch au premier onglet).
/// Masque le HUD pendant le boot, désactive les racines de la scène inventaire jusqu'à navigation,
/// et délègue la scène visible initiale à <see cref="SceneNavigator.SetInitialScene"/>.
/// </summary>
[DefaultExecutionOrder(-1000)]
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private LoadingScreen loadingScreen;

    [Tooltip("Durée minimale d'affichage de l'écran de chargement (secondes).")]
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

            // 1. Shell persistant (0 % → 25 %)
            await LoadWithProgress(SceneId.NavigationHUD, 0f, 0.25f);

            // 2. Scène de contenu principale (25 % → 60 %)
            await LoadWithProgress(SceneId.HomeScene, 0.25f, 0.6f);

            // 3. Scène template UI inventaire — chargée tôt pour migration vers UIManager.
            await LoadWithProgress(SceneId.Inventaire, 0.6f, 1f);
            SetSceneRootsActive(SceneId.Inventaire, false);

            // 4. Déclare HomeScene comme scène visible initiale.
            SceneNavigator.Instance?.SetInitialScene(SceneId.HomeScene);

            // 5. Temps d'affichage minimum.
            float elapsed = Time.realtimeSinceStartup - startTime;
            float remaining = minimumDisplaySeconds - elapsed;
            if (remaining > 0f)
                await Awaitable.WaitForSecondsAsync(remaining);

            // 6. Fade-out de l'écran de chargement.
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
            Debug.LogError($"[GameBootstrap] Impossible de charger '{sceneName}'. Vérifie les Build Settings.");
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

    /// <summary>Active ou désactive tous les GameObjects racines d'une scène chargée.</summary>
    private static void SetSceneRootsActive(string sceneName, bool active)
    {
        UnityEngine.SceneManagement.Scene scene =
            SceneManager.GetSceneByName(sceneName);

        if (!scene.IsValid() || !scene.isLoaded)
            return;

        foreach (GameObject root in scene.GetRootGameObjects())
            root.SetActive(active);
    }
}
