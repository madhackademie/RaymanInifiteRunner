using UnityEngine;

/// <summary>
/// Curseur gant pendant la pose de graines (preview placement). Suit le pointeur tant que le preview est actif.
/// </summary>
public class FarmPlantingCursor : MonoBehaviour
{
    private const int CursorSortingOrder = 60;
    private static readonly int PlantingParameterId = Animator.StringToHash("Planting");

    [SerializeField] private SpriteRenderer marker;
    [SerializeField] private Animator animator;
    [SerializeField] private PlantPlacementPreview placementPreview;

    private FarmBatchHarvestInput batchHarvest;

    private void Awake()
    {
        if (placementPreview == null)
            placementPreview = GetComponentInParent<PlantPlacementPreview>();

        batchHarvest = GetComponentInParent<FarmBatchHarvestInput>();
    }

    private void OnEnable()
    {
        if (batchHarvest == null)
            batchHarvest = GetComponentInParent<FarmBatchHarvestInput>();
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        SetGloveVisible(false);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Cursor.visible = true;
            return;
        }

        RefreshSystemCursorVisibility();
    }

    private void LateUpdate()
    {
        if (placementPreview == null)
            placementPreview = GetComponentInParent<PlantPlacementPreview>();

        // FarmBatchHarvestInput est ajouté à l'exécution par FarmGridPointerInput.Awake (parent) :
        // il peut ne pas exister encore lors de Awake/OnEnable, on le résout tant qu'il est null.
        if (batchHarvest == null)
            batchHarvest = GetComponentInParent<FarmBatchHarvestInput>();

        if (placementPreview == null || !placementPreview.IsPreviewModeActive)
        {
            SetGloveVisible(false);
            ApplySystemCursorVisibility();
            return;
        }

        SetGloveVisible(true);
        ApplySystemCursorVisibility();

        if (animator != null)
        {
            animator.SetBool(PlantingParameterId, placementPreview.IsPaintStrokeActive);
            animator.Update(0f);
        }

        Camera camera = Camera.main;
        if (camera == null || !FarmPointerInput.TryGetScreenPosition(out Vector2 screen))
            return;

        float depth = Mathf.Abs(camera.transform.position.z);
        Vector3 world = camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, depth));
        transform.position = new Vector3(world.x, world.y, 0f);
    }

    private void RefreshSystemCursorVisibility()
    {
        ApplySystemCursorVisibility();
    }

    private bool ShouldHideSystemCursor()
    {
        bool plantingActive = placementPreview != null && placementPreview.IsPreviewModeActive;
        bool harvestActive = batchHarvest != null && batchHarvest.Mode != FarmBatchHarvestMode.Idle;
        return plantingActive || harvestActive;
    }

    private void ApplySystemCursorVisibility()
    {
        // N'écrire que sur changement : réécrire Cursor.visible chaque frame fait scintiller le pointeur Windows.
        bool visible = !ShouldHideSystemCursor();
        if (Cursor.visible != visible)
            Cursor.visible = visible;
    }

    private void SetGloveVisible(bool show)
    {
        if (marker != null)
        {
            marker.enabled = show;
            marker.sortingOrder = CursorSortingOrder;
            marker.color = Color.white;
        }

        if (!show && animator != null)
            animator.SetBool(PlantingParameterId, false);
    }
}
