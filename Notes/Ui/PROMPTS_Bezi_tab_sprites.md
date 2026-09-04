# Prompts Bezy — sprites onglets HUD `[BZ-TAB-SPRITES-001]`

**Statut :** brouillon — **ne pas envoyer à Bezy**.

**Tâche session :** `[P0-TAB-SPRITES-001]` — mise en place + polish des sprites sur les onglets.  
**Job Bezy :** `[BZ-TAB-SPRITES-001]` — `/prefab-ui-3phases`  
**Skill :** `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`  
**File :** `Notes/Bezi/BEZY_QUEUE.md`  
**Art (hors Bezy) :** Vague H `Notes/Art/PROMPT_generation_icones.md` — Dump → promo auteur `Sprites/UI/` **avant** wiring.

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

## Checklist review (après livraison Bezy)

- [ ] Un sprite par onglet ciblé, lisible en taille mobile
- [ ] États idle / selected (et press si demandé) cohérents
- [ ] Layers UI = 5
- [ ] `NavTab` press inchangé sauf consigne contraire
- [ ] Playtest auteur hors prompt Bezy
