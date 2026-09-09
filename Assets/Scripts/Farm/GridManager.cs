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

    [Tooltip("Décale la grille en cellules (sprite IBC ne suit pas). +Y = nord. Fractions OK.")]
    [SerializeField] private Vector2 originShiftCells = Vector2.zero;

    [Tooltip("Used when Origin From Transform is false and Use Scriptable Config is false.")]
    [SerializeField] private Vector2 instanceWorldOrigin = Vector2.zero;

    [Tooltip("Enfant Grid : Move / Scale Unity. Null = ancien calage (transform racine + originShift).")]
    [SerializeField] private Transform layoutTransform;

    [Header("Coordinate projection")]
    [Tooltip("Orthogonal = carrés. Isometric = losanges 2:1 (hauteur cellule = largeur × 0.5 si taille uniforme).")]
    [SerializeField] private GridCoordinateMode coordinateMode = GridCoordinateMode.Orthogonal;

    /// <summary>Ratio hauteur/largeur d'une cellule iso en taille uniforme (losange jeu classique).</summary>
    private const float IsometricHeightRatio = 0.5f;

    public GridData Grid { get; private set; }

    // Maps any occupied cell to the root GameObject of the plant that occupies it.
    private readonly Dictionary<Vector2Int, GameObject> _plantByCell = new();
    private readonly List<GameObject> _plantHitBuffer = new();
    private static readonly Vector2[] CellCornerBuffer = new Vector2[4];

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

    /// <summary>
    /// Ordre de dessin : plus <c>col+row</c> = plus devant (iso). Orthogonal : même formule.
    /// </summary>
    public int GetCellDrawOrder(Vector2Int cell, int baseOrder = 0) =>
        baseOrder + cell.x + cell.y;

    public Vector2 WorldOrigin => _worldOrigin;

    /// <summary>Rebuild mapper after Inspector / Scene gizmos change cell size or shift.</summary>
    public void RebuildMapperFromInspector()
    {
        _coordinateMapper = null;
        ResolveLayout(out _columns, out _rows, out _cellSizeWorld, out _worldOrigin);
        if (_columns < 1 || _rows < 1)
            return;

        _coordinateMapper = GridCoordinateMapperFactory.Create(coordinateMode, BuildLayoutSnapshot());
    }

    /// <summary>
    /// AABB monde de la grille entière (coin bas-gauche).
    /// Orthogonal : rectangle cols×rows. Isometric : enveloppe des losanges.
    /// </summary>
    public Rect GetWorldRect()
    {
        IGridCoordinateMapper mapper = GetOrCreateMapper(out int cols, out int rows);
        return ComputeAabb(mapper, cols, rows);
    }

    /// <summary>
    /// AABB pour caler le sprite IBC : même taille que la grille, sans
    /// <see cref="originShiftCells"/> (la cuve ne suit pas le décalage).
    /// </summary>
    public Rect GetSpriteFitWorldRect()
    {
        Rect gridRect = GetWorldRect();
        Vector2 shift = GetOriginShiftWorld();
        return new Rect(gridRect.xMin - shift.x, gridRect.yMin - shift.y, gridRect.width, gridRect.height);
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

        ApplyLayoutHandle(ref cellSizeWorld, ref worldOrigin, columns, rows);
    }

    private void ApplyLayoutHandle(
        ref Vector2 cellSizeWorld, ref Vector2 worldOrigin, int columns, int rows)
    {
        bool useHandle = layoutTransform != null;
        if (useHandle)
        {
            float sizeMul = Mathf.Max(0.01f, Mathf.Abs(layoutTransform.lossyScale.x));
            cellSizeWorld *= sizeMul;
            worldOrigin = (Vector2)layoutTransform.position + originOffset;
        }

        if (coordinateMode == GridCoordinateMode.Isometric && IsUniformCellSize(cellSizeWorld))
            cellSizeWorld = new Vector2(cellSizeWorld.x, cellSizeWorld.x * IsometricHeightRatio);

        if (useHandle)
        {
            worldOrigin = CenterOriginOnHandle(worldOrigin, cellSizeWorld, columns, rows);
            return;
        }

        if (coordinateMode == GridCoordinateMode.Isometric)
            worldOrigin.x += columns * cellSizeWorld.x * 0.5f;

        worldOrigin += GetOriginShiftWorld(cellSizeWorld);
    }

    private Vector2 CenterOriginOnHandle(Vector2 origin, Vector2 cellSize, int columns, int rows)
    {
        var probe = GridCoordinateMapperFactory.Create(
            coordinateMode, new GridLayoutSnapshot(origin, cellSize, columns, rows));
        Vector2 aabbCenter = ComputeAabb(probe, columns, rows).center;
        return origin + (Vector2)layoutTransform.position - aabbCenter;
    }

    private static bool IsUniformCellSize(Vector2 cellSizeWorld) =>
        Mathf.Abs(cellSizeWorld.x - cellSizeWorld.y) < 0.001f;

    private Vector2 GetOriginShiftWorld()
    {
        ResolveLayout(out _, out _, out Vector2 cellSz, out _);
        return GetOriginShiftWorld(cellSz);
    }

    private Vector2 GetOriginShiftWorld(Vector2 cellSizeWorld) =>
        new(originShiftCells.x * cellSizeWorld.x, originShiftCells.y * cellSizeWorld.y);

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

    private static Rect ComputeAabb(IGridCoordinateMapper mapper, int cols, int rows)
    {
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
    /// Sommet central iso d'un footprint 2×2 standard ((0,0)…(1,1)) — point où les 4 losanges se rejoignent.
    /// </summary>
    public bool TryGetIsoFootprintHub(Vector2Int anchor, Vector2Int[] footprint, out Vector2 hub)
    {
        hub = default;

        if (coordinateMode != GridCoordinateMode.Isometric || !IsStandard2x2Footprint(footprint))
            return false;

        Vector2 north = GridToWorldCenter(anchor);
        Vector2 south = GridToWorldCenter(anchor + new Vector2Int(1, 1));
        hub = (north + south) * 0.5f;
        return true;
    }

    /// <summary>
    /// Position monde du pivot sprite (hub iso 2×2 si applicable, sinon centre footprint).
    /// </summary>
    public Vector2 GetPlantSpriteWorldPosition(Vector2Int anchor, PlantDefinition plant)
    {
        if (plant == null)
            return GridToWorldCenter(anchor);

        Vector2 basePos = TryGetIsoFootprintHub(anchor, plant.footprint, out Vector2 hub)
            ? hub
            : GetFootprintWorldCenter(anchor, plant.GetSpritePlacementOffsets());

        return basePos + plant.spriteWorldOffset + plant.isoSpriteViewOffset;
    }

    private static bool IsStandard2x2Footprint(Vector2Int[] footprint)
    {
        if (footprint == null || footprint.Length != 4)
            return false;

        return ContainsFootprintOffset(footprint, Vector2Int.zero) &&
               ContainsFootprintOffset(footprint, new Vector2Int(1, 0)) &&
               ContainsFootprintOffset(footprint, new Vector2Int(0, 1)) &&
               ContainsFootprintOffset(footprint, new Vector2Int(1, 1));
    }

    private static bool ContainsFootprintOffset(Vector2Int[] footprint, Vector2Int offset)
    {
        for (int i = 0; i < footprint.Length; i++)
        {
            if (footprint[i] == offset)
                return true;
        }

        return false;
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

    /// <summary>
    /// Résout la cellule cliquée : priorité plante (losanges footprint iso uniquement) puis sol.
    /// La canopée hors footprint ne bloque pas le clic (plante derrière ou cellule voisine).
    /// </summary>
    public bool TryResolveClickTarget(Vector2 worldPosition, out Vector2Int cell)
    {
        if (TryResolvePlantAtWorld(worldPosition, out Vector2Int plantCell))
        {
            cell = plantCell;
            return true;
        }

        return TryWorldToCell(worldPosition, out cell);
    }

    private bool TryResolvePlantAtWorld(Vector2 world, out Vector2Int cell)
    {
        cell = default;
        GameObject bestPlant = null;
        int bestDrawOrder = int.MinValue;

        CollectUniquePlants(_plantHitBuffer);
        for (int i = 0; i < _plantHitBuffer.Count; i++)
        {
            GameObject plant = _plantHitBuffer[i];
            if (!TryScorePlantHit(plant, world, out int drawOrder, out Vector2Int hitCell))
                continue;

            if (drawOrder <= bestDrawOrder)
                continue;

            bestDrawOrder = drawOrder;
            bestPlant     = plant;
            cell          = hitCell;
        }

        return bestPlant != null;
    }

    private bool TryScorePlantHit(GameObject plant, Vector2 world, out int drawOrder, out Vector2Int cell)
    {
        drawOrder = int.MinValue;
        cell      = default;

        // Footprint losanges only — sprite bounds (canopée / alpha) must not steal clicks
        // from neighbouring cells or plants behind.
        if (!TryGetPlantFootprintHit(plant, world, out Vector2Int footprintCell))
            return false;

        drawOrder = ComputePlantDrawOrder(plant);
        cell      = footprintCell;
        return true;
    }

    private bool TryGetPlantFootprintHit(GameObject plant, Vector2 world, out Vector2Int hitCell)
    {
        hitCell = default;

        if (!plant.TryGetComponent(out PlantPersistenceMarker marker))
            return false;

        if (!plant.TryGetComponent(out PlantDefinitionHolder holder) || holder.Definition == null)
            return false;

        if (_coordinateMapper == null)
            return false;

        Vector2Int anchor = marker.Anchor;
        foreach (Vector2Int offset in holder.Definition.footprint)
        {
            Vector2Int coords = anchor + offset;
            if (!IsInBounds(coords))
                continue;

            _coordinateMapper.GetCellCorners(coords, CellCornerBuffer);
            if (!FarmGridHitTest.IsPointInConvexQuad(world, CellCornerBuffer))
                continue;

            hitCell = coords;
            return true;
        }

        return false;
    }

    private int ComputePlantDrawOrder(GameObject plant)
    {
        if (!plant.TryGetComponent(out PlantPersistenceMarker marker) ||
            !plant.TryGetComponent(out PlantDefinitionHolder holder) ||
            holder.Definition == null)
            return int.MinValue;

        int maxOrder = int.MinValue;
        foreach (Vector2Int coords in holder.Definition.GetOccupiedCells(marker.Anchor))
            maxOrder = Mathf.Max(maxOrder, GetCellDrawOrder(coords));

        return maxOrder;
    }

    private void CollectUniquePlants(List<GameObject> buffer)
    {
        buffer.Clear();
        foreach (KeyValuePair<Vector2Int, GameObject> entry in _plantByCell)
        {
            GameObject plant = entry.Value;
            if (plant == null || buffer.Contains(plant))
                continue;

            buffer.Add(plant);
        }
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
                Vector2Int cell = new(col, row);
                bool occupied = Grid != null && Grid.IsInBounds(cell) && Grid.IsOccupied(cell);
                Color outline = occupied
                    ? new Color(1f, 0.2f, 0.2f, 0.7f)
                    : new Color(0f, 1f, 0.4f, 0.45f);
                DrawCellOutlineGizmo(mapper, cell, cellSz, outline);
            }
        }
    }

    private void DrawCellOutlineGizmo(
        IGridCoordinateMapper mapper, Vector2Int cell, Vector2 cellSz, Color outline)
    {
        Gizmos.color = outline;
        Vector2[] pts = new Vector2[4];
        if (coordinateMode == GridCoordinateMode.Isometric)
        {
            Vector2 center = mapper.CellToWorldCenter(cell);
            float hw = cellSz.x * 0.5f;
            float hh = cellSz.y * 0.5f;
            pts[0] = center + new Vector2(0f, hh);
            pts[1] = center + new Vector2(hw, 0f);
            pts[2] = center + new Vector2(0f, -hh);
            pts[3] = center + new Vector2(-hw, 0f);
        }
        else
        {
            Vector2 topLeft = mapper.CellToWorldTopLeft(cell);
            pts[0] = topLeft;
            pts[1] = topLeft + new Vector2(cellSz.x, 0f);
            pts[2] = topLeft + new Vector2(cellSz.x, -cellSz.y);
            pts[3] = topLeft + new Vector2(0f, -cellSz.y);
        }

        for (int i = 0; i < 4; i++)
            Gizmos.DrawLine(pts[i], pts[(i + 1) % 4]);
    }
#endif
}
