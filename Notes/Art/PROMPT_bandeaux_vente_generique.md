# Prompt générique — bandeaux canaux de vente (illustration fond)

**IDs :** `[W4]` · `[BZ-SALE-BANDEAU-*-ART]`  
**UI :** `SaleChannelBandeauView` — `Illustration` en **full-bleed** (pas vignette 80×80, pas filigrane shop α 0.30).  
**Spec :** `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`  
**Charte :** `RaymanFarm_UI_CartoonCel` — `Notes/Art/WORKFLOW_creation_assets.md` §3.1  
**Dump :** `Assets/Art/Assets Store Dump/Ui/`  
**Promo :** `Assets/Art/Sprites/UI/SaleChannels/` → ex. `BandeauVente_Voisinage.png`

---

## 1) Technique Unity (à respecter avant le style)

| Paramètre | Valeur |
|-----------|--------|
| Canvas réf. | **1080 × 1920** (`NavigationHUD`) |
| Hauteur bandeau | **180 px** |
| Largeur utile illustration | **~1048 px** (padding liste **16 px** L/R) |
| **Ratio cible** | **~6 : 1** paysage (1048 ÷ 180 ≈ 5,82) |
| **Pixels export IA (recommandé @2×)** | **2048 × 360** ou **1920 × 330** |
| Alternative acceptable | **1920 × 384** (5 : 1, légèrement plus haut) |
| À éviter | Carré, portrait, ou **~2,5 : 1** (anciens filigranes ~1983×793) si fond plein bandeau |

**Comportement runtime :** l’image remplit le rectangle du bandeau (`Illustration` stretch parent). Un ratio proche de **6 : 1** limite la déformation si l’Image est en stretch ; en *Preserve Aspect*, un mauvais ratio laisse des bandes.

### Zone sûre (titre + étoiles par-dessus)

```
┌────────────────────────────────────────────────────────────┐
│  TITRE TMP (gauche)              ★ ★ ★ ★ ★ (droite)      │  ← ~48 px haut, calme / lisible
│                                                            │
│         scène principale (haie, vélo, étal, etc.)          │  ← centre + bas
│                                                            │
└────────────────────────────────────────────────────────────┘
```

- **Haut ~25–30 %** : pas de détail critique, contrastes forts ni visages coupés — le TMP + outline passent dessus.
- **Bas ~10 %** : éviter texte / watermark IA.
- **Pas de texte** dans le PNG (Unity affiche « Voisinage », etc.).

### Nommage fichier

`BandeauVente_[Canal]_YYYYMMDD.png` — ex. `BandeauVente_Voisinage_20260923.png`

---

## 2) Charte graphique (fusionnée)

**Style lock :** `RaymanFarm_UI_CartoonCel`

- Illustration **2D UI mobile**, vue **de face / paysage** — **PAS** isométrique 2:1 monde, **PAS** grille jeu, **PAS** photoreal, **PAS** pixel art.
- Cartoon cozy ferme / village : couleurs **vives et chaudes**, silhouettes **chunky**, **cel shading** doux.
- Contours cartoon **épais** ; personnages et props lisibles : trait extérieur **noir net** (pas de frange grise AA sur découpes alpha si fond transparent).
- Scènes plein cadre : même famille que §1 `PROMPT_generation_icones.md` (cel + bois rustique où pertinent).
- Ambiance **Township / cozy mobile** — pas thème zombie, pas horror, pas UI bulles Tribez.
- **Refs projet (joindre en IMAGE2 si ChatGPT) :** `Dump/Ui/Tab_Plus/FirstTryBandeauAtelier.png`, `Dump/Ui/cadreBoisFinal.png`, bandeau promu `Sprites/UI/SaleChannels/BandeauVente_Voisinage.png` si itération.

**Interdits communs**

- Texte, chiffres, watermark, logo stock, QR, signature IA.
- Bandeau type « filigrane centré » à 30 % alpha (ancien shop) — ici **fond plein** ou décor qui couvre toute la largeur.
- Copier Bikini Bottom (sol cyan / bordures violettes).

---

## 3) Prompt principal (copier-coller — anglais pour les générateurs)

Remplacer **`[SCENE_THEME]`** par le canal (ex. *suburban hedge and wicker basket with fresh lettuce*, *portable vegetable stall on a country path*, *cargo bike with fish crates*).

