# Génération d’icônes UI — prompt générique + file quotidienne

**Création :** 2026-08-31  
**Usage :** 1 icône / jour (~15 min) — coller le prompt, déposer le PNG dans le Dump, ne pas promouvoir tout seul.

> Ce prompt sert les **icônes UI** (inventaire, shop, slots, halo, craft).  
> Il ne sert **pas** les sprites monde (stades plante sur la grille, cuve IBC, fishtank). Pipeline monde = autre brief.

Dump (brut) : `Assets/Art/Assets Store Dump/` — sous-dossier thématique, jamais la racine.  
Promo runtime : `Assets/Art/Sprites/` **uniquement après OK auteur**. Règle : `.cursor/rules/art_asset_dump.mdc`.

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
5. Promo `Sprites/` = session auteur dédiée, pas le soir même « parce que c’est joli ».

Hors file : si une session code a besoin d’une icône **tout de suite**, la faire ce jour-là et **remonter** la ligne en tête.

---

## 3) File des icônes (ordre quotidien)

Statut dans **cette** note seulement (`à générer` / `en dump` / `promu`). Les tâches projet restent dans `Notes/Todo_project.md`.

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
| G1 | Boulon (monnaie quête) | `a large rustic metal bolt token` | `Dump/Ui/Currency/` | à générer | |
| G2 | Paquet graines commun (daily) | `a common brown seed bundle tied with twine` | `Dump/Ui/Quests/` | à générer | |
| G3 | Paquet graines uncommon (hebdo) | `a uncommon green-ribbon seed bundle` | `Dump/Ui/Quests/` | à générer | |
| G4 | Paquet graines rare (mensuel) | `a rare gold-ribbon seed bundle` | `Dump/Ui/Quests/` | à générer | |
| G5 | Icône Atelier (nav) | `a wooden workbench with a hammer` | `Dump/Ui/` | à générer | |
| G6 | Icône Quêtes (nav) | `a wooden clipboard with a checkmark` | `Dump/Ui/` | à générer | |

Déjà en Dump (ne pas regénérer tant que non validé) : `IconeMarket.png`, `IconePlay.png`, `IconeInventaire.png`.

---

## 4) Hors scope de ce prompt

| Asset | Pourquoi | Où |
|-------|----------|----|
| Stades plante monde (graine → seedling) | Vue grille / iso, pas icône isolée | Autre brief ; protocole `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md` |
| Cuve IBC / deck iso 2:1 | Contrainte grille, pas icône | `Notes/Farm/CABLAGE_biofiltre_ibc_grille_bezi.md` |
| Illustrations bandeaux vente | Scène, pas icon | Concepts Dump `Ui/ConceptVeloMarchand*.png` etc. |
| Poissons / fishtank jouables | Cible ~lvl 10 | Plus tard, brief dédié |

---

## 5) Prochaines 7 icônes (copie rapide)

Ordre immédiat :

1. A1 — graines laitue  
2. A2 — laitue récoltée  
3. A3 — graines tomate  
4. A4 — tomate récoltée  
5. B1 — halo Commerce  
6. C1 — graines anti-limaces  
7. E6 — bac DWC particulier  

Quand une ligne passe en `en dump`, décaler celle d’après.
