using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BiofiltreLayoutDefinition))]
public class BiofiltreLayoutDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var layout = (BiofiltreLayoutDefinition)target;

        EditorGUILayout.Space(6);
        if (GUILayout.Button("Copier deckNormalized → deckShapeUv (rect)"))
        {
            Undo.RecordObject(layout, "Sync deck shape UV");
            layout.deckShapeUv.SetFromNormalizedRect(layout.deckNormalized);
            EditorUtility.SetDirty(layout);
        }

        if (layout.deckShapeUv.IsValid())
        {
            Rect bounds = layout.deckShapeUv.GetBoundingRect();
            EditorGUILayout.LabelField(
                "BBox UV",
                $"x={bounds.x:F3} y={bounds.y:F3} w={bounds.width:F3} h={bounds.height:F3}");
        }

        EditorGUILayout.HelpBox(
            "Bake columns/rows : sélectionner le prefab Biofiltre en scène → BiofiltreLayoutBinder → Bake B.",
            MessageType.None);
    }
}
