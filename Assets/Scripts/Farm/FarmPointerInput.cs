using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Lecture pointeur unifiée souris + tactile pour la ferme.
/// Point d'entrée unique : <c>Mouse.current</c> est null sur mobile et provoquait
/// une NullReferenceException dans la preview de pose.
/// </summary>
public static class FarmPointerInput
{
    /// <summary>Id de pointeur souris attendu par l'EventSystem (PointerInputModule.kMouseLeftId).</summary>
    public const int MousePointerId = -1;

    /// <summary>
    /// Vrai à la frame où l'appui principal démarre (tap tactile ou clic gauche).
    /// </summary>
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

    /// <summary>
    /// Position écran courante du pointeur. En tactile elle n'existe que pendant
    /// l'appui : le fantôme de pose suit donc le doigt, pas un survol.
    /// </summary>
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

    /// <summary>Annulation desktop : clic droit ou Échap. Sans équivalent tactile.</summary>
    public static bool WasCancelPressed()
    {
        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.wasPressedThisFrame)
            return true;

        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
    }

    /// <summary>
    /// Vrai si le pointeur est au-dessus de l'UI : évite de traverser un popup
    /// pour cliquer la grille en dessous.
    /// </summary>
    public static bool IsOverUi(int pointerId)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;

        // La surcharge sans argument reste consultée : en tactile l'id fourni par
        // le module d'entrée ne correspond pas toujours au touchId.
        return eventSystem.IsPointerOverGameObject(pointerId) ||
               eventSystem.IsPointerOverGameObject();
    }
}
