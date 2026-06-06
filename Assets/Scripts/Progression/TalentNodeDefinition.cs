using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Definition statique d'un noeud de talents.
/// </summary>
[CreateAssetMenu(fileName = "TalentNode_", menuName = "Game/Progression/Talent Node")]
public class TalentNodeDefinition : ScriptableObject
{
    [Header("Identite")]
    [SerializeField] private string nodeId = "talent.node.id";
    [SerializeField] private string displayName = "Noeud";
    [SerializeField] private string trackId = ProgressionTrackId.Commerce;

    [Header("Progression")]
    [SerializeField] private int costPoints = 1;
    [SerializeField] private int maxRank = 1;
    [SerializeField] private int requiredPlayerLevel = 1;
    [SerializeField] private List<string> prerequisiteNodeIds = new();

    public string NodeId => nodeId;
    public string DisplayName => displayName;
    public string TrackId => trackId;
    public int CostPoints => Mathf.Max(1, costPoints);
    public int MaxRank => Mathf.Max(1, maxRank);
    public int RequiredPlayerLevel => Mathf.Max(1, requiredPlayerLevel);
    public IReadOnlyList<string> PrerequisiteNodeIds => prerequisiteNodeIds;

    public void InitializeRuntime(
        string id,
        string name,
        string ownerTrackId,
        int pointsCost,
        int maxNodeRank,
        IEnumerable<string> prerequisites)
    {
        nodeId = id;
        displayName = name;
        trackId = ownerTrackId;
        costPoints = Mathf.Max(1, pointsCost);
        maxRank = Mathf.Max(1, maxNodeRank);
        requiredPlayerLevel = 1;
        prerequisiteNodeIds = prerequisites == null ? new List<string>() : new List<string>(prerequisites);
    }
}
