# Prompts Bezy — Filigrane sac inventaire `[BZ-INV-SAC-FILIGRANE-001]`

**Prefab :** `Assets/Prefabs/Ui/InventoryScreen.prefab` → `InventorySplitLayout/InventoryPanel`  
**Sprite (déjà promu, même que l’onglet HUD) :** `Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Inventaire_glyph.png`  
**Layers :** UI = `m_Layer: 5`  
**Ne pas rescanner tout le projet.** Pas de C#. Ne pas régénérer le sprite.

**Succès Bezy = Save + liste. STOP. Pas de Simulate / Play Mode.**

**Contexte :** le gros rectangle sombre (titre + grille + wallet) = `InventoryPanel`.  
Test auteur : alpha 0.50 trop visible (2026-09-21) → **0.12** (même valeur que le filigrane Commerce).

**Hors scope :** `TalentTreeOverlay` / `Filigrane` Commerce, `NavigationHUD`, onglets filtre, C#.

**Prérequis Editor :** ouvrir `InventoryScreen.prefab` en Prefab Mode avant l’appel.

---

## Phase 1 — Image filigrane 50% (COPIER TEL QUEL)

```
[BZ-INV-SAC-FILIGRANE-001] Phase 1 ONLY — backpack watermark on InventoryPanel. Wait success. STOP after save.

Do not rescan whole project. Do not modify C#. Do not recreate sprites.
Do not touch TalentTreeOverlay / existing Filigrane (commerce tree).
Do not rename InventoryPanel, Header, ScrollView, WalletBar, InventoryFilterBar.

File ONLY:
- Assets/Prefabs/Ui/InventoryScreen.prefab
Sprite:
- Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Inventaire_glyph.png
  (same sprite as TabInventaire Icon)

REQUIRED:
1) Under InventoryPanel create UI child Image named SacFiligrane if missing.
2) RectTransform: stretch full InventoryPanel (anchors 0,0–1,1 ; offsets 0 ; pivot 0.5/0.5).
3) Sibling order STRICT under InventoryPanel:
   - index 0 = SacFiligrane
   - then Header, ScrollView, WalletBar (keep as-is)
4) SacFiligrane Image:
   - Source Image = IconeTab_Inventaire_glyph sprite
   - Preserve Aspect = true
   - Color white RGB (1,1,1) alpha 0.50
   - raycastTarget = false
5) m_Layer: 5 on SacFiligrane.
6) ScrollView Image: keep component; set Color alpha to 0 (keep RGB). Do not disable ScrollRect.
7) Do not add Mask / Button / LayoutGroup on SacFiligrane.
8) Save.

Interdits: no C#; no Simulate; do not parent SacFiligrane under Content (would scroll); do not edit NavigationHUD.

Done = Save. List SacFiligrane sibling index + sprite path + color RGBA + ScrollView Image alpha + layer. STOP.
```

**Chars ~1450** — OK &lt; 3500.

### Checklist review Phase 1 (Cursor)

- [x] `SacFiligrane` sous `InventoryPanel`, sibling 0
- [x] Sprite `IconeTab_Inventaire_glyph`, Preserve Aspect, α 0.50, raycast off, layer 5, stretch
- [x] `ScrollView` Image alpha = 0 ; ScrollRect intact
- [x] `TalentTreeOverlay/Filigrane` Commerce inchangé (α 0.14)
- [ ] Extra Bezy : RectTransform layout (filter tabs, TitleLabel, wallet icons, InventoryFilterBar) rebakés `0,0` — à confirmer Play Mode (LayoutGroup devrait recaler)

### Après Bezy (auteur)

1. Playtest : sac visible en fond de grille, slots cliquables, wallet OK.
2. Si trop fort → prochaine passe alpha 0.20 / 0.12 (pas de nouvelle hiérarchie).

---

## Lancement Unity

Prefab Mode : `Assets/Prefabs/Ui/InventoryScreen.prefab`

```
/prefab-ui-3phases
Task ID: [BZ-INV-SAC-FILIGRANE-001]
Prefab: Assets/Prefabs/Ui/InventoryScreen.prefab
Phase: 1
```

Puis coller le bloc Phase 1, **ou** :

```
@Assets/Docs/Bezi/PROMPTS_Bezi_inventory_sac_filigrane.md
[BZ-INV-SAC-FILIGRANE-001] Execute PHASE 1 only. Prefab Mode InventoryScreen.prefab. No .cs. Save. List. STOP.
```
