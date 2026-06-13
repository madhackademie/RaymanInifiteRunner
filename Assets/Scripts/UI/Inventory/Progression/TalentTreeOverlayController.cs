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
    [SerializeField] private bool createRuntimePurchaseButtonWhenMissing = true;

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
    private GameObject runtimePurchaseButtonRoot;
    private string lastPurchaseFeedback;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(Close);

        ResolveProgressionService();
        EnsureRuntimePurchaseButton();
        BindPurchaseButton();
        RegisterPurchaseClickHandler();
        SubscribeProgressionEvents();

        // Prefab déjà inactif (alpha 0) : ne pas appeler HideImmediate ici —
        // SetActive(true) dans Open déclenche Awake et HideImmediate désactivait
        // l'overlay avant StartFade (coroutine impossible sur GO inactif).
        ApplyCanvasGroupState(0f);
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(Close);

        UnsubscribeProgressionEvents();
        UnregisterPurchaseClickHandler();
        DestroyRuntimePurchaseButton();
    }

    public void Open(string trackId)
    {
        if (isTransitioning || string.IsNullOrEmpty(trackId))
            return;

        EnsureOverlayRootActive();

        CurrentTrackId = trackId;
        IsOpen = true;
        lastPurchaseFeedback = null;
        RefreshOverlayContent();
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

    private void EnsureOverlayRootActive()
    {
        if (overlayRoot != null && !overlayRoot.activeSelf)
            overlayRoot.SetActive(true);
    }

    private void StartFade(float targetAlpha, Action onComplete = null)
    {
        EnsureOverlayRootActive();

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

    private void EnsureRuntimePurchaseButton()
    {
        if (purchaseNextButton != null || !createRuntimePurchaseButtonWhenMissing)
            return;

        Transform panel = GetOverlayPanelTransform();
        if (panel == null)
            return;

        runtimePurchaseButtonRoot = new GameObject(
            "PurchaseNextButton",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button));

        runtimePurchaseButtonRoot.transform.SetParent(panel, false);

        RectTransform buttonRect = runtimePurchaseButtonRoot.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1f, 0f);
        buttonRect.anchorMax = new Vector2(1f, 0f);
        buttonRect.pivot = new Vector2(1f, 0f);
        buttonRect.anchoredPosition = new Vector2(-16f, 16f);
        buttonRect.sizeDelta = new Vector2(180f, 40f);

        Image buttonImage = runtimePurchaseButtonRoot.GetComponent<Image>();
        buttonImage.color = new Color(0.12f, 0.35f, 0.18f, 1f);
        buttonImage.raycastTarget = true;

        purchaseNextButton = runtimePurchaseButtonRoot.GetComponent<Button>();
        purchaseNextButton.targetGraphic = buttonImage;

        GameObject labelRoot = new GameObject("Label", typeof(RectTransform));
        labelRoot.transform.SetParent(runtimePurchaseButtonRoot.transform, false);

        RectTransform labelRect = labelRoot.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI label = labelRoot.AddComponent<TextMeshProUGUI>();
        label.text = "Acheter";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 16f;
        label.color = Color.white;
        label.raycastTarget = false;
    }

    private void BindPurchaseButton()
    {
        if (purchaseNextButton != null || !autoBindPlaceholderAsPurchaseButton || bodyPlaceholderLabel == null)
            return;

        GameObject clickRoot = bodyPlaceholderLabel.transform.parent != null
            ? bodyPlaceholderLabel.transform.parent.gameObject
            : bodyPlaceholderLabel.gameObject;

        Image panelImage = clickRoot.GetComponent<Image>();
        if (panelImage != null)
            panelImage.raycastTarget = true;

        bodyPlaceholderLabel.raycastTarget = false;

        purchaseNextButton = clickRoot.GetComponent<Button>();
        if (purchaseNextButton == null)
        {
            purchaseNextButton = clickRoot.AddComponent<Button>();
            purchaseButtonWasAutoAdded = true;
        }

        purchaseNextButton.targetGraphic = panelImage != null ? panelImage : bodyPlaceholderLabel;
    }

    private void RegisterPurchaseClickHandler()
    {
        if (purchaseNextButton != null)
            purchaseNextButton.onClick.AddListener(HandlePurchaseNextClicked);
    }

    private void UnregisterPurchaseClickHandler()
    {
        if (purchaseNextButton != null)
            purchaseNextButton.onClick.RemoveListener(HandlePurchaseNextClicked);

        if (purchaseButtonWasAutoAdded && purchaseNextButton != null)
            Destroy(purchaseNextButton);
    }

    private void DestroyRuntimePurchaseButton()
    {
        if (runtimePurchaseButtonRoot == null)
            return;

        Destroy(runtimePurchaseButtonRoot);
        runtimePurchaseButtonRoot = null;
        purchaseNextButton = null;
    }

    private Transform GetOverlayPanelTransform()
    {
        if (backButton != null)
            return backButton.transform.parent;

        if (bodyPlaceholderLabel != null && bodyPlaceholderLabel.transform.parent != null)
            return bodyPlaceholderLabel.transform.parent.parent;

        return overlayRoot != null ? overlayRoot.transform : transform;
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

        bool purchased = progressionService.TryPurchaseFirstAvailableNode(
            CurrentTrackId,
            out TalentNodeDefinition purchasedNode,
            out string reason);

        lastPurchaseFeedback = purchased
            ? $"Noeud achete : {purchasedNode.DisplayName}."
            : reason;

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
        string actionHint = HasPurchasableNode()
            ? "Appuie sur « Acheter » (en bas a droite) pour debloquer le prochain noeud."
            : "Aucun noeud achetable pour l'instant (points ou pre-requis manquants).";

        if (string.IsNullOrEmpty(lastPurchaseFeedback))
            return $"{summary}\n\n{actionHint}";

        return $"{summary}\n\n{actionHint}\n\n{lastPurchaseFeedback}";
    }
}
