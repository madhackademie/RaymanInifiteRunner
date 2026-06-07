using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Service runtime de progression des talents (MVP).
/// </summary>
public class TalentProgressionService : MonoBehaviour
{
    private const string MockRootNodeId = "talent.commerce.root";
    private const string MockBuyerNodeId = "talent.commerce.buyer.discount1";
    private const string MockSellerNodeId = "talent.commerce.seller.price1";

    [Header("Definitions")]
    [SerializeField] private TalentTrackDefinition[] trackDefinitions = Array.Empty<TalentTrackDefinition>();

    [Header("Player state")]
    [SerializeField] private PlayerTalentProgressState playerState = new();
    [SerializeField] private int currentPlayerLevel = 1;
    [SerializeField] private int startingSkillPoints = 3;
    [SerializeField] private bool initializeStartingSkillPoints = true;
    [SerializeField] private bool hasInitializedStartingPoints;

    [Header("MVP bootstrap")]
    [SerializeField] private bool autoCreateMockCommerceTrackWhenEmpty = true;

    private readonly Dictionary<string, TalentTrackDefinition> tracksById = new();
    private readonly Dictionary<string, TalentNodeDefinition> nodesById = new();

    public event Action StateChanged;

    public int AvailableSkillPoints => playerState.AvailableSkillPoints;
    public int CurrentPlayerLevel => Mathf.Max(1, currentPlayerLevel);

    private void Awake()
    {
        EnsureDefinitions();
        BuildLookups();
        EnsureStartingSkillPoints();
        NotifyStateChanged();
    }

    public void AddSkillPoints(int amount)
    {
        if (amount <= 0)
            return;

        playerState.AddSkillPoints(amount);
        NotifyStateChanged();
    }

    public void SetPlayerLevel(int level)
    {
        currentPlayerLevel = Mathf.Max(1, level);
        NotifyStateChanged();
    }

    public int GetNodeRank(string nodeId) => playerState.GetRank(nodeId);

    public string GetTrackDisplayName(string trackId)
    {
        return TryGetTrack(trackId, out TalentTrackDefinition track)
            ? track.DisplayName
            : ProgressionTrackId.GetDisplayName(trackId);
    }

    public IReadOnlyList<TalentNodeDefinition> GetNodesForTrack(string trackId)
    {
        return TryGetTrack(trackId, out TalentTrackDefinition track)
            ? track.Nodes
            : Array.Empty<TalentNodeDefinition>();
    }

    public TalentNodeStatus GetNodeStatus(string nodeId)
    {
        if (!TryGetNode(nodeId, out TalentNodeDefinition node))
            return TalentNodeStatus.Locked;

        int rank = playerState.GetRank(nodeId);
        if (rank >= node.MaxRank)
            return TalentNodeStatus.Maxed;

        if (rank > 0)
            return TalentNodeStatus.Purchased;

        return CanPurchase(nodeId, out _) ? TalentNodeStatus.Available : TalentNodeStatus.Locked;
    }

    public bool CanPurchase(string nodeId, out string reason)
    {
        if (!TryGetNode(nodeId, out TalentNodeDefinition node))
            return Fail("Noeud introuvable.", out reason);

        int rank = playerState.GetRank(nodeId);
        if (rank >= node.MaxRank)
            return Fail("Noeud deja au rang max.", out reason);

        if (!ArePrerequisitesMet(node))
            return Fail("Pre-requis manquants.", out reason);

        if (CurrentPlayerLevel < node.RequiredPlayerLevel)
            return Fail("Niveau joueur insuffisant.", out reason);

        if (AvailableSkillPoints < node.CostPoints)
            return Fail("Points de competence insuffisants.", out reason);

        reason = string.Empty;
        return true;
    }

    public bool TryPurchaseNode(string nodeId, out string reason)
    {
        if (!CanPurchase(nodeId, out reason))
            return false;

        TalentNodeDefinition node = nodesById[nodeId];
        if (!playerState.TrySpendSkillPoints(node.CostPoints))
            return Fail("Depense de points refusee.", out reason);

        int nextRank = playerState.GetRank(nodeId) + 1;
        playerState.SetRank(nodeId, nextRank);
        NotifyStateChanged();
        reason = string.Empty;
        return true;
    }

    public bool TryPurchaseFirstAvailableNode(
        string trackId,
        out TalentNodeDefinition purchasedNode,
        out string reason)
    {
        purchasedNode = null;
        IReadOnlyList<TalentNodeDefinition> nodes = GetNodesForTrack(trackId);
        for (int i = 0; i < nodes.Count; i++)
        {
            TalentNodeDefinition node = nodes[i];
            if (node == null || !CanPurchase(node.NodeId, out _))
                continue;

            if (!TryPurchaseNode(node.NodeId, out reason))
                return false;

            purchasedNode = node;
            return true;
        }

        reason = "Aucun noeud achetable sur cette piste.";
        return false;
    }

    public bool CanPurchaseAnyNode(string trackId)
    {
        IReadOnlyList<TalentNodeDefinition> nodes = GetNodesForTrack(trackId);
        for (int i = 0; i < nodes.Count; i++)
        {
            TalentNodeDefinition node = nodes[i];
            if (node != null && CanPurchase(node.NodeId, out _))
                return true;
        }

        return false;
    }

