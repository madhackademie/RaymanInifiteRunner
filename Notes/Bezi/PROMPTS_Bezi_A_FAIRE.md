# Bezy — prompts encore à faire (index unique)

**MAJ :** 2026-09-22 — les anciens `Notes/Ui/PROMPTS_Bezi_*.md` sont supprimés (doublons / livrés).  
**Texte complet des prompts :** `Assets/Docs/Bezi/PROMPTS_Bezi_*.md` (fichier `@` dans Unity).  
**Statut opérationnel :** cocher dans `Notes/Bezi/BEZY_QUEUE.md` après chaque phase.

**Livrés / historique :** `Notes/Bezi/ARCHIVE_prompts_bezi_index.md`

---

## Priorité immédiate (file courante)

| Ordre | Task ID | Phase | Prefab / cible | Prompt `@` |
|------|---------|-------|----------------|------------|
| 1 | `[BZ-INV-DROP-CLOSE-POS-001]` | 1 | `ShopItemPopup.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_close_topright.md` |
| 2 | `[BZ-INV-TABS-003]` | 1 | `InventoryScreen.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_tab_recoltes_second.md` |
| 3 | `[BZ-HUD-CLOSE-STRIP-001]` | 1→3 | Inventaire → Shop → Vente | `Assets/Docs/Bezi/PROMPTS_Bezi_hud_screenroot_strip_close.md` |
| 4 | `[BZ-FEATURES-HUB-WIP-COPY-001]` | 1 | `FeaturesHubScreen.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_features_hub_wip_copy.md` |
| 5 | `[BZ-SHOP-FILIGRANE-001]` | 2 | `ShopScreen.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_shop_filigrane.md` |
| 6 | `[BZ-UIKIT-POPUP-MOCK-001]` | 3 | `ShopItemPopup_WoodMockup.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_shopitempopup_wood_mockup.md` |
| 7 | `[BZ-INV-WALLET-NAV-BAND-002]` | 2 | `InventoryScreen.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_inventory_wallet_nav_band.md` |

Blocs copier-coller : section **Bloc de lancement** dans `BEZY_QUEUE.md`.

---

## Farm / caméra (branche `feature/plant-harvest-zoom`)

| Task ID | Phase | Cible | Prompt `@` |
|---------|-------|-------|------------|
| `[BZ-FARM-CAMERA-VIEW-001]` | 1 | `Biofiltre.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_farm_camera_view.md` |
| `[BZ-FARM-CAMERA-VIEW-001]` | 2 | `FirstLvl.unity` Main Camera | idem |

**Cursor avant Ph.1 :** scripts `BiofiltreViewBounds` / `FarmCameraController` sur la branche feature.

---

## HUD onglets + VFX farm

| Task ID | Phase | Cible | Prompt `@` |
|---------|-------|-------|------------|
| `[BZ-TAB-SPRITES-001]` | 1–3 TBD | `NavigationHUD` | `Assets/Docs/Bezi/PROMPTS_Bezi_tab_sprites.md` |
| `[BZ-FARM-HARVEST-READY-VFX-002]` | 5 | `HarvestReadyFx.prefab` | `Assets/Docs/Bezi/PROMPTS_Bezi_harvest_ready_vfx.md` |

---

## Polish / backlog (pas P0 session)

| Task ID | Notes | Prompt `@` |
|---------|-------|------------|
| `[BZ-NAV-TAB5-CLIP-001]` | 5ᵉ onglet coupé — scène | `Assets/Docs/Bezi/PROMPTS_Bezi_nav_tab5_clip_fix.md` |
| `[BZ-NAV-WOOD-FRAME-001]` / refonte | 9-slice bois onglets — après mockups 4 onglets | `Assets/Docs/Bezi/PROMPTS_Bezi_nav_wood_frame_slice.md` |
| `[CT-FARM-BIO-SCALE-001]` | Scale mobile biofiltre | `Assets/Docs/Bezi/PROMPTS_Bezi_biofilter_mobile_scale.md` |

---

## Règle d’usage

1. Ouvrir le `.md` listé ci-dessus dans `Assets/Docs/Bezi/`.
2. Nouveau thread Bezy → `@` ce fichier + `@Notes/Bezi/RULES_bezy_code.md` si C# visuel.
3. Prefab UI : `/prefab-ui-3phases` + **une phase** par appel.
4. Cocher `BEZY_QUEUE.md` + commit prefab (auteur).
