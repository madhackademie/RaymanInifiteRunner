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
    private const string BedSpriteObjectName = "BedSprite";

    [Header("Containers")]
    [Tooltip("Parent transform for generated cell GameObjects.")]
    [SerializeField] private Transform gridContainer;

    [Tooltip("Parent transform where plant prefabs should be placed.")]
    [SerializeField] private Transform plantsContainer;

    [Header("Cell visuals")]
    [Tooltip("Sprite used for each cell. Leave empty to use a procedural white square.")]
    [SerializeField] private Sprite cellSprite;

    [Tooltip("Sorting order for cell sprites (above bed, below plants).")]
    [SerializeField] private int cellSortingOrder = 1;

    [Tooltip("When true, cell overlays are hidden until plant placement preview starts.")]
    [SerializeField] private bool showGridOnlyDuringPlacement = true;

    [Header("Bed (planter container)")]
    [Tooltip("Skin = sprite du bac. La grille ne change pas de taille en jeu.")]
    [SerializeField] private BiofiltreBedSkin bedSkin;

    [Tooltip("1 = largeur du sprite = largeur de la grille. Ajuster une fois par prefab.")]
    [SerializeField] [Min(0.05f)] private float bedScale = 1f;

    [Tooltip("Decalage monde du bac par rapport au centre de la grille.")]
    [SerializeField] private Vector2 bedOffset;

    [Tooltip("Sorting order for the planter sprite (behind grid and plants).")]
    [SerializeField] private int bedSortingOrder = -10;

    [Tooltip("Minimum sorting order for planted sprites (root + children).")]
    [SerializeField] private int plantSortingOrder = 5;

    /// <summary>Fired when any cell in this biofiltre is clicked.</summary>
    public event Action<BiofiltreCell> OnCellClicked;

    /// <summary>Exposes the Plants container so external systems can parent plant objects to it.</summary>
    public Transform PlantsContainer => plantsContainer;

    private GridManager gridManager;
    private BiofiltreCell[,] cells;
    private Sprite runtimeSquareSprite;
    private SpriteRenderer bedRenderer;
    private bool gridVisualVisible;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    // GenerateGrid is called by BiofiltreManager.Start — do not call it here too
    // (double ClearGrid destroys Inspector selection → "Object at index 0 is null").

    /// <summary>
    /// Creates the bed sprite once in Play Mode. Edit-mode preview uses gizmos only (no GameObject).
    /// </summary>
    public void EnsureBedSprite()
    {
        if (!Application.isPlaying)
            return;

        Sprite sprite = bedSkin != null ? bedSkin.Sprite : null;
        if (sprite == null)
        {
            Debug.LogWarning("[BiofiltreGridVisualizer] Bed sprite missing — assign Bed Skin.", this);
            return;
        }

#if UNITY_EDITOR
        WarnIfMultipleBedSprites();
#endif

        SpriteRenderer bed = GetOrCreateBedRenderer();
        bed.sprite       = sprite;
        bed.drawMode     = SpriteDrawMode.Simple;
        bed.sortingOrder = bedSortingOrder;
        bed.gameObject.SetActive(true);
        bed.transform.SetAsFirstSibling();
        FitBedToGrid(bed, sprite);
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

        int     columns  = gridManager.Columns;
        int     rows     = gridManager.Rows;
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

                SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                sr.sprite       = spriteToUse;
                sr.sortingOrder = cellSortingOrder;

                float ppu    = spriteToUse.pixelsPerUnit;
                float scaleX = cellSize.x * ppu / spriteToUse.rect.width;
                float scaleY = cellSize.y * ppu / spriteToUse.rect.height;
                cellObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                BoxCollider2D box = cellObj.AddComponent<BoxCollider2D>();
                box.size = Vector2.one;

                BiofiltreCell cell = cellObj.AddComponent<BiofiltreCell>();
                cell.Initialize(coords);
                cell.OnCellClicked += HandleCellClicked;

                cells[col, row] = cell;
            }
        }

        SetGridVisualVisible(!showGridOnlyDuringPlacement);
    }

    /// <summary>Shows or hides cell overlay sprites. Colliders stay active for farm clicks.</summary>
    public void SetGridVisualVisible(bool visible)
    {
        gridVisualVisible = visible;

        if (cells == null)
            return;

        int columns = gridManager != null ? gridManager.Columns : cells.GetLength(0);
        int rows    = gridManager != null ? gridManager.Rows    : cells.GetLength(1);

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                BiofiltreCell cell = cells[col, row];
                if (cell == null)
                    continue;

                if (cell.TryGetComponent(out SpriteRenderer sr))
                    sr.enabled = visible;
            }
        }

        if (visible)
            RefreshAllCellStates();
    }

    public bool IsGridVisualVisible => gridVisualVisible;

    /// <summary>Ensures plant sprites draw above the bed (keeps prefab child offsets if already higher).</summary>
    public void ApplyPlantDrawOrder(GameObject plantRoot)
    {
        if (plantRoot == null)
            return;

        foreach (SpriteRenderer sr in plantRoot.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (sr.sortingOrder < plantSortingOrder)
                sr.sortingOrder = plantSortingOrder;
        }
    }

    /// <summary>Layout for edit-mode gizmo preview (no SpriteRenderer in the Scene).</summary>
    public bool TryGetBedWorldTransform(out Sprite sprite, out Vector3 position, out float uniformScale)
    {
        sprite = bedSkin != null ? bedSkin.Sprite : null;
        position = Vector3.zero;
        uniformScale = 1f;
        if (sprite == null)
            return false;

        if (gridManager == null)
            gridManager = GetComponent<GridManager>();
        if (gridManager == null)
            return false;

        Vector2 gridSize = new Vector2(
            gridManager.Columns * gridManager.CellSizeWorld.x,
            gridManager.Rows * gridManager.CellSizeWorld.y
        );
        Vector2 native = sprite.bounds.size;
        if (gridSize.x < 0.01f || native.x < 0.01f)
            return false;

        Vector2 origin = gridManager.WorldOrigin;
        Vector2 gridCenter = new Vector2(origin.x + gridSize.x * 0.5f, origin.y - gridSize.y * 0.5f);
        uniformScale = gridSize.x / native.x * Mathf.Max(0.05f, bedScale);
        position = new Vector3(gridCenter.x + bedOffset.x, gridCenter.y + bedOffset.y, 0f);
        return true;
    }

    /// <summary>Re-applies bed sprite after a skin change. Play Mode only.</summary>
    public void RefreshBedSprite()
    {
        if (!Application.isPlaying)
            return;

        EnsureBedSprite();
    }

    /// <summary>Destroys all generated cell GameObjects.</summary>
    public void ClearGrid()
    {
        if (gridContainer == null)
            return;

#if UNITY_EDITOR
        RetargetEditorSelectionAwayFromGrid();
#endif

        for (int i = gridContainer.childCount - 1; i >= 0; i--)
            Destroy(gridContainer.GetChild(i).gameObject);

        cells = null;
    }

