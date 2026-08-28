# Prompts Bezy — étoiles UI génériques `[P0-UI-STAR-SLOT-001]`

**But :**  
1. **UiStarSlot** = 1 slot + 1 fill (atome).  
2. **UiStarRow** = rangée parent : **5** enfants nested `UiStarSlot` (base prestige / canaux GDD ★1–5).  
   `visibleSlotCount` / `SetFilledCount` pour afficher / remplir ; capacité fixe **5** (pas 3, pas 7).

**GDD :** `SPEC_vente_production_boucle_jeu.md` §2.9 (bandeaux ★1–5) · `SPEC_prestige_generation_systemes.md` / biofiltre (★0–5, portes ★3/★5).

**Activation :** pas de logique métier dans les prefabs.  
- Slot : `UiStarSlotView.SetFilled(bool)`  
- Row : `UiStarRowView.SetVisibleSlotCount(n)` + `SetFilledCount(n)`

**Credits Bezy :** reset **30** de chaque mois.

**Prefabs :**
- `Assets/Prefabs/Ui/Common/UiStarSlot.prefab`
- `Assets/Prefabs/Ui/Common/UiStarRow.prefab` (après Ph.3)

**Scripts (déjà dans le repo, ne pas modifier / recréer) :**  
- `Assets/Scripts/UI/Stars/UiStarSlotView.cs`
- `Assets/Scripts/UI/Stars/UiStarRowView.cs`

**Sprites canoniques (hors Dump) :**
- Slot : `Assets/Art/Sprites/UI/Progression/StarSlot.png`
- Fill : `Assets/Art/Sprites/UI/Progression/StarFill.png`

**Succès Bezy = Save. List what changed. STOP. Pas de Simulate / Play Mode.**

**Ordre :** Ph.1 → Ph.2 → Ph.3 (slot) → Ph.4 → Ph.5 (row).  
**Hors scope :** ne pas remplacer les carrés du bandeau vente (intégration = chantier suivant).

**Resize :**
- Slot : `sizeDelta` / LayoutElement sur l’instance (24 / 32 / 48).
- Row : spacing + taille des enfants nested ; scale uniforme via LayoutElement des slots.

---

## Phase 1 — Shell prefab (hiérarchie seule)

```
[P0-UI-STAR-SLOT-001] Phase 1 ONLY — UiStarSlot prefab shell. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.

Create NEW prefab ONLY:
- Assets/Prefabs/Ui/Common/UiStarSlot.prefab

Root GameObject name: UiStarSlot
Children (exact names, RectTransform only — no Image/TMP yet):
UiStarSlot
├── Slot
└── Fill

Rules:
- m_Layer: 5 on root + Slot + Fill (UI=5; Water=4 — never 4).
- Root RectTransform: anchors middle-center, pivot 0.5/0.5, sizeDelta 32 x 32.
- Slot + Fill: stretch full parent (anchors 0,0–1,1 ; offsets 0 ; pivot 0.5/0.5).
- Sibling order: Slot first, Fill second (Fill draws on top).
- No LayoutGroup / ContentSizeFitter / Button / Animator / Canvas.
- Do not edit SaleChannelBandeauView or any other prefab.

Done = Save prefab. List GO hierarchy + root sizeDelta. STOP.
```

**Checklist review Ph.1 (Cursor)** — **OK Bezy 2026-08-28**
- [x] Prefab `UiStarSlot.prefab` créé
- [x] Root 32×32, Slot + Fill stretch, layer 5
- [x] Pas d’Image encore

---

## Phase 2 — Visuels Slot + Fill

```
[P0-UI-STAR-SLOT-001] Phase 2 ONLY — Slot + Fill images. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT use Assets/Art/Assets Store Dump/ paths.

File ONLY:
- Assets/Prefabs/Ui/Common/UiStarSlot.prefab

On UiStarSlot root:
- Add LayoutElement: preferredWidth 32, preferredHeight 32, minWidth 32, minHeight 32.
  (Lets parent layouts size the star; author can override sizeDelta on instances.)

On Slot:
- Add Image
- Sprite = Assets/Art/Sprites/UI/Progression/StarSlot.png
- Color white RGB (1,1,1) alpha 1
- Preserve Aspect = true
- raycastTarget = false
- Type = Simple
- Keep stretch full parent

On Fill:
- Add Image
- Sprite = Assets/Art/Sprites/UI/Progression/StarFill.png
- Color white RGB (1,1,1) alpha 1
- Preserve Aspect = true
- raycastTarget = false
- Type = Simple
- Keep stretch full parent
- Leave Fill ACTIVE (preview filled); Cursor script will toggle at runtime.

m_Layer: 5. Do not edit other prefabs.

Done = Save. List sprite paths on Slot + Fill + LayoutElement sizes. STOP.
```

