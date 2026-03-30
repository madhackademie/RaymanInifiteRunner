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
1. Spécifier inventaire (slots, pile max, API `TryAdd`, résultats d’ajout).
2. Finaliser machine d’états plante (stades visuels déjà posés : `Seedling -> BabyLeaf -> Growing -> Mature -> Bolting`).
3. Implémenter flux minimal jouable (croissance timer + clic mature -> tentative inventaire -> feedback UI).
4. Continuer nettoyage/scoping assets prototype et vérifier les références Unity après purge 3D.

### Références clés
- `PROJECT_LOG.md`
- `Notes/Todo_project.md`
- `Notes/Learning/Event_Listener_Unity_CSharp.md`
- `Assets/Scripts/Data/PlantDefinition.cs`

