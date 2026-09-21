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

    public static bool TryGetPanScreenPosition(bool includePinchPan, out Vector2 screenPosition)
    {
        if (includePinchPan && TryGetPinch(out _, out Vector2 pinchMid))
        {
            screenPosition = pinchMid;
            return true;
        }

        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.middleButton.isPressed)
        {
            screenPosition = mouse.position.ReadValue();
            return !FarmPointerInput.IsOverUi(MousePointerId);
        }

        screenPosition = default;
        return false;
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
