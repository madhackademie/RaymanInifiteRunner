## Assistant Context — RaymanInifiteRunner

### Etat actuel (compact)
- Projet Unity 6000.3.x : **boot** `Bootstrap.unity` → **`GameBootstrap`** charge additivement le shell **`NavigationHUD`** puis la scène de contenu **`HomeScene`** ; transitions de contenu via **`SceneNavigator`** (**additif** + **`UnloadSceneAsync`**) et constantes **`SceneId`** ; gameplay ferme dans **`FirstLvl`** avec **`FirstLvlController`** (retour hub).
- Session 2026-04-20 matin (portable) : séparation en cours de l’inventaire hors gameplay `FirstLvl` (objectif multi-levels) + mise en place d’un inventaire persistant JSON ; reprise incomplète après interruption côté BezyIA.
- Workflow notes : `PROJECT_LOG.md`, `Notes/Todo_project.md`, `Notes/Learning/`.
- Pipeline art : **2D SpriteRenderer** (prototype mobile).
- Données plante / inventaire : `PlantDefinition` + **`harvestStages`** / **`laitue_mature`**, assets sous **`Assets/Data/`**. Doc : **`Docs/PLANTES_ET_INVENTAIRE.md`**.

### Décisions techniques actées
- Workflow Git: commencer les sessions par `git fetch` + `git status -sb`; penser `Save All` avant commit.
- **Feature scènes / navigation / UI multi-stage** : branche de travail **créée** (2026-04-17) ; poursuivre les commits sur cette branche et fusionner dans `main` selon **`GIT_HELPER.md` --3--** quand le lot sera validé. Règle générale pour les prochains gros chantiers : idem **`GIT_HELPER.md`**.
- Architecture gameplay plante: modèle hybride
  - `ScriptableObject` pour données statiques de type de plante
  - `MonoBehaviour` pour état runtime en scène
- Récolte/inventaire: comportement courant prévu en “all-or-nothing” tant que l’ajout partiel n’est pas formalisé.
- **Récolte (décision jeu, 2026)** : **une seule récolte par plante** puis **destruction**. Plusieurs lignes dans **`harvestStages`** = **choix de timing** (ex. récolter à Mature ou attendre Seedling) : l’UI n’expose que la config du **stade courant** ; ce n’est **pas** deux récoltes d’affilée sur la même instance. Pas de « première récolte puis plante intacte pour une deuxième » sans changer ce flux.

### Priorités en cours
1. **Inventaire (priorité immédiate)** : finaliser la **séparation inventaire/gameplay** (actuellement `FirstLvl`, cible tous niveaux), rétablir la scène inventaire dédiée, et sécuriser le flux de reprise après interruption BezyIA.
2. **Persistance inventaire JSON** : fiabiliser save/load (ouverture scène, changement de scène, relance jeu) ; vérifier cohérence UI/slots.
3. **Navigation inter-scène / UI** : **debug** et **durcissement** du flux **`SceneNavigator`** (additif + unload async), tests complets (Build Settings, double **`EventSystem`**, retours depuis **`FirstLvl`**) — **`Notes/Todo_project.md`**, **`Notes/Ui/Todo_ui.md`**, **`Notes/Ui/ARCHI_hud_ui_manager_additive.md`**.
4. **Persistance grille** : état des cellules / cultures à la **fermeture de scène** et à la **quitt** (piste **`ScriptableObject`** + save ultérieure) — **`Notes/Todo_project.md`**.
5. **Croissance plantes hors scène / hors ligne** : recalcul via **UTC** à la reprise ; **cloud** (ex. UGS) en évolution possible — croiser **`Timer`**, spec temps GDD, **`Notes/Todo_project.md`**.
6. **LoadingScreen — visuel** : illustration + intégration **`Bootstrap`** — **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**.
7. **Doc flux** : `Notes/Farm/SYSTEMES_carte_mentale.md` ; **`Docs/PLANTES_ET_INVENTAIRE.md`**.
8. Nettoyage assets prototype / références Unity.

### Prompt de reprise BezyIA
- Prompt à relancer tel quel :
  - `"encore une fois il y a eu une coupure peux tu reprendre toutefois j'ai du fermer la session unity entre temps donc je ne sais pas si tu va retrouver toutes les traces necessaires. il te faudra te fier au thread."`

### Références clés
- `Assets/Scripts/Systems/SceneNavigator.cs` — transitions contenu additif + unload async
- `Notes/Ui/LOADINGSCREEN_image_workflow.md` — art + intégration écran de chargement
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md` — navigation scènes, sync/async, HUD global
- `PROJECT_LOG.md` (dernière entrée **2026-04-19** — persistance grille + temps hors scène)
- `Docs/PLANTES_ET_INVENTAIRE.md` — `harvestItemId` / `itemId`, checklist nouvelle plante
- `Notes/Todo_project.md`
- `Notes/Farm/SYSTEMES_carte_mentale.md`
- `Notes/Learning/CSharp_bases_et_Cursor_Unity.md` — rappels grille / `=>` / Cursor C#
- `Notes/Learning/Event_Listener_Unity_CSharp.md`
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Inventory/PlayerInventory.cs`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`

