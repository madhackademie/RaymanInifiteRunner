# le protocole d'ouverture et fermeture du fichier IA/Github

## --1--
Mise a jour github commande : cf @GIT_HELPER.md [git helper](GIT_HELPER.md)

## --2--
Faire lire a cursor le PROJECT_LOG [journal](PROJECT_LOG.md)
commande “Bootstrap session : lis @WORKFLOW_PROTOCOL.md, @ASSISTANT_CONTEXT.md, @PROJECT_LOG.md, @Notes/Todo_project.md”

Pour toute demande de gestion de projet (tache du jour, priorite, prochaine session):
- lire d'abord les 4 fichiers ci-dessus,
- verifier la branche Git courante (`git branch --show-current` ou `git status -sb`),
- **rappeler en tete de reponse** si l'on n'est pas sur `main` : nom de la branche feature/rework + lien avec la priorite (`Notes/Todo_project.md` bloc *Contexte Git*),
- repondre uniquement avec la priorite la plus recente issue des docs,
- ne jamais inventer une tache generique.

Source de statut (anti-doublon):
- `Notes/Todo_project.md` est la source unique des statuts `[ ]/[~]/[x]`.
- Les autres notes servent de detail d'implementation et doivent pointer vers ce fichier.
- Guide utilisateur de suivi: `Notes/GUIDE_suivi_projet.md`.

## --3--
Fin de session : tu me dis “Mets à jour le journal”.
J’ajoute une entrée datée avec :
objectifs du jour
changements effectués (fichiers/commits si tu en as)
décisions
problèmes/solutions testées
prochaines étapes (checklist)

## --4--
Commit / push : **responsabilite auteur du projet** (pas l'assistant sauf demande explicite).

- L'assistant prepare les changements (code + docs) et peut suggerer message de commit + liste de fichiers.
- **Seul l'auteur** execute `git add`, `git commit`, `git push` (delais, autorisations, hooks).
- Demander explicitement a l'assistant : « commit », « push », « merge », etc. si tu veux qu'il le fasse.

Commandes : cf @GIT_HELPER.md [git helper](GIT_HELPER.md). Regle Cursor : `.cursor/rules/git_commits_user_only.mdc`.

## --5--
**Nouvelle feature** : créer une branche dédiée avant un gros bloc de code — voir **`GIT_HELPER.md`** section **--3--** (*Branche par feature + fusion dans main*). Même logique avec un **fork** GitHub si besoin (branche sur le fork, puis PR).