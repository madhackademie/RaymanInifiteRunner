# Spec — Insecte au stade Flowering (abeille / papillon)

**Ticket** : `[CT-FARM-POLISH-002]` · priorités session `[P0-FARM-INSECT-001..003]`  
**Statut** : art MVP intégré — sheet Fly 6 frames prêt ; scripts / Bezy path à enchaîner  
**Date** : 2026-07-25 (maj art `Bee_Fly`)  
**Liens** : `PlantGrow` / `PlantDefinition` · `Notes/Todo_project.md` · shell Bezy `[BZ-POLISH-014]`  
**Réfs qualité VFX/critters** : Farm Together, Dinkum, Coral Island → `Notes/References/REFERENCES_jeux_inspiration.md` § E

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
| Art par plante | **Non** | 1 sheet / espèce d’insecte, partagé |
| Path par plante | **Oui** | Silhouette laitue ≠ tomate → nodes différents |
| Flip direction | `SpriteRenderer.flipX` (+ deadzone) | Art vers la **droite** ; pas de sheet 8 directions |
| Activation | Seulement stade `Flowering` | Perf + sens gameplay |
| Unity Sprite Atlas | **Plus tard** | Voir § Atlas ci-dessous — inutile pour démarrer le code |

**Mémoire** : une texture abeille chargée une fois, réutilisée par toutes les instances flowering. Ce qui change par plante = **positions des nodes**, pas un nouveau sheet.

**Clarification « 8 directions » (ChatGPT)** : la promesse décrit **8 frames de battement d’ailes** (loop vol), **pas** 8 orientations cardinales. Notre runtime = **1 profil + flipX**.

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

## Atlas sprites — utile ? maintenant ?

### Deux notions différentes

| Terme | Qu’est-ce | Quand |
|-------|-----------|--------|
| **Sprite sheet** (PNG grille) | Une texture, `Sprite Mode = Multiple`, slice en frames | **Maintenant** — c’est la livraison ChatGPT |
| **Unity Sprite Atlas** (asset `.spriteatlasv2`) | Pack runtime de **plusieurs** sprites/textures pour batcher les draw calls | **Plus tard** — optimisation |

Le sheet 1024×1024 découpé = déjà un « atlas art ». Le **Sprite Atlas** Unity est une étape **supplémentaire** qui regroupe plusieurs assets (abeille + papillon + sprites plantes + UI…) dans une ou des textures packées.

### Faut-il le créer maintenant ?

**Non.** Raisons :

1. Le code référence des `Sprite` / clips d’anim — ça marche **sans** Sprite Atlas.
2. L’art n’est pas encore livré ; créer un atlas vide / incomplet = maintenance inutile.
3. Un seul sheet abeille actif = gain de batch quasi nul (1 texture déjà).
4. On crée l’atlas quand le **pack insectes** (et idéalement les sprites farm co-visibles) est stable, ou si le Frame Debugger montre trop de breaks de batch.

### Quand le faire (todo `[P0-FARM-INSECT-003]`)

- Pack MVP livré (abeille + papillon au minimum), **ou**
- Profilage : trop de material / texture switches sur la scène ferme.

Jusque-là : import Multiple + compression Unity suffit.

---

## Pipeline de livraison (ordre)

| # | Qui | Livrable |
|---|-----|----------|
| 0 | Cursor **demain** | Scripts path + FSM + hook Flowering (**placeholders** OK) `[P0-FARM-INSECT-001]` |
| 1 | Art (ChatGPT + crop) | Sheet abeille **1020×132** / **6 frames Fly** — `Assets/Art/Sprites/Farm/Insects/Bee_Fly.png` |
| 2 | Unity import | Slice Multiple 170×132, pivots centre — **fait** ; clip Animator `Bee_Fly` reste `[P0-FARM-INSECT-002]` |
| 3 | Bezy | Shell overlay / nodes sur prefabs plantes |
| 4 | Auteur | Nodes laitue / tomate ; playtest |
| 5 | Plus tard | Sprite Atlas Unity si besoin ; pack multi-insectes ; frames Forage dédiées |

---

## Spec art — promesse ChatGPT (référence de code)

> Art **pas encore livré** (plus de crédit). On code contre cette grille.

### Livrable abeille (MVP) — **intégré 2026-07-25**

- Fichier : `Assets/Art/Sprites/Farm/Insects/Bee_Fly.png`
- Strip **1020 × 132**, fond transparent (RGBA), **1 rangée × 6 frames** (pad +3 px pour blocs DXT 4×4).
- Cellules : **170 × 132** (`Bee_Fly_01` … `Bee_Fly_06`), pivot Center, Mesh Type Full Rect.
- Android : Override **ETC2 RGBA8** (comme sprites laitue).
- Loop Fly ~12 FPS (ailes hautes → basses).
- Orientation : ¾ / profil vers la droite (flipX runtime).
- Source brute (backup) : `Assets/Art/Assets Store Dump/abeille.png` (1024×129, non rognée).

### Forage (MVP sans sheet dédié)

Tant que ChatGPT ne livre que le loop Fly :

- État `Forage` = **même clip Fly** à vitesse réduite **ou** bob scale sur place.
- Frames Forage dédiées = polish ultérieur (rangée B ou 2ᵉ sheet).

### Pack complet proposé (backlog art — pas bloquant code)

Même style, une grille 8 frames (ou plus) par espèce :

| Insecte | Usage ferme pressenti |
|---------|------------------------|
| Abeille | Flowering (MVP) |
| Papillon | Flowering variantes |
| Coccinelle | Sol / feuilles (ambiance) |
| Ver de terre | Après récolte / sol humide |
| Escargot | Lent, sol |
| Sauterelle | Rare / jump |
| Araignée | Coin sombre / night? |
| Moustique | Near water aquaponie |
| Mouche | Compost / mature? |

Code MVP : enum `insectKind` extensible, **seules Bee (+ Butterfly)** branchées au Flowering.

### Option premium 16 frames (plus tard)

Battement + micro-mouvement corps / pattes — **ne pas bloquer** le runtime ; remplacer les clips quand dispo.

### Checklist import Unity

- [x] Dossier : `Assets/Art/Sprites/Farm/Insects/`
- [x] Texture Type : Sprite (2D et UI) · Mode **Multiple**
- [x] Slice grille **170×132** (6 colonnes × 1 rangée)
- [x] Pivot : Center · noms `Bee_Fly_01` … `Bee_Fly_06`
- [ ] Animation Clip `Bee_Fly` (sample ~12 FPS) + Animator Controller
- [ ] **Pas** de Sprite Atlas tant que `[P0-FARM-INSECT-003]` non décidé

### Prompt de rappel (si regénération)

```text
Sprite sheet 1024x1024, transparent background, 4x2 grid, each cell 256x256.
Cute honeybee side view facing RIGHT, same art style as our farm plant sprites.
8-frame wing flap loop: high, mid, low, mid, high, mid, low, mid.
Centered in each cell, no text, no drop shadow, consistent outline.
```

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

1. [x] Scripts path + FSM + hook Flowering (`InsectPathAnchor` / `InsectPathFollower` / `PlantGrow`).  
2. [x] Art + Bezy P1–P3 (`Bee.prefab`, path sur `LaitueObj`).  
3. [ ] **Playtest** `[P0-FARM-INSECT-PLAY-001]` : Flowering → abeille active ; hors Flowering → path off.  
4. **Ne pas** créer de Sprite Atlas maintenant (`[P0-FARM-INSECT-003]`).
