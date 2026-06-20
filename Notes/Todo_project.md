# Todo projet — source unique des tâches

Ce fichier est la **source de vérité unique** pour le statut des tâches du projet.

Règle de suivi :
- **Uniquement ici** on utilise `[ ]`, `[~]`, `[x]` pour le statut des tâches (historique des closes : `PROJECT_LOG.md`).
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

### Contexte Git (rappel obligatoire « tâche du jour »)

> Branche courante : **`main`** — chantier **canaux de vente** démarré (2026-06-17) : shell HUD onglet **Vente** + écran `SaleChannelsScreen` placeholder. Commit local recommandé avant session Bezy.

### Reprise session — canaux de vente (bandeaux Bezy)

> **Priorité immédiate :** remplacer le shell par la **liste scrollable de bandeaux** (Voisinage actif ★1, bandoulière/vélo verrouillés) via **Bezy**, puis playtest HUD → Vente.

**Ordre suggéré :**

1. [ ] **[P0-SALE-PLAY-001]** Playtest rapide shell actuel : Bootstrap → Home → onglet **Vente** → écran placeholder + fermeture OK.
2. [ ] **[P0-SALE-BEZI-001]** Bezy **Phase 1** — scroll + hiérarchie bandeaux (`PROMPTS_Bezi_sale_channels.md`).
3. [ ] **[P0-SALE-BEZI-002]** Bezy **Phase 2** — visuels bandeau (étoiles ★, illustration placeholder, overlay verrouillé).
4. [ ] **[P0-SALE-BEZI-003]** Bezy **Phase 3** — wiring Inspector + review Cursor (`SaleChannelBandeauView` si script ajouté).
5. [ ] **[P0-SALE-PLAY-002]** Playtest post-Bezy : 3 bandeaux scrollables, Voisinage actif, 2 verrouillés lisibles.

**Session suivante (Cursor, après bandeaux OK) :**

- [ ] **[P0-SALE-CODE-001]** `SaleChannelService` + popup vente salades (cap 2) — spec GDD §2.5.

**Références**

- UI / architecture : `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`
- Prompts Bezy phasés : `Notes/Ui/PROMPTS_Bezi_sale_channels.md`
- GDD économie : `Notes/GDD/SPEC_vente_production_boucle_jeu.md`

---

### Clos — shell HUD Vente (2026-06-17)

- [x] **[P0-SALE-SHELL-001]** `ScreenId.SaleChannels` + onglet HUD **Vente** + prefab shell `SaleChannelsScreen.prefab`.

---

### Reprise session (historique — arbre talents)

> **Arbre talents Commerce : MVP validé** — feature gelée ; reprise plus tard en polish (backlog `BL-INV-TALENT-*`).

---

### Clos — lot halo / arbre talents (2026-06-16)

- [x] **[P0-INV-HALO-012]** Composition `Track_Commerce` + binding overlay (workflow étapes 0–8).
- [x] **[P0-INV-HALO-013]** Playtest arbre Commerce — **MVP OK** (overlay, nœuds, achat mock, Retour).

---

### Ordre session (historique 2026-06-12)

**1 — Playtest MVP talents — clos**

- [x] Playtest MVP talents sur **`main`** — **validé** (2026-06-12) : halo P1 Commerce → overlay → achat nœud mock (Racine + branches) → Retour OK.

**Playtest inventaire halo — clos**

- [x] **[P0-INV-HALO-004]** **Playtest** onglet Inventaire (HUD) sur **`main`** — **validé** (2026-06-10) :
  - 8 slots visibles (P1–P8, pas de prefab cassé/pink),
  - clic P1–P8 → overlay talents + titre piste,
  - **Retour** → overlay fermé + grille restaurée,
  - grille inventaire bien visible (pas de HUD en fond, header non décalé).

**2 — Code / UI halo — historique**

