# [BZ-FARM-HARVEST-CURSOR-001] Curseur gant récolte batch

**C# visuel seulement.** Le mode est déjà public sur `FarmBatchHarvestInput` (`Idle`, `Armed`, `Stroking`, événement `ModeChanged`). Ne pas modifier ce fichier.

**Art :** pas encore là. Placeholder blanc. Les clips `GloveIdle` et `GloveHarvest` seront branchés plus tard sur l’Animator, sans changer le script.

**Succès :** `Save. List files and methods. STOP.` Pas de Simulate / Play Mode.

---

## Job unique — script + enfant prefab

```
[BZ-FARM-HARVEST-CURSOR-001] View only. STOP after this.

@Notes/Bezi/RULES_bezy_code.md

READ ONLY:
@Assets/Scripts/Farm/FarmBatchHarvestInput.cs
@Assets/Prefabs/World/Biofiltre.prefab

DO NOT rescan the project. DO NOT edit FarmBatchHarvestInput.cs.
DO NOT edit any other .cs. DO NOT use Find or FindObjectOfType.

Create Assets/Scripts/Farm/FarmBatchHarvestCursor.cs
Class FarmBatchHarvestCursor : MonoBehaviour.

SerializeField SpriteRenderer marker.
SerializeField Animator animator. Animator may be null.
Cache FarmBatchHarvestInput in OnEnable with GetComponentInParent. Unsubscribe in OnDisable.

Subscribe to ModeChanged.
Idle: marker disabled.
Armed: marker enabled, Animator bool Harvesting = false (clip GloveIdle later).
Stroking: marker enabled, Harvesting = true (clip GloveHarvest later).
If animator is null, still show or hide the marker.

LateUpdate only while mode is not Idle.
Follow the mouse with FarmPointerInput.TryGetScreenPosition and Camera.main.
World point at the camera depth. Add a named constant lift on Y (0.4).
sortingOrder = 60. Color white. No sprite swap.

On the prefab, under the object that already has FarmGridPointerInput,
create child BatchHarvestCursor, layer 0 (same as that object, not UI).
SpriteRenderer + this script. Leave Animator empty.
Wire marker to that SpriteRenderer.

Save. List files and methods. STOP.
```
