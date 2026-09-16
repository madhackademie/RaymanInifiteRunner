# NOTE — MVP hook, palier commercial, équilibrage plantes

**Date :** 2026-09-16  
**Statut :** analyse Cursor (critique + proposition).  
**Chiffres timers / économie plantes :** tableur `Notes/GDD/data/GDD_balance.xlsx` — **pas** cette note, **pas** `Laitue.asset` (debug).  
**Source brute :** `Notes/GDD/analyseMVP_Hoock_equilibrage.txt` (inbox prompts — ne plus relancer tel quel).  
**Alimente :** `[BL-GDD-001]` GDD MVP · `[BL-GDD-010]` timers laitue · revue `[BL-GDD-009]`.

> Ce fichier **remplace les 3 mega-prompts** pour le travail courant.  
> Les prompts propres (rejouables / VM) sont en **§8**.  
> Le schéma data (ce qu’une IA « GameDesignData » doit remplir) est en **§6**.

---

## 0. Verdict sur les prompts bruts

Les idées sont bonnes. Le format actuel **ne peut pas** produire une analyse fiable.

| Problème | Effet |
|----------|--------|
| 3 prompts qui se recoupent (MVP + palier + data) | L’IA mélange hook, wishlist et table Excel |
| « Lvl 10 » sans dire **quel** niveau | Collision halo joueur / ★ biofiltre / palier plante / map |
| Wishlist (repro poissons, market multi, élec, quêtes boulons) dans le même jet que « features minimum » | Le MVP gonfle ; le hook se noie |
| 15 plantes posées comme prérequis du hook | Hay Day scale, pas un soft-launch aquaponie |
| Demande de relancer toutes les 2 semaines **et** de réécrire toutes les specs | Risque de dérive : on écrase des décisions déjà actées |

**Ce qui est déjà tranché ailleurs — ne pas re-décider ici :**

| Sujet | Source | Valeur actée / de travail |
|-------|--------|---------------------------|
| Cadence sessions | `SPEC_progression_xp_joueur_et_biofiltre.md` §3.1 | 3–5 j pour ★1 · **2–3 sessions/j** · **5–7 min** (prompt brut : 5–10 min — même famille) |
| ★1 biofiltre | même spec §3.2 | 240 XP + 50 salades + 100 germinations + 50 graines |
| Récolte XOR | même spec + `PlantDefinition.maxHarvestCount = 1` | Mature **ou** graines, pas les deux |
| Prestige | `SPEC_prestige_generation_systemes.md` | Par instance, portes ★3 **ou** ★5, pas wipe joueur |
| Vente V0 | `SPEC_vente_production_boucle_jeu.md` | Voisinage seul · 15 gold/salade · volume bas |
| Stades runtime | `PlantDefinition` / `PlantGrow` | 7 stades (pas 6) : Graine → Starting → Baby → Growing → Mature → Flowering → Seedling |
| Atelier / quêtes | `SPEC_craft_atelier_aquaponique.md` | Indispensable **plus tard**, pas le hook j1 |
| Poisson gameplay | specs système + vente ★5 | Après chiffre / après leafy |

**Runtime aujourd’hui :** 1 plante (`Laitue.asset`), durées **debug** (1 s partout sauf Flowering 3500 s). Aucun taux de germination. Pas de plafond offline (`[BL-GDD-002]`). Le hook n’est **pas** un problème de catalogue à 15 espèces.

---

## 1. Vocabulaire — arrêter « lvl 10 »

| Axe | Nom | Rôle | MVP commercial |
|-----|-----|------|----------------|
| **A** | Halo joueur (`ProgressionTrackId`) | Talents globaux | Invisible / 1 piste Commerce max |
| **B** | Étoiles biofiltre 0–5 | Upgrade **cette** cuve | ★1 visible = vrai palier semaine 1 |
| **C** | Génération prestige G0–G2 | Rétention **après** ★3 | Hors soft-launch |
| **D** | Palier plante **T1–T5** | Unlock catalogue | T1 = 3 leafy au launch |
| **E** | Map / système | 1 scène `FirstLvl` = 1 ensemble aquaponique | 1 map |

Le « palier lvl 10 qui débloque les poissons » du prompt brut se traduit ainsi :

