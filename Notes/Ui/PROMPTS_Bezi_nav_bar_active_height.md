# [BZ-NAV-BAR-HEIGHT-001] Fond nav 140 px — slots restent 128 (bas)

**Scène :** `Assets/Scenes/NavigationHUD.unity`  
**Cible :** `NavBarContainer` Image (bandeau sombre) — **pas** le widget monnaie 275.  
**Variante auteur 2026-09-16 :** fond **140** ; onglets **128 ancrés bas** (C# `NavigationHUD.NavTabSlotHeight`).  
**Interdit :** agrandir les `Tab*` / Icon / IconLift / SelectedFrame. Ça gonfle le zoom actif (×1.87).  
**Spec :** `Notes/Ui/SPEC_nav_onglets_zoom_actif.md` (pas de RectMask2D).

**Déjà fait Cursor :** `ApplyEqualNavTabSlots` → hauteur slot **128**, pivot bas. Ne **pas** éditer `.cs`.  
**Après Bezy :** playtest auteur (taille icône+cadre actif = avant). `UIManager.NavBarHeight` reste **260** (modales).  
**Checkpoint :** tag `hud-bandeau-before-fond140-slots128` · `backup/hud-bandeau-fond140-slots128`.

---

## Prompt copier-coller Bezy (scène ONLY)

```
[BZ-NAV-BAR-HEIGHT-001] NavBarContainer height EXACTLY 140 px. SCENE ONLY. STOP.

OPEN Assets/Scenes/NavigationHUD.unity FIRST. UI m_Layer = 5. Do NOT rescan. Do NOT edit .cs. Do NOT edit prefabs.

GOAL: Dark fond only grows to 140 px. Tab slots stay 128 px at the BOTTOM (runtime C#). Active icon + SelectedFrame must KEEP current pixel size. Do NOT use 128 for the fond. Do NOT guess 176–192.

1) NavBarContainer RectTransform:
   - Keep AnchorMin (0,0) AnchorMax (1,0) Pivot (0.5,0) AnchoredPosition (0,0).
   - Set sizeDelta to (0, 140). Height = 140 only.
   - Image (dark strip) stretch full rect. Raycast ON OK.

2) Do NOT resize TabAventures, TabInventaire, TabShop, TabVente, TabMoreOption.
   Do NOT change their anchors, pivot, sizeDelta, or sibling order.
   Do NOT enable/tweak HorizontalLayoutGroup to fill 140 (runtime disables HLG).
   Do NOT add RectMask2D / Mask on NavBarContainer or any tab.

3) Do NOT change Icon, IconLift, Glow, Label, SelectedFrame (sizeDelta, scale, pos, expand).
   Keep idle SelectedFrame X: TabAventures +10 ; TabVente and TabMoreOption -10.

4) If currency HUD (stamps + 275) overlaps the taller bar, nudge its Y up a few px only. Do not resize that widget.

Save scene. Reply: NavBarContainer sizeDelta.y = 140. Confirm Tab* sizeDelta unchanged. List what changed. STOP. No Play Mode / Simulate.
```

**Lancement Unity :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_nav_bar_active_height.md
[BZ-NAV-BAR-HEIGHT-001] NavBarContainer height EXACTLY 140 px. SCENE ONLY. STOP.
```
