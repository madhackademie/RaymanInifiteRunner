# Architecture cible — HUD / UI Manager en scène additive

## État au 2026-04-19 (implémenté dans le dépôt)

- **`Bootstrap.unity`** : première scène du **Build Settings** ; **`GameBootstrap`** + **`LoadingScreen`**. Illustration load — **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**.
- **`GameBootstrap`** : charge en **additif** **`NavigationHUD`**, puis **`HomeScene`** (plus **`FirstLvl`** direct au boot) ; barre **`allowSceneActivation`** sur l’`AsyncOperation` ; **`SceneNavigator.Instance.SetInitialScene(HomeScene)`**.
- **`SceneNavigator`** (`Assets/Scripts/Systems/SceneNavigator.cs`) : singleton **`DontDestroyOnLoad`** ; **`GoTo(sceneName)`** = **`LoadSceneAsync(..., Additive)`** puis **`UnloadSceneAsync(previous)`** (une seule scène de **contenu** à la fois ; le shell n’est pas déchargé). Événements **`OnBeforeSceneLoad`** / **`OnAfterSceneLoad`**.
- **`SceneId`** (dans `ScreenId.cs`) : noms de scènes (`HomeScene`, `Inventaire`, `FirstLvl`, `Map`, …).
- **`NavigationHUD.unity`** : scène shell — **`NavigationHUD`**, **`UIManager`**, **`SceneNavigator`**, **`EventSystem`** (unicité à maintenir).
- **`UIManager`** : prefabs globaux (`ShowScreen` / `HideScreen` / …) — inchangé dans le principe.
- **`NavigationHUD`** : onglets appellent **`SceneNavigator.GoTo`** (**`HomeScene`**, **`Inventaire`**) ; en **`ShowExitOnly`**, **`OnExitClicked`** déclenche **`OnExitToHomeRequested`** (implémenté côté **`FirstLvlController`** → retour hub).
- **`HomeScene`** + **`MapSceneController`** : hub d’entrées (`MapNodeData`) vers les scènes de jeu ; progression **`MapProgressionData`**.
- **`PlayerInventory`** : **`DontDestroyOnLoad`**.
- **`Inventaire.unity`** : peut coexister comme **scène de contenu** (onglet HUD) en parallèle du prefab inventaire **`UIManager`** — clarifier le double chemin si besoin.

## Problème restant (prochain chantier)

- **Debug / durcissement** : enchaînements **`SceneNavigator`** (timings async, scènes manquantes au build, transitions concurrentes), cohérence HUD + **`UIManager`** lors des changements de scène.
- **Persistance** : grille / cultures non sauvegardées automatiquement à la fermeture — voir **`Notes/Todo_project.md`** (ScriptableObject + save).
- **Temps de croissance** : logique encore centrée scène — recalcul **hors scène / offline** à spécifier (UTC) ; cloud ultérieur.

## Constat historique (avant Bootstrap — archivé)

- Ancien flux possible : `MainMenuUI` → `LoadScene("FirstLvl")` sans shell ; `InventorySceneController` + navigation additive partielle vers **`Inventaire.unity`**.

## Direction recommandée

### 1. Scène additive persistante pour l'UI shell

Créer une couche UI persistante chargée une seule fois :

- `NavigationHUD.unity` est la scène shell UI globale,
- elle contient :
  - le HUD de navigation,
  - le **`UIManager`**,
  - l'unique `EventSystem`,
  - le **`screenRoot`** pour instancier les prefabs d’écrans.

Cette scène doit être chargée avant ou en même temps que la première scène gameplay.

## 2. Rôle du `UIManager`

Le `UIManager` ne porte pas la logique métier de chaque écran. Il orchestre :

- l’instanciation / préchargement des **prefabs** d’écrans (listes prioritaire + secondaire),
- l'activation/désactivation des roots UI déjà instanciés (`SetActive`),
- la navigation entre écrans globaux (par `screenId`),
- (optionnel futur) le mode d'affichage du HUD selon la scène active — aujourd’hui partiellement piloté par **`NavigationHUD`** + appels statiques.

Responsabilités présentes ou prévues :

- `EnsureShellLoaded()` (statique)
- `PreloadPriorityScreens()` / `PreloadScreenLazy(screenId)`
- `ShowScreen(screenId)` / `HideScreen(screenId)` / `HideAllGlobalUI()`
- (évolution) `ShowGameplayHUD()` / `ShowOverlayHUD()` si on extrait tout le mode HUD du MonoBehaviour `NavigationHUD`

## 3. Modèle conseillé pour la navigation instantanée

Pour les écrans que le joueur ouvre souvent (**inventaire**, plus tard **market**) :

1. **instancier une fois** le prefab (ou charger une scène additive légère une fois — variante),
2. récupérer son root UI principal,
3. le laisser en mémoire,
4. utiliser `SetActive(true/false)` sur ce root pour afficher/masquer l'écran.

