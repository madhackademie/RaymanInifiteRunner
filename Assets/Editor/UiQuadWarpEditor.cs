using UnityEditor;
using UnityEngine;

/// <summary>
/// Poignées Scene : 4 coins indépendants (pas l’outil Rect Unity).
/// </summary>
[CustomEditor(typeof(UiQuadWarp))]
public class UiQuadWarpEditor : Editor
{
    private static readonly Color HandleColor = new(1f, 0.85f, 0.15f, 1f);
    private static readonly Color LineColor = new(1f, 0.85f, 0.15f, 0.9f);
    private const float HandleScale = 0.12f;
    private const int CornerCount = 4;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox(
            "Les cercles bleus de l’outil Rect restent un rectangle.\n" +
            "Poignées jaunes Scene = coins du losange / trapèze (angles cassés).\n" +
            "Sélectionne la rangée (PrimaryRow / StarRow / SecondaryRow).",
            MessageType.Info);

        if (GUILayout.Button("Reset corners to rect"))
        {
            var warp = (UiQuadWarp)target;
            Undo.RecordObject(warp, "Reset UI quad warp");
            warp.ResetCornersToRect();
            EditorUtility.SetDirty(warp);
        }
    }

    [InitializeOnLoadMethod]
    private static void RegisterSceneGui()
    {
        SceneView.duringSceneGui -= DrawSelectedWarp;
        SceneView.duringSceneGui += DrawSelectedWarp;
    }

    private static void DrawSelectedWarp(SceneView sceneView)
    {
        GameObject go = Selection.activeGameObject;
        if (go == null)
            return;

        UiQuadWarp warp = go.GetComponent<UiQuadWarp>()
            ?? go.GetComponentInParent<UiQuadWarp>();
        if (warp == null)
            return;

        DrawWarpHandles(warp);
    }

    private static void DrawWarpHandles(UiQuadWarp warp)
    {
        if (warp == null)
            return;

        Transform t = warp.transform;
        Vector3[] world = new Vector3[CornerCount];
        for (int i = 0; i < CornerCount; i++)
            world[i] = t.TransformPoint(warp.GetCorner(i));

        Handles.color = LineColor;
        Handles.DrawAAPolyLine(3f, world[0], world[1], world[2], world[3], world[0]);

        for (int i = 0; i < CornerCount; i++)
            DrawCornerHandle(warp, i, world[i]);
    }

    private static void DrawCornerHandle(UiQuadWarp warp, int index, Vector3 world)
    {
        float size = HandleUtility.GetHandleSize(world) * HandleScale;
        Handles.color = HandleColor;
        Handles.DrawSolidDisc(world, Vector3.forward, size * 0.35f);

        EditorGUI.BeginChangeCheck();
        Vector3 dragged = Handles.Slider2D(
            world, Vector3.forward, Vector3.right, Vector3.up, size, Handles.DotHandleCap, Vector2.zero);
        if (!EditorGUI.EndChangeCheck())
            return;

        Undo.RecordObject(warp, "Move UI quad corner");
        Vector3 local = warp.transform.InverseTransformPoint(dragged);
        warp.SetCorner(index, new Vector2(local.x, local.y));
        EditorUtility.SetDirty(warp);
        warp.DirtyMeshes();
    }
}
