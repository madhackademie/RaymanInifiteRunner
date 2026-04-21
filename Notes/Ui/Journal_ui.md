# UI Journal (Stack + Localization)

But: centraliser toutes les décisions, idées et futures tâches liées à l’UI (panneaux, animations, localisation TextMeshPro).

Règle de fonctionnement :
- Tu peux ajouter des idées en vrac dans `## Inbox (idées brutes)`
- Quand on revient dessus, je les réorganise en `## Tâches (planifiées)` et je mets à jour `## Décisions actées` / `## Questions ouvertes`.

---

## Décisions actées

### 1) Animation UI
- On démarre avec une UI simple (prototype) puis on enrichit en “polish”.
- On privilégie `Animator` (cinématiques via fade + slide) pour l’UI et les transitions de panneaux.
- `Cinemachine` est utilisé pour la caméra (les séquences UI/monde/caméra seront orchestrées plus tard).

### 2) Structure des panneaux (profondeur 2-3 layers)
- Modèle recommandé : un panneau de base + 1..2 overlays par thème (stack).
- Règle d’interaction : seul le panneau “au-dessus” reçoit les clics/raycast (les overlays sont non-interactifs quand ils ne sont pas top).

### 3) Séparation Country vs Language
- `country` sert pour les mécanismes liés à la région (publicité, market, etc.) via détection locale.
- `language` sert pour les textes UI (TextMeshPro) et est choisi par le joueur.
- Langue par défaut : au départ, `language = languageFromCountry(country)` puis l’override joueur prend le dessus.

### 4) Localization UI avec TextMeshPro
- Le mécanisme sera déclenché quand le joueur change l’option de langue.
- Le TextMeshPro affiché doit être piloté par une logique centrale (language manager + mapping key -> string).
- Idéalement : utiliser des “keys” stables côté UI pour faciliter le passage du prototype au manager final.

### 5) UI globale partagée entre toutes les scènes
- Direction retenue : charger une UI shell globale persistante pour toutes les scènes de jeu.
- **Implémenté** : scène **`Bootstrap.unity`** (index 0) + **`GameBootstrap`** charge additivement **`NavigationHUD`** puis **`HomeScene`** (mise à jour **2026-04-19** ; avant : **`FirstLvl`** direct) avec **`LoadingScreen`** ; **`UIManager`** + **`SceneNavigator`** dans le shell ; écrans **prefabs** + **`EnsureShellLoaded()`**.
- Variante « scènes UI `.unity` en additif » : encore possible pour certains écrans ; l’inventaire gameplay peut transiter entièrement par prefab + `ScreenId.Inventory` — vérifier la dépréciation de **`Inventaire.unity`** au build si plus utilisée.
- L’expérience cible reste : boot un peu plus lourd, navigation quasi instantanée entre panneaux préchargés.
- Un seul `EventSystem` dans le shell ; pas de second `EventSystem` dans les scènes chargées par-dessus.
- Un prompt simple et court reste dans **`Todo_ui.md`** / **`ARCHI_hud_ui_manager_additive.md`** (utile Bezi / rappel).
- La même architecture pourra aussi héberger des UI flottantes globales (popups de confirmation, notifications/toasts, fenêtres modales) via le `UIManager`, sans coupler ces éléments à une scène métier.

### 6) Hub multi-scènes — **`HomeScene`** (ex-« Carte ») + retour depuis le niveau
- **Implémenté (2026-04-19)** : le hub d’accueil est la scène **`HomeScene`** (`MapSceneController`, progression **`MapProgressionData`**) ; le bootstrap charge **`NavigationHUD`** puis **`HomeScene`** ; les transitions de **scène de contenu** passent par **`SceneNavigator`** (additif + **`UnloadSceneAsync`**). La scène **`Map`** reste prévue dans **`SceneId`** pour une carte monde distincte si besoin.
- **Comportement gameplay** : depuis **`FirstLvl`**, la **croix** déclenche **`OnExitToHomeRequested`** → **`FirstLvlController`** → **`GoTo(HomeScene)`** (plus seulement masquer l’UI globale).
- **Suite** : **debug** des flux async, persistance ferme, recalcul temps de croissance hors scène — **`Notes/Todo_project.md`**, **`ARCHI_hud_ui_manager_additive.md`**.

---

## Architecture UI cible (haut niveau)

### Stack manager (2-3 layers)
- `UIManager` (ou `UIRootController`)
  - maintient la stack de panneaux ouverts
  - gère le “push/pop” des overlays
  - évite d’ouvrir/fermer plusieurs panneaux en conflit pendant une animation (optionnel mais utile)

- `UIPanel` (composant sur prefab)
  - référence `Animator` (clip Open/Close) et éventuellement `CanvasGroup`
  - expose des méthodes : `Open()`, `Close()`
  - expose des callbacks/événements “animation finished” (via Animation Events)

### Orchestration “séquence” (pour plus tard)
- Notion de “séquence” = suite d’étapes (UI / World / Camera)
- Pour ce journal : pour l’instant on se limite à UI, mais on prépare le modèle d’étapes pour synchroniser plus tard.

---

## LanguageManager (spécification fonctionnelle)

### Rôles
- Entrée : `SetLanguage(selectedLanguage)` déclenché par le menu d’options.
- Sortie : mise à jour de tous les `TextMeshProUGUI` localisés.

### Comportement
- Au boot :
  - détecter `country`
  - calculer `defaultLanguage = languageFromCountry(country)`
  - si une override joueur existe : utiliser `override`
  - sinon : utiliser `defaultLanguage`
