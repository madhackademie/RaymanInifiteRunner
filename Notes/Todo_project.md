# Todo projet — hub global

Liens vers les TODOs thématiques : `Notes/Ui/Todo_ui.md`, `Notes/Farm/SPEC_plant_footprint_prompt.md`, `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, `Notes/Farm/TODO_plantation_pipeline.md`, `Notes/Farm/SYSTEMES_carte_mentale.md`, `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`, etc. **État code / assets (snapshot)** : `Notes/Codebase_etat_reference.md` ; journal chronologique : `PROJECT_LOG.md` (racine).

---

## Prochaine session (priorité immédiate)

> **2026-04-12 — priorité inchangée** : la prochaine session reste centrée sur le **même objectif** que celle d’aujourd’hui — **câblage récolte / inventaire en scène** (test jeu réel). Les données laitue + règle `harvestItemId` ↔ `itemId` sont posées ; voir **`Docs/PLANTES_ET_INVENTAIRE.md`** et **`PROJECT_LOG.md`** (entrée *fin de session* du 2026-04-12).

- [ ] **Câblage récolte / inventaire (priorité #1)** : boucler le flux **scène + Inspector** — `BiofiltreManager` (`itemDatabase`, `playerInventory`, `harvestPanelUI`), `HarvestPanelUI`, `PlantHarvestInteractor` / prefab plante, `PlayerInventory`, `ItemDatabase` (inclure les `ItemDefinition` utilisés), `InventoryUI` + slots, `InventoryFeedbackUI` si besoin. Scène **`FirstLvl`** (ou test dédié). DoD : cellule occupée → panel → **Récolter** → `TryAdd`, UI inventaire à jour, cas **inventaire plein**. Schéma : Zoom D dans `Notes/Farm/SYSTEMES_carte_mentale.md`. *(Inclut la suite éventuelle Bezi / mature vs graines une fois ce flux minimal validé.)*
- [ ] **Git — branche par feature + merge** : à partir des prochaines features, suivre le protocole **`GIT_HELPER.md` section --3--** (créer `feature/…` depuis `main` à jour, pousser, tester, puis **merge** dans `main` une fois validé — PR GitHub ou `git merge` local). *Rétrospective* : idéalement **avant** gros chantiers type récolte / inventaire ; pour l’instant consigner l’usage pour les suites (ex. mature/graines). Ajuster le helper si la branche par défaut n’est pas `main`.
- [ ] **Lecture / compréhension du flux** *(optionnelle mais utile après câblage)* : parcourir grille → `HarvestPanelUI` → `ConfirmHarvest` → `TryAdd` — `Notes/Farm/SYSTEMES_carte_mentale.md` (Zoom D), `Notes/Codebase_etat_reference.md`.
- [ ] **Récolte — verrou d’état + double récolte (refactor)** : après succès `TryAdd`, faire avancer la plante (ou consommer `maxHarvestCount`) pour **éviter la double récolte** au même stade. Trancher / modéliser les **deux récoltes** du cycle (ex. Leafy : **Mature** = item feuille, **Seedling** = graines) : soit second `harvestItemId` + stade dans `PlantDefinition`, soit composant / stratégie dédiée — mettre à jour `PlantHarvestInteractor` en conséquence.
- [ ] **Documentation systèmes** : lire / compléter la **carte des flux** (`Notes/Farm/SYSTEMES_carte_mentale.md`) : plantation (grille, `BiofiltreManager`, `PlantDefinition`), croissance (`PlantGrow`), récolte ↔ inventaire ; ajuster le schéma si de nouveaux points d’entrée apparaissent.

- [x] **Documentation — nouvelles plantes** : rédiger le **workflow pas à pas** pour ajouter une plante (création / duplication `PlantDefinition`, footprint + prefab `PlantGrow`, entrée `SeedSelectionUI` / `SeedEntry`, vérification sur la grille). Réfs code : `BiofiltreManager`, `PlantPlacementPreview`, `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`. **Guide** : `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md`.
- [x] **Art — sprites** : retirer le **fond blanc** sur les sprites concernés et les exporter avec **transparence** (canal alpha) ; dans Unity, vérifier l’import texture (alpha / compression) pour éviter les halos blancs.
- [x] **Gameplay — footprint (données)** : le `PlantDefinition` contient déjà `footprint` + `GetOccupiedCells` ; finaliser la **compréhension** (dont **dédoublonnage** des offsets — voir session assistant) et les assets plantes avec les bons footprints (ex. salade 2×2). Réfs : `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, `Notes/Farm/SPEC_plant_footprint_prompt.md`.
- [x] **Gameplay — grille + plantation** : noyau **`GridData` + `GridManager` + `GridConfig`** + **flux biofiltre** (`BiofiltreManager`, `BiofiltreGridVisualizer`, `BiofiltreCell`, preview `PlantPlacementPreview`, UI `SeedSelectionUI` / `SeedSlotUI`). Détail historique : **`Notes/Farm/TODO_plantation_pipeline.md`** (étapes 1–3 réalisées ; pas de classe `BuildManager`, équivalent fonctionnel ci-dessus).