- [x] Bezy **Phase 1** shell — validée Cursor (2026-06-04).
- [x] **[P0-INV-HALO-002]** Phase 2 + 2 bis — débloquée **Cursor** (2026-06-05) : régression slot réparée, GUID panel corrigé, `BodyText` TMP ajouté.
- [x] **[P0-INV-HALO-003]** **Phase 3 wiring** (Bezy) — **review Cursor OK** (2026-06-05) : `PlayerHaloSlotUI`, `PlayerHaloPanelController` (slots 01→08 ordonnés), `TalentTreeOverlayController`, `InventoryScreenController` câblés.
- [x] Fix layout (2026-06-05) : `VerticalLayoutGroup.ChildControlHeight=1` → grille restaurée.

**2 — [PRIORITÉ IMMÉDIATE] Lier halo → arbres de compétences (visuel éditeur)** — foundation Cursor OK (2026-06-12)

> Plan historique (3 étapes) : **`Notes/Ui/SESSION_prochaine_halo_arbres_competences.md`**  
> **Décision layout 2026-06-12** : arbres composés **à la main dans l'éditeur Unity** — spec **`Notes/Ui/SPEC_talent_tree_layout_editeur.md`**, procédure **`Notes/Ui/WORKFLOW_creation_arbre_talents.md`**.

- [x] **[P0-INV-HALO-006]** **Étape 1 (code)** — `ProgressionTrackId` renommé (6 pistes + 2 réservées) + `HaloSlotOrder`.
- [~] **[P0-INV-HALO-006b]** Aligner prefab Bezy slots (labels / `trackId` Inspector) — playtest OK ; reste polish prefab.
- [~] **[P0-INV-HALO-007]** **Étape 2** — SO + `TalentProgressionService` mock Commerce en runtime ; overlay texte + achat OK. Reste : assets SO sur disque + prefab arbre visuel.
- [x] **[P0-INV-HALO-009]** **Décision layout** — 1 prefab/piste + `TreeContent` partagé + swap dynamique (2026-06-12, cf. spec).
- [x] **[P0-INV-HALO-008]** **Bezy briques arbre** — Ph.1–3 **validées Cursor** (2026-06-12) : `TreeScrollView`, `TreeContent`, `TalentNodeView`, `TalentTreeEdgeView`, wiring overlay (`treeContentHost`).
- [x] **[P0-INV-HALO-011]** **Cursor foundation** — scripts + Custom Editor + `TalentTreeOverlayController` swap prefab (2026-06-12).
- [x] **[P0-INV-HALO-012]** **Auteur composition** — `Track_Commerce.prefab` : MVP **validé** (2026-06-16). Polish → backlog `BL-INV-TALENT-*`.

**4 — Notes tablette perso (complément)**

- [~] **[P0-IDEA-001]** Synthèse halo importée dans **`INBOX_notes_tablette_recherches.md`** § *Synthèse 2026-06-05* — reste à compléter depuis tablette (coûts, effets %, nœuds détaillés).
- [ ] Trace finale dans **`PROJECT_LOG.md`** quand étapes 1–3 validées.

**Références**

- **Layout éditeur (décision 2026-06-07)** : `Notes/Ui/SPEC_talent_tree_layout_editeur.md`
- **Procédure arbre talents 8 étapes (auteur)** : `Notes/Ui/WORKFLOW_creation_arbre_talents.md`
- **Bezy fix affichage arbre (Phase 4)** : `Notes/Ui/PROMPTS_Bezi_talent_tree.md`
- **Session prochaine (3 étapes historiques)** : `Notes/Ui/SESSION_prochaine_halo_arbres_competences.md`
- Spec halo : `Notes/Ui/SPEC_rework_inventaire_halo_progression.md`
- Arbre UI : `Notes/Ui/ARBRE_inventory_halo_ui.md`
- Fallback wiring : menu Unity `Rayman → UI → Wire Inventory Halo (Phase 3)` (`Assets/Editor/InventoryHaloPrefabWiring.cs`)
- Hub tablette : `Notes/GDD/INBOX_notes_tablette_recherches.md`

