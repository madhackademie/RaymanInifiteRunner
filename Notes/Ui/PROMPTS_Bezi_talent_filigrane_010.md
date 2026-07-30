# Prompts Bezy — Filigrane arbre talents Commerce `[BZ-POLISH-010]`

**Backlog lié :** `[BL-INV-TALENT-001]` · `[P0-ART-FILIGRANE-001]`  
**Sprite (déjà importé) :** `Assets/Art/Sprites/UI/Progression/CommerceFiligrane.png`  
**Prefab :** `Assets/Prefabs/Ui/InventoryScreen.prefab` → `TalentTreeOverlay/OverlayPanel/TreeMountHost`  
**Layers :** UI = `m_Layer: 5`  
**Ne pas rescanner tout le projet.** Pas de C#. Ne pas régénérer le sprite.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

**Contexte :** `TreeMountHost` existe et est wired (`[BZ-POLISH-009]`). Runtime instancie `Track_Commerce` **enfant** de `TreeMountHost` → ordre siblings : **0 = fond opaque**, **1 = Filigrane**, puis Track_* runtime.

**Hors scope :** contrastes nœuds, zoom/scroll, autres pistes, C#.

**Ordre :** Ph.1 OK → **Ph.2 fond opaque** (playtest : bleed inventaire derrière filigrane).

---

## Phase 1 — Filigrane Image — OK (archive)

```
[BZ-POLISH-010] Phase 1 ONLY — Filigrane Image under TreeMountHost. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate sprite.
Do not rename TreeMountHost / OverlayPanel / TalentTreeOverlay.
Do not touch TalentNodeView / Track_Commerce / edges.
Keep treeMountHost SerializeField wired.

Files:
- Assets/Prefabs/Ui/InventoryScreen.prefab
- Sprite: Assets/Art/Sprites/UI/Progression/CommerceFiligrane.png

REQUIRED:
1) Under OverlayPanel/TreeMountHost create UI child named Filigrane (Image) if missing.
2) RectTransform: stretch full TreeMountHost (anchors 0,0–1,1 ; offsets 0) ; pivot 0.5/0.5.
3) SetAsFirstSibling (index 0) so runtime Track_* draws above.
4) Image:
   - Source Image = CommerceFiligrane sprite
   - Preserve Aspect = true
   - Color white RGB (1,1,1) alpha 0.12
   - raycastTarget = false
5) m_Layer: 5 on Filigrane.
6) Do not add Mask / Button / LayoutGroup on Filigrane or TreeMountHost.
7) Save.

Interdits: no C#; no Simulate; no alpha > 0.20; do not clear trackPrefabBindings.

Done = Save. List Filigrane sibling index + Image sprite path + color RGBA + layer. STOP.
```

**Chars ~1250** — OK &lt; 3500.

### Checklist review Phase 1 (Cursor)

- [x] `Filigrane` sous `TreeMountHost`, sibling 0 (seul enfant)
- [x] Sprite `CommerceFiligrane` (`edb4a1cfea…`), Preserve Aspect, α 0.12, raycast off
- [x] Layer 5 ; stretch full host
- [x] `treeMountHost` + binding Commerce intacts

### Après Phase 1 (auteur)

- Playtest : filigrane OK mais **bleed inventaire** derrière → Ph.2 fond opaque.

---

## Phase 2 — Fond opaque derrière filigrane (COPIER TEL QUEL)

```
[BZ-POLISH-010] Phase 2 ONLY — opaque FiligraneBackdrop behind Filigrane. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate sprite.
Do not rename Filigrane / TreeMountHost. Keep Filigrane Image as-is (sprite + alpha 0.12).
Keep treeMountHost + trackPrefabBindings intact.

File ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab

REQUIRED:
1) Under TreeMountHost create UI child Image named FiligraneBackdrop if missing.
2) RectTransform: stretch full TreeMountHost (anchors 0,0–1,1 ; sizeDelta 0 ; pivot 0.5/0.5).
3) Sibling order STRICT:
   - index 0 = FiligraneBackdrop
   - index 1 = Filigrane
   (runtime Track_* will spawn after)
4) FiligraneBackdrop Image:
   - Sprite = Unity built-in UISprite / Knob OR leave default white sprite (solid color OK)
   - Color RGB (0.15, 0.13, 0.20) alpha 1.0  (match OverlayPanel, fully opaque)
   - Preserve Aspect = false
   - raycastTarget = false
5) m_Layer: 5 on FiligraneBackdrop.
6) Do not add Mask / Button. Do not change Filigrane alpha.
7) Save.

Interdits: no C#; no Simulate; do not put Filigrane behind backdrop.

Done = Save. List sibling order + FiligraneBackdrop color RGBA + layer. STOP.
```

**Chars ~1200** — OK &lt; 3500.

### Checklist review Phase 2 (Cursor)

- [x] `FiligraneBackdrop` sibling 0 ; `Filigrane` sibling 1
- [x] Color (0.15, 0.13, 0.20) a=1 ; raycast off ; layer 5 ; stretch
- [x] Filigrane inchangé (α 0.12, sprite OK)
- [x] Note : sprite backdrop = null (couleur pleine) — OK Editor ; si invisible en build → white sprite runtime (comme HUD Vente)

### Après Bezy (auteur)

1. [ ] Playtest : plus de bleed inventaire derrière le filigrane.
2. Si OverlayPanel fuit encore hors zone arbre → remonter OverlayPanel alpha à 1 (hors ce job si OK).
3. Cocher `[BZ-POLISH-010]` clos playtest.
