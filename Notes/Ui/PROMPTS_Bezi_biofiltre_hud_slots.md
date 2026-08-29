# Prompts Bezy — HUD slots biofiltre `[BZ-FARM-BIOHUD-*]`

**Skill :** `/prefab-ui-3phases` — `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**Brief VM / C# :** `Notes/Farm/PROMPT_agent_vm_biofiltre_hud_slots.md`  
**Modèle :** `Notes/Ui/PROMPTS_Bezi_ui_star_slot.md` (atome + rangée N). **Ne pas** recréer `UiStarSlot` / `UiStarRow`.

**Succès Bezy = Save. List what changed. STOP. Pas de Simulate / Play Mode.**  
**Layer :** `m_Layer: 5` (Water=4 — never 4).  
**Art :** uniquement `Assets/Art/Sprites/UI/Biofiltre/` — **jamais** `Assets Store Dump`.  
**Never unpack** nested `UiStar*` ni les slots biofiltre.

**Prérequis Cursor :** scripts existants (ne pas modifier / recréer) :

- `Assets/Scripts/UI/BiofiltreHud/UiBiofiltreSlotView.cs`
- `Assets/Scripts/UI/BiofiltreHud/UiBiofiltreSlotRowView.cs`
- `Assets/Scripts/UI/BiofiltreHud/BiofiltreHudView.cs`
- `Assets/Scripts/UI/Stars/UiStarRowView.cs` (host seulement)

**Ordre jobs :** PRIM-001 (slot Ph.1–3, row Ph.4–5) → SEC-001 (idem) → HOST-001 (Ph.1–3).  
**Un job / une phase / un appel skill.** Prefab Mode ouvert recommandé ; **obligatoire** Ph.3.

**Crédits Bezy :** reset **30** de chaque mois.

---

## Job A — `[BZ-FARM-BIOHUD-PRIM-001]` primaire

**Prefabs :**  
`Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlot.prefab`  
`Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab` (après Ph.3)

Sprites : `slotBiofiltrePrimaire_0` Slot · `_1` Fill · `_2` Lock  
(`Assets/Art/Sprites/UI/Biofiltre/slotBiofiltrePrimaire.png`)

### Phase 1 — Shell atome primaire

```
[BZ-FARM-BIOHUD-PRIM-001] Phase 1 ONLY — UiBiofiltrePrimarySlot shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.

Create NEW prefab ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlot.prefab

Root name: UiBiofiltrePrimarySlot
Children (RectTransform only — no Image/TMP):
UiBiofiltrePrimarySlot
├── Slot
├── Fill
└── Lock

Rules:
- m_Layer: 5 on root + Slot + Fill + Lock.
- Root: anchors middle-center, pivot 0.5/0.5, sizeDelta 72 x 80.
- Slot, Fill, Lock: stretch parent (anchors 0,0–1,1 ; offsets 0 ; pivot 0.5/0.5).
- Sibling order: Slot, Fill, Lock (Lock on top).
- No LayoutGroup / Button / Animator / Canvas.
- Do not edit UiStarSlot, UiStarRow, Biofiltre.prefab, SaleChannel*.

Done = Save. List hierarchy + root sizeDelta. STOP.
```

### Phase 2 — Visuels atome primaire

```
[BZ-FARM-BIOHUD-PRIM-001] Phase 2 ONLY — primary slot images. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT use Assets/Art/Assets Store Dump/ paths.

File ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlot.prefab

On root: LayoutElement preferred/min 72 x 80.

On Slot: Image, sprite slotBiofiltrePrimaire_0 from
Assets/Art/Sprites/UI/Biofiltre/slotBiofiltrePrimaire.png
Preserve Aspect true, raycastTarget false, Type Simple, white, stretch parent.

On Fill: Image, sprite slotBiofiltrePrimaire_1 (same png). Preserve Aspect true, raycast OFF, Type Simple. Set m_IsActive 0 (hidden — locked preview).

On Lock: Image, sprite slotBiofiltrePrimaire_2. Preserve Aspect true, raycast OFF, Type Simple. Leave ACTIVE.

m_Layer: 5. Do not edit other prefabs.

Done = Save. List sprite names + Fill inactive + Lock active. STOP.
```

### Phase 3 — Wire atome primaire

```
[BZ-FARM-BIOHUD-PRIM-001] Phase 3 ONLY — wire UiBiofiltreSlotView on primary slot. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. No Simulate. No OnClick.

File ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlot.prefab

Script already exists (reuse, do not recreate):
- Assets/Scripts/UI/BiofiltreHud/UiBiofiltreSlotView.cs

On root: add UiBiofiltreSlotView
- slotImage → Slot Image
- fillImage → Fill Image
- lockImage → Lock Image
Default state Locked if a serialized enum/bool exists; keep Fill inactive, Lock active.

