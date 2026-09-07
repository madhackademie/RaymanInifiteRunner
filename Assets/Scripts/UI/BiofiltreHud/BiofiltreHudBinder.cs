using UnityEngine;

/// <summary>
/// Relie le HUD world déjà enfant du biofiltre (pas d’Instantiate, pas de recale).
/// Pose des rows = transforms en Edit / Prefab Mode.
/// </summary>
[RequireComponent(typeof(GridManager))]
public class BiofiltreHudBinder : MonoBehaviour
{
    private const int HudSortingOrder = 20;

    [Tooltip("Enfant BiofiltreHud (nested). Pas un prefab à cloner.")]
    [SerializeField] private BiofiltreHudView hud;

    [SerializeField] private GridManager gridManager;

    private void Awake()
    {
        if (gridManager == null)
            gridManager = GetComponent<GridManager>();

        if (hud == null)
            hud = GetComponentInChildren<BiofiltreHudView>(true);

        if (hud == null)
        {
            Debug.LogWarning(
                "[BiofiltreHudBinder] HUD enfant manquant — fail closed.",
                this);
        }
    }

    private void Start()
    {
        BindEventCameraOnly();
    }

    private void BindEventCameraOnly()
    {
        if (hud == null)
            return;

        Canvas canvas = hud.GetComponent<Canvas>();
        if (canvas == null)
            canvas = hud.GetComponentInChildren<Canvas>(true);
        if (canvas == null)
            return;

        // Ne pas setter renderMode ni scaler : Unity recalcule scale/rotation du canvas.
        if (canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        canvas.sortingOrder = HudSortingOrder;
    }
}
