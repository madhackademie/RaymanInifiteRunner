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
    [Tooltip("Ignore tout cooldown (playtest). Sinon durée = Neighbor Sale Cooldown Seconds.")]
    [SerializeField] private bool ignoreSaleCooldown;

    private readonly Dictionary<string, long> fallbackLastSaleUtcTicksByChannel = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SaleChannelService] Instance dupliquée — une seule attendue dans NavigationHUD.", this);
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        LoadCooldownState();
    }

    /// <summary>Playtest : efface les cooldowns en mémoire + fichier <c>sale_channels.json</c>.</summary>
    [ContextMenu("Debug/Clear Sale Cooldowns")]
    public void DebugClearSaleCooldowns()
    {
        fallbackLastSaleUtcTicksByChannel.Clear();

        SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
        if (unlockService != null)
        {
            foreach (string channelId in new List<string>(unlockService.SharedData.LastSaleUtcTicksByChannel.Keys))
                unlockService.SharedData.LastSaleUtcTicksByChannel.Remove(channelId);

            SaleChannelSaveService.Save(unlockService.SharedData);
        }
        else
        {
            SaleChannelSaveService.Delete();
        }

        Debug.Log("[SaleChannelService] Cooldowns vente effacés.");
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
        SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
        if (unlockService != null)
            return unlockService.IsChannelUnlocked(channelId);

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

        if (channelId != SaleChannelId.Neighbor)
            return false;

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
        if (channelId != SaleChannelId.Neighbor)
            return false;

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

        if (quantity <= 0 || channelId != SaleChannelId.Neighbor)
        {
            failureMessage = "Canal de vente non pris en charge.";
            return false;
        }

        if (quantity > neighborMaxQuantityPerSale)
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

        RecordSuccessfulSale(channelId, quantity);
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

        if (title.Contains("Voisinage", StringComparison.OrdinalIgnoreCase))
        {
            channelId = SaleChannelId.Neighbor;
            return true;
        }

        if (title.Contains("Bandouli", StringComparison.OrdinalIgnoreCase))
        {
            channelId = SaleChannelId.Sash;
            return true;
        }

        if (title.Contains("Vélo", StringComparison.OrdinalIgnoreCase) ||
            title.Contains("Velo", StringComparison.OrdinalIgnoreCase))
        {
            channelId = SaleChannelId.Bike;
            return true;
        }

        return false;
    }

    private void RecordSuccessfulSale(string channelId, int quantitySold)
    {
        if (string.IsNullOrWhiteSpace(channelId))
            return;

        long utcTicks = FarmTimeService.UtcNowTicks;
        SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
        if (unlockService != null)
        {
            unlockService.SetLastSaleUtcTicks(channelId, utcTicks);
            unlockService.RecordSale(channelId, quantitySold);
        }
        else
        {
            fallbackLastSaleUtcTicksByChannel[channelId] = utcTicks;
            PersistCooldownState();
        }
    }

    private void LoadCooldownState()
    {
        fallbackLastSaleUtcTicksByChannel.Clear();

        if (SaleChannelUnlockService.Instance != null)
            return;

        if (!SaleChannelSaveService.TryLoad(out SaleChannelPersistedData loaded))
            return;

        foreach (KeyValuePair<string, long> entry in loaded.LastSaleUtcTicksByChannel)
            fallbackLastSaleUtcTicksByChannel[entry.Key] = entry.Value;
    }

    private void PersistCooldownState()
    {
        if (SaleChannelUnlockService.Instance != null)
            return;

        var data = new SaleChannelPersistedData();
        foreach (KeyValuePair<string, long> entry in fallbackLastSaleUtcTicksByChannel)
            data.LastSaleUtcTicksByChannel[entry.Key] = entry.Value;

        SaleChannelSaveService.Save(data);
    }

    private bool TryGetLastSaleUtc(string channelId, out DateTime lastSaleUtc)
    {
        lastSaleUtc = default;

        SaleChannelUnlockService unlockService = SaleChannelUnlockService.Instance;
        if (unlockService != null &&
            unlockService.TryGetLastSaleUtcTicks(channelId, out long ticks) &&
            ticks > 0)
        {
            lastSaleUtc = new DateTime(ticks, DateTimeKind.Utc);
            return true;
        }

        if (!fallbackLastSaleUtcTicksByChannel.TryGetValue(channelId, out ticks) || ticks <= 0)
            return false;

        lastSaleUtc = new DateTime(ticks, DateTimeKind.Utc);
        return true;
    }

    private float GetCooldownSeconds(string channelId)
    {
        return neighborSaleCooldownSeconds;
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