Keep LayoutElement + Images. m_Layer: 5.

Done = Save. Confirm three Image refs assigned. STOP.
```

### Phase 4 — Shell rangée primaire (N=3)

```
[BZ-FARM-BIOHUD-PRIM-001] Phase 4 ONLY — UiBiofiltrePrimarySlotRow shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT recreate UiBiofiltrePrimarySlot. Do NOT unpack nested prefabs.

Create NEW prefab ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab

Root name: UiBiofiltrePrimarySlotRow
m_Layer: 5.

Nest THREE Prefab Instances of
Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlot.prefab
Names: Primary1, Primary2, Primary3 (left to right).

On root: HorizontalLayoutGroup spacing 8, childAlignment MiddleLeft,
childControlWidth=false, childControlHeight=false,
childForceExpandWidth=false, childForceExpandHeight=false
RectTransform: anchors middle-left, pivot 0,0.5, sizeDelta ~240 x 80

No extra Images/scripts this phase. Keep nested size 72x80.

Done = Save. List 3 nested instance names. STOP.
```

### Phase 5 — Wire rangée primaire

```
[BZ-FARM-BIOHUD-PRIM-001] Phase 5 ONLY — wire UiBiofiltreSlotRowView primary. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate. Do not unpack nested.

File ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab

Script already exists:
- Assets/Scripts/UI/BiofiltreHud/UiBiofiltreSlotRowView.cs

On root: add UiBiofiltreSlotRowView
- slots array size 3 → Primary1…Primary3 UiBiofiltreSlotView (order)
- visibleSlotCount = 3 if the field exists

m_Layer: 5.

Done = Save. Confirm slots[3] wired. STOP.
```

**Checklist review PRIM (Cursor)**
- [ ] Slot 72×80, Slot/Fill/Lock, layer 5
- [ ] Sprites `_0` `_1` `_2` (Sprites/UI/Biofiltre, pas Dump)
- [ ] Fill inactive, Lock active
- [ ] Row 3 nested, HLG spacing 8, view wired

---

## Job B — `[BZ-FARM-BIOHUD-SEC-001]` secondaire

**Prefabs :**  
`Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab`  
`Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab`

Sprites : `slotBiofiltreSecondaire_0` Slot · `_1` Fill · `_2` Lock

### Phase 1 — Shell atome secondaire

```
[BZ-FARM-BIOHUD-SEC-001] Phase 1 ONLY — UiBiofiltreSecondarySlot shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.

Create NEW prefab ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab

Root name: UiBiofiltreSecondarySlot
Children RectTransform only:
UiBiofiltreSecondarySlot
├── Slot
├── Fill
└── Lock

- m_Layer: 5 all.
- Root: middle-center, pivot 0.5/0.5, sizeDelta 48 x 48.
- Slot, Fill, Lock: stretch parent. Order Slot, Fill, Lock.
- No Image/Layout/Button/Canvas. Do not edit primary or UiStar*.

Done = Save. List hierarchy + 48x48. STOP.
```

### Phase 2 — Visuels atome secondaire

```
[BZ-FARM-BIOHUD-SEC-001] Phase 2 ONLY — secondary slot images. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT use Dump paths.

File ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab

Root LayoutElement preferred/min 48 x 48.

Slot Image: slotBiofiltreSecondaire_0 from
Assets/Art/Sprites/UI/Biofiltre/slotBiofiltreSecondaire.png
Preserve Aspect, raycast OFF, Simple, white, stretch.

Fill Image: slotBiofiltreSecondaire_1. Preserve Aspect, raycast OFF. m_IsActive 0.

Lock Image: slotBiofiltreSecondaire_2. Preserve Aspect, raycast OFF. ACTIVE.

m_Layer: 5.

Done = Save. List sprites + Fill inactive. STOP.
```

### Phase 3 — Wire atome secondaire

```
[BZ-FARM-BIOHUD-SEC-001] Phase 3 ONLY — wire UiBiofiltreSlotView on secondary. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.

File ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab

Reuse Assets/Scripts/UI/BiofiltreHud/UiBiofiltreSlotView.cs (same as primary, do not duplicate script).

Root: add UiBiofiltreSlotView — slotImage/fillImage/lockImage → Slot/Fill/Lock Images.
Fill inactive, Lock active. m_Layer: 5.

Done = Save. Confirm refs. STOP.
```

### Phase 4 — Shell rangée secondaire (N=5)

```
[BZ-FARM-BIOHUD-SEC-001] Phase 4 ONLY — UiBiofiltreSecondarySlotRow shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate. Do not unpack.

Create NEW prefab ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab

Root: UiBiofiltreSecondarySlotRow, m_Layer: 5.

Nest FIVE instances of
Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab
Names: Secondary1 … Secondary5.

