# git helper 

Je vais essayer d'organiser et mettre ici le workflow et les commandes basiques de github (80/20)

## --0-- Checklist rapide (branche + synchro)

Utilise cette séquence au début de chaque session ou après un switch de branche :

```bash
git branch --show-current
git fetch --all
git status
git branch -vv
```

Interprétation rapide :
- `git branch --show-current` : confirme la branche active.
- `git status` : te dit si tu es en avance / en retard / à jour.
- `git branch -vv` : vérifie le tracking (`origin/...`) de la branche courante.

Si la branche suit bien le remote (`[origin/nom-branche]`) :

```bash
git pull
```

Si `git pull` répond "There is no tracking information for the current branch" :

```bash
git branch --set-upstream-to=origin/nom-branche
git pull
```

Exemple concret :

```bash
git branch --set-upstream-to=origin/feature/scene-inventaire
git pull
```


## --1--A CHAQUE NOUVELLE SESSION CONTROL DU COMMIT

Pour savoir s’il y a des changements à récupérer depuis GitHub, puis mettre à jour ton dossier, fais :

1) Vérifier s’il y a des commits à récupérer (sans rien modifier)

```bash
git status
git fetch
```

Si tu vois “Your branch is behind …” → il y a des changements à récupérer.
Alternative directe (voir les commits manquants) :
```bash
git fetch
git log --oneline HEAD..origin/main
```
2) Mettre à jour ton dossier avec ces changements
```bash
git pull
```
Si tu as des modifications locales (ça peut bloquer)
Vérifie :
```bash
git status
```
Si tu as des fichiers modifiés, il faudra commit ou stash avant de pull (sinon conflits/erreurs possibles).

## --2--Protocole “fin de session” pour push proprement (PowerShell)
Dans le dossier du projet :

Vérifier l’état local
```bash
git status
```
Mettre à jour depuis GitHub avant de pousser (évite les surprises)
```bash
git fetch
git status
```
Si “behind” → fais :
```bash
git pull
```
Préparer le commit
Voir ce qui a changé :
```bash
git diff
```
Ajouter les fichiers voulus :
```bash
git add -A
git add .
git add nomfichier.extension
```

Créer le commit
```bash
git commit -m "Ton message de commit"
```
Pousser vers GitHub
```bash
git push
```
Vérification
```bash
git status
```
Si git pull refuse à cause de modifs locales
Option simple :
```bash
git stash
git pull
git stash pop
```
Puis tu résous les conflits si besoin, et tu continues (add/commit/push).

## --3-- Branche par feature + fusion dans `main` (recommandé)

Objectif : **une branche Git par fonctionnalité** (ex. récolte, inventaire, UI menu). Tant que la feature n’est pas validée en jeu / tests, les commits restent sur la branche ; une fois OK, tu **fusionnes** dans `main` (idéalement après revue rapide).

### Impératif — feature « scènes / navigation / UI multi-stage » (projet RaymanInfiniteRunner)

Pour le chantier **système de scènes** (Inventaire, Market, HUD sur tous les stages, Additive vs panneaux, etc.) : **ne pas développer sur `main`**. Créer **obligatoirement** une branche dédiée **avant** le premier commit de ce bloc (ex. `feature/scenes-navigation-ui` — renommer si le design retient surtout des **panneaux UI** plutôt que des `.unity` séparés).

- **Branche locale** : `git checkout -b feature/…` depuis `main` à jour (voir ci-dessous) — c’est l’équivalent pratique d’un « fork de travail » sur le même dépôt.
- **Fork GitHub** (si ton équipe travaille ainsi) : même règle — la feature vit sur une **branche** de ton fork, pas directement sur `main` du fork, jusqu’à merge / PR vers le dépôt principal.

### Avant de coder une nouvelle feature

1. Sauvegarder tout dans l’éditeur (**Save All**).
2. Depuis la racine du repo :
```bash
git status
git fetch
git checkout main
git pull
```
3. Créer une branche décrite clairement (préfixe `feature/` conseillé) :
```bash
git checkout -b feature/nom-court-explicite
```
Exemples : `feature/harvest-inventory`, `feature/scenes-navigation-ui`, `feature/mature-seeds-harvest`, `fix/grid-occupancy`.

### Pendant le développement

- Commits **petits et fréquents** avec un message clair (`feat: …`, `fix: …`, `chore: …` si tu adoptes ce style).
- Première poussée de la branche vers GitHub :
```bash
git push -u origin feature/nom-court-explicite
```
- Poursuite : `git add …`, `git commit -m "…"`, `git push`.

### Quand la feature est terminée et testée

**Option A — Pull Request (recommandé si tu travailles seul avec historique lisible)**  
Sur GitHub : ouvrir une PR `feature/…` → `main`, vérifier le diff, merger (bouton *Merge*). En local ensuite :
```bash
git checkout main
git pull
```
Tu peux supprimer la branche distante depuis l’interface GitHub ; en local : `git branch -d feature/nom-court-explicite`.

**Option B — Fusion en local puis push**
```bash
git checkout main
git pull
git merge feature/nom-court-explicite
```
En cas de conflits : résoudre les fichiers marqués, `git add`, `git commit` (Git complète le message de merge). Puis :
```bash
git push
git branch -d feature/nom-court-explicite
```

### Rappels

- Si `main` a avancé **pendant** que tu es sur ta branche, mets à jour ta base avant de merger :
```bash
git checkout feature/nom-court-explicite
git fetch
git merge main
```
(résoudre les conflits si besoin, tester, puis `git push`).

- Retard pris (comme pour récolte / inventaire sans branche dédiée) : à partir de maintenant, enchaîner les **prochaines grosses features** sur une branche `feature/…` ; les changements déjà sur `main` restent l’historique actuel.

### Liens

- Début / fin de session : sections **--1--** et **--2--** ci-dessus ; ouverture de session : `WORKFLOW_PROTOCOL.md`.