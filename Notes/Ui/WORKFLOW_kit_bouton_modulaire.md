# Workflow — kit bouton modulaire (Frame / Fill / décor)

**Création :** 2026-10-20  
**Statut :** **pause prod** (2026-10-20 soir) — labo `TestUi` ; **prod = prefabs legacy** teal/oval. Reprise kit après `[P0-UI-BTN-LAYOUT-TIERS-001]`. Pilote art : `Assets/Art/Sprites/UI/KitBouton/`.

**Objectif :** documenter la procédure et les **contraintes** pour créer des boutons popup **homogènes**, **recolorables** (variants sans PNG par couleur), **réutilisables en layout**, et **itérables** quand une base (ancienne ou en cours) est incorrecte.

**Lié :**
- Mockup → prefab écran : `Notes/Ui/WORKFLOW_creation_ui_mockup_bezi.md`
- Méthodo 9-slice / tiers : `Notes/Ui/SPEC_methodologie_ui_ia_kit_bezi.md`
- Info bois vs actions : `Notes/Ui/NOTE_chrome_info_vs_actions.md`
- Import art : `Notes/Art/WORKFLOW_creation_assets.md` (Sprites d’abord, **plus de Dump** pour le kit bouton courant)
- Popups runtime : `Notes/Ui/popup_generique.md`

---

## 1) Principes (décisions 2026-10)

| Principe | Détail |
|----------|--------|
| **Sprites d’abord** | Nouvelles pièces kit → `Assets/Art/Sprites/UI/KitBouton/` (ou sous-dossier kit explicite). |
| **Décomposition** | **Frame** (bois 9-slice) + **Fill** (centre teintable) + **décor** (feuilles, gloss) en calques / PNG séparés. |
| **Couleur** | Variantes = `Image.color` sur **Fill** (PNG blanc / gris clair). **Ne pas** teinter le Frame bois. |
| **9-slice** | Pas de `VerticalLayoutGroup` qui redimensionne les **Images Sliced** tant que le tier n’est pas validé ; layout sur la **racine prefab** + `LayoutElement`. |
| **Référence visuelle** | Ancien monolithe `UiKit_BtnPrimary_teal_20260919.png` = **style**, pas dimensions à copier. |

---

## 2) Contraintes de création (checklist dure)

### 2.1 Canvas et ratio

- **Canvas référence jeu :** 1080×1920 portrait, **PPU 100**.
- **Ratio tier Primary popup :** **10:3** (ex. rect UI **320×96** ↔ PNG **640×192** pour multiplier **2**).
- **Frame** et **Fill** : **même largeur × hauteur en pixels** à l’export (ex. tous deux **640×192**).
- **Ne pas** étirer L/H séparément dans Paint.NET pour « corriger » un ratio — **regénérer IA** au bon ratio natif si le rendu est mauvais.

### 2.2 9-slice (Frame)

- Bordures **fines** dans le PNG (indicatif @ 640×192 : caps **~48 px**, bandes haut/bas **~32–40 px**) pour tenir en **tier M/S** (hauteur **80** voire **64–72** px) sans multiplier > 3.
- **Caps** gauche/droite : courbes uniquement ; **milieu** haut/bas : grain horizontal **tileable**, sans rivets/feuilles/texte.
- Centre du Frame = **trou** (magenta `#FF00FF` à la génération, détouré par l’auteur) — pas de couleur baked.

### 2.3 Fill

- PNG **neutre** : `#FFFFFF` ou `#F5F5F5` — **pas** de teal/orange/coral dans le fichier.
- Aligné visuellement dans le **trou** du Frame (même canvas 640×192).
- Unity : **Simple** ou Sliced léger ; **inset** RectTransform (~8–12 px) si le blanc dépasse sous le bois.

### 2.4 Décor (feuilles, bulles gloss)

- **Hors** zones 9-slice qui s’étirent : prefab enfants **ancrés coins**, taille **fixe px**, `raycast` off.
- Feuilles : cap **droite** (haut) + cap **gauche** (bas) — pas sur le milieu étirable du Frame.

### 2.5 Tiers hauteur (layout futur)