- **J7–J10 calendaire** (pas joueur lvl 10) → fishtank **décoratif** possible.  
- Gameplay poisson (qualité eau, bonus/malus) → **après** T1–T2 leafy + vente qui tourne.  
- Aligné auteur : « poissons et biome uniquement si le jeu fait du chiffre ».

---

## 2. Cible rétention (inchangée, précisée)

Objectif produit (auteur + spec ★1) :

```
2–3 connexions / jour × 5–10 min  →  récupérer le farm + 1 objectif d’upgrade
```

Repères marché (idle, pas à copier) :

| Source | Chiffre | Lecture pour nous |
|--------|---------|-------------------|
| GameAnalytics idle | ~8 min / session, ~5 sessions/j | Notre cible est **plus cozy** (moins de check-ins). Choix volontaire. |
| AFK Arena / idle type | Coffre offline **cap ~12 h** → 2 visites/j naturelles | On a besoin d’un **cap offline** (`[BL-GDD-002]`), pas de 15 plantes. |
| Hay Day early | Blé 2 min, maïs 5, soja 20, puis saut 2 h+ | Le 2 min = « wheating » (joueur qui reste). **À éviter** comme unique crop : ça pousse à 5+ sessions. |

**Implication :** le hook = **quelque chose de prêt entre deux visites** + **un palier visible** (★1 ou recherche). Pas un simulateur élec/eau au dashboard.

---

## 3. Palier commercial recommandé (soft-launch)

Pas « contenu jusqu’au lvl 10 ». Palier = **la boucle tient 7–10 jours** sans nouveau système.

### 3.1 Must (hook)

| Feature | État repo | Pourquoi c’est le hook |
|---------|-----------|------------------------|
| Planter / récolter | Livré (laitue) | Action |
| Inventaire | Livré | Stock |
| Achat graines | Livré (shop) | Réinvestissement |
| Vente voisinage | Livré V0 (timer 24 h en cours) | Convertisseur or |
| Croissance offline + cap | **Manquant** `[BL-GDD-002]` | Raison de revenir |
| Feedback « prêt à récolter » | Chantier farm VFX | Dopamine au login |
| 1 objectif d’upgrade visible | ★1 biofiltre **spec only** | « Avancer » |

Sans offline cap + feedback prêt + palier visible, **ajouter des plantes ne crée pas de hook**.

### 3.2 Should (semaine 1–2, même map)

- **3 plantes T1** (laitue, basilic, blette) — mix visuel, pas 15.  
- ★1 biofiltre jouable (jauges, pas forcément ★2–5).  
- 1 recherche simple (débloque T2 **ou** bandoulière, pas les deux d’un coup).

### 3.3 Later / si chiffre

Prestige G1 · fishtank décoratif · nœuds panneau Biofiltre/Poisson/Techno · atelier · quêtes daily · canaux vente ★2 · fruiting (tomate) gated maturité.

### 3.4 Vote / hors MVP (garder en backlog, ne pas chiffrer maintenant)

Atelier DIY (wormbox, hotbin, serre…) · élec/chauffage autonome · quêtes daily/hebdo/mensuel + monnaie boulons · repro poissons 3★ · market multi · dashboard élec/eau/nourriture · achievements Steam.

Ces items ont **déjà** une spec ou un backlog (`[BL-GDD-005]` à `[BL-GDD-008]`). Ne pas les ré-inventer dans l’analyse MVP.

---

## 4. Catalogue plantes — 15 = vision map 1, pas le launch

Idée auteur **3 par palier** : conservée. Lecture corrigée : **3 au T1 launch**, 15 = plafond `FirstLvl` jusqu’à sabloponie / 2ᵉ système.

Gating fruits / racines : déjà dans spec maturité biofiltre + inbox (sabloponie, minéralisation). **Ne pas** débloquer tomate au T1 « pour le rendu ».

### 4.1 Mix proposé (leafy d’abord)

