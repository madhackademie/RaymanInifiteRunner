# Project log — RaymanInfiniteRunner journal chronologique

## 2026-09-04 — Priorité Bezy : sprites onglets HUD

### Décision auteur
- Nouveau P0 : **mise en place + polish des sprites** sur les onglets, **à faire faire par Bezy**.
- L’auteur fournira la liste des changements visuels ; Cursor rédigera les prompts Ph.1–3 ; **validation ensemble** avant envoi Bezy.

### IDs
- `[P0-TAB-SPRITES-001]` — suivi session / docs.
- `[BZ-TAB-SPRITES-001]` — job Bezy `/prefab-ui-3phases`.

### Cible (à figer avec le brief)
- Probable : `NavigationHUD` (`TabAventures` / `TabInventaire` / `TabShop` / `TabVente`).
- Barre inventaire `InventoryFilterBar` seulement si le brief le dit.
- Art Dump → promo `Sprites/UI/` **avant** Bezy (Vague H). Bezy = wiring / polish prefab, pas génération d’images.

### Docs
- Stub prompts (ne pas envoyer) : `Notes/Ui/PROMPTS_Bezi_tab_sprites.md`
- File : `Notes/Bezi/BEZY_QUEUE.md` · `Notes/Ui/TODO_Bezy_polish_semaine.md` job #26

### Prochaine session (référence « tâche du jour »)
1. Brief visuel auteur → prompts `[BZ-TAB-SPRITES-001]` → validation → Bezy.
2. Auteur Inspector : `BiofiltreHudBinder` + `hudPrefab` = `BiofiltreHud`.
3. Playtest HUD world FirstLvl.
4. Reportés : `[P0-FARM-SPRITE-ALPHA-001]`, `[P0-SALE-QTY-RAND-001]`.

---

## 2026-09-02 — Playtest IBC / grille OK

### Décision auteur
- `[P0-FARM-IBC-GRID-001]` **clos** : la grille **correspond** à la cuve et **fonctionne** (deck ↔ 10×10, pose).
- Grille **carrée** (`coordinateMode = Orthogonal`) conservée — l’art `Cuve_IBC` a un dessus carré. Iso 2:1 reportée (nouveau sprite losange requis).

### Suite (référence « tâche du jour »)
1. Auteur Inspector : Add `BiofiltreHudBinder` + `hudPrefab` = `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab`.
2. Playtest HUD world FirstLvl.
3. Reportés : `[P0-FARM-SPRITE-ALPHA-001]`, `[P0-SALE-QTY-RAND-001]`.

---

## 2026-09-02 — Bezy HOST HUD biofiltre clos

### Livré
- `[BZ-FARM-BIOHUD-HOST-001]` Ph.1–3 : `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab` — Canvas World Space, nested PrimaryRow / StarRow / SecondaryRow (pas unpack), `BiofiltreHudView` câblé (preview ★ 1/5).
- Polish optionnel : `sortingOrder` 0 (spec 20), `sizeDelta` 100×100 (spec 800×600) — le binder scale à la grille.

### Suite (plus de skill Bezy sur ce chantier)
- Auteur : Add `BiofiltreHudBinder` sur `Biofiltre` + `hudPrefab` = `BiofiltreHud`.
- Playtest HUD world FirstLvl.
- Priorité #1 inchangée : `[P0-FARM-IBC-GRID-001]` Inspector IBC.

---

## 2026-08-31 — Note icônes quotidiennes + gate Git par session

### Décisions
- Prompt générique icônes UI + file quotidienne : `Notes/Art/PROMPT_generation_icones.md` (Dump, pas promo auto).
- Gate pull Git = **par session de travail**, plus par chat **ni par jour** : tampon local `.cursor/session_pull_ok` (gitignored, une machine = une session). Fermeture = journal. 2e session le même jour = « on reprend » / tâche du jour / autre PC. Garde-fou : tampon `open` d’un autre jour = zombie → re-pull.

### Prochaine session (inchangée)
- `[P0-FARM-IBC-GRID-001]` auteur Inspector IBC.
- Bezy : `[BZ-FARM-BIOHUD-HOST-001]` Phase 1.

---

## 2026-08-31 — Fin session : skill HUD slots (PRIM clos, SEC row P3 en attente)

### Objectif
- Suivi `/prefab-ui-3phases` sur `feature/rework-biofiltre-grid` (pas de Phase 4 skill : un cycle 1–3 par prefab).

### Livré Bezy (review Cursor YAML)
- `[BZ-FARM-BIOHUD-PRIM-001]` **clos** : `UiBiofiltrePrimarySlotRow` (Slot1–3 nested, view wired). Polish optionnel : HLG spacing 4 / 224 au lieu de 8 / 240.
- `[BZ-FARM-BIOHUD-SEC-001]` atome **clos** : `UiBiofiltreSecondarySlot` (Images + SlotView wired). Polish : sizeDelta encore 72×80 (nested row override 48×48).
- Rangée secondaire : Ph.1–3 **OK** (`slots[5]` Secondary1–5, `visibleSlotCount=5`). Polish : HLG spacing 10 (spec 6).

### Décision auteur (fin de session)
- Suite Bezy : **`[BZ-FARM-BIOHUD-HOST-001]`** (`BiofiltreHud.prefab` Ph.1–3). Puis assignation `hudPrefab` auteur + playtest HUD world.
- Priorité #1 prochaine session reste **`[P0-FARM-IBC-GRID-001]`** (Inspector, pas Bezy).

### Prochaine session (référence « tâche du jour »)
1. `[P0-FARM-IBC-GRID-001]` auteur : `BiofiltreIbcSpriteFitter` + `ibcSprite` = `Cuve_IBC`.
2. Bezy : **`[BZ-FARM-BIOHUD-HOST-001]` Phase 1** — `Notes/Bezi/BEZY_QUEUE.md` + `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md`.
3. Reportés : `[P0-FARM-SPRITE-ALPHA-001]`, `[P0-SALE-QTY-RAND-001]`.

---

## 2026-08-31 — Prochaine session : assignation IBC auteur (pas Bezy)

### Décision auteur
- L’assignation `BiofiltreIbcSpriteFitter` / `ibcSprite` sur `Biofiltre.prefab` (World) est **auteur Inspector**, pas Bezy.
- Sprite runtime déjà promu : `Assets/Art/Sprites/Farm/Biofiltre/Cuve_IBC.png` (Dump `Cuve_IBC_deck_carre_plus_face.png`).

### Prochaine session (référence « tâche du jour »)
- **[P0-FARM-IBC-GRID-001]** Prefab Mode `Biofiltre` → Add Component `BiofiltreIbcSpriteFitter` → `ibcSprite` = `Cuve_IBC` → Play Mode alignement deck ↔ grille.
- Bezy (parallèle, skill) : `[BZ-FARM-BIOHUD-SEC-001]` — `Notes/Bezi/BEZY_QUEUE.md`.
- Reportés : `[P0-FARM-SPRITE-ALPHA-001]`, `[P0-SALE-QTY-RAND-001]`.

---

## 2026-08-30 — Playtest grille biofiltre validé (rework sans colliders)

### Décision auteur
- `[P0-FARM-GRID-PLAY-001]` **clos** : grille + clics coordonnées + pose/récolte + pause/recall persistance **fonctionnels** sur `feature/rework-biofiltre-grid`.
- `[P0-FARM-PLANT-TOUCH-001]` **clos** — même playtest (tactile via `FarmPointerInput`).
- Playtest HUD world FirstLvl reporté **après** jobs Bezy HOST (grille déjà OK).

### Changements (docs)
- `Notes/Todo_project.md`, `ASSISTANT_CONTEXT.md`, règles session — statuts `[x]`, note régression `GenerateGrid` 2× corrigée sur la feature.

### Prochaine session
- `[P0-FARM-IBC-GRID-001]` (câbler `BiofiltreIbcSpriteFitter` sur prefab).

---

## 2026-08-30 — Playtest vente étoiles validé

### Décision auteur
- `[P0-SALE-STAR-PLAY-001]` **clos** : playtest des 3 bandeaux (voisinage, bandoulière, vélo) — hover ★ → jauges + texte + fill live **fonctionnel**.

### Changements (docs)
- `Notes/Todo_project.md` — statut `[x]`, retiré des priorités immédiates.
- `ASSISTANT_CONTEXT.md`, règles session — mémoire alignée.

### Prochaine session
- `[P0-FARM-IBC-GRID-001]` (branche `feature/rework-biofiltre-grid`).

---

## 2026-08-29 — Agent VM : scripts HUD slots biofiltre (C# + art promo)

### Livré (branche `cursor/biofiltre-hud-slots-vm-957c`)
- Art promu : `Assets/Art/Sprites/UI/Biofiltre/slotBiofiltrePrimaire.png`, `slotBiofiltreSecondaire.png` (nouveaux GUID, slices `_0`/`_1`/`_2`).
- Scripts : `Assets/Scripts/UI/BiofiltreHud/` — `BiofiltreSlotVisualState`, `UiBiofiltreSlotView`, `UiBiofiltreSlotRowView`, `BiofiltreHudView`, `BiofiltreHudBinder`.
- `GridManager.GetWorldRect()` — AABB monde grille (origine bas-gauche).
- Binder : offsets normalisés + extra monde par instance ; warning fail-closed si `hudPrefab` null.
- **Pas de prefab** YAML (Bezy : `[BZ-FARM-BIOHUD-PRIM-001]` → SEC → HOST).

### Suite
- Bezy Ph.1–3 via `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md`.
- Assigner `hudPrefab` sur l’instance biofiltre (Inspector) après `BiofiltreHud.prefab`.
- Playtest FirstLvl auteur (après Bezy).

---

## 2026-08-29 — Brief agent VM : HUD slots biofiltre (étoiles déjà là)

### Décision auteur
- Mockup `Assets/Art/Mocup/biofiltreInterface_1.png` : HUD world sur chaque biofiltre (★ + slots primaires + slots secondaires).
- **Ne pas recréer** le système d’étoiles (`UiStarSlot` / `UiStarRow`) — le nested dans le HUD.
- Deux prefab-familles **sur le modèle ★** : N slots **verrouillés** (primaire 3, secondaire 5).
- Même HUD pour **tous** les biofiltres, **recalé** (systèmes non carrés, tailles différentes).
- Prefabs + art = Bezy skill `/prefab-ui-3phases`. Agent VM = C# + promo Dump→Sprites **slots UI seulement** (pas la cuve IBC).

### Docs (non commit — auteur)
- `Notes/Farm/PROMPT_agent_vm_biofiltre_hud_slots.md` — prompt collable overnight.
- `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md` — jobs PRIM / SEC / HOST (phases < 3500 car.).
- IDs : `[P0-FARM-BIOHUD-001]`, `[BZ-FARM-BIOHUD-PRIM-001]`, `[BZ-FARM-BIOHUD-SEC-001]`, `[BZ-FARM-BIOHUD-HOST-001]`.

### Hors scope brief
- `[P0-FARM-IBC-GRID-001]` (tâche auteur lendemain).
- Métier prestige / shields / save (`[BL-GDD-007]` reste design).
- Bed skin (chantier annulé). Prefabs YAML = Bezy seulement.

---

## 2026-08-29 — Art IBC 3/4 + priorité demain = grille dans le carré

### Objectif
- Sprite cuve IBC (mockup) en vue face avant, **dessus = carré parfait (4 angles 90°)**.

### Livré (Dump, non promu)
- `Assets/Art/Assets Store Dump/ElementProd/Biofiltre/Cuve_IBC_3quart_carre_parfait.png`
- Réf : `Planteur_carre_3quart` (composition) + `Assets/Art/Mocup/biofiltreInterface_1.png` (IBC).
- Projection **oblique** (pas de perspective) pour garder le carré.

### Décision auteur (prochaine session)
- **[P0-FARM-IBC-GRID-001]** : demain, caler le sprite IBC sur la grille. Référence « tâche du jour ».
- **Contrainte :** le sprite se **redimensionne** pour **accepter** la grille (`GridManager` inchangé). Pas l’inverse (on ne déforme pas cellules / pas / dimensions grille pour l’art).
- Branche : `feature/rework-biofiltre-grid`.

### Hors scope ce soir
- Promo Dump → `Sprites/`.
- Wiring runtime / bed skin (chantier précédent annulé).

---

## 2026-08-29 — Workflow skill Bezy + suppression branche Workspace Rules

### Décision auteur
- Peu de temps Unity → les crédits Bezy ne se consomment pas. Le **skill** `/prefab-ui-3phases` est le chemin de prod (Cursor prépare, auteur lance 2–5 min).
- Abandon de la chaîne « Workspace Rules + GitHub MCP » (brouillon cloud).

### Changements (non commit — auteur)
- `Notes/Bezi/WORKFLOW_skill_prefab_ui.md` — note d’ouverture / recette de lancement.
- Rappels bootstrap : `ASSISTANT_CONTEXT.md`, `WORKFLOW_PROTOCOL.md`, `.cursor/rules/bezy_skill_workflow.mdc` (+ mises à jour `bezy_execution_phases.mdc`, protocoles session).
- Branche remote `cursor/bezi-workspace-rules-skill-76a4` + PR draft #14 fermées/supprimées (contenu skill déjà sur `main` via `7a61b3f`).

### Prochaine session
- Relire `Notes/Bezi/WORKFLOW_skill_prefab_ui.md` à l’ouverture.
- Cursor propose un bloc `/prefab-ui-3phases` prêt (file Bezy `#13` ou job UI suivant).
- Playtest `[P0-SALE-STAR-PLAY-001]` + FirstLvl post-rollback.

---

## 2026-08-29 — Miroir repo du skill Bezi `/prefab-ui-3phases`

### Contexte
- Skill installé et smoke-testé dans Bezi (demande Task ID + chemin `Assets/Prefabs/Ui/` + phase, sans mutation).
- Copie runtime Bezi : `%AppData%\Roaming\com.bezi.app\skills\prefab-ui-3phases\SKILL.md` (hors Git).

### Livré
- `Notes/Bezi/SKILL_prefab_ui_3_phases.md` — miroir Git du skill validé (inserts Prefab Mode Ph.1–2 + never-unpack nested).
- `Notes/Bezi/README_bezi.md` — section setup intégrations.

### Hors scope
- Your Rules Bezi (collage UI, pas un fichier repo sur `main`).
- GitHub MCP (skip : `@` local suffit).

---

## 2026-08-29 — Sync doc : branche de travail `feature/sale-bandeaux`

### Contexte
- Revue des branches : seules `main` (`480a679`) et `feature/sale-bandeaux` (`8da822b`) existent sur le remote.
- La feature est **4 commits d'avance / 0 de retard** sur `main` → merge fast-forward possible. Aucune PR ouverte.
- Les docs de suivi de `main` sont **périmées** (elles annoncent encore `main` comme branche courante).

### Changements (non commit — auteur)
- `.cursor/rules/project_management_session_protocol.mdc` § 5 : mémoire branche + priorités passées de mai 2026 à l'état 2026-08-29.
- `.cursor/rules/session_planning_memory.mdc` : même mise à jour (branche + `[P0-FARM-BIOFILTRE-CLEAN-001]` / `[P0-SALE-STAR-PLAY-001]`).
- `Notes/Todo_project.md` § *Contexte Git* : ajout de l'état remote (avance/retard, PR, doc `main` périmée).

---

## 2026-08-29 — Rollback : chantier « bed skin » biofiltre annulé

### Décision auteur
- Le code biofiltre du dernier commit (`8da822b`) est **trop buggé** (doublon cuve, erreurs Inspector) → **tout annuler** au lieu de réparer.
- Référence de retour retenue : **`main` (`480a679`)**, **pas** `54ebd3a` : le commit précédent contient déjà le `BedSprite` fautif (`transform.Find` ignore les enfants inactifs → création en double). Revenir à `54ebd3a` aurait gardé le bug.
- Le travail **étoiles / bandeaux de vente** du même commit est **intégralement conservé**.

### Fichiers remis à l'état `main`
- `BiofiltreGridVisualizer.cs` (bed sprite, `SetGridVisualVisible`, `ApplyPlantDrawOrder`, rustines Editor de sélection)
- `BiofiltreManager.cs` (`EnsureBedSprite`, toggle `GridLinesRenderer`, `OnPlacementPreviewEnded`)
- `BiofiltreCell.cs`, `GridManager.cs` (getters `EnsureLayoutForEditor`, `OnDrawGizmosSelected`), `PlantPlacementPreview.cs`
- `Assets/Prefabs/World/Biofiltre.prefab`, `Assets/Scenes/FirstLvl.unity` (`GridLinesRenderer` réactivé)

### Fichiers supprimés
- `Assets/Scripts/Data/BiofiltreBedSkin.cs`
- `Assets/Editor/BiofiltreEditorCleanup.cs`, `BiofiltreBedScenePreview.cs`, `BiofiltreBedSkinEditor.cs`
- `Assets/Data/Ferme/BiofiltreBed_Ibc3Quart.asset`, `BiofiltreBed_Bois3Quart.asset`
- `Assets/Prefabs/World/Biofiltre_Bois.prefab`

### Conservé volontairement
- Sprites `Assets/Art/Sprites/Farm/Biofiltre/` (art réutilisable, désormais sans référence).
- Métas d'import laitue (`maxTextureSize` 1024) — contrainte auteur « ne pas toucher aux plantes ».
- Tooltip raccourci de `PlantDefinition.spriteWorldOffset` (commentaire seul).

### Vérifications
- Diff vs `main` sur tout le périmètre biofiltre : **vide**.
- Aucune référence orpheline (`BiofiltreBedSkin`, `EnsureBedSprite`, `BedSprite`, …) ni GUID cassé dans `Assets/`.
- Aucun autre commit de la branche ne touchait ces fichiers → rien d'autre écrasé.

### Régression connue réintroduite (état `main`)
- `GenerateGrid()` est appelé **2×** au démarrage (`BiofiltreGridVisualizer.Start` + `BiofiltreManager.Start`) → double `ClearGrid`, erreur Inspector *« Object at index 0 is null »* si une cellule est sélectionnée en Play. Assumé : c'est l'état d'avant chantier.

### Prochaine session
1. Playtest FirstLvl pour valider le retour à l'état `main` (biofiltre + pose laitue).
2. `[P0-SALE-STAR-PLAY-001]` playtest tooltip étoiles bandeaux vente.

---

## 2026-08-29 — Fin session : biofiltre doublon cuve + prio tooltip ★ vente

### Objectifs session
- Biofiltre visuel : cuve IBC 3/4, grille feedback pose uniquement, sorting bac / grille / plantes.
- Fix Inspector `SerializedObjectNotCreatableException` après Play Mode.

### Changements (non commit — auteur)
- `BiofiltreGridVisualizer` : `EnsureBedSprite` séparé de `GenerateGrid` ; grille masquée hors mode pose.
- `BiofiltreEditorCleanup` : garde sélection Inspector (Play exit).
- `BiofiltreBedScenePreview` : gizmo édition cuve (ne pas superposer si `BedSprite` traîne).
- Erreurs compile CS0103 / CS0122 corrigées (fusion Editor, pas d’appel runtime → Editor).

### Problème ouvert
- **Doublon cuve** visible en jeu / Scene — auteur investiguera (1× `BedSprite` attendu en Play).
- **Contrainte auteur : ne pas toucher aux plantes** (prefabs, sorting, placement).

### Prochaine session (priorité immédiate — auteur)
1. **[P0-FARM-BIOFILTRE-CLEAN-001]** Réparer doublon cuve + simplifier le chantier biofiltre (sans plantes).
2. **[P0-SALE-STAR-PLAY-001]** Playtest tooltip étoiles bandeaux vente (hover ★ → jauges + texte).

### Prochaines étapes reportées
- `[P0-FARM-SPRITE-ALPHA-001]` fond noir laitue
- `[P0-FARM-PLANT-TOUCH-001]` pose tactile mobile

---

## 2026-08-27 — Fin session : prochaine prio fond noir laitue biofiltre

### Objectifs / playtest Android
- Premier Build And Run sur Samsung SM-A137F (ARMv7). Premier build ~2 h (IL2CPP × 2 archi).
- Pose laitue en erreur sur mobile.

### Changements (non commit — auteur)
- `ProjectSettings` : Target Architectures **ARMv7 + ARM64** (le A13 est 32 bits).
- Logs `adb logcat -s Unity` : NRE `PlantPlacementPreview.UpdateGhostPosition` (`Mouse.current` null).

### Décision prochaine session (auteur)
- **Priorité #1 :** `[P0-FARM-SPRITE-ALPHA-001]` fond noir sur salades + sprites laitue dans le biofiltre.
- Puis `[P0-FARM-PLANT-TOUCH-001]` pose tactile. Vente ★ playtest reporté.

### Prochaines étapes
1. Alpha / import sprites `Assets/Art/Sprites/Plantes/Laitue/`
2. Input tactile `PlantPlacementPreview`
3. Cette tâche est la référence « tâche du jour » au prochain bootstrap.

---

## 2026-08-27 — Reco ★4 : pas de lock wait-★5 + éclats

- Ne pas forcer ★5 si le joueur est à ★4 (endgame trop long, bloque le focus).
- Reco : secondaire dès ★3 **y compris ★4** ; primaire seulement à ★5.
- Reward des ★ extra : **éclats** = nombre d’étoiles tuées (3/4/5) pour shields.
- **Pas acté** — à valider auteur. Spec §1.2 `SPEC_biofiltre_slots_shields.md`.

---

## 2026-08-27 — Prestige : kill des étoiles (★3 et ★5)

- **Acté :** tout prestige **tue les ★**, porte 3 ou 5. Pas de checkpoint.
- Intention : **focus / theorycraft** joueur (défenses vs serre vs hybride).
- Slots déjà ouverts **se gardent**. Spec `SPEC_biofiltre_slots_shields.md` §1.

---

## 2026-08-27 — GDD slots / shields biofiltre (prestige ★3 ou ★5)

### Décision auteur
- **Soit** prestige **★3** → ouvre **1 slot secondaire** (5 : slug, souris, oiseau, fourmis, moisissure).
- **Soit** prestige **★5** → ouvre **1 slot primaire** (3 : serre + 2 TBD). Choix par génération (étoiles reset).
- Shields à **niveaux**. Anti-slug : (1) graines consommable, clignote si vide, 1 graine ≈ 5 limaces, raids **nuit + pluie** ; (2) barrière cuivre −50 % ; (3) cuivre électrifié −75 % ; (4) nématodes consommable, 90 % tant qu’actif.
- Serre : voile de forçage → bâche à bulles → géodésique.
- Monnaie des paliers : prochains prestiges **ou** étoiles-monnaie **ou** or — **ouvert**.

### Doc
- `Notes/GDD/SPEC_biofiltre_slots_shields.md`
- Prestige §6 mis à jour (plus « prestige seulement à ★5 »)
- Backlog `[BL-GDD-007]`

---

## 2026-08-27 — Correction GDD : récolte salade XOR graines

- Une plante ne donne **pas** salade puis graines. Choix unique (Mature **ou** Seedling).
- ★1 : 50 salades + 50 graines = **100 plants** ; 100 germinations ≈ une pose par plant récolté.
- Spec `SPEC_progression_xp_joueur_et_biofiltre.md` §3.2 corrigée.

---

## 2026-08-27 — GDD étoiles biofiltre (★1) + cadence

### Décision auteur
- Le biofiltre a aussi un **système d’étoiles** (grammaire bandeaux), **avant** le prestige.
- **★1** (valeurs de travail, playtest) — **toutes** requises :
  1. XP système **240** (note « 80 × 4 » : 80×4=320 ≠ 240 → doc retient 240 = 80×3 j, variante 320 ouverte)
  2. Récoltes **salade** **50**
  3. **Germinations** **100** (réussies vs tentées : **ouvert**, reco V0 = tentées + 2 compteurs save)
  4. **Graines récoltées** **50**
- Cadence cible : **3–5 jours**, **2–3 sessions/j** de **5–7 min** (voire moins). Recaler les nombres, pas la croissance plantes.
- **Correction (même session) :** une plante = **soit** salade **soit** graines (`maxHarvestCount = 1`). 50+50 = **100 récoltes**, pas 50 doubles cycles. 100 germinations ≈ 1:1 avec ces 100 plantes. Le joueur doit splitter.

### Doc
- `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md` §3
- Prestige reste **après** la courbe ★ (`SPEC_prestige_generation_systemes.md` §6)
- Backlog `[BL-GDD-003]`

---

## 2026-08-27 — GDD prestige / génération par système

### Décision auteur
- Prestige **local** (biofiltre, plus tard bandeaux), **pas** un reset de tout le jeu.
- Prestige = **nettoyage + upgrade**. Grille biofiltre **obligatoirement vide** ; sinon **message bloquant** (pas d’arrachage forcé).
- **G1** : +5 % **croissance** seulement (isolation / habillage). **Pas** de quantité.
- **G2** : media meilleure qualité (qualité d’eau **quand** fishtank) **+** +5 % **quantité**. Évite un rush trop fort trop tôt.
- **Cap** vitesse / générations à calibrer plus tard (anti spam salade en 2 s).
- Bandeaux : horizon, relance **après** courbe ★5 (pas à ★3).

### Doc
- Spec : `Notes/GDD/SPEC_prestige_generation_systemes.md`
- Backlog : `[BL-GDD-006]`
- Liens : maturité biofiltre, panneau aquaponique, vente §2.9 / §5.6

### Hors scope session
- Pas de code. Priorité playtest vente inchangée.

---

## 2026-08-26 — Playtest ★ : layout barres cassé → fix Cursor

### Symptôme (capture auteur)
- Texte `Ventes/Salades/Or` déborde à gauche du panneau.
- Pas de mini-barres visibles (Track/Fill existaient mais conteneurs 100×100).

### Fix Cursor
- `NextBlock` + 3 barres : stretch largeur tooltip, hauteur 22.
- VLG `ChildForceExpandWidth`, `LayoutElement.flexibleWidth`.
- Labels barres : overflow Masking + marge 4 px.
- `SaleChannelStarTooltipHost` : rebuild layout barres au Show.

### Suite
- Re-playtest `[P0-SALE-STAR-PLAY-001]`. Bezy Ph.5 optionnel seulement si encore KO.

---

## 2026-08-25 — Fin session : playtest ★ reporté prochaine session

### Décision auteur
- `[P0-SALE-STAR-PLAY-001]` **prochaine session #1** (pas ce soir).

### Livré cette session
- Compteurs tooltip ★ (`current/required`, or canal).
- Jauges Bezy Ph.1–2 + fill/wiring Cursor `[P0-SALE-STAR-BARS-001]`.

### Prochaine session
1. Playtest hover ★ : 3 jauges, texte in-bar, fill live, clic vente OK.
2. `[P0-SALE-QTY-RAND-001]` rand 1–3 salades.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Bezy Ph.2 jauges tooltip ★ OK (+ fix fill Cursor)

### Bezy
- LayoutElement h=22, Track/Fill/Label stretch, TMP overlay, couleurs OK.
- **Limite API :** `fillMethod` / `fillAmount` / `fillOrigin` rejetés → Radial360 par défaut.

### Cursor (patch prefab YAML)
- Fill Horizontal Left : SalesBar 0.4, ItemsBar 0.5, GoldBar 0.25.
- **Ph.3 wiring** aussi fait côté Cursor (host + 3 `SaleChannelStarProgressBarView`).

### Playtest
- `[P0-SALE-STAR-PLAY-001]` → **prochaine session** (décision auteur fin session).

---

## 2026-08-25 — Bezy Ph.1 jauges tooltip ★ OK

### Review Cursor
- `NextBlock` : NextTitle → SalesBar / ItemsBar / GoldBar → NextBody
- Track / Fill / Label ×3, RectTransform only, layer 5
- Host `salesBar`/`itemsBar`/`goldBar` encore vides → **Ph.2 puis Ph.3**

### Suite
- Bezy **Ph.2 maintenant** (`PROMPTS_Bezi_sale_channel_star_bars.md`)

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Prompts Bezy jauges tooltip ★ `[P0-SALE-STAR-BARS-001]`

### Demande
- Barres de progression dans le tooltip, texte `50/2000` **à l’intérieur**.

### Cursor
- Scripts : `SaleChannelStarProgressBarView` + rows snapshot (fill runtime).
- Fallback texte si Bezy pas encore câblé.
- Prompts Ph.1–3 : `Notes/Ui/PROMPTS_Bezi_sale_channel_star_bars.md` (< 3500 car.).

### Bezy
- **Ph.1 maintenant** (shell RectTransform only). Attendre succès avant Ph.2.
- Prefab : `SaleChannelsScreen` uniquement. Pas le bandeau.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Progression ★ dans le tooltip (current/required)

### Demande
- Le joueur ne voyait pas s’il était à 50 or ou 1999 / 2000.

