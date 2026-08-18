using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Écran HUD des canaux de vente (écoulement production).
/// Bandeaux UI (Bezy) + popup vente via pipeline générique + <see cref="SaleChannelService"/>.
/// </summary>
public class RuntimeSaleChannelsScreen : MonoBehaviour
{
    private const string BandeauxContentPath = "Body/BandeauxScrollView/Viewport/BandeauxContent";
    private const string NoStockMessage = "Aucune laitue mature à vendre pour ce canal.";
    private const float CooldownRefreshIntervalSeconds = 1f;
    private const float SaleMoneyBurstDestroyDelaySeconds = 1.5f;

    [Header("Popups (ScreenPopupHost)")]
    [SerializeField] private string saleSellPopupId = PopupId.SaleChannelSell;
    [SerializeField] private string resourceFeedbackPopupId = PopupId.ShopResourceFeedback;

    [Header("Bindings UI (prefab)")]
    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform bandeauxContainer;
    [SerializeField] private Image rootBackdropImage;
    [SerializeField] private SaleChannelUnlockTooltipHost unlockTooltipHost;

    [Header("VFX vente")]
    [Tooltip("Prefab Bezy SaleMoneyBurst (ref Simulate / Phase 3). Runtime = burst UI Overlay.")]
    [SerializeField] private GameObject saleMoneyBurstPrefab;

    private Button hookedCloseButton;
    private SaleChannelBandeauView[] bandeauViews = System.Array.Empty<SaleChannelBandeauView>();
    private bool bandeauxHooked;
    private bool sellPopupHandlerWired;
    private ShopItemPopupController sellPopupInstance;
    private ResourceFeedbackPopupUI resourceFeedbackPopupInstance;
    private ScreenPopupHost screenPopupHost;
    private string pendingChannelId;
    private Coroutine cooldownRefreshRoutine;

    private void Awake()
    {
        ResolveBindingsIfNeeded();
        ResolveUnlockTooltipHost();
        HookCloseButton();
        HookBandeauViews();
    }

    private void OnEnable()
    {
        Refresh();
        StartCooldownRefreshIfNeeded();
    }

    private void OnDisable()
    {
        unlockTooltipHost?.Hide();
        StopCooldownRefresh();
    }

    private void OnDestroy()
    {
        StopCooldownRefresh();
        UnhookBandeauViews();
        UnhookSellPopupHandler();
    }

    public void Refresh()
    {
        ApplyShellBackdrop();
        RefreshBandeauStates(playUnlockableReveal: true);
    }

    private void RefreshBandeauStates(bool playUnlockableReveal)
    {
        SaleChannelService service = SaleChannelService.Instance;
        SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
        unlockService?.RefreshProgress();

        foreach (SaleChannelBandeauView bandeau in bandeauViews)
        {
            if (bandeau == null)
                continue;

            if (!SaleChannelService.TryResolveChannelId(bandeau, out string channelId))
            {
                bandeau.ClearProgressionState();
                bandeau.ApplyCooldownState(false, null);
                bandeau.ApplyLockedInteractable();
                continue;
            }

            ApplyProgressionVisuals(bandeau, channelId, unlockService, playUnlockableReveal);

            if (bandeau.IsProgressionBlockingSale)
            {
                bandeau.ApplyCooldownState(false, null);
                bandeau.ApplyLockedInteractable();
                continue;
            }

            if (service == null)
            {
                bandeau.ApplyCooldownState(false, null);
                bandeau.ApplyLockedInteractable();
                continue;
            }

            if (service.TryGetCooldownRemainingSeconds(channelId, out float remainingSeconds))
            {
                string remainingText = SaleChannelCooldownFormatter.FormatRemainingSeconds(remainingSeconds);
                bandeau.ApplyCooldownState(true, remainingText);
            }
            else
            {
                bandeau.ApplyCooldownState(false, null);
            }

            bandeau.ApplyLockedInteractable();
        }
    }

