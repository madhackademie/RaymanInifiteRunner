using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applique un dégradé horizontal aux vertex d'un composant Graphic (Image, etc.).
/// Le dégradé est calculé sur la largeur totale du RectTransform, garantissant
/// une cohérence visuelle quelle que soit la valeur de fillAmount.
/// </summary>
[RequireComponent(typeof(Graphic))]
public class HorizontalGradient : BaseMeshEffect
{
    [SerializeField] private Color leftColor  = Color.blue;
    [SerializeField] private Color rightColor = Color.green;

    /// <summary>Rafraîchit le mesh pour appliquer le nouveau gradient.</summary>
    public void SetColors(Color left, Color right)
    {
        leftColor  = left;
        rightColor = right;
        graphic.SetVerticesDirty();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);

        if (vertices.Count == 0) return;

        // On normalise par rapport à la largeur du PARENT (= la barre complète),
        // pas la largeur courante du fill. Ainsi le dégradé représente toujours
        // la position dans la barre entière, quelle que soit la progression.
        RectTransform parent = graphic.rectTransform.parent as RectTransform;
        float fullWidth = parent != null ? parent.rect.width : graphic.rectTransform.rect.width;

        if (fullWidth <= 0f) return;

        // xMin en espace local du fill (dépend du pivot)
        float xMin = graphic.rectTransform.rect.xMin;

        for (int i = 0; i < vertices.Count; i++)
        {
            UIVertex vertex = vertices[i];
            float t = Mathf.Clamp01((vertex.position.x - xMin) / fullWidth);
            vertex.color = Color.Lerp(leftColor, rightColor, t);
            vertices[i] = vertex;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertices);
    }
}
