# Spéc — prestige / génération par système (biofiltre, bandeaux)

**Statut :** vision auteur actée (2026-08-27) — **pas de code** dans ce document.  
**Granularité :** prestige **par instance / par canal**, jamais un wipe global de la ferme.  
**Backlog :** `[BL-GDD-006]`.

> Vocabulaire **joueur** : **Reconstruire** / **Nouvelle génération**.  
> « Reset » = terme interne / design uniquement.

---

## 1. Intention

Quand un **outil** a assez vécu, le joueur peut le **nettoyer et l’upgrader** (nouvelle génération). Il reprend le même emplacement avec un **plancher** plus haut et un **visage neuf**. Il ne recommence pas le jeu.

| Couche | Exemple | Prestige ? |
|--------|---------|------------|
| Joueur (halo, recettes, talents) | Arbre Commerce | **Non** — on ne reset pas le joueur |
| Instance ferme | Biofiltre `FirstLvl` | **Oui** — portes **★3** (slot secondaire) **ou** **★5** (slot primaire) |
| Canal de vente | Bandeau voisinage ★1–5 | **Horizon** — après la courbe étoiles du canal (§7) |

**Ce qui se garde toujours :** unlocks joueur, talents halo, recettes, canaux débloqués, monnaie, relations PNJ, maturité « j’ai le droit de planter X » côté joueur.

**Ce qui se recycle :** niveau / usure / habillage **de cet objet**, jauges locales de génération.

---

## 2. Prestige = nettoyage + upgrade

Un prestige n’est **pas** un respec de nœuds (question ouverte du panneau aquaponique). C’est un **rite d’installation** :

1. Le joueur **vide** la grille (récolte / retrait de **toutes** les cultures).
2. Il confirme **Reconstruire**.
3. Le jeu joue le **nettoyage** (narratif + VFX / nouvel habillage) **et** applique l’**upgrade** de la génération suivante.

Si une culture est encore sur la grille → **blocage** + message. Pas de prestige partiel, pas d’arrachage forcé.

---

## 3. Garde-fou grille (biofiltre) — bloquant

### 3.1 Condition

Le prestige du biofiltre n’est **autorisé** que si **aucune cellule occupée** : aucune plante à n’importe quel stade (graine → seedling → baby → croissance → mature / récoltable / fleur).

Une plante **prête à récolter** compte comme occupation : le joueur doit d’abord récolter (ou retirer) jusqu’à grille vide.

Ancrage runtime futur : `BiofiltreManager` / `BiofiltreGridVisualizer` / `BiofiltreCell` — toute instance `PlantGrow` encore parente de la grille.

### 3.2 Message (fail closed)

Si le joueur tente le prestige alors qu’une culture est en cours :

- **Ne pas** lancer le prestige.
- **Afficher** un message explicite (popup pipeline générique : `PopupId` dédié + `ScreenPopupHost` sur `FirstLvl` — pas d’instanciation ad hoc).

**Copy de travail (FR, à figer en localisation) :**

> Tu as encore des plantes sur le biofiltre. Récolte ou vide la grille avant de le reconstruire.  
> Reconstruire = nettoyer l’installation, puis l’améliorer.

Le bouton peut rester visible mais **disabled** + même explication au clic / tooltip, tant que le message bloquant existe (pas d’échec silencieux).

### 3.3 Après confirmation (grille déjà vide)

Le prestige peut alors enchaîner nettoyage + application des bonus / skin de la génération. Les cultures **ne sont pas** détruites par le prestige : elles ont déjà été vidées par le joueur.

---

## 4. Générations biofiltre (actées)

Les bonus **se cumulent** d’une génération à l’autre. On **n’empile pas** vitesse **et** quantité dès le premier prestige, pour éviter un rush trop fort trop tôt.

| Génération | Prestige n° | Croissance (vitesse) | Quantité récolte | Autre | Narratif / visuel |
|------------|-------------|----------------------|------------------|-------|-------------------|
| **G0** | — (état de départ) | 0 | 0 | Media / cuve de base | IBC / planteur de départ |
| **G1** | 1er | **+5 %** | **aucun** | Isolation | Habillage plus beau + isolation (justifie la stabilité / vitesse) |
| **G2** | 2e | +5 % (conservé) | **+5 %** | **Media de meilleure qualité** | Media visible ; **qualité de l’eau** une fois le fishtank implémenté |
| **G3+** | 3e et suivants | TBD | TBD | TBD | TBD — **sous cap** (§5) |

Constantes de design (noms internes, pas de magic number en code) :

- `BiofiltrePrestigeGen1GrowthBonus` = **+0,05** sur la vitesse de croissance (`PlantGrow` / durées de stades).
- `BiofiltrePrestigeGen2QuantityBonus` = **+0,05** sur la quantité de production (récolte).
- G2 **n’ajoute pas** un second +5 % vitesse : G1 reste le seul palier vitesse acté pour l’instant.

**Isolation (G1) :** plus de stabilité thermique → cycles plus courts. Pas de bonus quantité (l’isolation ne fait pas pousser plus de feuilles).

**Media (G2) :** meilleur support racinaire / filtration → +5 % quantité **maintenant** ; plus tard, modificateur **qualité d’eau** lu par le fishtank (espèces, densité, stabilité). Ne pas coder le lien eau tant que le bassin n’existe pas : **réserver le flag** `mediaQualityTier` (ou équivalent) sur l’instance.

---

