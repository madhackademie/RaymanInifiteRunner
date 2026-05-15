## Assistant Context — RaymanInifiteRunner

### Etat actuel (compact)
- Projet Unity 6000.3.x : **boot** `Bootstrap.unity` → **`GameBootstrap`** charge additivement le shell **`NavigationHUD`**, **`HomeScene`**, puis **`Inventaire`** (eager, racines masquées jusqu’à navigation) ; visibilité des scènes de contenu via **`SceneNavigator.ShowScene`** (`SetActive` sur racines + lazy-load optionnel pour scènes listées) ; constantes **`SceneId`** ; gameplay ferme dans **`FirstLvl`** avec **`FirstLvlController`** (retour hub sur **`OnExitToHomeRequested`**).
- Session 2026-04-20 matin (portable) : séparation en cours de l’inventaire hors gameplay `FirstLvl` (objectif multi-levels) + mise en place d’un inventaire persistant JSON ; reprise incomplète après interruption côté BezyIA.
- Workflow notes : `PROJECT_LOG.md`, `Notes/Todo_project.md`, `Notes/Learning/`.
- Suivi taches : **statut source unique** dans `Notes/Todo_project.md` (les autres notes gardent le detail technique).
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
1. **Prochaine session (validée auteur, fin session 2026-05-15)** : **[P0-SHOP-POP-001]** créer une **branche Git** puis **polish popup d’achat** shop (`ShopItemPopup`) — détail **`Notes/Todo_project.md`**, **`PROJECT_LOG.md`** (*Fin de session*). Ensuite **[P0-FARM-SEED-INV-001]** : **graines plantation ↔ inventaire** (`SeedSelectionUI` vs `PlayerInventory`).
2. **Shop — polish** (contenu de P0-SHOP-POP-001) : UI/UX, saisie quantité, **Max**, confirmation — **`Notes/Ui/Todo_ui.md`**, **`popup_generique.md`** §3 (`CT-SHOP-002` … `CT-SHOP-006`).
3. **Inventaire** : finaliser la **séparation inventaire/gameplay** (actuellement `FirstLvl`, cible tous niveaux), rétablir la scène inventaire dédiée si encore pertinente, et sécuriser le flux de reprise après interruption BezyIA.
4. **Persistance inventaire JSON** : fiabiliser save/load (ouverture scène, changement de scène, relance jeu) ; vérifier cohérence UI/slots.
5. **~2026-05-01 — Audit Bezi + refactor navigation Scene/UI** : terminer l’audit sur le flux réel (`ShowScene`, boot eager, `UIManager`) ; **clean/refactor** ; supprimer ou documenter le code mort ; **réaligner** `ARCHI` / `Journal_ui` / `Todo_ui` / guide scènes — **`Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`**, **`PROJECT_LOG.md`** (2026-04-21).
6. **Navigation inter-scène / UI** : playtests et durcissement (Build Settings, double **`EventSystem`**, tous chemins hub ↔ inventaire ↔ niveau) — croiser **`Notes/Todo_project.md`**, **`Notes/Ui/Todo_ui.md`** ; réf. **`Notes/Ui/SceneUiLoadManagement.md`**.
7. **Persistance grille** : état des cellules / cultures à la **fermeture de scène** et à la **quitt** (piste **`ScriptableObject`** + save ultérieure) — **`Notes/Todo_project.md`**.
8. **Croissance plantes hors scène / hors ligne** : recalcul via **UTC** à la reprise ; **cloud** (ex. UGS) en évolution possible — croiser **`Timer`**, spec temps GDD, **`Notes/Todo_project.md`**.
9. **LoadingScreen — visuel** : illustration + intégration **`Bootstrap`** — **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**.
10. **Doc flux** : `Notes/Farm/SYSTEMES_carte_mentale.md` ; **`Docs/PLANTES_ET_INVENTAIRE.md`**.
11. Nettoyage assets prototype / références Unity.

### Rappel protocole gestion de projet (session)
- Pour toute question "tache du jour / priorite / prochaine session", lire en premier:
  - `WORKFLOW_PROTOCOL.md`
  - `ASSISTANT_CONTEXT.md`
  - `PROJECT_LOG.md` (derniere entree)
  - `Notes/Todo_project.md` (prochaine session / priorite immediate)
- Repondre uniquement avec la priorite la plus recente issue des docs, sans invention.
- Priorite immediate actuellement retenue (a revalider via docs a chaque session) — **fin session 2026-05-15** :
  - **[P0-SHOP-POP-001]** branche + polish popup d'achat shop ; puis **[P0-FARM-SEED-INV-001]** graines plantation ↔ inventaire.

### Prompt de reprise BezyIA
- Prompt à relancer tel quel :
  - `"encore une fois il y a eu une coupure peux tu reprendre toutefois j'ai du fermer la session unity entre temps donc je ne sais pas si tu va retrouver toutes les traces necessaires. il te faudra te fier au thread."`

### Références clés
- `Assets/Scripts/Systems/SceneNavigator.cs` — visibilité des scènes de contenu (`ShowScene`, lazy optionnel)
- `Notes/Ui/LOADINGSCREEN_image_workflow.md` — art + intégration écran de chargement
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md` — navigation scènes, sync/async, HUD global
- `PROJECT_LOG.md` (entrées **2026-05-15** — popups FirstLvl / shop / ferme + *Fin de session*)
- `Docs/PLANTES_ET_INVENTAIRE.md` — `harvestItemId` / `itemId`, checklist nouvelle plante
- `Notes/Todo_project.md`
- `Notes/GUIDE_suivi_projet.md` — mode d’emploi suivi (source unique + IDs)
- `Notes/Farm/SYSTEMES_carte_mentale.md`
- `Notes/Learning/CSharp_bases_et_Cursor_Unity.md` — rappels grille / `=>` / Cursor C#
- `Notes/Learning/Event_Listener_Unity_CSharp.md`
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Inventory/PlayerInventory.cs`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`

