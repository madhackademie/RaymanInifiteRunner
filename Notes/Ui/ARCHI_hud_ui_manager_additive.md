# Architecture cible — HUD / UI Manager en scène additive

## État au 2026-04-16 (implémenté dans le dépôt)

- **`Bootstrap.unity`** : première scène du **Build Settings** ; contient **`GameBootstrap`** + **`LoadingScreen`**. *2026-04-17* : intégration illustration load — **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**.
- **`GameBootstrap`** (`Assets/Scripts/Core/GameBootstrap.cs`) : charge en **additif** **`NavigationHUD`** puis **`FirstLvl`**, avec barre de progression sur l’`AsyncOperation`, puis masque le loading.
- **`NavigationHUD.unity`** : **dans le build** ; scène shell avec **`NavigationHUD`** + **`UIManager`** + **`EventSystem`** unique (à vérifier dans l’asset scène).
- **`UIManager`** (`Assets/Scripts/Systems/UIManager.cs`) : singleton **`DontDestroyOnLoad`**, registre d’écrans par **`screenId`**, listes **prioritaires** / **secondaires** de **`ScreenEntry`** (prefab → `Instantiate` sous `screenRoot`, puis **`SetActive`**). API : `PreloadPriorityScreens`, `PreloadScreenLazy`, `ShowScreen`, `HideScreen`, `HideAllGlobalUI`, `EnsureShellLoaded()` (charge le shell en additif si `Instance` absent).
- **`ScreenId`** : constantes d’ids (ex. **`Inventory`**).
- **`NavigationHUD`** : vue HUD (`ShowNavBar` / `ShowExitOnly` / `Hide`), **`DontDestroyOnLoad`** ; onglets délégués à **`UIManager`** pour l’inventaire prefab.
- **`PlayerInventory`** : toujours **`DontDestroyOnLoad`** — source de données transverse pour l’UI globale.
- **`Inventaire.unity`** : peut encore exister au build (legacy / tests) ; à trancher si tout l’inventaire passe par le prefab **`UIManager`**.

## Problème restant (prochain chantier)

- Pas encore de **scène hub `Carte`** pour centraliser la navigation **multi-scènes** avec le HUD dans le mode voulu sur ce hub.
- Le bouton **croix** depuis **`FirstLvl`** (**`OnExitClicked`**) ne charge pas encore explicitement **`Carte`** — il masque seulement l’UI globale et repasse en **`ShowExitOnly()`**.
- À documenter après choix : **unload** du niveau vs **pile additive**, sans **double `EventSystem`** ni fuites de scènes.

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
- **à étendre** : décision de **navigation monde** (ex. **`FirstLvl` → `Carte`**) — idéalement via un petit **`GameFlow` / `SceneNavigator`** pour ne pas alourdir le HUD.

### `InventorySceneController`

- garde la logique locale de binding (`InventoryUI.Bind(PlayerInventory.Instance)`),
- mais l'ouverture/fermeture de l'écran devrait être déclenchée par le manager global.

## 5. Séquence de démarrage actuelle vs cible hub **Carte**

**Actuel (Bootstrap)** :

1. `Bootstrap` active → `GameBootstrap.Awake`
2. chargement additif **`NavigationHUD`** (shell + `UIManager` + `EventSystem`)
3. `NavigationHUD.ShowExitOnly()` pour le gameplay plein écran
4. chargement additif **`FirstLvl`**
5. `UIManager.Start` → `PreloadPriorityScreens()` (prefabs inventaire, etc.)

**Cible prochaine** : insérer **`Carte`** comme hub après le shell (et charger **`FirstLvl`** depuis le hub plutôt que depuis `GameBootstrap`, selon design) — voir §10.

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
- hub **`Carte`** + navigation monde (croix → Carte),
- ajouter plus tard overlays, transitions et états UI globaux.

## 8. Décision de travail actuelle + suite

Base actuelle :

- `NavigationHUD.unity` comme scène shell additive,
- `UIManager` dans `Scripts/Systems` avec **prefabs** et listes prioritaire / secondaire,
- navigation instantanée via `SetActive` sur les instances.

**Prochaine décision produit / tech** : scène hub **`Carte`** + comportement **croix `FirstLvl` → `Carte`** (voir §10).

## 9. Prompt simple pour Bezi

```text
Créer une UI globale persistante partagée entre toutes les scènes du projet Unity.
Utiliser `NavigationHUD.unity` comme scène shell UI additive.
Créer un `UIManager` global qui précharge les scènes UI fréquentes en additif au démarrage, en particulier `Inventaire`, puis affiche/masque leurs roots avec `SetActive` pour rendre la navigation quasi instantanée.
Garder un seul `EventSystem`, laisser `NavigationHUD` comme vue HUD, et déplacer la logique de navigation globale dans le `UIManager`.
```

## 10. Prochaine étape — scène hub **`Carte`** + retour depuis le niveau

1. Créer **`Carte.unity`** : UI de sélection des destinations (multi-scènes / modes).
2. Après chargement du shell, charger **`Carte`** (au lieu de **`FirstLvl`** directement dans `GameBootstrap`, si le flux le veut) **ou** garder `FirstLvl` au boot mais garantir que la croix ramène toujours à **`Carte`** selon le game design.
3. Sur **`Carte`** : appeler **`NavigationHUD.ShowNavBar()`** (ou équivalent) pour que le **HUD persistant** reflète le contexte « menu de navigation ».
4. Implémenter **`FirstLvl` → `Carte`** sur **`OnExitClicked`** : décharger le niveau si nécessaire, charger **`Carte`**, sans dupliquer **`EventSystem`**.
5. Mettre à jour **`Notes/Ui/Todo_ui.md`** et ce fichier une fois le flux d’unload/load figé.
