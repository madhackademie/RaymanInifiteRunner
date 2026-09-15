# Prompt art — Hub Plus : bandeau + fanion (patron vente)

**IDs :** `[BL-UI-FEATURES-HUB-001]` · `[BL-ART-HUB-BANDEAU-001]`  
**UI ref code :** `SaleChannelBandeauView` — hauteur **180 px**, illustration **80×80**, fond Image (couleur cartoon V0).  
**Hub ref :** `FeaturesHubScreen.prefab` — fanion WIP `IconeHub_WipCone_en.png` (~**1:2** vertical, pivot haut).  
**Style :** bois rustique + cartoon mobile (aligné barre nav mockup + icônes §1 `PROMPT_generation_icones.md`).  
**Dump :** `Assets/Art/Assets Store Dump/Ui/FeaturesHub/`  
**Promo :** `Assets/Art/Sprites/UI/FeaturesHub/`

---

## 1) Réflexion — ce qu’on dessine (et ce qu’on ne dessine pas)

### Parallèle vente

L’écran **Vente** = liste **scroll vertical** de **bandeaux horizontaux** (titre, étoiles, vignette carrée, overlay verrouillé / cooldown).  
Le hub **Plus** reprend cette **grammaire** : une ligne = une feature (Mail, Quêtes, Craft, Vote roadmap, Options…).

### Rôle du fanion (« flag »)

- **Fanion** = repère visuel **vertical** à gauche (lisible en scroll, comme un onglet physique).
- **Bandeau** = **barre horizontale** sous le fanion (ou derrière) : zone titre + badge + contenu — même lecture que un canal de vente.
- **Ne pas** mettre le nom de la feature en anglais dans l’art final (sauf WIP) : Unity affiche le **TMP** ; le fanion porte seulement un **glyphe** ou reste **neutre**.

### Découpage assets (recommandé)

| Asset | Usage Unity | Ratio cible |
|-------|-------------|-------------|
| **HUB-B1** `BandeauHub_chrome_9s.png` | Image **Sliced** fond de ligne (tous les bandeaux) | **4:1** paysage (~1280×320) |
| **HUB-F0** `FanionHub_neutre.png` | Fanion vide (tint / swap icône enfant) | **1:2** (~512×1024) |
| **HUB-F*** | Fanions par feature (glyphe dans le tissu) | même silhouette que F0 |
| **HUB-V*** | Vignettes bandeau (optionnel) | **1:1** 256 ou 512 (comme illustration vente, plus grande si besoin) |

Générer **d’abord B1 + F0** ; valider en UI ; puis une série **HUB-F1…F6** (notifications, mail, vote, craft, quêtes, options).

### Zones sûres (bandeau B1)

```
     [fanion ~25% largeur, déborde au-dessus]
┌────┬──────────────────────────────────────────┐
│    │  TITRE (TMP)          badges / étoiles   │  ← marge haute 12%
│pole│  [vignette 80–96px]    texte secondaire  │  ← centre
│    │                                          │  ← marge basse 12%
└────┴──────────────────────────────────────────┘
     ↑ bord gauche réservé au mât (ne pas mettre de détail critique)
```

- **9-slice** : bords droits **plats** sur ~**18 %** de chaque côté (bois / pierre) pour `Image Type = Sliced`.
- **Centre** : panneau plus clair (parchemin / planche) pour le texte.
- **Fond PNG transparent** hors du panneau (pas de rectangle blanc plein écran).

### États (V1 — optionnel)

- **Normal** : couleurs ci-dessus.
- **Sélectionné** : même fichier + **tint or** en Unity, **ou** variante `BandeauHub_chrome_selected_9s.png` (bordure dorée plus marquée).
- **Verrouillé** : pas d’asset dédié V0 — overlay gris + cadenas en UI (comme vente).

---

## 2) Prompt — HUB-B1 bandeau chrome (9-slice)

**Référence visuelle :** bandeau vente (fond bleu cartoon) + barre nav pierre/bois mockup.  
**Sortie :** 1 PNG, **1280×320 px**, transparent hors forme.