### Cursor
- Persist `GoldEarned` par canal (`sale_channels.json`).
- `RecordSale(channelId, qty, gold)` depuis `TrySell`.
- Tooltip ★ : `○ Or gagné : 50/2000` (style cadenas).
- Or = **gagné via ce canal**, pas le wallet (le shop peut faire baisser l’or en poche).
- Seuils GDD §2.9 : 5 ventes, 50 salades, 2000 or. Upgrade ★2 toujours `[P0-SALE-STAR-001]`.

### Playtest
- Hover ★ voisinage : ventes / salades / or en `current/required`.
- Saves déjà existantes : or estimé à 15 × salades écoulées (prix V0), puis tracking réel à chaque vente.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain). Pas de job Bezy.

---

## 2026-08-25 — Annuler recherche + doublon croix

### Cause
- `SetConfirmInteractable(false)` (or 285 < 300) désactivait aussi **Annuler**.
- Croix sous la carte + Annuler overlay = même sortie en flux Recherche.

### Cursor
- Annuler toujours cliquable.
- Croix masquée tant que `ConfirmOverlay` est visible.

### Pas Bezy
- Wiring prefab déjà OK ; pas de prompt.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Tooltip palier off si bandeau actif

### Demande
- Désactiver le tooltip ★ une fois le bandeau **vendable**. Pas un job Bezy (état runtime, 1 prefab).

### Cursor
- `AllowsStarTooltip` : uniquement en **cooldown**.
- Bandeau actif : raycast `Stars` off + pas de Show. Clic = vente.
- Overlay cadenas : tooltip déblocage inchangé.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Bezy Ph.4 tooltip étoiles sous les ★ OK

### Review Cursor
- `SaleChannelStarTooltip` pivot (0.5, 1), inactif.
- Host : `screenOffset` (0, -16), `canvasEdgePadding` 12.
- Tooltip cadenas inchangé.

### Suite auteur
- Playtest hover ★ : panneau **sous** la rangée, entier à l’écran ; clic bandeau = vente.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Tooltip étoiles hors écran → clamp + Bezy Ph.4

### Playtest
- Panneau palier collé en haut, titre coupé par le bord écran.

### Cursor
- `SaleChannelStarTooltipHost` : rebuild layout + **clamp** dans le parent canvas (padding 12).

### Bezy
- Ph.4 : pivot haut + `screenOffset` (0, -16) pour coller le panneau **sous** les ★.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Bezy Ph.3 étoiles bandeaux OK

### Review Cursor
- `SaleChannelStarHover` sur `Stars` (refs vides).
- `SaleChannelStarTooltipHost` câblé (panel + 4 TMP + NextBlock + CanvasGroup).
- `RuntimeSaleChannelsScreen.starTooltipHost` assigné.
- `starImages[5]` + hover cadenas inchangés.
- 4 TMP : LiberationSans + orthographic UI.

### Suite auteur
- Playtest `[P0-SALE-STAR-PLAY-001]` : hover ★ voisinage → tooltip courant/next ; clic bandeau = vente ; hover cadenas ≠ tooltip étoiles.
- Puis rand ★1 `[P0-SALE-QTY-RAND-001]` (Cursor).

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Bezy Ph.2 étoiles bandeaux OK

### Review Cursor (YAML, capture plein écran ignoré)
- `Stars` : Image α 0.01 raycast ON ; Star1 rose, Star2–5 grises, raycast OFF, 5 enfants.
- `SaleChannelStarTooltip` : panneau dark α 0.94, 320 px, pivot bas, CanvasGroup no raycast, VLG + CSF.
- Unlock tooltip / LockedOverlay intacts.

### Nit (Ph.3)
- 4 TMP du tooltip : `fontAsset` vide + `isOrthographic=0` — copier police unlock TitleLabel.

### Suite
- Envoyer **Ph.3** wiring + fix TMP.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Bezy Ph.1 étoiles bandeaux OK

### Review Cursor
- `SaleChannelStarTooltip` inactif, layer 5, dernier sibling racine `SaleChannelsScreen`.
- Hiérarchie : CurrentBlock (CurrentTitle/Body) + NextBlock (NextTitle/Body) — RectTransform only.
- Tooltip cadenas + prefab bandeau intacts.

### Suite
- Envoyer **Ph.2** (`PROMPTS_Bezi_sale_channel_stars.md`) — visuels ★ + panneau tooltip.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-25 — Prompts Bezy étoiles bandeaux `[P0-SALE-STAR-UI-001]`

### Fait
- Prompts phasés : `Notes/Ui/PROMPTS_Bezi_sale_channel_stars.md` (Ph.1 shell tooltip / Ph.2 visuels / Ph.3 wiring).
- Panneau **dédié** `SaleChannelStarTooltip` (pas recycle cadenas — collision hover).
- Scripts Cursor prêts pour Ph.3 : `SaleChannelStarHover`, `SaleChannelStarTooltipHost`, `SaleChannelStarUiCopy`, hook `RuntimeSaleChannelsScreen` + `ApplyStarFill` sur le bandeau.

### Décision
- Brûler crédits Bezy **avant reset 30 août** : envoyer **Ph.1 seulement**, attendre succès.
- Rand ★1 `[P0-SALE-QTY-RAND-001]` reste Cursor, en parallèle.

### Prochaine étape
- Auteur : coller Ph.1 Bezy (prefab `SaleChannelsScreen` ouvert).
- Puis Ph.2 → Ph.3 → playtest `[P0-SALE-STAR-PLAY-001]`.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-19 — Bezy Ph.4 unlock bandeau (stretch ancre)

### Review Cursor
- `UnlockableFxAnchor` : stretch-fill `LockedOverlay`, **1er sibling**.
- `SparkleImageSecondary` : retiré.
- `SparkleImage` : conservé (sprite source) — **forcé inactif** (Bezy l’avait laissé actif + stretch, flash plein bandeau).

### Hook Cursor
- Inchangé : `SaleChannelUnlockableSparkleVfx` sur toute la surface en « Prêt ! ».

### Playtest auteur
- Sparkle animé sur **toute la surface** du bandeau Vélo « Prêt ! » — **OK 2026-08-19**.
- **Règle :** même logique unlock/sparkle/cooldown/★ sur le dernier bandeau **et les prochains** — `[BL-SALE-BANDEAU-TPL-001]`, spec §8.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-19 — Prochaine session = rand ★1 + polish étoiles 3 bandeaux

### Décision auteur
- **[P0-SALE-QTY-RAND-001]** (déjà indiqué, rappel) : après fin timer, **rand 1–3 salades** au ★1.
- Enchaîner polish **3 bandeaux vente** : système d’étoiles + images + tooltip **valeur palier courant / suivant**.
- Tooltip : **hover rangée d’étoiles** (recommandé / acté pour la prochaine session) — pas tooltip général sur tout le bandeau.

### Trace todo
- Ordre *Prochaine session* : RAND → playtest RAND → `[P0-SALE-STAR-001]` → `[P0-SALE-STAR-UI-001]` → playtest étoiles.
- **Référence « tâche du jour » :** `[P0-SALE-QTY-RAND-001]` en premier, puis polish étoiles.
- Onglets inventaire `[P0-INV-TABS-001]` après ce lot vente.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-19 — Prochaine session = random 1–3 salades ★1 `[P0-SALE-QTY-RAND-001]`

### Décision auteur
- Déblocage vélo : **25 salades** écoulées = OK (plus de ventes, pas baisser le seuil).
- **★1 bandeau** : une fois le **timer / cooldown** terminé, quantité **aléatoire entre 1 et 3 salades** (remplace le plafond fixe 2).
- Étoiles 2+ : plus tard.

### Trace todo
- [ ] **[P0-SALE-QTY-RAND-001]** + playtest **[P0-SALE-QTY-RAND-PLAY-001]** en tête de *Prochaine session* (`Notes/Todo_project.md`).
- Sera la référence au prochain « tâche du jour ».
- Onglets inventaire `[P0-INV-TABS-001]` restent ensuite.

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-19 — Hook sparkle déblocage bandeau (`UnlockableFxAnchor`)

### Fait
- Bezy Ph.3 livrée : `UnlockableFxAnchor` + sparkles sous `LockedOverlay`.
- Cursor : `SaleChannelBandeauView` active l’ancre **uniquement** en état « Prêt ! » (`Unlockable`) ; Find path une fois puis cache.
- Docs : `PROMPTS_Bezi_sale_channel_unlock_ui.md`, `Notes/Todo_project.md` `[P0-SALE-BEZI-UNLOCK-003]`, spec §8.

### Playtest auteur
- HUD → Vente → bandeau Vélo « Prêt ! » : sparkle visible + pulse cadenas.
- Autres états (Bientôt / recherche / débloqué) : sparkle off.

### Suite
- **[P0-INV-TABS-001]** onglets inventaire (priorité prochaine session).

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-19 — Playtest déblocage canaux vente OK + doc Bezy unlock

### Fait
- Playtest auteur **[P0-SALE-PLAY-005]** validé : tooltip hover, état « Prêt ! », confirmation recherche, timer déblocage.
- Docs mises à jour : `PROMPTS_Bezi_sale_channel_unlock_ui.md` (Ph.1–2 clos, checklist cochée, prompt Ph.3 prêt).
- `Notes/Todo_project.md` : tâches unlock Cursor + Bezy Ph.1–2 + playtest clos ; Ph.3 sparkle optionnel `[P0-SALE-BEZI-UNLOCK-003]`.

### Reste Bezy vente (non bloquant)
- **[P0-SALE-BEZI-UNLOCK-003]** sparkle `UnlockableFxAnchor` sur bandeau (optionnel).
- **[BZ-POLISH-018]** Ph.3 VFX pièces — bloqué art `Assets/Art/Sprites/VFX/Sale/`.

### Suite (priorité inchangée)
- **[P0-INV-TABS-001]** onglets inventaire (prochaine session).

### Crédits Bezy
- Reset mensuel le **30 août** (prochain).

---

## 2026-08-18 — Spec inventaire multiverse + prompts Bezy onglets

### Décisions auteur (GDD)
- Stock **unique** ; onglets = vues filtrées **par défaut** (pas de double sac silencieux).
- **Vue jeu** : défaut, **slots limités** par jeu.
- **Vue Tout** : cross-jeu, **sans limite** slots jeu — gérer / vendre / craft depuis n’importe où.
- Transfert manuel entre sacs : **possible plus tard**, pas flux défaut.
- **Craft cross-jeu** : toujours autorisé.
- **Équipement équipé** : propre au jeu où porté.
- **Utiliser** item : situationnel / metadata par jeu — **TBD**, pas V0.
- **Market** : filtres riches — **TBD**.

### Livrables docs
- `Notes/GDD/SPEC_inventaire_multiverse_hub.md` (nouveau)
- `Notes/Ui/PROMPTS_Bezi_inventory_tabs.md` — Bezy `[BZ-INV-TABS-001]` Ph.1→3

### Suite immédiate
1. Bezy Ph.1 barre onglets `InventoryScreen.prefab`
2. Cursor : `InventoryFilterTabBar.cs` + `ItemCategory` + filtre `InventoryUI`
3. Playtest `[P0-INV-TABS-PLAY-001]`

---

## 2026-08-18 — Prochaine session = onglets inventaire `[P0-INV-TABS-001]`

### Décision auteur
- Subdiviser l’inventaire (grille basse) avec des **onglets** : **Graines** vs **Consommables** (anti-fourmi, test eau, etc.).
- Remplace `[BZ-POLISH-013]` comme référence « tâche du jour ».

### Trace todo
- [ ] **[P0-INV-TABS-001]** + playtest **[P0-INV-TABS-PLAY-001]** en tête de *Prochaine session* (`Notes/Todo_project.md`).
- Sera la référence au prochain « tâche du jour ».

### Intention (pas encore implémenté)
- Data : catégorie item (aujourd’hui `ItemDefinition` = Standard / Currency seulement).
- UI : `filterBarPlaceholder` inactif dans `InventoryScreen` — barre onglets (Bezy) + filtre grille (Cursor).
- À trancher en session : onglet **Récoltes** (laitue / prod) en 3ᵉ, ou pas.

### Ensuite
1. File Bezy **#13** `[BZ-POLISH-013]` audit layers
2. Commit auteur lot session + docs si besoin

### Crédits Bezy
- Reset le **30** de chaque mois (prochain : **30 août**).

---

## 2026-08-18 — Playtest HomeScene `[P0-HOME-PLAY-012]` validé

### Décision auteur
- Playtest hub Home (ACCUEIL + bouton + FirstLvl) **OK** — « tout roule ».

### Trace
- [x] `[P0-HOME-PLAY-012]` + `[BZ-POLISH-012]` **clos** dans `Notes/Todo_project.md`.

### Prochaine session (référence « tâche du jour »)
1. **[BZ-POLISH-013]** Audit layers UI — file Bezy #13
2. Commit auteur lot session + docs si besoin

### Crédits Bezy
- Reset le **30** de chaque mois (prochain : **30 août**).

---

## 2026-08-05 — Prochaine session = playtest HomeScene `[P0-HOME-PLAY-012]`

### Décision auteur
- Reporter le playtest hub Home en **priorité prochaine session** (pas traité maintenant).

### Trace todo
- [ ] **[P0-HOME-PLAY-012]** placé en tête de *Prochaine session* (`Notes/Todo_project.md`).
- Sera la référence au prochain « tâche du jour ».

### Checklist playtest (rappel)
- Bootstrap → Home : titre **ACCUEIL** ; bouton « Commencer l'aventure » lisible / tap OK ; clic → FirstLvl ; marges header/HUD.

### Après playtest OK
1. Clos `[BZ-POLISH-012]` + `[P0-HOME-PLAY-012]` — **fait 2026-08-18**
2. File Bezy **#13** `[BZ-POLISH-013]`
3. Commit auteur lot session + docs

### Crédits Bezy
- Reset le **30** de chaque mois (prochain : **30 août**).

---

## 2026-08-05 — `[BZ-POLISH-012]` HomeScene Bezy Ph.1–3 OK

### Fait (Bezy)
- Ph.1–2 : `MapNodeButton` layers 5 + h=112 + Label 36 / Subtitle 22
- Ph.3 : `HeaderTitle` « ACCUEIL » + padding NodesContainer 48/48/140/120 + Background `(0.07,0.07,0.12)`
- **Blocage résolu :** scène `HomeScene` doit être **ouverte** dans l’Editor pour persister les writes Bezy

### Suite (mise à jour)
- Playtest reporté → **[P0-HOME-PLAY-012]** prochaine session (entrée ci-dessus).

### Prompts
- `Notes/Ui/PROMPTS_Bezi_home_012.md`

---

## 2026-08-05 — Playtest `[BZ-POLISH-011]` LoadingScreen validé

### Décision auteur
- Playtest Bootstrap (barre 0→100 %, %, fade) **OK**.

### Trace
- `[BZ-POLISH-011]` **clos** (Bezy Ph.1–3 + playtest) dans `Notes/Todo_project.md`.

### Prochaine session (référence « tâche du jour »)
1. **[BZ-POLISH-012]** HomeScene — file Bezy #12 (boutons + titres lisibilité mobile)
2. Suite #13 audit layers
3. Commit auteur lot session + docs

### Crédits Bezy
- Reset le **30** de chaque mois (prochain : **30 août**).

---

## 2026-08-05 — `[BZ-POLISH-011]` LoadingScreen Bezy Ph.1–3 OK

### Fait (Bezy)
- Ph.1 : layers UI=`5` sous `LoadingCanvas`
- Ph.2 : `ProgressBarContainer` inset + safe bottom ; track h=28 ; `%` font 36 Bold
- Ph.3 : Background `(0.06,0.06,0.08)` ; SplashImage placeholder α `0.35`
- Fill toujours via `anchorMax.x` + `HorizontalGradient` ; SerializeFields intacts

### Note review
- Sprites Image encore `null` (Background / Bg / Fill) — OK Editor souvent ; build si noir → UISprite

### Suite
- Playtest auteur — **validé** (entrée suivante).

### Prompts
- `Notes/Ui/PROMPTS_Bezi_loading_011.md`

---

## 2026-08-05 — Playtest layout talents Commerce + PA validé

### Décision auteur
- Point 1 (playtest Inventaire → P1 Commerce : contrastes + fond/filigrane + HUD PA haut-droite) **validé**.

### Trace todo
- [x] Playtest auteur global layout talents + PA — clos dans `Notes/Todo_project.md`.
- Conflit Git résiduel dans *Contexte Git* du todo **nettoyé** ; branche doc = **`main`**.

### Prochaine session (référence « tâche du jour »)
1. **[BZ-POLISH-011]** LoadingScreen — file Bezy #11 (layout barre + %)
2. Suite file #12→#13
3. Commit auteur lot polish si besoin

### Crédits Bezy
- Reset le **30** de chaque mois (prochain : **30 août**).

---

## 2026-07-30 — Layers fond + filigrane (sans déformer l’art)

### Demande auteur
- Remettre filigrane non déformé ; fond opaque séparé derrière.

### Fix
- `FondPanel` (ex-FiligraneBackdrop) : stretch plein host, Image opaque + UISprite builtin.
- `Filigrane` : ancre centre, `960×960`, `PreserveAspect = 1`, alpha 0.14.

### Playtest
- Inventaire → P1 Commerce : fond plein + balance non étirée.

---

## 2026-07-30 — Fix décalage TreeMount / filigrane (Cursor)

### Cause
- Filigrane `PreserveAspect` → lettrebox (pas plein cadre).
- Runtime `SetAsLastSibling` sur `TreeMountHost` (mauvais ordre draw).
- `BodyPlaceholder` encore inset actif possible.

### Fix
- `PreserveAspect = 0` sur Filigrane ; BodyPlaceholder inactive + stretch 0.
- `SetAsFirstSibling` + `NormalizeTreeMountHostLayout` à chaque resolve.

### Playtest
- Inventaire → P1 Commerce : fond/filigrane bord à bord OverlayPanel ; titre/Retour au-dessus ; PA haut-droite.

---

## 2026-07-30 — `[CT-UI-SAFE-PA-001]` TreeMount plein + HUD PA haut-droite

### Fait (Cursor — demande auteur implement plan)
- `TreeMountHost` sizeDelta `(0,0)` + `OverlayPanel` alpha 1 ; sibling order fond puis `TrackTitle`/`BackButton`
- `EnsureRuntimeTreeMountHost` offsets `(0,0)`
- `ActionPointsHudWidget` + instance `NavigationHUD` : anchors top-right, pos `(-16,-16)`
- Doc : `Notes/Ui/CONVENTION_hud_pa_safe_zone.md` + prompts archive `PROMPTS_Bezi_hud_pa_safe_zone.md`

### Suite
1. Playtest auteur Inventaire → P1 Commerce
2. Commit auteur

---

## 2026-07-30 — Mémo crédits Bezy : reset le 30 du mois

### Décision / rappel
- Les crédits Bezy se **réinitialisent en dur le 30 de chaque mois** (pas le 1er, pas le dernier jour si ≠ 30).
- Documenté pour rappel à chaque session : `ASSISTANT_CONTEXT.md`, `Notes/Bezi/README_bezi.md`, `WORKFLOW_PROTOCOL.md`, règles Cursor Bezy / gestion projet, `TODO_Bezy_polish_semaine.md`.

### Suite session (inchangée)
- Bezy `[BZ-POLISH-010]` Ph.2 fond opaque derrière filigrane.

---

## 2026-07-30 — Art filigrane Commerce importé → Bezy `[BZ-POLISH-010]`

### Fait (Cursor)
- PNG déplacé : `Assets/Art/Sprites/UI/Progression/CommerceFiligrane.png`
- Import Sprite single + `alphaIsTransparency` (fond déjà transparent)
- Prompts Bezy : `Notes/Ui/PROMPTS_Bezi_talent_filigrane_010.md`

### Suite
1. Bezy Ph.1 `[BZ-POLISH-010]` — Image `Filigrane` sous `TreeMountHost`
2. Playtest auteur Inventaire → P1 Commerce

### Commit
- À faire côté **auteur**.

---

## 2026-07-30 — Prochaine session = filigrane Commerce (art ChatGPT)

### Décision auteur
- Pas de job Bezy filigrane **aujourd’hui**.
- Priorité **prochaine session** : générer l’image filigrane Commerce via ChatGPT, puis import Unity + Bezy `[BZ-POLISH-010]`.

### Intent art (rappel)
- Emblème soft : balance + pièce / étiquette (acheteur vs vendeur).
- Flat vector, PNG transparent 1024×1024, peu de détail au centre.
- En jeu : Image sous `TreeMountHost`, alpha ~0.08–0.15.

### Trace todo
- [ ] **[P0-ART-FILIGRANE-001]** + chaîne import → `[BZ-POLISH-010]` placés en *Prochaine session* (`Notes/Todo_project.md`).
- Sera la référence au prochain « tâche du jour ».

### Session du jour (fait)
- `[BZ-POLISH-009]` Bezy Ph.1–3 OK (TreeMountHost + contrastes nœuds/edges).

### Commit
- À faire côté **auteur**.

---

## 2026-07-30 — Idea backlog : récolte grille en un clic

### Décision
- Feature **QoL** notée pour plus tard (pas d’implémentation maintenant) : bouton qui récolte **tout** ce qui est récoltable sur la grille biofiltre (simule clic + validation par plante).
- Accès conditionné à monétisation : **pub** / **monnaie issue des pubs** / **pass NoPub** (choix design à faire plus tard).

### Trace todo
- [ ] **[BL-FARM-HARVEST-ALL-001]** ajouté dans `Notes/Todo_project.md` § Backlog → *Ferme — QoL monétisé*.

### Suite session (inchangée)
1. Prochain Bezy : `[BZ-POLISH-010]` filigrane Commerce
2. Playtest auteur `[BZ-POLISH-009]` talents Commerce

---

## 2026-07-29 — `[BZ-POLISH-008]` Bezy Ph.1–3 OK + hooks Open/Close

### Review
- Ph.1 layers 5 — OK
- Ph.2 lisibilité — OK
- Ph.3 Open/Close scale+alpha sur Panel — OK

### Cursor
- [x] `ResourceFeedbackPopupUI` triggers Open/Close + hide après 0.14 s
- [x] `panelAnimator` câblé sur prefab

### Suite
1. Prochain Bezy : `[BZ-POLISH-009]` talents Commerce
2. Batch playtests (E HUD PA, inventaire plein, toast)

### Commit
- À faire côté **auteur**.

---

## 2026-07-29 — `[BZ-POLISH-007]` Bezy Ph.1–3 OK + hook Show

### Review
- Ph.1 layers 5 — OK
- Ph.2 lisibilité (font 32, icon 100) — OK
- Ph.3 ShowPunch scale only on LootFlyoutGroup — OK (pas d’alpha)

### Cursor
- [x] `HarvestRewardFeedbackPopupUI.PlayShowPunch()` → trigger `Show`

### Suite
1. Prochain Bezy : `[BZ-POLISH-008]` popup inventaire plein
2. Batch E playtest HUD PA / toast récolte

### Commit
- À faire côté **auteur**.

---

## 2026-07-29 — `[BZ-POLISH-005]` Bezy Ph.1–3 OK + wire empty catalogue

### Review Bezy
- Ph.1 layers 5 — OK
- Ph.2 grille / close / backdrops — OK
- Ph.3 `EmptyCataloguePanel` inactive + textes — OK

### Cursor
- [x] `RuntimeShopScreen.emptyCataloguePanel` + Show/Hide si 0 offre / erreur catalogue

### Suite
1. Prochain Bezy : `[BZ-POLISH-007]` toast / feedback récolte (`[BZ-POLISH-006]` déjà clos)
2. Batch E playtest HUD PA

### Commit
- À faire côté **auteur**.

---

## 2026-07-29 — Hooks `[BZ-POLISH-002]` + prompts Bezy `[BZ-POLISH-005]`

### Fait (Cursor)
- [x] `ActionPointService.OnSpendRefused` quand PA insuffisants
- [x] `ActionPointsHudView` → trigger Animator `Refuse`
- [x] `ActionPointFatigueTooltipHost` → FadeIn/FadeOut + hide après 0.12 s
- [x] Prompts Bezy shop screen : `Notes/Ui/PROMPTS_Bezi_shop_screen_005.md`

### Suite
1. Bezy Ph.1 `[BZ-POLISH-005]` ShopScreen layers
2. Batch E playtest HUD PA (Refuse / Fill / tooltip)

### Commit
- À faire côté **auteur**.

---

## 2026-07-29 — `[BZ-POLISH-002]` Bezy Ph.1–3 OK (HUD PA)

### Review Cursor
- Ph.1 RefuseShake + trigger `Refuse` — OK (pulse Row ; pas d’euler Z)
- Ph.2 SpendPulse + path `ProgressBar/BarFill` — OK
- Ph.3 Tooltip layer 5 + CanvasGroup + FadeIn/Out 0.12 s — OK

### Suite
1. Hooks Cursor : `Refuse` si PA insuffisant + tooltip fade
2. Batch E playtest (`Notes/Todo_playtest.md`)
3. Prochain Bezy file : #5→#16

### Commit
- À faire côté **auteur**.

---

## 2026-07-29 — File playtest batch + cheatsheet PA/inventaire

### Fait
- [x] `Notes/Todo_playtest.md` : cheatsheet forcer PA / inventaire / vente / Flowering à la mano
- [x] Batches A–E listés + F clos (drop/insecte/DirtBurst)
- [x] Context Menu debug sur `ActionPointService` : remaining 0 / 1 / refill / delete save
- [x] Pointeur depuis `Notes/Todo_project.md` + `PLAYTEST_points_actions_v0.md`

### Pourquoi
- Auteur veut faire les playtests **en batch** et savoir comment modifier l’état à la mano.

### Commit
- À faire côté **auteur**.

---

## 2026-07-29 — Playtests validés ; prochaine = HUD PA Bezy

### Contexte
- Branche : **`polish/ui-bezy`**
- Auteur : tous les playtests de la priorité immédiate **passés et validés**.

### Clos (playtests auteur)
- [x] **[P0-INV-DROP-PLAY-001]** Inventaire drop (stock → détail → Jeter → compost → retrait slot)
- [x] **[P0-INV-DROP-001]** Bezy Ph.1–4c drop / compost
- [x] **[BZ-POLISH-014]** / **[P0-FARM-INSECT-PLAY-001]** Insecte Flowering
- [x] **[P0-FARM-VFX-PLAY-002]** DirtBurst plant / arrachage / récolte

### Prochaine session (confirmée)
1. **[BZ-POLISH-002]** HUD PA suite polish (Refuse pulse, fill conso, tooltip fade) — job Bezy prioritaire
2. Ne pas relancer wallet punch (`[BZ-POLISH-015]` PARK UX)
3. Suite file Bezy #5→#16 + Cursor `[P0-AP-CODE-002]` si utile

### Trace
- `Notes/Todo_project.md` § Prochaine session mis à jour
- Sera reprise au prochain « tâche du jour »

### Commit
- À faire côté **auteur** (docs + lot drop/insecte/VFX si pas encore poussé).

---

## 2026-07-28 (fin session) — Prochaine = playtest inventaire drop

### Contexte
- Branche : **`polish/ui-bezy`**
- Inventaire drop monté (scripts + Bezy + compost) ; inventaire joueur **vide** → playtest bloqué.

### Prochaine session (confirmée)
1. **Remplir l’inventaire** : FirstLvl → planter / pousser / **récolter** laitues (ou graines) pour avoir ≥1 stack
2. **[P0-INV-DROP-PLAY-001]** Playtest drop : clic slot → quantité/Max (stack) → Jeter → confirm → compost → retrait slot
3. Si besoin : valider / relancer Bezy **Ph.4c** (compost +100 px + chute sur tas)
4. Ensuite : playtest insecte Flowering / DirtBurst

### Trace
- Priorité écrite dans `Notes/Todo_project.md` § Prochaine session
- Sera reprise au prochain « tâche du jour »

### Commit
- À faire côté **auteur** (lot inventaire drop + insecte + docs).

---

## 2026-07-28 — Insecte Flowering : rand Bee/Butterfly + sens path

### Décisions
- Au **démarrage** Flowering uniquement : 50 % abeille / 50 % papillon ; 50 % sens nodes +1 / −1
- Pas de re-roll à chaque node
- `PlantDefinition.insectKind` : ajout `RandomBeeOrButterfly` (Laitue = 3)

### Fait
- [x] `InsectPathFollower` : `pathDirection` rand + `ApplyVisualKind` (swap controllers)
- [x] `InsectPathAnchor.SetPathActive(active, visualKind)`
- [x] `PlantGrow.SyncInsectPathForStage` → `ResolveRuntimeInsectKind()`
- [x] Art/anim papillon : `Butterfly_Fly.anim` + `Butterfly.controller`
- [x] Prefab `Bee` : refs `beeController` + `butterflyController`
- [x] Spec maj : `Notes/Farm/SPEC_insecte_flowering.md`

### Playtest
1. Forcer Flowering (laitue) plusieurs fois → parfois bee, parfois butterfly
2. Sens de parcours nodes parfois inverse
3. flipX selon direction de vol

