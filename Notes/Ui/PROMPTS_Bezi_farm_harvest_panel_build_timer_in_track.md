# Bezy — Timer dans la barre de progression (MVP)

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-006-TRACK]`  
**Décision auteur 2026-10-07 :** pas de ligne timer séparée — le compte à rebours vit **dans** `ProgressTrack` (fill + texte). Supprimer `TimerPlaque`.  
**C# :** inchangé — `HarvestPanelUI` garde `timerLabel` + `stageProgressFill` (refs Inspector plus tard).

**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`

---

```
[BZ-FARM-HARVEST-PANEL-BUILD-006-TRACK] Timer inside ProgressTrack. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy ONLY. No C#. No HarvestPanelUI yet.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Layer 5. Do not rescan project.

1) DELETE GameObject TimerPlaque completely.

2) Reparent TimerLabel under ProgressTrack (NOT under Content directly).
   Sibling order inside ProgressTrack (bottom to top draw):
   - ProgressFill (first)
   - TimerLabel (last — draws on top of fill)

3) TimerLabel RectTransform:
   - Stretch full track (anchors 0,0 to 1,1), offsets ~12 L/R, 8 T/B (inside groove)
   - Remove LayoutElement from TimerLabel if present (not a VLG row anymore)

4) TimerLabel TMP:
   - Text "02:15:30", center + middle align, size ~22
   - Color dark brown readable on teal fill AND tan track: ~ RGB 0.32, 0.22, 0.14
   - Raycast OFF, font bold optional

5) Content VLG direct children order (no TimerLabel, no TimerPlaque):
   PlantNameLabel, StageLabel, IconFrame, ProgressTrack, YieldLabel
   (+ BatchHarvestButton only if already in prefab)

6) Do NOT change ProgressFill fill settings, CloseButton, header sprites.

Save prefab. Confirm hierarchy ProgressTrack > ProgressFill + TimerLabel. STOP. No Play Mode.
```
