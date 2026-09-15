# [BZ-NAV-TAB5-CLIP-001] Nav — 5ᵉ onglet (Plus) coupé

**Scène :** `Assets/Scenes/NavigationHUD.unity`  
**Spec :** `Notes/Ui/SPEC_nav_onglets_zoom_actif.md` (pas de RectMask2D sur la barre)  
**Cursor (déjà) :** `NavigationHUD.cs` — labels `ignoreLayout`, rebuild HLG après refresh modale — **insuffisant**.

**Symptôme auteur :** onglet 1 **Aventures** = jamais coupé ; onglet 5 **Plus** = coupé quasi systématique après usage 2–4 ; parfois aléatoire sur 2–4.

**Fix Bezy historique (2026-09-13, `PROMPTS_Bezi_tab_sprites.md` § correctifs cadre) :**
- **Aventures** `SelectedFrame` repos **X = +10** (bord gauche écran).
- **Vente** repos **X = -10** (bord droit).
- **Plus** doit avoir le **même -10 X** que Vente — souvent oublié à la création du 5e onglet.
- **Jamais** `RectMask2D` sur `NavBarContainer`.
- Autre fix 2026-09-15 : **relink sprites** `Nav/Tabs` (cadres qui s’empilaient sur Plus = mauvais PNG, pas clip layout).

**Cursor (2026-09-15) :** layout 5 slots manuel + retrait reorder `ScreenRoot` + `-10 X` sur `TabMoreOption/SelectedFrame` en scène.

---

## Prompt copier-coller Bezy (scène ONLY)

```
[BZ-NAV-TAB5-CLIP-001] Fix TabMoreOption clipped in nav bar. SCENE ONLY. STOP.

Do NOT rescan project. OPEN Assets/Scenes/NavigationHUD.unity FIRST. UI m_Layer = 5.

BUG: 5th tab Plus icon/frame cut (top or right) after clicking Inventaire/Shop/Vente; tab 1 Aventures OK. Cursor layout rebuild in NavigationHUD.cs did not fix — fix scene layout.

SCOPE: NavBarContainer + TabAventures, TabInventaire, TabShop, TabSaleChannels, TabMoreOption only. NO UIManager, SceneNavigator, popups, other scenes.

1) NavBarContainer HorizontalLayoutGroup: padding left/right 8–12 px; childAlignment LowerCenter; spacing 0; childForceExpandWidth ON; childControlWidth ON.

2) On EACH of the 5 tab root Buttons: LayoutElement minWidth=0, flexibleWidth=1, preferredWidth=-1 (equal slots on all screen widths).

3) TabMoreOption vs TabAventures: same hierarchy pattern (SelectedFrame, IconLift, Label). Extra ActiveLava on Plus → LayoutElement ignoreLayout=true (must not steal width).

4) SelectedFrame idle (all 5 tabs): match TabAventures — stretch anchors, anchoredPosition (10,0), sizeDelta (-4,-4) if Aventures uses that. Same idle on TabMoreOption SelectedFrame.

5) Icon (Image) under IconLift: Preserve Aspect ON; pivot center; raycast off OK.

6) Remove RectMask2D / Mask on NavBarContainer or any tab if present (zoom must not clip).

7) If Plus still clips vertically when active: NavBarContainer height 136–144 (was 128).

Do NOT run Play Mode / Simulate. Save scene. List what changed. STOP.
```

**Lancement Unity :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_nav_tab5_clip_fix.md
[BZ-NAV-TAB5-CLIP-001] Fix TabMoreOption clipped in nav bar. SCENE ONLY. STOP.
```

---

## Phase 2 (seulement si scène insuffisante)

Thread séparé + `@Notes/Bezi/RULES_bezy_code.md` — ajuster `NavigationHUD.cs` : constantes lift/scale **réduites pour Tab.MoreOption only** ou padding slot bord droit. Cursor revue après Bezy.
