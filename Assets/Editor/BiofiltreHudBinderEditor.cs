using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector HUD biofiltre — rows posées à la main (plus d’instantiate).
/// </summary>
[CustomEditor(typeof(BiofiltreHudBinder))]
public class BiofiltreHudBinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox(
            "HUD = enfant BiofiltreHud. Grille = enfant Grid (Move / Scale).\n" +
            "IbcSprite / rows HUD : mêmes outils Unity. Play ne recale plus.",
            MessageType.Info);
    }
}