    private static void ApplyProgressionVisuals(
        SaleChannelBandeauView bandeau,
        string channelId,
        SaleChannelUnlockService unlockService,
        bool playUnlockableReveal)
    {
        if (channelId == SaleChannelId.Neighbor || unlockService == null)
        {
            bandeau.ClearProgressionState();
            return;
        }

        if (!unlockService.TryGetProgressSnapshot(channelId, out SaleChannelUnlockProgressSnapshot snapshot))
        {
            bandeau.ClearProgressionState();
            return;
        }

        if (snapshot.Phase == SaleChannelProgressionPhase.Unlocked)
        {
            bandeau.ClearProgressionState();
            return;
        }

        bandeau.ApplyProgressionState(snapshot, playUnlockableReveal);
    }

    private void StartCooldownRefreshIfNeeded()
    {
        StopCooldownRefresh();

        if (!isActiveAndEnabled)
            return;

        cooldownRefreshRoutine = StartCoroutine(CooldownRefreshRoutine());
    }

    private void StopCooldownRefresh()
    {
        if (cooldownRefreshRoutine == null)
            return;

        StopCoroutine(cooldownRefreshRoutine);
        cooldownRefreshRoutine = null;
    }

    private IEnumerator CooldownRefreshRoutine()
    {
        var wait = new WaitForSecondsRealtime(CooldownRefreshIntervalSeconds);

        while (isActiveAndEnabled)
        {
            yield return wait;

            SaleChannelService service = SaleChannelService.Instance;
            SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
            unlockService?.RefreshProgress();

            bool keepRefreshing = unlockService != null && unlockService.HasActiveResearch();

            if (service != null)
            {
                foreach (SaleChannelBandeauView bandeau in bandeauViews)
                {
                    if (bandeau == null)
                        continue;

                    if (!SaleChannelService.TryResolveChannelId(bandeau, out string channelId))
                        continue;

                    ApplyProgressionVisuals(bandeau, channelId, unlockService, playUnlockableReveal: false);

                    if (bandeau.IsProgressionBlockingSale)
                    {
                        if (bandeau.ProgressionPhase == SaleChannelProgressionPhase.ResearchInProgress)
                            keepRefreshing = true;

                        continue;
                    }

                    if (service.TryGetCooldownRemainingSeconds(channelId, out float remainingSeconds))
                    {
                        keepRefreshing = true;
                        bandeau.ApplyCooldownState(
                            true,
                            SaleChannelCooldownFormatter.FormatRemainingSeconds(remainingSeconds));
                    }
                    else if (bandeau.IsOnCooldown)
                    {
                        bandeau.ApplyCooldownState(false, null);
                    }
                }
            }

            if (!keepRefreshing)
                break;
        }

        cooldownRefreshRoutine = null;
    }

    private void ResolveBindingsIfNeeded()
    {
        if (closeButton == null)
            closeButton = transform.Find("Header/CloseButton")?.GetComponent<Button>();

        if (bandeauxContainer == null)
            bandeauxContainer = transform.Find(BandeauxContentPath) as RectTransform;

        if (rootBackdropImage == null)
            rootBackdropImage = GetComponent<Image>();

        if (screenPopupHost == null)
            screenPopupHost = GetComponentInChildren<ScreenPopupHost>(true);
    }

    private void ResolveUnlockTooltipHost()
    {
        if (unlockTooltipHost != null)
            return;

        unlockTooltipHost = GetComponentInChildren<SaleChannelUnlockTooltipHost>(true);
        if (unlockTooltipHost != null)
            return;

        Debug.LogWarning(
            "[RuntimeSaleChannelsScreen] unlockTooltipHost absent — tooltip déblocage désactivé jusqu'au wiring Bezy. " +
            "Voir Notes/Ui/PROMPTS_Bezi_sale_channel_unlock_ui.md",
            this);
    }

    private void HookBandeauViews()
    {
        if (bandeauxHooked || bandeauxContainer == null)
            return;

        bandeauViews = bandeauxContainer.GetComponentsInChildren<SaleChannelBandeauView>(true);
        foreach (SaleChannelBandeauView bandeau in bandeauViews)
        {
            bandeau.OnBandeauClicked += HandleBandeauClicked;
            bandeau.OnUnlockResearchRequested += HandleUnlockResearchRequested;
        }

        WireProgressionHovers();
        bandeauxHooked = true;
    }

