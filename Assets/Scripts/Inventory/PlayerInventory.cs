using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory. Lives in the NavigationHUD scene which is never unloaded.
/// Exposes add/remove operations and fires OnInventoryChanged whenever the state changes.
/// Automatically saves to disk on every mutation and loads on first Awake.
/// </summary>
[DefaultExecutionOrder(-100)]
public class PlayerInventory : MonoBehaviour
{
    /// <summary>Singleton instance. Resolved from the NavigationHUD scene, always available.</summary>
    public static PlayerInventory Instance { get; private set; }

    /// <summary>Id d'item utilisé en secours si aucun item de graine de départ n'est assigné dans l'Inspector.</summary>
    private const string DefaultStartingSeedItemId = "laitue_seed";

    [SerializeField] private int slotCount = 20;

    [Tooltip("Database used to resolve item IDs during save/load.")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Économie")]
    [Tooltip("Montant crédité une seule fois par profil (première sauvegarde ou ancienne sans ce flag), puis persisté comme les autres items.")]
    [SerializeField] [Min(0)] private int startingCurrencyAmount = 100;

    [Header("Pack graines départ")]
    [Tooltip("Item consommé à la plantation (ex. laitue_seed). Crédité une fois par profil si startingSeedAmount > 0.")]
    [SerializeField] private ItemDefinition startingSeedItem;
    [SerializeField] [Min(0)] private int startingSeedAmount = 3;

    /// <summary>Résolution des définitions d’items (shop prototype JSON, sauvegarde, etc.).</summary>
    public ItemDatabase ItemDatabase => itemDatabase;

    private readonly List<InventorySlot> slots = new();

    /// <summary>Si la réserve de départ monnaie a déjà été appliquée pour ce fichier de sauvegarde.</summary>
    private bool startingCurrencyApplied;

    private bool startingSeedsApplied;

    /// <summary>True si <see cref="LoadFromDisk"/> a restauré un fichier (pas un reset / nouvelle partie).</summary>
    private bool profileLoadedFromSave;

    /// <summary>Fired after any successful mutation of the inventory.</summary>
    public event Action OnInventoryChanged;

    /// <summary>Read-only view of all inventory slots.</summary>
    public IReadOnlyList<InventorySlot> Slots => slots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[PlayerInventory] Instance dupliquée détectée — une seule doit exister dans NavigationHUD.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitialiseSlots();
        LoadFromDisk();
    }

    private void OnApplicationQuit() => SaveToDisk();

    private void OnDisable()
    {
        if (Instance == this)
            SaveToDisk();
    }

    // ── Initialisation ────────────────────────────────────────────────────────

    private void InitialiseSlots()
    {
        slots.Clear();
        for (int i = 0; i < slotCount; i++)
            slots.Add(new InventorySlot());
    }

    // ── Persistence ───────────────────────────────────────────────────────────

    /// <summary>Persists the current inventory state to disk.</summary>
    public void SaveToDisk()
    {
        InventorySaveService.Save(slots, startingCurrencyApplied, startingSeedsApplied);
    }

    /// <summary>Restores the inventory state from disk. Replaces all current slot data.</summary>
    public void LoadFromDisk()
    {
        if (itemDatabase == null)
        {
            Debug.LogWarning("[PlayerInventory] itemDatabase non assigné — sauvegarde désactivée.");
            return;
        }

        profileLoadedFromSave = InventorySaveService.TryLoad(
                itemDatabase,
                slots,
                out int count,
                out bool currencyFlagFromDisk,
                out bool seedsFlagFromDisk);

        if (profileLoadedFromSave)
        {
            startingCurrencyApplied = currencyFlagFromDisk;
            startingSeedsApplied = seedsFlagFromDisk;
            Debug.Log($"[PlayerInventory] {count} slot(s) restauré(s) depuis la sauvegarde.");
        }
        else
        {
            startingCurrencyApplied = false;
            startingSeedsApplied = false;
        }

        ApplyStartingCurrencyGrantIfNeeded();
        ApplyStartingSeedsGrantIfNeeded();
    }

    /// <summary>
    /// Pack monnaie unique par profil : crédite <see cref="startingCurrencyAmount"/> une fois,
    /// puis enregistre le tout dans <c>inventory.json</c> comme un item normal.
    /// </summary>
    private void ApplyStartingCurrencyGrantIfNeeded()
    {
        if (startingCurrencyApplied || itemDatabase == null)
            return;

        ItemDefinition currency = itemDatabase.PrimaryCurrency;
        if (currency == null || startingCurrencyAmount <= 0)
            return;

        startingCurrencyApplied = true;
        if (!InventoryCurrencyAccount.TryCredit(this, currency, startingCurrencyAmount))
        {
            startingCurrencyApplied = false;
            Debug.LogWarning(
                "[PlayerInventory] Réserve monnaie de départ impossible (inventaire plein ?). Réessayez après libération d’un slot.",
                this);
            return;
        }
    }

