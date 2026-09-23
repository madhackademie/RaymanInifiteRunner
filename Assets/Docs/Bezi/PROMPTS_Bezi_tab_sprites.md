# Prompts Bezy — sprites onglets HUD `[BZ-TAB-SPRITES-001]`

**Statut :** **CLOS playtest 2026-09-23** — HUD + 5 onglets validés auteur.  
**Historique :** **pivot 2026-09-10** — mockup zoom actif + glyphes simples (`Notes/Ui/SPEC_nav_onglets_zoom_actif.md`). Anciens pilotes iso = référence hiérarchie seulement ; **nouvelle passe Bezy** après promo sprites H-nav mockup.

### Régression connue (2026-09-09)

- **Wallet / solde gold** en haut de la barre nav (`NavigationHUD`) **absent** après passes Bezy onglets — **à corriger plus tard** (pas bloquant ce prompt).
- ID suivi : `[P0-NAV-WALLET-REG-001]` dans `Notes/Todo_project.md`.

**Tâche session :** `[P0-TAB-SPRITES-001]` — mise en place + polish des sprites sur les onglets.  
**Job Bezy :** `[BZ-TAB-SPRITES-001]` — `/prefab-ui-3phases`  
**Skill :** `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**File :** `Notes/Bezi/BEZY_QUEUE.md`  
**Art (hors Bezy) :** Vague H `Notes/Art/PROMPT_generation_icones.md` — Dump → promo auteur `Sprites/UI/` **avant** wiring.  
**Hub Plus (V0 onglet) :** `Assets/Docs/Bezi/PROMPTS_Bezi_tab_more_option.md` — `[BZ-TAB-MORE-001]` `TabMoreOption` sans sprite.

**Succès Bezy = Save + liste changements. STOP. Pas de Simulate / Play Mode.**

---

## Gate (obligatoire)

1. L’auteur fournit la **liste des changements visuels** (quels onglets, quels sprites, idle / selected / press, tailles, labels, etc.).
2. Cursor rédige **Phase 1 → 2 → 3** ici (une phase par appel, moins de 3500 caractères).
3. **Validation auteur** des prompts collables.
4. Ensuite seulement : file `BEZY_QUEUE.md` + lancement Unity.

Tant que les blocs Phase ci-dessous sont vides : **aucun envoi Bezy**.

---

## Cible (mockup 2026-09-10)

**NavigationHUD** — **5 onglets** (4 + `TabPlus` quand présent) :

- Hiérarchie : `SelectedFrame` + **`Glow`** + **`IconLift`** + `Icon` + **`Label`** (TMP, actif seulement)
- Spec complète : `Notes/Ui/SPEC_nav_onglets_zoom_actif.md`
- **C# / visuel onglets :** Bezy (scène + `NavigationHUD.cs` mockup — `@Notes/Bezi/RULES_bezy_code.md`). Cursor : specs, prompts, revue — pas le tuning placement/zoom sauf « sans Bezy ».
- Press `[BZ-POLISH-006]` `NavTab.controller` — **ne pas casser**

**Optionnel — barre inventaire** (`InventoryScreen` / `InventoryFilterBar`) : uniquement si le brief auteur le demande. Ne pas mélanger HUD nav et filtres inventaire dans le même prompt.

**Hors scope Bezy :** génération PNG, promo Dump → Sprites, logique C# (`SceneNavigator`), Simulate.

**Prérequis Editor :** ouvrir la scène / le prefab cible en mode dédié avant d’envoyer (workaround path Bezy — `Notes/Bezi/README_bezi.md`).

---

## Pilote mockup zoom — TabAventures (Play) — 2026-09-10

**Prérequis auteur :** glyphe Play assigné sur `TabAventures/Icon` (promo `Sprites/UI/Nav/Tabs/`).  
**Spec :** `Notes/Ui/SPEC_nav_onglets_zoom_actif.md`  
**Une phase par appel Bezy.** Succès = Save + liste changements. **STOP. Pas Simulate.**

### Phase 1 — Hiérarchie seule

```
[BZ-TAB-SPRITES-001] PILOT TabAventures mockup zoom — PHASE 1 hierarchy ONLY. Wait success. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. Layer UI = 5.
Do NOT rescan whole project. Do NOT edit C#. File ONLY this scene.