| Palier | Gate (travail) | 3 plantes | Rôle visuel / loop |
|--------|----------------|-----------|---------------------|
| **T1** | Jour 1 | Laitue · Basilic · Blette | Masse verte / vertical aromatique / tiges couleur |
| **T2** | ★1 **ou** ~j3–5 | Épinard · Roquette · Menthe | Densité / découpe feuille / accent |
| **T3** | ★2 ou recherche ~j7–10 | Pak choi · Cresson · Coriandre | Silhouette bac + lore aquaponie (cresson) |
| **T4** | Maturité biofiltre / minéralisation | Tomate cerise · Poivron · Concombre **ou** fraise | `PlantGrowthPattern.Fruiting` |
| **T5** | Sabloponie / 2ᵉ media | Radis · Carotte · Betterave | Racines — autre bac, pas le deck leafy |

**Launch art :** 1 SO existe (`lettuce`). Priorité prod = **2 spritesheets T1** (basilic, blette), pas 15.

### 4.2 Pourquoi planter « toutes » les plantes sans quêtes ?

Les quêtes ciblent la palette, OK. Sans quêtes, il faut **d’autres raisons orthogonales** (sinon le joueur spam laitue) :

1. **Rendu** — 3 silhouettes T1 déjà (auteur).  
2. **Durée** — une plante « entre deux visites » vs une « overnight ».  
3. **XOR économique** — une espèce plus rentable **feuille**, une plus rentable **graines**.  
4. **Canal vente** — ★2 voisinage débloque « autres légumes » (`SPEC_vente` §2.9) : planter T2 = ouvrir l’offre.  
5. **Recherche** — nœud « récolter 20 basilic » (volume, pas timer).  
6. **Footprint** — 2×2 laitue vs 1×1 menthe = puzzle grille (déjà dans `PlantDefinition.footprint`).  
7. **Collection deck** — pattern Pocket Plants / Plantera, secondaire.

Si 1, 2 et 3 sont vrais, le joueur mixe **sans** daily. Les quêtes accélèrent, elles ne portent pas le mix.

---

## 5. Modèle mathématique (temps & prix)

**Source des nombres :** `Notes/GDD/data/GDD_balance.xlsx` (miroir CSV dans le même dossier).  
Cette section = **règles de bande** seulement. Les minutes par stade se remplissent dans l’onglet `stades_timers` colonne `gddMinutes`. Unity n’est mis à jour que si `applyToUnity = oui` **et** demande auteur.

### 5.1 Formule temps jusqu’à Mature

Ne pas scaler linéairement « ×2 par palier » (T5 devient injouable) ni rester à 1 s (debug actuel).

**Proposition :** trois **bandes de session**, pas une expo aveugle.

| Bande | `timeToMature` cible | Usage |
|-------|----------------------|--------|
| **S** session | 8–20 min | 1 cycle possible dans une visite longue (tutoriel / dopamine). **Une seule** plante T1 (laitue). |
| **B** between | 3–5 h | Prêt à la 2ᵉ visite du jour. T1 basilic / blette + T2. |
| **N** night | 8–14 h | Planté le soir, prêt le matin. T3+ et **chemin graines** (Flowering → Seedling). |

Chemin graines (même instance, XOR) : `timeToSeed = timeToMature + tFlowering + tSeedling` ≈ bande **N** même si Mature était **B**. Ça donne une raison d’attendre sans 2ᵉ récolte.

Répartition stades (exemple laitue bande S+B hybride — **à playtester**) :

| Stade | % du cycle feuille | Ex. total Mature = 4 h |
|-------|--------------------|-------------------------|
| Graine | 2 % | ~5 min (on voit germer au login) |
| Starting | 5 % | ~12 min |
| Baby | 10 % | ~24 min |
| Growing | 83 % | ~3 h 20 |
| Mature | 0 (fenêtre récolte, pas timer forcé) | joueur coupe ou laisse |
| Flowering | +35 % du cycle feuille | ~1 h 20 si graines |
| Seedling | +15 % | ~35 min |

**Cap vitesse** (prestige + nœuds) : déjà prévu `MinPlantCycleSeconds` dans spec prestige §5. À chiffrer **après** G1 jouable, pas dans le MVP.

Germination / finalisation : **table commune** par qualité de graine (commun / unco / rare), **pas** par palier T1–T5. Les paliers changent **temps** et **prix**. Les RNG (fourmis, oiseaux) arrivent avec les shields `[BL-GDD-007]`. V0 : germination = 100 % (comme aujourd’hui : poser ≈ germer). Compteurs ★1 : sauver tentées **et** réussies (spec §3.4).

### 5.2 Prix (voisinage)

