using UnityEngine;

/// <summary>
/// Curseur gant de la récolte en balayage. Suit la souris tant que le mode n'est pas Idle.
/// </summary>
public class FarmBatchHarvestCursor : MonoBehaviour
{
    private const int CursorSortingOrder = 60;
    private static readonly int HarvestingParameterId = Animator.StringToHash("Harvesting");

    [SerializeField] private SpriteRenderer marker;
    [SerializeField] private Animator animator;

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

        Camera camera = Camera.main;
        if (camera == null || !FarmPointerInput.TryGetScreenPosition(out Vector2 screen))
            return;

        float depth = Mathf.Abs(camera.transform.position.z);
        Vector3 world = camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, depth));
        transform.position = new Vector3(world.x, world.y, 0f);
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
