# Bezy — Header popup récolte (`FarmHarvestPanel_Build`)

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-004]`  
**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Parent :** `Content` (VLG déjà en place)  
**Référence (lecture seule) :** `FarmHarvestPanel_WoodMockup.prefab` — `PlantNameLabel`, `StageLabel`, `IconFrame`, `PlantIcon`.

**Sprites :** `Assets/Art/Sprites/UI/UiKit_Frame_icon_20260919.png` (guid `a8e3521f610382f4d2705e91c34f680b`).

**Hors scope :** C#, `HarvestPanelUI`, progress/boutons, Simulate.

**Succès =** Save prefab + liste. **STOP.**

---

## Phase 1 — coller dans Bezy

```
[BZ-FARM-HARVEST-PANEL-BUILD-004] Header rows under Content. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy / Inspector ONLY. No C#. No HarvestPanelUI.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Do not rescan project. Layer 5. Do NOT edit WoodMockup or FarmHarvestPanel.prefab.

TARGET: GameObject Content (VerticalLayoutGroup already there). Add children ONLY under Content.
Do NOT move CloseButton. Do NOT change root panel size or Content VLG settings.

CHILD ORDER top-to-bottom under Content (VLG order):
1) PlantNameLabel
2) StageLabel
3) IconFrame

--- PlantNameLabel ---
- UI > TextMeshPro under Content, name: PlantNameLabel
- LayoutElement: Preferred Height 72, Flexible Width 1
- TMP: placeholder "Roquette", alignment center, font LiberationSans SDF (project default TMP), size ~36, bold if available
- Color warm parchment (~ RGB 0.98, 0.93, 0.82). Raycast Target OFF

--- StageLabel ---
- TMP under Content, name: StageLabel
- LayoutElement: Preferred Height 32
- TMP: placeholder "Mature", center, size ~22, color slightly muted tan/white. Raycast OFF

--- IconFrame ---
- UI Image under Content, name: IconFrame
- LayoutElement: Preferred Width 240, Preferred Height 240
- Image: sprite UiKit_Frame_icon_20260919.png (guid a8e3521f610382f4d2705e91c34f680b), Simple, Preserve Aspect ON, Raycast OFF

--- PlantIcon (child of IconFrame) ---
- UI Image child, name: PlantIcon (exact for HarvestPanelUI.plantIcon)
- RectTransform: anchor center, SizeDelta 120 x 120
- Image: no sprite assigned (empty), Preserve Aspect ON, Raycast OFF, color white

Do NOT add ProgressTrack or buttons in this phase.

Save prefab. List created objects + LayoutElement sizes. STOP. No Play Mode.
```

**Livré 2026-10-07 :** Bezy a résolu le cadre par nom de fichier `UiKit_Frame_icon_20260919.png` — OK.
