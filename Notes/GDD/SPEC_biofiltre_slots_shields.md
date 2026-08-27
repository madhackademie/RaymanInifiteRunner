# Spéc — slots & shields biofiltre (prestige ★3 / ★5)

**Statut :** vision auteur actée (2026-08-27) — **pas de code**.  
**Backlog :** `[BL-GDD-007]`.  
**Parent :** prestige = nettoyage + upgrade — `Notes/GDD/SPEC_prestige_generation_systemes.md`.

Les **slots** s’ouvrent au prestige. On y équipe des **shields** (protections) à **plusieurs niveaux**.

---

## 1. Deux portes + kill des étoiles (acté)

**Règle dure :** un prestige **tue les étoiles** de cette génération (`biofiltreStarTier` → **0**), **peu importe** la porte **★3** ou **★5**. Pas de checkpoint. Pas de « je garde mes ★ et j’ouvre un slot ».

C’est **volontaire** : le joueur choisit son **focus** et le **theorycraft** qui va avec (défenses précoces vs structures tardives vs mix sur plusieurs générations).

| Porte | Condition | Slot | Éclats (reco §1.2) |
|-------|-----------|------|---------------------|
| — | ★1–★2 | Aucun prestige | — |
| **Secondaire** | **★3 ou ★4**, grille vide | **1 slot secondaire** | **3** ou **4** (autant de ★ tuées) |
| **Primaire** | **★5**, grille vide | **1 slot primaire** (ou downpick secondaire, reco) | **5** |

On n’ouvre **pas** un secondaire **et** un primaire dans le même cycle : les ★ sont à zéro, il faut remonter la courbe.

- ★1 n’ouvre **aucun** slot.
- Grille vide + message : inchangé (`SPEC_prestige_generation_systemes.md` §3).
- Bonus G1 / G2 (isolation, media, +5 %…) : selon le **numéro** de prestige, **en plus** du slot — identiques que le kill soit à ★3 ou ★5.

**Campagne :** 5 secondaires + 3 primaires = **8 prestiges** min. si 1 slot par prestige. Le **style** se construit en mixant les portes au fil des générations (ex. trois ★3 anti-pestes puis un ★5 serre, ou rush serre d’abord).

**Choix du slot :** le joueur **sélectionne** l’emplacement encore fermé de la classe (secondaire si ★3, primaire si ★5). Pas d’ordre forcé (anti-slug pas obligatoirement le 1er) — **TBD** tutoriel.

### 1.1 Theorycraft (intention produit)

Il n’y a **pas** de prestige « correct ». Exemples de focus, pas de méta imposée :

| Focus | Pattern | Payoff |
|-------|---------|--------|
| **Défense** | Prestiges ★3 répétés | 5 secondaires plus tôt ; serre plus tard |
| **Structure** | Attendre ★5 | Primaire (serre…) plus tôt ; pestes plus longtemps à nu |
| **Hybride** | Alterner ★3 / ★5 | Compromis ; chaque cycle un sacrifice de courbe ★ |

Les slots **ouverts se gardent** d’une génération à l’autre (ce qui meurt = les **étoiles**, pas les emplacements déjà débloqués). Shields équipés / paliers : se gardent aussi — **acté en travail** (sinon le theorycraft ne tient pas).

### 1.2 À ★4 : ne pas forcer ★5 — reco (à valider)

**Problème :** si ★4 « casse » la porte secondaire et oblige d’aller à ★5, le joueur est coincé. En endgame, un palier d’étoiles peut devenir **très long**. Changer de focus (raid limaces, besoin d’un secondaire **maintenant**) devient punitif.

**Ne pas** lock ★4 → wait ★5.

**Reco :**

1. **Prestige secondaire dès ★3, y compris ★4.** ★5 seul ouvre un **primaire**.
2. **Récompenser les ★ tuées**, pas seulement le type de slot. Chaque étoile sacrifiée → **1 éclat** (monnaie prestige / shields, piste B §4).  
   ★3 = 3 éclats + secondaire · ★4 = **4** éclats + secondaire · ★5 = 5 éclats + primaire.
3. **Deux couches de reward :**  
   - **Pendant** la génération : les bonus d’étoile ★2–★5 (encore TBD) — tenir ★4 a déjà payé tant qu’on ne prestige pas.  
   - **Au cash-out :** le slot **et** les éclats. ★4 n’est jamais « la même chose que ★3 + du grind perdu ».
4. **À ★5 :** le primaire est **débloqué**, pas forcé — pouvoir **downpick** un secondaire (tous les primaires déjà ouverts, ou focus peste). Les 5 éclats restent.

★2 : pas de prestige (trop tôt, comme ★1).

À valider auteur avant de passer **acté**.

---

## 2. Catalogue des slots

### 2.1 Secondaires (5) — prestige ★3

| # | Id (travail) | Aléa | Niveaux détaillés |
|---|--------------|------|-------------------|
| 1 | `slot.secondary.anti_slug` | Limaces | **Oui** — §3 (exemple acté) |
| 2 | `slot.secondary.anti_souris` | Souris | TBD (même grammaire : consommable → barrière → …) |
| 3 | `slot.secondary.anti_oiseau` | Oiseaux | TBD |
| 4 | `slot.secondary.anti_fourmis` | Fourmis | TBD |
| 5 | `slot.secondary.anti_moisissure` | Moisissure | TBD |

### 2.2 Primaires (3) — prestige ★5

| # | Id (travail) | Rôle | Niveaux |
|---|--------------|------|---------|
| 1 | `slot.primary.serre` | Climat / saison / gel / pluie sur les cultures | **Lvl 1** voile de **forçage** · **Lvl 2** **bâche à bulles** · **Lvl 3** **serre géodésique** |
| 2 | `slot.primary.tbd_2` | *À déterminer* | TBD |
| 3 | `slot.primary.tbd_3` | *À déterminer* | TBD |

