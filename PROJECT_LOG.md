# Project log — RaymanInfiniteRunner journal chronologique

## 2026-03-19
### Contexte
- Machine: PC bureau/ **PC portable**
- Unity: <version 6.0>
- Branche: <main/feature/...mise en place>

### Ce qu’on a fait
- [x] Mise en place des fichiers de workflow et de contexte (`WORKFLOW_PROTOCOL.md`, `ASSISTANT_CONTEXT.md`, `PROJECT_LOG.md`, `GIT_HELPER.md`)
- [x] Clarification du processus multi-machine (journal + contexte + règles)
- [x] Premiers tests de commandes Git (status, fetch, pull, add, commit, push)

### Problèmes rencontrés / pistes
- Blocage sur la coloration syntaxique Markdown pour les blocs de code dans `GIT_HELPER.md`
- Difficultés liées à la sauvegarde des fichiers avant commit (fichier vu comme vide sur GitHub)
- Confusion autour des écrans `@review-changes` et de l’état Git local vs distant

### Décisions
- Utiliser `PROJECT_LOG.md` comme journal chronologique et `ASSISTANT_CONTEXT.md` comme résumé d’état
- Mettre les procédures (prompts/commandes) dans `WORKFLOW_PROTOCOL.md`
- Centraliser les commandes Git courantes dans `GIT_HELPER.md`

### Prochaines actions (priorité)
1. Définir et documenter les règles du projet (style, organisation, conventions AI) dans un fichier dédié ou dans `ASSISTANT_CONTEXT.md`
2. Commencer à esquisser un GDD simple pour le jeu (concept, boucle de gameplay, scope minimal)
3. Continuer à stabiliser le workflow Git (autosave, routine début/fin de session)

### Liens utiles
- Issue/PR: …
- Docs: …

## 2026-03-20
### Contexte
- Machine: **PC bureau**/ PC portable
- Unity: <version 6.0>
- Branche: <main/feature/...mise en place/rules>

### Ce qu’on a fait
- [x] Mise en place des fichiers de rules architectures
- [x] premier jet d'organisation de prise de note et tache a accomplir
- [x] Clarification du rôle des fichiers Cursor `.mdc` (règles `alwaysApply`)
- [x] Proposition d’organisation des notes en `Notes/` avec sous-dossiers par thème et convention de nommage (`INBOX_`, `TODO_`, `DECISIONS_`, `SPEC_`)
- [x] Cadrage UI : architecture panneaux en stack 2-3 layers + animation via `Animator` (fade + slide) + localisation TMP

### Problèmes rencontrés / pistes
- Blocage sur le type d'ui et gameplay
- Quel organisation/structuration suivre sans se perdre


### Décisions
- Utiliser le dossiers note pour y inscrire les idées les recherches et possible futur action
- UI : démarrer en UI “super basic” (prototype), organiser les écrans en stack de profondeur 2-3, piloter les transitions via `Animator` (fade + slide).
- Localization : séparer `country` (détection locale pour pub/market) et `language` (choix joueur), avec un `LanguageManager` déclenché lors du changement d’option.
- Notes : plusieurs fichiers par thème (au lieu d’un “journal” unique) avec nommage stable : `INBOX_{theme}.md`, `TODO_{theme}.md`, `DECISIONS_{theme}.md`, `SPEC_{theme}.md` (et sous-thèmes optionnels).

### Prochaines actions (priorité)
1. Définir et créer une architecture d'organisation des données et taches à accomplir en mode thèmatique par exemple UI avec fichier enfant + regles de nomage 
2. Commencer à esquisser un GDD simple pour le jeu (concept, boucle de gameplay, scope minimal)
3. Continuer à stabiliser le workflow Git (autosave, routine début/fin de session)
4. comment suivre mes credit IA pour pouvoir lancer au minimum encore la commande de fin de session
5. Remplir `Notes/Ui/SPEC_ui.md` avec la trame (stack 2-3 layers, contrat UIPanel, localization TMP country vs language)
6. Définir une convention de “keys” pour les TextMeshPro (pour faciliter le futur passage au vrai LanguageManager)

