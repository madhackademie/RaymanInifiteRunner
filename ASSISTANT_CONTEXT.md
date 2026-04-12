## Assistant Context — RaymanInifiteRunner

### Etat actuel (compact)
- Projet Unity 6000.3.x avec base menu + premier niveau + scripts initiaux (`MainMenuUI`, `Timer`).
- Workflow notes en place: `PROJECT_LOG.md` (chronologique), `Notes/Todo_project.md` (hub TODO), `Notes/Learning/` (fiches pédagogiques).
- Pipeline art recentré sur une approche **2D SpriteRenderer** pour accélérer le prototypage mobile.
- Données plante / inventaire : `PlantDefinition` laitue avec **`harvestStages`** (ex. Mature → `harvestItemId` **`laitue_mature`**), item dans **`Assets/Data/Inventaire/`**, plante dans **`Assets/Data/Ferme/`**. Doc créateur : **`Docs/PLANTES_ET_INVENTAIRE.md`**.

### Décisions techniques actées
- Workflow Git: commencer les sessions par `git fetch` + `git status -sb`; penser `Save All` avant commit.
- Architecture gameplay plante: modèle hybride
  - `ScriptableObject` pour données statiques de type de plante
  - `MonoBehaviour` pour état runtime en scène
- Récolte/inventaire: comportement courant prévu en “all-or-nothing” tant que l’ajout partiel n’est pas formalisé.

### Priorités en cours
1. **Câblage récolte / inventaire (prochaine session — priorité identique)** : assignations Inspector + test jeu bout-en-bout (`BiofiltreManager`, `HarvestPanelUI`, `PlayerInventory`, `ItemDatabase`, `InventoryUI`, prefab plante). Voir `PROJECT_LOG.md` entrée **2026-04-12 — fin de session** et `Notes/Todo_project.md`.
2. **Récolte** : après câblage validé — verrou post-récolte + modèle **deux récoltes** (Mature / Seedling) si besoin.
3. **Doc flux** : `Notes/Farm/SYSTEMES_carte_mentale.md` ; données plante/item : **`Docs/PLANTES_ET_INVENTAIRE.md`**.
4. Continuer nettoyage/scoping assets prototype et vérifier les références Unity après purge 3D.

### Références clés
- `PROJECT_LOG.md` (entrée **2026-04-12 — fin de session** — données + doc ; câblage scène toujours à faire)
- `Docs/PLANTES_ET_INVENTAIRE.md` — `harvestItemId` / `itemId`, checklist nouvelle plante
- `Notes/Todo_project.md`
- `Notes/Farm/SYSTEMES_carte_mentale.md`
- `Notes/Learning/Event_Listener_Unity_CSharp.md`
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Inventory/PlayerInventory.cs`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`

