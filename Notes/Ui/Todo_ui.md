# TODO UI — vue détaillée (sans statut)

Statut officiel des tâches : **`Notes/Todo_project.md`** (source unique).

Règle :
- ne pas cocher `[ ]/[~]/[x]` ici ;
- cette note contient uniquement les détails d’implémentation UI et les liens utiles.

---

## Références rapides

- Backlog global : `Notes/Todo_project.md`
- Architecture shell/UI : `Notes/Ui/ARCHI_hud_ui_manager_additive.md`
- Audit Scene/UI : `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`
- Guide navigation scènes : `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`
- Pipeline popup générique : `Notes/Ui/popup_generique.md`

---

## Détails d’exécution UI

### Ferme — graines plantation (merge `main` 2026-05-19)
- **Livré** : stock inventaire, label `×N`, consommation au plant — `SeedSelectionUI` / `SeedSlotUI`, plan `Notes/Farm/REFACTOR_graines_plantation_inventaire.md`.
- **Bug ouvert [P0-FARM-BUG-001]** : après achat shop, titre « Aucune graine… » reste affiché avec slot visible — `PROJECT_LOG.md` 2026-05-22.
- **Prochaine session** : fix bug ; prefab EmptyState [P0-FARM-UI-001].

### FirstLvl — popups génériques
- Migrer l’UI de sélection de graines vers le pipeline popup générique (`PopupId` + binding + `ScreenPopupHost`) — **déjà sur pipeline** ; reste polish EmptyState.
- Migrer la popup plante (état/info/récolte) vers le même pipeline, sans second chemin legacy.
- Produire un inventaire des autres popups hors pipeline et proposer un ordre de migration.

### Shop — popup achat (`ShopItemPopup`) — livré 2026-05-19
- **Fait** (branche `rework/shopitempopup`, merge `main`) : `ShopItemPopupController` / `ShopItemPopupView`, prefab `Assets/Prefabs/Ui/ShopItemPopup.prefab` — input quantité, Max, overlay confirmation, `CurrencyBalanceUI` (solde) dans le Header. Statuts : **`Notes/Todo_project.md`** [P0-SHOP-POP-001], [CT-SHOP-003]…[006].
- **Reste optionnel** : passe UX globale shop [CT-SHOP-002] (focus, transitions, polish visuel).

### Navigation Scene/UI
- Revalider tous les chemins `SceneNavigator.ShowScene` (transitions, hub `HomeScene`, retour gameplay).
- Vérifier `NavigationHUD` + `MapSceneController` + `FirstLvlController` en playtest.
- Contrôler l’unicité de `EventSystem` et l’état des Build Settings.
- Aligner les docs historiques ("Carte") vers `HomeScene`/`Map` cible.

### LoadingScreen
- Intégrer l’illustration finale (poisson + arbre) dans `Bootstrap.unity`.
- Exécuter la QA loading (progression, fade, ordre de chargement, pas de flash UI).
- Réf workflow : `Notes/Ui/LOADINGSCREEN_image_workflow.md`.

---

## Localization TextMeshPro — plan

Décisions rappel :
- `country` et `language` sont distincts.
- langue par défaut dérivée du pays, override joueur prioritaire.
- changement de langue = refresh global des TMP localisés.

Étapes techniques :
- Définir `Language`/`Country` + mapping `defaultLanguageFromCountry`.
- Persister l’override (PlayerPrefs en proto).
- Implémenter `LanguageManager` (`CurrentLanguage`, `OnLanguageChanged`, `GetText` avec fallback).
- Créer `LocalizedTMPText` (`key`, abonnement event, refresh au `OnEnable`).
- Poser une convention stable de keys (`BTN_PLAY`, `TITLE_FARM`, ...).
- Prévoir phase proto (table simple) puis phase finale (SO/JSON).

---

## État déjà implémenté (mémo)

- `Bootstrap.unity` + `GameBootstrap` + `LoadingScreen`.
- `SceneNavigator` + `SceneId` pour transitions de scènes de contenu.
- `UIManager` global (show/hide écrans prefabs).
- `NavigationHUD` shell et `EventSystem` unique visé.
