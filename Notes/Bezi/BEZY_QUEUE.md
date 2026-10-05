# File Bezy — jobs à exécuter dans Unity

**Usage :** Cursor remplit les lignes `[ ]` (prep async). L’auteur exécute dans Unity + Bezy, puis passe en `[x]`.

**Étude complète :** `Notes/Bezi/ETUDE_prompts_bezi_distance.md`  
**Skill :** `/prefab-ui-3phases` (prefab) · scène : `@Assets/Docs/Bezi/PROMPTS_*.md` — `Notes/Bezi/BEZY_PROMPT_COPYPASTE.md`  
**À faire (index) :** `Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md`  
**Prompts `@` Unity (index minimal) :** `Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md` — pas de fichier pour jobs clos.  
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
| [x] | `[BZ-FARM-PLANTING-CURSOR-ANIM-001]` | **5** | Livré Bezy 2026-10-03 — `PlantPlacementCursor` sur `Biofiltre.prefab` (scale 0.75, GlovePlantCursor, refs OK). |
| [x] | `[BZ-FARM-HARVEST-CURSOR-POLISH-001]` | vue | **clos 2026-10-05** — scale 0,75 + `gripOffsetLocal` auteur ; playtest PC OK. |
| [x] | `[BZ-FARM-HARVEST-CURSOR-ANIM-001]` | anim | Joue 2026-10-03 — idle armé, arrachage au clic tenu. Pivots récolte seulement. |
| [x] | `[BZ-FARM-HARVEST-BATCH-BTN-001]` | prefab | Bouton blanc Batch câblé 2026-10-01 (Ph.1–3). |
| [x] | `[BZ-FARM-SEED-CLOSE-BEFORE-PLANT-001]` | C# | **clos 2026-10-04** — playtest OK (prompt fichier supprimé ; voir `PROJECT_LOG.md`). |
| [x] | `[BZ-FARM-LAITUE-ATLAS-RELOC-002]` | 1→3 | **clos 2026-10-05** — atlas `Plantes/AtlasLaitue.png`, wiring + playtest auteur OK |
| [x] | `[BZ-FARM-HARVEST-HIDE-SYSTEM-CURSOR-001]` | C# | **clos 2026-10-05** — Bezy `RefreshSystemCursorVisibility` sur `FarmBatchHarvestCursor` |
| [x] | `[BZ-FARM-EXIT-RESET-MODES-001]` | C# | **clos 2026-10-05** — croix FirstLvl → `AbortActiveFarmInteractionModes` |
| [x] | `[BZ-FARM-CURSOR-OS-CLIP-001]` | C# | **clos 2026-10-05** — visibilité OS unifiée planting/harvest + write-on-change |
| [x] | `[BZ-FARM-ROQUETTE-ATLAS-001]` | 1→3 | 2026-10-01 — slice/pivot auteur + Bezy Ph.3 wiring OK (review Cursor) |
| — | ~~`[BZ-FARM-CAM-RESET-001]`~~ | — | **Annulé 2026-10-04** — pan/zoom/molette suffisent ; pas de bouton reset. |

*(Voir Terminé + backlog ci-dessous.)*

### Backlog — rework (hors playtest file)

| Task ID | Notes |
|---------|--------|
| `[BZ-UIKIT-POPUP-MOCK-001]` | Mockup `ShopItemPopup_WoodMockup` **à refaire entièrement** (2026-09-23 : base visible, qualité insuffisante). Pas runtime. Reprendre Ph.1→3 ou nouveau brief auteur avant Bezy. |
| `[BL-FARM-HARVEST-BATCH-BTN-UI-001]` | Polish bouton récolte batch (placeholder blanc) — **après refonte UI bois** + **mockup Cursor** dans Dump (`Ui/Farm/`). Wiring OK `[BZ-FARM-HARVEST-BATCH-BTN-001]`. |

---

## Bloc de lancement (copier dans Bezy)

**Halo inventaire `[BZ-INV-HALO-SCALE-001]`** — Prefab Mode `Assets/Prefabs/Ui/InventoryScreen.prefab` (pas la scène) :

```
/prefab-ui-3phases
Task ID: [BZ-INV-HALO-SCALE-001]
Prefab: Assets/Prefabs/Ui/InventoryScreen.prefab
Phase: 1
```

Puis `@Assets/Docs/Bezi/PROMPTS_Bezi_inventory_halo_scale.md` (coller le bloc Phase 1). Wait success. Phase 2 puis 3, **nouveau thread** à chaque fois.

**Modales Bottom 260 `[BZ-HUD-MODAL-SAFE-BOTTOM-001]` :** **CLOS Bezy + Cursor** 2026-09-16 — Shop Ph.1–2, Hub 3a, Vente 3b (extras warp retirés), Inventaire 3c. `UIManager.NavBarHeight = 260f`. Pas FirstLvl.