#if UNITY_EDITOR
    private void RetargetEditorSelectionAwayFromGrid()
    {
        UnityEngine.Object[] selected = UnityEditor.Selection.objects;
        if (selected == null || selected.Length == 0)
            return;

        for (int i = 0; i < selected.Length; i++)
        {
            UnityEngine.Object item = selected[i];
            if (item == null)
                continue;

            GameObject go = item is GameObject g
                ? g
                : item is Component c ? c.gameObject : null;

            if (go == null)
                continue;

            if (go.transform == gridContainer || go.transform.IsChildOf(gridContainer))
            {
                UnityEditor.Selection.activeGameObject = gameObject;
                return;
            }
        }
    }
#endif

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

    private void HandleCellClicked(BiofiltreCell cell)
    {
        OnCellClicked?.Invoke(cell);
        Debug.Log($"[BiofiltreGridVisualizer] '{gameObject.name}' — cell clicked: {cell.GridCoordinates}");
    }

    private void FitBedToGrid(SpriteRenderer bed, Sprite sprite)
    {
        Vector2 gridSize = new Vector2(
            gridManager.Columns * gridManager.CellSizeWorld.x,
            gridManager.Rows * gridManager.CellSizeWorld.y
        );
        Vector2 native = sprite.bounds.size;
        if (gridSize.x < 0.01f || native.x < 0.01f)
        {
            Debug.LogWarning("[BiofiltreGridVisualizer] Cannot fit bed sprite (zero size).", this);
            return;
        }

        Vector2 origin = gridManager.WorldOrigin;
        Vector2 gridCenter = new Vector2(origin.x + gridSize.x * 0.5f, origin.y - gridSize.y * 0.5f);
        float scale = gridSize.x / native.x * Mathf.Max(0.05f, bedScale);
        bed.transform.localScale = Vector3.one * scale;
        bed.transform.position = new Vector3(gridCenter.x + bedOffset.x, gridCenter.y + bedOffset.y, 0f);
    }

    /// <summary>
    /// Returns the cached bed renderer or reuses/creates the single direct child BedSprite.
    /// transform.Find was removed: it skips inactive children and caused duplicate creation.
    /// </summary>
    private SpriteRenderer GetOrCreateBedRenderer()
    {
        if (bedRenderer != null)
            return bedRenderer;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name != BedSpriteObjectName)
                continue;

            if (child.TryGetComponent(out bedRenderer))
                return bedRenderer;

            bedRenderer = child.gameObject.AddComponent<SpriteRenderer>();
            return bedRenderer;
        }

        GameObject bedObject = new GameObject(BedSpriteObjectName);
        bedObject.transform.SetParent(transform, worldPositionStays: false);
        bedRenderer = bedObject.AddComponent<SpriteRenderer>();
        return bedRenderer;
    }

#if UNITY_EDITOR
    private void WarnIfMultipleBedSprites()
    {
        int count = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == BedSpriteObjectName)
                count++;
        }

        if (count <= 1)
            return;

        Debug.LogWarning(
            $"[BiofiltreGridVisualizer] {count} objets '{BedSpriteObjectName}' sous '{name}'. " +
            "Un seul est attendu — menu Rayman/Farm/Nettoyer sélection biofiltre.",
            this);
    }
#endif

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
