# Todo projet — hub global

Liens vers les TODOs thématiques : `Notes/Ui/Todo_ui.md`, `Notes/Farm/SPEC_plant_footprint_prompt.md`, `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, `Notes/Farm/TODO_plantation_pipeline.md`, `Notes/Farm/SYSTEMES_carte_mentale.md`, `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`, etc. **État code / assets (snapshot)** : `Notes/Codebase_etat_reference.md` ; journal chronologique : `PROJECT_LOG.md` (racine).

---

## Prochaine session (priorité immédiate)

> **2026-04-18 (fin de session)** : **prochaine session** — **contrôle et réparation** de la **navigation entre les scènes** ; **contrôle** des **items ajoutés** à l’inventaire (données + UI après `TryAdd`). Détail : entrée **`PROJECT_LOG.md`** du **2026-04-18**.
>
> **2026-04-16 (fin de session)** : le **bootstrap** et le **shell UI** sont en place (`Bootstrap.unity` → `NavigationHUD` additif → `FirstLvl`, `GameBootstrap`, `LoadingScreen`, `UIManager` avec prefabs prioritaires/secondaires, `ScreenId`). **2026-04-17** : branche de travail **navigation / UI** créée par l’auteur — plus de rappel « créer la branche » dans la checklist ci-dessous ; merge dans `main` selon **`GIT_HELPER.md` — --3--** quand la feature sera prête.
>
> **Prochaine session (focus auteur)** : **illustration + intégration** sur l’écran de chargement (**poisson + arbre** ou variante validée) — guide pas à pas **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**, détail **`Notes/Ui/Todo_ui.md`** (*Bootstrap & LoadingScreen*).
>
> **Suite chantier navigation** : scène hub **`Carte`** ; depuis **`FirstLvl`**, la **croix** doit ramener à **`Carte`**. Détail : **`Notes/Ui/Todo_ui.md`** (*Priorité session suivante — hub Carte*), **`Notes/Ui/Journal_ui.md`**, **`Notes/Ui/ARCHI_hud_ui_manager_additive.md`**.

- [ ] **Navigation scènes — audit + réparation (priorité session suivante)** : repasser tous les chemins (Bootstrap → `NavigationHUD` / shell → `FirstLvl` → UI inventaire / retours) ; corriger les bugs (ordre de chargement, scènes orphelines, **double `EventSystem`**, références **Build Settings**). Réfs : `GameBootstrap`, `UIManager`, `NavigationHUD`, `Notes/Ui/ARCHI_hud_ui_manager_additive.md`, `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`.
- [ ] **Inventaire — contrôle des items ajoutés (priorité session suivante)** : après récolte ou tout `TryAdd`, vérifier **`itemId`**, quantités, piles, cas **plein**, et **refresh UI** ; cohérence avec **`ItemDefinition`** / **`ItemDatabase`**. Réfs : `PlayerInventory`, UI inventaire, `Notes/Farm/SYSTEMES_carte_mentale.md` (Zoom D).
- [ ] **Illustration + intégration LoadingScreen (priorité session auteur)** : créer l’image, importer (sprite UI, alpha), placer dans **`Bootstrap.unity`** ; QA play + build — **`Notes/Ui/LOADINGSCREEN_image_workflow.md`**.
- [ ] **Hub `Carte` + retour croix depuis `FirstLvl`** : créer la scène **`Carte`**, l’ajouter au **Build Settings**, définir le flux *Bootstrap → shell → Carte → niveaux* (ou équivalent), implémenter la navigation **FirstLvl → Carte** sur le bouton exit du HUD sans casser l’unicité **`EventSystem`**. DoD : depuis le niveau, la croix ramène bien à **`Carte`** avec HUD cohérent ; documenter unload/load dans **`Notes/Ui/ARCHI_hud_ui_manager_additive.md`**.
- [ ] **Tests écran de chargement (QA)** : après intégration visuelle, reprendre la checklist **`Notes/Ui/Todo_ui.md`** (*Bootstrap & LoadingScreen* — barre, `AsyncOperation` 0.9, fade, pas de double `EventSystem`).
- [~] **Scènes Inventaire + Market + HUD global** : le **HUD / `UIManager`** est amorcé (prefabs + shell) ; poursuivre avec **Market** et clarifier le sort de **`Inventaire.unity`** au build si tout est migré en prefab.
- [x] **Session du jour — base navigation inventaire** : création d’une **scène prototype Inventaire** pour lancer le chantier navigation/UI.
- [ ] **Décision perf chargement scènes UI** : comparer et trancher **UI persistante + panels**, **`LoadScene` sync**, **`LoadSceneAsync`** (Single/Additive), puis documenter le choix final (latence perçue, complexité, mémoire, risques `EventSystem`/`DontDestroyOnLoad`) dans `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`.
- [x] **Câblage récolte / inventaire (scène + Inspector)** : flux **cellule occupée → panel → Récolter → `TryAdd`** + destruction plante, UI slots, cas inventaire plein — validé côté auteur (voir `Notes/Farm/SYSTEMES_carte_mentale.md` Zoom D).
- [ ] **Lecture / compréhension du flux** *(optionnelle mais utile après câblage)* : parcourir grille → `HarvestPanelUI` → `ConfirmHarvest` → `TryAdd` — `Notes/Farm/SYSTEMES_carte_mentale.md` (Zoom D), `Notes/Codebase_etat_reference.md`.
- ~~[ ] **Récolte — double cycle feuille + graine / verrou multi-récolte**~~ **Hors scope pour ce jeu** (décision 2026) : une récolte réussie = `Destroy` — voir `ASSISTANT_CONTEXT.md`. Pas de TODO actif ; rouvrir seulement si le design change plus tard.
- [ ] **Documentation systèmes** : lire / compléter la **carte des flux** (`Notes/Farm/SYSTEMES_carte_mentale.md`) : plantation (grille, `BiofiltreManager`, `PlantDefinition`), croissance (`PlantGrow`), récolte ↔ inventaire ; ajuster le schéma si de nouveaux points d’entrée apparaissent.

- [x] **Documentation — nouvelles plantes** : rédiger le **workflow pas à pas** pour ajouter une plante (création / duplication `PlantDefinition`, footprint + prefab `PlantGrow`, entrée `SeedSelectionUI` / `SeedEntry`, vérification sur la grille). Réfs code : `BiofiltreManager`, `PlantPlacementPreview`, `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`. **Guide** : `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md`.
- [x] **Art — sprites** : retirer le **fond blanc** sur les sprites concernés et les exporter avec **transparence** (canal alpha) ; dans Unity, vérifier l’import texture (alpha / compression) pour éviter les halos blancs.
- [x] **Gameplay — footprint (données)** : le `PlantDefinition` contient déjà `footprint` + `GetOccupiedCells` ; finaliser la **compréhension** (dont **dédoublonnage** des offsets — voir session assistant) et les assets plantes avec les bons footprints (ex. salade 2×2). Réfs : `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, `Notes/Farm/SPEC_plant_footprint_prompt.md`.
- [x] **Gameplay — grille + plantation** : noyau **`GridData` + `GridManager` + `GridConfig`** + **flux biofiltre** (`BiofiltreManager`, `BiofiltreGridVisualizer`, `BiofiltreCell`, preview `PlantPlacementPreview`, UI `SeedSelectionUI` / `SeedSlotUI`). Détail historique : **`Notes/Farm/TODO_plantation_pipeline.md`** (étapes 1–3 réalisées ; pas de classe `BuildManager`, équivalent fonctionnel ci-dessus).

