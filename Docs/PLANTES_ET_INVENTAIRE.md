# Plantes, récolte et inventaire — référence projet

Document de référence pour configurer une nouvelle culture et la lier à l’inventaire.  
**Emplacement :** `Docs/PLANTES_ET_INVENTAIRE.md` (à citer ou ouvrir lors des questions sur les plantes / items / récolte).

---

## 1. Rôles des ScriptableObjects

| Asset | Rôle |
|--------|------|
| **`PlantDefinition`** | Profil d’une **plante** : stades, durées, sprites, empreinte grille, tableau **`harvestStages`** (quoi récolter à quel stade). |
| **`ItemDefinition`** | Profil d’un **objet d’inventaire** : `itemId` (clé technique), nom affiché, icône, stack max. |
| **`ItemDatabase`** | Liste de tous les `ItemDefinition` connus en jeu ; la récolte résout l’item via **`GetById(itemId)`**. |

Une plante en scène = prefab + `PlantGrow` + `PlantDefinitionHolder` (définition assignée au placement).  
L’inventaire ne voit que des **items** (`ItemDefinition`), jamais directement une `PlantDefinition`.

---

## 2. Règle critique : `harvestItemId` ↔ `itemId`

Dans **`PlantDefinition` → `harvestStages`** (structure **`HarvestStageConfig`**), le champ **`harvestItemId`** doit être **exactement** la même chaîne que le champ **`itemId`** du `ItemDefinition` concerné.

- Ce n’est **pas** le nom du fichier `.asset` (ex. `LaitueMature`).
- Ce n’est **pas** le **`displayName`** (ex. « Laitue Mature »).
- C’est la valeur **`ItemId`** / `itemId` dans l’inspecteur du `ItemDefinition`.

**Exemple (laitue dans ce projet) :**

- `ItemDefinition` asset `LaitueMature` → `itemId` = `laitue_mature`
- Entrée `harvestStages` au stade **Mature** → `harvestItemId` = `laitue_mature`

Respecter la casse et les underscores : `laitue_mature` ≠ `LaitueMature`.

---

## 3. ItemDatabase obligatoire

Chaque `ItemDefinition` référencé par un `harvestItemId` doit être **ajouté à la liste** du **`ItemDatabase`** assigné au jeu (ex. sur `BiofiltreManager`).

Sinon, à l’exécution : `GetById` retourne `null` et la récolte ne peut pas résoudre l’item.

---

## 4. Où ranger les assets (convention projet)

| Dossier | Contenu typique |
|---------|------------------|
| `Assets/Data/Ferme/` | `PlantDefinition`, éventuellement configs grille liées à la ferme. |
| `Assets/Data/Inventaire/` | `ItemDefinition`, éventuellement copies locales ; **`ItemDatabase`** peut rester où tu préfères (souvent Inventaire ou racine `Data`). |

Les **scripts** restent sous `Assets/Scripts/`.

---

## 5. Menus « Create » Unity (raccourci)

- **Item :** `Create > Game/Data/Inventaire/Item (définition)`  
- **Base d’items :** `Create > Game/Data/Inventaire/Base d'items (ItemDatabase)`  
- **Plante :** `Create > Game/Data/Ferme/Plante (définition)`  
- **Grille :** `Create > Game/Data/Ferme/Grille (GridConfig)`

---

## 6. Checklist — nouvelle plante + récolte

1. Créer un **`ItemDefinition`** (ou réutiliser un item existant) ; choisir un **`itemId`** stable (ex. `tomate_mure`).
2. Ajouter cet item dans **`ItemDatabase`**.
3. Créer ou dupliquer une **`PlantDefinition`** ; remplir stades, sprites, footprint, etc.
4. Dans **`harvestStages`**, pour chaque stade récoltable :
   - **`stage`** = stade `PlantGrow` concerné (ex. **Mature**, **Seedling** pour graines…).
   - **`harvestItemId`** = **exactement** le `itemId` du `ItemDefinition`.
   - **`harvestAmountMin` / `harvestAmountMax`** = quantité aléatoire incluse entre min et max.
5. Vérifier que le prefab plante a **`PlantHarvestInteractor`**, **`PlantDefinitionHolder`**, **`PlantGrow`**, **`Collider2D`**, et que **`BiofiltreManager`** référence le bon **`ItemDatabase`** et **`PlayerInventory`**.

---

## 7. Flux runtime (rappel)

1. Clic cellule occupée → `BiofiltreManager` ouvre **`HarvestPanelUI`** avec la plante ciblée.  
2. Confirmation récolte → **`PlantHarvestInteractor`** lit la config du stade courant, résout l’item avec **`itemDatabase.GetById(harvestItemId)`**, puis **`playerInventory.TryAdd(item, quantité)`**.

---

## 8. Fichiers code utiles

- `Assets/Scripts/Data/PlantDefinition.cs` — `HarvestStageConfig`, `GetHarvestConfig`
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs` — récolte, injection inventaire / DB
- `Assets/Scripts/Farm/BiofiltreManager.cs` — placement, `InjectInventory`, panel
- `Assets/Scripts/Inventory/ItemDefinition.cs`, `ItemDatabase.cs`, `PlayerInventory.cs`

---

*Dernière mise à jour : alignée sur la convention `itemId` / `harvestItemId` et les dossiers `Assets/Data/Ferme` & `Assets/Data/Inventaire`.*

**Journal projet** : `PROJECT_LOG.md` — entrée *2026-04-12 — fin de session (données récolte + organisation assets + doc)*.
