# Prompts Bezy — EmptyState graines (`SeedSelectionUI`)

Spec : `Notes/Farm/REFACTOR_graines_plantation_inventaire.md` §4.4  
Script : `Assets/Scripts/UI/SeedSelectionUI.cs` (champs déjà prêts)  
**Layers UI :** `m_Layer: 5` — `@Notes/Ui/CONVENTION_layers_unity.md`  
**Ne pas rescanner tout le projet.** Pas de nouveaux sprites (placeholders couleur OK).

Champs à câbler (Phase 3) :
- `emptyStatePanel` → GameObject `EmptyStatePanel`
- `emptyStateLabel` → TMP message
- `openShopButton` → Button « Acheter »

Le script gère show/hide + `onClick` → shop. Ne pas modifier le C#.

---

## Historique

- **Phase 1** : hiérarchie EmptyState — **OK** (Bezy, 2026-07-23)
- **Phase 2** : polish visuel — **OK** (Bezy, 2026-07-23)
- **Phase 3** : wiring Inspector — **OK** (Bezy, 2026-07-23)

---

## Phase 1 — Hiérarchie EmptyState — OK

Validé repo (Bezy 2026-07-23) :
- `EmptyStatePanel` sous `Panel`, **inactif** (`m_IsActive: 0`), layer 5
- Enfants : `EmptyStateLabel` (« Aucune graine disponible ») + `OpenShopButton` (44 px, Button ColorTint) + `OpenShopLabel` (« Acheter »)
- VerticalLayoutGroup spacing 12, padding 16
- SerializeField empty* encore `None` (attendu Phase 1)
- Title / Close / SlotsContainer intacts

---

## Phase 2 — Polish visuel EmptyState — OK

Validé repo (Bezy 2026-07-23) :
- `EmptyStateLabel` : fontSize 30, blanc, wrap
- `OpenShopButton` : fond jaune fort (~0.95/0.75/0.1), label « Acheter » sombre contrasté
- SerializeField empty* encore `None` (attendu Phase 2)

---

## Phase 3 — Wiring Inspector — OK

Validé repo (Bezy 2026-07-23) :
- `emptyStatePanel` → EmptyStatePanel (GameObject, inactif par défaut)
- `emptyStateLabel` → EmptyStateLabel TMP
- `openShopButton` → OpenShopButton Button
- `panel` / `titleLabel` / `closeButton` / `slotsContainer` / `slotPrefab` intacts

---

## Anti-patterns

- Pas de `SceneManager.LoadScene` / logique shop dans le prefab (déjà dans le script)
- Pas de second EmptyState / duplicate Button
- Pas de fallback titre legacy une fois le panel câblé
