using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector IBC — sprite et grille se règlent à la main (Move / Scale Unity).
/// </summary>
[CustomEditor(typeof(BiofiltreIbcSpriteFitter))]
public class BiofiltreIbcSpriteFitterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox(
            "Sprite : enfant IbcSprite — Move / Rotate / Scale.\n" +
            "Grille : enfant Grid — Move / Scale (contraindre les proportions).\n" +
            "Iso 2:1 = forme des cellules, pas un auto-calage sur la cuve.\n" +
            "Le Play ne recale plus sprite ni grille.",
            MessageType.Info);

        if (GUILayout.Button("Fit sprite to grid (once)"))
        {
            var fitter = (BiofiltreIbcSpriteFitter)target;
            Undo.RecordObject(fitter, "Fit biofiltre sprite");
            fitter.FitToGrid();
            EditorUtility.SetDirty(fitter);
        }
    }
}
