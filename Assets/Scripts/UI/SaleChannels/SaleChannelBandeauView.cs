using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vue UI d'un bandeau canal de vente (prefab Bezy).
/// Phase 3 : wiring Inspector uniquement — logique vente = SaleChannelService (session suivante).
/// </summary>
public class SaleChannelBandeauView : MonoBehaviour
{
    [Header("Bindings UI (prefab)")]
    [SerializeField] private Button bandeauButton;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private Image[] starImages = new Image[5];
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Image illustrationImage;
    [SerializeField] private string channelId;

    public event Action<SaleChannelBandeauView> OnBandeauClicked;

    public string ChannelId => channelId;
    public string DisplayTitle => titleLabel != null ? titleLabel.text : name;
    public bool IsLocked => lockedOverlay != null && lockedOverlay.activeSelf;

    private void Awake()
    {
        if (bandeauButton != null)
            bandeauButton.onClick.AddListener(HandleBandeauClicked);

        ApplyLockedInteractable();
    }

    private void OnDestroy()
    {
        if (bandeauButton != null)
            bandeauButton.onClick.RemoveListener(HandleBandeauClicked);
    }

    public void ApplyLockedInteractable()
    {
        if (bandeauButton == null)
            return;

        bandeauButton.interactable = !IsLocked;
    }

    private void HandleBandeauClicked()
    {
        if (IsLocked)
            return;

        OnBandeauClicked?.Invoke(this);
    }
}
