using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toast ferme : icône item récolté + montée + fade progressif (feedback succès récolte).
/// Résolu via <see cref="PopupId.FarmHarvestReward"/> + <see cref="ScreenPopupHost"/>.
/// </summary>
public class HarvestRewardFeedbackPopupUI : MonoBehaviour
{
    [Header("Bindings UI")]
    [SerializeField] private RectTransform animatedRoot;
    [SerializeField] private CanvasGroup animatedCanvasGroup;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityLabel;

    [Header("Animation")]
    [SerializeField] [Min(0f)] private float riseDistance = 70f;
    [SerializeField] [Min(0f)] private float riseDuration = 0.85f;
    [SerializeField] [Min(0f)] private float fadeDuration = 0.5f;

    [Header("Position")]
    [Tooltip("Décalage en pixels écran appliqué après projection (ex. 0, 40 = au-dessus de la plante).")]
    [SerializeField] private Vector2 worldOffset = new Vector2(0f, 40f);
    [Tooltip("Caméra gameplay utilisée pour projeter la position monde → écran. Laissez vide pour utiliser Camera.main.")]
    [SerializeField] private Camera gameplayCamera;

    [Header("Behaviour")]
    [SerializeField] private bool hideOnAwake = true;

    private const float FadeStartRatio = 0.6f;

    private Vector2 startAnchoredPosition;
    private Coroutine showCoroutine;

    private void Awake()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (hideOnAwake)
            SetRootVisible(false);
    }

    /// <summary>
    /// Affiche le loot récolté (icône + quantité).
    /// Le toast démarre à la position monde de la plante récoltée, puis monte et disparaît.
    /// </summary>
    public void ShowHarvestReward(Sprite icon, string displayName, int amount, Vector3 worldPosition)
    {
        int resolvedAmount = Mathf.Max(1, amount);
        StopShowCoroutine();

        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = icon != null;
        }

        if (quantityLabel != null)
            quantityLabel.text = "+" + resolvedAmount;

        // Résoudre la position locale depuis worldPosition avant d'activer l'objet.
        if (!TryResolveLocalPosition(worldPosition, out Vector2 localPoint))
        {
            Debug.LogWarning("[HarvestRewardFeedbackPopupUI] Conversion world→local échouée — fallback centre.", this);
            localPoint = Vector2.zero;
        }

        startAnchoredPosition = localPoint;

        if (animatedRoot != null)
            animatedRoot.anchoredPosition = localPoint;

        if (animatedCanvasGroup != null)
            animatedCanvasGroup.alpha = 1f;

        gameObject.SetActive(true);
        SetRootVisible(true);
        transform.SetAsLastSibling();

        showCoroutine = StartCoroutine(AnimateRiseThenHide());
    }

    /// <summary>Masque immédiatement le toast et annule toute animation en cours.</summary>
    public void Hide()
    {
        StopShowCoroutine();
        SetRootVisible(false);
        gameObject.SetActive(false);
    }

    private void ResetAnimatedRoot()
    {
        if (animatedRoot != null)
            animatedRoot.anchoredPosition = startAnchoredPosition;

        if (animatedCanvasGroup != null)
            animatedCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Convertit worldPosition en coordonnées locales du parent de animatedRoot.
    /// Utilise gameplayCamera pour la projection monde → écran, et la caméra UI du Canvas pour la projection écran → rect.
    /// worldOffset est appliqué en pixels écran après projection.
    /// </summary>
    private bool TryResolveLocalPosition(Vector3 worldPosition, out Vector2 localPoint)
    {
        localPoint = Vector2.zero;

        if (animatedRoot == null)
            return false;

        Camera worldCam = gameplayCamera != null ? gameplayCamera : Camera.main;
        if (worldCam == null)
            return false;

        Canvas canvas = animatedRoot.GetComponentInParent<Canvas>(includeInactive: true);
        if (canvas == null)
            return false;

        Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // Projection monde → écran avec la caméra gameplay
        Vector3 screen3 = worldCam.WorldToScreenPoint(worldPosition);
        if (screen3.z < 0f)
            return false; // position derrière la caméra

        // worldOffset en pixels écran
        Vector2 screenPoint = new Vector2(screen3.x, screen3.y) + worldOffset;

        RectTransform parentRect = animatedRoot.parent as RectTransform;
        if (parentRect == null)
            return false;

        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, uiCam, out localPoint);

        Debug.Log($"[HarvestRewardFeedbackPopupUI] screen3={screen3} screenPoint={screenPoint} localPoint={localPoint} success={success}");

        return success;
    }

    private IEnumerator AnimateRiseThenHide()
    {
        if (animatedRoot == null || animatedCanvasGroup == null)
        {
            Hide();
            yield break;
        }

        float fadeDelay = riseDuration * FadeStartRatio;
        float riseElapsed = 0f;

        // Phase montée (+ début fade dès FadeStartRatio)
        while (riseElapsed < riseDuration)
        {
            riseElapsed += Time.deltaTime;

            float riseT   = Mathf.Clamp01(riseElapsed / riseDuration);
            float riseY   = Mathf.SmoothStep(0f, 1f, riseT) * riseDistance;
            animatedRoot.anchoredPosition = new Vector2(startAnchoredPosition.x, startAnchoredPosition.y + riseY);

            if (riseElapsed >= fadeDelay)
            {
                float fadeT = Mathf.Clamp01((riseElapsed - fadeDelay) / fadeDuration);
                animatedCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
            }

            yield return null;
        }

        // Fixer position finale
        animatedRoot.anchoredPosition = new Vector2(startAnchoredPosition.x, startAnchoredPosition.y + riseDistance);

        // Phase fade résiduelle (si fadeDuration dépasse la fin de la montée)
        float fadeElapsed = riseDuration - fadeDelay;
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            animatedCanvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(fadeElapsed / fadeDuration));
            yield return null;
        }

        animatedCanvasGroup.alpha = 0f;
        showCoroutine = null;
        Hide();
    }

    private void StopShowCoroutine()
    {
        if (showCoroutine == null)
            return;

        StopCoroutine(showCoroutine);
        showCoroutine = null;
    }

    private void SetRootVisible(bool visible)
    {
        GameObject target = animatedRoot != null ? animatedRoot.gameObject : gameObject;
        target.SetActive(visible);
    }
}
