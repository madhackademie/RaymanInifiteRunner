# Backlog art — création (Dump) → validation → Sprites

**Création :** 2026-08-31 · **MAJ :** 2026-09-23

> **Workflow complet (outils, Comfy Krea/Kontext/Qwen → ChatGPT, licences, UI kit) :** `Notes/Art/WORKFLOW_creation_assets.md` · stack Comfy : `Notes/Art/GUIDE_comfy_flux_models_local.md`  
> **Direction monde (2026-09-08) :** vue **iso 2:1** + **cartoon** (polish Zombie Castaways, sans thème zombie). Prompt : `Notes/Art/PROMPT_assets_monde_iso.md`. Icônes UI (§1) restent bois rustique.  
**Source unique** de tout l’art à produire (icônes UI, stades monde, IBC, fishtank, bandeaux…).  
**Usage :** 1 asset / jour (~15 min) — générer → Dump → **après OK auteur** copier dans le dossier `Sprites/` de la ligne.

**Contour (2026-09-23) :** tout prompt (§1 ou prompts spécialisés plus bas) doit inclure **EDGE_LOCK** — bordure **noire nette**, **pas d’anti-aliasing** sur la silhouette. Snippet EN : `Notes/Art/WORKFLOW_creation_assets.md` §3.1.

## Flux (toujours le même)

```
Cette note (à générer)
    → PNG dans Assets/Art/Assets Store Dump/…  (statut : en dump)
    → OK auteur
    → copie dans Assets/Art/Sprites/…          (statut : promu)
```

- **Jamais** coller un brut Dump sur un prefab / `PlantDefinition` / UI. Règle : `.cursor/rules/art_asset_dump.mdc`.
- Ne **pas** dupliquer la liste dans `Notes/Todo_project.md` (sauf chantier code, ID `P0-*` / `BL-*`).

### Où ajouter un élément

**Ici, §3** : nouvelle ligne dans la vague (A–H icônes, **W monde**).

Colonnes mini : `#` · Objet FR · Prompt objet · Dump · **Promo Sprites** · Statut `à générer`.

### Miroir Dump → Sprites (après validation)

Le sous-dossier Dump annonce déjà la cible. Promo = copier vers le chemin `Sprites/` de la colonne (créer le dossier s’il n’existe pas).

| Dump (brut) | Promo (validé) |
|-------------|----------------|
| `…/Dump/Ui/Items/` | `Assets/Art/Sprites/UI/Inventory/` |
| `…/Dump/Ui/Halo/` | `Assets/Art/Sprites/UI/Progression/` |
| `…/Dump/Ui/Shields/` | `Assets/Art/Sprites/UI/Biofiltre/` |
| `…/Dump/Ui/Craft/` | `Assets/Art/Sprites/UI/Craft/` |
| `…/Dump/Ui/Currency/` | `Assets/Art/Sprites/UI/Currency/` |
| `…/Dump/Ui/Quests/` | `Assets/Art/Sprites/UI/Quests/` |
| `…/Dump/Ui/Dashboard/` | `Assets/Art/Sprites/UI/Dashboard/` |
| `…/Dump/Ui/Nav/` | `Assets/Art/Sprites/UI/` |
| `…/Dump/Ui/Nav/Tabs/` | `Assets/Art/Sprites/UI/Nav/Tabs/` |
| `…/Dump/Ui/` (nav générique) | `Assets/Art/Sprites/UI/` |
| `…/Dump/Ui/KitRefonte/` | `Assets/Art/Sprites/UI/Kit/` |
| `…/Dump/Ui/Tab_Plus/` | `Assets/Art/Sprites/UI/` ou `Sprites/UI/FeaturesHub/` |
| `…/Dump/Plantes/<Nom>/` | `Assets/Art/Sprites/Plantes/<Nom>/` |
| `…/Dump/ElementProd/Biofiltre/` | `Assets/Art/Sprites/Farm/Biofiltre/` |
| `…/Dump/Poisson/` | `Assets/Art/Sprites/Farm/Poisson/` |

Le **§1** est le prompt **icône UI** (fond uni de détourage, pas transparent). Les lignes monde (vague W) peuvent utiliser un autre prompt : le noter dans la colonne Prompt.

---

## 1) Prompt générique (copier-coller)

Remplacer uniquement `[VOTRE OBJET ICI]` :

```
A 2D casual mobile game icon of [VOTRE OBJET ICI], cartoon style, vibrant colors, on a flat solid #FF00FF background. No transparency. Made with thick rustic light brown wooden textures, thick pure black outer outline, hard edges with NO anti-aliasing on the silhouette, smooth cel shading inside, cozy farming game aesthetic, high quality UI asset. EDGE LOCK: no gray semi-transparent fringe, no outer glow. BACKGROUND: #FF00FF only if that color is absent from the subject; otherwise #0000FF. Use #00FF00 only when the subject contains no green.
```

**Exemple :** même phrase avec `a packet of lettuce seeds`.

**Snippet EDGE_LOCK seul (ajouter si le générateur oublie le contour) :** voir bloc EN dans `Notes/Art/WORKFLOW_creation_assets.md` §3.1.

### Consignes de remplissage

- Objet **un seul**, lisible à **64–96 px** (inventaire mobile).
- **Contour :** bordure **noire nette**, **sans anti-aliasing** sur la découpe (évite halo sur HUD sombre — cf. `[P0-FARM-SPRITE-ALPHA-001]`).
- Nommer le fichier : `Icone_[Objet]_[YYYYMMDD].png` (ex. `Icone_GrainesAntiSlug_20260831.png`).
- Déposer dans le sous-dossier Dump de la famille (tableau §3).
- Fond uni `#FF00FF` (ou une autre couleur absente du dessin) = prévu : l’auteur détoure en deux clics, puis importe en sprite. Ne pas coller le PNG brut sur un prefab.

### Variantes (optionnel, même style)

| Besoin | Ajouter à la fin du prompt |
|--------|----------------------------|
| Paquet / sachet | `packaged as a small seed packet with a simple label` |
| Consommable (dose) | `shown as a small pouch or bottle, not a landscape scene` |
| Structure (serre, bac) | `isometric 3/4 view of the object only, no environment` |
| Portrait canal vente | **ne pas** utiliser ce prompt — voir **`Notes/Art/PROMPT_bandeaux_vente_generique.md`** (ratio ~6:1, 2048×360) |

---

## 2) Routine quotidienne

Voir **`Notes/Art/WORKFLOW_creation_assets.md` §8** pour l’arbre outils (ChatGPT / Comfy / Bezy).

