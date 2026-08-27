# Spéc — progression par système aquaponique (par niveau / scène)

**Statut :** note d’implémentation **future** (design + pistes techniques). Pas de code métier ni prefab UI dans ce document.  
**Date :** 2026-06-04  
**Contexte produit :** chaque **niveau de jeu** (scène de ferme) héberge un ou plusieurs **systèmes aquaponiques** ; aujourd’hui une seule scène gameplay ferme : **`FirstLvl`** (`SceneId.FirstLvl`).

---

## 1. Intention produit

Le joueur fait évoluer **son installation sur place** (biofiltre, bassin poisson, équipements), pas seulement son avatar global.

| Axe | Cible |
|-----|--------|
| **Granularité** | Progression **par instance de système** liée à un niveau / scène (ex. le biofiltre de `FirstLvl`). |
| **Accès UI** | Un **bouton** dans la scène gameplay ouvre un **panneau** dédié (pas l’inventaire halo). |
| **Structure** | Panneau à **onglets** : au minimum **Biofiltre**, **Poisson**, **Techno** (extensible). |
| **Dépenses** | Points gagnés quand le **système monte de niveau** + éventuellement **technologies** (déblocages / prérequis). |
| **Payoff** | Meilleure **production**, **rapidité**, **quantité** ; **réduction ou annulation** de certains **aléas** (limaces, faible germination, casse de récolte, etc.). |

> **À ne pas confondre** avec la progression **joueur** (halo inventaire, arbres globaux) : voir `Notes/Ui/SPEC_rework_inventaire_halo_progression.md` et `ProgressionTrackId`. Ici = **métier ferme local** par scène.

**Prestige / génération** (reconstruire le biofiltre : nettoyage + upgrade, grille vide) : autre couche, **pas** un respec des nœuds — `Notes/GDD/SPEC_prestige_generation_systemes.md`.

---

## 2. Glossaire

| Terme | Signification projet |
|--------|----------------------|
| **Système aquaponique** | Ensemble couplé plante ↔ eau ↔ poisson ↔ équipements sur **une** ferme / niveau. En v1 : souvent **un** biofiltre + logique associée dans la scène. |
| **Instance** | Objet scène persistant (ex. `BiofiltreManager` sur le prefab zone) portant l’état de progression **de ce** système. |
| **Niveau de jeu** | Scène Unity de gameplay ferme (`FirstLvl` aujourd’hui ; futurs niveaux = nouvelles scènes + nouvelles instances). |
| **Niveau système** | XP / paliers **de l’installation**, distinct du niveau joueur. |
| **Point système** | Monnaie dépensée dans les arbres / grilles de l’onglet ouvert. |
| **Technologie** | Nœud ou item de déblocage (prérequis, upgrade permanent sur l’instance). |
| **Aléa** | Événement ou modificateur négatif sur cycle culture / récolte (à concevoir). |

---

## 3. Relation avec les specs existantes

| Document | Lien |
|----------|------|
| `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md` | **Étoiles biofiltre** (★1 = XP 240 + 50 salades + 100 germinations + 50 graines) + gating fruits. La présente spec = nœuds / points / onglets. **Même `systemInstanceId`**. Ne pas dupliquer les bonus d’étoile dans les nœuds. |
| `Notes/Ui/SPEC_rework_inventaire_halo_progression.md` | Progression **globale** joueur (commerce, culture, etc.). Synergies possibles (ex. talent inventaire « +5 % vitesse croissance biofiltre ») mais **UI séparée**. |
| `Notes/Farm/SYSTEMES_carte_mentale.md` | Ancrage code : `BiofiltreManager`, `PlantGrow`, `FarmPersistenceCoordinator`, UI `SeedSelectionUI` / `HarvestPanelUI`. |
| `Notes/Ui/popup_generique.md` | Si le panneau est modal : nouveau `PopupId` + binding pour `ScreenId.FirstLvl` (ou overlay scène dédié — cf. §6). |

**Règle produit proposée :**

- **Unlock joueur** : « j’ai la graine / la recette / le niveau perso ».
- **Maturité / niveau système** : « cette ferme a assez vécu **ou** le joueur a investi ici ».
- Pour une plante gated : vérifier **les deux** si le design l’exige (`CanPlace` + données `PlantDefinition`).

---

## 4. Parcours joueur (UX cible)

```
[Scène FirstLvl — gameplay]
        │
        ▼
[Bouton « Système » / « Améliorer » / icône engrenage — emplacement TBD]
        │
        ▼
[Panneau progression système — reste en scène, pas de changement SceneNavigator]
        │
        ├── Onglet Biofiltre   (étoiles ★ + jauges, nœuds, germination, limaces, rendement…)
        ├── Onglet Poisson     (alimentation, densité, qualité eau, bonus indirect plantes…)
        └── Onglet Techno      (pompes, capteurs, automatisation, anti-aléa transverses…)
        │
        ▼
[Clic nœud / upgrade] → dépense points (+ coût techno si requis) → apply modificateurs
```

### 4.1 Emplacement du bouton (à trancher)

Pistes compatibles avec l’existant :