```
TASK: ONE wide horizontal mobile-game UI banner BACKGROUND illustration for a sale channel row.

FORMAT: landscape aspect ratio about 6:1, exactly 2048x360 pixels (or 1920x330). Full-bleed artwork meant to fill a thin UI strip (~180px tall on a 1080px-wide phone canvas). Flat front view, NOT isometric, NOT 3D render.

SCENE: [SCENE_THEME]. Cozy cartoon farming / neighborhood commerce vibe. Readable at phone scale: simple shapes, clear focal point in the center-lower area.

STYLE LOCK (RaymanFarm_UI_CartoonCel): 2D casual mobile UI illustration, vibrant warm colors, thick pure black outer outlines on characters and props, NO anti-aliasing on any transparent cutout edges, smooth cel shading, rustic light wood and garden tones where relevant, high-quality UI art. Same family as a cozy Township-like mobile farm UI — NOT photoreal, NOT pixel art, NOT horror, NOT zombie theme.

COMPOSITION SAFE ZONES: keep the top ~25% calmer (soft sky, hedge top, blurred distance) for UI title overlay on the left and star icons on the right. No faces or critical detail under that band. Main story and props in the middle and lower third.

BACKGROUND: either full opaque painted sky/ground across the entire frame OR transparent outside a soft vignette — prefer FULL WIDTH color/ground so Unity stretch shows no holes. NO letterboxing bars baked into the image.

FORBIDDEN: NO text, NO letters, NO numbers, NO logos, NO watermarks, NO UI chrome (no buttons, no frames, no star icons drawn in the art).

OUTPUT: single PNG, sRGB, no metadata text overlay.
```

---

## 4) Variantes par canal (thème `[SCENE_THEME]`)

| Canal | `[SCENE_THEME]` (extrait prompt) | Fichier promo cible |
|-------|----------------------------------|---------------------|
| **Voisinage** | quiet suburban garden, trimmed hedge, wicker basket with lettuce on a garden path, friendly neighbor fence gate | `BandeauVente_Voisinage.png` |
| **Bandoulière** | colorful portable market stall with vegetables and fish on a shoulder strap stand, country lane | `BandeauVente_Bandouliere.png` (à créer) |
| **Vélo marchand** | merchant cargo bike with crates of vegetables and fish, small town street | `BandeauVente_Velo.png` (à créer) |

Un bandeau = **un** appel IA. Ne pas fusionner les trois canaux dans une seule image.

---

## 5) Repasse ChatGPT (style lock sur brouillon Comfy)

Si sortie **Krea / Comfy** d’abord :

```
IMAGE1 = brouillon bandeau (layout OK)
IMAGE2 = FirstTryBandeauAtelier.png OR cadreBoisFinal.png

Unify STYLE only to match IMAGE2: thick pure black outer outlines, NO anti-aliasing on cutout edges, cel shading, cozy cartoon mobile UI. Keep IMAGE1 composition, camera, and aspect ratio exactly. Still NO text. Output 2048x360 PNG.
```

**Comfy resize (optionnel) :** bandeau **768×128** ou **768×320** en brouillon léger 1060 → upscale + repasse ChatGPT ; export final **2048×360** avant promo.

---

## 6) Après génération

1. PNG → `Assets/Art/Assets Store Dump/Ui/BandeauVente_[Canal]_YYYYMMDD.png`
2. OK auteur → copie `Assets/Art/Sprites/UI/SaleChannels/BandeauVente_[Canal].png`
3. Unity : instance canal dans `SaleChannelsScreen.prefab` → `Illustration` **Source Image**, Color **blanc RGB 1,1,1 alpha 1** (fond plein). Cooldown = grisé par overlay runtime, pas dans le PNG.
4. Playtest : Vente → bandeau → titre lisible, étoiles, clic vente, cooldown.

---

## 7) Liens

| Doc | Rôle |
|-----|------|
| `Notes/Art/PROMPT_generation_icones.md` §3 **W4** | Backlog bandeaux vente |
| `Assets/Docs/Bezi/PROMPTS_Bezi_sale_bandeau_voisinage_art.md` | Wiring Bezy (pas regénérer le sprite) |
| `Notes/Art/PROMPT_bandeau_atelier_bricolage.md` | Bandeau hub Bricolage (ratio ~5:1, autre écran) |
