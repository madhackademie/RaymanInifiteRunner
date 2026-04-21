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

        // Le prefab a un Canvas (Environment) à la racine — on extrait InventorySlotUI
        // directement sous SlotsContainer pour que GridLayoutGroup le positionne correctement.
        if (slotUI.transform.parent != slotsContainer)
        {
            Transform wrapper = slotUI.transform.parent;
            slotUI.transform.SetParent(slotsContainer, false);
            wrapper.SetParent(null);
            Destroy(wrapper.gameObject);
        }

        spawnedSlots.Add(slotUI);
    }

    Canvas.ForceUpdateCanvases();

    RectTransform ct = slotsContainer as RectTransform;
        Debug.Log($"[InventoryUI] BuildSlots — Content.childCount={slotsContainer.childCount}, Content.rect={ct?.rect}");
        if (slotsContainer.childCount > 0)
        {
            RectTransform first = slotsContainer.GetChild(0) as RectTransform;
            Debug.Log($"[InventoryUI] Slot[0] — anchoredPos={first?.anchoredPosition}, sizeDelta={first?.sizeDelta}, active={first?.gameObject.activeSelf}");
        }
        Debug.Log($"[InventoryUI] BuildSlots — END");

    Refresh();
}

    
    // private void BuildSlots()
    // {
    //     foreach (InventorySlotUI slot in spawnedSlots)
    //         Destroy(slot.gameObject);

    //     spawnedSlots.Clear();

    //     foreach (InventorySlot _ in playerInventory.Slots)
    //     {
    //        // Instancie uniquement le sous-arbre InventorySlotUI (sans le Canvas wrapper).
    //         InventorySlotUI slotUI = Instantiate(slotPrefab.gameObject, slotsContainer)
    //             .GetComponent<InventorySlotUI>();

    //         spawnedSlots.Add(slotUI);
    //     }

    //     // Force canvas + layout pour s'assurer que les slots sont positionnés
    //     // et rendus correctement dès le premier frame.
    //     Canvas.ForceUpdateCanvases();

        

    //     Refresh();

    // }

    /// <summary>Repopulates all slot UIs from the current inventory state.</summary>
    public void Refresh()
    {
        if (playerInventory == null)
            return;

        IReadOnlyList<InventorySlot> slots = playerInventory.Slots;

        int nonEmpty = 0;
        foreach (InventorySlot s in slots)
            if (!s.IsEmpty) nonEmpty++;
        Debug.Log($"[InventoryUI] Refresh — spawnedSlots={spawnedSlots.Count}, non-empty={nonEmpty}");

        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            InventorySlot data = i < slots.Count ? slots[i] : null;
            spawnedSlots[i].Refresh(data);
        }
    }
}
