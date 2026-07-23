using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vue UI d'un bandeau canal de vente (prefab Bezy).
/// États : verrouillé (progression), cooldown 24 h (overlay + timer), actif.
/// </summary>
public class SaleChannelBandeauView : MonoBehaviour
{
    private const string IsOnCooldownAnimatorBool = "IsOnCooldown";

    private static readonly Color IllustrationActiveColor = Color.white;
    private static readonly Color IllustrationCooldownColor = new(0.55f, 0.55f, 0.55f, 1f);

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

    public string ChannelId => channelId;
    public string DisplayTitle => titleLabel != null ? titleLabel.text : name;
    public bool IsLocked => lockedOverlay != null && lockedOverlay.activeSelf;
    public bool IsOnCooldown { get; private set; }

    private void Awake()
    {
        ResolvePolishRefs();

        if (bandeauButton != null)
            bandeauButton.onClick.AddListener(HandleBandeauClicked);

        ApplyInteractableState();
        ApplyCooldownState(false, null);
    }

    private void LateUpdate()
    {
        // L'Animator (FadeIn / Write Defaults) peut écraser l'alpha : on force la lisibilité du timer.
        if (!IsOnCooldown || cooldownCanvasGroup == null)
            return;

        if (cooldownCanvasGroup.alpha < 0.99f)
            cooldownCanvasGroup.alpha = 1f;
    }

    private void OnDestroy()
    {
        if (bandeauButton != null)
            bandeauButton.onClick.RemoveListener(HandleBandeauClicked);
    }

    public void ApplyLockedInteractable()
    {
        ApplyInteractableState();
    }

    public void ApplyCooldownState(bool onCooldown, string remainingText)
    {
        ResolvePolishRefs();

        bool shouldShow = onCooldown && !IsLocked;
        IsOnCooldown = shouldShow;

        if (illustrationImage != null)
            illustrationImage.color = shouldShow ? IllustrationCooldownColor : IllustrationActiveColor;

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

    private void ApplyInteractableState()
    {
        if (bandeauButton == null)
            return;

        bandeauButton.interactable = !IsLocked && !IsOnCooldown;
    }

    private void HandleBandeauClicked()
    {
        if (IsLocked || IsOnCooldown)
            return;

        OnBandeauClicked?.Invoke(this);
    }
}
