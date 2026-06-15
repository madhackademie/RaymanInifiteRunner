using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ligne droite entre deux noeuds UI. Se repositionne en edit mode et en play mode.
/// </summary>
[ExecuteAlways]
public class TalentTreeEdgeView : MonoBehaviour
{
    private const float MinLineLength = 2f;

    [Header("Connexions")]
    [SerializeField] private TalentNodeView fromNode;
    [SerializeField] private TalentNodeView toNode;

    [Header("Rendu")]
    [SerializeField] private RectTransform lineRect;
    [SerializeField] private Image lineImage;
    [SerializeField] private float lineThickness = 4f;

    public TalentNodeView FromNode => fromNode;
    public TalentNodeView ToNode => toNode;

    private void OnEnable() => RefreshLine();

    private void LateUpdate() => RefreshLine();

    private void OnValidate() => RefreshLine();

    public void RefreshLine()
    {
        if (lineRect == null)
            lineRect = transform as RectTransform;

        if (fromNode == null || toNode == null || lineRect == null)
            return;

        RectTransform fromRect = fromNode.NodeRect;
        RectTransform toRect = toNode.NodeRect;
        if (fromRect == null || toRect == null)
            return;

        Vector2 fromPivotWorld = fromRect.TransformPoint(fromRect.rect.center);
        Vector2 toPivotWorld = toRect.TransformPoint(toRect.rect.center);

        Transform parent = lineRect.parent;
        Vector2 fromLocal = parent != null
            ? (Vector2)parent.InverseTransformPoint(fromPivotWorld)
            : fromPivotWorld;
        Vector2 toLocal = parent != null
            ? (Vector2)parent.InverseTransformPoint(toPivotWorld)
            : toPivotWorld;

        Vector2 delta = toLocal - fromLocal;
        float length = delta.magnitude;
        if (length < MinLineLength)
            length = MinLineLength;

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        Vector2 midpoint = (fromLocal + toLocal) * 0.5f;

        lineRect.anchoredPosition = midpoint;
        lineRect.sizeDelta = new Vector2(length, lineThickness);
        lineRect.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (lineImage != null)
            lineImage.raycastTarget = false;
    }
}
