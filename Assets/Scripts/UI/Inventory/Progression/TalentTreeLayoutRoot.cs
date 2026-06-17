using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Racine d'un prefab arbre de talents (une piste). Layout compose a la main dans l'editeur.
/// </summary>
public class TalentTreeLayoutRoot : MonoBehaviour
{
    private const float DefaultLayoutCanvasWidth = 800f;
    private const float DefaultLayoutCanvasHeight = 600f;

    [SerializeField] private string trackId = ProgressionTrackId.Commerce;
    [SerializeField] private Vector2 layoutCanvasSize = new Vector2(
        DefaultLayoutCanvasWidth,
        DefaultLayoutCanvasHeight);
    [SerializeField] private bool autoCenterVisualContent = true;
    [SerializeField] private TalentNodeView[] nodeViews = System.Array.Empty<TalentNodeView>();
    [SerializeField] private TalentTreeEdgeView[] edgeViews = System.Array.Empty<TalentTreeEdgeView>();

    private TalentProgressionService progressionService;

    public string TrackId => trackId;
    public Vector2 LayoutCanvasSize => layoutCanvasSize;
    public IReadOnlyList<TalentNodeView> NodeViews => nodeViews;
    public IReadOnlyList<TalentTreeEdgeView> EdgeViews => edgeViews;

    /// <summary>
    /// Applique le layout runtime apres Instantiate sous le hote overlay (centre ou scroll).
    /// </summary>
    public void ApplyRuntimeMountLayout(RectTransform mountHost, bool centerInHost)
    {
        RectTransform rect = transform as RectTransform;
        if (rect == null || mountHost == null)
            return;

        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;

        Vector2 canvasSize = layoutCanvasSize;
        if (canvasSize.x < 1f || canvasSize.y < 1f)
            canvasSize = new Vector2(DefaultLayoutCanvasWidth, DefaultLayoutCanvasHeight);

        if (centerInHost)
        {
            ApplyCenteredMountLayout(rect, canvasSize);
            return;
        }

        ApplyScrollContentMountLayout(rect, canvasSize);
    }

    /// <summary>
    /// Decale Nodes pour que le groupe de noeuds soit au centre du canvas arbre.
    /// A appeler apres layout canvas (fin de frame) — le Prefab Mode centre la vue, pas le contenu.
    /// </summary>
    public void CenterVisualContentInCanvas()
    {
        if (!autoCenterVisualContent)
            return;

        RectTransform root = transform as RectTransform;
        if (root == null || !TryGetNodeBoundsInRootLocal(root, out Vector2 contentCenter))
            return;

        Vector2 delta = root.rect.center - contentCenter;
        if (delta.sqrMagnitude < 0.25f)
            return;

        ApplyLayoutLayerOffset("Nodes", delta);
    }

    private void ApplyLayoutLayerOffset(string layerName, Vector2 delta)
    {
        Transform layer = transform.Find(layerName);
        if (layer == null)
            return;

        RectTransform layerRect = layer as RectTransform;
        if (layerRect == null)
            return;

        layerRect.anchoredPosition += delta;
    }

    private bool TryGetNodeBoundsInRootLocal(RectTransform root, out Vector2 contentCenter)
    {
        contentCenter = Vector2.zero;
        if (nodeViews == null || nodeViews.Length == 0)
            return false;

        Vector3[] corners = new Vector3[4];
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        bool hasBounds = false;

        for (int i = 0; i < nodeViews.Length; i++)
        {
            TalentNodeView nodeView = nodeViews[i];
            if (nodeView == null)
                continue;

            RectTransform nodeRect = nodeView.transform as RectTransform;
            if (nodeRect == null)
                continue;

            nodeRect.GetWorldCorners(corners);
            for (int c = 0; c < corners.Length; c++)
            {
                Vector3 local = root.InverseTransformPoint(corners[c]);
                minX = Mathf.Min(minX, local.x);
                minY = Mathf.Min(minY, local.y);
                maxX = Mathf.Max(maxX, local.x);
                maxY = Mathf.Max(maxY, local.y);
                hasBounds = true;
            }
        }

        if (!hasBounds)
            return false;

        contentCenter = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
        return true;
    }

    private static void ApplyCenteredMountLayout(RectTransform rect, Vector2 canvasSize)
    {
        // Conserver le pivot auteur du prefab (souvent haut-gauche) pour ne pas decaler les enfants.
        Vector2 prefabPivot = rect.pivot;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = prefabPivot;
        rect.sizeDelta = canvasSize;
        rect.anchoredPosition = new Vector2(
            (prefabPivot.x - 0.5f) * canvasSize.x,
            (prefabPivot.y - 0.5f) * canvasSize.y);
    }

    private static void ApplyScrollContentMountLayout(RectTransform rect, Vector2 canvasSize)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = canvasSize;
    }

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