1. Prendre **la première ligne `à générer`** du §3 (ordre = priorité produit).
2. Coller le prompt + objet anglais (colonne *Prompt objet*).
3. Sauver le PNG dans le Dump indiqué.
4. Dans ce fichier : passer la ligne en `en dump` + noter le nom de fichier.
5. **Après OK auteur** : copier le PNG vers le dossier **Promo Sprites** du tableau (miroir § ci-dessus). Statut `promu`. Pas de promo « parce que c’est joli » sans OK.

Hors file : si une session code a besoin d’un asset **tout de suite**, le faire ce jour-là et **remonter** la ligne en tête.

---

## 3) File unique (ordre quotidien)

Statut dans **cette** note seulement (`à générer` / `en dump` / `promu`). Les tâches projet restent dans `Notes/Todo_project.md`.

Colonne **Promo** = dossier `Sprites/` après validation (voir miroir en tête de note).

### Vague A — boucle ferme actuelle (priorité)

| # | Objet FR | Prompt objet (anglais) | Dump | Statut | Fichier |
|---|----------|------------------------|------|--------|---------|
| A1 | Graines de laitue | `a small rustic packet of lettuce seeds` | `Dump/Ui/Items/` | à générer | |
| A2 | Laitue récoltée (feuilles) | `a fresh harvested lettuce head` | `Dump/Ui/Items/` | à générer | |
| A3 | Graines de tomate | `a small rustic packet of tomato seeds` | `Dump/Ui/Items/` | à générer | |
| A4 | Tomate récoltée | `a ripe red tomato` | `Dump/Ui/Items/` | à générer | |
| A5 | Compost (icône item) | `a small wooden bucket of dark compost` | `Dump/Ui/Items/` | à générer | |
| A6 | Billet or (variante icône slot) | `a single gold banknote with a farm emblem` | `Dump/Ui/Currency/` | à générer | |

> A1–A2 : des sprites laitue existent déjà en runtime (`Sprites/Plantes/Laitue/`) mais le **fond noir** est un bug import (`[P0-FARM-SPRITE-ALPHA-001]`). Nouvelle icône UI propre = file art ; le fix alpha = autre chantier.

### Vague B — halo inventaire (6 pistes)

| # | Objet FR | Prompt objet | Dump | Statut | Fichier |
|---|----------|--------------|------|--------|---------|
| B1 | Piste Commerce | `a wooden market stall sign with a gold coin` | `Dump/Ui/Halo/` | à générer | |
| B2 | Piste Culture plantes | `a wooden sign with a sprouting seedling` | `Dump/Ui/Halo/` | à générer | |
| B3 | Piste Culture poissons | `a wooden sign with a small cartoon fish` | `Dump/Ui/Halo/` | à générer | |
| B4 | Piste Agronomie | `a wooden sign with a soil trowel and leaf` | `Dump/Ui/Halo/` | à générer | |
| B5 | Piste Logistique | `a wooden sign with a small delivery crate` | `Dump/Ui/Halo/` | à générer | |
| B6 | Piste Technologie | `a wooden sign with a simple gear and pipe` | `Dump/Ui/Halo/` | à générer | |

IDs code : `ProgressionTrackId` (Commerce, PlantCulture, FishCulture, Agronomy, Logistics, Technology). P7/P8 réservés : pas d’icône.

### Vague C — shields biofiltre (secondaires)

Réf. `Notes/GDD/SPEC_biofiltre_slots_shields.md`.

| # | Objet FR | Prompt objet | Dump | Statut | Fichier |
|---|----------|--------------|------|--------|---------|
| C1 | Graines anti-limaces (niv.1) | `a small packet of anti-slug pellets or seeds` | `Dump/Ui/Shields/` | à générer | |
| C2 | Barrière cuivre (niv.2) | `a short copper garden barrier strip` | `Dump/Ui/Shields/` | à générer | |
| C3 | Barrière cuivre électrifiée (niv.3) | `an electrified copper garden barrier with a tiny spark` | `Dump/Ui/Shields/` | à générer | |
| C4 | Nématodes (niv.4) | `a small bottle of beneficial nematodes` | `Dump/Ui/Shields/` | à générer | |
| C5 | Anti-souris | `a wooden mousetrap or mesh mouse guard` | `Dump/Ui/Shields/` | à générer | |
| C6 | Anti-oiseau | `a garden bird net or scarecrow head icon` | `Dump/Ui/Shields/` | à générer | |
| C7 | Anti-fourmis | `an ant barrier powder pouch` | `Dump/Ui/Shields/` | à générer | |
| C8 | Anti-moisissure | `a small bottle of garden anti-mold spray` | `Dump/Ui/Shields/` | à générer | |

### Vague D — structures primaires (serre)

| # | Objet FR | Prompt objet | Dump | Statut | Fichier |
|---|----------|--------------|------|--------|---------|
| D1 | Voile de forçage (serre niv.1) | `a garden frost fleece cover folded on wood` | `Dump/Ui/Shields/` | à générer | |
| D2 | Bâche à bulles (niv.2) | `a roll of bubble wrap greenhouse plastic` | `Dump/Ui/Shields/` | à générer | |
| D3 | Serre géodésique (niv.3) | `a small geodesic greenhouse` | `Dump/Ui/Shields/` | à générer | |

Primaires 2 et 3 (TBD GDD) : ajouter ici quand le rôle est tranché.

### Vague E — atelier craft (V0)

Réf. `Notes/GDD/SPEC_craft_atelier_aquaponique.md`.

| # | Objet FR | Prompt objet | Dump | Statut | Fichier |
|---|----------|--------------|------|--------|---------|
| E1 | Scrap / ferraille | `a small pile of scrap metal parts` | `Dump/Ui/Craft/` | à générer | |
| E2 | Fibre / corde | `a coil of rustic natural fiber rope` | `Dump/Ui/Craft/` | à générer | |
| E3 | Connecteur PVC | `a simple PVC pipe connector fitting` | `Dump/Ui/Craft/` | à générer | |
| E4 | Media filtrant (argile) | `a handful of expanded clay pebbles` | `Dump/Ui/Craft/` | à générer | |
| E5 | Kit tuyauterie | `a bundled kit of small water pipes` | `Dump/Ui/Craft/` | à générer | |
| E6 | Bac DWC particulier | `a small deep water culture planter box` | `Dump/Ui/Craft/` | à générer | |
| E7 | Bac DWC pro | `a larger sturdy professional DWC grow bed` | `Dump/Ui/Craft/` | à générer | |
| E8 | Pompe à eau | `a small aquarium water pump` | `Dump/Ui/Craft/` | à générer | |
| E9 | Capteur pH | `a simple pH sensor probe` | `Dump/Ui/Craft/` | à générer | |

