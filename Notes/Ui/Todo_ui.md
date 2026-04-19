# TODO UI — backlog

**Navigation scènes / Inventaire / Market (HUD global, Additive, sync-async)** : `GUIDE_scenes_navigation_Unity_inventaire_market.md`.

**Architecture shell + `UIManager` (référence)** : `ARCHI_hud_ui_manager_additive.md`.

---

## Priorité session suivante

### Debug / amélioration — **navigation additive + unload async** (2026-04-19)

- [ ] Repasser tous les chemins **`SceneNavigator.GoTo`** (chargement additif → **`UnloadSceneAsync`** sur la scène de contenu précédente) : scène inexistante, double clic, transition déjà en cours, cohérence **`CurrentScene`** avec **`SetInitialScene`** après bootstrap.
- [ ] Vérifier l’interaction **HUD** (`NavigationHUD` modes, onglets **`HomeScene`** / **`Inventaire`**, **`OnExitToHomeRequested`** + **`FirstLvlController`**) et **`MapSceneController`** (chargement depuis le hub, unload **`HomeScene`** si applicable).
- [ ] Playtest **Build Settings** + absence de **double `EventSystem`** ; après stabilisation, mettre à jour **`Notes/Ui/ARCHI_hud_ui_manager_additive.md`** (séquence réelle et pièges async).

---

### Focus auteur — **illustration LoadingScreen** (2026-04-17)

- **Objectif** : image finale (référence projet : **poisson + arbre**) + import Unity + placement dans **`Assets/Scenes/Bootstrap.unity`** sans régression barre / fade.
- **Guide** : **`Notes/Ui/LOADINGSCREEN_image_workflow.md`** (hiérarchie Canvas, import sprite, QA).
- Après intégration : cocher la case QA dans **`Notes/Todo_project.md`** (*Tests écran de chargement*).

---

### Hub **`HomeScene`** + HUD persistant + retour depuis **`FirstLvl`** (suite chantier)

**État (2026-04-19)** : le hub d’accueil est la scène **`HomeScene`** (`MapSceneController`, `SceneId.HomeScene`) ; **`GameBootstrap`** charge **`NavigationHUD`** puis **`HomeScene`** ; les onglets HUD utilisent **`SceneNavigator.GoTo`** vers **`HomeScene`** / **`Inventaire`**. Le retour depuis le gameplay passe par **`OnExitToHomeRequested`** + **`FirstLvlController`** → **`GoTo(HomeScene)`**.

Tâches restantes / alignement doc :

- [ ] Finaliser le flux **hub → `FirstLvl`** / autres nœuds ( **`MapNodeData`** ) et l’**unload** de **`HomeScene`** quand pertinent (`MapSceneController` — à valider en jeu).
- [ ] Harmoniser la doc ancienne « **`Carte`** » avec **`HomeScene`** / future **`Map`** dans **`ARCHI_hud_ui_manager_additive.md`** et guides.
- [ ] Revue **`Inventaire.unity`** : au **Build Settings** si encore utilisée comme scène de contenu ; cohabitation avec prefab **`ScreenId.Inventory`** via **`UIManager`**.

---

## Bootstrap & **LoadingScreen** — tests + visuel

- [ ] **Tests scène de chargement** : playtest **Editor** + idéalement **build dev** de **`Bootstrap.unity`** — vérifier barre de progression (`AsyncOperation` 0.9), ordre **NavigationHUD** → **FirstLvl**, absence de **double `EventSystem`**, fade-out **`LoadingScreen.Hide()`**, pas de frame « flash » UI avant la fin du chargement.
- [ ] **Image poisson / arbre (loading)** : **création** puis **affinage** d’une illustration **poisson + arbre** pour l’écran de chargement (référence de travail / ton : itérations type **« chatgptouille »** — génération + retouches jusqu’à un rendu acceptable) ; importer dans Unity (`Sprite` / `Texture2D` + alpha si besoin) et **câbler** sur **`LoadingScreen`** (Image / `RawImage` selon le setup actuel).

---

## État implémenté (rappel — ne pas re-planifier en doublon)

- [x] **`Bootstrap.unity`** en entrée + **`GameBootstrap`** + **`LoadingScreen`** (progression, chargement additif **`NavigationHUD`** puis **`HomeScene`** — plus **`FirstLvl`** direct au boot).
- [x] **`SceneNavigator`** + **`SceneId`** : transition de **scène de contenu** unique (additif + unload async de la précédente), shell non déchargé.
- [x] **`UIManager`** (`Assets/Scripts/Systems/UIManager.cs`) : listes **prioritaires** / **secondaires**, prefabs, `ShowScreen` / `HideScreen` / `HideAllGlobalUI`, `EnsureShellLoaded()`.
- [x] **`ScreenId`** + au minimum écran **Inventory** en prefab.
- [x] **`NavigationHUD.unity`** dans le build ; shell + un seul **`EventSystem`**.

---

## HUD / UI Manager — historique & prompt Bezi

Le prompt ci-dessous décrit encore le modèle « préchargement de **scènes** UI » ; le code actuel privilégie des **prefabs** sous `UIManager` (même intention UX : instantané après boot). Adapter le prompt si tu redélègues à un outil externe.

```text
Créer une UI globale persistante partagée entre toutes les scènes du projet Unity.
Utiliser `NavigationHUD.unity` comme scène shell UI additive.
Créer un `UIManager` global qui précharge les scènes UI fréquentes en additif au démarrage, en particulier `Inventaire`, puis affiche/masque leurs roots avec `SetActive` pour rendre la navigation quasi instantanée.
Garder un seul `EventSystem`, laisser `NavigationHUD` comme vue HUD, et déplacer la logique de navigation globale dans le `UIManager`.
```

---

## LanguageManager / TextMeshPro (transféré depuis `Decision_ui.md`)

### Décisions à respecter (rappel)
- `country` ≠ `language` : pays pour market/pub ; langue choisie par le joueur (override).
- Au boot : `language = defaultLanguageFromCountry(country)` sauf si override joueur déjà sauvegardé.
- Changement d’option langue → `OnLanguageChanged` → mise à jour de tous les TMP localisés.

### Tâches
- [ ] Définir l’énum / identifiants `Language` et `Country` (ou strings normalisées) selon le besoin prototype.
- [ ] Implémenter `defaultLanguageFromCountry(country)` (table de mapping).
- [ ] Persister le choix joueur (`language` override) — au minimum PlayerPrefs en proto, save plus tard si besoin.
- [ ] Créer un `LanguageManager` (singleton ou service injecté) avec :
  - [ ] propriété `CurrentLanguage`
  - [ ] événement `OnLanguageChanged(Language)`
  - [ ] méthode `GetText(string key)` avec **fallback** (langue courante → EN → afficher la key brute si absent).
- [ ] Créer un composant `LocalizedTMPText` sur les `TextMeshProUGUI` :
  - [ ] champ `key` (string stable)
  - [ ] abonnement à `OnLanguageChanged` + refresh au `OnEnable`.
- [ ] Documenter la **convention de keys** (`BTN_PLAY`, `TITLE_FARM`, etc.) dans `Spec_ui.md` ou ici.
- [ ] **Prototype** : table `language → key → string` (en dur ou ScriptableObject minimal).
- [ ] **Final** : migrer vers ScriptableObject ou JSON (même schéma clé/valeur).
- [ ] **DoD** : changer la langue dans les options met à jour **tous** les textes TMP localisés **sans** recréer la scène.

### Hors scope immédiat (référence)
- Intégration détection pays “réelle” (SDK / store) — peut rester mockée jusqu’au besoin market.
