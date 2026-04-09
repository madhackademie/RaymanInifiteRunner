## Assistant Context — RaymanInifiteRunner

### Etat actuel (compact)
- Projet Unity 6000.3.x avec base menu + premier niveau + scripts initiaux (`MainMenuUI`, `Timer`).
- Workflow notes en place: `PROJECT_LOG.md` (chronologique), `Notes/Todo_project.md` (hub TODO), `Notes/Learning/` (fiches pédagogiques).
- Pipeline art recentré sur une approche **2D SpriteRenderer** pour accélérer le prototypage mobile.
- Données plante amorcées en mode data-driven avec `PlantDefinition` + `Laitue.asset`.

### Décisions techniques actées
- Workflow Git: commencer les sessions par `git fetch` + `git status -sb`; penser `Save All` avant commit.
- Architecture gameplay plante: modèle hybride
  - `ScriptableObject` pour données statiques de type de plante
  - `MonoBehaviour` pour état runtime en scène
- Récolte/inventaire: comportement courant prévu en “all-or-nothing” tant que l’ajout partiel n’est pas formalisé.

### Priorités en cours
1. **Inventaire** : câbler et **tester** en scène (laitue : items DB, prefabs UI, `PlantHarvestInteractor`) — code présent, **non validé** en jeu.
2. **Récolte** : verrou post-récolte + modèle pour **deux récoltes** (ex. feuilles à `Mature`, graines à `Seedling`) — refactor `PlantDefinition` / `PlantHarvestInteractor` à planifier.
3. **Doc flux** : carte des systèmes — `Notes/Farm/SYSTEMES_carte_mentale.md` (plantation, croissance, inventaire).
4. Continuer nettoyage/scoping assets prototype et vérifier les références Unity après purge 3D.

### Références clés
- `PROJECT_LOG.md` (entrée **2026-04-09** — inventaire non testé)
- `Notes/Todo_project.md`
- `Notes/Farm/SYSTEMES_carte_mentale.md`
- `Notes/Learning/Event_Listener_Unity_CSharp.md`
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Inventory/PlayerInventory.cs`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`

