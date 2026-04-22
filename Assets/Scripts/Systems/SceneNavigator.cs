using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère la visibilité des scènes de contenu via SetActive sur leurs racines.
/// Les scènes "eager" sont toutes chargées au démarrage par GameBootstrap.
/// Les scènes "lazy" sont chargées à la première demande puis conservées en mémoire.
/// Le shell NavigationHUD n'est jamais touché.
/// </summary>
public class SceneNavigator : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────

    public static SceneNavigator Instance { get; private set; }

    /// <summary>Émis quand l'instance singleton est disponible.</summary>
    public static event Action<SceneNavigator> OnNavigatorAvailable;

    /// <summary>Émis quand l'instance singleton active est détruite.</summary>
    public static event Action OnNavigatorUnavailable;

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>Déclenché juste avant l'affichage d'une nouvelle scène.</summary>
    public event Action<string> OnBeforeSceneShown;

    /// <summary>Déclenché une fois la nouvelle scène visible et l'ancienne masquée.</summary>
    public event Action<string> OnAfterSceneShown;

    /// <summary>Déclenché à chaque changement d'état de transition.</summary>
    public event Action<bool> OnTransitionStateChanged;

    // ── State ─────────────────────────────────────────────────────────────────

    /// <summary>Nom de la scène de contenu actuellement visible.</summary>
    public string CurrentScene { get; private set; }

    /// <summary>True si une transition est en cours.</summary>
    public bool IsTransitioning { get; private set; }

    // ── Scènes lazy — chargées à la première demande ──────────────────────────

    /// <summary>
    /// Noms des scènes qui doivent être lazy-loadées (ex : niveaux lourds).
    /// À peupler depuis l'Inspector ou via RegisterLazyScene().
    /// </summary>
    [SerializeField] private List<string> lazyScenes = new();

    private readonly HashSet<string> lazySceneNames = new();
    private readonly HashSet<string> loadedLazyScenes = new();
    private string pendingSceneName;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (string sceneName in lazyScenes)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
                lazySceneNames.Add(sceneName);
        }

        OnNavigatorAvailable?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;
        OnNavigatorUnavailable?.Invoke();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Affiche une scène de contenu et masque la précédente.
    /// Si la scène est lazy et pas encore chargée, elle est chargée en additif d'abord.
    /// Sans effet si la scène demandée est déjà active ou si une transition est en cours.
    /// </summary>
    public async Awaitable ShowScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[SceneNavigator] ShowScene appelé avec un nom de scène vide.");
            return;
        }

        pendingSceneName = sceneName;

        if (IsTransitioning)
            return;

        while (!string.IsNullOrEmpty(pendingSceneName))
        {
            string requestedScene = pendingSceneName;
            pendingSceneName = null;

            if (CurrentScene == requestedScene)
                continue;

            await PerformTransition(requestedScene);
        }
    }

    /// <summary>
    /// Enregistre une scène comme lazy (sera chargée à la première demande).
    /// Utile pour les scènes ajoutées dynamiquement sans passer par l'Inspector.
    /// </summary>
    public void RegisterLazyScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        lazySceneNames.Add(sceneName);
    }

    /// <summary>
    /// Indique à SceneNavigator quelle scène est déjà visible au démarrage.
    /// À appeler depuis GameBootstrap après le chargement initial.
    /// </summary>
    public void SetInitialScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        CurrentScene = sceneName;
        OnAfterSceneShown?.Invoke(sceneName);
    }

    /// <summary>Retourne true si la scène est chargée en mémoire.</summary>
    public bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid() && scene.isLoaded;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Active ou désactive tous les GameObjects racines d'une scène.</summary>
    private static void SetSceneRootsActive(string sceneName, bool active)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogWarning($"[SceneNavigator] Scène '{sceneName}' introuvable ou non chargée.");
            return;
        }

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.activeSelf != active)
                root.SetActive(active);
        }
    }

    /// <summary>Gère une transition complète vers la scène cible.</summary>
    private async Awaitable PerformTransition(string sceneName)
    {
        SetTransitionState(true);
        OnBeforeSceneShown?.Invoke(sceneName);

        try
        {
            await EnsureSceneLoaded(sceneName);

            if (!IsSceneLoaded(sceneName))
            {
                Debug.LogError($"[SceneNavigator] La scène '{sceneName}' n'est pas chargée après tentative de chargement.");
                return;
            }

            if (!string.IsNullOrEmpty(CurrentScene))
                SetSceneRootsActive(CurrentScene, false);

            SetSceneRootsActive(sceneName, true);
            CurrentScene = sceneName;
            OnAfterSceneShown?.Invoke(sceneName);
        }
        finally
        {
            SetTransitionState(false);
        }
    }

    /// <summary>Charge une scène si nécessaire (lazy ou fallback de robustesse).</summary>
    private async Awaitable EnsureSceneLoaded(string sceneName)
    {
        if (IsSceneLoaded(sceneName))
            return;

        await LoadSceneAdditive(sceneName);

        if (lazySceneNames.Contains(sceneName))
            loadedLazyScenes.Add(sceneName);
    }

    /// <summary>Charge une scène en additif et attend qu'elle soit prête.</summary>
    private static async Awaitable LoadSceneAdditive(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (op == null)
        {
            Debug.LogError($"[SceneNavigator] Impossible de charger '{sceneName}'. Vérifie les Build Settings.");
            return;
        }

        while (!op.isDone)
            await Awaitable.NextFrameAsync();
    }

    private void SetTransitionState(bool isTransitioning)
    {
        if (IsTransitioning == isTransitioning)
            return;

        IsTransitioning = isTransitioning;
        OnTransitionStateChanged?.Invoke(isTransitioning);
    }
}
