## Assistant Context — RaymanInifiteRunner

### Etat actuel (compact)
- Projet Unity 6000.3.x avec base menu + premier niveau + scripts initiaux (`MainMenuUI`, `Timer`).
- Workflow notes en place: `PROJECT_LOG.md` (chronologique), `Notes/Todo_project.md` (hub TODO), `Notes/Learning/` (fiches pédagogiques).
- Pipeline art en exploration avec assets laitue (stades initiaux) + orientation possible vers un rendu 3D selon performances mobile.

### Décisions techniques actées
- Workflow Git: commencer les sessions par `git fetch` + `git status -sb`; penser `Save All` avant commit.
- Architecture gameplay plante: modèle hybride
  - `ScriptableObject` pour données statiques de type de plante
  - `MonoBehaviour` pour état runtime en scène
- Récolte/inventaire: comportement courant prévu en “all-or-nothing” tant que l’ajout partiel n’est pas formalisé.

### Priorités en cours
1. Spécifier inventaire (slots, pile max, API `TryAdd`, résultats d’ajout).
2. Définir machine d’états plante (`Seed -> Growing -> Mature -> Harvested`).
3. Implémenter flux minimal jouable (clic plante mature -> tentative inventaire -> feedback UI).
4. Continuer nettoyage/scoping assets prototype.

### Références clés
- `PROJECT_LOG.md`
- `Notes/Todo_project.md`
- `Notes/Learning/Event_Listener_Unity_CSharp.md`

