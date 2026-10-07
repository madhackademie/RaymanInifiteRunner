# Bezy — Fond bois `FarmHarvestPanel_Build` (scène labo)

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-001]`  
**Scène :** `Assets/Scenes/TestUi.unity` (ou scène labo ouverte si auteur a déplacé le Canvas)  
**Cible :** GameObject **`FarmHarvestPanel_Build`** enfant direct de **`Canvas`** (pas renommer le Canvas).  
**Sprite :** `Assets/Art/Sprites/UI/UiKit_Panel_9s_20260919.png` (guid `c4a91e7b2d6f48b09e3c1a5d8f0b2467`)  
**Référence tuning :** mockup shop Card — `Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_wood_mockup.md` § Phase 2 point 1.

**Hors scope :** C#, popups runtime, `FarmHarvestPanel.prefab`, Simulate, Play Mode, modal fullscreen blocker.

**Succès =** Save scène + liste changements. **STOP.**

---

## Phase 1 — coller dans Bezy

```
[BZ-FARM-HARVEST-PANEL-BUILD-001] FarmHarvestPanel_Build — panel 9s bois. STOP.

@Notes/Bezi/RULES_bezy_code.md — Inspector / hierarchy ONLY. No C#. No new scripts.

OPEN scene: Assets/Scenes/TestUi.unity
Do not rescan whole project. Layer UI = 5 on Canvas + FarmHarvestPanel_Build + any new child.

FIND under Canvas:
- GameObject name MUST be FarmHarvestPanel_Build (author lab root popup).
- If missing: create UI > Image under Canvas, name FarmHarvestPanel_Build.

DO NOT rename Canvas. DO NOT add full-screen blocker Image. DO NOT edit FarmHarvestPanel.prefab or FarmHarvestPanel_WoodMockup.prefab.

ON FarmHarvestPanel_Build (root RectTransform + Image on same GO is OK):

1) RectTransform
   - Anchors: center (0.5, 0.5) min/max, pivot (0.5, 0.5).
   - SizeDelta: 480 x 760.
   - AnchoredPosition: (0, 20, 0).

2) Image component
   - Sprite: Assets/Art/Sprites/UI/UiKit_Panel_9s_20260919.png
   - Color: (1, 1, 1, 1)
   - Image Type: Sliced (1). Fill Center ON.
   - Preserve Aspect OFF.
   - Raycast Target ON (blocks farm clicks only on this card, not full screen).
   - Pixels Per Unit Multiplier: 1 (leave 1 unless borders look too thick — then try 1.5, not Simple stretch).

3) Remove extra components if accidentally added (LayoutGroup, ContentSizeFitter) — keep only RectTransform + CanvasRenderer + Image unless Button was added by mistake.

4) Canvas parent (read-only check): Canvas Scaler Reference Resolution should stay author value (1080 x 1900 or 1920) — do not change Render Mode.

Save scene. Report: final sizeDelta, Image type, sprite name, layer. STOP. No Play Mode.
```

---

## Phase 2 optionnelle (auteur playtest d’abord)

```
[BZ-FARM-HARVEST-PANEL-BUILD-001] P2 — PPU tune only. STOP.

Same scene + FarmHarvestPanel_Build only.
If wood border too thick on Game 1920x1080 Portrait: set Image Pixels Per Unit Multiplier to 1.5 or 2.0.
Do NOT switch Image Type to Simple. Do NOT change SizeDelta unless author asked.

Save scene. STOP.
```
