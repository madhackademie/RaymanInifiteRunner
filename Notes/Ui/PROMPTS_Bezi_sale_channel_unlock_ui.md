# Prompts Bezy — déblocage canaux (tooltip + hover + unlockable)

**Statut :** Ph.1–4 **Bezy OK** ; hook Cursor sparkle Overlay **OK** (2026-08-19).  
**Spec :** `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md` §8  
**GDD :** `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §5.2  
**Règle :** hiérarchie / prefab / wiring Inspector = **Bezy**. Pas de Simulate / playtest visuel.

### Historique

| Phase | Statut | Date |
|-------|--------|------|
| Ph.1 tooltip `SaleChannelUnlockTooltip` | OK Bezy + review repo | 2026-08-18 |
| Ph.2 hover + wiring | OK Bezy + review repo | 2026-08-18 |
| Playtest auteur (tooltip, recherche, confirmation) | OK | 2026-08-19 |
| Ph.3 `UnlockableFxAnchor` sparkle | OK Bezy + hook Cursor | 2026-08-19 |
| Ph.4 ancre stretch pleine surface | OK Bezy + playtest auteur | 2026-08-19 |

Scripts Cursor (ne pas modifier) :
- `SaleChannelUnlockTooltipHost.cs`
- `SaleChannelBandeauProgressionHover.cs`
- `SaleChannelBandeauView.cs` (pulse code sur `LockIcon` — polish VFX optionnel Bezy Phase 3)

---

## Phase 1 — Panneau tooltip déblocage (hiérarchie seule) — **CLOS Bezy**

**Livré :** `SaleChannelsScreen.prefab` → `SaleChannelUnlockTooltip` + `SaleChannelUnlockTooltipHost`.

---

## Phase 1 — spec archive (référence)

**Prefab :** `Assets/Prefabs/Ui/SaleChannelsScreen.prefab`

Do NOT rescan whole project. Do NOT create C# scripts. Layer UI = 5.

Sous la racine `SaleChannelsScreen`, ajouter :

```
SaleChannelUnlockTooltip (INACTIVE par défaut)
├── Image fond (alpha ~0.94, dark panel, raycast OFF)
├── CanvasGroup (alpha 1, blocksRaycasts OFF)
├── VerticalLayoutGroup + ContentSizeFitter (preferred size)
├── TitleLabel (TMP bold ~20, placeholder "Débloquer — Vélo marchand")
└── BodyLabel (TMP regular ~16, multiline, placeholder conditions)
```

Contraintes :
- Panneau ~320 px large min, ancré centre, pivot bas-centre.
- Au-dessus du scroll bandeaux (dernier sibling ou sibling après Body).
- Pas d’Animator en Phase 1.

Save. List what changed. STOP.

---

## Phase 2 — Hover bandeaux verrouillés (composants + wiring) — **CLOS Bezy**

**Livré :** `SaleChannelBandeauProgressionHover` sur `LockedOverlay` ; `unlockTooltipHost` câblé ; `channelId` voisinage / bandouliere / velo_marchand.

---

## Phase 2 — spec archive (référence)

**Prefabs :**
- `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab`
- `Assets/Prefabs/Ui/SaleChannelsScreen.prefab` (instances bandeaux)

Do NOT rescan whole project. Do NOT create C# scripts.

1. Sur **`LockedOverlay`** du prefab bandeau :
   - Image : `raycastTarget = true` (reçoit le survol).
   - Ajouter composant **`SaleChannelBandeauProgressionHover`**.
   - Laisser champs Inspector vides (runtime résout le bandeau parent).

2. Sur **`SaleChannelsScreen`** racine :
   - Ajouter composant **`SaleChannelUnlockTooltipHost`** sur `SaleChannelUnlockTooltip`.
   - Câbler : panelRoot, panelRect, titleLabel, bodyLabel, panelCanvasGroup.

3. Sur **`RuntimeSaleChannelsScreen`** :
   - Câbler `unlockTooltipHost` → host Phase 2.

4. **Instances bandeaux** sous `BandeauxContent` — `SaleChannelBandeauView.channelId` :
   - Voisinage → `voisinage`
   - Bandoulière → `bandouliere`
   - Vélo marchand → `velo_marchand`

5. **`BientotLabel`** : actif quand overlay locked ; texte placeholder « Bientôt ».

Save. List what changed. STOP.

---

## Phase 3 — Polish état « Prêt ! » — **CLOS Bezy + hook Cursor**

**Livré Bezy :** `UnlockableFxAnchor` (inactif) + `SparkleImage` / `SparkleImageSecondary` sous `LockedOverlay`.  
**Hook Cursor :** `SaleChannelBandeauView` active l’ancre uniquement en `Unlockable` (Find path une fois, puis cache).

---

## Phase 3 — spec archive (référence)

**Prefab :** `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab` uniquement.

Feedback visuel unlockable (canal prêt à lancer recherche) :
- Sous `LockedOverlay`, ajouter **`UnlockableFxAnchor`** (INACTIVE par défaut) :
  - enfant particules UI légères ou Image sparkle (placeholder OK).
  - position proche `LockIcon`.
- **`BientotLabel`** : couleur dorée lisible sur overlay sombre (Cursor pilote le texte « Prêt ! »).
- **`LockIcon`** : laisser scale pulse Cursor ; pas d’Animator requis.

Référence vibe : `Assets/Prefabs/World/VFX/HarvestReadyFx.prefab` — sparkle discret, pas de VFX lourd.

**Après Bezy (Cursor) :** `SaleChannelBandeauView.ApplyUnlockableFx` — ancre ON si `ProgressionPhase == Unlockable`.

### Prompt copier-coller Bezy (Phase 3 ONLY)

```
[P0-SALE-BEZI-UNLOCK-003] Phase 3 ONLY — UnlockableFx sparkle on SaleChannelBandeauView. Wait success. STOP after save.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT edit SaleChannelsScreen.prefab or RuntimeSaleChannelsScreen.

