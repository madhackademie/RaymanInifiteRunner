# Carte mentale — flux ferme ↔ inventaire

Document de **visualisation** des liaisons entre scripts existants (Unity 6). À mettre à jour quand de nouveaux orchestrateurs (ex. `GameManager`) apparaissent.

---

## Carte multi-niveaux (monde → cellule)

**Métaphore** : tu zoomes comme sur une carte — d’abord le **monde** (tous les domaines), puis l’**organisme** (le biofiltre et ses composants), puis un **organe** (un cycle : plantation, croissance, récolte), puis la **cellule** (les méthodes à ouvrir dans l’IDE). Chaque zoom est **un diagramme séparé** pour ne pas tout mélanger ; le schéma « Vue synthèse » plus bas reste la vue d’ensemble en un coup d’œil.

| Niveau | Tu te demandes… | Où aller |
|--------|-----------------|----------|
| **Monde** | Quels domaines et flux entre eux ? | Diagramme *Niveau 1* |
| **Organisme** | Qu’est-ce qui vit sur le même objet / la même zone ? | *Niveau 2 — Biofiltre* |
| **Organe** | Pour *cette* interaction joueur, qui appelle qui ? | Zoom A, B, C ou D |
| **Cellule** | Nom exact de la fonction | Étiquettes `()` dans les zooms |