**Relink sprites nav `[BZ-NAV-TABS-SPRITE-FOLDER-001]` :** OK 2026-09-15.

**Nav 5ᵉ onglet coupé `[BZ-NAV-TAB5-CLIP-001]` :** prompt `Assets/Docs/Bezi/PROMPTS_Bezi_nav_tab5_clip_fix.md` (scène ; Cursor rebuild HLG insuffisant).

**Hauteur fond barre = 140 px `[BZ-NAV-BAR-HEIGHT-001]` :** **CLOS** Bezy + playtest 2026-09-16. `UIManager.NavBarHeight` reste 260. Checkpoint : `hud-bandeau-before-fond140-slots128`.  
**Onglets `[BZ-TAB-SPRITES-001]` :** `@Assets/Docs/Bezi/PROMPTS_Bezi_tab_sprites.md` — TabVente dernier prompt.  
**Glow cible `[BZ-FARM-PLANT-SELECT-GLOW-001]` :** **clos** 2026-09-10.  
**Bandeaux vente / sac / halo / modales / nav 140 :** clos — voir `ARCHIVE_prompts_bezi_index.md`.

---

## Terminé

| Task ID | Prefab | Phase | Date | Commit / note |
|---------|--------|-------|------|----------------|
| `[BZ-TAB-SPRITES-001]` | `NavigationHUD` | pilotes | 2026-09-23 | Playtest OK — 5 onglets + HUD nav |
| `[BZ-FARM-HARVEST-READY-VFX-002]` | `HarvestReadyFx.prefab` | 5–5f | 2026-09-23 | Playtest OK — sparkle récolte (Bezy tune) ; **retiré file active 2026-10-05** |
| `[BZ-FARM-LAITUE-ATLAS-RELOC-002]` | `AtlasLaitue.png` + refs | 1→3 | 2026-10-05 | Clos auteur — PPU 33, 7 stades, wiring laitue |
| `[BZ-FARM-CAM-TOWN-PAN-001]` | C# farm cam | — | 2026-09-29 | Long press pan + tap relâche — playtest APK OK |
| `[BZ-FARM-CAMERA-TOUCH-ZOOM-001]` | `FirstLvl` + C# farm cam | 1→2 | 2026-09-29 | Pinch zoom — inclus playtest APK 2026-09-29 |
| `[BZ-FARM-CAMERA-VIEW-001]` | `Biofiltre.prefab` + `FirstLvl` | 1→2 | 2026-09-23 | Bezy + playtest OK — bounds + FarmCameraController |
| `[BZ-INV-WALLET-NAV-BAND-002]` | `InventoryScreen.prefab` | 2 | 2026-09-23 | Playtest OK — chip devant nav, Y -40, sorting 60 |
| `[BZ-SHOP-FILIGRANE-001]` | `ShopScreen.prefab` | 1→2 | 2026-09-23 | Playtest OK — filigrane + sans voile SlotsGrid |
| `[BZ-FEATURES-HUB-WIP-COPY-001]` | `FeaturesHubScreen.prefab` | 1 | 2026-09-23 | Playtest OK — En construction / Bientôt disponible |
| `[BZ-HUD-CLOSE-STRIP-001]` | Inventaire · Shop · Vente | 1→3 | 2026-09-23 | Playtest OK — plus de CloseButton écran |
| `[BZ-INV-TABS-003]` | `InventoryScreen.prefab` | 1 | 2026-09-23 | Playtest OK — ordre Tout · Récoltes · Graines · Consommables |
| `[BZ-INV-DROP-CLOSE-POS-001]` | `ShopItemPopup.prefab` | 1 | 2026-09-23 | Playtest OK — retiré de la file |
| `[BZ-INV-WALLET-NAV-BAND-001]` | `InventoryScreen.prefab` | 1 | 2026-09-16 | chip Y -190 ; WalletBar 0 ; ScrollView -16 |
| `[BZ-NAV-BAR-HEIGHT-001]` | `NavigationHUD.unity` | scène | 2026-09-16 | fond 140 only ; tabs / Icon / frame inchangés |
| `[BZ-AP-HUD-SCALE-001]` | `ActionPointsHudWidget` + `NavigationHUD` | 1–3 | 2026-09-10 | 480×120, fonts ×2, instance scène OK — playtest auteur |

---

## Session remote — checklist rapide

1. `git pull`
2. Ouvrir prefab (Prefab Mode si Ph. 3+)
3. Nouveau thread Bezy → bloc ci-dessus + `@` prompt
4. Keep → `git diff` → commit → cocher `[x]` ici
