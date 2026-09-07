using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ajoute / rafraîchit les mesh effects des Images sous un <see cref="UiQuadWarp"/>.
/// </summary>
public static class UiQuadWarpGraphicUtil
{
    public static void EnsureEffects(UiQuadWarp warp)
    {
        if (warp == null)
            return;

        Graphic[] graphics = warp.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            Graphic graphic = graphics[i];
            if (graphic == null)
                continue;
            if (graphic.GetComponent<UiQuadWarpMeshEffect>() != null)
                continue;

            graphic.gameObject.AddComponent<UiQuadWarpMeshEffect>();
        }
    }

    public static void DirtyChildren(UiQuadWarp warp)
    {
        if (warp == null)
            return;

        Graphic[] graphics = warp.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] != null)
                graphics[i].SetVerticesDirty();
        }
    }
}
