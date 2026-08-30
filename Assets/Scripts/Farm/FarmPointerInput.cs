using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Lecture pointeur unifiée souris + tactile pour la ferme.
/// </summary>
public static class FarmPointerInput
{
    public const int MousePointerId = -1;

    /// <summary>Vrai à la frame où l'appui principal démarre (tap ou clic gauche).</summary>
    public static bool TryGetPrimaryPress(out Vector2 screenPosition, out int pointerId)
    {
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
        {
            screenPosition = touchscreen.primaryTouch.position.ReadValue();
            pointerId      = touchscreen.primaryTouch.touchId.ReadValue();
            return true;
        }

        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            screenPosition = mouse.position.ReadValue();
            pointerId      = MousePointerId;
            return true;
        }

        screenPosition = default;
        pointerId      = MousePointerId;
        return false;
    }

    /// <summary>Position écran courante du pointeur.</summary>
    public static bool TryGetScreenPosition(out Vector2 screenPosition)
    {
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            screenPosition = touchscreen.primaryTouch.position.ReadValue();
            return true;
        }

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            screenPosition = mouse.position.ReadValue();
            return true;
        }

        screenPosition = default;
        return false;
    }

    /// <summary>Appui principal maintenu (clic gauche ou doigt posé).</summary>
    public static bool IsPrimaryHeld()
    {
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
            return true;

        Mouse mouse = Mouse.current;
        return mouse != null && mouse.leftButton.isPressed;
    }

    /// <summary>Annulation desktop : clic droit ou Échap.</summary>
    public static bool WasCancelPressed()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.wasPressedThisFrame)
            return true;

        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
    }

    /// <summary>Vrai si le pointeur est au-dessus de l'UI.</summary>
    public static bool IsOverUi(int pointerId)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;

        return eventSystem.IsPointerOverGameObject(pointerId) ||
               eventSystem.IsPointerOverGameObject();
    }
}