### Fichiers
- `Assets/Scripts/Farm/Insect*.cs`, `PlantGrow.cs`, `PlantDefinition.cs`
- `Assets/Art/Animations/Farm/Insects/Butterfly*`
- `Assets/Prefabs/World/Insects/Bee.prefab`, `Assets/Data/Ferme/Laitue.asset`
- `Notes/Farm/SPEC_insecte_flowering.md`, `Notes/Todo_project.md`

### Commit
- À faire côté **auteur** — assistant non committer.

---

## 2026-07-28 — Inventaire détail item / drop (scripts + Bezy)

### Contexte
- Demande : clic item inventaire → popup détail + description + drop quantité/Max + surpopup confirmation.

### Fait (Cursor — scripts uniquement, pas de prefab)
- [x] `PopupId.InventoryItemDetail`
- [x] `ShopItemPopupFlowMode.Drop` + labels / ConfirmOverlay drop
- [x] `ItemDefinition.Description`
- [x] `InventoryUI` : clic slot → host générique → `TryRemove`
- [x] Docs : `Notes/Ui/popup_generique.md`, `Notes/Ui/PROMPTS_Bezi_inventory_item_drop.md`

### À faire (Bezy — prefab / scène / anims)
1. [x] **Phase 1** : binding — OK
2. [x] **Phase 2** : description — OK noop
3. [x] **Phase 3** : open anim — OK noop
4. [x] **Phase 4** : compost / `DropToTrash` — OK Bezy + **art CompostDrop slicé** (2026-07-28) sous `Art/Sprites/UI/Inventory/DropCompost/` ; TrashBin + flipbook anim ; menu `Rayman/UI/Bind Compost Drop Sprites`
5. Phase 5 optionnelle : prefab dédié — skip OK
6. **Playtest auteur** — prochaine

### Hooks Cursor (anims)
- Ouverture : `ShopItemPopupView` → `animator.SetBool("IsOpen", …)`
- Drop confirm : `PlayDropTrashAnimation` → trigger `DropToTrash` → callback `TryRemove`

### Commit
- À faire côté **auteur** (scripts + docs + prefab/anims Bezy + art compost).

---

## 2026-07-28 (suite) — Bezy P4b drop motion OK

- TrashBin Y **110** ; FlyingIcon chute vers compost (clip retuné)
- Prochaine : playtest auteur inventaire drop

---

## 2026-07-25 — VFX plantation branché (hook Play)

### Contexte
- Branche : **`feature/points-actions`**
- Prefab P1–P3 livré (`PlantingDirtBurst` + materials + sprites)

### Vérif livrable
- [x] DirtBurst + WormBurst, Play On Awake OFF
- [x] Materials dirt/worm + Texture Sheet Sprites `_0…_10` / worm `_0`

### Fait (Cursor)
- [x] `FarmDirtBurstVfx` + `BiofiltreManager.PlayPlantingDirtBurst` (plant joueur, pas restore save)
- [x] Burst dans `PlantHarvestInteractor.RemovePlantFromGrid` (récolte + arrachage)
- [x] Prefab assigné sur `Biofiltre.prefab`

### Prochaine (auteur)
1. **[P0-FARM-VFX-PLAY-002]** Playtest in-game
2. Retune si trop petit/rapide (`[P0-FARM-VFX-TUNE-001]`)

### Commit
- À faire côté **auteur**.

---

## 2026-07-25 (fin session) — VFX PlantingDirtBurst P3 + prio retune

### Contexte
- Branche : **`feature/points-actions`**
- Chantier : `[BZ-POLISH-016]` / sprites dirt + worm

### Fait
- Phase 3 partielle Bezy (material + Worm burst 2–5) ; sprites YAML puis binder Editor Cursor
- Fix Mesh Type **Full Rect** ; materials BaseMap dirt/worm ; menu `Rayman/VFX/Bind PlantingDirtBurst Sprites`
- Règle prompts Bezy : ne pas exiger Simulate / playtest comme succès Bezy (`bezy_execution_phases.mdc`, `Notes/Bezi/README_bezi.md`)
- Fix compile Unity 6 : `TextureImporterSettings.spriteMeshType` (plus `TextureImporter.spriteMeshType`)

### Feedback auteur
- Particules **trop petites** et **trop rapides** → retune prochaine session (pas ce soir)

### Prochaine session (priorité immédiate) — validée auteur
1. **[P0-FARM-VFX-TUNE-001]** Réglage + test dirt particles (`PlantingDirtBurst`) — taille + durée/vitesse
2. Puis hook `Play()` plant / arrachage / récolte

### Commit
- À faire côté **auteur** (prefab, mats, binder, docs).

---

## 2026-07-23 (fin session) — VFX plantation P2 validé + playtest priorité

### Contexte
- Branche : **`feature/points-actions`**
- Chantier : `[BZ-POLISH-016]` / `[CT-FARM-POLISH-003]`

### Validation Phase 2 (Cursor, repo) — **OK**
- Size 1→0.4, Color alpha→0, Velocity radial 1.2, Rotation Dirt ±180° / Worm ±90°
- Play On Awake OFF, Rate=0, Burst 14/1

### Prochaine session (priorité immédiate) — validée auteur
1. **[P0-FARM-VFX-PLAY-001]** Playtest P2 `PlantingDirtBurst` (Simulate / scène)
2. Si OK → Phase 3 sprites puis hook `Play()`

### Commit
- À faire côté **auteur**.

---

## 2026-07-23 — HUD Vente cooldown validé + polish PA en priorité prochaine

### Contexte
- Branche : **`feature/points-actions`**
- Playtest auteur : cooldown voisinage **OK** (overlay + timer visibles après fix Animator/alpha).

### Clos polish
- [x] **[BZ-POLISH-004]** HUD Vente — fade overlay cooldown, pulse timer, locked Bandoulière/Vélo lisible

### Prochaine session — priorité polish (validée auteur)
1. **[BZ-POLISH-002]** **HUD PA suite**
   - Pulse / shake léger si PA insuffisants (Refuse)
   - Micro-anim fill à la consommation
   - Tooltip fatigue fade in/out (Ph.4 déjà structurée)
2. Suite file Bezy #5→#16 (VFX plantation P2 reste dans la file farm / `[BZ-POLISH-016]`)

### Docs mises à jour
- `Notes/Todo_project.md` — section *Prochaine session — file Bezy polish*
- `Notes/Ui/TODO_Bezy_polish_semaine.md`
- `PROJECT_LOG.md`

### Commit
- À faire côté **auteur**.

---

## 2026-07-23 (fin session) — VFX plantation P1 + todo P2

### Contexte
- Branche : **`feature/points-actions`**
- Chantier : `[BZ-POLISH-016]` / `[CT-FARM-POLISH-003]` — particles plantation (recyclable arrachage / récolte)

### Fait
- [x] Import + config sprite vers : `Assets/Art/Sprites/VFX/Planting/wurmParticleFarmPlantation.png` (Multiple, slice serrée, pivot centre, alpha OK)
- [x] Prompts Bezy phasés : `Notes/Ui/PROMPTS_Bezi_planting_dirt_vfx.md`
- [x] **Phase 1 Bezy OK** : prefab `Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab` (`DirtBurst` + `WormBurst`, bursts 14/1, duration 0.6)
- [x] Correctif Cursor : `playOnAwake` OFF sur les 2 PS
- [x] Décision rappelée : **pas** de Sprite Atlas Unity pour ce VFX (sheet Multiple suffit)

### Pas fait (reporté)
- Phase 2 tuning (Color/Size/Velocity/Rotation over Lifetime)
- Phase 3 sprites + material
- Hook Cursor `Play()` plant / arrachage / récolte

### Prochaine session (priorité immédiate) — validée auteur
1. **[P0-FARM-VFX-001]** Contrôle / envoi **Phase 2** particules plantation (`PlantingDirtBurst`)
2. Puis Phase 3 si P2 OK ; insecte Flowering ensuite (`[P0-FARM-INSECT-001]`)

### Fichiers
- `Assets/Art/Sprites/VFX/Planting/wurmParticleFarmPlantation.png` (+ `.meta`)
- `Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab`
- `Notes/Ui/PROMPTS_Bezi_planting_dirt_vfx.md`
- `Notes/Ui/TODO_Bezy_polish_semaine.md`
- `Notes/Todo_project.md`
- `PROJECT_LOG.md`

### Commit
- À faire côté **auteur** — assistant non committer.

---

## 2026-07-23 — Vente bandeau cooldown polish `[BZ-POLISH-004]` livré

### Fait (Bezy)
- [x] Assets `SaleChannelBandeau*` (FadeIn alpha + Pulse label)
- [x] Prefab : CanvasGroup sur CooldownOverlay, Animator `IsOnCooldown`, locked lisible, layer 5
- [x] GUID `dac6251613d7f9849a21f9c1598ff676` inchangé

### Fait (Cursor)
- [x] `SaleChannelBandeauView` : bool Animator + alpha reset pour FadeIn
- [x] Fix binding pulse → path `CooldownOverlay/CooldownLabel`

### Playtest
1. Vente → cooldown fade + pulse timer
2. Bandoulière/Vélo locked lisible

### Commit
- À faire côté **auteur**.

---

## 2026-07-23 — Punch Click halo + talent nodes (visibilité)

### Problème
- Punch halo masqué par ouverture immédiate de l’overlay
- Nœuds talents : Idle seulement

### Fix Cursor
- [x] Délai ~0.18s après punch halo avant ouverture overlay
- [x] Clip Click halo plus fort (1.25, 0.22s)
- [x] `TalentNode_Click` + trigger ; punch puis achat (~0.12s)

### Playtest
1. Clic Commerce : punch visible puis overlay
2. Clic nœud Available : punch Icon puis achat

### Commit
- À faire côté **auteur**.

---

## 2026-07-23 (soir+) — Todo demain insecte + atlas + refs Farm Together / Dinkum / Coral Island

### Contexte
- Promesse art ChatGPT (crédits épuisés) : sheet abeille **1024×1024**, **8 frames Fly** (H-M-B-M…), grille 4×2 ; pack multi-insectes proposé en backlog.
- Clarif : « 8 directions » = **8 frames**, pas 8 orientations (runtime = flipX).

### Décisions
- **Coder demain** sur la promesse sheet (placeholders OK) — `[P0-FARM-INSECT-001]`
- **Unity Sprite Atlas : pas maintenant** — le PNG Multiple suffit ; atlas runtime plus tard (`[P0-FARM-INSECT-003]`)
- Pack étendu (papillon, coccinelle, ver, …) = backlog art ; MVP = abeille

### Fait
- [x] Todo « prochaine session » insecte en tête de `Notes/Todo_project.md`
- [x] Spec maj promesse ChatGPT + § Atlas : `Notes/Farm/SPEC_insecte_flowering.md`
- [x] Refs jeux § E : Farm Together, Dinkum, Coral Island → `Notes/References/REFERENCES_jeux_inspiration.md`

### Prochaine session (priorité)
1. `[P0-FARM-INSECT-001]` scripts path + FSM + hook Flowering
2. Dès PNG : `[P0-FARM-INSECT-002]` import + playtest
3. Pas d’atlas Unity tant que pack non stable

### Fichiers
- `Notes/Todo_project.md`
- `Notes/Farm/SPEC_insecte_flowering.md`
- `Notes/References/REFERENCES_jeux_inspiration.md`
- `PROJECT_LOG.md`

### Commit
- À faire côté **auteur** — assistant non committer.

---

## 2026-07-23 — TalentNode Idle breathe Bezy (livré)

### Fait (Bezy)
- [x] `TalentNode.controller` + `TalentNode_Idle.anim` (path `Icon`, scale 1→1.05→1, loop 1.4s)
- [x] `TalentNodeView.prefab` : Animator + controller, layers UI 5, Update Mode Unscaled
- [x] GUID prefab **inchangé** `0f1b14c68efb3324ba77f23eb509d0c8` → instances `Track_Commerce` OK
- [x] Pas de Probe / instance Bootstrap résiduelle
- Pas de hook Cursor (Idle auto au runtime)

### Playtest
1. Inventaire → Commerce → nœuds : Icon breathe Idle visible
2. Titre / overlays stables (seule Icon scale)

### Fichiers
- `Assets/Prefabs/Ui/Progression/TalentNodeView.prefab`
- `Assets/Animations/UI/TalentNode.controller`, `TalentNode_Idle.anim`
- `Notes/Ui/PROMPTS_Bezi_talent_node_idle.md`, `Notes/Todo_project.md`

### Commit
- À faire côté **auteur** — assistant non committer.

---

## 2026-07-23 — Halo inventaire micro-anim Bezy (livré) + note workaround prefab

### Fait (Bezy)
- [x] Clips + controller : `PlayerHaloSlot_Idle` / `_Click` / `PlayerHaloSlot.controller`
- [x] `PlayerHaloSlotUI.prefab` : Animator câblé, trigger `Click`, layers UI (`m_Layer: 5`)
- [x] GUID prefab **inchangé** `a1931597dd60ec948aeb14c6a9ccfa34` → 8 instances `PlayerHaloPanel` OK
- [x] Nettoyage `TestProbe` / probes ; Bootstrap sans instance temporaire résiduelle

### Fait (Cursor)
- [x] `PlayerHaloPanelController` : `PlayTrigger("Click")` au clic slot avant ouverture arbre

### Note technique Bezy (workaround)
La modification directe des GameObjects existants dans un prefab disque via les actions standard échouait systématiquement (bug de résolution de chemin ; repro aussi sur `ActionPointsHudWidget.prefab`). Contournement : instanciation temporaire dans `Bootstrap.unity` → mods (Animator, wiring, layers) → régénération du prefab à l’emplacement d’origine, GUID vérifié inchangé.

Documenté dans `Notes/Bezi/README_bezi.md` § *Workaround Bezy — bug résolution de chemin*.

### Playtest
1. Inventaire → slots halo : Idle breathe visible
2. Clic Commerce (ou autre) → punch Click puis overlay talents

### Fichiers
- `Assets/Prefabs/Ui/Progression/PlayerHaloSlotUI.prefab`
- `Assets/Animations/UI/PlayerHaloSlot*`
- `Assets/Scripts/UI/Inventory/Progression/PlayerHaloPanelController.cs`
- `Notes/Bezi/README_bezi.md`, `Notes/Todo_project.md`

### Commit
- À faire côté **auteur** — assistant non committer.

---

## 2026-07-23 — Spec insecte Flowering `[CT-FARM-POLISH-002]`

### Décisions
- Sprite sheet partagé (abeille / papillon), **pas** un sheet par plante
- Path configurable : nodes + arêtes sur prefab plante (laitue ≠ tomate)
- FSM : FlyAlongEdge → Forage au node → PickNextEdge
- Orientation : art vers la droite + `SpriteRenderer.flipX` (deadzone sur |vx|)
- Activation uniquement au stade `Flowering` via `PlantGrow`

### Fait
- [x] Note complète : `Notes/Farm/SPEC_insecte_flowering.md` (archi, FSM, perf, prompt ChatGPT sheet, critères d’acceptation)
- [x] Lien todo `[CT-FARM-POLISH-002]` → cette spec

### Prochaine étape (quand art prêt)
1. Générer / importer sheet abeille (`Assets/Art/Sprites/Farm/Insects/`)
2. Cursor : `InsectPathFollower` + champs `PlantDefinition` + hook Flowering
3. Bezy : nodes sur prefabs plantes + wiring overlay

### Fichiers
- `Notes/Farm/SPEC_insecte_flowering.md`
- `Notes/Todo_project.md`

### Commit
- À faire côté **auteur** — assistant non committer.

---

## 2026-07-23 (soir) — Art VFX planting + todo polish Bezy

### Fait
- [x] Sprite sheet particules terre/cailloux/feuilles importée et découpée 3×3 → `Assets/Art/Sprites/VFX/Planting/PlantationDirtParticules.png`
- [x] Todo polish ajouté : **[CT-FARM-POLISH-003]** / **[BZ-POLISH-016]** (plantation + récolte) — file Bezy #16

### Prochaine session (Bezy)
- Enchaîner file polish ; quand #16 : prefab Particle System burst (prompts à rédiger `PROMPTS_Bezi_planting_dirt_vfx.md`)
- Hook Cursor `Play()` plant + harvest après prefab

### Fichiers
- `Assets/Art/Sprites/VFX/Planting/PlantationDirtParticules.png` (+ `.meta`)
- `Notes/Todo_project.md`, `Notes/Ui/TODO_Bezi_polish_semaine.md`

---

## 2026-07-23 (fin) — Session Bezy polish + file semaine

### Contexte
- Branche **`feature/points-actions`**. Priorité : consommer crédits Bezy (prefabs / anims). Playtests reportés en batch.

### Objectifs du jour
- Wiring HUD PA + polish anim SpendPulse
- EmptyState graines
- Polish shop `[CT-SHOP-002]`
- File Bezy pour la semaine

### Fait
- [x] HUD PA Phase 3bis + Phase 5 SpendPulse (Bezy) + trigger Cursor
- [x] EmptyState `SeedSelectionUI` Ph.1–3 (Bezy)
- [x] Shop polish Ph.1–3 (layers, lisibilité, Open/Close scale+slide, animator/canvasGroup)
- [x] Playtests → `Notes/Todo_playtest.md` (+ Batch D QA shop / Bootstrap)
- [x] File overload Bezy : `Notes/Ui/TODO_Bezy_polish_semaine.md` (15 jobs)

### Décisions
- Playtests en batch, pas bloquants pour Bezy
- Pas de génération d’images via Bezy (placeholders / layers / Animator)

### Prochaine session
1. Enchaîner file Bezy à partir de **[BZ-POLISH-001]** (`TODO_Bezy_polish_semaine.md`)
2. Option Cursor : hook vente PA + commit lot local
3. Batch QA plus tard (Bootstrap UI parasite = §1 `PLAYTEST_shop_polish_ct002.md`)

### Fichiers touchés (session, non exhaustif)
- HUD PA : prefab, anims, `ActionPointsHudView.cs`, `NavigationHUD.unity`
- `SeedSelectionUI.prefab`
- `ShopItemPopup.prefab` + `ShopItemPopup_Open/Close.anim`
- Docs : `Todo_project`, `Todo_playtest`, `PROMPTS_Bezi_*`, `TODO_Bezy_polish_semaine`, `PLAYTEST_shop_polish_ct002`

### Commit
- À faire côté **auteur** (`git add` / `commit` / `push`) — assistant non committer.

---

## 2026-07-23 — Bezy prioritaire ; playtests reportés en batch

### Décision auteur
- Playtests **plus en priorité session** : batch dans `Notes/Todo_playtest.md`.
- Priorité immédiate : **crédits Bezy** (prefabs / polish UI).

### Fait session
- [x] HUD PA Phase 3bis + Phase 5 SpendPulse (Bezy) + trigger Cursor — review OK.
- [x] Playtests PA / vente / ferme déplacés → `Notes/Todo_playtest.md`.

### Prochaine session (priorité immédiate)
1. **[P0-FARM-BEZI-001]** / **[CT-FARM-UI-001]** EmptyState `SeedSelectionUI` — prompts `Notes/Ui/PROMPTS_Bezi_seed_empty_state.md` (Phase 1 → 2 → 3).
2. Option : **[CT-SHOP-002]** polish shop Bezy.
3. Playtests batch plus tard (`Todo_playtest.md`).

### Fichiers
- `Notes/Todo_playtest.md` (nouveau)
- `Notes/Ui/PROMPTS_Bezi_seed_empty_state.md` (nouveau)
- `Notes/Todo_project.md`, `Notes/Ui/PROMPTS_Bezi_action_points.md`
- HUD PA : anims + prefab + `ActionPointsHudView.cs`

---

## 2026-06-26 — Points d'action V0 (feature/points-actions) + design fatigue prochaine session

### Contexte
- Branche **`feature/points-actions`** — lot PA démarré après merge vente sur `main`.

### Fait (Cursor)
- [x] **[P0-AP-CODE-001]** `ActionPointService` + `ActionPointSaveService` + `ActionPointActionId` — budget 240/jour UTC, persistance `action_points.json`.
- [x] **[P0-AP-CODE-001b]** Hook planter — `BiofiltreManager.TryPlantSeedAt` (−1 PA, rollback si échec).
- [x] **`ActionPointsHudView.cs`** — affichage HUD (écoute `OnActionPointsChanged`).
- [x] Prefab `ActionPointsHudWidget` — layer 5 corrigé côté Cursor ; prompts `Notes/Ui/PROMPTS_Bezi_action_points.md`.

### Fait (Bezy)
- [x] HUD shell Ph.1 + micro P2 (instance NavigationHUD, layer 5 scène).
- [ ] **[P0-AP-BEZI-003]** Phase 3 wiring — en attente auteur.

### Décision design (prochaine session)
- **[CT-AP-DESIGN-001]** Réévaluer base PA et régénération progressive.
- Cible auteur : zone confort **100 PA (~10 h)** ; malus sur toutes les actions au-delà :
  - 10–12 h : **+15 %** coût PA
  - 12–14 h : **+25 %**
  - 14–16 h : **+50 %**
  - Plafond indicatif ~160 PA / ~16 h

### Prochaine session
1. Clôturer V0 : Bezy P3 + playtest **[P0-AP-PLAY-001]** + hooks récolte/vente.
2. Session design **[CT-AP-DESIGN-001]** — spec fatigue + régénération (`BL-AP-001` → `BL-AP-004`).