    public string BuildTrackSummary(string trackId)
    {
        if (!TryGetTrack(trackId, out TalentTrackDefinition track))
            return "Aucun arbre configure pour cette piste.";

        StringBuilder builder = new StringBuilder(256);
        builder.AppendLine($"{track.DisplayName} - {AvailableSkillPoints} pts disponibles");
        IReadOnlyList<TalentNodeDefinition> nodes = track.Nodes;
        for (int i = 0; i < nodes.Count; i++)
        {
            TalentNodeDefinition node = nodes[i];
            if (node == null)
                continue;

            int rank = playerState.GetRank(node.NodeId);
            TalentNodeStatus status = GetNodeStatus(node.NodeId);
            builder.Append($"* {node.DisplayName} ({rank}/{node.MaxRank})");
            builder.Append($" - cout {node.CostPoints} - {ToStatusLabel(status)}");
            if (i < nodes.Count - 1)
                builder.AppendLine();
        }

        return builder.ToString();
    }

    [ContextMenu("Talents/Add 1 skill point")]
    private void DebugAddSkillPoint() => AddSkillPoints(1);

    [ContextMenu("Talents/Reset progression")]
    private void DebugResetProgression()
    {
        playerState.Clear();
        hasInitializedStartingPoints = false;
        EnsureStartingSkillPoints();
        NotifyStateChanged();
    }

    private void EnsureDefinitions()
    {
        if (!autoCreateMockCommerceTrackWhenEmpty || HasDefinitions())
            return;

        trackDefinitions = new[] { CreateMockCommerceTrack() };
    }

    private bool HasDefinitions()
    {
        for (int i = 0; i < trackDefinitions.Length; i++)
        {
            TalentTrackDefinition track = trackDefinitions[i];
            if (track != null)
                return true;
        }

        return false;
    }

    private void BuildLookups()
    {
        tracksById.Clear();
        nodesById.Clear();
        for (int i = 0; i < trackDefinitions.Length; i++)
            RegisterTrack(trackDefinitions[i]);
    }

    private void RegisterTrack(TalentTrackDefinition track)
    {
        if (track == null || string.IsNullOrEmpty(track.TrackId))
            return;

        tracksById[track.TrackId] = track;
        IReadOnlyList<TalentNodeDefinition> nodes = track.Nodes;
        for (int i = 0; i < nodes.Count; i++)
        {
            TalentNodeDefinition node = nodes[i];
            if (node == null || string.IsNullOrEmpty(node.NodeId))
                continue;

            nodesById[node.NodeId] = node;
        }
    }

    private bool TryGetTrack(string trackId, out TalentTrackDefinition track)
    {
        if (string.IsNullOrEmpty(trackId))
        {
            track = null;
            return false;
        }

        return tracksById.TryGetValue(trackId, out track);
    }

    private bool TryGetNode(string nodeId, out TalentNodeDefinition node)
    {
        if (string.IsNullOrEmpty(nodeId))
        {
            node = null;
            return false;
        }

        return nodesById.TryGetValue(nodeId, out node);
    }

    private void EnsureStartingSkillPoints()
    {
        if (!initializeStartingSkillPoints || hasInitializedStartingPoints)
            return;

        playerState.AddSkillPoints(Mathf.Max(0, startingSkillPoints));
        hasInitializedStartingPoints = true;
    }

    private bool ArePrerequisitesMet(TalentNodeDefinition node)
    {
        IReadOnlyList<string> prerequisites = node.PrerequisiteNodeIds;
        for (int i = 0; i < prerequisites.Count; i++)
        {
            string prerequisiteId = prerequisites[i];
            if (playerState.GetRank(prerequisiteId) <= 0)
                return false;
        }

        return true;
    }

    private static bool Fail(string error, out string reason)
    {
        reason = error;
        return false;
    }

    private static string ToStatusLabel(TalentNodeStatus status)
    {
        return status switch
        {
            TalentNodeStatus.Locked => "Verrouille",
            TalentNodeStatus.Available => "Disponible",
            TalentNodeStatus.Purchased => "Achete",
            TalentNodeStatus.Maxed => "Max",
            _ => "Inconnu",
        };
    }

    private void NotifyStateChanged() => StateChanged?.Invoke();

    private TalentTrackDefinition CreateMockCommerceTrack()
    {
        TalentNodeDefinition root = CreateMockNode(
            MockRootNodeId,
            "Racine Commerce",
            ProgressionTrackId.Commerce,
            1,
            1,
            Array.Empty<string>());

        TalentNodeDefinition buyer = CreateMockNode(
            MockBuyerNodeId,
            "Acheteur -5%",
            ProgressionTrackId.Commerce,
            1,
            1,
            new[] { MockRootNodeId });

        TalentNodeDefinition seller = CreateMockNode(
            MockSellerNodeId,
            "Vendeur +5%",
            ProgressionTrackId.Commerce,
            1,
            1,
            new[] { MockRootNodeId });

        TalentTrackDefinition track = ScriptableObject.CreateInstance<TalentTrackDefinition>();
        track.InitializeRuntime(
            ProgressionTrackId.Commerce,
            "Commerce",
            "Commerce",
            new[] { root, buyer, seller });
        return track;
    }

    private TalentNodeDefinition CreateMockNode(
        string nodeId,
        string displayName,
        string trackId,
        int cost,
        int maxRank,
        IEnumerable<string> prerequisites)
    {
        TalentNodeDefinition node = ScriptableObject.CreateInstance<TalentNodeDefinition>();
        node.InitializeRuntime(nodeId, displayName, trackId, cost, maxRank, prerequisites);
        return node;
    }
}
