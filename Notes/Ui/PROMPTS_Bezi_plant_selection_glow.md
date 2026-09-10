# Bezy — glow sélection plante (footprint + silhouette)

**Task ID :** `[BZ-FARM-PLANT-SELECT-GLOW-001]` · **P0 :** `[P0-FARM-PLANT-SELECT-GLOW-001]`  
**Prefab :** `Assets/Prefabs/World/Plantes/LaitueObj.prefab` (modèle pour toutes plantes monde)  
**Script Cursor (après Ph.3) :** `PlantSelectionHighlight.cs` — Bezy **ne** code pas la logique.  
**Contexte :** clic = losanges footprint seuls ; le joueur doit **voir la cible** (feedback « clic pris » + quelle plante est sélectionnée). **Socle vert footprint = garder** en complément.

---

## Binôme

| Livrable | Agent |
|----------|--------|
| Enfant `SelectionGlow` + `SpriteRenderer`(s), sorting, couleurs, refs Inspector | **Bezy** |
| `PlantSelectionHighlight.cs`, branchement `BiofiltreManager` show/hide | **Cursor** |
| Socle vert footprint (existant) | **Cursor** — **garder** en complément |

---

## Cible visuelle

- Halo **jaune → blanc** autour de la **silhouette** du sprite plante (pas seulement le footprint).
- Glow **derrière** la plante (`sortingOrder` &lt; sprite principal) ou contour lisible mobile.
- `SetActive(false)` par défaut — activé par code à la sélection.
- Pas de Simulate / playtest dans le prompt Bezy.

---

## Phase 1 — Hiérarchie

Sous la racine `LaitueObj` (à côté du `SpriteRenderer` plante existant) :

```
LaitueObj
├── … (existant : PlantGrow, insectes, etc.)
└── SelectionGlow          ← NEW, inactive by default
    └── GlowSprite         ← SpriteRenderer
```

- `SelectionGlow` : local position `(0,0,0)`, rotation 0, scale 1.
- Ne pas déplacer le sprite plante existant.

**STOP.** Save. List changes.

---

## Phase 2 — Composants glow

Sur `GlowSprite` :

| Champ | Valeur |
|-------|--------|
| `SpriteRenderer.sprite` | **vide** (runtime = même sprite que la plante) |
| `SpriteRenderer.color` | jaune `(1, 0.92, 0.2, 0.75)` ou blanc `(1,1,1,0.6)` — teinte lisible sur fond IBC |
| `SpriteRenderer.sortingOrder` | **−1** vs sprite plante principal (glow derrière) |
| `localScale` | `(1.08, 1.08, 1)` — léger grossissement pour effet contour |
| Layer | même que plante (Default sauf convention projet) |

Option acceptable : **2** enfants `GlowOuter` (jaune, scale 1.10) + `GlowInner` (blanc, scale 1.04) si un seul renderer est trop plat.

**STOP.** Save. List changes.

---

## Phase 3 — Wiring

1. Ajouter composant `PlantSelectionHighlight` sur racine `LaitueObj` (script fourni par Cursor — si absent, créer GameObject vide + champ à lier plus tard).
2. Binder :
   - `plantRenderer` → `SpriteRenderer` principal de la plante
   - `glowRoot` → `SelectionGlow`
   - `glowRenderers` → tableau avec `GlowSprite` (ou Outer + Inner)
3. `SelectionGlow` **désactivé** dans le prefab.

**STOP.** Save. List SerializeField bindings.

---

## Checklist auteur (après Cursor + Bezy)

```
[ ] Clic footprint → socle vert (4 losanges) + glow silhouette
[ ] Fermeture popup récolte → glow off + socle off
[ ] Stades croissance : glow suit le sprite courant (PlantGrow)
[ ] Pas de glow sur ghost preview pose
```
