using UnityEngine;

/// <summary>
/// Rangée de slots biofiltre : N enfants <see cref="UiBiofiltreSlotView"/> nested.
/// Calque <see cref="UiStarRowView"/> — visible count et états pilotés de l’extérieur.
/// </summary>
public class UiBiofiltreSlotRowView : MonoBehaviour
{
    public const int PrimaryCapacity = 3;
    public const int SecondaryCapacity = 5;

    [SerializeField] private UiBiofiltreSlotView[] slots;
    [SerializeField] [Min(1)] private int visibleSlotCount = PrimaryCapacity;
    [SerializeField] private BiofiltreSlotVisualState stateOnStart = BiofiltreSlotVisualState.Locked;

    public int VisibleSlotCount => visibleSlotCount;
    public int SlotCapacity => slots != null ? slots.Length : 0;

    private void Awake()
    {
        ApplyVisibleCount(visibleSlotCount);
        ApplyAllState(stateOnStart);
    }

    public void SetVisibleSlotCount(int count)
    {
        ApplyVisibleCount(count);
    }

    public void SetAllLocked()
    {
        ApplyAllState(BiofiltreSlotVisualState.Locked);
    }

    public void SetSlotState(int index, BiofiltreSlotVisualState state)
    {
        if (slots == null || index < 0 || index >= slots.Length || slots[index] == null)
            return;

        slots[index].SetState(state);
    }

    private void ApplyVisibleCount(int count)
    {
        if (slots == null || slots.Length == 0)
        {
            visibleSlotCount = 0;
            return;
        }

        visibleSlotCount = Mathf.Clamp(count, 0, slots.Length);

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            slots[i].gameObject.SetActive(i < visibleSlotCount);
        }
    }

    private void ApplyAllState(BiofiltreSlotVisualState state)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null || i >= visibleSlotCount)
                continue;

            slots[i].SetState(state);
        }
    }
}