**Checklist review Ph.2 (Cursor)** — **OK Bezy 2026-08-28**
- [x] StarSlot / StarFill (Progression), preserve aspect, raycast off
- [x] LayoutElement 32×32 sur root
- [x] Pas de ref Dump

---

## Phase 3 — Wire UiStarSlotView

```
[P0-UI-STAR-SLOT-001] Phase 3 ONLY — wire UiStarSlotView. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. No Simulate. No OnClick.

File ONLY:
- Assets/Prefabs/Ui/Common/UiStarSlot.prefab

Script already exists (reuse, do not recreate):
- Assets/Scripts/UI/Stars/UiStarSlotView.cs

On root UiStarSlot:
- Add component UiStarSlotView
- slotImage → child Slot Image
- fillImage → child Fill Image
- filledOnStart = false (empty slot by default in prefab)

Keep LayoutElement + Images as-is. m_Layer: 5.

Done = Save. Confirm UiStarSlotView refs assigned + filledOnStart false. STOP.
```

**Checklist review Ph.3 (Cursor)** — **OK Bezy 2026-08-28**
- [x] `UiStarSlotView` câblé Slot + Fill
- [x] `filledOnStart = false`
- [x] Prefab prêt à être nested dans UiStarRow

---

## Phase 4 — Shell UiStarRow (5 nested UiStarSlot)

```
[P0-UI-STAR-SLOT-001] Phase 4 ONLY — UiStarRow shell with nested UiStarSlot. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT recreate UiStarSlot.prefab. Do NOT edit SaleChannelBandeauView.

Create NEW prefab ONLY:
- Assets/Prefabs/Ui/Common/UiStarRow.prefab

Root GameObject name: UiStarRow
m_Layer: 5.

Under UiStarRow: nest FIVE Prefab Instances of Assets/Prefabs/Ui/Common/UiStarSlot.prefab
as children (Unity nested prefab instances — NOT duplicated hierarchy copy).
Name instances: Star1, Star2, Star3, Star4, Star5 (left to right).

On UiStarRow root:
- HorizontalLayoutGroup: spacing 4, childAlignment MiddleLeft,
  childControlWidth=false, childControlHeight=false,
  childForceExpandWidth=false, childForceExpandHeight=false
- RectTransform: anchors middle-left, pivot 0,0.5, sizeDelta ~180 x 32

Do NOT add Images / scripts yet. Keep each nested slot size 32x32.

Done = Save. List 5 nested prefab instance names + parent. STOP.
```

**Checklist review Ph.4 (Cursor)** — **OK Bezy 2026-08-28**
- [x] `UiStarRow.prefab` + 5 nested `UiStarSlot` (Star1…Star5)
- [x] HorizontalLayoutGroup spacing 4
- [x] Layer 5

---

## Phase 5 — Wire UiStarRowView

```
[P0-UI-STAR-SLOT-001] Phase 5 ONLY — wire UiStarRowView. Wait success. STOP after save.

Do not rescan whole project. Do not modify C# scripts. No Simulate.

File ONLY:
- Assets/Prefabs/Ui/Common/UiStarRow.prefab

Script already exists (reuse, do not recreate):
- Assets/Scripts/UI/Stars/UiStarRowView.cs

On root UiStarRow:
- Add UiStarRowView
- slots array size 5 → Star1…Star5 UiStarSlotView (in order)
- visibleSlotCount = 5
- filledOnStart = 0

Do not unpack nested prefabs. Do not edit UiStarSlot asset itself.
m_Layer: 5.

Done = Save. Confirm slots[5] wired + visibleSlotCount 5 + filledOnStart 0. STOP.
```

**Checklist review Ph.5 (Cursor)** — **OK Bezy 2026-08-28**
- [x] `UiStarRowView.slots` = Star1…Star5
- [x] visibleSlotCount 5, filledOnStart 0

---

## Usage (après Bezy)

| API | Effet |
|-----|--------|
| `SetVisibleSlotCount(3)` | Affiche 3 slots, cache Star4–5 |
| `SetFilledCount(2)` | 2 premières étoiles remplies |

Inspector : même champs sur `UiStarRowView`.  
Intégration bandeau vente = chantier suivant (remplacer les carrés).
