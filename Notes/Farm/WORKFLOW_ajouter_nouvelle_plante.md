# Workflow — créer une nouvelle plante (protocole complet)

**Statut** : référence active (fusion 2026-07-29)  
**Plante pilote** : laitue — `Assets/Data/Ferme/Laitue.asset` + `Assets/Prefabs/World/Plantes/LaitueObj.prefab`  
**Objectif** : checklist unique depuis zéro jusqu’à une culture **plantable**, **récoltable**, avec feedback ambiance (insecte / sparkle) et **items inventaire** cohérents.

---

## Documents liés (ne pas dupliquer)

| Sujet | Note |
|-------|------|
| Footprint / grille | `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` |
| Pipeline plantation (historique) | `Notes/Farm/TODO_plantation_pipeline.md` |
| Graines ↔ inventaire | `Notes/Farm/REFACTOR_graines_plantation_inventaire.md` |
| Règle `harvestItemId` ↔ `itemId` | `Docs/PLANTES_ET_INVENTAIRE.md` |
| Insecte Flowering | `Notes/Farm/SPEC_insecte_flowering.md` + `Notes/Ui/PROMPTS_Bezi_insecte_flowering.md` |
| Sparkle récoltable | `Notes/Ui/PROMPTS_Bezi_harvest_ready_vfx.md` (`[BZ-POLISH-019]`) |
| Carte des systèmes | `Notes/Farm/SYSTEMES_carte_mentale.md` |
| État codebase | `Notes/Codebase_etat_reference.md` |

---

## Checklist rapide (copier pour une nouvelle plante)

```
[ ] 1. ItemDefinition(s) + ItemDatabase
[ ] 2. PlantDefinition (pattern, harvestStages, sprites, footprint, insecte, durées)
[ ] 3. Prefab monde (composants racine + enfants VFX)
[ ] 4. SeedEntry (definition + prefab + seedItem)
[ ] 5. Shop (optionnel) + canal de vente (optionnel)
[ ] 6. Playtest plantation → croissance → récolte Mature → Seedling
```

---

## Vue d’ensemble du flux joueur

1. Clic case **vide** biofiltre → `SeedSelectionUI` (filtre stock inventaire).
2. Slot cliquable si stock `seedItem` > 0 **et** `CanPlace(ancre, plantDefinition)`.
3. Preview fantôme → clic gauche confirme → `TryPlantSeedAt` consomme **1×** graine.
4. Instance : `PlantGrow` en **Graine**, grille occupée, VFX plantation éventuel.
5. Croissance auto selon `stageDurations` + pattern Leafy/Fruiting.
6. Stade récoltable (`GetHarvestConfig`) → **sparkle** `HarvestReadyFx` ON.
7. Stade **Flowering** (+ `insectKind ≠ None`) → **insecte** ON.
8. Récolte via `PlantHarvestInteractor` → item `harvestItemId` → inventaire.

---

## Étape 1 — Items inventaire (`ItemDefinition`)

Une plante = en général **plusieurs items** (pas le même SO que `PlantDefinition`).

| Rôle typique | Exemple laitue | Menu Create |
|--------------|----------------|-------------|
| Graine (planter + souvent loot Seedling) | `laitue_seed` | `Game/Data/Inventaire/Item (définition)` |
| Récolte Mature (feuilles / fruit) | `laitue_mature` | idem |

**Obligatoire :**

1. Créer les `ItemDefinition` (`itemId` stable, icône, `maxStack`, nom).
2. Les **ajouter à `ItemDatabase`** (sinon récolte / shop / plantation cassés).
3. Règle d’or : **`harvestItemId` = `itemId` exact** (casse, underscores).  
   Détail : `Docs/PLANTES_ET_INVENTAIRE.md`.

---

## Étape 2 — `PlantDefinition`

**Create** : `Game/Data/Ferme/Plante (définition)`  
**Dossier recommandé** : `Assets/Data/Ferme/`  
**Duplication** : partir de `Laitue.asset` puis renommer + changer tous les IDs.

### 2.1 Identity

| Champ | Règle |
|-------|--------|
| `plantId` | Clé logique unique (ex. `tomato`) |
| `displayName` | Affichage joueur |

### 2.2 Growth Pattern

