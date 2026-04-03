# Todo projet — hub global

Liens vers les TODOs thématiques : `Notes/Ui/Todo_ui.md`, `Notes/Farm/SPEC_plant_footprint_prompt.md`, `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, etc.

---

## Prochaine session (priorité immédiate)

- [ ] **Art — sprites** : retirer le **fond blanc** sur les sprites concernés et les exporter avec **transparence** (canal alpha) ; dans Unity, vérifier l’import texture (alpha / compression) pour éviter les halos blancs.
- [ ] **Gameplay — footprint (données)** : le `PlantDefinition` contient déjà `footprint` + `GetOccupiedCells` ; finaliser la **compréhension** (dont **dédoublonnage** des offsets — voir session assistant) et les assets plantes avec les bons footprints (ex. salade 2×2). Réfs : `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md`, `Notes/Farm/SPEC_plant_footprint_prompt.md`.
- [ ] **Après validation de la compréhension du code** : implémenter au minimum un **`GridSystem`** (état par `Vector2Int` : `IsFree` / `Occupy` / `Release`) **et** un **`BuildManager`** (ou service de placement) qui utilise `plant.GetOccupiedCells(origin)` pour `CanPlace` puis spawn — **idéalement les deux**, dans l’ordre **grille d’abord**, **manager ensuite** comme consommateur. Faire implémenter par **Bezi** et/ou **Cursor** si besoin.

---

## Prototype (phase actuelle)

- [ ] **Inventaire récolte (spécification)** : définir précisément le système (slots, taille max des piles, règle ajout partiel vs tout-ou-rien, codes retour de `TryAdd`).
- [~] **State machine culture** : stades visuels posés (`Seedling`, `BabyLeaf`, `Growing`, `Mature`, `Bolting`) ; finaliser transitions gameplay + règles de récolte.
- [ ] **Flux récolte minimal jouable** : clic sur objet mature -> tentative d’ajout inventaire -> succès = récolte/transition d’état, échec = popup inventaire plein et objet reste mature.
- [ ] **Timer gameplay** : revoir `Assets/Scripts/Core/Timer.cs` (scalabilité, `unscaledDeltaTime`, persistance/reprise offline via timestamp UTC).
- [ ] **Scènes Unity** : supprimer le doublon éventuel `Assets/SampleScene.unity` vs `Assets/Scenes/SampleScene.unity` et garder une seule scène de démarrage.
- [ ] **Menu principal** : terminer le panel Options (langue, audio, etc.) en s’alignant sur `Notes/Ui/Todo_ui.md`.

---

## GDD / Design

- [ ] **Esquisser le GDD simple** : concept, boucle principale, scope MVP.
- [ ] **Spec temps de ferme** : durée d’un jour ferme, vitesse de croissance, plafond offline, formule de reprise (`lastUtc -> delta`).
- [ ] **[OPTIONNEL] Références UI** : collecter 2–3 jeux de référence (captures) et noter ce qui est repris/évité.

---

## Workflow / Organisation

- [ ] **Règles projet** : documenter style de code, conventions AI/notes, organisation des dossiers (fichier dédié à confirmer).
- [ ] **Workflow Git** : ajouter/valider le rappel « Save All avant Git » dans `WORKFLOW_PROTOCOL.md`.
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
