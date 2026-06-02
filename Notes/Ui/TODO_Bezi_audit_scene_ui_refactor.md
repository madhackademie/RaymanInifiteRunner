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

> **MAJ 2026-06-02 (`chore/audit-cleanup-2026-06`)** : passe de nettoyage code réalisée — voir `PROJECT_LOG.md` 2026-06-02.

- [x] Code mort retiré (`Timer.cs`, API orphelines). `TryOpenHarvestPanel`/`FindInteractorAt` **n'existaient déjà plus** dans le code.
- [x] Legacy supprimé : `MainMenuUI` + `SampleScene` (×2) + entrée Build Settings.
- [x] Doublons factorisés (`PlantDefinition.GetSprite`, `RemovePlantFromGrid`, `ShopCatalogResolver`, `FarmPopupCanvasFactory`, `FarmStateSerializer`).
- [x] `InventorySceneController` : conservé (prefab runtime + bouton `Close` câblé), `Open()` mort retiré.
- [ ] Reste : harmoniser `ARCHI_hud_ui_manager_additive.md`, `Journal_ui.md`, `Todo_ui.md`, guide scènes avec le code réel (`ShowScene`/`SetActive`).

## Suivi documentaire

- Journal de session : `PROJECT_LOG.md`.
- Source de statut : `Notes/Todo_project.md`.
