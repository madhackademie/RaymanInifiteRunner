Je vais essayer d'organiser et mettre ici le workflow et les commandes basiques de github (80/20)


--1--A CHAQUE NOUVELLE SESSION CONTROL DU COMMIT

Pour savoir s’il y a des changements à récupérer depuis GitHub, puis mettre à jour ton dossier, fais :

1) Vérifier s’il y a des commits à récupérer (sans rien modifier)

git status 
git fetch

Si tu vois “Your branch is behind …” → il y a des changements à récupérer.
Alternative directe (voir les commits manquants) :
git fetchgit log --oneline HEAD..origin/main

2) Mettre à jour ton dossier avec ces changements
git pull
Si tu as des modifications locales (ça peut bloquer)
Vérifie :
git status
Si tu as des fichiers modifiés, il faudra commit ou stash avant de pull (sinon conflits/erreurs possibles).

Protocole “fin de session” pour push proprement (PowerShell)
Dans le dossier du projet :

Vérifier l’état local
git status
Mettre à jour depuis GitHub avant de pousser (évite les surprises)
git fetch
git status
Si “behind” → fais :
git pull
Préparer le commit
Voir ce qui a changé :
git diff
Ajouter les fichiers voulus :
git add -A
(ou git add <fichier> si tu veux être sélectif)

Créer le commit
git commit -m "Ton message de commit"
Pousser vers GitHub
git push
Vérification
git status
Si git pull refuse à cause de modifs locales
Option simple :

git stash
git pull
git stash pop
Puis tu résous les conflits si besoin, et tu continues (add/commit/push).