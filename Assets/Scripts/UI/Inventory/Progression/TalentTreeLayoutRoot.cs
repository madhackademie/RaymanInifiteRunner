using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Racine d'un prefab arbre de talents (une piste). Layout compose a la main dans l'editeur.
/// </summary>
public class TalentTreeLayoutRoot : MonoBehaviour
{
    [SerializeField] private string trackId = ProgressionTrackId.Commerce;
    [SerializeField] private TalentNodeView[] nodeViews = System.Array.Empty<TalentNodeView>();
    [SerializeField] private TalentTreeEdgeView[] edgeViews = System.Array.Empty<TalentTreeEdgeView>();

    private TalentProgressionService progressionService;

    public string TrackId => trackId;
    public IReadOnlyList<TalentNodeView> NodeViews => nodeViews;
    public IReadOnlyList<TalentTreeEdgeView> EdgeViews => edgeViews;

    public void CollectNodeViewsFromChildren()
    {
        nodeViews = GetComponentsInChildren<TalentNodeView>(true);
    }

    public void CollectEdgeViewsFromChildren()
    {
        edgeViews = GetComponentsInChildren<TalentTreeEdgeView>(true);
    }

    public void Bind(TalentProgressionService service)
    {
        progressionService = service;

        for (int i = 0; i < nodeViews.Length; i++)
        {
            if (nodeViews[i] != null)
                nodeViews[i].Bind(service);
        }

        RefreshAll();
    }

    public void Unbind()
    {
        for (int i = 0; i < nodeViews.Length; i++)
        {
            if (nodeViews[i] != null)
                nodeViews[i].Unbind();
        }

        progressionService = null;
    }

    public void RefreshAll()
    {
        for (int i = 0; i < nodeViews.Length; i++)
        {
            if (nodeViews[i] != null)
                nodeViews[i].Refresh();
        }

        for (int i = 0; i < edgeViews.Length; i++)
        {
            if (edgeViews[i] != null)
                edgeViews[i].RefreshLine();
        }
    }

    public bool ValidateEdgesAgainstDefinitions(bool logWarnings)
    {
        bool valid = true;
        for (int i = 0; i < edgeViews.Length; i++)
        {
            TalentTreeEdgeView edge = edgeViews[i];
            if (edge == null || edge.FromNode == null || edge.ToNode == null)
                continue;

            string fromId = edge.FromNode.NodeId;
            string toId = edge.ToNode.NodeId;
            if (string.IsNullOrEmpty(fromId) || string.IsNullOrEmpty(toId))
            {
                if (logWarnings)
                {
                    Debug.LogWarning(
                        $"[TalentTreeLayoutRoot] Edge sans NodeId sur {edge.name}.",
                        edge);
                }

                valid = false;
                continue;
            }

            if (!EdgeMatchesPrerequisite(edge.ToNode, fromId) && logWarnings)
            {
                Debug.LogWarning(
                    $"[TalentTreeLayoutRoot] Edge {fromId} -> {toId} ne correspond pas aux pre-requis SO.",
                    edge);
                valid = false;
            }
        }

        return valid;
    }

    private bool EdgeMatchesPrerequisite(TalentNodeView childView, string parentNodeId)
    {
        TalentNodeDefinition childDefinition = childView != null ? childView.Definition : null;
        if (childDefinition == null)
            return false;

        IReadOnlyList<string> prerequisites = childDefinition.PrerequisiteNodeIds;
        if (prerequisites.Count == 0)
            return true;

        for (int i = 0; i < prerequisites.Count; i++)
        {
            if (prerequisites[i] == parentNodeId)
                return true;
        }

        return false;
    }
}
