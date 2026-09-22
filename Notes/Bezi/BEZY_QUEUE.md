# File Bezy — jobs à exécuter dans Unity

**Usage :** Cursor remplit les lignes `[ ]` (prep async). L’auteur exécute dans Unity + Bezy, puis passe en `[x]`.

**Étude complète :** `Notes/Bezi/ETUDE_prompts_bezi_distance.md`  
**Skill :** `/prefab-ui-3phases` (prefab) · scène : `@Assets/Docs/Bezi/PROMPTS_*.md` — `Notes/Bezi/BEZY_PROMPT_COPYPASTE.md`  
**À faire (index) :** `Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md`  
**Prompts détaillés (`@` Unity) :** `Assets/Docs/Bezi/PROMPTS_Bezi_*.md`  
**Archive livrés :** `Notes/Bezi/ARCHIVE_prompts_bezi_index.md`

---

## Règles

- **Une ligne = une phase** (jamais Ph.1+2+3 fusionnées).
- Statut **uniquement ici** (`[ ]` / `[x]`).
- Après exécution : `git commit` prefab + cocher `[x]` + noter commit ou date.
- Playtest = **hors** prompt Bezy.

---

## En attente

**Tableau détaillé :** `Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md` (priorités + chemins `@`).

| Statut | Task ID | Phase | Notes |
|--------|---------|-------|-------|
| [ ] | `[BZ-INV-DROP-CLOSE-POS-001]` | 1 | Croix popup drop |
| [ ] | `[BZ-INV-TABS-003]` | 1 | Onglet Récoltes |
| [ ] | `[BZ-HUD-CLOSE-STRIP-001]` | 1→3 | Supprimer CloseButton legacy |
| [ ] | `[BZ-FEATURES-HUB-WIP-COPY-001]` | 1 | Copy WIP hub |
| [ ] | `[BZ-SHOP-FILIGRANE-001]` | 2 | Disable Image SlotsGrid |
| [ ] | `[BZ-UIKIT-POPUP-MOCK-001]` | 3 | Tailles mockup bois |
| [ ] | `[BZ-INV-WALLET-NAV-BAND-002]` | 2 | Y -100 + sorting 60 |
| [ ] | `[BZ-FARM-CAMERA-VIEW-001]` | 1→2 | Branche feature — Cursor scripts d’abord |
| [ ] | `[BZ-TAB-SPRITES-001]` | TBD | TabVente dernier |
| [ ] | `[BZ-FARM-HARVEST-READY-VFX-002]` | 5 | Particules plus grosses |

---

## Bloc de lancement (copier dans Bezy)

**Croix popup drop `[BZ-INV-DROP-CLOSE-POS-001]`** — Prefab Mode `Assets/Prefabs/Ui/ShopItemPopup.prefab` :

```
/prefab-ui-3phases
Task ID: [BZ-INV-DROP-CLOSE-POS-001]
Prefab: Assets/Prefabs/Ui/ShopItemPopup.prefab
Phase: 1
```

Puis `@Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_close_topright.md` (coller le bloc Phase 1). Save. List. STOP.

**Halo inventaire `[BZ-INV-HALO-SCALE-001]`** — Prefab Mode `Assets/Prefabs/Ui/InventoryScreen.prefab` (pas la scène) :

```
/prefab-ui-3phases
Task ID: [BZ-INV-HALO-SCALE-001]
Prefab: Assets/Prefabs/Ui/InventoryScreen.prefab
Phase: 1
```

Puis `@Assets/Docs/Bezi/PROMPTS_Bezi_inventory_halo_scale.md` (coller le bloc Phase 1). Wait success. Phase 2 puis 3, **nouveau thread** à chaque fois.

**Placeholder Hub Plus `[BZ-FEATURES-HUB-WIP-COPY-001]`** — d’abord Move Dump → `Sprites/UI/FeaturesHub/IconeHub_ComingSoon.png` (Sprite Mode **Single**), puis Prefab Mode `FeaturesHubScreen.prefab` :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_features_hub_wip_copy.md
[BZ-FEATURES-HUB-WIP-COPY-001] Execute PHASE 1 only. STOP.
```

Prefab Mode **`Assets/Prefabs/Ui/InventoryScreen.prefab`**, puis **d’abord** l’ordre onglets :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_inventory_tab_recoltes_second.md
[BZ-INV-TABS-003] Reorder harvest tab ONLY. STOP.
```

Ensuite Close HUD (Ph.1 même prefab, **nouveau thread**) :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md
[BZ-HUD-CLOSE-STRIP-001] Phase 1 ONLY. STOP.
```

**Modales Bottom 260 `[BZ-HUD-MODAL-SAFE-BOTTOM-001]` :** **CLOS Bezy + Cursor** 2026-09-16 — Shop Ph.1–2, Hub 3a, Vente 3b (extras warp retirés), Inventaire 3c. `UIManager.NavBarHeight = 260f`. Pas FirstLvl.

**Relink sprites nav `[BZ-NAV-TABS-SPRITE-FOLDER-001]` :** OK 2026-09-15.

**Nav 5ᵉ onglet coupé `[BZ-NAV-TAB5-CLIP-001]` :** prompt `Assets/Docs/Bezi/PROMPTS_Bezi_nav_tab5_clip_fix.md` (scène ; Cursor rebuild HLG insuffisant).

**Hauteur fond barre = 140 px `[BZ-NAV-BAR-HEIGHT-001]` :** **CLOS** Bezy + playtest 2026-09-16. `UIManager.NavBarHeight` reste 260. Checkpoint : `hud-bandeau-before-fond140-slots128`.  
**Onglets `[BZ-TAB-SPRITES-001]` :** `@Assets/Docs/Bezi/PROMPTS_Bezi_tab_sprites.md` — TabVente dernier prompt.  
**Sparkle récolte `[BZ-FARM-HARVEST-READY-VFX-002]` :** `@Assets/Docs/Bezi/PROMPTS_Bezi_harvest_ready_vfx.md` Phase 5.  
**Glow cible `[BZ-FARM-PLANT-SELECT-GLOW-001]` :** **clos** 2026-09-10.  
**Nest HUD :** skip — Cursor 2026-09-07.  
**Bandeaux vente / sac / halo / modales / nav 140 :** clos — voir `ARCHIVE_prompts_bezi_index.md`.

---

## Terminé

| Task ID | Prefab | Phase | Date | Commit / note |
|---------|--------|-------|------|----------------|
| `[BZ-INV-WALLET-NAV-BAND-001]` | `InventoryScreen.prefab` | 1 | 2026-09-16 | chip Y -190 ; WalletBar 0 ; ScrollView -16 |
| `[BZ-NAV-BAR-HEIGHT-001]` | `NavigationHUD.unity` | scène | 2026-09-16 | fond 140 only ; tabs / Icon / frame inchangés |
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
