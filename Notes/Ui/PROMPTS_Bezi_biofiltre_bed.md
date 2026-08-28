# Prompts Bezy — skin bac biofiltre (grille fixe)

**Prefab cible :** `Assets/Prefabs/World/Biofiltre.prefab`  
**Script (ne pas modifier) :** `Assets/Scripts/Farm/BiofiltreGridVisualizer.cs`  
**Skins (sprite only) :**
- IBC : `Assets/Data/Ferme/BiofiltreBed_Ibc3Quart.asset`
- Bois : `Assets/Data/Ferme/BiofiltreBed_Bois3Quart.asset`

**Règle :** la grille ne grandit pas en jeu. Un skin = un sprite. Échelle / offset sur le visualizer (`bedScale` 1 = largeur sprite = largeur grille, `bedOffset`).

**Règles Bezy :** ne pas rescanner tout le projet ; pas de scripts C# ; pas de Canvas ; pas d’enfant `BedSprite` (créé au runtime).  
Fin : `Save. List what changed. STOP.`

---

## Phase 1 — Wire Bed Skin

```
[BZ-FARM-BED-001] Phase 1 ONLY — wire Biofiltre bed SKIN.

Do NOT rescan whole project. Do NOT create scripts. Do NOT regenerate art. Do NOT add child GameObjects.

OPEN prefab: Assets/Prefabs/World/Biofiltre.prefab

On ROOT BiofiltreGridVisualizer:
- Bed Skin = Assets/Data/Ferme/BiofiltreBed_Ibc3Quart.asset
- Bed Scale = 1
- Bed Offset = 0, 0
- Bed Sorting Order = -1

Do NOT change GridManager.

Save prefab. List field values. STOP.
```
