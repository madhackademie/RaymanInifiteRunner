using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Owns and exposes the runtime GridData instance.
/// Handles grid ↔ world coordinate conversion (row 0 = top, rows increase downward).
/// Layout can come from a shared <see cref="GridConfig"/> or from per-instance fields (biofiltre, parcelles).
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Layout source")]
    [Tooltip("If true, columns / rows / cell size / uniform flag come from GridConfig.")]
    [SerializeField] private bool useScriptableConfig = true;

    [SerializeField] private GridConfig config;

    [Header("Instance layout (when Use Scriptable Config is off)")]
    [Min(1)] [SerializeField] private int instanceColumns = 10;
    [Min(1)] [SerializeField] private int instanceRows    = 10;

    [SerializeField] private bool instanceUniformCellSize = true;
    [Min(0.01f)] [SerializeField] private float instanceCellSize = 1f;
    [Min(0.01f)] [SerializeField] private float instanceCellWidth  = 1f;
    [Min(0.01f)] [SerializeField] private float instanceCellHeight = 1f;

    [Header("World origin (top-left of cell 0,0)")]
    [Tooltip("If true, origin = this transform's X/Y + Origin Offset. If false with Scriptable Config, uses GridConfig.origin.")]
    [SerializeField] private bool originFromTransform = false;

    [SerializeField] private Vector2 originOffset = Vector2.zero;

    [Tooltip("Used when Origin From Transform is false and Use Scriptable Config is false.")]
    [SerializeField] private Vector2 instanceWorldOrigin = Vector2.zero;

    public GridData Grid { get; private set; }

    // Maps any occupied cell to the root GameObject of the plant that occupies it.
    private readonly Dictionary<Vector2Int, GameObject> _plantByCell = new();

    private int _columns;
    private int _rows;
    private Vector2 _cellSizeWorld;
    private Vector2 _worldOrigin;

    public int Columns => _columns;
    public int Rows => _rows;

    /// <summary>Cell extent in world units (X = width, Y = height along grid rows).</summary>
    public Vector2 CellSizeWorld => _cellSizeWorld;

    /// <summary>Uniform cell size when width == height; otherwise Max for quick probes.</summary>
    public float CellSizeUniform => Mathf.Max(_cellSizeWorld.x, _cellSizeWorld.y);

    public Vector2 WorldOrigin => _worldOrigin;

    private void Awake()
    {
        ResolveLayout(out _columns, out _rows, out _cellSizeWorld, out _worldOrigin);

        if (_columns < 1 || _rows < 1)
        {
            Debug.LogError("[GridManager] Invalid grid dimensions.", this);
            return;
        }

        Grid = new GridData(_columns, _rows);
    }

    private void ResolveLayout(out int columns, out int rows, out Vector2 cellSizeWorld, out Vector2 worldOrigin)
    {
        if (useScriptableConfig)
        {
            if (config == null)
            {
                Debug.LogError("[GridManager] Use Scriptable Config is on but no GridConfig assigned.", this);
                columns = 1;
                rows = 1;
                cellSizeWorld = Vector2.one;
                worldOrigin = (Vector2)transform.position + originOffset;
                return;
            }

            columns = config.columns;
            rows = config.rows;
            cellSizeWorld = config.GetCellSizeWorld();
            worldOrigin = originFromTransform
                ? (Vector2)transform.position + originOffset
                : config.origin + originOffset;
        }
        else
        {
            columns = instanceColumns;
            rows = instanceRows;
            cellSizeWorld = instanceUniformCellSize
                ? new Vector2(instanceCellSize, instanceCellSize)
                : new Vector2(instanceCellWidth, instanceCellHeight);
            worldOrigin = originFromTransform
                ? (Vector2)transform.position + originOffset
                : instanceWorldOrigin + originOffset;
        }
    }

    // ── Coordinate conversion ─────────────────────────────────────────────────

    /// <summary>
    /// Converts a grid cell (col, row) to the world position of its top-left corner.
    /// Row 0 is the topmost row; rows increase downward (Y decreases in world space).
    /// </summary>
    public Vector2 GridToWorld(Vector2Int cell)
    {
        return _worldOrigin + new Vector2(
            cell.x * _cellSizeWorld.x,
            -cell.y * _cellSizeWorld.y
        );
    }

    /// <summary>
    /// Converts a grid cell to the world position of its center.
    /// </summary>
    public Vector2 GridToWorldCenter(Vector2Int cell)
    {
        return GridToWorld(cell) + new Vector2(_cellSizeWorld.x * 0.5f, -_cellSizeWorld.y * 0.5f);
    }

    /// <summary>
    /// Converts a world position to the grid cell (floor). May be out of bounds — use IsInBounds.
    /// </summary>
    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        Vector2 local = worldPos - _worldOrigin;
        return new Vector2Int(
            Mathf.FloorToInt(local.x / _cellSizeWorld.x),
            Mathf.FloorToInt(-local.y / _cellSizeWorld.y)
        );
    }

    // ── Convenience wrappers (delegates to GridData) ──────────────────────────

    /// <summary>Returns true if the cell is within grid bounds.</summary>
    public bool IsInBounds(Vector2Int cell) => Grid != null && Grid.IsInBounds(cell);

    /// <summary>Returns true if the cell is free and in bounds.</summary>
    public bool IsCellFree(Vector2Int cell) => Grid != null && Grid.IsFree(cell);

    /// <summary>Returns true if every cell in the collection is free and in bounds.</summary>
    public bool AreAllCellsFree(IEnumerable<Vector2Int> cells) => Grid != null && Grid.AreAllFree(cells);

    /// <summary>Marks a cell as occupied.</summary>
    public void OccupyCell(Vector2Int cell)
    {
        if (Grid != null) Grid.SetOccupied(cell);
    }

    /// <summary>Marks a collection of cells as occupied.</summary>
    public void OccupyCells(IEnumerable<Vector2Int> cells)
    {
        if (Grid != null) Grid.SetOccupied(cells);
    }

    /// <summary>Marks a cell as free.</summary>
    public void FreeCell(Vector2Int cell)
    {
        if (Grid != null) Grid.SetFree(cell);
    }

    /// <summary>Marks a collection of cells as free.</summary>
    public void FreeCells(IEnumerable<Vector2Int> cells)
    {
        if (Grid != null) Grid.SetFree(cells);
    }

    // ── Plant registry ────────────────────────────────────────────────────────

    /// <summary>
    /// Registers a plant GameObject against all cells of its footprint.
    /// Called by BiofiltreManager right after instantiation.
    /// </summary>
    public void RegisterPlant(IEnumerable<Vector2Int> cells, GameObject plant)
    {
        foreach (Vector2Int cell in cells)
            _plantByCell[cell] = plant;
    }

    /// <summary>
    /// Returns the plant GameObject occupying the given cell, or null if none.
    /// O(1) lookup — no spatial search.
    /// </summary>
    public GameObject GetPlantAt(Vector2Int cell) =>
        _plantByCell.TryGetValue(cell, out GameObject plant) ? plant : null;

    /// <summary>
    /// Removes the plant registry entries for the given cells.
    /// Call this when a plant is harvested or destroyed.
    /// </summary>
    public void UnregisterPlant(IEnumerable<Vector2Int> cells)
    {
        foreach (Vector2Int cell in cells)
            _plantByCell.Remove(cell);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        ResolveLayout(out int cols, out int rows, out Vector2 cellSz, out Vector2 origin);

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector2Int cell    = new Vector2Int(col, row);
                Vector2    topLeft = origin + new Vector2(col * cellSz.x, -row * cellSz.y);
                bool       occupied = Grid != null && Grid.IsInBounds(cell) && Grid.IsOccupied(cell);

                Color fillColor = occupied
                    ? new Color(1f, 0.2f, 0.2f, 0.4f)
                    : new Color(0f, 1f, 0.4f, 0.15f);

                Color outlineColor = new Color(0f, 1f, 0.4f, 0.5f);

                // Inset the fill slightly so the outline stays visible
                float inset = 0.025f;
                Vector3[] fillCorners =
                {
                    new Vector3(topLeft.x + inset,            topLeft.y - inset,            0f),
                    new Vector3(topLeft.x + cellSz.x - inset, topLeft.y - inset,            0f),
                    new Vector3(topLeft.x + cellSz.x - inset, topLeft.y - cellSz.y + inset, 0f),
                    new Vector3(topLeft.x + inset,            topLeft.y - cellSz.y + inset, 0f),
                };

                Vector3[] outlineCorners =
                {
                    new Vector3(topLeft.x,            topLeft.y,            0f),
                    new Vector3(topLeft.x + cellSz.x, topLeft.y,            0f),
                    new Vector3(topLeft.x + cellSz.x, topLeft.y - cellSz.y, 0f),
                    new Vector3(topLeft.x,            topLeft.y - cellSz.y, 0f),
                };

                UnityEditor.Handles.DrawSolidRectangleWithOutline(fillCorners,    fillColor,    Color.clear);
                UnityEditor.Handles.DrawSolidRectangleWithOutline(outlineCorners, Color.clear,  outlineColor);
            }
        }
    }
#endif
}
