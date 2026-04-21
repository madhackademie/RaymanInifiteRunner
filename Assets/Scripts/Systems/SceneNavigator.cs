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

    // ── Events ────────────────────────────────────────────────────────────────

    /// <summary>Déclenché juste avant l'affichage d'une nouvelle scène.</summary>
    public event Action<string> OnBeforeSceneShown;

    /// <summary>Déclenché une fois la nouvelle scène visible et l'ancienne masquée.</summary>
    public event Action<string> OnAfterSceneShown;

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
            lazySceneNames.Add(sceneName);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Affiche une scène de contenu et masque la précédente.
    /// Si la scène est lazy et pas encore chargée, elle est chargée en additif d'abord.
    /// Sans effet si la scène demandée est déjà active ou si une transition est en cours.
    /// </summary>
    public async Awaitable ShowScene(string sceneName)
    {
        if (IsTransitioning || CurrentScene == sceneName)
            return;

        IsTransitioning = true;
        OnBeforeSceneShown?.Invoke(sceneName);

        // 1. Lazy load si nécessaire.
        if (lazySceneNames.Contains(sceneName) && !loadedLazyScenes.Contains(sceneName))
            await LoadSceneAdditive(sceneName);

        // 2. Masquer la scène précédente.
        if (!string.IsNullOrEmpty(CurrentScene))
            SetSceneRootsActive(CurrentScene, false);

        // 3. Afficher la nouvelle scène.
        SetSceneRootsActive(sceneName, true);
        CurrentScene = sceneName;

        IsTransitioning = false;
        OnAfterSceneShown?.Invoke(sceneName);
    }

    /// <summary>
    /// Enregistre une scène comme lazy (sera chargée à la première demande).
    /// Utile pour les scènes ajoutées dynamiquement sans passer par l'Inspector.
    /// </summary>
    public void RegisterLazyScene(string sceneName)
    {
        lazySceneNames.Add(sceneName);
    }

    /// <summary>
    /// Indique à SceneNavigator quelle scène est déjà visible au démarrage.
    /// À appeler depuis GameBootstrap après le chargement initial.
    /// </summary>
    public void SetInitialScene(string sceneName)
    {
        CurrentScene = sceneName;
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
            root.SetActive(active);
    }

    /// <summary>Charge une scène en additif et attend qu'elle soit prête.</summary>
    private async Awaitable LoadSceneAdditive(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (op == null)
        {
            Debug.LogError($"[SceneNavigator] Impossible de charger '{sceneName}'. Vérifie les Build Settings.");
            return;
        }

        while (!op.isDone)
            await Awaitable.NextFrameAsync();

        loadedLazyScenes.Add(sceneName);

        // On masque immédiatement — ShowScene s'occupe de l'affichage ensuite.
        SetSceneRootsActive(sceneName, false);
    }
}