Target: TabAventures ONLY. Do NOT modify TabInventaire, TabShop, TabVente.

1) Under TabAventures, ensure child ORDER (first to last):
   SelectedFrame (keep)
   Glow (CREATE empty GameObject + RectTransform)
   IconLift (CREATE empty GameObject + RectTransform)
   Label (reuse existing Label child if present; else CREATE empty)

2) MOVE existing "Icon" GameObject to be child of IconLift (path: TabAventures/IconLift/Icon).
   KEEP the sprite already assigned on Icon Image (author placed glyph). Do NOT change sprite.

3) IconLift RectTransform: anchors min (0,0) max (1,1), pivot (0.5,0.5), anchoredPosition 0,0, sizeDelta 0,0, scale 1,1,1. No Image component yet.

4) Glow RectTransform: anchor center (0.5,0.5), pivot (0.5,0.5), sizeDelta 100x100, anchoredPosition (0, 8). No Image yet.

5) Do NOT add RectMask2D to NavBarContainer. KEEP Button, Animator NavTab, SelectedFrame borders, OnClick OnTabAventuresClicked.

Save. List hierarchy. STOP.
```

### Phase 2 — Composants visuels (legacy Knob — remplacé par `[BZ-TAB-SPRITES-002]`)

Voir section **Mockup FX glow + lave** ci-dessous.

### Phase 3 — Wiring minimal (2026-09-10) — compléter avec `[BZ-TAB-SPRITES-002]` Phase 3

---

## Mockup FX glow + lave — TabAventures `[BZ-TAB-SPRITES-002]`

**Prérequis :** Phase 1 hiérarchie OK (`IconLift`, `Glow`, `Label`). Materials Cursor : `Assets/Materials/UI/NavTabSoftGlow.mat`, `NavTabLavaBackdrop.mat`, `NavTabIconGrayscale.mat`.  
**Spec :** `Notes/Ui/SPEC_nav_onglets_zoom_actif.md`  
**Une phase par appel.** Succès = Save + liste. **STOP. Pas Simulate.**

### Phase 2 — Composants + hiérarchie FX

```
[BZ-TAB-SPRITES-002] TabAventures mockup FX — PHASE 2 components ONLY. STOP after Save.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer = 5 on all UI.
Do NOT rescan project. Do NOT edit .cs. File ONLY this scene. TabAventures ONLY.

1) Child ORDER under TabAventures:
   SelectedFrame (keep, default OFF)
   ActiveLava (CREATE if missing)
   IconLift (existing)
   Label (existing)

2) ActiveLava: OPTIONAL (Cursor `useActiveTabLavaBackdrop` = false by default). Prefer **cadre** not fond plein.
   If created: same as before but GO always OFF unless auteur demande lave.

2b) SelectedFrame: 4 borders gold 3px — **ON when tab active** (script toggles). Pas de Image fill sur le slot.

3) MOVE Glow under IconLift as FIRST child (TabAventures/IconLift/Glow, before Icon).
   Glow RT: anchor center, pivot center, pos(0,6), sizeDelta 148x148.
   Image: UISprite white, Material = Assets/Materials/UI/NavTabSoftGlow.mat,
   color RGB(1,0.88,0.22) alpha 0.72, Raycast OFF, GO OFF by default.

4) IconLift/Icon: stretch (0,0)-(1,1), sizeDelta(-8,-34), Preserve Aspect ON, white, Raycast OFF. KEEP author sprite.

5) Label TMP: text "Aventures", anchor bottom stretch, pivot(0.5,0), pos(0,12), height 30,
   font 24 bold, face RGB(1,0.92,0.2), Outline black ~0.28, Raycast OFF, GO OFF by default.

6) SelectedFrame: keep but OFF by default (Cursor désactive legacy cadre). No RectMask2D on NavBarContainer.

Save. List paths. STOP. No Play Mode.
```

### Phase 3 — Wiring HUDRoot

```
[BZ-TAB-SPRITES-002] TabAventures — PHASE 3 wiring ONLY. Phase 2 done. STOP.

