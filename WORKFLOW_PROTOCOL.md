# le protocole d'ouverture et fermeture du fichier IA/Github

## --1--
Mise a jour github commande : cf @GIT_HELPER.md [git helper](GIT_HELPER.md)

## --2--
Faire lire a cursor le PROJECT_LOG [journal](PROJECT_LOG.md)
commande “Bootstrap session : lis @WORKFLOW_PROTOCOL.md, @ASSISTANT_CONTEXT.md, @PROJECT_LOG.md”

## --3--
Fin de session : tu me dis “Mets à jour le journal”.
J’ajoute une entrée datée avec :
objectifs du jour
changements effectués (fichiers/commits si tu en as)
décisions
problèmes/solutions testées
prochaines étapes (checklist)

## --4--
add github commande commit/push : cf @GIT_HELPER.md [git helper](GIT_HELPER.md)

## --5--
**Nouvelle feature** : créer une branche dédiée avant un gros bloc de code — voir **`GIT_HELPER.md`** section **--3--** (*Branche par feature + fusion dans main*). **Obligatoire** pour la prochaine feature **scènes / HUD / UI Inventaire–Market** (branche type `feature/scenes-navigation-ui`) : pas d’implémentation de ce système directement sur `main` ; un fork GitHub, si utilisé, suit la même logique (branche sur le fork, puis PR).