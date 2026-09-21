using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Caméra farm : cadrage sur <see cref="BiofiltreViewBounds"/>, zoom borné, pan clampé.
/// Slice 1 PC : molette + clic milieu. Tactile Township : <c>enableTouchCamera</c>.
/// </summary>
[RequireComponent(typeof(Camera))]
public class FarmCameraController : MonoBehaviour
{
    private const float DefaultPaddingFactor = 1.08f;
    private const float DefaultMinOrthoSize = 1.5f;
    private const float ScrollZoomSensitivity = 0.001f;

    [SerializeField] private Camera worldCamera;
    [SerializeField] private BiofiltreViewBounds viewBounds;

    [Tooltip("Marge autour du rect pour la vue par défaut (1 = collé).")]
    [SerializeField] [Min(1f)] private float paddingFactor = DefaultPaddingFactor;

    [Tooltip("Zoom avant max (ortho size mini).")]
    [SerializeField] [Min(0.1f)] private float minOrthoSize = DefaultMinOrthoSize;

    [SerializeField] private bool frameOnStart = true;
    [SerializeField] private bool allowPan = true;

    [Tooltip("Slice 2 Township. Slice 1 PC = off (molette + clic milieu seulement).")]
    [SerializeField] private bool enableTouchCamera = false;

    private bool pinchActive;
    private float lastPinchDistance;
    private bool panAnchored;
    private Vector2 lastPanScreen;

    public void Focus(BiofiltreViewBounds bounds)
    {
        viewBounds = bounds;
        FrameToBounds();
    }

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        if (!TryResolve())
            return;

        if (frameOnStart)
            FrameToBounds();
        else
            ApplyClamp();
    }

    private void LateUpdate()
    {
        if (worldCamera == null || viewBounds == null)
            return;

        if (enableTouchCamera)
            HandlePinchZoom();
        HandleScrollZoom();
        if (allowPan)
            HandlePan();
        ApplyClamp();
    }

    private bool TryResolve()
    {
        if (worldCamera == null)
            worldCamera = GetComponent<Camera>();

        if (worldCamera == null || !worldCamera.orthographic)
        {
            Debug.LogWarning("[FarmCameraController] Caméra ortho manquante — cadrage farm off.", this);
            enabled = false;
            return false;
        }

        if (viewBounds == null)
            viewBounds = FindFirstObjectByType<BiofiltreViewBounds>();

        if (viewBounds != null)
            return true;

        Debug.LogWarning("[FarmCameraController] BiofiltreViewBounds manquant — Add Component sur le biofiltre.", this);
        enabled = false;
        return false;
    }

    private void FrameToBounds()
    {
        Rect aabb = viewBounds.GetWorldAabb();
        worldCamera.orthographicSize = ComputeMaxOrtho(aabb);
        Vector2 center = aabb.center;
        Vector3 pos = worldCamera.transform.position;
        worldCamera.transform.position = new Vector3(center.x, center.y, pos.z);
    }

    private void HandleScrollZoom()
    {
        if (!FarmCameraInput.TryGetScrollZoom(out float scrollY))
            return;

        Vector2 pivot = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        ApplyZoom(1f - scrollY * ScrollZoomSensitivity, pivot);
    }

    private void HandlePinchZoom()
    {
        if (!FarmCameraInput.TryGetPinch(out float distance, out Vector2 midpoint))
        {
            pinchActive = false;
            lastPinchDistance = 0f;
            return;
        }

        if (pinchActive && lastPinchDistance > 1f)
            ApplyZoom(lastPinchDistance / distance, midpoint);

        lastPinchDistance = distance;
        pinchActive = true;
    }

    private void HandlePan()
    {
        if (!FarmCameraInput.TryGetPanScreenPosition(enableTouchCamera, out Vector2 screenPosition))
        {
            panAnchored = false;
            return;
        }

        if (!panAnchored)
        {
            lastPanScreen = screenPosition;
            panAnchored = true;
            return;
        }

        Vector2 worldBefore = ScreenToWorld(lastPanScreen);
        Vector2 worldAfter = ScreenToWorld(screenPosition);
        Vector2 delta = worldBefore - worldAfter;
        lastPanScreen = screenPosition;
        if (delta.sqrMagnitude < 0.0001f)
            return;

        Vector3 pos = worldCamera.transform.position;
        worldCamera.transform.position = new Vector3(pos.x + delta.x, pos.y + delta.y, pos.z);
    }

    private void ApplyZoom(float factor, Vector2 screenPivot)
    {
        if (Mathf.Abs(factor - 1f) < 0.0001f)
            return;

        Vector2 worldBefore = ScreenToWorld(screenPivot);
        Rect aabb = viewBounds.GetWorldAabb();
        float maxOrtho = ComputeMaxOrtho(aabb);
        worldCamera.orthographicSize = FarmCameraViewMath.ClampOrthographicSize(
            worldCamera.orthographicSize * factor,
            minOrthoSize,
            maxOrtho);
        Vector2 worldAfter = ScreenToWorld(screenPivot);
        Vector2 delta = worldBefore - worldAfter;
        Vector3 pos = worldCamera.transform.position;
        worldCamera.transform.position = new Vector3(pos.x + delta.x, pos.y + delta.y, pos.z);
    }

    private void ApplyClamp()
    {
        Rect aabb = viewBounds.GetWorldAabb();
        float maxOrtho = ComputeMaxOrtho(aabb);
        worldCamera.orthographicSize = FarmCameraViewMath.ClampOrthographicSize(
            worldCamera.orthographicSize,
            minOrthoSize,
            maxOrtho);

        Vector3 pos = worldCamera.transform.position;
        Vector2 clamped = FarmCameraViewMath.ClampCameraCenter(
            pos,
            aabb,
            worldCamera.orthographicSize,
            worldCamera.aspect);
        worldCamera.transform.position = new Vector3(clamped.x, clamped.y, pos.z);
    }

    private float ComputeMaxOrtho(Rect aabb) =>
        FarmCameraViewMath.ComputeFitOrthographicSize(aabb, worldCamera.aspect, paddingFactor);

    private Vector2 ScreenToWorld(Vector2 screenPosition) =>
        worldCamera.ScreenToWorldPoint(screenPosition);
}
