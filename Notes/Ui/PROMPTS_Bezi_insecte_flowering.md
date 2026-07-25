# Prompts Bezy — Insecte Flowering `[BZ-POLISH-014]` / `[CT-FARM-POLISH-002]`

**Spec :** `Notes/Farm/SPEC_insecte_flowering.md`  
**Art prêt :** `Assets/Art/Sprites/Farm/Insects/Bee_Fly.png` → sprites `Bee_Fly_01` … `Bee_Fly_06`  
**Scripts Cursor (déjà créés, ne pas rescan)** :
- `Assets/Scripts/Farm/InsectPathAnchor.cs`
- `Assets/Scripts/Farm/InsectPathFollower.cs`
- `Assets/Scripts/Farm/InsectKind.cs`
- Hook : `PlantGrow.SyncInsectPathForStage` (Flowering)

**Règles :** ne pas rescanner tout le projet ; pas de nouveaux scripts C# ; world prefabs (pas UI Canvas) ; un prompt = une phase ; attendre OK avant la suivante.

### Historique

- **Phase 1** : Bee prefab + Animator Fly/Forage + clip 6 frames — **OK** (Bezy 2026-07-25)
- **Phase 2** : InsectPath + Node_0..3 + Bee sur LaitueObj (inactive) — **OK** (Bezy 2026-07-25)
- **Phase 3** : wiring Bee / InsectPathAnchor / PlantGrow / Laitue.insectKind — **OK** (Bezy 2026-07-25)

---

## Phase 1 — Prefab Bee + Animator (copier-coller Bezy) — CLOS

```
[BZ-POLISH-014] Phase 1 ONLY — Bee prefab + Animator. Wait success before Phase 2.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT create UI Canvas.

CREATE folder if missing: Assets/Prefabs/World/Insects/
CREATE folder if missing: Assets/Art/Animations/Farm/Insects/

CREATE prefab: Assets/Prefabs/World/Insects/Bee.prefab
Root GameObject name: Bee
Components on root:
- Transform
- SpriteRenderer: sprite = Bee_Fly_01 (from Assets/Art/Sprites/Farm/Insects/Bee_Fly.png), sorting layer same as plants / Default, order in layer = plant+1 or 5
- Animator
- InsectPathFollower (script already exists)

CREATE Animator Controller: Assets/Art/Animations/Farm/Insects/Bee.controller
States (same layer Base):
- Fly (default) — clip Bee_Fly looping
- Forage — same clip Bee_Fly OR duplicate clip Bee_Forage at ~0.45 speed, looping
NO transitions required if scripts use Animator.Play("Fly"|"Forage").
State names MUST be exactly: Fly , Forage

CREATE AnimationClip: Assets/Art/Animations/Farm/Insects/Bee_Fly.anim
- SpriteRenderer.sprite keyframes: Bee_Fly_01 → 02 → 03 → 04 → 05 → 06 → loop
- Sample ~12 FPS, Loop Time ON

Assign Bee.controller on Bee Animator.
On InsectPathFollower: wire spriteRenderer + animator (same root).

Scale Bee ~0.35–0.5 so it reads small next to lettuce.
Prefab inactive OK (plant activates path).

Confirm: prefab path, controller path, state names Fly/Forage, 6-frame loop.
```

---

## Phase 2 — Shell InsectPath sur LaitueObj (après Phase 1 OK)

```
[BZ-POLISH-014] Phase 2 ONLY — InsectPath shell on LaitueObj. Wait success before Phase 3.

Do NOT rescan whole project. Do NOT create scripts. Do NOT retouch Bee.prefab art.

EDIT prefab: Assets/Prefabs/World/Plantes/LaitueObj.prefab

Under root LaitueObj, CREATE child:
InsectPath (inactive by default)
  Components: Transform + InsectPathAnchor
  Children empty Transforms (local positions around lettuce crown, Y slightly above plant center):
    Node_0 , Node_1 , Node_2 , Node_3
  Child instance of Bee prefab named Bee (or InsectInstance)
    Keep Bee as child of InsectPath

Suggested local positions (tune visually in Scene):
  Node_0 (-0.25, 0.35, 0)
  Node_1 ( 0.25, 0.40, 0)
  Node_2 ( 0.20, 0.15, 0)
  Node_3 (-0.20, 0.18, 0)

On InsectPathAnchor: autoCollectNodes=true ; insectFollower → child Bee.
On PlantGrow (root): insectPath → InsectPath (optional but preferred).

Select InsectPath in Scene: yellow gizmo spheres+lines should show circuit.
Confirm hierarchy + InsectPath starts DISABLED.
```

---

## Phase 3 — Wiring + Laitue insectKind (après Phase 2 OK)

```
[BZ-POLISH-014] Phase 3 ONLY — Inspector wiring. No new hierarchy.

Do NOT rescan whole project. Do NOT create scripts.

1) Bee.prefab — InsectPathFollower:
   spriteRenderer → root SpriteRenderer
   animator → root Animator
   moveSpeed 0.8 ; forageDurationMin 0.5 ; forageDurationMax 1.5

2) LaitueObj — InsectPathAnchor:
   nodes auto from Node_0..3 (ContextMenu Refresh if needed)
   insectFollower → Bee child

3) LaitueObj — PlantGrow.insectPath → InsectPath

4) Assets/Data/Ferme/Laitue.asset — Insect Kind = Bee
   (leave speeds 0 = script defaults)

Play Mode smoke (author): force plant to Flowering → InsectPath active, bee loops Fly→Forage on nodes, flipX when moving left.

Confirm wiring checklist done.
```

---

## Checklist validation

| Critère | OK? |
|---------|-----|
| `Bee.prefab` + `Bee.controller` états `Fly` / `Forage` | x |
| Clip 6 frames ~12 FPS loop | x |
| `LaitueObj` → `InsectPath` (off) + `Node_0..3` + Bee | x |
| Gizmos circuit visibles | x (auteur Scene) |
| `Laitue.asset` InsectKind = Bee | x |
| Hors Flowering : path inactif | playtest |
