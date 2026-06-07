# Arbre UI — inventaire scindé (halo + talents)

Réf. visuelles :
- `Assets/Art/Models/ImageRef/UI/InventoryStats.png` (module halo)
- `Assets/Art/Models/ImageRef/UI/InventorySplitStatsCompetances.png` (écran complet, **sans zone A footer**)

Spec produit : `Notes/Ui/SPEC_rework_inventaire_halo_progression.md`

**Prefabs : Bezy.ai uniquement** (sauf demande explicite auteur) — voir `.cursor/rules/bezi_prefab_ownership.mdc`.

---

## Hiérarchie runtime (cible)

```
InventoryScreen                    [Canvas + InventoryScreenController]
├── Dimmer
├── InventorySplitLayout           [VerticalLayoutGroup]
│   ├── PlayerHaloPanel            [PlayerHaloPanelController]
│   │   ├── PortraitFrame / LevelLabel
│   │   └── HaloSlots
│   │       └── HaloSlot_01..08    [PlayerHaloSlotUI]
│   │           ├── AnimatedVisual   ← futurs clips / sprites
│   │           ├── PlaceholderLabel
│   │           └── LockedOverlay
│   ├── FilterBarPlaceholder       (inactif, phase 2)
│   └── InventoryPanel             [InventorySceneController + InventoryUI]
│       ├── Header / CloseButton
│       ├── ScrollView / grille
│       └── WalletBar
└── TalentTreeOverlay              [TalentTreeOverlayController, inactif par défaut]
    ├── OverlayDimmer
    └── OverlayPanel / Retour
```

---

## Scripts (Cursor — déjà en dépôt)

`Assets/Scripts/UI/Inventory/Progression/`

| Fichier | Rôle |
|---------|------|
| `ProgressionTrackId.cs` | IDs placeholder `track.placeholder.01` … `08` |
| `PlayerHaloSlotUI.cs` | Bouton + `AnimatedVisual` + labels |
| `PlayerHaloPanelController.cs` | 8 slots, niveau mock, `OnTrackSelected` |
| `TalentTreeOverlayController.cs` | Overlay arbre (fade, Retour) |
| `InventoryScreenController.cs` | Halo → overlay ; `CanvasGroup` sur `InventoryPanel` |

---

## Prefabs attendus (Bezy — à créer)

| Chemin | Contenu |
|--------|---------|
| `Assets/Prefabs/Ui/Progression/PlayerHaloSlotUI.prefab` | Slot : Button + `AnimatedVisual` + TMP + `PlayerHaloSlotUI` |
| `Assets/Prefabs/Ui/Progression/PlayerHaloPanel.prefab` | Halo 8 slots en orbite + portrait + niveau + `PlayerHaloPanelController` |
| `Assets/Prefabs/Ui/InventoryScreen.prefab` | Patch : `InventorySplitLayout`, enfant halo, overlay talents, `InventoryScreenController` |

Réutiliser scripts existants ; ne pas rescanner tout le projet. Phases Bezy : shell → composants → wiring (cf. `bezy_execution_phases.mdc`).

---

## Bindings Inspector (phase 3 Bezy)

**`PlayerHaloSlotUI`** : `trackId`, `clickButton`, `animatedVisual`, `placeholderLabel`, `levelBadge`, `lockedOverlay`, `animator` (optionnel).

**`PlayerHaloPanelController`** : `portraitImage`, `levelLabel`, `portraitAnimator`, tableau `haloSlots` (8).

**`TalentTreeOverlayController`** : `overlayRoot`, `canvasGroup`, `animator`, `trackTitleLabel`, `bodyPlaceholderLabel`, `backButton`.

**`InventoryScreenController`** (sur racine `InventoryScreen`) : `haloPanel`, `talentTreeOverlay`, `inventoryBodyCanvasGroup` sur `InventoryPanel`, `filterBarPlaceholder` (inactif).

**`InventoryPanel`** : ajouter `CanvasGroup` si absent (alpha 1 par défaut).

---

## Validation playtest

1. Onglet Inventaire (HUD).
2. Clic slot **P1…P8** → overlay placeholder + titre `trackId`.
3. Retour → overlay fermé, grille inchangée (pas de rebuild slots).

---

## Renommage pistes (après notes tablette)

1. `ProgressionTrackId.cs`
2. Labels / GameObjects dans prefabs Bezy
3. Sprites + `Animator` sur `AnimatedVisual`

---

## Phase suivante (hors coque)

- **Layout éditeur (décision 2026-06-07)** : `Notes/Ui/SPEC_talent_tree_layout_editeur.md` — nœuds déplacés à la main dans Unity, edges `[ExecuteAlways]`.
- **Session prochaine — 3 étapes** : `Notes/Ui/SESSION_prochaine_halo_arbres_competences.md`
  1. Renommer `ProgressionTrackId` (code OK)
  2. SO + service Commerce mock (partiel)
  3. Bezy briques + Cursor foundation + composition auteur `Track_Commerce`
- Filtres inventaire (onglets C)
- Données talents + save
- `PopupId` modales petites si besoin
