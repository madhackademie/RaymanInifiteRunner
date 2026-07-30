# Prompts Bezy — TreeMount plein cadre + HUD PA haut-droite `[CT-UI-SAFE-PA-001]`

**Note (2026-07-30) :** livré **Cursor** (demande auteur « implement the plan ») — prefabs / scène / runtime alignés. Archive pour rejeu Bezy si besoin.

**Succès Bezy = Save + liste. STOP. Pas de Simulate.**

---

## Phase 1 — TreeMount plein cadre (archive / rejeu)

```
[CT-UI-SAFE-PA-001] Phase 1 ONLY — TreeMountHost full OverlayPanel + sibling draw order. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Keep Filigrane / FiligraneBackdrop / treeMountHost / track.commerce.

File ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab

REQUIRED:
1) TreeMountHost: stretch parent ; sizeDelta (0,0) OR offsetMin/Max (0,0).
2) OverlayPanel Image alpha = 1.0 (color RGB 0.15/0.13/0.20).
3) Sibling order under OverlayPanel:
   - 0 TreeMountHost
   - then BodyPlaceholder / TreeScrollView
   - TrackTitle and BackButton LAST (draw above backdrop)
4) Keep FiligraneBackdrop then Filigrane under TreeMountHost.
5) Save.

Done = Save. List TreeMountHost sizeDelta + OverlayPanel children order. STOP.
```

---

## Phase 2 — HUD PA top-right (archive / rejeu)

```
[CT-UI-SAFE-PA-001] Phase 2 ONLY — ActionPointsHudWidget top-right. Wait success. STOP after save.

PREREQ: open Assets/Scenes/NavigationHUD.unity in Editor before running.
Do not rescan whole project. Do not modify C#. Keep size ~240x60. Layer 5.

Files:
- Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab
- Assets/Scenes/NavigationHUD.unity (instance overrides)

REQUIRED:
1) Prefab root RectTransform: anchorMin/Max (1,1) ; pivot (1,1) ; anchoredPosition (-16,-16) ; sizeDelta (240,60).
2) Apply same on NavigationHUD instance (clear conflicting overrides).
3) Save.

Done = Save. List anchors + position prefab + scene. STOP.
```