---

## Prototype (phase actuelle)

- [x] **Inventaire récolte (noyau)** : `PlayerInventory`, `TryAdd` / piles, flux récolte — bouclé pour la phase actuelle ; spec **Partial** / messages joueur à affiner seulement si besoin en playtest.
- [~] **State machine culture** : stades visuels posés (`Seedling`, `BabyLeaf`, `Growing`, `Mature`, `Bolting`) ; finaliser transitions gameplay + règles de récolte.
- [x] **Flux récolte minimal jouable** : validé (grille / panel / `TryAdd` / destruction) — raffinement UX optionnel.
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

- [ ] **[OPTIONNEL] Unity Gaming Services — inventaire** : après **MVP local** validé, enchaîner **Authentication → Cloud Save** (snapshot `{ itemId, quantity }`) puis **Economy** si besoin ; garder une couche client (singleton ou `IInventory…`) comme **vue** — décisions discutées session **2026-04-13**, voir **`PROJECT_LOG.md`**.
- [ ] **[OPTIONNEL] Workflow graphique + IA** : définir et mettre en place (ou intégrer au pipeline) un flux de production visuelle pour la phase polish — génération / itération d’assets avec [Adobe Firefly — AI pour le développement de jeux](https://www.adobe.com/products/firefly/discover/ai-for-game-developers.html), **uniquement** si la direction art retenue en a besoin après validation prototype/performance mobile (documentation et bonnes pratiques licence / usage à valider avant production commerciale).
- [ ] **Polish UI Inventaire** : améliorer lisibilité/hiérarchie visuelle (espacements, contrastes, feedback hover/clic, états vide/plein, cohérence typographique/icônes), puis valider sur résolutions desktop cibles.
- [ ] **Polish technique navigation UI** : benchmark rapide et choix final du mode le plus performant pour Inventaire/Market (**coque persistante**, **Additive Async**, ou alternative), avec règle de chargement partagée pour tous les stages.

- [ ] **[SUPPER_POLISH] graphique animation** création d'une animation d'abeille qui rode autour des plantes en fleurs avant passage fruits/graines

---

## Backlog — nettoyage projet

- [ ] Après validation du concept art : audit des dossiers sous `Assets/_Project/` (ou chemin choisi), suppression des dossiers vides inutiles + `.meta` associés, vérifier `git status` et commit dédié « chore: remove empty asset folders ».