### Vague F — plantes palier 1 puis suivants (icônes items)

Cible GDD : **15 plantes**, 3 par palier (lvl 1 / 3 / 5 / 7 / 10). Laitue + tomate = Vague A. Ici = **graine + récolte** par plante (2 jours / plante).

| # | Palier | Plante | Prompt graine | Prompt récolte | Statut graine | Statut récolte |
|---|--------|--------|---------------|----------------|---------------|----------------|
| F1 | 1 | Basilic | `a packet of basil seeds` | `a bunch of fresh basil leaves` | à générer | à générer |
| F2 | 1 | Blette (swiss chard) | `a packet of swiss chard seeds` | `a bunch of colorful swiss chard leaves` | à générer | à générer |
| F3 | 3 | Épinard | `a packet of spinach seeds` | `a bunch of spinach leaves` | à générer | à générer |
| F4 | 3 | Roquette | `a packet of arugula seeds` | `a bunch of arugula leaves` | à générer | à générer |
| F5 | 3 | Persil | `a packet of parsley seeds` | `a bunch of parsley` | à générer | à générer |
| F6 | 5 | Coriandre | `a packet of cilantro seeds` | `a bunch of cilantro` | à générer | à générer |
| F7 | 5 | Chou kale | `a packet of kale seeds` | `a bunch of curly kale` | à générer | à générer |
| F8 | 5 | Menthe | `a packet of mint seeds` | `a bunch of mint leaves` | à générer | à générer |
| F9 | 7 | Poivron | `a packet of bell pepper seeds` | `a ripe red bell pepper` | à générer | à générer |
| F10 | 7 | Concombre | `a packet of cucumber seeds` | `a fresh cucumber` | à générer | à générer |
| F11 | 7 | Haricot | `a packet of green bean seeds` | `a handful of green beans` | à générer | à générer |
| F12 | 10 | Fraise | `a packet of strawberry seeds` | `a ripe strawberry` | à générer | à générer |
| F13 | 10 | Radis | `a packet of radish seeds` | `a bunch of radishes` | à générer | à générer |

> Palier 10 fruits gourmands (tomate déjà en A) = plutôt **après** sabloponie / minéralisation. Liste F = proposition rendu mixte (feuilles tôt, fruits plus tard) — à recaler quand l’analyse MVP sera triée.

### Vague G — économie / quêtes / nav (plus tard)

| # | Objet FR | Prompt objet | Dump | Statut | Fichier |
|---|----------|--------------|------|--------|---------|
| G1 | Boulon wallet | → **H9** (ne pas dupliquer) | `Dump/Ui/Currency/` | alias H9 | |
| G2 | Paquet graines commun (daily) | `a common brown seed bundle tied with twine` | `Dump/Ui/Quests/` | à générer | |
| G3 | Paquet graines uncommon (hebdo) | `a uncommon green-ribbon seed bundle` | `Dump/Ui/Quests/` | à générer | |
| G4 | Paquet graines rare (mensuel) | `a rare gold-ribbon seed bundle` | `Dump/Ui/Quests/` | à générer | |
| G5 | Icône Atelier (nav) | `a wooden workbench with a hammer` | `Dump/Ui/` | à générer | |
| G6 | Icône Quêtes (nav) | → **H4** (onglet parent) | `Dump/Ui/` | alias H4 | |

Déjà en Dump (ne pas regénérer tant que non validé) : `IconeMarket.png`, `IconePlay.png`, `IconeInventaire.png` → promo cible `Sprites/UI/` après OK.

### Vague H — onglets HUD / dashboard / wallets (auteur 2026-09-02)

Prompt §1 (icône fond blanc). Quêtes : **1 onglet parent + 3 sous-classes** (daily / weekly / monthly).

**Wiring Bezy (après promo Sprites) :** `[P0-TAB-SPRITES-001]` / `[BZ-TAB-SPRITES-001]` — `@Assets/Docs/Bezi/PROMPTS_Bezi_tab_sprites.md`. Ne pas lancer Bezy tant que le brief visuel n’est pas validé.

| # | Objet FR | Prompt objet | Dump | Promo Sprites | Statut | Fichier |
|---|----------|--------------|------|---------------|--------|---------|
| H1 | Onglet Multiverse | prompt dédié ci-dessous (panier + vaisseau + runner + poulpe) | `Dump/Ui/Nav/` | `Sprites/UI/` | à générer | |
| H2 | Éclair consommation électricité | `a bright lightning bolt for electricity usage` | `Dump/Ui/Dashboard/` | `Sprites/UI/Dashboard/` | à générer | |
| H3 | Goutte consommation eau | `a single water droplet for water usage` | `Dump/Ui/Dashboard/` | `Sprites/UI/Dashboard/` | à générer | |
| H4 | Onglet Quêtes (parent) | `a wooden quest board or clipboard tab icon` | `Dump/Ui/Quests/` | `Sprites/UI/Quests/` | à générer | |
| H5 | Quêtes daily (sous-classe) | `a wooden daily quest icon with a small sun` | `Dump/Ui/Quests/` | `Sprites/UI/Quests/` | à générer | |
| H6 | Quêtes weekly (sous-classe) | `a wooden weekly quest icon with a seven-day calendar` | `Dump/Ui/Quests/` | `Sprites/UI/Quests/` | à générer | |
| H7 | Quêtes monthly (sous-classe) | `a wooden monthly quest icon with a full moon calendar` | `Dump/Ui/Quests/` | `Sprites/UI/Quests/` | à générer | |
| H8 | Onglet shop monnaie quêtes | `a wooden special shop stall tab icon with a metal bolt coin` | `Dump/Ui/Nav/` | `Sprites/UI/` | à générer | |
| H9 | Boulon (monnaie quête, wallet) | `a large rustic metal bolt token for a wallet currency icon` | `Dump/Ui/Currency/` | `Sprites/UI/Currency/` | à générer | |

H9 = icône wallet (à côté du billet or A6). H8 = **onglet** du shop qui dépense des boulons, pas la monnaie elle-même.

### Vague H-farm — popup plante (Bezy backlog UI bois)

Task Bezy : **`[BL-FARM-HARVEST-BATCH-BTN-UI-001]`** — remplacer le carré blanc du bouton récolte batch (`HarvestPanelUI`). **Mockup Cursor** (ou génération) **avant** Bezy ; wiring déjà OK `[BZ-FARM-HARVEST-BATCH-BTN-001]`.

