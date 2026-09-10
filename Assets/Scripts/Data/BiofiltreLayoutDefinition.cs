using UnityEngine;

/// <summary>
/// Définition partagée d'un biofiltre : grille gameplay + art IBC.
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

    [Header("Deck shape (UV)")]
    [Tooltip("Quad logique sur le sprite IBC (UV 0–1, bas-gauche). Bake B → columns/rows.")]
    public BiofiltreDeckShapeUv deckShapeUv;

    [Header("IBC art")]
    [Tooltip("Sprite promu Sprites/Farm/Biofiltre/, pas le Dump.")]
    public Sprite ibcSprite;

    [Tooltip("Legacy : bouton Fit once seulement. Préférer deckShapeUv + bake sur le prefab.")]
    public Rect deckNormalized = new Rect(0.059f, 0.5239f, 0.8844f, 0.441f);

    [Header("IBC display")]
    public int ibcSortingOrder = -1;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!deckShapeUv.IsValid() && deckNormalized.width > 0.01f && deckNormalized.height > 0.01f)
            deckShapeUv.SetFromNormalizedRect(deckNormalized);
    }

    [ContextMenu("Sync deck shape from deckNormalized rect")]
    private void SyncShapeFromLegacyRect()
    {
        deckShapeUv.SetFromNormalizedRect(deckNormalized);
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