Les primaires sont des **structures** (plus visibles sur le biofiltre) ; les secondaires sont des **défenses d’aléa**.

---

## 3. Exemple acté — slot secondaire anti-slug

Un seul slot, **4 niveaux** de shield (on équipe / on upgrade, on ne pose pas les 4 en même temps).

| Niv. | Shield | Type | Effet (valeurs de travail) | UI / règles |
|------|--------|------|----------------------------|-------------|
| **1** | **Graines anti-slug** | **Consommable** | 1 charge ≈ **5 limaces** stoppées (`seedChargesPerSlug = 5` — playtest) | Slot **clignote** quand **vide**. Sans charge : raids **non absorbés**. |
| **2** | **Barrière cuivre** | **Permanent** | **−50 %** intensité / dégâts des **slug raids** | Pas de clignotement vide (pas de stock). |
| **3** | **Barrière cuivre électrifiée** | **Permanent** | **−75 %** dégâts / « décimation » limaces | Upgrade de la barrière cuivre. |
| **4** | **Nématodes** | **Consommable** | **Bonne** réserve de limaces avant depletion ; **tant qu’actif : 90 %** de protection | Clignote à depletion. Meilleur tampon que les graines. |

**Pression limaces (quand ça spawn / ça tape) :**

- **Nuit** : génération de limaces.
- **Pluie** : génération de limaces (en plus ou à la place selon météo — **TBD** si cumul nuit+pluie).
- Quantité par raid : **TBD** ; les graines niv.1 servent de petit stock d’urgence (clignotement = « recharge-moi »).

Météo / cycle jour-nuit : pas encore de système ferme complet — **réserver** les flags `hazard.slug.night` et `hazard.slug.rain` pour quand le temps existera. Ne pas inventer un simulateur météo dans cette spec.

---

## 4. Upgrade des niveaux de shield (monnaie — ouvert)

Les **niveaux** (graines → cuivre → électrifié → nématodes, ou voile → bâche → géodésique) se débloquent **plus tard** que l’ouverture du slot. Pistes auteur, **une à retenir** (ou mix) :

| Piste | Idée | Risque |
|-------|------|--------|
| **A** | Prochains **prestiges** (chaque reconstruction débloque 1 palier sur un shield déjà sloté) | Lent ; lie upgrade défense au wipe d’étoiles |
| **B** | **Étoiles comme monnaie** | ★ tenues **et** currency — lisible seulement si on ne dépense pas les ★ **en cours** de courbe |
| **B2 (reco §1.2)** | **Éclats** = ★ **tuées** au prestige (3 / 4 / 5) | Reward du palier ★4 ; paliers de shield sans forcer un 2e prestige. Les ★ **vivantes** restent un palier, pas un porte-monnaie |
| **C** | **Or / monnaie gagnée en jeu** uniquement | Simple, aligne shop ; le prestige n’ouvre que le **slot vide** |

**Pas tranché.** Reco de doc : ouvrir le slot au prestige (**vide** ou shield niv.1 offert **TBD**) ; les paliers 2+ = piste **C** (or) ou **B** (étoiles) pour ne pas forcer un prestige à chaque fil de cuivre.

Si le prestige ★3 donne déjà le slot **avec** graines anti-slug niv.1 : le clignotement vide a un sens dès la première nuit.

---

## 5. UX (intention)

- Slots **fermés** : cadenas / silhouette jusqu’au prestige de la bonne porte.
- Slot **ouvert, consommable vide** : **clignote** (anti-slug graines / nématodes). Fail **visible**, pas silencieux.
- Slot **ouvert, permanent** : icône stable + % protection.
- Pose / upgrade shield : panneau système onglet Biofiltre (même host que `[BL-GDD-005]`) — **Bezy** plus tard, pas d’instanciation popup hors pipeline.

Ne **pas** dupliquer les nœuds « anti-limaces » du panneau aquaponique : les **shields** sont la défense V0 ; les nœuds = autre couche ou à fusionner plus tard.

---

## 6. Save (pistes)

Par instance biofiltre :

- `unlockedSecondarySlotIds[]` / `unlockedPrimarySlotIds[]`
- Par slot : `equippedShieldId`, `shieldTier`, `consumableCharges` (si applicable)

Ids stables (`HazardId` / `ShieldId`) pour limaces, souris, oiseaux, fourmis, moisissure.

---

## 7. Questions ouvertes

- [x] Kill des ★ au prestige ★3 **et** ★5 — **acté** (2026-08-27). Pas d’alternative checkpoint.
- [ ] **Reco §1.2** (pas encore acté) : secondaire à ★3 **et ★4** ; primaire à ★5 ; **éclats** = nombre de ★ tuées ; pas de lock wait-★5. Downpick secondaire à ★5.
- [ ] Slot ouvert **vide** vs **niv.1 offert**.
- [ ] Monnaie paliers : A / B / C (§4).
- [ ] 1 graine = 5 limaces : playtest.
- [ ] Cumul nuit + pluie.
- [ ] Deux primaires restants.
- [ ] Courbes souris / oiseaux / fourmis / moisissure (calquer anti-slug).
- [ ] Le prestige bandeau **n’ouvre pas** ces slots (biofiltre only).

---

## 8. Liens

| Doc | Lien |
|-----|------|
| Prestige, G1/G2, grille vide | `Notes/GDD/SPEC_prestige_generation_systemes.md` |
| Étoiles biofiltre ★1… | `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md` |
| Aléas / nœuds | `Notes/GDD/SPEC_progression_systeme_aquaponique_par_niveau.md` §5.4 |
| Tâches | `Notes/Todo_project.md` — `[BL-GDD-007]` |
