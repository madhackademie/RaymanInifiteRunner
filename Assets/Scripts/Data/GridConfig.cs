using UnityEngine;

/// <summary>
/// Design-time configuration for a farm grid.
/// Defines its dimensions, cell size, and world origin (top-left corner).
/// </summary>
[CreateAssetMenu(menuName = "Game/Grid/Grid Config", fileName = "GridConfig")]
public class GridConfig : ScriptableObject
{
    [Header("Dimensions")]
    [Min(1)] public int columns = 10;
    [Min(1)] public int rows    = 10;

    [Header("Cell size (world units)")]
    [Tooltip("If true, use Cell Size (uniform). If false, use Cell Width / Cell Height.")]
    public bool uniformCellSize = true;

    [Min(0.01f)] public float cellSize = 1f;

    [Min(0.01f)] public float cellWidth  = 1f;
    [Min(0.01f)] public float cellHeight = 1f;

    [Header("World")]
    [Tooltip("World position of the top-left corner of the grid (cell 0,0). Ignored if the GridManager uses transform as origin.")]
    public Vector2 origin = Vector2.zero;

    /// <summary>Returns cell extent in world units: X = width, Y = height (depth on screen).</summary>
    public Vector2 GetCellSizeWorld()
    {
        if (uniformCellSize)
            return new Vector2(cellSize, cellSize);
        return new Vector2(cellWidth, cellHeight);
    }
}
