# [BZ-FARM-MOBILE-SCALE-001] FirstLvl — cadrage biofiltre mobile (~−10 %)

**Branche :** `rework/biofilter-mobile-scale` (créer avant Bezy)  
**Task IDs :** `[P0-FARM-MOBILE-BIOFILTER-SCALE-001]` · croix liée `[P0-NAV-EXIT-ANCHOR-001]`  
**Comparatif :** `Docs/ScreenPlaytestMobile/README_comparatif_batch-H.md`  
**Cible visuelle :** `Docs/ScreenPlaytestMobile/20260916_REF_p0-farm-mobile-biofilter-scale_firstlvl-target-editor-free-aspect.png`  
**KO mobile :** `Docs/ScreenPlaytestMobile/20260916_SM-A137F_p0-farm-mobile-biofilter-scale_firstlvl-biofiltre-oversized-no-exit.jpg`

**Aujourd’hui (FirstLvl) :** `Main Camera` orthographic **size = 5** ; instance `Biofiltre` position scène **(−2,75 ; 2,98 ; 0)** ; scale racine **1,1,1**.  
**Ne pas** toucher la logique `BiofiltreManager` / popup / grille.

**Succès Bezy :** `Save. List what changed. STOP.` — **pas** Simulate / Play Mode.

**Une phase par appel.** Après Phase 1, l’auteur compare Game view **1080×1920** à la REF avant Phase 2.

---

## Phase 1 — Scène `FirstLvl` (monde + caméra)

```
[BZ-FARM-MOBILE-SCALE-001] Phase 1 ONLY — FirstLvl camera framing ~10% smaller biofiltre. STOP.

OPEN Assets/Scenes/FirstLvl.unity FIRST. Do NOT rescan whole project. Do NOT edit .cs.
Do NOT edit Assets/Prefabs/World/Biofiltre.prefab (scene overrides OK on the instance only).
Do NOT edit NavigationHUD.unity in this phase.

GOAL: Match author REF (Docs/ScreenPlaytestMobile/20260916_REF_*_target-editor-free-aspect.png):
biofiltre fully on screen with visible margins top/left/right (room for HUD overlay croix + PA widget).

1) Main Camera (orthographic):
   - Keep orthographic ON. Start from size 5 → increase toward ~5.45–5.55 (≈10% smaller subject). Pick ONE value that best matches REF at 1080x1920 Game view.
   - Keep camera position/rotation unless a small Y nudge centers the cuve vertically.

2) Biofiltre (prefab instance in scene, root Transform):
   - Prefer camera-only change. If still too large vs REF, uniform scale root to 0.90–0.95 (XYZ equal) on the INSTANCE override only — not the prefab asset.
   - Keep grid + IbcSprite + nested BiofiltreHud children aligned (scale parent root only).

3) Do NOT change: extraScale on BiofiltreIbcSpriteFitter, deck UV, GridManager cell size, HUD nested prefab hierarchy.

Save scene. Reply: final orthographic size, Biofiltre instance scale (if changed), camera position. STOP. No Play Mode.
```

**Lancement Unity :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_biofilter_mobile_scale.md
@Docs/ScreenPlaytestMobile/20260916_REF_p0-farm-mobile-biofilter-scale_firstlvl-target-editor-free-aspect.png
[BZ-FARM-MOBILE-SCALE-001] Phase 1 ONLY — FirstLvl camera framing ~10% smaller biofiltre. STOP.
```

---

## Phase 2 — Scène `NavigationHUD` (croix ExitOnly visible portrait)

```
[BZ-FARM-MOBILE-SCALE-001] Phase 2 ONLY — ExitButtonContainer safe top-left portrait. STOP.

OPEN Assets/Scenes/NavigationHUD.unity FIRST. UI m_Layer = 5. Do NOT rescan. Do NOT edit .cs.
Do NOT change NavBarContainer / tab layout.

GOAL: Red X exit (ExitButtonContainer) visible on 1080x1920 portrait like REF — not off-screen on phone.

1) ExitButtonContainer RectTransform:
   - Anchor TOP-LEFT (0,1) pivot (0,1). Remove center-anchor + large pixel offsets (-506, 861).
   - Position with safe margin: ~16–24 px from top-left (author tune: visible, not under notch).
   - Keep child Button/Image/TMP wiring intact.

2) Do NOT disable ExitButtonContainer. Do NOT move currency HUD unless it overlaps the exit button.

Save scene. Reply: anchor min/max, anchoredPosition, pivot. STOP. No Play Mode.
```

**Lancement Unity :**

```
@Assets/Docs/Bezi/PROMPTS_Bezi_biofilter_mobile_scale.md
[BZ-FARM-MOBILE-SCALE-001] Phase 2 ONLY — ExitButtonContainer safe top-left portrait. STOP.
```

---

## Après Bezy (Cursor, même branche)

- Si **APK SM-A137F** reste plus zoomé que Game view 1080×1920 : ajouter ajustement runtime caméra (aspect / safe area) sur `FirstLvl` — hors périmètre Bezy.
- Playtest : nouvelles captures `…_firstlvl-apres-fix-device.jpg` dans `Docs/ScreenPlaytestMobile/`.
- `UIManager` / `FirstLvlController` : pas de changement attendu pour le scale.
- **`[P0-UI-PA-HUD-TALENTTREE-001]`** — masquer `ActionPointsHudWidget` (`NavigationHUD.actionPointsHudRoot`) quand l’overlay arbre de talents inventaire est ouvert (recouvre les nœuds). **Cursor only**, pas Bezy.
