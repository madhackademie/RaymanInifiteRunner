using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the inventory panel: spawns InventorySlotUI instances and keeps them
/// in sync with the PlayerInventory via the OnInventoryChanged event.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotsContainer;

    private readonly List<InventorySlotUI> spawnedSlots = new();
    private bool hasWarnedAboutNestedSlotPrefab;

    /// <summary>True une fois que Bind() a été appelé avec un inventaire valide.</summary>
    public bool IsBound => playerInventory != null;

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= Refresh;
    }

    /// <summary>
    /// Injecte un <see cref="PlayerInventory"/> et construit les slots.
    /// </summary>
    public void Bind(PlayerInventory inventory)
    {
        if (inventory == null)
            return;

        if (playerInventory != null && playerInventory != inventory)
            playerInventory.OnInventoryChanged -= Refresh;

        playerInventory = inventory;
        Initialise();
    }

    private void Initialise()
    {
        playerInventory.OnInventoryChanged -= Refresh;
        playerInventory.OnInventoryChanged += Refresh;
        BuildSlots();
    }

    // ── Slot management ───────────────────────────────────────────────────────

    private void BuildSlots()
    {
        foreach (InventorySlotUI slot in spawnedSlots)
            Destroy(slot.gameObject);

        spawnedSlots.Clear();

        foreach (InventorySlot _ in playerInventory.Slots)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, slotsContainer);

            // Ancienne scène Inventaire : si le slot prefab est encore encapsulé, on le recolle
            // sous le container pour rester compatible sans casser la migration.
            if (slotUI.transform.parent != slotsContainer)
            {
                Transform wrapper = slotUI.transform.parent;
                slotUI.transform.SetParent(slotsContainer, false);
                wrapper.SetParent(null);
                Destroy(wrapper.gameObject);

                if (!hasWarnedAboutNestedSlotPrefab)
                {
                    Debug.LogWarning("[InventoryUI] InventorySlotUI prefab encapsulé détecté. Simplifie le prefab pour éviter les coûts de ré-parentage.");
                    hasWarnedAboutNestedSlotPrefab = true;
                }
            }

            spawnedSlots.Add(slotUI);
        }

        Refresh();
    }

    /// <summary>Repopulates all slot UIs from the current inventory state.</summary>
    public void Refresh()
    {
        if (playerInventory == null)
            return;

        IReadOnlyList<InventorySlot> slots = playerInventory.Slots;

        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            InventorySlot data = i < slots.Count ? slots[i] : null;
            spawnedSlots[i].Refresh(data);
        }
    }
}
