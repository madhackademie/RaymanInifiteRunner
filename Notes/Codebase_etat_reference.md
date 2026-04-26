# État de référence — scripts & assets (audit 2026-04-12, navigation 2026-04-19, alignement 2026-04-21, update 2026-04-26)

Document **ponctuel** : photographie du dépôt pour onboarding et agents. À **mettre à jour** après refactors importants.

---

## Arborescence `Assets/Scripts/`

| Dossier | Rôle principal |
|---------|----------------|
| `Core/` | `Timer.cs` ; **`GameBootstrap.cs`** — entrée `Bootstrap.unity`, charge additif **`NavigationHUD`** puis **`HomeScene`** ; `LoadingScreen` + **`SceneNavigator.SetInitialScene(HomeScene)`**. |
| `Systems/` | **`UIManager`** (prefabs sous shell, `ShowScreen` / `HideScreen`) ; **`SceneNavigator`** (`ShowScene` = `SetActive` sur racines de scène, lazy-load additif optionnel pour scènes listées) ; **`ScreenId`** / **`SceneId`**. |
| `Data/` | `PlantDefinition`, `GridConfig`, `GridData` — données grille et plantes (SO + runtime). |
| `Farm/` | Biofiltre, grille, placement, croissance, récolte (`BiofiltreManager`, `GridManager`, `PlantGrow`, `PlantHarvestInteractor`, …) + persistance JSON prototype (`FarmSaveService`, `PlantPersistenceMarker`). |
| `Inventory/` | `PlayerInventory`, `ItemDefinition`, `ItemDatabase`, `InventorySlot`, `InventoryResult`. |
| `UI/` | Menu, graines, preview ; sous-dossier `UI/Inventory/` pour inventaire + récolte. |

**Liste des classes publiques** (fichiers `.cs` sous `Assets/Scripts/`) :

- `Core` : `Timer`, `GameBootstrap`
- `Systems` : `UIManager`, `ScreenEntry` (même fichier que `UIManager`), `ScreenId`, `SceneId`, `SceneNavigator`
- `Data` : `GridConfig`, `GridData`, `PlantDefinition` (+ `PlantGrowthPattern` dans le même fichier)
- `Farm` : `BiofiltreCell`, `BiofiltreGridVisualizer`, `BiofiltreManager`, `GridLinesRenderer`, `GridManager`, `PlantDefinitionHolder`, `PlantGrow`, `PlantHarvestInteractor`, `PlantPlacementPreview`
- `Inventory` : `InventoryResult`, `InventorySlot`, `ItemDatabase`, `ItemDefinition`, `PlayerInventory`
- `UI` : `MainMenuUI`, `SeedSelectionUI` (+ `SeedEntry`), `SeedSlotUI`, `NavigationHUD`, `LoadingScreen`, `FirstLvlController`, `MapSceneController` (dossier `UI/Map/`), `MapNodeButton`, …
- `UI/Inventory` : `HarvestPanelUI`, `InventoryFeedbackUI`, `InventorySlotUI`, `InventoryUI`, `InventorySceneController`

---

## Flux gameplay documentés (code actuel)

### Menu → niveau (navigation contenu par visibilité)
- **Boot** : **`Bootstrap.unity`** → `GameBootstrap` charge additivement **`NavigationHUD`** puis **`HomeScene`** ; HUD shell masqué pendant le chargement ; **`SceneNavigator.SetInitialScene(HomeScene)`** — voir **Build Settings**.
- **Changement de « scène » visible** : **`SceneNavigator.ShowScene(nom)`** — désactive les racines de la scène de contenu courante, active celles de la cible ; si la cible est **lazy** (liste Inspector sur `SceneNavigator`), chargement additif **une fois** puis masquage jusqu’à affichage.
- Depuis **`HomeScene`**, **`MapSceneController`** appelle **`ShowScene(targetSceneName)`** (ex. **`FirstLvl`**) ; retour : **`FirstLvlController`** sur **`OnExitToHomeRequested`** → **`ShowScene(HomeScene)`** + **`NavigationHUD.ShowNavBar()`**.
- **Onglets HUD** : **`NavigationHUD`** → **`ShowScene(HomeScene)`** ; l’inventaire runtime passe par **`UIManager.ShowScreen(ScreenId.Inventory)`** (plus de dépendance scène `Inventaire`).
- **`LoadingScreen`** : barre + pourcentage + fade ; art — **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**.
- **`MainMenuUI`** / `SampleScene` : flux legacy possible — à réaligner si tout passe par **`Bootstrap`**.

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
| `Assets/Scenes/FirstLvl.unity` | Niveau ferme / biofiltre. |
| `Assets/Scenes/HomeScene.unity` | Hub d’accueil / **`MapSceneController`**. |
| `Assets/Scenes/SampleScene.unity` | Menu / tests (cf. Build Settings). |

---

## Points d’attention (design / dette)

1. **Deux stades récoltables** (`Mature`, `Seedling`) mais **un seul** `harvestItemId` dans l’asset — les graines fin de cycle partagent l’item feuille/fruit tant qu’on n’étend pas les données.
2. **`InventoryResult.Partial`** : la plante est retirée comme en **succès complet** (`OnHarvestSuccess`) — à trancher côté gameplay (perte vs conservation sur pied).
3. **`maxHarvestCount`** : présent sur `PlantDefinition`, pas exploité pour des récoltes répétées **sans** destruction dans `PlantHarvestInteractor`.
4. **`HarvestPanelUI.Update`** : rafraîchissement continu du timer tant que le panel est ouvert — acceptable prototype ; à revoir si plusieurs panneaux ou perf mobile.
5. **Doc historique** : plusieurs notes (`ARCHI_hud_ui_manager_additive.md`, `Todo_ui.md`, entrées **`PROJECT_LOG`**) décrivent encore **`GoTo` + `UnloadSceneAsync`** par transition — le code actuel privilégie **`ShowScene` + `SetActive` sur racines** ; à harmoniser lors de l’audit Bezi / refactor (voir **`Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`**).

---

## Notes liées (projet)

- Flux détaillé et diagrammes : `Notes/Farm/SYSTEMES_carte_mentale.md`
- Hub tâches : `Notes/Todo_project.md`
- Pipeline plantation : `Notes/Farm/TODO_plantation_pipeline.md`
- Ajout d’une plante : `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md`
- Journal chronologique : `PROJECT_LOG.md` (racine du repo)