Root HorizontalLayoutGroup spacing 6, MiddleLeft,
childControl/forceExpand all false.
sizeDelta ~280 x 48, anchors middle-left, pivot 0,0.5.

Done = Save. List 5 nested names. STOP.
```

### Phase 5 — Wire rangée secondaire

```
[BZ-FARM-BIOHUD-SEC-001] Phase 5 ONLY — wire UiBiofiltreSlotRowView secondary. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate. Do not unpack.

File ONLY:
- Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab

Reuse UiBiofiltreSlotRowView.cs (do not create a second row script).

Root: add UiBiofiltreSlotRowView
- slots[5] → Secondary1…5 UiBiofiltreSlotView
- visibleSlotCount = 5 if field exists

m_Layer: 5.

Done = Save. Confirm slots[5]. STOP.
```

**Checklist review SEC (Cursor)**
- [ ] 48×48, 5 nested, sprites secondaires Sprites/ pas Dump
- [ ] Même `UiBiofiltreSlotView` / `UiBiofiltreSlotRowView` que le primaire

---

## Job C — `[BZ-FARM-BIOHUD-HOST-001]` HUD world

**Prefab :** `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab`  
**Prérequis :** jobs A+B livrés + `UiStarRow.prefab` existant.

### Phase 1 — Shell HUD

```
[BZ-FARM-BIOHUD-HOST-001] Phase 1 ONLY — BiofiltreHud shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT unpack UiStarRow / slot rows. Do NOT edit Biofiltre.prefab (World).

Create NEW prefab ONLY:
- Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab

Root name: BiofiltreHud
m_Layer: 5.
Root RectTransform: center, pivot 0.5/0.5, sizeDelta 800 x 600 (placeholder; binder scales to grid).

Children (empty RectTransform mounts only this phase — no Canvas/Image yet):
BiofiltreHud
├── PrimaryMount
├── StarMount
└── SecondaryMount

Under PrimaryMount: nest ONE
Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab named PrimarySlotRow

Under StarMount: nest ONE
Assets/Prefabs/Ui/Common/UiStarRow.prefab named StarRow

Under SecondaryMount: nest ONE
Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab named SecondarySlotRow

Mounts: stretch or center as needed; do not add Images.

Done = Save. List 3 nested prefab instances + parents. STOP.
```

### Phase 2 — Canvas world

```
[BZ-FARM-BIOHUD-HOST-001] Phase 2 ONLY — World Space Canvas on BiofiltreHud. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate. Do not unpack nested.

File ONLY:
- Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab

On root BiofiltreHud:
- Canvas: Render Mode World Space. Plane Distance ignore. Sorting Order 20.
- CanvasScaler: World — leave default dynamic pixels if present; do not switch to Overlay.
- GraphicRaycaster: yes (slots may be clickable later).

Do NOT add panel Image full-bleed (HUD chrome = the nested rows only).
Do NOT edit nested slot/star prefab assets.
m_Layer: 5 on root + mounts.

Done = Save. List Canvas renderMode + sortingOrder. STOP.
```

### Phase 3 — Wire BiofiltreHudView

```
[BZ-FARM-BIOHUD-HOST-001] Phase 3 ONLY — wire BiofiltreHudView. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate. Do not unpack nested.

File ONLY:
- Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab

Script already exists:
- Assets/Scripts/UI/BiofiltreHud/BiofiltreHudView.cs

On root: add BiofiltreHudView
- starRow → StarMount/StarRow UiStarRowView
- primaryRow → PrimaryMount/PrimarySlotRow UiBiofiltreSlotRowView
- secondaryRow → SecondaryMount/SecondarySlotRow UiBiofiltreSlotRowView

If preview fields exist: star filledOnStart 1 / visible 5; leave slot rows locked.

m_Layer: 5. Do not add OnClick. Do not edit Assets/Prefabs/World/Biofiltre.prefab.

Done = Save. Confirm 3 view refs. STOP.
```

**Checklist review HOST (Cursor)**
- [ ] Nested StarRow + 2 slot rows, not unpacked
- [ ] Canvas World Space, layer 5
- [ ] `BiofiltreHudView` câblé
- [ ] Lien `hudPrefab` sur instance biofiltre = **manuel auteur** (World prefab hors skill)

---

## Blocs de lancement (auteur Unity)

```
/prefab-ui-3phases
Task ID: [BZ-FARM-BIOHUD-PRIM-001]
Prefab: Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlot.prefab
Phase: 1
```

Après Ph.3 slot : même Task ID, Prefab `UiBiofiltrePrimarySlotRow.prefab`, Phase 4 puis 5.

Puis SEC-001 (slot Phase 1–3, row 4–5).  
Puis HOST-001 Phases 1–3, Prefab `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab`.

`@Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md` à chaque appel.
