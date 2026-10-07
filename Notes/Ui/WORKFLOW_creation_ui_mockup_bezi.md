# Workflow — création d’écran / popup UI (mockup → prefab → Bezy)

**Validé auteur 2026-10-07** — pipeline utilisé en partie pour la popup récolte farm (`FarmHarvestPanel_FromMockup`).

Objectif : **mockup visuel d’abord**, prefab cible **vide ou minimal**, puis **Bezy** pour la hiérarchie / composants / wiring — **pas** tout coder ou tout prefab-yaml côté Cursor.

Références :
- **Validation tailles / 9-slice / bouton générique / vision UX future :** `Notes/Ui/SPEC_methodologie_ui_ia_kit_bezi.md`
- Skill prefab : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md` (`/prefab-ui-3phases`)
- C# view / RectTransform : `@Notes/Bezi/RULES_bezy_code.md`
- Prompts Bezy : **chat Cursor** (sauf tâche planifiée dans `Notes/Bezi/PROMPTS_Bezi_A_FAIRE.md`)
- Popups runtime : `Notes/Ui/popup_generique.md` (`PopupId`, binding `NavigationHUD` / `UIManager`)

---

## Vue d’ensemble (3 blocs)

| # | Bloc | Qui | Livrable |
|---|------|-----|----------|
| **1** | **Mockup** avec le **UiKit** existant | Auteur + Cursor (spec / art) | Référence visuelle validée (scène labo ou capture) |
| **2** | **Prefab UI vide** (coquille) | Auteur ou Bezy Ph.1 minimal | `Assets/Prefabs/Ui/…/*.prefab` — root + CanvasGroup si besoin, **sans** tout l’écran |
| **3** | **Génération UI Bezy** | Bezy (phases) | Hiérarchie, Image/TMP/Button, puis wiring `SerializeField` |

---

## 1) Mockup — kit d’abord, génération si manquant

1. **Lister** les pièces UiKit déjà dispo (`Assets/Prefabs/Ui/UiKitPrefab/…`, sprites `Assets/Art/Sprites/UI/…`).
2. **Composer** le mockup avec ces pièces :
   - scène labo dédiée (ex. `TestUi.unity`) **ou** prefab mockup temporaire ;
   - **réutiliser** les patterns validés (ex. bouton danger = même 9-slice que `UprootButton`, pas nested kit étiré).
3. **Élément absent du kit** :
   - **Cursor / générateur** → déposer brut dans `Assets/Art/Assets Store Dump/…` (fond uni magenta, voir `Notes/Art/WORKFLOW_creation_assets.md`) ;
   - auteur : détourage + **promo** vers Sprites si validé ;
   - option : pièce UiKit prefab réutilisable si la même brique revient souvent.
4. **Valider** le mockup auteur (Game view, résolution cible **1080×1920 Portrait**) **avant** d’engager Bezy sur le prefab prod.

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
