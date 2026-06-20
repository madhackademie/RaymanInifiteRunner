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
