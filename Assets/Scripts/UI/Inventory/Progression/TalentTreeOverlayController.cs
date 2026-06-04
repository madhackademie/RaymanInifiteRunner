using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Overlay arbre de talents au-dessus de l'inventaire (même écran, pas de changement de scène).
/// </summary>
public class TalentTreeOverlayController : MonoBehaviour
{
    private const string PlaceholderBodyFormat =
        "Arbre talents — placeholder\nPiste : {0}\n(Renommer pistes après notes tablette)";

    [Header("Root")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator animator;

    [Header("Contenu")]
    [SerializeField] private TextMeshProUGUI trackTitleLabel;
    [SerializeField] private TextMeshProUGUI bodyPlaceholderLabel;
    [SerializeField] private Button backButton;

    [Header("Transition")]
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private bool useFade = true;

    public bool IsOpen { get; private set; }
    public string CurrentTrackId { get; private set; }

    public event Action Closed;

    private Coroutine fadeRoutine;
    private bool isTransitioning;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(Close);

        HideImmediate();
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(Close);
    }

    public void Open(string trackId)
    {
        if (isTransitioning || string.IsNullOrEmpty(trackId))
            return;

        CurrentTrackId = trackId;
        IsOpen = true;

        if (trackTitleLabel != null)
            trackTitleLabel.text = trackId;

        if (bodyPlaceholderLabel != null)
            bodyPlaceholderLabel.text = string.Format(PlaceholderBodyFormat, trackId);

        if (overlayRoot != null)
            overlayRoot.SetActive(true);

        PlayAnimatorBool(isOpen: true);
        StartFade(1f);
    }

    public void Close()
    {
        if (isTransitioning || !IsOpen)
            return;

        PlayAnimatorBool(isOpen: false);
        StartFade(0f, onComplete: FinishClose);
    }

    private void FinishClose()
    {
        IsOpen = false;
        CurrentTrackId = null;

        if (overlayRoot != null)
            overlayRoot.SetActive(false);

        Closed?.Invoke();
    }

    private void HideImmediate()
    {
        IsOpen = false;
        CurrentTrackId = null;

        if (overlayRoot != null)
            overlayRoot.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    private void PlayAnimatorBool(bool isOpen)
    {
        if (animator == null)
            return;

        animator.SetBool("IsOpen", isOpen);
    }

    private void StartFade(float targetAlpha, Action onComplete = null)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, onComplete));
    }

    private IEnumerator FadeRoutine(float targetAlpha, Action onComplete)
    {
        isTransitioning = true;

        if (!useFade || canvasGroup == null)
        {
            ApplyCanvasGroupState(targetAlpha);
            isTransitioning = false;
            onComplete?.Invoke();
            yield break;
        }

        float start = canvasGroup.alpha;
        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, t);
            yield return null;
        }

        ApplyCanvasGroupState(targetAlpha);
        isTransitioning = false;
        onComplete?.Invoke();
    }

    private void ApplyCanvasGroupState(float alpha)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = alpha;
        bool visible = alpha > 0.01f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
}
