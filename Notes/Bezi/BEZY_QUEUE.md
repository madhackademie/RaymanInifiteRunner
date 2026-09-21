# File Bezy — jobs à exécuter dans Unity

**Usage :** Cursor remplit les lignes `[ ]` (prep async). L’auteur exécute dans Unity + Bezy, puis passe en `[x]`.

**Étude complète :** `Notes/Bezi/ETUDE_prompts_bezi_distance.md`  
**Skill :** `/prefab-ui-3phases` (prefab) · scène : `@Assets/Docs/Bezi/PROMPTS_*.md` — `Notes/Bezi/BEZY_PROMPT_COPYPASTE.md`  
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
| [x] | `[BZ-SALE-BANDEAU-VOISIN-ART-001]` | `SaleChannelBandeauView.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_voisinage_art.md` | branche courante | OK YAML 2026-09-21 — stretch + sibling 0. Cursor a retiré extras `UiQuadWarpMeshEffect` sur StarRow |
| [x] | `[BZ-SALE-BANDEAU-VOISIN-ART-001]` | `SaleChannelsScreen.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_voisinage_art.md` | branche courante | OK YAML 2026-09-21 — sprite Voisinage blanc a=1. Cursor a retiré 30× warp extras |
| [x] | `[BZ-SALE-BANDEAU-VOISIN-ART-001]` | `SaleChannelBandeauView.prefab` | 3 | `Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_voisinage_art.md` | branche courante | OK YAML 2026-09-21 — `TitleLabel_Voisinage_Outline.mat`. Cursor warp extras retirés |
| [x] | `[BZ-SALE-BANDEAU-LAYOUT-001]` | `SaleChannelsScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_layout_filigrane.md` | branche courante | OK YAML — `BandeauxContent` padding top **56** (playtest gap sous PA) |
| [x] | `[BZ-SALE-BANDEAU-VOISIN-FILIGRANE-001]` | `SaleChannelBandeauView.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_layout_filigrane.md` | branche courante | OK YAML 2026-09-21 — `Illustration` `Preserve Aspect` OFF (Cursor) |
| [x] | `[BZ-SALE-BANDEAU-VOISIN-FILIGRANE-001]` | `SaleChannelsScreen.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_layout_filigrane.md` | branche courante | OK YAML 2026-09-21 — Voisinage `m_Color.a` **0.33** + stretch template (Preserve Aspect OFF). Runtime `0.33f` |
| [ ] | `[BZ-FARM-CAMERA-VIEW-001]` | `Biofiltre.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_view.md` | `feature/plant-harvest-zoom` | Add `BiofiltreViewBounds` + Init IBC/grille. Pas de C#. Branche avant Bezy. |
| [ ] | `[BZ-FARM-CAMERA-VIEW-001]` | `FirstLvl.unity` Main Camera | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_view.md` | `feature/plant-harvest-zoom` | Add `FarmCameraController`, `enableTouchCamera=false`. Après Ph.1. |
| [x] | `[BZ-SHOP-FILIGRANE-001]` | `ShopScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_shop_filigrane.md` | `main` | OK YAML — ShopFiligrane sibling 0 α 0.30. Voile = SlotsGrid Image + C# ApplyContentPanel 0.995 |
| [ ] | `[BZ-SHOP-FILIGRANE-001]` | `ShopScreen.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_shop_filigrane.md` | `main` | Disable Image sur SlotsGrid (pas un GO Backdrop) |
| [x] | `[BZ-INV-SAC-FILIGRANE-001]` | `InventoryScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_sac_filigrane.md` | `main` | OK YAML 2026-09-21 — SacFiligrane sibling 0. α 0.50 trop visible → Cursor α **0.12** (Commerce). Tweak Play Mode = clone `InventoryScreen(Clone)`, pas le prefab asset |
| [x] | `[BZ-UIKIT-POPUP-MOCK-001]` | `ShopItemPopup_WoodMockup.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_wood_mockup.md` | `main` | OK 2026-09-19 — Card 480×760 padding 48/48/56/48. Original intact |
| [x] | `[BZ-UIKIT-POPUP-MOCK-001]` | `ShopItemPopup_WoodMockup.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_wood_mockup.md` | `main` | **Cursor** 2026-09-19 — Bezy noop (a lu « composants déjà là »). Sprites kit branchés YAML |
| [ ] | `[BZ-UIKIT-POPUP-MOCK-001]` | `ShopItemPopup_WoodMockup.prefab` | 3 | `Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_wood_mockup.md` | `main` | Tailles IconFrame/CTA/qty (après P2 OK) |
| [ ] | `[BZ-INV-DROP-CLOSE-POS-001]` | `ShopItemPopup.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_close_topright.md` | branche courante | Croix drop : Root bas-centre → Card haut-droit |
| [x] | `[BZ-INV-HALO-SCALE-001]` | `InventoryScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_halo_scale.md` | `fix/farm-iso-footprint-hit` | OK 2026-09-17 — Layout 400/560/flex0. Extra hors P1 : refHeight instance 315 (à corriger P2) |
| [x] partiel | `[BZ-INV-HALO-SCALE-001]` | `InventoryScreen.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_halo_scale.md` | `fix/farm-iso-footprint-hit` | OK 2026-09-18 — ref 320 + constrain HaloSlots. Scale 0.5 encore là → P3 |
| [x] | `[BZ-INV-HALO-SCALE-001]` | `InventoryScreen.prefab` + nested | 3 | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_halo_scale.md` | `fix/farm-iso-footprint-hit` | OK 2026-09-18 — source ref 320, layout 560. Playtest auteur au top |
| [ ] | `[BZ-FEATURES-HUB-WIP-COPY-001]` | `FeaturesHubScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_features_hub_wip_copy.md` | branche courante | Placeholder WIP : ComingSoon + En construction (pas bandeau atelier) |
| [ ] | `[BZ-INV-TABS-003]` | `InventoryScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_tab_recoltes_second.md` | branche courante | Récoltes entre Tout et Graines |
| [ ] | `[BZ-HUD-CLOSE-STRIP-001]` | `InventoryScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md` | branche courante | Delete Header/CloseButton |
| [ ] | `[BZ-HUD-CLOSE-STRIP-001]` | `ShopScreen.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md` | branche courante | Delete CloseButton |
| [ ] | `[BZ-HUD-CLOSE-STRIP-001]` | `SaleChannelsScreen.prefab` | 3 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md` | branche courante | Delete Header/CloseButton |
| [x] skip | `[BZ-FARM-BIOHUD-NEST-001]` | `Biofiltre.prefab` | — | Cursor a nesté 2026-09-07 | `main` | Plus besoin Bezy |
| [x] | `[BZ-TAB-MORE-001]` | `NavigationHUD.unity` | 3 | `Assets/Docs/Bezi/PROMPTS_Bezi_tab_more_option.md` | `main` | P1–P3 OK 2026-09-15 |
| [x] | `[BZ-FEATURES-HUB-V0-001]` | `FeaturesHubScreen.prefab` | 1b | `Assets/Docs/Bezi/PROMPTS_Bezi_features_hub_v0_placeholder.md` | `main` | P1+P1b OK 2026-09-15 — wiring script Cursor |
| [x] Ph.1 | `[BZ-HUD-MODAL-SAFE-BOTTOM-001]` | `ShopScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_modal_safe_bottom.md` | branche courante | OK 2026-09-16 — root Bottom 260, grid insets 24/24/16/0. |
| [x] Ph.2 | `[BZ-HUD-MODAL-SAFE-BOTTOM-001]` | `ShopScreen.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_modal_safe_bottom.md` | branche courante | OK 2026-09-16 — VerticalFit Unconstrained (0). |
| [x] Ph.3a | `[BZ-HUD-MODAL-SAFE-BOTTOM-001]` | `FeaturesHubScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_modal_safe_bottom.md` | branche courante | OK 2026-09-16 — root Bottom 260. PlaceholderRoot 640×720 intact. |
| [x] Ph.3b | `[BZ-HUD-MODAL-SAFE-BOTTOM-001]` | `SaleChannelsScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_modal_safe_bottom.md` | branche courante | OK 2026-09-16 — root Bottom 260. Cursor a retiré extras Bezy (`UiQuadWarpMeshEffect` nested). |
| [x] Ph.3c | `[BZ-HUD-MODAL-SAFE-BOTTOM-001]` | `InventoryScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_modal_safe_bottom.md` | branche courante | OK 2026-09-16 — root Bottom 260 + layer 5. ScrollView -108 intact. Cursor `NavBarHeight=260`. |
| [x] | `[BZ-NAV-BAR-HEIGHT-001]` | `NavigationHUD.unity` | scène | `Assets/Docs/Bezi/PROMPTS_Bezi_nav_bar_active_height.md` | `fix/farm-iso-footprint-hit` | OK 2026-09-16 — fond 140 only ; tabs inchangés ; playtest auteur OK |
| [x] | `[BZ-INV-WALLET-NAV-BAND-001]` | `InventoryScreen.prefab` | 1 | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_wallet_nav_band.md` | `fix/farm-iso-footprint-hit` | OK YAML ; playtest KO — trop bas + derrière nav |
| [ ] | `[BZ-INV-WALLET-NAV-BAND-002]` | `InventoryScreen.prefab` | 2 | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_wallet_nav_band.md` | `fix/farm-iso-footprint-hit` | Y -100 + Canvas sorting 60 |
| [ ] | `[BZ-TAB-SPRITES-001]` | `NavigationHUD` (tabs) | 1–3 TBD | `Notes/Ui/PROMPTS_Bezi_tab_sprites.md` | `main` | TabVente dernier prompt |
| [ ] | `[BZ-FARM-HARVEST-READY-VFX-002]` | `HarvestReadyFx.prefab` | 5 | `Notes/Ui/PROMPTS_Bezi_harvest_ready_vfx.md` | `main` | Particules récolte Mature+Seedling **plus grosses** (lisibilité) |
| [x] | `[BZ-FARM-PLANT-SELECT-GLOW-001]` | `LaitueObj.prefab` | 1–3 | `Notes/Ui/PROMPTS_Bezi_plant_selection_glow.md` | `main` | Livré 2026-09-10 — playtest auteur |

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
**Onglets `[BZ-TAB-SPRITES-001]` :** TabVente dernier prompt.  
**Sparkle récolte `[BZ-FARM-HARVEST-READY-VFX-002]` :** Phase 5 — peut partir sans attendre Cursor.  
**Glow cible `[BZ-FARM-PLANT-SELECT-GLOW-001]` :** Cursor livré — Bezy Ph.1–3 pour visuel prefab (jaune/blanc, scale).  
**Nest HUD :** skip — Cursor 2026-09-07.

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
