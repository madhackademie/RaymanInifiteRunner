using UnityEngine;

/// <summary>
/// Rangée d’étoiles : N enfants <see cref="UiStarSlotView"/> (instances du prefab slot).
/// Le nombre de slots visibles et le fill sont pilotés de l’extérieur — pas de logique métier.
/// </summary>
public class UiStarRowView : MonoBehaviour
{
    /// <summary>Capacité UI prestige / canaux : courbe ★1–5 (GDD vente + biofiltre).</summary>
    public const int PrestigeStarCapacity = 5;

    [SerializeField] private UiStarSlotView[] slots;
    [SerializeField] [Min(1)] private int visibleSlotCount = PrestigeStarCapacity;
    [SerializeField] [Min(0)] private int filledOnStart;

    public int VisibleSlotCount => visibleSlotCount;
    public int SlotCapacity => slots != null ? slots.Length : 0;

    private void Awake()
    {
        ApplyVisibleCount(visibleSlotCount);
        ApplyFilledCount(filledOnStart);
    }

    /// <summary>Combien de slots afficher (les suivants sont désactivés).</summary>
    public void SetVisibleSlotCount(int count)
    {
        ApplyVisibleCount(count);
    }

    /// <summary>Remplit les N premiers slots actifs (0 = tous vides).</summary>
    public void SetFilledCount(int filledCount)
    {
        ApplyFilledCount(filledCount);
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

    private void ApplyFilledCount(int filledCount)
    {
        if (slots == null)
            return;

        int maxFill = Mathf.Min(visibleSlotCount, slots.Length);
        int filled = Mathf.Clamp(filledCount, 0, maxFill);

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            bool active = i < visibleSlotCount;
            slots[i].SetFilled(active && i < filled);
        }
    }
}