| # | Objet FR | Prompt objet | Dump | Promo Sprites | Statut | Task |
|---|----------|--------------|------|---------------|--------|------|
| H-farm-1 | Barre bouton « récolte batch » (popup plante) | Bandeau horizontal bois + parchemin, gants + pousses à gauche, centre vide TMP ; voir prompt § H-farm-1 | `Dump/Ui/Farm/UiKit_BtnBatchHarvest_bar_20261005.png` | `Sprites/UI/Farm/` (après OK) | **généré — validation / 9-slice auteur** (2026-10-05) | `[BL-FARM-HARVEST-BATCH-BTN-UI-001]` |
| H-farm-2 | Fond 9-slice barre batch (**sans** icône) | Même barre que H-farm-1 mais **aucune** illustration ; cap gauche parchemin vide pour icône en enfant Unity | `Dump/Ui/Farm/UiKit_Bar_batchHarvest_9s_20261005.png` | `Sprites/UI/Farm/` (après OK) | **reporté** (test barre+icône d’abord) | `[BL-FARM-HARVEST-BATCH-BTN-UI-001]` |
| H-farm-4 | **Piste** progression stade (couloir vide) | Barre horizontale bois + rainure parchemin **vide** pour `Image` fill enfant ; remplace `UiKit_Row_list` sur mockup | `Dump/Ui/Farm/UiKit_Bar_progressTrack_9s_20261005.png` | — | **abandonné auteur 2026-10-07** (polish popup — timer TMP suffit) | ~~`stageProgressFill`~~ |
| H-farm-3 | (option) Icône batch seule | Crop / regen gant + pousses, fond magenta, **Simple** Image enfant | `Dump/Ui/Farm/UiKit_Icon_batchHarvest_20261005.png` | `Sprites/UI/Farm/` | **option** (crop H-farm-1 possible) | idem |
| H-farm-5 | **Croix close** popup récolte (rond rouge, X blanc) | Bouton circulaire rouge vif, X blanc, cel + highlight 3D — **pas** bois (illisible sur panneau bois) ; prompt § H-farm-5 | `Dump/Ui/Farm/UiKit_BtnClose_red_20261007.png` | `Sprites/UI/Farm/` (après OK) | **clos auteur 2026-10-07** (visuel OK sur prefab mockup ; regen optionnel) | ~~`[BZ-FARM-HARVEST-PANEL-BUILD-002]`~~ |
| H-farm-6 | **Timer popup** (option post-MVP) | **MVP :** TMP **centré dans** `ProgressTrack` (pas de ligne séparée) ; art option = icône horloge 64px à côté du texte | `Dump/Ui/Farm/UiKit_Icon_timer_20261007.png` (icône seule) | `Sprites/UI/Farm/` | **backlog** | timer in-track |

**Déjà en Dump (pas de crédit art sauf regen) :** `UiKit_BtnClose_wood_20261005` (**remplacé visuellement par H-farm-5**), `UiKit_BtnDanger_coral_20261005`, `UiKit_Badge_stage_20261005`, **`UiKit_Bar_fill_teal_20261005`** (remplissage progression — `Image` **Filled** horizontal, pas promu Sprites) → détourage + promo.

**Mockup popup (2026-10-07) :** pas de barre progression stade — **timer TMP** (`TimerLabel`) seulement. Art H-farm-4 / fill teal = hors scope MVP (fichiers Dump conservés, pas de promo).

#### Prompt ChatGPT — H-farm-5 Croix close rouge (coller tel quel)

**Dans ChatGPT :** joindre `UiKit_Panel_9s_20260919.png` ou `mockup_popup_recolte_roquette_20261005.jpg` (contexte bois). Fichier brut : `Assets/Art/Assets Store Dump/Ui/Farm/UiKit_BtnClose_red_20261007.png`. Détourage auteur (fond magenta).

```
STYLE LOCK — RaymanFarm UI (strict):
- 2D casual mobile game UI, cozy aquaponic farm, FUN cartoon (chunky, playful).
- Thick dark brown (#3d2817) OUTLINES on the button shape and on the X glyph. Cel shading, warm saturated colors.
- NOT photoreal, NOT 3D render, NOT glassmorphism, NOT flat Material UI.
- Flat orthographic front view. NO isometric.

EDGE LOCK: pure black (#000000) outer outline on the entire button silhouette. NO anti-aliasing — hard pixel-clean alpha edge after author removes background, no gray fringe, no outer glow.

BACKGROUND: one flat solid color that does not appear anywhere in the subject. No gradient, no floor, no cast shadow. Use #FF00FF. Do not output transparency.

One UI asset only — popup close control (must read clearly on light brown wood panel).

SUBJECT: a single CIRCULAR close button, square canvas 512x512 pixels, button fills ~88% of canvas (true circle, not oval). Painted red lacquer / warm coral-red (#d94a3a to #c62828 range), NOT wood texture on the disk.

3D POP ILLUSION (2D cel only): soft highlight arc on top-left (~10–2 o'clock), slightly darker red band on bottom-right edge, tiny inner rim shadow where disk meets outline — like a chunky candy button, still 2D cartoon.

GLYPH: bold white "X" (two thick rounded strokes), centered, ~45% of button diameter, white #FFFFFF with thin dark brown outline on the strokes so it stays readable on red. Strokes have rounded caps (friendly, not sharp military X).

NO text other than the X shape. NO wood grain on the red disk. NO drop shadow on the ground.

Export: one PNG, subject centered.
```

Unity : **Image Type Simple**, **Preserve Aspect**, ~72×72 sur `CloseButton` (TestUi / prefab build).

#### Prompt ChatGPT — H-farm-4 Piste progression stade (coller tel quel)

**Dans ChatGPT :** joindre `mockup_popup_recolte_roquette_20261005.jpg` (zone barre sous le nom / stades). Fichier : `Assets/Art/Assets Store Dump/Ui/Farm/UiKit_Bar_progressTrack_9s_20261005.png`.

```
STYLE LOCK — RaymanFarm UI (strict):
- 2D casual mobile game UI, cozy aquaponic farm, FUN cartoon (chunky, playful).
- Rustic light brown WOOD; thick dark brown OUTLINES; smooth cel shading.
- NOT photoreal, NOT 3D. Flat front view. NO text, NO numbers, NO icons.
- 9-SLICE FRIENDLY: straight horizontal top/bottom wood beams; left/right caps; center repeatable.

One UI asset only. Match the harvest popup mockup progress row.

SUBJECT: horizontal PROGRESS BAR TRACK (empty groove only — no fill color inside). Landscape about 1024x160 pixels (~6:1). Outer frame: carved wood border with small corner nails, same family as popup buttons. INNER: a recessed horizontal channel (darker tan shadow) showing where fill will go — the channel interior must be EMPTY and UNIFORM (flat mid-brown/tan, no teal, no gradient stripes, no tick marks). The channel is one continuous capsule-shaped gutter centered vertically. NO stage labels (F1 F2…) — Unity uses TMP separately.

BACKGROUND: flat solid #FF00FF only. #FF00FF must not appear inside the artwork.

EDGE LOCK: thick pure black outer silhouette; no gray fringe.

Output: single PNG.
```

