# Inbox — notes tablette (référence perso)

**« Tablette »** = support de **notes manuscrites** et **recherches perso** de l’auteur, **pas** un système ou une feature nommée dans le jeu.

Ce fichier est le **hub d’accueil** quand ces notes seront retranscrites ou importées dans le repo.

**Tâche projet liée :** **[P0-IDEA-001]** — `Notes/Todo_project.md` § *Prochaine session* (synthèse boucle gameplay + 3–5 tâches validées).

---

## Rôle

- Conserver le lien avec le rework inventaire : `Notes/Ui/SPEC_rework_inventaire_halo_progression.md`
- Accueillir progressivement le contenu des notes papier (pistes de recherche, boosts, arbres de talents, etc.)
- **Ne pas** inventer de règles ici tant que les notes originales ne sont pas apportées
- Croiser avec le **vrac déjà numérisé** (voir § *Contenu déjà dans le repo*) avant de dupliquer

---

## Cartographie — docs liés aux idées tablette

| Fichier | Contenu utile pour [P0-IDEA-001] | Statut |
|---------|----------------------------------|--------|
| **`Notes/GDD/Inbox_gdd.md`** | Brouillon GDD : états plantes (6 stades), salade vs tomate, Farm City, mini-Trello 15 min/jour, IA Google casual farm | **Partiellement transcrit** — à relire / compléter depuis tablette |
| **`Notes/Ui/SPEC_rework_inventaire_halo_progression.md`** | Vision UI halo (XP centre, talents acheteur / vendeur), §4 renvoie ici | Spec UI — règles métier **après** import notes |
| **`Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`** | XP joueur, maturité biofiltre (~10 cycles salade), gating cultures fruits | Spec partielle — à aligner avec notes tablette |
| **`Notes/References/REFERENCES_jeux_inspiration.md`** | Veille jeux idle / cozy farm — alimente idées, pas engagement prod | Référence externe |
| **`Notes/GDD/SPEC_vente_production_boucle_jeu.md`** | Vente des récoltes (boucle économie) — gap code, questions UX/prix, carnet veille §6 | Spec GDD — **2026-06-10** |
| **`Notes/Inbox_features.md`** | Vrac features sans priorité (vide pour l’instant) | Destination possible post-session |
| **`Notes/Ui/Journal_ui.md`** § *Inbox (idées brutes)* | Idées UI en vrac (historique) | Secondaire |
| **`Notes/Todo_project.md`** § *Stock en attente* | [CT-INV-HALO-001], [CT-FARM-UI-001], etc. — **gelé** jusqu’à clôture [P0-IDEA-001] | Priorités à revalider |

---

## Contenu déjà dans le repo (extrait `Inbox_gdd.md`)

> À **vérifier / compléter** avec les notes manuscrites tablette lors de [P0-IDEA-001].

- **États plantes (piste 6 stades)** : graines → seedling → baby → mature/récoltable → … (brouillon incomplet dans `Inbox_gdd.md`)
- **Salade vs tomate** : chaînes d’états différentes (croissance, fleur, fruit mûr / graines)
- **Meta / workflow auteur** : mini-Trello tâches 15 min/jour ; à voir Farm City ; veille IA casual farm
- **Halo inventaire** : arbres talents commerce (acheteur / vendeur) — intention visuelle dans spec halo, détails talents **manquants** (tablette)

---

## Synthèse notes perso — halo & compétences (session 2026-06-05)

> Import structuré pour la tâche **lier slots halo → arbres**. Plan d’exécution : `Notes/Ui/SESSION_prochaine_halo_arbres_competences.md`.

### Règles de progression joueur (notes + spec halo — actées)

- Points dépensés pour débloquer des nœuds ; **ordre libre** entre pistes.
- Chaque piste = **avantages et inconvénients** ; équilibrage = chantier long (volume/vitesse vs marge/prix).
- UI : **pas** d’onglets par compétence dans l’overlay — le **slot halo** choisit la piste.

### Pistes halo (mapping provisoire 8 slots)

| Slot | ID cible | Notes perso |
|------|----------|-------------|
| 1 | `track.commerce` | Branches **Acheteur** / **Vendeur** — **prototype v1** |
| 2 | `track.plant` | Lié aux 6 stades plantes (salade / tomate) — vitesse, rendement |
| 3 | `track.fish` | Culture poisson — bonus indirect plantes ; détail nœuds **TBD tablette** |
| 4 | `track.agronomy` | Anti-aléas : limaces, germination, casse récolte |
| 5 | `track.logistics` | Logistique — **TBD tablette** |
| 6 | `track.technology` | Recherches / équipement transverse |
| 7–8 | réservé | Verrouillés au lancement ou pistes futures |

### Arbre Commerce (structure notes — effets à chiffrer)

```
Racine Commerce
├── Acheteur : remises achat, offres shop
└── Vendeur : prix vente, bonus volume market
```

### Distinction explicite (note session 2026-06-04)

- **Halo inventaire** = progression **joueur** globale (cette tâche).
- **Panneau ferme `FirstLvl`** = progression **système aquaponique** par niveau (onglets Biofiltre / Poisson / Techno) → `Notes/GDD/SPEC_progression_systeme_aquaponique_par_niveau.md` — **autre chantier**.

### Encore manquant (tablette)

- Coûts en points par nœud Commerce.
- Effets % exacts Acheteur / Vendeur.
- Source XP / points joueur (niveau seul vs récoltes vs quêtes).
- Détail nœuds Culture plante, Poisson, Logistique.

---

## À importer (quand disponible — session [P0-IDEA-001])

- [ ] Transcription ou scan des notes manuscrites tablette
- [ ] Liste des pistes / recherches envisagées (agronomie, commerce, logistique…)
- [ ] Détail arbres (ex. commerce : acheteur / vendeur)
- [ ] Vision **boucle gameplay** explicite : ferme ↔ shop ↔ inventaire ↔ runner (si présente sur tablette)
- [ ] Toute recherche externe (références jeux, articles) — croiser avec `REFERENCES_jeux_inspiration.md`
- [ ] Synthèse 1 page + 3–5 tâches ordonnées → `Notes/Todo_project.md` + `PROJECT_LOG.md`

---

## Liens utiles côté projet

- Tâche session : **`Notes/Todo_project.md`** → **[P0-IDEA-001]**
- Vision UI halo + talents : `Notes/Ui/SPEC_rework_inventaire_halo_progression.md`
- XP joueur + biofiltre : `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md`
- Vrac GDD déjà saisi : `Notes/GDD/Inbox_gdd.md`
- Veille jeux : `Notes/References/REFERENCES_jeux_inspiration.md`
- Index doc : `INDEX.md` (entrées GDD / tablette)
