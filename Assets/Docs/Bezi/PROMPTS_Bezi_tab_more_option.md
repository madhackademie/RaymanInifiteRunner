# [BZ-TAB-MORE-001] TabMoreOption — Bezy prompt

**Ref `@` Unity :** `@Assets/Docs/Bezi/PROMPTS_Bezi_tab_more_option.md`  
**Scène :** `Assets/Scenes/NavigationHUD.unity` (must be open).  
**Pas** `/prefab-ui-3phases` (scène, not `Assets/Prefabs/Ui/`).  
**Pas** `.cs` (Cursor already wired `OnTabMoreOptionClicked`).  
**Sprite :** `Nav/Tabs/IconeTab_Plus_glyph.png` (feuille composite — Phase 2).

**Launch line (one phase per Bezy call):**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_tab_more_option.md
[BZ-TAB-MORE-001] Execute PHASE 1 only. NavigationHUD.unity open. No .cs. Save. List hierarchy. STOP.
```

Replace `PHASE 1` with `PHASE 2` or `PHASE 3` when ready.

---

## Phase 1 — hierarchy only (no sprite)

```
[BZ-TAB-MORE-001] PHASE 1 hierarchy ONLY. Wait success. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. Layer UI = 5.
Do NOT rescan whole project. Do NOT edit .cs. File ONLY this scene.

GOAL: add 5th nav tab TabMoreOption. NO sprite.

1) Duplicate TabInventaire. Rename the copy to TabMoreOption.
   Parent = NavBarContainer. Set as LAST sibling AFTER TabVente.
   Keep LayoutElement like other tabs. HLG already expands width.

2) Child ORDER under TabMoreOption (same as TabInventaire):
   SelectedFrame (default OFF)
   ActiveLava if present (GO OFF)
   IconLift
     Glow (first child of IconLift)
     Icon
   Label

3) TabMoreOption/IconLift/Icon Image: sprite = NONE. Color white alpha 0.
   Preserve Aspect ON. Raycast OFF. Do NOT assign any PNG.

4) KEEP Button + Animator NavTab (same controller as the duplicate source).
   CLEAR Button OnClick copied from Inventaire — leave OnClick empty until Phase 3.
   Do NOT leave OnTabInventaireClicked on this tab.

5) Do NOT modify TabAventures, TabInventaire, TabShop, TabVente.
   No RectMask2D on NavBarContainer. Do not restore wallet UI.

Save. List hierarchy paths. STOP. No Play Mode.
```

---

## Phase 2 — visuel mockup (sprite Plus = même pipeline que Inventaire)

```
[BZ-TAB-MORE-001] PHASE 2 components ONLY. P1 done. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer 5. TabMoreOption ONLY.
Reference TabInventaire READ-ONLY. Do NOT edit .cs. Do NOT change other tabs.

SPRITE (Plus tab): assign FULL texture (composite sheet OK — author validated):
TabMoreOption/IconLift/Icon → Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Plus_glyph.png
Same Image rules as TabInventaire/Icon: Preserve Aspect ON, Raycast OFF, color white alpha 1.
Do NOT slice unless author asks. If file missing: STOP.

1) Glow: copy TabInventaire/IconLift/Glow Image + RT
   (anchor center, pos 0,8, size 108x108, NavTabSoftGlow.mat,
   color RGB(1,0.78,0.2) alpha ~0.4, Raycast OFF). GO OFF default.

2) Icon: copy Inventaire Icon RT (stretch, sizeDelta about -8,-38).
   Sprite = IconeTab_Plus_glyph.png (path above). Preserve Aspect ON, Raycast OFF.

3) Label TMP: text "Plus". Copy TabInventaire/Label RT + style
   (fontSize 31, Bold, face RGB(1,0.78,0.2), Outline black ~0.28).
   Raycast OFF. GO OFF default (script shows when tab active).

4) SelectedFrame: copy Inventaire (anchoredPosition 0,0, sizeDelta -4,-4).
   GO OFF default. Do not add a new wood frame.

5) ActiveLava OFF. All mockup FX OFF by default.

Save. List paths changed. STOP. No Play Mode.
```

---

## Phase 3 — Inspector wiring (C# already in repo)

```
[BZ-TAB-MORE-001] PHASE 3 wiring ONLY. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer 5.
File ONLY this scene. Do NOT edit .cs (Cursor already added methods/fields).
No RectMask2D. Do NOT change other tabs. Icon sprite = IconeTab_Plus_glyph.png on TabMoreOption.

On HUDRoot component NavigationHUD assign:
- tabMoreOptionButton → TabMoreOption Button
- tabMoreOptionIcon → TabMoreOption/IconLift/Icon Image
- tabMoreOptionSelectedFrame → TabMoreOption/SelectedFrame
- tabMoreOptionIconLift → TabMoreOption/IconLift RectTransform
- tabMoreOptionGlow → TabMoreOption/IconLift/Glow
- tabMoreOptionLavaBackdrop → TabMoreOption/ActiveLava if exists, else None
- tabMoreOptionLabel → TabMoreOption/Label TMP

Button OnClick: ONE persistent call
HUDRoot / NavigationHUD.OnTabMoreOptionClicked
Remove any leftover OnTabInventaireClicked from the duplicate.

If a tabMoreOption* field is missing on HUDRoot: STOP and list missing names. Do not invent C#.

Save. List wired fields. STOP. No Play Mode.
```
