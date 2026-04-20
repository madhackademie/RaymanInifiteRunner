using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

            // Si le prefab a un Canvas wrapper comme racine (Canvas (Environment)),
            // on extrait le slot, on retire immédiatement le wrapper du container
            // pour que le GridLayoutGroup ne le compte pas, puis on le détruit.
            if (slotUI.transform.parent != slotsContainer)
            {
                Transform wrapper = slotUI.transform.parent;
                slotUI.transform.SetParent(slotsContainer, false);
                wrapper.SetParent(null);
                Destroy(wrapper.gameObject);
            }

            spawnedSlots.Add(slotUI);
        }

        // Force le calcul du layout avant Refresh pour que les slots soient
        // correctement positionnés dans le viewport dès le premier frame.
        LayoutRebuilder.ForceRebuildLayoutImmediate(slotsContainer as RectTransform);

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
