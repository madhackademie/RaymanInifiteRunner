# Prompts Bezy — TabMoreOption (5ᵉ onglet Plus)

**Tâche :** `[P0-UI-TAB-MORE-001]`  
**Job Bezy :** `[BZ-TAB-MORE-001]` — `/prefab-ui-3phases`  
**Skill :** `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**Scène :** `Assets/Scenes/NavigationHUD.unity`  
**Spec :** `Notes/Ui/SPEC_features_hub_plus.md` (variante B : 4 + Plus) · mockup `Notes/Ui/SPEC_nav_onglets_zoom_actif.md`  
**C# Cursor déjà livré :** `ScreenId.FeaturesHub`, `OnTabMoreOptionClicked`, champs `tabMoreOption*` — **Bezy ne touche pas au .cs**  
**Sprite :** **aucun en V0** (label `Plus` + cadre quand actif). Art après : § Sprite ci-dessous.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

---

## Bloc de lancement (coller dans Bezy)

Ouvrir **`NavigationHUD.unity`** avant l’appel. Une phase par thread.

```
/prefab-ui-3phases
Task ID: [BZ-TAB-MORE-001]
Prefab: Assets/Scenes/NavigationHUD.unity
Phase: 1
```

Puis `@Notes/Ui/PROMPTS_Bezi_tab_more_option.md`

---

## Phase 1 — Hiérarchie seule (pas de sprite)

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

## Phase 2 — Composants (toujours sans sprite)

```
[BZ-TAB-MORE-001] PHASE 2 components ONLY. P1 done. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer 5. TabMoreOption ONLY.
Reference TabInventaire READ-ONLY. Do NOT edit .cs. Do NOT change other tabs.

SPRITE LOCK: Icon sprite stays NONE. Do NOT assign any PNG.

1) Glow: copy TabInventaire/IconLift/Glow Image + RT
   (anchor center, pos 0,8, size 108x108, NavTabSoftGlow.mat,
   color RGB(1,0.78,0.2) alpha ~0.4, Raycast OFF). GO OFF default.

2) Icon: copy Inventaire Icon RT (stretch, sizeDelta about -8,-38).
   Sprite empty, alpha 0, Preserve Aspect ON, Raycast OFF.

3) Label TMP: text "Plus". Copy TabInventaire/Label RT + style
   (fontSize 31, Bold, face RGB(1,0.78,0.2), Outline black ~0.28).
   Raycast OFF. GO OFF default (script shows when tab active).

4) SelectedFrame: copy Inventaire (anchoredPosition 0,0, sizeDelta -4,-4).
   GO OFF default. Do not add a new wood frame.

5) ActiveLava OFF. All mockup FX OFF by default.

Save. List paths changed. STOP. No Play Mode.
```

---

## Phase 3 — Wiring Inspector (C# déjà là)

```
[BZ-TAB-MORE-001] PHASE 3 wiring ONLY. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer 5.
File ONLY this scene. Do NOT edit .cs (Cursor already added methods/fields).
No RectMask2D. Do NOT change other tabs. Icon sprite still NONE.

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

---

## Sprite (hors Bezy — après V0)

Dump : `Assets/Art/Assets Store Dump/Ui/Nav/Tabs/IconeTab_Plus_glyph.png`  
Promo après OK : `Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Plus_glyph.png` → `TabMoreOption/IconLift/Icon`

Joindre en option `IconePlay.png` (poids visuel). Prompt :

```
Mobile game bottom navigation GLYPH icon, single object only, casual fantasy stone-bar UI style like Clash-style menus but aquaponic farm theme. Bold thick silhouette, vibrant saturated colors, smooth cartoon shading, thick outline. Square canvas 256x256, subject fills 90% of frame, transparent background, NO text, NO frame, NO scene, NO isometric diorama — one readable symbol only.

A simple "more options" tab glyph: a 2x2 grid of four chunky rounded squares (or three bold dots in a circle). Same visual weight as the attached greenhouse icon. Centered, fills 90% of frame. NO letters, NO plus-sign made of thin lines that vanish at 72px. Hard silhouette, no outer glow.
```

Mini prompt Bezy **après promo** (pas maintenant) : assigner le sprite sur `TabMoreOption/IconLift/Icon` seulement.

---

## Hors scope (V0)

- Prefab `FeaturesHubScreen` / lignes Quêtes-Atelier-Mail — `[BL-UI-FEATURES-HUB-001]`
- Variante A (masquer Shop)
- Wallet disparu `[P0-NAV-WALLET-REG-001]`
