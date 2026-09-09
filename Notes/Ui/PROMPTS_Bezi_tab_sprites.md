# Prompts Bezy — sprites onglets HUD `[BZ-TAB-SPRITES-001]`

**Statut :** pilotes en cours — onglets 1–3 livrés, **TabVente** = dernier prompt ci-dessous.

### Régression connue (2026-09-09)

- **Wallet / solde gold** en haut de la barre nav (`NavigationHUD`) **absent** après passes Bezy onglets — **à corriger plus tard** (pas bloquant ce prompt).
- ID suivi : `[P0-NAV-WALLET-REG-001]` dans `Notes/Todo_project.md`.

**Tâche session :** `[P0-TAB-SPRITES-001]` — mise en place + polish des sprites sur les onglets.  
**Job Bezy :** `[BZ-TAB-SPRITES-001]` — `/prefab-ui-3phases`  
**Skill :** `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**File :** `Notes/Bezi/BEZY_QUEUE.md`  
**Art (hors Bezy) :** Vague H `Notes/Art/PROMPT_generation_icones.md` — Dump → promo auteur `Sprites/UI/` **avant** wiring.  
**Hub Plus (futur) :** `Notes/Ui/SPEC_features_hub_plus.md` — variante barre A (3+Plus) vs B (4+Plus) après playtest mobile.

**Succès Bezy = Save + liste changements. STOP. Pas de Simulate / Play Mode.**

---

## Gate (obligatoire)

1. L’auteur fournit la **liste des changements visuels** (quels onglets, quels sprites, idle / selected / press, tailles, labels, etc.).
2. Cursor rédige **Phase 1 → 2 → 3** ici (une phase par appel, moins de 3500 caractères).
3. **Validation auteur** des prompts collables.
4. Ensuite seulement : file `BEZY_QUEUE.md` + lancement Unity.

Tant que les blocs Phase ci-dessous sont vides : **aucun envoi Bezy**.

---

## Cible (à figer avec le brief)

**Probable — NavigationHUD** (scène `Assets/Scenes/NavigationHUD.unity`) :

- `TabAventures` / `TabInventaire` / `TabShop` / `TabVente`
- Images déjà exposées dans `Assets/Scripts/UI/NavigationHUD.cs` (`tabAventuresIcon`, etc.)
- Press existant `[BZ-POLISH-006]` : `NavTab.controller` — **ne pas casser** sans consigne.

**Optionnel — barre inventaire** (`InventoryScreen` / `InventoryFilterBar`) : uniquement si le brief auteur le demande. Ne pas mélanger HUD nav et filtres inventaire dans le même prompt.

**Hors scope Bezy :** génération PNG, promo Dump → Sprites, logique C# (`SceneNavigator`), Simulate.

**Prérequis Editor :** ouvrir la scène / le prefab cible en mode dédié avant d’envoyer (workaround path Bezy — `Notes/Bezi/README_bezi.md`).

---

## Phase 1 — Shell / hiérarchie sprites

```
(à rédiger après brief auteur — ne pas envoyer)
```

---

## Phase 2 — Composants visuels (Images / sprites)

```
(à rédiger après brief auteur — ne pas envoyer)
```

---

## Phase 3 — Wiring SerializeField / polish

```
(à rédiger après brief auteur — ne pas envoyer)
```

---

## Pilotes livrés (onglet par onglet)

| Onglet | Sprite | Statut |
|--------|--------|--------|
| `TabAventures` | `IconePlay.png` | OK pilote |
| `TabInventaire` | `IconeInventaire.png` | OK pilote |
| `TabShop` | `IconeMarket.png` | OK pilote |
| `TabVente` | `GoldBill.png` (V0 — remplacer par `IconeVente.png` quand promu) | **prompt ci-dessous** |

---

## Pilote 4 — TabVente (copier-coller Bezy)

**Prérequis :** Trim `GoldBill.png` dans Sprite Editor. Ouvrir `NavigationHUD.unity`.

```
[BZ-TAB-SPRITES-001] PILOT TabVente — copy TabAventures icon layout. Wait success. STOP.

OPEN Assets/Scenes/NavigationHUD.unity first. Layer UI = 5.
Do NOT modify C#. Do NOT change TabAventures, TabInventaire, TabShop.

File ONLY: Assets/Scenes/NavigationHUD.unity
Reference: TabAventures / TabInventaire / TabShop.

Target: TabVente ONLY (sale channels tab).

1) REMOVE VerticalLayoutGroup from TabVente.
2) COPY structure from TabAventures:
   - SelectedFrame (inactive default) + BorderTop/Bottom/Left/Right (3px), stretch -4,-4
   - Border color RGB(1, 0.78, 0.2) alpha 1
   - Icon: stretch anchors (0,0)-(1,1), sizeDelta -7,-7
   - Sprite: Assets/Art/Sprites/UI/Currency/GoldBill.png
   - Image: white, Preserve Aspect ON, Raycast OFF
3) Label: INACTIVE.
4) Wire NavigationHUD on HUDRoot:
   - tabSaleChannelsIcon → TabVente/Icon
   - tabSaleChannelsSelectedFrame → TabVente/SelectedFrame
5) KEEP Animator NavTab, Button Animation, OnClick OnTabSaleChannelsClicked.

Do NOT restore wallet UI (known regression — separate task).

Save. List changes. STOP.
```

---

## Checklist review (après livraison Bezy)

- [x] `TabAventures` — sprite + cadre
- [x] `TabInventaire` — sprite + cadre
- [x] `TabShop` — sprite + cadre
- [ ] `TabVente` — sprite + cadre
- [ ] Variantes tab 128² — `Notes/Art/PROMPT_generation_icones.md` § **Vague H-nav** (H-nav-1 → 4)
- [ ] Trim sprite par onglet (lisibilité) — ou remplacer par H-nav après promo
- [ ] Playtest navigation 4 onglets
- [ ] `[P0-NAV-WALLET-REG-001]` restaurer wallet barre nav
