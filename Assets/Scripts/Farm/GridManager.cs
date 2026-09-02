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

    [Header("Coordinate projection")]
    [Tooltip("Orthogonal = carrés. Isometric = losanges 2:1 (hauteur cellule = largeur × 0.5 si taille uniforme).")]
    [SerializeField] private GridCoordinateMode coordinateMode = GridCoordinateMode.Orthogonal;

    /// <summary>Ratio hauteur/largeur d'une cellule iso en taille uniforme (losange jeu classique).</summary>
    private const float IsometricHeightRatio = 0.5f;

    public GridData Grid { get; private set; }

    // Maps any occupied cell to the root GameObject of the plant that occupies it.
    private readonly Dictionary<Vector2Int, GameObject> _plantByCell = new();

    private int _columns;
    private int _rows;
    private Vector2 _cellSizeWorld;
    private Vector2 _worldOrigin;
    private IGridCoordinateMapper _coordinateMapper;

    public int Columns => _columns;
    public int Rows => _rows;

    /// <summary>Mode de projection actif (orthogonal ou isométrique).</summary>
    public GridCoordinateMode CoordinateMode => coordinateMode;

    /// <summary>Cell extent in world units (X = width, Y = height along grid rows).</summary>
    public Vector2 CellSizeWorld => _cellSizeWorld;

    /// <summary>Uniform cell size when width == height; otherwise Max for quick probes.</summary>
    public float CellSizeUniform => Mathf.Max(_cellSizeWorld.x, _cellSizeWorld.y);

    public Vector2 WorldOrigin => _worldOrigin;

    /// <summary>
    /// AABB monde de la grille entière (coin bas-gauche).
    /// Orthogonal : rectangle cols×rows. Isometric : enveloppe des losanges.
    /// </summary>
    public Rect GetWorldRect()
    {
        IGridCoordinateMapper mapper = GetOrCreateMapper(out int cols, out int rows);
        Vector2[] corners = new Vector2[4];
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        EncapsulateCell(mapper, new Vector2Int(0, 0), corners, ref minX, ref minY, ref maxX, ref maxY);
        EncapsulateCell(mapper, new Vector2Int(cols - 1, 0), corners, ref minX, ref minY, ref maxX, ref maxY);
        EncapsulateCell(mapper, new Vector2Int(0, rows - 1), corners, ref minX, ref minY, ref maxX, ref maxY);
        EncapsulateCell(mapper, new Vector2Int(cols - 1, rows - 1), corners, ref minX, ref minY, ref maxX, ref maxY);

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    /// <summary>4 coins monde de la cellule, winding horaire.</summary>
    public void GetCellWorldCorners(Vector2Int cell, Vector2[] corners)
    {
        GetOrCreateMapper(out _, out _).GetCellCorners(cell, corners);
    }

    private void Awake()
    {
        ResolveLayout(out _columns, out _rows, out _cellSizeWorld, out _worldOrigin);

        if (_columns < 1 || _rows < 1)
        {
            Debug.LogError("[GridManager] Invalid grid dimensions.", this);
            return;
        }

        Grid = new GridData(_columns, _rows);
        _coordinateMapper = GridCoordinateMapperFactory.Create(coordinateMode, BuildLayoutSnapshot());
    }

    private GridLayoutSnapshot BuildLayoutSnapshot() =>
        new(_worldOrigin, _cellSizeWorld, _columns, _rows);

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

        if (coordinateMode == GridCoordinateMode.Isometric && IsUniformCellSize(cellSizeWorld))
            cellSizeWorld = new Vector2(cellSizeWorld.x, cellSizeWorld.x * IsometricHeightRatio);
    }

    private static bool IsUniformCellSize(Vector2 cellSizeWorld) =>
        Mathf.Abs(cellSizeWorld.x - cellSizeWorld.y) < 0.001f;

    private IGridCoordinateMapper GetOrCreateMapper(out int cols, out int rows)
    {
        if (_coordinateMapper != null)
        {
            cols = _columns;
            rows = _rows;
            return _coordinateMapper;
        }

        ResolveLayout(out cols, out rows, out Vector2 cellSz, out Vector2 origin);
        return GridCoordinateMapperFactory.Create(
            coordinateMode,
            new GridLayoutSnapshot(origin, cellSz, cols, rows));
    }

    private static void EncapsulateCell(
        IGridCoordinateMapper mapper,
        Vector2Int cell,
        Vector2[] corners,
        ref float minX,
        ref float minY,
        ref float maxX,
        ref float maxY)
    {
        mapper.GetCellCorners(cell, corners);
        for (int i = 0; i < 4; i++)
        {
            minX = Mathf.Min(minX, corners[i].x);
            minY = Mathf.Min(minY, corners[i].y);
            maxX = Mathf.Max(maxX, corners[i].x);
            maxY = Mathf.Max(maxY, corners[i].y);
        }
    }

    // ── Coordinate conversion ─────────────────────────────────────────────────

    /// <summary>
    /// Converts a grid cell (col, row) to the world position of its top-left corner.
    /// Row 0 is the topmost row; rows increase downward (Y decreases in world space).
    /// </summary>
    public Vector2 GridToWorld(Vector2Int cell) =>
        _coordinateMapper.CellToWorldTopLeft(cell);

    /// <summary>Converts a grid cell to the world position of its center.</summary>
    public Vector2 GridToWorldCenter(Vector2Int cell) =>
        _coordinateMapper.CellToWorldCenter(cell);

    /// <summary>
    /// Geometric world center of a footprint (average of occupied cell centers).
    /// A 1×1 footprint equals <see cref="GridToWorldCenter"/>.
    /// </summary>
    public Vector2 GetFootprintWorldCenter(Vector2Int anchor, Vector2Int[] footprint)
    {
        if (footprint == null || footprint.Length == 0)
            return GridToWorldCenter(anchor);

        Vector2 sum = Vector2.zero;
        for (int i = 0; i < footprint.Length; i++)
            sum += GridToWorldCenter(anchor + footprint[i]);

        return sum / footprint.Length;
    }

    /// <summary>
    /// Converts a world position to the grid cell (floor). May be out of bounds — use IsInBounds.
    /// </summary>
    public Vector2Int WorldToGrid(Vector2 worldPos) =>
        _coordinateMapper.WorldToCell(worldPos);

    /// <summary>Écran/monde → cellule si dans les limites (clic grille oldschool).</summary>
    public bool TryWorldToCell(Vector2 worldPosition, out Vector2Int cell)
    {
        cell = WorldToGrid(worldPosition);
        return IsInBounds(cell);
    }

    // ── Convenience wrappers (delegates to GridData) ──────────────────────────

    /// <summary>Returns true if the cell is within grid bounds.</summary>
    public bool IsInBounds(Vector2Int cell) => Grid != null && Grid.IsInBounds(cell);

    /// <summary>Returns true if the cell is free and in bounds.</summary>
    public bool IsCellFree(Vector2Int cell) => Grid != null && Grid.IsFree(cell);

    /// <summary>Returns true if every cell in the collection is free and in bounds.</summary>
    public bool AreAllCellsFree(IEnumerable<Vector2Int> cells) => Grid != null && Grid.AreAllFree(cells);

    /// <summary>Marks a collection of cells as occupied.</summary>
    public void OccupyCells(IEnumerable<Vector2Int> cells)
    {
        if (Grid != null) Grid.SetOccupied(cells);
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

    /// <summary>
    /// Remise a zero runtime de la grille (occupation + registre plantes).
    /// </summary>
    public void ResetRuntimeState()
    {
        Grid?.Clear();
        _plantByCell.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        ResolveLayout(out int cols, out int rows, out Vector2 cellSz, out Vector2 origin);
        var snapshot = new GridLayoutSnapshot(origin, cellSz, cols, rows);
        IGridCoordinateMapper mapper = GridCoordinateMapperFactory.Create(coordinateMode, snapshot);

        for (int col = 0; col < cols; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector2Int cell     = new(col, row);
                bool       occupied = Grid != null && Grid.IsInBounds(cell) && Grid.IsOccupied(cell);

                Color fillColor    = occupied
                    ? new Color(1f, 0.2f, 0.2f, 0.4f)
                    : new Color(0f, 1f, 0.4f, 0.15f);
                Color outlineColor = new Color(0f, 1f, 0.4f, 0.5f);

                if (coordinateMode == GridCoordinateMode.Isometric)
                    DrawIsoCellGizmo(mapper, cell, cellSz, fillColor, outlineColor);
                else
                    DrawOrthoCellGizmo(mapper, cell, cellSz, fillColor, outlineColor);
            }
        }
    }

    private static void DrawOrthoCellGizmo(
        IGridCoordinateMapper mapper, Vector2Int cell, Vector2 cellSz,
        Color fillColor, Color outlineColor)
    {
        Vector2 topLeft = mapper.CellToWorldTopLeft(cell);
        float inset = 0.025f;

        Vector3[] fillCorners =
        {
            new(topLeft.x + inset,            topLeft.y - inset,            0f),
            new(topLeft.x + cellSz.x - inset, topLeft.y - inset,            0f),
            new(topLeft.x + cellSz.x - inset, topLeft.y - cellSz.y + inset, 0f),
            new(topLeft.x + inset,            topLeft.y - cellSz.y + inset, 0f),
        };

        Vector3[] outlineCorners =
        {
            new(topLeft.x,            topLeft.y,            0f),
            new(topLeft.x + cellSz.x, topLeft.y,            0f),
            new(topLeft.x + cellSz.x, topLeft.y - cellSz.y, 0f),
            new(topLeft.x,            topLeft.y - cellSz.y, 0f),
        };

        UnityEditor.Handles.DrawSolidRectangleWithOutline(fillCorners, fillColor, Color.clear);
        UnityEditor.Handles.DrawSolidRectangleWithOutline(outlineCorners, Color.clear, outlineColor);
    }

    private static void DrawIsoCellGizmo(
        IGridCoordinateMapper mapper, Vector2Int cell, Vector2 cellSz,
        Color fillColor, Color outlineColor)
    {
        Vector2 center = mapper.CellToWorldCenter(cell);
        float hw = cellSz.x * 0.5f;
        float hh = cellSz.y * 0.5f;

        Vector3[] corners =
        {
            new(center.x,     center.y + hh, 0f),
            new(center.x + hw, center.y,     0f),
            new(center.x,     center.y - hh, 0f),
            new(center.x - hw, center.y,     0f),
        };

        UnityEditor.Handles.DrawSolidRectangleWithOutline(corners, fillColor, outlineColor);
    }
#endif
}
