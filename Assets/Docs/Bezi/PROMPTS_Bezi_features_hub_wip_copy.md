# [BZ-FEATURES-HUB-WIP-COPY-001] Hub Plus — placeholder « en construction »

**Quoi :** écran WIP central (`WipIllustration` + `TitleLabel` + `SubtitleLabel`).  
**Pas :** bandeaux / sous-onglets atelier, `FirstTryBandeauAtelier`, spec hub long terme.  
**Prefab :** `Assets/Prefabs/Ui/FeaturesHubScreen.prefab`  
**Pas de** `/prefab-ui-3phases`. Pas de `.cs`.

Le C# `RuntimeFeaturesHubScreen` réécrit les TMP au `OnEnable` (défauts ou overrides). Bezy **doit** remplir `titleOverride` / `subtitleOverride` (sinon Play Mode revient à « Hub Plus »).

## Cibles figées

| Élément | Valeur |
|---------|--------|
| Sprite | `Assets/Art/Sprites/UI/FeaturesHub/IconeHub_ComingSoon.png` |
| Titre | `En construction` |
| Sous-titre | `Bientôt disponible` |

## Avant Bezy (auteur, ~30 s)

1. Unity Project : **Move** `Assets/Art/Assets Store Dump/Ui/IconeHub_ComingSoon.png`  
   → `Assets/Art/Sprites/UI/FeaturesHub/IconeHub_ComingSoon.png`
2. Inspector import : Texture Type **Sprite (2D and UI)** · Sprite Mode **Single** (pas Multiple) · PPU 100 · Alpha Is Transparency.
3. Prefab Mode : `Assets/Prefabs/Ui/FeaturesHubScreen.prefab`

Si le sprite manque après Move : **STOP** — ne pas pointer le Dump depuis le prefab.

---

## Phase 1 — sprite + copy + overrides

```
[BZ-FEATURES-HUB-WIP-COPY-001] PHASE 1 ONLY. OPEN Assets/Prefabs/Ui/FeaturesHubScreen.prefab. STOP.

Do NOT rescan whole project. Do NOT edit .cs, UIManager, NavigationHUD, FirstLvl. m_Layer 5.
Do NOT add bandeaux / sub-tabs / CloseButton. Do NOT Set Native Size.
Do NOT add Mask. Playtest auteur will check screen overflow.

KEEP PlaceholderRoot 640x720, center 0.5/0.5.

WipIllustration (Image comingSoonIcon):
- Sprite = Assets/Art/Sprites/UI/FeaturesHub/IconeHub_ComingSoon.png
- Preserve Aspect ON, Raycast OFF
- Anchors/pivot top-center (0.5,1)/(0.5,1)
- Pos 0,0 ; size 480x480 (inside PlaceholderRoot; NOT native 1014x1382)
- Entire sprite must stay inside PlaceholderRoot (no overflow past 640x720)

TitleLabel TMP:
- text = En construction
- keep fontSize 42 Bold, gold RGB(1,0.78,0.2), center, Raycast OFF
- Pos Y -500 ; 600x60 ; anchors/pivot top-center

SubtitleLabel TMP:
- text = Bientôt disponible
- keep fontSize 32, white a=0.85, center, Raycast OFF
- Pos Y -580 ; 600x50 ; anchors/pivot top-center

RuntimeFeaturesHubScreen on ROOT:
- keep comingSoonIcon / titleLabel / subtitleLabel wiring
- titleOverride = En construction
- subtitleOverride = Bientôt disponible

Save. List sprite + 2 TMP + 2 overrides. STOP. No Play Mode.
```

---

## Lancement Bezy

Prefab Mode `FeaturesHubScreen.prefab`, nouveau thread :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_features_hub_wip_copy.md
[BZ-FEATURES-HUB-WIP-COPY-001] Execute PHASE 1 only. STOP.
```
