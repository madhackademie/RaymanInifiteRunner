# Convention — zone réservée HUD Points d’action (PA)

**Décision (2026-07-30) :** un **seul slot chrome** pour le HUD PA — coin **haut-droite** du shell `NavigationHUD`. Pas de repositionnement par écran en V0.

**IDs suivi :** `[CT-UI-SAFE-PA-001]` · playtest lié overlay talents / inventaire.

---

## Slot global

| Paramètre | Valeur |
|-----------|--------|
| Prefab | `Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab` |
| Instance | `Assets/Scenes/NavigationHUD.unity` |
| Anchors | top-right `(1, 1)` |
| Pivot | `(1, 1)` |
| Position | `anchoredPosition ≈ (-16, -16)` |
| Taille | `≈ 240 × 60` |
| Layer | UI = 5 |

## Zone réservée (ne pas y placer titres / CTA critiques)

Rectangle approximatif depuis le coin haut-droit du canvas shell :

- largeur **≥ 256** (240 + marge 16)
- hauteur **≥ 76** (60 + marge 16)

Les overlays / écrans (inventaire, shop, ferme, talents) doivent composer **autour** :

- titres **centrés** ou **à gauche**
- boutons Retour / Close plutôt **haut-gauche** ou bas
- ne pas coller un label important sous le PA

## Fond overlay talents

- `TreeMountHost` stretch **plein** `OverlayPanel` (offsets 0) — normalisé aussi au runtime (`NormalizeTreeMountHostLayout`).
- Layers sous `TreeMountHost` :
  1. **`FondPanel`** — Image plein cadre, opaque `(0.15, 0.13, 0.20)`, sprite UI builtin (pas de déformation).
  2. **`Filigrane`** — centré, `1120×1120`, **Preserve Aspect**, alpha ~0.14 (pas stretch).
- Ordre draw : `TreeMountHost` **FirstSibling** puis `TrackTitle` / `BackButton` au-dessus.
- `BodyPlaceholder` : inactif par défaut quand arbre visuel.

## Anti-patterns

- `TreeMountHost.SetAsLastSibling` (couvre titre / Retour).
- Déplacer le PA dans chaque prefab d’écran.
- Ancrer le PA en haut-centre (masque titres).
- Couvrir le coin haut-droite avec une barre de titre pleine largeur sans inset.

## Références

- Shell : `NavigationHUD` + `UIManager`
- Overlay talents : `InventoryScreen` / `TalentTreeOverlayController`
- Credits Bezy : reset le **30** du mois (`Notes/Bezi/README_bezi.md`)
