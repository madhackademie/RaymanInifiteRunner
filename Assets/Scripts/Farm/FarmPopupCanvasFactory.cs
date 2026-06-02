using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Crée (ou retrouve) le canvas overlay et le RectTransform racine des popups ferme,
/// sous l'hôte de popups fourni. Logique de construction UI extraite de BiofiltreManager.
/// </summary>
public static class FarmPopupCanvasFactory
{
    private const int CanvasSortOrder = 10;
    private const string CanvasObjectName = "FarmUICanvas";
    private const string RootObjectName = "FarmPopupRoot";
    private static readonly Vector2 ReferenceResolution = new(800f, 600f);

    /// <summary>
    /// Retourne le RectTransform "FarmPopupRoot" existant sous <paramref name="host"/>,
    /// ou crée le canvas + la racine plein écran si absents.
    /// </summary>
    public static RectTransform CreateOrFind(Transform host)
    {
        if (host == null)
            return null;

        Transform existing = host.Find(RootObjectName);
        if (existing != null)
            return existing as RectTransform;

        var canvasGo = new GameObject(CanvasObjectName);
        canvasGo.transform.SetParent(host, false);

        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = CanvasSortOrder;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;

        canvasGo.AddComponent<GraphicRaycaster>();

        var rootGo = new GameObject(RootObjectName, typeof(RectTransform));
        RectTransform rect = rootGo.GetComponent<RectTransform>();
        rect.SetParent(canvasGo.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return rect;
    }
}
