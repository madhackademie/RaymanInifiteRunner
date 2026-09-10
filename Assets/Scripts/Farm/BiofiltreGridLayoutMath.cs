using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Résolution layout monde + bake dimensions grille (stratégie B — axes iso / ortho).
/// </summary>
public static class BiofiltreGridLayoutMath
{
    private const float IsometricHeightRatio = 0.5f;
    private const int MaxBakeIterations = 12;
    private const float NegativeCellTolerance = 0.35f;

    public static void ResolveWorldLayout(
        GridCoordinateMode coordinateMode,
        float instanceCellSize,
        bool instanceUniformCellSize,
        Vector2 originOffset,
        Transform layoutTransform,
        int columns,
        int rows,
        out Vector2 cellSizeWorld,
        out Vector2 worldOriginNorth)
    {
        cellSizeWorld = instanceUniformCellSize
            ? new Vector2(instanceCellSize, instanceCellSize)
            : new Vector2(instanceCellSize, instanceCellSize);

        worldOriginNorth = layoutTransform != null
            ? (Vector2)layoutTransform.position + originOffset
            : originOffset;

        bool useHandle = layoutTransform != null;
        if (useHandle)
        {
            float sizeMul = Mathf.Max(0.01f, Mathf.Abs(layoutTransform.lossyScale.x));
            cellSizeWorld *= sizeMul;
        }

        if (coordinateMode == GridCoordinateMode.Isometric && IsUniformCellSize(cellSizeWorld))
            cellSizeWorld = new Vector2(cellSizeWorld.x, cellSizeWorld.x * IsometricHeightRatio);

        if (useHandle)
        {
            worldOriginNorth = CenterOriginOnHandle(
                (Vector2)layoutTransform.position,
                worldOriginNorth,
                cellSizeWorld,
                columns,
                rows,
                coordinateMode);
        }
        else if (coordinateMode == GridCoordinateMode.Isometric)
        {
            worldOriginNorth.x += columns * cellSizeWorld.x * 0.5f;
        }
    }

    public static Vector2 WorldToCellContinuous(
        Vector2 world,
        Vector2 worldOriginNorth,
        Vector2 cellSizeWorld,
        GridCoordinateMode coordinateMode)
    {
        if (coordinateMode == GridCoordinateMode.Isometric)
            return WorldToCellContinuousIsometric(world, worldOriginNorth, cellSizeWorld);

        Vector2 local = world - worldOriginNorth;
        return new Vector2(
            local.x / cellSizeWorld.x,
            -local.y / cellSizeWorld.y);
    }

    /// <summary>
    /// Mesure le quad shape en espace cellule et retourne colonnes/lignes (entiers, min 1).
    /// Itère car l’origine est recentrée sur le handle quand cols/rows changent.
    /// </summary>
    public static bool TryBakeColumnRowCountB(
        IReadOnlyList<Vector2> shapeWorldCorners,
        GridCoordinateMode coordinateMode,
        float instanceCellSize,
        bool instanceUniformCellSize,
        Vector2 originOffset,
        Transform layoutTransform,
        int seedColumns,
        int seedRows,
        out int columns,
        out int rows,
        out string warning,
        out float lastMinCol,
        out float lastMaxCol,
        out float lastMinRow,
        out float lastMaxRow)
    {
        warning = null;
        lastMinCol = lastMaxCol = lastMinRow = lastMaxRow = 0f;
        columns = Mathf.Max(1, seedColumns);
        rows = Mathf.Max(1, seedRows);

        if (shapeWorldCorners == null || shapeWorldCorners.Count < 1)
        {
            warning = "Aucun coin de shape.";
            return false;
        }

        if (layoutTransform == null)
        {
            warning = "GridManager : assigner l’enfant Grid comme Layout Transform.";
            return false;
        }

        for (int iter = 0; iter < MaxBakeIterations; iter++)
        {
            ResolveWorldLayout(
                coordinateMode,
                instanceCellSize,
                instanceUniformCellSize,
                originOffset,
                layoutTransform,
                columns,
                rows,
                out Vector2 cellSizeWorld,
                out Vector2 originNorth);

            float minCol = float.MaxValue;
            float maxCol = float.MinValue;
            float minRow = float.MaxValue;
            float maxRow = float.MinValue;

            for (int i = 0; i < shapeWorldCorners.Count; i++)
            {
                Vector2 cr = WorldToCellContinuous(
                    shapeWorldCorners[i], originNorth, cellSizeWorld, coordinateMode);
                minCol = Mathf.Min(minCol, cr.x);
                maxCol = Mathf.Max(maxCol, cr.x);
                minRow = Mathf.Min(minRow, cr.y);
                maxRow = Mathf.Max(maxRow, cr.y);
            }

            lastMinCol = minCol;
            lastMaxCol = maxCol;
            lastMinRow = minRow;
            lastMaxRow = maxRow;

            if (minCol < -NegativeCellTolerance || minRow < -NegativeCellTolerance)
            {
                warning =
                    "Le shape dépasse avant la cellule (0,0) : le quad est un peu large ou l’enfant Grid est décalé. " +
                    "Rétrécir le quad cyan OU déplacer l’enfant Grid (petits pas) puis rebake. " +
                    $"Mesure bake : col [{minCol:F2}…{maxCol:F2}], row [{minRow:F2}…{maxRow:F2}].";
            }

            int newCols = Mathf.Max(1, Mathf.CeilToInt(maxCol + 0.02f));
            int newRows = Mathf.Max(1, Mathf.CeilToInt(maxRow + 0.02f));

            if (newCols == columns && newRows == rows)
                return true;

            columns = newCols;
            rows = newRows;
        }

        warning = (warning ?? "") + " Bake B : convergence partielle — vérifier le playtest.";
        return true;
    }

    private static Vector2 WorldToCellContinuousIsometric(
        Vector2 world,
        Vector2 worldOriginNorth,
        Vector2 cellSizeWorld)
    {
        float halfWidth = cellSizeWorld.x * 0.5f;
        float halfHeight = cellSizeWorld.y * 0.5f;
        float localX = world.x - worldOriginNorth.x;
        float localY = world.y - (worldOriginNorth.y - halfHeight);
        float colF = (localX / halfWidth - localY / halfHeight) * 0.5f;
        float rowF = (-localY / halfHeight - localX / halfWidth) * 0.5f;
        return new Vector2(colF, rowF);
    }

    private static Vector2 CenterOriginOnHandle(
        Vector2 handlePosition,
        Vector2 originBeforeCenter,
        Vector2 cellSize,
        int columnCount,
        int rowCount,
        GridCoordinateMode coordinateMode)
    {
        var probe = GridCoordinateMapperFactory.Create(
            coordinateMode,
            new GridLayoutSnapshot(originBeforeCenter, cellSize, columnCount, rowCount));
        Vector2 aabbCenter = ComputeAabb(probe, columnCount, rowCount).center;
        return originBeforeCenter + handlePosition - aabbCenter;
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

    private static bool IsUniformCellSize(Vector2 cellSizeWorld) =>
        Mathf.Abs(cellSizeWorld.x - cellSizeWorld.y) < 0.001f;
}
