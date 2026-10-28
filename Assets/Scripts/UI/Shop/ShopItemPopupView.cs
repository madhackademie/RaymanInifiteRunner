using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vue UI de la popup shop item.
/// Affichage uniquement : aucun calcul métier ni achat réel ici.
/// </summary>
public sealed class ShopItemPopupView : MonoBehaviour
{
    private const string DefaultConfirmMessage = "Confirmer l'achat ?";
    private const string DefaultSellConfirmMessage = "Confirmer la vente ?";
    private const string DefaultDropConfirmMessage = "Voulez-vous vraiment jeter ces ressources ?";
    private const string IsOpenAnimatorBool = "IsOpen";
    private const string DropToTrashTrigger = "DropToTrash";
    private const string BackdropChildName = "Backdrop";
    private const float DefaultBackdropFadeDuration = 0.2f;
    private const float DefaultCloseAnimDuration = 0.18f;
    private const float DefaultBackdropTargetAlpha = 0.75f;
    private const float DefaultDropTrashDuration = 0.85f;

    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Transition (backdrop fade + card slide)")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Animator animator;
    [SerializeField] private Image backdropImage;
    [Tooltip("Alpha cible du voile sombre (pas de fade sur la carte).")]
    [SerializeField] private float backdropTargetAlpha = DefaultBackdropTargetAlpha;
    [SerializeField] private float backdropFadeDuration = DefaultBackdropFadeDuration;
    [SerializeField] private float closeAnimDuration = DefaultCloseAnimDuration;
    [SerializeField] private bool useBackdropFade = true;

