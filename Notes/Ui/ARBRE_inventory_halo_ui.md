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
| `ProgressionTrackId.cs` | 8 IDs compétences (`track.marketing` … `track.shop`) + libellés courts/longs |
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
2. Clic slot halo → overlay placeholder + titre **nom lisible** (ex. « Marketing », « Magasin »).
3. Retour → overlay fermé, grille inchangée (pas de rebuild slots).

---

## Pistes intégrées (2026-06-08)

| Slot | ID | Label court |
|------|----|-------------|
| 1 | `track.marketing` | Marketing |
| 2 | `track.insect.feed` | Insectes |
| 3 | `track.bioconversion` | Bioconv. |
| 4 | `track.fish.reproduction` | Poisson |
| 5 | `track.water` | Eau |
| 6 | `track.gardening` | Jardin |
| 7 | `track.diy` | DIY |
| 8 | `track.shop` | Magasin |

Reste : sprites + `Animator` sur `AnimatedVisual` (Bezy).

---

## Phase suivante (hors coque)

- **Session prochaine — étapes 2–3** : `Notes/Ui/SESSION_prochaine_halo_arbres_competences.md`
  1. ~~Renommer `ProgressionTrackId`~~ **fait** (2026-06-08)
  2. SO + arbre Marketing mock
  3. Bezy overlay `TreeScrollView` + `TalentNodeUI`
- Filtres inventaire (onglets C)
- Données talents + save
- `PopupId` modales petites si besoin