Ancre runtime : **15 gold / salade**. Volume canal volontairement bas.

Règle de travail :

```
seedBuyPrice × 3 ≤ sellLeafVoisinage × harvestAmountAvg
```

Marge haute, volume bas = spec vente. Si la marge tombe sous ×2, le joueur n’a plus de « réinvestir ». Si elle dépasse ×8, le shop devient cosmétique.

Graines récoltées : **moins** d’or/unité que la feuille, **plus** de qty (`laitue_seed` 5–15 vs feuille 1–3) → le split XOR reste un vrai choix (cash maintenant vs autonomie graines).

### 5.3 Ordres de grandeur (exemples — **non figés**)

> Ne pas copier ces lignes dans Unity. Caler dans le tableur, laitue d’abord.

| Palier | Bande | `timeToMature` | `sellLeaf` (or) | `seedBuy` | Notes |
|--------|-------|----------------|-----------------|-----------|--------|
| T1 laitue | S puis B | 20 min **premier** cycle tutoriel **ou** 4 h dès j1 — **trancher en playtest** | 15 (actuel) | 5 | Ancre économie |
| T1 basilic | B | 3 h 30 | 18 | 6 | Qty feuille plus faible, prix unitaire ↑ |
| T1 blette | B | 5 h | 22 | 7 | Visuel ; un peu plus lent |
| T2 | B / N | 6–8 h | 28–35 | 10–12 | Déblocage ★1 |
| T3 | N | 10–14 h | 40–50 | 14–16 | J7–10 |
| T4 fruiting | N+ | 18–30 h | 70–90 | 24–30 | Gate maturité |
| T5 racine | N+ | 24–36 h | 80–100 | 28–35 | Autre bac |

**T1 laitue 20 min vs 4 h :** 20 min = plus de sessions (Hay Day). 4 h = colle à 2–3 visites. Reco : **4 h** + stades Graine/Starting **courts visibles** au moment du plant (30–90 s) pour ne pas « rien se passer » dans la session.

Cohérence ★1 : 50 salades + 50 graines ≈ 100 plantes menées à terme. Grille ~6–8 laitues 2×2 en parallèle × cycle 4 h → ordre de grandeur **3–5 jours**. Aligné spec. Si on passe laitue à 20 min, ★1 explose en 1 après-midi → **recaler les 50/50**, pas l’inverse.

---

## 6. GameDesignData — champs de recherche (schéma)

C’est la partie « autre IA data » : on ne lui demande plus une opinion produit. On lui demande de **remplir des tables** à ids stables.

Une passe = **un palier** ou **une table**. Pas tout le GDD.

### 6.1 Tables (ids)

| Table | Fichier | 1 ligne = |
|-------|---------|-----------|
| Mode d’emploi | `Notes/GDD/data/GDD_balance.xlsx` onglet `LIRE` | — |
| Session / rétention | onglet `retention` / `GDD_retention.csv` | 1 champ |
| Plante | onglet `plantes` / `GDD_plantes.csv` | 1 plante |
| Stades / timers | onglet `stades_timers` / `GDD_stades_timers.csv` | 1 stade |
| Récolte | onglet `recolte` / `GDD_recolte.csv` | 1 item récoltable |
| Unlock | plus tard | 1 gate |
| Économie canal | plus tard | prix + cap volume |
| ★ biofiltre | spec existante | palier ★ |
| Aléa / recherche | plus tard | — |

### 6.2 Champs — rétention (`gdd.retention`)

| Champ | Type | Cible actuelle | Source |
|-------|------|----------------|--------|
| `sessionsPerDayMin` / `Max` | int | 2 / 3 | auteur + spec ★1 |
| `sessionMinutesMin` / `Max` | int | 5 / 10 | auteur (spec : 5–7) |
| `star1CalendarDaysMin` / `Max` | int | 3 / 5 | spec ★1 |
| `offlineCapHours` | float | **TBD 8–12** | industrie idle ; `[BL-GDD-002]` |
| `hookLoginActions` | enum[] | CollectReady, SellIfCap, PlantEmpty, SeeStarProgress | cette note |

### 6.3 Champs — plante (`gdd.plant.*`)

Alignés `PlantDefinition` (ne pas inventer un 2ᵉ modèle runtime).

