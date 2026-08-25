# SPEC UI — Canaux de vente (écran HUD + bandeaux)

**Création :** 2026-06-17  
**Statut :** actif — V0 voisinage livré (bandeaux + vente) ; prochaine étape = **timer canal**  
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

## 2) État code / assets (2026-06-20)

### Livré (Bezy + Cursor)

| Élément | Chemin |
|---------|--------|
| Id écran | `Assets/Scripts/Systems/ScreenId.cs` → `SaleChannels` |
| Controller shell | `Assets/Scripts/UI/SaleChannels/RuntimeSaleChannelsScreen.cs` |
| Vue bandeau | `Assets/Scripts/UI/SaleChannels/SaleChannelBandeauView.cs` |
| Prefab écran | `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` |
| Prefab bandeau | `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab` |
| Scroll + 3 bandeaux | Voisinage actif, Bandoulière/Vélo verrouillés |
| Onglet HUD | `Assets/Scenes/NavigationHUD.unity` → `TabVente` |
| Popup vente | `PopupId.SaleChannelSell` + mode `ShopItemPopupFlowMode.Sell` |
| Service métier | `Assets/Scripts/Systems/SaleChannelService.cs` (voisinage, laitue, cap 2, 15 gold) |

### Prochaine session (Cursor)

| Élément | Owner |
|---------|-------|
| **Timer / cooldown canal** — ex. 1 vente / jour par canal | **Cursor** |
| Feedback UI bandeau (indispo + compte à rebours ou « demain ») | Cursor (+ Bezy si label dédié sur prefab) |
| Persistance dernier reset vente (save locale / service temps existant) | **Cursor** |

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
| `SaleChannelBandeauView` | `bandeauButton`, `titleLabel`, `starImages[]` (5), `lockedOverlay`, `illustrationImage`, `channelId`, `cooldownOverlay`, `cooldownLabel` |

Cursor fournira `SaleChannelBandeauView.cs` minimal (clic → event) **après** validation hiérarchie Bezy Phase 1.

---

## 6) Liens

| Document | Rôle |
|----------|------|
| `Notes/GDD/SPEC_vente_production_boucle_jeu.md` | Vision gameplay / économie |
| `Notes/Ui/popup_generique.md` | Pipeline popup (`PopupId.SaleChannelSell`) |
| `Notes/Ui/PROMPTS_Bezi_sale_channels.md` | Prompts phasés Bezy |
| `Notes/Todo_project.md` | Statut tâches `[P0-SALE-*]` |

---

## 7) Timer canal — cooldown 24 h (2026-06-20)

**Règle V0 :** après une vente réussie sur un canal, blocage **24 h** (UTC via `FarmTimeService`), puis déblocage automatique.

| Sujet | Implémentation |
|-------|----------------|
| Persistance | `SaleChannelSaveService` → `sale_channels.json` (`lastSaleUtcTicks` par `channelId`) |
| Service | `SaleChannelService` — `IsOnCooldown`, `TryGetCooldownRemainingSeconds`, enregistrement après `TrySell` OK |
| UI bandeau | `SaleChannelBandeauView.ApplyCooldownState` — overlay + label + illustration grisée |
| Refresh | `RuntimeSaleChannelsScreen` — refresh à l’ouverture + coroutine 1 s tant qu’un cooldown actif |
| Bezy | **Phase 4–5** — `CooldownOverlay` + `CooldownLabel` sur prefab bandeau (`PROMPTS_Bezi_sale_channels.md`) |
| Debug | `ignoreSaleCooldown` sur `SaleChannelService` (Inspector) |
| Playtest | **[P0-SALE-PLAY-004]** — vendre → overlay + timer → attendre / simuler 24 h → revendre |

---

## 8) Déblocage progression — Bandoulière / Vélo (2026-08-18)

**Ownership :** logique + scripts = **Cursor** ; tooltip, hover, polish bandeau = **Bezy** (`Notes/Ui/PROMPTS_Bezi_sale_channel_unlock_ui.md`).