**Unity :** piste = `Image` **Sliced** · enfant `ProgressFill` = `UiKit_Bar_fill_teal` (Dump → Sprites) · **Filled** horizontal · `HarvestPanelUI.stageProgressFill.fillAmount`.

#### Prompt ChatGPT — H-farm-2 Fond barre batch 9-slice sans icône (coller tel quel)

**Dans ChatGPT :** joindre **H-farm-1** (`UiKit_BtnBatchHarvest_bar_20261005.png` ou export chat) + mockup popup. Fichier : `Assets/Art/Assets Store Dump/Ui/Farm/UiKit_Bar_batchHarvest_9s_20261005.png`.

```
STYLE LOCK — RaymanFarm UI (strict):
- 2D casual mobile game UI, cozy aquaponic farm, FUN cartoon (chunky, playful, not corporate flat).
- Rustic LIGHT BROWN WOOD panels: visible grain, carved edges, small nail heads in corners, optional tiny leaf motif (subtle).
- Thick dark brown OUTLINES; smooth cel shading; warm gold/orange wood highlights (NOT solid teal).
- NOT photoreal, NOT 3D render. Flat orthographic front view only.
- NO text, NO letters, NO numbers, NO characters, NO hands, NO plants, NO icons baked in.
- 9-SLICE FRIENDLY: straight border beams ~18% width/height; center fill MUST be plain flat parchment only (no leaves, no gradient noise).

One UI asset only. Use the attached batch bar ONLY as style reference for wood frame, rivets, rope inner border, and parchment color — but REMOVE the glove and seedlings completely.

SUBJECT: same wide horizontal bar as the reference, landscape 1280x320 (4:1). Identical outer wood frame and corner rivets. Inner area: empty light tan parchment. The left 25% is EMPTY parchment (reserved for a separate Unity icon overlay) — same color as center, no drawing. Right 75% also empty parchment. No decorative leaf shapes anywhere (they break 9-slice repeat).

BACKGROUND: flat solid #FF00FF only. #FF00FF must not appear inside the artwork.

EDGE LOCK: thick pure black outer outline; no gray fringe.

Output: single PNG.
```

#### Prompt ChatGPT — H-farm-1 Barre récolte batch (coller tel quel)

**Dans ChatGPT :** joindre `Assets/Art/Assets Store Dump/Ui/mockup_popup_recolte_roquette_20261005.jpg` (+ optionnel `UiKit_BtnSecondary_oval_20260919.png`). Enregistrer brut : `Assets/Art/Assets Store Dump/Ui/Farm/UiKit_BtnBatchHarvest_bar_20261005.png`.

```
STYLE LOCK — RaymanFarm UI (strict):
- 2D casual mobile game UI, cozy aquaponic farm, FUN cartoon (chunky, playful, not corporate flat).
- Rustic LIGHT BROWN WOOD panels: visible grain, carved edges, small nail heads in corners, optional tiny leaf motif (subtle).
- Thick dark brown OUTLINES on interactive elements; smooth cel shading; warm saturated accents (teal, orange, gold).
- NOT photoreal, NOT 3D render, NOT glassmorphism, NOT minimal flat Material.
- NOT isometric. Flat UI orthographic front view only.
- NO text, NO letters, NO numbers baked in — Unity uses TMP.
- 9-SLICE FRIENDLY: straight vertical/horizontal border zones (~18% of width/height), center strip plain enough for Unity Sliced Image.

One UI asset only. Match the attached harvest popup mockup wood style.

SUBJECT: a wide horizontal BATCH HARVEST action bar (full-width strip button), landscape about 1280x320 pixels (4:1). Light tan parchment or warm wood center, thick rustic wood border top and bottom, rope or stitched edge detail like the mockup secondary buttons. In the center-left area (about 25% width): a fun cartoon icon — yellow-orange work glove OR small sweeping harvest motion over 2–3 tiny plants (readable at 64px, no gore). Rest of center empty for TMP label later. Slight gold or soft orange highlight on the bar (NOT the same solid teal as primary Récolter). Small rivets in corners.

BACKGROUND: flat solid #FF00FF only (no transparency, no gradient background, no shadow on floor). #FF00FF must not appear inside the artwork.

EDGE LOCK: thick pure black outer outline on the UI shape silhouette; no gray fringe, no outer glow.

Output: single PNG, one asset centered with padding inside the canvas.
```

**Post-génération :** détourage auteur (fond magenta) → Sprite Editor : bordures 9-slice avec **icône gant dans le cap gauche** (~28–32 % largeur), zone centrale parchemin **sans** l’icône dans le stretch. PPU multiplier au tuning comme les autres boutons kit.

#### Prompt ChatGPT — H1 Onglet Multiverse (coller tel quel)

**Dans ChatGPT :** joindre en référence `Assets/Art/Assets Store Dump/poulpe-lowpoly.png` (bleu ; variante violette : `poulpe-lowpoly-violet.png` si tu veux coller au billet). Puis coller :

```
A 2D casual mobile game icon of a multiverse hub emblem, cartoon style, vibrant colors, isolated on a white background. Made with thick rustic light brown wooden textures, thick outlines, smooth shading, cozy farming game aesthetic, high quality UI asset.

Use the attached image as the EXACT character reference for the mini octopus. Copy its low-poly faceted body, big round head, short stubby tentacles, large glossy black eyes with white glints, and small round mouth with pink inner ring. Do not redesign the octopus. Do not turn the octopus into wood. Keep it recognizable as the same mascot.

COMPOSITION (strict):
- Square icon, white background only, no scenery, no text, no UI bars.
- CENTER: the mini octopus mascot.
- Around it: exactly THREE satellite objects.
- The octopus and the three satellites all occupy the SAME visual space (equal bounding-box size, equal visual weight). None is bigger than the others.
- Place the three satellites in a balanced triangle around the center (one top, one bottom-left, one bottom-right), with even gaps. Do not overlap the octopus.

THE THREE SATELLITES (same size as the octopus):
1) A rustic light-brown wooden basket containing lettuce, a carrot, and a trout (farm harvest, not a landscape).
2) A cute cartoon 2D mobile-shooter spaceship (the ship only, no stars field, no battle scene).
3) A cute cartoon infinite-runner character, simple bonhomme in a running pose, side view (the character only, no road, no city).

Keep the wooden rustic texture on the basket and as a cozy farm-game finish on the ship and runner, but the octopus stays low-poly like the reference. Thick outlines, smooth shading, high quality UI asset, readable at small mobile-tab size.
```

