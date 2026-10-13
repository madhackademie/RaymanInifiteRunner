using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Curseur du pan caméra. Pendant la charge du long press : anneau de charge.
/// Pendant le pan actif (long press tactile démarré ou clic milieu PC) : flèches pulsantes.
/// </summary>
[DefaultExecutionOrder(200)]
public class FarmPanCursor : MonoBehaviour
{
    private const float PulseScaleMin = 0.94f;
    private const float PulseScaleMax = 1.06f;
    private const float PulseAlphaMin = 0.75f;
    private const float PulseAlphaMax = 1f;
    private const float PulsePeriodSeconds = 1f;

    [SerializeField] private Image chargeRing;
    [SerializeField] private SpriteRenderer panArrows;
    [SerializeField] private Camera worldCamera;
    [SerializeField, Range(0.1f, 3f)] private float arrowsScale = 2f;

    private Vector3 arrowsBaseScale = Vector3.one;

    /// <summary>Vrai tant que le curseur pan masque le curseur OS ; les autres curseurs farm ne doivent alors pas écrire Cursor.visible.</summary>
    public static bool HidesSystemCursor { get; private set; }

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (panArrows != null)
            arrowsBaseScale = panArrows.transform.localScale;
    }

    private void OnDisable()
    {
        SetRingVisible(false);
        SetArrowsVisible(false);
        SetSystemCursorVisible(true);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SetSystemCursorVisible(true);
    }

    private void LateUpdate()
    {
        if (FarmCameraInput.TryGetLongPressCharge(out float charge01, out Vector2 chargeScreen))
        {
            SetRingVisible(true);
            SetArrowsVisible(false);
            SetSystemCursorVisible(false);

            if (chargeRing != null)
                chargeRing.fillAmount = charge01;

            FollowScreenPosition(chargeScreen);
            return;
        }

        if (FarmCameraInput.TryGetLongPressPanScreenPosition(out Vector2 panScreen)
            || FarmCameraInput.TryGetPanScreenPosition(out panScreen))
        {
            SetRingVisible(false);
            SetArrowsVisible(true);
            SetSystemCursorVisible(false);
            FollowScreenPosition(panScreen);
            UpdateArrowsPulse();
            return;
        }

        SetRingVisible(false);
        SetArrowsVisible(false);
        SetSystemCursorVisible(true);
    }

    /// <summary>Affiche ou masque le curseur OS ; n'écrit Cursor.visible que si la valeur change.</summary>
    private void SetSystemCursorVisible(bool visible)
    {
        HidesSystemCursor = !visible;
        if (Cursor.visible != visible)
            Cursor.visible = visible;
    }

    /// <summary>Déplace le curseur dans le monde (z = 0) sous la position écran donnée.</summary>
    private void FollowScreenPosition(Vector2 screen)
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        if (worldCamera == null)
            return;

        float depth = Mathf.Abs(worldCamera.transform.position.z);
        Vector3 world = worldCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, depth));
        transform.position = new Vector3(world.x, world.y, 0f);
    }

    /// <summary>Pulse échelle (base x arrowsScale x facteur) et alpha des flèches, temps non scalé, sans Animator.</summary>
    private void UpdateArrowsPulse()
    {
        if (panArrows == null)
            return;

        float phase = Mathf.PingPong(Time.unscaledTime / (PulsePeriodSeconds * 0.5f), 1f);
        float smooth = Mathf.SmoothStep(0f, 1f, phase);
        float scaleFactor = Mathf.Lerp(PulseScaleMin, PulseScaleMax, smooth);
        panArrows.transform.localScale = arrowsBaseScale * arrowsScale * scaleFactor;

        Color color = panArrows.color;
        color.a = Mathf.Lerp(PulseAlphaMin, PulseAlphaMax, smooth);
        panArrows.color = color;
    }

    private void SetRingVisible(bool show)
    {
        if (chargeRing != null && chargeRing.enabled != show)
            chargeRing.enabled = show;
    }

    private void SetArrowsVisible(bool show)
    {
        if (panArrows != null && panArrows.enabled != show)
            panArrows.enabled = show;
    }
}
