using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// Bridges the biofiltre grid and the planting UI.
/// Listens to cell clicks from <see cref="BiofiltreGridVisualizer"/>,
/// opens the seed selection panel for empty cells, and handles plant placement.
/// </summary>
[RequireComponent(typeof(BiofiltreGridVisualizer))]
[RequireComponent(typeof(GridManager))]
public class BiofiltreManager : MonoBehaviour
{
    private const int FarmPopupCanvasSortOrder = 10;

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

    private BiofiltreGridVisualizer visualizer;
    private GridManager gridManager;
    private bool hasLoadedFromSave;
    private SeedSelectionUI cachedSeedSelectionUi;
    private bool suppressGridCellUiThisFrame;

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
    }

    private void OnDisable()
    {
        visualizer.OnCellClicked -= HandleCellClicked;
    }

    private void Start()
    {
        visualizer.GenerateGrid();

        RegisterFarmPopupBindingsIfPossible();
        WarmUpSeedSelectionPopup();

        TryLoadFarmState();
    }

    private void OnApplicationQuit()
    {
        SaveFarmState();
    }

    private void LateUpdate()
    {
        suppressGridCellUiThisFrame = false;
    }

    // ── Cell click ────────────────────────────────────────────────────────────

    /// <summary>
    /// Appelé par <see cref="PlantPlacementPreview"/> quand un clic souris est consommé
    /// (placement, annulation) pour éviter que le même clic rouvre le popup graines.
    /// </summary>
    public void SuppressGridCellUiThisFrame()
    {
        suppressGridCellUiThisFrame = true;
    }

    /// <summary>True tant que le fantôme de placement suit la souris.</summary>
    public bool IsPlantPlacementPreviewActive =>
        placementPreview != null && placementPreview.enabled;

    private bool ShouldBlockGridCellUi =>
        suppressGridCellUiThisFrame || IsPlantPlacementPreviewActive;

    private void HandleCellClicked(BiofiltreCell cell)
    {
        if (ShouldBlockGridCellUi)
        {
            HideFarmSeedSelectionPopup();
            return;
        }

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

        panel.Open(interactor, plantGrow, definition);
        return true;
    }

    // ── Footprint query (called by SeedSelectionUI) ───────────────────────────

    public bool CanPlace(Vector2Int anchor, PlantDefinition plantDefinition)
    {
        if (plantDefinition == null)
            return false;

        return gridManager.AreAllCellsFree(plantDefinition.GetOccupiedCells(anchor));
    }

    // ── Plant placement ───────────────────────────────────────────────────────

    public void PlantSeed(BiofiltreCell cell, PlantDefinition plantDefinition, GameObject plantPrefab)
    {
        PlantSeedAt(cell.GridCoordinates, plantDefinition, plantPrefab);
    }

    public void PlantSeedAt(Vector2Int anchor, PlantDefinition plantDefinition, GameObject plantPrefab)
    {
        PlantSeedAt(anchor, plantDefinition, plantPrefab, saveAfterPlacement: true);
    }

    private void PlantSeedAt(Vector2Int anchor, PlantDefinition plantDefinition, GameObject plantPrefab, bool saveAfterPlacement)
    {
        if (plantDefinition == null || plantPrefab == null)
        {
            Debug.LogWarning("[BiofiltreManager] PlantSeedAt called with null definition or prefab.", this);
            return;
        }

        foreach (Vector2Int occupied in plantDefinition.GetOccupiedCells(anchor))
        {
            if (!gridManager.IsCellFree(occupied))
            {
                Debug.Log($"[BiofiltreManager] Cannot plant — cell {occupied} is occupied.");
                return;
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

        if (saveAfterPlacement)
            SaveFarmState();
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

        DateTime nowUtc = DateTime.UtcNow;
        DateTime savedUtc = nowUtc;
        if (!string.IsNullOrEmpty(saveData.lastSavedUtc) &&
            DateTime.TryParse(saveData.lastSavedUtc, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsedUtc))
        {
            savedUtc = parsedUtc;
        }

        float offlineDelta = Mathf.Max(0f, (float)(nowUtc - savedUtc).TotalSeconds);

        foreach (FarmPlantRecord record in saveData.plants)
        {
            if (record == null || string.IsNullOrEmpty(record.plantId))
                continue;

            if (!seedCatalog.TryGetPlantDefinitionById(record.plantId, out PlantDefinition definition))
                continue;

            if (!seedCatalog.TryGetPlantPrefab(definition, out GameObject prefab))
                continue;

            Vector2Int anchor = new(record.anchorX, record.anchorY);
            PlantSeedAt(anchor, definition, prefab, saveAfterPlacement: false);

            GameObject plantObj = gridManager.GetPlantAt(anchor);
            if (plantObj == null || !plantObj.TryGetComponent(out PlantGrow grow))
                continue;

            grow.SetStageWithElapsed(record.currentStage, record.stageElapsedSeconds);
            grow.AdvanceBySeconds(offlineDelta);
        }
    }

    private void SaveFarmState()
    {
        if (!enablePrototypePersistence || visualizer == null || visualizer.PlantsContainer == null)
            return;

        List<FarmPlantRecord> records = new();

        foreach (Transform child in visualizer.PlantsContainer)
        {
            if (!child.TryGetComponent(out PlantPersistenceMarker marker))
                continue;

            if (!child.TryGetComponent(out PlantGrow grow))
                continue;

            if (string.IsNullOrEmpty(marker.PlantId))
                continue;

            records.Add(new FarmPlantRecord
            {
                plantId = marker.PlantId,
                anchorX = marker.Anchor.x,
                anchorY = marker.Anchor.y,
                currentStage = grow.CurrentStage,
                stageElapsedSeconds = grow.CurrentStageElapsedSeconds
            });
        }

        FarmSaveService.Save(records);
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
        cachedSeedSelectionUi = ui;
    }

    private ScreenPopupHost ResolveFarmPopupHost()
    {
        if (farmPopupHost != null)
            return farmPopupHost;

        farmPopupHost = FindFirstObjectByType<ScreenPopupHost>();
        if (farmPopupHost == null)
        {
            Debug.LogWarning(
                "[BiofiltreManager] ScreenPopupHost introuvable. Placez un ScreenPopupHost (ex. sur LevelController).",
                this);
        }

        return farmPopupHost;
    }

    private void EnsureFarmPopupRoot(ScreenPopupHost host)
    {
        if (farmPopupRoot != null || host == null)
            return;

        Transform existing = host.transform.Find("FarmPopupRoot");
        if (existing != null)
        {
            farmPopupRoot = existing as RectTransform;
            return;
        }

        var canvasGo = new GameObject("FarmUICanvas");
        canvasGo.transform.SetParent(host.transform, false);

        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = FarmPopupCanvasSortOrder;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800f, 600f);

        canvasGo.AddComponent<GraphicRaycaster>();

        var rootGo = new GameObject("FarmPopupRoot", typeof(RectTransform));
        RectTransform rect = rootGo.GetComponent<RectTransform>();
        rect.SetParent(canvasGo.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        farmPopupRoot = rect;
    }
}