OPEN NavigationHUD.unity. Do NOT edit C#. HUDRoot + TabAventures only.

NavigationHUD on HUDRoot — wire:
tabAventuresButton → TabAventures Button
tabAventuresIcon → TabAventures/IconLift/Icon (Image)
tabAventuresSelectedFrame → TabAventures/SelectedFrame
tabAventuresIconLift → TabAventures/IconLift (RectTransform)
tabAventuresGlow → TabAventures/IconLift/Glow
tabAventuresLavaBackdrop → TabAventures/ActiveLava
tabAventuresLabel → TabAventures/Label (TMP)
activeTabGlowMaterial → Assets/Materials/UI/NavTabSoftGlow.mat
activeTabLavaMaterial → Assets/Materials/UI/NavTabLavaBackdrop.mat
inactiveTabGrayscaleMaterial → Assets/Materials/UI/NavTabIconGrayscale.mat (if empty)

Do NOT change lift/scale numbers unless fields empty. KEEP Animator NavTab + OnClick.

Save. List refs. STOP.
```

---

## TabInventaire — copie patron mockup `[BZ-TAB-INVENTAIRE-MOCKUP-001]`

**Prérequis — art validé :** `Assets/Art/Sprites/UI/Inventory/InventaireIconeBagPack.png` sur `TabInventaire/IconLift/Icon` (promu depuis Dump 2026-09-13). Glyphe **sans texte** dans le PNG ; label TMP « Inventaire ».  
**Ownership (2026-09-14) :** placement, zoom, cadre, wiring = **Bezy** (scène + `NavigationHUD.cs` simple — `@Notes/Bezi/RULES_bezy_code.md`). Cursor = prompts + revue, pas le tuning visuel onglets.

### Correctifs layout cadre — anti-débord écran / coupe haut (2026-09-13)

**P1 ne touche pas** au `RectTransform` de `SelectedFrame` (repos inchangé).

| Onglet | `SelectedFrame` repos (scene) | Note |
|--------|-------------------------------|------|
| TabAventures | `anchoredPosition (10, 0)`, `sizeDelta (-4, -4)` | Correctif Bezy bord **gauche** écran — **ne pas copier +10** sur les autres onglets. |
| TabInventaire / TabShop | `(0, 0)` | Garder tel quel en P1–P2 sauf demande auteur. |
| TabVente | `(0, 0)` → cible wood **`-10` en X** repos seulement (`[BZ-NAV-WOOD-FRAME-001]` P2). | Évite débord **droite**. |

**Actif (runtime) :** `NavigationHUD` — `activeTabFrameActivePosY`, `activeTabFrameSizeDeltaExpand`, `activeTabIconScale`, lift, glow (Inspector + C# visuel). **Bezy** aligne Inventaire sur le patron TabAventures (déjà corrigé sur Play).

**Bezy — règles communes :**
- **Jamais** `RectMask2D` sur `NavBarContainer` (128 px haut) — sinon coupe icône zoomée / bois.
- `WoodFrame` (bois P2) : stretch dans `SelectedFrame`, **Preserve Aspect ON**, raycast OFF ; pas d’agrandissement manuel qui dépasse la safe zone latérale.
- P2 mockup : copier **IconLift / Glow / Label / Icon** depuis `TabAventures` (chiffres identiques), pas le **+10 X** du `SelectedFrame` Aventures.

### Phase 1 — Hiérarchie (validé 2026-09-13 — prêt à coller)

```
[BZ-TAB-INVENTAIRE-MOCKUP-001] PHASE 1 hierarchy ONLY. Wait success. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer = 5 on all new UI.
Do NOT rescan whole project. Do NOT edit .cs. File ONLY this scene.
Reference READ-ONLY: TabAventures (patron). Target: TabInventaire ONLY.
Do NOT modify TabAventures, TabShop, TabVente.

Current TabInventaire: SelectedFrame + Icon (direct child) + Label (inactive). No IconLift yet.

1) Child ORDER under TabInventaire (first to last):
   SelectedFrame (keep)
   ActiveLava (CREATE empty RectTransform — no Image yet)
   IconLift (CREATE empty RectTransform)
   Label (reuse existing; keep inactive)