### Fichiers touchés (non exhaustif)
- `Assets/Scripts/Systems/ActionPoint*.cs`
- `Assets/Scripts/UI/ActionPointsHudView.cs`
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab`
- `Assets/Scenes/NavigationHUD.unity`
- `Notes/Todo_project.md`, `Notes/Ui/PROMPTS_Bezi_action_points.md`

---

## 2026-06-20 (fin) — Bezy cooldown Ph.4–5 validées + prochaine session playtest

### Contexte
- Branche **`feature/vente-production`** — cooldown vente **24 h** (code Cursor + UI Bezy).

### Ce qu'on a fait
- [x] **[P0-SALE-TIMER-001]** — `SaleChannelSaveService`, cooldown 24 h UTC, refresh bandeau + coroutine timer.
- [x] **[P0-SALE-BEZI-004]** — Bezy hiérarchie `CooldownOverlay` + `CooldownLabel` — **review Cursor OK**.
- [x] **[P0-SALE-BEZI-005]** — Bezy wiring `cooldownOverlay` / `cooldownLabel` + `channelId=voisinage` sur instance Voisinage — **review Cursor OK**.

### Prochaine session (priorité immédiate)
- **[P0-SALE-PLAY-004]** Playtest cooldown complet :
  1. Récolter laitue → Vente → Voisinage → vendre.
  2. Vérifier overlay gris + label timer (descente ~1 s).
  3. Vérifier bandeau non cliquable + popup refusée si cooldown actif.
  4. Test rapide optionnel : `neighborSaleCooldownSeconds = 60` ou `ignoreSaleCooldown` sur `SaleChannelService`.
  5. Commit auteur sur `feature/vente-production` si OK.

### Fichiers touchés (session)
- `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab`
- `Assets/Prefabs/Ui/SaleChannelsScreen.prefab`
- `Assets/Scripts/Systems/SaleChannelSaveService.cs`, `SaleChannelCooldownFormatter.cs`, `SaleChannelService.cs`
- `Assets/Scripts/UI/SaleChannels/SaleChannelBandeauView.cs`, `RuntimeSaleChannelsScreen.cs`
- `Notes/Todo_project.md`, `Notes/Ui/PROMPTS_Bezi_sale_channels.md`, `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`

---

## 2026-06-20 (suite) — Cooldown vente 24 h (code) + Bezy overlay timer

### Ce qu'on a fait (Cursor)
- [x] **`SaleChannelSaveService`** — persistance `sale_channels.json` (`lastSaleUtcTicks` par canal).
- [x] **`SaleChannelService`** — cooldown **24 h** UTC, gate `CanSell` / `TrySell` / popup, flag debug `ignoreSaleCooldown`.
- [x] **`SaleChannelCooldownFormatter`** — affichage `23h 45m` / `12m 30s`.
- [x] **`SaleChannelBandeauView`** — API `ApplyCooldownState` (overlay, label, illustration grisée).
- [x] **`RuntimeSaleChannelsScreen`** — refresh bandeaux + coroutine 1 s pendant cooldown actif.

### Prochaine session
1. **[P0-SALE-BEZI-004–005]** Bezy — `CooldownOverlay` + `CooldownLabel` sur prefab bandeau (prompts Ph.4–5).
2. **[P0-SALE-PLAY-004]** Playtest cooldown complet (overlay + timer visibles).

### Fichiers touchés
- `Assets/Scripts/Systems/SaleChannelSaveService.cs`, `SaleChannelCooldownFormatter.cs`, `SaleChannelService.cs`
- `Assets/Scripts/UI/SaleChannels/SaleChannelBandeauView.cs`, `RuntimeSaleChannelsScreen.cs`
- `Notes/Ui/PROMPTS_Bezi_sale_channels.md`, `SPEC_sale_channels_ui_bandeaux.md`, `Notes/Todo_project.md`

---

### Contexte
- Branche **`feature/vente-production`** — chantier **canaux de vente** : bandeaux Bezy + vente laitue voisinage.

### Ce qu'on a fait
- [x] Bezy **Ph.1–3** — scroll, `SaleChannelBandeauView.prefab`, 3 bandeaux (Voisinage actif, Bandoulière/Vélo verrouillés), wiring Inspector — **review Cursor OK**.
- [x] **`SaleChannelService`** — voisinage, `laitue_mature`, 15 gold/unité, cap 2 ; `InventoryCurrencyAccount.TrySell`.
- [x] Popup vente — `PopupId.SaleChannelSell`, mode `ShopItemPopupFlowMode.Sell`, bindings `NavigationHUD.unity`.
- [x] Correctifs UI — IDs YAML `CloseLabel` ; croix fermeture `x` (police TMP) sur Vente + Shop.
- [x] Fix compile — `TrySell` param `out InventoryResult` ; warning `TalentNodeView.textWrappingMode`.
- [x] **[P0-SALE-PLAY-003]** Playtest auteur : récolte → Vente → Voisinage → popup → gold + inventaire **OK**.

### Prochaine session (priorité immédiate)
1. **[P0-SALE-TIMER-001]** Timer / cooldown par canal — ex. **1 vente par jour** sur Voisinage (persistance, blocage vente, feedback bandeau/popup).
2. **[P0-SALE-PLAY-004]** Playtest timer (vente → indispo → reset jour).
3. Commit auteur recommandé sur `feature/vente-production` avant ou après timer (selon auteur).

### Fichiers touchés (session)
- `Assets/Scripts/Systems/SaleChannelId.cs`, `SaleChannelService.cs`
- `Assets/Scripts/Inventory/InventoryCurrencyAccount.cs`
- `Assets/Scripts/UI/SaleChannels/RuntimeSaleChannelsScreen.cs`, `SaleChannelBandeauView.cs`
- `Assets/Scripts/UI/Shop/ShopItemPopupData.cs`, `ShopItemPopupController.cs`, `ShopItemPopupView.cs`
- `Assets/Scripts/UI/Popups/PopupId.cs`
- `Assets/Prefabs/Ui/SaleChannelsScreen.prefab`, `SaleChannels/SaleChannelBandeauView.prefab`, `ShopScreen.prefab`
- `Assets/Scenes/NavigationHUD.unity`
- `Notes/Todo_project.md`, `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`, `Notes/GDD/SPEC_vente_production_boucle_jeu.md`

---

## 2026-06-17 — Shell HUD Vente + prochaine session bandeaux Bezy

### Contexte
- Nouveau chantier **canaux de vente** : UX actée = **100 % UI** (clic bandeau), pas de scène PNJ.
- Vocabulaire : onglet HUD **« Vente »** (`ScreenId.SaleChannels`) — **pas** « Market » (réservé cloud global).

### Ce qu'on a fait
- [x] **`ScreenId.SaleChannels`** + **`RuntimeSaleChannelsScreen`** (shell placeholder).
- [x] **`SaleChannelsScreen.prefab`** + enregistrement `UIManager.secondaryScreens`.
- [x] Onglet **`TabVente`** dans **`NavigationHUD.unity`** (4e bouton nav bar, couleur active verte).
- [x] Docs : **`Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`**, **`Notes/Ui/PROMPTS_Bezi_sale_channels.md`** (Bezy Ph.1–3).
- [x] Mise à jour GDD **`SPEC_vente_production_boucle_jeu.md`** §4–§5.1 + **`Notes/Todo_project.md`**.

### Prochaine session (priorité immédiate)
1. **[P0-SALE-PLAY-001]** Playtest shell : HUD → Vente → fermeture.
2. **[P0-SALE-BEZI-001 à 003]** Bezy : scroll + bandeaux (Voisinage ★1 actif, bandoulière/vélo verrouillés) — prompts phasés.
3. **[P0-SALE-PLAY-002]** Playtest bandeaux scrollables.
4. *(Session suivante Cursor)* **[P0-SALE-CODE-001]** : `SaleChannelService` + popup vente salades.

### Fichiers touchés
- `Assets/Scripts/Systems/ScreenId.cs`
- `Assets/Scripts/UI/NavigationHUD.cs`
- `Assets/Scripts/UI/SaleChannels/RuntimeSaleChannelsScreen.cs`
- `Assets/Prefabs/Ui/SaleChannelsScreen.prefab`
- `Assets/Scenes/NavigationHUD.unity`
- `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`
- `Notes/Ui/PROMPTS_Bezi_sale_channels.md`
- `Notes/GDD/SPEC_vente_production_boucle_jeu.md`
- `Notes/Todo_project.md`

---

## 2026-06-16 (fin) — Arbre talents Commerce MVP validé + polish backlog

### Décision
- Feature **talent tree Commerce MVP fonctionnelle** — gel volontaire ; reprise plus tard en polish.
- Idées polish enregistrées : filigrane thématique par piste, zoom/scroll **conditionnel** (si arbre > écran, utile mobile).

### Backlog ajouté (`Notes/Todo_project.md`)
- **[BL-INV-TALENT-001]** Filigrane thématique par piste.
- **[BL-INV-TALENT-002]** Zoom + pan overlay (uniquement si contenu dépasse viewport).
- **[BL-INV-TALENT-003]** Prefabs définitifs Bezy + retrait contournements runtime.
- **[BL-INV-TALENT-004]** Arbres P2–P6 + bindings.

### Clos
- **[P0-INV-HALO-012]**, **[P0-INV-HALO-013]** — playtest Commerce OK.

### Prochaine session
- Priorité **non fixée** par l'auteur — commit lot talent tree recommandé ; sinon reprendre stock / **[P0-IDEA-001]**.

### Docs
- `Notes/Todo_project.md`, `Notes/Ui/WORKFLOW_creation_arbre_talents.md` § Polish, `ASSISTANT_CONTEXT.md`

---

## 2026-06-16 — Session debug affichage arbre Commerce + prochaine session Bezy

### Contexte
- Branche **`main`** — travail local (prefabs + scripts) **non commité** au moment de la fin de session.
- Playtest auteur : overlay s’ouvre (titre « Commerce »), mais **arbre visuel entier masqué ou illisible** en Game view.

### Diagnostic
- Titre overlay OK → `TalentTreeOverlayController.Open` + binding fonctionnent.
- Nœuds montés sous `TreeContent` / **Mask ScrollRect** → contenu clipé ou recouvert (`BodyPlaceholder`, `Dimmer` racine).
- `TalentNodeView` : titre sous overlays plein cadre (Bezy a corrigé l’ordre prefab ensuite).

### Changements effectués (Cursor — contournements temporaires)
- **`TalentTreeOverlayController`** : bypass ScrollRect (`TreeMountHost` runtime sous `OverlayPanel`), masque `BodyPlaceholder`, désactive `TreeScrollView` à l’ouverture ; layout centré 800×600.
- **`InventoryScreenController`** : désactive `Dimmer` racine quand overlay talents ouvert.
- **`TalentNodeView`** : MVP lisibilité titre (reposition au-dessus du nœud) — retiré après fix Bezy ordre prefab ; contournements overlay conservés en attente Bezy.
- **`InventoryScreen.prefab`** (local) : binding `track.commerce` → `Track_Commerce`, `TreeContent` vidé.

### Prochaine session — **[P0-INV-HALO-013]**
1. **Playtest** étape 8 (`WORKFLOW_creation_arbre_talents.md`) avec correctifs runtime.
2. **Bezy Phase 4** : fix affichage définitif prefabs (`TreeMountHost`, ScrollRect/Mask, contrastes, `TalentNodeView`) — prompt `Notes/Ui/PROMPTS_Bezi_talent_tree.md`.
3. Re-playtest ; retirer contournements Cursor si prefab OK ; **commit auteur** sur `main`.

### Fichiers touchés (session)
- `Assets/Scripts/UI/Inventory/Progression/TalentTreeOverlayController.cs`
- `Assets/Scripts/UI/Inventory/Progression/TalentNodeView.cs`
- `Assets/Scripts/UI/Inventory/Progression/InventoryScreenController.cs`
- `Assets/Prefabs/Ui/InventoryScreen.prefab` (local)
- `Notes/Todo_project.md`, `ASSISTANT_CONTEXT.md`, `Notes/Ui/WORKFLOW_creation_arbre_talents.md`, `Notes/Ui/PROMPTS_Bezi_talent_tree.md`

---

## 2026-06-15 — Merge talent tree sur `main` + prochaine session étape 7

### Contexte
- Branche de travail : **`main`** (`d2339e0`), remote à jour.
- Merge `feature/talent-tree-ui` → `main` après revert PR #11 : talent tree UI, SO Commerce, docs GDD réintégrés.

### Changements effectués (session)
- Résolution conflits merge (`InventoryScreen.prefab`, `WORKFLOW_creation_arbre_talents.md`).
- Restauration complète du lot talent tree (scripts, prefabs, assets, notes) depuis `feature/talent-tree-ui`.
- Commit : `merge: réintégrer feature/talent-tree-ui sur main`.

### État auteur `Track_Commerce` (fin session)
- [x] Étapes **0–6** : SO Commerce, prefab `Track_Commerce` avec nœuds/edges visuels.
- [~] Étape **5** : arrays `nodeViews` / `edgeViews` sur `TalentTreeLayoutRoot` **à revérifier** (Collect en Unity).
- [ ] Étape **7** : `trackPrefabBindings` encore **vide** dans `InventoryScreen.prefab`.
- [ ] Étape **8** : playtest P1 arbre visuel non fait.

### Prochaine session
1. **`Notes/Ui/WORKFLOW_creation_arbre_talents.md` — étape 7** : binder `track.commerce` → `Track_Commerce.prefab` dans `TalentTreeOverlayController`.
2. Revérifier Collect (étape 5) si besoin.
3. **Étape 8** : playtest Bootstrap → Inventaire → P1 Commerce.
4. Commit auteur sur **`main`** si playtest OK.

### Fichiers touchés (docs session)
- `Notes/Ui/WORKFLOW_creation_arbre_talents.md` (consolidé — procédure auteur autonome 8 étapes)
- `Notes/Todo_project.md`, `ASSISTANT_CONTEXT.md`, `PROJECT_LOG.md`, `Notes/GUIDE_suivi_projet.md`

---

## 2026-06-12 (soir) — Fin de session : arbres talents — composition en cours

### Contexte
- Branche **`feature/talent-tree-ui`** créée avant suite Bezy (lot isolé de `main`).

### Objectifs du jour
- Playtest MVP talents (texte) — déjà validé.
- Foundation Cursor arbres + Bezy Ph.1–3 + revue wiring.
- Documenter workflow auteur composition arbre.

### Changements effectués
- **Cursor** : `TalentNodeView`, `TalentTreeEdgeView`, `TalentTreeLayoutRoot`, `TalentTrackPrefabBinding`, `TalentTreeLayoutRootEditor`, évolution `TalentTreeOverlayController` (swap prefab).
- **Bezy** : `TalentNodeView.prefab`, `TalentTreeEdgeView.prefab`, patch `InventoryScreen` (`TreeScrollView`, `TreeContent`, wiring Phase 3) — **revue Cursor OK**.
- **Docs** : `SPEC_talent_tree_layout_editeur.md` (décisions actées), `PROMPTS_Bezi_talent_tree.md`, **`WORKFLOW_creation_arbre_talents.md`**, mises à jour `Todo_project`, `ARBRE_inventory_halo_ui.md`.

### Décisions
- 1 prefab arbre / piste + `TreeContent` partagé + swap dynamique ([P0-INV-HALO-009] clos).
- Composition `Track_Commerce` = travail auteur Unity (pas Bezy).

### État auteur fin session
- **[P0-INV-HALO-012]** en cours : auteur à l'**étape 1** du workflow (`Track_Commerce` racine + `TalentTreeLayoutRoot`).
- SO Commerce sur disque : **à confirmer** demain (étape 0 si pas encore faite).

### Prochaine session (2026-06-13 matin)
1. Reprendre `WORKFLOW_creation_arbre_talents.md` — étape 1 → 8.
2. Binding `track.commerce` dans overlay + playtest P1 arbre visuel.
3. Commit sur **`feature/talent-tree-ui`** (auteur) ; merge `main` après playtest OK.

### Fichiers touchés (non exhaustif — commit auteur)
- `Assets/Scripts/UI/Inventory/Progression/` (nouveaux scripts + overlay)
- `Assets/Editor/TalentTreeLayoutRootEditor.cs`
- `Assets/Prefabs/Ui/Progression/`, `Assets/Prefabs/Ui/InventoryScreen.prefab`
- `Notes/Ui/*.md`, `Notes/Todo_project.md`, `ASSISTANT_CONTEXT.md`, `PROJECT_LOG.md`

---

## 2026-06-12 — Doc workflow création arbre talents

### Fait
- Nouvelle note `Notes/Ui/WORKFLOW_creation_arbre_talents.md` : procédure auteur complète (SO → composition `Track_Commerce` → binding overlay → playtest).
- Renvois ajoutés : `SPEC_talent_tree_layout_editeur.md`, `PROMPTS_Bezi_talent_tree.md`, `ARBRE_inventory_halo_ui.md`, `Todo_project.md`.

---

## 2026-06-12 — Foundation arbres talents (layout éditeur)

### Décision [P0-INV-HALO-009]
- 1 prefab arbre par piste, conteneur scroll partagé, swap dynamique à l'ouverture overlay.

### Fait (Cursor)
- Scripts : `TalentNodeView`, `TalentTreeEdgeView`, `TalentTreeLayoutRoot`, `TalentTrackPrefabBinding`.
- `TalentTreeOverlayController` : `treeContentHost`, `trackPrefabBindings`, instantiate/bind/refresh, fallback texte MVP conservé.
- Custom Editor : `TalentTreeLayoutRootEditor` (collect nodes/edges, validate warnings).
- Doc : `SPEC_talent_tree_layout_editeur.md` actée, `PROMPTS_Bezi_talent_tree.md` Phase 1.

### Prochaine session
1. **[P0-INV-HALO-008]** Bezy Phase 1 (shell `TreeScrollView` + briques prefab).
2. **[P0-INV-HALO-012]** Composition auteur `Track_Commerce.prefab` après Bezy Phase 2–3.

---

## 2026-06-12 — Playtest MVP talents validé sur `main`

### Validation auteur
- [x] Playtest halo → overlay Commerce → achat nœuds mock (`TalentProgressionService`) → Retour : **OK** (déjà effectué côté auteur).
- Branche documentée `cursor/mvp-talent-tree-950d` : **absente** du remote ; code MVP talents présent sur **`main`**.

### Prochaine session
1. **[P0-INV-HALO-009]** — Valider points ouverts `Notes/Ui/SPEC_talent_tree_layout_editeur.md`.
2. **[P0-INV-HALO-008]** → **[P0-INV-HALO-011]** → **[P0-INV-HALO-012]** — briques Bezy, foundation Cursor, composition `Track_Commerce.prefab`.

---

## 2026-06-07 — Arbres talents : décision layout éditeur + fixes overlay/service

### Contexte
- Playtest / dev overlay talents : erreur coroutine sur GO inactif ; erreur compile CS0177 sur `out` parameter.
- Réflexion auteur : préférence pour **manipuler le rendu et déplacer les nœuds dans l’IDE Unity**, pas layout calculé en code.

### Fait
- **Fix** `TalentTreeOverlayController` : `Awake()` n’appelle plus `HideImmediate()` (race au 1er `Open` → coroutine sur GO inactif).
- **Fix** `TalentProgressionService.TryGetTrack` / `TryGetNode` : assignation `out` explicite si id vide (CS0177).
- **Doc** : nouvelle spec `Notes/Ui/SPEC_talent_tree_layout_editeur.md` (WYSIWYG prefab, `TalentNodeView`, `TalentTreeEdgeView`, `TalentTreeLayoutRoot`).
- **Todo** : `Notes/Todo_project.md` § prochaine session — tâches [P0-INV-HALO-009] à [P0-INV-HALO-012], alignement session + `ARBRE_inventory_halo_ui.md`.

### Décision
- Layout visuel des arbres = **composition prefab à la main** (RectTransform libre, edges éditeur) ; logique achat reste sur SO + `TalentProgressionService`.
- Réflexion auteur **en cours** (points ouverts dans la spec) avant implémentation scripts foundation.

### Prochaine session
1. **[P0-INV-HALO-004]** Playtest inventaire halo (si pas encore validé).
2. **[P0-INV-HALO-009]** Valider points ouverts spec layout éditeur.
3. Enchaîner Bezy briques → Cursor foundation → composition `Track_Commerce`.

---

## 2026-06-05 (soir) — Inventaire halo : Phase 2 bis débloquée + Phase 3 wiring OK + fix layout

### Contexte
- Bezy en difficulté (prompts + create/delete d’éléments UI). Reprise correctifs **côté Cursor** sur les prefabs, wiring Phase 3 finalisé par Bezy puis revu Cursor.

### Fait
- **Phase 2 bis (Cursor)** : `PlayerHaloSlotUI.prefab` réparé (Image/Button/TMP), `PlayerHaloPanel.prefab` couleurs + nom racine, `InventoryScreen.prefab` GUID panel corrigé (`1432e647…`) + `BodyText` TMP sous `BodyPlaceholder`.
- **Phase 3 wiring (Bezy, review Cursor OK)** : `PlayerHaloSlotUI`, `PlayerHaloPanelController` (haloSlots[8] ordonnés 01→08 vérifiés), `TalentTreeOverlayController`, `InventoryScreenController` — toutes refs résolues.
- **Fallback** : `Assets/Editor/InventoryHaloPrefabWiring.cs` (menu `Rayman → UI → Wire Inventory Halo (Phase 3)`) ; fix CS1503 (`SetFloat`).
- **Fix layout runtime** : `VerticalLayoutGroup` de `InventorySplitLayout` → `ChildControlHeight=1` (halo 300px via LayoutElement, `InventoryPanel` flexible). Corrige : grille disparue, header décalé, HUD visible en fond.

### Nettoyage doc
- Suppression de `Notes/Ui/PROMPTS_Bezi_inventory_halo.md` (prompts Bezy jugés non pertinents) + retrait des références (`INDEX.md`, `Todo_project.md`).

### Prochaine session — priorité immédiate
- **[P0-INV-HALO-004]** **Playtest** Inventaire : P1–P8 → overlay → Retour, grille visible (pas de HUD en fond). Puis [P0-IDEA-001] notes tablette + renommage `ProgressionTrackId`.

---

## 2026-06-05 — Review Cursor Phase 2 bis Bezy — **non validé (régression)**

### Constats
- `PlayerHaloSlotUI.prefab` : **régression** — remplacé par racine vide (Transform 3D, pas RectTransform, plus d’enfants Phase 1).
- `PlayerHaloPanel.prefab` : inchangé (shell) ; slots référencent toujours l’ancien fileID du slot — prefab cassé en UI.
- `BodyPlaceholder` : toujours Image seule, **pas de TMP**.

### Suite
- Reprendre Phase 2 bis Bezy en exigeant **restauration hiérarchie Phase 1** du slot avant composants, ou correction manuelle Unity.

---

## 2026-06-05 — Review Cursor Phase 2 Bezy inventaire halo

### Verdict
- **Partiel** — ne pas lancer Phase 3 tant que halo/slots pas complétés.

### OK (`InventoryScreen.prefab`)
- `VerticalLayoutGroup` sur `InventorySplitLayout`, `LayoutElement` halo h=300 + panel flexible, `CanvasGroup` sur `InventoryPanel`.
- `TalentTreeOverlay` **inactif**, `CanvasGroup` alpha 0 ; dimmer + panel + `BackButton` (Image+Button+TMP « Retour ») + `TrackTitle` TMP.
- Pas de scripts custom Phase 3.

### Manques / corrections Bezy (fin Phase 2)
- `PlayerHaloPanel.prefab` : racine / `PortraitFrame` / `LevelLabel` encore **shell** (pas Image + TMP « Niveau 1 »).
- `PlayerHaloSlotUI.prefab` : composants slot absents ou asset **cassé** (réf. guid `26dedaa…` dans le panel) — Image+Button racine, TMP labels, etc.
- `BodyPlaceholder` : Image seule → ajouter **TMP** pour Phase 3 (`bodyPlaceholderLabel`).
- Optionnel : `OverlayPanel` centré ~520×640 (actuellement stretch).

### Suite
- Finir Phase 2 sur prefabs Progression, puis Phase 3 wiring.

---

## 2026-06-04 — Inventaire halo : Phase 1 OK + priorités prochaine session

### Git
- Branche : **`feature/inventory-halo-ui`**.

### Fait
- Scripts coque `Assets/Scripts/UI/Inventory/Progression/`, docs (`ARBRE_inventory_halo_ui.md`, `PROMPTS_Bezi_inventory_halo.md`), règle `bezi_prefab_ownership.mdc`.
- **Bezy Phase 1** validée Cursor (prefabs `PlayerHaloSlotUI`, `PlayerHaloPanel`, patch `InventoryScreen`).

### Prochaine session (ordre validé auteur)
1. **[P0-INV-HALO-002]** Cursor — valider Phase 2 Bezy (composants UI).
2. **[P0-INV-HALO-003]** Bezy Phase 3 — wiring + playtest overlay talents.
3. **[P0-IDEA-001]** Import notes tablette perso → `INBOX_notes_tablette_recherches.md` / renommage `ProgressionTrackId`.

Trace : `Notes/Todo_project.md` § *Prochaine session*.

---

## 2026-06-02 — Repriorisation : session idées gameplay ([P0-IDEA-001])

### Décisions auteur
- **[P0-FARM-UI-001]** (EmptyStatePanel graines) **reclassé polish** → **[CT-FARM-UI-001]** en stock ; le code `SeedSelectionUI` est prêt, seul le prefab manque — non bloquant (playtests graines validés).
- **Prochaine session (demain matin)** : transcrire les **idées / réflexions sur tablette** (boucle gameplay, features projet) — **pas** import d'assets.
- **[CT-INV-HALO-001]**, polish ferme, etc. → **stock en attente** tant que **[P0-IDEA-001]** n'a pas produit une liste de priorités validées.

### Prochaine session
- **[P0-IDEA-001]** — synthèse idées + 3–5 tâches ordonnées dans `Notes/Todo_project.md`.

### Doc — liaison notes tablette
- Hub enrichi : `Notes/GDD/INBOX_notes_tablette_recherches.md` (cartographie + lien [P0-IDEA-001]).
- Renvois croisés : `Inbox_gdd.md`, `SPEC_rework_inventaire_halo_progression.md` §4, `Todo_project.md`, `ASSISTANT_CONTEXT.md`, `INDEX.md`.

---

## 2026-06-02 — Playtests ferme graines validés ([P0-FARM-BUG-001], [P0-FARM-PLAY-001])

### Validation auteur
- [x] **[P0-FARM-BUG-001]** Pack épuisé → achat shop ×1 → popup slot `Laitue ×1` **sans** message empty persistant.
- [x] **[P0-FARM-PLAY-001]** Boucle graines complète (plantation en chaîne, dernière graine → empty, shop, replant).

### Statut
- `[x]` dans `Notes/Todo_project.md` pour les deux IDs ci-dessus (avec [P0-FARM-BUG-002] déjà clos).

### Prochaine session
- **[P0-FARM-UI-001]** — EmptyStatePanel + bouton Acheter sur `SeedSelectionUI`.
- Puis **[CT-INV-HALO-001]** — rework inventaire halo (`Notes/Ui/SPEC_rework_inventaire_halo_progression.md`).

---

## 2026-06-02 — Toast récolte ferme + croissance offline UTC (merge `feature/farm-harvest-reward-popup`)

### Contexte
- Branche **`feature/farm-harvest-reward-popup`** (base `main` `45dd477`).
- Playtest auteur OK : toast récolte à la position plante, montée + fade ; croissance offline au retour scène / quit jeu.

### Livré — feedback récolte
- **`PopupId.FarmHarvestReward`** (`farm.harvest.reward`) + binding **`NavigationHUD`** → prefab **`HarvestRewardFeedbackPopup`**.
- **`HarvestRewardFeedbackPopupUI`** : icône `ItemDefinition.Icon` + texte `+X`, animation montée + fade à la **position monde** de la plante (projection `Camera.main` → canvas overlay).
- **`PlantHarvestInteractor`** : appel après récolte réussie (position capturée avant `Destroy`).

### Livré — croissance hors ligne (prototype)
- **`FarmTimeService`** : ticks UTC, plafond 72 h, rejet horloge reculée.
- **`FarmPersistenceCoordinator`** + **`FarmApplicationLifecycle`** : save quit / pause ; réapplication delta au retour ferme (`BiofiltreManager.OnEnable`).
- **`FarmSaveService`** v2 : `lastSavedUtcTicks` + `stageUpdatedUtcTicks` par plante.

### Docs
- `Notes/Ui/popup_generique.md` §2.5 — entrée `FarmHarvestReward`.
- `Notes/Todo_project.md`, `ASSISTANT_CONTEXT.md` — priorités et contexte Git realignés.

### Prochaine session
- **[P0-FARM-BUG-001]** / **[P0-FARM-PLAY-001]** si pas encore clos en playtest.
- **[P0-FARM-UI-001]** EmptyStatePanel graines.
- Rework inventaire halo : `Notes/Ui/SPEC_rework_inventaire_halo_progression.md`.

---

## 2026-06-02 — Validation playtest [P0-FARM-BUG-002]

### Contexte
- Branche **`main`**. Correctif plantation (panel info plante après pose) déjà intégré via audit **2026-06-02** et travail **`fix/seed-selection`** (2026-05-30).

### Validation auteur
- [x] Poser **plusieurs** graines d'affilée : **aucun** panel info plante (`FarmPlantHarvest`) à chaque pose.
- [x] **Dernière** graine : popup « plus de graines », fermeture manuelle — OK.

### Statut
- **[P0-FARM-BUG-002]** → `[x]` dans `Notes/Todo_project.md`.

### Prochaine session
- **[P0-FARM-BUG-001]** — playtest validation popup empty + slot après achat shop.
- Puis **[P0-FARM-PLAY-001]** — boucle graines complète.

---

## 2026-06-02 — Audit code : nettoyage code mort, factorisations, découpage god classes

### Contexte
- Branche dédiée **`chore/audit-cleanup-2026-06`** (base `main` `cedefd6`).
- Audit complet de `Assets/Scripts/` (57 scripts) : code mort/obsolète + opportunités d'amélioration. Vérification des références GUID en scènes/prefabs avant toute suppression.

### Changements
- **Bug corrigé** : `InventorySaveService.TryLoad` ne restaurait jamais `startingSeedsApplied` (risque de re-crédit des graines de départ au rechargement).
- **Code mort supprimé** : `Core/Timer.cs` (inutilisé) ; API orphelines (`NavigationHUD.ShowNavBar/ShowExitOnly/Hide`, `GridManager.OccupyCell/FreeCell` singuliers, `UIManager.HasScreen/PreloadScreenLazy`, `HudModalBackdrop.EnsureModalCanvas`, `InventorySceneController.Open`, `ItemDatabase.Items`, `CurrencyBalanceUI.SetReferences`, `HorizontalGradient.SetColors`, `PlantDefinition.IsHarvestableStage`).
- **Legacy supprimé** : `Assets/SampleScene.unity` (doublon orphelin) + `Assets/Scenes/SampleScene.unity` + `MainMenuUI.cs` + entrée Build Settings (boot = `Bootstrap`).
- **Factorisations / constantes** : `PlantDefinition.GetSprite()` (mapping stade→sprite unique) ; `PlantHarvestInteractor.RemovePlantFromGrid()` (récolte+arrachage) ; `UiMessages.InventoryFull` + constante `laitue_seed` ; `SceneId` déplacé dans `Systems/SceneId.cs`.
- **Découpage god classes** (extractions sans déplacer de `[SerializeField]`) : `ShopCatalogResolver` (RuntimeShopScreen), `FarmPopupCanvasFactory` + `FarmStateSerializer` (BiofiltreManager). `HarvestPanelUI` : visuels recalculés au changement de stade seulement (timer chaque frame).
- **Bug préexistant corrigé (playtest)** : `PlantPlacementPreview.ConfirmPlacement` lançait une `NullReferenceException` à la dernière graine (`Cancel()` mettait `biofiltreManager` à null avant l'appel `ReopenSeedSelectionAfterLastSeedPlanted`). Référence du manager capturée avant `Cancel()`.

### Validation
- Lint propre sur tout `Assets/Scripts/`. Playtest auteur OK (shop, ferme save/load, popup récolte, boucle graines, plus de NRE).

### Conséquence docs
- Doc obsolète réalignée : `TryOpenHarvestPanel` / `FindInteractorAt` / `ShowNavBar()` manuel n'existent plus dans le code (cf. `Notes/Codebase_etat_reference.md`).

---

## 2026-05-30 — Playtest plantation + régression panel info plante ([P0-FARM-BUG-002])

### Contexte playtest (auteur)
- Branche de travail : **`fix/seed-selection`** (correctif « dernière graine → état empty graines »).
- **Bug initial** (avant correctif) : en plantant la **dernière** graine, le panel info/récolte (`FarmPlantHarvest`) s'ouvrait au lieu de mettre à jour `SeedSelectionUI` avec le message « plus de graines ».
- **Comportement attendu** : pas d'ouverture auto du panel info au placement ; si stock graines épuisé → popup graines en état empty, fermeture manuelle par le joueur.

### Résultat playtest (régression)
- Après tentative correctif local (`SuppressFarmPointerUiThisFrame`, `ReopenSeedSelectionAfterLastSeedPlanted`, injection `BiofiltreManager` dans `PlantHarvestInteractor`) : le panel info plante s'affiche maintenant après **chaque** plantation confirmée (pas seulement la dernière graine) → **régression**.

### Piste technique (non corrigé — prochaine session)
- Le clic gauche de confirmation preview instancie la plante puis l'EventSystem peut router le même clic vers `PlantHarvestInteractor.OnPointerClick` (plante sous le curseur, collider actif).
- Le flag « suppress this frame » ne bloque peut‑être pas assez tôt / assez longtemps (ordre d'exécution `PlantPlacementPreview` vs EventSystem, reset `LateUpdate`).
- Revoir le flux complet : placement → consommation graine → UI graines (preview continue ou empty) **sans** `TryOpenHarvestPopup`.

### Fichiers touchés (WIP non mergé)
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`
- `Assets/Scripts/Farm/PlantPlacementPreview.cs`
- `Assets/Scripts/UI/SeedSelectionUI.cs`

### Correctif appliqué (même session, `fix/seed-selection`)
- **Cause racine** : la pose est confirmée au **pointer-down** (plante instanciée + collider actif) avant que l'EventSystem ne traite le clic ; `OnPointerClick` est ensuite livré au **pointer-up** (souvent frame suivante) sur cette nouvelle plante → ouverture `FarmPlantHarvest`. Le flag « suppress une frame » (réinitialisé en `LateUpdate`) était déjà expiré au relâchement. De plus, mon 1er essai forçait `Cancel()` après **chaque** pose : la preview désactivée ne masquait plus le clic → régression sur **toutes** les graines.
- **Fix** :
  - `BiofiltreManager` : suppression basée sur le **relâchement** du clic (`awaitingPlacementPointerRelease` + `placementReleaseGraceFrames`, polling `Mouse.current.leftButton` dans `Update`) ; `SuppressFarmPointerUiUntilPointerRelease()` remplace l'ancien flag une-frame ; `ShouldSuppressFarmPointerUi` couvre attente-relâchement + grâce + preview active.
  - `HandleCellClicked` (branche bloquée) : simple `return` (ne ferme plus le popup graines volontairement ré-ouvert).
  - `PlantPlacementPreview.ConfirmPlacement` : **preview maintenue active** tant qu'il reste des graines (enchaînement de poses) ; dernière graine → `Cancel()` + `ReopenSeedSelectionAfterLastSeedPlanted`.
  - `PlantHarvestInteractor` : `OnPointerClick` ignoré si `biofiltreManager.ShouldSuppressFarmPointerUi`.
  - `SeedSelectionUI` (chemin sans preview) : ré-ouverture état empty si stock épuisé après pose.

### À faire (validation)
- Playtest Unity : poser **plusieurs** graines d'affilée (aucun panel info ne doit s'ouvrir) ; à la **dernière** graine → popup « plus de graines », fermeture manuelle.
- Si OK : passer **[P0-FARM-BUG-002]** en `[x]`, puis re-valider [P0-FARM-BUG-001] et [P0-FARM-PLAY-001].

---

## 2026-05-23 — [P0-FARM-BUG-001] Correctif popup graines (message empty persistant)

### Contexte
- Bug reproduit (2026-05-22) : apres achat d'une graine, le popup affiche a la fois le slot `Laitue xN` et le message empty.
- Travail effectue sur branche : `cursor/fix-seed-popup-empty-state-cb4d`.

### Correctif code
- [x] `Assets/Scripts/UI/SeedSelectionUI.cs` :
  - ajout d'un cache du titre par defaut du panel (`defaultPanelTitle`) ;
  - resolution explicite d'un label titre fallback (`ResolveFallbackTitleLabel`) ;
  - `ShowEmptyState(...)` memorise le titre precedent avant de poser le message empty ;
  - `HideEmptyState()` restaure le titre par defaut apres re-affichage des slots.
- [x] Renforcement (iteration) :
  - switch explicite d'etat visuel (`HasSeeds` / `Empty`) pour eviter les etats mixtes ;
  - prefab `SeedSelectionUI` lie a `titleLabel` explicitement (plus de fallback implicite ambigu).

### Resultat attendu
- Scenario : pack vide -> achat shop x1 -> re-ouverture popup.
- Attendu : slot `Laitue x1` visible **sans** message empty persistant.

### A faire (validation)
- Playtest manuel Unity sur le scenario complet pour cloturer `P0-FARM-BUG-001`.
- Si OK : passer `P0-FARM-BUG-001` en `[x]` dans `Notes/Todo_project.md`.

---

## 2026-05-22 — Playtest graines + bug popup empty state ([P0-FARM-BUG-001])

### Contexte playtest (auteur)
Scénario reproduit sur `main` :
1. Consommer le **pack départ** (3 graines) → popup **« Aucune graine dans l'inventaire… »** (OK).
2. **Achat shop** de **1×** graine (`laitue_seed`).
3. Re-clic cellule vide → popup plantation affiche **`Laitue ×1`** (slot OK) **mais conserve le message empty** en titre → **BUG**.

Capture : titre empty + slot `Laitue ×1` simultanés (incohérent).

### Analyse technique (piste — non corrigé)
- `SeedSelectionUI.ShowEmptyState()` sans `emptyStatePanel` assigné : écrase le **TMP titre** du prefab via `panel.GetComponentInChildren<TextMeshProUGUI>()`.
- `HideEmptyState()` réactive `slotsContainer` mais **ne restaure pas** le texte du titre → message empty persiste après achat shop.
- Fichiers : `Assets/Scripts/UI/SeedSelectionUI.cs` (`ShowEmptyState` / `HideEmptyState`), prefab `Assets/Prefabs/Ui/SeedSelectionUI.prefab` (champs `emptyStatePanel` / `emptyStateLabel` encore vides).

### Correctif attendu (prochaine session)
- Restaurer le titre par défaut dans `HideEmptyState()` **ou** utiliser un panneau empty dédié ([P0-FARM-UI-001]) au lieu du fallback titre.
- Re-test scénario : pack épuisé → shop ×1 → popup sans message empty + slot `×1`.

### Autres notes playtest (session)
- **[~] [P0-FARM-PLAY-001]** : boucle graines partiellement validée ; bugs pack départ (6 graines / re-crédit à chaque relance) — correctifs locaux `PlayerInventory` (non commités au moment du log si working tree).

### Prochaine session
- **[P0-FARM-BUG-001]** corriger popup graines (message empty + slot simultanés).
- Puis clôturer **[P0-FARM-PLAY-001]** si scénario complet OK.

---

## 2026-05-19 — Fin de session

### Objectifs du jour
1. **[P0-SHOP-POP-001]** — polish popup achat shop (quantité, Max, confirmation, wallet) → merge `main`.
2. **[P0-FARM-SEED-INV-001]** — lier plantation ↔ inventaire (plus de plantation sans graines).
3. Git : branches `rework/shopitempopup`, `rework/selectionGraine` ; merges sur `main`.

### Changements effectués (commits clés sur `main`)
- **Shop** : `9720f92`, `f5cf8ec`, `2507172` — `ShopItemPopup` + docs.
- **Graines** : `5125d7c` — `SeedEntry.seedItem`, filtre stock, label `×N`, `TryPlantSeedAt`, pack 3× `laitue_seed`, flag save `startingSeedsApplied`.
- **Docs / merge** : `b82eec0` — clôture merge + todos.
- **Fichiers principaux** : `SeedSelectionUI.cs`, `SeedSlotUI.cs`, `BiofiltreManager.cs`, `PlantPlacementPreview.cs`, `PlayerInventory.cs`, `InventorySaveService.cs`, prefabs/scène `SeedSelectionUI`, `NavigationHUD`.

### Décisions
- **Consommation** : 1 graine retirée à la **confirmation** du placement (preview), pas à l’ouverture du popup.
- **Sac vide** : message dans le popup + ouverture **Shop** via `UIManager` (bouton dédié prefab encore optionnel).
- **Onboarding** : **3 graines** une fois par profil (`startingSeedsApplied` dans `inventory.json`), même pattern que la monnaie de départ.
- **Item référence** : `laitue_seed` (`LaitueSeedling.asset`) aligné shop / récolte / plantation.

### Problèmes / solutions
- **CS0103** `ApplyStartingSeedsGrantIfNeeded` introuvable → méthodes ajoutées dans `PlayerInventory.cs`.
- Ambiguïté `PlantSeedAt` → méthode interne **`PlantSeedAtInternal`** (save ferme sans consommation).

### Prochaine session (priorité — voir `Notes/Todo_project.md`)
1. **[P0-FARM-PLAY-001]** — playtest boucle graines sur `main` (pack départ, `×N`, 3 plants, blocage, shop, re-plant).
2. **[P0-FARM-UI-001]** — (optionnel) panneau EmptyState + bouton « Acheter » sur `SeedSelectionUI.prefab`.
3. Sinon : **[CT-SHOP-002]** polish UX shop ou **[CT-FARM-004]** persistance ferme complète.

---

## 2026-05-19 — [P0-FARM-SEED-INV-001] Merge graines ↔ inventaire sur `main`

### Livré
- [x] Fast-forward **`rework/selectionGraine`** → **`main`** (`5125d7c`) : filtre stock, affichage `×N`, `TryPlantSeedAt` + consommation, pack 3× `laitue_seed`, message + shop si vide.

### Prochaine session
- Définir priorité dans `Notes/Todo_project.md` (ex. [CT-SHOP-002], inventaire, doc popups).

---

## 2026-05-19 — [P0-FARM-SEED-INV-001] Plan graines ↔ inventaire (`rework/selectionGraine`)

### Contexte
- Bug : plantation possible **sans** graines en sac (`SeedSelectionUI` catalogue Inspector, pas de `TryRemove` dans `PlantSeedAt`).
- Branche feature créée : **`rework/selectionGraine`**.

### Décisions / doc
- Note refactor : **`Notes/Farm/REFACTOR_graines_plantation_inventaire.md`** (phases 1–6, options shop vs pack départ, cible `SeedEntry.seedItem` + consommation dans `BiofiltreManager`).
- Item graine référence : **`laitue_seed`** (`LaitueSeedling.asset`), aligné shop + récolte Seedling.

### Prochaine session (implémentation)
- Exécuter phases 1–4 du plan (MVP : filtre, quantité, consommation, empty state shop).
- Trancher option **pack graines départ** (phase 5) avec l’auteur.
- **Rappel session :** travail sur **`rework/selectionGraine`** (pas `main`) — bloc *Contexte Git* dans `Notes/Todo_project.md` + protocole `WORKFLOW_PROTOCOL.md` / règles Cursor.

---

## 2026-05-19 — [P0-SHOP-POP-001] Rework popup achat shop (`rework/shopitempopup`)

### Contexte
- Branche feature **`rework/shopitempopup`** (depuis `main` à jour).
- Objectif : polish flux §3 `Notes/Ui/popup_generique.md` sur **`ShopItemPopup`** (`PopupId.ShopItemPurchase`).

### Ce qu’on a fait
- [x] **`ShopItemPopupController`** : saisie quantité (`TMP_InputField`), bouton **Max** (solde / prix / `MaxQuantity` / place inventaire), **confirmation** avant `PurchaseRequested` (overlay si présent).
- [x] **`ShopItemPopupView`** : bindings Max, input, overlay confirmation, **`CurrencyBalanceUI`** wallet (`Solde : {0}`) rafraîchi à l’ouverture et à chaque changement de quantité.
- [x] **`ShopItemPopup.prefab`** : `QuantityInputField`, `MaxButton`, `ConfirmOverlay`, ligne **WalletBalance** dans le Header.
- [x] Commits branche : `rework amélioration du pop up shop item`, `correctif sur validation achat`.
- [x] Docs / todo : **`Notes/Todo_project.md`** ([P0-SHOP-POP-001], [CT-SHOP-003]…[006] cochés), **`popup_generique.md`** §3, **`Notes/Ui/Todo_ui.md`**.

### Prochaine session
- **[P0-FARM-SEED-INV-001]** graines plantation ↔ inventaire (`SeedSelectionUI` vs `PlayerInventory`).
- Optionnel : **[CT-SHOP-002]** polish UX visuel shop (hors MVP flux achat).

---

## 2026-05-15 — Fin de session — suites validées auteur

### Contexte
- Playtests pipeline popups / récolte : **OK** côté auteur.
- Correctif Unity : **`CurrencyBalanceUI.cs.meta`** — GUID invalide (33 hex au lieu de 32) remplacé par un GUID valide.

### Prochaine session (priorité immédiate) — enregistrée dans `Notes/Todo_project.md`
1. **[P0-SHOP-POP-001]** — Créer une **branche Git** puis **polish du popup d’achat** shop (`ShopItemPopup` / `PopupId.ShopItemPurchase`), en suivant **`Notes/Ui/Todo_ui.md`** + **`popup_generique.md`** §3 (`CT-SHOP-002` … `CT-SHOP-006`).
2. **[P0-FARM-SEED-INV-001]** — Vérifier le **lien graines plantation ↔ inventaire** (`SeedSelectionUI` / `SeedEntry` vs `PlayerInventory`) : disponibilité, cohérence des quantités, consommation au plant si prévu par le design.

### Références
- `Notes/Todo_project.md` § *Prochaine session (priorité immédiate)*

---

## 2026-05-15 — [P0-HARV-001] Popup récolte mode strict

### Changements
- [x] **`BiofiltreManager.ResolveFarmPopupHost`** : plus de `FindFirstObjectByType` — `farmPopupHost` Inspector obligatoire (déjà câblé sur `FirstLvl` → `LevelController` / `ScreenPopupHost`).
- [x] Cache + warmup **`HarvestPanelUI`** (lazy `FarmHarvestPanel.prefab`).
- [x] **`HarvestPanelUI`** : `InjectFarmPopupHost` ; **`Close()`** appelle **`TryHidePopup(FarmPlantHarvest)`**.
- [x] **`PlantHarvestInteractor`** : injection host sur le panel à l’ouverture.
- [x] **`Notes/Ui/popup_generique.md`** §2.5 mis à jour (pipeline lazy, plus de live instance).

### Test play (auteur)
- [x] Clic plante occupée → popup récolte ; Close / récolte / arrachage ; pas de warning host.
- [x] Inventaire plein à la récolte → `FarmInventoryFeedback`.

---

## 2026-05-15 — [P0-POP-003] Scan popups — verdict

### Méthode
- Inventaire des 5 constantes `PopupId` vs `UIManager.runtimePopupBindings` (`NavigationHUD.unity`).
- Grep chemins résiduels : `Instantiate` modal, `InventoryFeedbackUI`, `RegisterRuntimePopupLiveInstance`, instances scène `HarvestPanel` / `SeedSelectionUI`, ouvertures UI hors `ScreenPopupHost`.

### Tableau pipeline (état au scan)

| `PopupId` | Binding `screenId` | Prefab binding | Ouverture runtime |
|-----------|-------------------|----------------|-------------------|
| `shop.item.purchase` | `Shop` | `ShopItemPopup.prefab` | `RuntimeShopScreen` → `ScreenPopupHost.TryGetPopup` + `ShopItemPopupController.Open` |
| `shop.resource.feedback` | `Shop` | `ResourceFeedbackPopup.prefab` | `RuntimeShopScreen` → host |
| `farm.seed.selection` | `FirstLvlFarm` | `SeedSelectionUI.prefab` | `BiofiltreManager` → `TryShowPopup` + `SeedSelectionUI.Open` (lazy) |
| `farm.plant.harvest` | `FirstLvlFarm` | `FarmHarvestPanel.prefab` | `BiofiltreManager` / `PlantHarvestInteractor` → `TryShowPopup` + `HarvestPanelUI.Open` (lazy) |
| `farm.inventory.feedback` | `FirstLvlFarm` | `ResourceFeedbackPopup.prefab` (partagé shop) | `PlantHarvestInteractor` → host lazy |

Aucun `PopupId` déclaré sans binding. Aucune instance scène `HarvestPanel` / `SeedSelectionUI` dans `FirstLvl.unity` (host `ScreenPopupHost` sur `LevelController` uniquement).

### Legacy retiré (confirmé absent du code Assets)
- `InventoryFeedbackUI.cs`, prefab `InventoryFeedback.prefab`, objet `FeedbackMessage` dans `InventoryScreen.prefab`.
- `ScreenPopupHost.RegisterRuntimePopupLiveInstance` (API supprimée).

### Hors pipeline `PopupId` (accepté — UI inline / écrans, pas modales catalogue)

| Élément | Type | Action recommandée |
|---------|------|-------------------|
| `MainMenuUI.optionsPanel` | Toggle panel scène menu | Backlog optionnel produit ; pas bloquant pipeline |
| `WalletWidget` `expandedPanel` | Panneau wallet inventaire | Idem |
| `LoadingScreen` | Écran transition | Hors scope popups gameplay |
| Instanciation slots (`InventorySlotUI`, `SeedSlotUI`, shop slots) | Widgets liste | Normal — pas des popups |

### Écarts « strict shop » restants (≠ hors pipeline)

| Écart | Fichier | Backlog |
|-------|---------|---------|
| `FindFirstObjectByType<ScreenPopupHost>()` si `farmPopupHost` non assigné | `BiofiltreManager` | **[P0-HARV-001]** |
| Host ferme doit être injecté sur plantes (`InjectFarmPopupHost`) | `PlantHarvestInteractor` | Déjà en place ; durcir si host null en playtest |
| Docs obsolètes (`popup_generique.md` §2.5 live instance, `InventoryFeedbackUI` dans notes Farm) | Notes | **BL-POP-DOC-001** (sync doc) |

### Verdict **[P0-POP-003]**
**Chantier pipeline `PopupId` + `ScreenPopupBinding` + `ScreenPopupHost` : CLOS** pour le périmètre actuel (shop + ferme).  
Il ne reste **aucune modale métier** identifiée à migrer vers un nouveau `PopupId`.

**Suite priorisée :**
1. **[P0-HARV-001]** — polish strict récolte / résolution host (branche dédiée).
2. **BL-POP-DOC-001** — aligner `Notes/Ui/popup_generique.md` et cartes mentales Farm sur l’état lazy (sans `RegisterRuntimePopupLiveInstance`).

---

## 2026-05-15 — Prochaine session : scan popups + harvest sur branche

### Contexte
- Fin de session : commit des migrations popups (shop feedback, ferme inventaire plein, etc.) ; test play **inventaire plein** validé par l’auteur.

### Prochaine session (priorité immédiate) — validée par l’auteur
1. **[P0-POP-003]** Scanner le projet pour les popups / modales **non intégrés** au pipeline générique (`PopupId`, `ScreenPopupBinding`, `ScreenPopupHost`) ; produire un **verdict** (clos ou backlog).
2. **[P0-HARV-001]** Mettre à niveau **harvest** (`HarvestPanelUI` / `FarmPlantHarvest`) vers le modèle strict (prefab binding, moins d’instance scène / fallbacks) — **uniquement sur une nouvelle branche Git** avant codage (`GIT_HELPER.md`).

### Décision
- Le chantier harvest est **séparé** du scan popups et **isolé** sur branche feature pour ne pas mélanger avec le lot déjà commité sur `main`.

### Références
- `Notes/Todo_project.md` § *Prochaine session*
- `Notes/Ui/popup_generique.md` §2.5 (écart ferme vs shop strict)

---

## 2026-05-14 — IDs stables + guide utilisateur suivi

### Contexte
- Demande auteur : ajouter des IDs de taches stables et une note explicative pour rester dans le cadre de suivi.

### Ce qu’on a fait
- [x] **`Notes/Todo_project.md`** : ajout d’une convention d’IDs (`P0-*`, `CT-*`, `BL-*`) et attribution d’un ID stable a chaque tache active/backlog.
- [x] **Nouveau guide** : `Notes/GUIDE_suivi_projet.md` (mode d’emploi simple : source unique, routine debut/fin de session, anti-doublon, exemple concret).
- [x] **`WORKFLOW_PROTOCOL.md`** : ajout du lien vers le guide utilisateur.

### Decision
- Le suivi quotidien doit s’appuyer sur les IDs de `Notes/Todo_project.md` pour eviter les ambiguities de priorisation.

---

## 2026-05-14 — passe structure suivi projet/docs (safe rules)

### Contexte
- Demande auteur : améliorer la structuration du suivi de projet et de la documentation sans casser les éléments cités dans les rules.

### Ce qu’on a fait
- [x] **`Notes/Todo_project.md`** : ajout d’un bloc protocole anti-doublon et renommage explicite de la section en **`Prochaine session (priorité immédiate)`** pour rester compatible avec les règles de session.
- [x] **`WORKFLOW_PROTOCOL.md`** : rappel explicite de la source unique de statut (`Notes/Todo_project.md`).
- [x] **`ASSISTANT_CONTEXT.md`** : ajout d’un rappel “statut source unique” pour éviter les écarts entre notes.
- [x] **`Notes/Ui/Journal_ui.md`** : conversion en journal de contexte (sans cases de statut), en conservant l’historique et les décisions.
- [x] Vérification : dans `Notes/`, les statuts `[ ]/[~]/[x]` ne sont maintenant présents que dans `Notes/Todo_project.md`.

### Décision
- Le statut des tâches reste centralisé dans **un seul fichier** (`Notes/Todo_project.md`) ; les autres notes gardent uniquement le détail technique, les décisions et l’historique.

---

## 2026-05-14 — nettoyage backlog (source unique)

### Contexte
- Demande auteur : supprimer les doublons de tâches et centraliser le suivi sur une seule source.

### Ce qu’on a fait
- [x] **Source unique de statut** définie : `Notes/Todo_project.md` (seul fichier avec `[ ]/[~]/[x]`).
- [x] **Déduplication** : `Notes/Todo_project.md` restructuré (priorité immédiate + backlog consolidé sans répétitions historiques).
- [x] **Fichiers satellites convertis en vues de référence** (détails sans statut) :
  - `Notes/Ui/Todo_ui.md`
  - `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`
  - `Notes/Ui/SPEC_services_inventory_market_cloud.md`
  - `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`
  - `Notes/Bezi/README_bezi.md`
- [x] Ajout explicite de liens vers la source unique dans ces fichiers.

### Règle active
- Le statut des tâches est maintenu uniquement dans `Notes/Todo_project.md`.
- Les autres notes servent au contexte et aux checklists techniques détaillées.

---

## 2026-05-15 — FirstLvl : popups génériques graines + plante / récolte

### Ce qu’on a fait
- [x] **`PopupId.FarmSeedSelection`**, **`ScreenId.FirstLvlFarm`**, binding dans **`NavigationHUD.runtimePopupBindings`** (prefab `SeedSelectionUI`).
- [x] **`PopupId.FarmPlantHarvest`**, binding **`farm.plant.harvest`** (même asset prefab que la ligne graines uniquement pour remplir le champ Inspector ; **instance scène** utilisée via `RegisterRuntimePopupLiveInstance` — voir `Notes/Ui/popup_generique.md` §2.5).
- [x] **`ScreenPopupHost.RegisterRuntimePopupLiveInstance`** : instance scène enregistrée à la place du lazy prefab lorsque fournie.
- [x] **`UIManager.ApplyRuntimePopupBindingsToHost`** : support **`HarvestPanelUI`** live ; branche **`FarmPlantHarvest`** sans instancier le prefab du binding lorsque l’instance scène est fournie.
- [x] **`BiofiltreManager`** : ouverture graines via **`TryShowPopup`** + **`SeedSelectionUI.Open`** ; ouverture popup plante via **`TryShowPopup`** + **`HarvestPanelUI.Open`** ; reparentage **`HarvestPanelUI`** sous **`SeedSelectionUI`** au démarrage pour le tri canvas.
- [x] **`PlantHarvestInteractor.TryHarvest`** : passe par le host **`TryShowPopup`** lorsque disponible (cohérence avec le pipeline).

### Suite
- [ ] [P0-POP-006] puis [P0-POP-003] : voir **`Notes/Todo_project.md`** *Prochaine session* (test `FarmInventoryFeedback` + review transversale popups).

---

### Ferme — inventaire plein à la récolte (`ResourceFeedbackPopupUI`)

- [x] **`PopupId.FarmInventoryFeedback`** + binding **`FirstLvlFarm`** dans **`NavigationHUD.runtimePopupBindings`** (prefab **`ResourceFeedbackPopup`**, instance lazy sous le host ferme).
- [x] **`PlantHarvestInteractor`** : **`ShowInventoryFullFeedback`** via **`ScreenPopupHost.TryGetPopup`** ; injection **`InjectFarmPopupHost`** depuis **`BiofiltreManager.PlantSeedAt`** (plus de **`InventoryFeedbackUI`** sur l’interacteur).
- [x] **`LaitueObj.prefab`** : retrait de la référence **`feedbackUI`**.

---

### Shop — feedback ressources (`ResourceFeedbackPopupUI`)

- [x] **`PopupId.ShopResourceFeedback`** + binding **`NavigationHUD.runtimePopupBindings`** (Shop + prefab `ResourceFeedbackPopup`).
- [x] **`RuntimeShopScreen`** : résolution via **`ScreenPopupHost.TryGetPopup`** (cache) ; suppression fallback **`InventoryFeedbackUI`** / enfant embarqué.
- [x] **`ShopScreen.prefab`** : retrait de l’instance nested **`ResourceFeedbackPopup`** (évite doublon avec instanciation host).

### Fin de session — suivi (demande auteur, règle `project_management_session_protocol`)

**Objectifs rappelés** : poursuivre la migration des popups vers **`PopupId`** + **`ScreenPopupBinding`** + **`ScreenPopupHost`** ; traiter le feedback inventaire plein ferme.

**Décisions / état** : migrations code + bindings **`NavigationHUD`** documentées dans les sous-sections ci-dessus et dans **`Notes/Ui/popup_generique.md`**.

**Prochaines étapes** — remplacées par l’entrée journal **2026-05-15 — Prochaine session** ([P0-POP-003] scan popups, [P0-HARV-001] harvest sur branche dédiée).

---

## 2026-05-14 — base `main` + priorité popups FirstLvl

### Contexte
- `feature/shop` fusionnée dans **`main`** (fast-forward) et poussée sur **`origin/main`** : la branche de référence pour la suite est **`main`**.

### Prochaine session (priorité immédiate) — inchangée dans l’intention, recentrée sur FirstLvl
1. **Appliquer le système de popup générique dans `FirstLvl`** pour :
   - l’**apparition / sélection des graines** (UI sélectionnable branchée sur `PopupId` + `ScreenPopupBinding` + `ScreenPopupHost`, sans instanciation dispersée) ;
   - le **popup état plante** au clic (info + récolte), sur le même modèle que le shop HUD (`Notes/Ui/popup_generique.md`, règle `ui_popup_generic_runtime.mdc`).
2. **Scanner le projet** pour les autres popups encore hors pipeline générique ; lister et migrer par priorités (pas de fallback concurrent).

### Références code / assets
- `FirstLvl` / `SeedSelectionUI` / `HarvestPanelUI` / `PlantHarvestInteractor` / `BiofiltreManager`
- `UIManager.RegisterRuntimePopups`, `ScreenPopupHost`, `PopupId`

---

## 2026-05-12 — popups shop génériques (mode strict sans fallback)
### Contexte
- Demande auteur : documenter clairement le passage au système popup générique et supprimer toute logique legacy qui peut créer des doublons/ambiguïtés.

### Ce qu’on a fait
- [x] Refactor popup : base générique en place (`ScreenPopupHost`, `ScreenPopupBinding`, `PopupId`).
- [x] Mode strict activé : suppression des fallbacks legacy shop dans `UIManager` et `RuntimeShopScreen`.
- [x] Règle runtime : **une seule source de vérité** pour la popup item shop = binding explicite `screenId + popupId + prefab`.
- [x] Documentation mise à jour : `Notes/Ui/popup_generique.md` (section popup générique strict).

### État 2026-05-14 (clôture chantier « binding + playtest »)
- Binding `runtimePopupBindings` (Shop + `PopupId.ShopItemPurchase`) présent sur `main` ; flux popup item shop validé côté auteur.
- Les **prochaines actions** listées ci-dessous (2026-05-12) sont **archivées** ; ne plus les traiter comme TODO ouvert.

### Prochaines actions (archivé 2026-05-12 — ne plus suivre)
1. ~~Playtest + binding Bezi~~ → **fait** (voir entrée 2026-05-14 projet + `NavigationHUD.unity`).
2. ~~Prompt Inspector binding~~ → remplacé par la config actuelle dans le dépôt.

---

## 2026-05-12 — shop popup ressources insuffisantes
### Contexte
- Demande auteur : transformer le feedback « pas assez d’argent » en popup générique réutilisable pour n’importe quelle ressource requise, puis vérifier le linkage Bezi après test sans affichage.

### Ce qu’on a fait
- [x] Code : ajout de `ResourceFeedbackPopupUI` (`ShowInsufficientResources`, `ShowMessage`, bouton OK, auto-hide configurable).
- [x] Shop : `RuntimeShopScreen` utilise le popup générique pour le manque de ressources, avec fallback sur `InventoryFeedbackUI`.
- [x] Compatibilité : `InventoryFeedbackUI.ShowInsufficientFunds()` redirige vers le message générique.
- [x] Prefabs : `ResourceFeedbackPopup.prefab` créé par Bezi et lié dans `ShopScreen.prefab`.
- [x] Correctif linkage : le popup était bien assigné, mais l’instance Bezi avait un `RectTransform` avec `localScale = 0` et des ancres écrasées ; correction dans `ShopScreen.prefab`.
- [x] UX : `autoHideDelay` explique la disparition automatique ; mettre `0` pour garder le popup jusqu’au clic OK.

### Suite (polish uniquement — pas un blocage monnaie / popup)
Voir **`Notes/Ui/Todo_ui.md`** (*Shop — mécanique achat*) et **`Notes/Ui/popup_generique.md`** §3 : saisie quantité, **Max**, confirmation, passe UI/UX.

### Liens utiles
- `Assets/Scripts/UI/ResourceFeedbackPopupUI.cs`
- `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs`
- `Assets/Prefabs/Ui/ResourceFeedbackPopup.prefab`
- `Assets/Prefabs/Ui/ShopScreen.prefab`
- `Notes/Ui/popup_generique.md`
- `Notes/Ui/Todo_ui.md`

---

## 2026-05-12
### Contexte
- Demande auteur : la monnaie inventaire et le débit achat fonctionnent déjà ; nettoyer les tâches / docs qui les présentent encore comme priorité.

### Ce qu’on a fait
- [x] `Notes/Todo_project.md` : monnaie / débit achat cochés ; priorité immédiate recentrée sur UX shop, saisie quantité, bouton Max et confirmation avant paiement.
- [x] `Notes/Ui/Todo_ui.md` : checklist Shop mise à jour selon l’état réel (`PrimaryCurrency`, `InventoryCurrencyAccount`, `TryPurchase`, popup item + quantité +/-).
- [x] `Notes/Ui/popup_generique.md` : retrait des mentions obsolètes « pas d’achat / pas de monnaie » ; ajout de l’état actuel du flux achat.
- [x] `ASSISTANT_CONTEXT.md` et `Notes/Ui/Journal_ui.md` : priorité session nettoyée.

### Prochaine session (priorité)
1. Passe UI/UX Shop : lisibilité, enchaînement des modales et polish global.
2. Ajouter la saisie quantité PC/mobile et le bouton **Max**.
3. Ajouter une confirmation avant paiement si le flux de jeu la conserve.

---

## 2026-05-10
### Contexte
- Fin de session : nettoyage **`RuntimeInventoryScreen`** (suppression définitive du script + **`Library`** régénéré côté auteur pour resynchroniser l’import Unity). Doc navigation scène/UI : **`Notes/Ui/SceneUiLoadManagement.md`**.

### Prochaine session (priorités enregistrées par l’auteur)
1. Correction UI / amélioration expérience utilisateur (shop et flux associés).
2. Popup message « pas assez d’argent » (fonds insuffisants).
3. Saisie du nombre d’unités à acheter (input clavier PC + mobile).
4. Bouton **Max** : quantité max dérivée du solde et du prix unitaire (`floor(solde / prix)`), bornée par les plafonds métier déjà en place.

### Docs mises à jour (assistant)
- `Notes/Todo_project.md`, `ASSISTANT_CONTEXT.md`, `Notes/Ui/Todo_ui.md`, `Notes/Ui/popup_generique.md` (alignement spec shop / wallet).

---

## 2026-05-08
### Contexte
- Session post-intégration inventaire / shop / wallet ; décision auteur : **pas de script Python** pour générer ou maintenir les prefabs UI inventaire.

### Ce qu’on a fait
- [x] Document **`Notes/Ui/NOTE_inventory_wallet_upgrade.md`** : problématique **double source de vérité** (scène `Inventaire.unity` vs écran réel via **`UIManager` + `ScreenId.Inventory`** / **`RuntimeInventoryScreen`**), piège **Canvas/sorting**, pourquoi l’extraction YAML hors Unity est fragile et non upgradable par le flux Unity normal.
- [x] **`Notes/Todo_project.md`** : entrée **Prochaine session** pour reprendre inventaire + wallet **sans bidouille externe**, lien vers la note ci-dessus.
- [x] Suppression du script **`Tools/extract_inventory_prefab.py`** (workflow rejeté).

### Prochaines actions (priorité)
1. Reprendre une passe « inventaire + wallet » avec prefab édité dans Unity uniquement et une seule source de vérité (`UIManager`).
2. Continuer la stabilisation shop / monnaie selon les todos déjà listés dans `Todo_project.md`.

---

## 2026-03-19
### Contexte
- Machine: PC bureau/ **PC portable**
- Unity: <version 6.0>
- Branche: <main/feature/...mise en place>

### Ce qu’on a fait
- [x] Mise en place des fichiers de workflow et de contexte (`WORKFLOW_PROTOCOL.md`, `ASSISTANT_CONTEXT.md`, `PROJECT_LOG.md`, `GIT_HELPER.md`)
- [x] Clarification du processus multi-machine (journal + contexte + règles)
- [x] Premiers tests de commandes Git (status, fetch, pull, add, commit, push)

### Problèmes rencontrés / pistes
- Blocage sur la coloration syntaxique Markdown pour les blocs de code dans `GIT_HELPER.md`
- Difficultés liées à la sauvegarde des fichiers avant commit (fichier vu comme vide sur GitHub)
- Confusion autour des écrans `@review-changes` et de l’état Git local vs distant

### Décisions
- Utiliser `PROJECT_LOG.md` comme journal chronologique et `ASSISTANT_CONTEXT.md` comme résumé d’état
- Mettre les procédures (prompts/commandes) dans `WORKFLOW_PROTOCOL.md`
- Centraliser les commandes Git courantes dans `GIT_HELPER.md`

### Prochaines actions (priorité)
1. Définir et documenter les règles du projet (style, organisation, conventions AI) dans un fichier dédié ou dans `ASSISTANT_CONTEXT.md`
2. Commencer à esquisser un GDD simple pour le jeu (concept, boucle de gameplay, scope minimal)
3. Continuer à stabiliser le workflow Git (autosave, routine début/fin de session)

### Liens utiles
- Issue/PR: …
- Docs: …

## 2026-03-20
### Contexte
- Machine: **PC bureau**/ PC portable
- Unity: <version 6.0>
- Branche: <main/feature/...mise en place/rules>

