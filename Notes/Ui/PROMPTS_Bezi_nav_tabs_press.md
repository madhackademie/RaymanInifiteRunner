# Prompts Bezy — NavigationHUD press onglets `[BZ-POLISH-006]` (slice press)

**Cible :** `Assets/Scenes/NavigationHUD.unity` — `TabAventures`, `TabInventaire`, `TabShop`, `TabVente`  
**Pattern :** Button **Transition = Animation** (comme les états Unity `Normal`/`Pressed`) — **pas de nouveau script C#**.  
**Réf. punch :** même esprit que `PlayerHaloSlot_Click` / PA SpendPulse, mais **press soft** (scale ↓ puis retour).

**Hors scope cette slice :** contrastes couleurs actives (déjà dans `NavigationHUD.cs`), audit layers global multi-scènes.  
**Inclus léger :** layer UI `5` + hit area min ~44 sur les 4 tabs.

**Règles :** ne pas rescanner tout le projet ; pas de nouveaux scripts ; pas de Simulate / Play Mode ; un prompt = une phase.

### Historique

- **Phase 1** : clips + controller partagés — **OK** (Bezy 2026-07-29)
- **Phase 2** : wire 4 tabs — **OK** (Bezy 2026-07-29) après ouverture scène Editor (écritures silencieuses si scène non chargée)

### Prérequis Editor (Ph.2)

> **Auteur avant Bezy Ph.2 :** double-clic `Assets/Scenes/NavigationHUD.unity` dans Project (scène ouverte / chargée).  
> Bezy Actions API : pas d’« open existing scene » — sans ça, `addOrUpdateComponent` / `updateGameObject` sur tabs = no-op.  
> Voir aussi workaround path : `Notes/Bezi/README_bezi.md`.

---

## Phase 1 — Clips + Controller — **CLOS**

Validé repo (Bezy 2026-07-29) :
- `NavTab_Idle.anim` — path `""`, scale 1
- `NavTab_Pressed.anim` — 1 → 0.92 → 1 @ 0 / 0.06 / 0.16 s, path `""`
- `NavTab.controller` — triggers `Normal` / `Highlighted` / `Pressed` / `Selected` / `Disabled` ; états Idle + Pressed

### Archive prompt Ph.1

```
(voir historique — livré OK)
```

---

## Phase 2 — Wire 4 tabs — **CLOS**

Validé repo (Bezy 2026-07-29) :
- `TabAventures` / `TabInventaire` / `TabShop` / `TabVente` : Animator → `NavTab.controller` (`056572b3…`)
- `m_Transition: 3` (Animation) sur les 4 tabs
- `m_Layer: 5` sur tabs (+ enfants touchés)
- Hit area : layout NavBar ~270×120 — OK (≥ 44), pas de sizeDelta forcé
- `ExitButton` reste ColorTint (hors scope)

### Archive prompt Ph.2

```
(voir historique — livré OK)
```

---

## Checklist validation auteur — **OK** (2026-07-29)

| Critère | OK? |
|---------|-----|
| Press tab → scale 0.92 puis retour | x |
| ColorTint remplacé par Animation transition | x |
| Onglet actif (couleur icône script) toujours OK | x |
| Layer 5 sur les 4 tabs | x |
| Hit ~≥ 44 | x |
| Pas de nouveau C# | x |
| Playtest auteur navigation inchangée | x |

## Anti-patterns

- Ne pas demander Simulate à Bezy
- Ne pas animer la couleur d’icône (conflit `RefreshTabVisuals`)
- Ne pas fusionner hit-area audit global + press dans un monolithe hors ces 2 phases
