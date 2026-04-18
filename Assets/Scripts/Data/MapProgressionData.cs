using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject persistant l'état de déverrouillage des nœuds de la carte.
/// Utilisé comme source de vérité runtime. Pour la persistance longue durée,
/// sérialiser/désérialiser via PlayerPrefs ou un système de sauvegarde dédié.
/// </summary>
[CreateAssetMenu(fileName = "MapProgressionData", menuName = "Game/Map Progression Data")]
public class MapProgressionData : ScriptableObject
{
    // ── Sérialisation ─────────────────────────────────────────────────────────

    [Tooltip("Identifiants des nœuds actuellement déverrouillés.")]
    [SerializeField] private List<string> unlockedNodeIds = new();

    // ── Runtime ───────────────────────────────────────────────────────────────

    private HashSet<string> unlockedSet;

    private void OnEnable()
    {
        RebuildSet();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Retourne true si le nœud identifié par <paramref name="nodeId"/> est déverrouillé.</summary>
    public bool IsUnlocked(string nodeId)
    {
        EnsureSet();
        return unlockedSet.Contains(nodeId);
    }

    /// <summary>Déverrouille un nœud. Sans effet si déjà déverrouillé.</summary>
    public void Unlock(string nodeId)
    {
        EnsureSet();
        if (unlockedSet.Add(nodeId))
            unlockedNodeIds.Add(nodeId);
    }

    /// <summary>Réinitialise toute la progression (utile pour un nouveau jeu).</summary>
    public void ResetAll()
    {
        unlockedNodeIds.Clear();
        unlockedSet?.Clear();
    }

    /// <summary>
    /// Initialise les nœuds marqués comme déverrouillés par défaut depuis la liste de définitions.
    /// À appeler une fois au démarrage d'une nouvelle partie.
    /// </summary>
    public void InitializeDefaults(IEnumerable<MapNodeData> allNodes)
    {
        foreach (MapNodeData node in allNodes)
        {
            if (node.unlockedByDefault)
                Unlock(node.nodeId);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void RebuildSet()
    {
        unlockedSet = new HashSet<string>(unlockedNodeIds);
    }

    private void EnsureSet()
    {
        if (unlockedSet == null)
            RebuildSet();
    }
}