### Bugs / playtests ferme — clos

- [x] **[P0-FARM-BUG-001]** Popup empty + slot simultanés après achat shop. **Playtest validé** (2026-06-02).
- [x] **[P0-FARM-BUG-002]** Panel info plante après plantation. **Playtest validé** (2026-06-02).
- [x] **[P0-FARM-PLAY-001]** Boucle graines complète. **Playtest validé** (2026-06-02).
- [x] **[CT-FARM-POLISH-001]** Toast récolte **`FarmHarvestReward`**. Merge `main` 2026-06-02.

---

## Stock en attente (priorités à valider après [P0-IDEA-001])

> Tâches connues **gelées** : ne pas implémenter tant que la session idées n'a pas produit et validé le nouvel ordre P0/CT.

| Ordre indicatif | ID | Intention | Détail |
|-----------------|-----|-----------|--------|
| 1 | **[CT-INV-HALO-001]** | Rework inventaire halo + grille — Ph.1–3 + playtest **[P0-INV-HALO-004]** OK sur `main` ; suite arbres talents sur `cursor/mvp-talent-tree-950d` | `Notes/Ui/SPEC_rework_inventaire_halo_progression.md` |
| — | **[CT-FARM-UI-001]** | EmptyStatePanel graines (polish) | Prefab `SeedSelectionUI` — §4.4 `Notes/Farm/REFACTOR_graines_plantation_inventaire.md` ; **reclassé polish** (2026-06-02), plus P0 immédiat |
| — | **[CT-SHOP-002]** | Polish UX shop | Optionnel |
| — | **[CT-FARM-004]** | Persistance ferme scénario complet | Playtest long |
| — | **[CT-INV-001]** … **[CT-NAV-004]** | Voir § *Court terme* | Inchangé |

### Doc — popups
- [~] [BL-POP-DOC-001] `popup_generique.md` §2.5 aligné. Reste : `SYSTEMES_carte_mentale.md`, `Codebase_etat_reference.md`.

---

## Court terme (priorités actives)

### Canaux de vente — UI bandeaux (Bezy + Cursor)

> Détail : `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md` — statut tâches : § *Prochaine session* ci-dessus.

- [x] **[P0-SALE-SHELL-001]** Shell HUD : onglet Vente + `ScreenId.SaleChannels` + prefab placeholder.
- [ ] **[P0-SALE-BEZI-001]** Bezy Phase 1 — scroll + prefab `SaleChannelBandeauView` + 3 bandeaux instanciés.
- [ ] **[P0-SALE-BEZI-002]** Bezy Phase 2 — visuels bandeau (★, illustration, locked).
- [ ] **[P0-SALE-BEZI-003]** Bezy Phase 3 — wiring Inspector.
- [ ] **[P0-SALE-PLAY-001]** Playtest shell HUD Vente.
- [ ] **[P0-SALE-PLAY-002]** Playtest bandeaux post-Bezy.
- [ ] **[P0-SALE-CODE-001]** `SaleChannelService` + popup vente V0 voisinage (salades, cap 2).

### Shop — polish restant
- [x] [P0-SHOP-POP-001] Branche **`rework/shopitempopup`** + polish popup achat (`ShopItemPopup` : saisie quantité, Max, confirmation overlay, solde wallet dans le Header). **Fait** (2026-05-19) — merge `main`, voir `PROJECT_LOG.md`.
- [x] [CT-SHOP-001] Flux achat de base opérationnel (catalogue + popup item + transaction).
- [~] [CT-SHOP-002] Passe UI/UX shop (lisibilité, focus, transitions) — polish visuel optionnel restant.
- [x] [CT-SHOP-003] Saisie quantité (`TMP_InputField`, clamp min/max).
- [x] [CT-SHOP-004] Bouton Max (`floor(solde/prix)` + plafonds métier + place inventaire).
- [x] [CT-SHOP-005] Confirmation avant paiement (overlay `ConfirmOverlay` sur `ShopItemPopup.prefab`).
- [x] [CT-SHOP-006] Références Inspector popup item (prefab + `ShopItemPopupView` / `CurrencyBalanceUI` wallet).

