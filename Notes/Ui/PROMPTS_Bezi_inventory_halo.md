# Prompts Bezy — inventaire halo [CT-INV-HALO-001]

Branche : **`feature/inventory-halo-ui`**.  
Réfs visuelles : `Assets/Art/Models/ImageRef/UI/InventoryStats.png`, `InventorySplitStatsCompetances.png` (pas de footer zone A).  
Doc : `Notes/Ui/ARBRE_inventory_halo_ui.md`.

**Règle** : une phase à la fois — confirmer succès avant la suivante. Ne pas rescanner tout le projet. Réutiliser les scripts existants (ne pas les recréer).

---

## Phase 1 — Shell (hiérarchie seule)

**Limite** : shell uniquement — pas de `Image`/`Button`/`TMP`/scripts custom. RectTransform + noms d’objets.

```
Projet RaymanInfiniteRunner, branche feature/inventory-halo-ui.
NE PAS rescanner tout le projet. NE PAS recréer les scripts C#.

PHASE 1 UNIQUEMENT — shells GameObject + RectTransform. Pas de composants UI graphiques ni scripts custom.

Références visuelles (layout) :
- Assets/Art/Models/ImageRef/UI/InventoryStats.png (halo 8 slots + centre)
- Assets/Art/Models/ImageRef/UI/InventorySplitStatsCompetances.png (split halo + grille, SANS barre footer basse)

A) Créer dossier Assets/Prefabs/Ui/Progression/

B) Prefab Assets/Prefabs/Ui/Progression/PlayerHaloSlotUI.prefab
Racine PlayerHaloSlotUI 72x72, pivot 0.5/0.5
├── AnimatedVisual (64x64, centré)
├── PlaceholderLabel (ancré bas, stretch horizontal)
├── LevelBadge (coin bas-droit, petit)
└── LockedOverlay (plein parent, inactif par défaut)

C) Prefab Assets/Prefabs/Ui/Progression/PlayerHaloPanel.prefab
Racine PlayerHaloPanel — anchor top stretch, hauteur préférée ~300
├── PortraitFrame ~108x108, centre haut
├── LevelLabel sous portrait
└── HaloSlots (conteneur 400x400)
    └── HaloSlot_01 … HaloSlot_08 (instances prefab slot)
    Positions orbite : 8 points à 45° depuis le haut (rayon ~118 autour du centre du conteneur). Noms exacts HaloSlot_01..08.

D) Modifier UNIQUEMENT Assets/Prefabs/Ui/InventoryScreen.prefab :
- Garder Dimmer et contenu existant InventoryPanel (Header, ScrollView, WalletBar) INTACTS.
- Créer InventorySplitLayout (VerticalLayoutGroup sera phase 2) sous racine InventoryScreen, sibling après Dimmer.
- Reparenter InventoryPanel sous InventorySplitLayout (stretch, flexible height).
- Instancier PlayerHaloPanel comme premier enfant de InventorySplitLayout (au-dessus du panel).
- Créer FilterBarPlaceholder (hauteur ~40, inactif) entre halo et InventoryPanel.
- Créer TalentTreeOverlay (stretch plein écran, dernier sibling pour le tri) :
  ├── OverlayDimmer (plein écran)
  └── OverlayPanel centré ~520x640
      ├── TrackTitle (haut)
      ├── BodyPlaceholder (centre)
      └── BackButton (bas)

NE PAS toucher InventoryUI, InventorySceneController, CloseButton existants.
Livrable : liste des fichiers créés/modifiés. STOP — attendre validation avant phase 2.
```

---

## Phase 2 — Composants UI (sans wiring scripts)