Pourquoi :

- ouverture quasi instantanée,
- pas de `LoadScene` à chaque clic,
- pas de perte de refs runtime à chaque fermeture,
- architecture simple pour le prototype.

## 4. Répartition recommandée des responsabilités

### `MainMenuUI`

- ne devrait plus charger directement toute la navigation UI,
- devrait demander à un bootstrap ou scene loader d'ouvrir le shell + la scène gameplay cible.

### `NavigationHUD`

- vue HUD (modes d’affichage),
- pour l’inventaire : délégation à **`UIManager`** (plus de chargement scène inventaire dans ce flux),
- **navigation monde** : déléguée à **`SceneNavigator`** ; le hub cible est **`HomeScene`** (l’ancienne cible **`Carte`** dans les notes = rôle d’accueil, aujourd’hui **`HomeScene`** / future **`Map`**).

### `InventorySceneController`

- garde la logique locale de binding (`InventoryUI.Bind(PlayerInventory.Instance)`),
- mais l'ouverture/fermeture de l'écran devrait être déclenchée par le manager global.

## 5. Séquence de démarrage actuelle (Bootstrap + hub)

1. `Bootstrap` → `GameBootstrap.Awake`
2. chargement additif **`NavigationHUD`** (shell + `UIManager` + **`SceneNavigator`** + `EventSystem`)
3. chargement additif **`HomeScene`** ; `SceneNavigator.SetInitialScene(HomeScene)`
4. `MapSceneController` (Start) : `UIManager.EnsureShellLoaded()`, `NavigationHUD.ShowNavBar()`
5. `UIManager` → `PreloadPriorityScreens()` (prefabs, dont inventaire)
6. Le joueur lance **`FirstLvl`** (ou autre) depuis le hub → **`SceneNavigator.GoTo`** remplace la scène de **contenu** précédente par unload async

## 6. Points d'attention

- un seul `EventSystem` entre toutes les scènes additives,
- éviter de mélanger chargement de scènes et logique visuelle dans `NavigationHUD`,
- garder `BiofiltreManager` indépendant du HUD global,
- documenter l'ordre d'initialisation si plusieurs singletons persistent,
- `NavigationHUD.unity` est au build ; conserver une seule source de vérité pour l’ordre des scènes dans **Build Settings**.

## 7. Découpage d'implémentation (état)

### Etape 1 — sécuriser le shell UI — **fait**

### Etape 2 — `UIManager` — **fait** (prefabs + `ScreenId`)

### Etape 3 — inventaire préchargé — **fait côté prefab** ; scène `Inventaire.unity` à clarifier / retirer du build si obsolete

### Etape 4 — généraliser

- appliquer le même pattern à `Market`,
- stabiliser **`SceneNavigator`** + UX retours (toutes scènes de contenu),
- scène **`Map`** (navigation spatiale) si le design la sépare de **`HomeScene`**,
- ajouter plus tard overlays, transitions et états UI globaux.

## 8. Décision de travail actuelle + suite

Base actuelle :

- `NavigationHUD.unity` comme scène shell additive,
- `UIManager` dans `Scripts/Systems` avec **prefabs** et listes prioritaire / secondaire,
- navigation instantanée via `SetActive` sur les instances.

**Prochaine décision produit / tech** : finaliser le **playtest** navigation (**§6**) + **persistance** ferme / grille + **temps** hors scène — voir **`Notes/Todo_project.md`**.

## 9. Prompt simple pour Bezi

```text
Créer une UI globale persistante partagée entre toutes les scènes du projet Unity.
Utiliser `NavigationHUD.unity` comme scène shell UI additive.
Créer un `UIManager` global qui précharge les scènes UI fréquentes en additif au démarrage, en particulier `Inventaire`, puis affiche/masque leurs roots avec `SetActive` pour rendre la navigation quasi instantanée.
Garder un seul `EventSystem`, laisser `NavigationHUD` comme vue HUD, et déplacer la logique de navigation globale dans le `UIManager`.
```

## 10. Suite — hub **`HomeScene`**, retour gameplay, extensions

1. **`HomeScene`** sert de **hub** après bootstrap ; enrichir les **`MapNodeData`** / déverrouillages (`MapProgressionData`).
2. Retour **`FirstLvl` → `HomeScene`** : **`OnExitToHomeRequested`** + **`FirstLvlController`** (déjà orienté **`SceneNavigator`**) — valider en jeu tous les cas (inventaire ouvert, transitions async).
3. **`Map.unity`** : à brancher quand la carte monde sera distincte de l’écran d’accueil.
4. Mettre à jour **`Notes/Ui/Todo_ui.md`** après chaque changement de flux d’unload/load.
