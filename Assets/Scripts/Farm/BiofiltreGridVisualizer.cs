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

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        GenerateGrid();
    }

    /// <summary>
    /// (Re)generates all cell GameObjects under <see cref="gridContainer"/>.
    /// Destroys any previously generated cells first.
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

        Sprite spriteToUse = cellSprite != null ? cellSprite : GetOrCreateSquareSprite();

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector2Int coords      = new Vector2Int(col, row);
                Vector2    worldCenter = gridManager.GridToWorldCenter(coords);

                GameObject cellObj = new GameObject($"Cell_{col}_{row}");
                cellObj.transform.SetParent(gridContainer, worldPositionStays: false);
                cellObj.transform.position = worldCenter;

                // Scale the sprite to exactly match the cell size in world units
                SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                sr.sprite       = spriteToUse;
                sr.sortingOrder = cellSortingOrder;

                float ppu      = spriteToUse.pixelsPerUnit;
                float scaleX   = cellSize.x * ppu / spriteToUse.rect.width;
                float scaleY   = cellSize.y * ppu / spriteToUse.rect.height;
                cellObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                // Collider: size 1×1 in local space → world size = cellSize after scale
                BoxCollider2D box = cellObj.AddComponent<BoxCollider2D>();
                box.size = Vector2.one;

                BiofiltreCell cell = cellObj.AddComponent<BiofiltreCell>();
                cell.Initialize(coords);
                cell.OnCellClicked += HandleCellClicked;

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

    private void HandleCellClicked(BiofiltreCell cell)
    {
        OnCellClicked?.Invoke(cell);
        Debug.Log($"[BiofiltreGridVisualizer] '{gameObject.name}' — cell clicked: {cell.GridCoordinates}");
    }

    // ── Procedural fallback sprite ────────────────────────────────────────────

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
}
