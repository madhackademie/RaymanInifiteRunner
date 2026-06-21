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

    public event Action<SaleChannelBandeauView> OnBandeauClicked;

    public string ChannelId => channelId;
    public string DisplayTitle => titleLabel != null ? titleLabel.text : name;
    public bool IsLocked => lockedOverlay != null && lockedOverlay.activeSelf;
    public bool IsOnCooldown { get; private set; }

    private void Awake()
    {
        if (bandeauButton != null)
            bandeauButton.onClick.AddListener(HandleBandeauClicked);

        ApplyInteractableState();
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
        IsOnCooldown = onCooldown && !IsLocked;

        if (cooldownOverlay != null)
            cooldownOverlay.SetActive(IsOnCooldown);

        if (cooldownLabel != null)
        {
            cooldownLabel.gameObject.SetActive(IsOnCooldown);
            cooldownLabel.text = IsOnCooldown ? remainingText ?? string.Empty : string.Empty;
        }

        if (illustrationImage != null)
            illustrationImage.color = IsOnCooldown ? IllustrationCooldownColor : IllustrationActiveColor;

        ApplyInteractableState();
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