```
Projet RaymanInfiniteRunner. PHASE 2 UNIQUEMENT — ajouter composants Unity UI sur les shells phase 1. PAS de scripts custom, PAS de SerializeField wiring.

Fichiers cibles :
- Assets/Prefabs/Ui/Progression/PlayerHaloSlotUI.prefab
- Assets/Prefabs/Ui/Progression/PlayerHaloPanel.prefab
- Assets/Prefabs/Ui/InventoryScreen.prefab

PlayerHaloSlotUI :
- Racine : Image placeholder beige/gris + Button (target = Image racine)
- AnimatedVisual : Image couleur distincte, raycast OFF
- PlaceholderLabel : TextMeshProUGUI "P1", centré bas, raycast OFF
- LevelBadge : TextMeshProUGUI petit, raycast OFF
- LockedOverlay : Image semi-transparent noir, raycast OFF

PlayerHaloPanel :
- Racine : Image fond parchemin clair (ref InventoryStats.png)
- PortraitFrame : Image placeholder portrait
- LevelLabel : TMP "Niveau 1"
- HaloSlots : pas de layout group (positions manuelles phase 1)

InventoryScreen :
- InventorySplitLayout : VerticalLayoutGroup (childControlWidth/Height true, spacing 0), ContentSizeFitter si besoin
- PlayerHaloPanel : LayoutElement preferredHeight 300
- InventoryPanel : LayoutElement flexibleHeight 1 + CanvasGroup (alpha 1, interactable ON)
- FilterBarPlaceholder : LayoutElement h 40, Image discret optionnel, RESTE INACTIF
- TalentTreeOverlay : CanvasGroup (alpha 0) sur racine overlay
- OverlayDimmer : Image noir alpha ~0.55, raycast ON
- OverlayPanel : Image fond clair
- TrackTitle / BodyPlaceholder : TMP placeholder
- BackButton : Image + Button + enfant TMP "Retour"
- Racine TalentTreeOverlay : **INACTIF par défaut** (obligatoire — Phase 1 l’a laissé actif)

Couleurs = placeholders uniquement (pas d’assets finaux).
Livrable : checklist composants par prefab. STOP — attendre validation avant phase 3.
```

---

## Phase 3 — Scripts + wiring Inspector

```
Projet RaymanInfiniteRunner. PHASE 3 UNIQUEMENT — ajouter scripts existants + remplir SerializeField. Pas de nouvelle logique C#.

Scripts (NE PAS recréer) :
Assets/Scripts/UI/Inventory/Progression/PlayerHaloSlotUI.cs
Assets/Scripts/UI/Inventory/Progression/PlayerHaloPanelController.cs
Assets/Scripts/UI/Inventory/Progression/TalentTreeOverlayController.cs
Assets/Scripts/UI/Inventory/Progression/InventoryScreenController.cs

A) PlayerHaloSlotUI.prefab — composant sur racine :
trackId (voir ordre ci-dessous), clickButton, animatedVisual, placeholderLabel, levelBadge, lockedOverlay, animator (vide)

trackId par slot dans le panel (HaloSlot_01→08) :
track.placeholder.01 … track.placeholder.08 (sens horaire depuis 12h)

B) PlayerHaloPanel.prefab — PlayerHaloPanelController :
portraitImage, levelLabel, portraitAnimator (vide), haloSlots[8] références HaloSlot_01..08

C) TalentTreeOverlay sur InventoryScreen :
TalentTreeOverlayController sur racine TalentTreeOverlay :
overlayRoot = racine TalentTreeOverlay, canvasGroup, animator (vide),
trackTitleLabel, bodyPlaceholderLabel, backButton

D) Racine InventoryScreen — ajouter InventoryScreenController :
haloPanel → PlayerHaloPanel instance
talentTreeOverlay → TalentTreeOverlay
inventoryBodyCanvasGroup → CanvasGroup sur InventoryPanel
filterBarPlaceholder → FilterBarPlaceholder (inactive)
inventoryDimAlphaWhenTreeOpen = 0.35

NE PAS supprimer InventorySceneController sur InventoryPanel.
NE PAS modifier UIManager / NavigationHUD.

Tests attendus (play mode, onglet Inventaire HUD) :
- Clic P1–P8 ouvre overlay avec trackId
- Retour ferme overlay, grille toujours visible
Livrable : capture Inspector InventoryScreenController + liste champs remplis. FIN.
```

---

## Ordre d’exécution

1. Coller **Phase 1** dans Bezy → valider prefabs + hiérarchie.
2. **Phase 2** → valider composants.
3. **Phase 3** → playtest → commit prefabs côté auteur.

Après merge : renommer pistes via `ProgressionTrackId.cs` + sprites/Animator sur `AnimatedVisual`.
