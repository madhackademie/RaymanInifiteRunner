using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contrôleur de l'écran d'accueil (HomeScene).
/// Instancie et trie les boutons d'entrée, gère la transition vers les scènes de jeu
/// et expose l'API pour déverrouiller des entrées depuis d'autres systèmes.
/// </summary>
public class MapSceneController : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Données")]
    [Tooltip("Toutes les entrées du hub. L'ordre d'affichage est contrôlé par MapNodeData.sortOrder.")]
    [SerializeField] private List<MapNodeData> allNodes = new();

    [Tooltip("ScriptableObject persistant l'état de déverrouillage.")]
    [SerializeField] private MapProgressionData progressionData;

    [Header("Prefab & Container")]
    [Tooltip("Prefab d'un bouton d'entrée (doit contenir un MapNodeButton).")]
    [SerializeField] private MapNodeButton nodeButtonPrefab;

    [Tooltip("Parent vertical sous lequel les boutons sont instanciés (avec VerticalLayoutGroup).")]
    [SerializeField] private RectTransform nodesContainer;

    [Header("Loading")]
    [Tooltip("Référence optionnelle à un LoadingScreen local à la scène Hub.")]
    [SerializeField] private LoadingScreen loadingScreen;

    // ── Runtime ───────────────────────────────────────────────────────────────

    private readonly List<MapNodeButton> spawnedButtons = new();
    private List<MapNodeData> sortedNodes;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private async void Start()
    {
        await UIManager.EnsureShellLoaded();
        NavigationHUD.ShowNavBar();

        SortNodes();
        EnsureProgressionDefaults();
        BuildButtons();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Déverrouille une entrée par son identifiant et rafraîchit l'affichage.
    /// À appeler depuis un système de récompense après qu'un niveau est complété.
    /// </summary>
    public void UnlockNode(string nodeId)
    {
        progressionData.Unlock(nodeId);
        RefreshButtons();
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void SortNodes()
    {
        sortedNodes = new List<MapNodeData>(allNodes);
        sortedNodes.Sort((a, b) => a.sortOrder.CompareTo(b.sortOrder));
    }

    private void EnsureProgressionDefaults()
    {
        progressionData.InitializeDefaults(sortedNodes);
    }

    private void BuildButtons()
    {
        foreach (MapNodeButton existing in spawnedButtons)
        {
            if (existing != null)
                Destroy(existing.gameObject);
        }
        spawnedButtons.Clear();

        foreach (MapNodeData nodeData in sortedNodes)
        {
            MapNodeButton btn = Instantiate(nodeButtonPrefab, nodesContainer);
            bool isUnlocked = progressionData.IsUnlocked(nodeData.nodeId);
            btn.Setup(nodeData, isUnlocked);
            btn.OnNodeSelected += HandleNodeSelected;
            spawnedButtons.Add(btn);
        }
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            if (i >= sortedNodes.Count) break;
            MapNodeData data = sortedNodes[i];
            spawnedButtons[i].Setup(data, progressionData.IsUnlocked(data.nodeId));
        }
    }

    private async void HandleNodeSelected(MapNodeData data)
    {
        if (loadingScreen != null)
            loadingScreen.SetProgress(0f);

        // Charge la scène cible en additif pour préserver la scène shell NavigationHUD.
        AsyncOperation op = SceneManager.LoadSceneAsync(data.targetSceneName, LoadSceneMode.Additive);

        if (op == null)
        {
            Debug.LogError($"[MapSceneController] Impossible de charger '{data.targetSceneName}'. Vérifie les Build Settings.");
            return;
        }

        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            if (loadingScreen != null)
                loadingScreen.SetProgress(op.progress / 0.9f);
            await Awaitable.NextFrameAsync();
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetProgress(1f);
            await loadingScreen.Hide();
        }

        op.allowSceneActivation = true;

        // Attend que la scène soit bien activée avant de décharger HomeScene.
        while (!op.isDone)
            await Awaitable.NextFrameAsync();

        await SceneManager.UnloadSceneAsync(SceneId.HomeScene);
    }
}