    [Header("Item")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text rarityText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Quantity / Price")]
    [SerializeField] private GameObject quantityRowRoot;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_InputField quantityInputField;
    [SerializeField] private TMP_Text confirmButtonText;
    [SerializeField] private GameObject primaryConfirmButtonRoot;

    [Header("Buttons")]
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button maxButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [Header("Wallet (solde monnaie)")]
    [Tooltip("Affiche le solde PrimaryCurrency pendant l'achat. Utilise CurrencyBalanceUI.")]
    [SerializeField] private CurrencyBalanceUI walletBalance;

    [SerializeField] private GameObject walletRoot;

    [Header("Confirmation overlay (optionnel)")]
    [SerializeField] private GameObject confirmOverlayRoot;
    [SerializeField] private TMP_Text confirmMessageText;
    [SerializeField] private TMP_Text confirmTotalText;
    [SerializeField] private Button confirmPurchaseButton;
    [SerializeField] private Button confirmCancelButton;

    [Header("Drop trash (optionnel — inventaire / Bezy)")]
    [Tooltip("Racine visuelle poubelle + icône volante. Inactive hors anim drop.")]
    [SerializeField] private GameObject dropTrashRoot;
    [SerializeField] private Image dropFlyingIcon;
    [Tooltip("Animator avec trigger DropToTrash (clip icône → poubelle). Si vide, réutilise animator carte.")]
    [SerializeField] private Animator dropTrashAnimator;
    [SerializeField] private float dropTrashDuration = DefaultDropTrashDuration;

    [Tooltip("Départ du vol (défaut : itemIcon).")]
    [SerializeField] private RectTransform dropFlightStartAnchor;
    [Tooltip("Arrivée du vol (défaut : enfant TrashBin sous dropTrashRoot).")]
    [SerializeField] private RectTransform dropFlightEndAnchor;

    private const string DropTrashBinName = "TrashBin";
    private const float DropFlightFadeStart = 0.94f;
    private const float DropFlightShrinkStart = 0.6f;
    private const float DropFlightStartScale = 1f;
    private const float DropLandingHoldDuration = 0.25f;
    private const float DropLandingStartScale = 0.45f;
    private const float DropLandingEndScale = 0.15f;
    private bool isDropLandingActive;
    private float dropLandingProgress;
    private const float DropFlightMinArc = 40f;
    private const float DropFlightArcRatio = 0.25f;

    private bool isDropFlightActive;
    private float dropFlightProgress;
    private Vector2 dropFlightStart;
    private Vector2 dropFlightControl;
    private Vector2 dropFlightEnd;

    private bool suppressQuantityInputEvent;
    private Coroutine transitionRoutine;
    private Coroutine dropTrashRoutine;
    private bool isVisible;
    private bool isTransitioning;
    private Color backdropBaseColor = Color.black;

    public Action OnPlusClicked;
    public Action OnMinusClicked;
    public Action OnMaxClicked;
    public Action OnConfirmClicked;
    public Action OnCloseClicked;
    public Action<string> OnQuantityInputSubmitted;
    public Action OnConfirmPurchaseClicked;
    public Action OnConfirmCancelClicked;

    public bool HasConfirmOverlay => confirmOverlayRoot != null;

    /// <summary>True si Bezy a câblé une racine poubelle (anim drop possible).</summary>
    public bool HasDropTrashAnimation => dropTrashRoot != null;

    /// <summary>
    /// Ancre VFX monnaie : bouton Confirmer (overlay) → bouton Valider → MoneyBurstAnchor Bezy.
    /// </summary>
    public RectTransform ResolveMoneyBurstAnchor()
    {
        if (confirmPurchaseButton != null)
            return confirmPurchaseButton.transform as RectTransform;

        if (confirmButton != null)
            return confirmButton.transform as RectTransform;

        Transform marked = transform.Find("Root/MoneyBurstAnchor");
        if (marked != null)
            return marked as RectTransform;

        return transform as RectTransform;
    }

    private void Awake()
    {
        ResolveTransitionRefs();
        ResolveWalletReferences();
        BindButtons();
        BindQuantityInput();
        // Carte toujours opaque : seul le voile fade. CanvasGroup reste à 1.
        ApplyCanvasGroupInteractable(false);
        SetBackdropAlpha(0f);
        SetAnimatorOpen(false);
        ResetDropTrashVisuals();
    }

    private void OnDestroy()
    {
        UnbindButtons();
        UnbindQuantityInput();
        StopTransitionRoutine();
        StopDropTrashRoutine();
    }

    public void SetItemVisuals(ShopItemPopupData data)
    {
        if (data == null)
            return;

        if (itemIcon != null)
            itemIcon.sprite = data.Icon;

        if (itemNameText != null)
            itemNameText.text = data.DisplayName;

        if (rarityText != null)
            rarityText.text = data.RarityLabel;

        if (descriptionText != null)
            descriptionText.text = data.Description;
    }

    public void SetQuantity(int quantity)
    {
        int value = Mathf.Max(0, quantity);

        if (quantityText != null)
            quantityText.text = $"x{value}";

        if (quantityInputField != null)
        {
            suppressQuantityInputEvent = true;
            quantityInputField.SetTextWithoutNotify(value.ToString());
            suppressQuantityInputEvent = false;
        }
    }

    public void SetTotalPrice(
        int totalPrice,
        ShopItemPopupFlowMode flowMode = ShopItemPopupFlowMode.Purchase,
        int quantity = 0)
    {
        if (confirmButtonText == null)
            return;

        if (flowMode == ShopItemPopupFlowMode.Drop)
        {
            confirmButtonText.text = $"Jeter x{Mathf.Max(1, quantity)}";
            return;
        }

        int value = Mathf.Max(0, totalPrice);
        confirmButtonText.text = flowMode switch
        {
            ShopItemPopupFlowMode.Sell => $"Vendre {value}",
            ShopItemPopupFlowMode.Research => "Lancer la recherche",
            _ => $"Acheter {value}"
        };
    }

    public void SetQuantityControlsVisible(bool visible)
    {
        ResolveQuantityRowIfNeeded();

        if (quantityRowRoot != null)
            quantityRowRoot.SetActive(visible);

        if (primaryConfirmButtonRoot != null)
            primaryConfirmButtonRoot.SetActive(visible);
        else if (confirmButton != null)
            confirmButton.gameObject.SetActive(visible);
    }

    public void SetConfirmMessageForFlow(ShopItemPopupFlowMode flowMode)
    {
        if (confirmMessageText == null)
            return;

        confirmMessageText.text = flowMode switch
        {
            ShopItemPopupFlowMode.Sell => DefaultSellConfirmMessage,
            ShopItemPopupFlowMode.Drop => DefaultDropConfirmMessage,
            ShopItemPopupFlowMode.Research => DefaultConfirmMessage,
            _ => DefaultConfirmMessage
        };
    }

    public void SetWalletVisible(bool visible)
    {
        ResolveWalletReferences();

        if (walletRoot != null)
            walletRoot.SetActive(visible);
    }

    public void SetConfirmButtonLabel(string label)
    {
        if (confirmButtonText != null)
            confirmButtonText.text = label;
    }

    public void SetConfirmInteractable(bool interactable)
    {
        if (confirmButton != null)
            confirmButton.interactable = interactable;

        if (maxButton != null)
            maxButton.interactable = interactable;

        if (confirmPurchaseButton != null)
            confirmPurchaseButton.interactable = interactable;

        // Annuler / croix overlay : toujours cliquable (même si or insuffisant).
        if (confirmCancelButton != null)
            confirmCancelButton.interactable = true;
    }

    public void ShowConfirmOverlay(
        int totalPrice,
        ShopItemPopupFlowMode flowMode = ShopItemPopupFlowMode.Purchase,
        int quantity = 0)
    {
        if (confirmOverlayRoot == null)
            return;

        if (flowMode == ShopItemPopupFlowMode.Research)
        {
            if (confirmMessageText != null)
            {
                confirmMessageText.text =
                    $"Voulez-vous lancer la recherche et payer {Mathf.Max(0, totalPrice)} gold ?";
            }
        }
        else
        {
            SetConfirmMessageForFlow(flowMode);
        }

        if (confirmTotalText != null)
        {
            confirmTotalText.text = flowMode switch
            {
                ShopItemPopupFlowMode.Drop => $"Quantité : {Mathf.Max(1, quantity)}",
                ShopItemPopupFlowMode.Research => $"Coût : {Mathf.Max(0, totalPrice)} gold",
                _ => $"Total : {Mathf.Max(0, totalPrice)}"
            };
        }

        if (flowMode == ShopItemPopupFlowMode.Drop)
            SetWalletVisible(false);
        else
            RefreshWallet();

        confirmOverlayRoot.SetActive(true);
        confirmOverlayRoot.transform.SetAsLastSibling();
        SetCloseButtonVisible(false);
    }

    public void HideConfirmOverlay()
    {
        HideConfirmOverlay(restoreCloseButton: true);
    }

    private void HideConfirmOverlay(bool restoreCloseButton)
    {
        if (confirmOverlayRoot != null)
            confirmOverlayRoot.SetActive(false);

        if (restoreCloseButton)
            SetCloseButtonVisible(true);
    }

    private void SetCloseButtonVisible(bool visible)
    {
        if (closeButton != null)
            closeButton.gameObject.SetActive(visible);
    }

    /// <summary>
    /// Joue l'anim « item → poubelle » (Bezy : trigger <c>DropToTrash</c>), puis <paramref name="onComplete"/>.
    /// Sans racine câblée : callback immédiat.
    /// </summary>
    public void PlayDropTrashAnimation(Sprite icon, Action onComplete)
    {
        StopDropTrashRoutine();

        if (!HasDropTrashAnimation)
        {
            onComplete?.Invoke();
            return;
        }

        if (dropFlyingIcon != null)
        {
            dropFlyingIcon.sprite = icon;
            dropFlyingIcon.enabled = icon != null;
            Color flyingColor = dropFlyingIcon.color;
            flyingColor.a = 1f;
            dropFlyingIcon.color = flyingColor;
            // Repart toujours à taille pleine (le clip ne pilote plus l'échelle de l'icône).
            dropFlyingIcon.rectTransform.localScale = new Vector3(DropFlightStartScale, DropFlightStartScale, 1f);
        }

        dropTrashRoot.SetActive(true);

        Animator trashAnimator = dropTrashAnimator != null ? dropTrashAnimator : animator;
        if (trashAnimator != null)
            trashAnimator.SetTrigger(DropToTrashTrigger);

        dropTrashRoutine = StartCoroutine(DropTrashRoutine(onComplete));
    }

    public void ResetDropTrashVisuals()
    {
        StopDropTrashRoutine();

        if (dropTrashRoot != null)
            dropTrashRoot.SetActive(false);

        if (dropFlyingIcon != null)
        {
            dropFlyingIcon.enabled = false;
            dropFlyingIcon.rectTransform.localScale = new Vector3(DropFlightStartScale, DropFlightStartScale, 1f);
        }
    }

    public void RefreshWallet()
    {
        ResolveWalletReferences();

        if (walletRoot != null)
            walletRoot.SetActive(true);

        walletBalance?.Refresh();
    }

    public void Show()
    {
        if (isTransitioning && isVisible)
            return;

        HideConfirmOverlay();
        ResetDropTrashVisuals();
        RefreshWallet();
        EnsureContentActive();

        isVisible = true;
        ApplyCanvasGroupInteractable(true);
        SetAnimatorOpen(true);
        StartBackdropFade(backdropTargetAlpha);
    }

    public void Hide()
    {
        if (!isVisible && !IsContentActive())
            return;

        if (isTransitioning && !isVisible)
            return;

        HideConfirmOverlay(restoreCloseButton: false);
        ResetDropTrashVisuals();
        isVisible = false;
        ApplyCanvasGroupInteractable(false);
        SetAnimatorOpen(false);
        StartBackdropFade(0f, FinishHide);
    }

    private void FinishHide()
    {
        SetContentActive(false);
    }

    private void EnsureContentActive()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        SetContentActive(true);
    }

