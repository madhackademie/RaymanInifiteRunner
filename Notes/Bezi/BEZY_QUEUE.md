# File Bezy — jobs à exécuter dans Unity

**Usage :** Cursor remplit les lignes `[ ]` (prep async). L’auteur exécute dans Unity + Bezy, puis passe en `[x]`.

**Étude complète :** `Notes/Bezi/ETUDE_prompts_bezi_distance.md`  
**Skill :** `/prefab-ui-3phases` — `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**Prompts détaillés :** `Notes/Ui/PROMPTS_Bezi_*.md`

---

## Règles

- **Une ligne = une phase** (jamais Ph.1+2+3 fusionnées).
- Statut **uniquement ici** (`[ ]` / `[x]`).
- Après exécution : `git commit` prefab + cocher `[x]` + noter commit ou date.
- Playtest = **hors** prompt Bezy.

---

## En attente

| Statut | Task ID | Prefab | Phase | Prompt file | Branche | Notes |
|--------|---------|--------|-------|-------------|---------|-------|
| [x] | `[BZ-FARM-BIOHUD-PRIM-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab` | 1 | `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md` | `feature/rework-biofiltre-grid` | Shell OK 2026-08-31 (Slot1–3 nested) |
| [x] | `[BZ-FARM-BIOHUD-PRIM-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab` | 2 | idem | idem | HLG OK ; spacing 4→8 + wire = Ph.3 |
| [x] | `[BZ-FARM-BIOHUD-PRIM-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab` | 3 | `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md` | `feature/rework-biofiltre-grid` | slots[3] Slot1–3 OK ; spacing resté 4 |
| [x] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab` | 1 | `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md` | `feature/rework-biofiltre-grid` | Shell OK ; sizeDelta 72×80 → fixer 48 en Ph.2 |
| [x] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab` | 2 | idem | idem | Images `_0/_1/_2` OK ; 48×48 + wire = Ph.3 |
| [x] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab` | 3 | idem | idem | SlotView wired ; size 72×80 resté |
| [x] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab` | 1 | idem | idem | 5 nested Secondary1–5, 280×48, nested 48×48 |
| [x] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab` | 2 | idem | idem | HLG OK ; spacing 10 ; SlotRowView slots vides count=3 |
| [x] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab` | 3 | idem | idem | slots[5] Secondary1–5 ; count=5 ; spacing 10 resté |
| [ ] | `[BZ-FARM-BIOHUD-HOST-001]` | `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab` | 1–3 | idem | idem | HOST world canvas |

---

## Bloc de lancement (copier dans Bezy)

```
/prefab-ui-3phases
Task ID: [BZ-FARM-BIOHUD-HOST-001]
Prefab: Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab
Phase: 1
```

Puis `@Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md`

---

## Terminé

| Task ID | Prefab | Phase | Date | Commit / note |
|---------|--------|-------|------|----------------|
| `[BZ-FARM-BIOHUD-PRIM-001]` | `UiBiofiltrePrimarySlot.prefab` | 1–3 | 2026-08-30 | commit auteur `38f5dae` |
| `[BZ-FARM-BIOHUD-PRIM-001]` | `UiBiofiltrePrimarySlotRow.prefab` | 1 | 2026-08-31 | nested Slot1–3, sizeDelta 224×80 |
| `[BZ-FARM-BIOHUD-PRIM-001]` | `UiBiofiltrePrimarySlotRow.prefab` | 2 | 2026-08-31 | HLG MiddleLeft ; spacing 4 (à passer à 8 en Ph.3) |
| `[BZ-FARM-BIOHUD-PRIM-001]` | `UiBiofiltrePrimarySlotRow.prefab` | 3 | 2026-08-31 | slots[3] wired Slot1–3 ; spacing 4 / 224 resté |
| `[BZ-FARM-BIOHUD-SEC-001]` | `UiBiofiltreSecondarySlot.prefab` | 1 | 2026-08-31 | Slot/Fill/Lock layer 5 ; sizeDelta 72×80 (à 48) |
| `[BZ-FARM-BIOHUD-SEC-001]` | `UiBiofiltreSecondarySlot.prefab` | 2 | 2026-08-31 | sprites Sprites/ OK ; size 72×80 resté ; SlotView refs vides |
| `[BZ-FARM-BIOHUD-SEC-001]` | `UiBiofiltreSecondarySlot.prefab` | 3 | 2026-08-31 | 3 Images wired ; sizeDelta 72×80 resté |
| `[BZ-FARM-BIOHUD-SEC-001]` | `UiBiofiltreSecondarySlotRow.prefab` | 1 | 2026-08-31 | Secondary1–5 nested 48×48, root 280×48 |
| `[BZ-FARM-BIOHUD-SEC-001]` | `UiBiofiltreSecondarySlotRow.prefab` | 2 | 2026-08-31 | HLG MiddleLeft spacing 10 ; view ajouté slots[] vide |
| `[BZ-FARM-BIOHUD-SEC-001]` | `UiBiofiltreSecondarySlotRow.prefab` | 3 | 2026-08-31 | slots[5] wired Secondary1–5 ; count=5 ; spacing 10 resté |

---

## Session remote — checklist rapide

1. `git pull`
2. Ouvrir prefab (Prefab Mode si Ph. 3+)
3. Nouveau thread Bezy → bloc ci-dessus + `@` prompt
4. Keep → `git diff` → commit → cocher `[x]` ici