### Ce qu’on a fait
- [x] Mise en place des fichiers de rules architectures
- [x] premier jet d'organisation de prise de note et tache a accomplir
- [x] Clarification du rôle des fichiers Cursor `.mdc` (règles `alwaysApply`)
- [x] Proposition d’organisation des notes en `Notes/` avec sous-dossiers par thème et convention de nommage (`INBOX_`, `TODO_`, `DECISIONS_`, `SPEC_`)
- [x] Cadrage UI : architecture panneaux en stack 2-3 layers + animation via `Animator` (fade + slide) + localisation TMP

### Problèmes rencontrés / pistes
- Blocage sur le type d'ui et gameplay
- Quel organisation/structuration suivre sans se perdre


### Décisions
- Utiliser le dossiers note pour y inscrire les idées les recherches et possible futur action
- UI : démarrer en UI “super basic” (prototype), organiser les écrans en stack de profondeur 2-3, piloter les transitions via `Animator` (fade + slide).
- Localization : séparer `country` (détection locale pour pub/market) et `language` (choix joueur), avec un `LanguageManager` déclenché lors du changement d’option.
- Notes : plusieurs fichiers par thème (au lieu d’un “journal” unique) avec nommage stable : `INBOX_{theme}.md`, `TODO_{theme}.md`, `DECISIONS_{theme}.md`, `SPEC_{theme}.md` (et sous-thèmes optionnels).

