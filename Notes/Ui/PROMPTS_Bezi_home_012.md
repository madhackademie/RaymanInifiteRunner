# Prompts Bezy — HomeScene hub lisibilité `[BZ-POLISH-012]`

**Scène :** `Assets/Scenes/HomeScene.unity`  
**Prefab boutons :** `Assets/Prefabs/MapNodeButton.prefab`  
**Scripts (ne pas modifier) :** `MapSceneController.cs`, `MapNodeButton.cs`  
**Layers :** UI = `m_Layer: 5` — `Notes/Ui/CONVENTION_layers_unity.md`  
**Hors scope :** NavigationHUD, art splash Loading, MapNodeData assets, C#

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

**Contexte :**
- Hub spawn runtime des `MapNodeButton` sous `Canvas/NodesContainer` (VerticalLayoutGroup).
- Prefab encore en **layer 0** ; titres petits (Label 28 / Subtitle 20) ; hauteur bouton 80.
- Scène : Canvas/Background/NodesContainer déjà layer 5 ; **pas encore** de header « ACCUEIL ».

**SerializeFields à préserver :**
- Prefab : `backgroundImage`, `lockIcon`, `label`, `subtitleLabel`
- Scène : `MapSceneController.nodesContainer` → `NodesContainer` ; `nodeButtonPrefab` → `MapNodeButton`

**Ordre :** Ph.1 → Ph.2 → Ph.3 (attendre succès avant la suivante).

---

## Phase 1 — Layers MapNodeButton UI = 5 — **OK** (Bezy 2026-08-05)

```
[BZ-POLISH-012] Phase 1 ONLY — MapNodeButton layers UI=5. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not rename MapNodeButton / Label / SubtitleLabel / LockIcon.
Keep MapNodeButton SerializeFields wired: backgroundImage, lockIcon, label, subtitleLabel, unlockedColor, lockedColor.
Keep Button + MapNodeButton components on root.

File ONLY:
- Assets/Prefabs/MapNodeButton.prefab

REQUIRED:
1) Set m_Layer: 5 on root MapNodeButton AND every child (Label, SubtitleLabel, LockIcon).
2) Layer UI = m_Layer: 5 (TagManager Water=4, UI=5). Never use layer 4 for UI.
3) Do not change RectTransforms, fonts, colors, sprites, or layout this phase.
4) Save prefab.

Interdits: no C#; no Simulate/Play Mode; do not edit HomeScene this phase; do not break SerializeFields.

Done = Save. List each GO name + m_Layer. Confirm SerializeFields still assigned. STOP.
```

**Chars ~887** — OK &lt; 3500.

### Checklist review Phase 1 (Cursor)

- [x] Root + Label + SubtitleLabel + LockIcon = `m_Layer: 5`
- [x] SerializeFields intacts (`backgroundImage`, `lockIcon`, `label`, `subtitleLabel`)

---

## Phase 2 — Bouton mobile + titres — **OK** (Bezy 2026-08-05)

```
[BZ-POLISH-012] Phase 2 ONLY — MapNodeButton mobile hit + title readability. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not rename nodes.
Keep SerializeFields: backgroundImage, lockIcon, label, subtitleLabel. Keep Button + MapNodeButton on root.
Do not change unlockedColor/lockedColor script defaults unless Image contrast too low — then set unlockedColor RGB(0.22,0.62,0.28) a=1 ; lockedColor RGB(0.35,0.35,0.38) a=0.85.

File ONLY: Assets/Prefabs/MapNodeButton.prefab

REQUIRED:
1) Root RectTransform sizeDelta height >= 112 (keep width ~300 OR leave width; VerticalLayoutGroup controls width in scene). Add LayoutElement if missing: preferredHeight=112, minHeight=112, flexibleWidth=1.
2) Label (TMP): fontSize 36 ; Bold ; color white a=1 ; horizontal center ; vertical middle ; raycastTarget=false ; left/right margin ~16 via TMP margin or Rect inset.
3) SubtitleLabel (TMP): fontSize 22 ; color RGB(0.92,0.92,0.94) a=0.9 ; height >= 28 ; anchored near bottom inside button ; raycastTarget=false.
4) LockIcon: size >= 44x44 ; top-right or right-center ; raycastTarget=false ; visible only when locked (leave inactive logic to script — do not force Active).
5) Root Image (background): raycastTarget=true (hit area) ; assign built-in UISprite if sprite null (solid color OK).
6) m_Layer: 5 unchanged. Save.

Interdits: no C#; no Simulate; no HomeScene edits; no new scripts/Animator.

Done = Save. List root height + Label fontSize + Subtitle fontSize + LockIcon size + layers. STOP.
```

