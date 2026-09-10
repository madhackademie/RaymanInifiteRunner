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
| [x] skip | `[BZ-FARM-BIOHUD-NEST-001]` | `Biofiltre.prefab` | — | Cursor a nesté 2026-09-07 | `main` | Plus besoin Bezy |
| [ ] | `[BZ-TAB-SPRITES-001]` | `NavigationHUD` (tabs) | 1–3 TBD | `Notes/Ui/PROMPTS_Bezi_tab_sprites.md` | `main` | TabVente dernier prompt |
| [ ] | `[BZ-FARM-HARVEST-READY-VFX-002]` | `HarvestReadyFx.prefab` | 5 | `Notes/Ui/PROMPTS_Bezi_harvest_ready_vfx.md` | `main` | Particules récolte Mature+Seedling **plus grosses** (lisibilité) |
| [x] | `[BZ-FARM-PLANT-SELECT-GLOW-001]` | `LaitueObj.prefab` | 1–3 | `Notes/Ui/PROMPTS_Bezi_plant_selection_glow.md` | `main` | Livré 2026-09-10 — playtest auteur |

---

## Bloc de lancement (copier dans Bezy)

```
/prefab-ui-3phases
Task ID: [BZ-TAB-SPRITES-001]
Prefab: (TabVente — voir PROMPTS_Bezi_tab_sprites.md)
Phase: (dernier prompt)
```

**HUD PA ×2 `[BZ-AP-HUD-SCALE-001]` :** Ph.1–3 **OK** 2026-09-10 — playtest auteur.  
**Onglets `[BZ-TAB-SPRITES-001]` :** TabVente dernier prompt.  
**Sparkle récolte `[BZ-FARM-HARVEST-READY-VFX-002]` :** Phase 5 — peut partir sans attendre Cursor.  
**Glow cible `[BZ-FARM-PLANT-SELECT-GLOW-001]` :** Cursor livré — Bezy Ph.1–3 pour visuel prefab (jaune/blanc, scale).  
**Nest HUD :** skip — Cursor 2026-09-07.

---

## Terminé

| Task ID | Prefab | Phase | Date | Commit / note |
|---------|--------|-------|------|----------------|
| `[BZ-AP-HUD-SCALE-001]` | `ActionPointsHudWidget` + `NavigationHUD` | 1–3 | 2026-09-10 | 480×120, fonts ×2, instance scène OK — playtest auteur |
| `[BZ-FARM-BIOHUD-ISO-001]` | `BiofiltreHud.prefab` | 3 | 2026-09-06 | `topIsoLine` → `{fileID: 1385485198660413513}` |
| `[BZ-FARM-BIOHUD-ISO-001]` | `BiofiltreHud.prefab` | 2 | 2026-09-06 | No-op Bezy OK ; tilt+layout = binder |
| `[BZ-FARM-BIOHUD-ISO-001]` | `BiofiltreHud.prefab` | 1 | 2026-09-06 | TopIsoLine ; Primary+Star nested dessous ; Secondary racine |
| `[BZ-FARM-BIOHUD-HOST-001]` | `BiofiltreHud.prefab` | 1–3 | 2026-09-02 | Canvas World + HudView wired ; sorting 0 / 100×100 resté |
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
