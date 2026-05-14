# Todo projet — source unique des tâches

Ce fichier est la **source de vérité unique** pour le statut des tâches du projet.

Règle de suivi :
- **Uniquement ici** on utilise `[ ]`, `[~]`, `[x]`.
- Les autres fichiers (`Notes/Ui/Todo_ui.md`, `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`, `Notes/Ui/SPEC_services_inventory_market_cloud.md`, etc.) servent de **détail opérationnel** et doivent pointer vers cette page.

Références de détail :
- UI : `Notes/Ui/Todo_ui.md`
- Audit Scene/UI : `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`
- Cloud services : `Notes/Ui/SPEC_services_inventory_market_cloud.md`
- Navigation scènes : `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`
- Farm : `Notes/Farm/SYSTEMES_carte_mentale.md`
- Journal : `PROJECT_LOG.md`

---

## Priorité immédiate (session)

### FirstLvl — pipeline popup générique
- [ ] Migrer la popup **sélection des graines** vers le pipeline générique (`PopupId` + `ScreenPopupBinding` + `ScreenPopupHost`).
- [ ] Migrer la popup **plante (info/état + récolte)** vers le pipeline générique (sans chemin concurrent caché).
- [ ] Scanner le projet pour les popups hors pipeline générique et produire un plan de migration priorisé.

---

## Court terme (priorités actives)

### Shop — polish restant
- [~] Flux achat de base opérationnel (catalogue + popup item + transaction) ; finaliser le polish.
- [ ] Passe UI/UX shop (lisibilité, focus, enchaînement modales).
- [ ] Saisie quantité (`TMP_InputField`, clamp min/max).
- [ ] Bouton Max (`floor(solde/prix)` + plafonds métier).
- [ ] Confirmation avant paiement.
- [ ] Corriger les références Inspector manquantes côté shop/prefabs UI.

### Inventaire / wallet / runtime UI
- [ ] Stabiliser le wallet inventaire avec une seule source de vérité (`InventoryScreen` prefab via `UIManager`).
- [ ] Valider qu’il n’y a plus de dépendance runtime cachée à `Inventaire.unity` (ou documenter explicitement son rôle).
- [ ] Vérifier le flux `TryAdd` de bout en bout (id, quantités, stack, inventaire plein, refresh UI).

### Ferme — croissance et persistance
- [ ] Corriger la cohérence stade/durée après modification de `PlantDefinition` (runtime + reload).
- [ ] Garantir la persistance du temps déjà écoulé (stade + timer) via `FarmSaveService` / `PlantPersistenceMarker`.
- [ ] Revalider le démarrage visuel/logique au stade Graine puis transition vers feuilles.
- [ ] Valider la persistance JSON en scénario complet : pose -> quit -> relance -> récolte/arrache -> relance.

### Navigation Scene/UI
- [ ] Debug complet des flux `SceneNavigator.ShowScene` (transitions concurrentes, scènes orphelines, ordre d’activation).
- [~] Finaliser hub `HomeScene` + retour gameplay (croix/exit) et aligner la doc.
- [~] Poursuivre la migration Inventaire/Market/HUD global vers le flux cible.
- [ ] Trancher et documenter le mode de chargement final UI (persistant vs sync vs async/additive).

---

## Backlog structuré

### Audit / nettoyage technique
- [ ] Audit Bezi Scene/UI (checklist détaillée dans `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`).
- [ ] Nettoyer le code mort et harmoniser commentaires XML/scripts navigation/UI.
- [ ] Audit doc global projet + consolidation des notes obsolètes.
- [ ] Passe commentaires FR ciblée sur scripts complexes (`Assets/Scripts/**`).

### Prototype gameplay
- [~] Finaliser la state machine culture (transitions gameplay/récolte).
- [ ] Revoir `Assets/Scripts/Core/Timer.cs` (scalabilité, `unscaledDeltaTime`, offline UTC).
- [ ] Supprimer le doublon éventuel `Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity`.
- [ ] Terminer le panel Options du menu principal.
- [ ] Maintenir à jour la carte des flux système (`Notes/Farm/SYSTEMES_carte_mentale.md`).

### GDD / design
- [ ] Esquisser le GDD MVP (concept, boucle, scope).
- [ ] Spécifier le temps de ferme (`lastUtc -> delta`, plafond offline).
- [ ] Formaliser la progression XP joueur + maturité biofiltre.
- [ ] [OPTIONNEL] Collecter 2-3 références UI et noter ce qui est repris/évité.

### Workflow / documentation
- [ ] Documenter les règles projet (style, conventions AI/notes, organisation dossiers).
- [ ] Finaliser le rappel workflow Git "Save All avant Git".
- [ ] Définir les conventions de localisation (`country` vs `language`, keys TMP).
- [ ] [OPTIONNEL] Compléter `Notes/Bezi/README_bezi.md` (version Unity, scènes, bezi.actions).

### Art / assets
- [~] Finaliser le cycle complet de la laitue prototype.
- [~] Terminer le nettoyage des assets temporaires.
- [ ] Documenter le double procédé graphique (léger + 3D) et les critères d’usage.
- [ ] Centraliser un workflow 3D validé (sources -> export -> intégration).
- [ ] Décider la cible mobile 2D vs 3D selon playtests perf.
- [ ] Intégrer l’illustration LoadingScreen + QA (play + build).

### Cloud / post-prototype
- [ ] [OPTIONNEL] Préparer UGS inventaire (Auth -> Cloud Save -> Economy).
- [ ] Implémenter l’architecture services cloud inventaire/market (`IInventoryService` / `IMarketService`).
- [ ] Découpler gameplay de `PlayerInventory.Instance` avant sync cloud.
- [ ] [OPTIONNEL] Définir un workflow graphique IA (polish).
- [ ] Polish UI inventaire + polish technique navigation UI.

### Nettoyage dépôt
- [ ] Après validation art, audit/suppression des dossiers assets vides + commit dédié de nettoyage.
