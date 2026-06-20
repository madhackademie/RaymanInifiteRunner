# SPEC UI — Canaux de vente (écran HUD + bandeaux)

**Création :** 2026-06-17  
**Statut :** actif — shell HUD livré ; bandeaux = prochaine session Bezy  
**GDD :** `Notes/GDD/SPEC_vente_production_boucle_jeu.md`  
**Prompts Bezy :** `Notes/Ui/PROMPTS_Bezi_sale_channels.md`

---

## 1) Décisions architecture (2026-06-17)

| Sujet | Décision |
|-------|----------|
| Pas de scène dédiée | Vente 100 % UI — pas de PNJ 3D ni quartier `.unity` |
| Pas de « Market » HUD | **Market** = marché cloud global (spec cloud) ; onglet HUD = **« Vente »** |
| Id écran | `ScreenId.SaleChannels` (`"SaleChannels"`) |
| Entrée joueur | Onglet **Vente** dans `NavigationHUD` → `UIManager.TryShowScreen` |
| Interaction canal | Clic sur **bandeau** (liste scroll) — pas de dialogue monde |
| Prefabs UI | **Bezy** (ownership prefab) ; Cursor = scripts + specs + review |

---

## 2) État code / assets (2026-06-17)

### Livré (Cursor)

| Élément | Chemin |
|---------|--------|
| Id écran | `Assets/Scripts/Systems/ScreenId.cs` → `SaleChannels` |
| Controller shell | `Assets/Scripts/UI/SaleChannels/RuntimeSaleChannelsScreen.cs` |
| Prefab placeholder | `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` |
| Onglet HUD | `Assets/Scenes/NavigationHUD.unity` → `TabVente` |
| Enregistrement UIManager | `secondaryScreens` → `SaleChannels` + prefab |

### Manque (prochaine session)

| Élément | Owner |
|---------|-------|
| Liste scroll + **bandeau Voisinage** ★1 (bandoulière/vélo verrouillés) | **Bezy** |
| Prefab **`SaleChannelBandeauView`** réutilisable | **Bezy** |
| Popup confirmation vente (quantité, prix) | Bezy prefab + Cursor `PopupId` (après bandeaux) |
| `SaleChannelService` (TrySell inventaire → monnaie) | **Cursor** (après UI bandeau cliquable) |

---

## 3) Cible visuelle bandeau (V0)

Référence GDD §2.2 — `Notes/GDD/ref_ui_ecoulement_production_panneaux.png`.

**Proto V0 — un seul bandeau actif :**

```
┌─────────────────────────────────────────────┐
│  Voisinage          ★ ☆ ☆ ☆ ☆               │
│  [illustration placeholder]                 │
│                              (slots PNJ/ami │
│                               grisés V0)    │
└─────────────────────────────────────────────┘
```

- Bandoulière / vélo : bandeaux **visibles mais verrouillés** (gris + cadenas) ou absents — à trancher en Phase 1 Bezy ; recommandation : **1 actif + 2 verrouillés** pour lire la progression.
- Clic bandeau Voisinage actif → ouvre popup vente (chantier suivant, pas Phase 1–3 Bezy).

---

## 4) Hiérarchie cible `SaleChannelsScreen.prefab`

```
SaleChannelsScreen (root — RuntimeSaleChannelsScreen, Image backdrop)
├── Header
│   ├── TitleLabel ("Canaux de vente")
│   └── CloseButton
└── Body
    └── BandeauxScrollView (ScrollRect vertical)
        └── BandeauxContent (VerticalLayoutGroup ou ContentSizeFitter)
            ├── SaleChannelBandeauView (Voisinage — actif)
            ├── SaleChannelBandeauView (Bandoulière — locked)
            └── SaleChannelBandeauView (Vélo — locked)
```

**Prefab enfant :** `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab` (à créer Bezy).

---

## 5) Wiring Inspector (Phase 3 Bezy)

| Script | Champs |
|--------|--------|
| `RuntimeSaleChannelsScreen` | `closeButton`, `bandeauxContainer` (RectTransform sous scroll), `rootBackdropImage` |
| `SaleChannelBandeauView` (à créer Cursor si absent) | `bandeauButton`, `titleLabel`, `starImages[]` (5), `lockedOverlay`, `illustrationImage` |

Cursor fournira `SaleChannelBandeauView.cs` minimal (clic → event) **après** validation hiérarchie Bezy Phase 1.

---

## 6) Liens

| Document | Rôle |
|----------|------|
| `Notes/GDD/SPEC_vente_production_boucle_jeu.md` | Vision gameplay / économie |
| `Notes/Ui/popup_generique.md` | Pipeline popup (futur `PopupId` vente) |
| `Notes/Ui/PROMPTS_Bezi_sale_channels.md` | Prompts phasés Bezy |
| `Notes/Todo_project.md` | Statut tâches `[P0-SALE-*]` |
