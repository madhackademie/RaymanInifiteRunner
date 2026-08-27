# Spéc — progression : XP joueur + maturité / étoiles biofiltre

Références futures : unlocks plantes (`PlantDefinition` / tags), `BuildManager` / placement, persistance temps réel (`Timer`, UTC), instances de zones (biofiltre, lit de culture).

**Backlog :** `[BL-GDD-003]` (XP + étoiles + maturité) · prestige après la courbe ★ : `[BL-GDD-006]`.

---

## 1. XP joueur (à détailler — couche distincte)

- Progression **globale** du joueur (halo, niveaux, perks) : **pas** le compteur des étoiles biofiltre.
- Le **240 XP** du §3 est de l’**XP d’instance biofiltre** (cette cuve / ce système), pas l’XP halo.

---

## 2. Maturité du support (gating cultures avancées)

Objectif : les **plantes à fruits** (ou cultures avancées) ne sont plantables que sur un **système** assez mûr.

- Ancienne baseline (~10 cycles salade / ~1 an temps réel) : **repère historique**.
- Cible actuelle : accrocher le gating à une **étoile biofiltre** (ou à ses compteurs) plutôt que d’empiler un 3e axe parallèle aux ★ et aux nœuds du panneau. Palier exact (★1 vs ★2+) **TBD** une fois ★1 playtesté.

Placement (`CanPlace`) : **unlock joueur** (graine / recette) **et** **éligibilité du support** (étoile / maturité de **cette** instance).

---

## 3. Étoiles du biofiltre (première couche visible)

Même **grammaire** que les bandeaux de vente (`SPEC_vente_production_boucle_jeu.md` §2.9) : 1–5 ★ sur **l’instance**, conditions **cumulatives**, tooltip palier courant / suivant + jauges live.

| Couche | Rôle |
|--------|------|
| **Étoiles 0–5** | Upgrade **dans** une génération de la cuve (V0 de cette spec) |
| **Nœuds** panneau Biofiltre / Poisson / Techno | Choix fins, anti-aléas — `[BL-GDD-005]` ; ne pas dupliquer les bonus d’étoile |
| **Prestige** | Portes **★3** (1 slot secondaire) **ou** **★5** (1 slot primaire) — `SPEC_prestige_generation_systemes.md` + `SPEC_biofiltre_slots_shields.md` |

**Départ :** le biofiltre pose **0 étoile** (cuve neuve). La **première étoile (★1)** se **débloque** quand **toutes** les conditions du §3.2 sont réunies — contrairement au voisinage, où ★1 est l’état de départ du canal débloqué.

★2–5 : seuils **TBD** (même logique cumulative, chiffres après playtest ★1).

### 3.1 Cadence cible ★1 (temps réel joueur)

Ordre de grandeur auteur (2026-08-27) — **pas** un timer forcé dans le jeu :

- **3 à 5 jours** de jeu calendaire ;
- **2 à 3 sessions par jour** ;
- **5 à 7 minutes** par session (**voire moins**).

Fenêtre brute ~**30–105 min** de play étalé. Les 4 seuils du §3.2 doivent être **co-atteignables** dans cette fenêtre. Si le playtest sort trop vite ou trop long : **recaler les nombres**, pas raccourcir les stades plantes pour « coller » aux ★.

### 3.2 Conditions cumulatives ★1 (valeurs de travail)

Toutes **obligatoires** (ET, pas OU). Chiffres **à analyser, playtester et rééquilibrer**.

| # | Compteur (instance biofiltre) | Seuil ★1 | Note |
|---|-------------------------------|----------|------|
| 1 | **XP système** | **240** | Voir §3.3 (80 × ?). |
| 2 | **Récoltes salade** | **50** | Stade **Mature** laitue / leafy (`PlantDefinition` harvest salade). |
| 3 | **Germinations** | **100** | Réussies **ou** tentées — **ouvert**, voir §3.4. |
| 4 | **Graines récoltées** | **50** | Récolte au stade **Seedling** (item graine, ex. `laitue_seed`). |

**Règle récolte (une plante = un choix) :** salade **ou** graines, **pas les deux**. Aligné runtime : `PlantDefinition.maxHarvestCount = 1` — récolter à **Mature** (salade) **ou** laisser aller jusqu’à **Seedling** (graines). Pas deux récoltes d’affilée sur la même instance.