| Tier | Rect bouton (canvas) | Notes |
|------|----------------------|--------|
| **L** | 320 × **96** | CTA principal popup |
| **M** | 320 × **80** | Annuler, secondaire |
| **S** | 280 × **64–72** | Dense ; **≥ 48** px tap min |

**Validation obligatoire** dans `TestUi` : même prefab testé en **96 / 80 / 64** px de haut + largeur **320** puis **480** (stretch).

Formule rapide :  
`hauteur_rect ≥ (border_T + border_B) / multiplier + centre_min`  
Si **64 px** KO → **regénérer Frame** (bordures trop épaisses), pas seulement monter le multiplier.

### 2.6 Unity import

- **Texture Type :** Sprite (2D and UI), **PPU 100**.
- **Frame :** Image **Sliced**, borders dans **Sprite Editor** (re-saisir après remplacement PNG).
- **Résolution Paint.NET (px/cm) :** ignorée — seuls les **pixels** comptent.
- **Ne pas déplacer** les PNG après réglage borders (reset import).

### 2.7 Prefab

- Chemin cible : `Assets/Prefabs/Ui/Kit/Btn_Primary_Base.prefab` (+ **Variants** couleur).
- Hiérarchie (ordre back → front) : **Fill** → **Frame** → **Decor_*** → **Label** (TMP).
- Racine : **Button**, **LayoutElement** (`minHeight` 64, `preferredHeight` 96, `flexibleHeight` 0).
- **Pas** d’`onClick` Inspector sur popups métier (views bindent dans `Awake`).

---

## 3) Nommage et emplacements

| Rôle | Dossier | Nom fichier (simple) |
|------|---------|----------------------|
| Bordure bois | `Assets/Art/Sprites/UI/KitBouton/` | `Frame_Btn.png` |
| Centre teintable | idem | `Fill_Btn.png` |
| Feuilles | idem | `LeafTopRight.png`, `LeafBottomLeft.png` |
| Reflets bulle | idem | `GlossLarge.png`, `GlossSmall.png` (backlog) |
| Prefab base | `Assets/Prefabs/Ui/Kit/` | `Btn_Primary_Base.prefab` |
| Variants | idem | `Btn_Primary_Teal`, `Btn_Secondary_Parchment`, … |

Anciens monolithes (`UiKit_BtnPrimary_teal_20260919.png`, oval 511×268, etc.) : **conservés** jusqu’à remplacement popup par popup ; ne pas modifier pour le nouveau kit.

---

## 4) Procédure — voie recommandée (génération IA)

1. **Choisir tier** (L/M/S) et PNG cible (**640×192** pour L @ mult 2).
2. **Prompt Frame** : canvas **exact** 640×192, trou magenta, 9-slice friendly, ref = teal monolithe **style only**.  
   → `WORKFLOW_kit_bouton_modulaire.md` §6 ou historique prompts session 2026-10-20.
3. **Détourage** fond magenta (auteur) → remplacer `Frame_Btn.png`.
4. **Unity** : borders Frame → test Image **320×96**, tuner **Pixels Per Unit Multiplier** (~2).
5. **Prompt Fill** : même canvas **640×192**, centre blanc, aligné sur le trou (ref Frame en overlay ou même session IA).
6. **Détourage** → `Fill_Btn.png` → test superposé Fill + Frame (teal sur Fill.color).
7. **Tests tiers** §2.5 → si KO, **noter dans §5** et regénérer Frame (pas force-ratio Paint.NET).
8. **Bezy** : prefab `Btn_Primary_Base` → variants → remplacement **pilote** un bouton (`HarvestButton` ou `CloseButton` / Annuler).
9. **Playtest** → généraliser récolte + popup quantité quand kit complet (feuilles, gloss optionnel).

**Bezy popups visuels :** skill `/unity-ugui` + mockup ; **wiring / kit prefab** : prompt dédié sans `/unity-ugui` (voir `WORKFLOW_creation_ui_mockup_bezi.md`).

---

## 5) Procédure — voie secours (alignement manuel)

Utiliser **seulement** si IA indisponible ou Fill légèrement décalé :

