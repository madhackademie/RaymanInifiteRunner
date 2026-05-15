using UnityEngine;
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
    [Header("UI")]
    [Tooltip("Instance scène SeedSelectionUI (catalogue graines + option live pour le pipeline popup).")]
    [SerializeField] private SeedSelectionUI seedSelectionUI;

    [Tooltip("Hôte popups ferme (ScreenPopupHost, ex. sur LevelController dans FirstLvl).")]
    [SerializeField] private ScreenPopupHost farmPopupHost;

    [Tooltip("Panel de récolte ouvert quand le joueur clique sur une plante mature.")]
    [SerializeField] private HarvestPanelUI harvestPanelUI;

    [Header("Harvest")]
    [Tooltip("Base de données d'items pour résoudre les récoltes. Injectée dans chaque PlantHarvestInteractor.")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("Persistence Prototype")]
    [Tooltip("Active la sauvegarde JSON locale des plantes posees sur la grille.")]
    [SerializeField] private bool enablePrototypePersistence = true;

    private BiofiltreGridVisualizer visualizer;
    private GridManager gridManager;
    private bool hasLoadedFromSave;

    private void Awake()
    {
        visualizer  = GetComponent<BiofiltreGridVisualizer>();
        gridManager = GetComponent<GridManager>();
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
        // S'assure que les cellules existent avant restauration.
        visualizer.GenerateGrid();

        RegisterFarmPopupBindingsIfPossible();
        TryParentHarvestPanelUnderSeedRoot();

        // Recharge la ferme depuis JSON si disponible.
        TryLoadFarmState();
    }

    private void OnApplicationQuit()
    {
        // Filet de securite de fin de session.
        SaveFarmState();
    }

    // ── Cell click ────────────────────────────────────────────────────────────

    private void HandleCellClicked(BiofiltreCell cell)
    {
        // A placement preview is already running — let it handle all clicks.
        if (seedSelectionUI != null && seedSelectionUI.IsPreviewActive)
            return;

        if (gridManager.IsCellFree(cell.GridCoordinates))
        {
            // Cellule libre → sélection de graine (pipeline générique)
            if (!TryOpenFarmSeedSelection(cell))
                return;
        }
        else
        {
            // Cellule occupée → ouvrir le popup d'info plante
            TryOpenPlantPopup(cell.GridCoordinates);
        }
    }

    /// <summary>
    /// Ouvre le popup d'info pour la plante occupant la cellule cliquée.
    /// Lookup O(1) via le registre de GridManager — aucune recherche spatiale.
    /// </summary>
    private void TryOpenPlantPopup(Vector2Int coords)
    {
        GameObject plantObj = gridManager.GetPlantAt(coords);

        if (plantObj == null)
        {
            Debug.Log($"[BiofiltreManager] Aucune plante enregistrée à la cellule {coords}.");
            return;
        }

        if (harvestPanelUI == null)
        {
            Debug.LogWarning("[BiofiltreManager] HarvestPanelUI non assigné.", this);
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

        harvestPanelUI.Open(interactor, plantGrow, holder != null ? holder.Definition : null);
    }

    /// <summary>
    /// Variante historique : recherche spatiale d'un <see cref="PlantHarvestInteractor"/> puis <see cref="PlantHarvestInteractor.TryHarvest"/>.
    /// Le flux actuel du clic grille passe par <see cref="TryOpenPlantPopup"/> (lookup par coordonnées via la grille).
    /// Conserver uniquement si un scénario futur réutilise cette recherche ; sinon candidat à suppression lors du nettoyage.
    /// </summary>
    private void TryOpenHarvestPanel(Vector2Int coords)
    {
        // On cherche la plante dans le container des plantes par position de cellule
        Vector2 worldCenter = gridManager.GridToWorldCenter(coords);

        PlantHarvestInteractor interactor = FindInteractorAt(worldCenter);

        if (interactor == null)
        {
            Debug.Log($"[BiofiltreManager] Aucun PlantHarvestInteractor trouvé à la cellule {coords}.");
            return;
        }

        interactor.TryHarvest();
    }

    /// <summary>
    /// Utilisé uniquement par <see cref="TryOpenHarvestPanel"/> (non branché sur le clic grille actuel).
    /// </summary>
    private PlantHarvestInteractor FindInteractorAt(Vector2 worldCenter)
    {
        const float SearchRadius = 0.1f;

        PlantHarvestInteractor closest  = null;
        float                  minDist  = float.MaxValue;

        foreach (Transform child in visualizer.PlantsContainer)
        {
            float dist = Vector2.Distance(child.position, worldCenter);

            if (dist < SearchRadius && dist < minDist)
            {
                if (child.TryGetComponent(out PlantHarvestInteractor interactor))
                {
                    closest = interactor;
                    minDist = dist;
                }
            }
        }

        // Fallback : on cherche par footprint (plantes multi-cellules)
        if (closest == null)
        {
            foreach (Transform child in visualizer.PlantsContainer)
            {
                if (!child.TryGetComponent(out PlantHarvestInteractor interactor))
                    continue;

                if (!child.TryGetComponent(out PlantDefinitionHolder holder) || holder.Definition == null)
                    continue;

                Vector2Int anchor = gridManager.WorldToGrid(child.position);
                foreach (Vector2Int cell in holder.Definition.GetOccupiedCells(anchor))
                {
                    if (cell == gridManager.WorldToGrid(worldCenter))
                    {
                        closest = interactor;
                        break;
                    }
                }

                if (closest != null)
                    break;
            }
        }

        return closest;
    }

    // ── Footprint query (called by SeedSelectionUI) ───────────────────────────

    /// <summary>
    /// Returns true if every cell of the plant's footprint is free at the given anchor.
    /// Used by the UI to enable or disable seed slots before the player selects one.
    /// </summary>
    public bool CanPlace(Vector2Int anchor, PlantDefinition plantDefinition)
    {
        if (plantDefinition == null) return false;
        return gridManager.AreAllCellsFree(plantDefinition.GetOccupiedCells(anchor));
    }

    // ── Plant placement ───────────────────────────────────────────────────────

    /// <summary>
    /// Plants the given definition on the target cell.
    /// Called by <see cref="SeedSelectionUI"/> after the player selects a seed (legacy direct path).
    /// </summary>
    public void PlantSeed(BiofiltreCell cell, PlantDefinition plantDefinition, GameObject plantPrefab)
    {
        PlantSeedAt(cell.GridCoordinates, plantDefinition, plantPrefab);
    }

    /// <summary>
    /// Plants the given definition at the specified grid anchor.
    /// Called by <see cref="PlantPlacementPreview"/> after the player confirms placement.
    /// </summary>
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

        // Verify the footprint is still free (multi-cell plants)
        foreach (Vector2Int occupied in plantDefinition.GetOccupiedCells(anchor))
        {
            if (!gridManager.IsCellFree(occupied))
            {
                Debug.Log($"[BiofiltreManager] Cannot plant — cell {occupied} is occupied.");
                return;
            }
        }

        // Instantiate under Plants container
        Vector2 worldCenter   = gridManager.GridToWorldCenter(anchor);
        Vector2 spawnPosition = worldCenter + plantDefinition.spriteWorldOffset;
        GameObject instance   = Instantiate(
            plantPrefab,
            spawnPosition,
            Quaternion.identity,
            visualizer.PlantsContainer
        );
        instance.name = $"{plantDefinition.displayName}_{anchor}";

        // Initialize PlantGrow to Graine stage
        if (instance.TryGetComponent(out PlantGrow plantGrow))
            plantGrow.SetStage(PlantGrow.GrowthStage.Graine);

        // Provide PlantDefinition to optional harvest interactor
        if (instance.TryGetComponent(out PlantDefinitionHolder holder))
            holder.Initialise(plantDefinition);

        if (instance.TryGetComponent(out PlantPersistenceMarker marker))
            marker.Initialise(plantDefinition.plantId, anchor);
        else
            instance.AddComponent<PlantPersistenceMarker>().Initialise(plantDefinition.plantId, anchor);

        // Fournir le contexte grille et le panel de récolte à l'interacteur
        if (instance.TryGetComponent(out PlantHarvestInteractor harvestInteractor))
        {
            Vector2Int[] cells = System.Linq.Enumerable.ToArray(plantDefinition.GetOccupiedCells(anchor));
            harvestInteractor.Initialise(gridManager, visualizer, cells);
            harvestInteractor.InjectHarvestPanel(harvestPanelUI);
            harvestInteractor.InjectInventory(itemDatabase);
            harvestInteractor.SetOnPlantRemoved(SaveFarmState);
        }

        // Mark cells occupied in GridData + plant registry
        Vector2Int[] footprintCells = System.Linq.Enumerable.ToArray(plantDefinition.GetOccupiedCells(anchor));
        gridManager.OccupyCells(footprintCells);
        gridManager.RegisterPlant(footprintCells, instance);

        // Update visual states of affected cells
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

        // Nettoyage complet puis reconstruction depuis le JSON.
        // Important: on repart d'un etat vide pour eviter des doublons runtime.
        foreach (Transform child in visualizer.PlantsContainer)
            Destroy(child.gameObject);

        gridManager.ResetRuntimeState();
        visualizer.RefreshAllCellStates();

        DateTime nowUtc = DateTime.UtcNow;
        DateTime savedUtc = nowUtc;
        if (!string.IsNullOrEmpty(saveData.lastSavedUtc) && DateTime.TryParse(saveData.lastSavedUtc, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsedUtc))
            savedUtc = parsedUtc;

        float offlineDelta = Mathf.Max(0f, (float)(nowUtc - savedUtc).TotalSeconds);

        foreach (FarmPlantRecord record in saveData.plants)
        {
            if (record == null || string.IsNullOrEmpty(record.plantId))
                continue;

            PlantDefinition definition = ResolvePlantDefinition(record.plantId);
            if (definition == null)
                continue;

            if (!seedSelectionUI.TryGetPlantPrefab(definition, out GameObject prefab))
                continue;

            Vector2Int anchor = new(record.anchorX, record.anchorY);
            // Re-instancie la plante comme une pose normale, mais sans resauvegarder
            // pendant la reconstruction.
            PlantSeedAt(anchor, definition, prefab, saveAfterPlacement: false);

            GameObject plantObj = gridManager.GetPlantAt(anchor);
            if (plantObj == null || !plantObj.TryGetComponent(out PlantGrow grow))
                continue;

            // Rejoue l'etat de croissance sauvegarde.
            grow.SetStageWithElapsed(record.currentStage, record.stageElapsedSeconds);
            // Puis applique la progression hors ligne depuis le dernier save UTC.
            grow.AdvanceBySeconds(offlineDelta);
        }
    }

    private void SaveFarmState()
    {
        if (!enablePrototypePersistence || visualizer == null || visualizer.PlantsContainer == null)
            return;

        // Snapshot runtime des plantes actuellement presentes.
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

    private void RegisterFarmPopupBindingsIfPossible()
    {
        if (farmPopupHost == null)
            farmPopupHost = FindFirstObjectByType<ScreenPopupHost>();

        if (UIManager.Instance == null)
        {
            Debug.LogWarning(
                "[BiofiltreManager] UIManager absent — impossible d'appliquer les bindings popups ferme.",
                this);
            return;
        }

        if (farmPopupHost == null)
        {
            Debug.LogWarning(
                "[BiofiltreManager] ScreenPopupHost introuvable — bindings popups ferme non appliqués.",
                this);
            return;
        }

        UIManager.Instance.ApplyRuntimePopupBindingsToHost(
            ScreenId.FirstLvlFarm,
            farmPopupHost,
            seedSelectionUI);
    }

    private void TryParentHarvestPanelUnderSeedRoot()
    {
        if (harvestPanelUI == null || seedSelectionUI == null)
            return;

        Transform seedRoot = seedSelectionUI.transform;
        if (harvestPanelUI.transform.parent == seedRoot)
            return;

        harvestPanelUI.transform.SetParent(seedRoot, false);
    }

    private bool TryOpenFarmSeedSelection(BiofiltreCell cell)
    {
        if (farmPopupHost == null)
            farmPopupHost = FindFirstObjectByType<ScreenPopupHost>();

        if (farmPopupHost == null)
        {
            Debug.LogWarning(
                "[BiofiltreManager] ScreenPopupHost introuvable. Placez un ScreenPopupHost (ex. sur LevelController).",
                this);
            return false;
        }

        if (!farmPopupHost.TryShowPopup(PopupId.FarmSeedSelection, out SeedSelectionUI ui))
        {
            Debug.LogWarning(
                "[BiofiltreManager] Popup graines introuvable. Vérifiez UIManager.runtimePopupBindings " +
                $"(screenId={ScreenId.FirstLvlFarm}, popupId={PopupId.FarmSeedSelection}).",
                this);
            return false;
        }

        ui.Open(cell, this);
        return true;
    }

    private PlantDefinition ResolvePlantDefinition(string plantId)
    {
        if (string.IsNullOrEmpty(plantId) || seedSelectionUI == null)
            return null;

        return seedSelectionUI.TryGetPlantDefinitionById(plantId, out PlantDefinition definition)
            ? definition
            : null;
    }
}
