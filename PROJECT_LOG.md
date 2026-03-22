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