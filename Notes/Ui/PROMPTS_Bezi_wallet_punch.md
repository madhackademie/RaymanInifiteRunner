# Prompts Bezy — Wallet punch +1/−1 `[BZ-POLISH-015]`

> **PARK UX (2026-07-27)** — Bezy Ph.1–3 livré, mais **surface invisible au delta** : pas de gain monnaie avec l’inventaire ouvert ; solde shop Header non perçu / popup se ferme après débit.  
> **Ne plus investir** Bezy/Cursor dessus. Assets + hooks `Gain`/`Spend` **conservés** (réutilisables si chip HUD ou feedback local plus tard).  
> **Reprise** après décision GDD : question ouverte dans `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §5.8 (+ `Notes/GDD/NOTE_affichage_monnaie_hud.md`).

**Pattern :** même famille que PA `SpendPulse` (`Assets/Animations/UI/`).  
**Scripts Cursor (déjà prêts, ne pas recréer) :**
- `Assets/Scripts/UI/Inventory/CurrencyBalanceUI.cs` — triggers `Gain` / `Spend`
- `Assets/Scripts/UI/Inventory/WalletWidget.cs` — idem sur solde primary

**Cibles runtime (park — pas le bon moment UX) :**
- Shop : `ShopItemPopup` → `WalletBalance` (`CurrencyBalanceUI`)
- Inventaire : `InventoryScreen` → `WalletWidget`

**Règles :** ne pas rescanner tout le projet ; pas de nouveaux scripts C# ; pas de nouveaux sprites ; un prompt = une phase ; attendre OK avant la suivante ; **pas** de Simulate / Play Mode.

### Historique

- **Phase 1** : clips + controller partagés — **OK** (Bezy 2026-07-27)
- **Phase 2** : wiring ShopItemPopup WalletBalance — **OK** (Bezy 2026-07-27)
- **Phase 3** : wiring InventoryScreen WalletWidget — **OK** (Bezy 2026-07-27)
- **Park UX** : playtest / polish **suspendus** jusqu’à décision affichage monnaie (GDD §5.8)

---

## Phase 1 — Clips + Animator Controller — **CLOS**

Validé repo (Bezy 2026-07-27) :
- `WalletBalance_Idle.anim` — path `""`, scale 1
- `WalletBalance_GainPulse.anim` — 1 → 1.1 → 0.98 → 1, 0.28 s, path `""`
- `WalletBalance_SpendPulse.anim` — 1 → 1.06 → 0.94 → 1, 0.28 s, path `""`
- `WalletBalance.controller` — triggers `Gain` / `Spend`, états Idle / GainPulse / SpendPulse

### Archive prompt Ph.1

```
(voir historique — livré OK)
```

---

## Phase 2 — Wiring ShopItemPopup WalletBalance — **CLOS**

Validé repo (Bezy 2026-07-27) :
- Animator sur `WalletBalance` → `WalletBalance.controller`
- `CurrencyBalanceUI.animator` câblé
- Update Mode Normal, Culling Always Animate
- Labels / format / currencyItem inchangés

### Archive prompt Ph.2

```
(voir historique — livré OK)
```

---

## Phase 3 — Wiring InventoryScreen WalletWidget — **CLOS**

Validé repo (Bezy 2026-07-27) :
- `WalletWidget_Idle` / `GainPulse` / `SpendPulse` — path `CollapsedRow`
- `WalletWidget.controller` — triggers `Gain` / `Spend`
- Animator sur `WalletWidget` → `WalletWidget.controller`
- `WalletWidget.animator` câblé

### Archive prompt Ph.3

```
(voir historique — livré OK)
```

---

## Checklist validation auteur — **PARK** (ne pas jouer tant que surface GDD non tranchée)

| Critère | OK? |
|---------|-----|
| Triggers exacts `Gain` / `Spend` | Bezy OK |
| Shop : achat → solde baisse → SpendPulse | **N/A UX** — wallet peu/pas vu |
| Inventaire ouvert : gain monnaie → GainPulse | **N/A UX** — pas de gain dans cet écran |
| 1er refresh / OnEnable : **pas** de pulse (baseline) | hooks OK |
| Pas de jitter layout permanent (scale revient à 1) | |

---

## Anti-patterns

- Ne pas animer la couleur TMP (conflit lisibilité / localisation)
- Ne pas animer `ExpandedPanel`
- Ne pas demander Simulate à Bezy
- Ne pas fusionner Ph.1–3 dans un seul prompt
- **Ne pas relancer de polish Bezy wallet** tant que §5.8 GDD n’est pas tranché
