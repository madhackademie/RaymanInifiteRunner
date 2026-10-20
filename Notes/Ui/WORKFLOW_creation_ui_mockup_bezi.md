# Workflow — création d’écran / popup UI (mockup → prefab → Bezy)

**Validé auteur 2026-10-07** — pipeline utilisé en partie pour la popup récolte farm (`FarmHarvestPanel_FromMockup`).

Objectif : **mockup visuel d’abord**, prefab cible **vide ou minimal**, puis **Bezy** pour la hiérarchie / composants / wiring — **pas** tout coder ou tout prefab-yaml côté Cursor.

Références :
- **Kit bouton modulaire (Frame / Fill, contraintes, Paint.NET vs Unity) :** `Notes/Ui/WORKFLOW_kit_bouton_modulaire.md`
- **Validation tailles / 9-slice / bouton générique / vision UX future :** `Notes/Ui/SPEC_methodologie_ui_ia_kit_bezi.md`
- Skill prefab : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md` (`/prefab-ui-3phases`)
- C# view / RectTransform : `@Notes/Bezi/RULES_bezy_code.md`
- Prompts Bezy : **chat Cursor** (sauf tâche planifiée dans `Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md`)
- Popups runtime : `Notes/Ui/popup_generique.md` (`PopupId`, binding `NavigationHUD` / `UIManager`)

---

## Vue d’ensemble (3 blocs)

| # | Bloc | Qui | Livrable |
|---|------|-----|----------|
| **1** | **Deux mockups** : kit seul, puis kit + pièces manquantes | Auteur + Cursor (spec / art) | Deux références visuelles validées (scène labo ou capture) |
| **2** | **Prefab UI vide** (coquille) | Auteur ou Bezy Ph.1 minimal | `Assets/Prefabs/Ui/…/*.prefab` — root + CanvasGroup si besoin, **sans** tout l’écran |
| **3** | **Génération UI Bezy** | Bezy (phases) | Hiérarchie, Image/TMP/Button, puis wiring `SerializeField` |

---

## 1) Mockup — deux images, kit d’abord

**Décision auteur 2026-10-19.** Chaque écran ou popup part en **deux mockups**, avant tout prefab et avant Bezy.

| Mockup | Contenu | But |
|--------|---------|-----|
| **A — kit** | Uniquement les sprites déjà dans `Assets/Art/Sprites/UI/` (plus l’icône métier déjà en `Sprites/`). Aucune pièce inventée. | Voir ce que le kit sait faire, et **laisser les trous visibles**. |
| **B — kit + pièces** | Les mêmes briques, plus une création **seulement** là où le kit ne couvre pas le geste, ou là où une brique déjà dessinée est plus juste (ex. danger corail encore au Dump). | Proposer le complément, pas un deuxième style. |

Règles communes :

1. **Lister** les pièces UiKit déjà dispo (`Assets/Prefabs/Ui/UiKitPrefab/…`, sprites `Assets/Art/Sprites/UI/…`).
2. **Composer** A avec ces pièces (scène labo `TestUi` **ou** capture). Réutiliser les patterns validés (bouton danger = même famille que `UprootButton`, pas un nested kit étiré).
3. **Trou dans le kit** → une pièce = un rôle (fond slicable **sans** icône ni texte baked). Fond uni magenta (`Notes/Art/WORKFLOW_creation_assets.md`). Déposer au Dump. L’auteur détoure et promeut vers `Sprites/` s’il valide.
4. **Valider** les deux mockups (Game view, **1080×1920** portrait) **avant** d’engager Bezy.

Exemple 2026-10-19 — popup quantité / jeter (`ShopItemPopup`, flux Drop) : A = cadre + boutons kit, stepper bricolé sur `UiKit_Row_list`, Jeter en orange faute de danger en `Sprites/`. B = steppers, plaque chiffre, chip Max, Jeter corail (Dump) + icône compost. Fichiers : `Dump/Ui/mockup_popup_item_qty_kit_20261019.png` et `mockup_popup_item_qty_propose_20261019.png`.

**Exemple 2026-10-07 — popup récolte :** cadre bois + boutons teal / coral / orange batch ; barre progression **abandonnée** ; `CloseButton`, `YieldLabel`, alignement boutons copié depuis `UprootButton`.

---

## 2) Prefab UI vide (coquille prod)

1. Créer le prefab **prod** au bon chemin (ex. `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_FromMockup.prefab`).
2. Contenu minimal :
   - root `RectTransform` full stretch (ou taille fixe si spec) ;
   - **layer UI = 5** ;
   - pas obligatoire de dupliquer tout le mockup à la main — Bezy peut partir d’un root nommé explicitement.
3. **Scène labo** : garder le mockup pour comparaison ; **ne pas** brancher le runtime sur la scène test tant que le prefab prod + wiring ne sont pas OK.
4. **Rollback** : tag git ou binding ancien prefab conservé (`farm.plant.harvest` → ancien `FarmHarvestPanel` jusqu’au switch).

---

## 3) Bezy — génération UI (phases)

Découper comme le skill 3 phases (ou prompts chat séquentiels, **&lt; 3500 car.** par appel) :

| Phase | Contenu | Fin |
|-------|---------|-----|
| **1** | Hiérarchie GameObjects (noms **stables** = ceux du mockup / spec) | Save. STOP. |
| **2** | Composants : Image, Button, TMP, LayoutElement, sprites assignés | Save. STOP. |
| **3** | Wiring : `SerializeField`, scripts view (`HarvestPanelUI`, etc.) — **pas** onClick Inspector si le script branche dans `Awake` | Save. STOP. |

Règles :
- **Ne pas** rescan tout le projet ; chemins prefab + script **explicites** dans le prompt.
- **Pas** Simulate / Play Mode dans le prompt Bezy.
- Logique métier, `PopupId`, services : **Cursor** — pas Bezy.

**Exemple 2026-10-07 :** jobs successifs (batch orange, uproot, yield…) puis **`[BZ-FARM-HARVEST-FROMMOCKUP-WIRE-001]`** — `HarvestPanelUI` sur le prefab FromMockup ; binding runtime dans `NavigationHUD.unity`.

---

## 4) Runtime (après Bezy wiring)

1. Vérifier **PopupId** / `ScreenPopupBinding` (`Notes/Ui/popup_generique.md`).
2. `NavigationHUD` → `UIManager.runtimePopupBindings` : pointer le **prefab prod** mockup (ex. FromMockup).
3. Playtest **Editor** puis **mobile** quand l’auteur le demande (sprites data, taille `PlantIcon`, etc.).

---

## Anti-patterns

- Cursor qui réécrit le YAML prefab/scène UI pour « aller plus vite » (sauf fix minimal + ref Bezy).
- Un seul mega-prompt Bezy (phases 1–3 + wiring + playtest).
- Brancher runtime avant mockup validé ou avant wiring script.
- Nested prefab kit **étiré** pour boutons 9-slice (préférer Image Sliced + taille fixe sur le mockup).

---

## Checklist rapide (nouvel écran)

- [ ] Mockup OK (kit + art Dump promu si besoin)
- [ ] Prefab prod coquille créé, noms GO figés
- [ ] Bezy Ph.1 → Ph.2 → Ph.3 (ou équivalent chat)
- [ ] Script view / binding popup côté Cursor si nouveau `PopupId`
- [ ] Binding `NavigationHUD` + playtest
- [ ] Entrée todo / journal si décision auteur (merge, mobile, suite feature)