---

## Prototype (phase actuelle)

- [~] **Inventaire récolte (spécification)** : une première implémentation existe (`PlayerInventory.TryAdd`, piles, `InventoryResult` y compris partiel) ; finaliser la **spec joueur** (tout-ou-rien vs partiel accepté en gameplay) après tests en scène.
- [~] **State machine culture** : stades visuels posés (`Seedling`, `BabyLeaf`, `Growing`, `Mature`, `Bolting`) ; finaliser transitions gameplay + règles de récolte.
- [ ] **Flux récolte minimal jouable** : clic sur objet mature -> tentative d’ajout inventaire -> succès = récolte/transition d’état, échec = popup inventaire plein et objet reste mature.
- [ ] **Timer gameplay** : revoir `Assets/Scripts/Core/Timer.cs` (scalabilité, `unscaledDeltaTime`, persistance/reprise offline via timestamp UTC).
- [ ] **Scènes Unity** : supprimer le doublon éventuel `Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity` et garder une seule scène de démarrage.
- [ ] **Menu principal** : terminer le panel Options (langue, audio, etc.) en s’alignant sur `Notes/Ui/Todo_ui.md`.

---

## GDD / Design

- [ ] **Esquisser le GDD simple** : concept, boucle principale, scope MVP.
- [ ] **Spec temps de ferme** : durée d’un jour ferme, vitesse de croissance, plafond offline, formule de reprise (`lastUtc -> delta`) ; **croiser** avec la maturité système / cycles (`Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`).
- [ ] **Progression** : **XP joueur** + **maturité biofiltre / système** (cycles, ~1 an temps réel, ordre de grandeur ~10 salades pour cultures fruits avancées) — brouillon : `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`.
- [ ] **[OPTIONNEL] Références UI** : collecter 2–3 jeux de référence (captures) et noter ce qui est repris/évité.

---

## Workflow / Organisation

- [ ] **Règles projet** : documenter style de code, conventions AI/notes, organisation des dossiers (fichier dédié à confirmer).
- [ ] **Workflow Git** : rappel « Save All avant Git » dans `WORKFLOW_PROTOCOL.md` ; **branche par feature + merge** documenté dans **`GIT_HELPER.md` (--3--)** et rappel session dans **`WORKFLOW_PROTOCOL.md` (--5--)**.
- [ ] **Conventions de localisation** : définir les clés TextMeshPro et valider la séparation `country` vs `language`.
- [ ] **[OPTIONNEL] Bezi docs** : compléter `Notes/Bezi/README_bezi.md` (version Unity exacte, scènes de travail, `bezi.actions`).

---

## Art / Assets (prototype)

- [~] **Laitue prototype** : `PlantDefinition` + asset `Laitue` créés ; finaliser le prefab et le cycle complet en jeu.
- [~] **Nettoyage assets temporaires** : purge 3D engagée (samples plugin / generated models) ; valider les références restantes.
- [ ] **Deux procédés graphiques** : documenter les 2 voies visuelles (procédé graphique léger + workflow 3D) et leurs conditions d’usage.
- [ ] **Workflow 3D validé** : centraliser le pipeline de sortie des assets 3D (sources, exports, intégration Unity) puisque des assets sont déjà produits via ce workflow.
- [ ] **Décision cible mobile (2D vs 3D)** : si les tests mobile sont fluides/stables (FPS, chauffe, mémoire, batterie), valider une direction finale en **3D** ; sinon conserver une option plus légère.

---

## Polish — après validation du prototype

- [ ] **[OPTIONNEL] Workflow graphique + IA** : définir et mettre en place (ou intégrer au pipeline) un flux de production visuelle pour la phase polish — génération / itération d’assets avec [Adobe Firefly — AI pour le développement de jeux](https://www.adobe.com/products/firefly/discover/ai-for-game-developers.html), **uniquement** si la direction art retenue en a besoin après validation prototype/performance mobile (documentation et bonnes pratiques licence / usage à valider avant production commerciale).

- [ ] **[SUPPER_POLISH] graphique animation** création d'une animation d'abeille qui rode autour des plantes en fleurs avant passage fruits/graines

---

## Backlog — nettoyage projet

- [ ] Après validation du concept art : audit des dossiers sous `Assets/_Project/` (ou chemin choisi), suppression des dossiers vides inutiles + `.meta` associés, vérifier `git status` et commit dédié « chore: remove empty asset folders ».