## 5. Cap anti-spam (obligatoire plus tard)

Sans plafond, des prestiges répétés + autres bonus (talents, nœuds système) peuvent ramener un cycle salade à **quelques secondes**.

**Décision produit :** un **cap** sera nécessaire. Valeurs **non calibrées** (playtest).

Pistes (en choisir une ou les combiner à l’équilibrage) :

- **Plancher de durée** : aucun stade / cycle complet sous `MinPlantCycleSeconds` (ex. ordre de grandeur à playtester).
- **Plafond de générations** ou **rendement décroissant** (G3 = +3 %, G4 = +2 %…).
- **Plafond additif** sur le modificateur prestige seul (ex. croissance prestige max +25 %).

Le cap s’applique au **modificateur prestige** (et éventuellement au total vitesse après talents). À trancher quand G3+ et le panneau nœuds seront jouables.

---

## 6. Portes de prestige biofiltre (★3 ou ★5)

Le garde-fou **grille vide** est acté. ★1 n’ouvre **aucun** slot.

**Règle dure — kill des étoiles :** tout prestige (★3 **ou** ★5) remet `biofiltreStarTier` à **0**. C’est le joueur qui choisit son **focus** et le **theorycraft** (rush défenses vs wait structures vs hybride). Les **slots déjà ouverts** se gardent.

| Porte | Ouvre | Détail |
|-------|--------|--------|
| Prestige à **★3 ou ★4** | **1 slot secondaire** | Reco : pas de lock ★4→★5 |
| Prestige à **★5** | **1 slot primaire** (downpick secondaire reco) | + éclats = ★ tuées — `SPEC_biofiltre_slots_shields.md` §1.2 |

Pas les deux dans le même cycle (les ★ sont mortes). Détail + exemples de styles : **`Notes/GDD/SPEC_biofiltre_slots_shields.md`** §1.

Bonus isolation / media / +5 % : selon le **numéro** de prestige (G1, G2…), **en plus** du slot.

Bandeaux vente : **autre** système — ne pas calquer ★3 = slot. Prestige canal reste horizon **après** sa propre courbe (§7).

---

## 7. Horizon — bandeaux de vente

Même **famille** de mécanique (prestige local), **pas** V0.

- Les **étoiles 1–5** restent l’upgrade **dans** une génération du canal (`SPEC_vente_production_boucle_jeu.md` §2.9).
- Relancer un bandeau **avant ★5** (ex. à ★3) coupe la courbe **vente** : **à éviter** (les portes ★3/★5 **slots** sont **biofiltre only**).
- Cible : prestige canal **après** maîtrise de la courbe (★5 ou palier « génération » post-★5) → étoiles redescendent, **plancher** de base plus haut (volume / prix / voisin fidèle — TBD).
- Chaque canal prestige **tout seul** (voisinage relancé ≠ vélo).
- Garde-fou analogue possible : pas de cycle de vente **en cours** sur ce canal (cooldown / stock délégué) — à spécifier quand les bandeaux seront jouables en ★5.

---

## 8. Architecture (pistes, plus tard)

| Donnée | Rôle |
|--------|------|
| `biofiltreStarTier` | 0–5 — courbe **dans** la génération courante |
| `prestigeGeneration` | 0, 1, 2… — après prestige |
| `mediaQualityTier` | 0 = base, 1+ = G2 ; lu plus tard par fishtank |
| `unlockedSecondarySlotIds` / `unlockedPrimarySlotIds` | Slots ouverts au prestige ★3 / ★5 |
| `equippedShieldId` + `shieldTier` + charges | Shields — `SPEC_biofiltre_slots_shields.md` |

Save : même identité d’instance que le panneau aquaponique (`systemInstanceId`, ex. `firstlvl.biofiltre.main`).

UI : bouton **Reconstruire** dans le panneau système (onglet Biofiltre) ou proximité world — **TBD** avec l’emplacement du bouton `[BL-GDD-005]`. Confirmation = popup générique ; refus grille occupée = **autre** (ou même) popup avec copy §3.2.

---

## 9. Questions ouvertes

- [x] Kill des ★ au prestige ★3 **et** ★5 — acté. Theorycraft / focus joueur. Slots gardés. — `SPEC_biofiltre_slots_shields.md` §1.
- [ ] Forme exacte du **cap** vitesse / générations (§5).
- [ ] G3+ : quels bonus (eau, anti-aléa, slots grille) vs skin seulement.
- [ ] Le nettoyage a-t-il une **durée** (mini cinématique, timer) ou est-il instantané après confirm ?
- [ ] Prestige bandeaux : palier exact (★5 vs post-★5) et bonus de plancher.
- [ ] Respec des **nœuds** du panneau ≠ prestige : oui/non, coût — reste ouvert dans `[BL-GDD-005]`.

---

## 10. Liens

| Doc | Lien |
|-----|------|
| Slots / shields (★3 secondaire, ★5 primaire) | `Notes/GDD/SPEC_biofiltre_slots_shields.md` |
| Panneau onglets Biofiltre / Poisson / Techno | `Notes/GDD/SPEC_progression_systeme_aquaponique_par_niveau.md` |
| Étoiles canaux de vente | `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §2.9 |
| Carte code ferme | `Notes/Farm/SYSTEMES_carte_mentale.md` |
| Popup runtime | `Notes/Ui/popup_generique.md` |
| Tâches | `Notes/Todo_project.md` — `[BL-GDD-006]` |
