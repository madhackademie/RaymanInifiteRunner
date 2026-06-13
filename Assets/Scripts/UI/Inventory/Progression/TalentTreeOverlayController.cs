using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Overlay arbre de talents au-dessus de l'inventaire (meme ecran, pas de changement de scene).
/// </summary>
public class TalentTreeOverlayController : MonoBehaviour
{
    private const string MissingServiceMessage =
        "Service de talents absent.\nAjoute TalentProgressionService dans la scene.";

    private const string MissingTreePrefabMessage =
        "Arbre visuel a venir pour cette piste.\nLe resume texte reste disponible ci-dessous.";

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

    [Header("Arbre visuel (swap prefab par piste)")]
    [SerializeField] private RectTransform treeContentHost;
    [SerializeField] private TalentTrackPrefabBinding[] trackPrefabBindings = Array.Empty<TalentTrackPrefabBinding>();
    [SerializeField] private bool hidePlaceholderWhenTreeVisible = true;
    [SerializeField] private bool hideFallbackPurchaseWhenTreeVisible = true;

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
    private TalentTreeLayoutRoot activeLayoutRoot;
    private GameObject activeTreeInstance;
    private bool isVisualTreeActive;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(Close);

        ResolveTreeContentHost();
        ResolveProgressionService();
        EnsureRuntimePurchaseButton();
        BindPurchaseButton();
        RegisterPurchaseClickHandler();
        SubscribeProgressionEvents();

        ApplyCanvasGroupState(0f);
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(Close);

        UnsubscribeProgressionEvents();
        UnregisterPurchaseClickHandler();
        DestroyRuntimePurchaseButton();
        ClearActiveTreeInstance();
    }

    public void Open(string trackId)
    {
        if (isTransitioning || string.IsNullOrEmpty(trackId))
            return;

        EnsureOverlayRootActive();

        CurrentTrackId = trackId;
        IsOpen = true;
        lastPurchaseFeedback = null;
        MountTrackVisual(trackId);
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
        ClearActiveTreeInstance();
        isVisualTreeActive = false;

        if (overlayRoot != null)
            overlayRoot.SetActive(false);

        Closed?.Invoke();
    }

    private void MountTrackVisual(string trackId)
    {
        ClearActiveTreeInstance();
        isVisualTreeActive = false;

        if (!TryGetTreePrefab(trackId, out TalentTreeLayoutRoot prefab))
            return;

        if (treeContentHost == null)
        {
            Debug.LogWarning(
                "[TalentTreeOverlayController] treeContentHost absent — arbre visuel non monte.");
            return;
        }

        activeTreeInstance = Instantiate(prefab.gameObject, treeContentHost);
        activeTreeInstance.name = prefab.name;

        RectTransform instanceRect = activeTreeInstance.transform as RectTransform;
        if (instanceRect != null)
        {
            instanceRect.anchorMin = Vector2.zero;
            instanceRect.anchorMax = Vector2.one;
            instanceRect.offsetMin = Vector2.zero;
            instanceRect.offsetMax = Vector2.zero;
            instanceRect.localScale = Vector3.one;
        }

        activeLayoutRoot = activeTreeInstance.GetComponent<TalentTreeLayoutRoot>();
        if (activeLayoutRoot == null)
        {
            Debug.LogWarning(
                $"[TalentTreeOverlayController] Prefab '{prefab.name}' sans TalentTreeLayoutRoot.");
            ClearActiveTreeInstance();
            return;
        }

        if (progressionService != null)
            activeLayoutRoot.Bind(progressionService);

        isVisualTreeActive = true;
    }

    private void ClearActiveTreeInstance()
    {
        if (activeLayoutRoot != null)
        {
            activeLayoutRoot.Unbind();
            activeLayoutRoot = null;
        }

        if (activeTreeInstance != null)
        {
            Destroy(activeTreeInstance);
            activeTreeInstance = null;
        }
    }

    private bool TryGetTreePrefab(string trackId, out TalentTreeLayoutRoot prefab)
    {
        prefab = null;
        for (int i = 0; i < trackPrefabBindings.Length; i++)
        {
            TalentTrackPrefabBinding binding = trackPrefabBindings[i];
            if (!binding.IsValid || binding.TrackId != trackId)
                continue;

            prefab = binding.TreePrefab;
            return prefab != null;
        }

        return false;
    }

    private void RefreshOverlayContent()
    {
        if (trackTitleLabel != null)
            trackTitleLabel.text = GetTrackTitle();

        ApplyPlaceholderVisibility();
        if (bodyPlaceholderLabel != null)
            bodyPlaceholderLabel.text = GetBodyText();

        if (isVisualTreeActive && activeLayoutRoot != null)
            activeLayoutRoot.RefreshAll();

        UpdatePurchaseButtonState();
    }

    private void ApplyPlaceholderVisibility()
    {
        if (bodyPlaceholderLabel == null)
            return;

        bool showPlaceholderText = !isVisualTreeActive || !hidePlaceholderWhenTreeVisible;
        bodyPlaceholderLabel.gameObject.SetActive(showPlaceholderText);

        if (!showPlaceholderText)
            return;

        Transform placeholderRoot = bodyPlaceholderLabel.transform.parent;
        if (placeholderRoot != null)
            placeholderRoot.gameObject.SetActive(true);
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

    private void ResolveTreeContentHost()
    {
        if (treeContentHost != null)
            return;

        if (bodyPlaceholderLabel != null)
            treeContentHost = bodyPlaceholderLabel.transform.parent as RectTransform;
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

        bool hideForVisualTree = isVisualTreeActive && hideFallbackPurchaseWhenTreeVisible;
        if (hideForVisualTree)
        {
            purchaseNextButton.gameObject.SetActive(false);
            return;
        }

        purchaseNextButton.gameObject.SetActive(true);
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
        if (isVisualTreeActive && hidePlaceholderWhenTreeVisible)
            return string.Empty;

        if (progressionService == null || string.IsNullOrEmpty(CurrentTrackId))
            return MissingServiceMessage;

        string summary = progressionService.BuildTrackSummary(CurrentTrackId);
        string actionHint = GetActionHint();
        string missingTreeHint = isVisualTreeActive ? string.Empty : $"\n\n{MissingTreePrefabMessage}";

        if (string.IsNullOrEmpty(lastPurchaseFeedback))
            return $"{summary}\n\n{actionHint}{missingTreeHint}";

        return $"{summary}\n\n{actionHint}{missingTreeHint}\n\n{lastPurchaseFeedback}";
    }

    private string GetActionHint()
    {
        if (isVisualTreeActive)
            return "Clique un noeud disponible pour debloquer la branche.";

        return HasPurchasableNode()
            ? "Appuie sur « Acheter » (en bas a droite) pour debloquer le prochain noeud."
            : "Aucun noeud achetable pour l'instant (points ou pre-requis manquants).";
    }
}
