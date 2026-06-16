using UnityEngine;

/// <summary>
/// Orchestration de l'écran inventaire scindé : halo, grille (legacy), overlay talents.
/// Zone footer (A) hors scope.
/// </summary>
public class InventoryScreenController : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] private PlayerHaloPanelController haloPanel;
    [SerializeField] private TalentTreeOverlayController talentTreeOverlay;
    [Header("Grille — atténuation")]
    [SerializeField] private CanvasGroup inventoryBodyCanvasGroup;
    [SerializeField] private GameObject screenDimmer;

    [Header("Filtres (phase 2)")]
    [SerializeField] private GameObject filterBarPlaceholder;

    [SerializeField] private float inventoryDimAlphaWhenTreeOpen = 0.35f;

    private bool wired;

    private void Awake()
    {
        ResolveScreenDimmer();
        WireModules();
    }

    private void OnEnable()
    {
        if (filterBarPlaceholder != null)
            filterBarPlaceholder.SetActive(false);

        SetInventoryBodyDim(0f);
    }

    private void OnDestroy()
    {
        UnwireModules();
    }

    private void WireModules()
    {
        if (wired)
            return;

        if (haloPanel != null)
            haloPanel.OnTrackSelected += HandleTrackSelected;

        if (talentTreeOverlay != null)
            talentTreeOverlay.Closed += HandleTalentTreeClosed;

        wired = true;
    }

    private void UnwireModules()
    {
        if (!wired)
            return;

        if (haloPanel != null)
            haloPanel.OnTrackSelected -= HandleTrackSelected;

        if (talentTreeOverlay != null)
            talentTreeOverlay.Closed -= HandleTalentTreeClosed;

        wired = false;
    }

    private void HandleTrackSelected(string trackId)
    {
        if (talentTreeOverlay == null)
        {
            Debug.LogWarning(
                $"[InventoryScreenController] Overlay talents absent — piste '{trackId}' ignorée.");
            return;
        }

        SetInventoryBodyDim(inventoryDimAlphaWhenTreeOpen);
        SetScreenDimmerActive(false);
        talentTreeOverlay.Open(trackId);
    }

    private void HandleTalentTreeClosed()
    {
        SetInventoryBodyDim(0f);
        SetScreenDimmerActive(true);
    }

    private void ResolveScreenDimmer()
    {
        if (screenDimmer != null)
            return;

        Transform dimmer = transform.Find("Dimmer");
        if (dimmer != null)
            screenDimmer = dimmer.gameObject;
    }

    private void SetScreenDimmerActive(bool active)
    {
        if (screenDimmer != null)
            screenDimmer.SetActive(active);
    }

    private void SetInventoryBodyDim(float dimAmount)
    {
        if (inventoryBodyCanvasGroup == null)
            return;

        bool dimmed = dimAmount > 0f;
        inventoryBodyCanvasGroup.alpha = dimmed ? inventoryDimAlphaWhenTreeOpen : 1f;
        inventoryBodyCanvasGroup.interactable = !dimmed;
        inventoryBodyCanvasGroup.blocksRaycasts = !dimmed;
    }
}