| Champ | Type | Notes |
|-------|------|--------|
| `plantId` | string | = `PlantDefinition.plantId` (`lettuce`) |
| `displayName` | string | |
| `growthPattern` | Leafy \| Fruiting | |
| `unlockTier` | T1…T5 | |
| `visualRole` | mass \| vertical \| accent \| trailing | mix rendu |
| `footprintCells` | int | laitue = 4 |
| `timeToMatureSeconds` | float | somme stades jusqu’à Mature |
| `timeToSeedSeconds` | float | Mature + Flowering + Seedling (si on laisse) |
| `sessionBand` | S \| B \| N | §5.1 |
| `seedBuyPrice` | int | shop |
| `sellLeafPrice` | int | canal voisinage V0 |
| `sellSeedPrice` | int | |
| `harvestLeafMin` / `Max` | int | existant |
| `harvestSeedMin` / `Max` | int | existant |
| `xpOnLeafHarvest` | int | TBD — alimente 240 XP ★1 |
| `xpOnSeedHarvest` | int | TBD |
| `germinationBase` | 0–1 | V0 = 1.0 |
| `harvestCompleteBase` | 0–1 | V0 = 1.0 ; hazards plus tard |
| `unlockGate` | see §6.4 | |

**Interdit dans cette table :** élec, eau, farine poisson, market multi, repro. Autre système.

### 6.4 Champs — unlock (`gdd.unlock.plant`)

| Champ | Type | Exemple |
|-------|------|---------|
| `gateType` | `none` \| `biofiltreStar` \| `researchId` \| `systemMaturity` \| `mapId` | T4 = `systemMaturity` |
| `gateValue` | int / string | `1` (★1) ou `research.t2.leafy` |
| `requiresPlayerUnlock` | bool | graine connue halo |
| `requiresSupportEligibility` | bool | spec `CanPlace` double check |

### 6.5 Champs — dashboard joueur (minimum réel)

| Champ HUD | MVP | Plus tard |
|-----------|-----|-----------|
| `PrimaryCurrency` | **oui** | |
| Jauges ★ biofiltre (4 compteurs) | **oui** dès ★1 | |
| Stock graines / récoltes | inventaire, pas HUD | |
| Conso élec / eau / farine | **non** | sim + image éco |
| Leaderboard / Steam | **non** | post-chiffre |

Le dashboard « élec + eau + nourriture » du prompt brut est un **jeu de gestion** par-dessus le idle. Il tue le session 5–10 min.

### 6.6 Ce qu’on demande à une IA data (contrat)

Entrée : ce schéma + les specs liées + « ne pas inventer de feature ».  
Sortie : CSV / markdown **tables remplies**, questions **uniquement** si un champ bloque.  
Interdit : réécrire les specs prestige / vente / slots.

Prompt type : **§8.2**.

---

## 7. Doublons & questions ouvertes (à trancher, pas à halluciner)

| Sujet | Tension | Reco Cursor |
|-------|---------|-------------|
| 5–7 min (spec) vs 5–10 min (prompt) | Cosmétique | Garder **5–10** comme fourchette produit, 5–7 comme cible ★1 |
| Laitue 20 min vs 4 h | Change ★1 | Reco **4 h** + germ visuel court |
| 3 plantes/palier × 5 = 15 vs launch | Scope art | Launch **T1 seulement** |
| Poisson j7–10 décoratif vs gameplay | Auteur déjà prudent | Déco OK ; sim eau **non** tant que T1 pas sticky |
| Recherche « forcer à planter » vs halo Commerce | 2 arbres | Recherche **locale système** (`[BL-GDD-005]`) ≠ halo |
| Quêtes + 3ᵉ monnaie boulons | Inflation | 1 soft currency jusqu’à vente ★2 jouée |
| Analytique Unity + tests agents | Outillage | Après cap offline ; events = ids §6 (`plant_harvest`, `session_start`, `star1_progress`) |
| Inbox 6 stades vs runtime 7 | Doc | Runtime **gagne** ; mettre à jour `Inbox_gdd.md` |

---

## 8. Stock de prompts (propres, 1 job = 1 table)

À coller dans un chat / VM. **Ne pas** concaténer.

### 8.1 Revue MVP (toutes les 2 semaines)

