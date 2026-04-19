using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère les transitions entre scènes de contenu (Layer 0).
/// Le shell NavigationHUD n'est jamais déchargé.
/// Une seule scène de contenu est active à la fois.
/// </summary>
public class SceneNavigator : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────

    public static SceneNavigator Instance { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>Déclenché juste avant le chargement d'une nouvelle scène.</summary>
    public event Action<string> OnBeforeSceneLoad;

    /// <summary>Déclenché une fois la nouvelle scène active et l'ancienne déchargée.</summary>
    public event Action<string> OnAfterSceneLoad;

    // ── State ─────────────────────────────────────────────────────────────────

    /// <summary>Nom de la scène de contenu actuellement chargée.</summary>
    public string CurrentScene { get; private set; }

    /// <summary>True si une transition est en cours.</summary>
    public bool IsTransitioning { get; private set; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Charge une scène de contenu en Additive et décharge la précédente.
    /// Sans effet si la scène demandée est déjà active ou si une transition est en cours.
    /// </summary>
    public async Awaitable GoTo(string sceneName)
    {
        if (IsTransitioning)
        {
            Debug.LogWarning($"[SceneNavigator] Transition déjà en cours, GoTo('{sceneName}') ignoré.");
            return;
        }

        if (CurrentScene == sceneName)
            return;

        IsTransitioning = true;
        OnBeforeSceneLoad?.Invoke(sceneName);

        string previousScene = CurrentScene;

        // 1. Charge la nouvelle scène.
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (load == null)
        {
            Debug.LogError($"[SceneNavigator] Impossible de charger '{sceneName}'. Vérifie les Build Settings.");
            IsTransitioning = false;
            return;
        }

        while (!load.isDone)
            await Awaitable.NextFrameAsync();

        CurrentScene = sceneName;

        // 2. Décharge l'ancienne scène uniquement si elle est encore en mémoire.
        if (!string.IsNullOrEmpty(previousScene))
        {
            Scene previous = SceneManager.GetSceneByName(previousScene);
            if (previous.IsValid() && previous.isLoaded)
            {
                AsyncOperation unload = SceneManager.UnloadSceneAsync(previous);
                if (unload != null)
                    while (!unload.isDone)
                        await Awaitable.NextFrameAsync();
            }
        }

        IsTransitioning = false;
        OnAfterSceneLoad?.Invoke(sceneName);
    }

    /// <summary>
    /// Indique à SceneNavigator quelle scène est déjà chargée (ex: HomeScene chargée par Bootstrap).
    /// À appeler une seule fois depuis GameBootstrap après le chargement initial.
    /// </summary>
    public void SetInitialScene(string sceneName)
    {
        CurrentScene = sceneName;
    }
}
