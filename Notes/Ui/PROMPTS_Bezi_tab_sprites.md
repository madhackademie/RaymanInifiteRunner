# Prompts Bezy — sprites onglets HUD `[BZ-TAB-SPRITES-001]`

**Statut :** **pivot 2026-09-10** — mockup zoom actif + glyphes simples (`Notes/Ui/SPEC_nav_onglets_zoom_actif.md`). Anciens pilotes iso = référence hiérarchie seulement ; **nouvelle passe Bezy** après promo sprites H-nav mockup.

### Régression connue (2026-09-09)

- **Wallet / solde gold** en haut de la barre nav (`NavigationHUD`) **absent** après passes Bezy onglets — **à corriger plus tard** (pas bloquant ce prompt).
- ID suivi : `[P0-NAV-WALLET-REG-001]` dans `Notes/Todo_project.md`.

**Tâche session :** `[P0-TAB-SPRITES-001]` — mise en place + polish des sprites sur les onglets.  
**Job Bezy :** `[BZ-TAB-SPRITES-001]` — `/prefab-ui-3phases`  
**Skill :** `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**File :** `Notes/Bezi/BEZY_QUEUE.md`  
**Art (hors Bezy) :** Vague H `Notes/Art/PROMPT_generation_icones.md` — Dump → promo auteur `Sprites/UI/` **avant** wiring.  
**Hub Plus (futur) :** `Notes/Ui/SPEC_features_hub_plus.md` — variante barre A (3+Plus) vs B (4+Plus) après playtest mobile.

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
- Scripts : Cursor étend `NavigationHUD` (`IconLift`, glow, label) — Bezy **ne modifie pas** le C#
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

**Prérequis :** glyphe sac **sans texte ni cadre** dans le PNG (`Sprites/UI/Nav/Tabs/`).  
**Cursor :** wiring glow/lift Inventaire dans `NavigationHUD.cs` **après** Bezy P1–P3 (champs à ajouter).

### Phase 1 — Hiérarchie

```
[BZ-TAB-INVENTAIRE-MOCKUP-001] PHASE 1 hierarchy ONLY. STOP.

OPEN NavigationHUD.unity. m_Layer 5. TabInventaire ONLY. Do NOT edit C#.

Mirror TabAventures structure:
SelectedFrame (keep) → CREATE ActiveLava → CREATE IconLift → Label (reuse or create).

MOVE Icon to TabInventaire/IconLift/Icon. KEEP sprite on Image.

IconLift: stretch fill, pos 0, sizeDelta 0. Glow empty RT under IconLift later (phase 2). Label sibling last.

Save. List hierarchy. STOP.
```

### Phase 2 — Composants (copie Aventures)

```
[BZ-TAB-INVENTAIRE-MOCKUP-001] PHASE 2 components. P1 done. STOP.

OPEN NavigationHUD.unity. TabInventaire ONLY. Copy numeric/layout from TabAventures/ActiveLava, IconLift/Glow, Label.

ActiveLava + Glow materials: NavTabLavaBackdrop.mat, NavTabSoftGlow.mat (same paths as Aventures).
Label text "Inventaire", same TMP style (24 bold, yellow, black outline).
Icon sizeDelta (-8,-34). All FX GO OFF by default.

Save. STOP. No Simulate.
```

### Phase 3 — Wiring (champs existants)

```
[BZ-TAB-INVENTAIRE-MOCKUP-001] PHASE 3 wiring. STOP.

OPEN NavigationHUD.unity. Do NOT edit C#.

HUDRoot NavigationHUD:
tabInventaireButton, tabInventaireIcon → IconLift/Icon,
tabInventaireSelectedFrame, tabInventaireLabel → Label TMP.

Do NOT wire Glow/Lava/IconLift yet (Cursor SerializeFields next).

Save. List refs. STOP.
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
| `TabInventaire` | `IconeInventaire.png` | OK pilote |
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
- [ ] `TabVente` — sprite + cadre
- [ ] Variantes tab 128² — `Notes/Art/PROMPT_generation_icones.md` § **Vague H-nav** (H-nav-1 → 4)
- [ ] Trim sprite par onglet (lisibilité) — ou remplacer par H-nav après promo
- [ ] Playtest navigation 4 onglets
- [ ] `[P0-NAV-WALLET-REG-001]` restaurer wallet barre nav
