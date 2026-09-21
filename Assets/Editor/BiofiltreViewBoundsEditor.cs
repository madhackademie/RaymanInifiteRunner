using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(BiofiltreViewBounds))]
public class BiofiltreViewBoundsEditor : Editor
{
    private BoxBoundsHandle boxHandle;

    private void OnEnable()
    {
        boxHandle = new BoxBoundsHandle
        {
            axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y
        };
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var bounds = (BiofiltreViewBounds)target;

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "Rect orange = vue caméra par défaut + limite de zoom out.\n" +
            "Indépendant du bake grille (cyan) et de GetWorldRect.\n" +
            "Init = point de départ, puis ajuster les poignées Scene.",
            MessageType.Info);

        if (GUILayout.Button("Init from grid AABB"))
            ApplyFit(bounds, bounds.FitFromGrid, "Fit view bounds from grid");

        if (GUILayout.Button("Init from IBC sprite"))
            ApplyFit(bounds, bounds.FitFromIbcSprite, "Fit view bounds from IBC");
    }

    private void OnSceneGUI()
    {
        var bounds = (BiofiltreViewBounds)target;
        using (new Handles.DrawingScope(new Color(1f, 0.45f, 0.12f, 1f), bounds.transform.localToWorldMatrix))
        {
            boxHandle.center = bounds.LocalCenter;
            boxHandle.size = new Vector3(bounds.LocalSize.x, bounds.LocalSize.y, 0f);
            EditorGUI.BeginChangeCheck();
            boxHandle.DrawHandle();
            if (!EditorGUI.EndChangeCheck())
                return;

            Undo.RecordObject(bounds, "Resize biofiltre view bounds");
            Vector3 handleSize = boxHandle.size;
            bounds.SetLocalRect(boxHandle.center, new Vector2(handleSize.x, handleSize.y));
            EditorUtility.SetDirty(bounds);
        }
    }

    private static void ApplyFit(BiofiltreViewBounds bounds, System.Action fit, string undoLabel)
    {
        Undo.RecordObject(bounds, undoLabel);
        fit();
        EditorUtility.SetDirty(bounds);
    }
}
