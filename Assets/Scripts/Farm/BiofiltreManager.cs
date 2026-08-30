using UnityEngine;

/// <summary>
/// Bridges the biofiltre grid and the planting UI.
/// Listens to cell clicks from <see cref="BiofiltreGridVisualizer"/>,
/// opens the seed selection panel for empty cells, and handles plant placement.
/// </summary>
[RequireComponent(typeof(BiofiltreGridVisualizer))]
[RequireComponent(typeof(GridManager))]
public class BiofiltreManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Hôte popups ferme (ScreenPopupHost, ex. sur LevelController dans FirstLvl).")]
    [SerializeField] private ScreenPopupHost farmPopupHost;

    [Tooltip("Parent RectTransform des popups lazy (optionnel — sinon FarmUICanvas créé sous le host).")]
    [SerializeField] private RectTransform farmPopupRoot;

    [Tooltip("Preview de placement injecté dans SeedSelectionUI à l'instanciation.")]
    [SerializeField] private PlantPlacementPreview placementPreview;

    [Header("Harvest")]
    [Tooltip("Base de données d'items pour résoudre les récoltes. Injectée dans chaque PlantHarvestInteractor.")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Persistence Prototype")]
    [Tooltip("Active la sauvegarde JSON locale des plantes posees sur la grille.")]
    [SerializeField] private bool enablePrototypePersistence = true;

    [Header("VFX")]
    [Tooltip("Burst terre / vers — Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab")]
    [SerializeField] private GameObject plantingDirtBurstPrefab;

    [Tooltip("Délai avant Destroy du VFX (durée PS + marge).")]
    [SerializeField] private float plantingDirtBurstDestroyDelay = 2f;

    private BiofiltreGridVisualizer visualizer;
    private GridManager gridManager;
    private bool hasLoadedFromSave;
    private bool runtimeInitialized;
    private SeedSelectionUI cachedSeedSelectionUi;
    private HarvestPanelUI cachedHarvestPanelUi;

    // Suppression du clic de placement : le clic confirmant la pose est livré par
    // l'EventSystem au relâchement (souvent une frame plus tard que l'instanciation
    // de la plante). On suppresse donc jusqu'au relâchement effectif + quelques frames
    // de grâce, pour couvrir l'ordre d'exécution EventSystem vs scripts.
    private bool awaitingPlacementPointerRelease;
    private int placementReleaseGraceFrames;

    private void Awake()
    {
        visualizer  = GetComponent<BiofiltreGridVisualizer>();
        gridManager = GetComponent<GridManager>();

        if (placementPreview == null)
            placementPreview = GetComponent<PlantPlacementPreview>();
    }

    private void OnEnable()
    {
        visualizer.OnCellClicked += HandleCellClicked;
        FarmPersistenceCoordinator.Register(this);

        // Scène masquée via SceneNavigator : Start ne repasse pas — recalcul UTC ici.
        if (runtimeInitialized)
            ApplyOfflineGrowthSinceLastSave();
    }

    private void OnDisable()
    {
        visualizer.OnCellClicked -= HandleCellClicked;
        SaveFarmState();
        FarmPersistenceCoordinator.Unregister(this);
    }

    private void Start()
    {
        visualizer.GenerateGrid();

        RegisterFarmPopupBindingsIfPossible();
        WarmUpSeedSelectionPopup();
        WarmUpHarvestPanelPopup();

        TryLoadFarmState();
        runtimeInitialized = true;
    }

    /// <summary>Sauvegarde JSON demandée par <see cref="FarmPersistenceCoordinator"/> (quit / pause).</summary>
    public void FlushPersistence() => SaveFarmState();

    /// <summary>Croissance offline au retour focus / reprise sans quitter la scène.</summary>
    public void ApplyOfflinePersistence() => ApplyOfflineGrowthSinceLastSave();

    private void Update()
    {
        if (awaitingPlacementPointerRelease)
        {
            if (!FarmPointerInput.IsPrimaryHeld())
            {
                // Relâchement détecté : le clic (OnPointerClick) est livré sur cette frame.
                // On conserve la suppression encore quelques frames de grâce.
                awaitingPlacementPointerRelease = false;
                placementReleaseGraceFrames = 2;
            }
        }
        else if (placementReleaseGraceFrames > 0)
        {
            placementReleaseGraceFrames--;
        }
    }

    // ── Cell click ────────────────────────────────────────────────────────────

    /// <summary>
    /// Appelé quand un clic souris confirme/annule une pose : on neutralise ce clic
    /// jusqu'à son relâchement pour qu'il ne déclenche pas graines / info plante.
    /// </summary>
    public void SuppressFarmPointerUiUntilPointerRelease()
    {
        awaitingPlacementPointerRelease = true;
        placementReleaseGraceFrames = Mathf.Max(placementReleaseGraceFrames, 1);
    }

    /// <summary>True tant que le fantôme de placement suit la souris.</summary>
    public bool IsPlantPlacementPreviewActive =>
        placementPreview != null && placementPreview.enabled;

    /// <summary>Bloque les clics ferme déjà consommés (grille ou plante sous la souris).</summary>
    public bool ShouldSuppressFarmPointerUi =>
        awaitingPlacementPointerRelease ||
        placementReleaseGraceFrames > 0 ||
        IsPlantPlacementPreviewActive;

    private bool ShouldBlockGridCellUi => ShouldSuppressFarmPointerUi;

    private void HandleCellClicked(BiofiltreCell cell)
    {
        // Clic déjà consommé par une pose : ne pas (re)déclencher graines / info plante,
        // ni fermer un popup graines volontairement ré-ouvert (état « plus de graines »).
        if (ShouldBlockGridCellUi)
            return;

        if (gridManager.IsCellFree(cell.GridCoordinates))
            TryOpenFarmSeedSelection(cell);
        else
            TryOpenPlantPopup(cell.GridCoordinates);
    }

    /// <summary>Ferme le popup graines (panneau + instance lazy host).</summary>
    public void HideFarmSeedSelectionPopup()
    {
        if (TryResolveSeedSelectionUI(out SeedSelectionUI ui))
            ui.Close();

        ScreenPopupHost host = ResolveFarmPopupHost();
        if (host != null)
            host.TryHidePopup(PopupId.FarmSeedSelection);
    }

    /// <summary>Cache le popup graines au démarrage du mode preview.</summary>
    internal void OnPlacementPreviewStarted()
    {
        HideFarmSeedSelectionPopup();
    }

    /// <summary>
    /// Rouvre le popup graines en état vide après consommation de la dernière graine.
    /// </summary>
    internal void ReopenSeedSelectionAfterLastSeedPlanted(BiofiltreCell contextCell)
    {
        if (contextCell == null)
            return;

        HideFarmPlantHarvestPopup();
        TryOpenFarmSeedSelection(contextCell);
    }

    private void TryOpenPlantPopup(Vector2Int coords)
    {
        GameObject plantObj = gridManager.GetPlantAt(coords);

        if (plantObj == null)
        {
            Debug.Log($"[BiofiltreManager] Aucune plante enregistrée à la cellule {coords}.");
            return;
        }

        PlantGrow plantGrow = plantObj.GetComponent<PlantGrow>();
        PlantDefinitionHolder holder = plantObj.GetComponent<PlantDefinitionHolder>();
        PlantHarvestInteractor interactor = plantObj.GetComponent<PlantHarvestInteractor>();

        if (plantGrow == null)
        {
            Debug.LogWarning($"[BiofiltreManager] PlantGrow manquant sur '{plantObj.name}'.", this);
            return;
        }

        TryOpenFarmPlantHarvestPopup(interactor, plantGrow, holder != null ? holder.Definition : null);
    }

    private bool TryOpenFarmPlantHarvestPopup(
        PlantHarvestInteractor interactor,
        PlantGrow plantGrow,
        PlantDefinition definition)
    {
        ScreenPopupHost host = ResolveFarmPopupHost();
        if (host == null)
            return false;

        if (!host.TryShowPopup(PopupId.FarmPlantHarvest, out HarvestPanelUI panel))
        {
            Debug.LogWarning(
                "[BiofiltreManager] Popup plante / récolte introuvable. Vérifiez UIManager.runtimePopupBindings " +
                $"(screenId={ScreenId.FirstLvlFarm}, popupId={PopupId.FarmPlantHarvest}).",
                this);
            return false;
        }

        ConfigureHarvestPanelInstance(panel);
        panel.Open(interactor, plantGrow, definition);
        return true;
    }

    /// <summary>Ferme le popup récolte (panneau + instance lazy host).</summary>
    public void HideFarmPlantHarvestPopup()
    {
        if (TryResolveHarvestPanelUI(out HarvestPanelUI ui))
            ui.Close();
    }

    // ── Footprint query (called by SeedSelectionUI) ───────────────────────────

    /// <summary>
    /// Vérifie qu'un ancre explicite permet de placer la plante (preview souris).
    /// </summary>
    public bool CanPlace(Vector2Int anchor, PlantDefinition plantDefinition)
    {
        if (plantDefinition == null)
            return false;

        return gridManager.AreAllCellsFree(plantDefinition.GetOccupiedCells(anchor));
    }

    /// <summary>
    /// Vérifie si la plante peut couvrir <paramref name="targetCell"/> (cellule cliquée),
    /// en testant toutes les ancres candidates du footprint — pas seulement targetCell comme (0,0).
    /// </summary>
    public bool CanPlaceAtCell(Vector2Int targetCell, PlantDefinition plantDefinition)
    {
        return TryResolvePlacementAnchor(targetCell, plantDefinition, out _);
    }

    /// <summary>
    /// Résout l'ancre de pose lorsque le joueur a cliqué <paramref name="targetCell"/>.
    /// </summary>
    public bool TryResolvePlacementAnchor(
        Vector2Int targetCell,
        PlantDefinition plantDefinition,
        out Vector2Int anchor)
    {
        anchor = default;

        if (plantDefinition == null)
            return false;

        foreach (Vector2Int candidate in plantDefinition.EnumerateAnchorCandidatesForCell(targetCell))
        {
            if (!gridManager.IsInBounds(candidate))
                continue;

            if (gridManager.AreAllCellsFree(plantDefinition.GetOccupiedCells(candidate)))
            {
                anchor = candidate;
                return true;
            }
        }

        return false;
    }

    // ── Plant placement ───────────────────────────────────────────────────────

    public void PlantSeed(BiofiltreCell cell, PlantDefinition plantDefinition, GameObject plantPrefab)
    {
        PlantSeedAtInternal(cell.GridCoordinates, plantDefinition, plantPrefab, saveAfterPlacement: true);
    }

    public void PlantSeedAt(Vector2Int anchor, PlantDefinition plantDefinition, GameObject plantPrefab)
    {
        PlantSeedAtInternal(anchor, plantDefinition, plantPrefab, saveAfterPlacement: true);
    }

    /// <summary>
    /// Plants at anchor after consuming one <paramref name="seedItem"/> from <see cref="PlayerInventory"/>.
    /// </summary>
    public bool TryPlantSeedAt(
        Vector2Int anchor,
        PlantDefinition plantDefinition,
        GameObject plantPrefab,
        ItemDefinition seedItem)
    {
        if (plantDefinition == null || plantPrefab == null)
        {
            Debug.LogWarning("[BiofiltreManager] TryPlantSeedAt: definition or prefab null.", this);
            return false;
        }

        if (!TryResolvePlacementAnchor(anchor, plantDefinition, out Vector2Int resolvedAnchor))
            return false;

        ActionPointService actionPoints = ActionPointService.Instance;
        int plantCost = actionPoints != null
            ? actionPoints.GetCostForAction(ActionPointActionId.PlantSeed)
            : 0;

        if (actionPoints != null &&
            !actionPoints.TryConsume(ActionPointActionId.PlantSeed, plantCost, out string apMessage))
        {
            Debug.Log($"[BiofiltreManager] TryPlantSeedAt: {apMessage}", this);
            return false;
        }

        if (seedItem != null)
        {
            PlayerInventory inventory = PlayerInventory.Instance;
            if (inventory == null)
            {
                Debug.LogWarning("[BiofiltreManager] TryPlantSeedAt: PlayerInventory introuvable.", this);
                return false;
            }

            if (inventory.TryRemove(seedItem, 1) != InventoryResult.Success)
            {
                Debug.Log("[BiofiltreManager] TryPlantSeedAt: stock graine insuffisant.", this);
                actionPoints?.Refund(plantCost);
                return false;
            }
        }

        if (PlantSeedAtInternal(resolvedAnchor, plantDefinition, plantPrefab, saveAfterPlacement: true))
            return true;

        if (seedItem != null && PlayerInventory.Instance != null)
            PlayerInventory.Instance.TryAdd(seedItem, 1);

        actionPoints?.Refund(plantCost);
        return false;
    }

    /// <summary>Placement sans consommation (restauration sauvegarde ferme).</summary>
    private bool PlantSeedAtInternal(Vector2Int anchor, PlantDefinition plantDefinition, GameObject plantPrefab, bool saveAfterPlacement)
    {
        if (plantDefinition == null || plantPrefab == null)
        {
            Debug.LogWarning("[BiofiltreManager] PlantSeedAt called with null definition or prefab.", this);
            return false;
        }

        foreach (Vector2Int occupied in plantDefinition.GetOccupiedCells(anchor))
        {
            if (!gridManager.IsCellFree(occupied))
            {
                Debug.Log($"[BiofiltreManager] Cannot plant — cell {occupied} is occupied.");
                return false;
            }
        }

        Vector2 worldCenter   = gridManager.GridToWorldCenter(anchor);
        Vector2 spawnPosition = worldCenter + plantDefinition.spriteWorldOffset;
        GameObject instance   = Instantiate(
            plantPrefab,
            spawnPosition,
            Quaternion.identity,
            visualizer.PlantsContainer
        );
        instance.name = $"{plantDefinition.displayName}_{anchor}";

        if (instance.TryGetComponent(out PlantGrow plantGrow))
            plantGrow.SetStage(PlantGrow.GrowthStage.Graine);

        if (instance.TryGetComponent(out PlantDefinitionHolder holder))
            holder.Initialise(plantDefinition);

        if (instance.TryGetComponent(out PlantPersistenceMarker marker))
            marker.Initialise(plantDefinition.plantId, anchor);
        else
            instance.AddComponent<PlantPersistenceMarker>().Initialise(plantDefinition.plantId, anchor);

        if (instance.TryGetComponent(out PlantHarvestInteractor harvestInteractor))
        {
            Vector2Int[] cells = System.Linq.Enumerable.ToArray(plantDefinition.GetOccupiedCells(anchor));
            harvestInteractor.Initialise(gridManager, visualizer, cells);
            harvestInteractor.InjectInventory(itemDatabase);
            harvestInteractor.InjectFarmPopupHost(ResolveFarmPopupHost());
            harvestInteractor.InjectBiofiltreManager(this);
            harvestInteractor.SetOnPlantRemoved(SaveFarmState);
        }

        Vector2Int[] footprintCells = System.Linq.Enumerable.ToArray(plantDefinition.GetOccupiedCells(anchor));
        gridManager.OccupyCells(footprintCells);
        gridManager.RegisterPlant(footprintCells, instance);

        foreach (Vector2Int coords in plantDefinition.GetOccupiedCells(anchor))
        {
            BiofiltreCell affectedCell = visualizer.GetCell(coords);
            affectedCell?.SetVisualState(true);
        }

        Debug.Log($"[BiofiltreManager] Planted '{plantDefinition.displayName}' at {anchor}.");

        // VFX / punch seulement sur placement joueur (pas à la restauration save).
        if (saveAfterPlacement)
        {
            PlayPlantingDirtBurst(instance.transform.position);
            if (instance.TryGetComponent(out PlantPlantingPunch plantingPunch))
                plantingPunch.Play();
            SaveFarmState();
        }

        return true;
    }

    /// <summary>
    /// Burst particules plantation / arrachage / récolte (même prefab).
    /// </summary>
    public void PlayPlantingDirtBurst(Vector3 worldPosition)
    {
        if (plantingDirtBurstPrefab == null)
        {
            Debug.LogWarning(
                "[BiofiltreManager] plantingDirtBurstPrefab non assigné — VFX ignoré. " +
                "Référence : Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab",
                this);
            return;
        }

        FarmDirtBurstVfx.Play(plantingDirtBurstPrefab, worldPosition, plantingDirtBurstDestroyDelay);
    }

    private void TryLoadFarmState()
    {
        if (!enablePrototypePersistence || hasLoadedFromSave)
            return;

        hasLoadedFromSave = true;

        if (!FarmSaveService.TryLoad(out FarmSaveService.FarmSaveData saveData) || saveData.plants == null)
            return;

        if (!TryResolveSeedSelectionUI(out SeedSelectionUI seedCatalog))
        {
            Debug.LogWarning(
                "[BiofiltreManager] Catalogue graines indisponible — sauvegarde ferme non restaurée.",
                this);
            return;
        }

        foreach (Transform child in visualizer.PlantsContainer)
            Destroy(child.gameObject);

        gridManager.ResetRuntimeState();
        visualizer.RefreshAllCellStates();

        float offlineDelta = FarmStateSerializer.ComputeOfflineSeconds(
            saveData.lastSavedUtcTicks,
            saveData.lastSavedUtc);

        foreach (FarmPlantRecord record in saveData.plants)
        {
            if (record == null || string.IsNullOrEmpty(record.plantId))
                continue;

            if (!seedCatalog.TryGetPlantDefinitionById(record.plantId, out PlantDefinition definition))
                continue;

            if (!seedCatalog.TryGetPlantPrefab(definition, out GameObject prefab))
                continue;

            Vector2Int anchor = new(record.anchorX, record.anchorY);
            PlantSeedAtInternal(anchor, definition, prefab, saveAfterPlacement: false);

            GameObject plantObj = gridManager.GetPlantAt(anchor);
            if (plantObj == null || !plantObj.TryGetComponent(out PlantGrow grow))
                continue;

            grow.SetStageWithElapsed(record.currentStage, record.stageElapsedSeconds);
            grow.AdvanceBySeconds(offlineDelta);
        }
    }

    /// <summary>
    /// Applique la croissance écoulée depuis la dernière sauvegarde UTC (retour en ferme).
    /// </summary>
    private void ApplyOfflineGrowthSinceLastSave()
    {
        if (!enablePrototypePersistence || visualizer == null || visualizer.PlantsContainer == null)
            return;

        if (!FarmSaveService.TryLoad(out FarmSaveService.FarmSaveData saveData))
            return;

        float offlineDelta = FarmStateSerializer.ComputeOfflineSeconds(
            saveData.lastSavedUtcTicks,
            saveData.lastSavedUtc);
        if (offlineDelta <= 0f)
            return;

        foreach (Transform child in visualizer.PlantsContainer)
        {
            if (child.TryGetComponent(out PlantGrow grow))
                grow.AdvanceBySeconds(offlineDelta);
        }

        SaveFarmState();
        Debug.Log($"[BiofiltreManager] Croissance hors ligne : +{offlineDelta:F0}s appliquée.");
    }

    /// <summary>
    /// Sauvegarde immédiate après pose, récolte ou arrachage (même chemin JSON, sans inventaire).
    /// </summary>
    private void SaveFarmState()
    {
        if (!enablePrototypePersistence || visualizer == null || visualizer.PlantsContainer == null)
            return;

        FarmSaveService.Save(FarmStateSerializer.BuildRecords(visualizer.PlantsContainer));
    }

    // ── Popup pipeline ────────────────────────────────────────────────────────

    private void RegisterFarmPopupBindingsIfPossible()
    {
        ScreenPopupHost host = ResolveFarmPopupHost();
        if (host == null)
            return;

        if (UIManager.Instance == null)
        {
            Debug.LogWarning(
                "[BiofiltreManager] UIManager absent — impossible d'appliquer les bindings popups ferme.",
                this);
            return;
        }

        UIManager.Instance.ApplyRuntimePopupBindingsToHost(ScreenId.FirstLvlFarm, host);
        EnsureFarmPopupRoot(host);
        host.ConfigureDefaultPopupRoot(farmPopupRoot);
    }

    private void WarmUpSeedSelectionPopup()
    {
        if (!TryResolveSeedSelectionUI(out SeedSelectionUI seedUi))
        {
            Debug.LogWarning(
                "[BiofiltreManager] Impossible de précharger SeedSelectionUI. " +
                $"Vérifiez binding ({ScreenId.FirstLvlFarm}, {PopupId.FarmSeedSelection}).",
                this);
            return;
        }

        seedUi.gameObject.SetActive(false);
    }

    private void WarmUpHarvestPanelPopup()
    {
        if (!TryResolveHarvestPanelUI(out HarvestPanelUI panel))
        {
            Debug.LogWarning(
                "[BiofiltreManager] Impossible de précharger HarvestPanelUI. " +
                $"Vérifiez binding ({ScreenId.FirstLvlFarm}, {PopupId.FarmPlantHarvest}).",
                this);
            return;
        }

        panel.gameObject.SetActive(false);
    }

    private bool TryOpenFarmSeedSelection(BiofiltreCell cell)
    {
        ScreenPopupHost host = ResolveFarmPopupHost();
        if (host == null)
            return false;

        if (!host.TryShowPopup(PopupId.FarmSeedSelection, out SeedSelectionUI ui))
        {
            Debug.LogWarning(
                "[BiofiltreManager] Popup graines introuvable. Vérifiez UIManager.runtimePopupBindings " +
                $"(screenId={ScreenId.FirstLvlFarm}, popupId={PopupId.FarmSeedSelection}).",
                this);
            return false;
        }

        ConfigureSeedSelectionInstance(ui);
        ui.Open(cell, this);
        return true;
    }

    private bool TryResolveSeedSelectionUI(out SeedSelectionUI ui)
    {
        if (cachedSeedSelectionUi != null)
        {
            ui = cachedSeedSelectionUi;
            return true;
        }

        ScreenPopupHost host = ResolveFarmPopupHost();
        if (host == null || !host.TryGetPopup(PopupId.FarmSeedSelection, out ui))
        {
            ui = null;
            return false;
        }

        ConfigureSeedSelectionInstance(ui);
        return true;
    }

    private void ConfigureSeedSelectionInstance(SeedSelectionUI ui)
    {
        if (ui == null)
            return;

        ui.InjectPlacementPreview(placementPreview);
        ui.InjectPlayerInventory(PlayerInventory.Instance);
        cachedSeedSelectionUi = ui;
    }

    private bool TryResolveHarvestPanelUI(out HarvestPanelUI ui)
    {
        if (cachedHarvestPanelUi != null)
        {
            ui = cachedHarvestPanelUi;
            return true;
        }

        ScreenPopupHost host = ResolveFarmPopupHost();
        if (host == null || !host.TryGetPopup(PopupId.FarmPlantHarvest, out ui))
        {
            ui = null;
            return false;
        }

        ConfigureHarvestPanelInstance(ui);
        return true;
    }

    private void ConfigureHarvestPanelInstance(HarvestPanelUI ui)
    {
        if (ui == null)
            return;

        ScreenPopupHost host = ResolveFarmPopupHost();
        if (host != null)
            ui.InjectFarmPopupHost(host);

        cachedHarvestPanelUi = ui;
    }

    private ScreenPopupHost ResolveFarmPopupHost()
    {
        if (farmPopupHost != null)
            return farmPopupHost;

        Debug.LogWarning(
            "[BiofiltreManager] farmPopupHost non assigné. " +
            "Liez le ScreenPopupHost du LevelController (FirstLvl) sur BiofiltreManager.",
            this);
        return null;
    }

    private void EnsureFarmPopupRoot(ScreenPopupHost host)
    {
        if (farmPopupRoot != null || host == null)
            return;

        farmPopupRoot = FarmPopupCanvasFactory.CreateOrFind(host.transform);
    }
}
