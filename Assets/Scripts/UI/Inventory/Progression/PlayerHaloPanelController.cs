using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Zone haute inventaire : portrait, niveau joueur (mock), 8 slots halo.
/// </summary>
public class PlayerHaloPanelController : MonoBehaviour
{
    private const string DefaultLevelFormat = "Niveau {0}";

    [Header("Centre")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Animator portraitAnimator;

    [Header("Slots")]
    [SerializeField] private PlayerHaloSlotUI[] haloSlots = Array.Empty<PlayerHaloSlotUI>();

    [Header("Placeholder — mock")]
    [SerializeField] private int mockPlayerLevel = 1;

    public event Action<string> OnTrackSelected;

    private void Awake()
    {
        EnsureSlotsFromChildren();
        WireSlots();
        RefreshPlaceholderPresentation();
    }

    private void OnDestroy()
    {
        UnwireSlots();
    }

    /// <summary>Rafraîchit le niveau centre (brancher XP service plus tard).</summary>
    public void RefreshPlayerLevel(int level)
    {
        mockPlayerLevel = Mathf.Max(1, level);
        if (levelLabel != null)
            levelLabel.text = string.Format(DefaultLevelFormat, mockPlayerLevel);
    }

    public void RefreshPlaceholderPresentation()
    {
        RefreshPlayerLevel(mockPlayerLevel);

        string[] trackIds = ProgressionTrackId.HaloSlotOrder;
        for (int i = 0; i < haloSlots.Length; i++)
        {
            if (haloSlots[i] == null)
                continue;

            string id = i < trackIds.Length ? trackIds[i] : ProgressionTrackId.Marketing;
            string shortLabel = ProgressionTrackId.GetShortLabel(id);
            haloSlots[i].Configure(id, shortLabel, locked: false, displayLevel: 0);
        }
    }

    private void EnsureSlotsFromChildren()
    {
        if (haloSlots != null && haloSlots.Length > 0)
            return;

        haloSlots = GetComponentsInChildren<PlayerHaloSlotUI>(true);
    }

    private void WireSlots()
    {
        foreach (PlayerHaloSlotUI slot in haloSlots)
        {
            if (slot == null)
                continue;

            slot.OnClicked -= HandleSlotClicked;
            slot.OnClicked += HandleSlotClicked;
        }
    }

    private void UnwireSlots()
    {
        foreach (PlayerHaloSlotUI slot in haloSlots)
        {
            if (slot == null)
                continue;

            slot.OnClicked -= HandleSlotClicked;
        }
    }

    private void HandleSlotClicked(PlayerHaloSlotUI slot)
    {
        if (slot == null || string.IsNullOrEmpty(slot.TrackId))
            return;

        OnTrackSelected?.Invoke(slot.TrackId);
    }
}
