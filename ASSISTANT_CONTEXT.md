## Assistant Context — RaymanInifiteRunner

### Etat actuel (compact)
- Projet Unity 6000.3.x avec base menu + premier niveau + scripts initiaux (`MainMenuUI`, `Timer`).
- Workflow notes en place: `PROJECT_LOG.md` (chronologique), `Notes/Todo_project.md` (hub TODO), `Notes/Learning/` (fiches pédagogiques).
- Pipeline art recentré sur une approche **2D SpriteRenderer** pour accélérer le prototypage mobile.
- Données plante / inventaire : `PlantDefinition` laitue avec **`harvestStages`** (ex. Mature → `harvestItemId` **`laitue_mature`**), item dans **`Assets/Data/Inventaire/`**, plante dans **`Assets/Data/Ferme/`**. Doc créateur : **`Docs/PLANTES_ET_INVENTAIRE.md`**.

### Décisions techniques actées
- Workflow Git: commencer les sessions par `git fetch` + `git status -sb`; penser `Save All` avant commit.
- **Feature scènes / navigation / UI multi-stage** : branche de travail **créée** (2026-04-17) ; poursuivre les commits sur cette branche et fusionner dans `main` selon **`GIT_HELPER.md` --3--** quand le lot sera validé. Règle générale pour les prochains gros chantiers : idem **`GIT_HELPER.md`**.
- Architecture gameplay plante: modèle hybride
  - `ScriptableObject` pour données statiques de type de plante
  - `MonoBehaviour` pour état runtime en scène
- Récolte/inventaire: comportement courant prévu en “all-or-nothing” tant que l’ajout partiel n’est pas formalisé.
- **Récolte (décision jeu, 2026)** : **une seule récolte par plante** puis **destruction**. Plusieurs lignes dans **`harvestStages`** = **choix de timing** (ex. récolter à Mature ou attendre Seedling) : l’UI n’expose que la config du **stade courant** ; ce n’est **pas** deux récoltes d’affilée sur la même instance. Pas de « première récolte puis plante intacte pour une deuxième » sans changer ce flux.

### Priorités en cours
1. **LoadingScreen — visuel** : illustration (ex. poisson + arbre), import sprite UI, intégration scène **`Bootstrap`** — **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**, `Notes/Ui/Todo_ui.md`.
2. **Scènes Inventaire + Market + HUD sur tous les stages** : hub **`Carte`**, retour croix **`FirstLvl` → Carte**, prefabs / `UIManager` — **`Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`**, `Notes/Todo_project.md` (*Prochaine session*).
3. **Inventaire gameplay** : noyau récolte ↔ `PlayerInventory` considéré **bouclé** ; affinements UX (Partial, messages) seulement si besoin en playtest.
4. **Doc flux** : `Notes/Farm/SYSTEMES_carte_mentale.md` ; données plante/item : **`Docs/PLANTES_ET_INVENTAIRE.md`**.
5. Continuer nettoyage/scoping assets prototype et vérifier les références Unity après purge 3D.

### Références clés
- `Notes/Ui/LOADINGSCREEN_image_workflow.md` — art + intégration écran de chargement
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md` — navigation scènes, sync/async, HUD global
- `PROJECT_LOG.md` (dernière entrée **2026-04-17** — notes LoadingScreen + branche navigation)
- `Docs/PLANTES_ET_INVENTAIRE.md` — `harvestItemId` / `itemId`, checklist nouvelle plante
- `Notes/Todo_project.md`
- `Notes/Farm/SYSTEMES_carte_mentale.md`
- `Notes/Learning/CSharp_bases_et_Cursor_Unity.md` — rappels grille / `=>` / Cursor C#
- `Notes/Learning/Event_Listener_Unity_CSharp.md`
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Inventory/PlayerInventory.cs`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`