- **HUD ferme** : bouton proche des actions déjà gérées par `NavigationHUD` / canvas scène `FirstLvl` (à ne pas confondre avec retour Home).
- **Proximité biofiltre** : bouton world-space ou bulle UI sur la zone `BiofiltreManager` (plus immersif, moins visible au premier abord).
- **Raccourci** : même panneau ouvrable depuis un menu pause ferme (phase 2).

**Contrainte projet :** pas de `SceneManager.LoadScene` depuis ce flux — panneau **overlay** ou popup host sur la scène courante.

### 4.2 Panneau à onglets

| Onglet | Rôle design (exemples, non figés) |
|--------|-----------------------------------|
| **Biofiltre** | Vitesse stades `PlantGrow`, bonus `harvestAmount`, résistance **germination**, anti-**limaces**, footprint / slots (tardif). |
| **Poisson** | Modificateurs indirects (nutriments eau, stabilité), déblocage espèces ou ratios (futur). |
| **Techno** | Transversal : alertes, réduction aléas globaux, qualité de vie (moins de clics), prérequis pour nœuds des autres onglets. |

**Extensibilité :** onglets supplémentaires possibles (ex. **Serre**, **Compost**, **Énergie**) quand un niveau ajoute des sous-systèmes.

Chaque onglet = **arbre ou grille de nœuds** (même grammaire visuelle que l’overlay talents inventaire, mais données et effets **locaux à l’instance**).

---

## 5. Boucle de progression système

### 5.1 Sources d’XP système (à calibrer)

| Source | Description |
|--------|-------------|
| Cycles complets | Plantation → récolte validée sur ce biofiltre (aligné maturité § `SPEC_progression_xp_joueur_et_biofiltre`). |
| Récoltes | XP proportionnel au type / quantité (`PlantDefinition`). |
| Quêtes / objectifs niveau | Bonus one-shot par scène (futur). |
| Temps réel actif | Optionnel, cumul hors pause — à trancher avec spec temps ferme. |

À la montée de **niveau système** : créditer **N points** (+ éventuellement **1 point techno** ou déblocage d’onglet).

### 5.2 Dépenses

- **Points système** : achat de nœuds dans un onglet (coût variable, prérequis en chaîne ou en étoile).
- **Technologies** : items ou flags persistants (ex. « Capteur pH » débloque branche Poisson tier 2).
- **Ressources inventaire** (option phase 3) : certains nœuds consomment aussi des matériaux — à décider pour ne pas doubler le shop.

### 5.3 Effets gameplay (modificateurs)

Centraliser l’application dans un service du type **`IAquaponicSystemModifiers`** (nom provisoire), lu par :

- `BiofiltreManager` / `CanPlace` / placement ;
- `PlantGrow` (durées de stades, taux d’échec germination) ;
- `PlantHarvestInteractor` / quantités min-max ;
- futurs systèmes **aléas** (limaces, maladies, météo serre).

| Catégorie | Exemples d’effets |
|-----------|-------------------|
| **Production** | `+X %` quantité récolte, chance double récolte. |
| **Rapidité** | `−Y %` temps par stade ou stade « Germination » uniquement. |
| **Quantité** | Plafond `harvestAmountMax`, multiplicateur par culture. |
| **Anti-aléa** | Réduction proba limaces ; bonus taux germination ; protection % récolte « cassée ». |

Les valeurs doivent être **data-driven** (ScriptableObject ou table par nœud), pas codées en dur dans la vue UI.

### 5.4 Catalogue d’aléas (placeholder GDD)

À détailler lors de l’import notes tablette ; liste de travail :

| Aléa | Impact typique | Onglet le plus logique |
|------|----------------|------------------------|
| Limaces | Perte partielle / totale ; raids **nuit** + **pluie** | Slot secondaire anti-slug (`SPEC_biofiltre_slots_shields.md`) |
| Souris / oiseaux / fourmis / moisissure | TBD | Slots secondaires 2–5 |
| Faible germination | Stade bloqué ou plante perdue | Biofiltre (nœuds) |
| Eau instable | Malus croissance toutes plantes | Poisson + Techno |
| Maladie poisson | Malus indirect long terme | Poisson |
| Panne pompe | Pause croissance jusqu’à réparation | Techno |

Chaque aléa = **id stable** (`HazardId`) + résistance max stackable depuis les nœuds.

---

## 6. Architecture technique proposée (future)

### 6.1 Identité instance

```text
systemInstanceId  (string, stable par prefab/scène)
  ex. "firstlvl.biofiltre.main"
sceneId             (SceneId.FirstLvl)
```

- Un contrôleur scène léger **`AquaponicSystemSceneBridge`** sur `FirstLvl` (ou extension de `FirstLvlController`) enregistre l’instance active auprès du service.
- Réutiliser le pattern **`FarmPersistenceCoordinator`** / save JSON existante : bloc **`aquaponicSystems[]`** dans la save joueur ou save scène selon modèle retenu.

### 6.2 Données

| Asset / type | Rôle |
|--------------|------|
| `AquaponicSystemDefinition` | Métadonnées par niveau (id, onglets actifs, courbe XP). |
| `SystemProgressionNodeDefinition` | Id, onglet, coûts, prérequis, liste `ModifierSpec`. |
| `AquaponicSystemProgressState` | Runtime + save : niveau, XP, points non dépensés, nœuds achetés, techs. |

