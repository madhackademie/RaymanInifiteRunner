# [BZ-FARM-SEED-CLOSE-BEFORE-PLANT-001] Fermer le choix de graines avant le popup plante

**Demande auteur 2026-10-01.** Un seul fichier. Pas de prefab, pas de scène, pas de nouveau popup.

| Fichier | Rôle |
|---------|------|
| `Assets/Scripts/Farm/BiofiltreManager.cs` | Clic case : graines si libre, popup plante si occupée |

**Déjà en place (ne pas réécrire) :**
- Case libre → `TryOpenFarmSeedSelection` (`PopupId.FarmSeedSelection`). Comportement OK.
- Case occupée → `TryOpenPlantPopup` → `TryOpenFarmPlantHarvestPopup` (`PopupId.FarmPlantHarvest`).
- `HideFarmSeedSelectionPopup()` existe déjà (ferme l’UI + le host).

**Bug :** `TryOpenPlantPopup` n’appelle pas `HideFarmSeedSelectionPopup()`. Un clic sur une plante derrière le choix de graines ouvre le popup plante pendant que les graines restent ouvertes.

**Hors scope :** prefabs, `PopupId`, `ScreenPopupHost`, `UIManager`, `SeedSelectionUI`, `HarvestPanelUI`, caméra, plantation, nouveau binding.

**Succès :** `Save. List what changed. STOP.` Pas de Simulate / Play Mode.

---

## Phase unique — C#

```
[BZ-FARM-SEED-CLOSE-BEFORE-PLANT-001] C# ONLY. STOP after this.

READ ONLY:
@Assets/Scripts/Farm/BiofiltreManager.cs
@Notes/Bezi/RULES_bezy_code.md

DO NOT rescan the project. DO NOT edit prefabs, scenes, PopupId, ScreenPopupHost, UIManager, or any other script.

In TryOpenPlantPopup, call the existing HideFarmSeedSelectionPopup() immediately BEFORE TryOpenFarmPlantHarvestPopup. Reuse that method. Do not write a second close path.

Leave empty-cell seed opening unchanged (TryOpenFarmSeedSelection).

Save. List the file and the method you changed. STOP.
```