### Inventaire / wallet / runtime UI
- [~] **[CT-INV-HALO-001]** Rework inventaire halo + grille — Ph.1–3 + playtest halo OK ; **MVP arbre Commerce OK** (2026-06-16) ; polish arbre → backlog.
- [ ] [CT-INV-001] Stabiliser le wallet inventaire avec une seule source de vérité (`InventoryScreen` prefab via `UIManager`).
- [ ] [CT-INV-002] Valider qu'il n'y a plus de dépendance runtime cachée à `Inventaire.unity` (ou documenter explicitement son rôle).
- [ ] [CT-INV-003] Vérifier le flux `TryAdd` de bout en bout (id, quantités, stack, inventaire plein, refresh UI).

### Ferme — polish & UI
- [ ] **[CT-FARM-UI-001]** *(ex-P0, polish)* Prefab **`SeedSelectionUI`** : **EmptyStatePanel** + bouton **Acheter** — code prêt, prefab à câbler. §4.4 `Notes/Farm/REFACTOR_graines_plantation_inventaire.md`. **Stock** jusqu'à validation post-[P0-IDEA-001].

### Ferme — croissance et persistance
- [~] [CT-FARM-001] Corriger la cohérence stade/durée après modification de `PlantDefinition` (runtime + reload).
- [~] [CT-FARM-002] Croissance offline UTC prototype (`FarmTimeService`, quit/pause, retour scène) — playtest auteur OK 2026-06-02 ; affiner plafond / cloud plus tard.
- [ ] [CT-FARM-003] Revalider le démarrage visuel/logique au stade Graine puis transition vers feuilles.
- [~] [CT-FARM-004] Persistance JSON scénario complet : pose → quit → relance → récolte (offline inclus).

