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

    [Header("Popups (ScreenPopupHost)")]
    [SerializeField] private string saleSellPopupId = PopupId.SaleChannelSell;
    [SerializeField] private string resourceFeedbackPopupId = PopupId.ShopResourceFeedback;

    [Header("Bindings UI (prefab)")]
    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform bandeauxContainer;
    [SerializeField] private Image rootBackdropImage;

    private Button hookedCloseButton;
    private SaleChannelBandeauView[] bandeauViews = System.Array.Empty<SaleChannelBandeauView>();
    private bool bandeauxHooked;
    private bool sellPopupHandlerWired;
    private ShopItemPopupController sellPopupInstance;
    private ResourceFeedbackPopupUI resourceFeedbackPopupInstance;
    private ScreenPopupHost screenPopupHost;
    private string pendingChannelId;

    private void Awake()
    {
        ResolveBindingsIfNeeded();
        HookCloseButton();
        HookBandeauViews();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        UnhookBandeauViews();
        UnhookSellPopupHandler();
    }

    public void Refresh()
    {
        ApplyShellBackdrop();

        foreach (SaleChannelBandeauView bandeau in bandeauViews)
        {
            if (bandeau != null)
                bandeau.ApplyLockedInteractable();
        }
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

    private void HookBandeauViews()
    {
        if (bandeauxHooked || bandeauxContainer == null)
            return;

        bandeauViews = bandeauxContainer.GetComponentsInChildren<SaleChannelBandeauView>(true);
        foreach (SaleChannelBandeauView bandeau in bandeauViews)
            bandeau.OnBandeauClicked += HandleBandeauClicked;

        bandeauxHooked = true;
    }

    private void UnhookBandeauViews()
    {
        if (!bandeauxHooked)
            return;

        foreach (SaleChannelBandeauView bandeau in bandeauViews)
        {
            if (bandeau != null)
                bandeau.OnBandeauClicked -= HandleBandeauClicked;
        }

        bandeauxHooked = false;
        bandeauViews = System.Array.Empty<SaleChannelBandeauView>();
    }

    private void HandleBandeauClicked(SaleChannelBandeauView bandeau)
    {
        if (bandeau == null || bandeau.IsLocked)
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

        ResolveSellPopup()?.Close();
        pendingChannelId = null;
        Refresh();
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
