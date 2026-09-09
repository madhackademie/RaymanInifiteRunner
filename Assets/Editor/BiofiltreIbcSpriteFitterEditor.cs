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
            "Layout : BiofiltreLayoutDefinition (colonnes, cellSize, ibcSprite, deck UV).\n" +
            "Sprite : enfant IbcSprite — Move / Rotate / Scale (calage visuel manuel).\n" +
            "Grille : enfant Grid — Move / Scale (contraindre les proportions).\n" +
            "Iso : Fit once = point de départ AABB, pas fiable seul — playtest + calage main.\n" +
            "Prompt art deck : Notes/Art/PROMPT_ibc_deck_template.md",
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