Target prefab ONLY: Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab
Layer UI = 5 on all new GameObjects.

Under existing LockedOverlay (keep LockIcon + BientotLabel + SaleChannelBandeauProgressionHover unchanged):
1) Add GameObject UnlockableFxAnchor (INACTIVE by default). RectTransform centered on LockIcon area, offset Y ~8, size ~64x64.
2) Child SparkleImage: UI Image, soft yellow/white glow placeholder sprite (or simple circle), Preserve Aspect, raycast OFF, alpha ~0.85.
3) Optional: second subtle sparkle child OR lightweight UI ParticleSystem (layer 5). Play On Awake OFF.

Rules:
- Do NOT add Animator on LockIcon (Cursor pulse code stays).
- Do NOT wire C# scripts. Cursor will toggle UnlockableFxAnchor at runtime later.
- Do NOT change bandeauButton, starImages, cooldownOverlay, channelId, hover wiring.

Reference vibe: Assets/Prefabs/World/VFX/HarvestReadyFx.prefab (subtle sparkle).

Save prefab. List what changed. STOP.
```

*(~1 450 caractères — sous limite 3 500.)*

---

## Phase 4 — Ancre pleine surface — **CLOS Bezy**

**Livré :** `UnlockableFxAnchor` stretch-fill `LockedOverlay`, **premier sibling** (derrière cadenas / label) ; `SparkleImageSecondary` retiré ; `SparkleImage` conservé (sprite source, inactif).

**Réutiliser** cette logique sur **tous** les bandeaux suivants (même prefab). Voir spec §8 *Convention* + `[BL-SALE-BANDEAU-TPL-001]`.

---

## Phase 4 — spec archive (référence)

**Prefab :** `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab` uniquement.

HUD Overlay **masque** les ParticleSystem monde — **ne pas** ajouter de PS. Cursor joue les sparkles UI au runtime (`SaleChannelUnlockableSparkleVfx`). Bezy = hiérarchie propre.

**Après Bezy (Cursor) :** déjà en place — ancre ON en « Prêt ! », sparkles UI sur toute la surface.

### Prompt copier-coller Bezy (Phase 4 ONLY)

```
[P0-SALE-BEZI-UNLOCK-004] Phase 4 ONLY — stretch UnlockableFxAnchor full bandeau. Wait success. STOP after save.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT edit SaleChannelsScreen.prefab. Do NOT add ParticleSystem (HUD Overlay hides world PS; Cursor owns runtime sparkles).

Target prefab ONLY: Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab
Layer UI = 5.

On existing UnlockableFxAnchor (keep name, keep INACTIVE by default):
1) RectTransform stretch-fill LockedOverlay: anchors min (0,0) max (1,1), pivot 0.5 0.5, offsetMin/offsetMax 0, sizeDelta 0, anchoredPosition 0.
2) Set as FIRST sibling under LockedOverlay (behind LockIcon + BientotLabel).
3) DELETE SparkleImageSecondary only.
4) Keep SparkleImage INACTIVE (sprite StarsParticle stays — Cursor uses it as sprite source, then hides it at runtime).

Keep unchanged: LockIcon, BientotLabel, SaleChannelBandeauProgressionHover, bandeauButton, starImages, cooldownOverlay, channelId.

Save prefab. List what changed. STOP.
```

---

## Checklist auteur (après Bezy — pas dans prompt Bezy)

- [x] HUD → Vente → survol bandeau Vélo verrouillé → tooltip conditions + coût (2026-08-19)
- [x] Bandeau « Prêt ! » → pulse cadenas à l’ouverture écran (2026-08-19)
- [x] Clic bandeau prêt → confirmation recherche → or débité + timer overlay (2026-08-19)
- [x] Playtest valeurs SO : `Assets/Data/SaleChannels/Unlock_*.asset` (2026-08-19)
- [x] Ph.3 sparkle `UnlockableFxAnchor` (Bezy) + hook Cursor (2026-08-19)
- [x] Ph.4 ancre stretch pleine surface (Bezy) + playtest sparkle plein bandeau **OK** (2026-08-19)

**Service Cursor** (déjà sur `NavigationHUD` / `PlayerInventory`) :
- `SaleChannelUnlockService` + defs Bandoulière / Vélo
