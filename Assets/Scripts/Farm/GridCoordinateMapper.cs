using UnityEngine;

/// <summary>Projection monde ↔ cellule. Orthogonal = actuel ; Isometric = losanges (futur).</summary>
public enum GridCoordinateMode
{
    Orthogonal = 0,
    Isometric  = 1,
}

/// <summary>Paramètres runtime partagés par tous les mappers.</summary>
public readonly struct GridLayoutSnapshot
{
    public GridLayoutSnapshot(Vector2 worldOrigin, Vector2 cellSizeWorld, int columns, int rows)
    {
        WorldOrigin   = worldOrigin;
        CellSizeWorld = cellSizeWorld;
        Columns       = columns;
        Rows          = rows;
    }

    public Vector2 WorldOrigin   { get; }
    public Vector2 CellSizeWorld { get; }
    public int     Columns       { get; }
    public int     Rows          { get; }
}

/// <summary>
/// Conversion monde ↔ (col, row) sans physique.
/// Modèle CodeMonkey : une seule responsabilité, swappable pour l'iso.
/// </summary>
public interface IGridCoordinateMapper
{
    Vector2Int WorldToCell(Vector2 worldPosition);
    Vector2      CellToWorldTopLeft(Vector2Int cell);
    Vector2      CellToWorldCenter(Vector2Int cell);
}

/// <summary>Grille rectangulaire : Floor sur X/Y local (row 0 en haut).</summary>
public sealed class OrthogonalGridCoordinateMapper : IGridCoordinateMapper
{
    readonly Vector2 _origin;
    readonly Vector2 _cellSize;

    public OrthogonalGridCoordinateMapper(GridLayoutSnapshot layout)
    {
        _origin   = layout.WorldOrigin;
        _cellSize = layout.CellSizeWorld;
    }

    public Vector2Int WorldToCell(Vector2 worldPosition)
    {
        Vector2 local = worldPosition - _origin;
        return new Vector2Int(
            Mathf.FloorToInt(local.x / _cellSize.x),
            Mathf.FloorToInt(-local.y / _cellSize.y)
        );
    }

    public Vector2 CellToWorldTopLeft(Vector2Int cell) =>
        _origin + new Vector2(cell.x * _cellSize.x, -cell.y * _cellSize.y);

    public Vector2 CellToWorldCenter(Vector2Int cell) =>
        CellToWorldTopLeft(cell) + new Vector2(_cellSize.x * 0.5f, -_cellSize.y * 0.5f);
}

/// <summary>
/// Grille en losanges (vue isométrique 2D). Formules standard col/row ↔ monde.
/// Le visuel des cellules (BiofiltreGridVisualizer) reste carré tant qu'il n'est pas adapté.
/// </summary>
public sealed class IsometricGridCoordinateMapper : IGridCoordinateMapper
{
    readonly Vector2 _origin;
    readonly float   _halfWidth;
    readonly float   _halfHeight;

    public IsometricGridCoordinateMapper(GridLayoutSnapshot layout)
    {
        _origin     = layout.WorldOrigin;
        _halfWidth  = layout.CellSizeWorld.x * 0.5f;
        _halfHeight = layout.CellSizeWorld.y * 0.5f;
    }

    public Vector2Int WorldToCell(Vector2 worldPosition)
    {
        float localX = worldPosition.x - _origin.x;
        float localY = worldPosition.y - _origin.y;

        float colF = (localX / _halfWidth + localY / _halfHeight) * 0.5f;
        float rowF = (localY / _halfHeight - localX / _halfWidth) * 0.5f;

        return new Vector2Int(Mathf.FloorToInt(colF), Mathf.FloorToInt(rowF));
    }

    public Vector2 CellToWorldTopLeft(Vector2Int cell)
    {
        float x = _origin.x + (cell.x - cell.y) * _halfWidth;
        float y = _origin.y - (cell.x + cell.y) * _halfHeight;
        return new Vector2(x, y);
    }

    public Vector2 CellToWorldCenter(Vector2Int cell)
    {
        Vector2 topLeft = CellToWorldTopLeft(cell);
        return topLeft + new Vector2(0f, -_halfHeight);
    }
}

public static class GridCoordinateMapperFactory
{
    public static IGridCoordinateMapper Create(GridCoordinateMode mode, GridLayoutSnapshot layout) =>
        mode switch
        {
            GridCoordinateMode.Isometric => new IsometricGridCoordinateMapper(layout),
            _                            => new OrthogonalGridCoordinateMapper(layout),
        };
}
