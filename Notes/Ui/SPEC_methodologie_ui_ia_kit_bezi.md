# Méthodologie — UI générée IA, kit 9-slice, refontes Bezy

**Création :** 2026-10-07  
**Statut :** **cadre méthodo** — l’UI runtime **reste inchangée** tant que les P0 session (popup farm, mobile) ne sont pas clos ; ce doc guide les **prochaines** générations et l’**épuration** progressive du kit.

**Lié :**
- Mockup → prefab → Bezy : `Notes/Ui/WORKFLOW_creation_ui_mockup_bezi.md`
- Skill `/prefab-ui-3phases` : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`
- Génération art + Dump : `Notes/Art/WORKFLOW_creation_assets.md`
- Prompts kit bois / 9-slice : `Notes/Ui/PROMPT_chatgpt_ui_kit_planche_bois.md`
- Responsive shell : `Notes/Ui/NOTE_validation_ui_responsive_shell_popups.md`

---

## 1) Principes

| Principe | Détail |
|----------|--------|
| **1 asset IA = 1 rôle slicable** | Pas de mockup écran complet comme source de prod ; pas de décor / icône **dans** la zone 9-slice centrale si elle doit s’étirer. |
| **Validation avant Bezy prod** | Promo `Sprites/` + bordures Sprite Editor + **1 rect de référence** testé en scène labo (`TestUi`, 1080×1920). |
| **Prefabs = composition** | Bouton = fond **Sliced** + enfant **Icon** (Simple, Preserve Aspect) + **Label** TMP — pas nested prefab kit **étiré** (leçon popup récolte 2026-10). |
| **Épuration continue** | Chaque asset validé remplace ou déprécie les doublons ; backlog `[BL-UI-KIT-AUDIT-001]`. |

---

## 2) Checklist validation — génération asset UI (IA → Dump → Sprites)

À cocher **auteur** avant promo et avant prompt Bezy.

### 2.1 Prompt / fichier source

- [ ] Vue **ortho**, pas de perspective ; pas de texte baked (TMP Unity).
- [ ] **STYLE LOCK** + **CHROMA_BG** (`Notes/Art/WORKFLOW_creation_assets.md` §3.1).
- [ ] Pour tout **chrome étirable** : phrase explicite **9-SLICE FRIENDLY** (bords droits ~15–20 % largeur/hauteur, centre **aplati** sans motif qui se répète).
- [ ] **Icônes / illustrations** : asset **séparé** (fond simple) si le fond du bouton doit stretch (cf. H-farm-1 vs H-farm-2 dans `Notes/Art/PROMPT_generation_icones.md`).

### 2.2 Détourage + import Unity

- [ ] Fond chroma retiré ; pas de frange grise (EDGE_LOCK).
- [ ] **Mesh Type : Full Rect** ; **Image Type : Sliced** sur le prefab cible.
- [ ] **Sprite Editor** : bordures L/R/T/B documentées (note ou `.meta` commentaire auteur).
- [ ] **PPU = 100** ; **Pixels Per Unit Multiplier** choisi pour la **hauteur cible** du rect (voir §3 — ne pas compresser le bois en baissant le multiplier « pour tenir »).

### 2.3 Test 9-slice (obligatoire pour boutons / barres / panneaux)

Tester sur un **RectTransform fixe** (§3) dans `TestUi` :

- [ ] Largeur **min** + largeur **max** (ex. bouton 320 px et 480 px) : coins bois **sans couture**, centre sans motif qui « tile » mal.
- [ ] Hauteur **fixe** du tier UI (ex. 96 px) : épaisseur bois **stable** vs référence mockup.
- [ ] Si KO : regénérer (H-farm-2 pattern) ou ajuster bordures — **ne pas** livrer à Bezy un sprite « presque » slicable.

### 2.4 Hors 9-slice (icônes seules)

- [ ] **Simple** + **Preserve Aspect** ; taille cible en px **canvas** (souvent 64–96 pour action, 48 min tap — skill Bezy).
- [ ] Pas de 9-slice sur glyphes ronds / outils (serpe, arrosoir — futur UX §6).

---

## 3) Grille de tailles — boutons et barres (référence projet)

**Canvas référence :** 1080×1920 portrait, PPU 100 (`NOTE_validation_ui_responsive_shell_popups.md`).

### 3.1 Tiers validés ou en cours de validation

| Tier | Usage | Rect cible (× canvas) | Exemple repo | Statut |
|------|--------|------------------------|--------------|--------|
| **Primary L** | CTA principal popup | **320 × 96** | Boutons Récolter / Arracher (mockup harvest) | **Validé** mockup 2026-10 |
| **Primary kit** | Pièce réutilisable labo | **360 × 120** | `UiKit_Piece_BtnPrimary_Teal` | **Validé** auteur 2026-10-05 |
| **Secondary oval** | Annuler, secondaire | Taille piece kit oval | `UiKit_Piece_BtnSecondary_Oval` | **Validé** |
| **Bar batch** | Fond horizontal + icône enfant | Hauteur ~**96–120**, largeur stretch | H-farm-1 / split H-farm-2 | **En épuration** |
| **Tap min** | Zone clic | **≥ 48 × 48** | Skill Bezy Ph.2 | Règle |

### 3.2 Règle multiplier / bois

Documentée dans `PROMPT_chatgpt_ui_kit_planche_bois.md` §2bis :

- Multiplier = `hauteur_sprite / hauteur_rect_cible` (ex. teal 317 px → rect 120 → **2,64** sur la piece 360×120).
- **Nouveau chrome IA :** designer le PNG pour **une hauteur cible** du tier (ex. 160 px @2x → 96 px écran avec multiplier 2), pas l’inverse.

### 3.3 À formaliser (backlog kit)

- [ ] Table unique **`UiKit_SIZES.md`** ou section ici : panneaux popup, badges, close 72×72, etc.
- [ ] **2 largeurs standard** primary (320 vs 360) — décider si on converge sur **320×96** partout popup.

---

## 4) Bouton générique — cible architecture (pas encore prod unique)

**Objectif :** un **prefab base** + **Prefab Variants** (couleur / rôle) + slot **Icon** optionnel, sans regénérer tout le bois à chaque bouton.

### 4.1 Hiérarchie cible (contrat Bezy)

```
Btn_Generic_Base (RectTransform tier §3)
├── Background   Image Sliced  (sprite 9-slice bois+centre)
├── Icon         Image Simple  (optional, Preserve Aspect, anchor left)
└── Label        TMP           (centre ou offset si icône)
```

- **Button** sur root ou sur Background (projet : conserver pattern existant des vues).
- **Pas** d’icône dans le PNG slicé du fond (split art **H-farm-2**).

### 4.2 Couleur / variantes

| Approche | Quand | Limite |
|----------|--------|--------|
| **Prefab Variant** par accent (teal, coral, orange…) | Centres teal/coral **distincts** en art | Plus de sprites, rendu fidèle charte |
| **`Image.color` tint** | Prototype rapide, centre **neutre** | Teinte aussi le bois — **déconseillé** pour le chrome bois |
| **Swap sprite** même layout | Variantes déjà en `Sprites/UI/` | Pattern actuel shop / harvest |

**Décision ouverte :** variant par **sprite centre** (teal vs coral) + même trame bois, vs un seul sprite + shader (hors scope MVP).

### 4.3 Icône

- Enfant **`Icon`** : assignation runtime ou prefab variant ; **jamais** stretch avec le fond.
- Génération IA : prompt **icône seule** 256–512 px → Dump → Sprites.

### 4.4 Livrable backlog

- `[BL-UI-KIT-GENERIC-BUTTON-001]` — prefab `UiKit_Btn_Generic_Base` + variants Primary_Teal / Danger_Coral / … + doc wiring mockup.
- Bezy : Ph.1–3 sur **un** tier 320×96 ; Cursor : pas de YAML prefab long terme.

---

## 5) Programme test & épuration (dans le temps)

| Phase | Action | ID |
|-------|--------|-----|
| **Inventaire** | Lister sprites UI `Sprites/UI/` + pieces `UiKit_*` + doublons Dump | `[BL-UI-KIT-AUDIT-001]` |
| **Retirer** | Placeholders blancs, nested kit cassés, assets abandonnés (H-farm-4…) | au fil des playtests |
| **Converger** | 1 fond 9-slice batch, 1 primary, 1 secondary | après generic button |
| **Doc** | Bordures + multiplier + rect cible **par sprite validé** | section §3 enrichie |
| **Bezy** | Re-wiring écrans un par un seulement quand mockup + asset **OK checklist §2** | workflow mockup |

**Rythme suggéré :** 1 sprite ou 1 tier bouton **par session** max (éviter refonte kit monolithique).

---

## 6) Vision UX — polish futur (hors implémentation actuelle)

**Décision auteur 2026-10-07 :** ne **pas** changer l’UI farm / popups **maintenant**. Noter la direction pour refonte **polish UX/UI** ultérieure.

### 6.1 Direction (réf. Township-like)

- **Moins de grands panneaux** « pavés » centrés ; infos **succinctes sur l’objet** (plante, bâtiment) dans le monde / au-dessus de la cible.
- **Actions contextuelles** : une **ligne d’outils en bas d’écran** (serpe, arrosoir, …) relative à l’élément cliqué (culture, construction, etc.).
- **Déroulé / tiroir** pour choisir l’action si plusieurs options (scroll horizontal ou expand).

### 6.2 Prérequis avant spec détaillée

- [ ] **Screenshots auteur** (Township ou jeu ref.) → `Assets/Art/Assets Store Dump/Ui/RefUX/` (ex. `ref_township_plant_actions.png`).
- [ ] Spec interaction : farm vs ville, coexistence avec `NavigationHUD`, `PopupId` existants.
- [ ] Prototype **mockup only** (pas runtime) dans `TestUi`.

**Backlog :** `[BL-UI-UX-CONTEXT-ACTION-BAR-001]` — voir `Notes/Todo_project.md`.

---

## 7) Où ça s’insère dans le workflow mockup → Bezy

```
Brief §2 checklist IA
    → mockup TestUi (tiers §3, generic §4)
    → validation 9-slice auteur
    → coquille prefab prod
    → Bezy /prefab-ui-3phases (Image Sliced + Icon enfant + TMP)
    → runtime Cursor (PopupId, bindings)
```

**Anti-pattern rappel :** générer un bouton « complet » (bois + gant + texte) en un PNG slicé — préférer **fond + icône enfant** (déjà pratiqué pour batch H-farm-1 / H-farm-2).
