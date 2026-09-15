# [BZ-NAV-BAR-HEIGHT-001] Barre nav — fond 140 px (IconLift + zoom)

**Scène :** `Assets/Scenes/NavigationHUD.unity`  
**Cible :** `NavBarContainer` (bandeau sombre derrière les 5 onglets) — **pas** le widget monnaie 275.  
**Aujourd’hui :** `sizeDelta.y = 128`. **Cible auteur :** **140**.  
**Spec :** `Notes/Ui/SPEC_nav_onglets_zoom_actif.md` (pas de RectMask2D).

**Après Bezy :** Cursor aligne `UIManager.NavBarHeight = 140f` (modales).

---

## Prompt copier-coller Bezy (scène ONLY)

```
[BZ-NAV-BAR-HEIGHT-001] NavBarContainer height EXACTLY 140 px. SCENE ONLY. STOP.

OPEN Assets/Scenes/NavigationHUD.unity FIRST. UI m_Layer = 5. Do NOT rescan. Do NOT edit .cs.

GOAL: Dark fond (NavBarContainer Image) must be 140 px tall so the active tab IconLift + zoom + SelectedFrame + TMP label sit ON the dark strip. Author: 140 is the correct fond size. Do NOT use 128. Do NOT guess 176–192.

1) NavBarContainer RectTransform:
   - Keep anchor bottom stretch: AnchorMin (0,0) AnchorMax (1,0) Pivot (0.5,0) AnchoredPosition (0,0).
   - Set sizeDelta to (0, 140). Height = 140 only.
   - Image (dark strip) stretch full rect. Raycast ON OK.

2) 5 tab roots stay children of NavBarContainer (order: TabAventures, TabInventaire, TabShop, TabVente, TabMoreOption). HLG unchanged (padding L/R 10, childAlignment LowerCenter, childControl/ForceExpand ON). Tabs fill the new 140 height. Do NOT add RectMask2D / Mask.

3) Do NOT change icon scale, IconLift Y, Glow, or SelectedFrame expand. Keep idle SelectedFrame X: TabAventures +10 ; TabVente and TabMoreOption -10.

4) If currency HUD (stamps + 275) overlaps the taller bar, nudge its Y up a few px only. Do not resize that widget.

Save scene. Reply: NavBarContainer sizeDelta.y = 140. List what changed. STOP. No Play Mode / Simulate.
```

**Lancement Unity :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_nav_bar_active_height.md
[BZ-NAV-BAR-HEIGHT-001] NavBarContainer height EXACTLY 140 px. SCENE ONLY. STOP.
```
