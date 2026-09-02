# Backlog art — création (Dump) → validation → Sprites

**Création :** 2026-08-31 · **MAJ :** 2026-09-02  
**Source unique** de tout l’art à produire (icônes UI, stades monde, IBC, fishtank, bandeaux…).  
**Usage :** 1 asset / jour (~15 min) — générer → Dump → **après OK auteur** copier dans le dossier `Sprites/` de la ligne.

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
| `…/Dump/Ui/` (nav générique) | `Assets/Art/Sprites/UI/` |
| `…/Dump/Plantes/<Nom>/` | `Assets/Art/Sprites/Plantes/<Nom>/` |
| `…/Dump/ElementProd/Biofiltre/` | `Assets/Art/Sprites/Farm/Biofiltre/` |
| `…/Dump/Poisson/` | `Assets/Art/Sprites/Farm/Poisson/` |

Le **§1** est le prompt **icône UI** (fond blanc). Les lignes monde (vague W) peuvent utiliser un autre prompt : le noter dans la colonne Prompt.

---

## 1) Prompt générique (copier-coller)

Remplacer uniquement `[VOTRE OBJET ICI]` :

```
A 2D casual mobile game icon of [VOTRE OBJET ICI], cartoon style, vibrant colors, isolated on a white background. Made with thick rustic light brown wooden textures, thick outlines, smooth shading, cozy farming game aesthetic, high quality UI asset.
```

**Exemple :** `A 2D casual mobile game icon of a packet of lettuce seeds, cartoon style, vibrant colors, isolated on a white background. Made with thick rustic light brown wooden textures, thick outlines, smooth shading, cozy farming game aesthetic, high quality UI asset.`

### Consignes de remplissage

- Objet **un seul**, lisible à **64–96 px** (inventaire mobile).
- Nommer le fichier : `Icone_[Objet]_[YYYYMMDD].png` (ex. `Icone_GrainesAntiSlug_20260831.png`).
- Déposer dans le sous-dossier Dump de la famille (tableau §3).
- Fond blanc isolé = prévu : on détourera / importera en sprite plus tard. Ne pas coller le PNG brut sur un prefab.

### Variantes (optionnel, même style)

| Besoin | Ajouter à la fin du prompt |
|--------|----------------------------|
| Paquet / sachet | `packaged as a small seed packet with a simple label` |
| Consommable (dose) | `shown as a small pouch or bottle, not a landscape scene` |
| Structure (serre, bac) | `isometric 3/4 view of the object only, no environment` |
| Portrait canal vente | **ne pas** utiliser ce prompt — ce sont des illustrations bandeau, pas des icônes |

---

## 2) Routine quotidienne

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

### Vague W — monde (même liste, autre brief visuel)

Même fichier, **pas** le prompt icône §1 par défaut. Détail pose plante : `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md`. IBC : `Notes/Farm/CABLAGE_biofiltre_ibc_grille_bezi.md`.

| # | Objet FR | Prompt / brief | Dump | Promo Sprites | Statut | Fichier |
|---|----------|----------------|------|---------------|--------|---------|
| W1 | Cuve IBC dessus losange 2:1 | brief iso grille (pas icône) | `Dump/ElementProd/Biofiltre/` | `Sprites/Farm/Biofiltre/` | à générer | |
| W2 | Stades laitue monde (fix alpha) | import / détourage — `[P0-FARM-SPRITE-ALPHA-001]` | `Dump/Plantes/Laitue/` | `Sprites/Plantes/Laitue/` | à générer | |
| W3 | Stades tomate monde (canopée) | 7 stades grille | `Dump/Plantes/Tomate/` | `Sprites/Plantes/Tomate/` | à générer | |
| W4 | Illustrations bandeaux vente | scène, pas icon | `Dump/Ui/` | `Sprites/UI/` | à générer | |
| W5 | Fishtank / poissons jouables | ~lvl 10 | `Dump/Poisson/` | `Sprites/Farm/Poisson/` | à générer | |

Ajouter ici toute ligne monde (nouveau stade plante, bac, insecte, VFX still, etc.).

---

## 4) Prochaines 7 lignes (copie rapide)

Ordre immédiat :

1. A1 — graines laitue  
2. A2 — laitue récoltée  
3. A3 — graines tomate  
4. A4 — tomate récoltée  
5. B1 — halo Commerce  
6. C1 — graines anti-limaces  
7. E6 — bac DWC particulier  

Quand une ligne passe en `en dump`, décaler celle d’après.
