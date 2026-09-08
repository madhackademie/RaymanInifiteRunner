using UnityEngine;

/// <summary>
/// Cellule visuelle d'une grille de biofiltre : coordonnées + teinte d'occupation.
/// Le clic est résolu par calcul dans <see cref="FarmGridPointerInput"/>.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BiofiltreCell : MonoBehaviour
{
    private static readonly Color ColorEmpty    = new Color(0.30f, 0.75f, 0.40f, 0.25f);
    private static readonly Color ColorOccupied = new Color(0.20f, 0.55f, 0.30f, 0.50f);
    private static readonly Color ColorPreviewValid   = new Color(0.22f, 0.95f, 0.38f, 0.82f);
    private static readonly Color ColorPreviewInvalid = new Color(1.00f, 0.22f, 0.22f, 0.60f);
    private static readonly Color ColorSelected       = new Color(0.22f, 0.95f, 0.38f, 0.88f);

    /// <summary>Column / row coordinates of this cell in its parent grid.</summary>
    public Vector2Int GridCoordinates { get; private set; }

    private SpriteRenderer spriteRenderer;
    private bool occupied;
    private CellHighlightMode highlightMode = CellHighlightMode.None;

    private enum CellHighlightMode
    {
        None,
        PlacementValid,
        PlacementInvalid,
        Selected,
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Sets up the cell after instantiation.
    /// Must be called by <see cref="BiofiltreGridVisualizer"/> right after creating the GameObject.
    /// </summary>
    public void Initialize(Vector2Int coordinates)
    {
        GridCoordinates = coordinates;
        name = $"Cell_{coordinates.x}_{coordinates.y}";
        SetVisualState(false);
    }

    /// <summary>Updates the cell tint to reflect its occupied / empty state.</summary>
    public void SetVisualState(bool occupied)
    {
        this.occupied = occupied;
        if (highlightMode == CellHighlightMode.None)
            ApplyBaseColor();
    }

    /// <summary>Teinte temporaire du footprint pendant la preview de pose.</summary>
    public void SetPlacementPreview(bool valid)
    {
        highlightMode = valid ? CellHighlightMode.PlacementValid : CellHighlightMode.PlacementInvalid;
        ApplyHighlightColor();
    }

    /// <summary>Illumine le socle (footprint) d'une plante sélectionnée.</summary>
    public void SetSelectionHighlight(bool active)
    {
        highlightMode = active ? CellHighlightMode.Selected : CellHighlightMode.None;
        if (highlightMode == CellHighlightMode.None)
            ApplyBaseColor();
        else
            ApplyHighlightColor();
    }

    /// <summary>Retire preview / sélection et restaure l'état d'occupation.</summary>
    public void ClearTransientHighlight()
    {
        highlightMode = CellHighlightMode.None;
        ApplyBaseColor();
    }

    private void ApplyBaseColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = occupied ? ColorOccupied : ColorEmpty;
    }

    private void ApplyHighlightColor()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.color = highlightMode switch
        {
            CellHighlightMode.PlacementValid   => ColorPreviewValid,
            CellHighlightMode.PlacementInvalid => ColorPreviewInvalid,
            CellHighlightMode.Selected         => ColorSelected,
            _                                  => occupied ? ColorOccupied : ColorEmpty,
        };
    }
}