| Profil | Fin de cycle | Usage |
|--------|--------------|--------|
| **Leafy** | Growing → **Mature** (récolte) → Flowering → Seedling | Laitue, épinard… |
| **Fruiting** | Growing → Flowering → **Mature** (récolte) → Seedling | Tomate, poivron… |

Les **7 slots sprites** restent les mêmes ; seule l’interprétation change (Flowering avant/après Mature).

### 2.3 Harvest (`harvestStages[]`)

Une entrée **par stade récoltable** (pas un seul `harvestItemId` global) :

| Exemple Leafy | `stage` | `harvestItemId` |
|---------------|---------|-----------------|
| Feuilles | `Mature` | `xxx_mature` |
| Graines | `Seedling` | `xxx_seed` (souvent = item graine) |

- `harvestAmountMin` / `Max` ≥ 1  
- `maxHarvestCount` : présent ; MVP = souvent 1 (récolte détruit la plante — voir dette dans `Codebase_etat_reference.md`)

**Effet runtime** : dès qu’un stade a une config → `PlantGrow` active le **sparkle** (`HarvestReadyFxAnchor`).

### 2.4 Stage Sprites

Renseigner `spriteGraine` … `spriteSeedling` (7 slots). Preview placement utilise aujourd’hui surtout le sprite de preview (cf. `PlantPlacementPreview`) — garder au minimum Graine + Mature + Seedling lisibles.

### 2.5 Grid Placement

- `footprint` **doit** contenir `(0,0)` (warning Editor sinon).
- Pas de doublons d’offsets.
- `spriteWorldOffset` : alignement pivot sprite ↔ ancre grille.  
  Guide : `GUIDE_footprint_GetOccupiedCells.md`.

### 2.6 Insecte Flowering

| `insectKind` | Effet |
|--------------|--------|
| `None` | Pas d’insecte |
| `Bee` / `Butterfly` | Espèce fixe |
| `RandomBeeOrButterfly` | 50/50 au start Flowering (laitue) |

Overrides optionnels : `insectMoveSpeed`, `forageDurationMin/Max` (0 = défauts script).  
Spec : `SPEC_insecte_flowering.md`.  
**Prefab** : sans `InsectPath` sous la plante, le champ est ignoré (pas de crash).

### 2.7 Stage Durations

Entrées `stage` + `durationSeconds`.  
`0` = pas d’auto-advance (OK en dernier stade ; warning si stade non terminal).

Pour **Fruiting**, soigner surtout **Flowering** (avant Mature).

---

## Étape 3 — Prefab monde

**Référence** : `Assets/Prefabs/World/Plantes/LaitueObj.prefab`  
**Dossier** : `Assets/Prefabs/World/Plantes/`

### 3.1 Hiérarchie cible

```
Plante_Xxx (root)
├── SpriteRenderer
├── PlantGrow                    ← plantDefinition + refs optionnelles
├── PlantDefinitionHolder        ← même PlantDefinition
├── PlantHarvestInteractor
├── Collider2D (raycast récolte / clic)
├── PlantPlantingPunch           ← optionnel (polish pose)
├── InsectPath (souvent inactif hors Flowering)
│   ├── Node_0 … Node_N
│   └── Bee / InsectInstance (prefab partagé)
└── HarvestReadyAnchor (INACTIVE par défaut)
    └── HarvestReadyFx (instance prefab partagé)
        └── Sparkle (ParticleSystem)
```

### 3.2 Composants racine (obligatoires gameplay)

| Composant | Rôle |
|-----------|------|
| `SpriteRenderer` | Requis par `PlantGrow` |
| `PlantGrow` | Stades, timers, sync insecte + sparkle |
| `PlantDefinitionHolder` | Accès definition pour harvest / grille |
| `PlantHarvestInteractor` | Clic → popup récolte |
| `Collider2D` | Hit pour récolte / interactions |

À la pose, `BiofiltreManager` appelle `SetStage(Graine)` + `Initialise` holder / interactor — le `initialStage` du prefab ne gouverne **pas** les cultures plantées en jeu.

### 3.3 Enfants VFX / ambiance

| Enfant | Quand | Prefab / art partagé | Script |
|--------|-------|----------------------|--------|
| `InsectPath` | Stade `Flowering` + `insectKind ≠ None` | `Assets/Prefabs/World/Insects/Bee.prefab` (+ Butterfly) | `InsectPathAnchor` + follower |
| `HarvestReadyAnchor` | Tout stade avec `GetHarvestConfig ≠ null` | `Assets/Prefabs/World/VFX/HarvestReadyFx.prefab` | `HarvestReadyFxAnchor` |