    private void SetContentActive(bool active)
    {
        if (root != null)
        {
            root.SetActive(active);
            return;
        }

        if (!active && gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private bool IsContentActive()
    {
        if (root != null)
            return root.activeSelf;

        return gameObject.activeSelf;
    }

    private void ResolveTransitionRefs()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (backdropImage == null)
            backdropImage = FindBackdropImage();

        if (backdropImage != null)
        {
            backdropBaseColor = backdropImage.color;
            // Conserve l'alpha designer du prefab (ex. 0.75).
            if (backdropBaseColor.a > 0.01f)
                backdropTargetAlpha = backdropBaseColor.a;
        }

        // La carte ne doit jamais hériter d'un alpha CanvasGroup < 1.
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private Image FindBackdropImage()
    {
        if (root == null)
            return null;

        Transform backdrop = root.transform.Find(BackdropChildName);
        return backdrop != null ? backdrop.GetComponent<Image>() : null;
    }

    private void SetAnimatorOpen(bool isOpen)
    {
        if (animator == null)
            return;

        animator.SetBool(IsOpenAnimatorBool, isOpen);
    }

    private void StartBackdropFade(float targetAlpha, Action onComplete = null)
    {
        StopTransitionRoutine();
        transitionRoutine = StartCoroutine(BackdropFadeRoutine(targetAlpha, onComplete));
    }

    private IEnumerator BackdropFadeRoutine(float targetAlpha, Action onComplete)
    {
        isTransitioning = true;

        if (!useBackdropFade || backdropImage == null)
        {
            SetBackdropAlpha(targetAlpha);
            if (targetAlpha <= 0.01f)
                yield return WaitCloseAnimIfNeeded();

            isTransitioning = false;
            onComplete?.Invoke();
            yield break;
        }

        float start = backdropImage.color.a;
        float duration = Mathf.Max(0.01f, backdropFadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Ease in-out : voile plus doux qu'un lerp linéaire.
            float eased = t * t * (3f - 2f * t);
            SetBackdropAlpha(Mathf.Lerp(start, targetAlpha, eased));
            yield return null;
        }

        SetBackdropAlpha(targetAlpha);

        if (targetAlpha <= 0.01f)
            yield return WaitCloseAnimIfNeeded();

        isTransitioning = false;
        onComplete?.Invoke();
    }

    private IEnumerator WaitCloseAnimIfNeeded()
    {
        float remaining = Mathf.Max(0f, closeAnimDuration - backdropFadeDuration);
        if (remaining <= 0f)
            yield break;

        float elapsed = 0f;
        while (elapsed < remaining)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void SetBackdropAlpha(float alpha)
    {
        if (backdropImage == null)
            return;

        Color color = backdropBaseColor;
        color.a = Mathf.Clamp01(alpha);
        backdropImage.color = color;
    }

    private void ApplyCanvasGroupInteractable(bool interactable)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = interactable;
        canvasGroup.interactable = interactable;
    }

    private void StopTransitionRoutine()
    {
        if (transitionRoutine == null)
            return;

        StopCoroutine(transitionRoutine);
        transitionRoutine = null;
        isTransitioning = false;
    }

    private IEnumerator DropTrashRoutine(Action onComplete)
    {
        float duration = Mathf.Max(0.05f, dropTrashDuration);
        float elapsed = 0f;
        bool hasFlight = PrepareDropFlight();
        if (hasFlight)
            ApplyDropFlight(0f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (hasFlight)
                ApplyDropFlight(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        if (hasFlight)
        {
            ApplyDropFlight(1f);
            isDropFlightActive = false;

            // L'icône reste sur le compost : shrink final + fondu pendant ce maintien.
            isDropLandingActive = true;
            float landingElapsed = 0f;
            while (landingElapsed < DropLandingHoldDuration)
            {
                landingElapsed += Time.unscaledDeltaTime;
                dropLandingProgress = Mathf.Clamp01(landingElapsed / DropLandingHoldDuration);
                ApplyDropLanding();
                yield return null;
            }

            isDropLandingActive = false;
        }

        isDropFlightActive = false;
        if (dropFlyingIcon != null)
            dropFlyingIcon.enabled = false;

        dropTrashRoutine = null;
        onComplete?.Invoke();
    }

    private void LateUpdate()
    {
        // Réapplique le chemin après l'Animator pour que le clip n'écrase pas la position/alpha.
        if (isDropFlightActive)
            ApplyDropFlight(dropFlightProgress);
        else if (isDropLandingActive)
            ApplyDropLanding();
    }

    /// <summary>Phase d'atterrissage : l'icône reste sur le tas, rétrécit puis disparaît.</summary>
    private void ApplyDropLanding()
    {
        if (dropFlyingIcon == null)
            return;

        float scale = Mathf.Lerp(DropLandingStartScale, DropLandingEndScale, dropLandingProgress);
        dropFlyingIcon.rectTransform.localScale = new Vector3(scale, scale, 1f);

        Color color = dropFlyingIcon.color;
        color.a = 1f - dropLandingProgress;
        dropFlyingIcon.color = color;
    }

    /// <summary>Calcule départ / arrivée / contrôle (espace local du parent de l'icône volante).</summary>
    private bool PrepareDropFlight()
    {
        isDropFlightActive = false;
        if (dropFlyingIcon == null || !dropFlyingIcon.enabled)
            return false;

        RectTransform flying = dropFlyingIcon.rectTransform;
        RectTransform parent = flying.parent as RectTransform;
        if (parent == null)
            return false;

        RectTransform start = dropFlightStartAnchor != null
            ? dropFlightStartAnchor
            : (itemIcon != null ? itemIcon.rectTransform : null);
        RectTransform end = dropFlightEndAnchor != null ? dropFlightEndAnchor : FindTrashBin();
        if (start == null || end == null)
            return false;

        dropFlightStart = GetCenterInParentSpace(start, parent);
        dropFlightEnd = GetCenterInParentSpace(end, parent);
        float arcHeight = Mathf.Max(DropFlightMinArc, Vector2.Distance(dropFlightStart, dropFlightEnd) * DropFlightArcRatio);
        dropFlightControl = (dropFlightStart + dropFlightEnd) * 0.5f + Vector2.up * arcHeight;
        dropFlightProgress = 0f;
        isDropFlightActive = true;
        return true;
    }

    private RectTransform FindTrashBin()
    {
        if (dropTrashRoot == null)
            return null;

        Transform[] children = dropTrashRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == DropTrashBinName)
                return children[i] as RectTransform;
        }

        return null;
    }

    private static Vector2 GetCenterInParentSpace(RectTransform target, RectTransform parent)
    {
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);
        Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;
        Vector3 local = parent.InverseTransformPoint(worldCenter);
        return new Vector2(local.x, local.y);
    }

    private void ApplyDropFlight(float t)
    {
        if (dropFlyingIcon == null)
            return;

        dropFlightProgress = t;
        float inv = 1f - t;
        Vector2 position = inv * inv * dropFlightStart + 2f * inv * t * dropFlightControl + t * t * dropFlightEnd;

        RectTransform flying = dropFlyingIcon.rectTransform;
        // localPosition == position dans le repère du parent, indépendant des ancres.
        flying.localPosition = new Vector3(position.x, position.y, flying.localPosition.z);

        // Taille pleine la majeure partie du vol, puis rétrécissement doux à l'approche du compost.
        float shrink = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(DropFlightShrinkStart, 1f, t));
        float scale = Mathf.Lerp(DropFlightStartScale, DropLandingStartScale, shrink);
        flying.localScale = new Vector3(scale, scale, 1f);

        // Alpha reste à 1 pendant tout le vol ; le fondu se fait dans la phase d'atterrissage.
        Color color = dropFlyingIcon.color;
        color.a = 1f;
        dropFlyingIcon.color = color;
    }

