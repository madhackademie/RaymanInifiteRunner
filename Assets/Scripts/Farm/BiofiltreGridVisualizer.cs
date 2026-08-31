using System;
using UnityEngine;

/// <summary>
/// Generates and manages the visual cell grid for a biofiltre at runtime.
/// Each cell is a square sprite that is clickable via <see cref="BiofiltreCell"/>.
/// Requires a <see cref="GridManager"/> on the same GameObject.
/// </summary>
[RequireComponent(typeof(GridManager))]
public class BiofiltreGridVisualizer : MonoBehaviour
{
    [Header("Containers")]
    [Tooltip("Parent transform for generated cell GameObjects.")]
    [SerializeField] private Transform gridContainer;

    [Tooltip("Parent transform where plant prefabs should be placed.")]
    [SerializeField] private Transform plantsContainer;

    [Header("Cell visuals")]
    [Tooltip("Sprite used for each cell. Leave empty to use a procedural white square.")]
    [SerializeField] private Sprite cellSprite;

    [Tooltip("Sorting order for cell sprites.")]
    [SerializeField] private int cellSortingOrder = 0;

    /// <summary>Fired when any cell in this biofiltre is clicked.</summary>
    public event Action<BiofiltreCell> OnCellClicked;

    /// <summary>Exposes the Plants container so external systems can parent plant objects to it.</summary>
    public Transform PlantsContainer => plantsContainer;

    private GridManager gridManager;
    private BiofiltreCell[,] cells;
    private Sprite runtimeSquareSprite;
    private Sprite runtimeDiamondSprite;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    /// <summary>
    /// (Re)generates all cell GameObjects under <see cref="gridContainer"/>.
    /// Destroys any previously generated cells first.
    /// Appelé une fois par <see cref="BiofiltreManager.Start"/> (pas ici : double ClearGrid).
    /// </summary>
    public void GenerateGrid()
    {
        if (gridManager == null)
        {
            Debug.LogError("[BiofiltreGridVisualizer] No GridManager found on this GameObject.", this);
            return;
        }

        ClearGrid();

        int    columns  = gridManager.Columns;
        int    rows     = gridManager.Rows;
        Vector2 cellSize = gridManager.CellSizeWorld;

        cells = new BiofiltreCell[columns, rows];

        Sprite spriteToUse = ResolveCellSprite();

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector2Int coords      = new Vector2Int(col, row);
                Vector2    worldCenter = gridManager.GridToWorldCenter(coords);

                GameObject cellObj = new GameObject($"Cell_{col}_{row}");
                cellObj.transform.SetParent(gridContainer, worldPositionStays: false);
                cellObj.transform.position = worldCenter;

                SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                sr.sprite       = spriteToUse;
                sr.sortingOrder = cellSortingOrder + col + row;

                float ppu = spriteToUse.pixelsPerUnit;
                float scaleX = cellSize.x * ppu / spriteToUse.rect.width;
                float scaleY = cellSize.y * ppu / spriteToUse.rect.height;
                cellObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                BiofiltreCell cell = cellObj.AddComponent<BiofiltreCell>();
                cell.Initialize(coords);

                cells[col, row] = cell;
            }
        }
    }

    /// <summary>Destroys all generated cell GameObjects.</summary>
    public void ClearGrid()
    {
        if (gridContainer == null)
            return;

        for (int i = gridContainer.childCount - 1; i >= 0; i--)
            Destroy(gridContainer.GetChild(i).gameObject);
    }

    /// <summary>
    /// Returns the <see cref="BiofiltreCell"/> at the given grid coordinates,
    /// or null if the coordinates are out of bounds or the grid has not been generated yet.
    /// </summary>
    public BiofiltreCell GetCell(Vector2Int coords)
    {
        if (cells == null || !gridManager.IsInBounds(coords))
            return null;

        return cells[coords.x, coords.y];
    }

    /// <summary>
    /// Aligne tous les visuels de cellules avec l'etat d'occupation runtime de la grille.
    /// </summary>
    public void RefreshAllCellStates()
    {
        if (cells == null || gridManager == null)
            return;

        int columns = gridManager.Columns;
        int rows = gridManager.Rows;
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector2Int coords = new(col, row);
                BiofiltreCell cell = cells[col, row];
                if (cell != null)
                    cell.SetVisualState(!gridManager.IsCellFree(coords));
            }
        }
    }

    /// <summary>
    /// Point d'entrée du clic grille, appelé par <see cref="FarmGridPointerInput"/>.
    /// </summary>
    public void NotifyCellClicked(Vector2Int coords)
    {
        BiofiltreCell cell = GetCell(coords);
        if (cell == null)
            return;

        OnCellClicked?.Invoke(cell);
        Debug.Log($"[BiofiltreGridVisualizer] '{gameObject.name}' — cell clicked: {coords}");
    }

    // ── Procedural fallback sprite ────────────────────────────────────────────

    private Sprite ResolveCellSprite()
    {
        if (gridManager.CoordinateMode == GridCoordinateMode.Isometric)
            return GetOrCreateDiamondSprite();

        return cellSprite != null ? cellSprite : GetOrCreateSquareSprite();
    }

    private Sprite GetOrCreateSquareSprite()
    {
        if (runtimeSquareSprite != null)
            return runtimeSquareSprite;

        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, mipChain: false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode   = TextureWrapMode.Clamp
        };

        Color white = Color.white;
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                tex.SetPixel(x, y, white);
        tex.Apply();

        runtimeSquareSprite = Sprite.Create(
            tex,
            new Rect(0, 0, 4, 4),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit: 4f
        );

        return runtimeSquareSprite;
    }

    private Sprite GetOrCreateDiamondSprite()
    {
        if (runtimeDiamondSprite != null)
            return runtimeDiamondSprite;

        const int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode   = TextureWrapMode.Clamp
        };

        float center = (size - 1) * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float nx = (x - center) / center;
                float ny = (y - center) / center;
                bool inside = Mathf.Abs(nx) + Mathf.Abs(ny) <= 1.02f;
                tex.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }

        tex.Apply();
        runtimeDiamondSprite = Sprite.Create(
            tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit: size);
        return runtimeDiamondSprite;
    }
}
