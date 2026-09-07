using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Déforme le mesh d’une Image selon le <see cref="UiQuadWarp"/> parent.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Graphic))]
public class UiQuadWarpMeshEffect : BaseMeshEffect
{
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0)
            return;

        UiQuadWarp warp = GetComponentInParent<UiQuadWarp>();
        if (warp == null || !warp.HasCustomWarp)
            return;

        Transform graphicTransform = graphic.transform;
        UIVertex vertex = new UIVertex();
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            vertex.position = warp.WarpVertex(graphicTransform, vertex.position);
            vh.SetUIVertex(vertex, i);
        }
    }
}
