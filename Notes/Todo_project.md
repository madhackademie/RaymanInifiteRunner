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
- Guide utilisateur (suivi) : `Notes/GUIDE_suivi_projet.md`

---

## Protocole de mise à jour (anti-doublon)

- Mettre à jour le statut uniquement dans ce fichier.
- Dans les notes thématiques, documenter le **comment** (détails techniques), pas le **statut**.
- Si une nouvelle priorité est décidée en session :
  1. la placer dans `## Prochaine session (priorité immédiate)`,
  2. la tracer dans `PROJECT_LOG.md`.

Convention d'IDs :
- `P0-*` = priorité immédiate session.
- `CT-*` = court terme actif.
- `BL-*` = backlog structuré.
- Les IDs restent stables dans le temps (on met à jour le statut, pas l'ID).

---

## Prochaine session (priorité immédiate)

### Popups — clôture pipeline générique
- [x] [P0-POP-003] **Scanner le projet** : verdict **CLOS** — voir `PROJECT_LOG.md` entrée *2026-05-15 — [P0-POP-003] Scan popups — verdict* ; suite : **[P0-HARV-001]** + **BL-POP-DOC-001** (sync doc).

### Doc — popups
- [~] [BL-POP-DOC-001] `popup_generique.md` §2.5 aligné (2026-05-15). Reste : `SYSTEMES_carte_mentale.md`, `Codebase_etat_reference.md`.

### Ferme — harvest (branche dédiée)
- [x] [P0-HARV-001] Popup récolte mode strict lazy — voir `PROJECT_LOG.md` entrée *2026-05-15 — [P0-HARV-001]*. **Playtest FirstLvl** à valider par l’auteur.

### Popups — migrations déjà livrées (référence)
- [x] [P0-POP-001] Graines (`FarmSeedSelection`). **Fait** (2026-05-15).
- [x] [P0-POP-002] Plante / récolte (`FarmPlantHarvest`, instance scène). **Fait** (2026-05-15).
- [x] [P0-POP-004] Shop feedback ressources (`ShopResourceFeedback`). **Fait** (2026-05-15).
- [x] [P0-POP-005] Ferme inventaire plein (`FarmInventoryFeedback`). **Fait** (2026-05-15).
- [x] [P0-POP-006] Test play inventaire plein → popup `ResourceFeedbackPopup`. **Fait** (2026-05-15, session auteur).

---

## Court terme (priorités actives)

### Shop — polish restant
- [~] [CT-SHOP-001] Flux achat de base opérationnel (catalogue + popup item + transaction) ; finaliser le polish.
- [ ] [CT-SHOP-002] Passe UI/UX shop (lisibilité, focus, enchaînement modales).
- [ ] [CT-SHOP-003] Saisie quantité (`TMP_InputField`, clamp min/max).
- [ ] [CT-SHOP-004] Bouton Max (`floor(solde/prix)` + plafonds métier).
- [ ] [CT-SHOP-005] Confirmation avant paiement.
- [ ] [CT-SHOP-006] Corriger les références Inspector manquantes côté shop/prefabs UI.

### Inventaire / wallet / runtime UI
- [ ] [CT-INV-001] Stabiliser le wallet inventaire avec une seule source de vérité (`InventoryScreen` prefab via `UIManager`).
- [ ] [CT-INV-002] Valider qu’il n’y a plus de dépendance runtime cachée à `Inventaire.unity` (ou documenter explicitement son rôle).
- [ ] [CT-INV-003] Vérifier le flux `TryAdd` de bout en bout (id, quantités, stack, inventaire plein, refresh UI).

### Ferme — croissance et persistance
- [~] [CT-FARM-000] Popup récolte : voir **[P0-HARV-001]** (prochaine session, **branche dédiée**).
- [ ] [CT-FARM-001] Corriger la cohérence stade/durée après modification de `PlantDefinition` (runtime + reload).
- [ ] [CT-FARM-002] Garantir la persistance du temps déjà écoulé (stade + timer) via `FarmSaveService` / `PlantPersistenceMarker`.
- [ ] [CT-FARM-003] Revalider le démarrage visuel/logique au stade Graine puis transition vers feuilles.
- [ ] [CT-FARM-004] Valider la persistance JSON en scénario complet : pose -> quit -> relance -> récolte/arrache -> relance.

### Navigation Scene/UI
- [ ] [CT-NAV-001] Debug complet des flux `SceneNavigator.ShowScene` (transitions concurrentes, scènes orphelines, ordre d’activation).
- [~] [CT-NAV-002] Finaliser hub `HomeScene` + retour gameplay (croix/exit) et aligner la doc.
- [~] [CT-NAV-003] Poursuivre la migration Inventaire/Market/HUD global vers le flux cible.
- [ ] [CT-NAV-004] Trancher et documenter le mode de chargement final UI (persistant vs sync vs async/additive).

---

## Backlog structuré

### Audit / nettoyage technique
- [ ] [BL-AUD-001] Audit Bezi Scene/UI (checklist détaillée dans `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`).
- [ ] [BL-AUD-002] Nettoyer le code mort et harmoniser commentaires XML/scripts navigation/UI.
- [ ] [BL-AUD-003] Audit doc global projet + consolidation des notes obsolètes.
- [ ] [BL-AUD-004] Passe commentaires FR ciblée sur scripts complexes (`Assets/Scripts/**`).

### Prototype gameplay
- [~] [BL-PROTO-001] Finaliser la state machine culture (transitions gameplay/récolte).
- [ ] [BL-PROTO-002] Revoir `Assets/Scripts/Core/Timer.cs` (scalabilité, `unscaledDeltaTime`, offline UTC).
- [ ] [BL-PROTO-003] Supprimer le doublon éventuel `Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity`.
- [ ] [BL-PROTO-004] Terminer le panel Options du menu principal.
- [ ] [BL-PROTO-005] Maintenir à jour la carte des flux système (`Notes/Farm/SYSTEMES_carte_mentale.md`).

### GDD / design
- [ ] [BL-GDD-001] Esquisser le GDD MVP (concept, boucle, scope).
- [ ] [BL-GDD-002] Spécifier le temps de ferme (`lastUtc -> delta`, plafond offline).
- [ ] [BL-GDD-003] Formaliser la progression XP joueur + maturité biofiltre.
- [ ] [BL-GDD-004] [OPTIONNEL] Collecter 2-3 références UI et noter ce qui est repris/évité.

### Workflow / documentation
- [ ] [BL-DOC-001] Documenter les règles projet (style, conventions AI/notes, organisation dossiers).
- [ ] [BL-DOC-002] Finaliser le rappel workflow Git "Save All avant Git".
- [ ] [BL-DOC-003] Définir les conventions de localisation (`country` vs `language`, keys TMP).
- [ ] [BL-DOC-004] [OPTIONNEL] Compléter `Notes/Bezi/README_bezi.md` (version Unity, scènes, bezi.actions).

### Art / assets
- [~] [BL-ART-001] Finaliser le cycle complet de la laitue prototype.
- [~] [BL-ART-002] Terminer le nettoyage des assets temporaires.
- [ ] [BL-ART-003] Documenter le double procédé graphique (léger + 3D) et les critères d’usage.
- [ ] [BL-ART-004] Centraliser un workflow 3D validé (sources -> export -> intégration).
- [ ] [BL-ART-005] Décider la cible mobile 2D vs 3D selon playtests perf.
- [ ] [BL-ART-006] Intégrer l’illustration LoadingScreen + QA (play + build).

### Cloud / post-prototype
- [ ] [BL-CLOUD-001] [OPTIONNEL] Préparer UGS inventaire (Auth -> Cloud Save -> Economy).
- [ ] [BL-CLOUD-002] Implémenter l’architecture services cloud inventaire/market (`IInventoryService` / `IMarketService`).
- [ ] [BL-CLOUD-003] Découpler gameplay de `PlayerInventory.Instance` avant sync cloud.
- [ ] [BL-CLOUD-004] [OPTIONNEL] Définir un workflow graphique IA (polish).
- [ ] [BL-CLOUD-005] Polish UI inventaire + polish technique navigation UI.

### Nettoyage dépôt
- [ ] [BL-REPO-001] Après validation art, audit/suppression des dossiers assets vides + commit dédié de nettoyage.
