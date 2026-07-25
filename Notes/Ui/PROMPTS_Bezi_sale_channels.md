# Prompts Bezy — écran Vente / bandeaux canaux

Spec : `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md`  
GDD : `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §2.2  
**Ne pas rescanner tout le projet.** Réutiliser `SaleChannelsScreen.prefab` et scripts existants.

---

## Phase 1 — Shell scroll + hiérarchie bandeaux (attendre validation avant Phase 2)

**Objectif :** remplacer le placeholder Body par une zone scroll ; créer le prefab bandeau **sans** scripts custom ni wiring events.

Modifier / créer :

1. **`Assets/Prefabs/Ui/SaleChannelsScreen.prefab`**
   - Garder `Header` (titre + CloseButton) et `RuntimeSaleChannelsScreen` sur la racine.
   - Remplacer `Body/PlaceholderLabel` par :
     - `BandeauxScrollView` (ScrollRect vertical + Mask + viewport Image)
     - Enfant `BandeauxContent` (VerticalLayoutGroup, spacing ~16, padding 16).

2. **`Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab`** (nouveau)
   - Racine RectTransform hauteur ~180, Image fond bandeau (couleur cartoon, pas sprite final).
   - Enfants vides / placeholders :
     - `HeaderRow` : `TitleLabel` TMP (« Voisinage »), rangée `Stars` (5 Images ★).
     - `Illustration` : Image placeholder isométrique.
     - `LockedOverlay` (Image semi-transparent + icône cadenas) — **actif sur copies verrouillées**.

3. Instancier **3 bandeaux** sous `BandeauxContent` :
   - Voisinage — overlay verrouillé **désactivé**
   - Bandoulière — `LockedOverlay` **actif**
   - Vélo marchand — `LockedOverlay` **actif**

**Contraintes :** pas de `SaleChannelBandeauView` script en Phase 1. Pas de popup vente.

Confirmer fichiers + capture hiérarchie avant Phase 2.

---

## Phase 2 — Composants visuels bandeau (après Phase 1 OK)

Sur `SaleChannelBandeauView.prefab` :

- `TitleLabel` : TMP bold, lisible mobile.
- 5 étoiles : Images carrées 24×24 (★1 pleine rose, ★2–5 grisées pour Voisinage V0).
- `Illustration` : Image preserve aspect, fond distinct par canal (placeholder couleur OK).
- Bouton invisible ou Image + Button sur toute la zone bandeau (transition ColorTint).
- États verrouillé : `LockedOverlay` + TMP « Bientôt » optionnel.

Polish scroll : `BandeauxContent` + ContentSizeFitter vertical si besoin.

---

## Phase 3 — Wiring Inspector (après Phase 2 OK)

Scripts Cursor (déjà ou à ajouter après review Phase 2) :

| Prefab | Script | Champs |
|--------|--------|--------|
| `SaleChannelBandeauView` | `SaleChannelBandeauView` | bandeauButton, titleLabel, starImages[5], lockedOverlay, illustrationImage |
| `SaleChannelsScreen` | `RuntimeSaleChannelsScreen` | closeButton, bandeauxContainer → `BandeauxContent`, rootBackdropImage |

**Phase 3 uniquement :** lier les 3 instances bandeau ; pas de logique vente (service Cursor = session suivante).

Confirmer play mode : HUD → Vente → scroll 3 bandeaux, Voisinage cliquable visuellement, 2 autres grisés.

---

## Phase 4 — Overlay cooldown 24 h (hiérarchie seule, après Phase 3 OK)

**Objectif :** état visuel « canal en recharge » — illustration grisée + temps restant. **Pas de logique C#** (déjà dans `SaleChannelBandeauView.cs`).

Modifier **`Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab`** uniquement :

1. Ajouter enfant **`CooldownOverlay`** (Image semi-transparente gris foncé ~60 % alpha) — **couvre toute la zone bandeau** sauf `HeaderRow` (titre + étoiles restent lisibles).
2. Ajouter enfant **`CooldownLabel`** (TMP centré sur l’overlay) :
   - texte placeholder : `23h 45m`
   - bold, blanc ou jaune pâle, lisible mobile (~28–32 pt).
3. **`CooldownOverlay` + `CooldownLabel` inactifs par défaut** sur le prefab racine (le script les active en runtime).
4. Ne pas modifier `LockedOverlay` (canaux verrouillés progression).

**Contraintes :** ne pas rescanner tout le projet. Ne pas toucher aux scripts. Ne pas câbler Inspector (Phase 5).

Confirmer hiérarchie + capture avant Phase 5.

---

## Phase 5 — Wiring cooldown (après Phase 4 OK)

Sur **`SaleChannelBandeauView.prefab`** + les **3 instances** sous `SaleChannelsScreen` :

| Script | Nouveaux champs |
|--------|-----------------|
| `SaleChannelBandeauView` | `cooldownOverlay` → GameObject CooldownOverlay |
| | `cooldownLabel` → TMP CooldownLabel |

Vérifier que `illustrationImage`, `bandeauButton`, `lockedOverlay`, `channelId` restent câblés (Phase 3).

**Play mode attendu :** après une vente voisinage (test auteur), bandeau Voisinage → overlay visible + label compte à rebours ; Bandoulière/Vélo inchangés (locked).

Confirmer wiring Inspector avant fin.

---

## Suite polish — VFX pièces / billets `[BZ-POLISH-018]`

Feedback monnaie au clic / vente réussie (ParticleSystem).  
Prompts dédiés : `Notes/Ui/PROMPTS_Bezi_sale_money_vfx.md` (Phases 1→2→3).  
**Hors ce fichier** — ne pas fusionner avec Phases 4–5 cooldown ni Phase 6 fond opaque.

---

## Phase 6 — Fond opaque HUD Vente `[BZ-POLISH-017]` — **CLOS Bezy** (2026-07-26)

**Bug :** ouvrir onglet Vente → bouton Home « Commencer l’aventure » visible par transparence (root Image alpha ~0.6, pas de panel contenu).

**Livré :** root `(0.04,0.04,0.06) a=0.98` + `Body/ContentBackdrop` `(0.07,0.07,0.09) a=0.99` — sprites Image encore null (playtest auteur).

**Réf. Shop :** même intention que `HudModalBackdrop` / `ShopScreen` (fond quasi opaque).

```
[BZ-POLISH-017] Phase 6 ONLY — opaque backdrop SaleChannels. Wait success. STOP after save.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT change bandeaux / cooldown / wiring RuntimeSaleChannelsScreen fields.

