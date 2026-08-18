using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vue UI d'un bandeau canal de vente (prefab Bezy).
/// États : verrouillé (progression), unlockable, recherche, cooldown 24 h, actif.
/// </summary>
public class SaleChannelBandeauView : MonoBehaviour
{
    private const string IsOnCooldownAnimatorBool = "IsOnCooldown";

    private static readonly Color IllustrationActiveColor = Color.white;
    private static readonly Color IllustrationCooldownColor = new(0.55f, 0.55f, 0.55f, 1f);
    private static readonly Color IllustrationLockedColor = new(0.45f, 0.45f, 0.45f, 1f);
    private static readonly Color IllustrationUnlockableColor = new(0.95f, 0.92f, 0.75f, 1f);
    private static readonly Color LockIconDefaultColor = Color.white;
    private static readonly Color LockIconUnlockableColor = new(1f, 0.86f, 0.35f, 1f);

    [Header("Bindings UI (prefab)")]
    [SerializeField] private Button bandeauButton;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private Image[] starImages = new Image[5];
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Image illustrationImage;
    [SerializeField] private string channelId;

    [Header("Cooldown 24 h (Bezy Phase 4)")]
    [SerializeField] private GameObject cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownLabel;
    [SerializeField] private CanvasGroup cooldownCanvasGroup;
    [SerializeField] private Animator animator;

    public event Action<SaleChannelBandeauView> OnBandeauClicked;
    public event Action<SaleChannelBandeauView> OnUnlockResearchRequested;

    public string ChannelId => channelId;
    public string DisplayTitle => titleLabel != null ? titleLabel.text : name;
    public bool IsOnCooldown { get; private set; }
    public SaleChannelProgressionPhase ProgressionPhase { get; private set; } = SaleChannelProgressionPhase.Unlocked;
    public bool ShowsProgressionOverlay =>
        ProgressionPhase is SaleChannelProgressionPhase.Locked
            or SaleChannelProgressionPhase.Unlockable
            or SaleChannelProgressionPhase.ResearchInProgress;
    public bool CanStartUnlockResearch => ProgressionPhase == SaleChannelProgressionPhase.Unlockable;
    public bool IsProgressionBlockingSale => ShowsProgressionOverlay;

    private Image lockIconImage;
    private TextMeshProUGUI statusLabel;
    private RectTransform lockIconRect;
    private Coroutine unlockablePulseRoutine;
    private SaleChannelUnlockProgressSnapshot progressSnapshot;

    private void Awake()
    {
        ResolvePolishRefs();
        ResolveProgressionRefs();

        if (bandeauButton != null)
            bandeauButton.onClick.AddListener(HandleBandeauClicked);

        ApplyInteractableState();
        ApplyCooldownState(false, null);
    }

    private void LateUpdate()
    {
        if (!IsOnCooldown || cooldownCanvasGroup == null)
            return;

        if (cooldownCanvasGroup.alpha < 0.99f)
            cooldownCanvasGroup.alpha = 1f;
    }

    private void OnDestroy()
    {
        StopUnlockablePulse();

        if (bandeauButton != null)
            bandeauButton.onClick.RemoveListener(HandleBandeauClicked);
    }

    public void ApplyProgressionState(SaleChannelUnlockProgressSnapshot snapshot, bool playUnlockableReveal)
    {
        progressSnapshot = snapshot;
        ProgressionPhase = snapshot.Phase;

        ResolveProgressionRefs();

        bool showOverlay = ShowsProgressionOverlay;
        if (lockedOverlay != null)
            lockedOverlay.SetActive(showOverlay);

        if (statusLabel != null)
        {
            statusLabel.gameObject.SetActive(showOverlay);
            statusLabel.text = snapshot.StatusLabel;
            statusLabel.color = ProgressionPhase == SaleChannelProgressionPhase.Unlockable
                ? LockIconUnlockableColor
                : Color.white;
        }

        if (lockIconImage != null)
        {
            lockIconImage.color = ProgressionPhase == SaleChannelProgressionPhase.Unlockable
                ? LockIconUnlockableColor
                : LockIconDefaultColor;
        }

        if (illustrationImage != null && !IsOnCooldown)
        {
            illustrationImage.color = ProgressionPhase switch
            {
                SaleChannelProgressionPhase.Unlockable => IllustrationUnlockableColor,
                SaleChannelProgressionPhase.Unlocked => IllustrationActiveColor,
                _ => IllustrationLockedColor,
            };
        }

        if (ProgressionPhase == SaleChannelProgressionPhase.Unlockable)
        {
            if (playUnlockableReveal)
                PlayUnlockableReveal();
            else
                StartUnlockablePulseLoop();
        }
        else
        {
            StopUnlockablePulse();
            ResetLockIconScale();
        }

        ApplyInteractableState();
    }

    public void ClearProgressionState()
    {
        ProgressionPhase = SaleChannelProgressionPhase.Unlocked;
        progressSnapshot = default;

        if (lockedOverlay != null)
            lockedOverlay.SetActive(false);

        StopUnlockablePulse();
        ResetLockIconScale();
        ApplyInteractableState();
    }

    public bool TryGetProgressSnapshot(out SaleChannelUnlockProgressSnapshot snapshot)
    {
        snapshot = progressSnapshot;
        return ShowsProgressionOverlay && !string.IsNullOrWhiteSpace(snapshot.ChannelId);
    }

    public void RequestUnlockResearch()
    {
        if (!CanStartUnlockResearch)
            return;

        OnUnlockResearchRequested?.Invoke(this);
    }

    public void PlayUnlockableReveal()
    {
        StopUnlockablePulse();
        unlockablePulseRoutine = StartCoroutine(UnlockableRevealRoutine());
    }

