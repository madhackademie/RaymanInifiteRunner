using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pure C# runtime grid. Stores cell states as bytes: 0 = free, 1 = occupied.
/// Row 0 is the top row; rows increase downward.
/// </summary>
public class GridData
{
    public const byte Free     = 0;
    public const byte Occupied = 1;

    public int Columns { get; }
    public int Rows    { get; }

    private readonly byte[,] cells;

    public GridData(int columns, int rows)
    {
        Columns = columns;
        Rows    = rows;
        cells   = new byte[columns, rows];
    }

    // ── Bounds ────────────────────────────────────────────────────────────────

    /// <summary>Returns true if the cell is within grid bounds.</summary>
    public bool IsInBounds(Vector2Int cell) =>
        cell.x >= 0 && cell.x < Columns &&
        cell.y >= 0 && cell.y < Rows;

    // ── State queries ─────────────────────────────────────────────────────────

    /// <summary>Returns true if the cell is free (value == 0). Out-of-bounds cells are never free.</summary>
    public bool IsFree(Vector2Int cell) =>
        IsInBounds(cell) && cells[cell.x, cell.y] == Free;

    /// <summary>Returns true if the cell is occupied. Out-of-bounds cells are always considered occupied.</summary>
    public bool IsOccupied(Vector2Int cell) => !IsFree(cell);

    /// <summary>Returns true if every cell in the collection is free and in bounds.</summary>
    public bool AreAllFree(IEnumerable<Vector2Int> cellList)
    {
        foreach (Vector2Int cell in cellList)
        {
            if (!IsFree(cell))
                return false;
        }
        return true;
    }

    // ── State mutations ───────────────────────────────────────────────────────

    /// <summary>Marks a cell as occupied. Silently ignored if out of bounds.</summary>
    public void SetOccupied(Vector2Int cell)
    {
        if (IsInBounds(cell))
            cells[cell.x, cell.y] = Occupied;
    }

    /// <summary>Marks a cell as free. Silently ignored if out of bounds.</summary>
    public void SetFree(Vector2Int cell)
    {
        if (IsInBounds(cell))
            cells[cell.x, cell.y] = Free;
    }

    /// <summary>Marks every cell in the collection as occupied.</summary>
    public void SetOccupied(IEnumerable<Vector2Int> cellList)
    {
        foreach (Vector2Int cell in cellList)
            SetOccupied(cell);
    }

    /// <summary>Marks every cell in the collection as free.</summary>
    public void SetFree(IEnumerable<Vector2Int> cellList)
    {
        foreach (Vector2Int cell in cellList)
            SetFree(cell);
    }

    /// <summary>Resets all cells to free.</summary>
    public void Clear()
    {
        System.Array.Clear(cells, 0, cells.Length);
    }

    /// <summary>Returns the raw state byte of a cell (0 or 1). Returns Occupied if out of bounds.</summary>
    public byte GetRaw(Vector2Int cell) =>
        IsInBounds(cell) ? cells[cell.x, cell.y] : Occupied;
}