Sortie : `Dump/Ui/Nav/Icone_OngletMultiverse_YYYYMMDD.png`.

### Vague H-nav — variantes onglets barre `NavigationHUD` (128², lisibilité)

> **MAJ 2026-09-10 — direction mockup auteur :** icônes **glyphes simples** (pas diorama iso), remplissent la case ; **gris = tint Unity** ; **actif = zoom + lueur + label** (spec `Notes/Ui/SPEC_nav_onglets_zoom_actif.md`). Les prompts **H-nav mockup** ci-dessous **remplacent** le style « iso simplifié » pour la barre. Garder les anciennes refs uniquement pour l’**identité** du symbole (serre, sac, étal, billet, plus).

#### H-nav mockup 2026-09-10 — 5 glyphes barre (priorité art onglets)

**Réf visuelle :** mockups auteur → `Dump/Ui/Nav/ref_mockup_tabs_inactif.png` / `ref_mockup_tabs_actif_zoom.png`.

| Règle | Valeur |
|-------|--------|
| Export | **256×256** PNG, fond **transparent** (pas blanc opaque) |
| Sujet | **Un seul symbole**, silhouette **épaisse**, **~90 %** du cadre |
| Style | Casual mobile, bordures lisibles, couleurs **vives** (état actif) ; lisible à **72 px** |
| Inactif en jeu | Gris via Unity — **ne pas** générer une version grise séparée en V1 |
| Pas de texte | **Aucun mot sur le PNG** — pas « Inventaire », pas cadre or : labels + glow = **Unity** (TMP sous l’icône, `Glow` actif seulement) |

**Prompt commun (glyphe barre) :**

```
Mobile game bottom navigation GLYPH icon, single object only, casual fantasy stone-bar UI style like Clash-style menus but aquaponic farm theme. Bold thick silhouette, vibrant saturated colors, smooth cartoon shading, thick outline. Square canvas 256x256, subject fills 90% of frame, transparent background, NO text, NO frame, NO scene, NO isometric diorama — one readable symbol only.
```

| # | Fichier Dump | Symbole |
|---|--------------|---------|
| H-nav-1 | `IconeTab_Aventures_glyph.png` | Small greenhouse dome OR play/run hub emblem (simple) |
| H-nav-2 | `IconeTab_Inventaire_glyph.png` | Sac à dos + testeur pH (LCD) + fanes carotte (mockup) |
| H-nav-3 | `IconeTab_Shop_glyph.png` | Market stall awning, minimal |
| H-nav-4 | `IconeTab_Vente_glyph.png` | Coin stack or single banknote square, octopus+cabbage motif tiny |
| H-nav-5 | `IconeTab_Plus_glyph.png` | 2x2 grid of rounded squares → `TabMoreOption/Icon` |

Promo : `Assets/Art/Sprites/UI/Nav/Tabs/` — PPU **100**, Trim, assign on `Tab*/Icon`.

#### H-nav-2 — Inventaire (mockup glyphe sac à dos + tech + loot)

**Thème auteur :** inventaire = **RPG loot** (récolte) + **tech aquaponie** (mesure pH / EC — ici testeur pH lisible).

**Dans ChatGPT / générateur :** joindre en option `Assets/Art/Assets Store Dump/Ui/Nav/ref_mockup_tabs_actif_zoom.png` (silhouette sac actif). Puis coller :

```
Mobile game bottom navigation GLYPH icon, casual fantasy stone-bar UI style like Clash-style menus but aquaponic farm theme. Bold thick silhouette, vibrant saturated colors, smooth cartoon shading, thick outline. Square canvas 256x256, composition fills 90% of frame, transparent background, NO text, NO outer frame, NO landscape scene, NO isometric diorama — one centered icon composition only.

CORE: a chunky cartoon adventure BACKPACK, front or slight 3/4, two thick shoulder straps, main body centered, cozy farm-game colors (teal pack, orange/brown leather accents). NOT a wooden crate.

TWO SIDE POCKETS (strict, readable at 72px):
1) LEFT pocket (viewer left): a handheld pH meter probe sticking OUT of the pocket — chunky cartoon style, yellow or gray body, clear LCD screen showing a simple numeric readout (e.g. "7.0" or pH bars), NO tiny unreadable digits.
2) RIGHT pocket (viewer right): LARGE wilted carrot tops / feathery carrot leaves (🥕 greens) sticking OUT of the pocket — noticeably big, about half the visual height of the backpack flap area, slightly droopy "inventory loot" look, bright orange carrot tips peeking if needed.

Keep all elements attached to the backpack (nothing floating detached). Same visual weight as sibling nav glyphs. Inactive gray tint applied in Unity — export FULL COLOR only, one PNG.
```

**Sortie Dump :** `Assets/Art/Assets Store Dump/Ui/Nav/Tabs/IconeTab_Inventaire_glyph.png`  
**Promo après OK auteur :** `Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Inventaire_glyph.png` → sprite sur `TabInventaire/Icon` (`NavigationHUD.unity`).

#### H-nav-2 — passes de correction (mix **tech + oldschool**)

**Workflow ChatGPT / générateur :**  
1. Joindre **ton PNG actuel** (résultat à corriger).  
2. Joindre **`Assets/Art/Assets Store Dump/Ui/IconeInventaire.png`** (identité oldschool : bois clair, logo poisson+feuilles, testeur pH LCD « pH 7.2 », outils, truite, carotte/laitue, shading cartoon épais).  
3. Option barre nav : **`Dump/Ui/Nav/ref_mockup_tabs_actif_zoom.png`** ou **`Dump/ElementsReflexionsImages/MockupOngletInventaireBas_1.jpg`** (lisibilité glyphe).  
4. Une passe à la fois ; **Save** entre chaque.

**Passe A — ancrer le rendu oldschool (sans changer la composition si elle est déjà bonne)**

```
Edit the ATTACHED current inventory tab icon. Use the SECOND attached image (wooden crate inventory icon) ONLY as style reference — do NOT turn the result back into a crate.

KEEP from your current image: backpack layout, side pockets, pH probe position, large carrot tops, overall silhouette.

RESTYLE to match the reference crate icon's OLD-SCHOOL game art:
- Same cozy cartoon farming RPG look: soft gradients, thick dark outlines, slightly glossy highlights like hand-painted mobile UI.
- Warm light wood / leather accents on the backpack (similar warmth to the reference wooden crate planks).
- Small carved or stamped emblem on the backpack flap: fish silhouette + two leaves like the reference crate logo (simple, bold, not tiny).

Do NOT add hammer, screwdriver, trout, or lettuce unless already in your image — focus on backpack + pH + carrot greens only.

Output: square 256x256, TRANSPARENT background, full color, readable at 72px on a dark stone nav bar.
```

