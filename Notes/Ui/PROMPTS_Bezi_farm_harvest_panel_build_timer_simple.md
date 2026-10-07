# Bezy — Timer allégé (texte seul) — **remplacé**

**Remplacé par :** `PROMPTS_Bezi_farm_harvest_panel_build_timer_in_track.md` — timer **dans** `ProgressTrack` (pas de ligne VLG dédiée).

**Task historique :** `[BZ-FARM-HARVEST-PANEL-BUILD-006-SIMPLE]`  
**Décision auteur 2026-10-07 :** bandeaux `Bar_rope` / `Row_list` **trop chargés** sur le panneau bois → **pas de plaque** pour le MVP. Art dédié plus tard (`H-farm-6` option icône horloge).

**Prefab :** `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`  
**Runtime :** seul **`TimerLabel`** (TMP) est câblé dans `HarvestPanelUI` — pas de `TimerPlaque` requis.

---

```
[BZ-FARM-HARVEST-PANEL-BUILD-006-SIMPLE] Timer text only — remove TimerPlaque. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy ONLY. No C#.

OPEN Prefab Mode: Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab
Layer 5. Do not rescan project.

GOAL: lightweight timer row — TMP only, no Image band behind timer.

1) REMOVE GameObject TimerPlaque entirely (delete parent + move child up):
   - Keep TMP object named TimerLabel.

2) Reparent TimerLabel directly under Content (VerticalLayoutGroup).

3) VLG child order under Content must be:
   PlantNameLabel, StageLabel, IconFrame, ProgressTrack, TimerLabel, YieldLabel
   (BatchHarvestButton after if already exists — if not, skip)

4) TimerLabel setup:
   - LayoutElement: Preferred Height 40, Flexible Width 1
   - TMP: placeholder "02:15:30" (runtime will overwrite)
   - Font size ~26, center alignment
   - Color muted warm gray-brown (readable on wood, not full white banner): ~ RGB 0.75, 0.68, 0.58
   - Raycast OFF. No outline box, no Image sibling.

5) Do NOT change ProgressTrack, YieldLabel, CloseButton, header, batch buttons.

Save prefab. Confirm TimerPlaque deleted + TimerLabel parent = Content. STOP. No Play Mode.
```

**Post-MVP art :** icône horloge 64px (`H-farm-6`) en enfant horizontal layout **optionnel** — pas avant validation MVP texte seul.
