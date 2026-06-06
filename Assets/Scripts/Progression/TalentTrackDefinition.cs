using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Definition statique d'une piste de talents.
/// </summary>
[CreateAssetMenu(fileName = "TalentTrack_", menuName = "Game/Progression/Talent Track")]
public class TalentTrackDefinition : ScriptableObject
{
    [Header("Identite")]
    [SerializeField] private string trackId = ProgressionTrackId.Commerce;
    [SerializeField] private string displayName = "Piste";
    [SerializeField] private string shortLabel = "Piste";

    [Header("Noeuds")]
    [SerializeField] private List<TalentNodeDefinition> nodes = new();

    public string TrackId => trackId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName)
        ? ProgressionTrackId.GetDisplayName(trackId)
        : displayName;
    public string ShortLabel => string.IsNullOrWhiteSpace(shortLabel)
        ? ProgressionTrackId.GetShortLabel(trackId)
        : shortLabel;
    public IReadOnlyList<TalentNodeDefinition> Nodes => nodes;

    public void InitializeRuntime(string id, string name, string label, IEnumerable<TalentNodeDefinition> runtimeNodes)
    {
        trackId = id;
        displayName = name;
        shortLabel = label;
        nodes = runtimeNodes == null ? new List<TalentNodeDefinition>() : new List<TalentNodeDefinition>(runtimeNodes);
    }
}
