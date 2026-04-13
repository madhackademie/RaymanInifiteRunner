# État de référence — scripts & assets (audit 2026-04-12)

Document **ponctuel** : photographie du dépôt pour onboarding et agents. À **mettre à jour** après refactors importants.

---

## Arborescence `Assets/Scripts/`

| Dossier | Rôle principal |
|---------|----------------|
| `Core/` | `Timer.cs` — minuteur Countdown / Stopwatch, `UnityEvent` tick / completed. |
| `Data/` | `PlantDefinition`, `GridConfig`, `GridData` — données grille et plantes (SO + runtime). |
| `Farm/` | Biofiltre, grille, placement, croissance, récolte (`BiofiltreManager`, `GridManager`, `PlantGrow`, `PlantHarvestInteractor`, …). |
| `Inventory/` | `PlayerInventory`, `ItemDefinition`, `ItemDatabase`, `InventorySlot`, `InventoryResult`. |
| `UI/` | Menu, graines, preview ; sous-dossier `UI/Inventory/` pour inventaire + récolte. |

**Liste des classes publiques** (fichiers `.cs` sous `Assets/Scripts/`) :

- `Core` : `Timer`
- `Data` : `GridConfig`, `GridData`, `PlantDefinition` (+ `PlantGrowthPattern` dans le même fichier)
- `Farm` : `BiofiltreCell`, `BiofiltreGridVisualizer`, `BiofiltreManager`, `GridLinesRenderer`, `GridManager`, `PlantDefinitionHolder`, `PlantGrow`, `PlantHarvestInteractor`, `PlantPlacementPreview`
- `Inventory` : `InventoryResult`, `InventorySlot`, `ItemDatabase`, `ItemDefinition`, `PlayerInventory`
- `UI` : `MainMenuUI`, `SeedSelectionUI` (+ `SeedEntry`), `SeedSlotUI`
- `UI/Inventory` : `HarvestPanelUI`, `InventoryFeedbackUI`, `InventorySlotUI`, `InventoryUI`

---

## Flux gameplay documentés (code actuel)

### Menu → niveau
- `MainMenuUI` sur la scène menu (`SampleScene` sous `Assets/Scenes/` ou doublon racine — voir Build Settings).
- Chargement de `FirstLvl` au clic Start (historique dans `PROJECT_LOG.md`).

### Plantation (cellule libre)
1. `BiofiltreCell` (clic) → événement `BiofiltreGridVisualizer.OnCellClicked`.
2. `BiofiltreManager.HandleCellClicked` — si preview active, sortie anticipée.
3. Si `GridManager.IsCellFree` → `SeedSelectionUI.Open(cell, manager)`.
4. Choix graine / preview → `BiofiltreManager.PlantSeedAt` : instanciation prefab, `PlantGrow.SetStage(Graine)`, `PlantDefinitionHolder.Initialise`, `PlantHarvestInteractor.Initialise` + `InjectHarvestPanel` + `InjectInventory`, occupation grille + `RegisterPlant`, visuels cellules.

### Récolte / info plante (cellule occupée)
1. `BiofiltreManager.HandleCellClicked` — cellule occupée → **`TryOpenPlantPopup(coords)`**.
2. `gridManager.GetPlantAt(coords)` → `HarvestPanelUI.Open(interactor, plantGrow, definition)`.
3. Panel : tout stade visible ; bouton **Récolter** si **Mature** ou **Seedling** ; **Arracher** → `PlantHarvestInteractor.Uproot()`.
4. Confirmation récolte : `PlantHarvestInteractor.ConfirmHarvest()` → `ApplyHarvest` → `PlayerInventory.TryAdd`.

### Clic direct sur la plante (scène)
- `PlantHarvestInteractor` implémente **`IPointerClickHandler`** : `OnPointerClick` appelle **`ConfirmHarvest()`** sans ouvrir le panel (récolte immédiate si `IsHarvestable()`). Nécessite **EventSystem** + **Physics2DRaycaster** sur la caméra.

### Code présent mais non branché au clic grille
- `BiofiltreManager.TryOpenHarvestPanel` et `FindInteractorAt` : **non appelés** par `HandleCellClicked` à la date de l’audit. Voir `PROJECT_LOG.md` (2026-04-12).

---

## Inventaire (résumé API)

- **`PlayerInventory`** : slots en liste, `TryAdd(ItemDefinition, int)`, `TryRemove`, `Count`, `HasSpaceFor`, événement `OnInventoryChanged` ; accès singleton **`Instance`** (initialisation dans `Awake`). Les consommateurs (ex. **`BiofiltreManager`**) peuvent résoudre la référence sans `[SerializeField]` — voir ordre d’exécution : `Notes/Farm/PlayerInventory_Instance_et_ordre_Awake.md`.
- **`InventoryResult`** : `Success`, `Partial`, `Full`, `InvalidItem`.
- **`ItemDatabase`** (SO) : `GetById(string)`.
- **UI** : `InventoryUI` / `InventorySlotUI`, `InventoryFeedbackUI` (ex. inventaire plein).

---

## Données plante (`PlantDefinition`)

- Identité, **GrowthPattern** (`Leafy` / `Fruiting`), **HarvestStage** (souvent `Mature`).
- Harvest : `harvestItemId`, min/max quantité, `maxHarvestCount`.
- Sprites par stade, **footprint** `Vector2Int[]` avec ancre `(0,0)`, `spriteWorldOffset`.
- Durées par stade (`StageDuration[]`, `GetDuration`).

`PlantGrow` applique l’ordre des stades selon le pattern (feuille vs fruit).

---

## Prefabs & scènes utiles (hors packages)

| Chemin | Usage probable |
|--------|------------------|
| `Assets/Prefabs/World/Biofiltre.prefab` | Zone culture + `BiofiltreManager` / visualizer / grille. |
| `Assets/Prefabs/World/Plantes/LaitueObj.prefab` | Instance plante (`PlantGrow`, collider, interactor, holder). |
| `Assets/Prefabs/Ui/InventoryPanel.prefab` | UI inventaire. |
| `Assets/Prefabs/Ui/InventorySlotUI.prefab` | Slot ligne inventaire. |
| `Assets/Prefabs/Ui/SeedSlotUI.prefab` | Ligne liste graines. |
| `Assets/Scenes/FirstLvl.unity` | Niveau ferme / biofiltre (câblage à valider en jeu). |
| `Assets/Scenes/SampleScene.unity` | Menu / tests (cf. Build Settings). |

---

## Points d’attention (design / dette)

1. **Deux stades récoltables** (`Mature`, `Seedling`) mais **un seul** `harvestItemId` dans l’asset — les graines fin de cycle partagent l’item feuille/fruit tant qu’on n’étend pas les données.
2. **`InventoryResult.Partial`** : la plante est retirée comme en **succès complet** (`OnHarvestSuccess`) — à trancher côté gameplay (perte vs conservation sur pied).
3. **`maxHarvestCount`** : présent sur `PlantDefinition`, pas exploité pour des récoltes répétées **sans** destruction dans `PlantHarvestInteractor`.
4. **`HarvestPanelUI.Update`** : rafraîchissement continu du timer tant que le panel est ouvert — acceptable prototype ; à revoir si plusieurs panneaux ou perf mobile.

---

## Notes liées (projet)

- Flux détaillé et diagrammes : `Notes/Farm/SYSTEMES_carte_mentale.md`
- Hub tâches : `Notes/Todo_project.md`
- Pipeline plantation : `Notes/Farm/TODO_plantation_pipeline.md`
- Ajout d’une plante : `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md`
- Journal chronologique : `PROJECT_LOG.md` (racine du repo)