| Sujet | Owner | Détail |
|-------|-------|--------|
| Conditions + compteurs vente | Cursor | `SaleChannelUnlockService`, SO `Assets/Data/SaleChannels/Unlock_*.asset` |
| Recherche timer + coût or | Cursor | persistance `sale_channels.json` (stats + research) |
| Tooltip conditions / coût | **Bezy** ✅ Ph.1 | `SaleChannelUnlockTooltip` sur `SaleChannelsScreen` |
| Survol bandeau verrouillé | **Bezy** ✅ Ph.2 | `SaleChannelBandeauProgressionHover` sur `LockedOverlay` |
| Pulse « Prêt ! » | Cursor ✅ + **Bezy** ✅ Ph.3–4 | pulse `LockIcon` ; sparkle plein bandeau Overlay — **playtest OK 2026-08-19** |
| `channelId` instances bandeaux | **Bezy** ✅ Ph.2 | `voisinage` / `bandouliere` / `velo_marchand` |
| Playtest déblocage | auteur ✅ 2026-08-19 | tooltip + recherche + confirmation — voir `PROMPTS_Bezi_sale_channel_unlock_ui.md` |

**États bandeau (progression, distinct du cooldown 24 h) :**
- `Locked` — overlay + « Bientôt » + tooltip progrès
- `Unlockable` — overlay + « Prêt ! » + pulse cadenas à l’ouverture UI + sparkle plein bandeau
- `ResearchInProgress` — overlay + timer recherche
- `Unlocked` — overlay off, canal utilisable (soumis au cooldown vente)

### Convention — tout nouveau bandeau (2026-08-19)

**Même logique que Bandoulière / Vélo** (playtest sparkle OK). Ne pas créer un bandeau one-off.

Checklist obligatoire :
1. **Instance** de `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab` (pas un duplicata débranché).
2. `channelId` Inspector : constante `SaleChannelId` (ex. prochain canal).
3. SO `Assets/Data/SaleChannels/Unlock_<Canal>.asset` + entrée dans `SaleChannelUnlockService.unlockDefinitions`.
4. Hérite tout seul : overlay locked, tooltip hover, « Prêt ! », sparkle Overlay, cooldown 24 h, rangée ★ (polish `[P0-SALE-STAR-*]`).
5. Bezy : wiring `channelId` + illustration uniquement. Cursor : id + SO + conditions. **Pas de ParticleSystem monde** (HUD Overlay).

Réf. : `Notes/Todo_project.md` `[BL-SALE-BANDEAU-TPL-001]`.

---

## 9) Étoiles bandeau + tooltip palier (2026-08-25)

**Tâche :** `[P0-SALE-STAR-UI-001]` — prompts `Notes/Ui/PROMPTS_Bezi_sale_channel_stars.md`.

| Sujet | Owner | Détail |
|-------|-------|--------|
| Hiérarchie tooltip `SaleChannelStarTooltip` | **Bezy** Ph.1 | Panneau dédié (pas `SaleChannelUnlockTooltip`) |
| ★ pleines/vides + hit hover sur `Stars` | **Bezy** Ph.2 | Image alpha 0.01 raycast ON sur `Stars` ; Star1–5 raycast OFF |
| Wiring `SaleChannelStarHover` + host | **Bezy** Ph.3 | Champs vides hover = resolve runtime |
| Copy palier + `ApplyStarFill` | Cursor | ★ visuelles 1/5 jusqu’à `[P0-SALE-STAR-001]` ; tooltip ★2 = compteurs live |
| Compteurs tooltip ★ | Cursor | `SaleChannelStarUiCopy` : ventes / salades / **or gagné sur le canal** (`current/required`) |
| Jauges dans le tooltip | **Bezy** `[P0-SALE-STAR-BARS-001]` | 3 barres Image Filled + TMP overlay ; fill runtime Cursor |

Hover **uniquement** sur la rangée `Stars`. Clic étoiles → transmis au bandeau (vente / recherche). Tooltip cadenas inchangé sur `LockedOverlay`.

**Lecture progression ★1 → ★2 (GDD §2.9, valeurs de travail) :**
- Ventes : `SaleCount` / 5
- Salades écoulées : `ItemsSold` / 50
- Or gagné **via ce canal** (pas le wallet) : `GoldEarned` / 2000
- L’upgrade réel (★2 accordée) reste `[P0-SALE-STAR-001]`.