EDIT ONLY: Assets/Prefabs/Ui/SaleChannelsScreen.prefab

1) Root SaleChannelsScreen Image (rootBackdropImage):
   - Keep stretch full screen (anchors 0,0–1,1).
   - Assign a solid fill sprite (UI Sprite / white square OK — NO new art).
   - Color almost opaque dark: RGB ~ (0.04, 0.04, 0.06), alpha ≥ 0.97
   - Raycast Target = ON (block clicks to Home behind)
   - Layer = 5 (UI)

2) Under Body, as FIRST child (behind BandeauxScrollView):
   CREATE ContentBackdrop
   - RectTransform stretch full Body
   - Image: solid fill sprite, color RGB ~ (0.07, 0.07, 0.09), alpha ≥ 0.99
   - Raycast Target = ON
   - Layer = 5
   - Sibling index 0 so scroll stays on top

3) Do NOT lower Viewport Mask Image alpha (keep mask). Do NOT touch Header/CloseButton/bandeaux.

Save. List hierarchy + Image colors/alphas. STOP. No Simulate / Play Mode.
```

**Après Bezy (Cursor optionnel) :** aligner `RuntimeSaleChannelsScreen.ApplyShellBackdrop` sur `HudModalBackdrop.ApplyRootBackground` (+ champ `contentBackdropImage` comme Shop) pour forcer l’opaque au runtime.