    private void WireProgressionHovers()
    {
        SaleChannelBandeauProgressionHover[] hovers =
            bandeauxContainer.GetComponentsInChildren<SaleChannelBandeauProgressionHover>(true);

        foreach (SaleChannelBandeauProgressionHover hover in hovers)
        {
            if (hover == null)
                continue;

            hover.ConfigureFromHierarchy(unlockTooltipHost);
        }

        if (hovers.Length == 0)
        {
            Debug.LogWarning(
                "[RuntimeSaleChannelsScreen] Aucun SaleChannelBandeauProgressionHover — survol bandeau verrouillé inactif. " +
                "Bezy : ajouter sur LockedOverlay (cf. PROMPTS_Bezi_sale_channel_unlock_ui.md).",
                this);
        }
    }

    private void UnhookBandeauViews()
    {
        if (!bandeauxHooked)
            return;

        foreach (SaleChannelBandeauView bandeau in bandeauViews)
        {
            if (bandeau == null)
                continue;

            bandeau.OnBandeauClicked -= HandleBandeauClicked;
            bandeau.OnUnlockResearchRequested -= HandleUnlockResearchRequested;
        }

        bandeauxHooked = false;
        bandeauViews = System.Array.Empty<SaleChannelBandeauView>();
    }

    private void HandleUnlockResearchRequested(SaleChannelBandeauView bandeau)
    {
        if (bandeau == null)
            return;

        unlockTooltipHost?.Hide();

        if (!SaleChannelService.TryResolveChannelId(bandeau, out string channelId))
        {
            ShowFeedbackMessage("Canal de vente non reconnu.");
            return;
        }

        SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
        if (unlockService == null)
        {
            ShowFeedbackMessage("Progression canaux indisponible.");
            return;
        }

        if (!unlockService.TryStartResearch(channelId, out string failureMessage))
        {
            ShowFeedbackMessage(string.IsNullOrEmpty(failureMessage)
                ? "Recherche impossible."
                : failureMessage);
            return;
        }

        ShowFeedbackMessage("Recherche lancée — revenez quand le timer sera terminé.");
        RefreshBandeauStates(playUnlockableReveal: false);
        StartCooldownRefreshIfNeeded();
    }

    private void HandleBandeauClicked(SaleChannelBandeauView bandeau)
    {
        if (bandeau == null || bandeau.IsProgressionBlockingSale || bandeau.IsOnCooldown)
            return;

        if (!SaleChannelService.TryResolveChannelId(bandeau, out string channelId))
        {
            ShowFeedbackMessage("Canal de vente non reconnu.");
            return;
        }

        SaleChannelService service = SaleChannelService.Instance;
        if (service == null)
        {
            Debug.LogWarning("[RuntimeSaleChannelsScreen] SaleChannelService absent — ajoutez-le sur PlayerInventory (NavigationHUD).");
            return;
        }

        if (service.IsOnCooldown(channelId) &&
            service.TryGetCooldownMessage(channelId, out string cooldownMessage))
        {
            ShowFeedbackMessage(cooldownMessage);
            RefreshBandeauStates(playUnlockableReveal: false);
            StartCooldownRefreshIfNeeded();
            return;
        }

        if (!service.TryBuildSellPopupData(channelId, out ShopItemPopupData popupData))
        {
            ShowFeedbackMessage(NoStockMessage);
            return;
        }

        ShopItemPopupController popup = ResolveSellPopup();
        if (popup == null)
        {
            Debug.LogWarning(
                $"[RuntimeSaleChannelsScreen] Popup vente introuvable (popupId='{saleSellPopupId}'). " +
                $"Ajoutez un ScreenPopupBinding ({ScreenId.SaleChannels} + {PopupId.SaleChannelSell}).");
            return;
        }

        EnsureSellPopupWired(popup);
        pendingChannelId = channelId;

        if (!popup.gameObject.activeSelf)
            popup.gameObject.SetActive(true);

        popup.transform.SetAsLastSibling();
        popup.Open(popupData, ShopItemPopupFlowMode.Sell);
    }

    private void HandleSellRequested(ShopItemPopupData data, int quantity, int totalGain)
    {
        if (data == null || string.IsNullOrEmpty(pendingChannelId))
            return;

        SaleChannelService service = SaleChannelService.Instance;
        if (service == null)
            return;

        if (!service.TrySell(pendingChannelId, quantity, out string failureMessage))
        {
            ShowFeedbackMessage(string.IsNullOrEmpty(failureMessage)
                ? "Vente impossible."
                : failureMessage);
            return;
        }

        ShopItemPopupController popup = ResolveSellPopup();
        PlaySaleMoneyBurst(popup);
        popup?.Close();
        pendingChannelId = null;
        Refresh();
        StartCooldownRefreshIfNeeded();
    }