### Liens utiles
- Issue/PR: …
- Docs: …

## 2026-03-21
### Contexte
- Machine: **PC bureau** (session courante) · PC portable
- Unity: <version 6.0>
- Branche: <main / feature selon le repo>

### Ce qu’on a fait
- [x] Note de référence Bezi : `Notes/Bezi/README_bezi.md` (Welcome, index `llms.txt`, prompting, threads `@`, images, sécurité IP)
- [x] Transfert des tâches **LanguageManager / TextMeshPro** de `Notes/Ui/Decision_ui.md` vers `Notes/Ui/Todo_ui.md` (checklist d’implémentation)
- [x] `Decision_ui.md` : section localisation réduite à la **décision** + renvoi vers `Todo_ui.md` ; DoD UI (stack / fade-slide) conservé hors critère langue

### Problèmes rencontrés / pistes
- Clarifier plus tard **bezi.actions** dans la même note ou fichier dédié quand l’usage est figé

### Décisions
- Garder la doc agent **dans le repo** sous `Notes/Bezi/` pour partage Cursor / équipe et traçabilité

### Prochaines actions (priorité)
1. Compléter `Notes/Bezi/README_bezi.md` (Unity exact, scènes de travail, bezi.actions)
2. Remplir `Notes/Ui/Spec_ui.md` si la spec UI doit vivre séparément de `Decision_ui.md`
3. Poursuivre le hub `Notes/Todo_project.md` (liens vers TODOs thématiques sans dupliquer `PROJECT_LOG`)
4. Mise en place règle pour bezi voir [Thomas brush](https://youtu.be/LdZ0po5wU_0?t=204)

### Liens utiles
- Bezi Welcome : https://docs.bezi.com/get-started/welcome
- Index doc : https://docs.bezi.com/llms.txt

## 2026-03-22
### Contexte
- Machine: **PC bureau** (session courante) · PC portable
- Unity: 6000.3.x (réf. build locale)
- Branche: <main / feature selon le repo>

### Ce qu’on a fait
- [x] **Bezi (Sidekick)** : package installé `Packages/com.bezi.sidekick` — **Bezi Plugin v0.79.17** (dépendance `com.unity.nuget.newtonsoft-json`).
- [x] **UI prototype menu principal** (UGUI) sur la scène `Assets/SampleScene.unity` :
  - `Canvas` + `CanvasScaler`
  - `MainMenuPanel` avec composant **`MainMenuUI`**
  - boutons **`StartButton`** / **`OptionsButton`**
  - **`OptionsPanel`** (masqué au `Awake`, affiché/masqué au clic Options)
- [x] Script **`Assets/Scripts/UI/MainMenuUI.cs`** : `SerializeField` pour les boutons et le panel ; Start → `Debug.Log` + `SceneManager.LoadScene` en commentaire ; pas de `Update()` inutile.
- [x] Arborescence scripts amorcée : dossiers `Assets/Scripts/` avec `UI/`, et métas pour `Core/`, `Farm/`, `Data/`, `Localisation/` (structure projet).

### Problèmes rencontrés / pistes
- **Scène de build vs scène du menu** : `ProjectSettings/EditorBuildSettings` pointe vers `Assets/Scenes/SampleScene.unity`, alors que le menu prototype est dans **`Assets/SampleScene.unity`** — à aligner (une seule scène de démarrage ou fusion) avant build / tests device.

### Prochaines actions (priorité)
1. Choisir la scène unique de démarrage et mettre à jour **Editor Build Settings** + éventuellement supprimer le doublon `SampleScene`.
2. Remplacer le `Debug.Log` Start par `SceneManager.LoadScene` quand la scène gameplay existe.
3. Brancher le contenu réel du panneau Options (langue, audio, etc.) selon `Notes/Ui/Todo_ui.md`.

### Liens utiles
- Bezi install : https://docs.bezi.com/bezi/install-setup

## 2026-03-23
### Contexte
- Machine: **PC bureau** (session courante) · PC portable
- Unity: 6000.3.x
- Branche: <main / feature selon le repo>

### Ce qu’on a fait
- [x] **Références UI mobile** : pistes pour blueprints (stores, Behance/Dribbble, moodboards, contraintes safe area / zones pouces / HUD farm).
- [x] **Todo polish post-prototype** : entrée dans `Notes/Todo_project.md` — workflow graphique + [Adobe Firefly (jeu vidéo)](https://www.adobe.com/products/firefly/discover/ai-for-game-developers.html) ; rappel licence / usage commercial à valider plus tard.
- [x] **Game design (temps)** : discussion sur la modélisation du temps en farm (ex. croissance type ~1 % masse/jour) via compression, phases/jalons, parallélisme (plusieurs bassins / cultures), boucles courtes en session.
- [x] **Progression hors ligne (mobile)** : principe `lastUtc` → `delta` à la reprise, intégration analytique ou taux par seconde, **plafond offline**, UTC + gestion horloge ; salades 6–8 semaines réelles mappées sur temps compressé + sessions ~3 min/jour.

### Décisions
- **Todo projet** : conserver une **vue globale** dans `Notes/Todo_project.md` pour le polish Firefly ; **pas** de migration vers `Notes/Art/` tant que le volet n’est pas attaqué.

### Prochaines actions (priorité)
1. Collecter 2–3 **jeux références** (screenshots) + noter dans `Notes/Ui/` ce qui est repris ou évité.
2. Esquisser une **spec temps** (durée d’un « jour ferme », cap offline, formule croissance) dans le GDD ou une note `Notes/GDD/`.
3. Aligner **Editor Build Settings** / scène menu (`Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity`) quand le proto gameplay est prêt.

### Liens utiles
- Firefly & jeu : https://www.adobe.com/products/firefly/discover/ai-for-game-developers.html
- Cozy UI (article de référence) : https://sdlccorp.com/post/the-art-of-designing-intuitive-user-interfaces-in-cozy-games/

## 2026-03-24
### Contexte
- Machine: **PC bureau** (relecture du projet après restore / remise en route Unity)
- Unity: 6000.3.x
- Branche: <main / feature selon le repo>

### Ce qu’on a fait / état constaté (relecture)
- [x] **Build Settings** (`EditorBuildSettings`) : scène **0** = `Assets/Scenes/SampleScene.unity`, scène **1** = `Assets/Scenes/FirstLvl.unity` (les deux activées) — flux menu → niveau.
- [x] **Menu** : `MainMenuUI` est référencé dans **`Assets/Scenes/SampleScene.unity`**.
- [x] **Script** `Assets/Scripts/UI/MainMenuUI.cs` : Start / Options + panneau options ; `SceneManager.LoadScene("FirstLvl")` au clic Start.
- [x] Rappel : `git restore` n’affiche souvent rien si OK ; l’UI disparaît si le `.unity` **dans Git** n’avait pas les branchements — **commit** menu + `.meta` une fois stable.
- [x] **Placement UI** : contrôleur sous le **Canvas**, pas sur la Main Camera.
- [x] **Timer (Core)** : premier script **`Assets/Scripts/Core/Timer.cs`** — minuteur générique **Countdown** ou **Stopwatch** sur `MonoBehaviour`, incrément `elapsedTime` dans `Update` via `Time.deltaTime`, durée configurable, `autoStart` / `loop`, événements **`UnityEvent<float> onTick`** (temps courant : restant ou écoulé selon le mode) et **`onCompleted`**, plus `StartTimer` / `Pause` / `Stop` / `Restart` / `SetDuration` ; exposé : `ElapsedTime`, `RemainingTime`, `NormalizedProgress`, `IsRunning`.
- [x] **Usage prévu** : s’appuyer sur ce timer (ou une évolution) pour **valider les durées de croissance** des ressources du joueur **avant** qu’elles deviennent **collectables** (prototype en jeu ; lien futur avec spec **temps réel / offline UTC** du GDD).
- [ ] **Suivi** : prévoir une passe **revue + améliorations** du `Timer` avec l’assistant (perf si nombreux timers, `unscaledDeltaTime`, persistance / reprise hors ligne, etc.).

### Problèmes rencontrés / pistes
- Possible **doublon** `Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity` — à trancher / nettoyer.
- Le `Timer` actuel est **temps de jeu** (`Time.deltaTime`) : pour croissance longue + **offline**, il faudra probablement compléter avec une couche **données + timestamp** (voir discussions 2026-03-23) plutôt que uniquement ce composant seul.

### Prochaines actions (priorité)
1. **Commit** scène menu + branchements `MainMenuUI` après validation play mode.
2. Supprimer le `SampleScene` dupliqué si inutile.
3. Panneau **Options** : contenu réel selon `Notes/Ui/Todo_ui.md`.
4. **Session Timer** : relire `Timer.cs` avec l’assistant (comportement détaillé + pistes d’évolution pour croissance / collecte + offline).

### Liens utiles
- `Notes/Ui/Todo_ui.md` — LanguageManager / TMP

## 2026-03-26
### Contexte
- Machine: **PC portable** (session de reprise et stabilisation workflow)
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Stabilisation du workflow Git/Markdown (rappels sur `fetch`, `status`, `pull`, `add`, `commit`, `push`).
- [x] Clarification du comportement Git : `git status` ne reflète l’état distant qu’après `git fetch`.
- [x] Diagnostic des confusions locale/distant (fichier perçu vide sur GitHub car non sauvegardé/commit/push au bon moment).
- [x] Clarification UI GitHub : différence entre vue `Code` (source actuelle) et vue `commit/PR/compare` (diff coloré).
- [x] Clarification Markdown : coloration des blocs de commandes (`bash` souvent plus lisible que `powershell` pour simples commandes Git).
- [x] Cadrage gameplay technique : modèle “plante qui mûrit puis récolte” avec état mature, clic, tentative d’ajout inventaire, refus si plein.
- [x] Proposition d’architecture event-driven : objet récoltable -> demande de récolte -> inventaire répond succès/échec -> UI message si inventaire plein.
- [x] Création d’un dossier de notes pédagogiques `Notes/Learning/`.
- [x] Ajout d’une fiche explicative `Notes/Learning/Event_Listener_Unity_CSharp.md` (concepts, patterns d’abonnement, erreurs fréquentes, mini plan d’implémentation).
- [x] Ajout d’un index `Notes/Learning/README_learning.md` pour structurer l’apprentissage technique.

### Problèmes rencontrés / pistes
- Incompréhension fréquente entre “fichier modifié en mémoire éditeur” vs “fichier sauvegardé sur disque” avant Git.
- `@review-changes` interprété comme blocage, alors que c’est un écran de revue (pas l’état Git réel).
- Attente de coloration forte des commandes Git dans tous les contextes (rendu variable selon chat/Cursor/GitHub).

### Décisions
- Conserver un protocole simple de début de session : `git fetch` puis `git status -sb`.
- Continuer à documenter les commandes dans `GIT_HELPER.md` en blocs `bash` pour lisibilité.
- Garder la logique de récolte “all-or-nothing” tant que la règle d’ajout partiel n’est pas explicitement définie.

### Prochaines actions (priorité)
1. Spécifier précisément le système inventaire (slots, stack max, ajout partiel ou non) dans une note GDD dédiée.
2. Définir les états de culture (`graine`, `croissance`, `mature`, `récolté`) et leurs transitions.
3. Implémenter un flux de test minimal : clic objet mature -> `TryAdd` inventaire -> succès/réussite UI -> reset état plante.
4. Ajouter dans `WORKFLOW_PROTOCOL.md` un rappel explicite “Save All avant Git”.

### Liens utiles
- `GIT_HELPER.md` — routine Git opératoire
- `WORKFLOW_PROTOCOL.md` — protocole début/fin de session
- `Notes/Learning/README_learning.md` — index des notes pédagogiques
- `Notes/Learning/Event_Listener_Unity_CSharp.md` — cours event/listener Unity C#

## 2026-03-26 (suite)
### Contexte
- Machine: **PC bureau**
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Création / import des assets **laitue** (modèles et matériaux) pour avancer le prototype visuel.

### Problèmes rencontrés / pistes
- À valider ensuite: quels assets “laitue” restent dans le scope prototype et lesquels seront nettoyés/remplacés après validation du concept art.

### Prochaines actions (priorité)
1. Finaliser la version “prototype” des assets laitue à conserver.
2. Nettoyer les assets temporaires non retenus avant commit final art.

### Liens utiles
- `Assets/Art/Models/`
- `Assets/GeneratedModels/`

## 2026-03-26 (mini session PC portable)
### Contexte
- Machine: **PC portable**
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Clarification technique sur l’erreur réseau du package 404 Gen3D (connexion coupée côté hôte distant, probable limitation temporaire/charge service).
- [x] Consolidation du hub `Notes/Todo_project.md` à partir de `PROJECT_LOG.md` (synthèse et priorisation par sections).
- [x] Ajout de la piste “double procédé graphique” (pipeline léger + pipeline 3D) et règle de décision mobile -> 3D si performances stables.
- [x] Marquage des tâches non bloquantes avec `[OPTIONNEL]`.
- [x] Cadrage architectural pour le système plante générique: `ScriptableObject` (données statiques) + `MonoBehaviour` (état runtime), événements de récolte et règles inventaire.

### Problèmes rencontrés / pistes
- Le service 404 Gen3D peut couper la connexion en période de charge (retry à heures creuses recommandé).
- Besoin de transformer le cadrage d’architecture plante en note de référence + squelette de code exploitable.

### Décisions
- Démarrer le système plante sur un modèle **hybride SO + MB** plutôt que “MB only” pour éviter un gros refactor.
- Conserver la règle de récolte **all-or-nothing** tant que l’ajout partiel n’est pas défini.

### Prochaines actions (priorité)
1. Implémenter le squelette `PlantDefinition` + `PlantInstance`.
2. Définir l’API `Inventory.TryAdd(...)` et l’enum `InventoryAddResult`.
3. Tester le flux minimal: `Mature` -> clic -> `TryAdd` -> succès/échec + UI.

### Liens utiles
- `Notes/Todo_project.md`
- `Notes/Learning/Event_Listener_Unity_CSharp.md`

## 2026-03-30
### Contexte
- Machine: **PC portable**
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Allégement du projet côté assets en supprimant une partie des contenus 3D de test non prioritaires (samples 404 Gen plugin, `GeneratedModels`, anciens modèles laitue 3D).
- [x] Orientation confirmée vers un flux **SpriteRenderer 2D** pour accélérer le prototypage mobile.
- [x] Ajout du `ScriptableObject` générique `PlantDefinition` avec stades visuels (`seedling`, `babyLeaf`, `growing`, `mature`, `bolting`).
- [x] Création de l’asset plante `Laitue.asset` pour initialiser un premier type de plante data-driven.
- [x] Mise à jour de `PlantGrow` pour lire `PlantDefinition` et appliquer le sprite selon le stade.

### Problèmes rencontrés / pistes
- Temps de reload/compilation Unity encore élevé même pour de petites modifications de scripts.
- Choix assumé: continuer en version simple orientée 2D pour limiter la friction de prod.

### Décisions
- Prioriser la livraison d’un prototype jouable léger avec pipeline sprite.
- Garder le 3D comme piste ultérieure conditionnée à la stabilité/performance et au temps disponible.

### Prochaines actions (priorité)
1. Brancher la logique de croissance temporelle dans `PlantGrow` (transition de stades avec timer).
2. Connecter la récolte à l’inventaire (`TryAdd`) avec gestion d’échec inventaire plein.
3. Nettoyer les références orphelines Unity éventuelles après la suppression d’assets 3D.

### Liens utiles
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Farm/PlantGrow.cs`
- `Assets/Scripts/Data/Laitue.asset`

## 2026-03-31
### Contexte
- Machine: **PC bureau** (organisation docs / prochaine session)
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Ajout dans `Notes/Todo_project.md` de **2 tâches à cocher** pour la prochaine session : (1) sprites **sans fond blanc** / transparence, (2) **footprint** plantes + grille / `BuildManager` (Bezi et/ou Cursor).
- [x] Création de `Notes/Farm/SPEC_plant_footprint_prompt.md` : modèle `origin + offsets`, exemples poireau / salade 2×2 / tomate en croix, extension type `PlantDefinition` avec `Vector2Int[] footprint`, pseudo-code de validation, conventions à trancher (axes, rotation), **bloc prompt** copier-coller.
- [x] Hub `Todo_project.md` : lien vers la note Farm pour retrouver vite le spec/prompt.

### Prochaines actions (priorité)
1. Traiter les deux cases **Prochaine session** dans `Notes/Todo_project.md`.
2. Après impl footprint, aligner `PlantGrow` / prefab avec une seule racine par instance multi-cellules si souhaité.

### Liens utiles
- `Notes/Todo_project.md`
- `Notes/Farm/SPEC_plant_footprint_prompt.md`

## 2026-04-02
### Contexte
- Machine: **PC bureau** (compte rendu session + doc footprint)
- Unity: 6000.3.x
- Branche: `main` (commit utilisateur prévu après cette mise à jour)

### Ce qu’on a fait
- [x] **Revue `PlantDefinition`** : footprint en `Vector2Int[]` (offsets relatifs), défaut `(0,0)` ; `GetOccupiedCells(origin)` pour projeter l’origine de pose sur les cellules absolues ; `OnValidate` pour imposer la présence de `(0,0)` dans le footprint.
- [x] **Alignement avec la spec** : comportement conforme à l’intention décrite dans `Notes/Farm/SPEC_plant_footprint_prompt.md` (prochaine étape côté code : grille + `BuildManager` / service qui consomme `GetOccupiedCells`).
- [x] **Documentation** : création de `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` — fonctionnement de `GetOccupiedCells`, pseudo-appel depuis un `BuildManager` (`CanPlace` / `Occupy`), exemple **footprint 2×2** (salade), et rappel pour la session suivante sur le **dédoublonnage** des cellules.

### Points d’attention (prochaine session avec l’assistant)
1. **Dédoublonnage** : expliquer en détail pourquoi et comment éviter les **offsets dupliqués** dans `footprint` (effets sur `Occupy` / compteurs), et quelles options d’implémentation (HashSet, normalisation dans `OnValidate`, API distincte).
2. **`GetOccupiedCells` + `BuildManager`** : reprendre le guide `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` si besoin ; câbler la **vraie** grille (`IsFree` / `Occupy` / `Release`) et trancher la **convention d’axes** (X/Y) pour les offsets 2×2 et suivants.

### Prochaines actions (priorité)
1. **Commit** par l’utilisateur : `PROJECT_LOG.md`, `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, et tout autre fichier déjà prêt dans le working tree.
2. Implémenter le **module grille** + premier **placement** utilisant `PlantDefinition.GetOccupiedCells`.
3. Poursuivre les tâches **Prochaine session** dans `Notes/Todo_project.md` (sprites transparence, etc.).

### Liens utiles
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`
- `Notes/Farm/SPEC_plant_footprint_prompt.md`

## 2026-04-03
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : 6000.3.x
- Branche : `main` (push utilisateur prévu après cette mise à jour)

### Ce qu’on a fait
- [x] **Grille modulable** : extension de `GridConfig` (cases **carrées** via `cellSize` ou **rectangulaires** via `cellWidth` / `cellHeight` + `uniformCellSize`).
- [x] **`GridManager`** : mise en page soit depuis un **ScriptableObject** `GridConfig`, soit **par instance** (colonnes, lignes, taille de cellule) pour prefabs type zone de culture / biofiltre ; origine monde = **`GridConfig.origin`** ou **`transform` + offset** (`originFromTransform`).
- [x] **Décision design** : **pas de rotation des cultures** pour le prototype (un seul footprint par plante, pas de retournement horizontal des assets) ; cases « vides » = règles de placement / forme de zone.
- [x] **Documentation / suite** : entrée de journal (cette section) ; note d’enchaînement **`Notes/Farm/TODO_plantation_pipeline.md`** (prefab grille → UI plantation → BuildManager) ; mise à jour du hub **`Notes/Todo_project.md`**.

### Problèmes rencontrés / pistes
- Première grille en scène : caler **taille de cellule** et **origine** sur les sprites (itération visuelle) ; les gizmos du `GridManager` aident au réglage.

### Décisions
- **Ordre d’implémentation plantation** : (1) prefab de base avec `GridManager` ; (2) **UI de sélection de graine en premier** pour figer la référence `PlantDefinition` / footprint côté joueur et pour le fantôme ; (3) **`BuildManager`** (ou service équivalent) consommateur de `GetOccupiedCells` + grille. Détail : `Notes/Farm/TODO_plantation_pipeline.md`.

### Prochaines actions (priorité)
1. **Prefab « base plantation »** : GameObject + `GridManager` (mode instance recommandé pour prototyper), dimensions et `instanceCellSize` / origine ; optionnel collider 2D sur la zone pour futurs raycasts.
2. **UI plantation** : sélection de graine (`PlantDefinition`), affichage ou rappel du footprint (icône / grille miniature / texte) selon le niveau de polish souhaité.
3. **`BuildManager` / placement** : `WorldToGrid`, `CanPlace` / `OccupyCells`, preview semi-transparent, clic pour instancier — voir `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`.

### Liens utiles
- `Assets/Scripts/Farm/GridManager.cs`
- `Assets/Scripts/Data/GridConfig.cs`
- `Assets/Scripts/Data/GridData.cs`
- `Notes/Farm/TODO_plantation_pipeline.md`
- `Notes/Todo_project.md`

## 2026-04-07
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : 6000.3.x
- Branche : `main`

### Ce qu’on a fait
- [x] **Pipeline plantation complet (Bezi / éditeur)** : le rôle prévu pour un futur `BuildManager` est assumé par **`BiofiltreManager`** + **`PlantPlacementPreview`** (pas de classe nommée `BuildManager` dans le projet).
- [x] **`BiofiltreManager`** : pont grille ↔ UI ; écoute les clics sur cellules via **`BiofiltreGridVisualizer`** ; ouvre **`SeedSelectionUI`** sur cellule libre ; `CanPlace(anchor, PlantDefinition)` via `GetOccupiedCells` + `GridManager.AreAllCellsFree` ; **`PlantSeedAt`** : instanciation du prefab sous le conteneur plantes, `PlantGrow` → stade `Graine`, **`OccupyCells`** + mise à jour visuelle des **`BiofiltreCell`** touchées.
- [x] **`PlantPlacementPreview`** : fantôme semi-transparent collé à la grille (Input System souris) ; teinte vert / rouge selon validité du footprint ; clic gauche confirme, clic droit / **Escape** annulent.
- [x] **`SeedSelectionUI`** + **`SeedSlotUI`** : panneau de graines (`SeedEntry` définition + prefab), activation conditionnelle des slots selon `CanPlace` à l’ancre de la cellule cliquée ; lancement du preview à la sélection (repli possible sans preview si non assigné).
- [x] **`BiofiltreGridVisualizer`** : génère les **`BiofiltreCell`** (collider 2D, `IPointerClickHandler`) alignées sur **`GridManager`** ; expose un conteneur pour les instances de plantes.
- [x] **`BiofiltreCell`** : cellule cliquable, coordonnées grille, états visuels vide / occupé.
- [x] **`GridLinesRenderer`** (optionnel) : rendu de lignes de grille sur le même objet que `GridManager`.

### Décisions / nomenclature
- Le **service de placement** est **scindé** : logique métier grille + pose dans **`BiofiltreManager`**, interaction souris + fantôme dans **`PlantPlacementPreview`**. Les guides qui parlent encore de `BuildManager` restent valables conceptuellement (`CanPlace` / `Occupy` / preview).

### Problèmes rencontrés / pistes
- Aucun blocage noté dans cette entrée ; **prochaine doc** : formaliser le **workflow d’ajout d’une nouvelle plante** (asset `PlantDefinition`, footprint, prefab, entrées UI).

### Prochaines actions (priorité)
1. **Rédiger la documentation** : workflow « ajout de nouvelles plantes » (références : `PlantDefinition`, `SeedSelectionUI` / `SeedEntry`, prefab avec `PlantGrow`, règles footprint — voir `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`).
2. Enchaîner sur le **prototype** : récolte ↔ inventaire, timers de croissance, ou maturité biofiltre / progression selon `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`.

### Liens utiles
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Scripts/Farm/PlantPlacementPreview.cs`
- `Assets/Scripts/Farm/BiofiltreGridVisualizer.cs`
- `Assets/Scripts/Farm/BiofiltreCell.cs`
- `Assets/Scripts/UI/SeedSelectionUI.cs`
- `Assets/Scripts/UI/SeedSlotUI.cs`
- `Assets/Scripts/Farm/GridLinesRenderer.cs`
- `Notes/Farm/TODO_plantation_pipeline.md`
- `Notes/Todo_project.md`

## 2026-04-09
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : 6000.3.x
- Branche : `main` (fichiers inventaire / ferme en cours d’intégration scène)

### Ce qu’on a fait
- [x] **Système d’inventaire (code)** : couche données + runtime + UI de base, **non validé en jeu** (pas de scénario de test bout-en-bout sur la laitue / prefab plante).
  - **Données** : `ItemDefinition` (id, nom, icône, `maxStack`), `ItemDatabase` (résolution par `itemId`).
  - **Runtime** : `PlayerInventory` (`TryAdd` / `TryRemove` / `Count` / `HasSpaceFor`, résultats `InventoryResult` incluant ajout partiel), `InventorySlot`.
  - **UI** : `InventoryUI` + `InventorySlotUI`, `InventoryFeedbackUI` (ex. inventaire plein).
  - **Pont ferme** : `PlantHarvestInteractor` sur la plante (`Collider2D`, `PlantGrow`) — clic souris (`OnMouseDown`), résolution de l’item via `PlantDefinition.harvestItemId` ou override, appel `PlayerInventory.TryAdd`.
- [x] **Support placement → récolte** : `PlantDefinitionHolder` (définition posée par `BiofiltreManager` à l’instanciation) pour que la récolte lise `HarvestStage` et `harvestItemId` sans coupler au pipeline de pose.

### Problèmes rencontrés / pistes
- **Récolte** : le pipeline prévoit **deux moments** de récolte possibles sur un cycle (ex. profil **Leafy** : récolte **Mature** puis cycle **Flowering → Seedling** pour graines) ; `PlantDefinition` n’expose aujourd’hui qu’un seul `harvestStage` + un `harvestItemId`. `PlantHarvestInteractor.OnHarvestSuccess` est un **placeholder** : pas d’avancement de stade ni de **verrou** d’état (risque de double-clic / récolte hors design).
- **Tests** : aucun test automatisé ni check-list scène documentée pour valider ajout d’item, UI, et échec « inventaire plein » sur la salade.

### Décisions / suite
- **Prochaine session** : (1) **implémenter et tester** l’inventaire en conditions réelles (assets laitue : `PlantDefinition`, entrées `ItemDatabase`, composants sur le prefab plante / joueur / canvas) ; (2) **verrouiller** le comportement de récolte (une fois récolté, transition d’état ou compteur `maxHarvestCount`) ; (3) **refactor** à envisager pour distinguer clairement **récolte « corps de récolte »** (feuilles / fruit au stade configuré) vs **récolte graines** (ex. `Seedling`) — données (`itemId`, stade, quantités min/max) + un seul interactor ou stratégie par type de récolte.
- **Documentation** : carte des systèmes existants (plantation, croissance, récolte, inventaire) — voir `Notes/Farm/SYSTEMES_carte_mentale.md`.

### Liens utiles
- `Assets/Scripts/Inventory/`
- `Assets/Scripts/UI/Inventory/`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`
- `Assets/Scripts/Farm/PlantDefinitionHolder.cs`
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Notes/Farm/SYSTEMES_carte_mentale.md`
- `Notes/Todo_project.md`