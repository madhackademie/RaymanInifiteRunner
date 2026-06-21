using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logique métier V0 — vente voisinage (laitue mature, plafond quantité, crédit monnaie, cooldown 24 h).
/// </summary>
public class SaleChannelService : MonoBehaviour
{
    public static SaleChannelService Instance { get; private set; }

    private const string NeighborSellItemId = "laitue_mature";
    private const int NeighborUnitPrice = 15;
    private const int NeighborMaxQuantityPerSale = 2;
    private const float NeighborSaleCooldownSeconds = 24f * 3600f;

    [Header("Proto V0 — voisinage")]
    [SerializeField] private string neighborSellItemId = NeighborSellItemId;
    [SerializeField] private int neighborUnitPrice = NeighborUnitPrice;
    [SerializeField] private int neighborMaxQuantityPerSale = NeighborMaxQuantityPerSale;
    [SerializeField] private float neighborSaleCooldownSeconds = NeighborSaleCooldownSeconds;

    [Header("Debug")]
    [SerializeField] private bool ignoreSaleCooldown;

    private readonly Dictionary<string, long> lastSaleUtcTicksByChannel = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SaleChannelService] Instance dupliquée — une seule attendue dans NavigationHUD.", this);
            Destroy(this);
            return;
        }

        Instance = this;
        LoadCooldownState();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnApplicationQuit()
    {
        PersistCooldownState();
    }

    public bool IsChannelUnlocked(string channelId)
    {
        return channelId == SaleChannelId.Neighbor;
    }

    public bool IsOnCooldown(string channelId)
    {
        return TryGetCooldownRemainingSeconds(channelId, out _);
    }

    public bool TryGetCooldownRemainingSeconds(string channelId, out float remainingSeconds)
    {
        remainingSeconds = 0f;

        if (ignoreSaleCooldown || string.IsNullOrWhiteSpace(channelId))
            return false;

        if (!TryGetLastSaleUtc(channelId, out DateTime lastSaleUtc))
            return false;

        float cooldownSeconds = GetCooldownSeconds(channelId);
        DateTime nowUtc = DateTime.UtcNow;

        if (nowUtc < lastSaleUtc)
        {
            remainingSeconds = cooldownSeconds;
            return true;
        }

        double elapsedSeconds = (nowUtc - lastSaleUtc).TotalSeconds;
        if (elapsedSeconds >= cooldownSeconds)
            return false;

        remainingSeconds = (float)(cooldownSeconds - elapsedSeconds);
        return remainingSeconds > 0f;
    }

    public bool TryGetCooldownMessage(string channelId, out string message)
    {
        message = null;

        if (!TryGetCooldownRemainingSeconds(channelId, out float remainingSeconds))
            return false;

        message = $"Canal indisponible — déblocage dans {SaleChannelCooldownFormatter.FormatRemainingSeconds(remainingSeconds)}.";
        return true;
    }

    public bool TryBuildSellPopupData(string channelId, out ShopItemPopupData popupData)
    {
        popupData = null;

        if (!IsChannelUnlocked(channelId))
            return false;

        if (IsOnCooldown(channelId))
            return false;

        ItemDefinition item = ResolveSellItem();
        if (item == null)
        {
            Debug.LogWarning(
                $"[SaleChannelService] Item vente introuvable (id='{neighborSellItemId}'). " +
                "Vérifiez ItemDatabase / PlayerInventory.");
            return false;
        }

        PlayerInventory inventory = PlayerInventory.Instance;
        int owned = inventory != null ? inventory.Count(item) : 0;
        int maxQty = ComputeMaxSellQuantity(owned, neighborMaxQuantityPerSale);

        if (maxQty <= 0)
            return false;

        popupData = new ShopItemPopupData(
            item.ItemId,
            item.DisplayName,
            "Voisinage",
            $"Vendre via le canal voisinage (max {neighborMaxQuantityPerSale} par vente).",
            item.Icon,
            neighborUnitPrice,
            minQuantity: 1,
            maxQuantity: maxQty);

        return true;
    }

    public bool CanSell(string channelId, int quantity)
    {
        if (!IsChannelUnlocked(channelId) || quantity <= 0)
            return false;

        if (IsOnCooldown(channelId))
            return false;

        if (quantity > neighborMaxQuantityPerSale)
            return false;

        ItemDefinition item = ResolveSellItem();
        PlayerInventory inventory = PlayerInventory.Instance;
        if (item == null || inventory == null)
            return false;

        return inventory.Count(item) >= quantity;
    }

    public bool TrySell(string channelId, int quantity, out string failureMessage)
    {
        failureMessage = null;

        if (!IsChannelUnlocked(channelId))
        {
            failureMessage = "Ce canal de vente n'est pas encore débloqué.";
            return false;
        }

        if (IsOnCooldown(channelId) &&
            TryGetCooldownMessage(channelId, out string cooldownMessage))
        {
            failureMessage = cooldownMessage;
            return false;
        }

        if (quantity <= 0 || quantity > neighborMaxQuantityPerSale)
        {
            failureMessage = $"Quantité invalide (max {neighborMaxQuantityPerSale}).";
            return false;
        }

        PlayerInventory inventory = PlayerInventory.Instance;
        ItemDefinition item = ResolveSellItem();
        ItemDefinition currency = inventory?.ItemDatabase?.PrimaryCurrency;

        if (inventory == null || item == null)
        {
            failureMessage = "Inventaire indisponible.";
            return false;
        }

        if (currency == null)
        {
            failureMessage = "Monnaie non configurée.";
            return false;
        }

        if (inventory.Count(item) < quantity)
        {
            failureMessage = "Stock insuffisant pour cette vente.";
            return false;
        }

        int totalGain = neighborUnitPrice * quantity;
        if (!InventoryCurrencyAccount.TrySell(
                inventory, item, quantity, currency, totalGain, out _))
        {
            failureMessage = "Vente impossible (inventaire ou monnaie).";
            return false;
        }

        RecordSuccessfulSale(channelId);
        return true;
    }

    public static bool TryResolveChannelId(SaleChannelBandeauView bandeau, out string channelId)
    {
        channelId = null;
        if (bandeau == null)
            return false;

        if (!string.IsNullOrWhiteSpace(bandeau.ChannelId))
        {
            channelId = bandeau.ChannelId.Trim();
            return true;
        }

        string title = bandeau.DisplayTitle;
        if (string.IsNullOrWhiteSpace(title))
            return false;

        if (title.Contains("Voisinage"))
        {
            channelId = SaleChannelId.Neighbor;
            return true;
        }

        return false;
    }

    private void RecordSuccessfulSale(string channelId)
    {
        if (string.IsNullOrWhiteSpace(channelId))
            return;

        lastSaleUtcTicksByChannel[channelId] = FarmTimeService.UtcNowTicks;
        PersistCooldownState();
    }

    private void LoadCooldownState()
    {
        lastSaleUtcTicksByChannel.Clear();

        if (!SaleChannelSaveService.TryLoad(out Dictionary<string, long> loaded))
            return;

        foreach (KeyValuePair<string, long> entry in loaded)
            lastSaleUtcTicksByChannel[entry.Key] = entry.Value;
    }

    private void PersistCooldownState()
    {
        SaleChannelSaveService.Save(lastSaleUtcTicksByChannel);
    }

    private bool TryGetLastSaleUtc(string channelId, out DateTime lastSaleUtc)
    {
        lastSaleUtc = default;

        if (!lastSaleUtcTicksByChannel.TryGetValue(channelId, out long ticks) || ticks <= 0)
            return false;

        lastSaleUtc = new DateTime(ticks, DateTimeKind.Utc);
        return true;
    }

    private float GetCooldownSeconds(string channelId)
    {
        if (channelId == SaleChannelId.Neighbor)
            return neighborSaleCooldownSeconds;

        return NeighborSaleCooldownSeconds;
    }

    private ItemDefinition ResolveSellItem()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null || inventory.ItemDatabase == null)
            return null;

        return inventory.ItemDatabase.GetById(neighborSellItemId);
    }

    private static int ComputeMaxSellQuantity(int owned, int channelCap)
    {
        if (owned <= 0 || channelCap <= 0)
            return 0;

        return Mathf.Min(owned, channelCap);
    }
}
