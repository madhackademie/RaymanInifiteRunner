# Prompts ChatGPT — refonte UI kit (bois + cartoon fun)

**Création :** 2026-09-19  
**Workflow assets :** `Notes/Art/WORKFLOW_creation_assets.md`  
**Contexte :** UI runtime éloignée de la charte → refaire le **chrome** (panneaux, boutons, barre nav) puis implémentation **Bezy** (prefabs / 9-slice).  
**Charte :** `Notes/Art/NOTE_graphique.md` (monde iso ≠ **UI bois rustique** § `PROMPT_generation_icones.md` §1) · mockup nav `Notes/Ui/SPEC_nav_onglets_zoom_actif.md` · bandeau `FirstTryBandeauAtelier` / §1 icônes.  
**Dump :** `Assets/Art/Assets Store Dump/Ui/KitRefonte/`  
**Promo :** `Assets/Art/Sprites/UI/Kit/` (après OK auteur)  
**Bezy :** wiring + 9-slice Inspector — prompts séparés après promo (`PROMPTS_Bezi_nav_wood_frame_slice.md` comme modèle).

---

## 1) Planche UI : oui, mais **pas** une seule image fourre-tout

| Approche | Conversion Bezy | Recommandation |
|----------|-----------------|----------------|
| **1 PNG = 1 chrome** (panneau, bouton, onglet…) | **Haute** — 9-slice propre | **Prod** |
| **Planche ortho** (grille fixe, fond transparent) | **Moyenne** — découpe manuelle / Photopea | **Moodboard + batch v1** |
| **1 mockup écran complet** | **Basse** — perspective, ombres, pas 9-slice | **Direction seulement** |

ChatGPT **peut** générer une planche **si** tu imposes : vue **plate** (pas perspective), **contours droits** sur les panneaux, **cellules séparées** par gouttière transparente, **aucun texte** dans les assets (TMP Unity).

**Éviter :** une planche avec barre nav + inventaire + popups en perspective — impossible à slicer proprement.

---

## 2) STYLE LOCK (coller en tête de **chaque** prompt ChatGPT)

Joindre **1 ref** : `FirstTryBandeauAtelier.png` **ou** mockup bois `Dump/Ui/cadreBoisFinal.png` **ou** icône §1.

```
STYLE LOCK — RaymanFarm UI (strict):
- 2D casual mobile game UI, cozy aquaponic farm, FUN cartoon (chunky, playful, not corporate flat).
- Rustic LIGHT BROWN WOOD panels: visible grain, carved edges, small nail heads in corners, optional tiny fish/leaf/carrot motif (subtle, not noisy).
- Thick dark brown OUTLINES on interactive elements; smooth cel shading; warm saturated accent colors (teal, orange, gold highlights).
- NOT photoreal, NOT 3D render, NOT glassmorphism, NOT minimal flat Material, NOT gray stone bar (old nav) unless explicitly asked.
- NOT isometric. Flat UI orthographic front view only.
- Background: fully TRANSPARENT outside each UI shape (alpha PNG).
- NO text, NO letters, NO numbers, NO UI labels baked in — Unity uses TMP.
- 9-SLICE FRIENDLY: straight vertical/horizontal border zones (~18% of width/height), center fill plain enough for Sliced Image.
```

---

## 3) Ordre de génération (max conversion Comfy → ChatGPT → Bezy)

1. **P0 — Style lock** (1 image) : 1 panneau + 1 bouton sur fond transparent — valider bois + fun.  
2. **Chrome modulaire** (1 asset / prompt ou planche §4).  
3. **Repasse ChatGPT** si sortie Comfy : image1 = layout, image2 = ref style lock, « unify only » (`GUIDE_comfy_flux_models_local.md`).  
4. **Promo Sprites** + **Sprite Editor** borders (Cursor doc, auteur ou Bezy tuning).  
5. **Bezy** : `/prefab-ui-3phases` ou prompts type `PROMPTS_Bezi_nav_wood_frame_slice.md`.

---

## 4) Prompt — planche ortho (grille 2×3, concept + découpe)

**Sortie cible :** 2048×2048 PNG transparent, **6 cellules** ~640×640 avec **80 px gutter** transparent entre cellules.

