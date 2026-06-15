using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vue UI d'un noeud d'arbre de talents (position fixee dans le prefab editeur).
/// </summary>
public class TalentNodeView : MonoBehaviour
{
    [Header("Definition")]
    [SerializeField] private TalentNodeDefinition nodeDefinition;

    [Header("Visuel")]
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject availableOverlay;
    [SerializeField] private GameObject purchasedOverlay;

    private TalentProgressionService progressionService;

    public string NodeId => nodeDefinition != null ? nodeDefinition.NodeId : string.Empty;
    public TalentNodeDefinition Definition => nodeDefinition;
    public RectTransform NodeRect => transform as RectTransform;

    private void Awake()
    {
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(HandlePurchaseClicked);
    }

    private void OnDestroy()
    {
        if (purchaseButton != null)
            purchaseButton.onClick.RemoveListener(HandlePurchaseClicked);
    }

    public void Bind(TalentProgressionService service)
    {
        progressionService = service;
        Refresh();
    }

    public void Unbind()
    {
        progressionService = null;
    }

    public void Refresh()
    {
        if (nodeDefinition == null)
        {
            ApplyMissingDefinitionState();
            return;
        }

        if (titleLabel != null)
            titleLabel.text = nodeDefinition.DisplayName;

        TalentNodeStatus status = progressionService != null
            ? progressionService.GetNodeStatus(nodeDefinition.NodeId)
            : TalentNodeStatus.Locked;

        ApplyStatusVisuals(status);
        UpdatePurchaseButton(status);
    }

    private void HandlePurchaseClicked()
    {
        if (progressionService == null || nodeDefinition == null)
            return;

        if (!progressionService.TryPurchaseNode(nodeDefinition.NodeId, out _))
            return;

        Refresh();
    }

    private void ApplyStatusVisuals(TalentNodeStatus status)
    {
        SetOverlayActive(lockedOverlay, status == TalentNodeStatus.Locked);
        SetOverlayActive(availableOverlay, status == TalentNodeStatus.Available);
        SetOverlayActive(
            purchasedOverlay,
            status == TalentNodeStatus.Purchased || status == TalentNodeStatus.Maxed);
    }

    private void UpdatePurchaseButton(TalentNodeStatus status)
    {
        if (purchaseButton == null)
            return;

        bool canPurchase = status == TalentNodeStatus.Available;
        purchaseButton.interactable = canPurchase;
    }

    private void ApplyMissingDefinitionState()
    {
        if (titleLabel != null)
            titleLabel.text = "Noeud (SO manquant)";

        SetOverlayActive(lockedOverlay, true);
        SetOverlayActive(availableOverlay, false);
        SetOverlayActive(purchasedOverlay, false);

        if (purchaseButton != null)
            purchaseButton.interactable = false;
    }

    private static void SetOverlayActive(GameObject overlay, bool active)
    {
        if (overlay != null)
            overlay.SetActive(active);
    }
}
