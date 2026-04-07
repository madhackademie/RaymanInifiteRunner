# Spéc — progression : XP joueur + maturité biofiltre / système

Références futures : unlocks plantes (`PlantDefinition` / tags), `BuildManager` / placement, persistance temps réel (`Timer`, UTC), instances de zones (biofiltre, lit de culture).

---

## 1. XP joueur (à détailler)

- Prévoir un **système d’XP (ou progression)** côté **joueur** (niveaux, perks, déblocages globaux, etc.).
- Le contenu exact (sources d’XP, courbes, récompenses) reste **à concevoir** ; cette note pose surtout le **couplage** avec la maturité des systèmes de culture ci-dessous.

---

## 2. « XP » ou maturité du biofiltre / du système de culture

Objectif design : les **plantes à fruits** (ou cultures avancées) **débloquées plus tard** ne doivent **pas** être plantables n’importe où : seulement dans un **système** (ex. biofiltre, zone dédiée) qui a déjà **mûri** via des **cycles de culture complétés**.

### Règle cible (ordre de grandeur)

- **Seuil minimum** : le système doit avoir enregistré l’équivalent d’**au moins une année en temps réel** d’activité / de cycles (à lier au **temps réel** du jeu — hors pause ou selon règles produit à trancher).
- **Conversion indicative** pour calibrer : environ **10 cycles de salade** ≈ baseline de ce qu’il faut pour considérer le système « assez mature » pour accueillir des cultures fruits avancées.

> Les chiffres (10 salades, 1 an) sont des **repères de design** ; l’implémentation pourra utiliser un compteur de **cycles complétés** et/ou du **temps réel accumulé**, tant que la contrainte produit reste claire.

### Placement (gameplay)

- Au moment du **placement** (`CanPlace` ou équivalent) : pour une plante **gated** par maturité système, vérifier que **l’instance** de biofiltre / lit ciblé satisfait le seuil (cycles + éventuelle règle temps réel).
- Distinguer clairement : **unlock joueur** (« j’ai la graine / la recette ») vs **éligibilité du support** (« ce biofiltre a assez vécu »).

---

## 3. Pistes d’implémentation (plus tard)

- Données **par instance** (prefab zone) : compteur de cycles, timestamp cumulé ou dernier reset.
- **Sauvegarde** alignée sur le modèle offline / UTC du projet.
- **Données designers** : champs sur `PlantDefinition` ou table dédiée (ex. `minSystemCycles`, `minRealtimeSeconds`, tag `RequiresMatureSystem`).

---

## 4. Liens avec d’autres notes

- Pipeline plantation : `Notes/Farm/TODO_plantation_pipeline.md`
- Temps ferme / reprise : à croiser avec **Spec temps de ferme** (`Notes/Todo_project.md` → section GDD)
