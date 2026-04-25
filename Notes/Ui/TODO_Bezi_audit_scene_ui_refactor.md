# TODO — Audit Bezi + refactor navigation Scene / UI

**Cible de session recommandée : ~2026-05-01** (environ dix jours après 2026-04-21). À ajuster selon le calendrier réel.

## Objectif

Clôturer l’audit **Bezi** sur la **nouvelle approche** de gestion des scènes et de l’UI (shell `NavigationHUD`, visibilité des scènes de contenu via **`SceneNavigator.ShowScene`** + `SetActive` sur les racines, chargement eager au boot dans **`GameBootstrap`**, prefabs globaux **`UIManager`**), puis enchaîner sur **nettoyage / refactor** et **alignement des commentaires** et de la doc Markdown.

## Checklist audit Bezi (scène / UI)

- [ ] Parcourir **`SceneNavigator`**, **`GameBootstrap`**, **`NavigationHUD`**, **`MapSceneController`**, **`FirstLvlController`**, **`UIManager`**, **`InventorySceneController`** (scène `Inventaire.unity` si utilisée) et valider le flux réel en **Play Mode** + **Build Settings**.
- [ ] Vérifier la cohabitation **scène `Inventaire`** (contenu) vs **prefab inventaire** sous **`UIManager`** : une seule source de vérité ou rôle documenté pour chaque chemin.
- [ ] Confirmer la liste **`lazyScenes`** sur **`SceneNavigator`** (Inspector) : quelles scènes sont chargées au premier accès vs **eager** au boot.
- [ ] Contrôler **un seul `EventSystem`**, ordre de chargement, et absence de scène « fantôme » (racines actives par erreur).

## Clean / refactor (après audit)

- [ ] Supprimer ou **brancher** le code mort identifié (ex. **`BiofiltreManager.TryOpenHarvestPanel`** / **`FindInteractorAt`** — non utilisés par le clic grille ; décision : suppression vs réutilisation documentée).
- [ ] Passer en revue les scripts UI / navigation pour **commentaires XML** et en-têtes de fichier à jour (alignés sur **`ShowScene`** et le flux runtime actuel).
- [ ] Harmoniser les notes **`Notes/Ui/ARCHI_hud_ui_manager_additive.md`**, **`Notes/Ui/Journal_ui.md`**, **`Notes/Ui/Todo_ui.md`**, **`Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`** avec le comportement **réel** du code (voir aussi **`Notes/Codebase_etat_reference.md`**).
- [ ] Inventaire des **classes peu ou pas référencées** dans les scripts (ex. recherche d’usage de `InventorySceneController` hors scène) ; supprimer ou documenter « réservé scène Unity ».

## Suivi

- Journal : **`PROJECT_LOG.md`** (entrée du jour de la session d’audit).
- Hub tâches : **`Notes/Todo_project.md`**, **`Notes/Ui/Todo_ui.md`**.
