# Bezy — Croix close `FarmHarvestPanel_Build` (scène labo)

**Statut :** **clos 2026-10-07** — CloseButton validé auteur sur mockup (`FromMockup` / workflow unity-ugui). Ne pas relancer sauf changement art.

**Task :** `[BZ-FARM-HARVEST-PANEL-BUILD-002]`  
**Scène :** `Assets/Scenes/TestUi.unity`  
**Parent :** `FarmHarvestPanel_Build`  
**Référence layout :** `CloseButton` dans `Assets/Prefabs/Ui/FarmHarvestPanel_WoodMockup.prefab` (coin haut-droit 72×72).

**Sprite :** `Assets/Art/Assets Store Dump/Ui/UiKit_BtnClose_wood_20261005.png` (guid `c49b9da07d3a3a94b8bf70c580718456`, sub-sprite `UiKit_BtnClose_wood_20261005_0`).

**Hors scope :** C#, `HarvestPanelUI`, onClick, runtime prefabs, resize panel, Simulate.

**Succès =** Save scène + liste. **STOP.**

---

## Phase 1 — coller dans Bezy

```
[BZ-FARM-HARVEST-PANEL-BUILD-002] CloseButton wood on FarmHarvestPanel_Build. STOP.

@Notes/Bezi/RULES_bezy_code.md — hierarchy / Inspector ONLY. No C#. No scripts.

OPEN scene: Assets/Scenes/TestUi.unity
Do not rescan whole project. Layer 5 on all UI touched.

READ reference ONLY (do not edit this prefab):
Assets/Prefabs/Ui/FarmHarvestPanel_WoodMockup.prefab — GameObject CloseButton (top-right 72x72).

TARGET parent: FarmHarvestPanel_Build under Canvas (must exist with panel 9s Image).

CREATE child under FarmHarvestPanel_Build:
- Name: CloseButton (exact, for future HarvestPanelUI.closeButton wiring).

RectTransform (match WoodMockup):
- Anchors min/max (1, 1) top-right.
- Pivot (1, 1).
- AnchoredPosition (-36, -36).
- SizeDelta (72, 72).

Components on CloseButton root:
- Image: sprite Assets/Art/Assets Store Dump/Ui/UiKit_BtnClose_wood_20261005.png
  Color (1,1,1,1). Type Simple (0). Preserve Aspect ON. Raycast Target ON.
- Button: Target Graphic = Image. Transition Color Tint. No persistent onClick (lab mockup).

Do NOT add TMP Label (sprite includes X art). Do NOT add LayoutGroup on panel.
Do NOT change FarmHarvestPanel_Build size or panel sprite.
Do NOT edit FarmHarvestPanel.prefab / FarmHarvestPanel_WoodMockup.prefab.

Ensure CloseButton is LAST sibling OR on top in hierarchy so it draws above panel (after panel background if single GO — add as child so it renders on top).

Save scene. Report CloseButton rect + sprite assigned. STOP. No Play Mode.
```
