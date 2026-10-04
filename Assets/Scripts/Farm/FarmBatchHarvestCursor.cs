using UnityEngine;

/// <summary>
/// Curseur gant de la récolte en balayage. La racine suit la cible grille ;
/// le point de contact visuel (base du gant) est corrigé via <see cref="gripOffsetLocal"/>.
/// </summary>
public class FarmBatchHarvestCursor : MonoBehaviour
{
    private const int CursorSortingOrder = 60;
    private static readonly int HarvestingParameterId = Animator.StringToHash("Harvesting");

    [SerializeField] private SpriteRenderer marker;
    [SerializeField] private Animator animator;

    [Header("Alignement grille (option B)")]
    [Tooltip("Offset local ajouté au point de contact auto (bas-centre du sprite courant).")]
    [SerializeField] private Vector2 gripOffsetLocal;

    [Tooltip("Point de contact = bas-centre du sprite (bounds), recalculé chaque frame (anim).")]
    [SerializeField] private bool autoGripFromSpriteBottomCenter = true;

    private FarmBatchHarvestInput harvestInput;

    private void OnEnable()
    {
        BindInput();
    }

    private void BindInput()
    {
        if (harvestInput != null)
            return;

        harvestInput = GetComponentInParent<FarmBatchHarvestInput>();
        if (harvestInput == null)
        {
            ApplyMode(FarmBatchHarvestMode.Idle);
            return;
        }

        harvestInput.ModeChanged += OnModeChanged;
        ApplyMode(harvestInput.Mode);
    }

    private void OnDisable()
    {
        if (harvestInput != null)
            harvestInput.ModeChanged -= OnModeChanged;

        harvestInput = null;
        Cursor.visible = true;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Cursor.visible = true;
            return;
        }

        bool showGlove = harvestInput != null && harvestInput.Mode != FarmBatchHarvestMode.Idle;
        Cursor.visible = !showGlove;
    }

    private void LateUpdate()
    {
        if (harvestInput == null)
            BindInput();

        if (harvestInput == null || harvestInput.Mode == FarmBatchHarvestMode.Idle)
            return;

        if (!harvestInput.TryGetCursorWorld(out Vector3 targetWorld))
            return;

        // Racine positionnée pour que le point de contact (grip) coïncide avec la cible monde.
        transform.position = targetWorld - GetGripOffsetWorld();
    }

    /// <summary>
    /// Décalage monde pivot → point de contact (bas-centre du sprite).
    /// Le SpriteRenderer est sur ce transform : ne pas y ajouter localPosition,
    /// sinon l'offset inclut la position du curseur et le gant part à l'opposé du pointeur.
    /// </summary>
    private Vector3 GetGripOffsetWorld()
    {
        Vector3 gripLocal = new Vector3(gripOffsetLocal.x, gripOffsetLocal.y, 0f);

        if (autoGripFromSpriteBottomCenter && marker != null && marker.sprite != null)
        {
            Bounds bounds = marker.sprite.bounds;
            gripLocal.x += bounds.center.x;
            gripLocal.y += bounds.min.y;
        }

        Transform spriteTransform = marker != null ? marker.transform : transform;
        if (spriteTransform != transform)
            gripLocal += spriteTransform.localPosition;

        return spriteTransform.TransformVector(gripLocal);
    }

    private void OnModeChanged(FarmBatchHarvestMode mode)
    {
        ApplyMode(mode);
    }

    private void ApplyMode(FarmBatchHarvestMode mode)
    {
        bool showGlove = mode != FarmBatchHarvestMode.Idle;
        if (marker != null)
        {
            marker.enabled = showGlove;
            marker.sortingOrder = CursorSortingOrder;
            marker.color = Color.white;
        }

        Cursor.visible = !showGlove;

        if (animator != null && showGlove)
        {
            animator.SetBool(HarvestingParameterId, mode == FarmBatchHarvestMode.Stroking);
            animator.Update(0f);
        }
    }
}