2) MOVE existing Icon GameObject → TabInventaire/IconLift/Icon.
   KEEP sprite InventaireIconeBagPack.png (path above). Do NOT change sprite.

3) IconLift RT: anchors min (0,0) max (1,1), pivot (0.5,0.5), anchoredPosition 0,0, sizeDelta 0,0, scale 1,1,1. No Image.

4) Under IconLift CREATE empty child Glow (RectTransform only — Image in Phase 2). Glow = first sibling before Icon.

5) ActiveLava RT: copy anchor/pivot/size from TabAventures/ActiveLava if present; else stretch like Aventures sibling #2. No Image yet.

6) KEEP Button, Animator NavTab, OnClick OnTabInventaireClicked. No RectMask2D on NavBarContainer.

7) DO NOT change TabInventaire/SelectedFrame RectTransform (stay anchoredPosition 0,0 sizeDelta -4,-4).

Save. List final hierarchy paths. STOP. No Play Mode.
```

### Phase 2 — Composants (copie Aventures) — prêt après P1 OK

```
[BZ-TAB-INVENTAIRE-MOCKUP-001] PHASE 2 components ONLY. P1 done. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer 5. TabInventaire ONLY.
Reference TabAventures READ-ONLY. Do NOT edit .cs. No RectMask2D on NavBarContainer.

SPRITE LOCK (mandatory): TabInventaire/IconLift/Icon Image → ONLY
Assets/Art/Sprites/UI/Inventory/InventaireIconeBagPack.png
Do NOT use IconeInventaire.png or any other sprite.

DO NOT change TabInventaire/SelectedFrame (keep anchoredPosition 0,0 sizeDelta -4,-4).
DO NOT copy TabAventures SelectedFrame +10 X.

1) TabInventaire/IconLift/Glow — ADD Image if missing. Match TabAventures/IconLift/Glow:
   RT anchor center (0.5,0.5), pivot center, pos (0,8), sizeDelta 108x108.
   Image: UISprite white, Material Assets/Materials/UI/NavTabSoftGlow.mat,
   color RGB(1,0.78,0.2) alpha 0.4, Preserve Aspect ON, Raycast OFF. GO OFF default.

2) TabInventaire/IconLift/Icon: stretch (0,0)-(1,1), sizeDelta (-8,-38), Preserve Aspect ON, white, Raycast OFF.
   Sprite MUST stay Assets/Art/Sprites/UI/Inventory/InventaireIconeBagPack.png — do NOT reassign.

3) TabInventaire/Label TMP: text "Inventaire". Copy TabAventures/Label RT: bottom stretch, pivot (0.5,0), pos (0,10), height 32.
   fontSize 31, Bold, face RGB(1,0.78,0.2), Outline black width ~0.28, Raycast OFF, GO OFF default.

4) TabInventaire/ActiveLava: GO OFF (no Image required). Border children on SelectedFrame: leave as-is, OFF when inactive (script later).

5) All mockup FX (Glow, Label, ActiveLava) OFF by default.

Save. List paths changed. STOP. No Play Mode.
```

### Phase 3 — Parité comportement TabAventures (scène + C#) — **prêt à coller**

`@Notes/Bezi/RULES_bezy_code.md` — C# visuel autorisé sur `NavigationHUD.cs` uniquement.

```
[BZ-TAB-INVENTAIRE-MOCKUP-001] PHASE 3 — Inventaire = same behavior as TabAventures (Play). STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. m_Layer 5.
Files ONLY: Assets/Scenes/NavigationHUD.unity, Assets/Scripts/UI/NavigationHUD.cs
Read TabAventures as SOURCE OF TRUTH. Do NOT rescan whole project.
No RectMask2D on NavBarContainer. No SceneNavigator/UIManager/popup changes.

SPRITE: TabInventaire/IconLift/Icon = Assets/Art/Sprites/UI/Inventory/InventaireIconeBagPack.png ONLY.

GOAL: Inactive = icon bottom-aligned, same visual size as Play. Active click = same zoom, lift, glow, label, frame expand as TabAventures (serre).