```
Create ONE UI sprite sheet, square 2048x2048, transparent background outside shapes.

LAYOUT: strict 2 columns x 3 rows grid, equal cells, 80px empty transparent gutters between cells. Flat orthographic UI only. Apply STYLE LOCK.

Row1 Col1: NAV BAR strip — wide horizontal wooden dock, dark stone REPLACED by warm wood plank, subtle rope inlay, empty center for icons (no icons drawn).
Row1 Col2: TAB ACTIVE FRAME — chunky carved wood frame with gold inner rim, empty center hole for icon, swallowtail or rounded top, fun cartoon nails.

Row2 Col1: PANEL 9-SLICE — large rectangular parchment-on-wood panel, straight borders for Unity slicing, empty center.
Row2 Col2: PRIMARY BUTTON — big fun cartoon button (teal or orange), beveled wood rim, glossy highlight, pill shape, empty face for TMP.

Row3 Col1: SECONDARY BUTTON — smaller wood button, rope border, friendly shape.
Row3 Col2: SCROLL / LIST ROW chrome — horizontal wooden list row bar, lighter center strip, 4:1 wide rectangle in cell.

NO text anywhere. NO mock screen. Each cell ONE asset centered with padding inside its cell.
```

Après génération : Photopea / GIMP — découper 6 PNG → nommer `UiKit_NavBar_9s.png`, `UiKit_TabFrame_active.png`, etc.

---

## 5) Prompts — 1 asset / image (prod, meilleur pour Bezy)

### 5.1 Panneau fond (9-slice)

```
One UI asset only. STYLE LOCK. Horizontal wooden panel 1280x320, 4:1, transparent outside. Light tan parchment center, thick rustic wood border left/right/top/bottom (~18% straight beams). Small corner nails. Empty center for TMP. 9-slice friendly. PNG.
```

### 5.2 Bouton primary (fun cartoon)

```
One UI asset only. STYLE LOCK. Large cartoon PRIMARY button, landscape ~480x160, pill/rounded rectangle. Fun: slight bounce shape, orange or teal fill, wooden rim, tiny highlight gleam, optional small carrot or fish silhouette carved in wood corner (tiny). Empty center for label. Transparent PNG. Front view.
```

### 5.3 Bouton secondary / close chip

```
One UI asset only. STYLE LOCK. Smaller secondary button ~320x120, rope-stitched wood, softer color. OR circular close chip 128x128 wood with X carved (no letter — geometric X lines only). Transparent PNG.
```

### 5.4 Cadre onglet actif (nav)

```
One UI asset only. STYLE LOCK. Tab selected frame ~512x640 portrait, carved wood, gold inner edge, empty center for icon, bottom area NOT covered (label is TMP below). Match fun farm nav, not stone slab. Transparent PNG. 9-slice or preserve aspect in Unity.
```

### 5.5 Bandeau liste (vente / hub)

```
One UI asset only. STYLE LOCK. Same grammar as SaleChannelBandeau: wide bar 1280x320, wood+parchment, left 22% darker strip optional, center clean for title+stars+80px vignette. No stars drawn. Transparent PNG. 9-slice borders straight.
```

---

## 6) Implémentation Bezy (après art OK)

| Étape | Qui | Quoi |
|-------|-----|------|
| Import + 9-slice | Auteur / Bezy | Sprite Editor borders → doc dans prompt Bezy |
| Nav barre | Bezy | `NavigationHUD.unity` — remplacer fond pierre par `UiKit_NavBar_9s` Sliced |
| Onglets | Bezy | `WoodFrame` / `SelectedFrame` — `PROMPTS_Bezi_nav_wood_frame_slice.md` |
| Écrans | Bezy | Remplacer fonds `InventoryScreen`, `ShopScreen`, etc. — **une prefab / phase** |

Cursor : **prompts + spec**, pas prefab YAML (règle Bezy ownership).

---

## 7) Checklist avant promo Sprites

- [ ] Bords 9-slice droits testés dans Unity (Image Sliced, pas de stretch moche).  
- [ ] Alpha propre (pas de halo blanc sur HUD noir — cf. ComingSoon).  
- [ ] Aucun texte dans le PNG.  
- [ ] Même palette bois que bandeau atelier / icônes §1.  
- [ ] Boutons **fun** validés auteur (pas trop enfantin / pas trop sérieux).

---

## 8) ID tâche suggéré

`[P0-UI-KIT-REFONTE-001]` — style lock → chrome modular → Bezy nav → écrans modales.