    private void PlaySaleMoneyBurst(ShopItemPopupController popup)
    {
        if (popup == null)
            return;

        RectTransform anchor = popup.ResolveMoneyBurstAnchor();
        if (anchor == null)
            return;

        if (saleMoneyBurstPrefab == null)
        {
            Debug.LogWarning(
                "[RuntimeSaleChannelsScreen] saleMoneyBurstPrefab non assigné — burst UI quand même. " +
                "Réf. Bezy : Assets/Prefabs/Ui/VFX/SaleMoneyBurst.prefab",
                this);
        }

        SaleMoneyBurstVfx.Play(this, anchor, SaleMoneyBurstDestroyDelaySeconds);
    }

    private ShopItemPopupController ResolveSellPopup()
    {
        if (sellPopupInstance != null)
            return sellPopupInstance;

        ScreenPopupHost host = ResolvePopupHost();
        if (host != null &&
            host.HasPopup(saleSellPopupId) &&
            host.TryGetPopup(saleSellPopupId, out ShopItemPopupController popupFromHost))
        {
            sellPopupInstance = popupFromHost;
            return sellPopupInstance;
        }

        return null;
    }

    private ScreenPopupHost ResolvePopupHost()
    {
        if (screenPopupHost != null)
            return screenPopupHost;

        screenPopupHost = GetComponentInChildren<ScreenPopupHost>(true);
        return screenPopupHost;
    }

    private void EnsureSellPopupWired(ShopItemPopupController popup)
    {
        if (popup == null || sellPopupHandlerWired)
            return;

        popup.PurchaseRequested += HandleSellRequested;
        sellPopupHandlerWired = true;
    }

    private void UnhookSellPopupHandler()
    {
        if (!sellPopupHandlerWired || sellPopupInstance == null)
            return;

        sellPopupInstance.PurchaseRequested -= HandleSellRequested;
        sellPopupHandlerWired = false;
    }

    private ResourceFeedbackPopupUI ResolveResourceFeedbackPopup()
    {
        if (resourceFeedbackPopupInstance != null)
            return resourceFeedbackPopupInstance;

        ScreenPopupHost host = ResolvePopupHost();
        if (host != null &&
            host.HasPopup(resourceFeedbackPopupId) &&
            host.TryGetPopup(resourceFeedbackPopupId, out ResourceFeedbackPopupUI fromHost))
        {
            resourceFeedbackPopupInstance = fromHost;
            return resourceFeedbackPopupInstance;
        }

        return null;
    }

    private void ShowFeedbackMessage(string message)
    {
        ResourceFeedbackPopupUI popup = ResolveResourceFeedbackPopup();
        if (popup != null)
        {
            popup.ShowMessage(message);
            return;
        }

        Debug.LogWarning(
            "[RuntimeSaleChannelsScreen] ResourceFeedbackPopup introuvable — message : " + message +
            $". Ajoutez un ScreenPopupBinding ({ScreenId.SaleChannels} + {PopupId.ShopResourceFeedback}).");
    }

    private void HookCloseButton()
    {
        if (closeButton == null || closeButton == hookedCloseButton)
            return;

        if (hookedCloseButton != null)
            hookedCloseButton.onClick.RemoveListener(HandleCloseClicked);

        hookedCloseButton = closeButton;
        hookedCloseButton.onClick.AddListener(HandleCloseClicked);
    }

    private void HandleCloseClicked()
    {
        unlockTooltipHost?.Hide();

        if (UIManager.Instance != null)
            UIManager.Instance.HideScreen(ScreenId.SaleChannels);
    }

    private void ApplyShellBackdrop()
    {
        if (UIManager.Instance == null || rootBackdropImage == null)
            return;

        Sprite backdrop = UIManager.Instance.HudModalBackdropSprite;
        if (backdrop == null)
            return;

        rootBackdropImage.sprite = backdrop;
        rootBackdropImage.color = UIManager.Instance.HudModalBackdropTint;
    }
}
