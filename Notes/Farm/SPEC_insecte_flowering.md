# Spec — Insecte au stade Flowering (abeille / papillon)

**Ticket** : `[CT-FARM-POLISH-002]`  
**Statut** : spec prête — art + implémentation à faire  
**Date** : 2026-07-23  
**Liens** : `PlantGrow` / `PlantDefinition` · `Notes/Todo_project.md` · shell Bezy `[BZ-POLISH-014]` (placeholder overlay)

---

## Objectif joueur

Quand une plante est au stade **Flowering**, un petit insecte (abeille ou papillon) **virevolte** autour des fleurs : il vole de point en point, **butine** un instant, puis repart. Lisibilité + ambiance ferme, coût perf bas.

- **Leafy** (laitue, épinard…) : Flowering = **après** récolte feuilles.  
- **Fruiting** (tomate…) : Flowering = **avant** maturité / récolte.

---

## Décisions techniques (validées)

| Sujet | Choix | Pourquoi |
|-------|--------|----------|
| Rendu insecte | **Sprite sheet** + Animator | Pas un essaim : 1–few “personnages” lisibles |
| Particules | Non (ou pollen optionnel plus tard) | Moins de contrôle / identité |
| Shader | Non pour l’insecte | Coût / complexité ; éventuel wind feuilles ailleurs |
| Art par plante | **Non** | 1–2 sheets partagés (abeille, papillon) |
| Path par plante | **Oui** | Silhouette laitue ≠ tomate → nodes différents |
| Flip direction | `SpriteRenderer.flipX` (+ deadzone) | Art dessiné d’un seul côté |
| Activation | Seulement stade `Flowering` | Perf + sens gameplay |

**Mémoire** : une texture abeille chargée une fois, réutilisée par toutes les instances flowering. Ce qui change par plante = **positions des nodes**, pas un nouveau sheet.

---

## Architecture cible

```
Plant prefab (monde)
├── SpriteRenderer + PlantGrow          ← sprite plante
└── InsectPath (enfant, souvent inactif)
    ├── Node_0, Node_1, … Node_N        ← Transforms (positions fleurs)
    └── InsectInstance (spawn / ref)    ← Bee ou Butterfly prefab
            ├── SpriteRenderer
            └── Animator (Fly / Forage)
```

### Responsabilités

| Pièce | Rôle |
|-------|------|
| `PlantDefinition` | `insectKind` (None / Bee / Butterfly) + params optionnels (vitesse, durée butinage) |
| Prefab plante | Graphe local : liste de `Transform` nodes (+ arêtes optionnelles) |
| Prefab insecte | Art + Animator + script follower (réutilisable) |
| `PlantGrow.SetStage` | Active/désactive le path / insecte si stade == `Flowering` |

### Partagé vs par plante

| Élément | Partagé jeu | Par `PlantDefinition` / prefab |
|---------|-------------|-------------------------------|
| Sheet + clips Fly / Forage | Oui | — |
| Script FSM + move | Oui | — |
| Prefab Bee / Butterfly | Oui | Choix via `insectKind` |
| Nodes / arêtes | — | Positions adaptées à la silhouette |
| Vitesse / temps forage | Défauts globaux | Override optionnel |

---

## FSM insecte

```
        ┌─────────────────────────────────────────┐
        ▼                                         │
   FlyAlongEdge ──► ArriveNode ──► Forage ──► PickNextEdge
        ▲                │                         │
        └────────────────┴─────────────────────────┘
```

1. **FlyAlongEdge** — clip `Fly` (battement), déplacement le long de l’arête (lerp ou léger bruit Y).  
2. **ArriveNode** — court / instantané ; mémorise le node courant.  
3. **Forage** — clip `Forage` (ailes plus lentes + bob léger), durée ~0.5–2 s, **pas de flip**.  
4. **PickNextEdge** — voisin aléatoire ou suivant dans la boucle → retour Fly.

Pas de pathfinding global : graphe **local** 2–6 nodes sur le prefab suffit.

### Graphe de path (configurable)

**MVP** : boucle ordonnée `0 → 1 → 2 → … → 0`.  
**V1** : liste d’arêtes `(fromIndex, toIndex)` pour éviter des traversées absurdes.

Exemple placement :

- **Laitue** : 3–4 nodes en couronne basse autour du cœur fleuri.  
- **Tomate** : 2–3 nodes plus hauts, près des grappes / fleurs.

---

## Orientation sprite (gauche / droite)

Art du sheet : insecte **toujours orienté vers la droite** (convention).

Pendant `FlyAlongEdge` uniquement :

```text
dir = nextNode - currentPos
si |dir.x| > deadzone (ex. 0.01) :
    flipX = (dir.x < 0)
sinon :
    garder la dernière orientation   // vol surtout vertical
```

**Préférer `SpriteRenderer.flipX`** plutôt que `localScale.x = ±1` si un enfant anime déjà le scale (bob butinage) → pas de conflit.

Pendant `Forage` : ne pas changer `flipX` (reste dans le sens d’arrivée).

---

## Données / Inspector (cible)

### Sur `PlantDefinition` (proposition)

```text
insectKind          : None | Bee | Butterfly
insectMoveSpeed     : float (optionnel, défaut script)
forageDurationMin   : float
forageDurationMax   : float
```

