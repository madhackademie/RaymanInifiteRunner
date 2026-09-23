using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Détection hybride PC / tactile pour l'UI (overlays virtuels, hints).
/// Ne remplace pas la lecture gameplay — voir FarmPointerInput vs InputAction (map Player).
/// </summary>
public static class GameControlMode
{
    public enum ControlKind
    {
        KeyboardMouse,
        Gamepad,
        Touch,
        Unknown
    }

    /// <summary>Vrai si l'UI devrait proposer des contrôles à l'écran (mobile / tactile).</summary>
    public static bool PrefersTouchUi => ResolveKind() == ControlKind.Touch;

    public static ControlKind CurrentKind => ResolveKind();

    public static event Action<ControlKind> ModeChanged;

    private static ControlKind lastKind = ControlKind.Unknown;
    private static bool listening;

    /// <summary>À appeler une fois au bootstrap (ex. UIManager ou loader runner).</summary>
    public static void EnsureListening()
    {
        if (listening)
            return;

        listening = true;
        InputSystem.onDeviceChange += OnDeviceChange;
        PublishIfChanged();
    }

    public static void StopListening()
    {
        if (!listening)
            return;

        InputSystem.onDeviceChange -= OnDeviceChange;
        listening = false;
        lastKind = ControlKind.Unknown;
    }

    private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change is InputDeviceChange.Added or InputDeviceChange.Removed or InputDeviceChange.Enabled
            or InputDeviceChange.Disabled)
            PublishIfChanged();
    }

    private static void PublishIfChanged()
    {
        ControlKind kind = ResolveKind();
        if (kind == lastKind)
            return;

        lastKind = kind;
        ModeChanged?.Invoke(kind);
    }

    private static ControlKind ResolveKind()
    {
        if (Application.isMobilePlatform)
            return ControlKind.Touch;

        Touchscreen touchscreen = Touchscreen.current;
        bool touchHeld = touchscreen != null && touchscreen.primaryTouch.press.isPressed;

        Keyboard keyboard = Keyboard.current;
        bool keyHeld = keyboard != null && keyboard.anyKey.isPressed;

        Mouse mouse = Mouse.current;
        bool mouseHeld = mouse != null &&
                         (mouse.leftButton.isPressed || mouse.rightButton.isPressed || mouse.middleButton.isPressed);

        Gamepad pad = Gamepad.current;
        bool padHeld = pad != null && (pad.leftStick.ReadValue().sqrMagnitude > 0.04f ||
                                       pad.rightStick.ReadValue().sqrMagnitude > 0.04f ||
                                       pad.buttonSouth.isPressed || pad.buttonEast.isPressed ||
                                       pad.buttonWest.isPressed || pad.buttonNorth.isPressed);

        if (padHeld)
            return ControlKind.Gamepad;

        if (touchHeld && !keyHeld && !mouseHeld)
            return ControlKind.Touch;

        if (keyHeld || mouseHeld)
            return ControlKind.KeyboardMouse;

        if (touchscreen != null && mouse == null && keyboard == null)
            return ControlKind.Touch;

        if (mouse != null || keyboard != null)
            return ControlKind.KeyboardMouse;

        if (pad != null)
            return ControlKind.Gamepad;

        return ControlKind.Unknown;
    }
}