- Quand le joueur change la langue :
  - enregistrer l’override (plus tard : PlayerPrefs/Save)
  - notifier les textes (event)

### Données (phase prototype -> phase manager)
- Phase prototype possible :
  - mapping rapide `language -> string` par key (ou switch)
- Phase manager final :
  - table de localisation par langue (ScriptableObject/JSON)
  - fallback (ex. EN si missing)

---

## Inbox (idées brutes)

- [ ] (Vrac) …
- [ ] (Vrac) …

---

## Tâches (planifiées)

### A) UI prototype (baseline)
1. [ ] Créer 1 panneau “base” (ex. Menu/Farm) en prefab.
2. [ ] Ajouter `Animator` + clips `Open`/`Close` (fade + slide).
3. [ ] Ajouter un `UIPanel` composant qui expose `Open/Close` et gère l’activation/raycast.

### B) Stack 2-3 layers
1. [ ] Implémenter `UIManager` qui supporte `push overlay` et `pop overlay`.
2. [ ] Mettre en place la règle “seul top reçoit les clics”.

### B bis) UI globale multi-scènes
1. [x] Créer une UI globale partagée entre toutes les scènes de jeu (shell + prefabs via `UIManager`).
2. [x] Utiliser `NavigationHUD.unity` comme scène shell UI additive (chargée depuis `Bootstrap` ou `EnsureShellLoaded`).
3. [x] Créer un `UIManager` global avec préchargement des **prefabs** prioritaires (équivalent fonctionnel au préchargement de scènes UI).
4. [~] Précharger les UI fréquentes : inventaire via prefab + `ScreenId.Inventory` ; ajouter `Market`, `Settings`, etc. quand existants.
5. [x] Navigation instantanée post-boot via `SetActive` sur les instances prefab.
6. [x] Un seul `EventSystem` dans le shell ; `NavigationHUD` + `UIManager` en `DontDestroyOnLoad` sur leurs racines respectives.
7. [x] Orchestration inventaire / masquage global déléguée à `UIManager` depuis `NavigationHUD` (plus de chargement scène inventaire dans le HUD pour ce flux).
8. [x] Boot `Bootstrap.unity` + `LoadingScreen` pour absorber le coût initial.

### B ter) Hub **Carte** + flux retour niveau (prochaine implémentation)
1. [ ] Créer la scène **`Carte`** et l’intégrer au **Build Settings** + ordre de flux (après Bootstrap / avec shell déjà chargé).
2. [ ] Sur le hub : navigation vers les scènes / modes voulus ; **HUD persistant** visible et cohérent (tabs, etc.).
3. [ ] Depuis **`FirstLvl`** : clic **croix** → transition vers **`Carte`** (définir unload `FirstLvl` vs stack additive).
4. [ ] Mettre à jour `GameBootstrap` si le premier écran « monde » n’est plus `FirstLvl` mais **`Carte`** (shell puis Carte, puis niveaux à la demande).
5. [ ] Nettoyer ou documenter **`Inventaire.unity`** si le build ne doit plus l’utiliser.

### C) Localization TextMeshPro
1. [ ] Définir une convention de keys stables pour les textes UI (ex. `BTN_PLAY`, `TITLE_FARM`).
2. [ ] Implémenter un mécanisme temporaire (prototype) qui met à jour les TMP quand `language` change.
3. [ ] Préparer le passage au `LanguageManager` final (source de données + fallback).

### D) Préparation “séquences” (plus tard)
1. [ ] Définir une API d’étapes (UI step, Camera step) pour synchroniser fade/slide + blend Cinemachine.

---

## Questions ouvertes

- [ ] Quels types de panneaux seront “modaux” (bloquent tout) vs “non-modaux” ?
- [ ] Combien de textes à localiser au démarrage (10 ? 50 ? 200) ?
- [ ] Faut-il un fallback EN ou fallback = langue du pays ?
- [ ] Où stocker `language override` (PlayerPrefs, save file, autre) ?

---

## Journal de changements (pour le futur)

Date | Changement
:---|:---
2026-04-21 | Plan **~2026-05-01** : audit **Bezi** + clean/refactor navigation (**`SceneNavigator.ShowScene`**, boot eager `GameBootstrap`, alignement doc vs anciennes mentions `GoTo`/`Unload`) — note **`Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`** ; snapshot **`Notes/Codebase_etat_reference.md`** mis à jour.
2026-04-16 | Décision ajoutée : viser une UI globale persistante multi-scènes avec préchargement additif des écrans UI fréquents et navigation instantanée post-boot.
2026-04-16 | Ajout d'un prompt simple à copier dans Bezi pour lancer le chantier `UIManager` global / shell UI additive.
2026-04-16 (fin) | Implémenté : `Bootstrap.unity` + `GameBootstrap` + `LoadingScreen`, `UIManager` (listes prioritaire/secondaire, prefabs), `ScreenId`, shell dans le build ; priorité suivante : scène hub **`Carte`**, croix **`FirstLvl` → Carte**.
2026-04-16+ | TODO ajouté : **tests** scène de load (`Bootstrap` / `LoadingScreen`) ; **création + affinage** illustration **poisson/arbre** pour l’écran de chargement (réf. de travail « chatgptouille ») puis branchement sur `LoadingScreen`.
2026-04-17 | Branche navigation / UI **créée** : retrait des rappels « créer la branche avant le chantier » dans **`Notes/Todo_project.md`** ; **prochaine session auteur** = visuel **`LoadingScreen`** ; nouveau guide **`Notes/Ui/LOADINGSCREEN_image_workflow.md`** ; journal **`PROJECT_LOG.md`**.

