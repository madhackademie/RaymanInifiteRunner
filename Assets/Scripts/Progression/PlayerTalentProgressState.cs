using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Etat joueur des talents : points disponibles + rang par noeud.
/// </summary>
[Serializable]
public class PlayerTalentProgressState
{
    [Serializable]
    private struct NodeRankEntry
    {
        public string nodeId;
        public int rank;

        public NodeRankEntry(string id, int value)
        {
            nodeId = id;
            rank = value;
        }
    }

    [SerializeField] private int availableSkillPoints;
    [SerializeField] private List<NodeRankEntry> nodeRanks = new();

    private Dictionary<string, int> rankLookup;

    public int AvailableSkillPoints => Mathf.Max(0, availableSkillPoints);

    public int GetRank(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId))
            return 0;

        EnsureLookup();
        return rankLookup.TryGetValue(nodeId, out int rank) ? Mathf.Max(0, rank) : 0;
    }

    public void AddSkillPoints(int amount)
    {
        if (amount <= 0)
            return;

        availableSkillPoints += amount;
    }

    public bool TrySpendSkillPoints(int amount)
    {
        int clampedCost = Mathf.Max(0, amount);
        if (availableSkillPoints < clampedCost)
            return false;

        availableSkillPoints -= clampedCost;
        return true;
    }

    public void SetRank(string nodeId, int rank)
    {
        if (string.IsNullOrEmpty(nodeId))
            return;

        EnsureLookup();
        int clampedRank = Mathf.Max(0, rank);
        rankLookup[nodeId] = clampedRank;
        SyncEntriesFromLookup();
    }

    public void Clear()
    {
        availableSkillPoints = 0;
        nodeRanks.Clear();
        rankLookup?.Clear();
    }

    private void EnsureLookup()
    {
        if (rankLookup != null)
            return;

        rankLookup = new Dictionary<string, int>(nodeRanks.Count);
        foreach (NodeRankEntry entry in nodeRanks)
        {
            if (string.IsNullOrEmpty(entry.nodeId))
                continue;

            rankLookup[entry.nodeId] = Mathf.Max(0, entry.rank);
        }
    }

    private void SyncEntriesFromLookup()
    {
        nodeRanks.Clear();
        foreach (KeyValuePair<string, int> pair in rankLookup)
            nodeRanks.Add(new NodeRankEntry(pair.Key, pair.Value));
    }
}
