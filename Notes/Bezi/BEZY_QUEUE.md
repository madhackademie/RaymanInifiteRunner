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
| [ ] | `[BZ-FARM-BIOHUD-PRIM-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab` | 4 | `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md` | `feature/rework-biofiltre-grid` | Shell row 3 nested |
| [ ] | `[BZ-FARM-BIOHUD-PRIM-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab` | 5 | idem | idem | Wire `UiBiofiltreSlotRowView` |
| [ ] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlot.prefab` | 1–3 | idem | idem | Atome secondaire |
| [ ] | `[BZ-FARM-BIOHUD-SEC-001]` | `Assets/Prefabs/Ui/Common/UiBiofiltreSecondarySlotRow.prefab` | 4–5 | idem | idem | Row 5 nested |
| [ ] | `[BZ-FARM-BIOHUD-HOST-001]` | `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab` | 1–3 | idem | idem | HOST world canvas |

---

## Bloc de lancement (copier dans Bezy)

```
/prefab-ui-3phases
Task ID: [BZ-FARM-BIOHUD-PRIM-001]
Prefab: Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlotRow.prefab
Phase: 4
```

Puis `@Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md`

---

## Terminé

| Task ID | Prefab | Phase | Date | Commit / note |
|---------|--------|-------|------|----------------|
| `[BZ-FARM-BIOHUD-PRIM-001]` | `UiBiofiltrePrimarySlot.prefab` | 1–3 | 2026-08-30 | commit auteur `38f5dae` |

---

## Session remote — checklist rapide

1. `git pull`
2. Ouvrir prefab (Prefab Mode si Ph. 3+)
3. Nouveau thread Bezy → bloc ci-dessus + `@` prompt
4. Keep → `git diff` → commit → cocher `[x]` ici
