using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Caméra scène ferme (ex. FirstLvl) : pan/zoom libres ; clamp optionnel via <see cref="FarmLevelViewBounds"/>.
/// <see cref="BiofiltreViewBounds"/> = vue de départ / focus biofiltre (rect orange), pas prison runtime.
/// </summary>
[RequireComponent(typeof(Camera))]
public class FarmCameraController : MonoBehaviour
{
    private const float DefaultPaddingFactor = 1.08f;
    private const float DefaultMinOrthoSize = 1.5f;
    private const float NoMaxOrthoSize = 0f;

    [SerializeField] private Camera worldCamera;

    [Tooltip("Cadre initial ou Focus(biofiltre). Ne limite pas pan/zoom en jeu.")]
    [SerializeField] private BiofiltreViewBounds viewBounds;

    [Tooltip("Bornes niveau (multi-biofiltres). Vide = pan libre jusqu'à mise en scène.")]
    [SerializeField] private FarmLevelViewBounds levelViewBounds;

    [Tooltip("Marge autour du rect pour FrameToBounds / Focus uniquement.")]
    [SerializeField] [Min(1f)] private float paddingFactor = DefaultPaddingFactor;

    [Tooltip("Zoom avant max (ortho size mini).")]
    [SerializeField] [Min(0.1f)] private float minOrthoSize = DefaultMinOrthoSize;

    [Tooltip("Zoom arrière max (ortho size). 0 = pas de plafond.")]
    [SerializeField] [Min(0f)] private float maxOrthoSize = NoMaxOrthoSize;

    [Tooltip("Pas de zoom de la molette PC. Ne change pas le pinch.")]
    [SerializeField] [Min(0.0001f)] private float scrollZoomSensitivity = 0.1f;

    [SerializeField] private bool frameOnStart = true;
    [SerializeField] private bool allowPan = true;

    [Tooltip("Tactile : long press pan + pinch zoom.")]
    [SerializeField] private bool enableTouchCamera = false;

    [SerializeField] private bool autoEnableTouchOnMobile = true;

    [SerializeField] [Range(0.25f, 2f)] private float pinchZoomStrength = 1f;

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

        if (autoEnableTouchOnMobile && Application.isMobilePlatform)
            enableTouchCamera = true;

        if (frameOnStart && viewBounds != null)
            FrameToBounds();
        else
            ApplyClamp();
    }

    private void LateUpdate()
    {
        if (worldCamera == null)
            return;

        if (enableTouchCamera && FarmCameraInput.IsPinchGestureActive())
            HandlePinchZoom();
        else
        {
            HandleScrollZoom();
            if (allowPan)
                HandlePan();
        }

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

        if (viewBounds == null && frameOnStart)
            Debug.LogWarning("[FarmCameraController] Pas de BiofiltreViewBounds — frame de départ ignoré.", this);

        return true;
    }

    private void FrameToBounds()
    {
        if (viewBounds == null)
            return;

        Rect aabb = viewBounds.GetWorldAabb();
        worldCamera.orthographicSize = ComputeFrameOrtho(aabb);
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
        ApplyZoom(1f - scrollY * scrollZoomSensitivity, pivot);
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
        {
            float ratio = lastPinchDistance / distance;
            ApplyZoom(Mathf.Pow(ratio, pinchZoomStrength), midpoint);
        }

        lastPinchDistance = distance;
        pinchActive = true;
    }

    private void HandlePan()
    {
        bool hasPan = enableTouchCamera
            ? FarmCameraInput.TryGetLongPressPanScreenPosition(out Vector2 screenPosition)
            : FarmCameraInput.TryGetPanScreenPosition(out screenPosition);

        if (!hasPan)
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
        worldCamera.orthographicSize = FarmCameraViewMath.ClampOrthographicSize(
            worldCamera.orthographicSize * factor,
            minOrthoSize,
            ResolveMaxOrthoSize());
        Vector2 worldAfter = ScreenToWorld(screenPivot);
        Vector2 delta = worldBefore - worldAfter;
        Vector3 pos = worldCamera.transform.position;
        worldCamera.transform.position = new Vector3(pos.x + delta.x, pos.y + delta.y, pos.z);
    }

    private void ApplyClamp()
    {
        worldCamera.orthographicSize = FarmCameraViewMath.ClampOrthographicSize(
            worldCamera.orthographicSize,
            minOrthoSize,
            ResolveMaxOrthoSize());

        if (levelViewBounds == null)
            return;

        Rect levelAabb = levelViewBounds.GetWorldAabb();
        Vector3 pos = worldCamera.transform.position;
        Vector2 clamped = FarmCameraViewMath.ClampCameraCenter(
            pos,
            levelAabb,
            worldCamera.orthographicSize,
            worldCamera.aspect);
        worldCamera.transform.position = new Vector3(clamped.x, clamped.y, pos.z);
    }

    private float ResolveMaxOrthoSize() =>
        maxOrthoSize > minOrthoSize ? maxOrthoSize : float.MaxValue;

    private float ComputeFrameOrtho(Rect aabb) =>
        FarmCameraViewMath.ComputeFitOrthographicSize(aabb, worldCamera.aspect, paddingFactor);

    private Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        float depth = Mathf.Abs(worldCamera.transform.position.z);
        Vector3 world = worldCamera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, depth));
        return new Vector2(world.x, world.y);
    }
}
