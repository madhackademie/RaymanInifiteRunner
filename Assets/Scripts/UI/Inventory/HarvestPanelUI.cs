using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel de récolte : affiché quand le joueur clique sur une plante mature.
/// Montre l'icône et le nom de la plante, puis applique la récolte au clic sur "Récolter".
/// </summary>
public class HarvestPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Visuals")]
    [SerializeField] private Image plantIcon;
    [SerializeField] private TextMeshProUGUI plantNameLabel;
    [SerializeField] private TextMeshProUGUI harvestAmountLabel;

    [Header("Boutons")]
    [SerializeField] private Button harvestButton;
    [SerializeField] private Button cancelButton;

    private PlantHarvestInteractor currentTarget;

    private void Awake()
    {
        harvestButton.onClick.AddListener(OnHarvestClicked);
        cancelButton.onClick.AddListener(Close);
        panel.SetActive(false);
    }

    // ── API publique ──────────────────────────────────────────────────────────

    /// <summary>
    /// Ouvre le panneau pour la plante ciblée.
    /// </summary>
    public void Open(PlantHarvestInteractor interactor, PlantDefinition definition, int harvestAmount)
    {
        currentTarget = interactor;

        plantIcon.sprite         = definition.spriteMature;
        plantIcon.enabled        = definition.spriteMature != null;
        plantNameLabel.text      = definition.displayName;
        harvestAmountLabel.text  = $"x{harvestAmount}";

        panel.SetActive(true);
    }

    /// <summary>Ferme le panneau sans récolter.</summary>
    public void Close()
    {
        panel.SetActive(false);
        currentTarget = null;
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private void OnHarvestClicked()
    {
        if (currentTarget == null)
        {
            Close();
            return;
        }

        currentTarget.ConfirmHarvest();
        Close();
    }
}