**Chars ~1526** — OK &lt; 3500.

### Checklist review Phase 2 (Cursor)

- [x] Hauteur 112 + LayoutElement min/preferred 112
- [x] Label 36 Bold blanc ; Subtitle 22 `(0.92,0.92,0.94) a=0.9`
- [x] LockIcon 44² ; root raycast OK ; layers 5 ; SerializeFields OK
- [ ] Note : sprite Image root encore `null` possible — OK Editor ; UISprite si build noir

---

## Phase 3 — Header ACCUEIL + padding hub — **OK** (Bezy 2026-08-05)

> **Note Bezy :** écrire `HomeScene.unity` exige la scène **ouverte** dans l’Editor (sinon writes « OK » sans persist). Documenté ici.

```
[BZ-POLISH-012] Phase 3 ONLY — HomeScene header title + container safe padding. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate MapManager / MapSceneController wiring.
Keep MapSceneController refs: allNodes, progressionData, nodeButtonPrefab, nodesContainer intact.
Do not edit MapNodeButton.prefab this phase (Ph.2 done).

Scene ONLY: Assets/Scenes/HomeScene.unity

REQUIRED:
1) Under Canvas create TMP child HeaderTitle if missing (sibling above NodesContainer, after Background).
2) HeaderTitle: text "ACCUEIL" ; fontSize 48 Bold ; color white a=1 ; center top — anchorMin(0,1) Max(1,1) pivot(0.5,1) ; height 72 ; anchoredPos Y=-48 ; raycastTarget=false ; m_Layer: 5.
3) Sibling order under Canvas: 0=Background, 1=HeaderTitle, 2=NodesContainer.
4) NodesContainer VerticalLayoutGroup padding: Left/Right 48 ; Top 140 (clear header) ; Bottom 120 (clear NavigationHUD). Spacing 20. Keep ContentSizeFitter vertical preferred.
5) NodesContainer stretch full Canvas ; m_Layer: 5. Background stretch full ; color RGB(0.07,0.07,0.12) a=1 ; UISprite if null ; raycastTarget=false.
6) Confirm Canvas + children m_Layer: 5. MapManager stays layer 0. Save scene.

Interdits: no C#; no Simulate/Play Mode; do not change MapNode Data assets; do not break nodesContainer SerializeField.

Done = Save. List Canvas children order + HeaderTitle fontSize + NodesContainer padding + MapSceneController.nodesContainer OK. STOP.
```

**Chars ~1457** — OK &lt; 3500.

### Checklist review Phase 3 (Cursor)

- [x] `HeaderTitle` « ACCUEIL » 48 Bold, Y=-48, h=72, layer 5, raycast off
- [x] Sibling order Canvas : Background → HeaderTitle → NodesContainer
- [x] Padding L/R 48 · Top 140 · Bottom 120 · spacing 20
- [x] Background `(0.07,0.07,0.12)` ; `nodesContainer` + `nodeButtonPrefab` refs OK
- [x] Playtest auteur → **[P0-HOME-PLAY-012]** — **OK 2026-08-18**

---

## Après Bezy (auteur)

1. Playtest : Bootstrap → Home — titre ACCUEIL + bouton « Commencer l'aventure » lisible / tap confortable.
2. Marquer `[BZ-POLISH-012]` clos dans `Notes/Todo_project.md` + `PROJECT_LOG.md`.
3. Suite file : `#13` audit layers.
