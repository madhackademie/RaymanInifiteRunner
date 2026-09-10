# SPEC UI — Onglets nav : icônes simples + zoom actif (mockup auteur)

**Création :** 2026-09-10  
**Statut :** **validé direction auteur** — implémentation après promo sprites + Bezy  
**Tâches :** `[P0-TAB-SPRITES-001]` / `[BZ-TAB-SPRITES-001]` · `[BL-UI-FEATURES-HUB-001]` (5ᵉ onglet Plus)  
**Art :** `Notes/Art/PROMPT_generation_icones.md` § **H-nav mockup 2026-09-10**  
**Réf. mockup :** déposer captures auteur dans `Assets/Art/Assets Store Dump/Ui/Nav/ref_mockup_tabs_inactif.png` + `ref_mockup_tabs_actif_zoom.png`

---

## 1) Intention (mockup)

| État | Comportement visuel |
|------|---------------------|
| **Inactif** | Icône **simple**, **grisée**, remplit la **case** de la barre pierre ; pas de label ; pas de halo ; même taille pour les 5 onglets. |
| **Actif** | Icône **couleur**, **zoom** (~1,2–1,35×), **dépasse** vers le haut de la barre ; **lueur** chaude derrière ; **cadre or** sur la case ; **label** (TMP) sous l’icône, jaune/or, gras. |
| **Press** | Conserver `NavTab` animator (léger squash) — ne pas remplacer par rotation / carousel. |

**Abandonne** pour ce chantier : cadre actif seul sans zoom (livraison Bezy 2026-09-09) ; icônes iso diorama détaillées en barre.

**Inspiration mockup :** barre pierre sombre, glyphes lisibles, onglet actif « héros » (sac à dos / inventaire sur la ref active).

---

## 2) Les 5 onglets barre (cible)

| # | Onglet | Label actif (FR) | Symbole icône (simple) |
|---|--------|------------------|-------------------------|
| 1 | Aventures | `Aventures` | Serre / play (hub ferme) |
| 2 | Inventaire | `Inventaire` | Sac à dos : testeur pH (LCD) + fanes carotte sortant des poches |
| 3 | Shop | `Shop` | Étaloir / boutique |
| 4 | Vente | `Vente` | Billet / pièces (carré) |
| 5 | Plus | `Plus` | Grille 2×2 ou « … » |

Quêtes, Atelier, Mail → **écran Plus** (`SPEC_features_hub_plus.md`), pas la barre.

**Option plus tard :** onglet central Aventures légèrement plus large (comme 1er mockup) — **hors V1** sauf demande auteur.

---

## 3) Hiérarchie cible par onglet (Bezy)

Remplacer / enrichir la structure actuelle (`SelectedFrame` + `Icon` stretch seul) :

```
TabX (Button + Animator NavTab + Image fond slot pierre optionnel)
├── SelectedFrame          (bordures or — actif seulement)
├── Glow                   (Image soft, additive ou alpha faible — actif seulement)
├── IconLift               (RectTransform — scale + offset Y pilotés par script)
│   └── Icon               (Image, Preserve Aspect, sprite couleur)
└── Label                  (TMP — actif seulement, ancré bas du slot ou sous IconLift)
```

**Règles layout :**

- **Inactif :** `IconLift` scale **1**, pos Y **0** ; `Label` **off** ; `Glow` **off** ; `Icon.color` ≈ RGB **0,45–0,55** (gris, pas tint orange).
- **Actif :** `IconLift` scale **~1,25–1,35**, pos Y **+12 à +24 px** (sort du bandeau) ; `Label` **on** ; `Glow` **on** ; `Icon.color` **blanc** (couleurs sprite).
- **NavBarContainer** : pas de `RectMask2D` qui coupe le zoom — ou icône active rendue au-dessus d’un sibling `NavBarIconsOverlay` (à trancher en Phase 1 Bezy).
- Hauteur barre : envisager **128 px** (au lieu de 120) pour la marge du débordement.

---

## 4) Cursor — `NavigationHUD.cs`

Étendre `ApplyTabVisual` (ou petit helper `NavTabVisualState`) :

| Champ SerializeField (par onglet) | Rôle |
|-----------------------------------|------|
| `Image tabXIcon` | déjà présent |
| `GameObject tabXSelectedFrame` | déjà présent |
| `RectTransform tabXIconLift` | **nouveau** |
| `GameObject tabXGlow` | **nouveau** |
| `TMP_Text tabXLabel` | réactiver wiring (existait, était off) |

**Constantes suggérées :** `inactiveIconGray = 0.5f`, `activeIconScale = 1.3f`, `activeIconLiftY = 18f`, transition optionnelle `0.15s` (plus tard).

**Ne pas** teinter l’icône active en orange — seulement gris inactif / blanc actif (mockup).

---

## 5) Art — une sprite couleur par onglet

- **Un PNG** par onglet (couleurs vives), fond **transparent**, glyphe **~90 %** du carré export **256×256** (ou 192).
- **Inactif = gris** côté **Unity** (`Image.color`), pas besoin de doublon gray PNG sauf si le rendu te déplaît.
- Style : **symbole unique**, lisible à 72 px, proche mockup **pierre / casual mobile** — **pas** diorama iso 3/4.
- Prompts : § **H-nav mockup 2026-09-10** dans `PROMPT_generation_icones.md`.

---

## 6) Bezy — phases (après sprites promo)

| Phase | Contenu |
|-------|---------|
| **1** | Hiérarchie `IconLift` / `Glow` / `Label` sur **un** pilote (`TabInventaire`) |
| **2** | Images, TMP, couleurs glow, ancres débordement haut |
| **3** | Dupliquer sur 4 autres onglets + wiring `NavigationHUD` |
| **4** | (Cursor) brancher scale/label/glow dans `ApplyTabVisual` |

Gate : même règle que `PROMPTS_Bezi_tab_sprites.md` — prompts validés avant envoi.

---

## 7) Ordre de travail

1. Générer **5 icônes** (Dump → promo `Sprites/UI/Nav/Tabs/`)  
2. Auteur : import Single + Trim + test assignation sur un onglet  
3. Bezy pilote **TabInventaire** (structure mockup actif)  
4. Cursor : logique zoom + label + glow  
5. Bezy : rollout 4 autres tabs  
6. Playtest mobile 5 onglets  
7. Wallet `[P0-NAV-WALLET-REG-001]` à part  

---

## 8) Polish backlog — cadre bois + truite (auteur 2026-09-10)

**V0 actuel :** `Glow` (Image Knob jaune, alpha ~0,45) + zoom icône.

**Cible polish :** `[BL-UI-NAV-TAB-FRAME-001]` — GameObject **`TabActiveWoodFrame`** (ou sprite 9-slice) activé avec l’onglet sélectionné :

- Cadre **bois** autour de la cellule (pas les 4 barres `SelectedFrame` legacy).
- **Truite** (ou mascotte poisson) **centrée en haut** du cadre.
- Même pipeline que `Glow` : `SetActive` depuis `NavigationHUD` quand l’onglet est actif.
- Réutilisable sur les 5 onglets après rollout Bezy.

---

## 9) Liens

- `Notes/Ui/SPEC_features_hub_plus.md` — contenu onglet Plus  
- `Notes/Ui/PROMPTS_Bezi_tab_sprites.md` — à mettre à jour après cette spec  
- `Assets/Animations/UI/NavTab.controller` — press à préserver  
