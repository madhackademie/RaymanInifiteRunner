using UnityEngine;

/// <summary>
/// Définition partagée d'un biofiltre : grille gameplay + art IBC (deck UV).
/// Une variante = un asset (petit 10×10, grand 24×18, etc.) avec la même cellSize canonique.
/// Le calage visuel Grid / IbcSprite reste sur le prefab (transforms enfants).
/// </summary>
[CreateAssetMenu(menuName = "Game/Data/Ferme/Biofiltre layout", fileName = "BiofiltreLayout")]
public class BiofiltreLayoutDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Stable id for save / factory map (e.g. biofiltre_standard_10).")]
    public string layoutId = "biofiltre_standard";

    public string displayName = "Biofiltre";

    [Header("Grid (gameplay)")]
    [Min(1)] public int columns = 10;
    [Min(1)] public int rows    = 10;

    [Tooltip("Largeur de cellule en unités monde (hauteur iso = ×0.5 si uniforme).")]
    [Min(0.01f)] public float cellSize = 1f;

    public GridCoordinateMode coordinateMode = GridCoordinateMode.Isometric;

    [Header("IBC art")]
    [Tooltip("Sprite promu Sprites/Farm/Biofiltre/, pas le Dump.")]
    public Sprite ibcSprite;

    [Tooltip("Zone billes plantables en UV 0–1 (origine bas-gauche du sprite). Mesurer par asset.")]
    public Rect deckNormalized = new Rect(0.059f, 0.5239f, 0.8844f, 0.441f);

    [Header("IBC display")]
    public int ibcSortingOrder = -1;
}