    private void ApplyStartingSeedsGrantIfNeeded()
    {
        if (startingSeedsApplied || itemDatabase == null)
            return;

        ItemDefinition seed = ResolveStartingSeedItem();
        if (seed == null || startingSeedAmount <= 0)
            return;

        int existing = Count(seed);

        // Migration : stock déjà >= pack sans flag (ex. double crédit ancien bug).
        if (existing >= startingSeedAmount)
        {
            MarkStartingSeedsApplied();
            return;
        }

        // Sauvegarde existante + 0 graine : pack déjà consommé (flag absent des anciennes saves).
        if (profileLoadedFromSave && existing == 0)
        {
            MarkStartingSeedsApplied();
            return;
        }

        // Nouveau profil ou reset inventaire : crédit unique.
        startingSeedsApplied = true;
        InventoryResult result = TryAdd(seed, startingSeedAmount);
        if (result == InventoryResult.Full)
        {
            startingSeedsApplied = false;
            SaveToDisk();
            Debug.LogWarning(
                "[PlayerInventory] Pack graines de départ impossible (inventaire plein ?).",
                this);
            return;
        }

        SaveToDisk();
    }

    private void MarkStartingSeedsApplied()
    {
        startingSeedsApplied = true;
        SaveToDisk();
    }

    private ItemDefinition ResolveStartingSeedItem()
    {
        if (startingSeedItem != null)
            return startingSeedItem;

        return itemDatabase != null ? itemDatabase.GetById(DefaultStartingSeedItemId) : null;
    }

    /// <summary>Clears all slots and deletes the save file from disk.</summary>
    [ContextMenu("Inventaire — Reset + supprimer inventory.json")]
    public void ResetAndDeleteSave()
    {
        InitialiseSlots();
        InventorySaveService.Delete();
        profileLoadedFromSave = false;
        startingCurrencyApplied = false;
        startingSeedsApplied = false;
        ApplyStartingCurrencyGrantIfNeeded();
        ApplyStartingSeedsGrantIfNeeded();
        SaveToDisk();
        OnInventoryChanged?.Invoke();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Tries to add the given quantity of an item to the inventory.
    /// Items are stacked into existing matching slots first, then into empty slots.
    /// </summary>
    /// <returns>
    /// <see cref="InventoryResult.Success"/>     — all added.<br/>
    /// <see cref="InventoryResult.Partial"/>     — partially added (inventory nearly full).<br/>
    /// <see cref="InventoryResult.Full"/>        — nothing could be added.<br/>
    /// <see cref="InventoryResult.InvalidItem"/> — item reference is null.
    /// </returns>
    public InventoryResult TryAdd(ItemDefinition item, int quantity)
    {
        if (item == null)
            return InventoryResult.InvalidItem;

        if (quantity <= 0)
            return InventoryResult.Success;

        int remaining = quantity;

        // Pass 1 — fill existing partial stacks
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty || slot.Item != item || slot.IsFull)
                continue;

            remaining -= slot.Add(remaining);

            if (remaining <= 0)
                break;
        }

        // Pass 2 — use empty slots
        if (remaining > 0)
        {
            foreach (InventorySlot slot in slots)
            {
                if (!slot.IsEmpty)
                    continue;

                slot.Set(item, 0);
                remaining -= slot.Add(remaining);

                if (remaining <= 0)
                    break;
            }
        }

        bool anythingAdded = remaining < quantity;

        if (anythingAdded)
        {
            OnInventoryChanged?.Invoke();
            SaveToDisk();
        }

        if (remaining >= quantity)
            return InventoryResult.Full;

        return remaining > 0 ? InventoryResult.Partial : InventoryResult.Success;
    }

    /// <summary>
    /// Tries to remove the given quantity of an item from the inventory.
    /// </summary>
    /// <returns>
    /// <see cref="InventoryResult.Success"/>     — all removed.<br/>
    /// <see cref="InventoryResult.Partial"/>     — partially removed (not enough stock).<br/>
    /// <see cref="InventoryResult.Full"/>        — none found to remove.<br/>
    /// <see cref="InventoryResult.InvalidItem"/> — item reference is null.
    /// </returns>
    public InventoryResult TryRemove(ItemDefinition item, int quantity)
    {
        if (item == null)
            return InventoryResult.InvalidItem;

        if (quantity <= 0)
            return InventoryResult.Success;

        int remaining = quantity;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty || slot.Item != item)
                continue;

            remaining -= slot.Remove(remaining);

            if (remaining <= 0)
                break;
        }

        bool anythingRemoved = remaining < quantity;

        if (anythingRemoved)
        {
            OnInventoryChanged?.Invoke();
            SaveToDisk();
        }

        if (remaining >= quantity)
            return InventoryResult.Full;

        return remaining > 0 ? InventoryResult.Partial : InventoryResult.Success;
    }

    /// <summary>Returns the total quantity of the given item across all slots.</summary>
    public int Count(ItemDefinition item)
    {
        if (item == null)
            return 0;

        int total = 0;
        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty && slot.Item == item)
                total += slot.Quantity;
        }
        return total;
    }

    /// <summary>Returns true if the inventory can accept at least one unit of the given item.</summary>
    public bool HasSpaceFor(ItemDefinition item)
    {
        if (item == null)
            return false;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
                return true;

            if (slot.Item == item && !slot.IsFull)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Indique si au moins <paramref name="quantity"/> unités peuvent être ajoutées en réutilisant piles et slots vides.
    /// </summary>
    public bool CanFitQuantity(ItemDefinition item, int quantity)
    {
        if (item == null || quantity <= 0)
            return true;

        int remaining = quantity;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty)
                remaining -= Mathf.Min(item.MaxStack, remaining);
            else if (slot.Item == item && !slot.IsFull)
                remaining -= slot.RemainingSpace;

            if (remaining <= 0)
                return true;
        }

        return false;
    }
}
