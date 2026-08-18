# Prompts Bezy — déblocage canaux (tooltip + hover + unlockable)

**Statut :** en attente Bezy — logique Cursor livrée (`SaleChannelUnlockService`, scripts UI).  
**Spec :** `Notes/Ui/SPEC_sale_channels_ui_bandeaux.md` §8  
**GDD :** `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §5.2  
**Règle :** hiérarchie / prefab / wiring Inspector = **Bezy**. Pas de Simulate / playtest visuel.

Scripts Cursor (ne pas modifier) :
- `SaleChannelUnlockTooltipHost.cs`
- `SaleChannelBandeauProgressionHover.cs`
- `SaleChannelBandeauView.cs` (pulse code sur `LockIcon` — polish VFX optionnel Bezy Phase 3)

---

## Phase 1 — Panneau tooltip déblocage (hiérarchie seule)

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

## Phase 2 — Hover bandeaux verrouillés (composants + wiring)

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

## Phase 3 — Polish état « Prêt ! » (optionnel, après Phase 2 OK)

**Prefab :** `SaleChannelBandeauView.prefab` uniquement.

Do NOT rescan whole project. Do NOT create C# scripts. Do NOT edit SaleChannelsScreen logic.

Feedback visuel unlockable (canal prêt à lancer recherche) :
- Sous `LockedOverlay`, ajouter **`UnlockableFxAnchor`** (INACTIVE par défaut) :
  - enfant particules UI légères ou Image sparkle (placeholder OK).
  - position proche `LockIcon`.
- **`BientotLabel`** : couleur dorée quand texte « Prêt ! » (Cursor pilote le texte).
- **`LockIcon`** : laisser scale pulse Cursor ; pas d’Animator requis.

Référence vibe : sparkle récolte plante (`HarvestReadyFx`) — ici en UI bandeau.

Save. List what changed. STOP.

---

## Checklist auteur (après Bezy — pas dans prompt Bezy)

- [ ] HUD → Vente → survol bandeau Vélo verrouillé → tooltip conditions + coût
- [ ] Bandeau « Prêt ! » → pulse cadenas à l’ouverture écran
- [ ] Clic bandeau prêt → lance recherche (or débité, timer overlay)
- [ ] Playtest valeurs SO : `Assets/Data/SaleChannels/Unlock_*.asset`

**Service Cursor** (déjà sur `NavigationHUD` / `PlayerInventory`) :
- `SaleChannelUnlockService` + defs Bandoulière / Vélo
