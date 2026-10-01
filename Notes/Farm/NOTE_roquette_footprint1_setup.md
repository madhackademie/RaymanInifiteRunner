# Roquette — footprint 1×1 (prep mobile)

**Statut :** **playtest OK auteur 2026-10-01** (shop, pose 1 case, croissance). Suite = mode touch mobile.

**IDs :** `FarmPlantIds.Roquette` · `FarmItemIds.RoquetteSeed` / `RoquetteMature`

## Assets créés

| Fichier | Rôle |
|---------|------|
| `Assets/Data/Ferme/Roquette.asset` | `PlantDefinition` — footprint `(0,0)` seul |
| `Assets/Data/Inventaire/RoquetteSeedling.asset` | Item `roquette_seed` |
| `Assets/Data/Inventaire/RoquetteMature.asset` | Item `roquette_mature` |
| `Assets/Prefabs/World/Plantes/RoquetteObj.prefab` | Clone `LaitueObj` → définition Roquette |
| `Assets/Data/Shop/ShopItem_RoquetteSeedling.asset` | Achat shop (à lier au catalogue shop si besoin) |

**Sprites :** `AtlasRoquette.png` (7 slices) → `Roquette.asset` câblé. Conformité : `Notes/Art/NOTE_atlas_plantes_conformite.md`.

## Câblage déjà fait

- `ItemDatabase` : graines + récolte roquette
- `SeedSelectionUI.prefab` : 2ᵉ entrée `availableSeeds`

## Implémentation pas à pas (Unity)

### Étape A — Atlas (déjà fait côté repo)

1. Ouvrir Unity → laisser reimporter `AtlasRoquette.png`.
2. Inspector texture : **PPU 100** (laitue reste 33). Ajuster **`stageDisplayScales`** sur `Roquette.asset` si graine ou Mature trop petit/grand.
3. Sprite Editor : **7** sprites nommés `Roquette_01_Graine` … `Roquette_07_Seedling`.  
   Si slices incorrects : menu projet → relancer `py -3 scripts/build_roquette_atlas_slices.py` puis Reimport.

### Étape B — PlantDefinition

1. Ouvrir `Assets/Data/Ferme/Roquette.asset`.
2. Vérifier les 7 champs **Stage Sprites** (pas de Missing).
3. Footprint : une seule cellule `(0,0)`.

### Étape C — Playtest plantation

1. Shop → graines roquette (catalogue déjà lié).
2. Biofiltre → popup graines → **Roquette** → ghost 1 case → planter.
3. Simulate : croissance 7 stades, récolte Mature puis cycle Seedling.

### Étape D — Ajustement visuel (si pied flotte)

Sur `Roquette.asset` uniquement : `isoSpriteViewOffset` (Y− = vers le bas écran) par petits pas (0.05–0.1 monde).

## Test en jeu

1. Shop : graines roquette.
2. Ou créditer `roquette_seed` via inventaire / debug.
3. Choisir roquette dans la popup graines → preview 1 cellule → planter (tap mobile au relâchement).

## Suite mobile (hors data)

Chantier gameplay : ne plus traiter le touch comme la souris (mode plantation dédié). La roquette sert de **plante de référence 1×1** pour valider clic cellule, preview et hit footprint sans losange 2×2 laitue.