```
Contexte: jeu aquaponie idle cozy, map FirstLvl.
Lis seulement: Notes/GDD/NOTE_mvp_hook_equilibrage.md, SPEC_progression_xp_joueur_et_biofiltre.md, SPEC_vente_production_boucle_jeu.md, PROJECT_LOG.md (dernière entrée).
Tâche: 1) lister ce qui a changé depuis la date de la note; 2) dire si le palier commercial (Must §3.1) est plus proche ou plus loin; 3) max 5 questions si specs se contredisent.
Interdit: inventer des features, réécrire les SPEC_*, proposer 15 plantes, toucher au code.
Sortie: section datée à append dans NOTE_mvp_hook_equilibrage.md §9.
```

### 8.2 Remplir GameDesignData plantes (IA data)

```
Tu remplis des tables, tu ne designs pas le jeu.
Schéma: Notes/GDD/NOTE_mvp_hook_equilibrage.md §6.
Ancres: laitue plantId=lettuce, sellLeaf=15, harvest feuille 1–3, graines 5–15, footprint 2×2, XOR Mature/Seedling, maxHarvestCount=1.
Cadence: 2–3 sessions/j, 5–10 min, ★1 en 3–5 jours (50 salades + 50 graines + 100 germinations).
Tâche: proposer UNIQUEMENT T1 (3 plantes: lettuce, basil, swiss_chard) — timeToMatureSeconds, répartition 7 stades, seedBuy, sellLeaf, sessionBand.
Contrainte: basil + chard en bande B (3–5 h). Laitue: justifier 4 h (reco) vs 20 min.
Sortie: tableau markdown champs §6.3. Liste TBD explicites. Pas de T2–T5.
```

### 8.3 Recalage timers après playtest

```
Entrée: durées réelles playtest (coller) + nb cellules occupées + nb sessions/j observé.
Comparer à gdd.retention et aux seuils ★1.
Si trop vite: monter timeToMature, ne pas baisser 50/50.
Si trop long: baisser Growing seulement, garder Graine/Starting visibles (< 2 min).
Sortie: nouveau tableau stades + impact estimé jours jusqu’à ★1.
```

### 8.4 Cut features (quand la wishlist revient)

```
Classer chaque item: Must hook / Should S1 / Later / Vote.
Références: NOTE_mvp_hook_equilibrage.md §3 + backlog BL-GDD-005 à 008.
Interdit: mettre élec, repro poisson, market multi dans Must.
```

---

## 9. Contrôle récurrent

| Id | Quand | Quoi |
|----|--------|------|
| `[BL-GDD-009]` | **2026-09-30** puis +14 j | Relancer prompt §8.1 · append une sous-section datée ici |
| Playtest timers | Dès durées ≠ debug 1 s | Prompt §8.3 |
| Soft-launch checklist | Avant store | Must §3.1 tous verts |

### Journal des revues

_(vide — première revue 2026-09-30)_

---

## 10. Liens

| Doc | Usage |
|-----|--------|
| `analyseMVP_Hoock_equilibrage.txt` | Inbox auteur (prompts bruts) |
| `SPEC_progression_xp_joueur_et_biofiltre.md` | ★1, cadence, XOR |
| `SPEC_progression_systeme_aquaponique_par_niveau.md` | Nœuds, aléas |
| `SPEC_prestige_generation_systemes.md` | G1/G2, cap vitesse |
| `SPEC_biofiltre_slots_shields.md` | Hazards / shields |
| `SPEC_vente_production_boucle_jeu.md` | Or, canaux, ★ vente |
| `SPEC_craft_atelier_aquaponique.md` | Atelier = pas le hook |
| `INBOX_notes_tablette_recherches.md` | Halo vs système |
| `Inbox_gdd.md` | 6 stades brouillon (obsolète vs runtime) |
| `Notes/References/REFERENCES_jeux_inspiration.md` | Tiny Harvest, Idle Farming Empire, Hay Day, Plantera |
| `Notes/GDD/data/GDD_balance.xlsx` | Tableur timers / plantes / rétention (CSV miroir) |
| `Assets/Data/Ferme/Laitue.asset` | Ancre runtime **debug** — ne pas caler ici |
| `Notes/Todo_project.md` | `[BL-GDD-001]` `[BL-GDD-002]` `[BL-GDD-009]` `[BL-GDD-010]` |
