# RULE DRAFT — Bezi IA — Architecture Scene/UI runtime (Unity)

Ce document sert de base pour créer une **rule Bezi IA** sur la gestion des scènes et de l'UI dans ce projet.

---

## 1) Objectif architecture

Conserver une architecture **réactive, lisible et performante** :

- **Scènes de contenu additives** (pas de `LoadScene` direct dispersé).
- **Navigation centralisée** par `SceneNavigator`.
- **HUD shell persistant** (`NavigationHUD`) piloté par l'état de navigation.
- **Écrans globaux** (`UIManager`) pour overlays/panels transverses.
- **Source unique de vérité** pour l'état runtime : `SceneNavigator` (`CurrentScene`, `IsTransitioning`).

---

## 2) Composants runtime et responsabilités

### `GameBootstrap`
- Charge les scènes de base au lancement (`NavigationHUD`, `HomeScene`, template `Inventaire`).
- Définit la scène initiale visible (`SetInitialScene(HomeScene)`).
- Gère le LoadingScreen de boot.

### `SceneNavigator`
- API unique de navigation : `ShowScene(sceneName)`.
- Séquence les demandes (`pendingSceneName`) pour éviter collisions.
- Émet les événements :
  - `OnBeforeSceneShown`
  - `OnAfterSceneShown`
  - `OnTransitionStateChanged(bool)`
- Active/désactive les racines de scènes de contenu.

### `NavigationHUD`
- Réagit aux événements de `SceneNavigator`.
- Choisit automatiquement le mode :
  - `Hidden` (transition)
  - `Navigation` (Home/Inventaire)
  - `ExitOnly` (gameplay)
- Les tabs demandent des transitions via `SceneNavigator` ou des écrans via `UIManager`.

### `UIManager`
- Gère les écrans globaux (show/hide, preload, lazy).
- Masque tous les écrans globaux pendant transition.
- Peut migrer progressivement des écrans issus de scènes vers des écrans shell (ex: `Inventory`).

---

## 3) Flux runtime (ouverture d'une scène)

1. UI/action appelle `SceneNavigator.ShowScene(target)`.
2. `SceneNavigator` passe `IsTransitioning = true` et émet `OnTransitionStateChanged(true)`.
3. `NavigationHUD` passe en `Hidden`.
4. `UIManager` masque ses écrans globaux.
5. Si nécessaire, la scène cible est chargée en additif.
6. Les roots de l'ancienne scène sont désactivés, ceux de la nouvelle activés.
7. `CurrentScene` est mis à jour et `OnAfterSceneShown(target)` est émis.
8. `NavigationHUD` choisit automatiquement le mode final selon la scène active.
9. `IsTransitioning = false`.

---

## 4) Règles Bezi IA à appliquer (proposition)

1. **Navigation**
   - Toujours passer par `SceneNavigator.ShowScene`.
   - Interdire les `SceneManager.LoadScene(...)` directs dans les scripts UI/gameplay (hors bootstrap technique validé).

2. **HUD/UI globale**
   - Ne pas piloter le HUD à la main depuis les scènes sauf exception documentée.
   - Utiliser les événements `SceneNavigator` pour mode/visibilité.
   - Utiliser `UIManager` pour overlays globaux.

3. **Transitions**
   - Toute transition doit cacher l'UI globale non critique.
   - Éviter les double-clics problématiques : respecter `IsTransitioning`.

4. **Scènes**
   - Conserver les scènes de contenu additives.
   - Préférer activation/désactivation de racines à des reloads complets.

5. **Perf/UI**
   - Éviter un canvas monolithique unique si cela augmente les rebuilds.
   - Préférer shell UI + écrans ciblés.

6. **Migration progressive**
   - Pour migrer une UI de scène vers UI globale :
     - cloner sous `screenRoot`,
     - retirer les composants canvas locaux si nécessaire,
     - enregistrer dans `UIManager` avec un `ScreenId`.

---

## 5) TODO session soir (rappel stratégique)

- Retirer la dépendance runtime à `Inventaire.unity` :
  - remplacer la source scène par un prefab/global screen propre,
  - garder `Inventaire.unity` uniquement comme template éditorial ou la retirer des flux runtime.

