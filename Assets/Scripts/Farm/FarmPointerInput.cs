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
        if (touchscreen != null)
        {
            var press = touchscreen.primaryTouch.press;
            if (press.isPressed || press.wasReleasedThisFrame)
            {
                screenPosition = touchscreen.primaryTouch.position.ReadValue();
                return true;
            }
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

    /// <summary>
    /// Doigt prioritaire s'il est posé ou vient d'être relâché, sinon clic gauche.
    /// </summary>
    public static bool TryReadPrimaryStroke(
        out bool pressedThisFrame,
        out bool held,
        out bool released,
        out bool overUi)
    {
        pressedThisFrame = false;
        held = false;
        released = false;
        overUi = false;

        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen != null)
        {
            var press = touchscreen.primaryTouch.press;
            if (press.isPressed || press.wasReleasedThisFrame)
            {
                int pointerId = touchscreen.primaryTouch.touchId.ReadValue();
                pressedThisFrame = press.wasPressedThisFrame;
                held = press.isPressed;
                released = press.wasReleasedThisFrame;
                overUi = IsOverUi(pointerId);
                return true;
            }
        }

        Mouse mouse = Mouse.current;
        if (mouse == null)
            return false;

        pressedThisFrame = mouse.leftButton.wasPressedThisFrame;
        held = mouse.leftButton.isPressed;
        released = mouse.leftButton.wasReleasedThisFrame;
        overUi = IsOverUi(MousePointerId);
        return true;
    }

    /// <summary>Souris active, sans doigt posé. Le tactile garde son propre geste.</summary>
    public static bool IsMouseDriven()
    {
        if (Mouse.current == null)
            return false;

        Touchscreen touchscreen = Touchscreen.current;
        return touchscreen == null || !touchscreen.primaryTouch.press.isPressed;
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

        // Tactile : uniquement le fingerId (sans id = faux positifs fréquents sur mobile).
        if (pointerId >= 0)
            return eventSystem.IsPointerOverGameObject(pointerId);

        return eventSystem.IsPointerOverGameObject();
    }
}