Outils : [Mermaid Live](https://mermaid.live) pour exporter PNG/SVG ; ou copier un bloc dans Cursor avec extension Mermaid.

### Niveau 1 — Monde (domaines + ponts)

```mermaid
flowchart TB
  subgraph Monde["Monde — scène & persistance"]
    direction TB
    subgraph D["Données (assets)"]
      PD[PlantDefinition]
      CFG[GridConfig / données grille]
      IDB[ItemDatabase]
      ID[ItemDefinition]
    end
    subgraph BF["Biofiltre — interaction sol"]
      BFM[BiofiltreManager]
      BGV[BiofiltreGridVisualizer]
      GM[GridManager]
      BCell[BiofiltreCell]
    end
    subgraph UI_Farm["UI ferme"]
      SSU[SeedSelectionUI]
      PPP[PlantPlacementPreview]
      HUI[HarvestPanelUI]
    end
    subgraph Plante["Instance plante (prefab)"]
      PG[PlantGrow]
      PDH[PlantDefinitionHolder]
      PHI[PlantHarvestInteractor]
    end
    subgraph Inv["Inventaire joueur"]
      PI[PlayerInventory]
      IUI[InventoryUI…]
      IFB[InventoryFeedbackUI]
    end
  end

  CFG --> GM
  PD --> PDH
  PD --> PG
  IDB --> ID

  BGV --> GM
  BGV --> BCell
  BFM --> BGV
  BFM --> GM
  BFM --> SSU
  BFM --> HUI
  SSU --> PPP
  PPP --> BFM

  BFM -->|"instancie"| Plante
  PHI --> PG
  PHI --> PDH
  PHI --> IDB
  PHI --> PI
  PHI --> HUI
  PHI --> IFB
  PI --> IUI
```

### Niveau 2 — Organisme « un biofiltre » (composants sur le même GameObject)

```mermaid
flowchart LR
  subgraph GO["GameObject Biofiltre (typique)"]
    GM[GridManager]
    BGV[BiofiltreGridVisualizer]
    BFM[BiofiltreManager]
  end

  BFM --- BGV
  BFM --- GM
  BGV --- GM

  BGV -->|"Start →"| Gen[GenerateGrid]
  Gen --> Cells["Cell_x_y + BiofiltreCell"]
```

---

### Zoom A — Cycle « grille visible & cliquable »

```mermaid
flowchart TD
  Start([Start scène]) --> BGV_Start[BiofiltreGridVisualizer.Start]
  BGV_Start --> Gen[BiofiltreGridVisualizer.GenerateGrid]
  Gen --> Loop["Pour chaque case col, row"]
  Loop --> NewGO[new GameObject + SpriteRenderer + BoxCollider2D]
  NewGO --> Add[BiofiltreCell.Initialize coords]
  Add --> Sub[BiofiltreCell.OnCellClicked += HandleCellClicked]
  Sub --> VizHandler[BiofiltreGridVisualizer.HandleCellClicked]
  VizHandler --> Event[BiofiltreGridVisualizer.OnCellClicked.Invoke]

  Click([Joueur clique cellule]) --> IPC[BiofiltreCell.OnPointerClick]
  IPC --> Fire[BiofiltreCell.OnCellClicked]
  Fire --> VizHandler
```

### Zoom B — Cycle « plantation » (chaîne d’appels)

```mermaid
flowchart TD
  C([Clic cellule]) --> Mgr[BiofiltreManager.HandleCellClicked]
  Mgr --> Preview{SeedSelectionUI.IsPreviewActive ?}
  Preview -->|oui| Ignore[return — pas de re-route cellule]
  Preview -->|non| Free{GridManager.IsCellFree}
  Free -->|oui| Open[SeedSelectionUI.Open cell, manager]
  Open --> Build[SeedSelectionUI.BuildSlots]
  Build --> Can[BiofiltreManager.CanPlace anchor, PlantDefinition]
  Can --> GridFree[GridManager.AreAllCellsFree]

  Pick([Joueur choisit un slot]) --> HS[SeedSelectionUI.HandleSeedSelected]
  HS --> Close[SeedSelectionUI.Close]
  HS --> HasPPP{placementPreview != null ?}
  HasPPP -->|non| PS[BiofiltreManager.PlantSeed]
  PS --> PSA[PlantSeedAt]
  HasPPP -->|oui| Begin[PlantPlacementPreview.Begin]
  Begin --> Upd[PlantPlacementPreview.Update — souris]
  Upd --> Conf{clic gauche valide ?}
  Conf -->|oui| PSA
  PSA --> Inst[Instantiate prefab sous PlantsContainer]
  PSA --> Grow[PlantGrow.SetStage Graine]
  PSA --> Hold[PlantDefinitionHolder.Initialise]
  PSA --> HarvInit[PlantHarvestInteractor.Initialise + InjectHarvestPanel]
  PSA --> Occ[GridManager.OccupyCells]
  PSA --> Vis[BiofiltreCell.SetVisualState true par case]
```

### Zoom C — Cycle « croissance » (autonome sur la plante)

```mermaid
flowchart TD
  Aw([Après pose]) --> ST[PlantGrow.SetStage Graine — appelé par BiofiltreManager]
  ST --> Start[PlantGrow.Start]
  Start --> Set2[SetStage initialStage si besoin]

  Loop[Chaque frame] --> Up[PlantGrow.Update]
  Up --> Tim{stageTimer >= currentStageDuration ?}
  Tim -->|non| Loop
  Tim -->|oui| Adv[AdvanceToNextStage]
  Adv --> Spr[GetSpriteForStage + définition PlantDefinition]

  Note1[Données : PlantDefinition.GetDuration par stade, GrowthPattern Leafy/Fruiting]
```

### Zoom D — Cycle « récolte » (cellule occupée ou panel)

```mermaid
flowchart TD
  C([Clic cellule occupée]) --> Mgr[BiofiltreManager.HandleCellClicked]
  Mgr --> Pop[TryOpenPlantPopup coords]
  Pop --> Get[GridManager.GetPlantAt]
  Get --> Open[HarvestPanelUI.Open interactor plantGrow definition]

  Open --> UI[Panel: infos tout stade bouton Récolter si Mature ou Seedling]
  Btn([Bouton Récolter]) --> OH[HarvestPanelUI.OnHarvestClicked]
  OH --> CH[PlantHarvestInteractor.ConfirmHarvest]
  CH --> App[ApplyHarvest]
  App --> TryAdd[PlayerInventory.TryAdd]
  TryAdd --> Succ{Success / Partial ?}
  Succ -->|oui| OK[OnHarvestSuccess]
  OK --> Free[GridManager.FreeCells + UnregisterPlant]
  OK --> Vis[BiofiltreCell.SetVisualState false]
  OK --> Des[Destroy plante]
  Succ -->|Full| FB[InventoryFeedbackUI.ShowInventoryFull]

  Ptr([Clic direct plante IPointerClickHandler]) --> CH2[PlantHarvestInteractor.OnPointerClick]
  CH2 --> CH
```

**Note (code 2026-04)** : `TryOpenHarvestPanel` / `FindInteractorAt` existent encore dans `BiofiltreManager` mais **ne sont pas** utilisés par `HandleCellClicked` ; le chemin grille passe par **`TryOpenPlantPopup`** + registre de plante. Voir `Notes/Codebase_etat_reference.md`.

### UML classes — vue simplifiée (référence)

```mermaid
classDiagram
  direction TB
  class BiofiltreManager {
    +HandleCellClicked()
    +CanPlace()
    +PlantSeed()
    +PlantSeedAt()
    -TryOpenPlantPopup()
  }
  class BiofiltreGridVisualizer {
    +GenerateGrid()
    +GetCell()
    +OnCellClicked
  }
  class BiofiltreCell {
    +OnPointerClick()
    +Initialize()
    +SetVisualState()
  }
  class SeedSelectionUI {
    +Open()
    +Close()
    +IsPreviewActive
    -BuildSlots()
    -HandleSeedSelected()
  }
  class PlantPlacementPreview {
    +Begin()
    +Update()
    -ConfirmPlacement()
  }
  class PlantHarvestInteractor {
    +TryHarvest()
    +ConfirmHarvest()
    +Initialise()
  }

  BiofiltreManager --> BiofiltreGridVisualizer
  BiofiltreManager --> SeedSelectionUI
  BiofiltreManager --> PlantPlacementPreview : configure scène
  SeedSelectionUI --> PlantPlacementPreview
  PlantPlacementPreview --> BiofiltreManager
  BiofiltreGridVisualizer --> BiofiltreCell : crée N
  BiofiltreManager ..> PlantHarvestInteractor : après Instantiate
```

---

## Vue synthèse (Mermaid)

```mermaid
flowchart TB
  subgraph Données
    PD[PlantDefinition]
    SO_Grid[GridConfig / GridData]
    ID[ItemDefinition]
    IDB[ItemDatabase]
  end

  subgraph Grille_et_pose
    GM[GridManager]
    BGV[BiofiltreGridVisualizer]
    BCell[BiofiltreCell]
    BFM[BiofiltreManager]
    PPP[PlantPlacementPreview]
  end

  subgraph Plante_runtime
    PG[PlantGrow]
    PDH[PlantDefinitionHolder]
    PHI[PlantHarvestInteractor]
  end

  subgraph Inventaire
    PI[PlayerInventory]
    IUI[InventoryUI / InventorySlotUI]
    IFB[InventoryFeedbackUI]
  end

  SO_Grid --> GM
  BGV --> GM
  BGV --> BCell
  BCell -->|clic cellule libre| BFM
  BFM -->|ouvre| SSU[SeedSelectionUI]
  SSU -->|graine choisie + ancre| PPP
  PPP -->|confirme placement| BFM
  BFM -->|instancie prefab plante| PG
  BFM -->|Initialise| PDH
  PD --> PDH
  PD --> PG

  PHI --> PG
  PHI --> PDH
  PHI --> IDB
  IDB --> ID
  PHI --> PI
  PHI -->|inventaire plein| IFB
  PI -->|OnInventoryChanged| IUI
```

---

## Plantation (grille → graine au sol)

| Étape | Rôle | Fichiers / remarques |
|--------|------|----------------------|
| Config cases | Taille, origine, `WorldToGrid` | `GridManager`, `GridConfig` |
| Cellules cliquables | Raycast / UI pointer | `BiofiltreCell` (généré par `BiofiltreGridVisualizer`) |
| Choix de la graine | Liste `SeedEntry` → `PlantDefinition` | `SeedSelectionUI`, `SeedSlotUI` |
| Validité footprint | Toutes les cellules libres | `BiofiltreManager.CanPlace` + `PlantDefinition.GetOccupiedCells` |
| Fantôme | Souris, vert/rouge | `PlantPlacementPreview` |
| Pose | Instanciation + occupation grille | `BiofiltreManager.PlantSeedAt` → `PlantGrow` stade `Graine`, `OccupyCells`, `PlantDefinitionHolder.Initialise` |

**Note** : il n’y a pas de classe nommée `BuildManager` ; le rôle est tenu par **`BiofiltreManager`** + **`PlantPlacementPreview`**.

---

## Croissance

| Élément | Détail |
|---------|--------|
| `PlantGrow` | Timers par stade (`PlantDefinition.GetDuration`), enchaînement **Leafy** vs **Fruiting** |
| Sprites | Lus sur `PlantDefinition` par stade (`spriteMature`, `spriteFlowering`, etc.) |
| Récoltable | Stade = `PlantDefinition.HarvestStage` (souvent `Mature`) |

---

## Récolte ↔ inventaire

| Élément | Détail |
|---------|--------|
| Déclencheur (grille) | Clic **cellule occupée** → `BiofiltreManager.TryOpenPlantPopup` → `GridManager.GetPlantAt` → **`HarvestPanelUI.Open`** (interactor + `PlantGrow` + `PlantDefinition`). |
| Déclencheur (plante) | **`PlantHarvestInteractor`** (`IPointerClickHandler`) : clic sur le collider 2D de la plante → **`ConfirmHarvest`** direct si récoltable (caméra avec **Physics2DRaycaster** + EventSystem). |
| Éligibilité (récolte) | **`IsHarvestable()`** : stade **Mature** ou **Seedling** (le panel n’active le bouton *Récolter* que dans ces cas). Le champ `PlantDefinition.HarvestStage` reste la référence data (souvent `Mature`). |
| Item | `harvestItemId` sur `PlantDefinition` ou override sur le composant ; résolution via **`ItemDatabase`**. |
| Ajout | **`PlayerInventory.TryAdd(ItemDefinition, int)`** → **`InventoryResult`** (inclut **Partial**). |
| Feedback | `InventoryFeedbackUI` si inventaire plein. |
| Après succès | **`OnHarvestSuccess`** : libère les cases, désenregistre la plante, **Destroy** — pas de seconde récolte sur la même instance. |
| **Pistes design** | **Mature** et **Seedling** partagent encore le **même** `harvestItemId` ; **`maxHarvestCount`** non utilisé pour des récoltes répétées sur pied ; comportement **`Partial`** + destruction de la plante à trancher (perte de la quantité non stockée). |

---

## Deux récoltes sur un cycle (intention design)

Profil **Leafy** (dans `PlantGrow`) : `… → Mature (récolte feuilles) → Flowering → Seedling`.

- Aujourd’hui : une seule paire **`harvestStage` + `harvestItemId`** dans `PlantDefinition`.
- Piste : second couple stade/item pour **graines**, ou structure de « phases de récolte » + compteurs (`maxHarvestCount` déjà présent sur l’asset mais non câblé dans `PlantHarvestInteractor`).

---

## Légende rapide

- **ScriptableObject** : `PlantDefinition`, `ItemDefinition`, `GridConfig`…
- **MonoBehaviour scène** : grille, UI, plantes instanciées, inventaire joueur
