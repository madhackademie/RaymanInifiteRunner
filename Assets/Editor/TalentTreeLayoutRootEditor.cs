using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TalentTreeLayoutRoot))]
public class TalentTreeLayoutRootEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var root = (TalentTreeLayoutRoot)target;
        EditorGUILayout.Space(8f);

        if (GUILayout.Button("Collect child nodes"))
            root.CollectNodeViewsFromChildren();

        if (GUILayout.Button("Collect child edges"))
            root.CollectEdgeViewsFromChildren();

        if (GUILayout.Button("Validate edges vs prerequisites (warning)"))
            root.ValidateEdgesAgainstDefinitions(logWarnings: true);
    }
}