1. Paint.NET : **nouveau** 640×192 (pas « redimensionner » le Frame en décochant proportions sur l’ensemble — ça déforme).
2. Coller **Frame** → scale **uniforme** (proportions **on**) → centrer.
3. Coller **Fill** → scale uniforme + **déplacement** pour le trou ; **priorité d’ajustement : Fill**, Frame = référence forme.
4. Exporter **deux** PNG **640×192**.

Si le Frame seul est mauvais après scale → **retour §4** (regénération), pas force-ratio.

---

## 6) Prompts types (copier-coller)

### Frame seul (640×192 natif)

Référence : `UiKit_BtnPrimary_teal_20260919.png`. Fond `#FF00FF`. Bordure bois seule, trou central magenta, 9-slice friendly, caps ~48 px, bandes ~32–40 px, pas de texte/feuilles/fill coloré.

### Fill seul (640×192 natif)

Même canvas que Frame validé. Centre pilule `#FFFFFF`, pas de bois, aligné sur trou de la ref Frame (capture overlay si besoin).

### Feuilles (overlay, pas 9-slice)

Deux grappes séparées (haut-droite, bas-gauche), PNG dédiés, **Simple**, ancrage prefab.

*(Prompts complets : fil de session 2026-10-20 ou compléter ici au fil des validations.)*

---

## 7) Journal d’itération — bases et erreurs connues

| Date / base | Problème | Leçon / action |
|-------------|----------|----------------|
| **UiKit_BtnPrimary_teal_582×317** | Bordures 9-slice ~200 px → boutons énormes ou multiplier 4–6 | Ne plus utiliser comme **source dimensions** ; style uniquement. |
| **UiKit_BtnSecondary_oval_511×268** | Idem + confusion Annuler vs primary | Remplacer par kit modulaire oval/primary unifié. |
| **Popup qty FromMockup** | `Content` + VLG casse 9-slice | Pas de layout sur Images Sliced ; layout sur racine prefab bouton + `LayoutElement`. |
| **Frame_Btn 963×429 + Fill 900×309** | Ratios différents | Export **même** W×H ; regénérer Frame **640×192** plutôt que force-ratio. |
| **Paint.NET resize** | « Résolution » px/cm ≠ Unity | Ignorer ; seulement pixels + PPU 100. |
| **Monolithe → variants couleur** | Un PNG par couleur | Fill blanc + `Image.color` + Prefab Variant. |

**En cours (2026-10-20) :** Frame `KitBouton/` validé ; **Fill provisoire** (pas de dégradé relief neutre — crédits gen épuisés). Continuer prefab + wiring ; **remplacer `Fill_Btn.png` au même chemin** quand le PNG définitif est prêt (même nom → prefab Unity se met à jour, revoir inset / color si besoin).

---

## 8) Definition of done (une pièce ou prefab)

- [ ] PNG aux dimensions documentées (Frame = Fill).
- [ ] Borders Frame Sprite Editor OK ; test **320×96** + **320×80** + **320×64**.
- [ ] Largeur **480** sans couture horizontale bizarre.
- [ ] Fill teinté (teal + 1 autre couleur) sans refaire PNG.
- [ ] Prefab avec `LayoutElement` ; prêt pour nested sous `VerticalLayoutGroup` **sur le prefab**, pas sur le Frame interne.
- [ ] Trace dans `PROJECT_LOG.md` si remplacement runtime popup.

---

## 9) IDs backlog liés

- `[BL-UI-BTN-SMALL-9SLICE-001]` — kit boutons compacts (ce workflow).
- `[BL-UI-KIT-GENERIC-BUTTON-001]` — prefab base + variants (SPEC §4).
- `[P0-UI-DROP-COMPOST-ANIM-001]` — anim jeter (hors scope bouton, même popups).

---

## 10) Mise à jour de ce doc

À chaque **erreur** ou **décision** (nouveau tier, nouveau nom, changement chemin) :

1. Ajouter une ligne au **§7**.
2. Ajuster **§2** si contrainte nouvelle.
3. Une phrase dans `PROJECT_LOG.md` si changement visible en prod.
