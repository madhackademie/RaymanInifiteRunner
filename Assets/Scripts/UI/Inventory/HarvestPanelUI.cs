using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup affiché quand le joueur clique sur une plante dans la grille.
/// Affiche le sprite du stade courant, un timer, et des boutons conditionnels
/// selon le stade (récolter si Mature, graines si Seedling, arracher dans les deux cas).
/// </summary>
public class HarvestPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Visuals")]
    [SerializeField] private Image plantIcon;
    [SerializeField] private TextMeshProUGUI plantNameLabel;
    [SerializeField] private TextMeshProUGUI stageLabel;
    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private TextMeshProUGUI yieldLabel;

    [Header("Boutons")]
    [SerializeField] private Button harvestButton;
    [SerializeField] private Button uprootButton;
    [SerializeField] private Button closeButton;

    private PlantHarvestInteractor currentTarget;
    private PlantGrow currentPlantGrow;
    private PlantDefinition currentDefinition;
    private bool isOpen;

    // Noms lisibles des stades
    private static readonly System.Collections.Generic.Dictionary<PlantGrow.GrowthStage, string> StageNames =
        new()
        {
            { PlantGrow.GrowthStage.Graine,    "Graine"      },
            { PlantGrow.GrowthStage.Starting,  "Germination" },
            { PlantGrow.GrowthStage.Baby,      "Plantule"    },
            { PlantGrow.GrowthStage.Growing,   "Croissance"  },
            { PlantGrow.GrowthStage.Mature,    "Mature"      },
            { PlantGrow.GrowthStage.Flowering, "Floraison"   },
            { PlantGrow.GrowthStage.Seedling,  "Graines"     },
        };

    private void Awake()
    {
        harvestButton.onClick.AddListener(OnHarvestClicked);
        uprootButton.onClick.AddListener(OnUprootClicked);
        closeButton.onClick.AddListener(Close);
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!isOpen || currentPlantGrow == null)
            return;

        RefreshDynamic();
    }

    // ── API publique ──────────────────────────────────────────────────────────

    /// <summary>
    /// Ouvre le popup pour la plante ciblée, peu importe son stade.
    /// </summary>
    public void Open(PlantHarvestInteractor interactor, PlantGrow plantGrow, PlantDefinition definition)
    {
        currentTarget     = interactor;
        currentPlantGrow  = plantGrow;
        currentDefinition = definition;

        plantNameLabel.text = definition != null ? definition.displayName : interactor.gameObject.name;

        RefreshDynamic();

        panel.SetActive(true);
        isOpen = true;
    }

    /// <summary>Ferme le popup.</summary>
    public void Close()
    {
        isOpen = false;
        panel.SetActive(false);
        currentTarget     = null;
        currentPlantGrow  = null;
        currentDefinition = null;
    }

    // ── Rafraîchissement ──────────────────────────────────────────────────────

    /// <summary>Met à jour les éléments qui changent chaque frame (timer, stade, boutons).</summary>
    private void RefreshDynamic()
    {
        PlantGrow.GrowthStage stage = currentPlantGrow.CurrentStage;

        // Sprite du stade courant
        Sprite stageSprite = GetSpriteForStage(stage);
        plantIcon.sprite  = stageSprite;
        plantIcon.enabled = stageSprite != null;

        // Nom du stade
        stageLabel.text = StageNames.TryGetValue(stage, out string name) ? name : stage.ToString();

        // Timer
        float progress = currentPlantGrow.StageProgress;
        float duration = currentDefinition != null ? currentDefinition.GetDuration(stage) : 0f;
        if (duration > 0f)
        {
            float remaining = duration * (1f - progress);
            timerLabel.text = FormatTime(remaining);
        }
        else
        {
            timerLabel.text = "—";
        }

        // Stade récoltable → config issue de la PlantDefinition
        HarvestStageConfig? harvestConfig = currentDefinition?.GetHarvestConfig(stage);
        bool isHarvestable = harvestConfig.HasValue;

        // Label quantité
        if (isHarvestable)
        {
            int min = harvestConfig.Value.harvestAmountMin;
            int max = harvestConfig.Value.harvestAmountMax;
            yieldLabel.text = min == max ? $"x{min}" : $"x{min}–{max}";
            yieldLabel.gameObject.SetActive(true);
        }
        else
        {
            yieldLabel.gameObject.SetActive(false);
        }

        // Bouton récolter : visible et interactable uniquement si récoltable
        harvestButton.gameObject.SetActive(isHarvestable);
        harvestButton.interactable = isHarvestable;
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private void OnHarvestClicked()
    {
        if (currentTarget == null) { Close(); return; }
        currentTarget.ConfirmHarvest();
        Close();
    }

    private void OnUprootClicked()
    {
        if (currentTarget == null) { Close(); return; }
        currentTarget.Uproot();
        Close();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Sprite GetSpriteForStage(PlantGrow.GrowthStage stage)
    {
        if (currentDefinition == null)
            return null;

        return stage switch
        {
            PlantGrow.GrowthStage.Graine    => currentDefinition.spriteGraine,
            PlantGrow.GrowthStage.Starting  => currentDefinition.spriteStarting,
            PlantGrow.GrowthStage.Baby      => currentDefinition.spriteBaby,
            PlantGrow.GrowthStage.Growing   => currentDefinition.spriteGrowing,
            PlantGrow.GrowthStage.Mature    => currentDefinition.spriteMature,
            PlantGrow.GrowthStage.Flowering => currentDefinition.spriteFlowering,
            PlantGrow.GrowthStage.Seedling  => currentDefinition.spriteSeedling,
            _                               => null
        };
    }

    private static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return m > 0 ? $"{m}m {s:D2}s" : $"{s}s";
    }
}
