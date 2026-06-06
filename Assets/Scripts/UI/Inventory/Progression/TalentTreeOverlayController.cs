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
    private const string MissingServiceMessage =
        "Service de talents absent.\nAjoute TalentProgressionService dans la scene.";

    [Header("Root")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator animator;

    [Header("Contenu")]
    [SerializeField] private TextMeshProUGUI trackTitleLabel;
    [SerializeField] private TextMeshProUGUI bodyPlaceholderLabel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button purchaseNextButton;
    [SerializeField] private bool autoBindPlaceholderAsPurchaseButton = true;

    [Header("Progression (MVP)")]
    [SerializeField] private TalentProgressionService progressionService;
    [SerializeField] private bool autoCreateServiceWhenMissing = true;

    [Header("Transition")]
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private bool useFade = true;

    public bool IsOpen { get; private set; }
    public string CurrentTrackId { get; private set; }

    public event Action Closed;

    private Coroutine fadeRoutine;
    private bool isTransitioning;
    private bool purchaseButtonWasAutoAdded;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(Close);

        ResolveProgressionService();
        BindPurchaseButton();
        SubscribeProgressionEvents();
        HideImmediate();
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(Close);

        UnsubscribeProgressionEvents();
        UnbindPurchaseButton();
    }

    public void Open(string trackId)
    {
        if (isTransitioning || string.IsNullOrEmpty(trackId))
            return;

        CurrentTrackId = trackId;
        IsOpen = true;
        RefreshOverlayContent();
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

    private void RefreshOverlayContent()
    {
        if (trackTitleLabel != null)
            trackTitleLabel.text = GetTrackTitle();

        if (bodyPlaceholderLabel != null)
            bodyPlaceholderLabel.text = GetBodyText();

        UpdatePurchaseButtonState();
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

        UpdatePurchaseButtonState();
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

    private void ResolveProgressionService()
    {
        if (progressionService != null)
            return;

        progressionService = FindFirstObjectByType<TalentProgressionService>();
        if (progressionService != null || !autoCreateServiceWhenMissing)
            return;

        GameObject serviceRoot = new GameObject("TalentProgressionService");
        progressionService = serviceRoot.AddComponent<TalentProgressionService>();
    }

    private void BindPurchaseButton()
    {
        if (purchaseNextButton == null && autoBindPlaceholderAsPurchaseButton && bodyPlaceholderLabel != null)
        {
            purchaseNextButton = bodyPlaceholderLabel.GetComponent<Button>();
            if (purchaseNextButton == null)
            {
                purchaseNextButton = bodyPlaceholderLabel.gameObject.AddComponent<Button>();
                purchaseButtonWasAutoAdded = true;
            }

            purchaseNextButton.targetGraphic = bodyPlaceholderLabel;
        }

        if (purchaseNextButton != null)
            purchaseNextButton.onClick.AddListener(HandlePurchaseNextClicked);
    }

    private void UnbindPurchaseButton()
    {
        if (purchaseNextButton != null)
            purchaseNextButton.onClick.RemoveListener(HandlePurchaseNextClicked);

        if (purchaseButtonWasAutoAdded && purchaseNextButton != null)
            Destroy(purchaseNextButton);
    }

    private void SubscribeProgressionEvents()
    {
        if (progressionService != null)
            progressionService.StateChanged += HandleProgressionStateChanged;
    }

    private void UnsubscribeProgressionEvents()
    {
        if (progressionService != null)
            progressionService.StateChanged -= HandleProgressionStateChanged;
    }

    private void HandleProgressionStateChanged()
    {
        if (IsOpen)
            RefreshOverlayContent();
    }

    private void HandlePurchaseNextClicked()
    {
        if (!IsOpen || progressionService == null || string.IsNullOrEmpty(CurrentTrackId))
            return;

        bool purchased = progressionService.TryPurchaseFirstAvailableNode(CurrentTrackId, out _, out _);
        if (purchased)
            RefreshOverlayContent();
    }

    private void UpdatePurchaseButtonState()
    {
        if (purchaseNextButton == null)
            return;

        bool enabled = IsOpen && progressionService != null && HasPurchasableNode();
        purchaseNextButton.interactable = enabled;
    }

    private bool HasPurchasableNode()
    {
        if (progressionService == null || string.IsNullOrEmpty(CurrentTrackId))
            return false;

        return progressionService.CanPurchaseAnyNode(CurrentTrackId);
    }

    private string GetTrackTitle()
    {
        if (progressionService == null || string.IsNullOrEmpty(CurrentTrackId))
            return CurrentTrackId;

        return progressionService.GetTrackDisplayName(CurrentTrackId);
    }

    private string GetBodyText()
    {
        if (progressionService == null || string.IsNullOrEmpty(CurrentTrackId))
            return MissingServiceMessage;

        string summary = progressionService.BuildTrackSummary(CurrentTrackId);
        return $"{summary}\n\nClique sur ce panneau pour acheter le prochain noeud disponible.";
    }
}
