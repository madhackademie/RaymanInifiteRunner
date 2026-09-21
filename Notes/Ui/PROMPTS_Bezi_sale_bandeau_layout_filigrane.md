# Prompts — bandeaux vente : décalage PA + filigrane Voisinage

**Série A — layout :** `[BZ-SALE-BANDEAU-LAYOUT-001]`  
**Série B — filigrane :** `[BZ-SALE-BANDEAU-VOISIN-FILIGRANE-001]`

**Contexte playtest :** sur l’écran Vente, le HUD **PA reste visible** (`UIManager.ShouldHideActionPointsHud` = false). Le premier bandeau remonte sous la barre PA. L’illustration Voisinage est **centrée** avec bandes bleues (`Preserve Aspect` + ratio ~carré).

**Layers :** UI = `m_Layer: 5`  
**Interdit Bezy :** `UiQuadWarpMeshEffect`, Simulate, rescan projet, C#.

**Succès Bezy = Save + liste. STOP.**

---

## Série A — Descendre tous les bandeaux sous le compteur PA

**Fichier :** `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` uniquement.

**Cible YAML actuelle :** `BandeauxContent` → `VerticalLayoutGroup` padding top **16**.  
**Objectif visuel :** premier bandeau **entièrement sous** `ActionPointsHudWidget` + **8 px** d’air (ajuster 48–72 px de padding top si besoin au playtest).

### Phase 1 — Padding scroll (COPIER TEL QUEL)

```
[BZ-SALE-BANDEAU-LAYOUT-001] Phase 1 ONLY — push bandeaux below PA HUD. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT edit SaleChannelBandeauView.prefab.
Do NOT change SaleChannelsScreen root RectTransform (anchoredPosition y=130, sizeDelta y=-260).
Do NOT move Header / TitleLabel "Canaux de vente".
Do NOT add UiQuadWarpMeshEffect.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

REQUIRED:
1) BandeauxContent (ScrollRect content, VerticalLayoutGroup):
   - Increase padding TOP from 16 to 56 (keep Left/Right 16, Bottom 16, Spacing 16).
2) If still tight under PA bar, optionally add Viewport (under BandeauxScrollView) RectTransform offset:
   - offsetMax y = -8 (top inset 8px). Only if padding alone is insufficient — prefer padding first.
3) Keep 3 nested SaleChannelBandeauView instances unchanged (no unpack).
4) bandeauxContainer wiring on RuntimeSaleChannelsScreen unchanged.
5) Save.

Done = Save. List BandeauxContent padding top + optional Viewport offsetMax.y. STOP.
```

---

## Série B — Illustration pleine largeur en filigrane 30 %

### Avis technique (auteur / Cursor)

| Approche | Résultat |
|----------|----------|
| **Stretch sans Preserve Aspect** (sprite actuel ~carré) | Remplit la largeur mais **écrase** le panier / iso — déconseillé en filigrane lisible. |
| **Preserve Aspect + stretch** (état actuel) | **Bandes bleues** sur les côtés — normal. |
| **Regénération paysage ultra-large** (recommandé) | Ratio **~4:1** (ex. 1792×448), sujet **centré**, haie/ciel **continus sur les bords** → stretch léger ou `Preserve Aspect` avec peu de marge. |
| **Runtime** | `SaleChannelBandeauView` remet `illustrationImage.color` en **alpha 1** à l’ouverture → après Bezy, **ticket Cursor** : alpha filigrane **0.30** sur états actif / unlockable (cooldown garde gris). |

**Pattern shop :** `Notes/Ui/PROMPTS_Bezi_shop_filigrane.md` (sibling 0, α 0.30).

### Phase 1 — Template Illustration « mode filigrane » (COPIER TEL QUEL)

```
[BZ-SALE-BANDEAU-VOISIN-FILIGRANE-001] Phase 1 ONLY — template Illustration filigrane setup. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT edit SaleChannelsScreen.prefab this phase.
Do NOT touch TitleLabel, Stars, overlays, root blue Image, Button.
Do NOT add UiQuadWarpMeshEffect.

File ONLY:
- Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab

REQUIRED:
1) Illustration: keep stretch anchors 0,0–1,1 offsets 0 sibling index 0 (behind HeaderRow).
2) Illustration Image: Preserve Aspect = OFF (false). Image Type = Simple. raycastTarget = false.
3) Template stays invisible: Sprite None, Color white RGB 1,1,1 alpha 0.
4) Keep illustrationImage SerializeField wired.
5) Save.

Done = Save. List Illustration preserveAspect + color alpha + anchors. STOP.
```

### Phase 2 — Instance Voisinage α 0.30 + stretch (COPIER TEL QUEL)

```
[BZ-SALE-BANDEAU-VOISIN-FILIGRANE-001] Phase 2 ONLY — Voisinage filigrane instance. STOP after save.

Do not rescan whole project. Do not modify C#. No Simulate.
Do NOT edit SaleChannelBandeauView.prefab this phase.
Do NOT change Bandoulière or Vélo instances.
Do NOT unpack bandeau prefabs.

File ONLY:
- Assets/Prefabs/Ui/SaleChannelsScreen.prefab

Sprite (keep):
- Assets/Art/Sprites/UI/SaleChannels/BandeauVente_Voisinage.png

REQUIRED — nested instance named Voisinage only:
1) Illustration Image override: keep sprite assigned.
2) Color RGBA = (1, 1, 1, 0.30) — filigrane like shop.
3) Preserve Aspect = OFF on this instance override.
4) Do not move Illustration RectTransform (still full stretch under bandeau).
5) TitleLabel outline + stars + root blue button unchanged.
6) Save.

Done = Save. List Voisinage Illustration color.a + preserveAspect. STOP.
```

### Phase 3 (Cursor, pas Bezy) — Alpha runtime

Après Ph.2 Bezy : patch `SaleChannelBandeauView.cs` — constante `IllustrationFiligraneAlpha = 0.3f` appliquée aux couleurs `Unlocked` / `Unlockable` (pas écraser à alpha 1). Cooldown / locked inchangés.

### Option art — regénération (ChatGPT / autre, hors Bezy)

**Quand :** si Ph.2 laisse une déformation visible ou des bords vides même sans Preserve Aspect.

**Prompt image (copier) :**

```
Bandeau UI jeu mobile, format paysage ultra-large 4:1 (1792×448 px), fond transparent ou uni très clair.
Scène cartoon isométrique douce : haie verte continue sur TOUTE la largeur, clôture bois, panier d’osier au centre avec laitue atlas cartoon, carotte, truite ; cottage et arbres en arrière-plan CENTRÉS ; ciel bleu dégradé aux extrémités.
Composition pensée pour stretch horizontal léger : détails importants au centre 40 %, côtés gauche/droite = haie + fleurs répétitives, pas de bords vides.
Style cohérent jeu casual aquaponie, couleurs saturées modérées, pas de texte, pas de cadre.
```

**Pipeline projet :** dump → `Assets/Art/Assets Store Dump/Ui/` → validation auteur → promouvoir vers `Assets/Art/Sprites/UI/SaleChannels/BandeauVente_Voisinage.png` (remplace) → playtest sans changer les prompts Bezy Ph.2.

---

## Checklist review Cursor

### Série A
- [ ] `BandeauxContent` padding top ≥ 56 (ou Viewport inset)
- [ ] Playtest : gap ~8 px sous barre PA, scroll OK

### Série B
- [ ] Template `Preserve Aspect` OFF
- [ ] Voisinage α 0.30, stretch plein bandeau
- [ ] Cursor Ph.3 alpha runtime
- [ ] Regén sprite si déformation gênante
