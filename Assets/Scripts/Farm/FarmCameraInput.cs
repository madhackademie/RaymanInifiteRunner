using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/// <summary>
/// Lecture zoom/pan ferme (molette, pinch, clic milieu). Hors clic plantation.
/// </summary>
public static class FarmCameraInput
{
    private const int MousePointerId = -1;
    private const int MinPinchTouches = 2;
    private const float LongPressSeconds = 0.45f;
    private const float TapSlopPixels = 12f;

    private static bool longPressTracking;
    private static bool longPressFired;
    private static float longPressStartTime;
    private static Vector2 longPressStartScreen;

    /// <summary>Vrai tant que la caméra a « pris » le pointeur principal (slop dépassé ou long press) : le tap grille doit être ignoré.</summary>
    public static bool IsPrimaryPointerConsumedByCamera { get; private set; }

    public static bool TryGetScrollZoom(out float scrollY)
    {
        scrollY = 0f;
        if (FarmPointerInput.IsOverUi(MousePointerId))
            return false;

        Mouse mouse = Mouse.current;
        if (mouse == null)
            return false;

        scrollY = mouse.scroll.ReadValue().y;
        return Mathf.Abs(scrollY) > 0.01f;
    }

    public static bool TryGetPinch(out float distance, out Vector2 midpoint)
    {
        distance = 0f;
        midpoint = default;
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen == null)
            return false;

        if (!TryReadTwoTouches(touchscreen, out Vector2 a, out Vector2 b))
            return false;

        midpoint = (a + b) * 0.5f;
        distance = Vector2.Distance(a, b);
        return distance > 1f;
    }

    /// <summary>Pan PC : clic milieu uniquement (pas de pan au pinch — zoom et pan sont exclus).</summary>
    public static bool TryGetPanScreenPosition(out Vector2 screenPosition)
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.middleButton.isPressed)
        {
            screenPosition = mouse.position.ReadValue();
            return !FarmPointerInput.IsOverUi(MousePointerId);
        }

        screenPosition = default;
        return false;
    }

    /// <summary>Vrai si un geste pinch (2 doigts) est en cours — pan 1 doigt interdit.</summary>
    public static bool IsPinchGestureActive()
    {
        Touchscreen touchscreen = Touchscreen.current;
        return touchscreen != null && CountActiveTouches(touchscreen) >= MinPinchTouches;
    }

    /// <summary>
    /// Pan tactile Township : appui long (<see cref="LongPressSeconds"/>) puis drag 1 doigt.
    /// Pinch = zoom (+ rotation future) ; jamais pan simultané.
    /// </summary>
    public static bool TryGetLongPressPanScreenPosition(out Vector2 screenPosition)
    {
        screenPosition = default;

        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen == null)
        {
            ResetLongPressPan();
            return false;
        }

        if (IsPinchGestureActive())
        {
            ResetLongPressPan();
            return false;
        }

        TouchControl primaryTouch = touchscreen.primaryTouch;
        if (!primaryTouch.press.isPressed)
        {
            ResetLongPressPan();
            return false;
        }

        int pointerId = primaryTouch.touchId.ReadValue();
        if (FarmPointerInput.IsOverUi(pointerId))
        {
            ResetLongPressPan();
            return false;
        }

        Vector2 currentScreen = primaryTouch.position.ReadValue();

        if (!longPressTracking)
        {
            longPressTracking = true;
            longPressFired = false;
            longPressStartTime = Time.unscaledTime;
            longPressStartScreen = currentScreen;
            IsPrimaryPointerConsumedByCamera = false;
            return false;
        }

        if (!longPressFired)
        {
            float movedDistance = Vector2.Distance(currentScreen, longPressStartScreen);
            if (movedDistance > TapSlopPixels)
                IsPrimaryPointerConsumedByCamera = true;

            float elapsed = Time.unscaledTime - longPressStartTime;
            if (elapsed < LongPressSeconds)
                return false;

            longPressFired = true;
            IsPrimaryPointerConsumedByCamera = true;
        }

        screenPosition = currentScreen;
        return true;
    }

    /// <summary>
    /// Tap tactile au relâchement (Township) : court, sans slop, sans long press / pan caméra.
    /// À appeler en <c>Update</c> avant le <c>LateUpdate</c> caméra qui reset l'état.
    /// </summary>
    public static bool TryGetPrimaryTapOnRelease(out Vector2 screenPosition, out int pointerId)
    {
        screenPosition = default;
        pointerId = MousePointerId;

        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen == null)
            return false;

        TouchControl primaryTouch = touchscreen.primaryTouch;
        if (!primaryTouch.press.wasReleasedThisFrame)
            return false;

        pointerId = primaryTouch.touchId.ReadValue();
        screenPosition = primaryTouch.position.ReadValue();

        if (FarmPointerInput.IsOverUi(pointerId))
        {
            ResetLongPressPan();
            return false;
        }

        bool sameFrameTap = primaryTouch.press.wasPressedThisFrame;
        bool wasTracking = longPressTracking;
        bool panFired = longPressFired;
        bool consumed = IsPrimaryPointerConsumedByCamera;
        float startTime = longPressStartTime;
        Vector2 startScreen = longPressStartScreen;

        // Toujours clôturer la session au relâchement (avant Tick / LateUpdate caméra).
        ResetLongPressPan();

        if (sameFrameTap)
            return true;

        if (!wasTracking || panFired || consumed)
            return false;

        float elapsed = Time.unscaledTime - startTime;
        if (elapsed >= LongPressSeconds)
            return false;

        if (Vector2.Distance(screenPosition, startScreen) > TapSlopPixels)
            return false;

        return true;
    }

    /// <summary>Met à jour le suivi long press tant que le doigt est posé (ne pas appeler au relâchement avant le tap).</summary>
    public static void TickLongPressPanTracking()
    {
        if (Touchscreen.current == null)
        {
            ResetLongPressPan();
            return;
        }

        TouchControl primaryTouch = Touchscreen.current.primaryTouch;
        if (!primaryTouch.press.isPressed)
            return;

        TryGetLongPressPanScreenPosition(out _);
    }

    private static void ResetLongPressPan()
    {
        longPressTracking = false;
        longPressFired = false;
        IsPrimaryPointerConsumedByCamera = false;
    }

    private static int CountActiveTouches(Touchscreen touchscreen)
    {
        int count = 0;
        foreach (TouchControl touch in touchscreen.touches)
        {
            if (touch.isInProgress)
                count++;
        }

        return count;
    }

    private static bool TryReadTwoTouches(Touchscreen touchscreen, out Vector2 first, out Vector2 second)
    {
        first = default;
        second = default;
        int count = 0;
        foreach (TouchControl touch in touchscreen.touches)
        {
            if (!touch.isInProgress)
                continue;

            int touchId = touch.touchId.ReadValue();
            if (FarmPointerInput.IsOverUi(touchId))
                return false;

            if (count == 0)
                first = touch.position.ReadValue();
            else
                second = touch.position.ReadValue();

            count++;
            if (count >= MinPinchTouches)
                return true;
        }

        return false;
    }
}
