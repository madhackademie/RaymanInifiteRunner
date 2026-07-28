using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Path local sous une plante : enfants <c>Node_*</c> placés en Scene.
/// Chaque node = arrêt (butinage) puis vol vers le suivant (sens fixé au RestartCircuit).
/// </summary>
public class InsectPathAnchor : MonoBehaviour
{
    private const string NodePrefix = "Node";

    [Header("Editor / Prefab")]
    [Tooltip("Si true, RefreshNodesFromChildren remplit la liste depuis les enfants Node_*.")]
    [SerializeField] private bool autoCollectNodes = true;

    [SerializeField] private Transform[] nodes = System.Array.Empty<Transform>();

    [Tooltip("Instance insecte sous ce path, ou assignée manuellement.")]
    [SerializeField] private InsectPathFollower insectFollower;

    [Header("Gizmos")]
    [SerializeField] private Color gizmoColor = new Color(1f, 0.85f, 0.2f, 0.9f);
    [SerializeField] private float gizmoRadius = 0.08f;

    /// <summary>Nodes ordonnés du circuit.</summary>
    public IReadOnlyList<Transform> Nodes => nodes;

    /// <summary>Follower insecte branché sur ce path.</summary>
    public InsectPathFollower InsectFollower => insectFollower;

    /// <summary>
    /// Active/désactive le path + insecte.
    /// Si <paramref name="visualKind"/> est Bee/Butterfly, applique le controller avant RestartCircuit.
    /// </summary>
    public void SetPathActive(bool active, InsectKind visualKind = InsectKind.Bee)
    {
        gameObject.SetActive(active);

        if (!active || insectFollower == null)
            return;

        insectFollower.BindPath(this);
        if (visualKind == InsectKind.Bee || visualKind == InsectKind.Butterfly)
            insectFollower.ApplyVisualKind(visualKind);
        insectFollower.RestartCircuit();
    }

    [ContextMenu("Refresh Nodes From Children")]
    public void RefreshNodesFromChildren()
    {
        if (!autoCollectNodes)
            return;

        var found = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (insectFollower != null && child == insectFollower.transform)
                continue;

            if (child.name.StartsWith(NodePrefix, System.StringComparison.OrdinalIgnoreCase))
                found.Add(child);
        }

        nodes = found.ToArray();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (insectFollower == null)
            insectFollower = GetComponentInChildren<InsectPathFollower>(true);

        if (autoCollectNodes)
            RefreshNodesFromChildren();
    }

    private void OnDrawGizmosSelected()
    {
        if (nodes == null || nodes.Length == 0)
            return;

        Gizmos.color = gizmoColor;
        for (int i = 0; i < nodes.Length; i++)
        {
            Transform node = nodes[i];
            if (node == null)
                continue;

            Gizmos.DrawWireSphere(node.position, gizmoRadius);
            Transform next = nodes[(i + 1) % nodes.Length];
            if (next != null)
                Gizmos.DrawLine(node.position, next.position);
        }
    }
#endif
}