### Navigation Scene/UI
- [ ] [CT-NAV-001] Debug complet des flux `SceneNavigator.ShowScene` (transitions concurrentes, scènes orphelines, ordre d'activation).
- [~] [CT-NAV-002] Finaliser hub `HomeScene` + retour gameplay (croix/exit) et aligner la doc.
- [~] [CT-NAV-003] Poursuivre la migration Inventaire/Market/HUD global vers le flux cible.
- [ ] [CT-NAV-004] Trancher et documenter le mode de chargement final UI (persistant vs sync vs async/additive).

---

## Backlog structuré

### Inventaire — arbres talents (polish / post-MVP)

> Feature **MVP fonctionnelle** (2026-06-16) — reprise volontaire plus tard. Détail : `Notes/Ui/WORKFLOW_creation_arbre_talents.md` § *Polish / backlog*.

- [ ] **[BL-INV-TALENT-001]** **Filigrane thématique** par piste : décor discret sous l'arbre (ex. motifs Commerce, plantes, poissons…) — Image/Sprite dans `TreeMountHost` ou fond overlay, swappable par binding piste (Bezy prefab + évent. champ SO/piste).
- [ ] **[BL-INV-TALENT-002]** **Zoom + pan (scroll)** overlay — **conditionnel** : activer seulement si le layout arbre **dépasse le viewport** (arbres denses, tablettes petit format). Réutiliser ou remplacer `TreeScrollView` ; pinch-to-zoom mobile si pertinent. **Ne pas implémenter** tant que les arbres tiennent à l'écran (ex. Commerce 3 nœuds).
- [ ] **[BL-INV-TALENT-003]** **Prefabs définitifs** : `TreeMountHost` fixe (Bezy Phase 4), retrait contournements runtime Cursor, contrastes nœuds/overlays — voir `PROMPTS_Bezi_talent_tree.md`.
- [ ] **[BL-INV-TALENT-004]** Dupliquer arbres **P2–P6** (`Track_Plant`, etc.) + bindings overlay quand le design GDD sera prêt.

### Audit / nettoyage technique
- [ ] [BL-AUD-001] Audit Bezy Scene/UI (checklist détaillée dans `Notes/Ui/TODO_Bezi_audit_scene_ui_refactor.md`).
- [ ] [BL-AUD-002] Nettoyer le code mort et harmoniser commentaires XML/scripts navigation/UI.
- [ ] [BL-AUD-003] Audit doc global projet + consolidation des notes obsolètes.
- [ ] [BL-AUD-004] Passe commentaires FR ciblée sur scripts complexes (`Assets/Scripts/**`).

### Prototype gameplay
- [~] [BL-PROTO-001] Finaliser la state machine culture (transitions gameplay/récolte).
- [ ] [BL-PROTO-002] Revoir `Assets/Scripts/Core/Timer.cs` (scalabilité, `unscaledDeltaTime`, offline UTC).
- [ ] [BL-PROTO-003] Supprimer le doublon éventuel `Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity`.
- [ ] [BL-PROTO-004] Terminer le panel Options du menu principal.
- [ ] [BL-PROTO-005] Maintenir à jour la carte des flux système (`Notes/Farm/SYSTEMES_carte_mentale.md`).
- [ ] [BL-QUEST-DAILY-001] Ajouter une feature de quêtes quotidiennes (missions courtes) avec récompenses en ressources + points de compétences (design, logique runtime, reset journalier UTC, UI de suivi, persistance).

### GDD / design
- [ ] [BL-GDD-001] Esquisser le GDD MVP (concept, boucle, scope).
- [ ] [BL-GDD-002] Spécifier le temps de ferme (`lastUtc -> delta`, plafond offline).
- [ ] [BL-GDD-003] Formaliser la progression XP joueur + maturité biofiltre.
- [ ] [BL-GDD-004] [OPTIONNEL] Collecter 2-3 références UI et noter ce qui est repris/évité.
- [ ] [BL-GDD-005] Progression système aquaponique par scène (`FirstLvl+`) — spec `SPEC_progression_systeme_aquaponique_par_niveau.md` (panneau onglets, points, anti-aléas).

### Workflow / documentation
- [ ] [BL-DOC-001] Documenter les règles projet (style, conventions AI/notes, organisation dossiers).
- [ ] [BL-DOC-002] Finaliser le rappel workflow Git "Save All avant Git".
- [ ] [BL-DOC-003] Définir les conventions de localisation (`country` vs `language`, keys TMP).
- [ ] [BL-DOC-004] [OPTIONNEL] Compléter `Notes/Bezi/README_bezi.md` (version Unity, scènes, bezi.actions).

### Art / assets
- [~] [BL-ART-001] Finaliser le cycle complet de la laitue prototype.
- [~] [BL-ART-002] Terminer le nettoyage des assets temporaires.
- [ ] [BL-ART-003] Documenter le double procédé graphique (léger + 3D) et les critères d'usage.
- [ ] [BL-ART-004] Centraliser un workflow 3D validé (sources -> export -> intégration).
- [ ] [BL-ART-005] Décider la cible mobile 2D vs 3D selon playtests perf.
- [ ] [BL-ART-006] Intégrer l'illustration LoadingScreen + QA (play + build).

### Cloud / post-prototype
- [ ] [BL-CLOUD-001] [OPTIONNEL] Préparer UGS inventaire (Auth -> Cloud Save -> Economy).
- [ ] [BL-CLOUD-002] Implémenter l'architecture services cloud inventaire/market (`IInventoryService` / `IMarketService`).
- [ ] [BL-CLOUD-003] Découpler gameplay de `PlayerInventory.Instance` avant sync cloud.
- [ ] [BL-CLOUD-004] [OPTIONNEL] Définir un workflow graphique IA (polish).
- [ ] [BL-CLOUD-005] Polish UI inventaire + polish technique navigation UI.

### Nettoyage dépôt
- [ ] [BL-REPO-001] Après validation art, audit/suppression des dossiers assets vides + commit dédié de nettoyage.
