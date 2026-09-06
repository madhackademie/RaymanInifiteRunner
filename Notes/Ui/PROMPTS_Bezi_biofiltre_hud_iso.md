# Prompts Bezy — HUD biofiltre iso `[BZ-FARM-BIOHUD-ISO-001]`

**Skill :** `/prefab-ui-3phases` — `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**Prefab :** `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab`  
**Branche :** `feature/biofiltre-isometric`  
**Succès Bezy :** `Save. List what changed. STOP.` — **pas** de Simulate / Play Mode.

La rangée **secondaire** (5 cadenas bleus, face cuve) **ne change pas**.  
La rangée **haut** (3 slots primaires ornés + étoiles) doit suivre la pente **iso 2:1** (`z = -26.57°`, `atan(0.5)`), comme la grille / le bois du deck.

**Never unpack** `PrimaryRow` / `StarRow` / `SecondaryRow`.  
**Layer :** `m_Layer: 5`. Ne pas éditer `Biofiltre.prefab` (World).

**Cursor déjà prêt :** `BiofiltreHudView.topIsoLine` + binder pose `TopIsoLine` sur le rebord (plus de flottement y=1.08).

**Une phase par appel.** Prefab Mode **obligatoire** Ph.3.

---

## Phase 1 — Hiérarchie `TopIsoLine`

```
[BZ-FARM-BIOHUD-ISO-001] Phase 1 ONLY — TopIsoLine hierarchy. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT unpack nested PrimaryRow / StarRow / SecondaryRow.
Do NOT edit Biofiltre.prefab, UiBiofiltre* slots, UiStarRow.

File ONLY:
- Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab

Prefab Mode. Root BiofiltreHud already exists (Canvas World Space).
Current children: PrimaryRow, StarRow, SecondaryRow (nested prefab instances).

Create ONE empty child under root named TopIsoLine
(RectTransform only — no Image, no Canvas, no script).

Reparent, keep nested instances, do not unpack:
- PrimaryRow → child of TopIsoLine
- StarRow → child of TopIsoLine
SecondaryRow stays a direct child of BiofiltreHud.

Target hierarchy:
BiofiltreHud
├── TopIsoLine
│   ├── PrimaryRow
│   └── StarRow
└── SecondaryRow

m_Layer: 5 on TopIsoLine. No new C#.

Done = Save. List hierarchy (4 names). STOP.
```

---

## Phase 2 — Tilt iso 2:1 + layout

> **Clos no-op 2026-09-06 (volontaire).** `TopIsoLine` = pivot Transform uniquement. Pas d’Image / Canvas / HLG.  
> Tilt `z = -26.57` + alignement Primary|Star = **`BiofiltreHudBinder`** (Cursor). Passer à la Phase 3.

---

---

## Phase 3 — Wiring `topIsoLine`

```
[BZ-FARM-BIOHUD-ISO-001] Phase 3 ONLY — wire topIsoLine. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate. Do not unpack nested.
Prefab Mode REQUIRED.

File ONLY:
- Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab

Reuse existing script (do not recreate):
- Assets/Scripts/UI/BiofiltreHud/BiofiltreHudView.cs

On root BiofiltreHudView:
- topIsoLine → child TopIsoLine (RectTransform)
- primaryRow → TopIsoLine/PrimaryRow (UiBiofiltreSlotRowView)
- starRow → TopIsoLine/StarRow (UiStarRowView)
- secondaryRow → SecondaryRow (UiBiofiltreSlotRowView)
previewStarFilled = 1, previewStarVisible = 5.

Keep one Canvas World Space on root. No second Canvas.
Keep GraphicRaycaster. m_Layer: 5. No OnClick.
Do not edit Biofiltre.prefab (World).

Done = Save. Confirm 4 refs including topIsoLine. STOP.
```

---

## Bloc de lancement (auteur Unity)

Prefab Mode : ouvrir `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab`.

```
/prefab-ui-3phases
Task ID: [BZ-FARM-BIOHUD-ISO-001]
Prefab: Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab
Phase: 1
```

Puis `@Notes/Ui/PROMPTS_Bezi_biofiltre_hud_iso.md`

Après Ph.3 : playtest auteur FirstLvl (hors prompt). Ajuster `topIsoNormalizedAnchor` / `topIsoWorldOffset` sur `BiofiltreHudBinder` si le strip n’est pas pile sur le bois.