### Prochaines actions (priorité)
1. Définir et créer une architecture d'organisation des données et taches à accomplir en mode thèmatique par exemple UI avec fichier enfant + regles de nomage 
2. Commencer à esquisser un GDD simple pour le jeu (concept, boucle de gameplay, scope minimal)
3. Continuer à stabiliser le workflow Git (autosave, routine début/fin de session)
4. comment suivre mes credit IA pour pouvoir lancer au minimum encore la commande de fin de session
5. Remplir `Notes/Ui/SPEC_ui.md` avec la trame (stack 2-3 layers, contrat UIPanel, localization TMP country vs language)
6. Définir une convention de “keys” pour les TextMeshPro (pour faciliter le futur passage au vrai LanguageManager)

### Liens utiles
- Issue/PR: …
- Docs: …

## 2026-03-21
### Contexte
- Machine: **PC bureau** (session courante) · PC portable
- Unity: <version 6.0>
- Branche: <main / feature selon le repo>

### Ce qu’on a fait
- [x] Note de référence Bezi : `Notes/Bezi/README_bezi.md` (Welcome, index `llms.txt`, prompting, threads `@`, images, sécurité IP)
- [x] Transfert des tâches **LanguageManager / TextMeshPro** de `Notes/Ui/Decision_ui.md` vers `Notes/Ui/Todo_ui.md` (checklist d’implémentation)
- [x] `Decision_ui.md` : section localisation réduite à la **décision** + renvoi vers `Todo_ui.md` ; DoD UI (stack / fade-slide) conservé hors critère langue

### Problèmes rencontrés / pistes
- Clarifier plus tard **bezi.actions** dans la même note ou fichier dédié quand l’usage est figé

### Décisions
- Garder la doc agent **dans le repo** sous `Notes/Bezi/` pour partage Cursor / équipe et traçabilité