    private void StopDropTrashRoutine()
    {
        isDropFlightActive = false;
        isDropLandingActive = false;

        if (dropTrashRoutine == null)
            return;

        StopCoroutine(dropTrashRoutine);
        dropTrashRoutine = null;
    }

    private void BindButtons()
    {
        if (plusButton != null)
            plusButton.onClick.AddListener(HandlePlusClicked);

        if (minusButton != null)
            minusButton.onClick.AddListener(HandleMinusClicked);

        if (maxButton != null)
            maxButton.onClick.AddListener(HandleMaxClicked);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(HandleConfirmClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(HandleCloseClicked);

        if (confirmPurchaseButton != null)
            confirmPurchaseButton.onClick.AddListener(HandleConfirmPurchaseClicked);

        if (confirmCancelButton != null)
            confirmCancelButton.onClick.AddListener(HandleConfirmCancelClicked);
    }

    private void UnbindButtons()
    {
        if (plusButton != null)
            plusButton.onClick.RemoveListener(HandlePlusClicked);

        if (minusButton != null)
            minusButton.onClick.RemoveListener(HandleMinusClicked);

        if (maxButton != null)
            maxButton.onClick.RemoveListener(HandleMaxClicked);

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(HandleConfirmClicked);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(HandleCloseClicked);

        if (confirmPurchaseButton != null)
            confirmPurchaseButton.onClick.RemoveListener(HandleConfirmPurchaseClicked);

        if (confirmCancelButton != null)
            confirmCancelButton.onClick.RemoveListener(HandleConfirmCancelClicked);
    }

    private void BindQuantityInput()
    {
        if (quantityInputField == null)
            return;

        quantityInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        quantityInputField.onEndEdit.AddListener(HandleQuantityInputEndEdit);
    }

    private void UnbindQuantityInput()
    {
        if (quantityInputField == null)
            return;

        quantityInputField.onEndEdit.RemoveListener(HandleQuantityInputEndEdit);
    }

    private void HandleQuantityInputEndEdit(string text)
    {
        if (suppressQuantityInputEvent)
            return;

        OnQuantityInputSubmitted?.Invoke(text);
    }

    private void HandlePlusClicked() => OnPlusClicked?.Invoke();
    private void HandleMinusClicked() => OnMinusClicked?.Invoke();
    private void HandleMaxClicked() => OnMaxClicked?.Invoke();
    private void HandleConfirmClicked() => OnConfirmClicked?.Invoke();
    private void HandleCloseClicked() => OnCloseClicked?.Invoke();
    private void HandleConfirmPurchaseClicked() => OnConfirmPurchaseClicked?.Invoke();
    private void HandleConfirmCancelClicked() => OnConfirmCancelClicked?.Invoke();

    private void ResolveWalletReferences()
    {
        if (walletBalance == null)
            walletBalance = GetComponentInChildren<CurrencyBalanceUI>(true);

        if (walletRoot == null && walletBalance != null)
            walletRoot = walletBalance.gameObject;

        if (walletBalance == null)
            return;

        if (walletBalance.CurrencyItem != null)
            return;

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory?.ItemDatabase?.PrimaryCurrency != null)
            walletBalance.SetCurrencyItem(inventory.ItemDatabase.PrimaryCurrency);
    }

    private void ResolveQuantityRowIfNeeded()
    {
        if (quantityRowRoot != null)
            return;

        Transform quantityRow = transform.Find("Root/QuantityRow");
        if (quantityRow != null)
            quantityRowRoot = quantityRow.gameObject;

        if (primaryConfirmButtonRoot == null && confirmButton != null)
            primaryConfirmButtonRoot = confirmButton.gameObject;
    }
}