**Recyclage :**

- Art insecte / sparkle = **1 fois pour tout le jeu**.
- Par plante = **positions** (nodes InsectPath, hauteur `HarvestReadyAnchor` ~ au-dessus du feuillage).

**Bezy** : prompts phasés dans `Notes/Ui/PROMPTS_Bezi_insecte_flowering.md` et `PROMPTS_Bezi_harvest_ready_vfx.md`.  
**Cursor** : hooks déjà dans `PlantGrow` (`SyncInsectPathForStage`, `SyncHarvestReadyFxForStage`).

Sur `PlantGrow` Inspector (recommandé) :

- `insectPath` → enfant `InsectPath`
- `harvestReadyFx` → enfant `HarvestReadyAnchor`  
  (sinon fallback `GetComponentInChildren` incl. inactifs)

### 3.4 Debug Editor

Sur `PlantGrow` (context menu) :

- Force Flowering (insecte)
- Force Mature (sparkle récolte)
- Force Seedling (sparkle graines)

---

## Étape 4 — Catalogue plantation (`SeedEntry`)

Sur `SeedSelectionUI` → `availableSeeds` :

| Champ | Contenu |
|-------|---------|
| `plantDefinition` | Asset étape 2 |
| `plantPrefab` | Prefab étape 3 |
| `seedItem` | `ItemDefinition` graine (étape 1) — **obligatoire** |

Sans les trois, le slot est ignoré / non plantable.

Vérifier aussi : `Seed Slot UI`, `Slots Container`, `Placement Preview`, lien `BiofiltreManager` → panneau graines.

---

## Étape 5 — Shop & vente (optionnel mais fréquent)

| Système | À faire |
|---------|---------|
| **Shop** | `ShopItemDefinition` qui vend `seedItem` (+ listing écran shop) |
| **Canal vente** | Si le loot Mature/Seedling doit être vendable : entrée `SaleChannel` / listing avec le bon `itemId` |

Pack départ / empty state graines : déjà géré côté inventaire + UI (voir note refactor graines).

---

## Étape 6 — Playtest (DoD)

1. Avoir ≥1 graine en inventaire (shop ou cheat).
2. Planter → cases footprint occupées, preview vert/rouge OK.
3. Avancer jusqu’à **Mature** → sparkle ON → récolte → item Mature en sac, plante retirée (MVP).
4. Ou forcer **Flowering** → insecte ON, sparkle OFF (sauf si harvest configuré sur Flowering).
5. Forcer **Seedling** → sparkle ON → récolte graines = `seedItem`.
6. Sans stock → empty state / CTA shop.
7. Footprint bord de grille → slot grisé / preview rouge.

---

## Fichiers code (rappel)

| Fichier | Rôle |
|---------|------|
| `Assets/Scripts/Data/PlantDefinition.cs` | SO plante |
| `Assets/Scripts/Farm/PlantGrow.cs` | Stades + sync VFX |
| `Assets/Scripts/Farm/HarvestReadyFxAnchor.cs` | Sparkle récoltable |
| `Assets/Scripts/Farm/InsectPathAnchor.cs` / `InsectPathFollower.cs` | Insecte Flowering |
| `Assets/Scripts/Farm/BiofiltreManager.cs` | Pose + occupation |
| `Assets/Scripts/Farm/PlantHarvestInteractor.cs` | Récolte |
| `Assets/Scripts/UI/SeedSelectionUI.cs` | `SeedEntry` |
| `Assets/Scripts/Inventory/ItemDefinition.cs` / `ItemDatabase.cs` | Items |

---

## Protocole « prochaine plante » (ordre recommandé)

1. Dupliquer **items** laitue → nouveaux `itemId`.
2. Dupliquer **PlantDefinition** → pattern + harvestStages + sprites + footprint + insecte.
3. Dupliquer **LaitueObj** → renommer, rebrancher definition, retuner nodes InsectPath + hauteur sparkle.
4. Ajouter **SeedEntry** + shop si besoin.
5. Playtest checklist §6.
6. Journaliser dans `PROJECT_LOG.md` / cocher todo si ticket ouvert.

Ce document est la **source unique** pour l’ajout de plantes ; les SPEC VFX détaillent le polish Bezy, pas le flux métier complet.
