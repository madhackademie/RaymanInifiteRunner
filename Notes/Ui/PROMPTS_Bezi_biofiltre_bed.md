# Prompts Bezy — skin bac biofiltre (grille fixe)

**Prefab cible :** `Assets/Prefabs/World/Biofiltre.prefab`  
**Script (ne pas modifier) :** `Assets/Scripts/Farm/BiofiltreGridVisualizer.cs`  
**Skins (ne pas régénérer l’art) :**
- **À brancher (IBC) :** `Assets/Data/Ferme/BiofiltreBed_Ibc3Quart.asset`
- **Swap possible (bois) :** `Assets/Data/Ferme/BiofiltreBed_Bois3Quart.asset`
- **Ne pas** assigner un sprite PNG brut sur le visualizer

**Règle métier :** `GridManager` (colonnes / lignes / cell size / origin) **ne change pas**. Les deux skins partagent le même `innerRect` (zone d’argile mesurée) : `x=0.129 y=0.376 w=0.737 h=0.485`. Swap bois ↔ IBC sans bouger l’aire de culture.

**Règles Bezy :** ne pas rescanner tout le projet ; pas de nouveaux scripts C# ; pas de Canvas UI ; pas de nouvel enfant hiérarchie (`BedSprite` est créé au runtime). World layer default, **pas** layer 5.  
**Ne pas** demander Simulate / Play Mode. Fin : `Save. List what changed. STOP.`

**Playtest auteur après Bezy :** FirstLvl — overlays de cellules sur l’argile. Si le cadre mange des cases : tuner `innerRect` **sur le skin SO**, jamais la grille.

---

## Historique

- **Cursor 2026-08-27 :** sprites + C# — **OK**
- **Cursor 2026-08-27 :** wiring prefab retiré (propriété Bezy)
- **Cursor 2026-08-27 :** IBC recalé caméra bois + `BiofiltreBedSkin` (grille invariante)
- **Phase 1 Bezy :** wiring Inspector — à faire

---

## Phase 1 — Wire Bed Skin (copier-coller Bezy)

```
[BZ-FARM-BED-001] Phase 1 ONLY — wire Biofiltre bed SKIN. Do not start any other phase.

Do NOT rescan whole project. Do NOT create scripts. Do NOT regenerate art. Do NOT create UI Canvas. Do NOT add child GameObjects. Do NOT touch PlantingDirtBurst, LaitueObj, or UI prefabs.

OPEN prefab: Assets/Prefabs/World/Biofiltre.prefab

On the ROOT component BiofiltreGridVisualizer (script already there):
- Bed Skin = Assets/Data/Ferme/BiofiltreBed_Ibc3Quart.asset
- Bed Sorting Order = -1

Do NOT change GridManager (columns, rows, cell size, origin).
Do NOT assign a raw PNG on Bed Skin.
Do NOT change BiofiltreManager, PlantPlacementPreview, gridContainer, or plantsContainer.

Save prefab. List Bed Skin asset name + Bed Sorting Order. STOP.
```
