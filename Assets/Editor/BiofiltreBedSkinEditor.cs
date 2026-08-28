using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BiofiltreBedSkin))]
public class BiofiltreBedSkinEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (targets == null || targets.Length == 0 || target == null)
            return;

        EditorGUILayout.HelpBox(
            "Sprite du bac seulement. La grille (colonnes / cell size) reste sur GridManager.\n" +
            "Echelle et offset se règlent sur BiofiltreGridVisualizer (par instance, taille fixe).",
            MessageType.Info);
        DrawDefaultInspector();
    }
}
