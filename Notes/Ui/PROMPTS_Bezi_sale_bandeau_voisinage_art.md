# Prompts Bezy — art bandeau Voisinage `[BZ-SALE-BANDEAU-VOISIN-ART-001]`

**But :** brancher la scène haie + panier en **fond plein** du bandeau Voisinage. Pas la vignette 80×80. Pas un filigrane shop α 0.30.

**Prefabs :**
- Ph.1 + Ph.3 : `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab` (template unique — `[BL-SALE-BANDEAU-TPL-001]`)
- Ph.2 : `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` (instance **Voisinage** seulement)

**Sprite promu :** `Assets/Art/Sprites/UI/SaleChannels/BandeauVente_Voisinage.png`  
Dump : `Assets/Art/Assets Store Dump/Ui/Vente_voisin_bandeau_filigrane.png`

**Layers :** UI = `m_Layer: 5`  
**Ne pas rescanner tout le projet.** Pas de C#. Ne pas régénérer le sprite.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

**Hors scope :** Bandoulière, Vélo, overlays, étoiles, scripts vente, NavigationHUD.

**Prérequis Editor :** Prefab Mode sur le prefab de la phase avant l’appel.

---

## Phase 1 — Illustration stretch (template) (COPIER TEL QUEL)

```
[BZ-SALE-BANDEAU-VOISIN-ART-001] Phase 1 ONLY — Illustration full-bleed behind header. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT fork SaleChannelBandeauView. Do NOT edit SaleChannelsScreen.prefab this phase.
Do NOT assign the voisinage sprite this phase (template shared by 3 channels).
Do NOT touch LockedOverlay, CooldownOverlay, UnlockableFxAnchor, Stars, TitleLabel, Button, LayoutElement.

File ONLY:
- Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab

REQUIRED:
1) Illustration RectTransform: stretch full parent (anchors 0,0–1,1 ; offsets 0 ; pivot 0.5/0.5). NOT 80x80 centered.
2) Sibling order STRICT under SaleChannelBandeauView root:
   - index 0 = Illustration (behind)
   - index 1 = HeaderRow
   - then LockedOverlay, CooldownOverlay (keep as later siblings)
3) Illustration Image: keep component. Preserve Aspect = true. raycastTarget = false. Color white RGB 1,1,1 alpha 0 (invisible on template). Sprite stays None.
4) Keep SaleChannelBandeauView.illustrationImage wired to Illustration Image.
5) Root Image (blue) stays Button targetGraphic. Do not add Mask. Height stays 180.
6) m_Layer: 5 on Illustration.
7) Save.

Interdits: no C#; no new GO; do not put art on root Image; NOT a 0.30 shop filigrane.

Done = Save. List Illustration anchors + sizeDelta + sibling index + Image sprite/color/preserveAspect + illustrationImage still wired. STOP.
```

**Chars ~1550** — OK &lt; 3500.

### Checklist review Phase 1 (Cursor)

- [x] Illustration stretch 0,0–1,1, sibling 0
- [x] Sprite None, Color a=0, Preserve Aspect ON, raycast off
- [x] `illustrationImage` toujours câblé
- [x] HeaderRow / overlays intacts ; hauteur 180
- [x] Extra Bezy retiré : 10× `UiQuadWarpMeshEffect` sur nested `UiStarRow`

---

## Phase 2 — Sprite instance Voisinage (COPIER TEL QUEL)

```
[BZ-SALE-BANDEAU-VOISIN-ART-001] Phase 2 ONLY — sprite on Voisinage instance. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT edit SaleChannelBandeauView.prefab this phase.
Do NOT change Bandoulière or Vélo marchand instances (no sprite).
Do NOT unpack nested bandeau prefabs.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Sprite:
- Assets/Art/Sprites/UI/SaleChannels/BandeauVente_Voisinage.png

REQUIRED:
1) Nested instance named Voisinage (channelId voisinage) under BandeauxContent.
2) On that instance ONLY, Illustration Image:
   - Source Image = BandeauVente_Voisinage
   - Color white RGB (1,1,1) alpha 1.00 (override any green tint)
   - Preserve Aspect = true
   - raycastTarget = false
3) Keep TitleLabel, stars, LockedOverlay inactive, cooldown wiring.
4) Bandoulière + Vélo marchand: Illustration sprite stays None. Do not copy voisinage art.
5) Save.

Interdits: no C#; NOT a 0.30 watermark; do not assign this sprite on the shared template.

Done = Save. List Voisinage Illustration sprite path + RGBA + Bandoulière/Vélo sprite still none. STOP.
```

**Chars ~1250** — OK &lt; 3500.

### Checklist review Phase 2 (Cursor)

- [x] Voisinage Illustration = `BandeauVente_Voisinage`, blanc a=1
- [x] Bandoulière / Vélo : pas ce sprite
- [x] Nested prefabs non unpack
- [x] Extra Bezy retiré : 30× `UiQuadWarpMeshEffect` (10 par instance)

---

## Phase 3 — Titre lisible (COPIER TEL QUEL)

```
[BZ-SALE-BANDEAU-VOISIN-ART-001] Phase 3 ONLY — TitleLabel outline on art. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT reassign sprites. Do NOT move Illustration. Do NOT edit SaleChannelsScreen.
Do NOT add UiQuadWarpMeshEffect. Do NOT touch Stars / StarRow.

File ONLY:
- Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab

REQUIRED:
1) TitleLabel TMP: keep current text and left HeaderRow position.
2) Enable outline: dark brown/black, width ~0.2–0.25, so title reads over the cottage.
3) Keep TitleLabel color. Do not move Stars. HeaderRow height stays 48.
4) Save.

Interdits: no C#; no Simulate; do not restyle CooldownLabel.

Done = Save. List TitleLabel outline on/off + color + width. STOP.
```

**Chars ~850** — OK &lt; 3500.

### Checklist review Phase 3 (Cursor)

- [x] Outline TitleLabel on (`Assets/Materials/UI/SaleChannels/TitleLabel_Voisinage_Outline.mat`)
- [x] Stars / Illustration inchangés
- [x] Extra Bezy retiré : 10× `UiQuadWarpMeshEffect` sur nested StarRow (encore)

### Après Bezy (auteur)

1. Playtest hors prompt : onglet Vente → bandeau Voisinage = scène haie/panier ; titre + étoiles au-dessus ; Bandoulière/Vélo inchangés.
2. Cooldown : l’illustration doit encore griser (C# existant).

---

## Lancement Unity

Prefab Mode : `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab`

```
/prefab-ui-3phases
Task ID: [BZ-SALE-BANDEAU-VOISIN-ART-001]
Prefab: Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab
Phase: 1
```

Puis coller le bloc Phase 1, **ou** :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_voisinage_art.md
[BZ-SALE-BANDEAU-VOISIN-ART-001] Execute PHASE 1 only. Prefab Mode SaleChannelBandeauView.prefab. Illustration stretch. No sprite yet. No .cs. Save. List. STOP.
```