```
2D UI game asset, horizontal feature row panel for a mobile farming game hub menu, cartoon cozy style.

SHAPE: a wide horizontal wooden banner bar, landscape 4:1 aspect ratio (1280x320 pixels). Flat front view, NOT isometric. The left 22% is a slightly darker wooden vertical strip (flag pole mount area) with a simple round wooden peg at the top. The main panel is a light tan parchment-colored wooden board with thick rustic light-brown wood borders on top and bottom.

STYLE: thick dark brown outlines, smooth cel shading, vibrant but warm colors, same family as casual Township-like UI (wood + stone), NO photoreal, NO 3D render look, NO text, NO letters, NO numbers, NO icons inside the panel.

9-SLICE FRIENDLY: perfectly straight left and right edge borders (vertical wood beams) occupying about 18% width on each side; top and bottom borders are straight horizontal planks about 18% height; center fill is a simple flat parchment texture with minimal detail (no busy grain in the center).

COMPOSITION: leave the center 60% mostly empty and clean for overlaid UI text. Small decorative corner nails on the four inner corners only. Subtle shadow under the bottom edge (soft, inside the sprite, not a huge drop shadow on transparency).

BACKGROUND: fully transparent outside the banner silhouette.

NO characters, NO plants, NO mail envelope, NO quest scroll — chrome only.
```

**Import Unity :** Sprite Mode Single, Mesh Type Full Rect, **Pixels Per Unit 100**, Border L/R/T/B ≈ **230, 230, 58, 58** (ajuster au visuel), Filter Bilinear.

---

## 3) Prompt — HUB-F0 fanion neutre (patron)

**Sortie :** 1 PNG, **512×1024 px**, transparent hors fanion.  
**Alignement :** mât sur le **bord gauche** du sprite (pivot Unity **(0, 1)** haut-gauche recommandé).

```
2D UI game asset, vertical pennant flag for a mobile farming game menu, cartoon cozy style.

SHAPE: tall narrow pennant flag, portrait 1:2 aspect ratio (512x1024 pixels). A short wooden flag pole on the FAR LEFT edge (8% width), vertical, with a round wooden finial on top. The flag cloth hangs from the pole and forms a soft triangular swallowtail bottom (two subtle points), NOT a sharp medieval shield.

STYLE: thick dark brown outlines, smooth cel shading, rustic light-brown wood pole, flag cloth in warm cream / light beige fabric with a thin darker stitched border. Empty center — NO symbol, NO text, NO icon, NO badge.

FABRIC: gentle fold curves suggesting cloth, but keep the middle 70% flat enough for a future small icon overlay.

BACKGROUND: fully transparent outside pole and cloth.

Flat UI view, NOT perspective, NOT waving in wind dramatically, NOT isometric.
```

---

## 4) Prompt commun — fanion avec glyphe (HUB-F1…F6)

Remplacer `[GLYPH SUBJECT]` et le nom de fichier. **Garder exactement la même silhouette** que HUB-F0 (regénérer avec image ref F0 si possible).

```
Same pennant flag layout and proportions as the reference image (wood pole on far left, swallowtail bottom, 512x1024). Keep pole and cloth border identical.

Add ONE simple centered cartoon glyph painted on the flag cloth: [GLYPH SUBJECT]. Glyph uses thick outlines, 2-3 flat colors max, readable at 64px width. NO text words.

Transparent background outside flag. Flat UI, cozy farming mobile game style.
```

| ID | Fichier suggéré | `[GLYPH SUBJECT]` |
|----|-----------------|-------------------|
| F1 | `FanionHub_Notifications.png` | a golden bell with a tiny dot (unread) |
| F2 | `FanionHub_Mailbox.png` | a small wooden mailbox with letter sticking out |
| F3 | `FanionHub_RoadmapVote.png` | a hand putting a ballot into a wooden box |
| F4 | `FanionHub_Craft.png` | a hammer and wrench crossed over a gear |
| F5 | `FanionHub_Quests.png` | a scroll with a star seal (no readable text on scroll) |
| F6 | `FanionHub_Settings.png` | a cog wheel with a small slider lever |

---

## 5) Prompt — vignette bandeau (optionnel, comme illustration vente)

Carré **512×512**, fond transparent ou très léger, scène **simple** (pas paysage entier).

Exemple Quêtes :

```
2D casual mobile game UI vignette, square 512x512, cartoon farming style, thick outlines. A wooden clipboard with three colorful checkmarks and a small star badge — NO readable text, NO letters. Isolated, transparent background, soft shading, cozy UI illustration for a quest log row (not a full scene).
```

---

## 6) Checklist auteur

1. Générer **B1** + **F0** → Dump.  
2. Composer dans Unity sur une ligne test **180 px** haut (bandeau) + fanion **Preserve Aspect**, pivot haut-gauche.  
3. Valider lisibilité à **390 px** large écran.  
4. Regénérer fanions F1–F6.  
5. Promo vers `Sprites/UI/FeaturesHub/` après OK.  
6. Bezy : prefab `FeatureHubBandeauView` calqué sur `SaleChannelBandeauView` (sans étoiles vente si inutile).
