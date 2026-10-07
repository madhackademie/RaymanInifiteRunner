using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup affiché quand le joueur clique sur une plante dans la grille.
/// Affiche l’icône produit (stade Mature), le stade courant, un timer, et des boutons conditionnels
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
    [SerializeField] private Image stageProgressFill;

    [Header("Boutons")]
    [SerializeField] private Button harvestButton;
    [SerializeField] private Button uprootButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button batchHarvestButton;

    private const float DisabledAlpha = 0.35f;

    // Alpha d'origine des graphics (targetGraphic + TMP) des boutons de travail, pour restauration.
    private readonly System.Collections.Generic.Dictionary<Graphic, float> defaultGraphicAlphas = new();

    private PlantHarvestInteractor currentTarget;
    private PlantGrow currentPlantGrow;
    private PlantDefinition currentDefinition;
    private ScreenPopupHost farmPopupHost;
    private BiofiltreManager biofiltreManager;
    private bool isOpen;

    // Suivi du stade rendu : les visuels (sprite, nom, rendement, boutons) ne dépendent
    // que du stade, on évite donc de les recalculer à chaque frame — seul le timer l'est.
    private PlantGrow.GrowthStage lastRenderedStage;
    private bool hasRenderedStage;

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
        if (cancelButton != null)
            cancelButton.onClick.AddListener(Close);
        if (batchHarvestButton != null)
            batchHarvestButton.onClick.AddListener(OnBatchHarvestClicked);
        panel.SetActive(false);
    }

    private void Update()
    {
        if (!isOpen || currentPlantGrow == null)
            return;

        RefreshDynamic();
    }

    // ── API publique ──────────────────────────────────────────────────────────

    /// <summary>Injecte l'hôte popup ferme (depuis <see cref="BiofiltreManager"/>).</summary>
    public void InjectFarmPopupHost(ScreenPopupHost host)
    {
        if (host != null)
            farmPopupHost = host;
    }

    /// <summary>Retire la surbrillance socle à la fermeture du panneau.</summary>
    public void InjectBiofiltreManager(BiofiltreManager manager)
    {
        if (manager != null)
            biofiltreManager = manager;
    }

    /// <summary>
    /// Ouvre le popup pour la plante ciblée, peu importe son stade.
    /// </summary>
    public void Open(PlantHarvestInteractor interactor, PlantGrow plantGrow, PlantDefinition definition)
    {
        currentTarget     = interactor;
        currentPlantGrow  = plantGrow;
        currentDefinition = definition;

        plantNameLabel.text = definition != null ? definition.displayName : interactor.gameObject.name;

        hasRenderedStage = false;
        RefreshDynamic();

        panel.SetActive(true);
        isOpen = true;
        RefreshActionPointButtons();
        SubscribeActionPoints();
    }

    /// <summary>Ferme le panneau et masque l'instance lazy via le host.</summary>
    public void Close()
    {
        isOpen = false;
        UnsubscribeActionPoints();
        panel.SetActive(false);
        currentTarget     = null;
        currentPlantGrow  = null;
        currentDefinition = null;

        biofiltreManager?.ClearPlantSelectionHighlight();

        if (farmPopupHost != null)
            farmPopupHost.TryHidePopup(PopupId.FarmPlantHarvest);
    }

    // ── Rafraîchissement ──────────────────────────────────────────────────────

    /// <summary>
    /// Rafraîchit le panneau : visuels du stade (uniquement si le stade a changé) + timer (chaque frame).
    /// </summary>
    private void RefreshDynamic()
    {
        PlantGrow.GrowthStage stage = currentPlantGrow.CurrentStage;

        if (!hasRenderedStage || stage != lastRenderedStage)
        {
            RefreshStageVisuals(stage);
            lastRenderedStage = stage;
            hasRenderedStage = true;
        }

        RefreshTimer(stage);
    }

    /// <summary>Met à jour les éléments dépendant uniquement du stade (icône produit, nom, rendement, boutons).</summary>
    private void RefreshStageVisuals(PlantGrow.GrowthStage stage)
    {
        Sprite productIcon = GetHarvestProductIconSprite();
        plantIcon.sprite  = productIcon;
        plantIcon.enabled = productIcon != null;

        stageLabel.text = StageNames.TryGetValue(stage, out string name) ? name : stage.ToString();

        HarvestStageConfig? harvestConfig = currentDefinition?.GetHarvestConfig(stage);
        bool isHarvestable = harvestConfig.HasValue;

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

        harvestButton.gameObject.SetActive(isHarvestable);
        harvestButton.interactable = isHarvestable;
        RefreshActionPointButtons();
    }

    /// <summary>
    /// Grise harvest / batch / arrachage si PA insuffisants (même coût travail V0). Annuler reste actif.
    /// </summary>
    private void RefreshActionPointButtons()
    {
        bool canAffordWork = CanAffordAction(ActionPointActionId.Harvest);

        if (harvestButton != null && harvestButton.gameObject.activeSelf)
        {
            bool isHarvestable = currentDefinition != null && currentPlantGrow != null
                                 && currentDefinition.GetHarvestConfig(currentPlantGrow.CurrentStage).HasValue;
            harvestButton.interactable = isHarvestable;
        }

        if (batchHarvestButton != null && batchHarvestButton.gameObject.activeSelf)
            batchHarvestButton.interactable = true;

        if (uprootButton != null && uprootButton.gameObject.activeSelf)
            uprootButton.interactable = true;

        ApplyWorkButtonPaVisual(harvestButton, canAffordWork);
        ApplyWorkButtonPaVisual(batchHarvestButton, canAffordWork);
        ApplyWorkButtonPaVisual(uprootButton, canAffordWork);
    }

    /// <summary>
    /// Grise visuellement un bouton de travail si PA insuffisants, sans toucher à interactable.
    /// Agit sur le targetGraphic et les TMP enfants (alpha), plus un CanvasGroup optionnel.
    /// </summary>
    private void ApplyWorkButtonPaVisual(Button button, bool canAffordWork)
    {
        if (button == null || !button.gameObject.activeSelf)
            return;

        if (button.targetGraphic != null)
            SetGraphicAlpha(button.targetGraphic, canAffordWork);

        TextMeshProUGUI[] labels = button.GetComponentsInChildren<TextMeshProUGUI>(false);
        for (int i = 0; i < labels.Length; i++)
            SetGraphicAlpha(labels[i], canAffordWork);

        if (button.TryGetComponent(out CanvasGroup group))
            group.alpha = canAffordWork ? 1f : DisabledAlpha;
    }

    // Applique l'alpha grisé ou restaure l'alpha d'origine (mis en cache au premier usage), RGB préservé.
    private void SetGraphicAlpha(Graphic graphic, bool canAffordWork)
    {
        if (graphic == null)
            return;

        if (!defaultGraphicAlphas.TryGetValue(graphic, out float defaultAlpha))
        {
            defaultAlpha = graphic.color.a;
            defaultGraphicAlphas[graphic] = defaultAlpha;
        }

        Color color = graphic.color;
        color.a = canAffordWork ? defaultAlpha : DisabledAlpha;
        graphic.color = color;
    }

    // Déclenche le feedback PA (throttle dans le service) sans effet gameplay.
    private static void PlayPaRefuseFeedback(string actionId)
    {
        if (ActionPointService.Instance != null && !CanAffordAction(actionId))
            ActionPointService.Instance.TryConsume(actionId, out _);
    }

    // Retourne true si les PA sont suffisants (ou si le service est absent).
    private static bool CanAffordAction(string actionId)
    {
        ActionPointService service = ActionPointService.Instance;
        return service == null || service.CanAfford(service.GetCostForAction(actionId));
    }

    private void SubscribeActionPoints()
    {
        UnsubscribeActionPoints();
        if (ActionPointService.Instance != null)
            ActionPointService.Instance.OnActionPointsChanged += HandleActionPointsChanged;
    }

    private void UnsubscribeActionPoints()
    {
        if (ActionPointService.Instance != null)
            ActionPointService.Instance.OnActionPointsChanged -= HandleActionPointsChanged;
    }

    private void HandleActionPointsChanged()
    {
        if (isOpen)
            RefreshActionPointButtons();
    }

    private void OnDisable()
    {
        UnsubscribeActionPoints();
    }

    private void OnDestroy()
    {
        UnsubscribeActionPoints();
    }

    /// <summary>Met à jour le label de temps restant du stade courant (rafraîchi chaque frame).</summary>
    private void RefreshTimer(PlantGrow.GrowthStage stage)
    {
        float duration = currentDefinition != null ? currentDefinition.GetDuration(stage) : 0f;
        if (duration > 0f)
        {
            float remaining = duration * (1f - currentPlantGrow.StageProgress);
            timerLabel.text = FormatTime(remaining);
        }
        else
        {
            timerLabel.text = "—";
        }

        RefreshStageProgress(duration);
    }

    // Durée nulle = stade sans attente : barre pleine. Sinon avancement du stade courant.
    private void RefreshStageProgress(float duration)
    {
        if (stageProgressFill == null || currentPlantGrow == null)
            return;

        float progress = duration > 0f ? currentPlantGrow.StageProgress : 1f;
        stageProgressFill.fillAmount = Mathf.Clamp01(progress);
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private void OnHarvestClicked()
    {
        if (!CanAffordAction(ActionPointActionId.Harvest)) { PlayPaRefuseFeedback(ActionPointActionId.Harvest); return; }
        if (currentTarget == null) { Close(); return; }
        currentTarget.ConfirmHarvest();
        Close();
    }

    private void OnBatchHarvestClicked()
    {
        if (!CanAffordAction(ActionPointActionId.Harvest)) { PlayPaRefuseFeedback(ActionPointActionId.Harvest); return; }
        Close();
        biofiltreManager?.ArmBatchHarvest();
    }

    private void OnUprootClicked()
    {
        if (!CanAffordAction(ActionPointActionId.Harvest)) { PlayPaRefuseFeedback(ActionPointActionId.Harvest); return; }
        if (currentTarget == null) { Close(); return; }
        currentTarget.Uproot();
        Close();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Icône popup = laitue / produit récoltable (stade Mature, slot SO), pas le sprite de croissance courant.
    /// </summary>
    private Sprite GetHarvestProductIconSprite()
    {
        if (currentDefinition == null)
            return null;

        Sprite mature = currentDefinition.GetSprite(PlantGrow.GrowthStage.Mature);
        if (mature != null)
            return mature;

        return currentDefinition.GetSprite(currentPlantGrow != null
            ? currentPlantGrow.CurrentStage
            : PlantGrow.GrowthStage.Mature);
    }

    private static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return m > 0 ? $"{m}m {s:D2}s" : $"{s}s";
    }
}
