# Bezy — HUD PA ×2 (lisibilité)

**Task ID :** `[BZ-AP-HUD-SCALE-001]` · **P0 :** `[P0-AP-HUD-SCALE-001]`  
**Prefab :** `Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab`  
**Instance :** `Assets/Scenes/NavigationHUD.unity`  
**Décision auteur 2026-09-10 :** agrandir **2×** le chrome PA (capture `240 / 240` trop petit).  
**Convention :** `Notes/Ui/CONVENTION_hud_pa_safe_zone.md` (mettre à jour après Ph.3).

**Succès Bezy = Save + liste. STOP. Pas de Simulate.**  
Ne pas rescanner tout le projet. Ne pas modifier le C#. Ne pas ajouter / renommer / supprimer de GameObjects. Ne pas unpack. Layer UI = `m_Layer: 5`.

Ancien → nouveau (×2) :

| Élément | Avant | Après |
|---------|-------|-------|
| Root `sizeDelta` | 240 × 60 | **480 × 120** |
| Icon LayoutElement | 32 × 32 | **64 × 64** |
| Barre hauteur | 6 | **12** |
| PointsLabel | 18 | **36** |
| SubtitleLabel | 12 | **24** |

Ancres **inchangées** : top-right `(1,1)`, pivot `(1,1)`, `anchoredPosition (-16, -16)`.

---

## Phase 1 — Layout RectTransform (prefab only)

```
[BZ-AP-HUD-SCALE-001] Phase 1 ONLY — scale ActionPointsHudWidget x2 (layout). Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not add/rename/delete GameObjects. Do not unpack. Layer 5. Keep top-right anchors.

File ONLY:
- Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab

REQUIRED (exact 2x values):
1) Root ActionPointsHudWidget RectTransform:
   keep anchorMin/Max (1,1), pivot (1,1), anchoredPosition (-16,-16)
   sizeDelta (480, 120)
2) Row HorizontalLayoutGroup: padding Left/Right/Top/Bottom = 20 ; spacing = 16
3) Row RectTransform: keep stretch parent ; sizeDelta (0, -12) ; anchoredPosition (0, 6)
4) Icon LayoutElement: MinWidth 64, MinHeight 64, PreferredWidth 64, PreferredHeight 64
5) ProgressBar RectTransform: keep bottom stretch (anchorMin 0,0 / anchorMax 1,0 / pivot 0.5,0) ; sizeDelta (0, 12)
6) TextColumn VerticalLayoutGroup spacing = 4

Save. List root sizeDelta, Icon preferred 64, ProgressBar height, Row padding. STOP.
```

---

## Phase 2 — TMP + tooltip (prefab only)

```
[BZ-AP-HUD-SCALE-001] Phase 2 ONLY — TMP + tooltip x2. Wait success. STOP after save.

PREREQ: Phase 1 saved. Do not rescan whole project. Do not modify C#. Do not add/rename GOs. Auto-size OFF.

File ONLY:
- Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab

REQUIRED:
1) PointsLabel TMP: fontSize 36, fontSizeBase 36, fontSizeMin 36
2) SubtitleLabel TMP: fontSize 24, fontSizeBase 24, fontSizeMin 24
3) FatigueTooltipPanel VerticalLayoutGroup: padding L/R=16 T/B=12 ; spacing=6
4) TitleLabel TMP: fontSize 22, fontSizeBase 22
5) BodyLabel TMP: fontSize 20, fontSizeBase 20
6) BodyLabel LayoutElement PreferredWidth 360
7) ActionPointFatigueTooltipHost screenOffset (0, 24)

Keep colors, wiring ActionPointsHudView, Animator Spend/Refuse. Keep FatigueTooltipPanel inactive.

Save. List the 4 font sizes + tooltip padding + screenOffset. STOP.
```

---

## Phase 3 — Instance scène NavigationHUD

```
[BZ-AP-HUD-SCALE-001] Phase 3 ONLY — scene instance sizeDelta 480x120. Wait success. STOP after save.

PREREQ: open Assets/Scenes/NavigationHUD.unity in Editor before running.
Do not rescan whole project. Do not modify C#. Layer 5. Keep top-right. Do not move wallet or tabs.

Files:
- Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab
- Assets/Scenes/NavigationHUD.unity (instance ActionPointsHudWidget)

REQUIRED:
1) Prefab root still sizeDelta (480, 120), anchors (1,1), pivot (1,1), pos (-16,-16)
2) NavigationHUD instance: sizeDelta x=480 y=120 (replace old 240/60 overrides)
3) Instance anchoredPosition stays (-16,-16)

Save. List prefab + scene sizeDelta. STOP.
```

---

## Checklist auteur (après Ph.3, hors Bezy)

- [ ] Playtest : `240 / 240` lisible tablette, barre 3 zones + overlay conso OK
- [ ] Tooltip hover Comfort / Caution / Fatigue encore sous le widget
- [ ] Pas de collision avec onglets bas ; wallet `[P0-NAV-WALLET-REG-001]` à revoir si overlap