Conséquence pour ★1 : **50 + 50 = 100 plantes menées à récolte**, pas 50 cycles « complets ».

| Compteur | 50 + 50 récoltes XOR | 100 germinations |
|----------|----------------------|------------------|
| Lecture | Le joueur doit **splitter** : assez de coupes feuille **et** assez de montées à graines. Uniquement farm salade (ou uniquement graines) **ne suffit pas**. | **~1 germination par plante récoltée** si tout ce qui germe est récolté (l’un ou l’autre). |

Marge playtest : arrachage, germinations ratées, plantes non récoltées → germinations peuvent dépasser les récoltes utiles. Si le palier lit les **tentées**, le 100 est un volume de poses ; les 50/50 restent le split de destination.

**Unité des compteurs 2 et 4 (à figer en playtest) :**

- Reco de travail : **événements** (1 validation de récolte = +1), pour que les bonus de **quantité** (prestige G2, nœuds) n’accélèrent pas les ★.
- Alternative alignée vente : **unités d’items** (50 salades = 50 items). Plus sensible aux `harvestAmount` min/max.

### 3.3 XP 240 et « 80 × 4 »

Cible retenue pour ★1 : **240 XP système**.

Note auteur : « 240 (80 × 4) ». **80 × 4 = 320**, pas 240.

| Lecture | Seuil | Cadence |
|---------|-------|---------|
| **A (retenue tant que non infirmée)** | **240** = **80 × 3** | ~**80 XP / jour** × **3 jours** |
| B | **320** = **80 × 4** | ~80 XP / jour × **4 jours** (toujours dans 3–5 j) |

**80** = ordre de grandeur d’XP système pour **une journée cible** (2–3 mini-sessions), **pas** encore une formule runtime. Sources d’XP (récolte, plantation, temps réel, quêtes) : **TBD** — à caler pour que 240 arrive **avec** les trois autres jauges, pas 2 jours avant.

Ne pas fusionner cet XP avec l’XP **joueur** halo.

### 3.4 Germinations : réussies ou tentées ?

**Ouvert.** Les deux n’existent pas encore comme aléas (aujourd’hui poser une graine ≈ germer). Quand l’aléa « faible germination » arrivera, le choix change le feeling.

| Option | Compte | Effet joueur |
|--------|--------|----------------|
| **Tentées** (reco V0) | Pose / entrée stade `Graine` | Volume d’usage du système ; le RNG futur ne bloque pas ★1 |
| **Réussies** | Passage `Graine` → `Starting` (ou équivalent) | L’étoile mesure un biofiltre qui « prend » ; puni si l’aléa est dur |

**Reco d’implémentation :** sauver **les deux** compteurs (`germinationAttempts`, `germinationSuccesses`) ; le palier ★1 lit **un** des deux (défaut proposé : **tentées**). UI : libellé aligné (« Graines plantées » vs « Germinations réussies ») pour ne pas mentir.

### 3.5 UI (intention)

- Rangée d’étoiles sur le biofiltre / panneau système (pas sur les bandeaux vente).
- Tooltip + jauges live des 4 conditions vers la ★ suivante — même esprit que `SaleChannelStarTooltip` (4 barres au lieu de 3).
- Prefab / wiring : **Bezy** plus tard ; logique / save : **Cursor**. Pipeline popup si confirmation de palier : `ScreenPopupHost`.

---

## 4. Pistes d’implémentation (plus tard)

Save **par instance** (`systemInstanceId`, ex. `firstlvl.biofiltre.main`) :

- `biofiltreStarTier` (0–5)
- `systemXp`
- `saladHarvestCount` / `seedHarvestCount`
- `germinationAttempts` / `germinationSuccesses`
- `prestigeGeneration` (après courbe ★)

Aligné `FarmPersistenceCoordinator` / JSON existant. Pas de magic numbers en code : constantes designer (`BiofiltreStar1SystemXp = 240`, etc.).

---

## 5. Liens avec d’autres notes

- Pipeline plantation : `Notes/Farm/TODO_plantation_pipeline.md`
- Temps ferme / reprise : spec temps de ferme (`Notes/Todo_project.md` → GDD)
- Panneau onglets Biofiltre / Poisson / Techno : `Notes/GDD/SPEC_progression_systeme_aquaponique_par_niveau.md`
- Prestige / génération (après ★) : `Notes/GDD/SPEC_prestige_generation_systemes.md`
- Étoiles **vente** (autre instance) : `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §2.9