### 6.3 UI (Bezy + scripts)

| Livrable | Agent |
|----------|--------|
| Prefab panneau + onglets + arbres | **Bezy** (phases shell → composants → wiring) |
| `AquaponicSystemPanelController`, `SystemTabController`, ids | **Cursor** |
| Bouton d’ouverture dans `FirstLvl` | **Bezy** + référence depuis bridge scène |

**Pipeline popup :** si modal global :

1. Constante `PopupId.AquaponicSystemProgression` (nom à affiner).
2. `ScreenPopupBinding` : `screenId = FirstLvl`, prefab panneau.
3. Ouverture via `ScreenPopupHost` depuis le bouton — **pas** d’instanciation ad hoc long terme.

### 6.4 Séparation des responsabilités

- **Vue UI** : affichage onglets, tooltips, états verrouillé / acheté.
- **`AquaponicSystemProgressionService`** : gain XP, achat nœud, validation prérequis, événements `OnSystemLevelUp`.
- **`AquaponicSystemModifiersProvider`** : agrège les modificateurs actifs pour le gameplay.
- **Pas** de logique métier lourde dans les `MonoBehaviour` UI seuls.

---

## 7. Multi-niveaux (au-delà de FirstLvl)

| Étape | Comportement |
|-------|----------------|
| v0 | Une instance, une scène `FirstLvl`. |
| v1 | Chaque nouvelle scène ferme déclare son `systemInstanceId` + `AquaponicSystemDefinition` propre. |
| v2 | Hub carte : aperçu niveau système avant entrée scène (optionnel). |

La progression **ne se partage pas** entre fermes sauf décision produit explicite (ex. « recherche globale » du joueur qui donne un bonus faible partout).

---

## 8. Phases d’implémentation suggérées

### Phase 0 — Design (actuel)

- [x] Noter la vision (ce document).
- [ ] Valider emplacement bouton + maquette onglets.
- [ ] Lister 5–10 nœuds v1 par onglet (mock équilibrage).
- [ ] Croiser aléas avec notes tablette `Notes/GDD/INBOX_notes_tablette_recherches.md`.

### Phase 1 — Coque UI (FirstLvl)

- [ ] Bouton + panneau 3 onglets (placeholders, pas de save).
- [ ] Navigation retour / fermeture sans quitter la scène.

### Phase 2 — Données & save

- [ ] Modèle `AquaponicSystemProgressState` + persistance.
- [ ] Gain XP sur récolte / cycle (hook `BiofiltreManager` ou harvest).

### Phase 3 — Gameplay

- [ ] Service modificateurs branché sur `PlantGrow` / récolte.
- [ ] Premier aléa + premier nœud anti-aléa (vertical slice).

### Phase 4 — Équilibrage & extension

- [ ] Courbes XP, coûts, synergies joueur global vs système local.
- [ ] Second niveau / scène pilote.

---

## 9. Questions ouvertes

- [ ] Le **niveau système** et la **maturité passive** (10 cycles salade) fusionnent-ils en un seul compteur ou restent-ils séparés ?
- [ ] Les **points** sont-ils **un pool global** par panneau ou **par onglet** ?
- [ ] Respec / reset des **nœuds** (oui/non, coût) ? **Distinct** du prestige installation (`SPEC_prestige_generation_systemes.md`).
- [ ] Où placer le bouton **Reconstruire** (même panneau onglet Biofiltre vs world) ?
- [ ] Le panneau est-il **popup** (`ScreenPopupHost`) ou **panel** enfant du canvas `FirstLvl` ?
- [ ] Faut-il afficher le **niveau système** dans le HUD en permanence ?
- [ ] Lien avec **économie** : les upgrades affectent-elles aussi les prix shop/market de ce biome ?

---

## 10. Références code (point d’ancrage)

| Fichier | Usage futur |
|---------|-------------|
| `Assets/Scripts/Farm/BiofiltreManager.cs` | Instance gameplay, hooks cycle / placement |
| `Assets/Scripts/Farm/FarmPersistenceCoordinator.cs` | Pattern register / save |
| `Assets/Scripts/Farm/PlantGrow.cs` | Application modificateurs temps / stades |
| `Assets/Scripts/UI/FirstLvlController.cs` | Extension possible bridge scène |
| `Assets/Scripts/Systems/SceneId.cs` | `FirstLvl` |
| `Assets/Scripts/UI/Inventory/Progression/*` | Référence **UI** arbre (halo), pas réutiliser les mêmes données |

---

## 11. Backlog projet

- Entrée backlog : **`[BL-GDD-005]`** — Progression système aquaponique par scène (panneau 3 onglets, points, anti-aléas).
- Lié à **`[BL-GDD-003]`** (XP joueur + maturité biofiltre) : implémenter ou fusionner les modèles de save en même temps si possible.
- Prestige / génération : **`[BL-GDD-006]`** — `SPEC_prestige_generation_systemes.md` (même `systemInstanceId`).
