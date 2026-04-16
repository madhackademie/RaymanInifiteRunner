# Architecture cible — HUD / UI Manager en scène additive

## Constat actuel

- `NavigationHUD` existe déjà et expose `ShowNavBar()`, `ShowExitOnly()` et `Hide()`.
- `NavigationHUD.unity` existe, mais n'est pas présente dans le build settings.
- `MainMenuUI` charge `FirstLvl` en `LoadScene("FirstLvl")` sans bootstrap HUD.
- `InventorySceneController` et `NavigationHUD` pilotent déjà une navigation additive partielle entre `FirstLvl` et `Inventaire`.
- `PlayerInventory` est persistant (`DontDestroyOnLoad`) et constitue une bonne source de données transverse pour l'UI globale.

## Problème à corriger

Le projet n'a pas encore de point d'entrée unique pour :

- charger les scènes globales au démarrage,
- garantir qu'un seul `EventSystem` pilote l'UI,
- garder le HUD disponible quelle que soit la scène gameplay,
- ouvrir/fermer les écrans UI instantanément sans recharger leur scène à chaque clic.

## Direction recommandée

### 1. Scène additive persistante pour l'UI shell

Créer une couche UI persistante chargée une seule fois :

- `NavigationHUD.unity` devient la scène shell UI globale,
- elle contient :
  - le HUD de navigation,
  - un futur `UIManager`,
  - l'unique `EventSystem`,
  - les roots UI globaux communs.

Cette scène doit être chargée avant ou en même temps que la première scène gameplay.

## 2. Rôle du futur `UIManager`

Le `UIManager` ne doit pas porter la logique métier de chaque écran. Il orchestre :

- le chargement additif initial des scènes UI,
- l'activation/désactivation des roots UI déjà chargés,
- la navigation entre écrans globaux,
- le mode d'affichage du HUD selon la scène active,
- les transitions simples si besoin plus tard.

Responsabilités conseillées :

- `EnsureShellLoaded()`
- `PreloadUIScreen(sceneName)`
- `ShowScreen(screenId)`
- `HideScreen(screenId)`
- `ShowGameplayHUD()`
- `ShowOverlayHUD()`
- `HideAllGlobalUI()`

## 3. Modèle conseillé pour la navigation instantanée

Pour les écrans que le joueur ouvre souvent (`Inventaire`, plus tard `Market`) :

1. charger la scène additive une seule fois,
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

- doit devenir une vue HUD pure,
- ne devrait pas décider seule quelles scènes charger/décharger,
- ses boutons devraient déléguer au futur `UIManager`.

### `InventorySceneController`

- garde la logique locale de binding (`InventoryUI.Bind(PlayerInventory.Instance)`),
- mais l'ouverture/fermeture de l'écran devrait être déclenchée par le manager global.

## 5. Séquence cible de démarrage

Option recommandée pour le prototype :

1. démarrer depuis menu,
2. charger `FirstLvl`,
3. charger additivement `NavigationHUD`,
4. précharger `Inventaire` additivement,
5. désactiver le root de l'inventaire,
6. laisser le HUD visible en mode gameplay.

Résultat :

- clic inventaire => `SetActive(true)` sur le root inventaire,
- retour gameplay => `SetActive(false)` sur le root inventaire,
- pas de rechargement de scène à chaque navigation.

## 6. Points d'attention

- un seul `EventSystem` entre toutes les scènes additives,
- éviter de mélanger chargement de scènes et logique visuelle dans `NavigationHUD`,
- garder `BiofiltreManager` indépendant du HUD global,
- documenter l'ordre d'initialisation si plusieurs singletons persistent,
- ajouter `NavigationHUD.unity` au build si elle devient une vraie dépendance runtime.

## 7. Découpage d'implémentation proposé

### Etape 1 — sécuriser le shell UI

- ajouter `NavigationHUD.unity` au build,
- définir quel objet bootstrap le shell,
- garantir l'unicité du `EventSystem`.

### Etape 2 — introduire le `UIManager`

- créer `Assets/Scripts/Systems/UIManager.cs`,
- déplacer la logique de navigation globale hors de `NavigationHUD`,
- centraliser les noms de scènes et IDs d'écrans.

### Etape 3 — convertir l'inventaire en écran préchargé

- charger `Inventaire` une fois,
- exposer un root UI activable,
- remplacer les `LoadScene`/`UnloadSceneAsync` fréquents par `SetActive`.

### Etape 4 — généraliser

- appliquer le même pattern à `Market`,
- ajouter plus tard overlays, transitions et états UI globaux.

## 8. Décision de travail recommandée

Pour ce projet, la meilleure base pour le prochain todo est :

- `NavigationHUD.unity` comme scène shell additive,
- un `UIManager` global dans `Scripts/Systems`,
- `Inventaire` gardée en scène additive légère mais préchargée,
- navigation instantanée via `SetActive` sur les roots UI.

Cela reste compatible avec la structure actuelle, limite le refactor, et prépare bien l'ajout d'autres écrans transverses.

## 9. Prompt simple pour Bezi

```text
Créer une UI globale persistante partagée entre toutes les scènes du projet Unity.
Utiliser `NavigationHUD.unity` comme scène shell UI additive.
Créer un `UIManager` global qui précharge les scènes UI fréquentes en additif au démarrage, en particulier `Inventaire`, puis affiche/masque leurs roots avec `SetActive` pour rendre la navigation quasi instantanée.
Garder un seul `EventSystem`, laisser `NavigationHUD` comme vue HUD, et déplacer la logique de navigation globale dans le `UIManager`.
```
