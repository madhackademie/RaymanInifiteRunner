using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a single clickable cell of a biofiltre grid.
/// Fires OnCellClicked when the player clicks this cell.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class BiofiltreCell : MonoBehaviour, IPointerClickHandler
{
    private static readonly Color ColorEmpty    = new Color(0.30f, 0.75f, 0.40f, 0.25f);
    private static readonly Color ColorOccupied = new Color(0.20f, 0.55f, 0.30f, 0.50f);

    /// <summary>Column / row coordinates of this cell in its parent grid.</summary>
    public Vector2Int GridCoordinates { get; private set; }

    /// <summary>Fired when the player clicks this cell. Passes itself as argument.</summary>
    public event Action<BiofiltreCell> OnCellClicked;

    private SpriteRenderer spriteRenderer;

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
        if (spriteRenderer != null)
            spriteRenderer.color = occupied ? ColorOccupied : ColorEmpty;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCellClicked?.Invoke(this);
    }
}