    public void ApplyLockedInteractable()
    {
        ApplyInteractableState();
    }

    public void ApplyCooldownState(bool onCooldown, string remainingText)
    {
        ResolvePolishRefs();

        bool shouldShow = onCooldown && !IsProgressionBlockingSale;
        IsOnCooldown = shouldShow;

        if (illustrationImage != null)
        {
            illustrationImage.color = shouldShow
                ? IllustrationCooldownColor
                : ProgressionPhase switch
                {
                    SaleChannelProgressionPhase.Unlockable => IllustrationUnlockableColor,
                    SaleChannelProgressionPhase.Unlocked => IllustrationActiveColor,
                    _ => IllustrationLockedColor,
                };
        }

        if (shouldShow)
            ShowCooldown(remainingText);
        else
            HideCooldown();

        ApplyInteractableState();
    }

    private void ShowCooldown(string remainingText)
    {
        if (cooldownOverlay != null)
            cooldownOverlay.SetActive(true);

        if (cooldownCanvasGroup != null)
            cooldownCanvasGroup.alpha = 1f;

        if (cooldownLabel != null)
        {
            cooldownLabel.gameObject.SetActive(true);
            cooldownLabel.text = string.IsNullOrEmpty(remainingText) ? "…" : remainingText;
        }

        if (animator != null)
            animator.SetBool(IsOnCooldownAnimatorBool, true);
    }

    private void HideCooldown()
    {
        if (animator != null)
            animator.SetBool(IsOnCooldownAnimatorBool, false);

        if (cooldownLabel != null)
        {
            cooldownLabel.text = string.Empty;
            cooldownLabel.gameObject.SetActive(false);
        }

        if (cooldownCanvasGroup != null)
            cooldownCanvasGroup.alpha = 0f;

        if (cooldownOverlay != null)
            cooldownOverlay.SetActive(false);
    }

    private void ResolvePolishRefs()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (cooldownOverlay == null)
            return;

        if (cooldownCanvasGroup == null)
            cooldownCanvasGroup = cooldownOverlay.GetComponent<CanvasGroup>();

        if (cooldownLabel == null)
            cooldownLabel = cooldownOverlay.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void ResolveProgressionRefs()
    {
        if (lockedOverlay == null)
            return;

        if (lockIconImage == null)
        {
            Transform lockIcon = lockedOverlay.transform.Find("LockIcon");
            if (lockIcon != null)
            {
                lockIconImage = lockIcon.GetComponent<Image>();
                lockIconRect = lockIcon as RectTransform;
            }
        }

        if (statusLabel == null)
        {
            Transform label = lockedOverlay.transform.Find("BientotLabel");
            if (label != null)
                statusLabel = label.GetComponent<TextMeshProUGUI>();
        }
    }

    private void ApplyInteractableState()
    {
        if (bandeauButton == null)
            return;

        bool canOpenSale = !IsProgressionBlockingSale && !IsOnCooldown;
        bandeauButton.interactable = canOpenSale || CanStartUnlockResearch;
    }

    private void HandleBandeauClicked()
    {
        if (CanStartUnlockResearch)
        {
            RequestUnlockResearch();
            return;
        }

        if (IsProgressionBlockingSale || IsOnCooldown)
            return;

        OnBandeauClicked?.Invoke(this);
    }

    private IEnumerator UnlockableRevealRoutine()
    {
        const int pulseCount = 3;
        const float pulseDurationSeconds = 0.45f;
        const float scalePeak = 1.18f;

        ResolveProgressionRefs();
        if (lockIconRect == null)
            yield break;

        Vector3 baseScale = Vector3.one;
        lockIconRect.localScale = baseScale;

        for (int i = 0; i < pulseCount; i++)
        {
            yield return AnimateScale(lockIconRect, baseScale, baseScale * scalePeak, pulseDurationSeconds * 0.45f);
            yield return AnimateScale(lockIconRect, baseScale * scalePeak, baseScale, pulseDurationSeconds * 0.55f);
        }

        unlockablePulseRoutine = null;
        StartUnlockablePulseLoop();
    }

    private void StartUnlockablePulseLoop()
    {
        if (ProgressionPhase != SaleChannelProgressionPhase.Unlockable)
            return;

        if (unlockablePulseRoutine != null)
            return;

        unlockablePulseRoutine = StartCoroutine(UnlockableIdlePulseRoutine());
    }

    private IEnumerator UnlockableIdlePulseRoutine()
    {
        const float cycleSeconds = 1.4f;
        const float scalePeak = 1.08f;

        ResolveProgressionRefs();
        if (lockIconRect == null)
            yield break;

        Vector3 baseScale = Vector3.one;

        while (ProgressionPhase == SaleChannelProgressionPhase.Unlockable)
        {
            yield return AnimateScale(lockIconRect, baseScale, baseScale * scalePeak, cycleSeconds * 0.5f);
            yield return AnimateScale(lockIconRect, baseScale * scalePeak, baseScale, cycleSeconds * 0.5f);
        }

        unlockablePulseRoutine = null;
    }

    private static IEnumerator AnimateScale(RectTransform target, Vector3 from, Vector3 to, float durationSeconds)
    {
        if (target == null)
            yield break;

        if (durationSeconds <= 0f)
        {
            target.localScale = to;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < durationSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / durationSeconds);
            target.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        target.localScale = to;
    }

    private void StopUnlockablePulse()
    {
        if (unlockablePulseRoutine == null)
            return;

        StopCoroutine(unlockablePulseRoutine);
        unlockablePulseRoutine = null;
    }

    private void ResetLockIconScale()
    {
        if (lockIconRect != null)
            lockIconRect.localScale = Vector3.one;
    }
}