SCENE TabInventaire:
1) Mirror TabAventures hierarchy + REST RectTransforms: IconLift, Icon (-8,-38), Glow, Label, SelectedFrame (X=0 NOT +10), borders/WoodFrame like Aventures.
2) If backpack sits high vs Play: nudge Icon anchoredPosition Y only (small negative), keep IconLift at rest (0,0).

C# NavigationHUD.cs:
3) Ensure TabInventaire uses the SAME mockup path as TabAventures (lift, scale, glow, label, SelectedFrame expand). Reuse existing helpers/constants — duplicate wiring only if SerializeFields missing.
4) Match HUDRoot Inspector refs: tabInventaireIconLift, tabInventaireGlow, tabInventaireIcon → IconLift/Icon, label, selectedFrame, lava optional.
5) Do NOT change shop/vente tabs. Do NOT refactor unrelated code.

Save. List scene paths + .cs methods/fields touched. STOP. No Play Mode.
```

---

## Phase 1 — Shell / hiérarchie sprites (legacy — autres onglets)

```
(à rédiger après brief auteur — ne pas envoyer)
```

---

## Phase 2 — Composants visuels (Images / sprites)

```
(à rédiger après brief auteur — ne pas envoyer)
```

---

## Phase 3 — Wiring SerializeField / polish

```
(à rédiger après brief auteur — ne pas envoyer)
```

---

## Pilotes livrés (onglet par onglet)

| Onglet | Sprite | Statut |
|--------|--------|--------|
| `TabAventures` | `IconePlay.png` | OK pilote |
| `TabInventaire` | `InventaireIconeBagPack.png` (`Sprites/UI/Inventory/`) | OK mockup 2026-09-13 |
| `TabShop` | `IconeMarket.png` | OK pilote |
| `TabVente` | `GoldBill.png` (V0 — remplacer par `IconeVente.png` quand promu) | **prompt ci-dessous** |

---

## Pilote 4 — TabVente (copier-coller Bezy)

**Prérequis :** Trim `GoldBill.png` dans Sprite Editor. Ouvrir `NavigationHUD.unity`.

```
[BZ-TAB-SPRITES-001] PILOT TabVente — copy TabAventures icon layout. Wait success. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. Layer UI = 5.
Do NOT modify C#. Do NOT change TabAventures, TabInventaire, TabShop.

File ONLY: Assets/Scenes/NavigationHUD.unity
Reference: TabAventures / TabInventaire / TabShop.

Target: TabVente ONLY (sale channels tab).

1) REMOVE VerticalLayoutGroup from TabVente.
2) COPY structure from TabAventures:
   - SelectedFrame (inactive default) + BorderTop/Bottom/Left/Right (3px), stretch -4,-4
   - Border color RGB(1, 0.78, 0.2) alpha 1
   - Icon: stretch anchors (0,0)-(1,1), sizeDelta -7,-7
   - Sprite: Assets/Art/Sprites/UI/Currency/GoldBill.png
   - Image: white, Preserve Aspect ON, Raycast OFF
3) Label: INACTIVE.
4) Wire NavigationHUD on HUDRoot:
   - tabSaleChannelsIcon → TabVente/Icon
   - tabSaleChannelsSelectedFrame → TabVente/SelectedFrame
5) KEEP Animator NavTab, Button Animation, OnClick OnTabSaleChannelsClicked.

Do NOT restore wallet UI (known regression — separate task).

Save. List changes. STOP.
```

---

## Checklist review (après livraison Bezy)

- [x] `TabAventures` — sprite + cadre
- [x] `TabInventaire` — sprite + cadre
- [x] `TabShop` — sprite + cadre
- [x] `TabVente` — sprite + cadre (playtest 2026-09-23)
- [x] Playtest navigation 5 onglets (2026-09-23)
- [ ] Variantes tab 128² — `Notes/Art/PROMPT_generation_icones.md` § **Vague H-nav** (H-nav-1 → 4)
- [ ] Trim sprite par onglet (lisibilité) — ou remplacer par H-nav après promo
- [ ] `[P0-NAV-WALLET-REG-001]` restaurer wallet barre nav