**Passe B — tech aquaponie (pH / EC lisible, même famille que la ref)**

```
Edit the ATTACHED inventory tab icon (latest version). Style reference: attached IconeInventaire.png pH meter ONLY.

UPGRADE the pH tester sticking out of the left pocket:
- Match the reference meter: white and blue plastic body, chunky probe, glowing blue LCD.
- Screen must show clear readable text: "pH 7.2" OR "7.2" with a tiny water droplet icon — NO blurry micro-digits.
- Optional subtle second line "EC" or a small bar graph under pH if it stays readable at 72px (aquaponic tech + RPG inventory vibe).

Keep backpack and LARGE wilted carrot tops in the right pocket unchanged in position. Thick outlines, old-school shading from pass A. Transparent background, 256x256.
```

**Passe C — fanes carotte « loot RPG » (taille + oldschool)**

```
Edit the ATTACHED inventory tab icon. Reference: IconeInventaire.png carrot and MockupOngletInventaireBas_1 inventory slot.

RIGHT pocket: enlarge feathery carrot tops so they are BIG — about 40–50% of the icon height, drooping out of the pocket like harvested loot (slightly wilted but still vivid green and orange tips).

Paint them with the same bold cartoon style as the reference carrot (thick leaves, saturated color, not flat vector).

LEFT pocket pH meter unchanged. Backpack emblem fish+leaves visible. Transparent 256x256, no text labels, no scene.
```

**Passe D — nettoyage glyphe barre (si trop détaillé / trop iso)**

```
Edit the ATTACHED inventory tab icon. Target: bottom navigation GLYPH like Clash-style stone bar tabs — NOT a full isometric diorama.

Simplify: remove extra props, depth, and ground shadow. Front-facing backpack, bold silhouette, fewer inner lines.
Keep ONLY: backpack + pH LCD meter (left pocket) + large carrot fronds (right pocket) + fish/leaves emblem on flap.
Same old-school painted look and colors as IconeInventaire reference. 90% frame fill, transparent background, 256x256.
```

**Si la génération est encore une caisse :** remplacer la passe A par :

```
Replace the wooden crate with an adventure BACKPACK while keeping the EXACT visual identity of the attached IconeInventaire.png (colors, outlines, pH meter design, carrot quality, fish+leaves logo on the flap). pH meter out of LEFT pocket, LARGE carrot tops out of RIGHT pocket. Transparent 256x256 nav glyph.
```

---

### Vague H-nav (archive 2026-09-09) — iso simplifié

**Contexte :** les PNG actuels (`IconePlay`, `IconeInventaire`, `IconeMarket`, `GoldBill`) sont des **visuels détaillés** (~1000+ px). **Remplacés** par glyphes mockup ci-dessus pour la barre.

**Ce n’est pas le prompt §1 (bois rustique).** Ancienne piste : cartoon iso / diorama simplifié — **ne plus utiliser pour la nav**.

| Règle | Valeur |
|-------|--------|
| Canvas export | **128×128 px** carré (ou 192×192 si tu préfères, puis downscale) |
| Sujet dans le cadre | **~85–90 %** de la hauteur/largeur utile, centré |
| Fond | **Blanc** (détourage import Unity) |
| Lisibilité | Formes **grosses**, peu de micro-détail, lisible à **~80 px** à l’écran |
| Poids visuel | Les **4** icônes doivent avoir la **même taille perçue** (pas un billet minuscule à côté d’une serre géante) |
| Ratio | Composition **carrée** — surtout **Vente** (billet en diagonale ou pile compacte, pas bandeau paysage) |
| Wiring | Après promo → remplacer le sprite sur `Tab*/Icon` dans `NavigationHUD.unity` (Bezy ou auteur) |

**Réfs visuelles (joindre à ChatGPT / générateur) :**

| Onglet | Réf actuelle (ne pas recopier 1:1 — simplifier) |
|--------|--------------------------------------------------|
| Aventures | `Assets/Art/Sprites/UI/Inventory/IconePlay.png` |
| Inventaire | `Assets/Art/Sprites/UI/Inventory/IconeInventaire.png` |
| Shop | `Assets/Art/Sprites/UI/Inventory/IconeMarket.png` |
| Vente | `Assets/Art/Sprites/UI/Currency/GoldBill.png` ou Dump `Ui/billet-poulpe-lowpoly.png` |

**Import Unity (après promo) :** Sprite Mode Single · **Trim** transparent · PPU **100** · `Alpha Is Transparency` ON.

| # | Onglet | Fichier Dump | Promo Sprites | Statut | Remplace sur |
|---|--------|--------------|---------------|--------|--------------|
| H-nav-1 | Aventures (Play / hub) | `Dump/Ui/Nav/Tabs/IconeTab_Aventures_128.png` | `Sprites/UI/Nav/Tabs/` | à générer | `TabAventures/Icon` |
| H-nav-2 | Inventaire | `Dump/Ui/Nav/Tabs/IconeTab_Inventaire_128.png` | idem | à générer | `TabInventaire/Icon` |
| H-nav-3 | Shop | `Dump/Ui/Nav/Tabs/IconeTab_Shop_128.png` | idem | à générer | `TabShop/Icon` |
| H-nav-4 | Vente | `Dump/Ui/Nav/Tabs/IconeTab_Vente_128.png` | idem | à générer | `TabVente/Icon` |
| H-nav-5 | **Plus** (hub features) | `Dump/Ui/Nav/Tabs/IconeTab_Plus_128.png` | idem | à générer | `TabPlus/Icon` (futur 5ᵉ onglet) |

**Cible nav (2026-09-09) :** barre **5 onglets** fixes (4 cœur + Plus). Quêtes, Atelier, Mail, Social, DIY = **lignes** dans l’écran Plus (`[BL-UI-FEATURES-HUB-001]`), pas des onglets supplémentaires.

**Prompt commun (préfixe — coller avant chaque variante) :**

```
Mobile game NAVIGATION TAB icon, casual cartoon style, vibrant colors, thick outlines, smooth shading, cozy aquaponic farm game. Square composition 128x128 pixels, subject centered, fills 85-90% of the frame, minimal empty margins, NO text, NO UI chrome, white background only. Simplify shapes for readability at 80px on screen — same visual weight as sibling tab icons. Do not add wooden rustic texture unless it is already on the reference object.
```

