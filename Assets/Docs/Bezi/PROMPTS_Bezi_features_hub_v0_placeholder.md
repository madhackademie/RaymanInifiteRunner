# [BZ-FEATURES-HUB-V0-001] FeaturesHub — écran noir « Bientôt disponible »

**V0** (pas la barre sous-onglets hub — voir `Notes/Ui/SPEC_features_hub_plus.md` plus tard).

**Script Cursor (ne pas modifier) :** `Assets/Scripts/UI/FeaturesHub/RuntimeFeaturesHubScreen.cs`

**Prefab cible :** `Assets/Prefabs/Ui/FeaturesHubScreen.prefab`

**Launch (une phase par appel) :**
```
@Assets/Docs/Bezi/PROMPTS_Bezi_features_hub_v0_placeholder.md
[BZ-FEATURES-HUB-V0-001] Execute PHASE 1 only. STOP.
```

---

## Phase 1 — Prefab shell + placeholder

```
[BZ-FEATURES-HUB-V0-001] PHASE 1 prefab ONLY. STOP.

CREATE Assets/Prefabs/Ui/FeaturesHubScreen.prefab (new).
Do NOT rescan whole project. Do NOT edit UIManager or NavigationHUD.unity. m_Layer 5.

ROOT FeaturesHubScreen:
- RectTransform stretch (0,0)-(1,1), pivot 0.5/0.5
- Image: sprite NONE, color RGBA(0.04,0.04,0.06,0.98), Raycast ON (blocks clicks behind)
- Add component RuntimeFeaturesHubScreen (script already in repo)

CHILD PlaceholderRoot (center anchor 0.5/0.5, width ~640, height ~720):
- WipIllustration (Image): max ~560x560, Preserve Aspect ON, Raycast OFF
  Sprite: Assets/Art/Sprites/UI/FeaturesHub/IconeHub_WipCone_en.png
  (If missing: STOP — author must Move+rename from Dump first.)
- TitleLabel (TMP): "Hub Plus", fontSize ~42, Bold, gold RGB(1,0.78,0.2), outline ~0.25, center, Raycast OFF
- SubtitleLabel (TMP): "Bientôt disponible", fontSize ~32, center, white alpha ~0.85, Raycast OFF
  Stack: illustration top, Title, Subtitle (~20px spacing).

CRITICAL — same modal shell as SaleChannelsScreen:
- ROOT Image MUST stay stretch full rect, alpha >= 0.95, Raycast Target ON (blocks gameplay clicks).
- Do NOT add holes or transparent full-screen areas. Icon/labels Raycast OFF only.

Wire RuntimeFeaturesHubScreen on ROOT:
- rootBackdropImage → root Image
- comingSoonIcon → WipIllustration Image
- titleLabel → TitleLabel
- subtitleLabel → SubtitleLabel

Save prefab. List hierarchy + wired fields. STOP. No Play Mode.
```

---

## Phase 1b — Composants manquants (si prefab = hiérarchie seule)

```
[BZ-FEATURES-HUB-V0-001] PHASE 1b components ONLY. OPEN existing Assets/Prefabs/Ui/FeaturesHubScreen.prefab. STOP.

Do NOT create new prefab. Do NOT edit UIManager or .cs. m_Layer 5.

ROOT FeaturesHubScreen — ADD if missing:
- CanvasRenderer + Image: stretch, sprite NONE, color RGBA(0.04,0.04,0.06,0.98), Raycast Target ON
- RuntimeFeaturesHubScreen (script guid 9cf3b2a5986c1e546bd732074f138406)

WipIllustration — ADD Image: sprite Assets/Art/Sprites/UI/FeaturesHub/IconeHub_WipCone_en.png
Preserve Aspect ON, Raycast OFF, 560x560

TitleLabel — ADD TextMeshProUGUI: "Hub Plus", fontSize 42, Bold, gold, outline, center, Raycast OFF

SubtitleLabel — ADD TMP: "Bientôt disponible", fontSize 32, center, white 0.85, Raycast OFF

Wire RuntimeFeaturesHubScreen:
rootBackdropImage→root Image, comingSoonIcon→WipIllustration, titleLabel, subtitleLabel

Save. List components added. STOP. No Play Mode.
```

---

## Phase 2 — Binding UIManager (auteur Unity, pas Bezy)

Bezy **INTERDIT** sur `UIManager` (`RULES_bezy_code.md`).

1. Ouvrir `Assets/Scenes/NavigationHUD.unity`
2. HUDRoot → **UIManager** → `Secondary Screens` → **+**
3. `screenId` = `FeaturesHub` (exact, comme `ScreenId.FeaturesHub`)
4. `prefab` = `FeaturesHubScreen.prefab`
5. Save scène → Playtest : onglet **Plus** ouvre l’écran noir + texte.

---

## Art — sprite WIP (auteur avant Bezy)

| Étape | Action |
|-------|--------|
| 1 | Créer dossier `Assets/Art/Sprites/UI/FeaturesHub/` |
| 2 | **Move** (Unity Project) depuis `Assets/Art/Assets Store Dump/wip-cone-fused-en.png` |
| 3 | Renommer **`IconeHub_WipCone_en.png`** |
| 4 | Import : Sprite 2D UI · PPU 100 · Alpha Is Transparency · Trim si besoin |

Texte FR sous l’illu = TMP Bezy (« Bientôt disponible »). L’anglais reste **dans** l’image seulement.

---

## Phase 1c — Calque opaque extra (optionnel)

```
[BZ-FEATURES-HUB-V0-001] PHASE 1c opaque ONLY. OPEN FeaturesHubScreen.prefab. STOP.

Do NOT edit .cs, UIManager, NavigationHUD. m_Layer 5.

1) ROOT Image: alpha color = 1, Raycast Target ON.

2) ADD child ModalBlocker FIRST under root (index 0, behind PlaceholderRoot):
   Stretch (0,0)-(1,1). Image white 1x1 sprite, color RGBA(0.04,0.04,0.06,1), Raycast ON.

3) Optional ContentBackdrop (stretch, index 1): color RGBA(0.07,0.07,0.09,1), Raycast ON.
   Wire RuntimeFeaturesHubScreen.contentBackdropImage if field exists.

Save. List. STOP. No Play Mode.
```

Masquage barre **PA** : déjà géré en **C#** (`NavigationHUD`) — ne pas toucher ActionPointsHudWidget.