*(Noms exacts à figer à l’implémentation Cursor.)*

### Sur le prefab plante — composant type `InsectPathAnchor`

```text
nodes[]             : Transform[]
useExplicitEdges    : bool
edges[]             : (from, to)   // si true
insectRoot          : Transform    // parent spawn / instance
```

### Sur le prefab insecte — composant type `InsectPathFollower`

```text
spriteRenderer
animator
moveSpeed
forageDurationRange
flipDeadZone
```

Triggers / états Animator : `Fly`, `Forage` (bool ou crossfade states).

---

## Perf & règles runtime

- Max **1 insecte** visible par plant instance (MVP).  
- Overlay / path **inactif** hors `Flowering`.  
- Pas d’`Update` lourd : un seul follower actif par plant flowering.  
- Sheets 256–512 px, compression Unity (ASTC / ETC selon plateforme).  
- Pas de sheet “abeille-laitue” vs “abeille-tomate”.

---

## Pipeline de livraison (ordre)

| # | Qui | Livrable |
|---|-----|----------|
| 1 | Art (ChatGPT / auteur) | Sprite sheet abeille (puis papillon) — voir prompt ci-dessous |
| 2 | Unity import | Slice Multiple, pivots centre, `Pixels Per Unit` aligné farm |
| 3 | Cursor | Scripts `InsectPathFollower` + hook `PlantGrow` + champs `PlantDefinition` |
| 4 | Bezy | Shell overlay / nodes sur prefabs plantes (`[BZ-POLISH-014]` puis wiring) |
| 5 | Auteur | Placer nodes laitue / tomate ; playtest Flowering |

---

## Spec art — sprite sheet abeille (prompt ChatGPT / image gen)

### Contraintes projet

- Style : **pixel art** ou **sprite 2D clean** cohérent avec les plantes farm (ajuster selon art plantes existantes).  
- Fond : **transparent** (PNG).  
- Orientation : insecte **face / 3/4, corps vers la DROITE**.  
- Taille cible sheet : **256×256** ou **512×256** (grille régulière).  
- Pas de texte, pas d’ombre portée lourde, pas de blur.

### Contenu frames (minimum)

**Rangée A — Fly (vol)** : 4 à 6 frames  
- Ailes hautes / moyennes / basses (loop battement).  
- Corps stable, légère variation verticale OK.

**Rangée B — Forage (butinage)** : 4 frames  
- Ailes semi-fermées ou battement plus lent.  
- Corps un peu plus bas / “posé” sur une fleur imaginaire.  
- Peut inclure une légère oscillation (même pose, micro-offset).

### Grille suggérée

```text
Sheet 512×256 (exemple)
┌────┬────┬────┬────┬────┬────┐
│ F0 │ F1 │ F2 │ F3 │ F4 │ F5 │  ← Fly
├────┼────┼────┼────┼────┴────┤
│ G0 │ G1 │ G2 │ G3 │         │  ← Forage
└────┴────┴────┴────┴─────────┘
Cellules ~64×64 ou 85×85 selon grille choisie (régulière !).
```

### Prompt type (à coller / adapter)

```text
Pixel art sprite sheet of a small cute honeybee, side view facing RIGHT,
transparent background, consistent outline, farm game style.
Top row: 6 frames wing-flapping flight loop.
Bottom row: 4 frames foraging / hovering over a flower (slower wings, body slightly lower).
No text, no shadows, even frame grid, 512x256 PNG.
```

### Variante papillon (plus tard)

Même grille / mêmes états `Fly` / `Forage`, art ailes plus larges, toujours orienté **droite**. Même script, `insectKind = Butterfly`.

### Checklist import Unity

- [ ] Texture Type : Sprite (2D et UI)  
- [ ] Sprite Mode : **Multiple**  
- [ ] Slice grille (cell size fixe)  
- [ ] Pivot : Center  
- [ ] Nommer : `Bee_Fly_0…`, `Bee_Forage_0…`  
- [ ] Dossier cible proposé : `Assets/Art/Sprites/Farm/Insects/`  
- [ ] Créer Animation Clips `Bee_Fly`, `Bee_Forage` + Controller

---

## Critères d’acceptation

- [ ] Une laitue en `Flowering` montre une abeille qui suit ses nodes.  
- [ ] Une tomate en `Flowering` réutilise le **même** prefab abeille, nodes **différents**.  
- [ ] Hors Flowering : aucun insecte actif.  
- [ ] Changement de direction horizontale → `flipX` correct, sans tremblement (deadzone).  
- [ ] `PlantDefinition` avec `insectKind = None` → pas d’insecte.  
- [ ] Pas de second système (particules / instantiate direct hors path).

---

## Anti-patterns

- Sheet distinct par espèce de plante.  
- Animator Controller dupliqué par plante.  
- Scale X négatif **et** anim de scale sur le même Transform.  
- Particules “fake bee” à la place du sprite.  
- Path hardcodé en code (positions magiques) au lieu de nodes sur le prefab.

---

## Suite immédiate

1. Générer / valider le **sheet abeille** (prompt ci-dessus).  
2. Quand l’art est OK : session Cursor scripts + session Bezy nodes sur prefabs.  
3. Mettre à jour cette note avec chemins d’assets réels et noms de classes finaux.