#### H-nav-1 — Aventures (serre / hub)

Joindre `IconePlay.png`. Puis :

```
[PREFIX H-nav commun ci-dessus]

Redraw a SIMPLIFIED version of the attached greenhouse geodesic dome icon: glass dome, plants inside, small blue base, tiny stairs. Keep the same identity and colors but fewer tiny details, bolder silhouette, centered, square crop, fills almost the entire 128x128 frame.
```

#### H-nav-2 — Inventaire (caisse)

Joindre `IconeInventaire.png`. Puis :

```
[PREFIX H-nav commun ci-dessus]

Redraw a SIMPLIFIED wooden crate icon like the reference: fish, carrot, lettuce, tools visible but chunky not tiny. Crate logo with fish+leaves on the side. Centered, square, 85-90% frame fill, readable at small size.
```

#### H-nav-3 — Shop (étal)

Joindre `IconeMarket.png`. Puis :

```
[PREFIX H-nav commun ci-dessus]

Redraw a SIMPLIFIED market stall icon like the reference: striped awning, shelves with seed packets and bottles, gold fish coin sign on top. Centered square composition, bold shapes, 85-90% frame fill, no wide empty sides.
```

#### H-nav-4 — Vente (billet)

Joindre `GoldBill.png` ou `billet-poulpe-lowpoly.png`. Puis :

```
[PREFIX H-nav commun ci-dessus]

Redraw a SIMPLIFIED banknote stack icon for a SALE tab: cute octopus face and cabbage head on the bill like the reference, held by a paper band. IMPORTANT: square composition — stack slightly rotated or compact pile so it fills 85-90% of a 128x128 frame (NOT a wide horizontal banner). Same visual size as the other three tab icons.
```

#### H-nav-5 — Plus (hub features)

Pas de réf obligatoire (ou joindre une icône « menu » existante du projet). Puis :

```
[PREFIX H-nav commun ci-dessus]

A simple "more features" navigation tab icon: a 2x2 grid of four rounded squares OR three horizontal dots inside a circle, cartoon style, bold thick outlines, same visual weight as the other tab icons. Centered, fills 85-90% of 128x128 frame. NO text.
```

**Après validation :** promo → Trim → remplacer sprite sur les 4 `Icon` (mini prompt Bezy ou auteur Inspector). Doc wiring : `Assets/Docs/Bezi/PROMPTS_Bezi_tab_sprites.md`.

**Nb d’onglets :** avec **4 onglets** la largeur cellule ≈ 25 % écran ; **5+ onglets** = icônes plus petites — prévoir regen ou barre plus haute (`NavBarContainer` 120→128 px).

### Vague W — monde (même liste, autre brief visuel)

Même fichier, **pas** le prompt icône §1 par défaut. Détail pose plante : `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md`. IBC : `Notes/Farm/CABLAGE_biofiltre_ibc_grille_bezi.md`.

| # | Objet FR | Prompt / brief | Dump | Promo Sprites | Statut | Fichier |
|---|----------|----------------|------|---------------|--------|---------|
| W1 | Cuve IBC dessus losange 2:1 | brief iso grille (pas icône) | `Dump/ElementProd/Biofiltre/` | `Sprites/Farm/Biofiltre/` | promu 2026-09-05 (2e passe 2:1) | `IbcIso.png` |
| W2 | Stades laitue monde (regen iso) | `Notes/Art/PROMPT_laitue_sprite_sheet_croissance.md` (charte `NOTE_graphique.md`) | `Dump/Plantes/Laitue/` | `Sprites/Plantes/Laitue/` | à générer | |
| W3 | Stades tomate monde (iso 3/4) | même charte que W2 — 7 stades grille | `Dump/Plantes/Tomate/` | `Sprites/Plantes/Tomate/` | à générer | |
| W4 | Illustrations bandeaux vente | `PROMPT_bandeaux_vente_generique.md` | `Dump/Ui/` | `Sprites/UI/SaleChannels/` | **Voisinage promu** 2026-09-21 ; bandoulière / vélo à générer | `BandeauVente_Voisinage.png` |
| HUB | Bandeau + fanion hub Plus | `Notes/Art/PROMPT_features_hub_bandeau_flag.md` | `Dump/Ui/FeaturesHub/` | `Sprites/UI/FeaturesHub/` | à générer | B1 chrome 9s + F0 neutre puis F1–F6 |
| W5 | Fishtank / poissons jouables | ~lvl 10 | `Dump/Poisson/` | `Sprites/Farm/Poisson/` | à générer | |

Ajouter ici toute ligne monde (nouveau stade plante, bac, insecte, VFX still, etc.).

### W1 — recale dessus (coller avec `IbcIso.png` en référence)

**Élément à modifier :** le **losange du dessus** seulement = cadre bois intérieur + billes d’argile.  
**Ne pas** redessiner : cuve blanche, cage, palette, pieds, robinet, barres **verticales**.

Grille jeu = iso **2:1** (pas l’iso 30° illustration). Angle des arêtes = **26,565°** depuis l’horizontale (`atan(0.5)`), **pas 30°**.

```
Edit this existing IBC tank illustration. Keep the white tank, metal cage, wooden pallet, black feet, and red tap unchanged. Vertical cage bars stay perfectly vertical.

ONLY redraw the TOP PLANTER: the wooden rim and the clay-pebble fill.

The inner opening of the wooden rim (the plantable deck) MUST be a perfect 2:1 isometric diamond:
- width = 2 × height
- vertices exactly North, East, South, West (same X for North and South, same Y for East and West)
- the four rim edges parallel to a 2:1 game isometric grid (edge angle 26.565° from horizontal, NOT 30° true isometric)
- North vertex at the top midpoint, South at the bottom midpoint of the deck, East/West at the left and right midpoints
- clay pebbles fill that diamond only, flush to the inner wooden edge, no skew, no rotation
- wooden rim follows the same diamond; do not rotate the top relative to the tank body

Transparent or white background, same cartoon farm style, thick outlines. Output a single PNG of the full IBC, same framing as the reference.
```

---

## 4) Prochaines 7 lignes (copie rapide)

Ordre immédiat :

0. **H-nav-1 → H-nav-4** — variantes onglets `NavigationHUD` 128² (après playtest layout Bezy)  
1. A1 — graines laitue  
2. A2 — laitue récoltée  
3. A3 — graines tomate  
4. A4 — tomate récoltée  
5. B1 — halo Commerce  
6. C1 — graines anti-limaces  
7. E6 — bac DWC particulier  

Quand une ligne passe en `en dump`, décaler celle d’après.
