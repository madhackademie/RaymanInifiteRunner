# Audit Bezi + refactor navigation Scene / UI (checklist de travail)

Statut officiel : **`Notes/Todo_project.md`** (source unique).

Objectif :
- auditer le flux runtime Scene/UI réel ;
- nettoyer/refactor les zones mortes ;
- réaligner les docs sur le comportement actuel du code.

---

## Checklist audit Bezi (scène / UI)

- Parcourir `SceneNavigator`, `GameBootstrap`, `NavigationHUD`, `MapSceneController`, `FirstLvlController`, `UIManager`, `InventorySceneController` (si utilisé) et valider en Play Mode + Build Settings.
- Vérifier la cohabitation scène `Inventaire` vs prefab inventaire (`UIManager`) et documenter le rôle de chaque chemin.
- Confirmer `lazyScenes` (Inspector) et distinguer eager boot vs lazy runtime.
- Contrôler l’unicité de `EventSystem`, l’ordre de chargement et l’absence de scène fantôme.

## Clean / refactor (après audit)

- Supprimer ou raccorder le code mort identifié (`BiofiltreManager.TryOpenHarvestPanel`, `FindInteractorAt`, etc.).
- Mettre à jour commentaires XML et en-têtes de scripts UI/navigation.
- Harmoniser `ARCHI_hud_ui_manager_additive.md`, `Journal_ui.md`, `Todo_ui.md`, `GUIDE_scenes_navigation_Unity_inventaire_market.md` avec le code réel.
- Recenser les classes peu référencées (dont `InventorySceneController`) et décider "supprimer" vs "réservé scène Unity".

## Suivi documentaire

- Journal de session : `PROJECT_LOG.md`.
- Source de statut : `Notes/Todo_project.md`.
