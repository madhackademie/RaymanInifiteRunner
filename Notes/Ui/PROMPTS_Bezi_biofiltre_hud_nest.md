# Prompts Bezy — HUD nested en dur `[BZ-FARM-BIOHUD-NEST-001]`

**Skill :** `/prefab-ui-3phases` — `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**Prefab cible :** `Assets/Prefabs/World/Biofiltre.prefab`  
**Nested :** `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab`  
**Branche :** `main` (livré, ex-`feature/biofiltre-isometric`)  
**Succès :** `Save. List what changed. STOP.` — **pas** de Simulate / Play Mode.

**Pourquoi :** instantiate au Play **ne marche pas** (rows invisibles en Edit, Save interdit en Play). Note : `Notes/Farm/NOTE_hud_biofiltre_prefab_en_dur.md`.

**Never unpack** BiofiltreHud / PrimaryRow / StarRow / SecondaryRow / TopIsoLine.  
**Layer HUD :** `m_Layer: 5`. Ne pas modifier C#.

**Une phase par appel.** Prefab Mode **obligatoire** (World `Biofiltre`).

**Statut 2026-09-07 :** **skip Bezy** — Cursor a nesté `BiofiltreHud` sous `Biofiltre.prefab`. Prompts ci-dessous **ne pas lancer**.

---

## Phase 1 — Neste BiofiltreHud

```
[BZ-FARM-BIOHUD-NEST-001] Phase 1 ONLY — nest BiofiltreHud under Biofiltre. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT unpack nested prefabs. Do NOT edit slot prefabs or UiStarRow.

File ONLY:
- Assets/Prefabs/World/Biofiltre.prefab

Prefab Mode. Root Biofiltre already has children IbcSprite, Grid, Plants.

Neste ONE instance (do not unpack):
- Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab
Name the instance: BiofiltreHud
Parent: Biofiltre root.
Keep IbcSprite, Grid, Plants as siblings.

Target:
Biofiltre
├── IbcSprite
├── Grid
├── Plants
└── BiofiltreHud   (nested, not unpacked)

Do not add extra empty mounts. No new C#.

Done = Save. List 4 children of Biofiltre. STOP.
```

---

## Phase 2 — Canvas / layer (pas de 2e Canvas)

```
[BZ-FARM-BIOHUD-NEST-001] Phase 2 ONLY — HUD layer + no second Canvas. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate. Do not unpack.

File ONLY:
- Assets/Prefabs/World/Biofiltre.prefab

Nested BiofiltreHud already has Canvas World Space. Do NOT add another Canvas on Biofiltre root.

On the nested BiofiltreHud instance:
- m_Layer: 5 on HUD root (and children if missing)
- Keep GraphicRaycaster on HUD
- localPosition 0,0,0 ; localRotation identity ; localScale 1,1,1 (author will move rows later)

Do not add Image/Button on Biofiltre root. Do not rotate rows. Do not unpack.

Done = Save. Confirm one Canvas total (on BiofiltreHud). STOP.
```

---

## Phase 3 — Rien à unpack ; confirmer hiérarchie

```
[BZ-FARM-BIOHUD-NEST-001] Phase 3 ONLY — confirm nested HUD, no unpack. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Prefab Mode REQUIRED.

File ONLY:
- Assets/Prefabs/World/Biofiltre.prefab

Do NOT add BiofiltreHudView (already on nested HUD).
Do NOT wire BiofiltreHudBinder fields (Cursor after this job).
Do NOT unpack BiofiltreHud.

Confirm and Save:
- BiofiltreHud is nested child of Biofiltre
- Inside HUD: TopIsoLine with PrimaryRow + StarRow, SecondaryRow sibling
- Still nested prefab instances (not unpacked)

If anything unpacked: STOP and report. Else Save. List hierarchy. STOP.
```

---

## Bloc de lancement (auteur Unity)

Prefab Mode : ouvrir `Assets/Prefabs/World/Biofiltre.prefab`.

```
/prefab-ui-3phases
Task ID: [BZ-FARM-BIOHUD-NEST-001]
Prefab: Assets/Prefabs/World/Biofiltre.prefab
Phase: 1
```

Puis `@Notes/Ui/PROMPTS_Bezi_biofiltre_hud_nest.md`