### Prochaines actions (priorité)
1. Compléter `Notes/Bezi/README_bezi.md` (Unity exact, scènes de travail, bezi.actions)
2. Remplir `Notes/Ui/Spec_ui.md` si la spec UI doit vivre séparément de `Decision_ui.md`
3. Poursuivre le hub `Notes/Todo_project.md` (liens vers TODOs thématiques sans dupliquer `PROJECT_LOG`)
4. Mise en place règle pour bezi voir [Thomas brush](https://youtu.be/LdZ0po5wU_0?t=204)

### Liens utiles
- Bezi Welcome : https://docs.bezi.com/get-started/welcome
- Index doc : https://docs.bezi.com/llms.txt

## 2026-03-22
### Contexte
- Machine: **PC bureau** (session courante) · PC portable
- Unity: 6000.3.x (réf. build locale)
- Branche: <main / feature selon le repo>

### Ce qu’on a fait
- [x] **Bezi (Sidekick)** : package installé `Packages/com.bezi.sidekick` — **Bezi Plugin v0.79.17** (dépendance `com.unity.nuget.newtonsoft-json`).
- [x] **UI prototype menu principal** (UGUI) sur la scène `Assets/SampleScene.unity` :
  - `Canvas` + `CanvasScaler`
  - `MainMenuPanel` avec composant **`MainMenuUI`**
  - boutons **`StartButton`** / **`OptionsButton`**
  - **`OptionsPanel`** (masqué au `Awake`, affiché/masqué au clic Options)
- [x] Script **`Assets/Scripts/UI/MainMenuUI.cs`** : `SerializeField` pour les boutons et le panel ; Start → `Debug.Log` + `SceneManager.LoadScene` en commentaire ; pas de `Update()` inutile.
- [x] Arborescence scripts amorcée : dossiers `Assets/Scripts/` avec `UI/`, et métas pour `Core/`, `Farm/`, `Data/`, `Localisation/` (structure projet).

### Problèmes rencontrés / pistes
- **Scène de build vs scène du menu** : `ProjectSettings/EditorBuildSettings` pointe vers `Assets/Scenes/SampleScene.unity`, alors que le menu prototype est dans **`Assets/SampleScene.unity`** — à aligner (une seule scène de démarrage ou fusion) avant build / tests device.

### Prochaines actions (priorité)
1. Choisir la scène unique de démarrage et mettre à jour **Editor Build Settings** + éventuellement supprimer le doublon `SampleScene`.
2. Remplacer le `Debug.Log` Start par `SceneManager.LoadScene` quand la scène gameplay existe.
3. Brancher le contenu réel du panneau Options (langue, audio, etc.) selon `Notes/Ui/Todo_ui.md`.

### Liens utiles
- Bezi install : https://docs.bezi.com/bezi/install-setup

## 2026-03-23
### Contexte
- Machine: **PC bureau** (session courante) · PC portable
- Unity: 6000.3.x
- Branche: <main / feature selon le repo>

### Ce qu’on a fait
- [x] **Références UI mobile** : pistes pour blueprints (stores, Behance/Dribbble, moodboards, contraintes safe area / zones pouces / HUD farm).
- [x] **Todo polish post-prototype** : entrée dans `Notes/Todo_project.md` — workflow graphique + [Adobe Firefly (jeu vidéo)](https://www.adobe.com/products/firefly/discover/ai-for-game-developers.html) ; rappel licence / usage commercial à valider plus tard.
- [x] **Game design (temps)** : discussion sur la modélisation du temps en farm (ex. croissance type ~1 % masse/jour) via compression, phases/jalons, parallélisme (plusieurs bassins / cultures), boucles courtes en session.
- [x] **Progression hors ligne (mobile)** : principe `lastUtc` → `delta` à la reprise, intégration analytique ou taux par seconde, **plafond offline**, UTC + gestion horloge ; salades 6–8 semaines réelles mappées sur temps compressé + sessions ~3 min/jour.

### Décisions
- **Todo projet** : conserver une **vue globale** dans `Notes/Todo_project.md` pour le polish Firefly ; **pas** de migration vers `Notes/Art/` tant que le volet n’est pas attaqué.

### Prochaines actions (priorité)
1. Collecter 2–3 **jeux références** (screenshots) + noter dans `Notes/Ui/` ce qui est repris ou évité.
2. Esquisser une **spec temps** (durée d’un « jour ferme », cap offline, formule croissance) dans le GDD ou une note `Notes/GDD/`.
3. Aligner **Editor Build Settings** / scène menu (`Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity`) quand le proto gameplay est prêt.

### Liens utiles
- Firefly & jeu : https://www.adobe.com/products/firefly/discover/ai-for-game-developers.html
- Cozy UI (article de référence) : https://sdlccorp.com/post/the-art-of-designing-intuitive-user-interfaces-in-cozy-games/

## 2026-03-24
### Contexte
- Machine: **PC bureau** (relecture du projet après restore / remise en route Unity)
- Unity: 6000.3.x
- Branche: <main / feature selon le repo>

### Ce qu’on a fait / état constaté (relecture)
- [x] **Build Settings** (`EditorBuildSettings`) : scène **0** = `Assets/Scenes/SampleScene.unity`, scène **1** = `Assets/Scenes/FirstLvl.unity` (les deux activées) — flux menu → niveau.
- [x] **Menu** : `MainMenuUI` est référencé dans **`Assets/Scenes/SampleScene.unity`**.
- [x] **Script** `Assets/Scripts/UI/MainMenuUI.cs` : Start / Options + panneau options ; `SceneManager.LoadScene("FirstLvl")` au clic Start.
- [x] Rappel : `git restore` n’affiche souvent rien si OK ; l’UI disparaît si le `.unity` **dans Git** n’avait pas les branchements — **commit** menu + `.meta` une fois stable.
- [x] **Placement UI** : contrôleur sous le **Canvas**, pas sur la Main Camera.
- [x] **Timer (Core)** : premier script **`Assets/Scripts/Core/Timer.cs`** — minuteur générique **Countdown** ou **Stopwatch** sur `MonoBehaviour`, incrément `elapsedTime` dans `Update` via `Time.deltaTime`, durée configurable, `autoStart` / `loop`, événements **`UnityEvent<float> onTick`** (temps courant : restant ou écoulé selon le mode) et **`onCompleted`**, plus `StartTimer` / `Pause` / `Stop` / `Restart` / `SetDuration` ; exposé : `ElapsedTime`, `RemainingTime`, `NormalizedProgress`, `IsRunning`.
- [x] **Usage prévu** : s’appuyer sur ce timer (ou une évolution) pour **valider les durées de croissance** des ressources du joueur **avant** qu’elles deviennent **collectables** (prototype en jeu ; lien futur avec spec **temps réel / offline UTC** du GDD).
- [ ] **Suivi** : prévoir une passe **revue + améliorations** du `Timer` avec l’assistant (perf si nombreux timers, `unscaledDeltaTime`, persistance / reprise hors ligne, etc.).

### Problèmes rencontrés / pistes
- Possible **doublon** `Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity` — à trancher / nettoyer.
- Le `Timer` actuel est **temps de jeu** (`Time.deltaTime`) : pour croissance longue + **offline**, il faudra probablement compléter avec une couche **données + timestamp** (voir discussions 2026-03-23) plutôt que uniquement ce composant seul.

### Prochaines actions (priorité)
1. **Commit** scène menu + branchements `MainMenuUI` après validation play mode.
2. Supprimer le `SampleScene` dupliqué si inutile.
3. Panneau **Options** : contenu réel selon `Notes/Ui/Todo_ui.md`.
4. **Session Timer** : relire `Timer.cs` avec l’assistant (comportement détaillé + pistes d’évolution pour croissance / collecte + offline).

### Liens utiles
- `Notes/Ui/Todo_ui.md` — LanguageManager / TMP

## 2026-03-26
### Contexte
- Machine: **PC portable** (session de reprise et stabilisation workflow)
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Stabilisation du workflow Git/Markdown (rappels sur `fetch`, `status`, `pull`, `add`, `commit`, `push`).
- [x] Clarification du comportement Git : `git status` ne reflète l’état distant qu’après `git fetch`.
- [x] Diagnostic des confusions locale/distant (fichier perçu vide sur GitHub car non sauvegardé/commit/push au bon moment).
- [x] Clarification UI GitHub : différence entre vue `Code` (source actuelle) et vue `commit/PR/compare` (diff coloré).
- [x] Clarification Markdown : coloration des blocs de commandes (`bash` souvent plus lisible que `powershell` pour simples commandes Git).
- [x] Cadrage gameplay technique : modèle “plante qui mûrit puis récolte” avec état mature, clic, tentative d’ajout inventaire, refus si plein.
- [x] Proposition d’architecture event-driven : objet récoltable -> demande de récolte -> inventaire répond succès/échec -> UI message si inventaire plein.
- [x] Création d’un dossier de notes pédagogiques `Notes/Learning/`.
- [x] Ajout d’une fiche explicative `Notes/Learning/Event_Listener_Unity_CSharp.md` (concepts, patterns d’abonnement, erreurs fréquentes, mini plan d’implémentation).
- [x] Ajout d’un index `Notes/Learning/README_learning.md` pour structurer l’apprentissage technique.

### Problèmes rencontrés / pistes
- Incompréhension fréquente entre “fichier modifié en mémoire éditeur” vs “fichier sauvegardé sur disque” avant Git.
- `@review-changes` interprété comme blocage, alors que c’est un écran de revue (pas l’état Git réel).
- Attente de coloration forte des commandes Git dans tous les contextes (rendu variable selon chat/Cursor/GitHub).

### Décisions
- Conserver un protocole simple de début de session : `git fetch` puis `git status -sb`.
- Continuer à documenter les commandes dans `GIT_HELPER.md` en blocs `bash` pour lisibilité.
- Garder la logique de récolte “all-or-nothing” tant que la règle d’ajout partiel n’est pas explicitement définie.

### Prochaines actions (priorité)
1. Spécifier précisément le système inventaire (slots, stack max, ajout partiel ou non) dans une note GDD dédiée.
2. Définir les états de culture (`graine`, `croissance`, `mature`, `récolté`) et leurs transitions.
3. Implémenter un flux de test minimal : clic objet mature -> `TryAdd` inventaire -> succès/réussite UI -> reset état plante.
4. Ajouter dans `WORKFLOW_PROTOCOL.md` un rappel explicite “Save All avant Git”.

### Liens utiles
- `GIT_HELPER.md` — routine Git opératoire
- `WORKFLOW_PROTOCOL.md` — protocole début/fin de session
- `Notes/Learning/README_learning.md` — index des notes pédagogiques
- `Notes/Learning/Event_Listener_Unity_CSharp.md` — cours event/listener Unity C#

## 2026-03-26 (suite)
### Contexte
- Machine: **PC bureau**
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Création / import des assets **laitue** (modèles et matériaux) pour avancer le prototype visuel.

### Problèmes rencontrés / pistes
- À valider ensuite: quels assets “laitue” restent dans le scope prototype et lesquels seront nettoyés/remplacés après validation du concept art.

### Prochaines actions (priorité)
1. Finaliser la version “prototype” des assets laitue à conserver.
2. Nettoyer les assets temporaires non retenus avant commit final art.

### Liens utiles
- `Assets/Art/Models/`
- `Assets/GeneratedModels/`

## 2026-03-26 (mini session PC portable)
### Contexte
- Machine: **PC portable**
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Clarification technique sur l’erreur réseau du package 404 Gen3D (connexion coupée côté hôte distant, probable limitation temporaire/charge service).
- [x] Consolidation du hub `Notes/Todo_project.md` à partir de `PROJECT_LOG.md` (synthèse et priorisation par sections).
- [x] Ajout de la piste “double procédé graphique” (pipeline léger + pipeline 3D) et règle de décision mobile -> 3D si performances stables.
- [x] Marquage des tâches non bloquantes avec `[OPTIONNEL]`.
- [x] Cadrage architectural pour le système plante générique: `ScriptableObject` (données statiques) + `MonoBehaviour` (état runtime), événements de récolte et règles inventaire.

### Problèmes rencontrés / pistes
- Le service 404 Gen3D peut couper la connexion en période de charge (retry à heures creuses recommandé).
- Besoin de transformer le cadrage d’architecture plante en note de référence + squelette de code exploitable.

### Décisions
- Démarrer le système plante sur un modèle **hybride SO + MB** plutôt que “MB only” pour éviter un gros refactor.
- Conserver la règle de récolte **all-or-nothing** tant que l’ajout partiel n’est pas défini.

### Prochaines actions (priorité)
1. Implémenter le squelette `PlantDefinition` + `PlantInstance`.
2. Définir l’API `Inventory.TryAdd(...)` et l’enum `InventoryAddResult`.
3. Tester le flux minimal: `Mature` -> clic -> `TryAdd` -> succès/échec + UI.

### Liens utiles
- `Notes/Todo_project.md`
- `Notes/Learning/Event_Listener_Unity_CSharp.md`

## 2026-03-30
### Contexte
- Machine: **PC portable**
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Allégement du projet côté assets en supprimant une partie des contenus 3D de test non prioritaires (samples 404 Gen plugin, `GeneratedModels`, anciens modèles laitue 3D).
- [x] Orientation confirmée vers un flux **SpriteRenderer 2D** pour accélérer le prototypage mobile.
- [x] Ajout du `ScriptableObject` générique `PlantDefinition` avec stades visuels (`seedling`, `babyLeaf`, `growing`, `mature`, `bolting`).
- [x] Création de l’asset plante `Laitue.asset` pour initialiser un premier type de plante data-driven.
- [x] Mise à jour de `PlantGrow` pour lire `PlantDefinition` et appliquer le sprite selon le stade.

### Problèmes rencontrés / pistes
- Temps de reload/compilation Unity encore élevé même pour de petites modifications de scripts.
- Choix assumé: continuer en version simple orientée 2D pour limiter la friction de prod.

### Décisions
- Prioriser la livraison d’un prototype jouable léger avec pipeline sprite.
- Garder le 3D comme piste ultérieure conditionnée à la stabilité/performance et au temps disponible.

### Prochaines actions (priorité)
1. Brancher la logique de croissance temporelle dans `PlantGrow` (transition de stades avec timer).
2. Connecter la récolte à l’inventaire (`TryAdd`) avec gestion d’échec inventaire plein.
3. Nettoyer les références orphelines Unity éventuelles après la suppression d’assets 3D.

### Liens utiles
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Assets/Scripts/Farm/PlantGrow.cs`
- `Assets/Scripts/Data/Laitue.asset`

## 2026-03-31
### Contexte
- Machine: **PC bureau** (organisation docs / prochaine session)
- Unity: 6000.3.x
- Branche: `main`

### Ce qu’on a fait
- [x] Ajout dans `Notes/Todo_project.md` de **2 tâches à cocher** pour la prochaine session : (1) sprites **sans fond blanc** / transparence, (2) **footprint** plantes + grille / `BuildManager` (Bezi et/ou Cursor).
- [x] Création de `Notes/Farm/SPEC_plant_footprint_prompt.md` : modèle `origin + offsets`, exemples poireau / salade 2×2 / tomate en croix, extension type `PlantDefinition` avec `Vector2Int[] footprint`, pseudo-code de validation, conventions à trancher (axes, rotation), **bloc prompt** copier-coller.
- [x] Hub `Todo_project.md` : lien vers la note Farm pour retrouver vite le spec/prompt.

### Prochaines actions (priorité)
1. Traiter les deux cases **Prochaine session** dans `Notes/Todo_project.md`.
2. Après impl footprint, aligner `PlantGrow` / prefab avec une seule racine par instance multi-cellules si souhaité.

### Liens utiles
- `Notes/Todo_project.md`
- `Notes/Farm/SPEC_plant_footprint_prompt.md`

## 2026-04-02
### Contexte
- Machine: **PC bureau** (compte rendu session + doc footprint)
- Unity: 6000.3.x
- Branche: `main` (commit utilisateur prévu après cette mise à jour)

### Ce qu’on a fait
- [x] **Revue `PlantDefinition`** : footprint en `Vector2Int[]` (offsets relatifs), défaut `(0,0)` ; `GetOccupiedCells(origin)` pour projeter l’origine de pose sur les cellules absolues ; `OnValidate` pour imposer la présence de `(0,0)` dans le footprint.
- [x] **Alignement avec la spec** : comportement conforme à l’intention décrite dans `Notes/Farm/SPEC_plant_footprint_prompt.md` (prochaine étape côté code : grille + `BuildManager` / service qui consomme `GetOccupiedCells`).
- [x] **Documentation** : création de `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` — fonctionnement de `GetOccupiedCells`, pseudo-appel depuis un `BuildManager` (`CanPlace` / `Occupy`), exemple **footprint 2×2** (salade), et rappel pour la session suivante sur le **dédoublonnage** des cellules.

### Points d’attention (prochaine session avec l’assistant)
1. **Dédoublonnage** : expliquer en détail pourquoi et comment éviter les **offsets dupliqués** dans `footprint` (effets sur `Occupy` / compteurs), et quelles options d’implémentation (HashSet, normalisation dans `OnValidate`, API distincte).
2. **`GetOccupiedCells` + `BuildManager`** : reprendre le guide `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` si besoin ; câbler la **vraie** grille (`IsFree` / `Occupy` / `Release`) et trancher la **convention d’axes** (X/Y) pour les offsets 2×2 et suivants.

### Prochaines actions (priorité)
1. **Commit** par l’utilisateur : `PROJECT_LOG.md`, `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, et tout autre fichier déjà prêt dans le working tree.
2. Implémenter le **module grille** + premier **placement** utilisant `PlantDefinition.GetOccupiedCells`.
3. Poursuivre les tâches **Prochaine session** dans `Notes/Todo_project.md` (sprites transparence, etc.).

### Liens utiles
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`
- `Notes/Farm/SPEC_plant_footprint_prompt.md`

## 2026-04-03
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : 6000.3.x
- Branche : `main` (push utilisateur prévu après cette mise à jour)

### Ce qu’on a fait
- [x] **Grille modulable** : extension de `GridConfig` (cases **carrées** via `cellSize` ou **rectangulaires** via `cellWidth` / `cellHeight` + `uniformCellSize`).
- [x] **`GridManager`** : mise en page soit depuis un **ScriptableObject** `GridConfig`, soit **par instance** (colonnes, lignes, taille de cellule) pour prefabs type zone de culture / biofiltre ; origine monde = **`GridConfig.origin`** ou **`transform` + offset** (`originFromTransform`).
- [x] **Décision design** : **pas de rotation des cultures** pour le prototype (un seul footprint par plante, pas de retournement horizontal des assets) ; cases « vides » = règles de placement / forme de zone.
- [x] **Documentation / suite** : entrée de journal (cette section) ; note d’enchaînement **`Notes/Farm/TODO_plantation_pipeline.md`** (prefab grille → UI plantation → BuildManager) ; mise à jour du hub **`Notes/Todo_project.md`**.

### Problèmes rencontrés / pistes
- Première grille en scène : caler **taille de cellule** et **origine** sur les sprites (itération visuelle) ; les gizmos du `GridManager` aident au réglage.

### Décisions
- **Ordre d’implémentation plantation** : (1) prefab de base avec `GridManager` ; (2) **UI de sélection de graine en premier** pour figer la référence `PlantDefinition` / footprint côté joueur et pour le fantôme ; (3) **`BuildManager`** (ou service équivalent) consommateur de `GetOccupiedCells` + grille. Détail : `Notes/Farm/TODO_plantation_pipeline.md`.

### Prochaines actions (priorité)
1. **Prefab « base plantation »** : GameObject + `GridManager` (mode instance recommandé pour prototyper), dimensions et `instanceCellSize` / origine ; optionnel collider 2D sur la zone pour futurs raycasts.
2. **UI plantation** : sélection de graine (`PlantDefinition`), affichage ou rappel du footprint (icône / grille miniature / texte) selon le niveau de polish souhaité.
3. **`BuildManager` / placement** : `WorldToGrid`, `CanPlace` / `OccupyCells`, preview semi-transparent, clic pour instancier — voir `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`.

### Liens utiles
- `Assets/Scripts/Farm/GridManager.cs`
- `Assets/Scripts/Data/GridConfig.cs`
- `Assets/Scripts/Data/GridData.cs`
- `Notes/Farm/TODO_plantation_pipeline.md`
- `Notes/Todo_project.md`

## 2026-04-07
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : 6000.3.x
- Branche : `main`

### Ce qu’on a fait
- [x] **Pipeline plantation complet (Bezi / éditeur)** : le rôle prévu pour un futur `BuildManager` est assumé par **`BiofiltreManager`** + **`PlantPlacementPreview`** (pas de classe nommée `BuildManager` dans le projet).
- [x] **`BiofiltreManager`** : pont grille ↔ UI ; écoute les clics sur cellules via **`BiofiltreGridVisualizer`** ; ouvre **`SeedSelectionUI`** sur cellule libre ; `CanPlace(anchor, PlantDefinition)` via `GetOccupiedCells` + `GridManager.AreAllCellsFree` ; **`PlantSeedAt`** : instanciation du prefab sous le conteneur plantes, `PlantGrow` → stade `Graine`, **`OccupyCells`** + mise à jour visuelle des **`BiofiltreCell`** touchées.
- [x] **`PlantPlacementPreview`** : fantôme semi-transparent collé à la grille (Input System souris) ; teinte vert / rouge selon validité du footprint ; clic gauche confirme, clic droit / **Escape** annulent.
- [x] **`SeedSelectionUI`** + **`SeedSlotUI`** : panneau de graines (`SeedEntry` définition + prefab), activation conditionnelle des slots selon `CanPlace` à l’ancre de la cellule cliquée ; lancement du preview à la sélection (repli possible sans preview si non assigné).
- [x] **`BiofiltreGridVisualizer`** : génère les **`BiofiltreCell`** (collider 2D, `IPointerClickHandler`) alignées sur **`GridManager`** ; expose un conteneur pour les instances de plantes.
- [x] **`BiofiltreCell`** : cellule cliquable, coordonnées grille, états visuels vide / occupé.
- [x] **`GridLinesRenderer`** (optionnel) : rendu de lignes de grille sur le même objet que `GridManager`.

### Décisions / nomenclature
- Le **service de placement** est **scindé** : logique métier grille + pose dans **`BiofiltreManager`**, interaction souris + fantôme dans **`PlantPlacementPreview`**. Les guides qui parlent encore de `BuildManager` restent valables conceptuellement (`CanPlace` / `Occupy` / preview).

### Problèmes rencontrés / pistes
- Aucun blocage noté dans cette entrée ; **prochaine doc** : formaliser le **workflow d’ajout d’une nouvelle plante** (asset `PlantDefinition`, footprint, prefab, entrées UI).

### Prochaines actions (priorité)
1. **Rédiger la documentation** : workflow « ajout de nouvelles plantes » (références : `PlantDefinition`, `SeedSelectionUI` / `SeedEntry`, prefab avec `PlantGrow`, règles footprint — voir `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`).
2. Enchaîner sur le **prototype** : récolte ↔ inventaire, timers de croissance, ou maturité biofiltre / progression selon `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`.

### Liens utiles
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Scripts/Farm/PlantPlacementPreview.cs`
- `Assets/Scripts/Farm/BiofiltreGridVisualizer.cs`
- `Assets/Scripts/Farm/BiofiltreCell.cs`
- `Assets/Scripts/UI/SeedSelectionUI.cs`
- `Assets/Scripts/UI/SeedSlotUI.cs`
- `Assets/Scripts/Farm/GridLinesRenderer.cs`
- `Notes/Farm/TODO_plantation_pipeline.md`
- `Notes/Todo_project.md`

## 2026-04-09
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : 6000.3.x
- Branche : `main` (fichiers inventaire / ferme en cours d’intégration scène)

### Ce qu’on a fait
- [x] **Système d’inventaire (code)** : couche données + runtime + UI de base, **non validé en jeu** (pas de scénario de test bout-en-bout sur la laitue / prefab plante).
  - **Données** : `ItemDefinition` (id, nom, icône, `maxStack`), `ItemDatabase` (résolution par `itemId`).
  - **Runtime** : `PlayerInventory` (`TryAdd` / `TryRemove` / `Count` / `HasSpaceFor`, résultats `InventoryResult` incluant ajout partiel), `InventorySlot`.
  - **UI** : `InventoryUI` + `InventorySlotUI`, `InventoryFeedbackUI` (ex. inventaire plein).
  - **Pont ferme** : `PlantHarvestInteractor` sur la plante (`Collider2D`, `PlantGrow`) — clic souris (`OnMouseDown`), résolution de l’item via `PlantDefinition.harvestItemId` ou override, appel `PlayerInventory.TryAdd`.
- [x] **Support placement → récolte** : `PlantDefinitionHolder` (définition posée par `BiofiltreManager` à l’instanciation) pour que la récolte lise `HarvestStage` et `harvestItemId` sans coupler au pipeline de pose.

### Problèmes rencontrés / pistes
- **Récolte** : le pipeline prévoit **deux moments** de récolte possibles sur un cycle (ex. profil **Leafy** : récolte **Mature** puis cycle **Flowering → Seedling** pour graines) ; `PlantDefinition` n’expose aujourd’hui qu’un seul `harvestStage` + un `harvestItemId`. `PlantHarvestInteractor.OnHarvestSuccess` est un **placeholder** : pas d’avancement de stade ni de **verrou** d’état (risque de double-clic / récolte hors design).
- **Tests** : aucun test automatisé ni check-list scène documentée pour valider ajout d’item, UI, et échec « inventaire plein » sur la salade.

### Décisions / suite
- **Prochaine session** : (1) **implémenter et tester** l’inventaire en conditions réelles (assets laitue : `PlantDefinition`, entrées `ItemDatabase`, composants sur le prefab plante / joueur / canvas) ; (2) **verrouiller** le comportement de récolte (une fois récolté, transition d’état ou compteur `maxHarvestCount`) ; (3) **refactor** à envisager pour distinguer clairement **récolte « corps de récolte »** (feuilles / fruit au stade configuré) vs **récolte graines** (ex. `Seedling`) — données (`itemId`, stade, quantités min/max) + un seul interactor ou stratégie par type de récolte.
- **Documentation** : carte des systèmes existants (plantation, croissance, récolte, inventaire) — voir `Notes/Farm/SYSTEMES_carte_mentale.md`.

### Liens utiles
- `Assets/Scripts/Inventory/`
- `Assets/Scripts/UI/Inventory/`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`
- `Assets/Scripts/Farm/PlantDefinitionHolder.cs`
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Scripts/Data/PlantDefinition.cs`
- `Notes/Farm/SYSTEMES_carte_mentale.md`
- `Notes/Todo_project.md`

## 2026-04-11
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : 6000.x (Unity 6)
- Branche : selon `git status` (fichiers ferme / inventaire / scène souvent encore non commités)

### Ce qu’on a fait
- [x] **Fin de session — notes & suivi** : priorité **inventaire récolte** consignée dans `Notes/Todo_project.md` (case *Inventaire récolte — finaliser et câbler*) ; rappel pipeline plantation **étape 4** dans `Notes/Farm/TODO_plantation_pipeline.md` (récolte ↔ inventaire).
- [~] **Travail en cours (code / scène)** : poursuite du flux **récolte avec panel** — scripts typiquement `HarvestPanelUI.cs`, `PlantHarvestInteractor.cs`, évolutions `BiofiltreManager.cs`, scène `FirstLvl.unity` (refs Inspector / prefabs à valider en jeu).

### Problèmes rencontrés / pistes
- **Câblage** : le flux Zoom D (carte mentale) reste à **valider bout-en-bout en scène** : ouverture du panel sur plante mature, bouton *Récolter*, `PlayerInventory.TryAdd`, UI inventaire, `InventoryFeedbackUI` si plein — sans se fier uniquement au schéma doc.
- **Design inchangé** : double récolte / `maxHarvestCount` / deux items (feuilles vs graines) — toujours ouverts (cf. entrée 2026-04-09).

### Prochaines actions (priorité)
1. **Finaliser et câbler le système d’inventaire récolte** en conditions réelles (laitue, `FirstLvl` ou scène de test) : assignations SerializeField, `ItemDatabase` / `ItemDefinition`, `PlayerInventory`, `HarvestPanelUI`, `PlantHarvestInteractor` + pont `BiofiltreManager` / grille occupée.
2. Ensuite : verrou récolte + spec deux récoltes si on touche au cycle plante.

### Liens utiles
- `Assets/Scripts/UI/Inventory/HarvestPanelUI.cs`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs`
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Scenes/FirstLvl.unity`
- `Notes/Todo_project.md`
- `Notes/Farm/TODO_plantation_pipeline.md`

## 2026-04-12
### Contexte
- Machine : **PC bureau** / PC portable (selon session)
- Unity : Unity 6 (6000.x)
- Branche : selon l’état Git local

### Ce qu’on a fait
- [x] **Audit code + assets (assistant)** : parcours de `Assets/Scripts/`, prefabs farm/UI principaux et scènes listées ci-dessous ; rédaction de **`Notes/Codebase_etat_reference.md`** (inventaire des scripts, flux réels, prefabs, points d’attention).
- [x] **Alignement doc** : mise à jour du **Zoom D** et du tableau « Récolte ↔ inventaire » dans **`Notes/Farm/SYSTEMES_carte_mentale.md`** pour refléter le chemin **`TryOpenPlantPopup`** / registre `GridManager.GetPlantAt` (remplace l’ancien schéma centré sur `TryOpenHarvestPanel`, aujourd’hui non appelé dans `HandleCellClicked`).
- [x] **Journal** : cette entrée dans **`PROJECT_LOG.md`**.

### État constaté (code au moment de l’audit)
- **Grille / biofiltre** : `BiofiltreManager` — cellule libre → `SeedSelectionUI` ; cellule occupée → **`TryOpenPlantPopup`** → `HarvestPanelUI.Open` avec lookup **`gridManager.GetPlantAt(coords)`** (pas de recherche spatiale sur le clic grille).
- **Récolte** : `PlantHarvestInteractor` — `IPointerClickHandler` sur la plante (**`OnPointerClick` → `ConfirmHarvest()`** direct si récoltable, utile avec **Physics2DRaycaster** sur la caméra) ; **`TryHarvest()`** ouvre le panel ou applique la récolte en fallback ; **`IsHarvestable()`** = stades **Mature** ou **Seedling** ; succès **Success** ou **Partial** → **`OnHarvestSuccess`** (libère grille + `Destroy`).
- **UI récolte** : `HarvestPanelUI` — popup à tout stade, bouton récolte si Mature/Seedling, bouton arracher (`Uproot`), rafraîchissement timer/stade en `Update` tant que le panel est ouvert.
- **Inventaire** : `PlayerInventory.TryAdd(ItemDefinition, int)` → `InventoryResult` (dont **Partial**) ; UI `InventoryUI` / `InventorySlotUI`, feedback `InventoryFeedbackUI`.
- **Données plante** : `PlantDefinition` — `PlantGrowthPattern` Leafy/Fruiting, `HarvestStage`, harvest min/max, `maxHarvestCount` (champ présent ; logique multi-récolte sans destruction à affiner si besoin).
- **Méthodes orphelines** : `BiofiltreManager.TryOpenHarvestPanel` / `FindInteractorAt` existent mais **ne sont pas invoquées** par le flux actuel du clic cellule — conservées pour référence ou suppression future.

### Prefabs / scènes repérés (non exhaustif hors packages)
- `Assets/Prefabs/World/Biofiltre.prefab`, `Assets/Prefabs/World/Plantes/LaitueObj.prefab`
- `Assets/Prefabs/Ui/InventoryPanel.prefab`, `Assets/Prefabs/Ui/InventorySlotUI.prefab`, `Assets/Prefabs/Ui/SeedSlotUI.prefab`
- `Assets/Scenes/FirstLvl.unity`, `Assets/Scenes/SampleScene.unity`, `Assets/SampleScene.unity` (doublon éventuel déjà noté dans les entrées précédentes)

### Problèmes rencontrés / pistes
- **Design** : Mature et Seedling utilisent le **même** `harvestItemId` tant qu’on n’ajoute pas un second item ou une règle par stade ; **`Partial`** déclenche quand même la destruction de la plante (perte de la récolte restante côté monde).
- **Dette** : trancher le sort de `TryOpenHarvestPanel` / `FindInteractorAt` (usage ou retrait) pour éviter la confusion avec la doc.

### Prochaines actions (priorité)
1. Jeu de tests manuel **`FirstLvl`** : clic cellule occupée → panel → récolte / inventaire plein / arrachage ; optionnel clic direct sur sprite plante (raycast 2D).
2. Poursuivre les cases **`Notes/Todo_project.md`** (inventaire récolte, spec deux récoltes, etc.).
3. Mettre à jour **`Notes/Codebase_etat_reference.md`** après tout refactor majeur (noms de méthodes, suppression du code mort).

### Liens utiles
- `Notes/Codebase_etat_reference.md` — état de référence post-audit
- `Notes/Farm/SYSTEMES_carte_mentale.md` — flux mis à jour (Zoom D)
- Dossier scripts : `Assets/Scripts/`

## 2026-04-12 — complément (plan prochaine session — Bezi / récolte)

### Contexte
- Demande utilisateur : consigner pour la **prochaine session** le suivi du travail **Bezi** sur la récolte **mature / graines**, la **création des SO et définitions**, et une passe de **compréhension** du système.

### Prochaines actions (priorité)
1. **Câblage** : reprendre le flux **récolte mature vs graines** (UI + interactor + scène) tel que avancé avec **Bezi** ; valider refs Inspector et prefabs (`FirstLvl` ou scène de test).
2. **Données** : créer ou compléter **`ItemDefinition`** (items distincts si besoin), entrées **`ItemDatabase`**, **`PlantDefinition`** cohérents avec les stades **Mature** et **Seedling**.
3. **Compréhension** : une session dédiée à **lire et tracer** le flux complet (grille occupée → `HarvestPanelUI` → `ConfirmHarvest` → `TryAdd`) — s’appuyer sur `Notes/Farm/SYSTEMES_carte_mentale.md` et `Notes/Codebase_etat_reference.md`.

### Liens utiles
- **`Notes/Todo_project.md`** — section *Prochaine session*, première case **Récolte Mature / Graines (Bezi)**.
- Scripts : `HarvestPanelUI.cs`, `PlantHarvestInteractor.cs`, `PlantDefinition.cs`, `PlantGrow.cs`, `ItemDatabase.cs` / `ItemDefinition.cs`.

## 2026-04-12 — complément (workflow Git — branche par feature)

### Contexte
- Souhait utilisateur : pour les prochaines features, **demander / suivre un protocole** « une branche par feature », merge une fois que tout fonctionne — idéalement **avant** les gros chantiers (ex. récolte / inventaire) ; à appliquer désormais systématiquement.

### Ce qu’on a fait
- [x] **Protocole écrit** : nouvelle section **`GIT_HELPER.md` — --3--** (*Branche par feature + fusion dans main*) : `checkout -b feature/…`, push, merge via PR ou `git merge`, rappel `merge main` dans la branche si besoin ; correction typo `git fetch` / `git log` dans la section --1--.
- [x] **Todos** : entrée **Git — branche par feature + merge** dans **`Notes/Todo_project.md`** (*Prochaine session*) ; case **Workflow Git** mise à jour dans *Workflow / Organisation*.
- [x] **Session** : **`WORKFLOW_PROTOCOL.md` — --5--** renvoie vers le helper pour démarrer une feature sur branche.

### Décisions
- Référence unique des commandes : **`GIT_HELPER.md`** ; le journal ne duplique pas la procédure complète.

### Liens utiles
- `GIT_HELPER.md` (sections --1-- à --3--)
- `WORKFLOW_PROTOCOL.md` (--4--, --5--)
- `Notes/Todo_project.md`

## 2026-04-12 — fin de session (données récolte + organisation assets + doc)

### Contexte
- Session utilisateur + assistant (Cursor) : clarification **récolte ↔ inventaire**, rangement des ScriptableObjects, doc de référence pour les prochaines plantes.

### Ce qu’on a fait
- [x] **Distinction SO** : explication *PlantDefinition* (ferme) vs *ItemDefinition* (inventaire) ; règle **`harvestItemId` = `itemId`** (pas le nom du fichier ni le `displayName`).
- [x] **Dossiers assets** : `Assets/Data/Inventaire/` (ex. `LaitueMature.asset` — item) et `Assets/Data/Ferme/` (ex. `Laitue.asset` — plante) ; déplacement des `.asset` depuis `Assets/Scripts/Data/`.
- [x] **Menus Create** : chemins regroupés sous `Game/Data/Inventaire/...` et `Game/Data/Ferme/...` (`ItemDefinition`, `ItemDatabase`, `PlantDefinition`, `GridConfig` dans `GridConfig.cs`).
- [x] **Laitue** : configuration du stade **Mature** dans `harvestStages` avec **`harvestItemId` = `laitue_mature`** (aligné sur l’`ItemDefinition` présent dans le projet).
- [x] **Documentation** : création de **`Docs/PLANTES_ET_INVENTAIRE.md`** (checklist nouvelle plante, ItemDatabase, flux runtime, liens scripts).

### Problèmes / rappels
- Tout **ItemDefinition** utilisé en récolte doit être **référencé dans `ItemDatabase`** et l’`itemId` doit matcher **exactement** le `harvestItemId` (casse, underscores).

### Prochaines actions (priorité — **inchangée** prochaine session)
1. **Câblage récolte / inventaire en scène** : `BiofiltreManager` (`itemDatabase`, `playerInventory`, `harvestPanelUI`), `HarvestPanelUI`, `InventoryUI`, prefabs ; test **`FirstLvl`** (ou scène dédiée) : cellule occupée → panel → récolte → slots à jour, cas inventaire plein.
2. Ensuite : verrou récolte / double récolte (Mature vs Seedling) selon `Notes/Todo_project.md`.

### Liens utiles
- `Docs/PLANTES_ET_INVENTAIRE.md`
- `Assets/Data/Inventaire/`, `Assets/Data/Ferme/`
- `Notes/Todo_project.md` — section *Prochaine session*
- `Notes/Farm/TODO_plantation_pipeline.md` — étape 4

## 2026-04-13 — fin de session (pédagogie C#, IDE, architecture inventaire / cloud)

### Contexte
- Session **questions / concepts** (pas de refactor code majeur) : compréhension de `GridData`, navigation IDE, feuille de route **prototype → Unity Gaming Services** (Cloud Save + Economy), vocabulaire produit (**MVP / PMV**).

### Ce qu’on a fait
- [x] **Clarifications C# / `GridData`** : type `byte`, tableau `new byte[columns, rows]` (une valeur par cellule, init à 0 par le runtime) ; méthodes **expression-bodied** (`=>` équivalent à un `return` court) ; chaîne **`AreAllFree`** : `BiofiltreManager` → `GridManager.AreAllCellsFree` → `GridData.AreAllFree`.
- [x] **IDE Cursor** : pourquoi l’extension **C# Microsoft** n’apparaît souvent **pas** dans le marketplace Cursor ; intérêt des extensions **C# « free/libre »** compatibles + rappel **Regenerate project files** (Unity) pour `.csproj`.
- [x] **Note projet** : `Notes/Farm/PlayerInventory_Instance_et_ordre_Awake.md` — `PlayerInventory` via **`Instance`** (plus de drag & drop Inspector pour ce lien) ; risque d’ordre d’**`Awake`** entre GameObjects → **Script Execution Order** si besoin (`PlayerInventory` avant `BiofiltreManager`).
- [x] **Architecture (discussion)** : chemin **prototype local** puis **Cloud Save** (snapshot IDs + qty), puis **Economy** ; le **singleton** client peut rester comme **vue / cache** même avec serveur autoritaire ; introduction **injection de dépendances** vs `Instance`.

### Fiches / liens ajoutés ou mis à jour cette session
- `Notes/Learning/CSharp_bases_et_Cursor_Unity.md` (nouvelle fiche récap pédagogique + IDE)
- `Notes/Learning/README_learning.md` — index mis à jour
- `Notes/Codebase_etat_reference.md` — rappel singleton + note Farm
- `Notes/Todo_project.md` — pointage doc session + backlog cloud optionnel

### Prochaines actions (priorité — **inchangée** côté gameplay)
1. **Câblage récolte / inventaire en scène** (`FirstLvl` ou test dédié) — toujours la priorité #1 (voir `Notes/Todo_project.md`).
2. Si warning **singleton null** au démarrage : vérifier **Script Execution Order** (voir note Farm ci-dessus).
3. Après validation prototype : esquisse technique **Auth → Cloud Save → Economy** (sans bloquer le MVP local).

### Liens utiles
- `Notes/Farm/PlayerInventory_Instance_et_ordre_Awake.md`
- `Notes/Learning/CSharp_bases_et_Cursor_Unity.md`
- `Notes/Todo_project.md`

## 2026-04-13 — complément (priorités : scènes Inventaire / Market)

### Contexte
- L’auteur considère le **noyau inventaire + flux récolte** comme **terminé** pour l’instant.
- **Prochaine session** : scène **Inventaire**, scène **Market**, **boutons UI** présents sur **tous les stages** ; décisions encore ouvertes : **superposition** des scènes / couches UI, **synchrone vs asynchrone** pour le chargement.

### Ce qu’on a fait
- [x] **Note technique** : `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md` — modes `Single` / **Additive**, `LoadScene` vs **`LoadSceneAsync`**, `allowSceneActivation`, **HUD / coque persistante**, `EventSystem` unique, tableau comparatif réactivité, pièges (`DontDestroyOnLoad`, `timeScale`), checklist décisions projet.
- [x] **Hub TODO** : `Notes/Todo_project.md` — *Prochaine session* réorientée (scènes Inventaire / Market + HUD) ; cases inventaire / flux récolte minimal marquées **[x]**.
- [x] **`ASSISTANT_CONTEXT.md`** : priorités alignées sur la navigation scènes + lien vers le guide.

### Prochaines actions (priorité)
1. Trancher **UI prefab** vs **scènes `.unity`** pour Inventaire / Market ; implémenter **prefab HUD** (ou Bootstrap) partagé entre stages.
2. Configurer **Build Settings** + scripts `SceneManager` (ou service dédié) selon le modèle retenu dans le guide.
3. Playtest **réactivité** au clic (pas de hitch : préchargement ou `SetActive` sur UI déjà chargée).

### Liens utiles
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`
- `Notes/Todo_project.md`
- `Notes/Ui/Todo_ui.md` — à croiser pour stack panneaux / options

### Décision / rappel Git
- **Obligatoire** : la prochaine implémentation **scènes / HUD / UI Inventaire–Market** doit démarrer sur une **branche `feature/…`** (pas sur `main`) ; fork GitHub = même logique sur branche du fork. Documenté dans **`GIT_HELPER.md` --3--**, **`WORKFLOW_PROTOCOL.md`**, **`Notes/Todo_project.md`**, en-tête du **guide UI scènes**.

## 2026-04-15 — session (scène prototype inventaire + plan polish/perf)

### Contexte
- Démarrage effectif du chantier **navigation Inventaire/Market**.
- Objectif immédiat : poser la base avec une **scène prototype Inventaire** et préparer les décisions techniques de chargement.

### Ce qu’on a fait
- [x] **Session cadrée** autour de la feature scènes/navigation : la **scène prototype Inventaire** est actée comme point d’entrée du chantier.
- [x] **Todos mis à jour** dans `Notes/Todo_project.md` pour refléter :
  - création de la base prototype Inventaire (fait),
  - tâche de décision/perf sur le mode de chargement (**persistant vs sync vs async/additive**),
  - tâches de **polish UI Inventaire** (visuel + technique).

### Prochaines actions (priorité)
1. Finaliser la scène prototype Inventaire (structure Canvas, panel racine, boutons retour/navigation).
2. Trancher le mode de chargement le plus performant (test rapide : temps d’ouverture perçu, stabilité EventSystem, mémoire).
3. Lancer la passe polish UI Inventaire (lisibilité, feedback interaction, cohérence visuelle) après validation du flux.

### Liens utiles
- `Notes/Todo_project.md`
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`
- `GIT_HELPER.md`

## 2026-04-16 — orientation UI globale multi-scènes

### Contexte
- Le prochain chantier prioritaire côté interface est désormais la mise en place d'une **UI globale** partagée entre toutes les scènes.
- Le besoin UX visé est assumé : **chargement initial plus lourd**, puis **navigation quasi instantanée** entre écrans/scènes UI.

### Ce qu’on a fait
- [x] Parcours du projet et repérage des briques déjà en place : `NavigationHUD`, `InventorySceneController`, `PlayerInventory`, scène `NavigationHUD.unity`, scène `Inventaire.unity`.
- [x] Ajout d'une note d'architecture dédiée : `Notes/Ui/ARCHI_hud_ui_manager_additive.md`.
- [x] Mise à jour du backlog UI dans `Notes/Ui/Todo_ui.md`.
- [x] Mise à jour du journal UI dans `Notes/Ui/Journal_ui.md`.

### Décisions
- Utiliser une **UI shell persistante** commune à toutes les scènes de jeu.
- Précharger en **additif** les scènes UI fréquentes (`Inventaire`, puis `Market`, `Settings`, etc.) au démarrage ou pendant le boot.
- Favoriser ensuite une navigation instantanée via **`SetActive(true/false)`** sur les roots UI déjà chargés, plutôt qu’un `LoadScene` / `UnloadSceneAsync` à chaque clic.
- Centraliser l’orchestration dans un futur **`UIManager` global** plutôt que dans `NavigationHUD` seul.

### Prochaines actions (priorité)
1. Créer la base technique de l’UI globale partagée entre toutes les scènes.
2. Définir le bootstrap de chargement : menu / boot -> shell UI -> `FirstLvl`.
3. Créer un `UIManager` global chargé du préchargement additif et de l’affichage/masquage des roots UI.
4. Précharger `Inventaire` comme premier écran UI global et valider la navigation instantanée en jeu.

### Liens utiles
- `Notes/Ui/ARCHI_hud_ui_manager_additive.md`
- `Notes/Ui/Todo_ui.md`
- `Notes/Ui/Journal_ui.md`
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`

## 2026-04-16 — fin de session (Bootstrap + `UIManager` + priorité hub **Carte**)

### Contexte
- Poursuite du chantier **shell UI / navigation multi-scènes** ; alignement des notes sur le **code réellement présent** dans le dépôt.

### Ce qu’on a fait (état code à date)
- [x] **Scène `Bootstrap.unity`** en entrée Build Settings (index 0) avec **`GameBootstrap`** (`Assets/Scripts/Core/GameBootstrap.cs`) : barre de progression via **`LoadingScreen`**, chargement additif **`NavigationHUD`** puis **`FirstLvl`**, fade-out à la fin.
- [x] **`UIManager`** (`Assets/Scripts/Systems/UIManager.cs`) : singleton `DontDestroyOnLoad`, listes **prioritaires** / **secondaires** (`ScreenEntry` + prefabs), `PreloadPriorityScreens()` au `Start`, API `ShowScreen` / `HideScreen` / `HideAllGlobalUI`, `EnsureShellLoaded()` pour chargement additif du shell si besoin.
- [x] **`ScreenId`** avec au minimum `Inventory` pour les ids d’écrans.
- [x] **`NavigationHUD`** : modes `ShowNavBar` / `ShowExitOnly` / `Hide`, callbacks délégués à **`UIManager`** (inventaire via prefab, plus de logique de chargement de scène inventaire dans le HUD pour ce flux).
- [x] **`NavigationHUD.unity`** référencée dans **Editor Build Settings** (avec `Bootstrap`, `FirstLvl`, `Inventaire`).

### Décisions / cadrage prochaine session
- Introduire une **scène intermédiaire « Carte »** (hub) pour naviguer vers les **multi-scènes** de jeu / features, avec le **HUD persistant** dans le mode d’affichage adapté (barre complète ou règles par contexte — à trancher en implémentation).
- Comportement attendu : depuis **`FirstLvl`**, un clic sur la **croix** (exit) doit **ramener à la scène `Carte`** (pas seulement masquer l’inventaire / `ShowExitOnly` local).
- Documenter le flux exact (unload `FirstLvl` vs stack additive) lors de l’implémentation pour éviter doubles `EventSystem` / fuites de scènes.

### Prochaines actions (priorité immédiate)
1. Créer la scène **`Carte`** (ou nom figé dans Build Settings), la placer dans le flux après Bootstrap : ex. **Bootstrap → shell + Carte** (ou **Bootstrap → shell → Carte** selon ordre choisi), puis chargement des niveaux depuis ce hub.
2. Implémenter la navigation **FirstLvl → Carte** sur **`NavigationHUD.OnExitClicked`** (ou service dédié `SceneFlow` / méthode sur `UIManager`) : `LoadScene` / `UnloadSceneAsync` cohérent avec le HUD déjà en `DontDestroyOnLoad`.
3. Vérifier la cohabitation avec **`Inventaire.unity`** (legacy ou scène encore au build) : soit retirer du build si tout passe par prefab + `UIManager`, soit documenter le double chemin jusqu’à migration complète.

### Liens utiles
- `Notes/Ui/Todo_ui.md` — bloc **Priorité session suivante**
- `Notes/Ui/Journal_ui.md` — décision hub Carte + croix
- `Notes/Ui/ARCHI_hud_ui_manager_additive.md` — état au 2026-04-16
- `Notes/Todo_project.md` — hub **Prochaine session**

## 2026-04-17 — notes (branche navigation + focus LoadingScreen)

### Contexte
- Session **documentation / backlog** : l’auteur confirme que la **branche** de travail pour le chantier navigation / UI est **créée** ; la **prochaine session de travail** est centrée sur la **création et l’intégration d’une image** pour l’écran de chargement (`LoadingScreen` / scène **`Bootstrap`**).

### Ce qu’on a fait
- [x] **Todos** : suppression du bloc *« Impératif Git — créer une branche avant… »* et de la case **Git — branche par feature** liée à ce chantier dans **`Notes/Todo_project.md`** ; réordonnancement : tâche **illustration + intégration LoadingScreen** en tête, **tests QA** load écran séparés après l’art ; hub **`Carte`** conservé comme suite chantier.
- [x] **Guide** : création de **`Notes/Ui/LOADINGSCREEN_image_workflow.md`** (chemins `LoadingScreen.cs`, `GameBootstrap.cs`, `Assets/Scenes/Bootstrap.unity`, import sprite UI, intégration hiérarchie sans C# obligatoire, QA).
- [x] **`Notes/Ui/Todo_ui.md`** : section *Priorité session suivante* — sous-section explicite **focus auteur LoadingScreen** + lien vers le guide ; hub Carte renommé en *suite chantier*.
- [x] **`ASSISTANT_CONTEXT.md`** : branche notée comme créée ; priorités réordonnées (LoadingScreen puis navigation).
- [x] **`WORKFLOW_PROTOCOL.md` (--5--)** et en-tête **`Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`** : formulation **générique** branche + merge (plus de ciblage « obligatoire prochaine feature scènes »).
- [x] **`Notes/Ui/Journal_ui.md`** : ligne de journal 2026-04-17.

### Décisions
- La **procédure** « une branche par gros chantier » reste documentée dans **`GIT_HELPER.md` — --3--** ; seuls les rappels **bloquants / checklist** « à faire avant de commencer » pour la branche déjà créée sont retirés des hubs **Todo** / protocole court.

### Prochaines actions (priorité)
1. **Auteur** : produire l’illustration et l’intégrer selon **`Notes/Ui/LOADINGSCREEN_image_workflow.md`** puis valider en **Play Mode** et **build dev**.
2. **Suite projet** : hub **`Carte`** + **`FirstLvl` → Carte`** (inchangé fonctionnellement, voir entrée 2026-04-16 fin).
3. Trancher / documenter le **mode de chargement UI** final quand le cycle navigation sera repris (`Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`).

### Liens utiles
- `Notes/Ui/LOADINGSCREEN_image_workflow.md`
- `Assets/Scripts/UI/LoadingScreen.cs`
- `Assets/Scripts/Core/GameBootstrap.cs`
- `Assets/Scenes/Bootstrap.unity`
- `Notes/Todo_project.md` — *Prochaine session*

## 2026-04-18 — fin de session (journal + cadrage prochaine session)

### Contexte
- Fin de session : mise à jour de la **documentation** et du **journal** pour figer l’état mental et les priorités.

### Ce qu’on a fait
- [x] **`PROJECT_LOG.md`** : entrée de **fin de session** avec la **prochaine session** explicitement cadrée (voir ci-dessous).
- [x] **`Notes/Todo_project.md`** : ajout des priorités **navigation scènes** (audit + correctifs) et **inventaire / items** (vérification données + UI après ajouts).

### Prochaine session (priorité — auteur)
1. **Navigation entre scènes** : **contrôle** complet des flux (Bootstrap → shell / HUD → niveaux → écrans UI), identification des **régressions** (chargement, ordre des scènes, retours, `DontDestroyOnLoad`), puis **réparation** ; vérifier **Editor Build Settings**, absence de **double `EventSystem`**, et cohérence avec **`GameBootstrap`**, **`UIManager`**, **`NavigationHUD`**.
2. **Items ajoutés à l’inventaire** : **contrôle** côté gameplay — après récolte ou tout autre `TryAdd`, vérifier **`itemId`**, quantités, piles, état **inventaire plein**, et **rafraîchissement UI** ; alignement avec **`ItemDefinition`** / **`ItemDatabase`** si besoin.

### Liens utiles
- `Notes/Todo_project.md` — *Prochaine session*
- `Notes/Ui/ARCHI_hud_ui_manager_additive.md`
- `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md`
- `Assets/Scripts/Core/GameBootstrap.cs`
- `Assets/Scripts/Systems/UIManager.cs`
- `Assets/Scripts/UI/NavigationHUD.cs`
- `Assets/Scripts/Inventory/PlayerInventory.cs`
- `Notes/Farm/SYSTEMES_carte_mentale.md` (Zoom D — récolte ↔ inventaire)

## 2026-04-19 — fin de session (parcours projet + backlog persistance / temps)

### Contexte
- Fin de session : mise à jour des **notes** après **parcours du dépôt** ; ajout de **TODOs** sur la navigation **additive + unload async**, la **persistance de la grille**, et le **temps de croissance** hors scène / hors ligne.

### Ce qu’on a fait (constat code + documentation)
- [x] **Parcours** des briques **navigation multi-scènes** : **`SceneNavigator`** (`LoadSceneAsync` en **additif** puis **`UnloadSceneAsync`** sur la scène de contenu précédente, `Awaitable`, garde-fous `IsTransitioning` / scène identique), constantes **`SceneId`** (`HomeScene`, `Inventaire`, `FirstLvl`, `Map`, …), **`GameBootstrap`** (chargement **`NavigationHUD`** puis **`HomeScene`**, appel **`SetInitialScene`**), **`NavigationHUD`** (onglets → `GoTo`, **`OnExitToHomeRequested`** en mode exit-only), **`FirstLvlController`** (retour **`HomeScene`**), **`MapSceneController`** sur **`HomeScene`** (hub + **`MapProgressionData`**).
- [x] **Documentation** : cette entrée ; hub **`Notes/Todo_project.md`** ; **`Notes/Ui/Todo_ui.md`** ; **`ASSISTANT_CONTEXT.md`** ; **`Notes/Codebase_etat_reference.md`** ; **`Notes/Ui/ARCHI_hud_ui_manager_additive.md`** ; ajustement **`Notes/Ui/Journal_ui.md`** (hub **`HomeScene`** vs ancienne formulation **`Carte`** seule).

### Problèmes / pistes
- La pile **scène de contenu unique + shell persistant** reste à **valider en playtest** (ordre load/unload, activation, transitions concurrentes, cohabitation **`UIManager`** / panneaux globaux).
- **Grille / farm** : aujourd’hui surtout **runtime en scène** — pas de sauvegarde systématique de l’occupation ni des timers de croissance à la fermeture.

### Prochaines actions (priorité)
1. **Navigation inter-scène / UI** : **debug** et **amélioration** du flux **additif + `UnloadSceneAsync`** (tous les chemins, Build Settings, pas de double **`EventSystem`**) — voir **`Notes/Todo_project.md`** et **`Notes/Ui/Todo_ui.md`**.
2. **Persistance grille** : faire persister l’**état de la grille** à la **fermeture de scène** et à la **fermeture du jeu** (piste **`ScriptableObject`** + évolution save fichier / JSON) — croiser `GridData`, `GridManager`, `BiofiltreManager`.
3. **Croissance hors scène / hors ligne** : recalcul du **temps restant** / des **stades** via **timestamps UTC** à la reprise ; **cloud** (ex. UGS Cloud Save) noté comme **piste future** si besoin multi-appareil.

### Liens utiles
- `Assets/Scripts/Systems/SceneNavigator.cs`
- `Assets/Scripts/Core/GameBootstrap.cs`
- `Assets/Scripts/UI/NavigationHUD.cs`
- `Assets/Scripts/UI/FirstLvlController.cs`
- `Assets/Scripts/UI/Map/MapSceneController.cs`
- `Assets/Scripts/Systems/ScreenId.cs` (`SceneId`)
- `Notes/Todo_project.md`

## 2026-04-20 — fin de session matin (PC portable)

### Contexte
- Clôture session du matin sur portable.
- Recentrage backlog sur le chantier inventaire multi-scènes.

### Ce qui est en cours
- [~] **Séparation inventaire / gameplay** : isolation progressive de l’inventaire hors logique de gameplay de `FirstLvl`, avec cible d’extension à **tous les niveaux** (architecture partagée).
- [~] **Inventaire persistant JSON** : mise en place d’une persistance locale via fichier JSON pour conserver l’état inventaire entre scènes et sessions.

### Problèmes / interruption
- La tâche inventaire a été **interrompue côté BezyIA** avant finalisation du flux dans la scène dédiée inventaire.
- Après interruption, la session Unity a été fermée ; reprise à faire en s’appuyant en priorité sur le thread de travail.

### TODO immédiat (prochaine reprise)
1. **Inventaire scène dédiée** : remettre l’inventaire en état fonctionnel dans sa scène dédiée (validation UI + data chargées depuis JSON).
2. **Continuité architecture** : poursuivre la séparation inventaire/gameplay pour `FirstLvl` puis généraliser aux autres niveaux.
3. **Reprise BezyIA** : relancer exactement avec ce prompt :
   - `"encore une fois il y a eu une coupure peux tu reprendre toutefois j'ai du fermer la session unity entre temps donc je ne sais pas si tu va retrouver toutes les traces necessaires. il te faudra te fier au thread."`

### Liens utiles
- `Notes/Todo_project.md` — bloc *Prochaine session (priorité immédiate)*
- `ASSISTANT_CONTEXT.md` — snapshot de reprise
- `Assets/Scripts/UI/NavigationHUD.cs`

## 2026-04-21 — notes doc + correctif retour gameplay + cible audit Bezi

### Contexte
- Demande : parcourir le projet, compléter les notes utiles, mettre à jour le journal, planifier **~10 jours** une session **audit Bezi** sur la nouvelle gestion **Scene / UI**, puis **clean/refactor** ; repérer **code orphelin** et **rajeunir les commentaires** selon le code actuel.

### Ce qu’on a fait
- [x] **Note dédiée** : `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md` (checklist audit Bezi, refactor, alignement doc / commentaires, code mort).
- [x] **Hub TODO** : `Notes/Todo_project.md` et `Notes/Ui/Todo_ui.md` — bloc *Session cible ~2026-05-01* avec renvoi vers la note ci-dessus.
- [x] **Index doc** : `INDEX.md` — entrée pour la nouvelle note UI.
- [x] **`Notes/Codebase_etat_reference.md`** : alignement sur le flux **`SceneNavigator.ShowScene`** (visibilité par `SetActive` sur racines + lazy optionnel) et boot **`GameBootstrap`** (eager **`Inventaire`** + note de dérive doc vs ancien **`GoTo` / Unload**).
- [x] **`ASSISTANT_CONTEXT.md`** : priorité ajoutée pour l’audit / refactor navigation UI.
- [x] **Code** — **`FirstLvlController`** : souscription manquante à **`OnExitToHomeRequested`** dans `Start` (le `OnDestroy` désabonnait sans jamais s’abonner — la croix ne pouvait pas déclencher le retour hub).
- [x] **Code** — **`BiofiltreManager`** : commentaires XML sur **`TryOpenHarvestPanel`** / **`FindInteractorAt`** (hors flux clic grille actuel ; candidats suppression au nettoyage).
- [x] **Code** — **`GameBootstrap`** : résumé XML du boot (shell + `HomeScene` + `Inventaire` eager, masquage racines inventaire).
- [x] **Style** — **`MapSceneController`** : indentation du bloc `OnEnable`.

### Prochaines actions (priorité — ~2026-05-01)
1. **Audit Bezi** : navigation **`ShowScene`**, liste lazy, cohabitation scène **`Inventaire`** / prefab **`UIManager`**, **`EventSystem`** — détail **`Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`**.
2. **Refactor** : trancher **`TryOpenHarvestPanel`** / **`FindInteractorAt`** ; harmoniser **`ARCHI_hud_ui_manager_additive.md`**, **`Journal_ui.md`**, **`Todo_ui.md`**, guide scènes avec le code (plus de **`UnloadSceneAsync`** par clic si obsolète).
3. Poursuivre l’inventaire des **scripts réservés scène** vs **morts** (ex. usages de **`InventorySceneController`**).

### Liens utiles
- `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`
- `Assets/Scripts/UI/FirstLvlController.cs`
- `Assets/Scripts/Systems/SceneNavigator.cs`
- `Assets/Scripts/Core/GameBootstrap.cs`

## 2026-04-22 — architecture Scene/UI réactive + note de rule Bezi

### Contexte
- Reprise du chantier navigation/UI sur base `feature/scene-inventaire`, avec objectif : architecture plus **réactive**, **propre** et **performante**.
- Consolidation documentaire demandée : produire une note exploitable comme future **rule Bezi IA**.

### Ce qu’on a fait
- [x] **Refactor navigation réactive** (`SceneNavigator`) :
  - événements de disponibilité (`OnNavigatorAvailable` / `OnNavigatorUnavailable`);
  - événement d’état de transition (`OnTransitionStateChanged(bool)`);
  - sérialisation des requêtes `ShowScene` (gestion des demandes concurrentes via `pendingSceneName`);
  - fallback robuste de chargement (`EnsureSceneLoaded`) + helper `IsSceneLoaded`.
- [x] **HUD piloté par état** (`NavigationHUD`) :
  - bind/unbind dynamique au navigator;
  - mode HUD dérivé de la scène active et de l’état de transition (`Hidden`, `Navigation`, `ExitOnly`);
  - suppression de plusieurs appels impératifs redondants.
- [x] **UI globale** (`UIManager`) :
  - écoute des transitions et masquage global au début d’un changement de scène;
  - API utilitaires (`HasScreen`, `TryShowScreen`, `TryHideScreen`).
- [x] **Suppression du prototype conflictuel** : retrait de `UIFlowController` (single-canvas agressif), incompatible avec l’architecture additive shell + scènes de contenu.
- [x] **Unification navigation** : `MainMenuUI` passe par `SceneNavigator` (plus de `LoadScene` direct côté scripts gameplay).
- [x] **Étape de centralisation inventaire** :
  - onglet Inventaire du HUD ouvre en priorité un écran global `ScreenId.Inventory` géré par `UIManager`;
  - fallback vers `SceneNavigator.ShowScene(SceneId.Inventaire)` si écran global indisponible;
  - migration progressive depuis la scène `Inventaire` (clone de `InventoryCanvas` sous `ScreenRoot`).
- [x] **Documentation** :
  - nouvelle note : `Notes/Ui/RULE_DRAFT_bezi_scene_ui_runtime.md` (fonctionnement runtime Scene/UI + conventions + anti-patterns + checklist PR) ;
  - index mis à jour (`INDEX.md`) ;
  - backlog TODO enrichi avec point “session de ce soir” sur la suppression de la dépendance runtime à la scène `Inventaire`.

### Décisions
- Conserver le modèle **shell persistant + scènes additives** (adapté Unity 6, simple à debugger, coûts maîtrisés).
- Centraliser la logique d’état (navigator + UI manager), sans forcer un canvas unique monolithique.
- Faire la migration de l’inventaire en **progressif** pour limiter le risque de régression.

### Session de ce soir — TODO ciblé
1. Finaliser la migration inventaire : **supprimer la dépendance runtime** à `Inventaire.unity` (garder template/prefab source).
2. Vérifier les références UI (close button, binding inventaire, slot prefab sans wrapper canvas).
3. Valider en Play Mode les transitions Home ↔ Inventaire ↔ FirstLvl avec overlay UI global.

### Liens utiles
- `Assets/Scripts/Systems/SceneNavigator.cs`
- `Assets/Scripts/Systems/UIManager.cs`
- `Assets/Scripts/UI/NavigationHUD.cs`
- `Assets/Scripts/UI/Inventory/InventorySceneController.cs`
- `Assets/Scripts/UI/MainMenuUI.cs`
- `Notes/Ui/RULE_DRAFT_bezi_scene_ui_runtime.md`
- `Notes/Todo_project.md`

### TODO prioritaire — prochaine session (soir ou autre)
1. **Git hygiene / livraison** :
   - fermer le **PR obsolète** #1 (branche source déjà supprimée) ;
   - vérifier qu'il ne reste qu'un seul flux actif (PR #2 vers `feature/scene-inventaire`).
2. **Nettoyage branches** :
   - valider la liste des branches locales/distantes encore utiles ;
   - supprimer les branches de travail périmées après confirmation de merge.
3. **Stratégie de fusion finale** :
   - une fois tests validés, fusionner proprement vers la branche inventaire cible ;
   - planifier ensuite le nettoyage de `main` (PR cleanup dédié plutôt qu'un mix de branches concurrentes).

## 2026-04-26 — session inventaire runtime + persistance gameplay JSON (prototype)

### Contexte
- Session orientée **validation prototype** : retirer la dépendance runtime à `Inventaire.unity`, stabiliser un inventaire global utilisable, puis poser une première persistance gameplay locale.

### Ce qu’on a fait
- [x] **Règle projet ajoutée** : nouvelle règle always-apply `scene_ui_runtime.mdc` pour verrouiller navigation `SceneNavigator`, HUD/UIManager, transitions, anti-patterns.
- [x] **Inventaire runtime** :
  - suppression de la dépendance à la scène `Assets/Scenes/Inventaire.unity` (retirée du build puis supprimée du repo) ;
  - `NavigationHUD` passe en ouverture inventaire via `UIManager` uniquement ;
  - `GameBootstrap` ne précharge plus `Inventaire` ;
  - fallback inventaire runtime créé (`RuntimeInventoryScreen`) puis aligné sur une **grille de slots** avec `InventorySlotUI` (proche du comportement précédent).
- [x] **Stabilisation runtime inventaire** :
  - correction `NullReferenceException` sur construction du panel runtime ;
  - correction font Unity 6 (`LegacyRuntime.ttf`).
- [x] **Roadmap/notes cloud** :
  - création `Notes/Ui/SPEC_services_inventory_market_cloud.md` (services `IInventoryService`/`IMarketService`, UI vue-only, états async, cache/retry, serveur autoritaire) ;
  - ajout explicite du prérequis **découplage gameplay -> inventaire** avant (ou tout début de) l’implémentation cloud.
- [x] **Persistance gameplay prototype (ferme)** :
  - ajout `FarmSaveService` (`farm_state.json`) ;
  - ajout `PlantPersistenceMarker` ;
  - autosave sur pose/récolte/arrachage + save à la fermeture ;
  - load/rebuild au démarrage du biofiltre + reprise offline simple (`lastSavedUtc` -> delta) via `PlantGrow.AdvanceBySeconds`.

### Problèmes rencontrés / pistes
- Le fallback inventaire runtime fonctionne pour prototype, mais nécessite un **polish UI** ultérieur (style, hiérarchie visuelle, animations).
- La doc contient encore des traces historiques "Inventaire scène dédiée" / anciens flux ; une passe de nettoyage global est souhaitable.
- **SO Plante (état actuel)** : deux `PlantDefinition` / SO liés à la laitue coexistent dans le runtime. Supprimer l'un des deux provoque une régression/casse du gameplay de plantation (seed UI / refs prefab-scène). Tant que la migration de références n'est pas traitée proprement, **ne supprimer aucun des deux**.

### Prochaines actions (priorité)
1. **Tester et affiner** la persistance gameplay JSON (pose, récolte, arrachage, relance jeu, progression offline).
2. Faire un **audit global de documentation projet** (cohérence code vs notes, nettoyage historique obsolète, consolidation des guides).
3. Poursuivre refactor/clean des scripts orphelins et commentaires (suite audit Bezi + doc globale).

### Liens utiles
- `Assets/Scripts/Farm/FarmSaveService.cs`
- `Assets/Scripts/Farm/PlantPersistenceMarker.cs`
- `Assets/Scripts/Farm/BiofiltreManager.cs`
- `Assets/Scripts/UI/Inventory/RuntimeInventoryScreen.cs`
- `Notes/Ui/SPEC_services_inventory_market_cloud.md`
- `Notes/Todo_project.md`

## 2026-04-27 — données laitue (un seul SO) + nettoyage tests + suivi Git / croissance

### Contexte
- Travail sur la branche de test / agent (références laitue, prefabs, `PlantDefinition`) et alignement avec **`main`**.
- Objectif produit : une **seule** définition plante laitue sous **`Assets/Data/Ferme/Laitue.asset`** (plus de doublon type ancien chemin sous `Scripts/Data`).

### Ce qu’on a fait
- [x] **Laitue** : consolidation des références — **un seul** `PlantDefinition` laitue dans **`Data/Ferme`** ; prefabs / UI (`SeedSelection`, prefab monde) pointent vers ce fichier ; items inventaire distincts (ex. graine / mature) restent des **`ItemDefinition`** séparés, ce qui est normal.
- [x] **Tests EditMode** : retrait du dossier **`Assets/Tests/`** et de l’assembly associé (`Farm.EditModeTests`) pour ne garder que le code **gameplay** exploitable en éditeur / build ; retrait de l’assembly runtime **`Rayman.Game`** (les scripts sous `Assets/Scripts` repassent dans l’assembly par défaut).
- [x] **Packages** : suppression de la dépendance directe **`com.unity.test-framework`** dans `Packages/manifest.json` (re-sync Unity au prochain lancement).

### Problèmes restants / à traiter (gameplay)
- **Stades / durées** : après changement des **durées** ou des règles de stade dans `PlantDefinition`, le **state affiché ou la progression** peut ne pas se recaler comme attendu (timers déjà démarrés, sérialisation partielle, ou absence de re-init au reload asset).
- **Persistance** : l’**état de croissance** (stade, timers, **temps déjà écoulé** sur le cycle) n’est pas garanti **permanent** tant que la chaîne save/load ne sérialise pas explicitement ces champs et ne les réapplique pas au réveil / changement de scène (croiser `FarmSaveService`, `PlantPersistenceMarker`, `PlantGrow`).
- **Git / livraison** : décider si on intègre le travail de la branche de test dans **`main`** via **PR** (recommandé) ou en **remplaçant** `main` par la branche (plus brutal) — action suivante : voir **`Notes/Todo_project.md`**.

### Prochaines actions (priorité)
1. **Git** : ouvrir une **PR** `cursor/test-laitue-references-9dc0` (ou branche équivalente) → **`main`**, ou documenter explicitement l’option « reset main » si tu la choisis volontairement.
2. **Croissance** : audit `PlantGrow` + save — **sérialiser** stade + temps écoulé (ou timestamps UTC) et **réappliquer** les durées du SO au chargement ; vérifier le cas « SO modifié en cours de partie ».
3. **Playtest** : scénario pose → quitter jeu / recharger scène → vérifier **stade** et **temps déjà consommé** sur chaque plante persistée.

### Liens utiles
- `Assets/Data/Ferme/Laitue.asset`
- `Assets/Scripts/Farm/PlantGrow.cs`
- `Assets/Scripts/Farm/FarmSaveService.cs`
- `Assets/Scripts/Farm/PlantPersistenceMarker.cs`
- `Notes/Todo_project.md`
- `GIT_HELPER.md` (--3-- branche + merge)

## 2026-04-29 — base Shop clone inventaire + points de liaison

### Contexte
- Démarrage de la feature **Shop/Magasin** sur la branche de travail dédiée.
- Objectif session : poser une base fonctionnelle rapide en réutilisant la logique inventaire existante.

### Ce qu’on a fait
- [x] **Base Shop runtime** : création d'un écran `Shop` calqué sur l'inventaire (grille de slots, `slotsContainer`, refresh sur `PlayerInventory.OnInventoryChanged`).
- [x] **Navigation HUD** : ajout du support onglet Shop dans `NavigationHUD` (ouverture/fermeture via `UIManager`, coexistence avec l'onglet Inventaire).
- [x] **UIManager / ScreenId** : ajout de `ScreenId.Shop` + fallback runtime `RuntimeShopScreen` similaire à `RuntimeInventoryScreen`.
- [x] **Règle UX conservée** : la barre de navigation persistante reste affichée dans le shell, conformément au flux Scene/UI en place.

### Points d'attention identifiés
- Certaines **références Inspector sont encore vides** selon les scènes/prefabs (notamment liaison bouton/icône Shop et potentiels prefabs UI dédiés).
- Pas de **prefab Shop final** prêt à brancher ; la base actuelle repose sur le runtime fallback.

### Prochaines actions (priorité)
1. Créer la ressource **Argent** (item dédié) et définir sa place dans le flux Inventaire + Shop.
2. Faire une passe de **linkage Inspector** (SerializeField non assignés / références nulles) sur les éléments UI liés au Shop.
3. Produire/valider un **prefab Shop** (ou demander support Bezi.ia pour accélérer la production UI), puis remplacer progressivement le fallback runtime.

### Liens utiles
- `Assets/Scripts/Systems/ScreenId.cs`
- `Assets/Scripts/Systems/UIManager.cs`
- `Assets/Scripts/UI/NavigationHUD.cs`
- `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs`
- `Notes/Todo_project.md`

## 2026-05-04 — doc Shop + backlog flux achat (argent)

### Contexte
- Demande : aligner la **documentation** sur la **gestion du shop** (état code + intention), mettre à jour **`PROJECT_LOG.md`**, et consigner en **todo prochaine session** la **mécanique d’achat** (UI détail, quantité, confirmation, monnaie, inventaire).

### Ce qu’on a fait
- [x] **`Notes/Ui/popup_generique.md`** : état réel (**catalogue JSON** `MarketCatalogPrototype` + `Resources/Market/market_catalog.json`, slots = offres, pas miroir inventaire) ; **spec flux achat** (clic → fenêtre image/prix/description optionnelle → +/−/payer, total `prix × quantité` sur le bouton, popup confirmation, succès `TryAdd` + débit / échec manque de fonds) ; polish **saisie quantité clavier** noté ; **Argent** comme première ressource monétaire.
- [x] **`Notes/Todo_project.md`** : case doc shop cochée ; nouvelle tâche **Shop — flux achat dédié** (référence croisée doc + `Todo_ui.md`).
- [x] **`Notes/Ui/Todo_ui.md`** : bloc **Shop — mécanique achat** avec checklist alignée sur la spec.
- [x] **`PROJECT_LOG.md`** : cette entrée.

### Prochaines actions (priorité — shop / économie)
1. **Argent** : `ItemDefinition` + entrée `ItemDatabase` (ou modèle solde retenu) et affichage solde dans le shop / HUD si pertinent.
2. **Implémenter le flux §3** de `popup_generique.md` : modal détail, quantité, libellé bouton avec total, confirmation, branchement `TryAdd` + messages fonds insuffisants.
3. Poursuivre linkage Inspector + prefab Shop dédié quand le flux métier est stable.

### Liens utiles
- `Notes/Ui/popup_generique.md`
- `Assets/Scripts/Market/MarketCatalogPrototype.cs`
- `Assets/Resources/Market/market_catalog.json`
- `Notes/Todo_project.md` — *Prochaine session*
- `Notes/Ui/Todo_ui.md` — *Shop — mécanique achat*

## 2026-05-07 — fin de session (shop popup + inventaire + priorite monnaie)

### Contexte
- Session de stabilisation du shop runtime: interaction clic item, popup d'achat, liaison donnees ScriptableObject.
- Demande de cloture: preparer clairement la prochaine session avec une priorite unique.

### Ce qu’on a fait
- [x] Shop popup: correction du flux de clic (slots cliquables + ouverture popup resolue).
- [x] Donnees shop: ajout des SO `ShopItemDefinition` / `ShopCatalogDefinition` et branchement dans `RuntimeShopScreen` (fallback JSON conserve).
- [x] Catalogue: retrait de `LaitueMature` du catalogue principal, conservation de l'asset renomme en `SellItem_LaitueMature`.
- [x] Achat -> inventaire: branchement du flux shop sur `PlayerInventory.TryAdd(...)` (meme comportement que recolte), avec gestion `Success/Partial/Full`.

### Prochaine session (priorite immediate)
1. **Monnaie inventaire + deduction achat**: creer la ressource monnaie (item + database + solde UI) et deduire le montant lors de l'achat shop avant `TryAdd`.
2. Bloquer l'achat si fonds insuffisants avec feedback utilisateur explicite.

### Suivi memoire de session
- [x] Ajout d'une regle always-apply: `.cursor/rules/session_planning_memory.mdc`.
- Objectif: quand l'utilisateur demande "quelle est la tache du jour/prochaine fois", ressortir en priorite la tache monnaie/deduction shop.

### Liens utiles
- `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs`
- `Assets/Scripts/Shop/ShopItemDefinition.cs`
- `Assets/Scripts/Shop/ShopCatalogDefinition.cs`
- `Notes/Todo_project.md`

## 2026-05-09 — refactor UI runtime (shop editor-first) + wallet inventaire + hygiene Git

### Contexte
- Session orientee stabilisation UI runtime: clarifier la source de verite des ecrans, sortir de la generation d'ecran en code pour le shop, et securiser l'inventaire runtime.
- Demande utilisateur: confirmer l'etat des branches, garder un seul flux distant coherent sur `feature/shop`.

### Ce qu'on a fait
- [x] **UIManager**: suppression du fallback de creation d'ecrans runtime en code (`EnsureInventoryScreenAvailable` / `EnsureShopScreenAvailable`).
- [x] **Shop editor-first**: refonte de `RuntimeShopScreen` pour fonctionner sur un prefab configure dans l'editeur (bindings serializes), sans construction complete de la hierarchie UI en code.
- [x] **Prefab Shop**: remplacement de `Assets/Prefabs/Ui/ShopScreen.prefab` par une version configuree pour le runtime.
- [x] **Inventaire runtime / wallet**: ajout explicite d'une instance `WalletRowUI` dans `Assets/Prefabs/Ui/InventoryScreen.prefab` (section `ExpandedPanel`) pour garantir la presence cote prefab runtime.
- [x] **Scene legacy**: suppression de `Assets/Scenes/Inventaire.unity` (et `.meta`) apres verification qu'elle n'etait plus utilisee par le HUD/runtime ni dans les Build Settings actifs.
- [x] **Clarification architecture**: confirmation que le HUD ouvre l'inventaire via `UIManager.TryShowScreen(ScreenId.Inventory)` (prefab runtime), pas via chargement de scene inventaire.
- [x] **Hygiene Git**:
  - push des changements sur la branche distante historique `origin/feature/shop`;
  - suppression de la branche distante parasite `origin/shop`;
  - branche locale `shop` re-rattachee a `origin/feature/shop`.

### Points d'attention
- Le flux runtime inventaire depend maintenant du prefab `Assets/Prefabs/Ui/InventoryScreen.prefab` comme source de verite.
- Si des ajustements visuels wallet/inventaire sont faits plus tard, les appliquer en priorite sur ce prefab runtime.

### Prochaines actions (priorite)
1. Playtest Unity complet en runtime (HUD -> Inventaire -> Shop -> retour) et verification console.
2. Verifier le comportement visuel/UX du wallet (`ExpandedPanel`, lignes dynamiques, duplication eventuelle des rows) apres ouverture/fermeture repetee de l'inventaire.
3. Poursuivre la priorite monnaie shop (debit, blocage fonds insuffisants, feedback UI) selon le backlog.

### Liens utiles
- `Assets/Scripts/Systems/UIManager.cs`
- `Assets/Scripts/UI/Shop/RuntimeShopScreen.cs`
- `Assets/Prefabs/Ui/ShopScreen.prefab`
- `Assets/Prefabs/Ui/InventoryScreen.prefab`
- `Assets/Scripts/UI/NavigationHUD.cs`
- `ProjectSettings/EditorBuildSettings.asset`