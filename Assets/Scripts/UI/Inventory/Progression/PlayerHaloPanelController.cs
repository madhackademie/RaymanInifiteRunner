using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Zone haute inventaire : portrait, niveau joueur (mock), 8 slots halo.
/// </summary>
public class PlayerHaloPanelController : MonoBehaviour
{
    private const string DefaultLevelFormat = "Niveau {0}";
    private const string ClickTriggerName = "Click";
    private const float DefaultClickFeedbackDelay = 0.18f;

    [Header("Centre")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private Animator portraitAnimator;

    [Header("Slots")]
    [SerializeField] private PlayerHaloSlotUI[] haloSlots = Array.Empty<PlayerHaloSlotUI>();

    [Header("Feedback clic")]
    [Tooltip("Laisse le punch Animator visible avant d'ouvrir l'overlay talents.")]
    [SerializeField] private float clickFeedbackDelay = DefaultClickFeedbackDelay;

    [Header("Placeholder — mock")]
    [SerializeField] private int mockPlayerLevel = 1;

    public event Action<string> OnTrackSelected;

    private Coroutine pendingTrackRoutine;

    private void Awake()
    {
        EnsureSlotsFromChildren();
        WireSlots();
        RefreshPlaceholderPresentation();
    }

    private void OnDestroy()
    {
        UnwireSlots();
        StopPendingTrackRoutine();
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

            string id = i < trackIds.Length ? trackIds[i] : ProgressionTrackId.Reserved07;
            string shortLabel = ProgressionTrackId.GetShortLabel(id);
            bool isLocked = ProgressionTrackId.IsReserved(id);
            haloSlots[i].Configure(id, shortLabel, isLocked, displayLevel: 0);
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

        if (pendingTrackRoutine != null)
            return;

        slot.PlayTrigger(ClickTriggerName);
        pendingTrackRoutine = StartCoroutine(OpenTrackAfterClickFeedback(slot.TrackId));
    }

    private IEnumerator OpenTrackAfterClickFeedback(string trackId)
    {
        float delay = Mathf.Max(0f, clickFeedbackDelay);
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        pendingTrackRoutine = null;
        OnTrackSelected?.Invoke(trackId);
    }

    private void StopPendingTrackRoutine()
    {
        if (pendingTrackRoutine == null)
            return;

        StopCoroutine(pendingTrackRoutine);
        pendingTrackRoutine = null;
    }
}
