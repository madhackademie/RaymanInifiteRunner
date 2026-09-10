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

### Phase 2 — Composants visuels

```
[BZ-TAB-SPRITES-001] PILOT TabAventures — PHASE 2 components ONLY. Phase 1 must be done. STOP after Save.

OPEN NavigationHUD.unity. Layer UI = 5. File ONLY this scene. TabAventures ONLY.

1) Glow: Add Image. Use default UI Sprite (Knob) or soft circle. Color RGB(1, 0.78, 0.2) alpha 0.35. Raycast OFF. GameObject INACTIVE by default.

2) Icon (under IconLift): Image white, Preserve Aspect ON, Raycast OFF. Stretch anchors (0,0)-(1,1), sizeDelta -8,-8. Do NOT replace author sprite.

3) Label: Add or enable TextMeshProUGUI. Text "Aventures". Font size 22, bold, color RGB(1, 0.78, 0.2). Anchor bottom center of tab, height ~22, width stretch with 4px side padding. GameObject INACTIVE by default. Raycast OFF.

4) SelectedFrame: keep gold borders 3px, inactive by default. No color tint on Icon.

5) NavBarContainer: set RectTransform height from 120 to 128 if safe (sizeDelta y=128). Only if no layout break.

Save. List components. STOP. No Simulate.
```

### Phase 3 — Wiring Inspector — **livré 2026-09-10**

Cursor : champs `tabAventuresIconLift` / `Glow` / `Label` + logique zoom dans `NavigationHUD.cs` (câblage scène sur `HUDRoot`).

```
[BZ-TAB-SPRITES-001] PILOT TabAventures — PHASE 3 wiring ONLY. STOP after Save.

OPEN NavigationHUD.unity. Do NOT edit C#. TabAventures ONLY.

On HUDRoot NavigationHUD component, wire EXISTING fields only:
- tabAventuresIcon → TabAventures/IconLift/Icon (Image)
- tabAventuresSelectedFrame → TabAventures/SelectedFrame

Do NOT add new script fields. Do NOT wire Glow, IconLift, or Label to NavigationHUD (Cursor will add SerializeFields later).

KEEP tabAventuresButton → TabAventures Button. KEEP Animator + NavTab controller.

Save. List wired references. STOP.
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
